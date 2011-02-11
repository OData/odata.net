//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Parser which consumes the URI path and produces the lexical object model.
    /// </summary>
    internal class UriQueryPathParser
    {
        /// <summary>
        /// Empty collection of key values.
        /// </summary>
        private static NamedValue[] EmptyKeyValues = new NamedValue[0];

        /// <summary>
        /// The maximum number of recursion nesting allowed.
        /// </summary>
        private int maxDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="maxDepth">The maximum depth of each part of the query - a recursion limit.</param>
        internal UriQueryPathParser(int maxDepth)
        {
            DebugUtils.CheckNoExternalCallers();

            this.maxDepth = maxDepth;
        }

        /// <summary>
        /// Parses the <paramref name="queryUri"/> and returns a new instance of <see cref="QueryToken"/>
        /// describing the query path specified by the URI.
        /// </summary>
        /// <param name="queryUri">The absolute URI which holds the query to parse. This must be a path relative to the <paramref name="serviceBaseUri"/>.</param>
        /// <param name="serviceBaseUri">The base URI of the service.</param>
        /// <returns>A new instance of <see cref="QueryToken"/> which represents the query path specified in the <paramref name="queryUri"/>.
        /// The result QueryToken is just a lexical representation.</returns>
        internal QueryToken ParseUri(Uri queryUri, Uri serviceBaseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(queryUri != null, "queryUri != null");
            Debug.Assert(serviceBaseUri != null, "serviceBaseUri != null");

            QueryToken path = this.CreatePathSegments(queryUri, serviceBaseUri);
            if (path == null)
            {
                // No segments were found - the service document query
                // The lexical representation of the service document is an identifier with an empty name
                path = new SegmentQueryToken() { Name = string.Empty };
            }

            return path;
        }

        /// <summary>
        /// Extracts the identifier part of the URI path segment.
        /// </summary>
        /// <param name="segment">Unescaped URI path segment.</param>
        /// <param name="identifier">Upon return, the identifier in the segment.</param>
        /// <returns>true if keys follow the identifier; false otherwise.</returns>
        private static bool ExtractSegmentIdentifier(string segment, out string identifier)
        {
            Debug.Assert(segment != null, "segment != null");

            //// This is a copy of the RequestUriProcessor.ExtractSegmentIdentifier

            int keyStart = segment.IndexOf('(');
            if (keyStart < 0)
            {
                identifier = segment;
                return false;
            }
            else
            {
                identifier = segment.Substring(0, keyStart);
                return true;
            }
        }

        /// <summary>Removes the parens around the key values part of a query.</summary>
        /// <param name='keyValues'>Key values with parens included.</param>
        /// <returns>Key values without parens.</returns>
        private static string RemoveKeyValuesParens(string keyValues)
        {
            Debug.Assert(keyValues != null, "keyValues != null");

            if (!(keyValues.Length > 0 && keyValues[0] == '(' && keyValues[keyValues.Length - 1] == ')'))
            {
                throw new ODataException(Strings.UriQueryPathParser_SyntaxError);
            }

            return keyValues.Substring(1, keyValues.Length - 2);
        }

        /// <summary>
        /// Parses the key values string and returns enumeration of key values found.
        /// </summary>
        /// <param name="keyValuesString">The key values string to parse.</param>
        /// <returns>Enumeration of key values found.</returns>
        private static IEnumerable<NamedValue> ParseKeyValues(string keyValuesString)
        {
            keyValuesString = RemoveKeyValuesParens(keyValuesString);

            // TODO: Validate that only named values or a single unnamed value is used.
            // TODO: Validate that named values don't contain duplicit keys.
            IEnumerable<NamedValue> keyValues = ParseKeyValuesFromUri(keyValuesString);
            if (keyValues == null)
            {
                throw new ODataException(Strings.UriQueryPathParser_SyntaxError);
            }

            return keyValues;
        }

        /// <summary>Attempts to parse key values from the specified text.</summary>
        /// <param name='text'>Text to parse (not null).</param>
        /// <returns>
        /// Enumeration of key values or null if there was a syntax error.
        /// </returns>
        /// <remarks>
        /// The returned instance contains only string values.
        /// </remarks>
        private static IEnumerable<NamedValue> ParseKeyValuesFromUri(string text)
        {
            Debug.Assert(text != null, "text != null");

            //// This is a modified copy of KeyInstance.TryParseFromUri

            ExpressionLexer lexer = new ExpressionLexer(text);
            ExpressionToken currentToken = lexer.CurrentToken;
            if (currentToken.Kind == ExpressionTokenKind.End)
            {
                return EmptyKeyValues;
            }

            List<NamedValue> keyValuesList = new List<NamedValue>();
            do
            {
                if (currentToken.Kind == ExpressionTokenKind.Identifier)
                {
                    // Name-value pair.
                    string identifier = lexer.CurrentToken.GetIdentifier();
                    lexer.NextToken();
                    if (lexer.CurrentToken.Kind != ExpressionTokenKind.Equal)
                    {
                        return null;
                    }

                    lexer.NextToken();
                    if (!lexer.CurrentToken.IsKeyValueToken)
                    {
                        return null;
                    }

                    keyValuesList.Add(new NamedValue() { Name = identifier, Value = ParseKeyValueLiteral(lexer) });
                }
                else if (currentToken.IsKeyValueToken)
                {
                    // Unnamed value.
                    keyValuesList.Add(new NamedValue() { Value = ParseKeyValueLiteral(lexer) });
                }
                else
                {
                    return null;
                }

                // Read the next token. We should be at the end, or find
                // we have a comma followed by something.
                currentToken = lexer.NextToken();
                if (currentToken.Kind == ExpressionTokenKind.Comma)
                {
                    currentToken = lexer.NextToken();
                    if (currentToken.Kind == ExpressionTokenKind.End)
                    {
                        // Trailing comma.
                        return null;
                    }
                }
            }
            while (currentToken.Kind != ExpressionTokenKind.End);

            return keyValuesList;
        }

        /// <summary>
        /// Parses the key value literal.
        /// </summary>
        /// <param name="lexer">The lexer positioned on the key value.</param>
        /// <returns>The literal query token.</returns>
        private static LiteralQueryToken ParseKeyValueLiteral(ExpressionLexer lexer)
        {
            Debug.Assert(lexer != null, "lexer != null");

            LiteralQueryToken result = UriQueryExpressionParser.TryParseLiteral(lexer);
            if (result == null)
            {
                throw new ODataException(Strings.UriQueryPathParser_InvalidKeyValueLiteral(lexer.CurrentToken.Text));
            }

            return result;
        }

        /// <summary>
        /// Creates a list of path segments for the specified URI query.
        /// </summary>
        /// <param name="queryUri">The absolute URI of the request.</param>
        /// <param name="serviceBaseUri">The service base URI for the request.</param>
        /// <returns>The <see cref="QueryToken"/> representing the query path.</returns>
        private QueryToken CreatePathSegments(Uri queryUri, Uri serviceBaseUri)
        {
            Debug.Assert(queryUri != null, "queryUri != null");
            Debug.Assert(serviceBaseUri != null, "serviceBaseUri != null");

            //// This method is a no-metadata, lexical only copy of RequestUriProcessor.CreateSegments

            List<string> uriSegments = this.EnumerateSegments(queryUri, serviceBaseUri);
            Debug.Assert(uriSegments.Count <= this.maxDepth, "uriSegments.Count <= this.maxDepth");

            // The segment before the one being processed
            SegmentQueryToken parentNode = null;

            foreach (string uriSegment in uriSegments)
            {
                Debug.Assert(uriSegment != null, "uriSegment != null");

                string segmentIdentifier;
                bool hasKeyValues = ExtractSegmentIdentifier(uriSegment, out segmentIdentifier);
                string keyValuesString = hasKeyValues ? uriSegment.Substring(segmentIdentifier.Length) : null;

                SegmentQueryToken segment = new SegmentQueryToken()
                {
                    Name = segmentIdentifier,
                    Parent = parentNode,
                    NamedValues = hasKeyValues ? ParseKeyValues(keyValuesString) : null
                };

                parentNode = segment;
            }

            return parentNode;
        }

        /// <summary>
        /// Returns list of segments in the specified path (eg: /foo/bar -&gt; foo, bar).
        /// </summary>
        /// <param name="absoluteUri">The absolute URI of the request.</param>
        /// <param name="serviceBaseUri">The service base URI for the request.</param>
        /// <returns>List of unescaped segments.</returns>
        private List<string> EnumerateSegments(Uri absoluteUri, Uri serviceBaseUri)
        {
            Debug.Assert(absoluteUri != null, "absoluteUri != null");
            Debug.Assert(absoluteUri.IsAbsoluteUri, "absoluteRequestUri.IsAbsoluteUri(" + absoluteUri.IsAbsoluteUri + ")");
            Debug.Assert(serviceBaseUri != null, "serviceBaseUri != null");
            Debug.Assert(serviceBaseUri.IsAbsoluteUri, "serviceBaseUri.IsAbsoluteUri(" + serviceBaseUri + ")");

            //// This is a copy of the RequestUriProcessor.EnumerateSegments

            if (!UriUtils.UriInvariantInsensitiveIsBaseOf(serviceBaseUri, absoluteUri))
            {
                throw new ODataException(Strings.UriQueryPathParser_RequestUriDoesNotHaveTheCorrectBaseUri(absoluteUri, serviceBaseUri));
            }

            try
            {
                Uri uri = absoluteUri;
                int numberOfSegmentsToSkip = 0;

                // Skip over the base URI segments
#if SILVERLIGHT
                numberOfSegmentsToSkip = serviceBaseUri.AbsolutePath.Split('/').Length;
                string[] uriSegments = uri.AbsolutePath.Split('/');
#else
                numberOfSegmentsToSkip = serviceBaseUri.Segments.Length;
                string[] uriSegments = uri.Segments;
#endif

                List<string> segments = new List<string>();
                for (int i = numberOfSegmentsToSkip; i < uriSegments.Length; i++)
                {
                    string segment = uriSegments[i];
                    if (segment.Length != 0 && segment != "/")
                    {
                        if (segment[segment.Length - 1] == '/')
                        {
                            segment = segment.Substring(0, segment.Length - 1);
                        }

                        if (segments.Count == this.maxDepth)
                        {
                            throw new ODataException(Strings.UriQueryPathParser_TooManySegments);
                        }

                        segments.Add(Uri.UnescapeDataString(segment));
                    }
                }

                return segments;
            }
            catch (UriFormatException uriFormatException)
            {
                throw new ODataException(Strings.UriQueryPathParser_SyntaxError, uriFormatException);
            }
        }
    }
}
