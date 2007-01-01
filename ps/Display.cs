using System;
using System.Reflection;
using System.Collections.Generic;

namespace ps
{
	abstract class Display
	{
		protected Processes processes;
		protected string outputColumns;
		protected List<Column> columns = new List<Column>();

		protected Display()
		{
		}

		public abstract void DisplayProcesses();

		protected void getColumns()
		{
			foreach (string name in outputColumns.Split(','))
			{
				if (Process.GetPropertyInfo(name) != null)
				{
					columns.Add(new Column(name, 0));
				}
			}
		}

		protected string getReplacedOutput(string output)
		{
			return output.Replace("\\t", "\t").Replace("\\n", "\n").Replace("\\r", "\r");
		}

		protected string getTemplatedOutput(Process process, string template)
		{
			string output = template;
			string[] columns = outputColumns.Split(',');

			for (int i = 0; i < columns.Length; i++)
			{
				string column = columns[i].Trim();

				output = output.Replace("{" + i + "}", process[column]);
			}

			return getReplacedOutput(output);
		}
	}
}
