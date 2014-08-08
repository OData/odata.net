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
    /// Object that knows how to parse a single term within a select expression. That is, apath to a property, 
    /// a wildcard, operation name, etc.
    /// </summary>
    internal sealed class NonOptionSelectExpandTermParser : SelectExpandTermParser
    {
        /// <summary>
        /// Build the NonOption strategy.
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxDepth">max recursive depth</param>
        public NonOptionSelectExpandTermParser(string clauseToParse, int maxDepth) : base(clauseToParse, maxDepth)
        {
        }

        /// <summary>
        /// Build the list of expand options
        /// Depends on whether options are allowed or not.
        /// </summary>
        /// <param name="isInnerTerm">is this an inner expand term</param>
        /// <param name="pathToken">the current level token, as a PathToken</param>
        /// <returns>An expand term token based on the path token.</returns>
        internal override ExpandTermToken BuildExpandTermToken(bool isInnerTerm, PathSegmentToken pathToken)
        {
            DebugUtils.CheckNoExternalCallers();
            if (this.IsNotEndOfTerm(false))
            {
                throw new ODataException(ODataErrorStrings.UriSelectParser_TermIsNotValid(this.Lexer.ExpressionText));
            }

            return new ExpandTermToken(pathToken);
        }

        /// <summary>
        /// determine whether we're at the end of a select or expand term
        /// </summary>
        /// <param name="isInnerTerm">flag to indicate whether this is an outer or inner select.</param>
        /// <returns>true if we are not at the end of a select term.</returns>
        internal override bool IsNotEndOfTerm(bool isInnerTerm)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.Lexer.CurrentToken.Kind != ExpressionTokenKind.End &&
                   this.Lexer.CurrentToken.Kind != ExpressionTokenKind.Comma;
        }
    }
}
