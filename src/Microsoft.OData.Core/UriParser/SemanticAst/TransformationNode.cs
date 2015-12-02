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
        public abstract TransformationNodeKind Kind
        {
            get;
        }

        public abstract IEdmTypeReference ItemType
        {
            get;
        }
    }
}
