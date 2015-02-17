//---------------------------------------------------------------------
// <copyright file="IWorkspaceBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Builds a workspace to be used by tests.
    /// </summary>
    public interface IWorkspaceBuilder
    {
        /// <summary>
        /// Builds the workspace from the specified model.
        /// </summary>
        /// <param name="modelSchema">The model schema.</param>
        /// <returns>
        /// Returns fully initialized instance of a class which derives from <see cref="Workspace"/>.
        /// </returns>
        Workspace BuildWorkspace(EntityModelSchema modelSchema);
    }
}