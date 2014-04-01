//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Interface for normalizing an expand tree
    /// </summary>
    //// TODO 1466134 We don't need this layer once V4 is working and always used.
    internal interface IExpandTreeNormalizer
    {
        /// <summary>
        /// Normalize an expand syntax tree into the new ExpandOption syntax.
        /// </summary>
        /// <param name="treeToNormalize">the tree to normalize</param>
        /// <returns>a new tree, in the new ExpandOption syntax</returns>
        ExpandToken NormalizeExpandTree(ExpandToken treeToNormalize);

        /// <summary>
        /// Invert the all of the paths in an expandToken, such that they are now in the same order as they are present in the 
        /// base url
        /// </summary>
        /// <param name="treeToInvert">the tree to invert paths on</param>
        /// <returns>a new tree with all of its paths inverted</returns>
        ExpandToken InvertPaths(ExpandToken treeToInvert);

        /// <summary>
        /// Collapse all redundant terms in an expand tree
        /// </summary>
        /// <param name="treeToCollapse">the tree to collapse</param>
        /// <returns>A new tree with all redundant terms collapsed.</returns>
        ExpandToken CombineTerms(ExpandToken treeToCollapse);

        /// <summary>
        /// Expand all the PathTokens in a particular term into their own separate terms.
        /// </summary>
        /// <param name="termToExpand">the term to expand</param>
        /// <returns>a new ExpandTermToken with each PathToken at its own level.</returns>
        ExpandTermToken BuildSubExpandTree(ExpandTermToken termToExpand);

        /// <summary>
        /// add a new expandTermToken into an exisiting token, adding any additional levels and trees along the way.
        /// </summary>
        /// <param name="existingToken">the exisiting (already expanded) token</param>
        /// <param name="newToken">the new (already expanded) token</param>
        /// <returns>the combined token, or, if the two are mutually exclusive, the same tokens</returns>
        ExpandTermToken CombineTerms(ExpandTermToken existingToken, ExpandTermToken newToken);

        /// <summary>
        /// Combine the child nodes of twoExpandTermTokens into one list of tokens
        /// </summary>
        /// <param name="existingToken">the existing token to to</param>
        /// <param name="newToken">the new token containing terms to add</param>
        /// <returns>a combined list of the all child nodes of the two tokens.</returns>
        IEnumerable<ExpandTermToken> CombineChildNodes(ExpandTermToken existingToken, ExpandTermToken newToken);
    }
}
