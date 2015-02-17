//---------------------------------------------------------------------
// <copyright file="DependencyResolutionOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Options which impact dependency resolution during dependency injection.
    /// </summary>
    public sealed class DependencyResolutionOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether this dependency is transient.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </value>
        public bool IsTransient { get; set; }

        /// <summary>
        /// Marks the dependency as transient - created instances will not be saved, but new
        /// instance of the dependency will be created.
        /// </summary>
        /// <returns>This object (suitable for chaining calls together)</returns>
        public DependencyResolutionOptions Transient()
        {
            this.IsTransient = true;
            return this;
        }
    }
}
