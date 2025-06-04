using Microsoft.OData.Core.NewWriter2;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.OData.Core.Tests.NewWriter2Tests;

public class NewWriter2ExperimentTests
{
    [Fact]
    public async Task TopLevelSimplePocoResourceSetResponse()
    {
        var model = new EdmModel();

        var entity = model.AddEntityType("ns", "Project");
        entity.AddKeys(entity.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        entity.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        entity.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean);

        model.AddEntityContainer("ns", "DefaultContainer")
            .AddEntitySet("Projects", entity);

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Projects", UriKind.Relative)
        ).ParseUri();

        IEnumerable<Project> projects = [
            new() { Id = 1, Name = "P1", IsActive = true },
            new() { Id = 2, Name = "P2", IsActive = false },
        ];

        using var output = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(output);

        var writerContext = new ODataJsonWriterContext
        {
            Model = model,
            ODataUri = odataUri,
            PayloadKind = ODataPayloadKind.ResourceSet,
            JsonWriter = new System.Text.Json.Utf8JsonWriter(output),
            ResourceWriterProvider = new ODataResourceWriterProvider(),
        };

        var writerStack = new ODataJsonWriterStack();

        var odataWriter = new ODataResourceSetEnumerableJsonWriter<Project>();
        await odataWriter.WriteAsync(projects, writerStack, writerContext);

        // TODO: should we guarantee flushing from within the writer?
        await jsonWriter.FlushAsync();

        output.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(output, Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        var expectedJson = @"{""@odata.context"":""http://service/odata/$metadata#Projects"",""value"":[{""Id"":1,""Name"":""P1"",""IsActive"":true},{""Id"":2,""Name"":""P2"",""IsActive"":false}]}";
        Assert.Equal(expectedJson, json);
    }

    class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
