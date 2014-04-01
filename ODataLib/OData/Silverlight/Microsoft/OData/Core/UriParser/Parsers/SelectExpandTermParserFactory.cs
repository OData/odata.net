//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Core.UriParser.Syntactic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

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
        public static ISelectExpandParser Create(string clauseToParse, ODataUriParserSettings settings)
        {
            if (settings.SupportExpandOptions)
            {
                return new V4SelectExpandParser(clauseToParse, settings.SelectExpandLimit);
            }
            else
            {
                return new NonOptionSelectExpandParser(clauseToParse, settings.SelectExpandLimit);
            }
        }
    }
}
