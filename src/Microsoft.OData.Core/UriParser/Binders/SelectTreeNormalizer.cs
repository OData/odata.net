//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Translate a select tree into the right format.
    /// </summary>
    internal static class SelectTreeNormalizer
    {
        /// <summary>
        /// Normalize a <see cref="SelectToken"/>.
        /// </summary>
        /// <param name="selectToken">The select token to normalize</param>
        /// <returns>Normalized SelectToken</returns>
        public static SelectToken NormalizeSelectTree(SelectToken selectToken)
        {
            // Be noted: It's not allowed have multple select clause with same path.
            // For example: $select=abc($top=2),abc($skip=2) is not allowed by design.
            // Cusotmer should combine them together, for example: $select=abc($top=2;$skip=2).
            // The logic is different with ExpandTreeNormalizer. We should change the logic in ExpandTreeNormalizer
            // in next breaking change version.
            VerifySelectToken(selectToken);

            // To normalize the select tree we need to:
            // invert the path tree on each of its select term tokens
            selectToken = NormalizeSelectPaths(selectToken);

            return selectToken;
        }

        private static void VerifySelectToken(SelectToken selectToken)
        {
            if (selectToken == null)
            {
                return;
            }

            HashSet<PathSegmentToken> pathTerms = new HashSet<PathSegmentToken>(new PathSegmentTokenEqualityComparer());
            foreach (SelectTermToken term in selectToken.SelectTerms)
            {
                if (pathTerms.Contains(term.PathToProperty))
                {
                    throw new ODataException(ODataErrorStrings.SelectTreeNormalizer_MultipleSelecTermWithSamePathFound(ToPathString(term.PathToProperty)));
                }
                else
                {
                    pathTerms.Add(term.PathToProperty);
                }

                // Verify at next level
                if (term.SelectOption != null)
                {
                    VerifySelectToken(term.SelectOption);
                }
            }
        }

        /// <summary>
        /// Get the path string for a path segment token.
        /// </summary>
        /// <param name="head">The head of the path</param>
        /// <returns>The path string.</returns>
        private static string ToPathString(PathSegmentToken head)
        {
            StringBuilder sb = new StringBuilder();
            PathSegmentToken curr = head;
            while (curr != null)
            {
                sb.Append(curr.Identifier);

                NonSystemToken nonSystem = curr as NonSystemToken;
                if (nonSystem != null && nonSystem.NamedValues != null)
                {
                    sb.Append("(");
                    bool first = true;
                    foreach (var item in nonSystem.NamedValues)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }

                        sb.Append(item.Name).Append("=").Append(item.Value.Value);
                    }

                    sb.Append(")");
                }

                curr = curr.NextToken;
                if (curr != null)
                {
                    sb.Append("/");
                }
            }

            return sb.ToString();
        }

        private static SelectToken NormalizeSelectPaths(SelectToken selectToken)
        {
            if (selectToken != null)
            {
                // iterate through each select term token, and reverse the tree in its path property
                foreach (SelectTermToken term in selectToken.SelectTerms)
                {
                    term.PathToProperty = term.PathToProperty.Reverse();

                    // we also need to call the select token normalizer for this level to reverse the select paths
                    if (term.SelectOption != null)
                    {
                        term.SelectOption = NormalizeSelectPaths(term.SelectOption);
                    }
                }
            }

            return selectToken;
        }
    }
}