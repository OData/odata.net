//---------------------------------------------------------------------
// <copyright file="SelectTreeNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Translate a select tree into the right format.
    /// </summary>
    internal sealed class SelectTreeNormalizer
    {
        /// <summary>
        /// Normalize a <see cref="SelectToken"/>.
        /// </summary>
        /// <param name="selectToken">The select token to normalize</param>
        /// <returns>Normalized SelectToken</returns>
        public static SelectToken NormalizeSelectTree(SelectToken selectToken)
        {
            // To normalize the select tree we need to:
            // invert the path tree on each of its select term tokens
            selectToken = NormalizeSelectPaths(selectToken);

            // combine terms that start with the path tree
            return CombineSelectToken(selectToken);
        }

        /// <summary>
        /// Collapse all redundant select term tokens in a select tree.
        /// </summary>
        /// <param name="selectToken">the select token to collapse</param>
        /// <returns>The collapsed select tree.</returns>
        public static SelectToken CombineSelectToken(SelectToken selectToken)
        {
            if (selectToken == null)
            {
                return null;
            }

            var combinedTerms = new Dictionary<PathSegmentToken, SelectTermToken>(new PathSegmentTokenEqualityComparer());

            CombineSelectTokenToDictionary(selectToken, combinedTerms);

            return new SelectToken(combinedTerms.Values);
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

        private static SelectTermToken CombineTerms(SelectTermToken existingToken, SelectTermToken newToken)
        {
            return new SelectTermToken(
                    existingToken.PathToProperty,
                    CombineQueryOption(existingToken.FilterOption, newToken.FilterOption, "$filter"),
                    CombineQueryOption(existingToken.OrderByOptions, newToken.OrderByOptions, "$orderby"),
                    CombineQueryOption(existingToken.TopOption, newToken.TopOption, "$top"),
                    CombineQueryOption(existingToken.SkipOption, newToken.SkipOption, "$skip"),
                    CombineQueryOption(existingToken.CountQueryOption, newToken.CountQueryOption, "$count"),
                    CombineQueryOption(existingToken.SearchOption, newToken.SearchOption, "$search"),
                    CombineSelects(existingToken.SelectOption, newToken.SelectOption),
                    CombineQueryOption(existingToken.ComputeOption, newToken.ComputeOption, "$compute"));
        }

        private static SelectToken CombineSelects(SelectToken existingToken, SelectToken newToken)
        {
            if (existingToken == null)
            {
                return newToken;
            }

            if (newToken == null)
            {
                return existingToken;
            }

            var combinedTerms = new Dictionary<PathSegmentToken, SelectTermToken>(new PathSegmentTokenEqualityComparer());

            CombineSelectTokenToDictionary(existingToken, combinedTerms);

            CombineSelectTokenToDictionary(newToken, combinedTerms);

            return new SelectToken(combinedTerms.Values);
        }

        private static void CombineSelectTokenToDictionary(SelectToken select, IDictionary<PathSegmentToken, SelectTermToken> combinedTerms)
        {
            if (select != null)
            {
                foreach (SelectTermToken termToken in select.SelectTerms)
                {
                    if (termToken.SelectOption != null)
                    {
                        termToken.SelectOption = CombineSelectToken(termToken.SelectOption);
                    }

                    SelectTermToken existingTermToken;
                    if (combinedTerms.TryGetValue(termToken.PathToProperty, out existingTermToken))
                    {
                        combinedTerms[termToken.PathToProperty] = CombineTerms(existingTermToken, termToken);
                    }
                    else
                    {
                        combinedTerms[termToken.PathToProperty] = termToken;
                    }
                }
            }
        }

        // filter, orderby, search, comput, top, skip, count
        private static T CombineQueryOption<T>(T existingFilterToken, T newFilterToken, string identifier)
        {
            if (existingFilterToken == null)
            {
                return newFilterToken;
            }
            else if (newFilterToken == null)
            {
                return existingFilterToken;
            }

            // We don't allow muliple query options (exception for $select) by design.
            // Theoretically, we can compare the query option contents, if they are same, we can pick one.
            // for example: $select=1($top=5),1($top=5;$count=true), we can allow this because $top=5 is same.
            throw new ODataException(ODataErrorStrings.SelectTreeNormalizer_MultipleQueryOptionsFound(identifier));
        }
    }
}