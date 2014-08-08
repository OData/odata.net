//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
