//   OData .NET Libraries ver. 6.8.1
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
