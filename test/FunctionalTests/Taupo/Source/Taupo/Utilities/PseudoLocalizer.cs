//---------------------------------------------------------------------
// <copyright file="PseudoLocalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Translates ASCII strings to Unicode characters by replacing letters with similar looking non-ASCII characters 
    /// (typically with diacritics or other alphabets)
    /// </summary>
    /// <remarks>The characters are chosen in such a way that converting the result to 1252 codepage will
    /// convert the string back to its ASCII form.</remarks>
    [ImplementationName(typeof(IPseudoLocalizer), "Default", HelpText = "Default pseudo-localizer")]
    public class PseudoLocalizer : IPseudoLocalizer
    {
        private Dictionary<char, string[]> characterSubstitutions;
        private IRandomNumberGenerator random;

        /// <summary>
        /// Initializes a new instance of the PseudoLocalizer class object.
        /// </summary>
        /// <param name="random">Random number generator to use.</param>
        public PseudoLocalizer(IRandomNumberGenerator random)
        {
            this.random = random;
            this.characterSubstitutions = new Dictionary<char, string[]>();
            this.PseudoLocalizeEveryNCharacters = 5;

            this.AddSubstitution('A', "\u0100", "\u0102", "\u0104", "\u01cd", "\u01de"); // ĀĂĄǍǞ
            this.AddSubstitution('C', "\u0106", "\u0108", "\u010a", "\u010c"); // ĆĈĊČ
            this.AddSubstitution('D', "\u010e"); // Ď
            this.AddSubstitution('E', "\u0112", "\u0114", "\u0116", "\u0118", "\u011a"); // ĒĔĖĘĚ
            this.AddSubstitution('F', "\u03a6"); // Φ
            this.AddSubstitution('G', "\u011c", "\u011e", "\u0120", "\u0122", "\u01e4", "\u01e6"); // ĜĞĠĢǤǦ
            this.AddSubstitution('H', "\u0124", "\u0126"); // ĤĦ
            this.AddSubstitution('I', "\u0128", "\u012a", "\u012c", "\u012e", "\u0130", "\u0197", "\u01cf"); // ĨĪĬĮİƗǏ
            this.AddSubstitution('J', "\u0134"); // Ĵ
            this.AddSubstitution('K', "\u0136", "\u01e8", "\u212a"); // ĶǨK
            this.AddSubstitution('L', "\u0139", "\u013b", "\u013d", "\u0141"); // ĹĻĽŁ
            this.AddSubstitution('N', "\u0143", "\u0145", "\u0147"); // ŃŅŇ
            this.AddSubstitution('O', "\u014c", "\u014e", "\u0150", "\u019f", "\u01a0", "\u01d1", "\u01ea", "\u01ec", "\u03a9"); // ŌŎŐƟƠǑǪǬΩ
            this.AddSubstitution('R', "\u0154", "\u0156", "\u0158"); // ŔŖŘ
            this.AddSubstitution('S', "\u015a", "\u015c", "\u015e"); // ŚŜŞ
            this.AddSubstitution('T', "\u0162", "\u0164", "\u0166", "\u01ae"); // ŢŤŦƮ
            this.AddSubstitution('U', "\u0168", "\u016a", "\u016c", "\u016e", "\u0170", "\u0172", "\u01af", "\u01d3", "\u01d5", "\u01d7", "\u01d9", "\u01db"); // ŨŪŬŮŰŲƯǓǕǗǙǛ
            this.AddSubstitution('W', "\u0174"); // Ŵ
            this.AddSubstitution('Y', "\u0176"); // Ŷ
            this.AddSubstitution('Z', "\u0179", "\u017b"); // ŹŻ
            this.AddSubstitution('a', "\u0101", "\u0103", "\u0105", "\u01ce", "\u01df", "\u03b1"); // āăąǎǟα
            this.AddSubstitution('b', "\u0180"); // ƀ
            this.AddSubstitution('c', "\u0107", "\u0109", "\u010b", "\u010d"); // ćĉċč
            this.AddSubstitution('d', "\u010f", "\u0111", "\u03b4"); // ďđδ
            this.AddSubstitution('e', "\u0113", "\u0115", "\u0117", "\u0119", "\u011b", "\u03b5"); // ēĕėęěε
            this.AddSubstitution('f', "\u03c6"); // φ
            this.AddSubstitution('g', "\u011d", "\u011f", "\u0121", "\u0123", "\u01e5", "\u01e7", "\u0261"); // ĝğġģǥǧɡ
            this.AddSubstitution('h', "\u0125", "\u0127", "\u04bb"); // ĥħһ
            this.AddSubstitution('i', "\u0129", "\u012b", "\u012d", "\u012f", "\u0131", "\u01d0"); // ĩīĭįıǐ
            this.AddSubstitution('j', "\u0135", "\u01f0"); // ĵǰ
            this.AddSubstitution('k', "\u0137", "\u01e9"); // ķǩ
            this.AddSubstitution('l', "\u013a", "\u013c", "\u013e", "\u0142", "\u019a"); // ĺļľłƚ
            this.AddSubstitution('n', "\u0144", "\u0146", "\u0148"); // ńņň
            this.AddSubstitution('o', "\u014d", "\u014f", "\u0151", "\u01a1", "\u01d2", "\u01eb", "\u01ed"); // ōŏőơǒǫǭ
            this.AddSubstitution('p', "\u03c0"); // π
            this.AddSubstitution('r', "\u0155", "\u0157", "\u0159"); // ŕŗř
            this.AddSubstitution('s', "\u015b", "\u015d", "\u015f", "\u03c3"); // śŝşσ
            this.AddSubstitution('t', "\u0163", "\u0165", "\u0167", "\u01ab", "\u03c4"); // ţťŧƫτ
            this.AddSubstitution('u', "\u0169", "\u016b", "\u016d", "\u016f", "\u0171", "\u0173", "\u01b0", "\u01d4", "\u01d6", "\u01d8", "\u01da", "\u01dc"); // ũūŭůűųưǔǖǘǚǜ
            this.AddSubstitution('w', "\u0175"); // ŵ
            this.AddSubstitution('y', "\u0177"); // ŷ
            this.AddSubstitution('z', "\u017a", "\u017c", "\u01b6"); // źżƶ
        }
        
        /// <summary>
        /// Gets or sets the number that specifies that every nth character should be pseudo-localized.
        /// </summary>
        /// <remarks>
        /// Set value to 1 to ensure all characters are pseudo-localized.
        /// </remarks>
        [InjectTestParameter("PseudoLocalizeEveryNCharacters", DefaultValueDescription = "5", HelpText = "Specifies that every nth character should be pseudo-localized.")]
        public int PseudoLocalizeEveryNCharacters { get; set; }

        /// <summary>
        /// Performs pseudo-localization of a given text.
        /// </summary>
        /// <param name="text">Text to be pseudo-localized.</param>
        /// <param name="isPropertyName">Indicates if the item is a property name</param>
        /// <returns>Pseudo-localized text.</returns>
        public string PseudoLocalize(string text, bool isPropertyName)
        {
            if (this.PseudoLocalizeEveryNCharacters < 1)
            {
                throw new TaupoInvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The " + typeof(PseudoLocalizer).Name + " cannot localize a string every {0} characters. This number must be positive.",
                        this.PseudoLocalizeEveryNCharacters));
            }

            StringBuilder sb = new StringBuilder(text.Length);

            for (int i = 0; i < text.Length; ++i)
            {
                char ch = text[i];
                string translation;

                if (i % this.PseudoLocalizeEveryNCharacters == 0 && this.TryTranslateChar(ch, out translation))
                {
                    sb.Append(translation);
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        private void AddSubstitution(char ch, params string[] substitutions)
        {
            this.characterSubstitutions.Add(ch, substitutions);
        }

        private bool TryTranslateChar(char ch, out string translation)
        {
            string[] possibleSubstitutions;

            if (!this.characterSubstitutions.TryGetValue(ch, out possibleSubstitutions))
            {
                translation = null;
                return false;
            }

            translation = this.random.ChooseFrom(possibleSubstitutions);
            return true;
        }
    }
}
