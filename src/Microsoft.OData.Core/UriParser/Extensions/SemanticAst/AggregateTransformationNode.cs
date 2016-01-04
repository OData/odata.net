//---------------------------------------------------------------------
// <copyright file="AggregateTransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using System.Collections.Generic;
    using TreeNodeKinds;

    /// <summary>
    /// Node representing a aggregate transformation.
    /// </summary>
    public sealed class AggregateTransformationNode : TransformationNode
    {
        private readonly IEnumerable<AggregateStatement> statements;

        /// <summary>
        /// Create a AggregateTransformationNode.
        /// </summary>
        /// <param name="statements">A list of <see cref="AggregateStatement"/>.</param>
        public AggregateTransformationNode(IEnumerable<AggregateStatement> statements)
        {
            ExceptionUtils.CheckArgumentNotNull(statements, "statements");

            this.statements = statements;
        }

        /// <summary>
        /// Gets the list of <see cref="AggregateStatement"/>.
        /// </summary>
        public IEnumerable<AggregateStatement> Statements
        {
            get 
            {
                return statements;
            }
        }

        /// <summary>
        /// Gets the kind of the transformation node.
        /// </summary>
        public override TransformationNodeKind Kind
        {
            get
            {
                return TransformationNodeKind.Aggregate;
            }
        }
    }
}
