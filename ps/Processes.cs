using System;
using System.Management;
using System.Collections.Generic;

namespace ps
{
	/// <summary>
	/// Summary description for Processes
	/// </summary>
	class Processes
	{
		private List<Process> list;
		private string machinename;
		private string username;
		private string password;
		private string query;
		private int waitInterval = 1000;

		public Processes()
		{
			this.machinename = Environment.MachineName;
		}

		public Processes(string machinename)
		{
			this.machinename = machinename;
		}

		public Processes(string username, string password)
		{
			this.machinename = Environment.MachineName;
			this.username = username;
			this.password = password;
		}

		public Processes(string machinename, string username, string password)
		{
			this.machinename = machinename;
			this.username = username;
			this.password = password;
		}		

		private Processes(List<Process> list)
		{
			this.list = list;
		}
		
		public void Populate(int waitInterval, string query)
		{
			this.waitInterval = waitInterval;
			Populate(query);
		}
		
		public void Populate(string query)
		{
			this.query = query;
			getProcesses();
		}

		#region Inherited stuff from generics
		public void Add(Process process)
		{
			this.list.Add(process);
		}

		public Process Find(Predicate<Process> match)
		{
			return this.list.Find(match);
		}

		public Processes FindAll(Predicate<Process> match)
		{
			return new Processes(this.list.FindAll(match));
		}

		public void Sort(string property, SortType sortType)
		{
			switch (property)
			{
				case Process.PERCENT_PROCESSOR_TIME:
					if (sortType == SortType.DESC)
					{
						list.Sort(delegate(Process x, Process y)
						{
							return Comparer<long>.Default.Compare(y.PercentProcessorTime, x.PercentProcessorTime);
						});
					}
					else if (sortType == SortType.ASC)
					{
						list.Sort(delegate(Process x, Process y)
						{
							return Comparer<long>.Default.Compare(x.PercentProcessorTime, y.PercentProcessorTime);
						});
					}

					break;
			}
		}

		public IEnumerator<Process> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void ForEach(Action<Process> action)
		{
			this.list.ForEach(action);
		}
		#endregion

		public Process this[string name]
		{
			get
			{
				return this.list.Find(delegate(Process p)
				{
					if (p.Name.Equals(name))
					{
						return true;
					}

					return false;
				});
			}
		}

		public Process this[int pid]
		{
			get
			{
				return this.list.Find(delegate(Process p)
				{
					if (p.IDProcess.Equals(pid))
					{
						return true;
					}

					return false;
				});
			}
		}

		public Process this[int pid, string name]
		{
			get
			{
				return this.list.Find(delegate(Process p)
				{
					if (p.Id.Equals(pid + "_" + name))
					{
						return true;
					}

					return false;
				});
			}
		}

		public Processes GetProcesses(int pid)
		{
			List<Process> matches = this.list.FindAll(delegate(Process p)
																	{
																		if (p.IDProcess.Equals(pid))
																		{
																			return true;
																		}

																		return false;
																	});

			return new Processes(matches);
		}

		public Processes GetChildProcesses(int pid)
		{
			List<Process> children = this.list.FindAll(delegate(Process p)
																	{
																		if (p.CreatingProcessID.Equals(pid))
																		{
																			return true;
																		}

																		return false;
																	});

			return new Processes(children);
		}

		#region getProcesses
		private ManagementScope setScope()
		{
			ManagementScope scope = new ManagementScope();

			if (!string.IsNullOrEmpty(this.username)
				&& !string.IsNullOrEmpty(this.password))
			{
				scope.Options.Username = username;
				scope.Options.Password = password;
			}

			// TODO: Use machinename in the path. I think.
			//scope.Path = ?

			return scope;
		}

		private void getProcesses()
		{
			// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/wmisdk/wmi/win32_perfrawdata_perfproc_process.asp
			this.list = new List<Process>();
			int numberOfCpus = Environment.ProcessorCount;

			ManagementScope scope = setScope();

			// Get process info.
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new WqlObjectQuery(query));

			foreach (ManagementBaseObject mbo in searcher.Get())
			{
				Process process = new Process();

				process.Caption = (mbo.Properties[Process.CAPTION] != null && mbo.Properties[Process.CAPTION].Value != null) ? mbo.Properties[Process.CAPTION].Value.ToString() : string.Empty;
				process.CreatingProcessID = Convert.ToInt32(mbo.Properties[Process.CREATING_PROCESS_ID].Value);
				process.Description = (mbo.Properties[Process.DESCRIPTION] != null && mbo.Properties[Process.DESCRIPTION].Value != null) ? mbo.Properties[Process.DESCRIPTION].Value.ToString() : string.Empty;
				process.ElapsedTime = Convert.ToInt64(mbo.Properties[Process.ELAPSED_TIME].Value);
				process.Frequency_Object = Convert.ToInt64(mbo.Properties[Process.FREQUENCY_OBJECT].Value);
				process.Frequency_PerfTime = Convert.ToInt64(mbo.Properties[Process.FREQUENCY_PERF_TIME].Value);
				process.Frequency_Sys100NS = Convert.ToInt64(mbo.Properties[Process.FREQUENCY_SYS_100_NS].Value);
				process.HandleCount = Convert.ToInt32(mbo.Properties[Process.HANDLE_COUNT].Value);
				process.IDProcess = Convert.ToInt32(mbo.Properties[Process.ID_PROCESS].Value);
				process.IODataBytesPerSec = Convert.ToInt64(mbo.Properties[Process.IO_DATA_BYTES_PER_SEC].Value);
				process.IODataOperationsPerSec = Convert.ToInt64(mbo.Properties[Process.IO_DATA_OPERATIONS_PER_SEC].Value);
				process.IOOtherBytesPerSec = Convert.ToInt64(mbo.Properties[Process.IO_OTHER_BYTES_PER_SEC].Value);
				process.IOOtherOperationsPerSec = Convert.ToInt64(mbo.Properties[Process.IO_OTHER_OPERATIONS_PER_SEC].Value);
				process.IOReadBytesPerSec = Convert.ToInt64(mbo.Properties[Process.IO_READ_BYTES_PER_SEC].Value);
				process.IOReadOperationsPerSec = Convert.ToInt64(mbo.Properties[Process.IO_READ_OPERATIONS_PER_SEC].Value);
				process.IOWriteBytesPerSec = Convert.ToInt64(mbo.Properties[Process.IO_WRITE_BYTES_PER_SEC].Value);
				process.IOWriteOperationsPerSec = Convert.ToInt64(mbo.Properties[Process.IO_WRITE_OPERATIONS_PER_SEC].Value);
				process.Name = mbo.Properties[Process.NAME].Value.ToString();
				process.PageFaultsPerSec = Convert.ToInt32(mbo.Properties[Process.PAGE_FAULTS_PER_SEC].Value);
				process.PageFileBytes = Convert.ToInt64(mbo.Properties[Process.PAGE_FILE_BYTES].Value);
				process.PageFileBytesPeak = Convert.ToInt64(mbo.Properties[Process.PAGE_FILE_BYTES_PEAK].Value);
				process.PercentPrivilegedTime = Convert.ToInt64(mbo.Properties[Process.PERCENT_PRIVILEDGED_TIME].Value);
				process.PercentProcessorTime = Convert.ToInt64(mbo.Properties[Process.PERCENT_PROCESSOR_TIME].Value);
				process.PercentUserTime = Convert.ToInt64(mbo.Properties[Process.PERCENT_USER_TIME].Value);
				process.PoolNonpagedBytes = Convert.ToInt32(mbo.Properties[Process.POOL_NONPAGED_BYTES].Value);
				process.PoolPagedBytes = Convert.ToInt32(mbo.Properties[Process.POOL_PAGED_BYTES].Value);
				process.PriorityBase = Convert.ToInt32(mbo.Properties[Process.PRIORITY_BASE].Value);
				process.PrivateBytes = Convert.ToInt64(mbo.Properties[Process.PRIVATE_BYTES].Value);
				process.ThreadCount = Convert.ToInt32(mbo.Properties[Process.THREAD_COUNT].Value);
				process.Timestamp_Object = Convert.ToInt64(mbo.Properties[Process.TIMESTAMP_OBJECT].Value);
				process.Timestamp_PerfTime = Convert.ToInt64(mbo.Properties[Process.TIMESTAMP_PERF_TIME].Value);
				process.Timestamp_Sys100NS = Convert.ToInt64(mbo.Properties[Process.TIMESTAMP_SYS_100_NS].Value);
				process.VirtualBytes = Convert.ToInt64(mbo.Properties[Process.VIRTUAL_BYTES].Value);
				process.VirtualBytesPeak = Convert.ToInt64(mbo.Properties[Process.VIRTUAL_BYTES_PEAK].Value);
				process.WorkingSet = Convert.ToInt64(mbo.Properties[Process.WORKING_SET].Value);
				process.WorkingSetPeak = Convert.ToInt64(mbo.Properties[Process.WORKING_SET_PEAK].Value);

				list.Add(process);
			}

			if (this.waitInterval > 0)
			{
				System.Threading.Thread.Sleep(this.waitInterval);

				foreach (ManagementBaseObject mbo in searcher.Get())
				{
					int pid = Convert.ToInt32(mbo.Properties[Process.ID_PROCESS].Value);
					string name = mbo.Properties[Process.NAME].Value.ToString();

					Process process = this[pid, name];

					if (process != null)
					{
						long value = calculateValue(542180608, process, Convert.ToInt64(mbo.Properties[Process.PERCENT_PROCESSOR_TIME].Value), Convert.ToInt64(mbo.Properties[Process.TIMESTAMP_SYS_100_NS].Value));

						process.PercentProcessorTime = value / numberOfCpus;
					}
				}
			}
		}

		private long calculateValue(int counterType, Process process, long numerator, long denominator)
		{
			long value = 0;

			if (counterType.Equals(542180608))
			{
				long numeratorValue = numerator - Convert.ToInt64(process.PercentProcessorTime);
				long denominatorValue = denominator - Convert.ToInt64(process.Timestamp_Sys100NS);
				
				value = (long)Math.Round(100 * (double)numeratorValue / (double)denominatorValue);
			}

			return value;
		}
		#endregion

		public string Machinename
		{
			get
			{
				return this.machinename;
			}
		}

		public string Username
		{
			get
			{
				return this.username;
			}
		}

		public string Query
		{
			get
			{
				return this.query;
			}
		}
		
		public int WaitInterval
		{
			get
			{
				return this.waitInterval;
			}	
		}
	}
}
