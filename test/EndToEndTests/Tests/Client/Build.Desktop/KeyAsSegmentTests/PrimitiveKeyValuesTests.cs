//---------------------------------------------------------------------
// <copyright file="PrimitiveKeyValuesTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using Microsoft.OData.Client;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PrimitiveKeysServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrimitiveKeysValuesTests : EndToEndTestBase
    {
        public PrimitiveKeysValuesTests()
            : base(ServiceDescriptors.PrimitiveKeysService)
        {
        }

        // github issuse: #896
        // [TestMethod /*Inconsistent reading and writing of key values*/]
        public void BinaryTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmBinarySet)
            {
                var queryResult = contextWrapper.CreateQuery<EdmBinary>("EdmBinarySet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value '{0}'", entry.Id.ToString());
            }
        }

        [TestMethod]
        public void BooleanTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmBooleanSet)
            {
                var queryResult = contextWrapper.CreateQuery<EdmBoolean>("EdmBooleanSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value '{0}'", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void DateTimeOffsetTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmDateTimeOffsetSet)
            {
                var queryResult = contextWrapper.CreateQuery<EdmDateTimeOffset>("EdmDateTimeOffsetSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString());
            }
        }

        [TestMethod]
        public void DecimalTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmDecimalSet)
            {
                var queryResult = contextWrapper.CreateQuery<EdmDecimal>("EdmDecimalSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void DoubleTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmDoubleSet)
            {
                if (IsNotSupportedKey(entry.Id))
                {
                    continue;
                }

                var queryResult = contextWrapper.CreateQuery<EdmDouble>("EdmDoubleSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void Int16Test()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmInt16Set)
            {
                var queryResult = contextWrapper.CreateQuery<EdmInt16>("EdmInt16Set").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void Int32Test()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmInt32Set)
            {
                var queryResult = contextWrapper.CreateQuery<EdmInt32>("EdmInt32Set").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void Int64Test()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmInt64Set)
            {
                var queryResult = contextWrapper.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void SingleTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmSingleSet)
            {
                if (IsNotSupportedKey(entry.Id))
                {
                    continue;
                }

                var queryResult = contextWrapper.CreateQuery<EdmSingle>("EdmSingleSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void StringTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmStringSet)
            {
                var queryResult = contextWrapper.CreateQuery<EdmString>("EdmStringSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id);
            }
        }

        // github issuse: #896
        // [TestMethod /* Incorrect parsing of url with single quote in key literal with KeyAsSegment url conventions */]
        public void StringTest_KeyWithSingleQuotes()
        {
            var entitySetName = "EdmStringSet";

            var context = this.CreateWrappedContext();
            context.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
            var query = context.CreateQuery<EdmString>(entitySetName).Where(e => e.Id == "'Hello'");
            Assert.AreEqual(this.ServiceUri.AbsoluteUri + entitySetName + "/''Hello''", query.ToString());
        }

        [TestMethod]
        public void TimeTest()
        {
            var contextWrapper = this.CreateWrappedContext();
            foreach (var entry in contextWrapper.Context.EdmTimeSet)
            {
                var queryResult = contextWrapper.CreateQuery<EdmTime>("EdmTimeSet").Where(e => e.Id.Equals(entry.Id)).ToArray();
                Assert.AreEqual(1, queryResult.Count(), "Expected a single result for key value {0}", entry.Id.ToString());
            }
        }

        private DataServiceContextWrapper<Services.TestServices.PrimitiveKeysServiceReference.TestContext> CreateWrappedContext()
        {
            var contextWrapper = base.CreateWrappedContext<Services.TestServices.PrimitiveKeysServiceReference.TestContext>();
            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
            return contextWrapper;
        }

        private static bool IsNotSupportedKey(float key)
        {
            return float.IsNaN(key);
        }

        private static bool IsNotSupportedKey(double key)
        {
            return double.IsNaN(key);
        }
    }
}
