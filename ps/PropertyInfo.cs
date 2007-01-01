using System;
using System.Collections.Generic;
using System.Text;

namespace ps
{
	// Include how to calculate each property and then use that when actually get values for calculated stuff (percents)
	// include whether the property is calculated or not (WMI reference is your friend here)

	/// <summary>
	/// Wrapper for the System.Reflection.PropertyInfo class
	/// </summary>
	public class PropertyInfo
	{
		private System.Reflection.PropertyInfo propertyInfo;

		private static List<string> calculatedProperties;

		private PropertyInfo()
		{
		}

		public PropertyInfo(System.Reflection.PropertyInfo pi)
		{
			propertyInfo = pi;
		}

		public string this[object obj]
		{
			get
			{
				return propertyInfo.GetValue(obj, null).ToString();
			}
		}

		public string Name
		{
			get
			{
				return propertyInfo.Name;
			}
		}

		public Type Type
		{
			get
			{
				return propertyInfo.PropertyType;
			}
		}

		public bool IsCalculatedProperty
		{
			get
			{
				if (calculatedProperties == null)
				{
					calculatedProperties = new List<string>();
					calculatedProperties.Add(Process.PERCENT_PROCESSOR_TIME);
				}

				return calculatedProperties.Contains(Name);
			}
		}
	}
}
