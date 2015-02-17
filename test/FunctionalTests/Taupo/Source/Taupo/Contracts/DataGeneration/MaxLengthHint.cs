//---------------------------------------------------------------------
// <copyright file="MaxLengthHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents maximum length.
    /// </summary>
    public sealed class MaxLengthHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the MaxLengthHint class.
        /// </summary>
        /// <param name="value">The maximum length.</param>
        internal MaxLengthHint(int value)
            : base(value)
        {
            ExceptionUtilities.Assert(value >= 0, "Maximum length cannot be less than 0. Actual {0}.", value); 
        }
    }
}
