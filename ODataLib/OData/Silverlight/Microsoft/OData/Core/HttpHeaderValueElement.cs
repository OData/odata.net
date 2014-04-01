//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Class to represent a HTTP header value element.
    /// </summary>
    internal sealed class HttpHeaderValueElement
    {
        /// <summary>
        /// Internal constructor to create a new instance of <see cref="HttpHeaderValueElement"/>.
        /// </summary>
        /// <param name="name">The name of the preference.</param>
        /// <param name="value">The value of the preference.</param>
        /// <param name="parameters">The enumeration of preference parameter key value pairs.</param>
        public HttpHeaderValueElement(string name, string value, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, "name");
            ExceptionUtils.CheckArgumentNotNull(parameters, "parameters");

            this.Name = name;
            this.Value = value;
            this.Parameters = parameters;
#if DEBUG
            AssertToken(this.Name);
            AssertTokenOrQuotedString(this.Value);
            foreach (KeyValuePair<string, string> kvp in this.Parameters)
            {
                AssertToken(kvp.Key);
                AssertTokenOrQuotedString(kvp.Value);
            }
#endif
        }

        /// <summary>
        /// The name of the preference.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The value of the preference.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// The enumeration of preference parameter key value pairs.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; private set; }

        /// <summary>
        /// Converts the current <see cref="HttpHeaderValueElement"/> to string.
        /// </summary>
        /// <returns>The string for <see cref="HttpHeaderValueElement"/>.</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            AppendNameValuePair(stringBuilder, this.Name, this.Value);
            foreach (KeyValuePair<string, string> parameter in this.Parameters)
            {
                stringBuilder.Append(HttpHeaderValueLexer.ParameterSeparator);
                AppendNameValuePair(stringBuilder, parameter.Key, parameter.Value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Appends the <paramref name="name"/> and <paramref name="value"/> to <paramref name="stringBuilder"/> as name=value.
        /// </summary>
        /// <param name="stringBuilder">The string builder to append to.</param>
        /// <param name="name">The name to append.</param>
        /// <param name="value">The value to append.</param>
        private static void AppendNameValuePair(StringBuilder stringBuilder, string name, string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            stringBuilder.Append(name);
            if (value != null)
            {
                stringBuilder.Append(HttpHeaderValueLexer.ValueSeparator);
                stringBuilder.Append(value);
            }
        }

#if DEBUG
        /// <summary>
        /// Asserts the given value is a token.
        /// </summary>
        /// <param name="value">value in question.</param>
        private static void AssertToken(string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(value), "!string.IsNullOrEmpty(value)");
            int index = 0;
            bool isQuotedString;
            HttpUtils.ReadTokenOrQuotedStringValue("header", value, ref index, out isQuotedString, message => new ODataException(message));
            Debug.Assert(!isQuotedString, "!isQuotedString");
            Debug.Assert(index == value.Length, "index == value.Length");
        }

        /// <summary>
        /// Asserts the given value is a token or a quoted-string.
        /// </summary>
        /// <param name="value">value in question.</param>
        private static void AssertTokenOrQuotedString(string value)
        {
            if (value == null)
            {
                return;
            }

            int index = 0;
            bool isQuotedString;
            HttpUtils.ReadTokenOrQuotedStringValue("header", value, ref index, out isQuotedString, message => new ODataException(message));
            Debug.Assert(index == value.Length, "index == value.Length");
        }
#endif
    }
}
