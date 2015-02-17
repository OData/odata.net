//---------------------------------------------------------------------
// <copyright file="CustomAlphabetStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// String generator which generates words based on a specified alphabet.
    /// </summary>
    public class CustomAlphabetStringGenerator : AlphabetStringGeneratorBase
    {
        private string[] alphabet;

        /// <summary>
        /// Initializes a new instance of the CustomAlphabetStringGenerator class.
        /// </summary>
        /// <param name="alphabet">The alphabet.</param>
        public CustomAlphabetStringGenerator(params string[] alphabet)
        {
            ExceptionUtilities.CheckArgumentNotNull(alphabet, "alphabet");
            if (alphabet.Length == 0)
            {
                throw new TaupoArgumentException("Alphabet cannot be empty.");
            }

            this.alphabet = alphabet;
        }

        /// <summary>
        /// Gets the alphabet to use for string generation.
        /// </summary>
        /// <returns>
        /// Array of strings comprising the alphabet.
        /// </returns>
        protected override string[] GetAlphabet()
        {
            return this.alphabet;
        }
    }
}
