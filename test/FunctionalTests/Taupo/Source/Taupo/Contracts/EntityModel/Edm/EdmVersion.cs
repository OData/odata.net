//---------------------------------------------------------------------
// <copyright file="EdmVersion.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    /// <summary>
    /// Version of EDM to use
    /// </summary>
    public enum EdmVersion
    {
        /// <summary>
        /// Version 4.0
        /// </summary>
        V40,

        /// <summary>
        /// Latest version
        /// </summary>
        Latest = V40
    }
}
