//---------------------------------------------------------------------
// <copyright file="PrimitiveValueFormatTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.TestCommon.Helpers;
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

    public static IEnumerable<object[]> MimeTypesData
    {
        get
        {
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
            yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata + MimeTypes.ODataParameterIEEE754Compatible };
        }
    }

    public string DataPattern = "(" + @"[\+-]?\d\.?\d*E?[\+-]?\d*" + "|" + @"INF|-INF|NaN" + ")";

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task LongWithoutSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmInt64Set(1)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmInt64Set(1)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task LongWithSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmSingleSet(1)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmSingleSet(1L)", mimeType, expectedKeySegment);
        await PrimitiveValueAsKeyInURLAsync("EdmSingleSet(1l)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FloatWithoutSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmSingleSet(1)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmSingleSet(1.0)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FloatWithSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmSingleSet(1)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmSingleSet(1.0F)", mimeType, expectedKeySegment);
        await PrimitiveValueAsKeyInURLAsync("EdmSingleSet(1.0f)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DoubleWithoutSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmDoubleSet(1.0)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmDoubleSet(1.0)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DoubleWithSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmDoubleSet(1.0)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmDoubleSet(1.0D)", mimeType, expectedKeySegment);
        await PrimitiveValueAsKeyInURLAsync("EdmDoubleSet(1.0d)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DecimalWithoutSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmDecimalSet(1)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmDecimalSet(1.0)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DecimalWithSuffixAsKeyInURLTest(string mimeType)
    {
        // Arrange
        var expectedKeySegment = "EdmDecimalSet(1)";

        // Act & Assert
        await PrimitiveValueAsKeyInURLAsync("EdmDecimalSet(1.0M)", mimeType, expectedKeySegment);
        await PrimitiveValueAsKeyInURLAsync("EdmDecimalSet(1.0m)", mimeType, expectedKeySegment);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task LongWithoutSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmInt64Set?$filter=Id ge -1", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task LongWithSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmInt64Set?$filter=Id ge -1L", mimeType);
        await PrimitiveValueInFilterInURLAsync("EdmInt64Set?$filter=Id ge -1l", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FloatWithoutSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmSingleSet?$filter=Id ge -1.0", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FloatWithSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmSingleSet?$filter=Id ge -1.0F", mimeType);
        await PrimitiveValueInFilterInURLAsync("EdmSingleSet?$filter=Id ge -1.0f", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DoubleWithoutSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmDoubleSet?$filter=Id ge -1.0", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DoubleWithSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmDoubleSet?$filter=Id ge -1.0D", mimeType);
        await PrimitiveValueInFilterInURLAsync("EdmDoubleSet?$filter=Id ge -1.0d", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DecimalWithoutSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmDecimalSet?$filter=Id ge -1.0", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DecimalWithSuffixInFilterInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInFilterInURLAsync("EdmDecimalSet?$filter=Id ge -1.0M", mimeType);
        await PrimitiveValueInFilterInURLAsync("EdmDecimalSet?$filter=Id ge -1.0m", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task LongWithoutSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmInt64Set?$skiptoken=-1", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task LongWithSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmInt64Set?$skiptoken=-1L", mimeType);
        await PrimitiveValueInSkipTokenInURLAsync("EdmInt64Set?$skiptoken=-1l", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FloatWithoutSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmSingleSet?$skiptoken=-1.0", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task FloatWithSuffixInSkipTokenInURLTest(string mimeType)
    {
        await PrimitiveValueInSkipTokenInURLAsync("EdmSingleSet?$skiptoken=-1.0F", mimeType);
        await PrimitiveValueInSkipTokenInURLAsync("EdmSingleSet?$skiptoken=-1.0f", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DoubleWithoutSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmDoubleSet?$skiptoken=-1.0", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DoubleWithSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmDoubleSet?$skiptoken=-1.0D", mimeType);
        await PrimitiveValueInSkipTokenInURLAsync("EdmDoubleSet?$skiptoken=-1.0d", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DecimalWithoutSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmDecimalSet?$skiptoken=-1.0", mimeType);
    }

    [Theory]
    [MemberData(nameof(MimeTypesData))]
    public async Task DecimalWithSuffixInSkipTokenInURLTest(string mimeType)
    {
        // Arrange & Act & Assert
        await PrimitiveValueInSkipTokenInURLAsync("EdmDecimalSet?$skiptoken=-1.0M", mimeType);
        await PrimitiveValueInSkipTokenInURLAsync("EdmDecimalSet?$skiptoken=-1.0m", mimeType);
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
        Assert.True(floatQueryResult.Count() > 0, "Expected one or more EdmSingle entities could be returned ");

        query = (from c in _context.EdmSingleSet where c.Id >= floatId select c) as DataServiceQuery<EdmSingle>;
        Assert.NotNull(query);
        floatQueryResult = await query.ExecuteAsync();
        Assert.True(floatQueryResult.Count() > 0, "Expected one or more EdmSingle entities could be returned ");

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
        Assert.True(doubleQueryResult.Count() > 0, "Expected one or more EdmDouble entities could be returned ");

        query = (from c in _context.EdmDoubleSet where c.Id >= doubleId select c) as DataServiceQuery<EdmDouble>;
        Assert.NotNull(query);
        doubleQueryResult = await query.ExecuteAsync();
        Assert.True(doubleQueryResult.Count() > 0, "Expected one or more EdmDouble.MaxValue could be returned ");
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
        Assert.True(decimalQueryResult.Count() > 0, "Expected one or more EdmDecimal entities could be returned ");

        query = (from c in _context.EdmDecimalSet where c.Id >= decimalId select c) as DataServiceQuery<EdmDecimal>;
        Assert.NotNull(query);
        decimalQueryResult = await query.ExecuteAsync();
        Assert.True(decimalQueryResult.Count() > 0, "Expected one or more EdmDecimal entities could be returned ");
    }

    #endregion

    #region Private methods

    private async Task PrimitiveValueAsKeyInURLAsync(string keySegment, string mimeType, string expectedKeySegment)
    {
        var entries = await TestsHelper.QueryResourceEntriesAsync(keySegment, mimeType);

        foreach (var entry in entries)
        {
            Assert.Contains(expectedKeySegment, entry.Id.ToString());

            if (mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata))
            {
                Assert.Contains(expectedKeySegment, entry.EditLink.ToString());
                Assert.Contains(expectedKeySegment, entry.ReadLink.ToString());
            }
        }
    }

    private async Task PrimitiveValueInFilterInURLAsync(string filterQuery, string mimeType)
    {
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(filterQuery, mimeType);

        var pattern = string.Concat(filterQuery.AsSpan(0, filterQuery.IndexOf('?')), @"\(", DataPattern, @"\)$");
        var rgx = new Regex(pattern, RegexOptions.IgnoreCase);
        foreach (var entry in resourceEntries)
        {
            Assert.True(rgx.Match(entry.Id.ToString()).Success);

            if (mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata))
            {
                Assert.True(rgx.Match(entry.EditLink.ToString()).Success);
                Assert.True(rgx.Match(entry.ReadLink.ToString()).Success);
            }
        }

        pattern = string.Concat(filterQuery.AsSpan(0, filterQuery.IndexOf('?')), @"\?\$filter=Id\sge\s", DataPattern, @"(L|F|D|M)?&\$skiptoken=Id-.*$");
        rgx = new Regex(pattern, RegexOptions.IgnoreCase);
        foreach (var feed in resourceSetEntries)
        {
            Assert.True(rgx.Match(feed.NextPageLink.ToString()).Success);
        }
    }

    private async Task PrimitiveValueInSkipTokenInURLAsync(string skipTokenQuery, string mimeType)
    {
        var (resourceEntries, resourceSetEntries) = await TestsHelper.QueryResourceAndResourceSetsAsync(skipTokenQuery, mimeType);

        var pattern = string.Concat(skipTokenQuery.AsSpan(0, skipTokenQuery.IndexOf('?')), @"\(", DataPattern, @"\)$");
        var rgx = new Regex(pattern, RegexOptions.IgnoreCase);

        foreach (var entry in resourceEntries)
        {
            Assert.True(rgx.Match(entry.Id.ToString()).Success);

            if (mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata))
            {
                Assert.True(rgx.Match(entry.EditLink.ToString()).Success);
                Assert.True(rgx.Match(entry.ReadLink.ToString()).Success);
            }
        }

        pattern = string.Concat(skipTokenQuery.AsSpan(0, skipTokenQuery.IndexOf('?')), @"\?\$skiptoken=Id-", DataPattern, "$");
        rgx = new Regex(pattern, RegexOptions.IgnoreCase);

        foreach (var feed in resourceSetEntries)
        {
            Assert.True(rgx.Match(feed.NextPageLink.ToString()).Success);
        }
    }

    private ODataMessageReaderTestsHelper TestsHelper
    {
        get
        {
            return new ODataMessageReaderTestsHelper(_baseUri, _model, Client);
        }
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "primitivekeyvalues/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
