//   OData .NET Libraries ver. 6.9
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
