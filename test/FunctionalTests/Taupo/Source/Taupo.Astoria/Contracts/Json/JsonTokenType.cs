//---------------------------------------------------------------------
// <copyright file="JsonTokenType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Json
{
    using System;

    /// <summary>
    /// Specifies the type of Json token.
    /// </summary>
    public enum JsonTokenType
    {
        /// <summary>
        /// Zero Value
        /// </summary>
        None,

        /// <summary>
        /// A numeric value token..
        /// </summary>
        Integer,

        /// <summary>
        /// A floating point value token..
        /// </summary>
        Float,

        /// <summary>
        /// A string name or value token.
        /// </summary>
        String,

        /// <summary>
        /// A bool true token.
        /// </summary>
        True,

        /// <summary>
        /// A bool false token.
        /// </summary>
        False,

        /// <summary>
        /// A null value token.
        /// </summary>
        Null,

        /// <summary>
        /// An array start token.
        /// </summary>
        LeftSquareBracket,

        /// <summary>
        /// An array end token.
        /// </summary>
        RightSquareBracket,

        /// <summary>
        /// An object start token.
        /// </summary>
        LeftCurly,

        /// <summary>
        /// An object end token.
        /// </summary>
        RightCurly,

        /// <summary>
        /// An separator token.
        /// </summary>
        Comma,

        /// <summary>
        /// Name value pair separator
        /// </summary>
        Colon,

        /// <summary>
        /// Date of type Byte Array
        /// </summary>
        ByteArray,
    }
}
