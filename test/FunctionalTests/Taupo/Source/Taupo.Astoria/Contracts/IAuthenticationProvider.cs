//---------------------------------------------------------------------
// <copyright file="IAuthenticationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Interface allows for a test to request an authentication provider which it can use to authenticate against
    /// a Data Service
    /// </summary>
    [ImplementationSelector("AuthenticationProvider", DefaultImplementation = "Anonymous", HelpText = "The authentication provider to use against the test data service")]
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Gets a value indicating whether the caller should use default credentials
        /// </summary>
        bool UseDefaultCredentials { get; }

        /// <summary>
        /// Gets the Http Headers for header based authentication mechanisms
        /// </summary>
        /// <returns>The Http headers to be injected into the request headers</returns>
        IDictionary<string, string> GetAuthenticationHeaders();

        /// <summary>
        /// Gets the authentication credentials required for NTLM based authentication mechanisms
        /// </summary>
        /// <returns>NTLM authentication credentials</returns>
        ICredentials GetAuthenticationCredentials();
    }
}