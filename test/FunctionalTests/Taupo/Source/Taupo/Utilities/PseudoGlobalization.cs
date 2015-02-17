//---------------------------------------------------------------------
// <copyright file="PseudoGlobalization.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.DataGeneration;

    /// <summary>
    /// Appends unicode characters to the end of a string
    /// </summary>
    [ImplementationName(typeof(IPseudoLocalizer), "Globalization", HelpText = "Add unicode charaters to strings")]
    public class PseudoGlobalization : IPseudoLocalizer
    {
        private WeightedStringGenerator stringGenerator;
        private IRandomNumberGenerator random;
        private List<char> unsupportedChars = new List<char> { '\u0686', '\u200c', '\u200d', '\u200e', '\u200f', '\ue864' };

        /// <summary>
        /// Initializes a new instance of the PseudoGlobalization class object.
        /// </summary>
        /// <param name="random">Random number generator to use.</param>
        public PseudoGlobalization(IRandomNumberGenerator random)
        {
            this.random = random;
            this.MinGlobalizationLength = 0;
            this.MaxGlobalizationLength = 10;
            this.stringGenerator = new WeightedStringGenerator()
            {
               { 1, StringGenerators.Chinese },
               { 1, StringGenerators.Greek },
               { 1, StringGenerators.Japanese },
               { 1, StringGenerators.Korean },
               { 1, StringGenerators.Turkish },
            };
        }

        /// <summary>
        /// Gets or sets the number that specifies the min length of the added unicode characters.
        /// </summary>
        /// <remarks>
        /// Set value to 1 to ensure all strings have unicode characters.
        /// </remarks>
        [InjectTestParameter("MinGlobalizationLength", DefaultValueDescription = "0", HelpText = "Specifies the min length of characters appended to end of string.")]
        public int MinGlobalizationLength { get; set; }

        /// <summary>
        /// Gets or sets the number that specifies the max length of the added unicode characters.
        /// </summary>
        /// <remarks>
        /// Must be greater than the MaxGlobalizationLength
        /// </remarks>
        [InjectTestParameter("MaxGlobalizationLength", DefaultValueDescription = "10", HelpText = "Specifies the max length of characters appended to end of string.")]
        public int MaxGlobalizationLength { get; set; }

        /// <summary>
        /// Pseudo-globablizes given string.
        /// </summary>
        /// <param name="text">String to pseudo-globilize</param>
        /// <param name="isPropertyName">Indicates if the item is a property name</param>
        /// <returns>String with Unicode characters appended to the end</returns>
        public string PseudoLocalize(string text, bool isPropertyName)
        {
            var appendix = this.stringGenerator.GenerateString(this.random, this.MinGlobalizationLength, this.MaxGlobalizationLength);
            appendix = this.RemoveUnsupportedChars(appendix);
            if (isPropertyName)
            {
                // TODO: Make sure the text is valid for xml names in case it is written in an xml response payload
                appendix = XmlConvert.EncodeNmToken(text);
            }

            return text += appendix;
        }

        /// <summary>
        /// Removes characters that are not supported as tokens in files for code gen purposes or xml names
        /// </summary>
        /// <param name="text">The string to modify</param>
        /// <returns>Valid code token</returns>
        private string RemoveUnsupportedChars(string text)
        {
            foreach (char c in this.unsupportedChars)
            {
                if (text.Contains(c.ToString()))
                {
                    text = text.Replace(c.ToString(), "_");
                }
            }

            return text;
        }
    }
}
