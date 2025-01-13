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
        [MemberData(nameof(GetEdmBinarySet))]
        public void BinaryTest(EdmBinary entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmBinary>("EdmBinarySet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmBooleanSet))]
        public void BooleanTest(EdmBoolean entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmBoolean>("EdmBooleanSet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmDateTimeOffsetSet))]
        public void DateTimeOffsetTest(EdmDateTimeOffset entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmDateTimeOffset>("EdmDateTimeOffsetSet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmDecimalSet))]
        public void DecimalTest(EdmDecimal entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmDecimal>("EdmDecimalSet").Where(e => e.Id == entry.Id);
            var queryResult = query.ToArray();

            // Assert
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmDoubleSet))]
        public void DoubleTest(EdmDouble entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmDouble>("EdmDoubleSet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmInt16Set))]
        public void Int16Test(EdmInt16 entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmInt16>("EdmInt16Set").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmInt32Set))]
        public void Int32Test(EdmInt32 entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmInt32>("EdmInt32Set").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmInt64Set))]
        public void Int64Test(EdmInt64 entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture)
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmSingleSet))]
        public void SingleTest(EdmSingle entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmSingle>("EdmSingleSet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmStringSet))]
        public void StringTest(EdmString entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmString>("EdmStringSet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0} entry.Id)
            Assert.Single(queryResult);
        }

        [Theory]
        [MemberData(nameof(GetEdmTimeSet))]
        public void TimeTest(EdmTime entry)
        {
            // Arrange & Act
            var query = _context.CreateQuery<EdmTime>("EdmTimeSet").Where(e => e.Id.Equals(entry.Id));
            var queryResult = query.ToArray();

            // Assert
            //Expected a single result for key value {0}, entry.Id.ToString()
            Assert.Single(queryResult);
        }

        #region TheoryData

        public static TheoryData<T> GetTheoryData<T>(Func<Container, IEnumerable<T>> dataSelector)
        {
            var data = new TheoryData<T>();
            var instance = CreateInstance();
            foreach (var entry in dataSelector(instance._context))
            {
                if ((entry is EdmSingle { Id: var idSingle } && IsNotSupportedKey(idSingle)) ||
                    (entry is EdmDouble { Id: var idDouble } && IsNotSupportedKey(idDouble)) ||
                    (entry is EdmDecimal { Id: Decimal.MaxValue or Decimal.MinValue }))
                {
                    continue;
                }

                data.Add(entry);
            }

            return data;
        }

        public static TheoryData<EdmBinary> GetEdmBinarySet() => GetTheoryData(ctx => ctx.EdmBinarySet);
        public static TheoryData<EdmBoolean> GetEdmBooleanSet() => GetTheoryData(ctx => ctx.EdmBooleanSet);
        public static TheoryData<EdmDateTimeOffset> GetEdmDateTimeOffsetSet() => GetTheoryData(ctx => ctx.EdmDateTimeOffsetSet);
        public static TheoryData<EdmDecimal> GetEdmDecimalSet() => GetTheoryData(ctx => ctx.EdmDecimalSet);
        public static TheoryData<EdmDouble> GetEdmDoubleSet() => GetTheoryData(ctx => ctx.EdmDoubleSet);
        public static TheoryData<EdmInt16> GetEdmInt16Set() => GetTheoryData(ctx => ctx.EdmInt16Set);
        public static TheoryData<EdmInt32> GetEdmInt32Set() => GetTheoryData(ctx => ctx.EdmInt32Set);
        public static TheoryData<EdmInt64> GetEdmInt64Set() => GetTheoryData(ctx => ctx.EdmInt64Set);
        public static TheoryData<EdmSingle> GetEdmSingleSet() => GetTheoryData(ctx => ctx.EdmSingleSet);
        public static TheoryData<EdmString> GetEdmStringSet() => GetTheoryData(ctx => ctx.EdmStringSet);
        public static TheoryData<EdmTime> GetEdmTimeSet() => GetTheoryData(ctx => ctx.EdmTimeSet);

        #endregion

        #region Private

        private static bool IsNotSupportedKey(object key)
        {
            if (key is float keyInFloat)
            {
                return float.IsNaN(keyInFloat);
            }
            if (key is double keyInDouble)
            {
                return double.IsNaN(keyInDouble);
            }

            return false;
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "primitivekeyvalues/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }

        private static PrimitiveKeysValuesTests CreateInstance()
        {
            var fixture = new TestWebApplicationFactory<TestsStartup>();
            return new PrimitiveKeysValuesTests(fixture);
        }

        #endregion
    }
}
