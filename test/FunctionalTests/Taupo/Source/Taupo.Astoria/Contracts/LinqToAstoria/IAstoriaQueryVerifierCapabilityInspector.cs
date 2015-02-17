//---------------------------------------------------------------------
// <copyright file="IAstoriaQueryVerifierCapabilityInspector.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Inspects the QueryVerifier to understand what level of capability it has
    /// </summary>
    [ImplementationSelector("AstoriaQueryVerifierCapabilityInspector", DefaultImplementation = "Default")]
    public interface IAstoriaQueryVerifierCapabilityInspector
    {
        /// <summary>
        /// Indicates the QueryVerifier runs on the client
        /// </summary>
        /// <returns>True if it executes on the client</returns>
        bool ExecutesOnDataServicesClient();

        /// <summary>
        /// Indicates that this query execution method supports projection when the keys aren't specified
        /// </summary>
        /// <returns>returns true if the executor supports projections with out keys</returns>
        bool SupportsProjectionWithoutKeys();
    }
}