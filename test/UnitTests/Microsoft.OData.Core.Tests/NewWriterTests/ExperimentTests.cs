using Microsoft.OData.Core.NewWriter;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Core.Tests.NewWriterTests;

public class ExperimentTests
{
    [Fact]
    public async Task WriteClrPayloadBasedOnIEdmModel()
    {
        var model = new EdmModel();

        var addressEntity = model.AddComplexType("ns", "Address");
        addressEntity.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
        addressEntity.AddStructuralProperty("Country", EdmPrimitiveTypeKind.String);

        var customerEntity = model.AddEntityType("ns", "Customer");
        var idProp = customerEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

        customerEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        customerEntity.AddStructuralProperty("EmailAddresses", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
        customerEntity.AddStructuralProperty("HomeAddress", new EdmComplexTypeReference(addressEntity, true));

        var customerEntitySet = model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Customers", customerEntity);


        var odataUri = new ODataUriParser(model, new Uri("Customers", UriKind.Relative)).ParseUri();

        List<Customer> data = [
            new Customer
            {
                Id = 1,
                Name = "John Doe",
                EmailAddresses = ["johndoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Nairobi",
                    Country = "Kenya"
                }
            },
            
            new()
            {
                Id = 2,
                Name = "Jane Doe",
                EmailAddresses = ["janedoe@mailer.com"],
                HomeAddress = new Address
                {
                    City = "Redmond",
                    Country = "United States"
                }
            }
        ];

        using var stream = new MemoryStream();
        Utf8JsonWriter writer = new(stream);
        var propertySelector = new ClrTypeEdmPropertySelector<Customer>();
        var propertyWriter = new ClrTypeEdmJsonPropertyWriter<Customer>();

        var resourceWriter = new ODataConventionalJsonResourceWriter<Customer>(
            writer,
            propertySelector,
            propertyWriter
            );

        var responseWriter = new ODataConventionalEntitySetJsonResponseWriter<Customer>(writer, resourceWriter);

        var context = new ODataWriterContext
        {
            Model = model,
            JsonWriter = writer
        };
        var state = new ODataWriterState
        {
            EdmType = customerEntitySet.Type,
            WriterContext = context
        };

        await responseWriter.WriteAsync(data, state);

        writer.Flush();
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var writtenPayload = await reader.ReadToEndAsync();

        var expectedPayload =
            "{\"@odata.context\":\"contextUrl\",\"value\":[{\"Id\":1,\"Name\":\"John Doe\",\"EmailAddresses\":[\"johndoe@mailer.com\"],\"HomeAddress\":{\"City\":\"Nairobi\",\"Country\":\"Kenya\"}},{\"Id\":2,\"Name\":\"Jane Doe\",\"EmailAddresses\":[\"janedoe@mailer.com\"],\"HomeAddress\":{\"City\":\"Redmond\",\"Country\":\"United States\"}}]}";
            

        Assert.Equal(expectedPayload, writtenPayload);
    }



    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<string> EmailAddresses { get; set; }

        public Address HomeAddress { get; set; }

    }

    class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
    }
}
