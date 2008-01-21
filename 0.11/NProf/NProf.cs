/***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003,2004,2005,2006 by Matthew Mastracci, Christian Staudenmeyer
    email                : mmastrac@canada.com
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

namespace NProf {
	public class NamespaceView : View {

		private void Filter(MethodView view) {
			view.Update(run, functions);
			List<TreeNode> remove = new List<TreeNode>();
			foreach (TreeNode node in view.Nodes) {
				TreeNodeCollection nodes = Nodes;
				foreach (string s in ((Function)node.Tag).Signature.Namespace.Split('.')) {
					if (nodes[s]!=null && nodes[s].Checked) {
						nodes = nodes[s].Nodes;
					}
					else {
						nodes = null;
						break;
					}
				}
				if (nodes != null) {
					int asdf = 0;
				}
				else {
					//view.Nodes.Remove(node);
					remove.Add(node);
					//node.Remove();
				}
			}
			foreach (TreeNode node in remove) {
				node.Remove();
			}
		}
		public NamespaceView() {
			this.CheckBoxes = true;
			this.AfterCheck += delegate(object sender, TreeViewEventArgs e) {
				Filter(NProf.callees);
				Filter(NProf.callers);
			};
		}
		private Run run;
		private Dictionary<int, Function> functions;
		public void Update(Run run, Dictionary<int, Function> functions) {
			this.run = run;
			this.functions = functions;
		//public void Update(Run run, Dictionary<int, Function> functions, Dictionary<int, Function> compareFunctions, Run oldRun) {
			BeginUpdate();
			foreach (Function function in functions.Values) {
				TreeNodeCollection items = this.Nodes;
				foreach (string name in function.Signature.Namespace.Split('.')) {
					bool found = false;
					foreach (TreeNode item in items) {
						if (item.Text == name) {
							items = item.Nodes;
							found = true;
							break;
						}
					}
					if (!found) {
						TreeNode item = items.Add(name,name);
						item.Checked = true;
						items = item.Nodes;
					}
				}
			}
			EndUpdate();
		}
	}
	public class SearchPanel : FlowLayoutPanel {
		private MethodView methodView;
		public void Find(bool forward, bool step) {
			if (methodView.SelectedNode != null) {
				methodView.Find(findText.Text, forward, step);
			}
			else {
				methodView.Find(findText.Text, forward, step);
			}
		}
		public SearchPanel(MethodView methodView) {
			Dock = DockStyle.Bottom;
			this.methodView = methodView;
			findText = new TextBox();
			findText.TextChanged += delegate {
				Find(true, false);
			};
			findText.KeyDown += delegate(object sender, KeyEventArgs e) {
				if (e.KeyCode == Keys.Enter) {
					Find(true, true);
					e.Handled = true;
				}
			};

			BorderStyle = BorderStyle.FixedSingle;
			WrapContents = false;
			AutoSize = true;
			Button findNext = new Button();
			findNext.AutoSize = true;
			findNext.Text = "Next";
			findNext.Click += delegate (object sender, EventArgs e) {
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
			findPrevious.Click += delegate(object sender, EventArgs e) {
				Find(false, true);
			};
			findPrevious.Text = "Previous";

			Controls.AddRange(new Control[] { findLabel, findText, findNext, findPrevious });
		}
		private TextBox findText;

	}
	public class NProf : Form {
		public static Font font = new Font("Courier New", 9.0f);
		public ListView runs;
		public NamespaceView namespaces;
		public static MethodView callees = new MethodView("Callees");

		public static MethodView callers = new MethodView("Callers");

		private Profiler profiler;
		public static TextBox application;
		public static TextBox arguments;
		public string Title {
			get {
				return "NProf " + Profiler.Version;
			}
		}
		public void GoToCaller(MethodView source) {
			MoveTo(source, callers);
		}
		public void GoToCallee(MethodView source)
		{
			MoveTo(source, callees);
		}

		public void MoveTo(MethodView source,MethodView target) {
			if (source.SelectedNode !=null) {
				target.MoveTo(((Function)source.SelectedNode.Tag).ID);
			}
		}
		private void CallersNext() {
			if (callers.SelectedNode != null ) {
				TreeNode item = callers.SelectedNode;

				int id = ((Function)item.Tag).ID;
				if (item.Parent.Parent== null) {
					callees.MoveTo(id);
				}
				else {
					callers.MoveTo(id);
				}
				item.Collapse();

			}
		}
		private void CalleesNext() {
			if (callees.SelectedNode != null) {
				TreeNode item = callees.SelectedNode;
				if (item.Parent.Parent == null) {
					callers.MoveTo(((Function)item.Tag).ID);
				}
				else {
					callees.MoveTo(((Function)item.Tag).ID);
				}
				item.Collapse();
			}
		}
		private NProf() {

			WindowState = FormWindowState.Maximized;
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = Title;
			profiler = new Profiler();
			runs = new ListView();
			runs.View = System.Windows.Forms.View.Details;
			namespaces = new NamespaceView();
			namespaces.Height = 100;
			namespaces.Text = "Namespaces";
			namespaces.Dock = DockStyle.Bottom;
			runs.Columns.Add("Executable");
			runs.Columns.Add("Time");
			runs.Font = font;
			runs.Dock = DockStyle.Fill;
			runs.Columns[0].Width = 105;



			runs.SelectedIndexChanged+= delegate {
				if (runs.SelectedItems.Count != 0) {
					ShowRun((Run)runs.SelectedItems[0].Tag);
				}
			};
			callers.GotFocus += delegate {
				//callees.SelectedNode.Clear();
				//callees.SelectedItems.Clear();
			};
			callers.DoubleClick += delegate { CallersNext(); };
			callers.KeyDown += delegate(object sender, KeyEventArgs e) {
				if (e.KeyData == Keys.Enter) {
					CallersNext();
				}
			};
			callees.KeyDown += delegate(object sender, KeyEventArgs e) {
				if (e.KeyData == Keys.Enter) {
					CalleesNext();
				}
			};
			// TODO: inherit from base class
			callees.GotFocus += delegate {
				//callers.SelectedItems.Clear();
			};
			callees.DoubleClick += delegate {
				CalleesNext();
			};
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
			//callees.SelectedItemsChanged += delegate {
			//    if (callees.SelectedItems.Count != 0) {
			//        ContainerListViewItem item = callees.SelectedItems[0];
			//        if (item.Items.Count == 0) {
			//            foreach (FunctionInfo f in ((FunctionInfo)item.Tag).Children.Values) {
			//                callees.AddFunctionItem(item.Items, f);
			//            }
			//        }
			//    }
			//};
			//findText = new TextBox();
			//findText.TextChanged += delegate {
			//    Find(true, false);
			//};
			//findText.KeyDown += delegate(object sender, KeyEventArgs e) {
			//    if (e.KeyCode == Keys.Enter) {
			//        Find(true, true);
			//        e.Handled = true;
			//    }
			//};
			Menu = new MainMenu(new MenuItem[] {
				new MenuItem(
					"File",
					new MenuItem[] {
						new MenuItem("E&xit",delegate {Close();})
					}),
				new MenuItem(
					"Project",
					new MenuItem[] {
						new MenuItem(
							"Start",
							delegate{StartRun();},
							Shortcut.F5)
			})});
			Panel panel = new Panel();
			panel.Dock = DockStyle.Fill;
			SplitContainer methodPanel = new SplitContainer();
			methodPanel.Orientation = Orientation.Horizontal;

			Splitter methodSplitter = new Splitter();
			methodSplitter.Dock = DockStyle.Bottom;

			callees.Size = new Size(100, 100);
			callers.Size = new Size(100, 100);
			callees.Dock = DockStyle.Fill;
			callers.Dock = DockStyle.Fill;
			methodPanel.Panel2.Controls.Add(callers);
			methodPanel.Panel1.Controls.Add(callees);
			Panel rightPanel = new Panel();
			methodPanel.Dock = DockStyle.Fill;

			rightPanel.Dock = DockStyle.Fill;
			rightPanel.Controls.Add(methodPanel);

			//rightPanel.Controls.Add(new SearchPanel(callers));

			Splitter mainSplitter = new Splitter();
			mainSplitter.Dock = DockStyle.Left;

			Panel leftPanel = new Panel();
			leftPanel.Width = 200;
			leftPanel.Dock = DockStyle.Left;
			leftPanel.Controls.Add(runs);
			Splitter leftSplitter = new Splitter();
			leftSplitter.Dock = DockStyle.Bottom;
			leftPanel.Controls.Add(leftSplitter);
			leftPanel.Controls.Add(namespaces);

			panel.Controls.AddRange(new Control[] { rightPanel, mainSplitter, leftPanel});
			methodPanel.Dock = DockStyle.Fill;


			TableLayoutPanel mainPanel = new TableLayoutPanel();
			application = new TextBox();
			application.Width = 300;
			arguments = new TextBox();
			arguments.Width = 300;
			mainPanel.Height = 100;
			mainPanel.AutoSize = true;
			mainPanel.Dock = DockStyle.Top;

			Label applicationLabel = new Label();
			applicationLabel.Text = "Executable:";
			applicationLabel.Dock = DockStyle.Fill;
			applicationLabel.TextAlign = ContentAlignment.MiddleLeft;
			applicationLabel.AutoSize = true;

			mainPanel.Controls.Add(applicationLabel, 0, 0);
			mainPanel.Controls.Add(application, 1, 0);
			Button browse = new Button();
			browse.Text = "&Browse...";
			browse.TabIndex = 0;
			browse.Focus();
			browse.Click += delegate {
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "Executable files (*.exe)|*.exe";
				DialogResult dr = dialog.ShowDialog();
				if (dr == DialogResult.OK) {
					application.Text = dialog.FileName;
					application.Focus();
					application.SelectAll();
				}
			};
			mainPanel.Controls.Add(browse, 2, 0);
			Label argumentLabel = new Label();
			argumentLabel.Text = "Arguments:";
			argumentLabel.Dock = DockStyle.Fill;
			argumentLabel.TextAlign = ContentAlignment.MiddleLeft;
			argumentLabel.AutoSize = true;
			mainPanel.Controls.Add(argumentLabel, 0, 1);
			mainPanel.Controls.Add(arguments, 1, 1);
			Controls.AddRange(new Control[] { panel, mainPanel});
			application.TextChanged += delegate {
				string fileName = Path.GetFileName(application.Text);
				Text = fileName + " - " + Title;
			};
		}
		private void StartRun() {
			Run run = new Run(profiler);
			run.profiler.completed = new EventHandler(run.Complete);
			run.Start();
		}
		public void AddRun(Run run) {
			string text = Path.GetFileNameWithoutExtension(application.Text) + " " + runs.Items.Count;
			string title = Path.GetFileNameWithoutExtension(application.Text);
			ListViewItem item = new ListViewItem(title);
			item.Tag = run;
			runs.Items.Add(item);
			runs.SelectedItems.Clear();
			item.SubItems[0].Text = title;
			item.Selected = true;
			ShowRun(run);
		}
		public void ShowRun(Run run) {
			Run compareRun;
			if (runs.SelectedItems.Count > 1) {
				ListViewItem first = runs.SelectedItems[runs.SelectedItems.Count - 1];
				compareRun = (Run)first.Tag;
			}
			else {
				compareRun = null;
			}
			callees.Update(run, run.functions);
			callers.Update(run, run.callers);
			namespaces.Update(run, run.callers);
		}
		[STAThread]
		static void Main(string[] args) {
			string s = Guid.NewGuid().ToString().ToUpper();
			Application.EnableVisualStyles();
			Application.Run(form);
		}
		public static NProf form = new NProf();

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NProf));
			this.SuspendLayout();
			// 
			// NProf
			// 
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "NProf";
			this.ResumeLayout(false);

		}
	}
	public class StackWalk {
		public StackWalk(int id, int index, List<int> frames) {
			this.id = id;
			this.frames = frames;
			this.length = index;
		}
		public readonly int length;
		public readonly int id;
		public readonly List<int> frames;
	}
	public class FunctionInfo {
		public readonly string Namespace;
		public readonly string Signature;
		public FunctionInfo(string signature, string nameSpace) {
			this.Signature = signature;
			this.Namespace = nameSpace;
		}
	}
	public class Function {
		public Run run;
		public FunctionInfo Signature {
			get {
				return run.signatures[ID];
			}
		}
		//public string Namespace {
		//    get {
		//        return nameSpace;
		//    }
		//}
		//private string nameSpace;
		public Function(int ID,Run run) {
			this.run = run;
			this.ID = ID;
			//this.nameSpace = nameSpace;
		}
		public readonly int ID;
		public int Samples;
		public int lastWalk;
		private Dictionary<int, Function> children;
		public List<StackWalk> stackWalks = new List<StackWalk>();
		public Dictionary<int, Function> Children {
			get {
				if (children == null) {
					children = new Dictionary<int, Function>();
					foreach (StackWalk walk in stackWalks) {
						if (walk.length != 0) {
							Function callee = Run.GetFunctionInfo(children, walk.frames[walk.length - 1],run);
							if (callee.lastWalk != walk.id) {
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
	public class Run {
		public static Function GetFunctionInfo(Dictionary<int, Function> functions, int id,Run run) {
			Function result;
			if (!functions.TryGetValue(id, out result)) {
				result = new Function(id, run);
				//result = new Function(id, run, "System.Web");
				functions[id] = result;
			}
			return result;
		}
		public int maxSamples;
		public List<List<int>> stackWalks = new List<List<int>>();
		private void InterpreteData() {
			Interprete(functions,false,this);
			Interprete(callers,true,this);
			List<Function> startFunctions = new List<Function>();
			maxSamples = 0;
			foreach (List<int> stackWalk in stackWalks) {
				if (stackWalk.Count != 0) {
					maxSamples++;
				}
			}
		}
		private void Interprete(Dictionary<int, Function> map,bool reverse,Run run) {
			int currentWalk = 0;
			foreach (List<int> original in stackWalks) {
				currentWalk++;
				List<int> stackWalk;
				if (reverse) {
					stackWalk = new List<int>(original);
					stackWalk.Reverse();
				}
				else {
					stackWalk = original;
				}
				for (int i = 0; i < stackWalk.Count; i++) {
					Function function = Run.GetFunctionInfo(map, stackWalk[stackWalk.Count - i - 1],run);
					if (function.lastWalk != currentWalk) {
						function.Samples++;
						function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.Count - i - 1, stackWalk));
					}
					function.lastWalk = currentWalk;
				}
			}
		}
		private string ReadString(BinaryReader br) {
			int length = br.ReadInt32();
			byte[] abString = new byte[length];
			br.Read(abString, 0, length);
			return System.Text.ASCIIEncoding.ASCII.GetString(abString, 0, length);
		}
		private string FileName {
			get {
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
		private void ReadStackWalks() {
			using (BinaryReader r = new BinaryReader(File.Open(FileName, FileMode.Open))) {
				this.end = DateTime.Now;
				while (true) {
					int functionId = r.ReadInt32();
					if (functionId == -1) {
						break;
					}
					signatures[functionId] = new FunctionInfo(
						ReadString(r),
						ReadString(r)
					);
				}
				while (true) {
					List<int> stackWalk = new List<int>();
					while (true) {
						int functionId = r.ReadInt32();
						if (functionId == -1) {
							break;
						}
						else if (functionId == -2) {
							return;
						}
						stackWalk.Add(functionId);
					}
					stackWalks.Add(stackWalk);
				}
			}
		}
		public void Complete(object sender, EventArgs e) {
			if (File.Exists(FileName)) {
				ReadStackWalks();
				File.Delete(FileName);
			}
			NProf.form.BeginInvoke(new EventHandler(delegate {
				InterpreteData();
				NProf.form.AddRun(this);
			}));
		}
		public Dictionary<int, FunctionInfo> signatures = new Dictionary<int, FunctionInfo>();
		//public Dictionary<int, string> signatures = new Dictionary<int, string>();
		public Dictionary<int, Function> functions = new Dictionary<int, Function>();
		public Dictionary<int, Function> callers = new Dictionary<int, Function>();
		private DateTime start=DateTime.MinValue;
		private DateTime end=DateTime.MinValue;
		public Run(Profiler p) {
			this.profiler = p;
		}
		public bool Start() {
			start = DateTime.Now;
			// refactor this:
			return profiler.Start(this);
		}
		public Profiler profiler;
	}
	public class View : TreeView {
		public View() {
			this.Font=NProf.font;
			this.ShowPlusMinus = true;
		}
	}
	public class MethodView : View {
		public void MoveTo(int id) {
			foreach (TreeNode item in Nodes) {
				if (((Function)item.Tag).ID == id) {
					this.SelectedNode = item;
					Invalidate();
					this.Focus();
					break;
				}
			}
		}
		public void Update(Run run, Dictionary<int, Function> functions) {
			currentRun = run;
			SuspendLayout();
			Invalidate();
			BeginUpdate();
			//Nodes.Clear();
			foreach (Function method in SortFunctions(functions.Values)) {
				AddItem(Nodes, method);
			}
			foreach (TreeNode item in Nodes) {
				MakeSureComputed(item);
			}
			EndUpdate();
			ResumeLayout();
		}
		// remove compareFunctions
		//public void Update(Run run, Dictionary<int, Function> functions) {
		//    currentRun = run;
		//    SuspendLayout();
		//    Invalidate();
		//    BeginUpdate();
		//    Nodes.Clear();
		//    //Items.Clear();
		//    //foreach (Function method in SortFunctions(functions.Values)) {
		//    //    Function oldFunction;
		//    //    if (compareFunctions != null && compareFunctions.ContainsKey(method.ID)) {
		//    //        oldFunction = compareFunctions[method.ID];
		//    //    }
		//    //    else {
		//    //        oldFunction = null;
		//    //    }
		//    //    AddItem(Nodes, method, oldFunction);
		//    //}
		//    foreach (Function method in SortFunctions(functions.Values)) {
		//        AddItem(Nodes, method);
		//    }
		//    foreach (TreeNode item in Nodes) {
		//        MakeSureComputed(item);
		//    }
		//    EndUpdate();
		//    ResumeLayout();
		//}
		public void Find(string text, bool forward, bool step) {
			if (text != "") {
				TreeNode item;
				if (SelectedNode == null) {
					if (Nodes.Count == 0) {
						item = null;
					}
					else {
						item = Nodes[0];
					}
				}
				else {
					if (step) {
						if (forward) {
							item = SelectedNode.NextVisibleNode;
						}
						else {
							item = SelectedNode.PrevVisibleNode;
						}
					}
					else {
						item = SelectedNode;
					}
				}
				TreeNode firstItem = item;
				while (item != null) {
					if (item.Text.ToLower().Contains(text.ToLower())) {
						SelectedNode = null;
						SelectedNode = item;
						this.Invalidate();
						break;
					}
					else {
						if (forward) {
							item = item.NextVisibleNode;
						}
						else {
							item = item.PrevVisibleNode;
						}
						if (item == null) {
							if (forward) {
								item = Nodes[0];
							}
							else {
								item = Nodes[Nodes.Count - 1];
							}
						}
					}
					if (item == firstItem) {
						break;
					}
				}
				if (item != null) {
				}
			}
		}
		public Run currentRun;
		//public Run currentOldRun;
		public MethodView(string name) {
			this.BeforeExpand += delegate(object sender, TreeViewCancelEventArgs e) {
				MakeSureComputed(e.Node);
			};
			this.SizeChanged += delegate {
			};
		}
		void MakeSureComputed(TreeNode item) {
			MakeSureComputed(item, true);
		}
		public List<Function> SortFunctions(IEnumerable<Function> f) {
			List<Function> functions = new List<Function>(f);
			functions.Sort(delegate(Function a, Function b) {
				return b.Samples.CompareTo(a.Samples);
			});
			return functions;
		}
		void MakeSureComputed(TreeNode item, bool parentExpanded) {
			if (item.Nodes.Count == 0) {
				foreach (Function function in SortFunctions(((Function)item.Tag).Children.Values)) {
					AddFunctionItem(item.Nodes, function);
				}
			}
			if (item.IsExpanded || parentExpanded) {
				foreach (TreeNode subItem in item.Nodes) {
					MakeSureComputed(subItem, item.IsExpanded);
				}
			}
		}
		private TreeNode AddItem(TreeNodeCollection parent, Function function) {
			string signature = currentRun.signatures[function.ID].Signature;
			if (parent[signature] != null) {
				return parent[signature];
			}
			else {
				int i = 0;
				for (; i < parent.Count; i++) {
					if (((Function)parent[i].Tag).Samples < function.Samples) {
						break;
					}
				}
				TreeNode item = parent.Insert(i,signature, signature);
				//TreeNode item = parent.Add(signature, signature);
				double fraction = ((double)function.Samples) / (double)currentRun.maxSamples;
				double percent = (100.0 * ((double)function.Samples / (double)function.run.maxSamples));
				item.Text = " " + percent.ToString("0.00;-0.00;0.00").PadLeft(5, ' ') + "  " + currentRun.signatures[function.ID].Signature.Trim();
				item.Tag = function;
				return item;
			}
		}
		void label_Paint(object sender, PaintEventArgs e) {
			e.Graphics.DrawRectangle(Pens.Red,new Rectangle(10,5,100,10));
		}
		public void AddFunctionItem(TreeNodeCollection parent, Function function) {
		//public void AddFunctionItem(ContainerListViewItemCollection parent, FunctionInfo function) {
			TreeNode item = AddItem(parent, function);
			//ContainerListViewItem item = AddItem(parent, function, null);
			foreach (Function callee in SortFunctions(function.Children.Values)) {
				AddItem(item.Nodes, callee);
				//AddItem(item.Items, callee, null);
			}
		}
		public void Add(Function function) {
			AddFunctionItem(Nodes, function);
			//AddFunctionItem(Items, function);
		}
	}
	public class Profiler {
		public const string ProfilerGuid = "107F578A-E019-4BAF-86A1-7128A749DB05";
		public const string Version = "0.11"; 
		public EventHandler completed;
		public bool Start(Run run) {
			this.run = run;
			process = new Process();
			process.StartInfo = new ProcessStartInfo(NProf.application.Text, NProf.arguments.Text);
			process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
			process.StartInfo.EnvironmentVariables["COR_PROFILER"] = "{" + ProfilerGuid + "}";
			process.StartInfo.UseShellExecute = false;
			process.EnableRaisingEvents = true;
			process.Exited += new EventHandler(OnProcessExited);
			return process.Start();
		}
		private void OnProcessExited(object oSender, EventArgs ea) {
			completed(null, null);
		}
		private Run run;
		private Process process;
	}
}