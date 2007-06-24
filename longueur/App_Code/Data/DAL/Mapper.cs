using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DAL {
	/// <summary>
	/// This class holds information about mapping a database field to a 
	/// object property.
	/// </summary>
	public class Mapper {
		private List<string> mappingInfo;

		/// <summary>
		/// Default constructor
		/// </summary>
		public Mapper() {
			mappingInfo = new List<string>();
		}

		/// <summary>
		/// Add mapping information. This method can be used to add more than one mapping at a time.
		/// You could use it like: mapper.Add("property1=field1", "property2=field2", "property3=field3", ...)
		/// </summary>
		/// <param name="mappings">mapping information in format "[property name]=[field name]"</param>
		public void Add(params string[] mappings) {
			foreach (string map in mappings)
				mappingInfo.Add(map);
		}

		/// <summary>
		/// Return mapping information held in this class as string array
		/// </summary>
		public string[] MappingInformation {
			get {
				string[] mappings = new string[mappingInfo.Count];
				mappingInfo.CopyTo(mappings);

				return mappings;
			}
		}

		/// <summary>
		/// Indexer property. By providing the name it returns the mapping info for that property.
		/// If the mapping information for the provided property does not exist, the indexer 
		/// return null. 
		/// You could use it like: string mapInfo = mapper["property1"];
		/// </summary>
		/// <param name="propertyName">the name of the property to return mapping information</param>
		/// <returns>mapping information for the property provided</returns>
		public string this[string propertyName] {
			get {
				foreach (string map in mappingInfo) {
					string[] spplitedString = map.Split('=');
					if (spplitedString[0] == propertyName)
						return map;
				}

				return null;
			}
		}

		/// <summary>
		/// Another indexer property. This property returns mapping information, that is stored in the list, in order.
		/// </summary>
		/// <param name="index">the index</param>
		/// <returns>mapping information</returns>
		public string this[int index] {
			get {
				if (index < mappingInfo.Count)
					return mappingInfo[index];
				else
					return null;
			}
		}

		/// <summary>
		/// Get the property name from the mapping information
		/// </summary>
		/// <param name="map">mapping information</param>
		/// <returns>the name of the property from the provided mapping information</returns>
		public static string GetProperty(string map) {
			// split the mapping info and return the name of the property
			string[] spplitedString = map.Split('=');
			return spplitedString[0];
		}

		/// <summary>
		/// Get the field name from the mapping information
		/// </summary>
		/// <param name="map">mapping information</param>
		/// <returns>the name of the field from the provided mapping information</returns>
		public static string GetField(string map) {
			// split the mapping info and return the name of the field
			string[] spplitedString = map.Split('=');
			return spplitedString[1];
		}

		/// <summary>
		/// Get the parameter name from the mapping information (if the mapper is used for storing data)
		/// </summary>
		/// <param name="map">mapping information</param>
		/// <returns>the name of the field from the provided mapping information</returns>
		public static string GetParameter(string map) {
			// split the mapping info and return the name of the field
			string[] spplitedString = map.Split('=');
			return spplitedString[1];
		}
	}
}