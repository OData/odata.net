//---------------------------------------------------------------------
// <copyright file="SingleEntityNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Base class for all semantic metadata bound nodes which represent a single composable entity value.
    /// </summary>
    public abstract class SingleEntityNode : SingleResourceNode
    {
        /// <summary>
        /// Gets the type of this single entity.
        /// </summary>
        public abstract IEdmEntityTypeReference EntityTypeReference { get; }
    }
}