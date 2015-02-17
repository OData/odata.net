//---------------------------------------------------------------------
// <copyright file="ResourceAlphabetStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Generates strings based on alphabet defined in an embedded resource.
    /// </summary>
    internal class ResourceAlphabetStringGenerator : AlphabetStringGeneratorBase
    {
        private string[] alphabet;
        private string resourceName;
        private Assembly assembly;

        /// <summary>
        /// Initializes a new instance of the ResourceAlphabetStringGenerator class.
        /// </summary>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public ResourceAlphabetStringGenerator(Assembly assembly, string resourceName)
        {
            ExceptionUtilities.CheckArgumentNotNull(assembly, "assembly");
            ExceptionUtilities.CheckArgumentNotNull(resourceName, "resourceName");

            this.assembly = assembly;
            this.resourceName = resourceName;
        }

        /// <summary>
        /// When overridden in a derived class, generates a random string of specified length
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="minLength">Minimum length of the generated string.</param>
        /// <param name="maxLength">Maximum length of the generated string.</param>
        /// <returns>Generated string.</returns>
        protected override string GenerateStringCore(IRandomNumberGenerator random, int minLength, int maxLength)
        {
            this.EnsureAlphabetLoaded();
            var minimumCharacterLength = this.alphabet.Min(w => w.Length);

            var desiredLength = random.NextFromRange(minLength, maxLength);

            // This is an intentionally naive check to determine whether the desired length
            // is actually one that this resource string generator can satisfy. Since the minimum
            // length of existing alphabets is either 1 or 2, and the alphabet that has
            // characters of length 2 has no characters of length 3 or more, then this check works.
            if ((desiredLength % minimumCharacterLength) != 0)
            {
                // Get the closest length to the randomly chosen length that is also a multiple
                // of minimumCharacterLength. Note that the integer division between desiredLength
                // and minimumCharacterLength is intentional.
                desiredLength = (desiredLength / minimumCharacterLength) * minimumCharacterLength;

                if (desiredLength < minLength)
                {
                    desiredLength += minimumCharacterLength;
                }

                if (desiredLength > maxLength)
                {
                    throw new TaupoArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "This string generator cannot generate a string of a length between {0} and {1}.",
                            minLength,
                            maxLength));
                }
            }

            StringBuilder sb = new StringBuilder(desiredLength);
            while (sb.Length < desiredLength)
            {
                var next = random.ChooseFrom(this.alphabet);

                if (next.Length + sb.Length > desiredLength)
                {
                    next = random.ChooseFrom(this.alphabet.Where(w => w.Length == minimumCharacterLength));
                }

                sb.Append(next);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the alphabet to use for string generation.
        /// </summary>
        /// <returns>
        /// Array of strings comprising the alphabet.
        /// </returns>
        protected override string[] GetAlphabet()
        {
            this.EnsureAlphabetLoaded();

            return this.alphabet;
        }

        /// <summary>
        /// Ensures that the alphabet has been loaded.
        /// </summary>
        private void EnsureAlphabetLoaded()
        {
            if (this.alphabet == null)
            {
                using (Stream stream = this.assembly.GetManifestResourceStream(this.resourceName))
                {
                    StreamReader streamReader = new StreamReader(stream);
                    this.alphabet = AlphabetFileParser.ParseAlphabetFile(streamReader);
                }
            }
        }
    }
}
