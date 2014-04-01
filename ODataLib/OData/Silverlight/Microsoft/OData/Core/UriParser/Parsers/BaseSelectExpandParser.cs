//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using Microsoft.OData.Core.UriParser.Syntactic;

    /// <summary>
    /// Object that knows how to parse a single term within a select expression. That is, a path to a property, 
    /// a wildcard, operation name, etc.
    /// TODO 1466134 We don't need this layer once V4 is working and always used.
    /// </summary>
    internal abstract class BaseSelectExpandParser : ISelectExpandParser
    {
        /// <summary>
        /// Lexer used to parse an the $select or $expand string.
        /// </summary>
        protected ExpressionLexer Lexer;

        /// <summary>
        /// True if we are we parsing $select.
        /// </summary>
        protected bool IsSelect;

        /// <summary>
        /// The current recursion depth.
        /// </summary>
        protected int RecursionDepth;

        /// <summary>
        /// The maximum allowable recursive depth.
        /// </summary>
        private readonly int maxRecursiveDepth;

        /// <summary>
        /// Create a SelectExpandTermParser.
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <param name="maxRecursiveDepth">the maximum recursive depth</param>
        protected BaseSelectExpandParser(string clauseToParse, int maxRecursiveDepth)
        {
            this.maxRecursiveDepth = maxRecursiveDepth;
            this.RecursionDepth = 0;

            // Sets up our lexer. We don't turn useSemicolonDelimiter on since the parsing code for expand options, 
            // which is the only thing that needs it, is in a different class that uses it's own lexer.
            this.Lexer = clauseToParse != null ? new ExpressionLexer(clauseToParse, false /*moveToFirstToken*/, false /*useSemicolonDelimiter*/) : null;
        }

        /// <summary>
        /// The maximum recursive depth.
        /// </summary>
        public int MaxRecursiveDepth 
        {
            get { return this.maxRecursiveDepth; }
        }

        /// <summary>
        /// Parses a full $select expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public abstract SelectToken ParseSelect();

        /// <summary>
        /// Parses a full $expand expression.
        /// </summary>
        /// <returns>The lexical token representing the select.</returns>
        public abstract ExpandToken ParseExpand();
    }
}
