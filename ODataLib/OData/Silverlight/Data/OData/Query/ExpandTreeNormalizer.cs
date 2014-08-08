//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Query.SyntacticAst
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Translator from the old expand syntax tree to the new Expand Option syntax tree
    /// </summary>
    internal static class ExpandTreeNormalizer
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
            ExpandToken invertedPathTree = InvertPaths(treeToNormalize);

            return CombineTerms(invertedPathTree);
        }

        /// <summary>
        /// Invert the all of the paths in an expandToken, such that they are now in the same order as they are present in the 
        /// base url
        /// </summary>
        /// <param name="treeToInvert">the tree to invert paths on</param>
        /// <returns>a new tree with all of its paths inverted</returns>
        public static ExpandToken InvertPaths(ExpandToken treeToInvert)
        {
            // iterate through each expand term token, and reverse the tree in its path property
            List<ExpandTermToken> updatedTerms = new List<ExpandTermToken>();
            foreach (ExpandTermToken term in treeToInvert.ExpandTerms)
            {
                PathReverser pathReverser = new PathReverser();
                PathSegmentToken reversedPath = term.PathToNavProp.Accept(pathReverser);
                ExpandTermToken newTerm = new ExpandTermToken(reversedPath, term.FilterOption, term.OrderByOption, term.TopOption, term.SkipOption, term.InlineCountOption, term.SelectOption, term.ExpandOption);
                updatedTerms.Add(newTerm);
            }

            return new ExpandToken(updatedTerms);
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
                ExpandTermToken expandedTerm = BuildSubExpandTree(termToken);
                AddOrCombine(combinedTerms, expandedTerm);
            }

            return new ExpandToken(combinedTerms.Values);
        }

        /// <summary>
        /// Expand all the PathTokens in a particular term into their own separate terms.
        /// </summary>
        /// <param name="termToExpand">the term to expand</param>
        /// <returns>a new ExpandTermToken with each PathToken at its own level.</returns>
        public static ExpandTermToken BuildSubExpandTree(ExpandTermToken termToExpand)
        {
            // walk up the path tree on the property token, adding new expand clauses for each level
            if (termToExpand.PathToNavProp.NextToken == null)
            {
                return termToExpand;
            }

            PathSegmentToken currentProperty = termToExpand.PathToNavProp;

            // if we find a type token, then the current property becomes a path instead, 
            // so that we can follow the chain of derived types from the base.
            // the path can only consist of NavProps or TypeSegments, we can
            // simply walk the tree of the current property until we find a non
            // type segment, then break its next link, then return the new path chain which
            // becomes our current property
            PathSegmentToken currentToken = currentProperty;
            while (currentToken.IsNamespaceOrContainerQualified())
            {
                currentToken = currentToken.NextToken;
                if (currentToken == null)
                {
                    throw new ODataException(ODataErrorStrings.ExpandTreeNormalizer_NonPathInPropertyChain);
                }
            }

            // get a pointer to the next property so that we can continue down the list.
            PathSegmentToken nextProperty = currentToken.NextToken;

            // chop off the next pointer to end this chain.
            currentToken.SetNextToken(null);

            ExpandToken subExpand;
            if (nextProperty != null)
            {
                var subExpandTermToken = new ExpandTermToken(
                    nextProperty, 
                    termToExpand.FilterOption, 
                    termToExpand.OrderByOption, 
                    termToExpand.TopOption,
                    termToExpand.SkipOption, 
                    termToExpand.InlineCountOption, 
                    termToExpand.SelectOption, 
                    /*expandOption*/ null);
                ExpandTermToken subExpandToken = BuildSubExpandTree(subExpandTermToken);
                subExpand = new ExpandToken(new[] { subExpandToken });
            }
            else
            {
                subExpand = new ExpandToken(new ExpandTermToken[0]);
            }

            return new ExpandTermToken(
                currentProperty, 
                termToExpand.FilterOption, 
                termToExpand.OrderByOption,
                termToExpand.TopOption,
                termToExpand.SkipOption,
                termToExpand.InlineCountOption,
                termToExpand.SelectOption,
                subExpand);
        }

        /// <summary>
        /// add a new expandTermToken into an exisiting token, adding any additional levels and trees along the way.
        /// </summary>
        /// <param name="existingToken">the exisiting (already expanded) token</param>
        /// <param name="newToken">the new (already expanded) token</param>
        /// <returns>the combined token, or, if the two are mutually exclusive, the same tokens</returns>
        public static ExpandTermToken CombineTerms(ExpandTermToken existingToken, ExpandTermToken newToken)
        {
            Debug.Assert(new PathSegmentTokenEqualityComparer().Equals(existingToken.PathToNavProp, newToken.PathToNavProp), "Paths should be equal.");

            List<ExpandTermToken> childNodes = CombineChildNodes(existingToken, newToken).ToList();
            return new ExpandTermToken(
                    existingToken.PathToNavProp,
                    existingToken.FilterOption,
                    existingToken.OrderByOption,
                    existingToken.TopOption,
                    existingToken.SkipOption,
                    existingToken.InlineCountOption,
                    existingToken.SelectOption,
                    new ExpandToken(childNodes));
        }

        /// <summary>
        /// Combine the child nodes of twoExpandTermTokens into one list of tokens
        /// </summary>
        /// <param name="existingToken">the existing token to to</param>
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
            if (combinedTerms.TryGetValue(expandedTerm.PathToNavProp, out existingTerm))
            {
                combinedTerms[expandedTerm.PathToNavProp] = CombineTerms(expandedTerm, existingTerm);
            }
            else
            {
                combinedTerms.Add(expandedTerm.PathToNavProp, expandedTerm);
            }
        }
    }
}
