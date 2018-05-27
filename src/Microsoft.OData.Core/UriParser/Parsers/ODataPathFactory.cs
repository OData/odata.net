//---------------------------------------------------------------------
// <copyright file="ODataPathFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;

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
