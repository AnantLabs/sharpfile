using System;
using System.Collections.Generic;
using System.Text;

namespace ps.Displays
{
	class DisplayTree : Display
	{
		private string treeDelimiter = "  ";
		private string template = "{0}\t{1}\t{2}\t{3}";

		public DisplayTree(Processes processes, string outputColumns, string treeDelimiter, string template)
		{
			this.processes = processes;
			this.outputColumns = outputColumns;
			this.treeDelimiter = treeDelimiter;
			this.template = template;
		}

		public override void DisplayProcesses()
		{
			Process idleProcess = processes[Process.IDLE];

			if (idleProcess != null)
			{
				Console.WriteLine(idleProcess.Name + "\t" + idleProcess.IDProcess);
			}

			Process systemProcess = processes[Process.SYSTEM];

			if (systemProcess != null)
			{
				Console.WriteLine(systemProcess.Name + "\t" + systemProcess.IDProcess);

				getChildProcesses(systemProcess.IDProcess, 1);
			}

			foreach (Process process in processes)
			{
				Processes foundProcesses = processes.GetProcesses(process.CreatingProcessID);

				if (foundProcesses.Count == 0)
				{
					Console.WriteLine(process.Name + "\t" + process.IDProcess);

					getChildProcesses(process.IDProcess, 1);
				}
			}
		}

		private void getChildProcesses(int pid, int processDepth)
		{
			Processes foundProcesses = processes.GetChildProcesses(pid);

			if (foundProcesses != null)
			{
				foundProcesses.ForEach(delegate(Process process)
				{
					for (int i = 0; i < processDepth; i++)
					{
						Console.Write(treeDelimiter);
					}

					Console.WriteLine(getTemplatedOutput(process, template));

					getChildProcesses(process.IDProcess, processDepth + 1);
				});
			}
		}
	}
}
