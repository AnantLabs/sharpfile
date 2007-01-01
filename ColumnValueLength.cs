using System;

namespace ps
{
	/// <summary>
	/// Summary description for ColumnValueLength
	/// </summary>
	public class ColumnValueLength
	{
		private string columnName;
		private int valueLength;

		public ColumnValueLength(string columnName, int valueLength)
		{
			this.columnName = columnName;
			this.valueLength = valueLength;
		}

		public string ColumnName
		{
			get
			{
				return columnName;
			}
			set
			{
				columnName = value;
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
	}
}
