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
