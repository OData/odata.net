//---------------------------------------------------------------------
// <copyright file="IRandomNumberGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    
    /// <summary>
    /// Generates random numbers.
    /// </summary>
    [ImplementationSelector("RNG", DefaultImplementation = "Default", IsGlobal = true)]
    public interface IRandomNumberGenerator
    {
        /// <summary>
        /// Gets or sets the intial seed of the Random number generator
        /// </summary>
        int InitialSeed { get; set; }

        /// <summary>
        /// Returns a random value between 0 and <paramref name="maxValue"/> - 1.
        /// </summary>
        /// <param name="maxValue">maximum value</param>
        /// <returns>Random value beween 0 and <paramref name="maxValue"/> - 1.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "Aligned with Random.Next()")]
        int Next(int maxValue);

        /// <summary>
        /// Returns a random sequence of bytes
        /// </summary>
        /// <param name="length">The length</param>
        /// <returns>Random sequence of bytes</returns>
        byte[] NextBytes(int length);
    }
}
