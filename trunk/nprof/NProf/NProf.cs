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
			Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			Text = Title;
			profiler = new Profiler();

			runs = new ContainerListView();
			ContainerListViewColumnHeader header = new ContainerListViewColumnHeader("Profiling runs", 90);
			runs.Columns.Add(header);
			runs.SizeChanged += delegate {
				header.Width = runs.Size.Width - 5;
			};
			runs.Dock = DockStyle.Left;
			runs.Width = 100;
			runs.DoubleClick += delegate {
				if (runs.SelectedItems.Count != 0) {
					ShowRun((Run)runs.SelectedItems[0].Tag);
				}
			};

			callers = new MethodView("Caller methods");
			callers.Size = new Size(100, 200);
			callers.Dock = DockStyle.Bottom;
			callers.GotFocus += delegate {
				callees.SelectedItems.Clear();
			};
			callers.DoubleClick += delegate {
				if (callers.SelectedItems.Count != 0) {
					callees.MoveTo(((FunctionInfo)callers.SelectedItems[0].Tag).ID);
				}
			};

			callees = new MethodView("Callee methods");
			callees.Size = new Size(100, 100);
			callees.Dock = DockStyle.Fill;
			callees.GotFocus += delegate {
				callers.SelectedItems.Clear();
			};
			callees.DoubleClick += delegate {
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
			Menu = new MainMenu(new MenuItem[]
				{
					new MenuItem(
						"File",
						new MenuItem[] 
						{
							new MenuItem(
								"&New",
								delegate
								{
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
						new MenuItem[]
						{
							new MenuItem(
								"Start",
								delegate{StartRun();},
								Shortcut.F5),
							new MenuItem("-"),
							new MenuItem("Find",delegate{ShowSearch();},Shortcut.CtrlF)
						})
				});
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
			closeFind.Click += delegate {
				findPanel.Visible = false;
			};

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

			findPanel.Controls.AddRange(new Control[] {
				closeFind,
				findLabel,
				findText,
				findNext ,
				findPrevious
			});

			methodPanel.Controls.AddRange(new Control[] {
				callees,
				methodSplitter,
				callers,
				findPanel
			});

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
			applicationLabel.Text = "Application:";
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
			Controls.AddRange(new Control[] { rightPanel, mainPanel });
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
			ContainerListViewItem item = new ContainerListViewItem(Path.GetFileNameWithoutExtension(application.Text) + " " + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern));
			item.Tag = run;
			runs.Items.Add(item);
			runs.SelectedItems.Clear();
			runs.SelectedItems.Add(item);
			ShowRun(run);
		}
		public void ShowRun(Run run) {
			callees.Update(run, run.functions);
			callers.Update(run, run.callers);
		}
		private void findNext_Click(object sender, EventArgs e) {
			Find(true, true);
		}
		private void findPrevious_Click(object sender, EventArgs e) {
			Find(false, true);
		}
		[STAThread]
		static void Main(string[] args) {
			string s = Guid.NewGuid().ToString();
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
							callee.stackWalks.Add(new StackWalk(walk.id, walk.length - 1, walk.frames));//.GetRange(0, walk.frames.Count - 1)));
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
			int currentWalk = 0;
			foreach (List<int> stackWalk in stackWalks) {
				currentWalk++;
				for (int i = 0; i < stackWalk.Count; i++) {
					FunctionInfo function = Run.GetFunctionInfo(functions, stackWalk[stackWalk.Count - i - 1]);
					if (function.lastWalk != currentWalk) {
						function.Samples++;
						function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.Count - i - 1, stackWalk));//.GetRange(0, ));
					}
					function.lastWalk = currentWalk;
				}
			}
			// combine with above
			foreach (List<int> reversedWalk in stackWalks) {
				List<int> stackWalk = new List<int>(reversedWalk);
				stackWalk.Reverse();
				currentWalk++;
				for (int i = 0; i < stackWalk.Count; i++) {
					FunctionInfo function = Run.GetFunctionInfo(callers, stackWalk[stackWalk.Count - i - 1]);
					if (function.lastWalk != currentWalk) {
						function.Samples++;
						function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.Count - i - 1, stackWalk));
					}
					function.lastWalk = currentWalk;
				}
			}
			maxSamples = 0;
			foreach (FunctionInfo function in functions.Values) {
				if (function.Samples > maxSamples) {
					maxSamples = function.Samples;
				}
			}
		}
		private string ReadString(BinaryReader br) {
			int length = br.ReadInt32();
			if (length > 2000 || length < 0) {
				byte[] abNextBytes = new byte[8];
				br.Read(abNextBytes, 0, 8);
				string strError = "Length was abnormally large or small (" + length.ToString("x") + ").  Next bytes were ";
				foreach (byte b in abNextBytes)
					strError += b.ToString("x") + " (" + (Char.IsControl((char)b) ? '-' : (char)b) + ") ";

				throw new InvalidOperationException(strError);
			}
			byte[] abString = new byte[length];
			int nRead = 0;
			DateTime dt = DateTime.Now;
			while (nRead < length) {
				nRead += br.Read(abString, nRead, length - nRead);

				// Make this loop finite (30 seconds)
				TimeSpan ts = DateTime.Now - dt;
				if (ts.TotalSeconds > 30)
					throw new InvalidOperationException("Timed out while waiting for length encoded string");
			}
			return System.Text.ASCIIEncoding.ASCII.GetString(abString, 0, length);
		}
		private string FileName {
			get {
				return Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), Profiler.PROFILER_GUID + ".nprof");
				//return "C:\\test.nprof";
			}
		}
		private void ReadStackWalks() {
			using (BinaryReader r = new BinaryReader(File.Open(FileName, FileMode.Open))) {
				while (true) {
					int functionId = r.ReadInt32();
					if (functionId == -1) {
						break;
					}
					signatures[functionId] = new FunctionSignature(
						r.ReadUInt32(),
						ReadString(r)
					);
					//signatures[functionId] = new FunctionSignature(
					//    r.ReadUInt32(),
					//    ReadString(r),
					//    ReadString(r),
					//    ReadString(r),
					//    ReadString(r)
					//);
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
		public Dictionary<int, FunctionSignature> signatures = new Dictionary<int, FunctionSignature>();
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
	public class MethodView : ContainerListView {
		public void MoveTo(int id) {
			SelectedItems.Clear();
			foreach (ContainerListViewItem item in Items) {
				if (((FunctionInfo)item.Tag).ID == id) {
					SelectedItems.Add(item);
					EnsureVisible(item);
					Focus();
					break;
				}
			}
		}
		public void Update(Run run, Dictionary<int, FunctionInfo> functions) {
			currentRun = run;
			SuspendLayout();
			BeginUpdate();
			Items.Clear();
			foreach (FunctionInfo method in SortFunctions(functions.Values)) {
				AddItem(Items, method);
			}
			foreach (ContainerListViewItem item in Items) {
				UpdateView(item);
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
		public MethodView(string name) {
			HeaderStyle = ColumnHeaderStyle.Clickable;
			Columns.Add(name);
			Columns.Add("Inclusive time");
			Columns.Add("Exclusive time");
			Columns[0].Width = 350;
			Columns[1].SortDataType = SortDataType.Double;
			Columns[2].SortDataType = SortDataType.Double;
			this.ShowPlusMinus = true;
			ShowRootTreeLines = true;
			ShowTreeLines = true;
			FullItemSelect = true;
			Columns[1].ContentAlign = ContentAlignment.MiddleRight;
			Columns[2].ContentAlign = ContentAlignment.MiddleRight;
			ColumnSortColor = Color.White;
			Font = new Font("Tahoma", 8.0f);
			this.BeforeExpand += delegate(object sender,ContainerListViewCancelEventArgs e) {
				UpdateView(e.Item);
			};
		}
		void MakeSureComputed(ContainerListViewItem item) {
			MakeSureComputed(item, false);
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
		void UpdateView(ContainerListViewItem item) {
			SuspendLayout();
			BeginUpdate();
			MakeSureComputed(item, true);
			EndUpdate();
			ResumeLayout();
		}
		private ContainerListViewItem AddItem(ContainerListViewItemCollection parent, FunctionInfo function) {
			ContainerListViewItem item = parent.Add(currentRun.signatures[function.ID].Signature);
			item.SubItems[1].Text = ((((double)function.Samples) / (double)currentRun.maxSamples) * 100.0).ToString("0.00;-0.00;0.00");
			int childSamples = 0;
			foreach (FunctionInfo f in function.Children.Values) {
				childSamples += f.Samples;
			}
			item.SubItems[2].Text = ((((double)function.Samples-childSamples) / (double)currentRun.maxSamples) * 100.0).ToString("0.00;-0.00;0.00");
			item.Tag = function;
			return item;
		}
		public void AddFunctionItem(ContainerListViewItemCollection parent, FunctionInfo function) {
			ContainerListViewItem item = AddItem(parent, function);
			foreach (FunctionInfo callee in function.Children.Values) {
				AddItem(item.Items, callee);
			}
		}
		public void Add(FunctionInfo function) {
			AddFunctionItem(Items, function);
		}
	}
	public class Profiler {
		public const string PROFILER_GUID = "029C3A01-70C1-46D2-92B7-24B157DF55CE";
		public static string Version {
			get { return "0.10.1"; }
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
	public class FunctionSignature {
		public FunctionSignature(UInt32 methodAttributes, string signature) {
			//int index = Math.Max(0, className.LastIndexOf('.') + 1);
			//string classOnly = className.Substring(index);
			//NameSpace = className.Substring(0, Math.Max(0, index - 1));
			this.Signature = signature;//classOnly + "." + functionName + "(" + parameters + ")";
		}
		public string NameSpace;
		public string Signature;
	}
	//public class FunctionSignature {
	//    public FunctionSignature(UInt32 methodAttributes, string returnType, string className, string functionName, string parameters) {
	//        int index = Math.Max(0, className.LastIndexOf('.') + 1);
	//        string classOnly = className.Substring(index);
	//        NameSpace = className.Substring(0, Math.Max(0,index-1));
	//        this.Signature = classOnly+ "." + functionName + "(" + parameters + ")";
	//        //this.Signature = className + "." + functionName + "(" + parameters + ")";
	//        //this.Signature = className + "." + functionName + "(" + parameters + ")";
	//    }
	//    public string NameSpace;
	//    public string Signature;
	//}
}