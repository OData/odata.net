//---------------------------------------------------------------------
// <copyright file="GroupByPropertyNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>
    /// A node representing a grouping property.
    /// </summary>
    public sealed class GroupByPropertyNode
    {
        private IList<GroupByPropertyNode> children = new List<GroupByPropertyNode>();

        private IEdmTypeReference typeReference;

        /// <summary>
        /// Create a GroupByPropertyNode.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="accessor">The <see cref="SingleValueNode"/> of this node.</param>
        public GroupByPropertyNode(string name, SingleValueNode accessor)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");

            this.Name = name;
            this.Accessor = accessor;
        }

        /// <summary>
        /// Create a GroupByPropertyNode.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="accessor">The <see cref="SingleValueNode"/> of this node.</param>
        /// <param name="type">The <see cref="IEdmTypeReference"/> of this node.</param>
        public GroupByPropertyNode(string name, SingleValueNode accessor, IEdmTypeReference type)
            : this(name, accessor)
        {
            this.typeReference = type;
        }

        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the <see cref="SingleValueNode"/> of this node.
        /// </summary>
        public SingleValueNode Accessor { get; private set; }

        /// <summary>
        /// Gets the <see cref="IEdmTypeReference"/> of this node.
        /// </summary>
        public IEdmTypeReference TypeReference 
        {
            get
            {
                if (Accessor == null)
                {
                    return null;
                }
                else
                {
                    return typeReference;
                }
            } 
        }

        /// <summary>
        /// Gets or sets the children <see cref="GroupByPropertyNode"/>s of this node.
        /// </summary>
        public IList<GroupByPropertyNode> Children
        {
            get
            {
                return children;
            }

            set
            {
                children = value;
            }
        }
    }
}
