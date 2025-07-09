//---------------------------------------------------------------------
// <copyright file="TypeDefinitionDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.OData.Formatter.Deserialization;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.TypeDefinition;

public class CustomResourceReserialize(IODataDeserializerProvider reserializeProvider) 
    : ODataResourceDeserializer(reserializeProvider)
{
    /// <remarks>
    /// This method overrides the base implementation to handle custom deserialization logic for specific properties:
    /// - For the "LargeNumbers" property, it deserializes the ODataCollectionValue into a Collection of UInt64.
    /// - For the "Descriptions" property, it deserializes the ODataCollectionValue into a Collection of strings.
    /// - For other properties, it falls back to the base implementation.
    /// </remarks>
    public override void ApplyStructuralProperty(object resource, ODataProperty structuralProperty, IEdmStructuredTypeReference structuredType, ODataDeserializerContext readContext)
    {
        if (structuralProperty.Name == "LargeNumbers")
        {
            if (structuralProperty.Value is ODataCollectionValue odataCollectionValue)
            {
                var collection = new Collection<UInt64>();
                foreach (var item in odataCollectionValue.Items)
                {
                    collection.Add(Convert.ToUInt64(item));
                }
                resource.GetType().GetProperty(structuralProperty.Name)?.SetValue(resource, collection);
            }
        }
        else if (structuralProperty.Name == "Descriptions")
        {
            if (structuralProperty.Value is ODataCollectionValue odataCollectionValue)
            {
                var collection = new Collection<string>();
                foreach (var item in odataCollectionValue.Items)
                {
                    collection.Add(Convert.ToString(item));
                }
                resource.GetType().GetProperty(structuralProperty.Name)?.SetValue(resource, collection);
            }
        }
        else
        {
            base.ApplyStructuralProperty(resource, structuralProperty, structuredType, readContext);
        }
    }
}
