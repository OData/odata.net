//---------------------------------------------------------------------
// <copyright file="VersionCalculationType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;

    /// <summary>
    /// Represents a enum for selecting what type of versioning calculation to be applied on EPM
    /// </summary>
    public enum VersionCalculationType
    {
        /// <summary>
        /// Metadata Enumeration Property
        /// </summary>
        Metadata,
        
        /// <summary>
        /// Request Enumeration
        /// </summary>
        Request,

        /// <summary>
        /// Response enumeration
        /// </summary>
        Response
    }
}