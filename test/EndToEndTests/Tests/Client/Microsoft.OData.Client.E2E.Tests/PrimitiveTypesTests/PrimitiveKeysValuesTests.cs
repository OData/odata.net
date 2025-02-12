//---------------------------------------------------------------------
// <copyright file="PrimitiveKeysValuesTests.cs" company="Microsoft">
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
using EdmBinary = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmBinary;
using EdmBoolean = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmBoolean;
using EdmDateTimeOffset = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmDateTimeOffset;
using EdmDecimal = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmDecimal;
using EdmDouble = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmDouble;
using EdmInt16 = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmInt16;
using EdmInt32 = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmInt32;
using EdmInt64 = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmInt64;
using EdmSingle = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmSingle;
using EdmString = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmString;
using EdmTime = Microsoft.OData.E2E.TestCommon.Common.Client.PrimitiveKeys.EdmTime;

namespace Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests
{
    public class PrimitiveKeysValuesTests : EndToEndTestBase<PrimitiveKeysValuesTests.TestsStartup>
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

        public PrimitiveKeysValuesTests(TestWebApplicationFactory<TestsStartup> fixture)
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

        [Theory]
        [InlineData(new byte[] { }, "binary''")]
        [InlineData(new byte[] { 1 }, "binary'AQ%3D%3D'")]
        [InlineData(new byte[] { 2, 3, 4 }, "binary'AgME'")]
        public void BinaryTest(byte[] entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmBinary>("EdmBinarySet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmBinarySet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmBinary>("EdmBinarySet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmBinarySet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void BooleanTest(bool entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmBoolean>("EdmBooleanSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmBooleanSet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmBoolean>("EdmBooleanSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmBooleanSet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }


        public static TheoryData<DateTimeOffset, string> GetEdmDateTimeOffsetSet
        {
            get
            {
                return new TheoryData<DateTimeOffset, string>
                {
                    { DateTimeOffset.MinValue, "0001-01-01T00:00:00Z" },
                    { DateTimeOffset.MaxValue, "9999-12-31T23:59:59.9999999Z" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetEdmDateTimeOffsetSet))]
        public void DateTimeOffsetTest(DateTimeOffset entryId, string expectedIdInUrl)
        {
            // Arrange 
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmDateTimeOffset>("EdmDateTimeOffsetSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmDateTimeOffsetSet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmDateTimeOffset>("EdmDateTimeOffsetSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmDateTimeOffsetSet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        public static TheoryData<decimal, string> GetEdmDecimalSet
        {
            get
            {
                return new TheoryData<decimal, string>
                {
                    { decimal.Zero, "0" },
                    { decimal.MinusOne, "-1" },
                    { decimal.One, "1" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetEdmDecimalSet))]
        public void DecimalTest(decimal entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmDecimal>("EdmDecimalSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmDecimalSet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmDecimal>("EdmDecimalSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmDecimalSet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData(0, "0.0")]
        [InlineData(-1, "-1.0")]
        [InlineData(1, "1.0")]
        [InlineData(double.MaxValue, "1.7976931348623157E%2B308")]
        [InlineData(double.MinValue, "-1.7976931348623157E%2B308")]
        [InlineData(double.Epsilon, "5E-324")]
        [InlineData(double.NegativeInfinity, "-INF")]
        [InlineData(double.PositiveInfinity, "INF")]
        public void DoubleTest(double entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmDouble>("EdmDoubleSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmDoubleSet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmDouble>("EdmDoubleSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmDoubleSet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData(short.MinValue, "-32768")]
        [InlineData(short.MaxValue, "32767")]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        public void Int16Test(short entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmInt16>("EdmInt16Set").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmInt16Set/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmInt16>("EdmInt16Set").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmInt16Set({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData(int.MinValue, "-2147483648")]
        [InlineData(int.MaxValue, "2147483647")]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        public void Int32Test(int entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmInt32>("EdmInt32Set").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmInt32Set/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmInt32>("EdmInt32Set").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmInt32Set({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData(long.MinValue, "-9223372036854775808")]
        [InlineData(long.MaxValue, "9223372036854775807")]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        public void Int64Test(long entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmInt64>("EdmInt64Set").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmInt64Set/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmInt64>("EdmInt64Set").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmInt64Set({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        [InlineData(float.MaxValue, "3.4028235E%2B38")]
        [InlineData(float.MinValue, "-3.4028235E%2B38")]
        [InlineData(float.Epsilon, "1E-45")]
        public void SingleTest(float entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmSingle>("EdmSingleSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmSingleSet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmSingle>("EdmSingleSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmSingleSet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        public static TheoryData<TimeSpan, string> GetEdmTimeSet
        {
            get
            {
                return new TheoryData<TimeSpan, string>
                {
                    { TimeSpan.MinValue, "duration'-P10675199DT2H48M5.4775808S'" },
                    { TimeSpan.MaxValue, "duration'P10675199DT2H48M5.4775807S'" },
                    { TimeSpan.Zero, "duration'PT0S'" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetEdmTimeSet))]
        public void TimeTest(TimeSpan entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmTime>("EdmTimeSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmTimeSet/{expectedIdInUrl}", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmTime>("EdmTimeSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmTimeSet({expectedIdInUrl})", queryParentheses.RequestUri.AbsoluteUri);
        }

        [Theory]
        [InlineData("!", "%21")]
        [InlineData("!!", "%21%21")]
        [InlineData("!!!", "%21%21%21")]
        [InlineData("*", "%2A")]
        [InlineData("**", "%2A%2A")]
        [InlineData("***", "%2A%2A%2A")]
        [InlineData("(", "%28")]
        [InlineData("((", "%28%28")]
        [InlineData("(((", "%28%28%28")]
        [InlineData(")", "%29")]
        [InlineData("))", "%29%29")]
        [InlineData(")))", "%29%29%29")]
        [InlineData(":", "%3A")]
        [InlineData("::", "%3A%3A")]
        [InlineData(":::", "%3A%3A%3A")]
        [InlineData(";", "%3B")]
        [InlineData(";;", "%3B%3B")]
        [InlineData(";;;", "%3B%3B%3B")]
        [InlineData("=", "%3D")]
        [InlineData("==", "%3D%3D")]
        [InlineData("===", "%3D%3D%3D")]
        [InlineData("@", "%40")]
        [InlineData("@@", "%40%40")]
        [InlineData("@@@", "%40%40%40")]
        [InlineData("[", "%5B")]
        [InlineData("[[", "%5B%5B")]
        [InlineData("[[[", "%5B%5B%5B")]
        [InlineData("]", "%5D")]
        [InlineData("]]", "%5D%5D")]
        [InlineData("]]]", "%5D%5D%5D")]
        [InlineData("}", "%7D")]
        [InlineData("}}", "%7D%7D")]
        [InlineData("}}}", "%7D%7D%7D")]
        [InlineData("{", "%7B")]
        [InlineData("{{", "%7B%7B")]
        [InlineData("{{{", "%7B%7B%7B")]
        [InlineData("?", "%3F")]
        [InlineData("??", "%3F%3F")]
        [InlineData("???", "%3F%3F%3F")]
        [InlineData("&", "%26")]
        [InlineData("&&", "%26%26")]
        [InlineData("&&&", "%26%26%26")]
        [InlineData("$", "%24")]
        [InlineData("$$", "%24%24")]
        [InlineData("$$$", "%24%24%24")]
        [InlineData(",", "%2C")]
        [InlineData(",,", "%2C%2C")]
        [InlineData(",,,", "%2C%2C%2C")]
        [InlineData("+", "%2B")]
        [InlineData("++", "%2B%2B")]
        [InlineData("+++", "%2B%2B%2B")]
        [InlineData("'", "''")]
        [InlineData("''", "''''")]
        [InlineData("'''", "''''''")]
        [InlineData("/", "%2F")]
        [InlineData("//", "%2F%2F")]
        [InlineData("///", "%2F%2F%2F")]
        [InlineData("\"", "%22")]
        [InlineData("\"\"", "%22%22")]
        [InlineData("\"\"\"", "%22%22%22")]
        [InlineData("SomeID", "SomeID")]
        public void StringTest(string entryId, string expectedIdInUrl)
        {
            // Arrange
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;

            // Act
            var query = _context.CreateQuery<EdmString>("EdmStringSet").ByKey(entryId);
            var queryResult = query.GetValue();

            // Assert
            Assert.EndsWith($"/odata/EdmStringSet/'{expectedIdInUrl}'", query.RequestUri.AbsoluteUri);
            Assert.NotNull(queryResult);
            Assert.Equal(entryId, queryResult.Id);

            // DataServiceUrlKeyDelimiter.Parentheses
            _context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;

            var queryParentheses = _context.CreateQuery<EdmString>("EdmStringSet").ByKey(entryId);
            Assert.EndsWith($"/odata/EdmStringSet('{expectedIdInUrl}')", queryParentheses.RequestUri.AbsoluteUri);
        }

        #region Private

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "primitivekeyvalues/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }

        #endregion
    }
}
