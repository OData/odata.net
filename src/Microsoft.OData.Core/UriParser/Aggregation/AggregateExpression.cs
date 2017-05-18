﻿//---------------------------------------------------------------------
// <copyright file="AggregateExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser.Aggregation
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// A aggregate expression representing a aggregation transformation.
    /// </summary>
    public sealed class AggregateExpression
    {
        private readonly AggregationMethod method;

        private readonly AggregationMethodDefinition methodDefinition;

        private readonly SingleValueNode expression;

        private readonly string alias;

        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Create a AggregateExpression.
        /// </summary>
        /// <param name="expression">The aggregation expression.</param>
        /// <param name="method">The <see cref="AggregationMethod"/>.</param>
        /// <param name="alias">The aggregation alias.</param>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> of this aggregate expression.</param>
        public AggregateExpression(SingleValueNode expression, AggregationMethod method, string alias, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");

            this.expression = expression;
            this.method = method;
            this.alias = alias;
            //// TypeRefrence is null for dynamic properties
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Create a AggregateExpression.
        /// </summary>
        /// <param name="expression">The aggregation expression.</param>
        /// <param name="methodDefinition">The <see cref="AggregationMethodDefinition"/>.</param>
        /// <param name="alias">The aggregation alias.</param>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> of this aggregate expression.</param>
        public AggregateExpression(SingleValueNode expression, AggregationMethodDefinition methodDefinition, string alias, IEdmTypeReference typeReference)
            : this(expression, methodDefinition.MethodKind, alias, typeReference)
        {
            this.methodDefinition = methodDefinition;
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
        /// Gets the <see cref="AggregationMethod"/>.
        /// </summary>
        public AggregationMethod Method
        {
            get
            {
                return method;
            }
        }

        /// <summary>
        /// Gets the <see cref="AggregationMethodDefinition"/>.
        /// </summary>
        public AggregationMethodDefinition MethodDefinition
        {
            get
            {
                return methodDefinition;
            }
        }

        /// <summary>
        /// Gets the aggregation alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return alias;
            }
        }

        /// <summary>
        /// Gets the <see cref="IEdmTypeReference"/> of this aggregate expression.
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
