//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with query tokens.
    /// </summary>
    internal static class QueryTokenUtils
    {
        /// <summary>
        /// Try to parse the given string to count.
        /// </summary>
        /// <param name="count">The string to be parsed.</param>
        /// <returns>query count value if successful. Null if not.</returns>
        internal static bool? ParseQueryCount(string count)
        {
            if (count == null)
            {
                return null;
            }

            // TODO: we ignore the case since this is the value of a query option
            if (string.Equals(count, ExpressionConstants.KeywordTrue, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(count, ExpressionConstants.KeywordFalse, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new ODataException(Strings.SyntacticTree_InvalidCountQueryOptionValue(
                    count,
                    string.Join(", ", new string[] { ExpressionConstants.KeywordTrue, ExpressionConstants.KeywordFalse })));
        }

        /// <summary>
        /// Try to parse the given segment name as a KeywordKind.
        /// </summary>
        /// <param name="segment">The segment name.</param>
        /// <returns>A KeywordKind value if successful. Null if not.</returns>
        internal static KeywordKind? ParseKeywordKind(string segment)
        {
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

                case UriQueryConstants.RefSegment:
                    return KeywordKind.Ref;
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
            switch (keyword)
            {
                case KeywordKind.Batch:
                    return UriQueryConstants.BatchSegment;

                case KeywordKind.Count:
                    return UriQueryConstants.CountSegment;

                case KeywordKind.Ref:
                    return UriQueryConstants.RefSegment;

                case KeywordKind.Metadata:
                    return UriQueryConstants.MetadataSegment;

                case KeywordKind.Value:
                    return UriQueryConstants.ValueSegment;
            }

            throw new InvalidOperationException("Should not have reached here with kind: " + keyword);
        }
    }
}
