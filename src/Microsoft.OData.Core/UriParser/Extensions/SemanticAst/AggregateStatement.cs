//---------------------------------------------------------------------
// <copyright file="AggregateStatement.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Extensions;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// A aggregate statement representing a aggregation transformation.
    /// </summary>
    public sealed class AggregateStatement
    {
        private readonly AggregationVerb withVerb;

        private readonly SingleValueNode expression;

        private readonly string alias;

        private readonly SingleValuePropertyAccessNode from;

        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Create a AggregateStatement.
        /// </summary>
        /// <param name="expression">The aggregation expression.</param>
        /// <param name="withVerb">The <see cref="AggregationVerb"/>.</param>
        /// <param name="from">The aggregation from <see cref="SingleValuePropertyAccessNode"/>.</param>
        /// <param name="alias">The aggregation alias.</param>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> of this aggregate statement.</param>
        public AggregateStatement(SingleValueNode expression, AggregationVerb withVerb, SingleValuePropertyAccessNode from, string alias, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            this.expression = expression;
            this.withVerb = withVerb;
            this.from = from;
            this.alias = alias;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Gets the aggregation expression.
        /// </summary>
        public SingleValueNode Expression
        {
            get 
            {
                return expression;
            }
        }

        /// <summary>
        /// Gets the <see cref="AggregationVerb"/>.
        /// </summary>
        public AggregationVerb WithVerb
        {
            get
            {
                return withVerb;
            }
        }

        /// <summary>
        /// Gets the aggregation alias.
        /// </summary>
        public string AsAlias
        {
            get
            {
                return alias;
            }
        }

        /// <summary>
        /// Gets the aggregation from <see cref="SingleValuePropertyAccessNode"/>.
        /// </summary>
        public SingleValuePropertyAccessNode From
        {
            get
            {
                return from;
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmTypeReference"/> of this aggregate statement.
        /// </summary>
        public IEdmTypeReference TypeReference
        {
            get
            {
                return this.typeReference;
            }
        }
    }
}
