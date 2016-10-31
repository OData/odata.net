//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
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
            DebugUtils.CheckNoExternalCallers();
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
