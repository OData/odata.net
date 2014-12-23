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

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Remove any wildcard characters from a given path.
    /// </summary>
    internal class RemoveWildcardVisitor : IPathSegmentTokenVisitor
    {
        /// <summary>
        /// The previous token, used as a cursor so that we can cut off the 
        /// next pointer of the previous token if we find a wildcard.
        /// </summary>
        private PathSegmentToken previous = null;

        /// <summary>
        /// Translate a SystemToken, this is illegal so this always throws.
        /// </summary>
        /// <param name="tokenIn">The SystemToken to translate.</param>
        public void Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Translate a NonSystemToken.
        /// </summary>
        /// <param name="tokenIn">The NonSystemToken to translate.</param>
        public void Visit(NonSystemToken tokenIn)
        {
            if (tokenIn.Identifier != UriHelper.ASTERISK.ToString())
            {
                if (tokenIn.NextToken == null)
                {
                    return;
                }

                previous = tokenIn;
                tokenIn.NextToken.Accept(this);
            }
            else
            {
                previous.SetNextToken(null);
                return;
            }
        }
    }
}
