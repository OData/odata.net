//---------------------------------------------------------------------
// <copyright file="TransformationNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
// OData v4 Aggregation Extensions.
namespace Microsoft.OData.Core.UriParser.Semantic
{
    using TreeNodeKinds;
    using Microsoft.OData.Edm;
    using System.Collections.Generic;

    public abstract class TransformationNode
    {
        /// <summary>
        /// Gets kind of transformation: groupby, aggregate, filter etc.
        /// </summary>
        public abstract TransformationNodeKind Kind
        {
            get;
        }

        /// <summary>
        /// Gets edm type returned from transformation
        /// </summary>
        public abstract IEdmTypeReference ItemType
        {
            get;
        }
    }
}
