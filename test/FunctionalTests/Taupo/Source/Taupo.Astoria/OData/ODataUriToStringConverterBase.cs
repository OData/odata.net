//---------------------------------------------------------------------
// <copyright file="ODataUriToStringConverterBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Base implementation if the IODataUriToStringConverter contract
    /// </summary>
    public abstract class ODataUriToStringConverterBase : IODataUriToStringConverter
    {
        /// <summary>
        /// Gets or sets the formatter to use for key values
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IODataLiteralConverter LiteralConverter { get; set; }

        /// <summary>
        /// Converts the given uri into a string
        /// </summary>
        /// <param name="uri">The uri to convert</param>
        /// <returns>The converted uri string</returns>
        public string ConvertToString(ODataUri uri)
        {
            ExceptionUtilities.CheckArgumentNotNull(uri, "uri");
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            return this.ConcatenateSegmentsAndQueryOptions(uri.Segments, this.GetSortedQueryOptions(uri));
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(ODataUriSegment segment)
        {
            ExceptionUtilities.CheckArgumentNotNull(segment, "segment");
            var entitySet = segment as EntitySetSegment;
            if (entitySet != null)
            {
                return this.ConvertToString(entitySet);
            }

            var key = segment as KeyExpressionSegment;
            if (key != null)
            {
                return this.ConvertToString(key);
            }

            var property = segment as PropertySegment;
            if (property != null)
            {
                return this.ConvertToString(property);
            }

            var navigation = segment as NavigationSegment;
            if (navigation != null)
            {
                return this.ConvertToString(navigation);
            }

            var serviceRoot = segment as ServiceRootSegment;
            if (serviceRoot != null)
            {
                return this.ConvertToString(serviceRoot);
            }

            var system = segment as SystemSegment;
            if (system != null)
            {
                return this.ConvertToString(system);
            }

            var namedStreamEndpoint = segment as NamedStreamSegment;
            if (namedStreamEndpoint != null)
            {
                return this.ConvertToString(namedStreamEndpoint);
            }

            var oftype = segment as EntityTypeSegment;
            if (oftype != null)
            {
                return this.ConvertToString(oftype);
            }

            var function = segment as FunctionSegment;
            if (function != null)
            {
                return this.ConvertToString(function);
            }

            var unknown = segment as UnrecognizedSegment;
            ExceptionUtilities.CheckObjectNotNull(unknown, "Segment was of unrecognized type: '{0}'", segment.GetType().FullName);
            return this.ConvertToString(unknown);
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(EntitySetSegment segment)
        {
            ExceptionUtilities.CheckArgumentNotNull(segment, "segment");
            ExceptionUtilities.CheckObjectNotNull(segment.EntitySet != null || segment.EdmEntitySet != null, "Segment's entity set was unexpectedly null");

            if (segment.EntitySet != null)
            {
                ExceptionUtilities.CheckObjectNotNull(segment.EntitySet.Container, "Segment's entity set's container was unexpectedly null");

                return this.EscapeUriString(segment.EntitySet.Name);
            }
            else
            {
                ExceptionUtilities.CheckObjectNotNull(segment.EdmEntitySet.Container, "Segment's entity set's container was unexpectedly null");

                return this.EscapeUriString(segment.EdmEntitySet.Container.Name + '.' + segment.EdmEntitySet.Name);
            }
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(KeyExpressionSegment segment)
        {
            ExceptionUtilities.CheckArgumentNotNull(segment, "segment");
            var pairs = segment.IncludedValues
                .OrderBy(v => v.Key.Name, new OrdinalStringComparer())
                .Select(v => new NamedValue(v.Key.Name, v.Value));

            StringBuilder builder = new StringBuilder();
            this.LiteralConverter.AppendKeyExpression(builder, pairs);
            return builder.ToString();
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(PropertySegment segment)
        {
            return this.EscapeUriString(segment.Property.Name);
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(NavigationSegment segment)
        {
            return this.EscapeUriString(segment.NavigationProperty.Name);
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(ServiceRootSegment segment)
        {
            return segment.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(SystemSegment segment)
        {
            return segment.Endpoint;
        }

        /// <summary>
        /// Converts the given Named Stream segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(NamedStreamSegment segment)
        {
            return this.EscapeUriString(segment.Name);
        }

        /// <summary>
        /// Converts the given OfType segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(EntityTypeSegment segment)
        {
            return this.EscapeUriString(segment.EntityType.FullName);
        }

        /// <summary>
        /// Converts the given ServiceOperation segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(FunctionSegment segment)
        {
            string uriEndingToUse = string.Empty;
            if (segment.UseParentheses)
            {
                uriEndingToUse = "()";
            }

            if (segment.Container != null)
            {
                return this.EscapeUriString(segment.Container.Name) + "." + this.EscapeUriString(segment.Function.Name) + uriEndingToUse;
            }
            else
            {
                return this.EscapeUriString(segment.Function.Name) + uriEndingToUse;
            }
        }

        /// <summary>
        /// Converts the given segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment string</returns>
        public string ConvertToString(UnrecognizedSegment segment)
        {
            return segment.Value;
        }

        /// <summary>
        /// Concatenates the given segments and query options into one string using standard delimiters
        /// </summary>
        /// <param name="segments">The segments in the order they should appear</param>
        /// <param name="queryOptions">The query options in the order they should appear</param>
        /// <returns>The concatenated uri string</returns>
        protected string ConcatenateSegmentsAndQueryOptions(IEnumerable<ODataUriSegment> segments, IEnumerable<KeyValuePair<string, string>> queryOptions)
        {
            ExceptionUtilities.CheckArgumentNotNull(segments, "segments");
            ExceptionUtilities.CheckArgumentNotNull(queryOptions, "queryOptions");

            StringBuilder builder = new StringBuilder();

            if (segments.Any())
            {
                this.ConcatenateSegments(builder, segments);
            }

            if (queryOptions.Any())
            {
                builder.Append('?');
                UriHelpers.ConcatenateQueryOptions(builder, queryOptions);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns the uri's query options in some intentional order
        /// </summary>
        /// <param name="uri">The uri to get query options for</param>
        /// <returns>The sorted query options</returns>
        protected abstract IEnumerable<KeyValuePair<string, string>> GetSortedQueryOptions(ODataUri uri);

        private string EscapeUriString(string stringToEscape)
        {
            if (stringToEscape == null)
            {
                return null;
            }
            else
            {
                return Uri.EscapeDataString(stringToEscape);
            }
        }

        private class OrdinalStringComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.CompareOrdinal(x, y);
            }
        }
    }
}
