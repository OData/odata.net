//---------------------------------------------------------------------
// <copyright file="IPayloadTransformationScope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;

    /// <summary>
    /// Contract for a fluent API for adding/removing payload transformations programmatically.
    /// </summary>
    public interface IPayloadTransformationScope
    {
        /// <summary>
        /// Adds the transform to the scope.
        /// </summary>
        /// <typeparam name="TPayload">The payload type for the transform.</typeparam>
        /// <param name="transform">The transform to add.</param>
        void Add<TPayload>(IPayloadTransform<TPayload> transform);

        /// <summary>
        /// Removes matching transforms from the scope.
        /// </summary>
        /// <typeparam name="TPayload">The payload type for the transforms.</typeparam>
        /// <param name="predicate">The predicate to use to find matching transforms.</param>
        void RemoveAll<TPayload>(Predicate<IPayloadTransform<TPayload>> predicate);

        /// <summary>
        /// Applies the current scope for the lifespan of the returned disposable. Disposing the returned object will terminate the current scope.
        /// </summary>
        /// <returns>An IDisposable object which helps in applying a scope and then disposing it once used.</returns>
        IDisposable Apply();
    }
}
