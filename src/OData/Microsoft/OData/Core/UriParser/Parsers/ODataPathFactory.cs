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
