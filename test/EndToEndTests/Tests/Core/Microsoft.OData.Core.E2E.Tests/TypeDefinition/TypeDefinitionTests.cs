//---------------------------------------------------------------------
// <copyright file="TypeDefinitionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Formatter.Deserialization;
using Microsoft.AspNetCore.OData.Formatter.Serialization;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.TypeDefinition;

public class TypeDefinitionTests : EndToEndTestBase<TypeDefinitionTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly IEdmModel _model;
    private readonly ODataMessageReaderSettings _readerSettings;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(TypeDefinitionTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", TypeDefinitionEdmModel.GetEdmModel(),
                    configureServices: s => s
                        .AddSingleton<IFilterBinder, CustomFilterBinder>()
                        .AddSingleton<IOrderByBinder, CustomOrderByBinder>()
                        .AddSingleton<ODataResourceDeserializer, CustomResourceReserialize>()
                        .AddSingleton<ODataResourceSerializer, CustomResourceSerializer>()
                ));
        }
    }

    public TypeDefinitionTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _model = TypeDefinitionEdmModel.GetEdmModel();

        _readerSettings = new ODataMessageReaderSettings
        {
            BaseUri = _baseUri
        };

        ResetDefaultDataSource();
    }

    // Constants
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.";
    private new const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private new const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryEntryWithTypeDefinitionMetadata(string mimeType)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnablePrimitiveTypeConversion = true };

        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var queryResponseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, queryResponseMessage.StatusCode);

        ODataResource? entry = null;

        using (var messageReader = new ODataMessageReader(queryResponseMessage, _readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entry = reader.Item as ODataResource;
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        Assert.NotNull(entry);
        Assert.Equal(1, (entry.Properties.Single(p => p.Name == "PersonId") as ODataProperty)?.Value);
        Assert.Equal("Bob", (entry.Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal("Cat", (entry.Properties.Single(p => p.Name == "LastName") as ODataProperty)?.Value);
        Assert.Equal("1.8m", (entry.Properties.Single(p => p.Name == "Height") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryTopLevelPropertyWithTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)/LastName", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? property = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            property = await messageReader.ReadPropertyAsync();
            Assert.NotNull(property);
        }

        Assert.Equal("Cat", property.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryComplexPropertyWithTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)/Address", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataResource? complex = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var odataReader = await messageReader.CreateODataResourceReaderAsync();
            while (await odataReader.ReadAsync())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    complex = odataReader.Item as ODataResource;
                }
            }

            Assert.NotNull(complex);
        }

        Assert.Equal("Zixing Road", (complex.Properties.Single(p => p.Name == "Road") as ODataProperty)?.Value);
        Assert.Equal("Shanghai", (complex.Properties.Single(p => p.Name == "City") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryCollectionPropertyWithTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)/Descriptions", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? property = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            property = await messageReader.ReadPropertyAsync();
            Assert.NotNull(property);
        }

        var collectionValue = property.Value as ODataCollectionValue;
        var items = collectionValue?.Items.OfType<object>().ToArray();
        Assert.NotNull(items);
        Assert.Equal(2, items.Length);
        Assert.Equal("Tall", items[1]);
        Assert.Equal("Collection(Edm.String)", collectionValue?.TypeName);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryCustomDefinedPropertyWithTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(4)/Height", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? property = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            property = await messageReader.ReadPropertyAsync();
            Assert.NotNull(property);
        }

        Assert.Equal("62in", property.Value);
    }

    [Fact]
    public async Task QueryPropertyValueWithTypeDefinition()
    {
        // Arrange
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "People(5)/LastName/$value", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", "*/*");

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var lastNameValue = messageReader.ReadValue(EdmCoreModel.Instance.GetString(false));
            Assert.Equal("Bee", lastNameValue);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryAndFilterOnPropertyWithTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People?$filter=FirstName ne 'Bob'", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = reader.Item as ODataResource;
                    if (entry != null && entry.TypeName.Contains("Person"))
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);

        }

        Assert.Equal(4, entries.Count);
        Assert.All(entries, entry => Assert.NotEqual("Bob", (entry.Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryAndOrderbyPropertyWithTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People?$orderby=FirstName desc", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = reader.Item as ODataResource;
                    if (entry != null && entry.TypeName.Contains("Person"))
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);

        }

        Assert.Equal(5, entries.Count);
        Assert.All(entries, entry => Assert.NotNull((entry.Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value));
        Assert.Equal("Peter", (entries[0].Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value);
        Assert.Equal("Bob", (entries[4].Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task InvokeFunctionWithDefinedTypeParameterAndReturnType(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "People(1)/Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.GetFullName(nickname='Moon')", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? fullName = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            fullName = await messageReader.ReadPropertyAsync();
            Assert.NotNull(fullName);
        }

        Assert.Equal("Bob (Moon) Cat", fullName.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task CreateEntityWithDefinedTypeProperties(string mimeType)
    {
        // Arrange
        var entryWrapper = new ODataResourceWrapper()
        {
            Resource = new ODataResource
            {
                TypeName = NameSpacePrefix + "Person",
                Properties =
                [
                    new ODataProperty { Name = "PersonId", Value = 101 },
                    new ODataProperty { Name = "FirstName", Value = "Peter" },
                    new ODataProperty { Name = "LastName", Value = "Zhang" },
                    new ODataProperty { Name = "Height", Value = "1.8m" },
                    new ODataProperty
                    {
                        Name = "Descriptions",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = "Collection(" + NameSpacePrefix + "Name)",
                            Items = new[] { "Description1", "Description2" }
                        }
                    }
                ]
            },
            NestedResourceInfoWrappers =
            [
                new ODataNestedResourceInfoWrapper
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "Address",
                        IsCollection = false
                    },
                    NestedResourceOrResourceSet = new ODataResourceWrapper()
                    {
                        Resource = new ODataResource()
                        {
                            TypeName = NameSpacePrefix + "Address",
                            Properties =
                            [
                                new ODataProperty
                                {
                                    Name = "Road",
                                    Value = "Road one"
                                },
                                new ODataProperty
                                {
                                    Name = "City",
                                    Value = "Shanghai"
                                }
                            ]
                        }
                    }
                },
            ]
        };

        var settings = new ODataMessageWriterSettings { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

        var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
        var peopleSet = _model.EntityContainer.FindEntitySet("People");

        var saveRequestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "People"), Client)
        {
            Method = "POST"
        };
        saveRequestMessage.SetHeader("Content-Type", mimeType);
        saveRequestMessage.SetHeader("Accept", mimeType);

        using (var messageWriter = new ODataMessageWriter(saveRequestMessage, settings, _model))
        {
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(peopleSet, personType);
            await ODataWriterHelper.WriteResourceAsync(odataWriter, entryWrapper);
        }

        // Act
        var responseMessage = await saveRequestMessage.GetResponseAsync();

        // verify the insert
        Assert.Equal(201, responseMessage.StatusCode);

        var entry = await this.QueryEntryAsync("People(101)", mimeType);

        Assert.NotNull(entry);
        Assert.Equal(101, (entry.Properties.Single(p => p.Name == "PersonId") as ODataProperty)?.Value);
        Assert.Equal("Zhang", (entry.Properties.Single(p => p.Name == "LastName") as ODataProperty)?.Value);
        Assert.Equal("1.8m", (entry.Properties.Single(p => p.Name == "Height") as ODataProperty)?.Value);

        var descriptions = (entry.Properties.Single(p => p.Name == "Descriptions") as ODataProperty)?.Value as ODataCollectionValue;
        Assert.NotNull(descriptions);
        Assert.Equal(2, descriptions.Items.Count());
        Assert.Equal("Description2", descriptions.Items.ElementAt(1));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryEntryWithUnsignedIntegerProperties(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Products", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var entries = new List<ODataResource>();
        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceSetReaderAsync();

            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    var entry = reader.Item as ODataResource;
                    if (entry != null && entry.TypeName.Contains("Product"))
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        Assert.Equal(5, entries.Count);

        var productid = (entries[0].Properties.Single(p => p.Name == "ProductId") as ODataProperty)?.Value;
        var quantity = (entries[0].Properties.Single(p => p.Name == "Quantity") as ODataProperty)?.Value;
        var lifetime = (entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds") as ODataProperty)?.Value;
        Assert.Equal((UInt16)11, productid);
        Assert.Equal(100u, quantity);
        Assert.Equal(3600ul, lifetime);

        productid = (entries[1].Properties.Single(p => p.Name == "ProductId") as ODataProperty)?.Value;
        quantity = (entries[1].Properties.Single(p => p.Name == "Quantity") as ODataProperty)?.Value;
        lifetime = (entries[1].Properties.Single(p => p.Name == "LifeTimeInSeconds") as ODataProperty)?.Value;
        Assert.Equal((UInt16)12, productid);
        Assert.Equal(UInt32.MaxValue, quantity);
        Assert.Equal(UInt64.MaxValue, lifetime);

        productid = (entries[2].Properties.Single(p => p.Name == "ProductId") as ODataProperty)?.Value;
        quantity = (entries[2].Properties.Single(p => p.Name == "Quantity") as ODataProperty)?.Value;
        lifetime = (entries[2].Properties.Single(p => p.Name == "LifeTimeInSeconds") as ODataProperty)?.Value;
        Assert.Equal((UInt16)13, productid);
        Assert.Equal(UInt32.MinValue, quantity);
        Assert.Equal(UInt64.MinValue, lifetime);
    }

    [Theory]
    [InlineData("Products(11)/ProductId", MimeTypeODataParameterFullMetadata)]
    [InlineData("Products(11)/ProductId", MimeTypeODataParameterMinimalMetadata)]
    [InlineData("Products(15)/Quantity", MimeTypeODataParameterFullMetadata)]
    [InlineData("Products(15)/Quantity", MimeTypeODataParameterMinimalMetadata)]
    [InlineData("Products(12)/LifeTimeInSeconds", MimeTypeODataParameterFullMetadata)]
    [InlineData ("Products(12)/LifeTimeInSeconds", MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryUnsignedIntegerProperties(string queryText, string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        ODataProperty? property = null;
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            property = messageReader.ReadProperty();
            Assert.NotNull(property);
        }

        if(queryText.EndsWith("ProductId"))
        {
            Assert.Equal((UInt16)11, property?.Value);
        }
        else if(queryText.EndsWith("Quantity"))
        {
            Assert.Equal(105u, property?.Value);
        }
        else if (queryText.EndsWith("LifeTimeInSeconds"))
        {
            Assert.Equal(18446744073709551615ul, property?.Value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryComplexPropertyWithUintMembers(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Products(11)/TheCombo", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataResource? complex = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var odataReader = messageReader.CreateODataResourceReader();
            while (odataReader.Read())
            {
                if (odataReader.State == ODataReaderState.ResourceEnd)
                {
                    complex = odataReader.Item as ODataResource;
                }
            }

            Assert.NotNull(complex);
        }

        // Assert
        Assert.NotNull(complex);
        Assert.Equal((UInt16)80, (complex.Properties.Single(p => p.Name == "Small") as ODataProperty)?.Value);
        Assert.Equal((UInt32)196, (complex.Properties.Single(p => p.Name == "Middle") as ODataProperty)?.Value);
        Assert.Equal((UInt64)3, (complex.Properties.Single(p => p.Name == "Large") as ODataProperty)?.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryCollectionPropertyOfUIntMembers(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Products(11)/LargeNumbers", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        ODataProperty? property = null;
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            property = messageReader.ReadProperty();
            Assert.NotNull(property);
        }

        Assert.NotNull(property);
        var collectionValue = property.Value as ODataCollectionValue;
        var items = collectionValue?.Items.OfType<object>().ToArray();

        // Assert
        Assert.NotNull(items);
        Assert.Equal(3, items.Length);
        Assert.Equal(36, (long)items[0]);
    }

    [Fact]
    public async Task QueryPropertyValueOfUintMembers()
    {
        // Arrange
        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "Products(11)/Quantity/$value", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", "*/*");

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var value = await messageReader.ReadValueAsync(EdmCoreModel.Instance.GetInt32(false));
            Assert.Equal(100, value);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryPropertyWithCustomEdmTypeDefinition(string mimeType)
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Products(11)/Temperature", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? property = null;
        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            property = await messageReader.ReadPropertyAsync();
            Assert.NotNull(property);
        }

        Assert.Equal("10.57℉", property.Value);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task CreateEntityWithUIntProperties(string mimeType)
    {
        // Arrange
        var entry = new ODataResource
        {
            TypeName = NameSpacePrefix + "Product",
            Properties = new[]
            {
                    new ODataProperty { Name = "ProductId", Value = (UInt16)101 },
                    new ODataProperty { Name = "Quantity", Value = 19u },
                    new ODataProperty { Name = "LifeTimeInSeconds", Value = 86ul },
                    new ODataProperty { Name = "NullableUInt32", Value = 37u },
                    new ODataProperty { Name = "Temperature", Value = "10.57℉" },
                    new ODataProperty
                    {
                        Name = "LargeNumbers",
                        Value = new ODataCollectionValue()
                        {
                            TypeName = "Collection(" + NameSpacePrefix + "UInt64)",
                            Items = [32ul, 97ul],

                        }
                    }
                }
        };

        var entryWrapper = new ODataResourceWrapper()
        {
            Resource = entry,
            NestedResourceInfoWrappers =
            [
                new ODataNestedResourceInfoWrapper()
                {
                    NestedResourceInfo = new ODataNestedResourceInfo()
                    {
                        Name = "TheCombo",
                        IsCollection = false
                    },
                    NestedResourceOrResourceSet = new ODataResourceWrapper()
                    {
                        Resource = new ODataResource()
                        {
                            TypeName = NameSpacePrefix + "NumberCombo",
                            Properties = new[]
                            {
                                new ODataProperty
                                {
                                    Name = "Small",
                                    Value = (UInt16)10
                                },
                                new ODataProperty
                                {
                                    Name = "Middle",
                                    Value = 33u
                                },
                                new ODataProperty
                                {
                                    Name = "Large",
                                    Value = 101ul
                                }
                            }
                        }
                    }
                }
            ]
        };
        var settings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };

        var productType = _model.FindDeclaredType(NameSpacePrefix + "Product") as IEdmEntityType;
        var productsSet = _model.EntityContainer.FindEntitySet("Products");

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + "Products"), Client);
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.Method = "POST";
        using (var messageWriter = new ODataMessageWriter(requestMessage, settings, _model))
        {
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(productsSet, productType);
            await ODataWriterHelper.WriteResourceAsync(odataWriter, entryWrapper);
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(201, responseMessage.StatusCode);

        var createdEntry = await this.QueryEntryAsync("Products(101)", mimeType);
        Assert.NotNull(createdEntry);
        Assert.Equal((UInt16)101, (createdEntry.Properties.Single(p => p.Name == "ProductId") as ODataProperty)?.Value);
        Assert.Equal(86ul, (createdEntry.Properties.Single(p => p.Name == "LifeTimeInSeconds") as ODataProperty)?.Value);
        Assert.Equal("10.57℉", (createdEntry.Properties.Single(p => p.Name == "Temperature") as ODataProperty)?.Value);

        var largeNumbers = (createdEntry.Properties.Single(p => p.Name == "LargeNumbers") as ODataProperty)?.Value as ODataCollectionValue;
        Assert.NotNull(largeNumbers);
        Assert.Equal(2, largeNumbers.Items.Count());
        Assert.Equal(32ul, largeNumbers.Items.ElementAt(0));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task InvokeActionWithUintParameterAndReturnType(string mimeType)
    {
        // Arrange
        var writerSettings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };
        var readerSettings = new ODataMessageReaderSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };
        var requestUri = new Uri(_baseUri + "Products(11)/Microsoft.OData.E2E.TestCommon.Common.Server.TypeDefinition.ExtendLifeTime", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUri, Client);

        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.Method = "POST";

        using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, _model))
        {
            var odataWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
            odataWriter.WriteStart();
            odataWriter.WriteValue("seconds", 1000u);
            odataWriter.WriteEnd();
        }

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var property = messageReader.ReadProperty();
            Assert.NotNull(property);
            Assert.Equal(4600, (long)property.Value);
        }
    }

    [Theory]
    [InlineData("Products?$filter=Quantity eq 100", MimeTypeODataParameterFullMetadata, "Quantity")]
    [InlineData("Products?$filter=Quantity eq 100", MimeTypeODataParameterMinimalMetadata, "Quantity")]
    [InlineData("Products?$filter=18446744073709551615 eq LifeTimeInSeconds", MimeTypeODataParameterFullMetadata, "LifeTimeInSeconds")]
    [InlineData("Products?$filter=18446744073709551615 eq LifeTimeInSeconds", MimeTypeODataParameterMinimalMetadata, "LifeTimeInSeconds")]
    [InlineData("Products?$filter=NullableUInt32 eq null", MimeTypeODataParameterFullMetadata, "NullableUInt32")]
    [InlineData("Products?$filter=NullableUInt32 eq null", MimeTypeODataParameterMinimalMetadata, "NullableUInt32")]
    public async Task QueryAndFilterByUnsignedIntegerProperties(string queryText, string mimeType, string propertyName)
    {
        // Arrange
        var entries = new List<ODataResource>();
        var requestUrl = new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.Contains("Product"))
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        if(propertyName == "Quantity")
        {
            Assert.Single(entries);
            var quantity = (entries[0].Properties.Single(p => p.Name == propertyName) as ODataProperty)?.Value;
            Assert.Equal(100u, quantity);
        }
        else if (propertyName == "LifeTimeInSeconds")
        {
            Assert.Single(entries);
            var lifetime = (entries[0].Properties.Single(p => p.Name == propertyName) as ODataProperty)?.Value; //UInt64.Max
            Assert.Equal(UInt64.MaxValue, lifetime); 
        }
        else if (propertyName == "NullableUInt32")
        {
            Assert.Equal(2, entries.Count);
            var nullable = (entries[0].Properties.Single(p => p.Name == propertyName) as ODataProperty)?.Value; //null
            Assert.Null(nullable);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryAndOrderByUnsignedIntegerProperties(string mimeType)
    {
        // Arrange
        var entries = new List<ODataResource>();

        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Products?$orderby=LifeTimeInSeconds desc", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry && entry.TypeName.Contains("Product"))
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        Assert.Equal(5, entries.Count);

        var lifetime = (entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds") as ODataProperty)?.Value;
        Assert.Equal(UInt64.MaxValue, lifetime);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task QueryAndSelectUnsignedIntegerProperties(string mimeType)
    {
        // Arrange
        var entries = new List<ODataResource>();

        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Products?$select=ProductId, LifeTimeInSeconds", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();

            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    if (reader.Item is ODataResource entry)
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    Assert.NotNull(reader.Item as ODataResourceSet);
                }
            }
            Assert.Equal(ODataReaderState.Completed, reader.State);

        }

        Assert.Equal(5, entries.Count);

        var lifetime = (entries[0].Properties.Single(p => p.Name == "LifeTimeInSeconds") as ODataProperty)?.Value;
        Assert.Equal(3600ul, lifetime);
    }

    #region Private methods

    public async Task<ODataResource?> QueryEntryAsync(string queryText, string mimeType)
    {
        var requestUrl = new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        var queryResponseMessage = await requestMessage.GetResponseAsync();

        Assert.Equal(200, queryResponseMessage.StatusCode);

        ODataResource? entry = null;

        using (var messageReader = new ODataMessageReader(queryResponseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceReader();
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd)
                {
                    entry = reader.Item as ODataResource;
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        return entry;
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "typedefinitiontests/Default.ResetDefaultDataSource", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(actionUri, Client);
        requestMessage.Method = "POST";

        var responseMessage = requestMessage.GetResponseAsync().Result;

        Assert.Equal(200, responseMessage.StatusCode);
    }

    #endregion
}
