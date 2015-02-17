//---------------------------------------------------------------------
// <copyright file="IDataServiceContextEventVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface for context event verifiers that need to be added to or removed from the context's events
    /// </summary>
    public interface IDataServiceContextEventVerifier
    {
        /// <summary>
        /// Registers this verifier on the context's event
        /// </summary>
        /// <param name="context">The context to verify events on</param>
        void RegisterEventHandler(DataServiceContext context);

        /// <summary>
        /// Unregisters this verifier from the context's event
        /// </summary>
        /// <param name="context">The context to stop verifing events on</param>
        /// <param name="inErrorState">A value indicating that we are recovering from an error</param>
        void UnregisterEventHandler(DataServiceContext context, bool inErrorState);
    }
}
