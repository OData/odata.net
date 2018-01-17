//---------------------------------------------------------------------
// <copyright file="ParseDynamicPathSegment.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Represents a delegate for parsing an unknown path segment or an open property segment
    /// </summary>
    /// <param name="previous">previous segment info.</param>
    /// <param name="identifier">name of the segment.</param>
    /// <param name="parenthesisExpression">The section of the segment inside parentheses, or null if there was none.</param>
    /// <returns>A collection of <see cref="ODataPathSegment"/> describing the given <paramref name="identifier"/> </returns>
    public delegate ICollection<ODataPathSegment> ParseDynamicPathSegment(ODataPathSegment previous, string identifier, string parenthesisExpression);
}