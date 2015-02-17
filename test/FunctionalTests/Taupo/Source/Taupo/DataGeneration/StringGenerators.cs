//---------------------------------------------------------------------
// <copyright file="StringGenerators.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Available string generators.
    /// </summary>
    public static class StringGenerators
    {
        /// <summary>
        /// Initializes static members of the StringGenerators class.
        /// </summary>
        static StringGenerators()
        {
            // alphabet-based generators
            Chinese = ResourceAlphabetGenerator("Chinese");
            ChineseTraditional = ResourceAlphabetGenerator("ChineseTraditional");
            Cyrillic = ResourceAlphabetGenerator("Cyrillic");
            English = ResourceAnsiAlphabetGenerator("English");
            German = ResourceAlphabetGenerator("German");
            Greek = ResourceAlphabetGenerator("Greek");
            Hebrew = ResourceAlphabetGenerator("Hebrew");
            Hindi = ResourceAlphabetGenerator("Hindi");
            Japanese = ResourceAlphabetGenerator("Japanese");
            Korean = ResourceAlphabetGenerator("Korean");
            Latin = ResourceAlphabetGenerator("Latin");
            Mongolian = ResourceAlphabetGenerator("Mongolian");
            NumbersOnly = ResourceAlphabetGenerator("NumbersOnly");
            Semigraphics = ResourceAlphabetGenerator("Semigraphics");
            SurrogateCharacters = ResourceAlphabetGenerator("SurrogateCharacters");
            Thai = ResourceAlphabetGenerator("Thai");
            Turkish = ResourceAlphabetGenerator("Turkish");

            // word list-based generators
            SqlInjections = ResourceWordListGenerator("SqlInjections");

            // custom generators
            ApiWords = new ApiWordsAnsiStringGenerator(typeof(string).GetAssembly());
        }

        /// <summary>
        /// Gets the generator using Chinese characters.
        /// </summary>
        public static IStringGenerator Chinese { get; private set; }

        /// <summary>
        /// Gets the generator using Traditional Chinese characters.
        /// </summary>
        public static IStringGenerator ChineseTraditional { get; private set; }

        /// <summary>
        /// Gets the generator using Cyrillic characters.
        /// </summary>
        public static IStringGenerator Cyrillic { get; private set; }

        /// <summary>
        /// Gets the generator using English characters.
        /// </summary>
        public static IAnsiStringGenerator English { get; private set; }

        /// <summary>
        /// Gets the generator using German characters.
        /// </summary>
        public static IStringGenerator German { get; private set; }

        /// <summary>
        /// Gets the generator using Greek characters.
        /// </summary>
        public static IStringGenerator Greek { get; private set; }

        /// <summary>
        /// Gets the generator using Hebrew characters.
        /// </summary>
        public static IStringGenerator Hebrew { get; private set; }

        /// <summary>
        /// Gets the generator using Hindi characters.
        /// </summary>
        public static IStringGenerator Hindi { get; private set; }

        /// <summary>
        /// Gets the generator using Japanese characters.
        /// </summary>
        public static IStringGenerator Japanese { get; private set; }

        /// <summary>
        /// Gets the generator using Korean characters.
        /// </summary>
        public static IStringGenerator Korean { get; private set; }

        /// <summary>
        /// Gets the generator using Latin characters.
        /// </summary>
        public static IStringGenerator Latin { get; private set; }

        /// <summary>
        /// Gets the generator using Mongolian characters.
        /// </summary>
        public static IStringGenerator Mongolian { get; private set; }

        /// <summary>
        /// Gets the generator using NumbersOnly characters.
        /// </summary>
        public static IStringGenerator NumbersOnly { get; private set; }

        /// <summary>
        /// Gets the generator using semigraphic characters.
        /// </summary>
        public static IStringGenerator Semigraphics { get; private set; }

        /// <summary>
        /// Gets the generator using Thai characters.
        /// </summary>
        public static IStringGenerator Thai { get; private set; }

        /// <summary>
        /// Gets the generator using Turkish characters.
        /// </summary>
        public static IStringGenerator Turkish { get; private set; }

        /// <summary>
        /// Gets the generator using surrogate characters.
        /// </summary>
        public static IStringGenerator SurrogateCharacters { get; private set; }

        /// <summary>
        /// Gets the SQL injections generator.
        /// </summary>
        public static IStringGenerator SqlInjections { get; private set; }

        /// <summary>
        /// Gets the API words generator.
        /// </summary>
        public static IAnsiStringGenerator ApiWords { get; private set; }

        private static IStringGenerator ResourceAlphabetGenerator(string alphabetName)
        {
            var assembly = typeof(StringGenerators).GetAssembly();
            string namePrefix = typeof(StringGenerators).Namespace + ".Alphabets.";

            return new ResourceAlphabetStringGenerator(assembly, namePrefix + alphabetName + ".txt");
        }

        private static IAnsiStringGenerator ResourceAnsiAlphabetGenerator(string alphabetName)
        {
            var assembly = typeof(StringGenerators).GetAssembly();
            string namePrefix = typeof(StringGenerators).Namespace + ".Alphabets.";

            return new ResourceAnsiAlphabetStringGenerator(assembly, namePrefix + alphabetName + ".txt");
        }

        private static IStringGenerator ResourceWordListGenerator(string wordListName)
        {
            var assembly = typeof(StringGenerators).GetAssembly();
            string namePrefix = typeof(StringGenerators).Namespace + ".WordLists.";

            return new ResourceWordListStringGenerator(assembly, namePrefix + wordListName + ".txt");
        }
    }
}
