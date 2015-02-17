//---------------------------------------------------------------------
// <copyright file="WeightedStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of a composite string generator for which randomly picks a generator from
    /// a weighted set and delegates string generation to it.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class WeightedStringGenerator : IStringGenerator, IEnumerable
    {
        private List<IStringGenerator> generators = new List<IStringGenerator>();
        private List<double> weights = new List<double>();

        /// <summary>
        /// Initializes a new instance of the WeightedStringGenerator class.
        /// </summary>
        public WeightedStringGenerator()
        {
        }

        /// <summary>
        /// Adds the specified string generator along with its weight to the list.
        /// </summary>
        /// <param name="weight">The weight.</param>
        /// <param name="stringGenerator">The string generator.</param>
        public void Add(double weight, IStringGenerator stringGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(stringGenerator, "stringGenerator");
            if (weight <= 0.0)
            {
                throw new TaupoArgumentException("Weight must be a positive number.");
            }

            this.generators.Add(stringGenerator);
            this.weights.Add(weight);
        }

        /// <summary>
        /// Generates random string of specified length.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="minLength">Minimum length of the generated string.</param>
        /// <param name="maxLength">Maximum length of the generated string.</param>
        /// <returns>Generated string.</returns>
        public string GenerateString(IRandomNumberGenerator random, int minLength, int maxLength)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckValidRange(minLength, "minLength", maxLength, "maxLength");

            var selectedGenerator = random.ChooseFrom(this.generators, this.weights);
            if (selectedGenerator == null)
            {
                throw new TaupoInvalidOperationException("Unable to select a string generator. Check the weights.");
            }

            return selectedGenerator.GenerateString(random, minLength, maxLength);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a colection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}
