using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Core.Aggregation
{
    public static class Extensions
    {
        /// <summary>
        /// Find an element in an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="elemenet"></param>
        /// <returns></returns>
        public static int Find<T>(this T[] array, T elemenet)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(elemenet))
                    return i;
            }
            return -1;
        }


        /// <summary>
        /// Remove a single occurrence of each character from the head or tail of the string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="charactersToTrim"></param>
        /// <returns></returns>
        public static string TrimOne(this string str, params char[] charactersToTrim)
        {
            int start = 0;
            int end = str.Length - 1;
            var removedFromStart = new List<char>();
            var removedFromEnd = new List<char>();

            for (int i = 0; i < charactersToTrim.Length; i++)
            {
                foreach (var c in charactersToTrim)
                {
                    if ((str[start] == c) && (!removedFromStart.Contains(c)))
                    {
                        start++;
                        removedFromStart.Add(c);
                    }
                    if ((str[end] == c) && (!removedFromEnd.Contains(c)))
                    {
                        end--;
                        removedFromEnd.Add(c);
                    }
                } 
            }
            return str.Substring(start, end - start + 1);

        }

        /// <summary>
        /// Remove call to the method from the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimMethodCallPrefix(this string str)
        {
            var p = str.IndexOf('(');
            if (p == -1)
            {
                return str;
            }
            else
            {
                return str.Substring(p).Trim().Trim('(');
            }
        }

        /// <summary>
        /// Remove the last ')' from the method call
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimMethodCallSufix(this string str)
        {
            return str.Trim().TrimOne(')');
            
        }

        /// <summary>
        /// Trim method call. Example: "round(value)" to: "value"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string TrimMethodCall(this string str)
        {
            return str.TrimMethodCallPrefix().TrimMethodCallSufix();
        }
    }
}
