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

namespace Microsoft.Data.OData.Query
{
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;

    /// <summary>
    /// Helper methods for the URI Parser.
    /// </summary>
    internal static class UriParserErrorHelper
    {
        /// <summary>
        /// Throws if the type is not related to the type of the given set.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="secondType">Second type, which should be related to the first type.</param>
        /// <param name="segmentName">The segment that is checking this.</param>
        public static void ThrowIfTypesUnrelated(IEdmType type, IEdmType secondType, string segmentName)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");
            ExceptionUtils.CheckArgumentNotNull(secondType, "secondType");
            IEdmType unwrappedType = type;
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType != null)
            {
                unwrappedType = collectionType.ElementType.Definition;
            }

            if (!unwrappedType.IsOrInheritsFrom(secondType) && !secondType.IsOrInheritsFrom(unwrappedType))
            {
                throw new ODataException(ODataErrorStrings.PathParser_TypeMustBeRelatedToSet(type, secondType, segmentName));
            }
        }
    }
}
