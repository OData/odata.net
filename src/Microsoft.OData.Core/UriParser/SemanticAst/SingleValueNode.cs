//---------------------------------------------------------------------
// <copyright file="SingleValueNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces

    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Base class for all semantic metadata bound nodes which represent a single composable value.
    /// </summary>
    public abstract class SingleValueNode : QueryNode
    {
        /// <summary>
        /// Gets the type of the single value this node represents.
        /// </summary>
        public abstract IEdmTypeReference TypeReference
        {
            get;
        }

        /// <summary>
        /// Gets the kind of this node.
        /// </summary>
        public override QueryNodeKind Kind
        {
            get { return (QueryNodeKind)this.InternalKind; }
        }
    }
}