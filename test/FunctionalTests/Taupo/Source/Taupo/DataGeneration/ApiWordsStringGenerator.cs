//---------------------------------------------------------------------
// <copyright file="ApiWordsStringGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Implementation of <see cref="IStringGenerator"/> which generates strings consisting of API words.
    /// </summary>
    public class ApiWordsStringGenerator : IStringGenerator
    {
        private IList<string> wordPool;
        private List<Assembly> assemblies;

        /// <summary>
        /// Initializes a new instance of the ApiWordsStringGenerator class.
        /// </summary>
        /// <param name="assemblies">The assemblies to use to initialize the word pool.</param>
        public ApiWordsStringGenerator(params Assembly[] assemblies)
        {
            ExceptionUtilities.CheckArgumentNotNull(assemblies, "assemblies");
            if (assemblies.Length == 0)
            {
                throw new TaupoArgumentException("Must provide at least one assembly.");
            }

            this.assemblies = new List<Assembly>(assemblies);
        }

        /// <summary>
        /// Gets the assemblies used to populate word list.
        /// </summary>
        /// <value>The assemblies.</value>
        public ReadOnlyCollection<Assembly> Assemblies
        {
            get { return this.assemblies.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the word pool .
        /// </summary>
        /// <value>The word pool.</value>
        public IEnumerable<string> WordPool
        {
            get { return this.GetWordList(); }
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

            var alphabet = this.GetWordList();
            int length = random.NextFromRange(minLength, maxLength);

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

        private static void AddAdditionalApiRelatedWordsToPool(ICollection<string> pool)
        {
            var additionalWords = new List<string>()
                 {
                     // Floating point related strings
                     "Infinity",
                     "-Infinity",
                     "NaN",
                     "-0.0567E+08",
                     "1.2345E-06",
                 };

            additionalWords.ForEach(pool.Add);
        }

        private IList<string> GetWordList()
        {
            if (this.wordPool == null)
            {
                this.wordPool = this.BuildWordPool();
                AddAdditionalApiRelatedWordsToPool(this.wordPool);
            }

            return this.wordPool;
        }

        /// <summary>
        /// Build a pool of usable words by scanning all public APIs in mscorlib.dll and Taupo dll
        /// and extracting individual words from Pascal-cased identifiers.
        /// </summary>
        /// <returns>Generated list of words.</returns>
        private IList<string> BuildWordPool()
        {
            ICollection<string> result = new List<string>();
            foreach (Assembly asm in this.assemblies)
            {
                this.AddWordsFromAssembly(result, asm);
            }

            return result.OrderBy(c => c).ToList();
        }

        private void AddWordsFromAssembly(ICollection<string> result, Assembly assembly)
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                this.AddWordsFromType(result, type);
            }
        }

        private void AddWordsFromType(ICollection<string> result, Type type)
        {
            this.AddWordsFromPascalCasedStringToPool(result, type.FullName);
            foreach (MemberInfo member in type.GetMembers())
            {
                this.AddWordsFromPascalCasedStringToPool(result, member.Name);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need lower case string here.")]
        private void AddWordsFromPascalCasedStringToPool(ICollection<string> result, string text)
        {
            int p = 0;

            while (p < text.Length)
            {
                // skip over non-words
                while (p < text.Length && !char.IsLetter(text, p))
                {
                    p++;
                }

                int start = p;

                while (p < text.Length && char.IsUpper(text, p))
                {
                    p++;
                }

                int endOfCapitals = p;

                while (p < text.Length && char.IsLower(text, p))
                {
                    p++;
                }

                int end = p;

                if (start != end && endOfCapitals == start + 1)
                {
                    string word = text.Substring(start, end - start).ToLowerInvariant();

                    if (word.Length >= 3 && !result.Contains(word))
                    {
                        result.Add(word);
                    }
                }
            }
        }
    }
}
