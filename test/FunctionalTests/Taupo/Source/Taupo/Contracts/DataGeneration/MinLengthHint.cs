//---------------------------------------------------------------------
// <copyright file="MinLengthHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents minimum length.
    /// </summary>
    public sealed class MinLengthHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the MinLengthHint class.
        /// </summary>
        /// <param name="value">The minimum length.</param>
        internal MinLengthHint(int value)
            : base(value)
        {
            ExceptionUtilities.Assert(value >= 0, "Minimum length cannot be less than 0. Actual {0}.", value); 
        }
    }
}
