//---------------------------------------------------------------------
// <copyright file="MaxValueHint`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;

    /// <summary>
    /// Represents maximum value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class MaxValueHint<TValue> : DataGenerationHint<TValue> where TValue : IComparable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the MaxValueHint class.
        /// </summary>
        /// <param name="value">The maximum value.</param>
        internal MaxValueHint(TValue value)
            : base(value)
        {
        }
    }
}
