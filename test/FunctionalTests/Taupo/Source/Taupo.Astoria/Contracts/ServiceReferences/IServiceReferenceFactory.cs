//---------------------------------------------------------------------
// <copyright file="IServiceReferenceFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences
{
    using System;
    using System.ServiceModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Factory contract for service references
    /// </summary>
    [ImplementationSelector("ServiceReferenceFactory", DefaultImplementation = "Default")]
    public interface IServiceReferenceFactory
    {
        /// <summary>
        /// Creates an instance of the given service reference client for a service with the given uri
        /// </summary>
        /// <typeparam name="TClient">The service reference client type</typeparam>
        /// <param name="serviceUri">The service uri</param>
        /// <returns>An instance of the client type</returns>
        TClient CreateInstance<TClient>(Uri serviceUri);
    }
}