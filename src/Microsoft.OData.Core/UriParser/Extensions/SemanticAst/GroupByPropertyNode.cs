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

    public sealed class GroupByPropertyNode
    {
        private IList<GroupByPropertyNode> children = new List<GroupByPropertyNode>();

        private IEdmTypeReference typeReference;

        public GroupByPropertyNode(string name, SingleValueNode accessor)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            // OK for accessor to be null
            this.Name = name;
            this.Accessor = accessor;
        }

        public GroupByPropertyNode(string name, SingleValueNode accessor, IEdmTypeReference type)
            :this(name, accessor)
        {
            this.typeReference = type;
        }

        public string Name { get; private set; }

        public SingleValueNode Accessor { get; private set; }

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
