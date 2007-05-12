using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

/// <summary>
/// Summary description for Wittisms
/// </summary>
[XmlRootAttribute(ElementName="wittisms")]
public class Wittisms
{
	private string wittism;
	private List<string> wittismsList;

	public Wittisms()
	{
		wittismsList = new List<string>();
	}

	[XmlElement("wittism")]
	public string[] WittismArray
	{
		get
		{
			return (string[])wittismsList.ToArray();
		}
		set
		{
			wittismsList = new List<string>(value);
		}
	}

	[XmlIgnore]
	public int Count
	{
		get
		{
			return wittismsList.Count;
		}
	}

	public string this[int idx]
	{
		get
		{
			if (wittismsList.Count <= idx) 
			{
				throw new Exception("There are only " + wittismsList.Count + " wittisms, jerk. So, don't ask for index " + idx + ".");
			}

			return wittismsList[idx];
		}
	}
}
