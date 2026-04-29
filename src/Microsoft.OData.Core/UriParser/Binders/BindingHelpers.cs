//---------------------------------------------------------------------
// <copyright file="BindingHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    internal static class BindingHelpers
    {
        public static void SetExpectedType(ResourceLiteralToken token, IEdmTypeReference expectedType)
        {
            if (token == null || expectedType == null)
            {
                return;
            }

            // If the expected type is untyped, it's valid to have a resource literal. We will not set the expected type in this case, and let the binding phase handle it.
            if (expectedType.IsUntyped())
            {
                // Leave the following code here for reference:
                // ExpectedType = EdmUntypedStructuredTypeReference.NullableTypeReference;
                token.ExpectedType = null;
                return;
            }

            if (expectedType.IsStructured())
            {
                token.ExpectedType = expectedType.AsStructured();
            }
            else
            {
                throw new ODataException(Error.Format(SRResources.ResourceLiteralToken_ExpectedStructuredType, expectedType.FullName(), expectedType.TypeKind()));
            }
        }

        public static void SetExpectedType(CollectionLiteralToken token, IEdmTypeReference expectedType)
        {
            if (token == null || expectedType == null)
            {
                return;
            }

            // If the expected type is untyped, it's valid to have a collection literal. We will not set the expected collection type in this case, and let the binding phase handle it.
            if (expectedType.IsUntyped())
            {
                // Leave the following code here for reference:
                // ExpectedCollectionType = new EdmCollectionTypeReference(new EdmCollectionType(EdmUntypedStructuredTypeReference.NullableTypeReference));
                token.ExpectedCollectionType = null;
                return;
            }

            if (expectedType.IsCollection())
            {
                token.ExpectedCollectionType = expectedType.AsCollection();
            }
            else
            {
                throw new ODataException(Error.Format(SRResources.CollectionLiteralToken_ExpectedCollectionType, expectedType.FullName(), expectedType.TypeKind()));
            }
        }
    }
}