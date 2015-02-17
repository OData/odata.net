//---------------------------------------------------------------------
// <copyright file="CustomResolutionStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;

    /// <summary>
    /// Dependency resolution strategy using custom function.
    /// </summary>
    internal class CustomResolutionStrategy : IResolutionStrategy
    {
        private Func<Type, object> resolver;

        /// <summary>
        /// Initializes a new instance of the CustomResolutionStrategy class.
        /// </summary>
        /// <param name="resolver">The resolver function.</param>
        public CustomResolutionStrategy(Func<Type, object> resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// Tries to resolve dependency in a given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="resolvedObject">The resolved object.</param>
        /// <returns>
        /// True if the resolution succeeded, false otherwise.
        /// </returns>
        public bool TryResolve(ResolutionContext context, out object resolvedObject)
        {
            resolvedObject = this.resolver(context.TargetType);
            return resolvedObject != null;
        }
    }
}
