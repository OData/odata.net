//---------------------------------------------------------------------
// <copyright file="FixedSetDataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Random data generator that uses fixed set of values to generate data.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class FixedSetDataGenerator<TData> : DataGenerator<TData>
    {
        private readonly IRandomNumberGenerator random;
        private readonly List<TData> interestingData;
        private readonly bool allowRepeats;

        /// <summary>
        /// Initializes a new instance of the FixedSetDataGenerator class.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        /// <param name="interestingData">Interesting data that should be generated randomly.</param>
        public FixedSetDataGenerator(IRandomNumberGenerator random, params TData[] interestingData)
            : this(random, true, interestingData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the FixedSetDataGenerator class.
        /// </summary>
        /// <param name="random">Random number generator.</param>
        /// <param name="allowRepeats">A value indicating whether repeats are allowed</param>
        /// <param name="interestingData">Interesting data that should be generated randomly.</param>
        public FixedSetDataGenerator(IRandomNumberGenerator random, bool allowRepeats, IEnumerable<TData> interestingData)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckCollectionNotEmpty(interestingData, "interestingData");

            this.random = random;
            this.allowRepeats = allowRepeats;
            this.interestingData = interestingData.ToList();
        }

        /// <summary>
        /// Generates data.
        /// </summary>
        /// <returns>Generated data.</returns>
        public override TData GenerateData()
        {
            ExceptionUtilities.Assert(this.interestingData.Count > 0, "Reached capacity for available unique data.");
            var result = this.random.ChooseFrom(this.interestingData);

            if (!this.allowRepeats)
            {
                this.interestingData.Remove(result);
            }

            return result;
        }
    }
}
