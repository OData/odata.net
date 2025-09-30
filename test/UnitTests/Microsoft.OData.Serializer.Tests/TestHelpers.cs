using Microsoft.OData.Edm;

namespace Microsoft.OData.Serializer.Tests;

internal static class TestHelpers
{
    public static EdmEnumType EnumType(this EdmModel model, string localName, string? namespaceName = null, EdmPrimitiveTypeKind underlyingType = EdmPrimitiveTypeKind.Int32, bool isFlags = false)
    {
        EdmEnumType enumType = new EdmEnumType(namespaceName, localName, underlyingType, isFlags);

        model.AddElement(enumType);
        return enumType;
    }
}
