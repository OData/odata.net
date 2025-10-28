using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests;

public class NullableEnumTests
{
    [Fact]
    public async Task WritesStringValueWhenNullableEnumIsNotNull()
    {
        var data = new TaskItem { Id = 1, Status = Status.Completed };

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Tasks(1)", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        var stream = new MemoryStream();

        await ODataSerializer.WriteAsync(data, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Tasks/$entity",
              "Id": 1,
              "Status": "Completed"
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);
    }

    [Fact]
    public async Task WritesNullWhenNullableEnumIsNull()
    {
        var data = new TaskItem { Id = 1, Status = null };

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Tasks(1)", UriKind.Relative)
        ).ParseUri();

        var options = new ODataSerializerOptions();
        var stream = new MemoryStream();

        await ODataSerializer.WriteAsync(data, stream, odataUri, model, options);

        stream.Position = 0;
        var actual = new StreamReader(stream).ReadToEnd();
        var normalizedActual = JsonSerializer.Serialize(JsonDocument.Parse(actual));

        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#Tasks/$entity",
              "Id": 1,
              "Status": null
            }
            """;
        var normalizedExpected = JsonSerializer.Serialize(JsonDocument.Parse(expected));

        Assert.Equal(normalizedExpected, normalizedActual);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();

        var statusEnum = new EdmEnumType("ns", "Status");
        statusEnum.AddMember(new EdmEnumMember(statusEnum, "Active", new EdmEnumMemberValue(0)));
        statusEnum.AddMember(new EdmEnumMember(statusEnum, "Completed", new EdmEnumMemberValue(1)));
        statusEnum.AddMember(new EdmEnumMember(statusEnum, "Failed", new EdmEnumMemberValue(2)));

        var taskType = new EdmEntityType("ns", "Task");
        taskType.AddKeys(taskType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        taskType.AddStructuralProperty("Status", new EdmEnumTypeReference(statusEnum, isNullable: true));
        model.AddElement(taskType);
        
        model.AddEntityContainer("ns", "Container").AddEntitySet("Tasks", taskType);

        return model;
    }

    [ODataType("ns.Task")]
    class TaskItem
    {
        public int Id { get; set; }
        public Status? Status { get; set; }
    }
    enum Status
    {
        Active,
        Completed,
        Failed
    }
}
