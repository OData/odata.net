//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces

    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    #endregion Namespaces

    /// <summary>
    /// Node representing a search term.
    /// </summary>
    public sealed class SearchTermNode : SingleValueNode
    {
        /// <summary>
        /// Bool reference, as search term is used for matching
        /// </summary>
        private static readonly IEdmTypeReference BoolTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(typeof(bool));

        /// <summary>
        /// The search term value.
        /// </summary>
        private readonly string text;
       
        /// <summary>
        /// Create a SearchTermNode
        /// </summary>
        /// <param name="text">The literal text for this node's value, formatted according to the OData URI literal formatting rules.</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input literalText is null.</exception>
        public SearchTermNode(string text)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(text, "literalText");
            this.text = text;
        }

        /// <summary>
        /// Gets the search term value.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }
        }

        /// <summary>
        /// Gets the resouce type of the single value this node represents.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get
            {
                return BoolTypeReference;
            }
        }

        /// <summary>
        /// Gets the kind of the query node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.SearchTerm;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walk a tree of <see cref="QueryNode"/>s.
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
