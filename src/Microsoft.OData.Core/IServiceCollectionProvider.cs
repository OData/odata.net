//---------------------------------------------------------------------
// <copyright file="IServiceCollectionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// An interface that provides a dependency injection <see cref="IServiceProvider"./>
    /// </summary>
    public interface IServiceCollectionProvider
    {
        /// <summary>
        /// Gets a <see cref="IServiceProvider"/> that contains
        /// all the services registered.
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}
