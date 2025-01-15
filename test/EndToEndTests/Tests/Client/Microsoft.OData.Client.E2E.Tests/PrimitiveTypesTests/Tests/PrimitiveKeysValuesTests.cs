//---------------------------------------------------------------------
// <copyright file="PrimitiveKeysValuesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys;
using Microsoft.OData.Client.E2E.Tests.Common.Server.PrimitiveKeys;
using Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests.Server;
using Microsoft.OData.Edm;
using Xunit;
using EdmBinary = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmBinary;
using EdmBoolean = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmBoolean;
using EdmDateTimeOffset = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmDateTimeOffset;
using EdmDecimal = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmDecimal;
using EdmDouble = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmDouble;
using EdmInt16 = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmInt16;
using EdmInt32 = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmInt32;
using EdmInt64 = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmInt64;
using EdmSingle = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmSingle;
using EdmString = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmString;
using EdmTime = Microsoft.OData.Client.E2E.Tests.Common.Client.PrimitiveKeys.EdmTime;

namespace Microsoft.OData.Client.E2E.Tests.PrimitiveTypesTests.Tests
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

        public PrimitiveKeysValuesTests(TestWebApplicationFactory<PrimitiveKeysValuesTests.TestsStartup> fixture)
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
        [InlineData(new Byte[] { }, "binary''")]
        [InlineData(new Byte[] { 1 }, "binary'AQ%3D%3D'")]
        [InlineData(new Byte[] { 2, 3, 4 }, "binary'AgME'")]
        public void BinaryTest(byte[] entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmBinary>("EdmBinarySet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmBinarySet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        [Theory]
        [InlineData(true, "true")]
        [InlineData(false, "false")]
        public void BooleanTest(bool entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmBoolean>("EdmBooleanSet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmBooleanSet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }


        public static TheoryData<DateTimeOffset, string> GetEdmDateTimeOffsetSet
        {
            get
            {
                return new TheoryData<DateTimeOffset, string>
                {
                    { DateTimeOffset.MinValue, "0001-01-01T00%3A00%3A00Z" },
                    { DateTimeOffset.MaxValue, "9999-12-31T23%3A59%3A59.9999999Z" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetEdmDateTimeOffsetSet))]
        public void DateTimeOffsetTest(DateTimeOffset entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmDateTimeOffset>("EdmDateTimeOffsetSet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmDateTimeOffsetSet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        public static TheoryData<decimal, string> GetEdmDecimalSet
        {
            get
            {
                return new TheoryData<decimal, string>
                {
                    { Decimal.Zero, "0" },
                    { Decimal.MinusOne, "-1" },
                    { Decimal.One, "1" }
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetEdmDecimalSet))]
        public void DecimalTest(decimal entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmDecimal>("EdmDecimalSet").Where(e => e.Id == entryId);
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmDecimalSet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        [Theory]
        [InlineData(0, "0.0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1.0")]
        [InlineData(Double.MaxValue, "1.7976931348623157E%2B308")]
        [InlineData(Double.MinValue, "-1.7976931348623157E%2B308")]
        [InlineData(Double.Epsilon, "5E-324")]
        [InlineData(Double.NegativeInfinity, "-INF")]
        [InlineData(Double.PositiveInfinity, "INF")]
        public void DoubleTest(double entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmDouble>("EdmDoubleSet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmDoubleSet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        [Theory]
        [InlineData(Int16.MinValue, "-32768")]
        [InlineData(Int16.MaxValue, "32767")]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        public void Int16Test(short entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmInt16>("EdmInt16Set").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmInt16Set?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        [Theory]
        [InlineData(Int32.MinValue, "-2147483648")]
        [InlineData(Int32.MaxValue, "2147483647")]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        public void Int32Test(int entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmInt32>("EdmInt32Set").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmInt32Set?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        [Theory]
        [InlineData(Int64.MinValue, "-9223372036854775808")]
        [InlineData(Int64.MaxValue, "9223372036854775807")]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        public void Int64Test(long entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmInt64Set?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(-1, "-1")]
        [InlineData(1, "1")]
        [InlineData(float.MaxValue, "3.4028235E%2B38")]
        [InlineData(float.MinValue, "-3.4028235E%2B38")]
        [InlineData(float.Epsilon, "1E-45")]
        [InlineData(float.NegativeInfinity, "-INF")]
        [InlineData(float.PositiveInfinity, "INF")]
        public void SingleTest(float entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmSingle>("EdmSingleSet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmSingleSet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
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
            // Arrange & Act
            var query = _context.CreateQuery<EdmTime>("EdmTimeSet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmTimeSet?$filter=Id eq {expectedIdInUrl}", query.ToString());
            Assert.Single(queryResult);
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
        [InlineData("}", "}")]
        [InlineData("}}", "}}")]
        [InlineData("}}}", "}}}")]
        [InlineData("{", "{")]
        [InlineData("{{", "{{")]
        [InlineData("{{{", "{{{")]
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
        [InlineData("'", "%27%27")]
        [InlineData("''", "%27%27%27%27")]
        [InlineData("'''", "%27%27%27%27%27%27")]
        [InlineData("/", "%2F")]
        [InlineData("//", "%2F%2F")]
        [InlineData("///", "%2F%2F%2F")]
        [InlineData("\"", "\"")]
        [InlineData("\"\"", "\"\"")]
        [InlineData("\"\"\"", "\"\"\"")]
        [InlineData("SomeID", "SomeID")]
        public void StringTest(string entryId, string expectedIdInUrl)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmString>("EdmStringSet").Where(e => e.Id.Equals(entryId));
            var queryResult = query.ToArray();

            // Assert
            Assert.Equal($"http://localhost/odata/EdmStringSet?$filter=Id eq '{expectedIdInUrl}'", query.ToString());
            Assert.Single(queryResult);
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
