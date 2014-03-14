//   Copyright 2011 Microsoft Corporation
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
