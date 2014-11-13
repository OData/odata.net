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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Collections.Generic;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with query tokens.
    /// </summary>
    internal static class QueryTokenUtils
    {
        /// <summary>
        /// Try to parse the given string as a InlineCountKind.
        /// </summary>
        /// <param name="inlineCount">The string to be parsed.</param>
        /// <returns>A InlineCountKind value if successful. Null if not.</returns>
        internal static InlineCountKind? ParseInlineCountKind(string inlineCount)
        {
            DebugUtils.CheckNoExternalCallers();
            if (inlineCount == null) 
            { 
                return null; 
            }

            // TODO: we ignore the case since this is the value of a query option
            if (string.Equals(inlineCount, ExpressionConstants.InlineCountAllPages, StringComparison.OrdinalIgnoreCase))
            {
                return InlineCountKind.AllPages;
            }

            if (string.Equals(inlineCount, ExpressionConstants.InlineCountNone, StringComparison.OrdinalIgnoreCase))
            {
                return InlineCountKind.None;
            }

            throw new ODataException(Strings.SyntacticTree_InvalidInlineCountQueryOptionValue(
                    inlineCount,
                    string.Join(", ", new string[] { ExpressionConstants.InlineCountNone, ExpressionConstants.InlineCountAllPages })));
        }

        /// <summary>
        /// Try to parse the given segment name as a KeywordKind.
        /// </summary>
        /// <param name="segment">The segment name.</param>
        /// <returns>A KeywordKind value if successful. Null if not.</returns>
        internal static KeywordKind? ParseKeywordKind(string segment)
        {
            DebugUtils.CheckNoExternalCallers();
            switch (segment)
            {
                case UriQueryConstants.MetadataSegment:
                    return KeywordKind.Metadata;

                case UriQueryConstants.CountSegment:
                    return KeywordKind.Count;

                case UriQueryConstants.ValueSegment:
                    return KeywordKind.Value;

                case UriQueryConstants.BatchSegment:
                    return KeywordKind.Batch;

                case UriQueryConstants.LinkSegment:
                    return KeywordKind.Links;
            }

            return null;
        }

        /// <summary>
        /// Get the Uri name equivalent of the given KeywordKind.
        /// </summary>
        /// <param name="keyword">The KeywordKind to get name for.</param>
        /// <returns>A $ keyword that represent the given keyword.</returns>
        internal static string GetNameFromKeywordKind(KeywordKind keyword)
        {
            DebugUtils.CheckNoExternalCallers();
            switch (keyword)
            {
                case KeywordKind.Batch:
                    return UriQueryConstants.BatchSegment;

                case KeywordKind.Count:
                    return UriQueryConstants.CountSegment;

                case KeywordKind.Links:
                    return UriQueryConstants.LinkSegment;

                case KeywordKind.Metadata:
                    return UriQueryConstants.MetadataSegment;

                case KeywordKind.Value:
                    return UriQueryConstants.ValueSegment;
            }

            throw new InvalidOperationException("Should not have reached here with kind: " + keyword);
        }
    }
}
