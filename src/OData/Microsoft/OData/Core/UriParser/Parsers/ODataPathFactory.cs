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
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.UriParser.Semantic;

    /// <summary>Translates from an IPathSegment into an ODataPath</summary>
    internal static class ODataPathFactory
    {
        /// <summary>
        /// Binds a collection of <paramref name="segments"/> to metadata, creating a semantic ODataPath object.
        /// </summary>
        /// <param name="segments">Collection of path segments.</param>
        /// <param name="configuration">The configuration to use when binding the path.</param>
        /// <returns>A semantic <see cref="ODataPath"/> object to describe the path.</returns>
        internal static ODataPath BindPath(ICollection<string> segments, ODataUriParserConfiguration configuration)
        {
            ODataPathParser semanticPathParser = new ODataPathParser(configuration);
            var intermediateSegments = semanticPathParser.ParsePath(segments);
            ODataPath path = new ODataPath(intermediateSegments);
            return path;
        }
    }
}
