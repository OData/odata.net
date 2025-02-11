//---------------------------------------------------------------------
// <copyright file="StreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using Car = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Car;
using CustomerInfo = Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.CustomerInfo;

namespace Microsoft.OData.Client.E2E.Tests.KeyAsSegmentTests.Tests;

public class StreamTests : EndToEndTestBase<StreamTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(KeyAsSegmentTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public StreamTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

        _model = CommonEndToEndEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    [Fact]
    public void GetReadStreamFromMle()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
        var customerInfoQuery = _context.CreateQuery<CustomerInfo>("CustomerInfos");

        foreach (var customerInfo in customerInfoQuery)
        {
            var streamResponse = _context.GetReadStream(customerInfo);
            Assert.NotNull(streamResponse);
            using (var reader = new StreamReader(streamResponse.Stream))
            {
                var response = reader.ReadToEnd();
                Assert.NotNull(response);
            }
        }
    }

    [Fact]
    public void GetReadStreamUriFromMle()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var customerInfoQuery = _context.CreateQuery<CustomerInfo>("CustomerInfos");

        foreach (var customerInfo in customerInfoQuery)
        {
            var streamResponseUri = _context.GetReadStreamUri(customerInfo);
            Assert.NotNull(streamResponseUri);

            var query = _context.CreateQuery<CustomerInfo>("CustomerInfos").ByKey(customerInfo.CustomerInfoId);

            Assert.False(query.RequestUri.AbsoluteUri.Contains('('), "Uri contains left parentheses");
            Assert.False(query.RequestUri.AbsoluteUri.Contains(')'), "Uri contains right parentheses");
            Assert.EndsWith($"odata/CustomerInfos/{customerInfo.CustomerInfoId}", query.RequestUri.AbsoluteUri);
        }
    }

    [Fact]
    public void GetReadStreamFromNamedStreamProperty()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        var firstCar = _context.CreateQuery<Car>("Cars").Take(1).Single();

        var streamResponse = _context.GetReadStream(firstCar, "Photo", new DataServiceRequestArgs());

        // VerifyStreamReadable
        using (var reader = new StreamReader(streamResponse.Stream))
        {
            var photo = reader.ReadToEnd();
            Assert.NotNull(photo);
        }
    }

    [Fact]
    public void SetSaveStreamOnMle()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        byte[] binaryTestData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
        var memoryStream = new MemoryStream(binaryTestData);

        var customerInfo = _context.CustomerInfos.Take(1).Single();

        // Act
        _context.SetSaveStream(customerInfo, memoryStream, true, "application/binary", "var1");
        var dataServiceResponse = _context.SaveChanges();
        var response = dataServiceResponse.SingleOrDefault() as ChangeOperationResponse;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(204, response.StatusCode);
    }

    [Fact]
    public void SetSaveStreamOnNamedStreamProperty()
    {
        // Arrange
        _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

        byte[] binaryTestData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
        var memoryStream = new MemoryStream(binaryTestData);

        // Act
        var firstCar = _context.CreateQuery<Car>("Cars").Take(1).Single();

        _context.SetSaveStream(firstCar, "Video", memoryStream, true, new DataServiceRequestArgs { ContentType = "application/binary" });
        var dataServiceResponse = _context.SaveChanges();
        var response = dataServiceResponse.SingleOrDefault() as ChangeOperationResponse;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(204, response.StatusCode);
        Assert.IsType<StreamDescriptor>(response.Descriptor);
    }

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "keyassegmenttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
