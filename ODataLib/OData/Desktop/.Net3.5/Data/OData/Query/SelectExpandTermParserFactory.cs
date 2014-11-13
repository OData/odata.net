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
    /// Build an ISelectExpandTermParser based on settings.
    /// </summary>
    internal static class SelectExpandTermParserFactory
    {
        /// <summary>
        /// Build a new ISelectExpandTermParser, either with expand options or without, based on the global settings.
        /// </summary>
        /// <param name="clauseToParse">the select or expand text to parse</param>
        /// <param name="settings">pointer to the top level object</param>
        /// <returns>A new ISelectExpandTermParser</returns>
        public static ISelectExpandTermParser Create(string clauseToParse, ODataUriParserSettings settings)
        {
            if (settings.SupportExpandOptions)
            {
                return new ExpandOptionSelectExpandTermParser(clauseToParse, settings.SelectExpandLimit);
            }
            else
            {
                return new NonOptionSelectExpandTermParser(clauseToParse, settings.SelectExpandLimit);
            }
        }

        /// <summary>
        /// Build a new ISelectExpandTermParser with default settings
        /// </summary>
        /// <param name="clauseToParse">the clause to parse</param>
        /// <returns>A NonOptionSelectExpandTermParser</returns>
        public static ISelectExpandTermParser Create(string clauseToParse)
        {
            return new NonOptionSelectExpandTermParser(clauseToParse, ODataUriParserSettings.DefaultSelectExpandLimit);
        }
    }
}
