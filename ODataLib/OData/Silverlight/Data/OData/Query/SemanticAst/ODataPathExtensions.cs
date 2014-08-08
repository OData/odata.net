//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Query.SemanticAst
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;

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
    }
}
