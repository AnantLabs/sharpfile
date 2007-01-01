using System;
using System.Collections.Generic;

namespace ps
{
	/// <summary>
	/// Summary information for Program.
	/// </summary>
	class Program
	{
		// Settings.
		private static bool isTreeDisplay = false;
		private static bool isTableDisplay = true;
		private static string treeDelimiter = "  ";
		private static string outputColumns = "Name,IDProcess,PercentProcessorTime,WorkingSet";
		private static string template = "{0}\t{1}\t{2}\t{3}";
		private static int waitInterval = 1000;

		// Member variables.
		private static string query = "SELECT * FROM Win32_PerfRawData_PerfProc_Process WHERE Name <> '_Total'";
		private static string queryWhereClause = string.Empty;
		
		// TODO: Sort when neccessary by this.
		private static string queryOrderClause = "PercentProcessorTime DESC";
		private static Processes processes;
		private static List<Column> columns = new List<Column>();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			DateTime startTime = DateTime.Now;
			getSettings(args);

			// TODO: This should be used... passed into display or something. Maybe.
			//getColumns();

			//foreach (Column column in columns)
			//{
			//    if (waitInterval > 0
			//            && propertyInfo.IsCalculatedProperty)
			//    {
			//        waitInterval = 0;
			//    }
			//}

			// TODO: If ouputcolumns contains a value in Process.CalculatedProperties, then set the waitInterval to 0
			if (!outputColumns.Contains(Process.PERCENT_PROCESSOR_TIME))
			{
				waitInterval = 0;
			}
			
			query = query + queryWhereClause;

			processes = new Processes();
			processes.Populate(waitInterval, query);
			displayProcesses();

			Console.WriteLine("Time to collect data: " + (((TimeSpan)DateTime.Now.Subtract(startTime)).TotalMilliseconds - waitInterval) + " ms");
		}

		private static void getColumns()
		{
			foreach (string name in outputColumns.Split(','))
			{
				PropertyInfo propertyInfo = Process.GetPropertyInfo(name);

				if (propertyInfo != null)
				{
					columns.Add(new Column(name, 0));
				}
			}
		}
		
		private static void getSettings(string[] args)
		{
			if (args != null && 
			    args.Length > 0)
			{
				int argIndex = 0;

				foreach (string arg in args)
				{
					switch (arg)
					{
						case "-f":
							queryWhereClause = " WHERE Name LIKE '" + args[argIndex + 1] + "%'";
							break;
						case "-t":
							isTreeDisplay = true;
							break;
						case "-n":
							isTableDisplay = false;

							if (args.Length > argIndex + 1
									&& !string.IsNullOrEmpty(args[argIndex + 1])
									&& !args[argIndex + 1].StartsWith("\"-"))
							{
								template = args[argIndex + 1].Trim();
							}

							break;
						case "-w":
							waitInterval = int.Parse(args[argIndex + 1]);
							break;
						case "-o":
							queryOrderClause = args[argIndex + 1];
							break;
						case "-d":
							treeDelimiter = args[argIndex + 1];
							break;
						case "-c":
							outputColumns = args[argIndex + 1];
							break;
					}

					argIndex++;
				}
			}
		}

		private static void displayProcesses()
		{
			Display display;
			processes.Sort(Process.PERCENT_PROCESSOR_TIME, SortType.DESC);	

			if (isTreeDisplay)
			{
				display = new Displays.DisplayTree(processes, outputColumns, treeDelimiter, template);
			}
			else if (isTableDisplay)
			{
				display = new Displays.DisplayTable(processes, outputColumns);
			}
			else
			{
				display = new Displays.DisplayNormal(processes, outputColumns, template);
			}

			display.DisplayProcesses();
		}
	}
}
