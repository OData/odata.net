//-----------------------------------------------------------------------------
// <copyright file="QueryCountTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.EndToEnd.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.E2E.TestCommon.Common.Server.QueryCount;
using Microsoft.OData.Edm;
using System.Text.RegularExpressions;

namespace Microsoft.OData.Core.E2E.Tests.QueryCountTests;

public class QueryCountTests : EndToEndTestBase<QueryCountTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(QueryCountTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
        }
    }

    public QueryCountTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
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

    // Constants
    private const string MimeTypeApplicationAtomXml = MimeTypes.ApplicationAtomXml;
    private const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private const string MimeTypeODataParameterMinimalMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata;
    private const string MimeTypeODataParameterIEEE754Compatible = MimeTypes.ApplicationJson + MimeTypes.ODataParameterIEEE754Compatible;
    private const string MimeTypeODataParameterNoMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata;

    #region Positive tests

    /// <summary>
    /// when $count=true results 
    /// odata.count in json payload 
    /// and 
    /// m:count in atom payload
    /// </summary>
    [Theory]
    [InlineData(MimeTypeApplicationAtomXml)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterIEEE754Compatible)]
    [InlineData(MimeTypeODataParameterNoMetadata)]
    public async Task CountPayloadVerification(string mimeType)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Customers?$count=true", UriKind.Absolute);

        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var responseString = await ReadResponseMessageAsync(requestMessage);

        // Assert
        if (mimeType.Contains(MimeTypes.ODataParameterIEEE754Compatible))
        {
            Assert.Contains("\"@odata.count\":\"10\"", responseString);
        }
        else
        {
            Assert.Contains("\"@odata.count\":10", responseString);
        }
    }

    /// <summary>
    /// $count=false is the default value 
    /// payload should be same with specifying nothing
    /// </summary>
    [Theory]
    [InlineData(MimeTypeApplicationAtomXml)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterIEEE754Compatible)]
    [InlineData(MimeTypeODataParameterNoMetadata)]
    public async Task FalseCountIsDefaultValue(string mimeType)
    {
        // Arrange
        ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri };

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "Computers", UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        // Act
        var defaultResponseString = await ReadResponseMessageAsync(requestMessage);

        var requestMessage2 = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + "Computers?$count=false", UriKind.Absolute), Client);
        requestMessage2.SetHeader("Accept", mimeType);

        var responseString = await ReadResponseMessageAsync(requestMessage2);

        // Assert
        if (mimeType == MimeTypes.ApplicationAtomXml)
        {
            // resulting atom payloads with/without model should be the same except for the updated time stamps
            const string pattern = @"<updated>([A-Za-z0-9\-\:]{20})\</updated>";
            const string replacement = "<updated>0000-00-00T00:00:00Z</updated>";
            defaultResponseString = Regex.Replace(defaultResponseString, pattern, (match) => replacement);
            responseString = Regex.Replace(responseString, pattern, (match) => replacement);
        }

        Assert.Equal(defaultResponseString, responseString);
    }

    #endregion

    #region Private

    private static async Task<string> ReadResponseMessageAsync(TestHttpClientRequestMessage requestMessage)
    {
        var responseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);

        var stream = await responseMessage.GetStreamAsync();
        using var streamReader = new StreamReader(stream);
        return await streamReader.ReadToEndAsync();
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "querycounttests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
