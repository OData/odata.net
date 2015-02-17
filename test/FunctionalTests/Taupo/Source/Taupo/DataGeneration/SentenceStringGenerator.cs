//---------------------------------------------------------------------
// <copyright file="SentenceStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Globalization;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of <see cref="IStringGenerator"/> which generates pseudo-sentences.
    /// </summary>
    /// <remarks>
    /// Generated sentences are sequences of shorter words, starting with a capital letter, separated by some readable glue 
    /// words and followed by a period sign.
    /// </remarks>
    public class SentenceStringGenerator : IStringGenerator
    {
        private static string[] glueWords = 
        {
            "and",
            "the",
            "if",
            "while",
            "or",
            "while",
            "then",
            "often",
            "sometimes",
            "thus",
            "a",
            "about",
            "in",
            "under",
            "over",
        };

        private IStringGenerator wordGenerator;

        /// <summary>
        /// Initializes a new instance of the SentenceStringGenerator class.
        /// </summary>
        /// <param name="wordGenerator">The word generator.</param>
        public SentenceStringGenerator(IStringGenerator wordGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(wordGenerator, "wordGenerator");
            this.wordGenerator = wordGenerator;
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

            int wordCount = random.NextFromRange(2, 10);
            int length = random.NextFromRange(minLength, maxLength);

            StringBuilder sb = new StringBuilder();
            while (sb.Length < length)
            {
                string word = this.wordGenerator.GenerateString(random, 2, Math.Max(3, length / wordCount));

                sb.Append(word);
                if (sb.Length < length)
                {
                    sb.Append(' ');
                    if (random.Next(100) < 30)
                    {
                        sb.Append(random.ChooseFrom(glueWords));
                        sb.Append(' ');
                    }
                }
            }

            if (maxLength > 0 && sb.Length > maxLength - 1)
            {
                sb.Length = maxLength - 1;
                sb.Append(".");

                sb[0] = char.ToUpperInvariant(sb[0]);
            }

            return sb.ToString();
        }
    }
}
