//---------------------------------------------------------------------
// <copyright file="IAstoriaStringResourceVerifiers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for getting astoria-specific resource string verifiers
    /// </summary>
    public interface IAstoriaStringResourceVerifiers
    {
        /// <summary>
        /// Gets the verifier for resources in Microsoft.OData.Service
        /// </summary>
        IStringResourceVerifier SystemDataServicesStringVerifier { get; }

        /// <summary>
        /// Gets the verifier for resources in Microsoft.OData.Client
        /// </summary>
        IStringResourceVerifier SystemDataServicesClientStringVerifier { get; }

        /// <summary>
        /// Gets the verifier for resources in Microsoft.OData.Core
        /// </summary>
        IStringResourceVerifier MicrosoftDataODataStringVerifier { get; }
    }
}
