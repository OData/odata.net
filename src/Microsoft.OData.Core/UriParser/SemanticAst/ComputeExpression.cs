//---------------------------------------------------------------------
// <copyright file="ComputeExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// An item that has been computed by the query at the current level of the tree.
    /// </summary>
    public sealed class ComputeExpression
    {
        private readonly SingleValueNode expression;

        private readonly string alias;

        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Create a ComputeExpression.
        /// </summary>
        /// <param name="expression">The compute expression.</param>
        /// <param name="alias">The compute alias.</param>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> of this aggregate expression.</param>
        public ComputeExpression(SingleValueNode expression, string alias, IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(expression, "expression");
            ExceptionUtils.CheckArgumentNotNull(alias, "alias");

            this.expression = expression;
            this.alias = alias;
            //// TypeRefrence is null for dynamic properties
            this.typeReference = typeReference;
        }

        /// <summary>
        /// Gets the aggregation expression.
        /// </summary>
        public SingleValueNode Expression
        {
            get
            {
                return this.expression;
            }
        }

        /// <summary>
        /// Gets the aggregation alias.
        /// </summary>
        public string Alias
        {
            get
            {
                return this.alias;
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