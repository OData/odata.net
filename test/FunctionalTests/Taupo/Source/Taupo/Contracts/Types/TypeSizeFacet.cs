//---------------------------------------------------------------------
// <copyright file="TypeSizeFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Size of a type (in bits)
    /// </summary>
    public class TypeSizeFacet : PrimitiveDataTypeFacet<int>
    {
        /// <summary>
        /// Initializes a new instance of the TypeSizeFacet class.
        /// </summary>
        /// <param name="value">The value.</param>
        public TypeSizeFacet(int value)
            : base(value)
        {
        }
    }
}
