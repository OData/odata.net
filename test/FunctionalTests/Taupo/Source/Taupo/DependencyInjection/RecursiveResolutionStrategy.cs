//---------------------------------------------------------------------
// <copyright file="RecursiveResolutionStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DependencyInjection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Attempts to resolve a dependency by invoking constructor of the implementation
    /// type and recursively resolving all constructor arguments.
    /// </summary>
    internal class RecursiveResolutionStrategy : IResolutionStrategy
    {
        private Type implementationType;

        /// <summary>
        /// Initializes a new instance of the RecursiveResolutionStrategy class.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        public RecursiveResolutionStrategy(Type implementationType)
        {
            ExceptionUtilities.CheckArgumentNotNull(implementationType, "implementationType");
            this.implementationType = implementationType;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// Value of <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            RecursiveResolutionStrategy other = obj as RecursiveResolutionStrategy;
            if (other == null)
            {
                return false;
            }

            return this.implementationType == other.implementationType;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.implementationType.GetHashCode() ^ 7;
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
            foreach (ConstructorInfo constructorInfo in this.implementationType.GetInstanceConstructors(true))
            {
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
                object[] parameterValues = new object[parameterInfos.Length];

                bool failed = false;

                for (int i = 0; i < parameterInfos.Length; ++i)
                {
                    if (!context.Container.TryCreateObject(this.implementationType, parameterInfos[i].ParameterType, context.ThrowOnError, out parameterValues[i]))
                    {
                        failed = true;
                        break;
                    }
                }

                if (!failed)
                {
                    resolvedObject = constructorInfo.Invoke(parameterValues);
                    foreach (object o in parameterValues)
                    {
                        context.Container.RegisterDependency(resolvedObject, o);
                    }

                    return true;
                }
            }

            resolvedObject = null;
            if (context.ThrowOnError)
            {
                throw new TaupoInvalidOperationException("No appropriate constructor on " + this.implementationType + " could be called.");
            }

            return false;
        }
    }
}
