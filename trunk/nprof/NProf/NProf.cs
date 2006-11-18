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
using DotNetLib.Windows.Forms;

namespace NProf {
	public class NProf : Form {
		public ContainerListView runs;
		private MethodView callees;
		private MethodView callers;
		private Profiler profiler;
		private TextBox findText;
		private FlowLayoutPanel findPanel;
		public static TextBox application;
		public static TextBox arguments;
		public void ShowSearch() {
			findPanel.Visible = !findPanel.Visible;
			findText.Focus();
		}
		public void Find(bool forward, bool step) {
			if (callers.SelectedItems.Count != 0) {
				callers.Find(findText.Text, forward, step);
			}
			else {
				callees.Find(findText.Text, forward, step);
			}
		}
		public string Title {
			get {
				return "NProf " + Profiler.Version;
			}
		}
		private NProf() {
			WindowState = FormWindowState.Maximized;
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = Title;
			profiler = new Profiler();
			runs = new ContainerListView();
			runs.ColumnSortColor = Color.White;
			runs.Columns.Add("Runs");
			runs.Columns.Add("Time");
			runs.AllowMultiSelect = true;
			runs.Dock = DockStyle.Left;
			runs.Width = 200;
			runs.SelectedItemsChanged += delegate {
				if (runs.SelectedItems.Count != 0) {
					ShowRun((Run)runs.SelectedItems[0].Tag);
				}
			};
			callers = new MethodView("Callers");
			callers.Size = new Size(100, 200);
			callers.Dock = DockStyle.Bottom;
			callers.GotFocus += delegate {
				callees.SelectedItems.Clear();
			};
			callers.DoubleClick+= delegate {
				if (callers.SelectedItems.Count != 0) {
					callees.MoveTo(((FunctionInfo)callers.SelectedItems[0].Tag).ID);
				}
			};
			callees = new MethodView("Callees");
			callees.Size = new Size(100, 100);
			callees.Dock = DockStyle.Fill;
			callees.GotFocus += delegate {
				callers.SelectedItems.Clear();
			};
			callees.DoubleClick+= delegate {
				if (callees.SelectedItems.Count != 0) {
					callers.MoveTo(((FunctionInfo)callees.SelectedItems[0].Tag).ID);
				}
			};
			callees.SelectedItemsChanged += delegate {
				if (callees.SelectedItems.Count != 0) {
					ContainerListViewItem item = callees.SelectedItems[0];
					if (item.Items.Count == 0) {
						foreach (FunctionInfo f in ((FunctionInfo)item.Tag).Children.Values) {
							callees.AddFunctionItem(item.Items, f);
						}
					}
				}
			};
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
			Menu = new MainMenu(new MenuItem[] {
				new MenuItem(
					"File",
					new MenuItem[] {
						new MenuItem("&New",
							delegate {
								runs.Items.Clear();
								NProf.arguments.Text = "";
								NProf.application.Text = "";
								callees.Items.Clear();
								callers.Items.Clear();
							},
							Shortcut.CtrlN),
						new MenuItem("-"),
						new MenuItem("E&xit",delegate {Close();})
					}),
				new MenuItem(
					"Project",
					new MenuItem[] {
						new MenuItem(
							"Start",
							delegate{StartRun();},
							Shortcut.F5),
						new MenuItem("-"),
						new MenuItem("Find",delegate{ShowSearch();},Shortcut.CtrlF)
			})});
			Panel rightPanel = new Panel();
			rightPanel.Dock = DockStyle.Fill;
			Panel methodPanel = new Panel();
			methodPanel.Size = new Size(100, 100);
			methodPanel.Dock = DockStyle.Fill;

			Splitter methodSplitter = new Splitter();
			methodSplitter.Dock = DockStyle.Bottom;
			findPanel = new FlowLayoutPanel();
			findPanel.BorderStyle = BorderStyle.FixedSingle;
			findPanel.Visible = false;
			findPanel.WrapContents = false;
			findPanel.AutoSize = true;
			findPanel.Dock = DockStyle.Top;
			Button closeFind = new Button();
			closeFind.Text = "x";
			closeFind.Width = 17;
			closeFind.Height = 20;
			closeFind.TextAlign = ContentAlignment.BottomLeft;
			closeFind.Click += delegate {findPanel.Visible = false;};

			Button findNext = new Button();
			findNext.AutoSize = true;
			findNext.Text = "Next";
			findNext.Click += new EventHandler(findNext_Click);

			Label findLabel = new Label();
			findLabel.Text = "Find:";
			findLabel.Dock = DockStyle.Fill;
			findLabel.TextAlign = ContentAlignment.MiddleLeft;
			findLabel.AutoSize = true;

			Button findPrevious = new Button();
			findPrevious.AutoSize = true;
			findPrevious.FlatAppearance.BorderSize = 0;
			findPrevious.Click += new EventHandler(findPrevious_Click);
			findPrevious.Text = "Previous";

			findPanel.Controls.AddRange(new Control[] {closeFind,findLabel,findText,findNext ,findPrevious});
			methodPanel.Controls.AddRange(new Control[] {callees,methodSplitter,callers,findPanel});

			Splitter mainSplitter = new Splitter();
			mainSplitter.Dock = DockStyle.Left;
			rightPanel.Controls.AddRange(new Control[] { methodPanel, mainSplitter, runs });

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
			browse.Text = "Browse...";
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
			Controls.AddRange(new Control[] { rightPanel, mainPanel});
			application.TextChanged += delegate {
				Text = Path.GetFileNameWithoutExtension(application.Text) + " - " + Title;
			};
			this.Load += delegate {
				Size = new Size(800, 600);
			};
		}
		private void StartRun() {
			string message;
			bool success = profiler.CheckSetup(out message);
			if (!success) {
				MessageBox.Show(this, message, "Application setup error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			Run run = new Run(profiler);
			run.profiler.completed = new EventHandler(run.Complete);
			run.Start();
		}
		public void AddRun(Run run) {
			int count=1;
			string text = Path.GetFileNameWithoutExtension(application.Text);
			int maxStackWalks=run.stackWalks.Count;
			foreach (ContainerListViewItem i in runs.Items) {
				if (i.Text.StartsWith(text)) {
					count++;
				}
				int c=((Run)i.Tag).stackWalks.Count;
				if(c>maxStackWalks) {
					maxStackWalks=c;
				}
			}
			string title = Path.GetFileNameWithoutExtension(application.Text) + " " + count;
			ContainerListViewItem item = new ContainerListViewItem(title);
			item.Tag = run;
			runs.Items.Add(item);
			runs.SelectedItems.Clear();
			item.SubItems[0].Text = title;
			item.SubItems[1].Text = (((double)run.stackWalks.Count) / maxStackWalks).ToString("0.00;-0.00;0.00")+"s";
			runs.SelectedItems.Add(item);
			ShowRun(run);
		}
		public void ShowRun(Run run) {
			Run compareRun;
			if (runs.SelectedItems.Count > 1) {
				ContainerListViewItem first = runs.SelectedItems[runs.SelectedItems.Count - 1];
				compareRun = (Run)first.Tag;
			}
			else {
				compareRun = null;
			}
			callees.Update(run, run.functions,compareRun!=null?compareRun.functions:null,compareRun);
			callers.Update(run, run.callers,compareRun!=null?compareRun.callers:null,compareRun);
		}
		private void findNext_Click(object sender, EventArgs e) {
			Find(true, true);
		}
		private void findPrevious_Click(object sender, EventArgs e) {
			Find(false, true);
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
		public FunctionInfo(int ID) {
			this.ID = ID;
		}
		public readonly int ID;
		public int Samples;
		public int lastWalk;
		private Dictionary<int, FunctionInfo> children;
		public List<StackWalk> stackWalks = new List<StackWalk>();
		public Dictionary<int, FunctionInfo> Children {
			get {
				if (children == null) {
					children = new Dictionary<int, FunctionInfo>();
					foreach (StackWalk walk in stackWalks) {
						if (walk.length != 0) {
							FunctionInfo callee = Run.GetFunctionInfo(children, walk.frames[walk.length - 1]);
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
		public static FunctionInfo GetFunctionInfo(Dictionary<int, FunctionInfo> functions, int id) {
			FunctionInfo result;
			if (!functions.TryGetValue(id, out result)) {
				result = new FunctionInfo(id);
				functions[id] = result;
			}
			return result;
		}
		public int maxSamples;
		public List<List<int>> stackWalks = new List<List<int>>();
		private void InterpreteData() {
			Interprete(functions,false);
			Interprete(callers,true);
			maxSamples = 0;
			foreach (FunctionInfo function in functions.Values) {
				if (function.Samples > maxSamples) {
					maxSamples = function.Samples;
				}
			}
		}
		private void Interprete2(int currentWalk, List<int> stackWalk,Dictionary<int, FunctionInfo> map) {
			for (int i = 0; i < stackWalk.Count; i++) {
				FunctionInfo function = Run.GetFunctionInfo(map, stackWalk[stackWalk.Count - i - 1]);
				if (function.lastWalk != currentWalk) {
					function.Samples++;
					function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.Count - i - 1, stackWalk));
				}
				function.lastWalk = currentWalk;
			}
		}
		private void Interprete(Dictionary<int, FunctionInfo> map,bool reverse) {
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
					FunctionInfo function = Run.GetFunctionInfo(map, stackWalk[stackWalk.Count - i - 1]);
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
				return Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), Profiler.PROFILER_GUID + ".nprof");
			}
		}
		private void ReadStackWalks() {
			using (BinaryReader r = new BinaryReader(File.Open(FileName, FileMode.Open))) {
				while (true) {
					int functionId = r.ReadInt32();
					if (functionId == -1) {
						break;
					}
					r.ReadUInt32();
					signatures[functionId] = ReadString(r);
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
		public Dictionary<int, string> signatures = new Dictionary<int, string>();
		public Dictionary<int, FunctionInfo> functions = new Dictionary<int, FunctionInfo>();
		public Dictionary<int, FunctionInfo> callers = new Dictionary<int, FunctionInfo>();
		public Run(Profiler p) {
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
		}
		public bool Start() {
			start = DateTime.Now;
			return profiler.Start(this);
		}
		private DateTime start;
		private DateTime end;
		public Profiler profiler;
	}
	public class View : ContainerListView {
		public View() {
			this.SizeChanged += delegate {
				this.Columns[0].Width = this.Width - 30;
			};
		}
	}
	public class MethodView : View {
		public void MoveTo(int id) {
			SelectedItems.Clear();
			foreach (ContainerListViewItem item in Items) {
				if (((FunctionInfo)item.Tag).ID == id) {
					SelectedItems.Add(item);
					EnsureVisible(item);
					Invalidate();
					break;
				}
			}
		}
		public void Update(Run run, Dictionary<int, FunctionInfo> functions, Dictionary<int, FunctionInfo> compareFunctions,Run oldRun) {
			currentRun = run;
			currentOldRun = oldRun;
			SuspendLayout();
			BeginUpdate();
			Items.Clear();
			foreach (FunctionInfo method in SortFunctions(functions.Values)) {
				FunctionInfo oldFunction;
				if (compareFunctions != null && compareFunctions.ContainsKey(method.ID)) {
					oldFunction = compareFunctions[method.ID];
				}
				else {
					oldFunction = null;
				}
				AddItem(Items, method,oldFunction);
			}
			//foreach (FunctionInfo method in SortFunctions(functions.Values)) {
			//    AddItem(Items, method,);
			//}
			foreach (ContainerListViewItem item in Items) {
				MakeSureComputed(item);
			}
			EndUpdate();
			ResumeLayout();
		}
		public void Find(string text, bool forward, bool step) {
			if (text != "") {
				ContainerListViewItem item;
				if (SelectedItems.Count == 0) {
					if (Items.Count == 0) {
						item = null;
					}
					else {
						item = Items[0];
					}
				}
				else {
					if (step) {
						if (forward) {
							item = SelectedItems[0].NextVisibleItem;
						}
						else {
							item = SelectedItems[0].PreviousVisibleItem;
						}
					}
					else {
						item = SelectedItems[0];
					}
				}
				ContainerListViewItem firstItem = item;
				while (item != null) {
					if (item.Text.ToLower().Contains(text.ToLower())) {
						SelectedItems.Clear();
						item.Focused = true;
						item.Selected = true;
						item.Focused = true;
						this.Invalidate();
						break;
					}
					else {
						if (forward) {
							item = item.NextVisibleItem;
						}
						else {
							item = item.PreviousVisibleItem;
						}
						if (item == null) {
							if (forward) {
								item = Items[0];
							}
							else {
								item = Items[Items.Count - 1];
							}
						}
					}
					if (item == firstItem) {
						break;
					}
				}
				if (item != null) {
					EnsureVisible(item);
				}
			}
		}
		public Run currentRun;
		public Run currentOldRun;
		public MethodView(string name) {
			this.DefaultItemHeight = 20;
			this.SelectedItemsChanged += new EventHandler(MethodView_SelectedItemsChanged);
			Columns.Add(name);
			Columns[0].Width = 350;
			Columns[0].SortDataType = SortDataType.String;
			this.ShowPlusMinus = true;
			ShowRootTreeLines = true;
			ShowTreeLines = true;
			FullItemSelect = true;
			ColumnSortColor = Color.White;
			Font = new Font("Tahoma", 8.0f);
			this.BeforeExpand += delegate(object sender,ContainerListViewCancelEventArgs e) {
				MakeSureComputed(e.Item);
			};
		}
		void MethodView_SelectedItemsChanged(object sender, EventArgs e) {
			//if (SelectedItems.Count != 0) {
			//    NProf.status.Text = ((((FunctionInfo)SelectedItems[0].Tag).Samples / (double)currentRun.maxSamples) * 100).ToString("0.00;-0.00;0.00") + "%";
			//}
		}
		void MakeSureComputed(ContainerListViewItem item) {
			MakeSureComputed(item, true);
		}
		public List<FunctionInfo> SortFunctions(IEnumerable<FunctionInfo> f) {
			List<FunctionInfo> functions = new List<FunctionInfo>(f);
			functions.Sort(delegate(FunctionInfo a, FunctionInfo b) {
				return b.Samples.CompareTo(a.Samples);
			});
			return functions;
		}
		void MakeSureComputed(ContainerListViewItem item, bool parentExpanded) {
			if (item.Items.Count == 0) {
				foreach (FunctionInfo function in SortFunctions(((FunctionInfo)item.Tag).Children.Values)) {
					AddFunctionItem(item.Items, function);
				}
			}
			if (item.Expanded || parentExpanded) {
				foreach (ContainerListViewItem subItem in item.Items) {
					MakeSureComputed(subItem, item.Expanded);
				}
			}
		}
		public class ChildLabel : Label {
			private ContainerListViewItem item;
			public ChildLabel(string text,ContainerListViewItem item) {
				this.item = item;
				this.Click += delegate {
					item.Selected = true;
				};
				this.Text = text;
				Margin = new Padding(0);
			}
		}
		private ContainerListViewItem AddItem(ContainerListViewItemCollection parent, FunctionInfo function,FunctionInfo oldFunction) {
			ContainerListViewItem item = parent.Add(currentRun.signatures[function.ID]);
			double fraction = ((double)function.Samples) / (double)currentRun.maxSamples;
			double oldFraction;
			if (oldFunction != null) {
				oldFraction = ((double)oldFunction.Samples) / (double)currentRun.maxSamples;
			}
			else {
				oldFraction = 0;
			}
			FlowLayoutPanel panel = new FlowLayoutPanel();
			panel.Padding = new Padding(0);
			panel.Margin = new Padding(0);
			panel.FlowDirection = FlowDirection.LeftToRight;

			Label signature=new ChildLabel(currentRun.signatures[function.ID],item);
			//signature.Margin = new Padding(0);
			//signature.Text=;
			signature.AutoSize = true;


			Label time = new ChildLabel((fraction * 100.0).ToString("0.00;-0.00;0.00"),item);
			time.Width = 34;
			time.TextAlign = ContentAlignment.TopRight;
			time.Padding = new Padding(0, 0, 2, 0);
			//time.Margin = new Padding(0);

			//Label time=new Label();
			//time.Text=(fraction * 100.0).ToString("0.00;-0.00;0.00");
			//time.Width = 34;
			//time.TextAlign = ContentAlignment.TopRight;
			//time.Padding = new Padding(0,0,2,0);
			//time.Margin = new Padding(0);

			panel.Controls.Add(time);
			if (oldFunction != null) {
				int difference=function.Samples-oldFunction.Samples;
				Label old = new ChildLabel((difference*fraction).ToString("+0.00;-0.00;+0.00"), item);
				old.Width = 44;
				panel.Controls.Add(old);
			}
			panel.Controls.Add(signature);
			item.SubItems[0].ItemControl = panel;

			//int childSamples = 0;
			//foreach (FunctionInfo f in function.Children.Values) {
			//    childSamples += f.Samples;
			//}
			item.Tag = function;
			return item;
		}
		void label_Paint(object sender, PaintEventArgs e) {
			e.Graphics.DrawRectangle(Pens.Red,new Rectangle(10,5,100,10));
		}
		public void AddFunctionItem(ContainerListViewItemCollection parent, FunctionInfo function) {
			ContainerListViewItem item = AddItem(parent, function,null);
			foreach (FunctionInfo callee in function.Children.Values) {
				AddItem(item.Items, callee,null);
			}
		}
		public void Add(FunctionInfo function) {
			AddFunctionItem(Items, function);
		}
	}
	public class Profiler {
		public const string PROFILER_GUID = "107F578A-E019-4BAF-86A1-7128A749DB05";
		public static string Version {
			get { return "0.11"; }
		}
		public EventHandler completed;
		public bool CheckSetup(out string message) {
			message = String.Empty;
			using (RegistryKey rk = Registry.ClassesRoot.OpenSubKey("CLSID\\" + "{" + PROFILER_GUID + "}")) {
				if (rk == null) {
					message = "Unable to find the registry key for the profiler hook.  Please register the NProf.Hook.dll file.";
					return false;
				}
			}
			return true;
		}
		public bool Start(Run run) {
			this.run = run;
			process = new Process();
			process.StartInfo = new ProcessStartInfo(NProf.application.Text, NProf.arguments.Text);
			process.StartInfo.EnvironmentVariables["COR_ENABLE_PROFILING"] = "0x1";
			process.StartInfo.EnvironmentVariables["COR_PROFILER"] = "{" + PROFILER_GUID + "}";
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