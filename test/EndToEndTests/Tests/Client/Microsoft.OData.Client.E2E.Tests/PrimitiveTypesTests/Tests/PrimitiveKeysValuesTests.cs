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

        [Fact]
        public void BinaryTest()
        {
            foreach (var entry in _context.EdmBinarySet)
            {
                var queryResult = _context.CreateQuery<EdmBinary>("EdmBinarySet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString()
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void BooleanTest()
        {
            foreach (var entry in _context.EdmBooleanSet)
            {
                var queryResult = _context.CreateQuery<EdmBoolean>("EdmBooleanSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value '{0}', entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void DateTimeOffsetTest()
        {
            foreach (var entry in _context.EdmDateTimeOffsetSet)
            {
                var queryResult = _context.CreateQuery<EdmDateTimeOffset>("EdmDateTimeOffsetSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString()
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void DecimalTest()
        {
            foreach (var entry in _context.EdmDecimalSet)
            {
                var queryResult = _context.CreateQuery<EdmDecimal>("EdmDecimalSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void DoubleTest()
        {
            foreach (var entry in _context.EdmDoubleSet)
            {
                if (IsNotSupportedKey(entry.Id))
                {
                    continue;
                }

                var queryResult = _context.CreateQuery<EdmDouble>("EdmDoubleSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void Int16Test()
        {
            foreach (var entry in _context.EdmInt16Set)
            {
                var queryResult = _context.CreateQuery<EdmInt16>("EdmInt16Set").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void Int32Test()
        {
            foreach (var entry in _context.EdmInt32Set)
            {
                var queryResult = _context.CreateQuery<EdmInt32>("EdmInt32Set").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void Int64Test()
        {
            foreach (var entry in _context.EdmInt64Set)
            {
                var queryResult = _context.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void SingleTest()
        {
            foreach (var entry in _context.EdmSingleSet)
            {
                if (IsNotSupportedKey(entry.Id))
                {
                    continue;
                }

                var queryResult = _context.CreateQuery<EdmSingle>("EdmSingleSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString(CultureInfo.InvariantCulture)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void StringTest()
        {
            foreach (var entry in _context.EdmStringSet)
            {
                var queryResult = _context.CreateQuery<EdmString>("EdmStringSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0} entry.Id)
                Assert.Single(queryResult);
            }
        }

        [Fact]
        public void TimeTest()
        {
            foreach (var entry in _context.EdmTimeSet)
            {
                var queryResult = _context.CreateQuery<EdmTime>("EdmTimeSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                //Expected a single result for key value {0}, entry.Id.ToString()
                Assert.Single(queryResult);
            }
        }

        #region Private

        private static bool IsNotSupportedKey(float key)
        {
            return float.IsNaN(key);
        }

        private static bool IsNotSupportedKey(double key)
        {
            return double.IsNaN(key);
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "primitivekeyvalues/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }

        #endregion
    }
}
