//   OData .NET Libraries ver. 6.8.1
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

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Traverse the tree to the last token, then add a new token there if provided.
    /// </summary>
    internal class AddNewEndingTokenVisitor : IPathSegmentTokenVisitor
    {
        /// <summary>
        /// The new token to add to the tree
        /// </summary>
        private readonly PathSegmentToken newTokenToAdd;

        /// <summary>
        /// Create a new AddNewEndingTokenVisitor, with the new token to add at the end.
        /// </summary>
        /// <param name="newTokenToAdd">a new token to add at the end of the path, can be null</param>
        public AddNewEndingTokenVisitor(PathSegmentToken newTokenToAdd)
        {
            this.newTokenToAdd = newTokenToAdd;
        }

        /// <summary>
        /// Traverse a SystemToken. Always throws because a SystemToken is illegal in this case.
        /// </summary>
        /// <param name="tokenIn">The system token to traverse</param>
        public void Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Traverse a NonSystemToken. 
        /// </summary>
        /// <param name="tokenIn">The NonSystemToken to traverse.</param>
        public void Visit(NonSystemToken tokenIn)
        {
            if (tokenIn.NextToken == null)
            {
                if (newTokenToAdd != null)
                {
                    tokenIn.SetNextToken(newTokenToAdd);
                }
            }
            else
            {
                tokenIn.NextToken.Accept(this);
            }
        }
    }
}
