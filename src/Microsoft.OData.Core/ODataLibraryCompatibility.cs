//---------------------------------------------------------------------
// <copyright file="ODataLibraryCompatibility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData
{
    /// <summary>
    /// Library compatibility levels.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum ODataLibraryCompatibility
    {
        /// <summary>
        /// Version 6.x
        /// </summary>
        Version6 = 060000,

        /// <summary>
        /// Version 7.x
        /// </summary>
        Version7 = 070000,

        /// <summary>
        /// The latest version of the library.
        /// </summary>
        Latest = int.MaxValue
    }
}
