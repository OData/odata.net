//---------------------------------------------------------------------
// <copyright file="WorkspaceEventArgs`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;

    /// <summary>
    /// Provides arguments to events during workspace creation.
    /// </summary>
    /// <typeparam name="TWorkspace">The type of the workspace.</typeparam>
    public sealed class WorkspaceEventArgs<TWorkspace> : EventArgs
        where TWorkspace : Workspace
    {
        /// <summary>
        /// Initializes a new instance of the WorkspaceEventArgs class.
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public WorkspaceEventArgs(TWorkspace workspace)
        {
            this.Workspace = workspace;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event has been handled.
        /// </summary>
        /// A value of <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets the workspace.
        /// </summary>
        /// <value>The workspace.</value>
        public TWorkspace Workspace { get; private set; }
    }
}