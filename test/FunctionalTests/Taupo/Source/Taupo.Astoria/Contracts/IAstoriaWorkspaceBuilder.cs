//---------------------------------------------------------------------
// <copyright file="IAstoriaWorkspaceBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for building astoria workspaces.
    /// </summary>
    [ImplementationSelector("AstoriaWorkspaceBuilder", DefaultImplementation = "Default")]
    public interface IAstoriaWorkspaceBuilder : IWorkspaceBuilder
    {
        /// <summary>
        /// Occurs when the model has been generated and can be customized to add Entity Types and ComplexTypes
        /// </summary>
        /// <remarks>
        /// Handlers of this event can manipulate contents of <see cref="Workspace.ConceptualModel"/>
        /// and should set Handled property to true.
        /// </remarks>
        event EventHandler<WorkspaceEventArgs<Workspace>> CustomizeModelBeforeDefaultFixups;

        /// <summary>
        /// Occurs when the model has been generated and can be customized after the default fixups have been run
        /// </summary>
        /// <remarks>
        /// Handlers of this event can manipulate contents of <see cref="Workspace.ConceptualModel"/>
        /// and should set Handled property to true.
        /// </remarks>
        event EventHandler<WorkspaceEventArgs<Workspace>> CustomizeModelAfterDefaultFixups;

        /// <summary>
        /// Gets or sets a value indicating whether the builder should skip generating client code
        /// </summary>
        bool SkipClientCodeGeneration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the builder should skip downloading the initial service data
        /// </summary>
        bool SkipDataDownload { get; set; }
    }
}
