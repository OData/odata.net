//---------------------------------------------------------------------
// <copyright file="IContainerProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// An interface that provides a dependency injection container.
    /// </summary>
    public interface IContainerProvider
    {
        /// <summary>
        /// Gets a container which implements <see cref="IServiceProvider"/> and contains
        /// all the services registered.
        /// </summary>
        IServiceProvider Container { get; }
    }
}
