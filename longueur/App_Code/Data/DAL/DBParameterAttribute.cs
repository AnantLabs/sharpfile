using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DAL {
	/// <summary>
	/// Specifies the name of the arameter in the Dbcommand that the property maps to 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DBParameterAttribute : Attribute {
		private string parameterName;

		/// <summary>
		/// constructor
		/// </summary>
		/// <param name="fieldName">name of the parameter that the property will be mapped to</param>
		public DBParameterAttribute(string parameterName) {
			this.parameterName = parameterName;
		}

		public string ParameterName {
			get { return parameterName; }
		}
	}
}