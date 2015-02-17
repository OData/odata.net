//---------------------------------------------------------------------
// <copyright file="AlphabetStringGeneratorBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Alphabet-based string generator.
    /// </summary>
    /// <remarks>
    /// Alphabet-based string generators generate random words by concatenating words from the specified alphabet.
    /// </remarks>
    public abstract class AlphabetStringGeneratorBase : IStringGenerator
    {
        /// <summary>
        /// Initializes a new instance of the AlphabetStringGeneratorBase class.
        /// </summary>
        protected AlphabetStringGeneratorBase()
        {
        }

        /// <summary>
        /// Generates random string of specified length
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="minLength">Minimum length of the generated string.</param>
        /// <param name="maxLength">Maximum length of the generated string.</param>
        /// <returns>Generated string.</returns>
        public string GenerateString(IRandomNumberGenerator random, int minLength, int maxLength)
        {
            ExceptionUtilities.CheckArgumentNotNull(random, "random");
            ExceptionUtilities.CheckValidRange(minLength, "minLength", maxLength, "maxLength");

            return this.GenerateStringCore(random, minLength, maxLength);
        }

        /// <summary>
        /// When overridden in a derived class, generates a random string of specified length
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="minLength">Minimum length of the generated string.</param>
        /// <param name="maxLength">Maximum length of the generated string.</param>
        /// <returns>Generated string.</returns>
        protected virtual string GenerateStringCore(IRandomNumberGenerator random, int minLength, int maxLength)
        {
            int length = random.NextFromRange(minLength, maxLength);
            var alphabet = this.GetAlphabet();

            StringBuilder sb = new StringBuilder(length);
            while (sb.Length < length)
            {
                sb.Append(random.ChooseFrom(alphabet));
            }

            if (sb.Length > maxLength)
            {
                sb.Length = maxLength;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the alphabet to use for string generation.
        /// </summary>
        /// <returns>Array of strings comprising the alphabet.</returns>
        protected abstract string[] GetAlphabet();
    }
}
