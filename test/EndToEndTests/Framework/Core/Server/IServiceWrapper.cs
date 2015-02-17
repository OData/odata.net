//---------------------------------------------------------------------
// <copyright file="IServiceWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Server
{
    using System;

    /// <summary>
    /// Interface for test service abstraction.
    /// </summary>
    public interface IServiceWrapper
    {
        /// <summary>
        /// Gets the URI for the service.
        /// </summary>
        Uri ServiceUri { get; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        void StartService();

        /// <summary>
        /// Stops the service.
        /// </summary>
        void StopService();
    }
}
