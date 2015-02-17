//---------------------------------------------------------------------
// <copyright file="FractionalSecondsHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Represents fraction seconds.
    /// </summary>
    public sealed class FractionalSecondsHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the FractionalSecondsHint class.
        /// </summary>
        /// <param name="value">Fractional seconds.</param>
        internal FractionalSecondsHint(int value)
            : base(value)
        {
        }
    }
}
