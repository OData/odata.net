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
