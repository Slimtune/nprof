 /***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003, 2004, 2005, 2006, 2007, 2008, 2009 by Matthew Mastracci, Christian Staudenmeyer
    email                : staudenmeyer@gmail.com
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;
using System.Globalization;
using System.Reflection;

namespace NProf
{
    using FunctionID = System.UInt64;
    public class NamespaceView : View
    {
		private void Filter(MethodView view)
		{
			view.BeginUpdate();
			view.Update(run, view.functions);
			List<TreeNode> remove = new List<TreeNode>();
			foreach (TreeNode node in view.Nodes)
			{
				TreeNodeCollection nodes = Nodes;
				foreach (string s in ((Function)node.Tag).Signature.Namespace.Split('.'))
				{
					if (nodes[s] != null && nodes[s].Checked)
					{
						nodes = nodes[s].Nodes;
					}
					else
					{
						nodes = null;
						break;
					}
				}
				if (nodes == null && ((Function)node.Tag).Signature.Namespace != "")
				{
					remove.Add(node);
				}
			}
			foreach (TreeNode node in remove)
			{
				node.Remove();
			}
			view.EndUpdate();
		}
		//private void Filter(MethodView view)
		//{
		//    view.BeginUpdate();
		//    view.Update(run, functions);
		//    List<TreeNode> remove = new List<TreeNode>();
		//    foreach (TreeNode node in view.Nodes)
		//    {
		//        TreeNodeCollection nodes = Nodes;
		//        foreach (string s in ((Function)node.Tag).Signature.Namespace.Split('.'))
		//        {
		//            if (nodes[s] != null && nodes[s].Checked)
		//            {
		//                nodes = nodes[s].Nodes;
		//            }
		//            else
		//            {
		//                nodes = null;
		//                break;
		//            }
		//        }
		//        if (nodes == null && ((Function)node.Tag).Signature.Namespace!="")
		//        {
		//            remove.Add(node);
		//        }
		//    }
		//    foreach (TreeNode node in remove)
		//    {
		//        node.Remove();
		//    }
		//    view.EndUpdate();
		//}
        public NamespaceView(RunView run)
        {
            this.CheckBoxes = true;
            this.AfterCheck += delegate(object sender, TreeViewEventArgs e)
            {
                if (!updating)
                {
                    Filter(run.callees);
                    Filter(run.callers);
                }
            };
        }
        private Run run;
        private FunctionMap functions;
        private bool updating = false;
        public void Update(Run run, FunctionMap functions)
        {
            this.Nodes.Clear();
            updating = true;
            this.run = run;
            this.functions = functions;
            BeginUpdate();
			List<Function> functionList = new List<Function>(functions.Values);
			functionList.Sort(new Comparison<Function>(delegate(Function a, Function b)
			{
				return a.Signature.Namespace.CompareTo(b.Signature.Namespace);
			}));
            foreach (Function function in functionList)
            {
				if (function.Signature.Namespace == "")
				{
					continue;
				}
                TreeNodeCollection items = this.Nodes;
                foreach (string name in function.Signature.Namespace.Split('.'))
                {
                    bool found = false;
                    foreach (TreeNode item in items)
                    {
                        if (item.Text == name)
                        {
                            items = item.Nodes;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        TreeNode item = items.Add(name, name);
                        item.Checked = true;
                        items = item.Nodes;
                    }
                }
            }
            EndUpdate();
            updating = false;
        }
    }
    public class SearchPanel : FlowLayoutPanel
    {
        private MethodView methodView;
        public void Find(bool forward, bool step)
        {
            if (methodView.SelectedNode != null)
            {
                methodView.Find(findText.Text, forward, step);
            }
            else
            {
                methodView.Find(findText.Text, forward, step);
            }
        }
        public SearchPanel(MethodView methodView)
        {
            Dock = DockStyle.Bottom;
            this.methodView = methodView;
            findText = new TextBox();
            findText.TextChanged += delegate
            {
                Find(true, false);
            };
            findText.KeyDown += delegate(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Find(true, true);
                    e.Handled = true;
                }
            };

            BorderStyle = BorderStyle.FixedSingle;
            WrapContents = false;
            AutoSize = true;
            Button findNext = new Button();
			Button close = new Button();
			close.Image=new Bitmap(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.close.gif"));

            findNext.AutoSize = true;
            findNext.Text = "Next";
            findNext.Click += delegate(object sender, EventArgs e)
            {
                Find(true, true);
            };


            Label findLabel = new Label();
            findLabel.Text = "Find:";
            findLabel.Dock = DockStyle.Fill;
            findLabel.TextAlign = ContentAlignment.MiddleLeft;
            findLabel.AutoSize = true;

            Button findPrevious = new Button();
            findPrevious.AutoSize = true;
            findPrevious.FlatAppearance.BorderSize = 0;
            findPrevious.Click += delegate(object sender, EventArgs e)
            {
                Find(false, true);
            };
            findPrevious.Text = "Previous";

            Controls.AddRange(new Control[] { findLabel, findText, findNext, findPrevious });
        }
        private TextBox findText;

    }
	public enum SortColumn
	{
		Inclusive,
		Exclusive,
		InclusiveChange,
		ExclusiveChange
	}
    public class RunView : TabPage
    {
		private ComboBox comparison = new ComboBox();
		private ComboBox order = new ComboBox();
        public static Label Caption(string text)
        {
            Label caption = new Label();
            caption.Text = text;
            caption.Dock = DockStyle.Top;
            caption.TextAlign = ContentAlignment.MiddleLeft;
            return caption;
        }
        public static int count = 0;
        public RunView(Run run)
        {
			this.KeyDown += delegate(object sender, KeyEventArgs e)
			{
				if (e.KeyCode == (Keys.F4 | Keys.Alt))
				{
					((TabControl)this.Parent).TabPages.Remove(this);

				}
			};
			order.Items.Add("Inclusive Time");
			order.Items.Add("Inclusive Time Change");
			order.Items.Add("Exclusive Time");
			order.Items.Add("Exclusive Time Change");
			order.SelectedIndex = 1;
			order.SelectedIndexChanged += new EventHandler(order_SelectedIndexChanged);

			callees = new MethodView("Callees",this);
			callers = new MethodView("Callers",this);

			this.run = run;
            count++;
            this.Text = Path.GetFileNameWithoutExtension(run.Executable) + " #" + count+"    ";
            namespaces = new NamespaceView(this);
            namespaces.Height = 100;
            namespaces.Dock = DockStyle.Fill;

			ContextMenu callsMenu = new ContextMenu(new MenuItem[] {
				new MenuItem("Show callers",delegate(object sender,EventArgs e){
					CallersNext();
					})
			});
			ContextMenu callersMenu = new ContextMenu(new MenuItem[] {
				new MenuItem("Show calls",delegate(object sender,EventArgs e){
					CalleesNext();
					})
			});
			callees.MouseClick+= delegate(object sender, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Right)
				{
					callsMenu.Show(callees,new Point(e.X,e.Y));
				}
			};
			callers.MouseClick += delegate(object sender, MouseEventArgs e)
			{
				if (e.Button == MouseButtons.Right)
				{
					callersMenu.Show(callers, new Point(e.X, e.Y));
				}
			};
			//callees.DoubleClick += delegate
			//{
			//    CalleesNext();
			//};
			//callees.MouseClick



            ContextMenu callerMenu = new ContextMenu(
                new MenuItem[] {
					new MenuItem(
						"go to method",
						delegate {
							GoToCaller(callers);
						},
						Shortcut.CtrlN
					),
					new MenuItem(
						"go to callee",
						delegate {
							GoToCallee(callers);
						},
						Shortcut.CtrlN
					)
				}
            );
            ContextMenu calleesMenu = new ContextMenu(
                new MenuItem[] {
					new MenuItem(
						"go to method",
						delegate
						{
							GoToCallee(callers);
						},
						Shortcut.CtrlE
					),
					new MenuItem(
						"go to callee",
						delegate 
						{
							GoToCaller(callers);
						},
						Shortcut.CtrlN
					)
				}
            );
            callees.Size = new Size(100, 100);
            callers.Size = new Size(100, 100);
            callees.Dock = DockStyle.Fill;
            callers.Dock = DockStyle.Fill;
            SplitContainer methodPanel = new SplitContainer();
            methodPanel.Orientation = Orientation.Horizontal;


			//const string columnCaptions = "        Inclusive   Method signature";

			

			methodPanel.Panel2.Controls.Add(callers);
			//methodPanel.Panel2.Controls.Add(Caption(columnCaptions));
			methodPanel.Panel2.Controls.Add(Caption("Callers"));
			methodPanel.Panel1.Controls.Add(callees);
			//methodPanel.Panel1.Controls.Add(Caption(columnCaptions));
			methodPanel.Panel1.Controls.Add(Caption("Calls"));
			SearchPanel searchPanel = new SearchPanel(callers);
			methodPanel.Panel1.Controls.Add(searchPanel);


			comparison.DropDownStyle = ComboBoxStyle.DropDownList;
			order.DropDownStyle = ComboBoxStyle.DropDownList;

			//comparison.Items.Add("hi!!");
			//comparison.SelectionMode = SelectionMode.One;
			comparison.Dock = DockStyle.Top;
			order.Dock = DockStyle.Top;

			foreach (RunView r in NProf.tabs.TabPages)
			{
				if (r != this)
				{
					comparison.Items.Add(r);
				}
			}
			comparison.DisplayMember = "Text";
			//comparison.DataSource = NProf.tabs.TabPages;

			comparison.Width = 100;

			//comparison.Height = 20;


			FlowLayoutPanel p = new FlowLayoutPanel();
			p.Height = 50;
			p.Dock = DockStyle.Top;
			methodPanel.Panel1.Controls.Add(p);
			p.Controls.Add(Caption("Order by:"));
			p.Controls.Add(order);
			p.Controls.Add(Caption("Compare to:"));
			p.Controls.Add(comparison);


			
            methodPanel.Dock = DockStyle.Fill;
            methodPanel.Dock = DockStyle.Fill;
            Panel panel = new Panel();
            Panel rightPanel = new Panel();

            Splitter mainSplitter = new Splitter();
            mainSplitter.Dock = DockStyle.Left;

            Panel leftPanel = new Panel();
            leftPanel.Width = 200;
            leftPanel.Dock = DockStyle.Left;

            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Controls.Add(methodPanel);


            leftPanel.Controls.Add(namespaces);
            leftPanel.Controls.Add(Caption("Namespaces"));

            panel.Dock = DockStyle.Fill;


            panel.Controls.AddRange(new Control[] { rightPanel, mainSplitter, leftPanel });
            panel.Padding = new Padding(5);
            namespaces.Update(run, run.callers);
			if (comparison.Items.Count != 0)
			{
				comparison.SelectedIndexChanged += new EventHandler(comparison_SelectedIndexChanged);
				comparison.SelectedIndex = comparison.Items.Count - 1;

			}
            callees.Update(run, run.functions);
            callers.Update(run, run.callers);
            this.Controls.Add(panel);
        }

		public SortColumn sort = SortColumn.Exclusive;

		void order_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch ((string)order.Items[order.SelectedIndex])
			{
				case "Inclusive Time":
					sort = SortColumn.Inclusive;
					break;
				case "Exclusive Time":
					sort = SortColumn.Exclusive;
					break;
				case "Inclusive Time Change":
					sort = SortColumn.InclusiveChange;
					break;
				case "Exclusive Time Change":
					sort = SortColumn.InclusiveChange;
					break;
			}
			callees.Nodes.Clear();
			callers.Nodes.Clear();
			callees.Update(run, run.functions);
			callers.Update(run, run.callers);
		}

		public Run run;

		void comparison_SelectedIndexChanged(object sender, EventArgs e)
		{
			//comparisonRun = (Run)comparison.SelectedValue;
			comparisonRun = ((RunView)comparison.Items[comparison.SelectedIndex]).run;
			callees.Nodes.Clear();
			callers.Nodes.Clear();
			callees.Update(run, run.functions);
			callers.Update(run, run.callers);
		}
		public Run comparisonRun;
        public void GoToCaller(MethodView source)
        {
            MoveTo(source, callers);
        }
        public void GoToCallee(MethodView source)
        {
            MoveTo(source, callees);
        }
        NamespaceView namespaces;
		public MethodView callees;// = new MethodView("Callees", this);
		public MethodView callers;// = new MethodView("Callers", this);

        public void MoveTo(MethodView source, MethodView target)
        {
            if (source.SelectedNode != null)
            {
                target.MoveTo(((Function)source.SelectedNode.Tag).ID);
            }
        }
        private void CallersNext()
        {
            if (callers.SelectedNode != null)
            {
                TreeNode item = callers.SelectedNode;

                FunctionID id = ((Function)item.Tag).ID;
                if (item.Parent.Parent == null)
                {
                    callees.MoveTo(id);
                }
                else
                {
                    callers.MoveTo(id);
                }
                item.Collapse();

            }
        }
        private void CalleesNext()
        {
            List<string> list = new List<string>();


            if (callees.SelectedNode != null)
            {
                TreeNode item = callees.SelectedNode;
				//if (item.Parent.Parent == null)
				//{
				//    callers.MoveTo(((Function)item.Tag).ID);
				//}
				//else
				//{
                    callees.MoveTo(((Function)item.Tag).ID);
				//}
				//item.Collapse();
            }
        }

    }
    public class NProf : Form
    {
        public static Font font = new Font("Courier New", 9.0f);

        private Profiler profiler;
        public static TextBox application;
        public static TextBox arguments;
        public static TextBox directory;
        public static Button start;
        public string Title
        {
            get
            {
                return "NProf " + Profiler.Version;
            }
        }
		FlowLayoutPanel mainPanel = new FlowLayoutPanel();

        Label help = MakeLabel("Select the application to profile and click 'Start'.");
        private NProf()
        {
            WindowState = FormWindowState.Maximized;
            Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
            Text = Title;
            profiler = new Profiler();

            Menu = new MainMenu(new MenuItem[] {
				new MenuItem(
					"File",
					new MenuItem[] {
						new MenuItem("E&xit",delegate {Close();})
					})
            });

            Splitter methodSplitter = new Splitter();
            methodSplitter.Dock = DockStyle.Bottom;

            Splitter leftSplitter = new Splitter();
            leftSplitter.Dock = DockStyle.Bottom;


			mainPanel.AutoSize = true;
            application = new TextBox();
			application.Width = 200;
			application.Dock = DockStyle.Fill;
            arguments = new TextBox();
			arguments.Width = 200;
            directory = new TextBox();
            start = new Button();
            start.Text = "";
            start.Click += delegate
            {
                StartRun();
            };
            start.Text = "Start";
            start.Anchor = AnchorStyles.Right;
            start.Height = application.Height;
            start.BackgroundImageLayout = ImageLayout.Zoom;
			directory.Width = 200;
            start.Width = start.PreferredSize.Width;
            mainPanel.Height = 100;
            mainPanel.AutoSize = true;
            mainPanel.Dock = DockStyle.Top;
            start.TextImageRelation = TextImageRelation.TextAboveImage;


            Button browse = new Button();
            browse.Anchor = AnchorStyles.Top;
            browse.Text = "Browse...";
            browse.Focus();
            browse.Height = application.Height;
            browse.TextAlign = ContentAlignment.MiddleCenter;
            browse.Click += delegate
            {
                OpenFileDialog dialog = new OpenFileDialog();
				dialog.FileName = application.Text;
                dialog.Filter = "Executable files (*.exe)|*.exe";
                DialogResult dr = dialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    application.Text = dialog.FileName;
                    application.Focus();
                    application.SelectAll();
                }
            };
            Button directoryBrowse = new Button();

            directoryBrowse.Text = "Browse...";
            directoryBrowse.Width = 20;
            directoryBrowse.Width = directoryBrowse.PreferredSize.Width;
            directoryBrowse.Height = directory.Height;

            directoryBrowse.Click += delegate
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
				dialog.SelectedPath = directory.Text;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    directory.Text = dialog.SelectedPath;
                }
            };
			mainPanel.Controls.Add(MakeLabel("Application:"));
			mainPanel.Controls.Add(application);
			mainPanel.Controls.Add(browse);
			mainPanel.Controls.Add(MakeLabel("Arguments:"));
			mainPanel.Controls.Add(arguments);
			mainPanel.Controls.Add(MakeLabel("Working directory:"));
			mainPanel.Controls.Add(directory);
			mainPanel.Controls.Add(directoryBrowse);
			mainPanel.Controls.Add(start);

            mainPanel.Padding = new Padding(3);
            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;
            help.Dock = DockStyle.Fill;
            help.TextAlign = ContentAlignment.MiddleCenter;
            help.AutoSize = false;
            Controls.AddRange(new Control[] {help, mainPanel });

            application.TextChanged += delegate
            {
                string fileName = Path.GetFileName(application.Text);
                Text = fileName + " - " + Title;
            };
			InitializeComponent();
			
			tabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			tabs.DrawItem += TabControl1_DrawItem;
			tabs.MouseClick += TabControl1_MouseClick;
			tabs.KeyDown += delegate(object sender, KeyEventArgs e)
			{
				if (e.KeyData == (Keys.F4 | Keys.Control))
				{
					tabs.TabPages.Remove(tabs.SelectedTab);

				}
			};

        }

		private Point _imageLocation = new Point(15, 5);   
		private Point _imgHitArea = new Point(13, 2);        
		private void TabControl1_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			try            
			{               
				//Close Image to draw                
				Image img = new Bitmap(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.close.gif"));
				Rectangle r = e.Bounds;               
				r = tabs.GetTabRect(e.Index);
				r.Offset(2, 2);
				Brush TitleBrush = new SolidBrush(Color.Black);
				Font f = this.Font;
				string title = tabs.TabPages[e.Index].Text;               
				e.Graphics.DrawString(title, f, TitleBrush, new PointF(r.X, r.Y));
				e.Graphics.DrawImage(img, new Point(r.X + (tabs.GetTabRect(e.Index).Width - _imageLocation.X), _imageLocation.Y));
			}
			catch (Exception) 
			{ 
			}
		}
		private void TabControl1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e) 
		{     
			TabControl tc = (TabControl)sender;     
			Point p = e.Location;     
			int _tabWidth = 0;     
			_tabWidth = tabs.GetTabRect(tc.SelectedIndex).Width - (_imgHitArea.X);     
			Rectangle r = tabs.GetTabRect(tc.SelectedIndex);     
			r.Offset(_tabWidth, _imgHitArea.Y);     
			r.Width = 16;     
			r.Height = 16;     
			if (r.Contains(p)) 
			{        
				TabPage TabP = (TabPage)tc.TabPages[tc.SelectedIndex];        
				tc.TabPages.Remove(TabP);    
			} 
		}
        private static Label MakeLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.AutoSize = true;
            return label;
        }
        public static TabControl tabs;
        private void StartRun()
        {
            Run run = new Run(profiler, application.Text);
            run.profiler.completed = new EventHandler(run.Complete);
            run.Start();
        }
		//public Button close = new Button();
        public void ShowRun(Run run)
        {
            RunView runView=new RunView(run);
            if (!Controls.Contains(tabs))
            {
                Controls.Remove(mainPanel);
                Controls.Remove(help);
                Controls.Add(tabs);
                Controls.Add(mainPanel);
            }
            tabs.Controls.Add(runView);
            tabs.SelectedTab = runView;
        }
        [STAThread]
        static void Main(string[] args)
        {
            string s = Guid.NewGuid().ToString().ToUpper();
            Application.EnableVisualStyles();
            Application.Run(form);
        }
        public static NProf form = new NProf();

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NProf));
            this.SuspendLayout();
            // 
            // NProf
            // 
            this.ClientSize = new System.Drawing.Size(292, 273);
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NProf";
            this.ResumeLayout(false);

        }
    }
    public class StackWalk
    {
        public StackWalk(int id, int index, List<FunctionID> frames)
        {
            this.id = id;
            this.frames = frames;
            this.length = index;
        }
        public readonly int length;
        public readonly int id;
        public readonly List<FunctionID> frames;
    }
    public class FunctionInfo
    {
        public readonly string Namespace;
        public readonly string Signature;
        public FunctionInfo(string signature, string nameSpace)
        {
            this.Signature = signature;
            this.Namespace = nameSpace;
        }
    }
    public class Function
    {
        public Run run;
		public readonly bool reverse;
        public FunctionInfo Signature
        {
            get
            {
                return run.signatures[ID];
            }
        }
        public Function(FunctionID ID, Run run,bool reverse)
        {
            this.run = run;
            this.ID = ID;
			this.reverse = reverse;
        }
        public readonly FunctionID ID;


		//public int ExclusiveSamples = 0;
		public int ExclusiveSamples
		{
			get
			{
				int samples=0;
				foreach(StackWalk stackWalk in stackWalks)
				{
					if (!reverse)
					{
						//if (stackWalk.length + 1 == stackWalk.frames.Count)
						//{
						//    samples++;
						//}
						if (stackWalk.length == 0)
						{
							samples++;
						}
					}
					else
					{
						if (stackWalk.length +1 == stackWalk.frames.Count)
						{
							samples++;
						}
					}

					//if(stackWalk.frames[stackWalk.length-1]==this.ID)
					//{
					//    samples++;
					//}
				}
				return samples;
			}
		}


        public int Samples;
        public int lastWalk;
		public double InclusiveChange;
		public double ExclusiveChange;
        private FunctionMap children;
        public List<StackWalk> stackWalks = new List<StackWalk>();
        public FunctionMap Children
        {
            get
            {
                if (children == null)
                {
                    children = new FunctionMap();
                    foreach (StackWalk walk in stackWalks)
                    {
                        if (walk.length != 0)
                        {
                            Function callee = Run.GetFunctionInfo(children, walk.frames[walk.length - 1], run,this.reverse);
                            if (callee.lastWalk != walk.id)
                            {
                                callee.Samples++;
                            }
                            callee.stackWalks.Add(new StackWalk(walk.id, walk.length - 1, walk.frames));
                        }
                    }
                }
                return children;
            }
        }
    }
    public class Run
    {
        private string executable;
        public override string ToString()
        {
            return executable +"s     " + Duration.TotalSeconds.ToString("0.00");
        }
        public static Function GetFunctionInfo(FunctionMap functions, FunctionID id, Run run,bool reverse)
        {
            Function result;
            if (!functions.TryGetValue(id, out result))
            {
                result = new Function(id, run,reverse);
                functions[id] = result;
            }
            return result;
        }
        public int maxSamples;
        public List<List<FunctionID>> stackWalks = new List<List<FunctionID>>();
        private void InterpreteData()
        {
            Interprete(functions, false, this);
            Interprete(callers, true, this);
            List<Function> startFunctions = new List<Function>();
            maxSamples = 0;
            foreach (List<FunctionID> stackWalk in stackWalks)
            {
                if (stackWalk.Count != 0)
                {
                    maxSamples++;
                }
            }
        }
        private void Interprete(FunctionMap map, bool reverse, Run run)
        {
            int currentWalk = 0;
            foreach (List<FunctionID> original in stackWalks)
            {
                currentWalk++;
                List<FunctionID> stackWalk;
                if (reverse)
                {
                    stackWalk = new List<FunctionID>(original);
                    stackWalk.Reverse();
                }
                else
                {
                    stackWalk = original;
                }
                for (int i = 0; i < stackWalk.Count; i++)
                {
                    Function function = Run.GetFunctionInfo(map, stackWalk[stackWalk.Count - i - 1], run,reverse);
                    if (function.lastWalk != currentWalk)
                    {
                        function.Samples++;
                        function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.Count - i - 1, stackWalk));
                    }
                    function.lastWalk = currentWalk;
                }
            }
			//foreach (List<FunctionID> stackWalk in stackWalks)
			//{
			//    Function f = Run.GetFunctionInfo(map, stackWalk[0], run,reverse);
			//    f.ExclusiveSamples++;
			//}
        }
        private string ReadString(BinaryReader br)
        {
            FunctionID length = br.ReadUInt64();
            byte[] abString = new byte[length];
            br.Read(abString, 0, (int)length);
            return System.Text.ASCIIEncoding.ASCII.GetString(abString, 0, (int)length);
        }
        private string FileName
        {
            get
            {
                // TODO: prefix with nprof, to make it easier to find
                return Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), Profiler.ProfilerGuid + ".nprof");
            }
        }
        public TimeSpan Duration
        {
            get
            {
                return this.end - this.start;
            }
        }
        private void ReadStackWalks()
        {
            signatures[0] = new FunctionInfo(Path.GetFileName(this.executable), "");
            using (BinaryReader r = new BinaryReader(File.Open(FileName, FileMode.Open)))
            {
                this.end = DateTime.Now;
                while (true)
                {
					//int functionId = r.ReadInt32();
                    FunctionID functionId = r.ReadUInt64();
                    if (functionId == FunctionID.MaxValue)
                    {
                        break;
                    }
                    signatures[functionId] = new FunctionInfo(
                        ReadString(r),
                        ReadString(r)
                    );
                }
                while (true)
                {
                    List<FunctionID> stackWalk = new List<FunctionID>();
                    while (true)
                    {
                        FunctionID functionId = r.ReadUInt64();
						//int functionId = r.ReadInt32();
						if (functionId == FunctionID.MaxValue)
                        {
                            break;
                        }
                        else if (functionId == FunctionID.MaxValue-1)
                        {
                            return;
                        }
                        stackWalk.Add(functionId);
                    }
                    stackWalk.Add(0);
                    stackWalks.Add(stackWalk);
                }
            }
        }
        public void Complete(object sender, EventArgs e)
        {
            if (File.Exists(FileName))
            {
				ReadStackWalks();
                File.Delete(FileName);
            }
            NProf.form.BeginInvoke(new EventHandler(delegate
            {
                InterpreteData();
                NProf.form.ShowRun(this);
            }));
        }
        public Dictionary<FunctionID, FunctionInfo> signatures = new Dictionary<FunctionID, FunctionInfo>();
        public FunctionMap functions = new FunctionMap();
        public FunctionMap callers = new FunctionMap();
        private DateTime start = DateTime.MinValue;
        private DateTime end = DateTime.MinValue;
        public Run(Profiler p,string executable)
        {
            this.profiler = p;
            this.executable = executable;
        }
        public string Executable
        {
            get
            {
                return this.executable;
            }
        }
        public bool Start()
        {
            start = DateTime.Now;
            return profiler.Start(this);
        }
        public Profiler profiler;
    }
    public class FunctionMap : Dictionary<FunctionID, Function>
    {
    }
	//public class Sorter : System.Collections.IComparer
	//{
	//    public int Compare(object x, object y)
	//    {
	//        if (((TreeNode)x).Tag == null)
	//        {
	//            return -1;
	//        }
	//        if (((TreeNode)y).Tag == null)
	//        {
	//            return 1;
	//        }
	//        return ((Function)((TreeNode)y).Tag).ExclusiveSamples.CompareTo(((Function)((TreeNode)x).Tag).ExclusiveSamples);
	//    }
	//}
    public class View : TreeView
    {
        public View()
        {
            this.Font = NProf.font;
            this.ShowPlusMinus = true;
        }
    }
    public class MethodView : View
    {
        public void MoveTo(FunctionID id)
        {
            foreach (TreeNode item in Nodes)
            {
                if (((Function)item.Tag).ID == id)
                {
                    this.SelectedNode = item;
                    Invalidate();
                    this.Focus();
                    break;
                }
            }
        }
		public FunctionMap functions;
        public void Update(Run run, FunctionMap functions)
        {
			this.functions = functions;
            currentRun = run;
            SuspendLayout();
            Invalidate();
            BeginUpdate();
            foreach(Function method in SortFunctions(functions.Values))
            {
                AddItem(Nodes, method);
            }
            foreach(TreeNode item in Nodes)
            {
                EnsureComputed(item);
            }
            EndUpdate();
            ResumeLayout();
        }
        public void Find(string text, bool forward, bool step)
        {
            if (text != "")
            {
                TreeNode item;
                if (SelectedNode == null)
                {
                    if (Nodes.Count == 0)
                    {
                        item = null;
                    }
                    else
                    {
                        item = Nodes[0];
                    }
                }
                else
                {
                    if (step)
                    {
                        if (forward)
                        {
                            item = SelectedNode.NextVisibleNode;
                        }
                        else
                        {
                            item = SelectedNode.PrevVisibleNode;
                        }
                    }
                    else
                    {
                        item = SelectedNode;
                    }
                }
                TreeNode firstItem = item;
                while (item != null)
                {
                    if (item.Text.ToLower().Contains(text.ToLower()))
                    {
                        SelectedNode = null;
                        SelectedNode = item;
                        this.Invalidate();
                        break;
                    }
                    else
                    {
                        if (forward)
                        {
                            item = item.NextVisibleNode;
                        }
                        else
                        {
                            item = item.PrevVisibleNode;
                        }
                        if (item == null)
                        {
                            if (forward)
                            {
                                item = Nodes[0];
                            }
                            else
                            {
                                item = Nodes[Nodes.Count - 1];
                            }
                        }
                    }
                    if (item == firstItem)
                    {
                        break;
                    }
                }
                if (item != null)
                {
                }
            }
        }
        public Run currentRun;
		private RunView runView;
        public MethodView(string name,RunView runView)
        {
			this.runView = runView;
            this.BeforeExpand += delegate(object sender, TreeViewCancelEventArgs e)
            {
                EnsureComputed(e.Node);
            };
            this.SizeChanged += delegate
            {
            };
			//this.TreeViewNodeSorter = new Sorter();

        }
        void EnsureComputed(TreeNode item)
        {
            MakeSureComputed(item, true);
        }
        public List<Function> SortFunctions(IEnumerable<Function> f)
        {
            List<Function> functions = new List<Function>(f);
            functions.Sort(delegate(Function a, Function b)
            {
				switch(runView.sort)
				{
					case SortColumn.Inclusive:
						return b.Samples.CompareTo(a.Samples);
					case SortColumn.Exclusive:
						return b.ExclusiveSamples.CompareTo(a.ExclusiveSamples);
					case SortColumn.InclusiveChange:
						return a.InclusiveChange.CompareTo(b.InclusiveChange);
					case SortColumn.ExclusiveChange:
						return a.ExclusiveChange.CompareTo(b.ExclusiveChange);
				}
				return b.Samples.CompareTo(a.Samples);
            });
            return functions;
        }
        void MakeSureComputed(TreeNode item, bool parentExpanded)
        {
            if (item.Nodes.Count == 0)
            {
                foreach (Function function in SortFunctions(((Function)item.Tag).Children.Values))
                {
                    AddFunctionItem(item.Nodes, function);
                }
            }
            if (item.IsExpanded || parentExpanded)
            {
                foreach (TreeNode subItem in item.Nodes)
                {
                    MakeSureComputed(subItem, item.IsExpanded);
                }
            }
        }
        private TreeNode AddItem(TreeNodeCollection parent, Function function)
        {
            string signature = currentRun.signatures[function.ID].Signature;
            if (parent[signature] != null)
            {
                return parent[signature];
            }
            else
            {
				//int i = 0;
				//for (; i < parent.Count; i++)
				//{
				//    if (((Function)parent[i].Tag).Samples < function.Samples)
				//    {
				//        break;
				//    }
				//}
				TreeNode item = new TreeNode();//
				item.Tag = function;
				parent.Add(item);


				//parent.Insert(i, signature, signature);
                double fraction = ((double)function.Samples) / (double)currentRun.maxSamples;
                double percent = (100.0 * ((double)function.Samples / (double)function.run.maxSamples));
				double exclusivePercent = (100.0 * ((double)function.ExclusiveSamples / (double)function.run.maxSamples));
				string text = percent.ToString("0.00;-0.00;0.00").PadLeft(7, ' ');
				if (runView.comparisonRun != null)
				{

					if (runView.comparisonRun.functions.ContainsKey(function.ID))
					{
						Function other = runView.comparisonRun.functions[function.ID];
						//text += "  ";
						function.InclusiveChange=(((double)function.Samples - (double)other.Samples)/ function.run.maxSamples) * 100;
						function.ExclusiveChange= (((double)function.ExclusiveSamples - (double)other.ExclusiveSamples) / function.run.maxSamples) * 100;
						//text += (function.Change).ToString("+0.00;-0.00;0.00").PadLeft(11, ' ');
					}
					else
					{
						function.InclusiveChange = fraction;// (((double)function.Samples - (double)other.Samples) / function.run.maxSamples) * 100;
						function.ExclusiveChange = exclusivePercent;
						//text += (function.Change).ToString("+0.00;-0.00;0.00").PadLeft(11, ' ');
						//text += "  ".PadLeft(11,' ');
					}
					text += FormatChange(function.InclusiveChange);
				}
				text += "" + exclusivePercent.ToString("0.00;-0.00;0.00").PadLeft(7, ' ');
				if (runView.comparisonRun != null)
				{
					text += FormatChange(function.ExclusiveChange);
					//text += "  (" + (function.ExclusiveChange).ToString("+##0.00;-##0.00;+  0.00").PadLeft(7, ' ') + ")";
				}

				text += "  " + currentRun.signatures[function.ID].Signature.Trim();
				item.Text=text;
				//item.Text = percent.ToString("0.00;-0.00;0.00").PadLeft(7, ' ') + "  " + currentRun.signatures[function.ID].Signature.Trim();
                return item;
            }
        }

		private static string FormatChange(double change)
		{
			return "  (" +(change < 0?"-":"+") +(change).ToString("0.00;0.00;0.00").PadLeft(7, ' ') + ")";
		}
        public void AddFunctionItem(TreeNodeCollection parent, Function function)
        {
            TreeNode item = AddItem(parent, function);
            foreach (Function callee in SortFunctions(function.Children.Values))
            {
                AddItem(item.Nodes, callee);
            }
        }
        public void Add(Function function)
        {
            AddFunctionItem(Nodes, function);
        }
    }
    public class Profiler
    {
        public const string ProfilerGuid = "61D3E5D7-53E9-46B2-8DBC-011099A91793";
        public const string Version = "0.11";
        public EventHandler completed;
        public bool Start(Run run)
        {
            this.run = run;
            process = new Process();
            process.StartInfo = new ProcessStartInfo(NProf.application.Text, NProf.arguments.Text);
            process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
            process.StartInfo.EnvironmentVariables["COR_PROFILER"] = "{" + ProfilerGuid + "}";
            process.StartInfo.UseShellExecute = false;
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(OnProcessExited);

            string directory;
            if (NProf.directory.Text != "")
            {
                directory = NProf.directory.Text;
            }
            else
            {
                directory = Path.GetDirectoryName(run.Executable);
            }
            Directory.SetCurrentDirectory(directory);
            return process.Start();
        }
        private void OnProcessExited(object oSender, EventArgs ea)
        {
            completed(null, null);
        }
        private Run run;
        private Process process;
    }
}