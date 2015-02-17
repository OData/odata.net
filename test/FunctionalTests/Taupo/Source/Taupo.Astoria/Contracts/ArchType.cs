//---------------------------------------------------------------------
// <copyright file="ArchType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    /// <summary>
    /// ArchType enumeration is used as input for the IWebServerLookup
    /// Each of the enumerations maps to a hardware architecture type 
    /// that is in the lab machines
    /// </summary>
    public enum ArchType
    {
        /// <summary>
        /// Reflects a machine with x86 arch
        /// </summary>
        X86,
        
        /// <summary>
        /// Reflects a machine with ia64 arch
        /// </summary>
        IA64,

        /// <summary>
        /// Reflects a machine with x64 arch
        /// </summary>
        Amd64,
    }
}