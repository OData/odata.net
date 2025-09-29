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
            new Uri("Users('id')/Files", UriKind.Relative)
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
        var normalizedOutput = JsonSerializer.Serialize(JsonDocument.Parse(rawOutput).RootElement);

        var expectedOutput = """
        {
          "@odata.context": "http://service/odata/$metadata#Users('id')/Files",
          "value": [
            {
              "@odata.id": "http://service/odata/Users('id')/Files('file-1')",
              "@odata.etag": "W/\"1\"",
              "Id": "file-1",
              "FileName": "MarketingReport01.pdf",
              "FileExtension": ".pdf",
              "FileSize": 10240,
              "Version": 1,
              "IsProtected": true,
              "Description": "Sample marketing document for testing and demonstration purposes - Item 1",
              "ExternalId": "00000000-0000-0000-0000-000000000001",
              "CreatedAt": "2024-01-01T09:00:00Z",
              "Tags": [
                "marketing",
                "sample",
                "priority-1",
                "pdf"
              ],
              "ActivityStats": [
                {
                  "Id": "activity-1-1",
                  "Actor": "user1@example.com",
                  "ActivityDateTime": "2024-01-01T09:00:00Z"
                },
                {
                  "Id": "activity-1-2",
                  "Actor": "editor1@example.com",
                  "ActivityDateTime": "2024-01-01T09:00:00Z"
                }
              ],
              "BinaryData": "U2FtcGxlIGNvbnRlbnQgZm9yIGZpbGUgMSAtIE1hcmtldGluZyBkZXBhcnRtZW50",
              "ByteCollection": "QWRkaXRpb25hbCBiaW5hcnkgZGF0YSAx",
              "FileContent": {
                "Text": "File content for Marketing report 1",
                "Annotation": "Generated sample data - Item 1"
              },
              "EntityACL": {
                "AccessControlEntries": [
                  {
                    "AccessRight": "Read",
                    "Claim": {
                      "Type": "user",
                      "Value": "user1@example.com"
                    }
                  },
                  {
                    "AccessRight": "Read",
                    "Claim": {
                      "Type": "group",
                      "Value": "marketing-group"
                    }
                  }
                ]
              },
              "AllExtensionsNames": [
                "CustomMetadata",
                "ProcessingInfo",
                "MarketingExtension"
              ],
              "AllExtensions": {
                "CustomMetadata": {
                  "author": "user1@example.com",
                  "category": "Marketing",
                  "priority": 1
                },
                "ProcessingInfo": {
                  "processed": true,
                  "processedDate": "2024-01-01T11:00:00Z",
                  "processingEngine": "SampleEngine v1.0"
                }
              },
              "ItemProperties": {
                "customField1@is.queryable": true,
                "customField1": "value-1",
                "customField2": 100,
                "customField3": true,
                "department": "Marketing",
                "fileAge": 1,
                "lastModifiedBy": "editor1@example.com",
                "links": [
                  {},
                  {},
                  {}
                ]
              }
            },
            {
              "@odata.id": "http://service/odata/Users('id')/Files('file-2')",
              "@odata.etag": "W/\"2\"",
              "Id": "file-2",
              "FileName": "EngineeringReport02.docx",
              "FileExtension": ".docx",
              "FileSize": 20480,
              "Version": 2,
              "IsProtected": false,
              "Description": "Sample engineering document for testing and demonstration purposes - Item 2",
              "ExternalId": "00000000-0000-0000-0000-000000000002",
              "CreatedAt": "2024-01-02T10:00:00Z",
              "Tags": [
                "engineering",
                "sample",
                "priority-2",
                "docx"
              ],
              "ActivityStats": [
                {
                  "Id": "activity-2-1",
                  "Actor": "user2@example.com",
                  "ActivityDateTime": "2024-01-02T10:00:00Z"
                },
                {
                  "Id": "activity-2-2",
                  "Actor": "editor2@example.com",
                  "ActivityDateTime": "2024-01-03T10:00:00Z"
                }
              ],
              "BinaryData": "U2FtcGxlIGNvbnRlbnQgZm9yIGZpbGUgMiAtIEVuZ2luZWVyaW5nIGRlcGFydG1lbnQ=",
              "ByteCollection": "QWRkaXRpb25hbCBiaW5hcnkgZGF0YSAy",
              "FileContent": {
                "Text": "File content for Engineering report 2",
                "Annotation": "Generated sample data - Item 2"
              },
              "EntityACL": {
                "AccessControlEntries": [
                  {
                    "AccessRight": "Write",
                    "Claim": {
                      "Type": "user",
                      "Value": "user2@example.com"
                    }
                  },
                  {
                    "AccessRight": "Read",
                    "Claim": {
                      "Type": "group",
                      "Value": "engineering-group"
                    }
                  }
                ]
              },
              "AllExtensionsNames": [
                "CustomMetadata",
                "ProcessingInfo",
                "EngineeringExtension"
              ],
              "AllExtensions": {
                "CustomMetadata": {
                  "author": "user2@example.com",
                  "category": "Engineering",
                  "priority": 2
                },
                "ProcessingInfo": {
                  "processed": false,
                  "processedDate": "2024-01-02T11:00:00Z",
                  "processingEngine": "SampleEngine v1.0"
                }
              },
              "ItemProperties": {
                "customField1@is.queryable": true,
                "customField1": "value-2",
                "customField2": 200,
                "customField3": false,
                "department": "Engineering",
                "fileAge": 2,
                "lastModifiedBy": "editor2@example.com",
                "links": [
                  {},
                  {},
                  {}
                ]
              }
            }
          ],
          "@odata.nextLink": "http://service/odata/Users/id/Files?$skiptoken=skip-token-2"
        }
        """;
        var normalizedExpectedOutput = JsonSerializer.Serialize(JsonDocument.Parse(expectedOutput).RootElement);

        Assert.Equal(normalizedExpectedOutput, normalizedOutput);
    }
}