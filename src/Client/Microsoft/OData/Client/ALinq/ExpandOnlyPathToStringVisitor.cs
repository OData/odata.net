//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
