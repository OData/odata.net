//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;

    /// <summary>
    /// Node representing a semantically parsed parameter to a function.
    /// </summary>
    public class NamedFunctionParameterNode : QueryNode
    {
        /// <summary>
        /// The name of this parameter
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The semantically parsed value of this parameter
        /// </summary>
        private readonly QueryNode value;

        /// <summary>
        /// Creates a NamedFunctionParameterNode to represent a semantically parsed parameter to a function.
        /// </summary>
        /// <param name="name">the name of this function</param>
        /// <param name="value">the already semantically parsed value of this parameter.</param>
        public NamedFunctionParameterNode(string name, QueryNode value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the name of this parameter
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the semantically parsed value of this parameter.
        /// </summary>
        public QueryNode Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the kind of this node
        /// </summary>
        public override QueryNodeKind Kind
        {
            get { return (QueryNodeKind)this.InternalKind; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.NamedFunctionParameter;
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
