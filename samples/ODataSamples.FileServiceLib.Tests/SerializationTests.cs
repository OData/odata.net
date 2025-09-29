using Microsoft.OData;
using Microsoft.OData.Serializer;
using Microsoft.OData.UriParser;
using ODataSamples.FileServiceLib.Api;
using ODataSamples.FileServiceLib.SampleData;
using ODataSamples.FileServiceLib.Schema;
using ODataSamples.FileServiceLib.Serialization;
using ODataSamples.FileServiceLib.Serialization.OData;
using System.Text.Json;

namespace ODataSamples.FileServiceLib.Tests;

public class SerializationTests
{
    [Fact]
    public async Task SerializesFindFileResponseCollection()
    {
        // Arrange
        var serializerOptions = ODataSerializerOptionsFactory.Create();
        FindFileResponse data = DataGenerator.CreateMultiFileResponseData(count: 2);

        var model = EdmModelHelper.EdmModel;

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Users('userId')/Files", UriKind.Relative)
        ).ParseUri();

        var customState = new ODataCustomState
        {
            IdSerializer = new IdPropertySerializer("http://service/odata")
        };

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, serializerOptions, customState);

        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var rawOutput = reader.ReadToEnd();
        var normalizedOutput = JsonSerializer.Serialize(JsonDocument.Parse(rawOutput).RootElement);

        var expectedOutput = """
        {

        }
        """;
        var normalizedExpectedOutput = JsonSerializer.Serialize(JsonDocument.Parse(expectedOutput).RootElement);

        Assert.Equal(normalizedExpectedOutput, normalizedOutput);
    }
}