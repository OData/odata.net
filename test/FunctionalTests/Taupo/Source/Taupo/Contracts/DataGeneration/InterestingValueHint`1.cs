//---------------------------------------------------------------------
// <copyright file="InterestingValueHint`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Data generation hint representing a particularly interesting value that should be generated with higher frequency.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class InterestingValueHint<TValue> : DataGenerationHint<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the InterestingValueHint class.
        /// </summary>
        /// <param name="value">The value of the hint.</param>
        public InterestingValueHint(TValue value)
            : base(value)
        {
        }
    }
}