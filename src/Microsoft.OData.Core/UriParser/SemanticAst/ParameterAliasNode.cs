//---------------------------------------------------------------------
// <copyright file="ParameterAliasNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents a parameter alias that appears in uri path, $filter or $orderby.
    /// </summary>
    public class ParameterAliasNode : SingleValueNode
    {
        /// <summary>
        /// The alias' type which is infered from the type of alias value's SingleValueNode.
        /// </summary>
        private readonly IEdmTypeReference typeReference;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="alias">The parameter alias.</param>
        /// <param name="typeReference">The alias' type which is infered from the type of alias value's SingleValueNode.</param>
        public ParameterAliasNode(string alias, IEdmTypeReference typeReference)
        {
            this.Alias = alias;
            this.typeReference = typeReference;
        }

        /// <summary>
        /// The parameter alias.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// The alias' type which is infered from the type of alias value's SingleValueNode
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.typeReference; }
        }

        /// <summary>
        /// Is InternalQueryNodeKind.ParameterAlias.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get { return InternalQueryNodeKind.ParameterAlias; }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> to walk a tree of <see cref="QueryNode"/>s.
        /// </summary>
        /// <typeparam name="T">Type that the visitor will return after visiting this token.</typeparam>
        /// <param name="visitor">An implementation of the visitor interface.</param>
        /// <returns>An object whose type is determined by the type parameter of the visitor.</returns>
        /// <exception cref="System.ArgumentNullException">Throws if the input visitor is null.</exception>
        public override T Accept<T>(QueryNodeVisitor<T> visitor)
        {
            ExceptionUtils.CheckArgumentNotNull(visitor, "visitor");
            return visitor.Visit(this);
        }
    }
}
