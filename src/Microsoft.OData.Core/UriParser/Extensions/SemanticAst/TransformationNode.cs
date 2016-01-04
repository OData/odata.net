//---------------------------------------------------------------------
// <copyright file="TransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser.Extensions.Semantic
{
    using TreeNodeKinds;

    /// <summary>
    /// Base class for all transformation nodes.
    /// </summary>
    public abstract class TransformationNode
    {
        /// <summary>
        /// Gets kind of transformation: groupby, aggregate, filter etc.
        /// </summary>
        public abstract TransformationNodeKind Kind
        {
            get;
        }
    }
}
