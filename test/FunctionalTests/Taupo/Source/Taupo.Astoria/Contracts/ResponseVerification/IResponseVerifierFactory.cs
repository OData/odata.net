//---------------------------------------------------------------------
// <copyright file="IResponseVerifierFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification
{
    using System;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for a factory for building response verifiers
    /// </summary>
    [ImplementationSelector("ResponseVerifierFactory", DefaultImplementation = "Default")]
    public interface IResponseVerifierFactory
    {
        /// <summary>
        /// Constructs the standard request verifier for a successful request
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code of the response</param>
        /// <returns>The standard verifier(s) for a successful request</returns>
        IResponseVerifier GetStandardVerifier(HttpStatusCode expectedStatusCode);

        /// <summary>
        /// Constructs the error request verifier for a invalid request
        /// </summary>
        /// <param name="expectedStatusCode">The expected status code of the response</param>
        /// <param name="expectedMessage">The expected error message</param>
        /// <param name="resourceVerifier">The resource verifier for the error message</param>
        /// <returns>The error verifier(s) for a error request</returns>
        IResponseVerifier GetErrorVerifier(HttpStatusCode expectedStatusCode, ExpectedErrorMessage expectedMessage, IStringResourceVerifier resourceVerifier);
    }
}
