//-----------------------------------------------------------------------------
// <copyright file="DynamicPropertiesTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.DynamicProperties;
using Xunit;
using Account = Microsoft.OData.E2E.TestCommon.Common.Client.Default.Account;
using AccountInfo = Microsoft.OData.E2E.TestCommon.Common.Client.Default.AccountInfo;
using GiftCard = Microsoft.OData.E2E.TestCommon.Common.Client.Default.GiftCard;

namespace Microsoft.OData.Client.E2E.Tests.DynamicProperties;

public class DynamicPropertiesTests : EndToEndTestBase<DynamicPropertiesTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly Container _context;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(DynamicPropertiesController), typeof(MetadataController));

            services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
        }
    }

    public DynamicPropertiesTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _context = new Container(_baseUri)
        {
            HttpClientFactory = HttpClientFactory
        };

        ResetDefaultDataSource();
    }

    [Fact]
    public void ShouldAddAndQueryDynamicProperties()
    {
        // Arrange
        var account = new Account()
        {
            AccountID = 2222,
            CountryRegion = "GB",
            AccountInfo = new AccountInfo()
            {
                FirstName = "James",
                LastName = "Bunder",
                DynamicProperties = new Dictionary<string, object>()
                {
                    { "IntNum", 123 },
                    { "DoubleNum", 123.45 },
                    { "FloatNum", 123.56f },
                    { "DecimalNum", 123.67m },
                    { "LongNum", 123456789L },
                    { "BigIntNum", 1234567890123456789L },
                    { "ShortNum", 12345 },
                    { "ByteNum", (byte)123 },
                    { "SByteNum", (sbyte)-123 },
                    { "String", "Sample String" },
                    { "Char", '/' },
                    { "Boolean", false },
                    { "Guid", Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6") },
                    { "DateTime", new DateTime(2023, 10, 25) },
                    { "DateTimeOffset", new DateTimeOffset(new DateTime(2023, 10, 25)) },
                    { "Date", new DateTime(2023, 10, 25).Date },
                    { "DateOnly", new DateOnly(2023, 10, 25) },
                    { "TimeOfDay", new DateTime(2023, 10, 25).TimeOfDay },
                    { "TimeOnly", new TimeOnly(2, 50, 20) },
                    { "Duration", new TimeSpan(2, 50, 20) },
                    { "Binary", new List<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 } },
                    { "EnumValueString", DaysOfWeekEnum.Tuesday.ToString() },
                    { "EnumValueInt", (long)DaysOfWeekEnum.Friday },
                    { "CollectionOfInt", new List<long> { 56, 100L, 456, 90L } }
                }
            },
            MyGiftCard = new GiftCard()
            {
                GiftCardID = 302,
                GiftCardNO = "BBA12BB",
                Amount = 200,
                ExperationDate = new DateTimeOffset(new DateTime(2014, 12, 30))
            }
        };

        _context.AddObject("Accounts", account);
        _context.SaveChanges();

        // Act
        var queried = _context.CreateQuery<Account>("Accounts")
            .Where(c => c.AccountID == 2222)
            .Single();

        // Assert
        Assert.Equal(12345, queried.AccountInfo.DynamicProperties["ShortNum"]);
        Assert.Equal(123456789L, queried.AccountInfo.DynamicProperties["LongNum"]);
        Assert.Equal(123.45, Convert.ToDouble(queried.AccountInfo.DynamicProperties["DoubleNum"]));
        Assert.Equal(123.56f, queried.AccountInfo.DynamicProperties["FloatNum"]);
        Assert.Equal(123.67m, queried.AccountInfo.DynamicProperties["DecimalNum"]);
        Assert.Equal(1234567890123456789L, queried.AccountInfo.DynamicProperties["BigIntNum"]);
        Assert.Equal(123, queried.AccountInfo.DynamicProperties["IntNum"]);

        Assert.Equal((byte)123, queried.AccountInfo.DynamicProperties["ByteNum"]);
        Assert.Equal((sbyte)-123, queried.AccountInfo.DynamicProperties["SByteNum"]);

        Assert.Equal("Sample String", queried.AccountInfo.DynamicProperties["String"]);
        Assert.Equal("/", queried.AccountInfo.DynamicProperties["Char"]);

        Assert.False((bool)queried.AccountInfo.DynamicProperties["Boolean"]);
        Assert.Equal(Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6"), queried.AccountInfo.DynamicProperties["Guid"]);

        Assert.Equal(DaysOfWeekEnum.Tuesday.ToString(), queried.AccountInfo.DynamicProperties["EnumValueString"]);
        Assert.Equal((long)DaysOfWeekEnum.Friday, queried.AccountInfo.DynamicProperties["EnumValueInt"]);

        Assert.Equal(new DateTime(2023, 10, 25).Date, Convert.ToDateTime(queried.AccountInfo.DynamicProperties["DateTime"]).Date);
        Assert.Equal(new DateTimeOffset(new DateTime(2023, 10, 25)), (DateTimeOffset)queried.AccountInfo.DynamicProperties["DateTimeOffset"]);
        Assert.Equal(new DateTime(2023, 10, 25).Date, (Convert.ToDateTime(queried.AccountInfo.DynamicProperties["Date"])).Date);
        Assert.Equal(new DateTime(2023, 10, 25).TimeOfDay, (TimeSpan)queried.AccountInfo.DynamicProperties["TimeOfDay"]);
        Assert.Equal(new Edm.TimeOfDay(new TimeOnly(2, 50, 20).Ticks), queried.AccountInfo.DynamicProperties["TimeOnly"]);
        Assert.Equal(new Edm.Date(2023, 10, 25), queried.AccountInfo.DynamicProperties["DateOnly"]);
        Assert.Equal(new TimeSpan(2, 50, 20), queried.AccountInfo.DynamicProperties["Duration"]);

        Assert.Equal(new Collection<byte> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, queried.AccountInfo.DynamicProperties["Binary"]);
        Assert.Equal(new Collection<long> { 56, 100L, 456, 90L }, queried.AccountInfo.DynamicProperties["CollectionOfInt"]);
    }

    [Fact]
    public void ShouldUpdateDynamicProperties()
    {
        // Arrange
        var account = new Account()
        {
            AccountID = 3333,
            CountryRegion = "GB",
            AccountInfo = new AccountInfo()
            {
                FirstName = "James",
                LastName = "Bunder",
                DynamicProperties = new Dictionary<string, object>()
                {
                    { "IntNum", 123 },
                    { "DoubleNum", 123.45 },
                    { "FloatNum", 123.45f },
                    { "DecimalNum", 123.45m },
                    { "LongNum", 123456789L },
                    { "ShortNum", 12345 },
                    { "ByteNum", (byte)123 },
                    { "SByteNum", (sbyte)-123 },
                    { "String", "Sample String" },
                    { "Char", '/' },
                    { "Boolean", false },
                    { "Guid", Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6") },
                    { "DateTime", new DateTime(2023, 10, 25) },
                    { "DateTimeOffset", new DateTimeOffset(new DateTime(2023, 10, 25)) },
                    { "Date", new DateTime(2023, 10, 25).Date },
                    { "DateOnly", new DateOnly(2023, 10, 25) },
                    { "TimeOfDay", new DateTime(2023, 10, 25).TimeOfDay },
                    { "TimeOnly", new TimeOnly(2, 50, 20) },
                    { "Duration", new TimeSpan(2, 50, 20) },
                    { "Binary", new List<byte> { 1, 2, 3, 4, 5, 6, 7 } },
                    { "CollectionOfInt", new List<long> { 456, 90L } }
                }
            }
        };

        _context.AddObject("Accounts", account);
        _context.SaveChanges();

        // Update dynamic properties
        var updatedProperties = new Dictionary<string, object>
        {
            { "ShortNum", 98756 },
            { "LongNum", 9876543210L },
            { "DoubleNum", -999.99 },
            { "FloatNum", 999.99f },
            { "IntNum", 999 },
            { "String", "Updated String" },
            { "Char", '!' },
            { "Boolean", true },
            { "Guid", Guid.Parse("12345678-1234-1234-1234-123456789012") },
            { "DateTime", new DateTime(2023, 11, 1) },
            { "DateTimeOffset", new DateTimeOffset(new DateTime(2023, 11, 1)) },
            { "Date", new DateTime(2023, 11, 1).Date },
            { "DateOnly", new DateOnly(2023, 11, 1) },
            { "TimeOfDay", new DateTime(2023, 11, 1).TimeOfDay },
            { "TimeOnly", new TimeOnly(3, 30, 15) },
            { "Duration", new TimeSpan(3, 30, 15) },
            { "Binary", new List<byte> { 8, 7, 6, 5, 4, 3, 2 } },
            { "CollectionOfInt", new List<long> { 1000L, 2000L } }
        };

        // Act
        var queried = _context.CreateQuery<Account>("Accounts")
            .Where(c => c.AccountID == 3333)
            .Single();

        queried.AccountInfo.DynamicProperties = updatedProperties;
        _context.UpdateObject(queried);
        _context.SaveChanges();

        // Assert
        Assert.Equal(98756, queried.AccountInfo.DynamicProperties["ShortNum"]);
        Assert.Equal(9876543210L, queried.AccountInfo.DynamicProperties["LongNum"]);
        Assert.Equal(-999.99, queried.AccountInfo.DynamicProperties["DoubleNum"]);
        Assert.Equal(999.99f, queried.AccountInfo.DynamicProperties["FloatNum"]);
        Assert.Equal(999, queried.AccountInfo.DynamicProperties["IntNum"]);
        Assert.Equal("Updated String", queried.AccountInfo.DynamicProperties["String"]);
        Assert.Equal('!', queried.AccountInfo.DynamicProperties["Char"]);
        Assert.True((bool)queried.AccountInfo.DynamicProperties["Boolean"]);
        Assert.Equal(Guid.Parse("12345678-1234-1234-1234-123456789012"), queried.AccountInfo.DynamicProperties["Guid"]);
        Assert.Equal(new DateTime(2023, 11, 1).Date, Convert.ToDateTime(queried.AccountInfo.DynamicProperties["DateTime"]).Date);
        Assert.Equal(new DateTimeOffset(new DateTime(2023, 11, 1)), (DateTimeOffset)queried.AccountInfo.DynamicProperties["DateTimeOffset"]);
        Assert.Equal(new DateTime(2023, 11, 1).Date, (Convert.ToDateTime(queried.AccountInfo.DynamicProperties["Date"])).Date);
        Assert.Equal(new DateOnly(2023, 11, 1), queried.AccountInfo.DynamicProperties["DateOnly"]);
        Assert.Equal(new TimeOnly(3, 30, 15), queried.AccountInfo.DynamicProperties["TimeOnly"]);
        Assert.Equal(new TimeSpan(3, 30, 15), queried.AccountInfo.DynamicProperties["Duration"]);
    }

    [Fact]
    public void ShouldRemoveDynamicProperties()
    {
        // Arrange
        var account = new Account()
        {
            AccountID = 4444,
            CountryRegion = "GB",
            AccountInfo = new AccountInfo()
            {
                FirstName = "James",
                LastName = "Bunder",
                DynamicProperties = new Dictionary<string, object>()
                {
                    { "IntNum", 12345 },
                    { "DoubleNum", 123.45 },
                    { "FloatNum", 123.45f },
                    { "DecimalNum", 123.45m },
                    { "LongNum", 123456789L },
                    { "ShortNum", 12345 },
                    { "ByteNum", (byte)123 },
                    { "SByteNum", (sbyte)-123 },
                    { "String", "Sample String" },
                    { "Char", '/' },
                    { "Boolean", false },
                    { "Guid", Guid.Parse("44677ffa-552a-40c4-ab4f-a6d3869d1cc6") },
                    { "DateTime", new DateTime(2023, 10, 25) },
                    { "DateTimeOffset", new DateTimeOffset(new DateTime(2023, 10, 25)) },
                    { "Date", new DateTime(2023, 10, 25).Date },
                    { "DateOnly", new DateOnly(2023, 10, 25) },
                    { "TimeOfDay", new DateTime(2023, 10, 25).TimeOfDay },
                    { "TimeOnly", new TimeOnly(2, 50, 20) },
                    { "Duration", new TimeSpan(2, 50, 20) }
                }
            }
        };

        _context.AddObject("Accounts", account);
        _context.SaveChanges();

        // Act
        var queried = _context.CreateQuery<Account>("Accounts")
            .Where(c => c.AccountID == 4444)
            .Single();

        // Remove some dynamic properties
        queried.AccountInfo.DynamicProperties.Remove("ShortNum");
        queried.AccountInfo.DynamicProperties.Remove("LongNum");
        queried.AccountInfo.DynamicProperties.Remove("ByteNum");
        queried.AccountInfo.DynamicProperties.Remove("TimeOfDay");
        queried.AccountInfo.DynamicProperties.Remove("Char");

        _context.UpdateObject(queried);
        _context.SaveChanges();

        // Assert
        var updated = _context.CreateQuery<Account>("Accounts")
            .Where(c => c.AccountID == 4444)
            .Single();

        Assert.False(updated.AccountInfo.DynamicProperties.ContainsKey("ShortNum"));
        Assert.False(updated.AccountInfo.DynamicProperties.ContainsKey("LongNum"));
        Assert.False(updated.AccountInfo.DynamicProperties.ContainsKey("ByteNum"));
        Assert.False(updated.AccountInfo.DynamicProperties.ContainsKey("TimeOfDay"));
        Assert.False(updated.AccountInfo.DynamicProperties.ContainsKey("Char"));

        Assert.Equal(12345, queried.AccountInfo.DynamicProperties["IntNum"]);
        Assert.Equal(123.45m, queried.AccountInfo.DynamicProperties["DecimalNum"]);
        Assert.Equal(123.45f, queried.AccountInfo.DynamicProperties["FloatNum"]);
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "dynamicpropertiestests/Default.ResetDefaultDataSource", UriKind.Absolute);
        _context.Execute(actionUri, "POST");
    }
}

public enum DaysOfWeekEnum
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6
}
