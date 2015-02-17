//---------------------------------------------------------------------
// <copyright file="NumericScaleFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Scale of a primitive type (such as decimal).
    /// </summary>
    public class NumericScaleFacet : PrimitiveDataTypeFacet<int>
    {
        /// <summary>
        /// Initializes a new instance of the NumericScaleFacet class.
        /// </summary>
        /// <param name="value">The value.</param>
        public NumericScaleFacet(int value)
            : base(value)
        {
        }
    }
}
