//---------------------------------------------------------------------
// <copyright file="ValidationHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Tests
{
    public static class ValidationHelper
    {
        /// <summary>
        /// This method takes 2 generic types and see if one is the deep copy  of other
        /// </summary>
        /// <typeparam name="T">Method accepts Generic type</typeparam>
        /// <param name="obj">Object to be compared with</param>
        /// <param name="copy">Copy object to compare</param>
        /// <returns>Returns a list of differences</returns>
        public static List<string> GetDifferences<T>(T obj, T copy)
        {
            var diff = new List<string>();
            EvaluateDifferences(obj, copy, diff);

            return diff;
        }


        /// <summary>
        /// To evaluate the differences of 2 generic types, called recursively
        /// </summary>
        /// <typeparam name="T">Method accepts Generic type</typeparam>
        /// <param name="obj">Object to be compared with</param>
        /// <param name="copy">Copy object to compare</param>
        /// <param name="diff">List Of Differences</param>
        /// <param name="objName">Optional parameter for objectname, to be used in differences string, default empty string</param>
        /// <param name="isCollection">Optional parameter to see if its a collection, default false</param>
        private static void EvaluateDifferences<T>(T obj, T copy, List<string> diff,string objName ="", bool isCollection =false)
        {
            if (obj == null && copy == null)
            {
                return;
            }

            string collectionStatement = isCollection?" collection":string.Empty;

            if (obj == null || copy == null)
            {
                diff.Add(string.Format("Value of{0} {1} does not match", collectionStatement, objName));
                return;
            }

            var objType = obj.GetType();

            if (typeof(Uri) == objType)
            {
                if (Uri.Compare(new Uri(obj.ToString()), new Uri(copy.ToString()), UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.CurrentCulture) != 0)
                {
                    diff.Add(string.Format("Value of Uri{0} {1} does not match.", collectionStatement,objName));
                }
            }
            else if (typeof(IComparable).IsAssignableFrom(objType) || objType.IsPrimitive() || objType.IsValueType())
            {
                if ((!obj.Equals(copy)))
                {
                    diff.Add(string.Format("Value of{0} {1} does not match.",collectionStatement, objName));
                }
            }
            else if (objType.IsClass())
            {
                foreach (var prop in obj.GetType().GetProperties().Where(x => x.CanRead))
                {
                    var val1 = prop.GetValue(obj);
                    var val2 = prop.GetValue(copy);

                    EvaluateDifferences(val1, val2, diff,prop.Name);
                }
            }
            else if (typeof(Enumerable).IsAssignableFrom(objType))
            {
                var collection1 = ((IEnumerable)obj).Cast<object>();
                var collection2 = ((IEnumerable)copy).Cast<object>();

                if (collection1.Count() != collection2.Count())
                {
                    diff.Add(string.Format("Collection count of {0} does not match.", objName));
                }
                else
                {
                    for (int i = 0; i < collection1.Count(); i++)
                    {
                        var element1 = collection1.ElementAt(i);
                        var element2 = collection2.ElementAt(i);

                        EvaluateDifferences(element1, element2,diff, collection1.GetType().Name, true);
                    }
                }
            }
        }
    }
}
