//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
{
    using System;
    using Microsoft.OData.Client.ALinq.UriParser;

    /// <summary>
    /// Construct a new copy of an existing tree
    /// </summary>
    internal class NewTreeBuilder : IPathSegmentTokenVisitor<PathSegmentToken>
    {
        /// <summary>
        /// Visit a SystemToken
        /// </summary>
        /// <param name="tokenIn">The SystemToken to visit</param>
        /// <returns>Always throws, since a SystemToken is illegal in a select or expand path.</returns>
        public PathSegmentToken Visit(SystemToken tokenIn)
        {
            throw new NotSupportedException(Strings.ALinq_IllegalSystemQueryOption(tokenIn.Identifier));
        }

        /// <summary>
        /// Visit a NonSystemToken
        /// </summary>
        /// <param name="tokenIn">The non system token to visit</param>
        /// <returns>A new copy of the input token.</returns>
        public PathSegmentToken Visit(NonSystemToken tokenIn)
        {
            if (tokenIn == null)
            {
                return null;
            }
            else
            {
                PathSegmentToken subToken = tokenIn.NextToken != null ? tokenIn.NextToken.Accept(this) : null;
                return new NonSystemToken(tokenIn.Identifier, tokenIn.NamedValues, subToken);
            }
        }
    }
}
