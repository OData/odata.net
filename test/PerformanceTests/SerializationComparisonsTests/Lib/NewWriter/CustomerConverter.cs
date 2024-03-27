using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ExperimentsLib.GenericEntityTypeConverter;

namespace ExperimentsLib
{
    // conceptually, the converter could be auto-generated using source generation
    // for user-defined CLR types, or the user could provider a custom "ODataJsonConverter<T">
    // that has access to the serializer context
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
            var selectExpandClause = _serializerContext.Uri.SelectAndExpand;
            IEdmStructuredType structuredType = _serializerContext.Model.FindType("NS.Customer") as IEdmStructuredType;
            

            // fast path when all fields are selected
            if (selectExpandClause == null || selectExpandClause.AllSelected)
            {
                writer.WriteNumber("Id", value.Id);
                writer.WriteString("Name", value.Name);

                writer.WritePropertyName("Emails");
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
            }
            else
            {
                var selectExpandNode = new SelectExpandNode(structuredType, _serializerContext);
                if (selectExpandNode.SelectedStructuralProperties != null)
                {
                    
                    foreach (var property in selectExpandNode.SelectedStructuralProperties)
                    {
                        if (property.Name == "Id")
                        {
                            writer.WriteNumber("Id", value.Id);
                        }

                        else if (property.Name == "Name")
                        {
                            writer.WriteString("Name", value.Name);
                        }

                        else if (property.Name == "Emails")
                        {
                            writer.WritePropertyName("Emails");
                            writer.WriteStartArray();
                            foreach (var email in value.Emails)
                            {
                                writer.WriteStringValue(email);
                            }
                            writer.WriteEndArray();
                        }
                    }
                }
                if (selectExpandNode.SelectedComplexProperties != null)
                {
                    foreach (KeyValuePair<IEdmStructuralProperty, PathSelectItem> complex in selectExpandNode.SelectedComplexProperties)
                    {
                        if (complex.Key.Name == "HomeAddress")
                        {
                            writer.WritePropertyName("HomeAddress");
                            _addressConverter.Write(writer, value.HomeAddress, options);
                        }

                        else if (complex.Key.Name == "Addresses")
                        {
                            writer.WritePropertyName("Addressess");
                            _addressCollectionConverter.Write(writer, value.Addresses, options);
                        }
                    }
                }
            }

            writer.WriteEndObject();
        }
    }

    internal class GenericEntityTypeConverter : JsonConverter<object>
    {
        Type typeToConvert;
        ODataSerializerContext context;
        public GenericEntityTypeConverter(Type type, JsonSerializerOptions options, ODataSerializerContext context)
        {
            this.typeToConvert = type;
            this.context = context;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var selectExpandClause = context.Uri.SelectAndExpand;
            IEdmStructuredType structuredType = context.Model.FindType("NS.Customer") as IEdmStructuredType;

            // fast path when all fields are selected
            if (selectExpandClause == null || selectExpandClause.AllSelected)
            {
                //writer.WriteNumber("Id", value.Id);
                //writer.WriteString("Name", value.Name);

                //writer.WritePropertyName("Emails");
                //writer.WriteStartArray();
                //foreach (var email in value.Emails)
                //{
                //    writer.WriteStringValue(email);
                //}
                //writer.WriteEndArray();

                //writer.WritePropertyName("HomeAddress");
                //_addressConverter.Write(writer, value.HomeAddress, options);

                //writer.WritePropertyName("Addressess");
                //_addressCollectionConverter.Write(writer, value.Addresses, options);
            }
            else
            {
                var selectExpandNode = new SelectExpandNode(structuredType, context);
                if (selectExpandNode.SelectedStructuralProperties != null)
                {
                    foreach (var property in selectExpandNode.SelectedStructuralProperties)
                    {
                        // using reflection for simplicity, should use cache getter delegate instead
                        // use optimized, generated writer to avoid boxing
                        var reflProperty = this.typeToConvert.GetProperty(property.Name);

                        object propertyValue = reflProperty.GetValue(value);
                        var clrPropertyType = reflProperty.PropertyType;
                        //var propertyConverter = options.GetConverter(clrPropertyType);
                        
                        if (clrPropertyType == typeof(int))
                        {
                            writer.WriteNumber(property.Name, (int)propertyValue);
                        }
                        else if (clrPropertyType == typeof(string))
                        {
                            writer.WriteString(property.Name, (string)propertyValue);
                        }

                        //else if (property.Name == "Emails")
                        //{
                        //    writer.WritePropertyName("Emails");
                        //    writer.WriteStartArray();
                        //    foreach (var email in value.Emails)
                        //    {
                        //        writer.WriteStringValue(email);
                        //    }
                        //    writer.WriteEndArray();
                        //}
                    }
                }
            }

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

    internal class GenericEntitySetConverter : JsonConverter<object>
    {
        private GenericEntityTypeConverter valueConverter;
        ODataSerializerContext context;
        public GenericEntitySetConverter(JsonSerializerOptions options, ODataSerializerContext context, Type elementType)
        {
            valueConverter = new GenericEntityTypeConverter(elementType, options, context);
            this.context = context;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            //throw new NotImplementedException();
            writer.WriteStartObject();
            writer.WriteString("Context", $"{context.Uri.ServiceRoot}$metadata#Customers");
            writer.WritePropertyName("values");
            writer.WriteStartArray();
            foreach (var item in value as IEnumerable)
            {
                valueConverter.Write(writer, item, options);
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

            return true;
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

            var genericEnumerbale = typeof(IEnumerable<>);
            if (TryGetEnumerableElementType(typeToConvert, out Type elementType))
            {
                return new GenericEntitySetConverter(options, this.context, elementType);
            }


            //return base.CreateConverter(typeToConvert, options);
            throw new JsonException($"No converter supported for type {typeToConvert.Name}");
        }

        private static bool TryGetEnumerableElementType(Type type, out Type elementType)
        {
            elementType = null;
            //var iEnumerableInterface = type.GetInterfaces()
            //        .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

    }

    internal class ODataSerializerContext
    {
        public IEdmModel Model { get; set; }
        public ODataUri Uri { get; set; }

        public SelectExpandClause SelectExpandClause => Uri.SelectAndExpand;

        public bool ExpandReference { get; set; }

        public ISet<string> ComputedProperties { get; set; }

    }
}