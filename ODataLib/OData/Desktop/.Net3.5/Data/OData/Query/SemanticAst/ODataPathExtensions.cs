//   OData .NET Libraries ver. 5.6.3
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
