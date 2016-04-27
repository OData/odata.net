//---------------------------------------------------------------------
// <copyright file="AllNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// Query node representing an All query.
    /// </summary>
    public sealed class AllNode : LambdaNode
    {
        /// <summary>
        /// Create an AllNode
        /// </summary>
        /// <param name="rangeVariables">The name of the rangeVariables list.</param>
        public AllNode(Collection<RangeVariable> rangeVariables) : this(rangeVariables, null)
        {
            Debug.Assert(false, "Don't ever call this, its for backcompat");
        }

        /// <summary>
        /// Create an AllNode
        /// </summary>
        /// <param name="rangeVariables">The name of the rangeVariables list.</param>
        /// <param name="currentRangeVariable">The new range variable being added by this all node</param>
        public AllNode(Collection<RangeVariable> rangeVariables, RangeVariable currentRangeVariable)
            : base(rangeVariables, currentRangeVariable)
        {
        }

        /// <summary>
        /// The resource type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return EdmCoreModel.Instance.GetBoolean(true); }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.All;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}