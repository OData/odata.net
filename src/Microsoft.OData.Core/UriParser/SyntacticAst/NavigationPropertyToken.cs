//---------------------------------------------------------------------
// <copyright file="NavigationPropertyToken.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Query.SyntacticAst
{
    #region Namespaces

    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Lexical token representing a single nonroot segment in the query path.
    /// </summary>
    internal class NavigationPropertyToken : QueryToken
    {
        /// <summary>
        /// The name of the segment.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The parent segment.
        /// </summary>
        private readonly QueryToken parent;

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        private readonly IEnumerable<NamedValue> namedValues;

        /// <summary>
        /// Create a new SegmentQueryToken given the name and parent and namedValues if any
        /// </summary>
        /// <param name="name">The name of the segment, the identifier.</param>
        /// <param name="parent">The parent segment, or null if this is the root segment.</param>
        /// <param name="namedValues">The named values in the key lookup for this segment.</param>
        public NavigationPropertyToken(string name, QueryToken parent, IEnumerable<NamedValue> namedValues)
        {
            // We allow an "empty" name segment so we can create one for the case of a service-document URL (which has no path)
            ExceptionUtils.CheckArgumentNotNull(name, "name");

            this.name = name;
            this.parent = parent;
            this.namedValues = namedValues == null ? null : new ReadOnlyEnumerable<NamedValue>(namedValues);
        }

        /// <summary>
        /// Designates whether the navigation property is OK to be the parent of an Any/All or the last segment of the path.
        /// </summary>
        public bool ValidCollection { get; set; }

        /// <summary>
        /// The kind of the query token.
        /// </summary>
        public override QueryTokenKind Kind
        {
            get { return QueryTokenKind.NonRootSegment; }
        }

        /// <summary>
        /// The name of the segment, the identifier.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// The parent segment, or null if this is the root segment.
        /// </summary>
        public QueryToken Parent
        {
            get { return this.parent; }
        }

        /// <summary>
        /// The named values in the key lookup for this segment.
        /// If the segment has no key lookup, then this property is null.
        /// If the segment has empty key lookup (), then this property is an empty collection.
        /// </summary>
        public IEnumerable<NamedValue> NamedValues
        {
            get { return this.namedValues; }
        }
    }
}
