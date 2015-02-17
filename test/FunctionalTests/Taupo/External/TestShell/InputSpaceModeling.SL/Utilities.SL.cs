//---------------------------------------------------------------------
// <copyright file="Utilities.SL.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.SqlServer.Test.TestShell.Core.InputSpaceModeling
{
    internal static class Utilities
    {
        public static bool Exists<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            ExceptionUtilities.CheckArgumentNotNull(predicate, "predicate");

            return collection.Any(x => predicate(x));
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            ExceptionUtilities.CheckArgumentNotNull(first, "first");
            ExceptionUtilities.CheckArgumentNotNull(second, "second");

            List<T> tmp = new List<T>(first);
            tmp.AddRange(second);

            return tmp.AsReadOnly();
        }

        public static IList<object> GetEnumValues(Type enumType)
        {
            if (!enumType.IsEnum())
            {
                throw new ArgumentException("Type is not an enum type: " + enumType.FullName, "enumType");
            }

            var fields = enumType.GetFields(true, true);
            var values = new List<object>();
            foreach (var item in fields)
            {
                values.Add(item.GetValue(null));
            }

            return values;
        }

        public static void RemoveAll<T>(this IList<T> list, Func<T, bool> predicate)
        {
            var itemsToRemove = list.Where(i => predicate(i)).ToList();
            foreach (var itemToRemove in itemsToRemove)
            {
                list.Remove(itemToRemove);
            }
        }

#if !WIN8
        public static TypeConverter GetTypeConverter(Type type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            TypeConverterAttribute attribute = (TypeConverterAttribute)Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), false);
            if (attribute != null)
            { 
                var converterType = Type.GetType(attribute.ConverterTypeName, false);
                if (converterType != null)
                {
                    return (TypeConverter)Activator.CreateInstance(converterType);
                }

                throw new InvalidOperationException("Cannot find type '" + attribute.ConverterTypeName +"'.");
            }

            throw new InvalidOperationException("Type '" + type.FullName + "' does not have " + typeof(TypeConverterAttribute).Name);
        } 
#endif
    }
}
