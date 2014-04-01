//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    #region Namespaces
    using System;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    #endregion Namespaces

    /// <summary>
    /// Node to represent a range variable in an Any or All clause that referrs to an entity.
    /// </summary>
    public sealed class EntityRangeVariableReferenceNode : SingleEntityNode
    {
        /// <summary>
        ///  The name of the associated range variable (null if none)
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The entity type of the associated range variable.
        /// </summary>
        private readonly IEdmEntityTypeReference entityTypeReference;

        /// <summary>
        /// The range variable that the node represents.
        /// </summary>
        private readonly EntityRangeVariable rangeVariable;

        /// <summary>
        /// The navigation source containing the collection that this range variable iterates over.
        /// </summary>
        private readonly IEdmNavigationSource navigationSource;

        /// <summary>
        /// Creates an <see cref="EntityRangeVariableReferenceNode"/>.
        /// </summary>
        /// <param name="name"> The name of the associated range variable (null if none)</param>
        /// <param name="rangeVariable">The actual range variable on the bind stack that this refers to</param>
        /// <exception cref="System.ArgumentNullException">Throws if the input name or rangeVariable is null.</exception>
        public EntityRangeVariableReferenceNode(string name, EntityRangeVariable rangeVariable)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            ExceptionUtils.CheckArgumentNotNull(rangeVariable, "rangeVariable");
            this.name = name;
            this.navigationSource = rangeVariable.NavigationSource;
            this.entityTypeReference = rangeVariable.EntityTypeReference;
            this.rangeVariable = rangeVariable;
        }

        /// <summary>
        /// Gets the name of the associated rangevariable (null if none)
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the entity type of the associated range variable.
        /// </summary>
        public override IEdmTypeReference TypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets the entity type of the associated range variable.
        /// </summary>
        public override IEdmEntityTypeReference EntityTypeReference
        {
            get { return this.entityTypeReference; }
        }

        /// <summary>
        /// Gets a reference to the range variable that this node represents.
        /// </summary>
        public EntityRangeVariable RangeVariable
        {
            get { return this.rangeVariable; }
        }

        /// <summary>
        /// Gets the navigation source containing the collection that this range variable iterates over.
        /// </summary>
        public override IEdmNavigationSource NavigationSource
        {
            get { return this.navigationSource; }
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        internal override InternalQueryNodeKind InternalKind
        {
            get
            {
                return InternalQueryNodeKind.EntityRangeVariableReference;
            }
        }

        /// <summary>
        /// Accept a <see cref="QueryNodeVisitor{T}"/> that walks a tree of <see cref="QueryNode"/>s.
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
