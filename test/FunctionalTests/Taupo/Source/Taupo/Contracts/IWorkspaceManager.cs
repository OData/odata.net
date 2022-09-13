//---------------------------------------------------------------------
// <copyright file="IWorkspaceManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Provides directory information for workspace builders.
    /// </summary>
    [ImplementationSelector("WorkspaceDirectory", DefaultImplementation = "Default", IsGlobal = true)]
    public interface IWorkspaceManager
    {
        /// <summary>
        /// Gets the unique workspace name given the proposed name.
        /// </summary>
        /// <param name="proposedName">Proposed name.</param>
        /// <returns>Unique workspace name.</returns>
        string GetUniqueWorkspaceName(string proposedName);

        /// <summary>
        /// Gets the workspace path.
        /// </summary>
        /// <param name="workspaceName">Name of the workspace.</param>
        /// <returns>Path to the workspace directory.</returns>
        string GetWorkspacePath(string workspaceName);
    }
}
