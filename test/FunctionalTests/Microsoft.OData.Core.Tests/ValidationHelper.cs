using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Tests
{
    public class ValidationHelper
    {
        /// <summary>
        /// This method takes 2 generic types and see if one is the deep copy  of other
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="copy"></param>
        /// <returns></returns>
        public static List<string> GetDifferences<T>(T obj, T copy)
        {
            var diff = new List<string>();
            EvaluateDifferences(obj, copy, diff);

            return diff;
        }

        /// <summary>
        /// Generic Method to evaluate differences of 2 objects, caled recursively
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="copy"></param>
        /// <param name="diff"></param>
        private static void EvaluateDifferences<T>(T obj, T copy, List<string> diff)
        {

            foreach (var prop in obj.GetType().GetProperties().Where(x => x.CanRead))
            {
                var val1 = prop.GetValue(obj);
                var val2 = prop.GetValue(copy);

                if (val1 == null && val2 == null)
                {
                    continue;
                }

                if (val1 == null || val2 == null)
                {
                    diff.Add(string.Format("Value of {0} does not match.", prop.Name));
                }
                else if (typeof(Uri) == prop.PropertyType)
                {
                    if (Uri.Compare((Uri)val1, (Uri)val2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.CurrentCulture) != 0)
                    {
                        diff.Add(string.Format("Value of Uri {0} does not match.", prop.Name));
                    }
                }
                else if (typeof(IComparable).IsAssignableFrom(prop.PropertyType) || prop.PropertyType.IsPrimitive() || prop.PropertyType.IsValueType())
                {
                    if ((!val1.Equals(val2)))
                    {
                        diff.Add(string.Format("Value of {0} does not match.", prop.Name));
                    }
                }
                else if (prop.PropertyType.IsClass())
                {
                    EvaluateDifferences(val1, val2, diff);
                }
                else if (typeof(Enumerable).IsAssignableFrom(prop.PropertyType))
                {
                    var collection1 = ((IEnumerable)val1).Cast<object>();
                    var collection2 = ((IEnumerable)val2).Cast<object>();

                    if (collection1.Count() != collection2.Count())
                    {
                        diff.Add(string.Format("Collection count of {0} does not match.", prop.Name));
                    }
                    else
                    {
                        for (int i = 0; i < collection1.Count(); i++)
                        {
                            var element1 = collection1.ElementAt(i);
                            var element2 = collection2.ElementAt(i);
                            var collpropType = element1.GetType();

                            if (element1 == null && element2 == null)
                            {
                                continue;
                            }

                            if (element1 == null || element2 == null)
                            {
                                diff.Add(string.Format("Value of items in Collection {0} does not match.", prop.Name));
                            }
                            else if (typeof(Uri) == collpropType)
                            {
                                if (Uri.Compare((Uri)element1, (Uri)element2, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.CurrentCulture) != 0)
                                {
                                    diff.Add(string.Format("Value of Uri collection {0} does not match.", prop.Name));
                                }
                            }
                            else if (typeof(IComparable).IsAssignableFrom(collpropType) || collpropType.IsPrimitive() || collpropType.IsValueType())
                            {
                                if ((!element1.Equals(element2)))
                                {
                                    diff.Add(string.Format("Values of Collection {0} does not match.", prop.Name));
                                }
                            }
                            else
                            {
                                EvaluateDifferences(element1, element2, diff);
                            }
                        }
                    }
                }
            }
        }
    }
}
