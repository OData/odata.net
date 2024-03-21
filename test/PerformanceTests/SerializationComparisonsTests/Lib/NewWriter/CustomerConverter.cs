using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExperimentsLib
{
    internal class CustomerConverter : JsonConverter<Customer>
    {
        ODataSerializerContext _serializerContext;
        AddressConverter _addressConverter;
        AddressCollectionConverter _addressCollectionConverter;
        public CustomerConverter(JsonSerializerOptions options, ODataSerializerContext context)
        {
            this._serializerContext = context;
            _addressConverter = (AddressConverter)options.GetConverter(typeof(Address));
            _addressCollectionConverter = (AddressCollectionConverter)options.GetConverter(typeof(IEnumerable<Address>));
        }
        public override Customer Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Customer value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Id", value.Id);
            writer.WriteString("Name", value.Name);
            writer.WriteStartArray();
            foreach (var email in value.Emails)
            {
                writer.WriteStringValue(email);
            }
            writer.WriteEndArray();

            writer.WritePropertyName("HomeAddress");
            _addressConverter.Write(writer, value.HomeAddress, options);

            writer.WritePropertyName("Addressess");
            _addressCollectionConverter.Write(writer, value.Addresses, options);

            writer.WriteEndObject();
        }
    }

    internal class AddressConverter : JsonConverter<Address>
    {
        public override Address Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Address value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("City", value.City);
            writer.WriteString("Street", value.Street);
            writer.WriteEndObject();
        }
    }

    internal class AddressCollectionConverter : JsonConverter<IEnumerable<Address>>
    {
        private JsonConverter<Address> valueConverter;
        ODataSerializerContext context;
        public AddressCollectionConverter(JsonSerializerOptions options, ODataSerializerContext context)
        {
            valueConverter = (JsonConverter<Address>)options.GetConverter(typeof(Address));
            this.context = context;
        }

        public override IEnumerable<Address> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<Address> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var address in value)
            {
                valueConverter.Write(writer, address, options);
            }

            writer.WriteEndArray();
        }
    }

    internal class CustomerEntitySetConverter : JsonConverter<IEnumerable<Customer>>
    {
        private JsonConverter<Customer> valueConverter;
        ODataSerializerContext context;
        public CustomerEntitySetConverter(JsonSerializerOptions options, ODataSerializerContext context)
        {
            valueConverter = (JsonConverter<Customer>)options.GetConverter(typeof(Customer));
            this.context = context;
        }
        public override IEnumerable<Customer> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, IEnumerable<Customer> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Context", $"{context.Uri.ServiceRoot}$metadata#Customers");
            writer.WritePropertyName("values");
            writer.WriteStartArray();
            foreach (var customer in value)
            {
                valueConverter.Write(writer, customer, options);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    internal class ODataJsonConverterFactory : JsonConverterFactory
    {
        private ODataSerializerContext context;
        public ODataJsonConverterFactory(ODataSerializerContext context)
        {
            this.context = context;
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(Customer))
            {
                return true;
            }

            if (typeToConvert == typeof(Address))
            {
                return true;
            }

            if (typeof(IEnumerable<Customer>).IsAssignableFrom(typeToConvert))
            {
                return true;
            }

            if (typeof(IEnumerable<Address>).IsAssignableFrom(typeToConvert))
            {
                return true;
            }

            return false;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(Customer))
            {
                return new CustomerConverter(options, context);
            }

            if (typeToConvert == typeof(Address))
            {
                return new AddressConverter();
            }

            if (typeof(IEnumerable<Customer>).IsAssignableFrom(typeToConvert))
            {
                return new CustomerEntitySetConverter(options, this.context);
            }

            if (typeof(IEnumerable<Address>).IsAssignableFrom(typeToConvert))
            {
                return new AddressCollectionConverter(options, this.context);
            }

            throw new JsonException($"No converter supported for type {typeToConvert.Name}");
        }

    }

    internal class ODataSerializerContext
    {
        public IEdmModel Model { get; set; }
        public ODataUri Uri { get; set; }

    }
}