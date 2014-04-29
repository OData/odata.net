//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.Syntactic;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with query options.
    /// </summary>
    internal static class QueryOptionUtils
    {
        /// <summary>
        /// Gets parameter alias names and strings, which starts with '@' char. and remove them from queryOptions collection.
        /// </summary>
        /// <param name="queryOptions">The queryOptions collection.</param>
        /// <returns>The dictionary of parameter alias names and strings.</returns>
        internal static Dictionary<string, string> GetParameterAliases(this List<CustomQueryOptionToken> queryOptions)
        {
            Debug.Assert(queryOptions != null, "queryOptions != null");
            Dictionary<string, string> ret = new Dictionary<string, string>(StringComparer.Ordinal);
            List<CustomQueryOptionToken> tokensToRemove = new List<CustomQueryOptionToken>();
            foreach (var s in queryOptions)
            {
                if ((!string.IsNullOrEmpty(s.Name)) && s.Name[0] == '@')
                {
                    ret[s.Name] = s.Value; // always keep the last one if name is duplicated.
                    tokensToRemove.Add(s);
                }
            }

            foreach (var s in tokensToRemove)
            {
                queryOptions.Remove(s);
            }

            return ret;
        }

        /// <summary>
        /// Returns a query option value by its name from the <paramref name="queryOptions"/> collection.
        /// </summary>
        /// <param name="queryOptions">The collection of query options.</param>
        /// <param name="queryOptionName">The name of the query option to get.</param>
        /// <returns>The value of the query option or null if no such query option exists.</returns>
        internal static string GetQueryOptionValue(this List<CustomQueryOptionToken> queryOptions, string queryOptionName)
        {
            Debug.Assert(queryOptions != null, "queryOptions != null");
            Debug.Assert(queryOptionName == null || queryOptionName.Length > 0, "queryOptionName == null || queryOptionName.Length > 0");

            CustomQueryOptionToken option = null;

            foreach (var queryOption in queryOptions)
            {
                if (queryOption.Name == queryOptionName)
                {
                    if (option == null)
                    {
                        option = queryOption;
                    }
                    else
                    {
                        throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(queryOptionName));
                    }
                }
            }

            return option == null ? null : option.Value;
        }

        /// <summary>
        /// Returns a query option value by its name and removes the query option from the <paramref name="queryOptions"/> collection.
        /// Currently, it is only used by un-exposed syntatic tree parsing.
        /// </summary>
        /// <param name="queryOptions">The collection of query options.</param>
        /// <param name="queryOptionName">The name of the query option to get.</param>
        /// <returns>The value of the query option or null if no such query option exists.</returns>
        internal static string GetQueryOptionValueAndRemove(this List<CustomQueryOptionToken> queryOptions, string queryOptionName)
        {
            Debug.Assert(queryOptions != null, "queryOptions != null");
            Debug.Assert(queryOptionName == null || queryOptionName.Length > 0, "queryOptionName == null || queryOptionName.Length > 0");

            CustomQueryOptionToken option = null;

            for (int i = 0; i < queryOptions.Count; ++i)
            {
                if (queryOptions[i].Name == queryOptionName)
                {
                    if (option == null)
                    {
                        option = queryOptions[i];
                    }
                    else
                    {
                        throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce(queryOptionName));
                    }

                    queryOptions.RemoveAt(i);
                    i--;
                }
            }

            return option == null ? null : option.Value;
        }
    }
}
