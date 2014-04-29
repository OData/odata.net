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
    /// Combine a top level select and expand token.
    /// </summary>
    internal static class SelectExpandSyntacticUnifier
    {
        /// <summary>
        /// Combine a top level select and expand token
        /// </summary>
        /// <param name="expand">the original expand token</param>
        /// <param name="select">the original select token</param>
        /// <returns>A new ExpandToken with the original select token embedded within a new top level expand token.</returns>
        public static ExpandToken Combine(ExpandToken expand, SelectToken select)
        {
            // build a new top level expand token embedding the top level select token.
            ExpandTermToken newTopLevelExpandTerm = new ExpandTermToken(new SystemToken(ExpressionConstants.It, null), select, expand);
            return new ExpandToken(new List<ExpandTermToken>() { newTopLevelExpandTerm });
        }
    }
}
