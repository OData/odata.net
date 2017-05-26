//---------------------------------------------------------------------
// <copyright file="QueryOptionUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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

        /// <summary>
        /// Parses query options from a specified URI into a dictionary.
        /// </summary>
        /// <param name="uri">The uri to get the query options from.</param>
        /// <returns>The parsed query options.</returns>
        /// <remarks>This method returns <see cref="List&lt;CustomQueryOptionToken&gt;"/> with all the query options.
        /// Note that it is valid to include multiple query options with the same name.</remarks>
        internal static List<CustomQueryOptionToken> ParseQueryOptions(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

            List<CustomQueryOptionToken> queryOptions = new List<CustomQueryOptionToken>();

            // COMPAT 31: Query options parsing
            // This method is a copy of System.Web.HttpValueCollection.FillFromString which is effectively the implementation
            // behind the System.Web.HttpUtility.ParseQueryString.
            // TODO: The System.Uri class does not replace/unescape URIs that use the '+' character to escape spaces;
            //      this however is common on the Web (also ASP.Net) and we have to figure out how we want to support it.
            string queryString = uri.Query.Replace('+', ' ');
            int length;
            if (queryString != null)
            {
                if (queryString.Length > 0 && queryString[0] == '?')
                {
                    queryString = queryString.Substring(1);
                }

                length = queryString.Length;
            }
            else
            {
                length = 0;
            }

            for (int i = 0; i < length; i++)
            {
                int startIndex = i;
                int equalSignIndex = -1;
                while (i < length)
                {
                    char ch = queryString[i];
                    if (ch == '=')
                    {
                        if (equalSignIndex < 0)
                        {
                            equalSignIndex = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    i++;
                }

                string queryOptionsName = null;
                string queryOptionValue = null;
                if (equalSignIndex >= 0)
                {
                    queryOptionsName = queryString.Substring(startIndex, equalSignIndex - startIndex);
                    queryOptionValue = queryString.Substring(equalSignIndex + 1, (i - equalSignIndex) - 1);
                }
                else
                {
                    queryOptionValue = queryString.Substring(startIndex, i - startIndex);
                }

                // COMPAT 31: Query options parsing
                // The System.Web version of the code uses HttpUtility.UrlDecode here, which calls into System.Web's own implementation
                // of the decoder. It's unclear if it's OK to use Uri.UnescapeDataString instead.
                queryOptionsName = queryOptionsName == null ? null : Uri.UnescapeDataString(queryOptionsName).Trim();
                queryOptionValue = queryOptionValue == null ? null : Uri.UnescapeDataString(queryOptionValue).Trim();

                queryOptions.Add(new CustomQueryOptionToken(queryOptionsName, queryOptionValue));

                if ((i == (length - 1)) && (queryString[i] == '&'))
                {
                    queryOptions.Add(new CustomQueryOptionToken(null, string.Empty));
                }
            }

            return queryOptions;
        }
    }
}