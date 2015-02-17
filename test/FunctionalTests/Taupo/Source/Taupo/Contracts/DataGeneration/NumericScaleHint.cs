//---------------------------------------------------------------------
// <copyright file="NumericScaleHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents a numeric scale.
    /// </summary>
    public sealed class NumericScaleHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the NumericScaleHint class.
        /// </summary>
        /// <param name="value">The numeric scale.</param>
        internal NumericScaleHint(int value)
            : base(value)
        {
        }
    }
}
