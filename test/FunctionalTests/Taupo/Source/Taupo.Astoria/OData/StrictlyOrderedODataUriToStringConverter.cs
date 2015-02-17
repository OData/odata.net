//---------------------------------------------------------------------
// <copyright file="StrictlyOrderedODataUriToStringConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An implementation of IODataUriToStringConverter that uses a strict ordering for query options
    /// </summary>
    [ImplementationName(typeof(IODataUriToStringConverter), "StrictlyOrdered", HelpText = "An OData to string converter that strictly orders the query options")]
    public class StrictlyOrderedODataUriToStringConverter : ODataUriToStringConverterBase
    {
        /// <summary>
        /// Returns the query option values with a strict sorting
        /// </summary>
        /// <param name="uri">The current uri</param>
        /// <returns>The uri's query options after sorting</returns>
        protected override IEnumerable<KeyValuePair<string, string>> GetSortedQueryOptions(ODataUri uri)
        {
            List<KeyValuePair<string, string>> queryOptions = new List<KeyValuePair<string, string>>();

            // NOTE: this is set up partly to match the order the product uses for next-links. We may need to make this more customizable.
            queryOptions.AddRange(uri.CustomQueryOptions);

            if (uri.Filter != null)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.Filter, uri.Filter));
            }

            if (uri.ExpandSegments.Count > 0)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.Expand, this.ConcatenateSegments(uri.ExpandSegments)));
            }

            if (uri.OrderBy != null)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.OrderBy, uri.OrderBy));
            }

            if (uri.InlineCount != null)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.InlineCount, uri.InlineCount));
            }

            if (uri.SelectSegments.Count > 0)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.Select, this.ConcatenateSegments(uri.SelectSegments)));
            }

            if (uri.Top.HasValue)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.Top, uri.Top.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (uri.Skip.HasValue)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.Skip, uri.Skip.Value.ToString(CultureInfo.InvariantCulture)));
            }

            if (uri.SkipToken != null)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.SkipToken, uri.SkipToken));
            }

            if (uri.Format != null)
            {
                queryOptions.Add(new KeyValuePair<string, string>(QueryOptions.Format, uri.Format));
            }

            return queryOptions;
        }
    }
}
