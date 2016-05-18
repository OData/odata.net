//---------------------------------------------------------------------
// <copyright file="HttpHeaderValueLexer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Lexer to parse HTTP header values.
    /// </summary>
    internal abstract class HttpHeaderValueLexer
    {
        /// <summary>
        /// The ',' separator.
        /// </summary>
        internal const string ElementSeparator = ",";

        /// <summary>
        /// The ';' separator.
        /// </summary>
        internal const string ParameterSeparator = ";";

        /// <summary>
        /// The '=' separator.
        /// </summary>
        internal const string ValueSeparator = "=";

        /// <summary>
        /// The name of the HTTP header being parsed.
        /// </summary>
        private readonly string httpHeaderName;

        /// <summary>
        /// The value of the HTTP header being parsed.
        /// </summary>
        private readonly string httpHeaderValue;

        /// <summary>
        /// The starting index to the next item to be parsed.
        /// </summary>
        private readonly int startIndexOfNextItem;

        /// <summary>
        /// The value of the current parsed item. If the item type is quoted-string, this returns the unescaped and unquoted string value. For other item types,
        /// the value is the same as the original text from the header.
        /// </summary>
        private readonly string value;

        /// <summary>
        /// The original text of the current parsed item. If the item type is quoted-string, this returns the escaped and quoted string value straight from the header.
        /// For other item types, the original text is the same as the item value.
        /// </summary>
        private readonly string originalText;

        /// <summary>
        /// Constructs a new instance of <see cref="HttpHeaderValueLexer"/>.
        /// </summary>
        /// <param name="httpHeaderName">The name of the HTTP header being parsed.</param>
        /// <param name="httpHeaderValue">The value of the HTTP header being parsed.</param>
        /// <param name="value">The value of the current parsed item. If the item type is quoted-string, this returns the unescaped and unquoted string value. For other item types,
        /// the value is the same as the original text from the header.</param>
        /// <param name="originalText">The original text of the current parsed item. If the item type is quoted-string, this returns the escaped and quoted string value straight from the header.
        /// For other item types, the original text is the same as the item value.</param>
        /// <param name="startIndexOfNextItem">The start index of the next item to be parsed.</param>
        private HttpHeaderValueLexer(string httpHeaderName, string httpHeaderValue, string value, string originalText, int startIndexOfNextItem)
        {
            this.httpHeaderName = httpHeaderName;
            this.httpHeaderValue = httpHeaderValue;
            this.value = value;
            this.originalText = originalText;

            // Skip over white spaces.
            if (this.httpHeaderValue != null)
            {
                HttpUtils.SkipWhitespace(this.httpHeaderValue, ref startIndexOfNextItem);
            }

            this.startIndexOfNextItem = startIndexOfNextItem;
        }

        /// <summary>
        /// The item type enum.
        /// </summary>
        internal enum HttpHeaderValueItemType
        {
            /// <summary>Currently at the start of the header value.</summary>
            Start = 0,

            /// <summary>The current item is a token.</summary>
            Token,

            /// <summary>The current item is a quoted-string.</summary>
            QuotedString,

            /// <summary>The current item is the header element separator ','.</summary>
            ElementSeparator,

            /// <summary>The current item is the parameter separator ';'.</summary>
            ParameterSeparator,

            /// <summary>The current item is the value separator '='.</summary>
            ValueSeparator,

            /// <summary>At the end of the header value.</summary>
            End
        }

        /// <summary>
        /// The value of the current parsed item. If the item type is quoted-string, this returns the unescaped and unquoted string value. For other item types,
        /// the value is the same as the original text from the header.
        /// </summary>
        internal string Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// The original text of the current parsed item. If the item type is quoted-string, this returns the escaped and quoted string value straight from the header.
        /// For other item types, the original text is the same as the item value.
        /// </summary>
        internal string OriginalText
        {
            get
            {
                return this.originalText;
            }
        }

        /// <summary>
        /// The type of the current parsed item.
        /// </summary>
        internal abstract HttpHeaderValueItemType Type
        {
            get;
        }

        /// <summary>
        /// Constructs a new instance of the HTTP header value item.
        /// </summary>
        /// <param name="httpHeaderName">The name of the HTTP header being parsed.</param>
        /// <param name="httpHeaderValue">The value of the HTTP header being parsed.</param>
        /// <returns>The newly created instance of <see cref="HttpHeaderValueLexer"/>.</returns>
        internal static HttpHeaderValueLexer Create(string httpHeaderName, string httpHeaderValue)
        {
            Debug.Assert(!string.IsNullOrEmpty(httpHeaderName), "!string.IsNullOrEmpty(httpHeaderName)");

            return new HttpHeaderStart(httpHeaderName, httpHeaderValue);
        }

        /// <summary>
        /// Reads the content of a HTTP header from this <see cref="HttpHeaderValueLexer"/> instance to a new <see cref="HttpHeaderValue"/> instance.
        /// </summary>
        /// <returns>A new <see cref="HttpHeaderValue"/> instance populated with the content from this <see cref="HttpHeaderValueLexer"/> instance.</returns>
        internal HttpHeaderValue ToHttpHeaderValue()
        {
            HttpHeaderValueLexer lexer = this;
            Debug.Assert(
                lexer.Type == HttpHeaderValueItemType.Start,
                "lexer.Type == HttpHeaderValueItemType.Start -- Should only call this method on a lexer that's not yet been read.");

            HttpHeaderValue headerValue = new HttpHeaderValue();

            // header     = "header-name" ":" 1#element
            // element    = token [ BWS "=" BWS (token | quoted-string) ]
            //              *( OWS ";" [ OWS parameter ] )
            // parameter  = token [ BWS "=" BWS (token | quoted-string) ]
            while (true)
            {
                if (lexer.Type == HttpHeaderValueItemType.End)
                {
                    break;
                }

                Debug.Assert(
                    lexer.Type == HttpHeaderValueItemType.Start || lexer.Type == HttpHeaderValueItemType.ElementSeparator,
                    "lexer.Type == HttpHeaderValueItemType.Start || lexer.Type == HttpHeaderValueItemType.ElementSeparator");

                lexer = lexer.ReadNext();

                Debug.Assert(
                    lexer.Type == HttpHeaderValueItemType.Token || lexer.Type == HttpHeaderValueItemType.End,
                    "lexer.Type == HttpHeaderValueItemType.Token || lexer.Type == HttpHeaderValueItemType.End");

                if (lexer.Type == HttpHeaderValueItemType.Token)
                {
                    var element = ReadHttpHeaderValueElement(ref lexer);

                    // If multiple elements with the same name encountered, the first one wins.
                    if (!headerValue.ContainsKey(element.Name))
                    {
                        headerValue.Add(element.Name, element);
                    }
                }
            }

            return headerValue;
        }

        /// <summary>
        /// Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.
        /// Parsing is based on this grammar:
        ///     header     = "header-name" ":" 1#element
        ///     element    = token [ BWS "=" BWS (token | quoted-string) ]
        ///                  *( OWS ";" [ OWS parameter ] )
        ///     parameter  = token [ BWS "=" BWS (token | quoted-string) ]
        /// </summary>
        /// <returns>Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.</returns>
        internal abstract HttpHeaderValueLexer ReadNext();

        /// <summary>
        /// Reads a <see cref="HttpHeaderValueElement"/> from <paramref name="lexer"/> and advances the <paramref name="lexer"/> forward.
        /// </summary>
        /// <param name="lexer">The lexer to read from.</param>
        /// <returns>The <see cref="HttpHeaderValueElement"/> that was read.</returns>
        private static HttpHeaderValueElement ReadHttpHeaderValueElement(ref HttpHeaderValueLexer lexer)
        {
            Debug.Assert(lexer.Type == HttpHeaderValueItemType.Token, "lexer.Type == HttpHeaderValueItemType.Token");

            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>> { ReadKeyValuePair(ref lexer) };
            while (lexer.Type == HttpHeaderValueItemType.ParameterSeparator)
            {
                lexer = lexer.ReadNext();
                keyValuePairs.Add(ReadKeyValuePair(ref lexer));
            }

            Debug.Assert(
                lexer.Type == HttpHeaderValueItemType.ElementSeparator || lexer.Type == HttpHeaderValueItemType.End,
                "lexer.Type == HttpHeaderValueItemType.ElementSeparator || lexer.Type == HttpHeaderValueItemType.End");
            return new HttpHeaderValueElement(keyValuePairs[0].Key, keyValuePairs[0].Value, keyValuePairs.Skip(1).ToArray());
        }

        /// <summary>
        /// Reads a token or token=(token|quoted-string) from the <paramref name="lexer"/>, convert it to a key value pair and advances the <paramref name="lexer"/>.
        /// </summary>
        /// <param name="lexer">The lexer to read from.</param>
        /// <returns>The converted key value pair.</returns>
        private static KeyValuePair<string, string> ReadKeyValuePair(ref HttpHeaderValueLexer lexer)
        {
            Debug.Assert(lexer.Type == HttpHeaderValueItemType.Token, "lexer.Type == HttpHeaderValueItemType.Token");

            string name = lexer.OriginalText;
            string value = null;
            lexer = lexer.ReadNext();
            if (lexer.Type == HttpHeaderValueItemType.ValueSeparator)
            {
                lexer = lexer.ReadNext();
                Debug.Assert(
                    lexer.Type == HttpHeaderValueItemType.Token || lexer.Type == HttpHeaderValueItemType.QuotedString,
                    "lexer.Type == HttpHeaderValueItemType.Token || lexer.Type == HttpHeaderValueItemType.QuotedString");

                value = lexer.OriginalText;
                lexer = lexer.ReadNext();
            }

            Debug.Assert(
                lexer.Type == HttpHeaderValueItemType.End || lexer.Type == HttpHeaderValueItemType.ElementSeparator || lexer.Type == HttpHeaderValueItemType.ParameterSeparator,
                "lexer.Type == HttpHeaderValueItemType.End || lexer.Type == HttpHeaderValueItemType.ElementSeparator || lexer.Type == HttpHeaderValueItemType.ParameterSeparator");

            return new KeyValuePair<string, string>(name, value);
        }

        /// <summary>
        /// Returns true if we've parsed to the end of the header value, false otherwise.
        /// </summary>
        /// <returns>Returns true if we've parsed to the end of the header value, false otherwise.</returns>
        private bool EndOfHeaderValue()
        {
            return this.startIndexOfNextItem == this.httpHeaderValue.Length;
        }

        /// <summary>
        /// Reads a token or quoted-string value from the header.
        /// </summary>
        /// <returns>The token or quoted-string value that was read from the header.</returns>
        private HttpHeaderValueLexer ReadNextTokenOrQuotedString()
        {
            int index = this.startIndexOfNextItem;
            bool isQuotedString;
            string nextValue = HttpUtils.ReadTokenOrQuotedStringValue(this.httpHeaderName, this.httpHeaderValue, ref index, out isQuotedString, message => new ODataException(message));

            // Instead of testing whether result is null or empty, we check to see if the index have moved forward because we can encounter the empty quoted string "".
            if (index == this.startIndexOfNextItem)
            {
                throw new ODataException(Strings.HttpHeaderValueLexer_FailedToReadTokenOrQuotedString(this.httpHeaderName, this.httpHeaderValue, this.startIndexOfNextItem));
            }

            if (isQuotedString)
            {
                string original = this.httpHeaderValue.Substring(this.startIndexOfNextItem, index - this.startIndexOfNextItem);
                return new HttpHeaderQuotedString(this.httpHeaderName, this.httpHeaderValue, nextValue, original, index);
            }

            return new HttpHeaderToken(this.httpHeaderName, this.httpHeaderValue, nextValue, index);
        }

        /// <summary>
        /// Reads a token from the header.
        /// </summary>
        /// <returns>The token item that was read from the header.</returns>
        private HttpHeaderToken ReadNextToken()
        {
            HttpHeaderValueLexer item = this.ReadNextTokenOrQuotedString();
            if (item.Type == HttpHeaderValueItemType.QuotedString)
            {
                throw new ODataException(Strings.HttpHeaderValueLexer_TokenExpectedButFoundQuotedString(this.httpHeaderName, this.httpHeaderValue, this.startIndexOfNextItem));
            }

            return (HttpHeaderToken)item;
        }

        /// <summary>
        /// Reads a separator from the header.
        /// </summary>
        /// <returns>The separator item that was read from the header.</returns>
        private HttpHeaderSeparator ReadNextSeparator()
        {
            Debug.Assert(this.startIndexOfNextItem < this.httpHeaderValue.Length, "this.startIndexOfNextItem < this.httpHeaderValue.Length");
            string separator = this.httpHeaderValue.Substring(this.startIndexOfNextItem, 1);
            if (separator != ElementSeparator && separator != ParameterSeparator && separator != ValueSeparator)
            {
                throw new ODataException(Strings.HttpHeaderValueLexer_UnrecognizedSeparator(this.httpHeaderName, this.httpHeaderValue, this.startIndexOfNextItem, separator));
            }

            return new HttpHeaderSeparator(this.httpHeaderName, this.httpHeaderValue, separator, this.startIndexOfNextItem + 1);
        }

        /// <summary>
        /// Represents the start of the http header value.
        /// </summary>
        private sealed class HttpHeaderStart : HttpHeaderValueLexer
        {
            /// <summary>
            /// Constructs a new instance of <see cref="HttpHeaderStart"/>.
            /// </summary>
            /// <param name="httpHeaderName">The name of the HTTP header being parsed.</param>
            /// <param name="httpHeaderValue">The value of the HTTP header being parsed.</param>
            internal HttpHeaderStart(string httpHeaderName, string httpHeaderValue)
                : base(httpHeaderName, httpHeaderValue, /*value*/ null, /*originalText*/ null, 0)
            {
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderName), "!string.IsNullOrEmpty(this.httpHeaderName)");
                Debug.Assert(
                    this.httpHeaderValue == null || this.startIndexOfNextItem <= this.httpHeaderValue.Length,
                    "this.httpHeaderValue == null || this.startIndexOfNextItem <= this.httpHeaderValue.Length");
            }

            /// <summary>
            /// The type of the current item.
            /// </summary>
            internal override HttpHeaderValueItemType Type
            {
                get
                {
                    return HttpHeaderValueItemType.Start;
                }
            }

            /// <summary>
            /// Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.
            /// Parsing is based on this grammar:
            ///     header     = "header-name" ":" 1#element
            ///     element    = token [ BWS "=" BWS (token | quoted-string) ]
            ///                  *( OWS ";" [ OWS parameter ] )
            ///     parameter  = token [ BWS "=" BWS (token | quoted-string) ]
            /// </summary>
            /// <returns>Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.</returns>
            internal override HttpHeaderValueLexer ReadNext()
            {
                if (this.httpHeaderValue == null || this.EndOfHeaderValue())
                {
                    return HttpHeaderEnd.Instance;
                }

                // The first item on the header must be a token.
                return this.ReadNextToken();
            }
        }

        /// <summary>
        /// Represents a token in the HTTP header value.
        /// </summary>
        private sealed class HttpHeaderToken : HttpHeaderValueLexer
        {
            /// <summary>
            /// Constructs a new instance of <see cref="HttpHeaderToken"/>.
            /// </summary>
            /// <param name="httpHeaderName">The name of the HTTP header being parsed.</param>
            /// <param name="httpHeaderValue">The value of the HTTP header being parsed.</param>
            /// <param name="value">The value of the token.</param>
            /// <param name="startIndexOfNextItem">The start index of the next item.</param>
            internal HttpHeaderToken(string httpHeaderName, string httpHeaderValue, string value, int startIndexOfNextItem)
                : base(httpHeaderName, httpHeaderValue, value, value, startIndexOfNextItem)
            {
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderName), "!string.IsNullOrEmpty(this.httpHeaderName)");
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderValue), "!string.IsNullOrEmpty(this.httpHeaderValue)");
                Debug.Assert(this.startIndexOfNextItem <= this.httpHeaderValue.Length, "this.startIndexOfNextItem <= this.httpHeaderValue.Length");
                Debug.Assert(!string.IsNullOrEmpty(this.value), "!string.IsNullOrEmpty(this.value)");
            }

            /// <summary>
            /// The type of the current item.
            /// </summary>
            internal override HttpHeaderValueItemType Type
            {
                get
                {
                    return HttpHeaderValueItemType.Token;
                }
            }

            /// <summary>
            /// Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.
            /// Parsing is based on this grammar:
            ///     header     = "header-name" ":" 1#element
            ///     element    = token [ BWS "=" BWS (token | quoted-string) ]
            ///                  *( OWS ";" [ OWS parameter ] )
            ///     parameter  = token [ BWS "=" BWS (token | quoted-string) ]
            /// </summary>
            /// <returns>Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.</returns>
            internal override HttpHeaderValueLexer ReadNext()
            {
                if (this.EndOfHeaderValue())
                {
                    return HttpHeaderEnd.Instance;
                }

                // ',', ';' or '=' can come after a token.
                return this.ReadNextSeparator();
            }
        }

        /// <summary>
        /// Represents a quoted-string in the HTTP header value.
        /// </summary>
        private sealed class HttpHeaderQuotedString : HttpHeaderValueLexer
        {
            /// <summary>
            /// Constructs a new instance of <see cref="HttpHeaderQuotedString"/>.
            /// </summary>
            /// <param name="httpHeaderName">The name of the HTTP header being parsed.</param>
            /// <param name="httpHeaderValue">The value of the HTTP header being parsed.</param>
            /// <param name="value">The value of the quoted string, unescaped and without quotes.</param>
            /// <param name="originalText">The original text of the quoted string, escaped and with quotes.</param>
            /// <param name="startIndexOfNextItem">The start index of the next item.</param>
            internal HttpHeaderQuotedString(string httpHeaderName, string httpHeaderValue, string value, string originalText, int startIndexOfNextItem)
                : base(httpHeaderName, httpHeaderValue, value, originalText, startIndexOfNextItem)
            {
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderName), "!string.IsNullOrEmpty(this.httpHeaderName)");
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderValue), "!string.IsNullOrEmpty(this.httpHeaderValue)");
                Debug.Assert(this.startIndexOfNextItem <= this.httpHeaderValue.Length, "this.startIndexOfNextItem <= this.httpHeaderValue.Length");
                Debug.Assert(!string.IsNullOrEmpty(this.originalText), "!string.IsNullOrEmpty(this.originalText)");
            }

            /// <summary>
            /// The type of the current item.
            /// </summary>
            internal override HttpHeaderValueItemType Type
            {
                get
                {
                    return HttpHeaderValueItemType.QuotedString;
                }
            }

            /// <summary>
            /// Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.
            /// Parsing is based on this grammar:
            ///     header     = "header-name" ":" 1#element
            ///     element    = token [ BWS "=" BWS (token | quoted-string) ]
            ///                  *( OWS ";" [ OWS parameter ] )
            ///     parameter  = token [ BWS "=" BWS (token | quoted-string) ]
            /// </summary>
            /// <returns>Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.</returns>
            internal override HttpHeaderValueLexer ReadNext()
            {
                if (this.EndOfHeaderValue())
                {
                    return HttpHeaderEnd.Instance;
                }

                HttpHeaderSeparator separator = this.ReadNextSeparator();

                // ',' and ';' can come after a quoted-string.
                if (separator.Value == ElementSeparator || separator.Value == ParameterSeparator)
                {
                    return separator;
                }

                throw new ODataException(Strings.HttpHeaderValueLexer_InvalidSeparatorAfterQuotedString(this.httpHeaderName, this.httpHeaderValue, this.startIndexOfNextItem, separator.Value));
            }
        }

        /// <summary>
        /// Represents a separator in the HTTP header value.
        /// </summary>
        private sealed class HttpHeaderSeparator : HttpHeaderValueLexer
        {
            /// <summary>
            /// Constructs a new instance of <see cref="HttpHeaderSeparator"/>.
            /// </summary>
            /// <param name="httpHeaderName">The name of the HTTP header being parsed.</param>
            /// <param name="httpHeaderValue">The value of the HTTP header being parsed.</param>
            /// <param name="value">The value of the separator.</param>
            /// <param name="startIndexOfNextItem">The start index of the next item.</param>
            internal HttpHeaderSeparator(string httpHeaderName, string httpHeaderValue, string value, int startIndexOfNextItem)
                : base(httpHeaderName, httpHeaderValue, value, value, startIndexOfNextItem)
            {
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderName), "!string.IsNullOrEmpty(this.httpHeaderName)");
                Debug.Assert(!string.IsNullOrEmpty(this.httpHeaderValue), "!string.IsNullOrEmpty(this.httpHeaderValue)");
                Debug.Assert(this.startIndexOfNextItem <= this.httpHeaderValue.Length, "this.startIndexOfNextItem <= this.httpHeaderValue.Length");
                Debug.Assert(
                    this.Value == ElementSeparator || this.Value == ParameterSeparator || this.Value == ValueSeparator,
                    "this.Value == CommaSeparator || this.Value == SemicolonSeparator || this.Value == EqualsSeparator");
            }

            /// <summary>
            /// The type of the current item.
            /// </summary>
            internal override HttpHeaderValueItemType Type
            {
                get
                {
                    switch (this.Value)
                    {
                        case ElementSeparator:
                            return HttpHeaderValueItemType.ElementSeparator;
                        case ParameterSeparator:
                            return HttpHeaderValueItemType.ParameterSeparator;
                        default:
                            Debug.Assert(this.Value == ValueSeparator, "this.Value == ValueSeparator");
                            return HttpHeaderValueItemType.ValueSeparator;
                    }
                }
            }

            /// <summary>
            /// Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.
            /// Parsing is based on this grammar:
            ///     header     = "header-name" ":" 1#element
            ///     element    = token [ BWS "=" BWS (token | quoted-string) ]
            ///                  *( OWS ";" [ OWS parameter ] )
            ///     parameter  = token [ BWS "=" BWS (token | quoted-string) ]
            /// </summary>
            /// <returns>Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.</returns>
            internal override HttpHeaderValueLexer ReadNext()
            {
                if (this.EndOfHeaderValue())
                {
                    throw new ODataException(Strings.HttpHeaderValueLexer_EndOfFileAfterSeparator(this.httpHeaderName, this.httpHeaderValue, this.startIndexOfNextItem, this.originalText));
                }

                // Token or quoted-string can come after '='. i.e. token ['=' (token | quoted-string)]
                if (this.Value == ValueSeparator)
                {
                    return this.ReadNextTokenOrQuotedString();
                }

                // Only token can come after ',' and ';'.
                return this.ReadNextToken();
            }
        }

        /// <summary>
        /// Represents the end of the http header value.
        /// </summary>
        private sealed class HttpHeaderEnd : HttpHeaderValueLexer
        {
            /// <summary>
            /// Static instance of the end item.
            /// </summary>
            internal static readonly HttpHeaderEnd Instance = new HttpHeaderEnd();

            /// <summary>
            /// Constructs a new instance of <see cref="HttpHeaderEnd"/>.
            /// </summary>
            private HttpHeaderEnd()
                : base(/*httpHeaderName*/ null, /*httpHeaderValue*/ null, /*value*/ null, /*originalText*/ null, /*startIndexOfNextItem*/ -1)
            {
            }

            /// <summary>
            /// The type of the current item.
            /// </summary>
            internal override HttpHeaderValueItemType Type
            {
                get
                {
                    return HttpHeaderValueItemType.End;
                }
            }

            /// <summary>
            /// Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.
            /// Parsing is based on this grammar:
            ///     header     = "header-name" ":" 1#element
            ///     element    = token [ BWS "=" BWS (token | quoted-string) ]
            ///                  *( OWS ";" [ OWS parameter ] )
            ///     parameter  = token [ BWS "=" BWS (token | quoted-string) ]
            /// </summary>
            /// <returns>Returns an instance of <see cref="HttpHeaderValueLexer"/> to parse the rest of the items on the header value.</returns>
            internal override HttpHeaderValueLexer ReadNext()
            {
                Debug.Assert(false, "Already reached EndOfHeaderValue, our code should never call HttpHeaderEnd.ReadNext().");
                return null;
            }
        }
    }
}
