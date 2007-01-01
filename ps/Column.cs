using System;
using System.Collections.Generic;

namespace ps
{
	/// <summary>
	/// Summary description for Column
	/// </summary>
	public class Column
	{
		private string name;
		private int valueLength;

		private static Dictionary<string, string> abbreviatedColumnNames;

		public Column(string name, int valueLength)
		{
			this.name = name;
			this.valueLength = valueLength;
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public int ValueLength
		{
			get
			{
				return valueLength;
			}
			set
			{
				valueLength = value;
			}
		}

		public string AbbreviatedName
		{
			get
			{
				// TODO: Make this an XML file, for more flexibility later. There should be a default resource in case it is not there.
				if (abbreviatedColumnNames == null)
				{
					abbreviatedColumnNames = new Dictionary<string, string>(36);

					abbreviatedColumnNames.Add(Process.PERCENT_PROCESSOR_TIME, "%C");
					abbreviatedColumnNames.Add(Process.ID_PROCESS, "Id");
					abbreviatedColumnNames.Add(Process.WORKING_SET, "WS");
					abbreviatedColumnNames.Add(Process.NAME, "Name");
				}

				return abbreviatedColumnNames[Name];
			}
		}
	}
}
