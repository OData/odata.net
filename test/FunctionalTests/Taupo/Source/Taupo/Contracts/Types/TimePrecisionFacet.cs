//---------------------------------------------------------------------
// <copyright file="TimePrecisionFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Precision of a date/time type.
    /// </summary>
    public class TimePrecisionFacet : PrimitiveDataTypeFacet<int>
    {
        /// <summary>
        /// Initializes a new instance of the TimePrecisionFacet class.
        /// </summary>
        /// <param name="value">The value.</param>
        public TimePrecisionFacet(int value)
            : base(value)
        {
        }
    }
}
