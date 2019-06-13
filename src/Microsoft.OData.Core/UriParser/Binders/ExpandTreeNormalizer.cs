//---------------------------------------------------------------------
// <copyright file="ExpandTreeNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.UriParser
{
    internal sealed class SelectExpandTreeNormalizer
    {
        public static ExpandToken NormalizeExpandTree(ExpandToken expand)
        {
            foreach (ExpandTermToken term in expand.ExpandTerms)
            {
                NormalizePath(term);
            }

            // to normalize a select token we just need to invert its paths, so that
            // we match the ordering on an ExpandToken.
            return expand;
        }

        public static SelectToken NormalizeSelectTree(SelectToken select)
        {
            foreach (SelectTermToken term in select.SelectTerms)
            {
                NormalizePath(term);
            }

            // to normalize a select token we just need to invert its paths, so that
            // we match the ordering on an ExpandToken.
            return select;
        }

        public static SelectExpandTermToken NormalizePath(SelectExpandTermToken token)
        {
            token.PathToProperty = token.PathToProperty.Reverse();

            // we also need to call the select token normalizer for this level to reverse the select paths
            if (token.SelectOption != null)
            {
                token.SelectOption = NormalizeSelectTree(token.SelectOption);
            }

            if (token.ExpandOption != null)
            {
                token.ExpandOption = NormalizeExpandTree(token.ExpandOption);
            }

            return token;
        }
    }

    /// <summary>
    /// Translator from the old expand syntax tree to the new Expand Option syntax tree
    /// </summary>
    internal sealed class ExpandTreeNormalizer
    {
        /// <summary>
        /// Normalize an expand syntax tree into the new ExpandOption syntax.
        /// </summary>
        /// <param name="treeToNormalize">the tree to normalize</param>
        /// <returns>a new tree, in the new ExpandOption syntax</returns>
        public static ExpandToken NormalizeExpandTree(ExpandToken treeToNormalize)
        {
            // To normalize the expand tree we need to
            // 1) invert the path tree on each of its expand term tokens
            // 2) combine terms that start with the path tree
            ExpandToken invertedPathTree = NormalizePaths(treeToNormalize);

            return CombineTerms(invertedPathTree);
        }

        /// <summary>
        /// Invert the all of the paths in an expandToken, such that they are now in the same order as they are present in the
        /// base url
        /// </summary>
        /// <param name="treeToInvert">the tree to invert paths on</param>
        /// <returns>a new tree with all of its paths inverted</returns>
        public static ExpandToken NormalizePaths(ExpandToken treeToInvert)
        {
            // iterate through each expand term token, and reverse the tree in its path property
            foreach (ExpandTermToken term in treeToInvert.ExpandTerms)
            {
                term.PathToProperty = term.PathToNavigationProp.Reverse();

                // we also need to call the select token normalizer for this level to reverse the select paths
                if (term.SelectOption != null)
                {
                    term.SelectOption = SelectTreeNormalizer.NormalizeSelectTree(term.SelectOption);
                }

                if (term.ExpandOption != null)
                {
                    term.ExpandOption = NormalizePaths(term.ExpandOption);
                }
            }

            return treeToInvert;
        }

        /// <summary>
        /// Collapse all redundant terms in an expand tree
        /// </summary>
        /// <param name="treeToCollapse">the tree to collapse</param>
        /// <returns>A new tree with all redundant terms collapsed.</returns>
        public static ExpandToken CombineTerms(ExpandToken treeToCollapse)
        {
            var combinedTerms = new Dictionary<PathSegmentToken, ExpandTermToken>(new PathSegmentTokenEqualityComparer());
            foreach (ExpandTermToken termToken in treeToCollapse.ExpandTerms)
            {
                ExpandTermToken finalTermToken = termToken;
                if (termToken.ExpandOption != null)
                {
                    ExpandToken newSubExpand = CombineTerms(termToken.ExpandOption);
                    finalTermToken = new ExpandTermToken(
                                                              termToken.PathToNavigationProp,
                                                              termToken.FilterOption,
                                                              termToken.OrderByOptions,
                                                              termToken.TopOption,
                                                              termToken.SkipOption,
                                                              termToken.CountQueryOption,
                                                              termToken.LevelsOption,
                                                              termToken.SearchOption,
                                                              RemoveDuplicateSelect(termToken.SelectOption),
                                                              newSubExpand,
                                                              termToken.ComputeOption,
                                                              termToken.ApplyOptions);
                }

                AddOrCombine(combinedTerms, finalTermToken);
            }

            return new ExpandToken(combinedTerms.Values);
        }

        /// <summary>
        /// add a new expandTermToken into an exisiting token, adding any additional levels and trees along the way.
        /// </summary>
        /// <param name="existingToken">the exisiting (already expanded) token</param>
        /// <param name="newToken">the new (already expanded) token</param>
        /// <returns>the combined token, or, if the two are mutually exclusive, the same tokens</returns>
        public static ExpandTermToken CombineTerms(ExpandTermToken existingToken, ExpandTermToken newToken)
        {
            Debug.Assert(new PathSegmentTokenEqualityComparer().Equals(existingToken.PathToNavigationProp, newToken.PathToNavigationProp), "Paths should be equal.");

            List<ExpandTermToken> childNodes = CombineChildNodes(existingToken, newToken).ToList();
            SelectToken combinedSelects = CombineSelects(existingToken, newToken);
            return new ExpandTermToken(
                    existingToken.PathToNavigationProp,
                    existingToken.FilterOption,
                    existingToken.OrderByOptions,
                    existingToken.TopOption,
                    existingToken.SkipOption,
                    existingToken.CountQueryOption,
                    existingToken.LevelsOption,
                    existingToken.SearchOption,
                    combinedSelects,
                    new ExpandToken(childNodes),
                    existingToken.ComputeOption,
                    existingToken.ApplyOptions);
        }

        /// <summary>
        /// Combine the child nodes of two ExpandTermTokens into one list of tokens
        /// </summary>
        /// <param name="existingToken">the existing token</param>
        /// <param name="newToken">the new token containing terms to add</param>
        /// <returns>a combined list of the all child nodes of the two tokens.</returns>
        public static IEnumerable<ExpandTermToken> CombineChildNodes(ExpandTermToken existingToken, ExpandTermToken newToken)
        {
            if (existingToken.ExpandOption == null && newToken.ExpandOption == null)
            {
                return new List<ExpandTermToken>();
            }

            var childNodes = new Dictionary<PathSegmentToken, ExpandTermToken>(new PathSegmentTokenEqualityComparer());
            if (existingToken.ExpandOption != null)
            {
                AddChildOptionsToDictionary(existingToken, childNodes);
            }

            if (newToken.ExpandOption != null)
            {
                AddChildOptionsToDictionary(newToken, childNodes);
            }

            return childNodes.Values;
        }

        public static IEnumerable<SelectTermToken> CombineChildNodes(SelectTermToken existingToken, SelectTermToken newToken)
        {
            if (existingToken.ExpandOption == null && newToken.ExpandOption == null)
            {
                return new List<SelectTermToken>();
            }

            var childNodes = new Dictionary<PathSegmentToken, SelectTermToken>(new PathSegmentTokenEqualityComparer());
            if (existingToken.ExpandOption != null)
            {
                AddChildOptionsToDictionary(existingToken, childNodes);
            }

            if (newToken.ExpandOption != null)
            {
                AddChildOptionsToDictionary(newToken, childNodes);
            }

            return childNodes.Values;
        }

        /// <summary>
        /// Add child options to a new dictionary
        /// </summary>
        /// <param name="newToken">the token with child nodes to add to the dictionary</param>
        /// <param name="combinedTerms">dictionary to add child nodes to</param>
        private static void AddChildOptionsToDictionary(SelectTermToken newToken, Dictionary<PathSegmentToken, SelectTermToken> combinedTerms)
        {
            foreach (SelectTermToken expandedTerm in newToken.SelectOption.SelectTerms)
            {
                AddOrCombine(combinedTerms, expandedTerm);
            }
        }

        /// <summary>
        /// Adds the expand token to the dictionary or combines it with an existing  or combines it with another existing token with an equivalent path.
        /// </summary>
        /// <param name="combinedTerms">The combined terms dictionary.</param>
        /// <param name="expandedTerm">The expanded term to add or combine.</param>
        private static void AddOrCombine(IDictionary<PathSegmentToken, SelectTermToken> combinedTerms, SelectTermToken expandedTerm)
        {
            SelectTermToken existingTerm;
            if (combinedTerms.TryGetValue(expandedTerm.PathToProperty, out existingTerm))
            {
               // combinedTerms[expandedTerm.PathToProperty] = CombineTerms(expandedTerm, existingTerm);
            }
            else
            {
                combinedTerms.Add(expandedTerm.PathToProperty, expandedTerm);
            }
        }

        /// <summary>
        /// Add child options to a new dictionary
        /// </summary>
        /// <param name="newToken">the token with child nodes to add to the dictionary</param>
        /// <param name="combinedTerms">dictionary to add child nodes to</param>
        private static void AddChildOptionsToDictionary(ExpandTermToken newToken, Dictionary<PathSegmentToken, ExpandTermToken> combinedTerms)
        {
            foreach (ExpandTermToken expandedTerm in newToken.ExpandOption.ExpandTerms)
            {
                AddOrCombine(combinedTerms, expandedTerm);
            }
        }

        /// <summary>
        /// Adds the expand token to the dictionary or combines it with an existing  or combines it with another existing token with an equivalent path.
        /// </summary>
        /// <param name="combinedTerms">The combined terms dictionary.</param>
        /// <param name="expandedTerm">The expanded term to add or combine.</param>
        private static void AddOrCombine(IDictionary<PathSegmentToken, ExpandTermToken> combinedTerms, ExpandTermToken expandedTerm)
        {
            ExpandTermToken existingTerm;
            if (combinedTerms.TryGetValue(expandedTerm.PathToNavigationProp, out existingTerm))
            {
                combinedTerms[expandedTerm.PathToNavigationProp] = CombineTerms(expandedTerm, existingTerm);
            }
            else
            {
                combinedTerms.Add(expandedTerm.PathToNavigationProp, expandedTerm);
            }
        }

        /// <summary>
        /// Combine together the select clauses of two ExpandTermTokens
        /// </summary>
        /// <param name="existingToken">the already existing expand term token</param>
        /// <param name="newToken">the new expand term token to be added</param>
        /// <returns>A new select term containing each of the selected entries.</returns>
        private static SelectToken CombineSelects(ExpandTermToken existingToken, ExpandTermToken newToken)
        {
            if (existingToken.SelectOption == null)
            {
                return newToken.SelectOption;
            }

            if (newToken.SelectOption == null)
            {
                return existingToken.SelectOption;
            }

            List<PathSegmentToken> newSelects = existingToken.SelectOption.Properties.ToList();
            newSelects.AddRange(newToken.SelectOption.Properties);
            return new SelectToken(newSelects.Distinct(new PathSegmentTokenEqualityComparer()));
        }

        /// <summary>
        /// Get rid of duplicate selected item in SelectToken
        /// </summary>
        /// <param name="selectToken">Select token to be dealt with</param>
        /// <returns>A new select term containing each of the </returns>
        private static SelectToken RemoveDuplicateSelect(SelectToken selectToken)
        {
            return selectToken != null
                    ? new SelectToken(selectToken.Properties.Distinct(new PathSegmentTokenEqualityComparer()))
                    : null;
        }
    }
}
