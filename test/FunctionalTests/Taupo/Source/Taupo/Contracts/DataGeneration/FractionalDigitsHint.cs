//---------------------------------------------------------------------
// <copyright file="FractionalDigitsHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents fractional digits.
    /// </summary>
    public sealed class FractionalDigitsHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the FractionalDigitsHint class.
        /// </summary>
        /// <param name="value">Fractional digits.</param>
        internal FractionalDigitsHint(int value)
            : base(value)
        {
        }
    }
}
