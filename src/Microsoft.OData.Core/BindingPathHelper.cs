//---------------------------------------------------------------------
// <copyright file="BindingPathHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace Microsoft.OData
{
    internal static class BindingPathHelper
    {
        /// <summary>
        /// Determine if the path of current navigation property is matching the binding path.
        /// The function used in FindNavigationTarget to resolve the navigation target for multi binding.
        /// </summary>
        /// <param name="bindingPath">The binding path.</param>
        /// <param name="parsedSegments">The list of segments in Uri path.</param>
        /// <returns>True if the path of navigation property in current scope is matching the <paramref name="bindingPath"/>.</returns>
        public static bool MatchBindingPath(IEdmPathExpression bindingPath, List<ODataPathSegment> parsedSegments)
        {
            List<string> paths = bindingPath.PathSegments.ToList();

            // If binding path only includes navigation property name, it matches.
            if (paths.Count == 1)
            {
                return true;
            }

            int pathIndex = paths.Count - 2; // Skip the last segment which is navigation property name.

            // Match from tail to head.
            for (int segmentIndex = parsedSegments.Count - 1; segmentIndex >= 0; segmentIndex--)
            {
                ODataPathSegment segment = parsedSegments[segmentIndex];

                // Containment navigation property or complex property in binding path.
                if (segment is PropertySegment || (segment is NavigationPropertySegment && segment.TargetEdmNavigationSource is IEdmContainedEntitySet))
                {
                    if (pathIndex < 0 || string.CompareOrdinal(paths[pathIndex], segment.Identifier) != 0)
                    {
                        return false;
                    }

                    pathIndex--;
                }
                else if (segment is TypeSegment)
                {
                    // May need match type if the binding path contains type cast.
                    if (pathIndex >= 0 && paths[pathIndex].Contains("."))
                    {
                        if (string.CompareOrdinal(paths[pathIndex], segment.EdmType.AsElementType().FullTypeName()) != 0)
                        {
                            return false;
                        }

                        pathIndex--;
                    }
                }
                else if (segment is EntitySetSegment
                      || segment is SingletonSegment
                      || segment is NavigationPropertySegment)
                {
                    // Containment navigation property in first if statement for NavigationPropertySegment.
                    break;
                }
            }

            // Return true if all the segments in binding path have been matched.
            return pathIndex == -1 ? true : false;
        }

        /// <summary>
        /// Match binding path with context Url for response payload.
        /// </summary>
        /// <param name="bindingPath">The binding path.</param>
        /// <param name="contextUrlSegments">The segments of context url of current response.</param>
        /// <returns>True if the context url segments matches the binding path.</returns>
        public static bool MatchBindingPath(IEdmPathExpression bindingPath, List<string> contextUrlSegments)
        {
            List<string> paths = bindingPath.PathSegments.ToList();

            int pathIndex = 0;

            // Try to match segments in context url with binding path discontinuously.
            // Since the segments of binding path may not appear continuously in context url.
            // For example: the binding path is "Address/Location/City", while context url is "http://host/People('abc')/Address(Road,Location/City)".
            // And the contextUrlSegments would be [People, 'abc', Address, Road, Location, City]
            foreach (var segment in contextUrlSegments)
            {
                if (string.CompareOrdinal(segment, paths[pathIndex]) == 0)
                {
                    if (pathIndex == paths.Count - 1)
                    {
                        return true;
                    }

                    pathIndex++;
                }
            }

            return false;
        }
    }
}
