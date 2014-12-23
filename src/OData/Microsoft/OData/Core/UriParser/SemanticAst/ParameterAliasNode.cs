//   OData .NET Libraries ver. 6.9
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
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
