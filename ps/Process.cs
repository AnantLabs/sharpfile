using System;
using System.Reflection;
using System.Collections.Generic;

namespace ps
{
	public class Process
	{
		public const string IDLE = "Idle";
		public const string SYSTEM = "System";

		public const string CAPTION = "Caption";
		public const string CREATING_PROCESS_ID = "CreatingProcessID";
		public const string DESCRIPTION = "Description";
		public const string ELAPSED_TIME = "ElapsedTime";
		public const string FREQUENCY_OBJECT = "Frequency_Object";
		public const string FREQUENCY_PERF_TIME = "Frequency_PerfTime";
		public const string FREQUENCY_SYS_100_NS = "Frequency_Sys100NS";
		public const string HANDLE_COUNT = "HandleCount";
		public const string ID_PROCESS = "IDProcess";
		public const string IO_DATA_OPERATIONS_PER_SEC = "IODataOperationsPerSec";
		public const string IO_OTHER_OPERATIONS_PER_SEC = "IOOtherOperationsPerSec";
		public const string IO_READ_BYTES_PER_SEC = "IOReadBytesPerSec";
		public const string IO_READ_OPERATIONS_PER_SEC = "IOReadOperationsPerSec";
		public const string IO_WRITE_BYTES_PER_SEC = "IOWriteBytesPerSec";
		public const string IO_WRITE_OPERATIONS_PER_SEC = "IOWriteOperationsPerSec";
		public const string IO_DATA_BYTES_PER_SEC = "IODataBytesPerSec";
		public const string IO_OTHER_BYTES_PER_SEC = "IOOtherBytesPerSec";
		public const string NAME = "Name";
		public const string PAGE_FAULTS_PER_SEC = "PageFaultsPerSec";
		public const string PAGE_FILE_BYTES = "PageFileBytes";
		public const string PAGE_FILE_BYTES_PEAK = "PageFileBytesPeak";
		public const string PERCENT_PRIVILEDGED_TIME = "PercentPrivilegedTime";
		public const string PERCENT_PROCESSOR_TIME = "PercentProcessorTime";
		public const string PERCENT_USER_TIME = "PercentUserTime";
		public const string POOL_NONPAGED_BYTES = "PoolNonpagedBytes";
		public const string POOL_PAGED_BYTES = "PoolPagedBytes";
		public const string PRIORITY_BASE = "PriorityBase";
		public const string PRIVATE_BYTES = "PrivateBytes";
		public const string THREAD_COUNT = "ThreadCount";
		public const string TIMESTAMP_OBJECT = "Timestamp_Object";
		public const string TIMESTAMP_PERF_TIME = "Timestamp_PerfTime";
		public const string TIMESTAMP_SYS_100_NS = "Timestamp_Sys100NS";
		public const string VIRTUAL_BYTES = "VirtualBytes";
		public const string VIRTUAL_BYTES_PEAK = "VirtualBytesPeak";
		public const string WORKING_SET = "WorkingSet";
		public const string WORKING_SET_PEAK = "WorkingSetPeak";

		private string caption;
		private int creatingProcessID;
		private string description;
		private long elapsedTime;
		private long frequency_Object;
		private long frequency_PerfTime;
		private long frequency_Sys100NS;
		private int handleCount;
		private int idProcess;
		private long ioDataOperationsPerSec;
		private long ioOtherOperationsPerSec;
		private long ioReadBytesPerSec;
		private long ioReadOperationsPerSec;
		private long ioWriteBytesPerSec;
		private long ioWriteOperationsPerSec;
		private long ioDataBytesPerSec;
		private long ioOtherBytesPerSec;
		private string name;
		private int pageFaultsPerSec;
		private long pageFileBytes;
		private long pageFileBytesPeak;
		private long percentPrivilegedTime;
		private long percentProcessorTime;
		private long percentUserTime;
		private int poolNonpagedBytes;
		private int poolPagedBytes;
		private int priorityBase;
		private long privateBytes;
		private int threadCount;
		private long timestamp_Object;
		private long timestamp_PerfTime;
		private long timestamp_Sys100NS;
		private long virtualBytes;
		private long virtualBytesPeak;
		private long workingSet;
		private long workingSetPeak;

		private static List<PropertyInfo> properties;
		private static List<string> calculatedProperties;

		public Process()
		{
		}
		
		public string this[string propertyName]
		{
			get
			{
				PropertyInfo propertyInfo = GetPropertyInfo(propertyName);

				if (propertyInfo != null)
				{
					return propertyInfo[this];
				}
				else
				{
					throw new Exception("The property name, " + name + " does not exist.");
				}
			}
		}

		public string Id
		{
			get
			{
				if (IDProcess > -1
					&& !string.IsNullOrEmpty(Name))
				{
					return IDProcess + "_" + Name;
				}

				return string.Empty;
			}
		}

		public string Name
		{
			get
			{
				if (string.IsNullOrEmpty(name))
				{
					name = string.Empty;
				}
				
				return name;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					name = string.Empty;
				}
				else
				{
					name = value;
				}
			}
		}

		public int IDProcess
		{
			get { return idProcess; }
			set { idProcess = value; }
		}

		public int CreatingProcessID 
		{ 
			get { return creatingProcessID; } 
			set { creatingProcessID = value; }
		}

		public string Caption
		{
			get { return caption; } 
			set { caption = value; }
		}
		
		public string Description
		{
			get { return description; } 
			set { description = value; }
		}

		public long ElapsedTime
		{
			get { return elapsedTime; } 
			set { elapsedTime = value; }
		}

		public long Frequency_Object
		{
			get { return frequency_Object; } 
			set { frequency_Object = value; }
		}

		public long Frequency_PerfTime
		{
			get { return frequency_PerfTime; } 
			set { frequency_PerfTime = value; }
		}

		public long Frequency_Sys100NS
		{
			get { return frequency_Sys100NS; } 
			set { frequency_Sys100NS = value; }
		}

		public int HandleCount
		{
			get { return handleCount; } 
			set { handleCount = value; }
		}

		public long IODataOperationsPerSec
		{
			get { return ioDataOperationsPerSec; } 
			set { ioDataOperationsPerSec = value; }
		}

		public long IOOtherOperationsPerSec
		{
			get { return ioOtherOperationsPerSec; } 
			set { ioOtherOperationsPerSec = value; }
		}

		public long IOReadBytesPerSec
		{
			get { return ioReadBytesPerSec; } 
			set { ioReadBytesPerSec = value; }
		}

		public long IOReadOperationsPerSec
		{
			get { return ioReadOperationsPerSec; } 
			set { ioReadOperationsPerSec = value; }
		}

		public long IOWriteBytesPerSec
		{
			get { return ioWriteBytesPerSec; } 
			set { ioWriteBytesPerSec = value; }
		}

		public long IOWriteOperationsPerSec
		{
			get { return ioWriteOperationsPerSec; } 
			set { ioWriteOperationsPerSec = value; }
		}

		public long IODataBytesPerSec
		{
			get { return ioDataBytesPerSec; } 
			set { ioDataBytesPerSec = value; }
		}

		public long IOOtherBytesPerSec
		{
			get { return ioOtherBytesPerSec; } 
			set { ioOtherBytesPerSec = value; }
		}

		public int PageFaultsPerSec
		{
			get { return pageFaultsPerSec; } 
			set { pageFaultsPerSec = value; }
		}

		public long PageFileBytes
		{
			get { return pageFileBytes; } 
			set { pageFileBytes = value; }
		}

		public long PageFileBytesPeak
		{
			get { return pageFileBytesPeak; } 
			set { pageFileBytesPeak = value; }
		}

		public long PercentPrivilegedTime
		{
			get { return percentPrivilegedTime; } 
			set { percentPrivilegedTime = value; }
		}

		public long PercentProcessorTime
		{
			get { return percentProcessorTime; } 
			set { percentProcessorTime = value; }
		}

		public long PercentUserTime
		{
			get { return percentUserTime; } 
			set { percentUserTime = value; }
		}

		public int PoolNonpagedBytes
		{
			get { return poolNonpagedBytes; } 
			set { poolNonpagedBytes = value; }
		}

		public int PoolPagedBytes
		{
			get { return poolPagedBytes; } 
			set { poolPagedBytes = value; }
		}

		public int PriorityBase
		{
			get { return priorityBase; } 
			set { priorityBase = value; }
		}

		public long PrivateBytes
		{
			get { return privateBytes; } 
			set { privateBytes = value; }
		}

		public int ThreadCount
		{
			get { return threadCount; } 
			set { threadCount = value; }
		}

		public long Timestamp_Object
		{
			get { return timestamp_Object; } 
			set { timestamp_Object = value; }
		}

		public long Timestamp_PerfTime
		{
			get { return timestamp_PerfTime; } 
			set { timestamp_PerfTime = value; }
		}

		public long Timestamp_Sys100NS
		{
			get { return timestamp_Sys100NS; } 
			set { timestamp_Sys100NS = value; }
		}

		public long VirtualBytes
		{
			get { return virtualBytes; } 
			set { virtualBytes = value; }
		}

		public long VirtualBytesPeak
		{
			get { return virtualBytesPeak; } 
			set { virtualBytesPeak = value; }
		}

		public long WorkingSet
		{
			get { return workingSet; } 
			set { workingSet = value; }
		}

		public long WorkingSetPeak
		{
			get { return workingSetPeak; } 
			set { workingSetPeak = value; }
		}

		public static List<PropertyInfo> Properties
		{
			get
			{
				if (properties == null)
				{
					List<System.Reflection.PropertyInfo> piList = new List<System.Reflection.PropertyInfo>(typeof(Process).GetProperties());

					properties = piList.ConvertAll<PropertyInfo>(delegate(System.Reflection.PropertyInfo pi)
					{
						return new PropertyInfo(pi);
					});
				}

				return properties;
			}
		}

		public static PropertyInfo GetPropertyInfo(string propertyName)
		{
			PropertyInfo propertyInfo = Properties.Find(delegate(PropertyInfo pi)
				{
					if (pi.Name == propertyName)
					{
						return true;
					}

					return false;
				});

			return propertyInfo;
		}
	}
}
