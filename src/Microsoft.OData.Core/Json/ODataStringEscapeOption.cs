//---------------------------------------------------------------------
// <copyright file="ODataStringEscapeOption.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    /// <summary>
    /// Enum type to specify how to escape string value when writing JSON text.
    /// </summary>
    public enum ODataStringEscapeOption
    {
        /// <summary>
        /// All non-ASCII and control characters (e.g. newline) are escaped.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Ascii is meaningful")]
        EscapeNonAscii,

        /// <summary>
        /// Only control characters (e.g. newline) are escaped.
        /// </summary>
        EscapeOnlyControls
    }
}
