using System;

namespace ps
{
	abstract class DisplayBase
	{
		protected Processes processes;

		protected DisplayBase()
		{
		}

		public DisplayBase(Processes processes)
		{
			this.processes = processes;
		}

		public abstract void DisplayProcesses();
	}
}
