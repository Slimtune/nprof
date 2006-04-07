using System;
using System.Collections.Generic;
using System.Text;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Project;
using NProf.Utilities.DataStore;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using NProf.Glue.Profiler.Info;
using System.Windows.Forms;


namespace NProf.Test
{
	public class Tests
	{
		public static void Main()
		{
			new Tests().RunTests();
		}
		public void RunTests()
		{
			bool allTestsSucessful = true;
			foreach (Type testType in this.GetType().GetNestedTypes())
			{
				if(testType.IsSubclassOf(typeof(TestCase)))
				{
					TestCase test = (TestCase)testType.GetConstructor(new Type[] { }).Invoke(null);
					Console.WriteLine(testType.Name + "...");


					DateTime startTime = DateTime.Now;
					string result = test.GetResult();
					TimeSpan duration = DateTime.Now - startTime;


					string testDirectory = Path.Combine(Path.Combine(NProfDirectory,@"NProf.Test\Tests"), testType.Name);

					string resultPath = Path.Combine(testDirectory, "result.txt");
					string checkPath = Path.Combine(testDirectory, "check.txt");

					Directory.CreateDirectory(testDirectory);
					if (!File.Exists(checkPath))
					{
						File.Create(checkPath).Close();
					}

					File.WriteAllText(resultPath, result.ToString(), Encoding.Default);
					string successText;
					if (!File.ReadAllText(resultPath).Equals(File.ReadAllText(checkPath)))
					{
						successText = "failed";
						allTestsSucessful = false;
					}
					else
					{
						successText = "succeeded";
					}
					Console.WriteLine(testType.Name+"  " + successText + "  " + duration.TotalSeconds.ToString() + " s");
				}
			}
			if (!allTestsSucessful)
			{
				Console.ReadLine();
			}
		}
		private static string NProfDirectory
		{
			get
			{
				return new DirectoryInfo(Application.StartupPath).Parent.Parent.Parent.FullName;
			}
		}
		public abstract class TestCase
		{
			public abstract string GetResult();
		}
		public class TestProfilee : TestCase
		{
			public override string GetResult()
			{
				XmlSerializer xsRun = new XmlSerializer(typeof(Run));
				ProjectInfo project = new ProjectInfo(ProjectType.File);
				Profiler profiler = new Profiler();
				//project.ApplicationName = @"C:\Meta\0.2\Meta.exe";// Path.Combine(NProfDirectory, @"TestProfilee\bin\Debug\TestProfilee.exe");
				//project.Arguments = "-test";
				project.ApplicationName = Path.Combine(NProfDirectory, @"TestProfilee\bin\Debug\TestProfilee.exe");
				project.ApplicationName = @"C:\nprof\trunk\nprof\TestProfilee\bin\Debug\TestProfilee.exe";

				Run run = project.CreateRun(profiler);
				run.StateChanged += new Run.RunStateEventHandler(run_StateChanged);
				run.Start();

				while (result == null)
				{
					Thread.Sleep(100);
				}
				return result;
			}
			private string result=null;
			void run_StateChanged(Run run, Run.RunState rsOld, Run.RunState rsNew)
			{
				if (rsNew == Run.RunState.Finished)
				{
					XmlAttributeOverrides overrides = new XmlAttributeOverrides();
					XmlAttributes ignore = new XmlAttributes();
					ignore.XmlIgnore = true;
					overrides.Add(typeof(Run), "Messages", ignore);
					overrides.Add(typeof(Run), "StartTime", ignore);
					overrides.Add(typeof(Run), "EndTime", ignore);
					overrides.Add(typeof(ProcessInfo), "ProcessID", ignore);
					overrides.Add(typeof(ThreadInfo), "StartTime", ignore);
					overrides.Add(typeof(ThreadInfo), "EndTime", ignore);
					overrides.Add(typeof(CalleeFunctionInfo), "TotalTime", ignore);
					overrides.Add(typeof(CalleeFunctionInfo), "TotalRecursiveTime", ignore);
					XmlSerializer xsRun = new XmlSerializer(typeof(Run), overrides);
					StringBuilder builder = new StringBuilder();
					xsRun.Serialize(new StringWriter(builder), run);
					result = builder.ToString();
				}
			}
		}
	}
}
