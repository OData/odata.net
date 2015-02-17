//---------------------------------------------------------------------
// <copyright file="TrustLevel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    /// <summary>
    /// Specifies a trust level of interest.
    /// </summary>
    public enum TrustLevel
    {
        /// <summary>
        /// Medium trust, whose permission set is dictated by
        /// the web_mediumtrust.config file in the framework directory.
        /// </summary>
        Medium = 0,

        /// <summary>
        /// Full trust. All permissions.
        /// </summary>
        Full = 1
    }
}
