//---------------------------------------------------------------------
// <copyright file="WindowsAuthenticationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Provides Windows authentication support for test request calls to the Data Service
    /// </summary>
    [ImplementationName(typeof(IAuthenticationProvider), "Windows")]
    public class WindowsAuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// Gets a value indicating whether the caller should use default credentials
        /// </summary>
        public bool UseDefaultCredentials
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the Http Headers for header based authentication mechanisms
        /// </summary>
        /// <returns>The Http headers to be injected into the request headers</returns>
        public IDictionary<string, string> GetAuthenticationHeaders()
        {
            return null;
        }

        /// <summary>
        /// Gets the authentication credentials required for NTLM based authentication mechanisms
        /// </summary>
        /// <returns>NTLM authentication credentials</returns>
        public ICredentials GetAuthenticationCredentials()
        {
            return null;
        }
    }
}
