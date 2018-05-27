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

                // Cache the cast result to avoid CA1800:DoNotCastUnnecessarily.
                bool segmentIsNavigationPropertySegment = segment is NavigationPropertySegment;

                // Containment navigation property or complex property in binding path.
                if (segment is PropertySegment || (segmentIsNavigationPropertySegment && segment.TargetEdmNavigationSource is IEdmContainedEntitySet))
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
                      || segmentIsNavigationPropertySegment)
                {
                    // Containment navigation property in first if statement for NavigationPropertySegment.
                    break;
                }
            }

            // Return true if all the segments in binding path have been matched.
            return pathIndex == -1 ? true : false;
        }
    }
}
