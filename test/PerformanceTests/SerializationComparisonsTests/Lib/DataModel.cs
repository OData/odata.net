//---------------------------------------------------------------------
// <copyright file="DataModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Buffers;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OData.Edm;

namespace ExperimentsLib
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Emails { get; set; }
        public string Bio { get; set; }

        [JsonConverter(typeof(Base64Converter))]
        public byte[] Content { get; set; }

        public Address HomeAddress { get; set; }
        public List<Address> Addresses { get; set; }
    }

    public class Address
    {
        public string City { get; set; }
        public object Misc { get; set; }
        public string Street { get; set; }
    }

    public static class DataModel
    {
        /// <summary>
        /// Gets an OData model for testing, defining
        /// and entity set of Customer instances.
        /// </summary>
        /// <returns>The OData model.</returns>
        public static IEdmModel GetEdmModel()
        {
            EdmModel model = new EdmModel();

            EdmComplexType addressType = new EdmComplexType("NS", "Address");
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            addressType.AddStructuralProperty("Misc", EdmUntypedStructuredTypeReference.NullableTypeReference);
            model.AddElement(addressType);

            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            customer.AddStructuralProperty("Emails",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: true))));
            customer.AddStructuralProperty("Bio", EdmPrimitiveTypeKind.String);
            customer.AddStructuralProperty("Content", EdmPrimitiveTypeKind.Binary);
            customer.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressType, isNullable: false));
            customer.AddStructuralProperty("Addresses",
                new EdmCollectionTypeReference(
                    new EdmCollectionType(new EdmComplexTypeReference(addressType, isNullable: false))));
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "DefaultContainer");
            container.AddEntitySet("Customers", customer);
            model.AddElement(container);

            return model;
        }
    }

    class Base64Converter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            byte[] encodedArray = null;
            int encodedLength = Base64.GetMaxEncodedToUtf8Length(value.Length);
            Span<byte> encoded = encodedLength <= 256 ?
                stackalloc byte[encodedLength] :
                encodedArray = ArrayPool<byte>.Shared.Rent(encodedLength);

            Base64.EncodeToUtf8(value, encoded, out int consumed, out int written);
            writer.WriteStringValue(encoded.Slice(0, written));

            if (encodedArray != null)
            {
                ArrayPool<byte>.Shared.Return(encodedArray);
            }
        }
    }
}
