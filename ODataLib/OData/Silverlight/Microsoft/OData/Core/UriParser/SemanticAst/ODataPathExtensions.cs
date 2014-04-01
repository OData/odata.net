//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
    using System.Collections.Generic;
    using Microsoft.OData.Core.UriParser.Visitors;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;

    /// <summary>
    /// Extension methods for <see cref="ODataPath"/>. These method provide convenince functions.
    /// TODO: Implement this class and it's visitors. These are stubs.
    /// </summary>
    /// <remarks>
    /// The values that these methods compute are not cached.
    /// </remarks>
    internal static class ODataPathExtensions
    {
        /// <summary>
        /// Computes the <see cref="IEdmTypeReference"/> of the resource identified by this <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path">Path to compute the type for.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> of the resource, or null if the path does not identify a 
        /// resource with a type.</returns>
        public static IEdmTypeReference EdmType(this ODataPath path)
        {
            return path.LastSegment.EdmType.ToTypeReference();
        }

        /// <summary>
        /// Computes the <see cref="IEdmNavigationSource"/> of the resource identified by this <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path">Path to compute the set for.</param>
        /// <returns>The <see cref="IEdmNavigationSource"/> of the resource, or null if the path does not identify a 
        /// resource that is part of a set.</returns>
        public static IEdmNavigationSource NavigationSource(this ODataPath path)
        {
            return path.LastSegment.TranslateWith(new DetermineNavigationSourceTranslator());
        }

        /// <summary>
        /// Computes whether or not the resource identified by this <see cref="ODataPath"/> is a collection.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>True if the resource if a feed or collection of primitive or complex types. False otherwise.</returns>
        public static bool IsCollection(this ODataPath path)
        {
            return path.LastSegment.TranslateWith(new IsCollectionTranslator());
        }

        /// <summary>
        /// Build a segment representing a navigation property.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <param name="navigationProperty">The navigation property this segment represents.</param>
        /// <param name="navigationSource">The navigation source of the entities targetted by this navigation property. This can be null.</param>
        /// <returns>The ODataPath with navigation property appended in the end in the end</returns>
        public static ODataPath AppendNavigationPropertySegment(this ODataPath path, IEdmNavigationProperty navigationProperty, IEdmNavigationSource navigationSource)
        {
            var newPath = new ODataPath(path);
            NavigationPropertySegment np = new NavigationPropertySegment(navigationProperty, navigationSource);
            newPath.Add(np);
            return newPath;
        }

        /// <summary>
        /// Append the key segment in the end of ODataPath, the method does not modify current ODataPath instance,
        /// it returns a new ODataPath without ending type segment.
        /// If last segment is type cast, the key would be appended before type cast segment.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <param name="keys">The set of key property names and the values to be used in searching for the given item.</param>
        /// <param name="edmType">The type of the item this key returns.</param>
        /// <param name="navigationSource">The navigation source that this key is used to search.</param>
        /// <returns>The ODataPath with key segment appended</returns>
        public static ODataPath AppendKeySegment(this ODataPath path, IEnumerable<KeyValuePair<string, object>> keys, IEdmEntityType edmType, IEdmNavigationSource navigationSource)
        {
            var handler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
            path.WalkWith(handler);
            KeySegment keySegment = new KeySegment(keys, edmType, navigationSource);
            ODataPath newPath = handler.FirstPart;
            newPath.Add(keySegment);
            foreach (var segment in handler.LastPart)
            {
                newPath.Add(segment);
            }

            return newPath;
        }

        /// <summary>
        /// Remove the key segment in the end of ODataPath, the method does not modify current ODataPath instance,
        /// it returns a new ODataPath without ending type segment.
        /// If last segment is type cast, the key before type cast segment would be removed.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>The ODataPath without key segment removed</returns>
        public static ODataPath TrimEndingKeySegment(this ODataPath path)
        {
            var typeHandler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
            var keyHandler = new SplitEndingSegmentOfTypeHandler<KeySegment>();
            path.WalkWith(typeHandler);
            typeHandler.FirstPart.WalkWith(keyHandler);
            ODataPath newPath = keyHandler.FirstPart;
            foreach (var segment in typeHandler.LastPart)
            {
                newPath.Add(segment);
            }

            return newPath;
        }


        /// <summary>
        /// Remove the type-cast segment in the end of ODataPath, the method does not modify current ODataPath instance,
        /// it returns a new ODataPath without ending type segment.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>The ODataPath without type-cast in the end</returns>
        public static ODataPath TrimEndingTypeSegment(this ODataPath path)
        {
            var handler = new SplitEndingSegmentOfTypeHandler<TypeSegment>();
            path.WalkWith(handler);
            return handler.FirstPart;
        }

        /// <summary>
        /// Computes whether or not the ODataPath targets at an individual property.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>True if the the ODataPath targets at an individual property. False otherwise.</returns>
        public static bool IsIndividualProperty(this ODataPath path)
        {
            ODataPathSegment lastSegmentWithTypeCast = path.TrimEndingTypeSegment().LastSegment;
            return lastSegmentWithTypeCast is PropertySegment || lastSegmentWithTypeCast is OpenPropertySegment;
        }

        /// <summary>
        /// Get the string representation of <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>The string representation of the path.</returns>
        public static string ToResourcePathString(this ODataPath path)
        {
            return string.Concat(path.WalkWith(PathSegmentToResourcePathTranslator.DefaultInstance)).TrimStart('/');
        }

        /// <summary>
        /// Translate an ODataExpandPath into an ODataSelectPath
        /// Depending on the constructor of ODataSelectPath to validate that we aren't adding any
        /// segments that are illegal for a select.
        /// </summary>
        /// <param name="path">the ODataExpandPath to translate</param>
        /// <returns>A new ODataSelect path with the same segments as the expand path.</returns>
        public static ODataSelectPath ToSelectPath(this ODataExpandPath path)
        {
            return new ODataSelectPath(path);
        }

        /// <summary>
        /// Translate an ODataSelectPath into an ODataExpandPath
        /// Depending on the constructor of ODataExpandPath to validate that we aren't adding any
        /// segments that are illegal for an expand.
        /// </summary>
        /// <param name="path">the ODataSelectPath to translate</param>
        /// <returns>A new ODataExpand path with the same segments as the select path.</returns>
        public static ODataExpandPath ToExpandPath(this ODataSelectPath path)
        {
            return new ODataExpandPath(path);
        }
    }
}
