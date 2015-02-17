//---------------------------------------------------------------------
// <copyright file="ISpecificExceptionMessageVerifierFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for building verifiers for specific exceptions for which resource identifiers are unknown.
    /// </summary>
    [ImplementationSelector("SpecificExceptionMessageVerifierFactory", DefaultImplementation = "Default")]
    public interface ISpecificExceptionMessageVerifierFactory
    {
        /// <summary>
        /// Creates a verifier for the given exception.
        /// </summary>
        /// <param name="actualException">The actual exception.</param>
        /// <returns>
        /// A verifier specific to the exception.
        /// </returns>
        IStringResourceVerifier CreateVerifier(Exception actualException);
    }
}
