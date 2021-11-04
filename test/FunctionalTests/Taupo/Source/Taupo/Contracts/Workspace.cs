//---------------------------------------------------------------------
// <copyright file="Workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Target workspace.
    /// </summary>
    public abstract class Workspace : IDisposable
    {
        /// <summary>
        /// Occurs when the workspace gets disposed.
        /// </summary>
        public event EventHandler OnDispose;

        /// <summary>
        /// Gets or sets the conceptual model.
        /// </summary>
        /// <value>The conceptual model.</value>
        public EntityModelSchema ConceptualModel { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public IProgrammingLanguageStrategy Language { get; set; }

        /// <summary>
        /// Gets or sets the workspace directory.
        /// </summary>
        /// <value>The workspace directory.</value>
        public string WorkspaceDirectory { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.OnDispose != null)
                {
                    this.OnDispose(this, null);
                    this.OnDispose = null;
                }
            }
        }
    }
}
