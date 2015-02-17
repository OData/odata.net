//---------------------------------------------------------------------
// <copyright file="DelegatedRandomDataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Random data generator that uses delegates to generate data.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class DelegatedRandomDataGenerator<TData> : DataGenerator<TData>
    {
        private IRandomNumberGenerator random;
        private Func<IRandomNumberGenerator, TData>[] generateDataDelegates;
        private TData[] interestingData;
        private long interestingDataCount;

        /// <summary>
        /// Initializes a new instance of the DelegatedRandomDataGenerator class.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        /// <param name="generateDataDelegate">Delegate to use when generating data.</param>
        /// <param name="interestingData">Interesting data that should be generated randomly.</param>
        public DelegatedRandomDataGenerator(IRandomNumberGenerator random, Func<IRandomNumberGenerator, TData> generateDataDelegate, params TData[] interestingData)
            : this(random, new Func<IRandomNumberGenerator, TData>[] { generateDataDelegate }, interestingData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DelegatedRandomDataGenerator class.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        /// <param name="generateDataDelegates">Delegates to use when generating data. Delegate is picked randomly each time if more than one delegate provided.</param>
        /// <param name="interestingData">Interesting data that should be generated randomly.</param>
        public DelegatedRandomDataGenerator(IRandomNumberGenerator random, IEnumerable<Func<IRandomNumberGenerator, TData>> generateDataDelegates, params TData[] interestingData)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckCollectionNotEmpty(generateDataDelegates, "generateDataDelegates");
            ExceptionUtilities.CheckCollectionDoesNotContainNulls(generateDataDelegates, "generateDataDelegates");
            ExceptionUtilities.CheckArgumentNotNull(interestingData, "interestingData");

            this.random = random;
            this.generateDataDelegates = generateDataDelegates.ToArray();
            this.interestingData = interestingData.ToArray();
        }

        /// <summary>
        /// Generates data.
        /// </summary>
        /// <returns>Generated data.</returns>
        public override TData GenerateData()
        {
            if (this.interestingData.Length > 0)
            {
                // First decide whether to use one of the interesting values or one of the random delegates
                // The probability of returning an interesting value should not be more than 1/3
                // When there is only one interesting value the probabiltiy of getting it is 1/10
                // (say, when the only interesting valus is null we'll get it with probability of 1/10)
                // Note that the above weights are somewhat ad-hoc to match what we had in previous test framework (CTF)
                int seed = this.random.Next(this.interestingData.Length * Math.Max(3, 10 / this.interestingData.Length));
                if (seed < this.interestingData.Length)
                {
                    var value = this.interestingData[this.interestingDataCount % this.interestingData.Length];
                    this.interestingDataCount++;
                    return value;
                }
            }

            return this.random.ChooseFrom(this.generateDataDelegates)(this.random);
        }
    }
}
