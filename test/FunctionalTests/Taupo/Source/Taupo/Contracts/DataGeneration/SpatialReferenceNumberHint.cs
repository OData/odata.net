//---------------------------------------------------------------------
// <copyright file="SpatialReferenceNumberHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a hint to generate spatial type data in a specific SRID.
    /// </summary>
    public sealed class SpatialReferenceNumberHint : DataGenerationHint<int>
    {
        /// <summary>
        /// Initializes a new instance of the SpatialReferenceNumberHint class.
        /// </summary>
        /// <param name="value">The SRID representation of the hint.</param>
        internal SpatialReferenceNumberHint(int value)
            : base(value)
        {
        }
    }
}
