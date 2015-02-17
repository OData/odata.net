//---------------------------------------------------------------------
// <copyright file="TryGetNextSeedStrategies.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Strategies for the TryGetNextSeed.
    /// </summary>
    internal static class TryGetNextSeedStrategies
    {
        /// <summary>
        /// TryGetNextSeed delegate: nextSeed = seed + 1, returns false in case of overflow.
        /// </summary>
        /// <returns>TryGetNextSeed delegate.</returns>
        public static TryGetNextSeedStrategy Sequence()
        {
            return SequenceWithStep(1);
        }

        /// <summary>
        /// TryGetNextSeed delegate: nextSeed = seed + step, returns false in case of overflow.
        /// </summary>
        /// <param name="step">The sequence step.</param>
        /// <returns>TryGetNextSeed delegate.</returns>
        public static TryGetNextSeedStrategy SequenceWithStep(long step)
        {
            return delegate(long seed, out long nextSeed)
                {
                    return TryGetNextSeedInternal(seed, step, out nextSeed);
                };
        }

        /// <summary>
        /// TryGetNextSeed delegate: nextSeed = seed + step, returns false in case of overflow or if |nextSeed| > |limit|.
        /// </summary>
        /// <param name="step">The sequence step.</param>
        /// <param name="limit">The sequence limit.</param>
        /// <returns>TryGetNextSeed delegate.</returns>
        public static TryGetNextSeedStrategy SequenceWithStepAndLimit(long step, long limit)
        {
            return delegate(long seed, out long nextSeed)
            {
                if (!TryGetNextSeedInternal(seed, step, out nextSeed))
                {
                    return false;
                }

                if (Math.Abs(nextSeed) > Math.Abs(limit))
                {
                    nextSeed = seed;
                    return false;
                }

                return true;
            };
        }

        /// <summary>
        /// TryGetNextSeed delegate: nextSeed = seed + step, 
        /// when nextSeed reaches the limit resets the seed to the initialSeed.
        /// </summary>
        /// <param name="initialSeed">The seed to reset to when limit is reached.</param>
        /// <param name="limit">The sequence limit.</param>
        /// <param name="step">The sequence step.</param>
        /// <returns>TryGetNextSeed delegate.</returns>
        public static TryGetNextSeedStrategy RepeatedSequence(long initialSeed, long limit, long step)
        {
            if (Math.Abs(initialSeed) > Math.Abs(limit))
            {
                throw new TaupoArgumentException("Input arguments are invalid: initial seed cannot exceed the limit.");
            }

            return delegate(long seed, out long nextSeed)
            {
                if (!TryGetNextSeedInternal(seed, step, out nextSeed) || (Math.Abs(nextSeed) > Math.Abs(limit)))
                {
                   nextSeed = initialSeed;
                }

                return true;
            };
        }

        private static bool TryGetNextSeedInternal(long seed, long step, out long nextSeed)
        {
            try
            {
                nextSeed = checked(seed + step);
            }
            catch (OverflowException)
            {
                nextSeed = seed;
                return false;
            }

            return true;
        }
    }
}
