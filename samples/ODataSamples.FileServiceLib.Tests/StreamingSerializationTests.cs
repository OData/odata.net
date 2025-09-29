using Microsoft.OData.Serializer;
using Microsoft.OData.UriParser;
using ODataSamples.FileServiceLib.Api;
using ODataSamples.FileServiceLib.SampleData;
using ODataSamples.FileServiceLib.Schema;
using ODataSamples.FileServiceLib.Serialization;
using ODataSamples.FileServiceLib.Serialization.OData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ODataSamples.FileServiceLib.Tests;

public class StreamingSerializationTests
{
    [Fact]
    public async Task SerializesStreamEnableFieldFromStreamSource()
    {
        // Arrange
        var serializerOptions = ODataSerializerOptionsFactory.Create();
        FindFileResponse collection = DataGenerator.CreateMultiFileResponseData(count: 1, new DataGenerationOptions
        {
            StreamFileContentText = true,
            StreamFileContentAnnotation = true
        });

        var data = collection.First();

        var model = EdmModelHelper.EdmModel;

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Users('id')/Files('file-1')", UriKind.Relative)
        ).ParseUri();

        var customState = new ODataCustomState
        {
            IdSerializer = new IdPropertySerializer("http://service/odata/Users('id')")
        };

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, serializerOptions, customState);

        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var rawOutput = reader.ReadToEnd();
        var parsedJson = JsonDocument.Parse(rawOutput).RootElement;

        var fileContentText = parsedJson.GetProperty("FileContent").GetProperty("Text").GetString();
        Assert.Contains("File content for Marketing report 1", fileContentText);

        var fileContentAnnotation = parsedJson.GetProperty("FileContent").GetProperty("Annotation").GetString();
        Assert.Contains("Generated sample data - Item 1", fileContentAnnotation);
    }

    [Fact]
    public async Task SerializesStreamEnableFieldFromStreamSourceWithLargeFields()
    {
        // Arrange
        var serializerOptions = ODataSerializerOptionsFactory.Create();
        FindFileResponse collection = DataGenerator.CreateMultiFileResponseData(count: 1, new DataGenerationOptions
        {
            StreamFileContentText = true,
            LargeTextPayload = true
        });

        var data = collection.First();

        var model = EdmModelHelper.EdmModel;

        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Users('id')/Files('file-1')", UriKind.Relative)
        ).ParseUri();

        var customState = new ODataCustomState
        {
            IdSerializer = new IdPropertySerializer("http://service/odata/Users('id')")
        };

        var stream = new MemoryStream();

        // Act
        await ODataSerializer.WriteAsync(data, stream, odataUri, model, serializerOptions, customState);

        // Assert
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var rawOutput = reader.ReadToEnd();
        var parsedJson = JsonDocument.Parse(rawOutput).RootElement;

        var fileContentText = parsedJson.GetProperty("FileContent").GetProperty("Text").GetString();
        var baseText = "File content for Marketing report 1";
        Assert.Contains(RepeatString(baseText, DataGenerator.LargeFieldMultiplier), fileContentText);

        var fileContentAnnotation = parsedJson.GetProperty("FileContent").GetProperty("Annotation").GetString();
        var baseAnnotation = "Generated sample data - Item 1";
        Assert.Contains(RepeatString(baseAnnotation, DataGenerator.LargeFieldMultiplier), fileContentAnnotation);
    }

    private static string RepeatString(string str, int count)
    {
        return string.Concat(Enumerable.Repeat(str, count));
    }
}
