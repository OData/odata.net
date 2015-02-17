//---------------------------------------------------------------------
// <copyright file="DataGenerationHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;

    /// <summary>
    /// Base class to represent a hint for data generation.
    /// </summary>
    public abstract class DataGenerationHint : IEquatable<DataGenerationHint>
    {
        /// <summary>
        /// Determines whether this data generation hint is equal to other data generation hint.
        /// </summary>
        /// <param name="other">The other data generation hint.</param>
        /// <returns>True if this <see cref="DataGenerationHint"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public abstract bool Equals(DataGenerationHint other);
    }
}
