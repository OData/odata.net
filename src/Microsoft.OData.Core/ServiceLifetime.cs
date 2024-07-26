//---------------------------------------------------------------------
// <copyright file="ServiceLifetime.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData
{
    /// <summary>
    /// Enumerates all kinds of lifetime of a service in an <see cref="IServiceProvider"/>.
    /// </summary>
    public enum ServiceLifetime
    {
        /// <summary>
        /// Indicates that a single instance of the service will be created.
        /// </summary>
        Singleton,

        /// <summary>
        /// Indicates that a new instance of the service will be created for each scope.
        /// </summary>
        Scoped,

        /// <summary>
        /// Indicates that a new instance of the service will be created every time it is requested.
        /// </summary>
        Transient
    }
}
