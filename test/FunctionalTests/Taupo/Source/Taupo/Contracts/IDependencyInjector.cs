//---------------------------------------------------------------------
// <copyright file="IDependencyInjector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System;

    /// <summary>
    /// Injects ambient dependencies into object.
    /// </summary>
    public interface IDependencyInjector
    {
        /// <summary>
        /// Resolves concrete type for a given contract type, constructs and returns the instance.
        /// </summary>
        /// <typeparam name="TContract">Contract type.</typeparam>
        /// <returns>
        /// Fully resolved object that implements the contract
        /// </returns>
        TContract Resolve<TContract>() where TContract : class;

        /// <summary>
        /// Resolves concrete type for a given contract type, constructs and returns the instance.
        /// </summary>
        /// <param name="contractType">Contract type.</param>
        /// <returns>
        /// Fully resolved object that implements the contract
        /// </returns>
        object Resolve(Type contractType);

        /// <summary>
        /// Injects dependencies into the specified object.
        /// </summary>
        /// <typeparam name="TObject">Type of the target object.</typeparam>
        /// <param name="targetObject">The target object.</param>
        /// <returns>Target object.</returns>
        TObject InjectDependenciesInto<TObject>(TObject targetObject)
            where TObject : class;
    }
}
