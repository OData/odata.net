//---------------------------------------------------------------------
// <copyright file="GroupByTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Extensions.TreeNodeKinds;
    using Microsoft.OData.Core.UriParser.Semantic;

    public sealed class GroupByTransformationNode : TransformationNode
    {
        private readonly CollectionNode _source;

        private readonly TransformationNode _childTransformation;

        private readonly IEnumerable<GroupByPropertyNode> _groupingProperties;

        public GroupByTransformationNode(
            IList<GroupByPropertyNode> groupingProperties,
            TransformationNode childTransformation,
            CollectionNode source)
        {
            ExceptionUtils.CheckArgumentNotNull(groupingProperties, "groupingProperties");
            // OK for source to be null

            this._groupingProperties = groupingProperties;
            this._childTransformation = childTransformation;
            this._source = source;
        }

        public IEnumerable<GroupByPropertyNode> GroupingProperties
        {
            get
            {
                return _groupingProperties;
            }
        }

        public TransformationNode ChildTransformation
        {
            get
            {
                return _childTransformation;
            }
        }

        public CollectionNode Source
        {
            get
            {
                return _source;
            }
        }

        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.GroupBy;
            }
        }
    }
}
