//---------------------------------------------------------------------
// <copyright file="AtomWorkspaceMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.Atom
{
    /// <summary>
    /// Atom metadata description for a workspace.
    /// </summary>
    public sealed class AtomWorkspaceMetadata
    {
        /// <summary>Gets or sets the title of the workspace.</summary>
        /// <returns>The title of the workspace.</returns>
        public AtomTextConstruct Title
        {
            get;
            set;
        }
    }
}
