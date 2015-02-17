//---------------------------------------------------------------------
// <copyright file="IsUnicodeFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Determines whether a primitive type (typically a string) supports Unicode characters.
    /// </summary>
    public class IsUnicodeFacet : PrimitiveDataTypeFacet<bool>
    {
        /// <summary>
        /// Initializes a new instance of the IsUnicodeFacet class.
        /// </summary>
        /// <param name="value">If set to <c>true</c> the type supports Unicode characters, false otherwise.</param>
        public IsUnicodeFacet(bool value)
            : base(value)
        {
        }
    }
}
