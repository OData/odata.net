//---------------------------------------------------------------------
// <copyright file="QueryExpressionToPhpOptionsConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Php
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;
    
    /// <summary>
    /// Class for Parsing the Query Expression and building the Query options for PHP
    /// </summary>
    public class QueryExpressionToPhpOptionsConverter
    {
        /// <summary>
        /// Gets or sets the expression to uri converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryToODataUriConverter ExpressionConverter { get; set; }

        /// <summary>
        /// Gets or sets the uri to string converter
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriToStringConverter { get; set; }

        /// <summary>
        /// Builds the QueryOptions 
        /// </summary>
        /// <param name="expression">The expression to convert</param>
        /// <returns>returns the query option for the PHP client library</returns>
        public PhpQueryOptions Convert(QueryExpression expression)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var odataUri = this.ExpressionConverter.ComputeUri(expression);

            var options = new PhpQueryOptions()
            {
                Filter = odataUri.Filter,
                OrderBy = odataUri.OrderBy,
                Count = odataUri.IsCount(),
                InlineCount = odataUri.InlineCount == QueryOptions.InlineCountAllPages,
            };

            string inlineCount;
            if (odataUri.CustomQueryOptions.TryGetValue(QueryOptions.InlineCount, out inlineCount))
            {
                options.InlineCount = inlineCount == QueryOptions.InlineCountAllPages;
            }

            if (odataUri.Skip.HasValue)
            {
                options.Skip = odataUri.Skip.Value.ToString(CultureInfo.InvariantCulture);
            }
            
            if (odataUri.Top.HasValue)
            {
                options.Top = odataUri.Top.Value.ToString(CultureInfo.InvariantCulture);
            }
            
            if (odataUri.ExpandSegments.Count > 0)
            {
                options.Expand = this.UriToStringConverter.ConcatenateSegments(odataUri.ExpandSegments);
            }

            if (odataUri.SelectSegments.Count > 0)
            {
                options.Select = this.UriToStringConverter.ConcatenateSegments(odataUri.SelectSegments);
            }

            EntitySet expectedEntitySet;
            if (odataUri.TryGetExpectedEntitySet(out expectedEntitySet))
            {
                options.EntityContainer = expectedEntitySet.Container.Name;
                options.EntitySet = expectedEntitySet.Name;
            }

            if (odataUri.IsEntity())
            {
                var key = odataUri.Segments.OfType<KeyExpressionSegment>().LastOrDefault();
                if (key != null)
                {
                    options.PrimaryKey = this.UriToStringConverter.ConvertToString(key);
                }
            }

            return options;
        }
    }
}