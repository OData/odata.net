//-----------------------------------------------------
// <copyright file="DynamicPropertiesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.DynamicProperties;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.DynamicProperties;

public class DynamicPropertiesTests : EndToEndTestBase<DynamicPropertiesTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;
    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default.";

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(DynamicPropertiesController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public DynamicPropertiesTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        _model = DefaultEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    public async Task CanReadAllDynamicProperties(string mimeType)
    {
        // Arrange
        var queryText = "Accounts(102)/AccountInfo";

        var readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri };
        var uri = new Uri(_baseUri.AbsoluteUri + queryText, UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(uri, Client);

        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        ODataProperty? property = null;
        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
        {
            property = messageReader.ReadProperty();
            Assert.NotNull(property);
        }

        // Assert
        Assert.NotNull(property);

        var properties = (property.Value as ODataResourceValue)?.Properties;
        Assert.NotNull(properties);

        Assert.Equal("James", properties.Single(p => p.Name == "FirstName").Value);
        Assert.Equal("Bunder", properties.Single(p => p.Name == "LastName").Value);

        Assert.Equal(12345, properties.Single(p => p.Name == "ShortNum").Value);
        Assert.Equal(123456789L, properties.Single(p => p.Name == "LongNum").Value);
        Assert.Equal((double)123.45, properties.Single(p => p.Name == "DoubleNum").Value);
        Assert.Equal(123.56f, properties.Single(p => p.Name == "FloatNum").Value);
        Assert.Equal(123.67m, properties.Single(p => p.Name == "DecimalNum").Value);
        Assert.Equal(1234567890123456789L, properties.Single(p => p.Name == "BigIntNum").Value);
        Assert.Equal(123, properties.Single(p => p.Name == "IntNum").Value);

        Assert.Equal((byte)123, (byte)properties.Single(p => p.Name == "ByteNum").Value);
        Assert.Equal((sbyte)-123, (sbyte)properties.Single(p => p.Name == "SByteNum").Value);

        Assert.Equal("Sample String", (string)properties.Single(p => p.Name == "String").Value);
        Assert.Equal("q", (string)properties.Single(p => p.Name == "Char").Value);

        Assert.False((bool)properties.Single(p => p.Name == "Boolean").Value);
        Assert.Equal(Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6"), (Guid)properties.Single(p => p.Name == "Guid").Value);

        Assert.Equal(new DateTimeOffset(new DateTime(2023, 10, 25)), properties.Single(p => p.Name == "DateTime").Value);
        Assert.Equal(new DateTimeOffset(new DateTime(2023, 10, 25)), properties.Single(p => p.Name == "DateTimeOffset").Value);
        Assert.Equal(new DateTimeOffset(new DateTime(2023, 10, 25)), properties.Single(p => p.Name == "Date").Value);
        Assert.Equal(new DateTime(2023, 10, 25).TimeOfDay, (TimeSpan)properties.Single(p => p.Name == "TimeOfDay").Value);
        Assert.Equal(new Edm.TimeOfDay(new TimeOnly(2, 50, 20).Ticks), properties.Single(p => p.Name == "TimeOnly").Value);
        Assert.Equal(new Edm.Date(2023, 10, 25), properties.Single(p => p.Name == "DateOnly").Value);
        Assert.Equal(new TimeSpan(2, 50, 20), properties.Single(p => p.Name == "Duration").Value);

        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9, 0], (properties.Single(p => p.Name == "Binary").Value as ODataCollectionValue)?.Items.Cast<byte>().ToList());
        Assert.Equal([56, 100L, 456, 90L], (properties.Single(p => p.Name == "CollectionOfInt").Value as ODataCollectionValue)?.Items.Cast<long>().ToList());
    }

    [Fact]
    public async Task CanAddItemWithDynamicProperties_And_VerifyValues()
    {
        // Arrange
        var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
        var accountSet = _model.EntityContainer.FindEntitySet("Accounts");

        var entry = new ODataResource() 
        { 
            TypeName = NameSpacePrefix + "Account",
            Properties = new[]
            {
                new ODataProperty { Name = "AccountID", Value = 789 },
                new ODataProperty { Name = "CountryRegion", Value = "CN" }
            }
        };

        var accountInfoNestedInfo = new ODataNestedResourceInfo() { Name = "AccountInfo", IsCollection = false };
        var accountInfoResource = new ODataResource
        {
            TypeName = NameSpacePrefix + "AccountInfo",
            Properties = new[]
            {
                new ODataProperty { Name = "FirstName", Value = "Peter" },
                new ODataProperty { Name = "LastName", Value = "Andy" },
                new ODataProperty { Name = "ShortNum", Value = 12345 },
                new ODataProperty { Name = "LongNum", Value = 123456789L },
                new ODataProperty { Name = "DoubleNum", Value = 123.45d },
                new ODataProperty { Name = "FloatNum", Value = 123.56f },
                new ODataProperty { Name = "DecimalNum", Value = 123.67m },
                new ODataProperty { Name = "BigIntNum", Value = 1234567890123456789L },
                new ODataProperty { Name = "IntNum", Value = 123 },
                new ODataProperty { Name = "ByteNum", Value = (byte)123 },
                new ODataProperty { Name = "SByteNum", Value = (sbyte)-123 },
                new ODataProperty { Name = "String", Value = "Sample String" },
                new ODataProperty { Name = "Boolean", Value = true },
                new ODataProperty { Name = "Guid", Value = Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6") },
                new ODataProperty { Name = "DateTimeOffset", Value = new DateTimeOffset(new DateTime(2023, 10, 25)) },
                new ODataProperty { Name = "TimeOfDay", Value = new Edm.TimeOfDay(2, 50, 20, 0) },
                new ODataProperty { Name = "Date", Value = new Edm.Date(2023, 10, 25) },
                new ODataProperty { Name = "TimeOnly", Value = new TimeOnly(3, 50, 30) },
                new ODataProperty { Name = "DateOnly", Value = new DateOnly(2023, 10, 25) },
                new ODataProperty { Name = "Duration", Value = new TimeSpan(2, 50, 20) }
            }
        };

        var settings = new ODataMessageWriterSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };
        var uri = new Uri(_baseUri.AbsoluteUri + "Accounts", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(uri, Client) { Method = "POST" };

        requestMessage.SetHeader("Content-Type", MimeTypes.ApplicationJson);
        requestMessage.SetHeader("Accept", MimeTypes.ApplicationJson);

        using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
        {
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(accountSet, accountType);
            await odataWriter.WriteStartAsync(entry);
            await odataWriter.WriteStartAsync(accountInfoNestedInfo);
            await odataWriter.WriteStartAsync(accountInfoResource);
            await odataWriter.WriteEndAsync();
            await odataWriter.WriteEndAsync();
            await odataWriter.WriteEndAsync();
        }
        
        // Act
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(201, responseMessage.StatusCode);

        var addedAccount = await _context.Accounts.ByKey(789).GetValueAsync();
        var info = addedAccount.AccountInfo;

        Assert.NotNull(info);
        Assert.Equal("Peter", info.FirstName);
        Assert.Equal("Andy", info.LastName);
        Assert.Equal(123.45,Convert.ToDouble(info.DynamicProperties["DoubleNum"]));
        Assert.Equal(123.56f, info.DynamicProperties["FloatNum"]);
        Assert.Equal(123.67m, info.DynamicProperties["DecimalNum"]);
        Assert.Equal(1234567890123456789L, info.DynamicProperties["BigIntNum"]);
        Assert.Equal(123, info.DynamicProperties["IntNum"]);
        Assert.Equal((byte)123, info.DynamicProperties["ByteNum"]);
        Assert.Equal((sbyte)-123, info.DynamicProperties["SByteNum"]);
        Assert.Equal("Sample String", info.DynamicProperties["String"]);
        Assert.True((bool)info.DynamicProperties["Boolean"]);
        Assert.Equal(Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6"), info.DynamicProperties["Guid"]);
        Assert.Equal(new DateTimeOffset(new DateTime(2023, 10, 25)), info.DynamicProperties["DateTimeOffset"]);
        Assert.Equal(new Edm.Date(2023, 10, 25), info.DynamicProperties["Date"]);
        Assert.Equal(new Edm.TimeOfDay(2, 50, 20, 0), info.DynamicProperties["TimeOfDay"]);
        Assert.Equal(new Edm.TimeOfDay(new TimeOnly(3, 50, 30).Ticks), info.DynamicProperties["TimeOnly"]);
        Assert.Equal(new Edm.Date(2023, 10, 25), info.DynamicProperties["DateOnly"]);
        Assert.Equal(new TimeSpan(2, 50, 20), info.DynamicProperties["Duration"]);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "dynamicpropertiestests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
