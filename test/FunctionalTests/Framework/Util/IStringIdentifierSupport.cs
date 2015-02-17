//---------------------------------------------------------------------
// <copyright file="IStringIdentifierSupport.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    /// <summary>
    /// Interface implemented by all classes that support providing
    /// a string identifier.
    /// </summary>
    public interface IStringIdentifierSupport
    {
        /// <summary>Non-null string identifier for this instance.</summary>
        /// <remarks>
        /// The string identifier obeys most rules of a C# identifier:
        /// no leading numbers, no whitespace, no punctuation, no
        /// symbols, no control characters. It is also non-zero length.
        /// </remarks>
        string StringIdentifier { get; }
    }
}
