//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM collection type.
    /// </summary>
    public class EdmCollectionTypeReference : EdmTypeReference, IEdmCollectionTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionTypeReference"/> class.
        /// </summary>
        /// <param name="collectionType">The type definition this reference refers to.</param>
        public EdmCollectionTypeReference(IEdmCollectionType collectionType)
            : base(collectionType, GetIsNullable(collectionType))
        {
        }

        private static bool GetIsNullable(IEdmCollectionType collectionType)
        {
            // check if the member type is entity, if yes, pass in default value (true),
            //   as per spec: A navigation property whose Type attribute specifies a collection MUST NOT
            //   specify a value for the Nullable attribute as the collection always exists, it may just be empty.
            // else replace with the nullable in member type reference
            IEdmTypeReference elementType = collectionType.ElementType;
            if (elementType == null)
            {
                return true;
            }

            IEdmEntityTypeReference entityReference = elementType as IEdmEntityTypeReference;
            return entityReference != null || collectionType.ElementType.IsNullable;
        }
    }
}
