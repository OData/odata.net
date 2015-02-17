//---------------------------------------------------------------------
// <copyright file="NumericPrecisionFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Precision of a primitive type (such as decimal or date/time).
    /// </summary>
    public class NumericPrecisionFacet : PrimitiveDataTypeFacet<int>
    {
        /// <summary>
        /// Initializes a new instance of the NumericPrecisionFacet class.
        /// </summary>
        /// <param name="value">The value.</param>
        public NumericPrecisionFacet(int value)
            : base(value)
        {
        }
    }
}
