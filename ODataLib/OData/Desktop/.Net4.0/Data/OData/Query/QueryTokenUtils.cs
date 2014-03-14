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

            // TODO - we ignore the case since this is the value of a query option
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
