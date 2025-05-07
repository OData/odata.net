//---------------------------------------------------------------------
// <copyright file="TypeDefinitionSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.TypeDefinition;

public class CustomResourceSerializer(IODataSerializerProvider serializerProvider) : ODataResourceSerializer(serializerProvider)
{
    private const string Namespace = "Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.";

    /// <remarks>
    /// This method overrides the base implementation to handle specific type definitions and custom serialization logic:
    /// - For properties named "Height" and "Temperature" that are type definitions, it converts their values to strings.
    /// - For properties named "LargeNumbers", it creates an OData collection value.
    /// - For other type definitions, it maps specific type definitions to their underlying types and converts the values accordingly.
    /// - If none of the above conditions are met, it falls back to the base implementation.
    /// </remarks>
    public override ODataProperty CreateStructuralProperty(IEdmStructuralProperty structuralProperty, ResourceContext resourceContext)
    {
        if (structuralProperty.Name == "Height" && structuralProperty.Type.IsTypeDefinition())
        {
            var propertyValue = resourceContext.GetPropertyValue(structuralProperty.Name);
            var typeDefinitionValue = propertyValue as Height;
            return new ODataProperty()
            {
                Name = structuralProperty.Name,
                Value = new ODataPrimitiveValue(typeDefinitionValue?.ToString())
            };
        }

        if (structuralProperty.Name == "Temperature" && structuralProperty.Type.IsTypeDefinition())
        {
            var propertyValue = resourceContext.GetPropertyValue(structuralProperty.Name);
            var typeDefinitionValue = propertyValue as Temperature;
            return new ODataProperty()
            {
                Name = structuralProperty.Name,
                Value = new ODataPrimitiveValue(typeDefinitionValue?.ToString())
            };
        }

        if (structuralProperty.Name == "LargeNumbers")
        {
            var propertyValue = resourceContext.GetPropertyValue(structuralProperty.Name);

            IEnumerable<object>? collectionItems = propertyValue switch
            {
                IEnumerable<UInt64> ulongCollection => ulongCollection.Cast<object>(),
                IEnumerable<object> objects => objects,
                _ => null
            };

            if (collectionItems != null)
            {
                return new ODataProperty()
                {
                    Name = structuralProperty.Name,
                    Value = new ODataCollectionValue
                    {
                        TypeName = "Collection(" + Namespace + "UInt64)",
                        Items = collectionItems
                    }
                };
            }
        }

        if (structuralProperty.Type.IsTypeDefinition())
        {
            var propertyValue = resourceContext.GetPropertyValue(structuralProperty.Name);

            // Check if the type is a type definition and if it has a corresponding underlying type
            var odataPropertyValue = new ODataPayloadValueConverter().ConvertToPayloadValue(propertyValue, structuralProperty.Type);

            return new ODataProperty()
            {
                Name = structuralProperty.Name,
                Value = odataPropertyValue
            };
        }

        return base.CreateStructuralProperty(structuralProperty, resourceContext);
    }
}
