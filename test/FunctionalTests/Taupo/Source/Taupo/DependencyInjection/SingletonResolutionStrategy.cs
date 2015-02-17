//---------------------------------------------------------------------
// <copyright file="SingletonResolutionStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;

    /// <summary>
    /// Simple resolution strategy which always returns a single object.
    /// </summary>
    internal class SingletonResolutionStrategy : IResolutionStrategy
    {
        private object implementationObject;

        /// <summary>
        /// Initializes a new instance of the SingletonResolutionStrategy class.
        /// </summary>
        /// <param name="implementationObject">The implementation object.</param>
        public SingletonResolutionStrategy(object implementationObject)
        {
            this.implementationObject = implementationObject;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// Value <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            SingletonResolutionStrategy other = obj as SingletonResolutionStrategy;
            if (other == null)
            {
                return false;
            }

            return this.implementationObject == other.implementationObject;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.implementationObject.GetHashCode() ^ 9;
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
            resolvedObject = this.implementationObject;
            return true;
        }
    }
}
