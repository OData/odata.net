//---------------------------------------------------------------------
// <copyright file="QueryGroupingType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a grouping type in a QueryType hierarchy.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Does not make sense in this context")]
    public class QueryGroupingType : QueryStructuralType
    {
        private QueryType elementType;

        /// <summary>
        /// Initializes a new instance of the QueryGroupingType class.
        /// </summary>
        /// <param name="keyType">Type of the grouping key.</param>
        /// <param name="elementType">Type of the grouping element.</param>
        /// <param name="evaluationStrategy">Evaluation strategy.</param>
        public QueryGroupingType(QueryType keyType, QueryType elementType, IQueryEvaluationStrategy evaluationStrategy)
            : base(evaluationStrategy)
        {
            this.Key = QueryProperty.Create("Key", keyType);
            this.elementType = elementType;

            QueryCollectionType collectionType = elementType.CreateCollectionType();
            this.Elements = QueryProperty.Create("Elements", collectionType);

            this.AddProperties(new[] { this.Key, this.Elements });
            this.MakeReadOnly();
        }

        /// <summary>
        /// Gets the Key property of the GroupingType.
        /// </summary>
        public QueryProperty Key { get; private set; }

        /// <summary>
        /// Gets the Elemenets property  of the GroupingType.
        /// </summary>
        public QueryProperty Elements { get; private set; }

        /// <summary>
        /// Gets the string representation of a given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "Grouping(Key: {0}, Element: {1})", this.Key.PropertyType.StringRepresentation, this.elementType.StringRepresentation);
            }
        }

        /// <summary>
        /// Determines whether the type can be assigned from another.
        /// </summary>
        /// <param name="queryType">Type to assign from.</param>
        /// <returns>True if assignment is possible, false otherwise.</returns>
        public override bool IsAssignableFrom(QueryType queryType)
        {
            ExceptionUtilities.CheckArgumentNotNull(queryType, "queryType");

            if (object.ReferenceEquals(this, queryType))
            {
                return true;
            }

            var other = queryType as QueryGroupingType;
            if (other == null)
            {
                return false;
            }

            return this.Key.PropertyType.IsAssignableFrom(other.Key.PropertyType) && this.Elements.PropertyType.IsAssignableFrom(other.Elements.PropertyType);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
