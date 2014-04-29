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
