//-----------------------------------------------------------------------------
// <copyright file="UntypedTypesTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.UntypedTypes;
using Microsoft.OData.Edm;

namespace Microsoft.OData.Client.E2E.Tests.UntypedTypesTests;

public class UntypedTypesTests : EndToEndTestBase<UntypedTypesTests.TestsStartup>
{
    private readonly Uri _baseUri;
    private readonly IEdmModel _model;

    public class TestsStartup : TestStartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureControllers(typeof(UntypedTypesController), typeof(MetadataController));

            services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                .AddRouteComponents("odata", UntypedTypesEdmModel.GetEdmModel()));
        }
    }

    public UntypedTypesTests(TestWebApplicationFactory<TestsStartup> fixture)
        : base(fixture)
    {
        _baseUri = new Uri(Client.BaseAddress, "odata/");
        _model = UntypedTypesEdmModel.GetEdmModel();

        ResetDefaultDataSource();
    }

    [Fact]
    public async Task ShouldQueryEntitiesWithUntypedProperties()
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Customers", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();
        var responseContent = await ReadAsStringAsync(responseMessage);

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);
        Assert.Equal(
            """
                        {
              "@odata.context": "http://localhost/odata/$metadata#Customers",
              "value": [
                {
                  "Id": 1,
                  "Name": "Customer 1",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 3.40282347E+38,
                  "UntypedList": [
                    "String in list",
                    123,
                    true,
                    45.67
                  ]
                },
                {
                  "Id": 2,
                  "Name": "Customer 2",
                  "UntypedProperty@odata.type": "#Int32",
                  "UntypedProperty": -456,
                  "UntypedList": [
                    "Another string",
                    789,
                    false,
                    12.34
                  ]
                },
                {
                  "Id": 3,
                  "Name": "Customer 3",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 6.02214076E+23,
                  "UntypedList": [
                    "Yet another string",
                    101112,
                    true,
                    56.78
                  ]
                },
                {
                  "Id": 4,
                  "Name": "Customer 4",
                  "UntypedProperty": null,
                  "UntypedList": []
                },
                {
                  "Id": 5,
                  "Name": "Customer 5",
                  "UntypedProperty@odata.type": "#Boolean",
                  "UntypedProperty": true,
                  "UntypedList": [
                    "Final string",
                    131415,
                    false,
                    90.12
                  ]
                },
                {
                  "Id": 6,
                  "Name": "Customer 6",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 78.9,
                  "UntypedList": [
                    "Extra string",
                    161718,
                    true,
                    34.56
                  ]
                },
                {
                  "Id": 7,
                  "Name": "Customer 7",
                  "UntypedProperty": [
                    "List as untyped property",
                    192021,
                    false,
                    78.9
                  ],
                  "UntypedList": [
                    "More strings",
                    222324,
                    true,
                    12.34
                  ]
                },
                {
                  "Id": 8,
                  "Name": "Small integer",
                  "UntypedProperty@odata.type": "#Int32",
                  "UntypedProperty": 123,
                  "UntypedList": [
                    "123",
                    123,
                    "Small integer"
                  ]
                },
                {
                  "Id": 9,
                  "Name": "Negative integer",
                  "UntypedProperty@odata.type": "#Int32",
                  "UntypedProperty": -42,
                  "UntypedList": [
                    "-42",
                    -42,
                    "Negative integer"
                  ]
                },
                {
                  "Id": 10,
                  "Name": "Max int32",
                  "UntypedProperty@odata.type": "#Int32",
                  "UntypedProperty": 2147483647,
                  "UntypedList": [
                    "2147483647",
                    2147483647,
                    "Max int32"
                  ]
                },
                {
                  "Id": 11,
                  "Name": "Beyond int32",
                  "UntypedProperty@odata.type": "#Int64",
                  "UntypedProperty": 2147483648,
                  "UntypedList": [
                    "2147483648",
                    2147483648,
                    "Beyond int32"
                  ]
                },
                {
                  "Id": 12,
                  "Name": "Max int64",
                  "UntypedProperty@odata.type": "#Int64",
                  "UntypedProperty": 9223372036854775807,
                  "UntypedList": [
                    "9223372036854775807",
                    9223372036854775807,
                    "Max int64"
                  ]
                },
                {
                  "Id": 13,
                  "Name": "Beyond int64",
                  "UntypedProperty@odata.type": "#Decimal",
                  "UntypedProperty": 9223372036854775808,
                  "UntypedList": [
                    "9223372036854775808",
                    "Beyond int64"
                  ]
                },
                {
                  "Id": 14,
                  "Name": "Simple decimal",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 123.456,
                  "UntypedList": [
                    "123.456",
                    123.456,
                    "Simple decimal"
                  ]
                },
                {
                  "Id": 15,
                  "Name": "Very small decimal",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 1E-07,
                  "UntypedList": [
                    "0.0000001",
                    0.0000001,
                    "Very small decimal"
                  ]
                },
                {
                  "Id": 16,
                  "Name": "High precision (pi)",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 3.141592653589793,
                  "UntypedList": [
                    "3.14159265358979323846",
                    3.141592653589793,
                    "High precision (pi)"
                  ]
                },
                {
                  "Id": 17,
                  "Name": "1. Large decimal with high precision",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 123456789012345.12,
                  "UntypedList": [
                    "123456789012345.123456789012345",
                    123456789012345.12,
                    "1. Large decimal with high precision"
                  ]
                },
                {
                  "Id": 18,
                  "Name": "2. Large decimal with high precision",
                  "UntypedProperty@odata.type": "#Decimal",
                  "UntypedProperty": 123456789123456789.12345678935,
                  "UntypedList": [
                    "123456789123456789.12345678935",
                    123456789123456789.12345678935,
                    "2. Large decimal with high precision"
                  ]
                },
                {
                  "Id": 19,
                  "Name": "3. Large decimal with high precision",
                  "UntypedProperty@odata.type": "#Decimal",
                  "UntypedProperty": 1234567891234567891234.1234568,
                  "UntypedList": [
                    "1234567891234567891234.12345678935546576",
                    "3. Large decimal with high precision"
                  ]
                },
                {
                  "Id": 20,
                  "Name": "Positive exponent",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 123400.0,
                  "UntypedList": [
                    "1.234e+5",
                    123400.0,
                    "Positive exponent"
                  ]
                },
                {
                  "Id": 21,
                  "Name": "Negative exponent",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 1.234E-05,
                  "UntypedList": [
                    "1.234e-5",
                    1.234E-05,
                    "Negative exponent"
                  ]
                },
                {
                  "Id": 22,
                  "Name": "Avogadro's number",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 6.02214076E+23,
                  "UntypedList": [
                    "6.02214076e+23",
                    6.02214076E+23,
                    "Avogadro's number"
                  ]
                },
                {
                  "Id": 23,
                  "Name": "Electron mass",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 9.1093837E-31,
                  "UntypedList": [
                    "9.1093837e-31",
                    9.1093837E-31,
                    "Electron mass"
                  ]
                },
                {
                  "Id": 24,
                  "Name": "Zero",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 0.0,
                  "UntypedList": [
                    "0.0",
                    0.0,
                    "Zero"
                  ]
                },
                {
                  "Id": 25,
                  "Name": "Negative zero",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": -0.0,
                  "UntypedList": [
                    "-0.0",
                    -0.0,
                    "Negative zero"
                  ]
                },
                {
                  "Id": 26,
                  "Name": "Max decimal",
                  "UntypedProperty@odata.type": "#Decimal",
                  "UntypedProperty": 79228162514264337593543950335,
                  "UntypedList": [
                    "79228162514264337593543950335",
                    79228162514264337593543950335,
                    "Max decimal"
                  ]
                },
                {
                  "Id": 27,
                  "Name": "Max double",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 1.7976931348623157E+308,
                  "UntypedList": [
                    "1.7976931348623157E+308",
                    1.7976931348623157E+308,
                    "Max double"
                  ]
                },
                {
                  "Id": 28,
                  "Name": "Max single",
                  "UntypedProperty@odata.type": "#Double",
                  "UntypedProperty": 3.40282347E+38,
                  "UntypedList": [
                    "3.40282347E+38",
                    3.40282347E+38,
                    "Max single"
                  ]
                }
              ]
            }
            """, responseContent);
    }

    [Fact]
    public async Task ShouldQuerySingleEntityWithUntypedProperties()
    {
        // Arrange
        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Customers(26)", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var responseMessage = await requestMessage.GetResponseAsync();
        var responseContent = await ReadAsStringAsync(responseMessage);

        // Assert
        Assert.Equal(200, responseMessage.StatusCode);
        Assert.Equal(
            """
            {
              "@odata.context": "http://localhost/odata/$metadata#Customers/$entity",
              "Id": 26,
              "Name": "Max decimal",
              "UntypedProperty@odata.type": "#Decimal",
              "UntypedProperty": 79228162514264337593543950335,
              "UntypedList": [
                "79228162514264337593543950335",
                79228162514264337593543950335,
                "Max decimal"
              ]
            }
            """, responseContent);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task QueryEntryWithUntypedTypes_NumericValueTypeIsDouble_WhereReadUntypedNumericAsDecimalFlagIsSetOrNot(bool readUntypedNumericAsDecimalFlagIsSetOrDefault)
    {
        // Arrange
        var readerSettings = new ODataMessageReaderSettings() 
        { 
            BaseUri = _baseUri, 
            EnablePrimitiveTypeConversion = true, 
            EnableMessageStreamDisposal = false,
            ShouldIncludeAnnotation = (annotationName) => true,
            Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
        };

        if (!readUntypedNumericAsDecimalFlagIsSetOrDefault)
        {
            readerSettings.LibraryCompatibility = ~ODataLibraryCompatibility.ReadUntypedNumericAsDecimal; // Make sure ReadUntypedNumericAsDecimal is not set
        }

        var requestUrl = new Uri(_baseUri.AbsoluteUri + "Customers(17)", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(requestUrl, Client);
        requestMessage.SetHeader("Accept", MimeTypeODataParameterMinimalMetadata);

        // Act
        var queryResponseMessage = await requestMessage.GetResponseAsync();

        // Assert
        Assert.Equal(200, queryResponseMessage.StatusCode);

        ODataResource? entry = null;
        using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, _model))
        {
            var reader = await messageReader.CreateODataResourceReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.State == ODataReaderState.ResourceStart)
                {
                    entry = reader.Item as ODataResource;
                }
            }

            Assert.Equal(ODataReaderState.Completed, reader.State);
        }

        Assert.NotNull(entry);
        var value = Assert.IsType<ODataProperty>(entry.Properties.First(p => p.Name == "UntypedProperty")).Value;
        Assert.IsType<double>(value);
        Assert.Equal(123456789012345.12, value);
    }

    #region Private Members

    private void ResetDefaultDataSource()
    {
        var actionUri = new Uri(_baseUri + "untypedtypes/Default.ResetDefaultDataSource", UriKind.Absolute);
        var requestMessage = new TestHttpClientRequestMessage(actionUri, Client);
        requestMessage.Method = "POST";

        var responseMessage = requestMessage.GetResponseAsync().Result;

        Assert.Equal(200, responseMessage.StatusCode);
    }

    private static async Task<string> ReadAsStringAsync(IODataResponseMessageAsync responseMessage)
    {
        using (Stream stream = await responseMessage.GetStreamAsync())
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                var content = await reader.ReadToEndAsync();

                // Format the content in JSON format
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                return JsonSerializer.Serialize(jsonElement,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
            }
        }
    }

    #endregion
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
