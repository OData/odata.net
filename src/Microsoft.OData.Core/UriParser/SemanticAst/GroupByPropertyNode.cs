//---------------------------------------------------------------------
// <copyright file="GroupByPropertyNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Core.UriParser.Semantic
{
    public class GroupByPropertyNode
    {

        public GroupByPropertyNode(string name, SingleValuePropertyAccessNode accessor)
        {
            ExceptionUtils.CheckArgumentNotNull(name, "name");
            // OK for accessor to be null
            this.Name = name;
            this.Accessor = accessor;
        }

        public string Name { get; private set; }

        public SingleValuePropertyAccessNode Accessor { get; private set; }
       
        public IList<GroupByPropertyNode> Children { get; } = new List<GroupByPropertyNode>();
    }
}
