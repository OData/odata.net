//---------------------------------------------------------------------
// <copyright file="SingletonUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.TestCommon.Helpers;
using Microsoft.OData.Client.E2E.Tests.Common.Client.Default.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.Default;
using Microsoft.OData.Client.E2E.Tests.SingletonTests.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.SingletonTests.Tests;

public class SingletonUpdateTests : EndToEndTestBase<SingletonUpdateTests.TestsStartup>
{
    private const string NameSpacePrefix = "Microsoft.OData.Client.E2E.Tests.Common.Server.Default";

    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(SingletonTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public SingletonUpdateTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    public static IEnumerable<object[]> MimeTypesData
    {
        get
        {
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task UpdateSingletonProperty(string mimeType)
    {
        ResetDefaultDataSource();

        // Arrange
        var cities = new Dictionary<string, string>
        {
            { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata, "Seattle" },
            { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata, "Paris" },
            { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata, "New York" }
        };

        // Act
        var entries = await TestsHelper.QueryResourceEntriesAsync("VipCustomer", mimeType);
        var customerEntry = entries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));

        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            // Assert
            Assert.NotNull(customerEntry);
            var cityProperty = customerEntry.Properties.Single(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(cityProperty);
            Assert.Equal("London", cityProperty.Value);
        }

        // Arrange
        var properties = new[] { new ODataProperty { Name = "City", Value = cities[mimeType] } };

        // Act
        await this.UpdateEntryAsync("Customer", "VipCustomer", mimeType, properties);
        var updatedEntries = await TestsHelper.QueryResourceEntriesAsync("VipCustomer", mimeType);

        // Assert
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            var updatedCustomerEntry = updatedEntries.SingleOrDefault(e => e != null && e.TypeName.EndsWith("Customer"));
            Assert.NotNull(updatedCustomerEntry);

            var cityProperty = updatedCustomerEntry.Properties.Single(p => p.Name == "City") as ODataProperty;
            Assert.NotNull(cityProperty);
            Assert.Equal(cities[mimeType], cityProperty.Value);
        }
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task UpdateSingletonComplexProperty(string mimeType)
    {
        // Arrange
        var complex0 = new ODataResource()
        {
            TypeName = $"{NameSpacePrefix}.Address",
            Properties =
            [
                new ODataProperty() {Name = "Street", Value = "1 Microsoft Way"},
                new ODataProperty() {Name = "City", Value = "London"},
                new ODataProperty() {Name = "PostalCode", Value = "98052"},
                new ODataProperty() {Name = "UpdatedTime", Value = DateTimeOffset.Parse("1/1/0001 12:00:00 AM +00:00")}
            ]
        };

        var homeAddress0 = new ODataNestedResourceInfo() { Name = "HomeAddress", IsCollection = false };
        homeAddress0.SetAnnotation(complex0);

        var complex1 = new ODataResource()
        {
            TypeName = $"{NameSpacePrefix}.Address",
            Properties =
            [
                new ODataProperty() {Name = "Street", Value = "Zixing 999"},
                new ODataProperty() {Name = "City", Value = "Seattle"},
                new ODataProperty() {Name = "PostalCode", Value = "1111"},
                new ODataProperty() {Name = "UpdatedTime", Value = DateTimeOffset.Parse("1/1/0001 12:00:00 AM +00:00")}
            ]
        };

        var homeAddress1 = new ODataNestedResourceInfo() { Name = "HomeAddress", IsCollection = false };
        homeAddress1.SetAnnotation(complex1);

        var currentHomeAddress = complex0;
        var updatedHomeAddress = complex1;
        var properties = new[] { homeAddress1 };

        if(mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata))
        {
            currentHomeAddress = complex0;
            updatedHomeAddress = complex0;
            properties = new[] { homeAddress0 };
        }

        // Act
        var entries = await TestsHelper.QueryResourceEntriesAsync("VipCustomer", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            // Assert
            ODataValueAssertEqualHelper.AssertODataPropertyAndResourceEqual(currentHomeAddress, entries[0]);
        }

        // Act
        await this.UpdateEntryAsync("Customer", "VipCustomer", mimeType, properties);

        var updatedEntries = await TestsHelper.QueryResourceEntriesAsync("VipCustomer", mimeType);
        if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
        {
            // Assert
            ODataValueAssertEqualHelper.AssertODataPropertyAndResourceEqual(updatedHomeAddress, updatedEntries[0]);
        }

        ResetDefaultDataSource();
    }

    #region Private methods

    private ODataMessageReaderTestsHelper TestsHelper
    {
        get
        {
            return new ODataMessageReaderTestsHelper(_baseUri, _model, Client);
        }
    }

    private async Task UpdateEntryAsync(string singletonType, string singletonName, string mimeType, IEnumerable<object> properties)
    {
        var entry = new ODataResource() { TypeName = $"{NameSpacePrefix}.{singletonType}" };
        var elementType = properties != null && properties.Any() ? properties.ElementAt(0).GetType() : null;

        Assert.NotNull(properties);

        if (elementType == typeof(ODataProperty))
        {
            entry.Properties = properties.Cast<ODataProperty>();
        }

        var settings = new ODataMessageWriterSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false, // Ensure the stream is not disposed of prematurely
        };

        var customerType = _model.FindDeclaredType(NameSpacePrefix + singletonType) as IEdmEntityType;
        var customerSet = _model.EntityContainer.FindSingleton(singletonName);

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri + singletonName), Client);
        requestMessage.SetHeader("Content-Type", mimeType);
        requestMessage.SetHeader("Accept", mimeType);
        requestMessage.Method = "PATCH";

        using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
        {
            var odataWriter = await messageWriter.CreateODataResourceWriterAsync(customerSet, customerType);
            await odataWriter.WriteStartAsync(entry);
            if (elementType == typeof(ODataNestedResourceInfo))
            {
                foreach (var p in properties)
                {
                    var nestedInfo = (ODataNestedResourceInfo)p;
                    await odataWriter.WriteStartAsync(nestedInfo);
                    await odataWriter.WriteStartAsync(nestedInfo.GetAnnotation<ODataResource>());
                    await odataWriter.WriteEndAsync();
                    await odataWriter.WriteEndAsync();
                }
            }
            await odataWriter.WriteEndAsync();
        }

        var responseMessage = await requestMessage.GetResponseAsync();

        // verify the update
        Assert.Equal(204, responseMessage.StatusCode);
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "singletontests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
