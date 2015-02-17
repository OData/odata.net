//---------------------------------------------------------------------
// <copyright file="MinValueHint`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;
    
    /// <summary>
    /// Represents the minimum value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class MinValueHint<TValue> : DataGenerationHint<TValue> where TValue : IComparable<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the MinValueHint class.
        /// </summary>
        /// <param name="value">The minimum value.</param>
        internal MinValueHint(TValue value)
            : base(value)
        {
        }
    }
}
