//---------------------------------------------------------------------
// <copyright file="MaxLengthFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Specifies maximum length of values of the type can hold (typically for strings and binary types).
    /// </summary>
    public class MaxLengthFacet : PrimitiveDataTypeFacet<int>
    {
        /// <summary>
        /// Initializes a new instance of the MaxLengthFacet class.
        /// </summary>
        /// <param name="maximumLength">Maximum length.</param>
        public MaxLengthFacet(int maximumLength)
            : base(maximumLength)
        {
        }
    }
}
