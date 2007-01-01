using System;
using System.Collections.Generic;
using System.Text;

namespace ps.Displays
{
	class DisplayNormal : Display
	{
		private string template = "{0}\t{1}\t{2}\t{3}";

		public DisplayNormal(Processes processes, string outputColumns, string template)
		{
			this.processes = processes;
			this.outputColumns = outputColumns;
			this.template = template;
		}

		public override void DisplayProcesses()
		{
			processes.ForEach(delegate(Process process)
			{
				Console.WriteLine(getTemplatedOutput(process, template));
			});
		}
	}
}
