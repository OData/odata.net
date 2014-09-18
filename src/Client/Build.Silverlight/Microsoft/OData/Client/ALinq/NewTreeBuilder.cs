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
