//---------------------------------------------------------------------
// <copyright file="TryGetNextSeedStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    /// <summary>
    /// Delegate that tries to get next seed based on the specified seed and returns true if succeeds.
    /// </summary>
    /// <returns>True if succeeds false otherwise. If false returned the nextSeed is set to the seed.</returns>
    /// <param name="seed">The current seed.</param>
    /// <param name="nextSeed">The next seed.</param>
    internal delegate bool TryGetNextSeedStrategy(long seed, out long nextSeed);
}
