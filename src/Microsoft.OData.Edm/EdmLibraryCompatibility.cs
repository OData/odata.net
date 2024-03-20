//---------------------------------------------------------------------
// <copyright file="EdmLibraryCompatibility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents compatibility settings for the EDM (Entity Data Model) library.
    /// </summary>
    [Flags]
    public enum EdmLibraryCompatibility
    {
        /// <summary>
        /// No flags set. Represents the base or default state.
        /// </summary>
        None = 0,

        /// <summary>
        /// When enabled, the CSDL Scale and SRID "variable" value will be written "Variable" instead of "variable" for compatibility with older versions of the library.
        /// </summary>
        UseLegacyVariableCasing = 1 << 0
    }
}
