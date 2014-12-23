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
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Build a string based on a path that contains only expands.
    /// </summary>
    internal class ExpandOnlyPathToStringVisitor : IPathSegmentTokenVisitor<string>
    {
        /// <summary>
        /// Const to represent the beginning of a sub expand clause.
        /// </summary>
        //// ($expand= 
        private readonly string subExpandStartingText = 
            new StringBuilder().Append(UriHelper.LEFTPAREN)
            .Append(UriHelper.DOLLARSIGN)
            .Append(UriHelper.OPTIONEXPAND)
            .Append(UriHelper.EQUALSSIGN).ToString();

        /// <summary>
        /// Visit a SystemToken
        /// </summary>
        /// <param name="tokenIn">the system token to visit</param>
        /// <returns>Always throws, since a system token is invalid in an expand path.</returns>
        public string Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Visit a NonSystemToken
        /// </summary>
        /// <param name="tokenIn">the token to visit</param>
        /// <returns>A string containing the expand represented by the input token.</returns>
        public string Visit(NonSystemToken tokenIn)
        {
            if (tokenIn.IsNamespaceOrContainerQualified())
            {
                if (tokenIn.NextToken != null)
                {
                    return tokenIn.Identifier + "/" + tokenIn.NextToken.Accept(this);
                }
                else
                {
                    throw new NotSupportedException(Strings.ALinq_TypeTokenWithNoTrailingNavProp(tokenIn.Identifier));
                }
            }
            else
            {
                if (tokenIn.NextToken == null)
                {
                    return tokenIn.Identifier;
                }
                else
                {
                    return tokenIn.Identifier + subExpandStartingText + tokenIn.NextToken.Accept(this) + UriHelper.RIGHTPAREN;
                }
            }
        }
    }
}
