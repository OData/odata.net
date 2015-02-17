//---------------------------------------------------------------------
// <copyright file="IResolutionStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Resolution strategy for dependency.
    /// </summary>
    internal interface IResolutionStrategy
    {
        /// <summary>
        /// Tries to resolve dependency in a given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="resolvedObject">The resolved object.</param>
        /// <returns>True if the resolution succeeded, false otherwise.</returns>
        bool TryResolve(ResolutionContext context, out object resolvedObject);
    }
}