using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Common.Comparers {
    // From: http://www.codeproject.com/csharp/DynamicCodeVsReflection.asp.
    public enum DynamicComparerType {
        Asc,
        Desc
    }

    public class DynamicComparer<T> : IComparer<T> {
        // Private members.
        private delegate int ComparerDelegate(T x, T y);
        private ComparerDelegate comparerDelegate;
        private DynamicComparerType sortType;

        /// <summary>
        /// Constructor that takes a property name and creates a dynamic delegate.
        /// </summary>
        /// <param name="name">Property name to compare against.</param>
        public DynamicComparer(string name, DynamicComparerType sortType) {
            this.sortType = sortType;
            comparerDelegate = getComparerDelegate(name);
        }

        /// <summary>
        /// Compare method that compares our two objects.
        /// </summary>
        /// <param name="x">Object x.</param>
        /// <param name="y">Object y.</param>
        /// <returns>Int which specifies the comparison value of x and y.</returns>
        public int Compare(T x, T y) {
            if (sortType == DynamicComparerType.Asc) {
                return comparerDelegate(x, y);
            } else {
                return comparerDelegate(y, x);
            }
        }

        /// <summary>
        /// Creates a delegate for the specified property name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>Delegate for our comparer.</returns>
        private ComparerDelegate getComparerDelegate(string name) {
            Type objectType = typeof(T);

            PropertyInfo pi = objectType.GetProperty(name);
            FieldInfo fi = objectType.GetField(name);
            Type memberType;
            bool isProperty = false;

            if (pi != null) {
                if (pi.GetGetMethod() != null) {
                    memberType = pi.PropertyType;
                    isProperty = true;
                } else {
                    throw new Exception(String.Format(
                                            "Property: '{0}' of Type: '{1}' " +
                                            "does not have a Public Get accessor",
                                            name, objectType.Name));
                }
            } else if (fi != null) {
                memberType = fi.FieldType;
            } else {
                throw new Exception(String.Format(
                                        "'{0}' is not a Public Field or Property" +
                                        " with a Get accessor for Type: '{1}' ",
                                        name, objectType.Name));
            }

            Type comparerType = typeof(Comparer<>).MakeGenericType(new Type[] { memberType });
            MethodInfo getDefaultMethod = comparerType.GetProperty("Default").GetGetMethod();
            MethodInfo compareMethod = getDefaultMethod.ReturnType.GetMethod("Compare");

            DynamicMethod dm = new DynamicMethod("Compare_" + name, typeof(int), new Type[] { objectType, objectType }, comparerType);
            ILGenerator il = dm.GetILGenerator();

            // Load Comparer<memberType>.Default onto the stack
            il.EmitCall(OpCodes.Call, getDefaultMethod, null);

            // Load the member from arg 0 onto the stack
            il.Emit(OpCodes.Ldarg_0);

            if (isProperty) {
                il.EmitCall(OpCodes.Callvirt, pi.GetGetMethod(), null);
            } else {
                il.Emit(OpCodes.Ldfld);
            }

            // Load the member from arg 1 onto the stack
            il.Emit(OpCodes.Ldarg_1);

            if (isProperty) {
                il.EmitCall(OpCodes.Callvirt, pi.GetGetMethod(), null);
            } else {
                il.Emit(OpCodes.Ldfld);
            }

            // Call the Compare method
            il.EmitCall(OpCodes.Callvirt, compareMethod, null);
            il.Emit(OpCodes.Ret);

            return (ComparerDelegate)dm.CreateDelegate(typeof(ComparerDelegate));
        }
    }
}