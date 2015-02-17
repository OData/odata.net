//---------------------------------------------------------------------
// <copyright file="AuthenticationMode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    /// <summary>
    /// Type which defines the various authentication modes for a Data Service
    /// </summary>
    public enum AuthenticationMode
    {
        /// <summary>
        /// No authentication mode required for the service
        /// </summary>
        None = 0,

        /// <summary>
        /// Use Basic authentication mode for the service
        /// </summary>
        Basic,

        /// <summary>
        /// Use Windows/NTLM authentication mode for the service
        /// </summary>
        Windows
    }
}
