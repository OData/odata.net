//---------------------------------------------------------------------
// <copyright file="PrimitiveValueFormatTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys;
using Microsoft.OData.Client.E2E.Tests.Common.Server.PrimitiveKeys;
using Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using EdmDecimal = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmDecimal;
using EdmDouble = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmDouble;
using EdmInt64 = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmInt64;
using EdmSingle = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmSingle;

namespace Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests.Tests;

public class PrimitiveValueFormatTest : EndToEndTestBase<PrimitiveValueFormatTest.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(PrimitiveKeyValuesTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", PrimitiveKeyValuesEdmModel.GetEdmModel()));
        }
    }

    public PrimitiveValueFormatTest(TestWebApplicationFactory<PrimitiveValueFormatTest.TestsStartup> fixture) 
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

        _model = PrimitiveKeyValuesEdmModel.GetEdmModel();
        ResetDefaultDataSource();
    }

    public string DataPattern = "(" + @"[\+-]?\d\.?\d*E?[\+-]?\d*" + "|" + @"INF|-INF|NaN" + ")";
    private const string MimeTypeODataParameterFullMetadata = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
    private const string MimeTypeODataParameterFullMetadataAndIEEE754Compatible = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata + MimeTypes.ODataParameterIEEE754Compatible;

    [Theory]
    [InlineData("EdmInt64Set(1)")]
    [InlineData("EdmInt64Set(1L)")]
    [InlineData("EdmInt64Set(1l)")]
    public async Task LongWithOrWithoutSuffixAsKeyInURLTest(string keySegment)
    {
        // Arrange
        var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
        var expectedKeySegment = "EdmInt64Set(1)";

        // Act
        var entries = await TestsHelper.QueryResourceEntriesAsync(keySegment, mimeType);
        var entry = entries.SingleOrDefault();

        // Assert
        Assert.Single(entries);
        Assert.NotNull(entry);
        Assert.Contains(expectedKeySegment, entry.Id.ToString());
        Assert.Contains(expectedKeySegment, entry.EditLink.ToString());
        Assert.Contains(expectedKeySegment, entry.ReadLink.ToString());
    }

    [Theory]
    [InlineData("EdmSingleSet(1.0)")]
    [InlineData("EdmSingleSet(1.0F)")]
    [InlineData("EdmSingleSet(1.0f)")]
    public async Task FloatWithOrWithoutSuffixAsKeyInURLTest(string keySegment)
    {
        // Arrange
        var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
        var expectedKeySegment = "EdmSingleSet(1)";

        // Act
        var entries = await TestsHelper.QueryResourceEntriesAsync(keySegment, mimeType);
        var entry = entries.SingleOrDefault();

        // Assert
        Assert.Single(entries);
        Assert.NotNull(entry);
        Assert.Contains(expectedKeySegment, entry.Id.ToString());
        Assert.Contains(expectedKeySegment, entry.EditLink.ToString());
        Assert.Contains(expectedKeySegment, entry.ReadLink.ToString());
    }

    [Theory]
    [InlineData("EdmDoubleSet(1.0)")]
    [InlineData("EdmDoubleSet(1.0D)")]
    [InlineData("EdmDoubleSet(1.0d)")]
    public async Task DoubleWithOrWithoutSuffixAsKeyInURLTest(string keySegment)
    {
        // Arrange
        var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
        var expectedKeySegment = "EdmDoubleSet(1.0)";

        // Act
        var entries = await TestsHelper.QueryResourceEntriesAsync(keySegment, mimeType);
        var entry = entries.SingleOrDefault();

        // Assert
        Assert.Single(entries);
        Assert.NotNull(entry);
        Assert.Contains(expectedKeySegment, entry.Id.ToString());
        Assert.Contains(expectedKeySegment, entry.EditLink.ToString());
        Assert.Contains(expectedKeySegment, entry.ReadLink.ToString());
    }

    [Theory]
    [InlineData("EdmDecimalSet(1.0)")]
    [InlineData("EdmDecimalSet(1.0M)")]
    [InlineData("EdmDecimalSet(1.0m)")]
    public async Task DecimalWithOrWithoutSuffixAsKeyInURLTest(string keySegment)
    {
        // Arrange
        var mimeType = MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata;
        var expectedKeySegment = "EdmDecimalSet(1)";

        // Act
        var entries = await TestsHelper.QueryResourceEntriesAsync(keySegment, mimeType);
        var entry = entries.SingleOrDefault();

        // Assert
        Assert.Single(entries);
        Assert.NotNull(entry);
        Assert.Contains(expectedKeySegment, entry.Id.ToString());
        Assert.Contains(expectedKeySegment, entry.EditLink.ToString());
        Assert.Contains(expectedKeySegment, entry.ReadLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmInt64Set?$filter=Id ge -1")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmInt64Set?$filter=Id ge -1L")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmInt64Set?$filter=Id ge -1l")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmInt64Set?$filter=Id ge -1")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmInt64Set?$filter=Id ge -1L")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmInt64Set?$filter=Id ge -1l")]
    public async Task LongWithOrWithoutSuffixInFilterInURLTest(string mimeType, string filterQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(filterQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmInt64Set(0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmInt64Set(0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmInt64Set(0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmInt64Set(-1)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmInt64Set(-1)", entry.EditLink.ToString());
        Assert.EndsWith("EdmInt64Set(-1)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith($"{filterQuery}&$skiptoken=Id-0", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmSingleSet?$filter=Id ge -1.0")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmSingleSet?$filter=Id ge -1.0F")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmSingleSet?$filter=Id ge -1.0f")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmSingleSet?$filter=Id ge -1.0")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmSingleSet?$filter=Id ge -1.0F")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmSingleSet?$filter=Id ge -1.0f")]
    public async Task FloatWithOrWithoutSuffixInFilterInURLTest(string mimeType, string filterQuery)
    {
        // Arrange & Act & Assert
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(filterQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmSingleSet(0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmSingleSet(0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmSingleSet(0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmSingleSet(-1)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmSingleSet(-1)", entry.EditLink.ToString());
        Assert.EndsWith("EdmSingleSet(-1)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith($"{filterQuery}&$skiptoken=Id-0", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDoubleSet?$filter=Id ge -1.0")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDoubleSet?$filter=Id ge -1.0D")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDoubleSet?$filter=Id ge -1.0d")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDoubleSet?$filter=Id ge -1.0")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDoubleSet?$filter=Id ge -1.0D")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDoubleSet?$filter=Id ge -1.0d")]
    public async Task DoubleWithOrWithoutSuffixInFilterInURLTest(string mimeType, string filterQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(filterQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDoubleSet(0.0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDoubleSet(0.0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDoubleSet(0.0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDoubleSet(-1.0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDoubleSet(-1.0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDoubleSet(-1.0)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith($"{filterQuery}&$skiptoken=Id-0.0", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDecimalSet?$filter=Id ge -1.0")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDecimalSet?$filter=Id ge -1.0M")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDecimalSet?$filter=Id ge -1.0m")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDecimalSet?$filter=Id ge -1.0")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDecimalSet?$filter=Id ge -1.0M")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDecimalSet?$filter=Id ge -1.0m")]
    public async Task DecimalWithOrWithoutSuffixInFilterInURLTest(string mimeType, string filterQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(filterQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDecimalSet(0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDecimalSet(0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDecimalSet(0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDecimalSet(-1)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDecimalSet(-1)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDecimalSet(-1)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith($"{filterQuery}&$skiptoken=Id-0", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmInt64Set?$skiptoken=-1")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmInt64Set?$skiptoken=-1L")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmInt64Set?$skiptoken=-1l")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmInt64Set?$skiptoken=-1")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmInt64Set?$skiptoken=-1L")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmInt64Set?$skiptoken=-1l")]
    public async Task LongWithOrWithoutSuffixInSkipTokenInURLTest(string mimeType, string skipTokenQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(skipTokenQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmInt64Set(0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmInt64Set(0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmInt64Set(0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmInt64Set(1)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmInt64Set(1)", entry.EditLink.ToString());
        Assert.EndsWith("EdmInt64Set(1)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith("EdmInt64Set?$skiptoken=Id-1", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmSingleSet?$skiptoken=-1.0")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmSingleSet?$skiptoken=-1.0F")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmSingleSet?$skiptoken=-1.0f")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmSingleSet?$skiptoken=-1.0")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmSingleSet?$skiptoken=-1.0F")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmSingleSet?$skiptoken=-1.0f")]
    public async Task FloatWithOrWithoutSuffixInSkipTokenInURLTest(string mimeType, string skipTokenQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(skipTokenQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmSingleSet(0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmSingleSet(0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmSingleSet(0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmSingleSet(1E-45)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmSingleSet(1E-45)", entry.EditLink.ToString());
        Assert.EndsWith("EdmSingleSet(1E-45)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith("EdmSingleSet?$skiptoken=Id-1E-45", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDoubleSet?$skiptoken=-1.0")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDoubleSet?$skiptoken=-1.0D")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDoubleSet?$skiptoken=-1.0d")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDoubleSet?$skiptoken=-1.0")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDoubleSet?$skiptoken=-1.0D")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDoubleSet?$skiptoken=-1.0d")]
    public async Task DoubleWithOrWithoutSuffixInSkipTokenInURLTest(string mimeType, string skipTokenQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(skipTokenQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDoubleSet(0.0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDoubleSet(0.0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDoubleSet(0.0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDoubleSet(5E-324)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDoubleSet(5E-324)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDoubleSet(5E-324)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith("EdmDoubleSet?$skiptoken=Id-5E-324", feed.NextPageLink.ToString());
    }

    [Theory]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDecimalSet?$skiptoken=-1.0")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDecimalSet?$skiptoken=-1.0M")]
    [InlineData(MimeTypeODataParameterFullMetadata, "EdmDecimalSet?$skiptoken=-1.0m")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDecimalSet?$skiptoken=-1.0")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDecimalSet?$skiptoken=-1.0M")]
    [InlineData(MimeTypeODataParameterFullMetadataAndIEEE754Compatible, "EdmDecimalSet?$skiptoken=-1.0m")]
    public async Task DecimalWithSuffixInSkipTokenInURLTest(string mimeType, string skipTokenQuery)
    {
        // Arrange & Act
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(skipTokenQuery, mimeType);

        // Assert
        Assert.Equal(2, resourceEntries.Count);

        var entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDecimalSet(0)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDecimalSet(0)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDecimalSet(0)", entry.ReadLink.ToString());

        entry = resourceEntries.SingleOrDefault(e => e.Id.ToString().EndsWith("EdmDecimalSet(1)"));
        Assert.NotNull(entry);
        Assert.EndsWith("EdmDecimalSet(1)", entry.EditLink.ToString());
        Assert.EndsWith("EdmDecimalSet(1)", entry.ReadLink.ToString());

        Assert.Single(resourceSetEntries);

        var feed = resourceSetEntries.SingleOrDefault();
        Assert.NotNull(feed);
        Assert.EndsWith("EdmDecimalSet?$skiptoken=Id-1", feed.NextPageLink.ToString());
    }

    #region Client Tests

    [Fact]
    public async Task LongInFilterLinqQueryTest()
    {
        // Arrange
        const long int64Id = 1;

        // Act & Assert
        var query = _context.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id >= int64Id) as DataServiceQuery<EdmInt64>;
        Assert.NotNull(query);
        var int64QueryResult = await query.ExecuteAsync();
        Assert.NotNull(int64QueryResult);
        Assert.True(int64QueryResult.Count() > 0, "Expected one or more EdmInt64 entities could be returned ");

        query = (from c in _context.EdmInt64Set where c.Id >= int64Id select c) as DataServiceQuery<EdmInt64>;
        Assert.NotNull(query);
        int64QueryResult = await query.ExecuteAsync();
        Assert.NotNull(int64QueryResult);
        Assert.True(int64QueryResult.Count() > 0, "Expected one or more EdmInt64 entities could be returned ");

        var anotherQuery = _context.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id == int64Id);
        int count = 0;
        foreach (var int64 in anotherQuery)
        {
            count++;
        }

        Assert.True(count == 1, "Expected one or more EdmInt64 entities could be returned ");

        var byKeyInt64QueryResult = await _context.EdmInt64Set.ByKey(new Dictionary<string, object> { { "Id", int64Id } }).GetValueAsync();
        Assert.True(byKeyInt64QueryResult.Id == 1, "Expected one or more EdmInt64 entities could be returned ");
    }

    [Fact]
    public async Task FloatInFilterLinqQueryTest()
    {
        // Arrange
        const float floatId = 1.0f;

        // Act & Assert
        var query = _context.CreateQuery<EdmSingle>("EdmSingleSet").Where(e => e.Id >= floatId) as DataServiceQuery<EdmSingle>;
        Assert.NotNull(query);
        var floatQueryResult = await query.ExecuteAsync();
        Assert.True(floatQueryResult.Count() > 0, "Expected one or more EdmSingle entities could be returned");

        query = (from c in _context.EdmSingleSet where c.Id >= floatId select c) as DataServiceQuery<EdmSingle>;
        Assert.NotNull(query);
        floatQueryResult = await query.ExecuteAsync();
        Assert.True(floatQueryResult.Count() > 0, "Expected one or more EdmSingle entities could be returned");

    }

    [Fact]
    public async Task DoubleInFilterLinqQueryTest()
    {
        // Arrange
        const double doubleId = 1.0;

        // Act & Assert
        var query = _context.CreateQuery<EdmDouble>("EdmDoubleSet").Where(e => e.Id >= doubleId) as DataServiceQuery<EdmDouble>;
        Assert.NotNull(query);
        var doubleQueryResult = await query.ExecuteAsync();
        Assert.True(doubleQueryResult.Count() > 0, "Expected one or more EdmDouble entities could be returned");

        query = (from c in _context.EdmDoubleSet where c.Id >= doubleId select c) as DataServiceQuery<EdmDouble>;
        Assert.NotNull(query);
        doubleQueryResult = await query.ExecuteAsync();
        Assert.True(doubleQueryResult.Count() > 0, "Expected one or more EdmDouble.MaxValue could be returned");
    }

    [Fact]
    public async Task DecimalInFilterLinqQueryTest()
    {
        // Arrange
        const decimal decimalId = 1.0M;

        // Act & Assert
        var query = _context.CreateQuery<EdmDecimal>("EdmDecimalSet").Where(e => e.Id >= decimalId) as DataServiceQuery<EdmDecimal>;
        Assert.NotNull(query);
        var decimalQueryResult = await query.ExecuteAsync();
        Assert.True(decimalQueryResult.Count() > 0, "Expected one or more EdmDecimal entities could be returned");

        query = (from c in _context.EdmDecimalSet where c.Id >= decimalId select c) as DataServiceQuery<EdmDecimal>;
        Assert.NotNull(query);
        decimalQueryResult = await query.ExecuteAsync();
        Assert.True(decimalQueryResult.Count() > 0, "Expected one or more EdmDecimal entities could be returned");
    }

    #endregion

    #region Private methods

    private PrimitiveTypesTestsHelper TestsHelper
    {
        get
        {
            return new PrimitiveTypesTestsHelper(_baseUri, _model, Client);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "primitivekeyvalues/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
