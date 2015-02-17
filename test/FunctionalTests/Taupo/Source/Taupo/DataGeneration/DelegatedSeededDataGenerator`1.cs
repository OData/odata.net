//---------------------------------------------------------------------
// <copyright file="DelegatedSeededDataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Seeded data generator that uses 'FromSeed' and 'TryGetNextSeed' delegates to generate data.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    internal class DelegatedSeededDataGenerator<TData> : DataGenerator<TData>
    {
        private long seed;
        private bool isCapacityReached;
        private Func<long, TData> fromSeed;
        private TryGetNextSeedStrategy tryGetNextSeed;

        /// <summary>
        /// Initializes a new instance of the DelegatedSeededDataGenerator class.
        /// Uses sequential strategy with step 1 to get next seed (i.e. nextSeed = seed + 1).
        /// </summary>
        /// <param name="initialSeed">The initial seed.</param>
        /// <param name="fromSeedDelegate">FromSeed delegate.</param>
        public DelegatedSeededDataGenerator(long initialSeed, Func<long, TData> fromSeedDelegate)
            : this(initialSeed, fromSeedDelegate, TryGetNextSeedStrategies.Sequence())
        {
        }

        /// <summary>
        /// Initializes a new instance of the DelegatedSeededDataGenerator class.
        /// </summary>
        /// <param name="initialSeed">The initial seed.</param>
        /// <param name="fromSeedDelegate">FromSeed delegate.</param>
        /// <param name="tryGetNextSeedDelegate">TryGetNextSeed delegate.</param>
        public DelegatedSeededDataGenerator(long initialSeed, Func<long, TData> fromSeedDelegate, TryGetNextSeedStrategy tryGetNextSeedDelegate)
        {
            ExceptionUtilities.CheckArgumentNotNull(fromSeedDelegate, "fromSeedDelegate");
            ExceptionUtilities.CheckArgumentNotNull(tryGetNextSeedDelegate, "tryGetNextSeedDelegate");

            this.seed = initialSeed;
            this.fromSeed = fromSeedDelegate;
            this.tryGetNextSeed = tryGetNextSeedDelegate;
        }

        /// <summary>
        /// Generates data based on the current seed.
        /// </summary>
        /// <returns>Generated data.</returns>
        /// <exception cref="TaupoInvalidOperationException">
        /// When generator capacity is reached and no more values can be generated.
        /// </exception>
        public override TData GenerateData()
        {
            if (this.isCapacityReached)
            {
                throw new TaupoInvalidOperationException("Generator capacity is reached: no more values can be generated.");
            }

            TData data = this.fromSeed(this.seed);

            long nextSeed;
            if (this.tryGetNextSeed(this.seed, out nextSeed))
            {
                this.seed = nextSeed;
            }
            else
            {
                this.isCapacityReached = true;
            }

            return data;
        }
    }
}
