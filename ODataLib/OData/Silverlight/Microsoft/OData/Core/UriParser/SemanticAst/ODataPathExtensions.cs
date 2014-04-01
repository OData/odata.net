//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Semantic
{
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
        /// Computes the <see cref="IEdmEntitySet"/> of the resource identified by this <see cref="ODataPath"/>.
        /// </summary>
        /// <param name="path">Path to compute the set for.</param>
        /// <returns>The <see cref="IEdmEntitySet"/> of the resource, or null if the path does not identify a 
        /// resource that is part of a set.</returns>
        public static IEdmEntitySet EntitySet(this ODataPath path)
        {
            return path.LastSegment.Translate(new DetermineEntitySetTranslator());
        }

        /// <summary>
        /// Computes whether or not the resource identified by this <see cref="ODataPath"/> is a collection.
        /// </summary>
        /// <param name="path">Path to perform the computation on.</param>
        /// <returns>True if the resource if a feed or collection of primitive or complex types. False otherwise.</returns>
        public static bool IsCollection(this ODataPath path)
        {
            return path.LastSegment.Translate(new IsCollectionTranslator());
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
