//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.OData.Query.SyntacticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Interface for the SelectExpandTermParsing strategy
    /// </summary>
    internal interface ISelectExpandTermParser
    {
        /// <summary>
        /// Parses a full $select expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        SelectToken ParseSelect();

        /// <summary>
        /// Parses a full $expand expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        ExpandToken ParseExpand();

        /// <summary>
        /// Parses a single term in a comma seperated list of things to select.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner or outer select term</param>
        /// <returns>A token representing thing to select.</returns>
        PathSegmentToken ParseSingleSelectTerm(bool isInnerTerm);

        /// <summary>
        /// Parses a single term in a comma seperated list of things to expand.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner or outer term.</param>
        /// <returns>A token representing thing to expand.</returns>
        ExpandTermToken ParseSingleExpandTerm(bool isInnerTerm);
    }
}
