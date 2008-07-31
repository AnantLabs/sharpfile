using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace Common {
    public static class Reflection {
        /// <summary>
        /// Instantiates an object based on an assembly name and fully-qualified type.
        /// </summary>
        /// <param name="assembly">Assembly name.</param>
        /// <param name="type">Fully-qualified type.</param>
        /// <returns>Object that cooresponds to the assembly and type.</returns>
        public static object InstantiateObject(string assembly, string type) {
            return InstantiateObject(assembly, type, null);
        }

        /// <summary>
        /// Instantiates an object based on an assembly name and fully-qualified type.
        /// </summary>
        /// <param name="assembly">Assembly name.</param>
        /// <param name="type">Fully-qualified type.</param>
        /// <returns>Object that cooresponds to the assembly and type.</returns>
        public static object InstantiateObject(string assembly, string type, params object[] arguments) {
            ObjectHandle objectHandle = Activator.CreateInstance(assembly, type, true, 0,
                null, arguments, null, null, null);
            return objectHandle.Unwrap();
        }

        /// <summary>
        /// Instantiates an object based on an assembly name and fully-qualified type.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="assembly">Assembly name.</param>
        /// <param name="type">Fully-qualified type.</param>
        /// <exception cref="System.TypeLoadException">Thrown when the instantiated object does not conform to the passed-in type parameter.</exception>
        /// <returns>Object that cooresponds to the assembly and type.</returns>
        public static T InstantiateObject<T>(string assembly, string type) {
            return InstantiateObject<T>(assembly, type, null);
        }

        /// <summary>
        /// Instantiates an object based on an assembly name and fully-qualified type.
        /// </summary>
        /// <typeparam name="T">Type of the return value.</typeparam>
        /// <param name="assembly">Assembly name.</param>
        /// <param name="type">Fully-qualified type.</param>
        /// <param name="arguments">Arguments.</param>
        /// <exception cref="System.TypeLoadException">Thrown when the instantiated object does not conform to the passed-in type parameter.</exception>
        /// <returns>Object that cooresponds to the assembly and type.</returns>
        public static T InstantiateObject<T>(string assembly, string type, params object[] arguments) {
            ObjectHandle objectHandle = Activator.CreateInstance(assembly, type, true, 0,
                null, arguments, null, null, null);
            object obj = objectHandle.Unwrap();

            if (obj is T) {
                return (T)obj;
            } else {
                throw new TypeLoadException(string.Format(
                    "Type, {0}, is not type of T, {1}.", type, typeof(T).Name));
            }
        }

        /// <summary>
        ///  Instantiates an object based on its type.
        /// </summary>
        /// <typeparam name="T"><Type of the return value./typeparam>
        /// <param name="type">Type to instantiate.</param>
		/// <exception cref="System.TypeLoadException">Thrown when the instantiated object does not conform to the passed-in type parameter.</exception>
        /// <returns>Object that cooresponds to the assembly and type.</returns>
        public static T InstantiateObject<T>(Type type) {
            object obj = Activator.CreateInstance(type);

            if (obj is T) {
                return (T)obj;
            } else {
                throw new TypeLoadException(string.Format(
                    "Type, {0}, is not type of T, {1}.", type, typeof(T).Name));
            }
        }

        /// <summary>
        /// Creates a delegate of the specified type that references the passed in method.
        /// Uses the BindingFlags of: Public | Static.
        /// </summary>
        /// <typeparam name="T">Type of the delegate.</typeparam>
        /// <param name="assembly">Assembly that the method is found in.</param>
        /// <param name="type">Fully qualified type where the method is found.</param>
        /// <param name="method">Name of the method.</param>
        /// <returns>T delegate that references the passed in method.</returns>
        public static T CreateDelegate<T>(string assembly, string type, string method) {
            return CreateDelegate<T>(assembly, type, method, BindingFlags.Public | BindingFlags.Static);
        }

        /// <summary>
        /// Creates a delegate of the specified type that references the passed in method.
        /// </summary>
        /// <typeparam name="T">Type of the delegate.</typeparam>
        /// <param name="assembly">Assembly that the method is found in.</param>
        /// <param name="type">Fully qualified type where the method is found.</param>
        /// <param name="method">Name of the method.</param>
        /// <param name="bindingFlags">BindingFlags to find the method.</param>
        /// <returns>T delegate that references the passed in method.</returns>
        public static T CreateDelegate<T>(string assembly, string type, string method, BindingFlags bindingFlags) {
            if (!typeof(T).IsSubclassOf(typeof(Delegate))) {
                throw new ArgumentException("Type T must be a delegate", "T");
            }

            Type generatedType = Assembly.Load(assembly).GetType(type, true);

            Delegate del = Delegate.CreateDelegate(
                typeof(T),
                generatedType.GetMethod(method, bindingFlags));

            if (del is T) {
                return (T)(object)del;
            } else {
                throw new Exception("Created delegate is not of the correct type, " + typeof(T).Name + ".");
            }
        }

        /// <summary>
        /// Deep copies the data from an object to a new object.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">Object to copy.</param>
        /// <returns>A new version of the object with all of it's values intact.</returns>
        public static T DeepCopy<T>(T obj) {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, obj);
            memoryStream.Position = 0;
            return (T)binaryFormatter.Deserialize(memoryStream);
        }

        /// <summary>
        /// Duplicates an object and fills any public properties or fields from the original.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj">Original object.</param>
        /// <returns>The duplicated object.</returns>
        public static T DuplicateObject<T>(T obj) {
            Type type = obj.GetType();
            
             T newObj = InstantiateObject<T>(
                    type.Assembly.FullName, type.FullName);

            DuplicateObject<T>(obj, newObj);

            return newObj;
        }

        /// <summary>
        /// Sets any public properties or fields from the original into the new object.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="obj">Original object.</param>
        /// <param name="newObj">New object.</param>
        public static void DuplicateObject<T>(T obj, T newObj) {
            // Need to use the type of the actual object and not just T because
            // T might be an interface and the actual class needs to be constructed.
            Type type = obj.GetType();

            foreach (PropertyInfo propertyInfo in type.GetProperties()) {
                // Only set properties which have a setter.
                if (propertyInfo.CanRead && propertyInfo.CanWrite) {
                    propertyInfo.SetValue(
                        newObj,
                        propertyInfo.GetValue(obj, null),
                        null);
                }
            }

            foreach (FieldInfo fieldInfo in type.GetFields()) {
                // Only set properties which have a setter.
                if (!fieldInfo.IsInitOnly && !fieldInfo.IsLiteral) {
                    fieldInfo.SetValue(
                        newObj,
                        fieldInfo.GetValue(obj));
                }
            }

            foreach (EventInfo eventInfo in type.GetEvents()) {
                // TODO: Determine if an event is wired for the obj instance
                // and wire up the new instance as well.
            }
        }
    }
}