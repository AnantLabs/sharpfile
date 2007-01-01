using System;
using System.Collections;
using System.Collections.Generic;
using Common;

namespace ps.Displays
{
	class DisplayTable : Display
	{
		public DisplayTable(Processes processes, string outputColumns)
		{
			this.processes = processes;
			this.outputColumns = outputColumns;

			getColumns();
		}

		public override void DisplayProcesses()
		{
			getColumnLengths();
			writeHeader();

			if (processes != null)
			{
				processes.ForEach(delegate(Process process)
				                  	{
				                  		columns.ForEach(delegate(Column column)
				                  		                	{
				                  		                		// TODO: Fix this so it's not lame.
																PropertyInfo propertyInfo = Process.GetPropertyInfo(column.Name);

																if (propertyInfo.Type == typeof(long)
																	&& !propertyInfo.IsCalculatedProperty)
				                  		                		{
				                  		                			string output = General.GetHumanReadableSize(Convert.ToInt64(process[column.Name]));
				                  		                			writePaddedOutput(output, column.ValueLength);
				                  		                		}
				                  		                		else
				                  		                		{
				                  		                			writePaddedOutput(process[column.Name], column.ValueLength);
				                  		                		}
				                  		                	});

				                  		Console.WriteLine();
				                  	});
			}
		}

		private void getColumnLengths()
		{
			processes.ForEach(delegate(Process p)
			{
				columns.ForEach(delegate(Column column)
				{
					if (p[column.Name].Length > column.ValueLength)
					{
						column.ValueLength = p[column.Name].Length + 1;
					}
				});
			});
		}

		private void writeHeader()
		{
			columns.ForEach(delegate(Column column)
			{
				writePaddedOutput(column.AbbreviatedName, column.ValueLength);
			});
			Console.WriteLine();

			columns.ForEach(delegate(Column column)
			{
				writePaddedOutput(string.Empty, column.ValueLength, '-');
			});
			Console.WriteLine();
		}

		private void writePaddedOutput(string output, int columnWidth)
		{
			writePaddedOutput(output, columnWidth, ' ');
		}

		private void writePaddedOutput(string output, int columnWidth, char paddingChar)
		{
			Console.Write(output.PadRight(columnWidth, paddingChar));
		}
	}
}
