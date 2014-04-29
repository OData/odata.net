//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Syntactic;
    using Microsoft.OData.Core.UriParser.Visitors;

    /// <summary>
    /// Reverse a Path
    /// </summary>
    internal sealed class PathReverser : PathSegmentTokenVisitor<PathSegmentToken>
    {
        /// <summary>
        /// any children of the root, will always be null on first call
        /// </summary>
        private readonly PathSegmentToken childToken;

        /// <summary>
        /// Build a PathReverser at the top level (with no child token)
        /// </summary>
        public PathReverser()
        {
            this.childToken = null;
        }

        /// <summary>
        /// Build a PathReverser based on a child token.
        /// </summary>
        /// <param name="childToken">the new child of this token</param>
        private PathReverser(PathSegmentToken childToken)
        {
            this.childToken = childToken;
        }

        /// <summary>
        /// Reverse a NonSystemToken
        /// </summary>
        /// <param name="tokenIn">the non system token to reverse</param>
        /// <returns>the reversed NonSystemToken</returns>
        public override PathSegmentToken Visit(NonSystemToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            if (tokenIn.NextToken != null)
            {
                NonSystemToken newNonSystemToken = new NonSystemToken(tokenIn.Identifier, tokenIn.NamedValues, this.childToken);
                return BuildNextStep(tokenIn.NextToken, newNonSystemToken);
            }
            else
            {
                return new NonSystemToken(tokenIn.Identifier, tokenIn.NamedValues, this.childToken);
            }
        }

        /// <summary>
        /// Reverse a SystemToken
        /// </summary>
        /// <param name="tokenIn">the SystemToken to reverse</param>
        /// <returns>the reversed SystemToken</returns>
        public override PathSegmentToken Visit(SystemToken tokenIn)
        {
            ExceptionUtils.CheckArgumentNotNull(tokenIn, "tokenIn");
            if (tokenIn.NextToken != null)
            {
                SystemToken newNonSystemToken = new SystemToken(tokenIn.Identifier, this.childToken);
                return BuildNextStep(tokenIn.NextToken, newNonSystemToken);
            }
            else
            {
                return new SystemToken(tokenIn.Identifier, this.childToken);
            }
        }

        /// <summary>
        /// Build the next level PathReverser
        /// </summary>
        /// <param name="nextLevelToken">the next level token</param>
        /// <param name="nextChildToken">the next levels child token</param>
        /// <returns>the path token from the next level.</returns>
        private static PathSegmentToken BuildNextStep(PathSegmentToken nextLevelToken, PathSegmentToken nextChildToken)
        {
            PathReverser nextStepReverser = new PathReverser(nextChildToken);
            return nextLevelToken.Accept(nextStepReverser);
        }
    }
}
