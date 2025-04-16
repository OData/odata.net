//---------------------------------------------------------------------
// <copyright file="BroadCoverageFilterFunctionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Core.E2E.Tests.BroadCoverageTests;

public class BroadCoverageFilterFunctionTests : EndToEndTestBase<BroadCoverageFilterFunctionTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly IEdmModel _model;
    private readonly ODataMessageReaderSettings _readerSettings;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(BroadCoverageTestsController), typeof(MetadataController));

            services.AddControllers().AddOData(opt =>
                opt.EnableQueryFeatures().AddRouteComponents("odata", BroadCoverageTestsEdmModel.GetEdmModel()));
        }
    }

    public BroadCoverageFilterFunctionTests(TestWebApplicationFactory<TestsStartup> fixture) : base(fixture)
    {
        if (Client.BaseAddress == null)
        {
            throw new ArgumentNullException(nameof(Client.BaseAddress), "Base address cannot be null");
        }

        _baseUri = new Uri(Client.BaseAddress, "odata/");

        _model = BroadCoverageTestsEdmModel.GetEdmModel();

        _readerSettings = new ODataMessageReaderSettings
        {
            BaseUri = _baseUri,
            EnableMessageStreamDisposal = false
        };

        ResetDefaultDataSource();
    }

    private const string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests.";

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterContainsFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=contains(UserName,'v')";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Contains("v", userName.ToString());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterMatchesPatternFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=matchesPattern(UserName,'chum$')";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Contains("chum", userName.ToString());
            Assert.Matches("chum$", userName.ToString());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterEndsWithFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=endswith(UserName,'chum')";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Contains("chum", userName.ToString());
            Assert.EndsWith("chum", userName.ToString());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterStartsWithFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=startswith(UserName,'v')";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Contains("v", userName.ToString());
            Assert.StartsWith("v", userName.ToString());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterLengthFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=length(UserName) eq 12";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Equal(12, userName.ToString().Length);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterIndexOfFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=indexof(UserName,'vincent') eq 0";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        var entry = entries.First();
        var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
        Assert.NotNull(userName);
        Assert.Equal("vincentcalabrese", userName.ToString());
        Assert.Equal(0, userName.ToString().IndexOf("vincent"));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterSubstring1Function(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=substring(UserName,1) eq 'incentcalabrese'";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Contains("incentcalabrese", userName.ToString());
            Assert.Equal("incentcalabrese", userName.ToString().Substring(1));
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterSubstring2Function(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=substring(UserName,1,6) eq 'incent'";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Contains("incent", userName.ToString());
            Assert.Equal("incent", userName.ToString().Substring(1, 6));
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterToLowerFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=tolower(UserName) eq 'vincentcalabrese'";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Equal("vincentcalabrese", userName.ToString());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterToUpperFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=toupper(UserName) eq 'VINCENTCALABRESE'";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Equal("VINCENTCALABRESE", userName.ToString().ToUpper());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterTrimFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=trim(UserName) eq 'vincentcalabrese'";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.Equal("vincentcalabrese", userName.ToString());
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterConcatFunction(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=concat(concat(FirstName,', '), LastName) eq 'Vincent, Calabrese'";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var firstName = (entry.Properties.Single(p => p.Name == "FirstName") as ODataProperty)?.Value;
            var lastName = (entry.Properties.Single(p => p.Name == "LastName") as ODataProperty)?.Value;
            Assert.NotNull(firstName);
            Assert.NotNull(lastName);
            Assert.Equal("Vincent, Calabrese", string.Concat(string.Concat(firstName, ", "), lastName));
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultDateTime_YearFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=year(StartsAt) eq 2014";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var property = (entry.Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
            Assert.NotNull(property);
            var startTime = (DateTimeOffset)property;
            Assert.Equal(2014, startTime.Year);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultDateTime_MonthFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=month(StartsAt) eq 2";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            var property = (entry.Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
            Assert.NotNull(property);
            var startTime = (DateTimeOffset)property;
            Assert.Equal(2, startTime.Month);
        }
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultDateTime_DayFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=day(StartsAt) eq 5";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.NotEmpty(entries);

        var entry = entries.First();
        var property = (entry.Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
        Assert.NotNull(property);
        var startTime = (DateTimeOffset)property;
        Assert.Equal(5, startTime.Day);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultDateTime_HourFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=hour(StartsAt) eq 10";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.NotEmpty(entries);

        var entry = entries.First();
        var property = (entry.Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
        Assert.NotNull(property);
        var startTime = (DateTimeOffset)property;
        Assert.Equal(10, startTime.Hour);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultDateTime_MinuteFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=minute(StartsAt) eq 21";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.NotEmpty(entries);

        var entry = entries.First();
        var property = (entry.Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
        Assert.NotNull(property);
        var startTime = (DateTimeOffset)property;
        Assert.Equal(21, startTime.Minute);
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultDateTime_SecondFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=second(StartsAt) eq 7";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.Equal(2, entries.Count);

        var valueFirst = (entries.First().Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
        Assert.NotNull(valueFirst);
        Assert.Equal(7, ((DateTimeOffset)valueFirst).Second);

        var valueLast = (entries.Last().Properties.Single(p => p.Name == "StartsAt") as ODataProperty)?.Value;
        Assert.NotNull(valueLast);
        Assert.Equal(7, ((DateTimeOffset)valueLast).Second);

    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultMath_RoundFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=round(Budget) eq 2650";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.Single(entries);
        var entry = entries.First();
        var value = (entry.Properties.Single(p => p.Name == "Budget") as ODataProperty)?.Value;
        Assert.NotNull(value);
        Assert.Equal((float)2650.1, value);
        Assert.Equal(2650, Math.Round(2650.1));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultMath_FloorFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=floor(Budget) eq 2650";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.Equal(2, entries.Count);
        var firstValue = (entries.First().Properties.Single(p => p.Name == "Budget") as ODataProperty)?.Value;
        Assert.NotNull(firstValue);
        Assert.Equal((float)2650.65, firstValue);

        var secondValue = (entries.Last().Properties.Single(p => p.Name == "Budget") as ODataProperty)?.Value;
        Assert.NotNull(secondValue);
        Assert.Equal((float)2650.1, secondValue);

        Assert.Equal(2650, Math.Floor(2650.1));
        Assert.Equal(2650, Math.Floor(2650.65));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultMath_CeilingFunction(string mimeType)
    {
        // Arrange
        string uri = "People('russellwhyte')/Trips?$filter=ceiling(Budget) eq 2651";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Trip");

        // Assert
        Assert.Equal(2, entries.Count);
        var firstValue = (entries.First().Properties.Single(p => p.Name == "Budget") as ODataProperty)?.Value;
        Assert.NotNull(firstValue);
        Assert.Equal((float)2650.65, firstValue);

        var secondValue = (entries.Last().Properties.Single(p => p.Name == "Budget") as ODataProperty)?.Value;
        Assert.NotNull(secondValue);
        Assert.Equal((float)2650.1, secondValue);

        Assert.Equal(2651, Math.Ceiling(2650.1));
        Assert.Equal(2651, Math.Ceiling(2650.65));
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultType_IsOfFunction_WithPrimitiveType(string mimeType)
    {
        // Arrange
        string uri = "People?$filter=isof(UserName, Edm.String)";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.Equal(20, entries.Count);
        var userName = (entries.First().Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
        Assert.Equal(typeof(String), userName?.GetType());

        Assert.All(entries, entry =>
        {
            var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
            Assert.NotNull(userName);
            Assert.IsType<string>(userName);
        });
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultType_IsOfFunction_WithEnum(string mimeType)
    {
        // Arrange
        string uri = $"People?$filter=isof(Gender,'Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests.PersonGender')";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "Person");

        // Assert
        Assert.Equal(20, entries.Count);
        var gender = (entries.First().Properties.Single(p => p.Name == "Gender") as ODataProperty)?.Value;
        Assert.Equal(typeof(ODataEnumValue), gender?.GetType());
        Assert.Equal("Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests.PersonGender", (gender as ODataEnumValue).TypeName);

        Assert.All(entries, entry =>
        {
            var gender = (entries.First().Properties.Single(p => p.Name == "Gender") as ODataProperty)?.Value;
            Assert.Equal(typeof(ODataEnumValue), gender?.GetType());
            Assert.Equal("Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests.PersonGender", (gender as ODataEnumValue).TypeName);
        });
    }

    [Theory]
    [InlineData(MimeTypeODataParameterMinimalMetadata)]
    [InlineData(MimeTypeODataParameterFullMetadata)]
    public async Task FilterDefaultType_IsOfFunction_WithEntityType(string mimeType)
    {
        // Arrange
        string uri = $"People?$filter=isof('Microsoft.OData.E2E.TestCommon.Common.Server.BroadCoverageTests.SpecialPerson')";

        // Act
        List<ODataResource> entries = await QueryFeedAsync(uri, mimeType, "SpecialPerson");

        // Assert
        Assert.Single(entries);
        var entry = entries.First();
        var userName = (entry.Properties.Single(p => p.Name == "UserName") as ODataProperty)?.Value;
        var specialID = (entry.Properties.Single(p => p.Name == "SpecialID") as ODataProperty)?.Value;
        Assert.Equal("thespecialperson", userName);
        Assert.Equal("SP-123456789", specialID);
    }

    #region Private

    private async Task<List<ODataResource>> QueryFeedAsync(string requestUri, string mimeType, params string[] resourceTypeNames)
    {
        var entries = new List<ODataResource>();

        var requestMessage = new TestHttpClientRequestMessage(new Uri(_baseUri.AbsoluteUri + requestUri, UriKind.Absolute), Client);
        requestMessage.SetHeader("Accept", mimeType);

        var responseMessage = await requestMessage.GetResponseAsync();
        Assert.Equal(200, responseMessage.StatusCode);

        using (var messageReader = new ODataMessageReader(responseMessage, _readerSettings, _model))
        {
            var reader = messageReader.CreateODataResourceSetReader();
            ODataResourceSet? feed = null;
            while (reader.Read())
            {
                if (reader.State == ODataReaderState.ResourceEnd && reader.Item is ODataResource entry)
                {
                    if (entry != null && resourceTypeNames.Any(r => entry.TypeName.EndsWith(r)))
                    {
                        entries.Add(entry);
                    }
                }
                else if (reader.State == ODataReaderState.ResourceSetEnd)
                {
                    feed = reader.Item as ODataResourceSet;
                }
            }

            Assert.NotNull(feed as ODataResourceSet);
            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        return entries;
    }

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "broadcoveragetests/Default.ResetDefaultDataSource", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(actionUri, Client);
        requestMessage.Method = "POST";

        var responseMessage = requestMessage.GetResponseAsync().Result;

        Assert.Equal(200, responseMessage.StatusCode);
    }

    #endregion
}
