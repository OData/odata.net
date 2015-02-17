//---------------------------------------------------------------------
// <copyright file="NumericPrecisionHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a numeric precision.
    /// </summary>
    public sealed class NumericPrecisionHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the NumericPrecisionHint class.
        /// </summary>
        /// <param name="value">The numeric precision.</param>
        internal NumericPrecisionHint(int value)
            : base(value)
        {
        }
    }
}
