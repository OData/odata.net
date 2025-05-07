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
        var customerEntity = model.AddEntityType("ns", "Customer");
        var idProp = customerEntity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
        customerEntity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);

        var customerEntitySet = model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Customers", customerEntity);


        var odataUri = new ODataUriParser(model, new Uri("Customers", UriKind.Relative)).ParseUri();

        List<Customer> data = [
            new Customer
            {
                Id = 1,
                Name = "John Doe"
            },
            new()
            {
                Id = 2,
                Name = "Jane Doe"
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

        var expectedPayload = @"{""@odata.context"":""contextUrl"",""value"":[{""Id"":1,""Name"":""John Doe""},{""Id"":2,""Name"":""Jane Doe""}]}";

        Assert.Equal(expectedPayload, writtenPayload);
    }

    class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
