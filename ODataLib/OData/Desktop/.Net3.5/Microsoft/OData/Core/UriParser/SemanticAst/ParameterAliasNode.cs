//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
