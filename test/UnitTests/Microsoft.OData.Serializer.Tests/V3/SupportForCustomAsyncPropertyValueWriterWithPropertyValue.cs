using Microsoft.OData.Edm;
using Microsoft.OData.Serializer.V3;
using Microsoft.OData.Serializer.V3.Adapters;
using Microsoft.OData.Serializer.V3.Attributes;
using Microsoft.OData.Serializer.V3.Json;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microsoft.OData.Serializer.Tests.V3;

public class SupportForCustomAsyncPropertyValueWriterWithPropertyValue
{
    [Fact]
    public async Task CanSerializePocoWithCustomAsyncPropertyValueWriter()
    {
        List<FileAttachment> customers = [
            new() { Id = 1, FileName = "file1.txt", Content = "This is the content of file 1", HasContentStream = false },

            // content  will be provided from the stream
            new() { Id = 2, FileName = "file2.txt", HasContentStream = true },
        ];

        var options = new ODataSerializerOptions();
        var output = new MemoryStream();

        var model = GetEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("FileAttachments", UriKind.Relative)
        ).ParseUri();

        await ODataSerializer.WriteAsync(customers, output, odataUri, model, options);

        output.Position = 0;

        var actual = new StreamReader(output).ReadToEnd();
        var actualNormalized = JsonSerializer.Serialize(JsonDocument.Parse(actual));
        var expected =
            """
            {
              "@odata.context": "http://service/odata/$metadata#FileAttachments",
              "value": [
                {
                  "Id": 1,
                  "FileName": "file1.txt",
                  "Content": "This is the content of file 1"
                },
                {
                  "Id": 2,
                  "FileName": "file2.txt",
                  "Content": "This is the content of file 2"
                }
              ]
            }
            """;


        var expectedNormalized = JsonSerializer.Serialize(JsonDocument.Parse(expected));
        Assert.Equal(expectedNormalized, actualNormalized);
    }

    private static IEdmModel GetEdmModel()
    {
        var model = new EdmModel();
        var fileAttachmentType = new EdmEntityType("NS", "FileAttachment");
        fileAttachmentType.AddKeys(fileAttachmentType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, isNullable: false));
        fileAttachmentType.AddStructuralProperty("FileName", EdmPrimitiveTypeKind.String, isNullable: false);
        fileAttachmentType.AddStructuralProperty("Content", EdmPrimitiveTypeKind.String, isNullable: true);

        model.AddElement(fileAttachmentType);

        var entityContainer = new EdmEntityContainer("NS", "DefaultContainer");
        model.AddElement(entityContainer);

        var fileAttachments = entityContainer.AddEntitySet("FileAttachments", fileAttachmentType);
        return model;
    }

    class FileAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string? Content { get; set; }

        [ODataValueWriter(typeof(StreamContentWriter))]
        public bool HasContentStream { get; set; }
    }

    class StreamContentWriter : ODataAsyncPropertyWriter<FileAttachment, string, CustomState>
    {
        public override async ValueTask WriteValueAsync(
            FileAttachment resource,
            string propertyValue,
            IStreamValueWriter<CustomState> writer,
            ODataWriterState<CustomState> state)
        {
            if (resource.HasContentStream)
            {
                await using var stream = state.CustomState.StreamProvider.GetContentStream(resource.Id);

                // TODO: this is inefficient, we ideally want to decode the bytes in chunks
                // and write to writer.WriteStringSegment
                // But ideally, we should provide a convenience API for writing string segment from
                // a byte array and handle the decoding internally.
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                await writer.WriteValueAsync(content, state);
            }

            writer.WriteValue(propertyValue, state);
        }
    }

    class ContentStreamProvider
    {
        public Stream GetContentStream(int fileId)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes($"This is the content of file {fileId}"));
        }
    }

    record struct CustomState(ContentStreamProvider StreamProvider);
}
