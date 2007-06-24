using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;

namespace Data.DAL {
	public class DBHelper : Data.Base {
		/// <summary>
		/// Generic method. Gets an object of type T from the data reader. It uses mapping information 
		/// provided to read a field from the reader, and gets the property name and sets
		/// the value of the property with the data which are held in database field
		/// </summary>
		/// <typeparam name="T">The type of object to be instantiated</typeparam>
		/// <param name="rdr">Data Reader where the data will be read from</param>
		/// <param name="mappings">mapping information</param>
		/// <returns>an instance of type T with the properties populated from database</returns>
		private static T GetItemFromReader<T>(IDataReader rdr, Mapper mappings) where T : class {
			Type type = typeof(T);
			T item = Activator.CreateInstance<T>(); // create an instance of the type provided
			foreach (string map in mappings.MappingInformation) {
				// for each mapping information 
				string property = Mapper.GetProperty(map);
				string field = Mapper.GetField(map);

				PropertyInfo propInfo = type.GetProperty(property); // ge the property by name

				if (Convert.IsDBNull(rdr[field])) // data in database is null, so do not set the value of the property
					continue;

				if (propInfo.PropertyType == rdr[field].GetType()) // if the property and database field are the same
					propInfo.SetValue(item, rdr[field], null); // set the value of property
				else {
					// change the type of the data in table to that of property and set the value of the property
					propInfo.SetValue(item, Convert.ChangeType(rdr[field], propInfo.PropertyType), null);
				}
			}
			return item;
		}

		/// <summary>
		/// Generic method. Gets an object of type T from the data reader. It uses attribute information 
		/// applied to a property to read a field from the reader, and gets the property name and sets
		/// the value of the property with the data which are held in database field
		/// </summary>
		/// <typeparam name="T">The type of object to be instantiated</typeparam>
		/// <param name="rdr">Data Reader where the data will be read from</param>
		/// <returns>an instance of type T with the properties populated from database</returns>
		private static T GetItemFromReader<T>(IDataReader rdr) where T : class {
			Type type = typeof(T);
			T item = Activator.CreateInstance<T>();
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo property in properties) {
				// for each property declared in the type provided check if the property is
				// decorated with the DBField attribute
				if (Attribute.IsDefined(property, typeof(DBFieldAttribute))) {
					DBFieldAttribute attrib = (DBFieldAttribute)Attribute.GetCustomAttribute(property, typeof(DBFieldAttribute));

					if (Convert.IsDBNull(rdr[attrib.FieldName])) // data in database is null, so do not set the value of the property
						continue;

					if (property.PropertyType == rdr[attrib.FieldName].GetType()) // if the property and database field are the same
						property.SetValue(item, rdr[attrib.FieldName], null); // set the value of property
					else {
						// change the type of the data in table to that of property and set the value of the property
						property.SetValue(item, Convert.ChangeType(rdr[attrib.FieldName], property.PropertyType), null);
					}
				}
			}

			return item;
		}

		/// <summary>
		/// Get one object from the database by using the attribute information
		/// </summary>
		/// <typeparam name="T">the type of object the collection will hold</typeparam>
		/// <param name="command">DbCommand that is used to read data from the database</param>
		/// <returns>populated object from the database</returns>
		public static T ReadObject<T>(IDbCommand command) where T : class {
			IDataReader reader = command.ExecuteReader();
			if (reader.Read())
				return GetItemFromReader<T>(reader);
			else
				return default(T);
		}

		/// <summary>
		/// Get one object from the database by using the mapping information frovided by Mapper class
		/// </summary>
		/// <typeparam name="T">the type of object the collection will hold</typeparam>
		/// <param name="command">DbCommand that is used to read data from the database</param>
		/// <returns>populated object from the database</returns>
		public static T ReadObject<T>(IDbCommand command, Mapper mappingInfo) where T : class {
			IDataReader reader = command.ExecuteReader();
			if (reader.Read())
				return GetItemFromReader<T>(reader, mappingInfo);
			else
				return default(T);
		}

		/// <summary>
		/// Get a collection of objects from the database by using the attribute information
		/// </summary>
		/// <typeparam name="T">the type of object the collection will hold</typeparam>
		/// <param name="command">DbCommand that is used to read data from the database</param>
		/// <returns>a collection of populated objects from the database</returns>
		public static List<T> ReadCollection<T>(IDbCommand command) where T : class {
			List<T> collection = new List<T>();
			IDataReader reader = command.ExecuteReader();
			while (reader.Read()) {
				T item = GetItemFromReader<T>(reader);
				collection.Add(item);
			}

			return collection;
		}

		public static List<T> ReadCollection<T>(string sql) where T : class {
			List<T> collection = new List<T>();

			lock (lockObject) {
				using (SqlCommand sqlCommand = getSqlCommand(sql)) {
					try {
						sqlConnection.Open();
						using (IDataReader reader = sqlCommand.ExecuteReader()) {
							while (reader.Read()) {
								T item = GetItemFromReader<T>(reader);
								collection.Add(item);
							}
						}
					} catch (Exception ex) {
						throw ex;
					} finally {
						sqlConnection.Close();
					}
				}
			}

			return collection;
		}

		/// <summary>
		/// Get a collection of objects from the database by using the mapping information provided 
		/// by Mapper class
		/// </summary>
		/// <typeparam name="T">the type of object the collection will hold</typeparam>
		/// <param name="command">DbCommand that is used to read data from the database</param>
		/// <returns>>a collection of populated objects from the database</returns>
		public static List<T> ReadCollection<T>(IDbCommand command, Mapper mappingInfo) where T : class {
			List<T> collection = new List<T>();
			IDataReader reader = command.ExecuteReader();
			while (reader.Read()) {
				T item = GetItemFromReader<T>(reader, mappingInfo);
				collection.Add(item);
			}

			return collection;
		}

		/// <summary>
		/// Saves the object into database using attribute information
		/// </summary>
		/// <param name="obj">the object to be saved</param>
		/// <param name="command">the Dbcommand used to save the object</param>
		public static void SaveObject(object obj, IDbCommand command) {
			Type type = obj.GetType();
			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo property in properties) {
				// for each property declared in the type provided check if the property is
				// decorated with the DBField attribute
				if (Attribute.IsDefined(property, typeof(DBParameterAttribute))) {
					DBParameterAttribute attrib = (DBParameterAttribute)Attribute.GetCustomAttribute(property, typeof(DBParameterAttribute));
					IDataParameter param = (IDataParameter)command.Parameters[attrib.ParameterName]; // get parameter
					param.Value = property.GetValue(obj, null); // set parameter value
				}
			}

			command.ExecuteNonQuery();
		}

		/// <summary>
		/// Saves the object into database using mapping information
		/// </summary>
		/// <param name="obj">object ot be saved</param>
		/// <param name="command">the Dbcommand used to save the objec</param>
		/// <param name="mappingInfo">mapping information (property=Sql Parameter)</param>
		public static void SaveObject(object obj, IDbCommand command, Mapper mappingInfo) {
			Type type = obj.GetType();
			foreach (string map in mappingInfo.MappingInformation) {
				// for each mapping information 
				string property = Mapper.GetProperty(map);
				string parameter = Mapper.GetParameter(map);

				PropertyInfo propInfo = type.GetProperty(property); // ge the property by name
				IDataParameter param = (IDataParameter)command.Parameters[parameter]; // get the parameter
				param.Value = propInfo.GetValue(obj, null); // set paramter value
			}

			command.ExecuteNonQuery();
		}

		public static void SaveCollection(IList collection, IDbCommand command) {
			foreach (object item in collection) {
				SaveObject(item, command);
			}
		}

		public static void SaveCollection(IList collection, IDbCommand command, Mapper mappingInfo) {
			foreach (object item in collection) {
				SaveObject(item, command, mappingInfo);
			}
		}
	}
}