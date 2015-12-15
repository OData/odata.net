//---------------------------------------------------------------------
// <copyright file="AggregateTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using TreeNodeKinds;
    using System.Collections.Generic;

    public sealed class AggregateTransformationNode : TransformationNode
    {
        public AggregateTransformationNode(IEnumerable<AggregateStatement> statements)
        {
            ExceptionUtils.CheckArgumentNotNull(statements, "statements");

            this._statements = statements;
        }

        private readonly IEnumerable<AggregateStatement> _statements;

        public IEnumerable<AggregateStatement> Statements
        {
            get {
                return _statements;
            }
        }

        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Aggregate;
            }
        }
    }
}
