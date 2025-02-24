//---------------------------------------------------------------------
// <copyright file="PrimitiveValueFormatTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys;
using Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveKeys;
using Microsoft.OData.E2E.TestCommon.Common.Server.PrimitiveTypes;
using Microsoft.OData.Edm;
using Xunit;
using EdmDecimal = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmDecimal;
using EdmDouble = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmDouble;
using EdmInt64 = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmInt64;
using EdmSingle = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmSingle;

namespace Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests;

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

    public PrimitiveValueFormatTest(TestWebApplicationFactory<TestsStartup> fixture)
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

    #region Private methods

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "primitivekeyvalues/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }

    #endregion
}
