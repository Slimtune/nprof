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
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;


namespace NProf_WPF {

	public partial class NProf : System.Windows.Window {
		public string NProfTitle {
			get {
				return "NProf " + Profiler.Version;
			}
		}
		//public void AddRun(Run run) {
		//    int count = 1;
		//    string text = Path.GetFileNameWithoutExtension(application.Text);
		//    foreach (ContainerListViewItem i in runs.Items) {
		//        if (i.Text.StartsWith(text)) {
		//            count++;
		//        }
		//    }
		//    ContainerListViewItem item = new ContainerListViewItem(Path.GetFileNameWithoutExtension(application.Text) + " " + count);
		//    item.Tag = run;
		//    runs.Items.Add(item);
		//    runs.SelectedItems.Clear();
		//    runs.SelectedItems.Add(item);
		//    ShowRun(run);
		//}
		//public void ShowRun(Run run) {
		//    callees.Update(run, run.functions);
		//    callers.Update(run, run.callers);
		//}
		//private void findNext_Click(object sender, EventArgs e) {
		//    Find(true, true);
		//}
		//private void findPrevious_Click(object sender, EventArgs e) {
		//    Find(false, true);
		//}
		//public void Find(bool forward, bool step) {
		//    if (callers.SelectedItems.Count != 0) {
		//        callers.Find(findText.Text, forward, step);
		//    }
		//    else {
		//        callees.Find(findText.Text, forward, step);
		//    }
		//}
		public ListView runs;
		private MethodView callees;
		private MethodView callers;
		private Profiler profiler;
		private TextBox findText;
		private StackPanel findPanel;
		public static TextBox executable;
		public static TextBox arguments;
		public MenuItem MenuItem(string header,object[] items) {
			MenuItem item=new MenuItem();
			item.Header=header;
			item.ItemsSource=items;
			return item;
		}
		public MenuItem MenuItem(string header,RoutedEventHandler e) {
			MenuItem item=new MenuItem();
			item.Header=header;
			item.Click+=e;
			return item;
		}
		public static NProf form;// = new NProf();
		public NProf() {
			form = this;
			InitializeComponent();

			//this.Icon = new Icon(this.GetType().Assembly.GetManifestResourceStream("NProf.Resources.app-icon.ico"));
			this.Title= NProfTitle;
			profiler = new Profiler();

			runs = new ListView();
			//ContainerListViewColumnHeader header = new ContainerListViewColumnHeader("Profiling runs", 90);
			//runs.Columns.Add(header);
			//runs.SizeChanged += delegate {
			//    header.Width = runs.Size.Width - 5;
			//};
			//runs.AllowMultiSelect = true;
			//runs.Dock = DockStyle.Left;
			//runs.Width = 100;
			//runs.DoubleClick += delegate {
			//    if (runs.SelectedItems.Count != 0) {
			//        ShowRun((Run)runs.SelectedItems[0].Tag);
			//    }
			//};

			callers = new MethodView("Caller methods");
			//callers.Size = new System.Drawing.Size(100, 200);
			//callers.Dock = DockStyle.Bottom;
			//callers.DoubleClick += delegate {
			//    if (callers.SelectedItems.Count != 0) {
			//        callees.MoveTo(((FunctionInfo)callers.SelectedItems[0].Tag).ID);
			//    }
			//};

			callees = new MethodView("Callee methods");
			//callees.Size = new Size(100, 100);
			//callees.Dock = DockStyle.Fill;
			//callees.GotFocus += delegate {
			//    callers.SelectedItems.Clear();
			//};
			//callees.DoubleClick += delegate {
			//    if (callees.SelectedItems.Count != 0) {
			//        callers.MoveTo(((FunctionInfo)callees.SelectedItems[0].Tag).ID);
			//    }
			//};
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
			findText = new TextBox();
			//findText.TextChanged += delegate {
			//    Find(true, false);
			//};
			//findText.KeyDown += delegate(object sender, KeyEventArgs e) {
			//    if (e.KeyCode == Keys.Enter) {
			//        Find(true, true);
			//        e.Handled = true;
			//    }
			//};
			Menu menu = new Menu();
			menu.ItemsSource = new MenuItem[] {
				MenuItem(
					"File",
					new object[] {
						MenuItem(
							"New",
							delegate
							{
								runs.Items.Clear();
								NProf.arguments.Text = "";
								NProf.executable.Text = "";
								callees.Items.Clear();
								callers.Items.Clear();
							}
						),
						new Separator(),
						MenuItem("Exit",delegate {Close();})
					}
				),
				MenuItem(
		            "Project",
		            new object[] {
		                MenuItem(
		                    "Start",
		                    delegate{StartRun();}
						),
		                new Separator(),
		                MenuItem("Find",delegate{ShowSearch();})
		            }
				)
		};
			//Panel rightPanel = new Panel();
			//rightPanel.Dock = DockStyle.Fill;
			//Panel methodPanel = new Panel();
			//methodPanel.Size = new Size(100, 100);
			//methodPanel.Dock = DockStyle.Fill;

			//Splitter methodSplitter = new Splitter();
			//methodSplitter.Dock = DockStyle.Bottom;
			//findPanel = new FlowLayoutPanel();
			//findPanel.BorderStyle = BorderStyle.FixedSingle;
			//findPanel.Visible = false;
			//findPanel.WrapContents = false;
			//findPanel.AutoSize = true;
			//findPanel.Dock = DockStyle.Top;
			//Button closeFind = new Button();
			//closeFind.Text = "x";
			//closeFind.Width = 17;
			//closeFind.Height = 20;
			//closeFind.TextAlign = ContentAlignment.BottomLeft;
			//closeFind.Click += delegate {
			//    findPanel.Visible = false;
			//};

			//Button findNext = new Button();
			//findNext.AutoSize = true;
			//findNext.Text = "Next";
			//findNext.Click += new EventHandler(findNext_Click);

			//Label findLabel = new Label();
			//findLabel.Text = "Find:";
			//findLabel.Dock = DockStyle.Fill;
			//findLabel.TextAlign = ContentAlignment.MiddleLeft;
			//findLabel.AutoSize = true;

			//Button findPrevious = new Button();
			//findPrevious.AutoSize = true;
			//findPrevious.FlatAppearance.BorderSize = 0;
			//findPrevious.Click += new EventHandler(findPrevious_Click);
			//findPrevious.Text = "Previous";

			//findPanel.Controls.AddRange(new Control[] {closeFind,findLabel,findText,findNext ,findPrevious});
			//methodPanel.Controls.AddRange(new Control[] {callees,methodSplitter,callers,findPanel});

			//Splitter mainSplitter = new Splitter();
			//mainSplitter.Dock = DockStyle.Left;
			//rightPanel.Controls.AddRange(new Control[] { methodPanel, mainSplitter, runs });

			StackPanel options = new StackPanel();
			executable = new TextBox();
			executable.Width = 300;
			arguments = new TextBox();

			Label applicationLabel = new Label();
			applicationLabel.Content = "Executable:";
			Button browse = new Button();
			browse.Content = "Browse...";
			browse.TabIndex = 0;
			browse.Focus();
			browse.Click += delegate {
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "Executable files (*.exe)|*.exe";
				if (dialog.ShowDialog() ==true) {
					executable.Text = dialog.FileName;
					executable.Focus();
					executable.SelectAll();
				}
			};
			StackPanel argumentsPanel = new StackPanel();
			argumentsPanel.Orientation = Orientation.Horizontal;

			arguments.Width = 300;
			Label argumentLabel = new Label();
			argumentLabel.Content = "Arguments:";
			argumentsPanel.Children.Add(argumentLabel);
			argumentsPanel.Children.Add(arguments);
			StackPanel executablePanel = new StackPanel();
			executablePanel.Orientation = Orientation.Horizontal;
			executablePanel.Children.Add(applicationLabel);
			executablePanel.Children.Add(executable);
			executablePanel.Children.Add(browse);
			options.Children.Add(executablePanel);
			options.Children.Add(argumentsPanel);
			StackPanel panel = new StackPanel();
			panel.Children.Add(menu);
			panel.Children.Add(options);
			StackPanel mainPanel=new StackPanel();
			mainPanel.Orientation = Orientation.Horizontal;
			runs.Width = 200;
			runs.Height = 600;
			mainPanel.Children.Add(runs);
			StackPanel info = new StackPanel();
			info.Orientation = Orientation.Vertical;
			callees.Width = 500;
			callees.Height = 300;
			callers.Width = 500;
			callers.Height = 300;
			info.Children.Add(callees);
			info.Children.Add(callers);
			mainPanel.Children.Add(info);
			panel.Children.Add(mainPanel);

			this.Content = panel;
			executable.TextChanged += delegate {
				Title = System.IO.Path.GetFileNameWithoutExtension(executable.Text) + " - " + NProfTitle;
			};
			this.Width = 800;
			this.Height = 600;
			//this.Load += delegate {
			//    Size = new Size(800, 600);
			//};
		}
		private void StartRun() {
			Run run = new Run(profiler);
			run.profiler.completed = new EventHandler(run.Complete);
			run.Start();
		}
		public void ShowSearch() {
			if (findPanel.Visibility == Visibility.Hidden) {
				findPanel.Visibility = Visibility.Visible;
			}
			else {
				findPanel.Visibility = Visibility.Hidden;
			}
			findText.Focus();
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
				Interprete(functions, false);
				Interprete(callers, true);
				maxSamples = 0;
				foreach (FunctionInfo function in functions.Values) {
					if (function.Samples > maxSamples) {
						maxSamples = function.Samples;
					}
				}
			}
			private void Interprete2(int currentWalk, List<int> stackWalk, Dictionary<int, FunctionInfo> map) {
				for (int i = 0; i < stackWalk.Count; i++) {
					FunctionInfo function = Run.GetFunctionInfo(map, stackWalk[stackWalk.Count - i - 1]);
					if (function.lastWalk != currentWalk) {
						function.Samples++;
						function.stackWalks.Add(new StackWalk(currentWalk, stackWalk.Count - i - 1, stackWalk));
					}
					function.lastWalk = currentWalk;
				}
			}
			private void Interprete(Dictionary<int, FunctionInfo> map, bool reverse) {
				int currentWalk = 0;
				foreach (List<int> stackWalk in stackWalks) {
					currentWalk++;
					if (reverse) {
						stackWalk.Reverse();
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
					return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetTempFileName()), Profiler.PROFILER_GUID + ".nprof");
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
				//NProf.form.BeginInvoke(new EventHandler(delegate {
				//    InterpreteData();
				//    NProf.form.AddRun(this);
				//}));
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
		public class MethodView : ListView {
			//public void MoveTo(int id) {
			//    SelectedItems.Clear();
			//    foreach (ContainerListViewItem item in Items) {
			//        if (((FunctionInfo)item.Tag).ID == id) {
			//            SelectedItems.Add(item);
			//            EnsureVisible(item);
			//            Focus();
			//            break;
			//        }
			//    }
			//}
			//public void Update(Run run, Dictionary<int, FunctionInfo> functions) {
			//    currentRun = run;
			//    SuspendLayout();
			//    BeginUpdate();
			//    Items.Clear();
			//    foreach (FunctionInfo method in SortFunctions(functions.Values)) {
			//        AddItem(Items, method);
			//    }
			//    foreach (ContainerListViewItem item in Items) {
			//        UpdateView(item);
			//    }
			//    EndUpdate();
			//    ResumeLayout();
			//}
			//public void Find(string text, bool forward, bool step) {
			//    if (text != "") {
			//        ContainerListViewItem item;
			//        if (SelectedItems.Count == 0) {
			//            if (Items.Count == 0) {
			//                item = null;
			//            }
			//            else {
			//                item = Items[0];
			//            }
			//        }
			//        else {
			//            if (step) {
			//                if (forward) {
			//                    item = SelectedItems[0].NextVisibleItem;
			//                }
			//                else {
			//                    item = SelectedItems[0].PreviousVisibleItem;
			//                }
			//            }
			//            else {
			//                item = SelectedItems[0];
			//            }
			//        }
			//        ContainerListViewItem firstItem = item;
			//        while (item != null) {
			//            if (item.Text.ToLower().Contains(text.ToLower())) {
			//                SelectedItems.Clear();
			//                item.Focused = true;
			//                item.Selected = true;
			//                this.Invalidate();
			//                break;
			//            }
			//            else {
			//                if (forward) {
			//                    item = item.NextVisibleItem;
			//                }
			//                else {
			//                    item = item.PreviousVisibleItem;
			//                }
			//                if (item == null) {
			//                    if (forward) {
			//                        item = Items[0];
			//                    }
			//                    else {
			//                        item = Items[Items.Count - 1];
			//                    }
			//                }
			//            }
			//            if (item == firstItem) {
			//                break;
			//            }
			//        }
			//        if (item != null) {
			//            EnsureVisible(item);
			//        }
			//    }
			//}
			//public Run currentRun;
			public MethodView(string name) {
				//HeaderStyle = ColumnHeaderStyle.Clickable;
				//Columns.Add(name);
				//Columns.Add("Inclusive time");
				//Columns.Add("Exclusive time");
				//Columns[0].Width = 350;
				//Columns[1].SortDataType = SortDataType.Double;
				//Columns[2].SortDataType = SortDataType.Double;
				//Columns[1].ToolTip = "Percentage of time spent in method and its children";
				//Columns[2].ToolTip = "Percentage of time spent in method";
				//this.ShowPlusMinus = true;
				//ShowRootTreeLines = true;
				//ShowTreeLines = true;
				//FullItemSelect = true;
				//Columns[1].ContentAlign = ContentAlignment.MiddleRight;
				//Columns[2].ContentAlign = ContentAlignment.MiddleRight;
				//ColumnSortColor = Color.White;
				//Font = new Font("Tahoma", 8.0f);
				//this.BeforeExpand += delegate(object sender, ContainerListViewCancelEventArgs e) {
				//    UpdateView(e.Item);
				//};
			}
			//void MakeSureComputed(ContainerListViewItem item) {
			//    MakeSureComputed(item, false);
			//}
			//public List<FunctionInfo> SortFunctions(IEnumerable<FunctionInfo> f) {
			//    List<FunctionInfo> functions = new List<FunctionInfo>(f);
			//    functions.Sort(delegate(FunctionInfo a, FunctionInfo b) {
			//        return b.Samples.CompareTo(a.Samples);
			//    });
			//    return functions;
			//}
			//void MakeSureComputed(ContainerListViewItem item, bool parentExpanded) {
			//    if (item.Items.Count == 0) {
			//        foreach (FunctionInfo function in SortFunctions(((FunctionInfo)item.Tag).Children.Values)) {
			//            AddFunctionItem(item.Items, function);
			//        }
			//    }
			//    if (item.Expanded || parentExpanded) {
			//        foreach (ContainerListViewItem subItem in item.Items) {
			//            MakeSureComputed(subItem, item.Expanded);
			//        }
			//    }
			//}
			//void UpdateView(ContainerListViewItem item) {
			//    SuspendLayout();
			//    BeginUpdate();
			//    MakeSureComputed(item, true);
			//    EndUpdate();
			//    ResumeLayout();
			//}
			//private ContainerListViewItem AddItem(ContainerListViewItemCollection parent, FunctionInfo function) {
			//    ContainerListViewItem item = parent.Add(currentRun.signatures[function.ID].Signature);
			//    item.SubItems[1].Text = ((((double)function.Samples) / (double)currentRun.maxSamples) * 100.0).ToString("0.00;-0.00;0.00");
			//    int childSamples = 0;
			//    foreach (FunctionInfo f in function.Children.Values) {
			//        childSamples += f.Samples;
			//    }
			//    item.SubItems[2].Text = ((((double)function.Samples - childSamples) / (double)currentRun.maxSamples) * 100.0).ToString("0.00;-0.00;0.00");
			//    item.Tag = function;
			//    return item;
			//}
			//public void AddFunctionItem(ContainerListViewItemCollection parent, FunctionInfo function) {
			//    ContainerListViewItem item = AddItem(parent, function);
			//    foreach (FunctionInfo callee in function.Children.Values) {
			//        AddItem(item.Items, callee);
			//    }
			//}
			//public void Add(FunctionInfo function) {
			//    AddFunctionItem(Items, function);
			//}
		}
		public class Profiler {
			public const string PROFILER_GUID = "029C3A01-70C1-46D2-92B7-24B157DF55CE";
			public static string Version {
				get { return "0.10.1"; }
			}
			public EventHandler completed;
			public bool Start(Run run) {
				this.run = run;
				process = new Process();
				process.StartInfo = new ProcessStartInfo(NProf.executable.Text, NProf.arguments.Text);
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
}