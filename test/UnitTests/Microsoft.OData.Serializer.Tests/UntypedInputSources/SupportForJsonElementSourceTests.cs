using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.OData.Serializer.Tests.UntypedInputSources;

public class SupportForJsonElementSourceTests
{
    [Fact]
    public async Task CanWritePayloadFromJsonElementSource()
    {
        // Arrange
        var rawInput = """
            [
                {
                    "Id": 1,
                    "Name": "Item 1",
                    "Details": {
                        "Description": "This is item 1",
                        "Price": 9.99
                    },
                    "Tags": ["Tag1", "Tag2"],
                    "Comments": [
                        {
                            "Id": 101,
                            "User": "UserA",
                            "Message": "Great item!"
                        },
                        {
                            "Id": 102,
                            "User": "UserB",
                            "Message": "Good value for money."
                        }
                    ]
                },
                {    "Id": 2,
                    "Name": "Item 2",
                    "Details": {
                        "Description": "This is item 2",
                        "Price": 19.99
                    },
                    "Tags": ["Tag3", "Tag4"],
                    "Comments": [
                        {
                            "Id": 201,
                            "User": "UserC",
                            "Message": "Not what I expected."
                        }
                    ]
                },
                {   
                    "Id": 3,
                    "Name": "Item 3",
                    "Details": {
                        "Description": "This is item 3",
                        "Price": 29.99
                    },
                    "Tags": [],
                    "Comments": []
                }
            ]
            """;

        var input = JsonDocument.Parse(rawInput).RootElement;

        var options = new ODataSerializerOptions<WriterState>();
        options.AddTypeInfo<JsonElement>(new()
        {
            GetEdmTypeName = (element, state) => state.CustomState.EdmStructuredType?.FullTypeName()!, // what about subtype
            GetValueKind = (element, state) => element.ValueKind switch
            {
                JsonValueKind.Array => ODataValueKind.Collection,
                JsonValueKind.Object => ODataValueKind.Resource,
                _ => ODataValueKind.Unknown
            },
            ElementSelector = new ODataElementEnumeratorSelector<JsonElement, JsonElement.ArrayEnumerator, JsonElement, WriterState>
            {
                GetElementsEnumerator = (resource, state) => resource.EnumerateArray(),
                WriteElement = (resource, value, writer, state) =>
                {
                    // If you need to pass item-level state, then we could wrap that info in the element type, i.e.
                    // (JsonElement, itemState)
                    return value.ValueKind switch
                    {
                        // We should use EDM type to disambiguate type when multiple types are possible
                        // TODO handle dates and byte arrays
                        JsonValueKind.String => writer.WriteValue(value.GetString(), state),
                        // TODO handle different kinds of numbers
                        JsonValueKind.Number => writer.WriteValue(value.GetDouble(), state),
                        JsonValueKind.True => writer.WriteValue(true, state),
                        JsonValueKind.False => writer.WriteValue(false, state),
                        JsonValueKind.Null => writer.WriteValue<object>(null, state),
                        JsonValueKind.Object => writer.WriteValue(value, state),
                        JsonValueKind.Array => writer.WriteValue(value, state),
                        _ => true,
                    };
                }
            },
            
            PropertySelector = new ODataPropertyEnumeratorSelector<JsonElement, JsonElement.ObjectEnumerator, JsonProperty, WriterState>
            {
                GetPropertiesEnumerator = (resource, state) => resource.EnumerateObject(),
                WriteProperty = (resource, property, writer, state) =>
                {
                    if (property.Name.StartsWith("#"))
                    {
                        // Annotations are handled separately
                        return true;
                    }

                    var value = property.Value;
                    return value.ValueKind switch
                    {
                        // TODO handle dates and byte arrays
                        JsonValueKind.String => writer.WriteProperty(property.Name, value.GetString(), state),
                        // TODO handle different kinds of numbers
                        JsonValueKind.Number => writer.WriteProperty(property.Name, value.GetDouble(), state),
                        JsonValueKind.True => writer.WriteProperty(property.Name, true, state),
                        JsonValueKind.False => writer.WriteProperty(property.Name, false, state),
                        JsonValueKind.Null => writer.WriteProperty<object>(property.Name, null, state),
                        JsonValueKind.Object => writer.WriteProperty(property.Name, value, state),
                        JsonValueKind.Array => writer.WriteProperty(property.Name, value, state),
                        
                        _ => true,
                    };
                }
            }

        });
        
        var model = CreateEdmModel();
        var odataUri = new ODataUriParser(
            model,
            new Uri("http://service/odata"),
            new Uri("Items", UriKind.Relative)
        ).ParseUri();

        var state = new WriterState
        {
            EdmStructuredType = model.FindDeclaredType("ns.Item") as IEdmStructuredType
        };

        // Act
        var stream = new MemoryStream();
        await ODataSerializer.WriteAsync(input, stream, odataUri, model, options, state);

        // Assert
        stream.Position = 0;
        var actualOutput = new StreamReader(stream).ReadToEnd();
        var actualNormalizedOutput = JsonSerializer.Serialize(JsonDocument.Parse(actualOutput));

        var expectedOutput = """
            {
                "@odata.context": "http://service/odata/$metadata#Items",
                "value": [
                    {
                        "Id": 1,
                        "Name": "Item 1",
                        "Details": {
                            "Description": "This is item 1",
                            "Price": 9.99
                        },
                        "Tags": ["Tag1", "Tag2"],
                        "Comments": [
                            {
                                "Id": 101,
                                "User": "UserA",
                                "Message": "Great item!"
                            },
                            {
                                "Id": 102,
                                "User": "UserB",
                                "Message": "Good value for money."
                            }
                        ]
                    },
                    {    "Id": 2,
                        "Name": "Item 2",
                        "Details": {
                            "Description": "This is item 2",
                            "Price": 19.99
                        },
                        "Tags": ["Tag3", "Tag4"],
                        "Comments": [
                            {
                                "Id": 201,
                                "User": "UserC",
                                "Message": "Not what I expected."
                            }
                        ]
                    },
                    {   
                        "Id": 3,
                        "Name": "Item 3",
                        "Details": {
                            "Description": "This is item 3",
                            "Price": 29.99
                        },
                        "Tags": [],
                        "Comments": []
                    }
                ]
            }
            """;
        var expectedNormalizedOutput = JsonSerializer.Serialize(JsonDocument.Parse(expectedOutput));

        Assert.Equal(expectedNormalizedOutput, actualNormalizedOutput);
    }


    private static IEdmModel CreateEdmModel()
    {
        var model = new EdmModel();

        var itemDetails = model.AddComplexType("ns", "ItemDetails");
        itemDetails.AddStructuralProperty("Description", EdmPrimitiveTypeKind.String);
        itemDetails.AddStructuralProperty("Price", EdmPrimitiveTypeKind.Decimal);

        var comment = model.AddEntityType("ns", "Comment");
        comment.AddKeys(comment.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        comment.AddStructuralProperty("User", EdmPrimitiveTypeKind.String);
        comment.AddStructuralProperty("Message", EdmPrimitiveTypeKind.String);

        var item = model.AddEntityType("ns", "Item");
        item.AddKeys(item.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
        item.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
        item.AddStructuralProperty("Tags", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(isNullable: false))));
        item.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
        {
            Name = "Comments",
            TargetMultiplicity = EdmMultiplicity.Many,
            Target = comment,
            ContainsTarget = true
        });

        model.AddEntityContainer("ns", "Container")
            .AddEntitySet("Items", item);

        return model;
    }

    class WriterState
    {
        public IEdmStructuredType? EdmStructuredType { get; set; }
    }

    async IAsyncEnumerable<JsonProperty> GetPropertiesEnumerator(JsonElement resource, ODataWriterState<WriterState> state)
    {
        foreach (var property in resource.EnumerateObject())
        {
            await Task.Delay(0);
            yield return property;
        }
    }
}
