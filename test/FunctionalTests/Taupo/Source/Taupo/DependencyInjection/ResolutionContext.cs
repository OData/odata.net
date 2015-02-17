//---------------------------------------------------------------------
// <copyright file="ResolutionContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;

    /// <summary>
    /// Context of the dependency resolution.
    /// </summary>
    internal class ResolutionContext
    {
        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public LightweightDependencyInjectionContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the target type whose dependency is being resolved.
        /// </summary>
        /// <value>Target type.</value>
        public Type TargetType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw on error.
        /// </summary>
        /// <value>A value of <c>true</c> if the resolver should throw on error; otherwise, <c>false</c>.</value>
        public bool ThrowOnError { get; set; }
    }
}
