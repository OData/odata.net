//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Data.OData.Query.SyntacticAst;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with query options.
    /// </summary>
    internal static class QueryOptionUtils
    {
        /// <summary>
        /// Returns a query option value by its name and removes the query option from the <paramref name="queryOptions"/> collection.
        /// </summary>
        /// <param name="queryOptions">The collection of query options.</param>
        /// <param name="queryOptionName">The name of the query option to get.</param>
        /// <returns>The value of the query option or null if no such query option exists.</returns>
        internal static string GetQueryOptionValueAndRemove(this List<CustomQueryOptionToken> queryOptions, string queryOptionName)
        {
            DebugUtils.CheckNoExternalCallers();
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
