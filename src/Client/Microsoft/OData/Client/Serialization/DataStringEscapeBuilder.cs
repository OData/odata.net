//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_SERVER
namespace Microsoft.OData.Service.Serializers
#else
namespace Microsoft.OData.Client
#endif
{
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Take a URI string and escape the data portion of it
    /// </summary>
    internal class DataStringEscapeBuilder
    {
        /// <summary>
        /// Sensitive characters that we should always skip
        /// This should be the set of Http control characters intersecting with 
        /// the set of characters OData literal format allows outside of strings
        /// (In V3: only +, as used in double literals ex. 3E+8)
        /// </summary>
        private const String SensitiveCharacters = "+";

        /// <summary>
        /// input string
        /// </summary>
        private readonly string input;

        /// <summary>
        /// output string
        /// </summary>
        private readonly StringBuilder output = new StringBuilder();

        /// <summary>
        /// the current index
        /// </summary>
        private int index;

        /// <summary>
        /// current quoted data string
        /// </summary>
        private StringBuilder quotedDataBuilder;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataString">The string to be escaped.</param>
        private DataStringEscapeBuilder(string dataString)
        {
            this.input = dataString;
        }

        /// <summary>
        /// Escape a URI string's data string portion
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The escaped string</returns>
        internal static string EscapeDataString(string input)
        {
            DataStringEscapeBuilder builder = new DataStringEscapeBuilder(input);
            return builder.Build();
        }

        /// <summary>
        /// Build a new escaped string
        /// </summary>
        /// <returns>The escaped string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Method explicitly only escapes specific characters that we know need escaping.")]
        private string Build()
        {
            Debug.Assert(this.index == 0, "Expected this.index to be 0, because Build can only be called once for an instance of DataStringEscapeBuilder.");
            Debug.Assert(this.output.Length == 0, "Expected this.output.Length to be 0, because Build can only be called once for an instance of DataStringEscapeBuilder.");

            for (this.index = 0; this.index < this.input.Length; ++this.index)
            {
                char current = this.input[this.index];
                if (current == '\'' || current == '"')
                {
                    this.ReadQuotedString(current);
                }
                else if (SensitiveCharacters.IndexOf(current) >= 0)
                {
                    this.output.Append(Uri.EscapeDataString(current.ToString()));
                }
                else
                {
                    this.output.Append(current);
                }
            }

            return this.output.ToString();
        }

        /// <summary>
        /// Read quoted string
        /// </summary>
        /// <param name="quoteStart">The character that started the quote</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Method explicitly only escapes values inside the single quotes.")]
        private void ReadQuotedString(char quoteStart)
        {
            if (this.quotedDataBuilder == null)
            {
                this.quotedDataBuilder = new StringBuilder();
            }
#if DEBUG
            else
            {
                Debug.Assert(this.quotedDataBuilder.Length == 0, "Expected quotedDataBuilder to have been cleared by previous call to ReadQuotedString");
            }
#endif

            this.output.Append(quoteStart);
            while (++this.index < this.input.Length)
            {
                if (this.input[this.index] == quoteStart)
                {
                    this.output.Append(Uri.EscapeDataString(this.quotedDataBuilder.ToString()));
                    this.output.Append(quoteStart);
                    this.quotedDataBuilder.Clear();

                    break;
                }

                this.quotedDataBuilder.Append(this.input[this.index]);
            }

            if (this.quotedDataBuilder.Length > 0)
            {
                // unterminated quote, should have validated before. We should not fail here.
                Debug.Assert(false, "unterminated quote in uri.");
                this.output.Append(Uri.EscapeDataString(this.quotedDataBuilder.ToString()));
                this.quotedDataBuilder.Clear();
            }
        }
    }
}
