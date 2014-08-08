//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Microsoft.Data.OData.Query.SyntacticAst;

    #endregion Namespaces

    /// <summary>
    /// Uri utility methods.
    /// </summary>
    internal static class UriUtils
    {
        /// <summary>
        /// Determines whether the <paramref name="baseUri"/> Uri instance is a 
        /// base of the specified Uri instance. 
        /// </summary>
        /// <remarks>
        /// The check is host agnostic. For example, "http://host1.com/Service.svc" is a valid base Uri of "https://host2.org/Service.svc/Bla" 
        /// but is not a valid base for "http://host1.com/OtherService.svc/Bla".
        /// </remarks>
        /// <param name="baseUri">The candidate base URI.</param>
        /// <param name="uri">The specified Uri instance to test.</param>
        /// <returns>true if the baseUri Uri instance is a base of uri; otherwise false.</returns>
        internal static bool UriInvariantInsensitiveIsBaseOf(Uri baseUri, Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(uri != null, "uri != null");

            Uri upperCurrent = CreateBaseComparableUri(baseUri);
            Uri upperUri = CreateBaseComparableUri(uri);

            return IsBaseOf(upperCurrent, upperUri);
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            List<CustomQueryOptionToken> queryOptions = new List<CustomQueryOptionToken>();

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

        /// <summary>
        /// is this selection item a structural or navigation property selection item.
        /// </summary>
        /// <param name="selectItem">the selection item to check</param>
        /// <returns>true if this selection item is a structural property selection item.</returns>
        internal static bool IsStructuralOrNavigationPropertySelectionItem(SelectItem selectItem)
        {
            DebugUtils.CheckNoExternalCallers();
            PathSelectItem pathSelectItem = selectItem as PathSelectItem;
            return pathSelectItem != null && (pathSelectItem.SelectedPath.LastSegment is NavigationPropertySegment || pathSelectItem.SelectedPath.LastSegment is PropertySegment);
        }

        /// <summary>Creates a URI suitable for host-agnostic comparison purposes.</summary>
        /// <param name="uri">URI to compare.</param>
        /// <returns>URI suitable for comparison.</returns>
        private static Uri CreateBaseComparableUri(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

#if PORTABLELIB
            uri = new Uri(UriUtilsCommon.UriToString(uri).ToUpperInvariant(), UriKind.RelativeOrAbsolute);
#else
            uri = new Uri(UriUtilsCommon.UriToString(uri).ToUpper(CultureInfo.InvariantCulture), UriKind.RelativeOrAbsolute);
#endif

            UriBuilder builder = new UriBuilder(uri);
            builder.Host = "h";
            builder.Port = 80;
            builder.Scheme = "http";
            return builder.Uri;
        }

        /// <summary>
        /// Check whether the <paramref name="baseUri"/> Uri is the base of the <paramref name="uri"/> Uri.
        /// </summary>
        /// <param name="baseUri">The candidate base Uri.</param>
        /// <param name="uri">The Uri to check.</param>
        /// <returns>True if the <paramref name="baseUri"/> is the base of the <paramref name="uri"/> Uri.</returns>
        private static bool IsBaseOf(Uri baseUri, Uri uri)
        {
            return baseUri.IsBaseOf(uri);
        }
    }
}
