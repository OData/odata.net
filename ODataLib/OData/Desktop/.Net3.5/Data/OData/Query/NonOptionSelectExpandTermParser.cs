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
