//---------------------------------------------------------------------
// <copyright file="PrimitiveValueFormatTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.PrimitiveTypesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.PrimitiveKeysServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrimitiveValueFormatTest : EndToEndTestBase
    {
        public PrimitiveValueFormatTest()
            : base(ServiceDescriptors.PrimitiveKeysService)
        {
        }

        private readonly string[] mimeTypes = new string[]
        {
            //MimeTypes.ApplicationAtomXml,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata + MimeTypes.ODataParameterIEEE754Compatible
        };

        private string dataPattern = "(" + @"[\+-]?\d\.?\d*E?[\+-]?\d*" + "|" + @"INF|-INF|NaN" + ")";

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        // NetCore: *QueryResult.Count() sends an synch query internally, so this needs to be re-written to be done asynch.
        // Otherwise this test throws a System.NotSupportedException.
        [TestMethod]
        public void LongInFilterLinqQuery()
        {
            const long int64Id = 1;

            var contextWrapper = this.CreateWrappedContext();

            var int64QueryResult = contextWrapper.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id >= int64Id);
            Assert.IsTrue(int64QueryResult.Count() > 0, "Expected one or more EdmInt64 entities could be returned ");

            int64QueryResult = (from c in contextWrapper.Context.EdmInt64Set where c.Id >= int64Id select c) as DataServiceQuery<EdmInt64>;
            Assert.IsTrue(int64QueryResult.Count() > 0, "Expected one or more EdmInt64 entities could be returned ");

            int64QueryResult = contextWrapper.CreateQuery<EdmInt64>("EdmInt64Set").Where(e => e.Id == int64Id);
            int count = 0;
            foreach (var int64 in int64QueryResult)
            {
                count++;
            }
            Assert.IsTrue(count == 1, "Expected one or more EdmInt64 entities could be returned ");

            var int64Query2 = contextWrapper.Context.EdmInt64Set.ByKey(new Dictionary<string, object> { { "Id", int64Id } });
            Assert.IsTrue(int64Query2.GetValue().Id == 1, "Expected one or more EdmInt64 entities could be returned ");
        }

        // NetCore: *QueryResult.Count() sends an synch query internally, so this needs to be re-written to be done asynch.
        // Otherwise this test throws a System.NotSupportedException.
        [TestMethod]
        public void FloatInFilterLinqQuery()
        {
            const float floatId = 1.0f;

            var contextWrapper = this.CreateWrappedContext();

            var floatQueryResult = contextWrapper.CreateQuery<EdmSingle>("EdmSingleSet").Where(e => e.Id >= floatId);
            Assert.IsTrue(floatQueryResult.Count() > 0, "Expected one or more EdmSingle entities could be returned ");

            floatQueryResult = (from c in contextWrapper.Context.EdmSingleSet where c.Id >= floatId select c) as DataServiceQuery<EdmSingle>;
            Assert.IsTrue(floatQueryResult.Count() > 0, "Expected one or more EdmSingle entities could be returned ");

        }

        // NetCore: *QueryResult.Count() sends an synch query internally, so this needs to be re-written to be done asynch.
        // Otherwise this test throws a System.NotSupportedException.
        [TestMethod]
        public void DoubleInFilterLinqQuery()
        {
            const double doubleId = 1.0;

            var contextWrapper = this.CreateWrappedContext();

            var doubleQueryResult = contextWrapper.CreateQuery<EdmDouble>("EdmDoubleSet").Where(e => e.Id >= doubleId);
            Assert.IsTrue(doubleQueryResult.Count() > 0, "Expected one or more EdmDouble entities could be returned ");

            doubleQueryResult = (from c in contextWrapper.Context.EdmDoubleSet where c.Id >= doubleId select c) as DataServiceQuery<EdmDouble>;
            Assert.IsTrue(doubleQueryResult.Count() > 0, "Expected one or more EdmDouble.MaxValue could be returned ");
        }

        // NetCore: *QueryResult.Count() sends an synch query internally, so this needs to be re-written to be done asynch.
        // Otherwise this test throws a System.NotSupportedException.
        [TestMethod]
        public void DecimalInFilterLinqQuery()
        {
            const decimal deciamlId = 1.0M;

            var contextWrapper = this.CreateWrappedContext();

            var decimalQueryResult = contextWrapper.CreateQuery<EdmDecimal>("EdmDecimalSet").Where(e => e.Id >= deciamlId);
            Assert.IsTrue(decimalQueryResult.Count() > 0, "Expected one or more EdmDecimal entities could be returned ");

            decimalQueryResult = (from c in contextWrapper.Context.EdmDecimalSet where c.Id >= deciamlId select c) as DataServiceQuery<EdmDecimal>;
            Assert.IsTrue(decimalQueryResult.Count() > 0, "Expected one or more EdmDecimal entities could be returned ");
        }
#endif

        [TestMethod]
        public void LongWithoutSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmInt64Set(1)");
        }

        [TestMethod]
        public void LongWithSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmInt64Set(1L)");
            PrimitiveValueAsKeyInURL("EdmInt64Set(1l)");
        }

        [TestMethod]
        public void FloatWithoutSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmSingleSet(1.0)");
        }

        [TestMethod]
        public void FloatWithSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmSingleSet(1.0F)");
            PrimitiveValueAsKeyInURL("EdmSingleSet(1.0f)");
        }

        [TestMethod]
        public void DoubleWithoutSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmDoubleSet(1.0)");
        }

        [TestMethod]
        public void DoubleWithSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmDoubleSet(1.0D)");
            PrimitiveValueAsKeyInURL("EdmDoubleSet(1.0d)");
        }

        [TestMethod]
        public void DecimalWithoutSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmDecimalSet(1.0)");
        }

        [TestMethod]
        public void DecimalWithSuffixAsKeyInURL()
        {
            PrimitiveValueAsKeyInURL("EdmDecimalSet(1.0M)");
            PrimitiveValueAsKeyInURL("EdmDecimalSet(1.0m)");
        }

        [TestMethod]
        public void LongWithoutSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmInt64Set?$filter=Id ge -1");
        }

        [TestMethod]
        public void LongWithSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmInt64Set?$filter=Id ge -1L");
            PrimitiveValueInFilterInURL("EdmInt64Set?$filter=Id ge -1l");
        }

        [TestMethod]
        public void FloatWithoutSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmSingleSet?$filter=Id ge -1.0");
        }

        [TestMethod]
        public void FloatWithSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmSingleSet?$filter=Id ge -1.0F");
            PrimitiveValueInFilterInURL("EdmSingleSet?$filter=Id ge -1.0f");
        }

        [TestMethod]
        public void DoubleWithoutSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmDoubleSet?$filter=Id ge -1.0");
        }

        [TestMethod]
        public void DoubleWithSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmDoubleSet?$filter=Id ge -1.0D");
            PrimitiveValueInFilterInURL("EdmDoubleSet?$filter=Id ge -1.0d");
        }

        [TestMethod]
        public void DecimalWithoutSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmDecimalSet?$filter=Id ge -1.0");
        }

        [TestMethod]
        public void DecimalWithSuffixInFilterInURL()
        {
            PrimitiveValueInFilterInURL("EdmDecimalSet?$filter=Id ge -1.0M");
            PrimitiveValueInFilterInURL("EdmDecimalSet?$filter=Id ge -1.0m");
        }

        [TestMethod]
        public void LongWithoutSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmInt64Set?$skiptoken=-1");
        }

        [TestMethod]
        public void LongWithSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmInt64Set?$skiptoken=-1L");
            PrimitiveValueInSkipTokenInURL("EdmInt64Set?$skiptoken=-1l");
        }

        [TestMethod]
        public void FloatWithoutSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmSingleSet?$skiptoken=-1.0");
        }

        [TestMethod]
        public void FloatWithSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmSingleSet?$skiptoken=-1.0F");
            PrimitiveValueInSkipTokenInURL("EdmSingleSet?$skiptoken=-1.0f");
        }

        [TestMethod]
        public void DoubleWithoutSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmDoubleSet?$skiptoken=-1.0");
        }

        [TestMethod]
        public void DoubleWithSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmDoubleSet?$skiptoken=-1.0D");
            PrimitiveValueInSkipTokenInURL("EdmDoubleSet?$skiptoken=-1.0d");
        }

        [TestMethod]
        public void DecimalWithoutSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmDecimalSet?$skiptoken=-1.0");
        }

        [TestMethod]
        public void DecimalWithSuffixInSkipTokenInURL()
        {
            PrimitiveValueInSkipTokenInURL("EdmDecimalSet?$skiptoken=-1.0M");
            PrimitiveValueInSkipTokenInURL("EdmDecimalSet?$skiptoken=-1.0m");
        }

        private void PrimitiveValueAsKeyInURL(string keySegment)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage(new Uri(this.ServiceUri.AbsoluteUri + keySegment, UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, RetrieveServiceEdmModel()))
                    {
                        var reader = messageReader.CreateODataResourceReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                var expectedKeySegment = keySegment.Substring(0, keySegment.IndexOf("(")) + "/1";
                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsTrue(entry.Id.ToString().Contains(expectedKeySegment), "Expected : Entry's Id doesn't contain trailing when Key is Int64/float/double/decimal");
                                if (mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata))
                                {
                                    Assert.IsTrue(entry.EditLink.ToString().Contains(expectedKeySegment), "Expected : Entry's EditLink doesn't contain trailing when Key is Int64/float/double/decimal");
                                    Assert.IsTrue(entry.ReadLink.ToString().Contains(expectedKeySegment), "Expected : Entry's ReadLink doesn't contain trailing when Key is Int64/float/double/decimal");
                                }
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        private void PrimitiveValueInFilterInURL(string filterQuery)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };
            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage(new Uri(this.ServiceUri.AbsoluteUri + filterQuery, UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, RetrieveServiceEdmModel()))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                string pattern = filterQuery.Substring(0, filterQuery.IndexOf("?")) + "/" + dataPattern + "$";
                                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsTrue(rgx.Match(entry.Id.ToString()).Success, "Expected : Entry's Id doesn't contain trailing when Key is Int64/float/double/decimal");
                                if (mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata))
                                {
                                    Assert.IsTrue(rgx.Match(entry.EditLink.ToString()).Success, "Expected : Entry's EditLink doesn't contain trailing when Key is Int64/float/double/decimal");
                                    Assert.IsTrue(rgx.Match(entry.ReadLink.ToString()).Success, "Expected : Entry's ReadLink doesn't contain trailing when Key is Int64/float/double/decimal");
                                }
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                //TODO: Nextlink is appened by data service. So whether L|F|D|M could be returned in nextLink
                                var pattern = filterQuery.Substring(0, filterQuery.IndexOf("?")) + @"\?\$filter=Id\sge\s" + dataPattern + @"(L|F|D|M)?&\$skiptoken=\d+$";
                                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                                ODataResourceSet feed = reader.Item as ODataResourceSet;
                                Assert.IsTrue(rgx.Match(feed.NextPageLink.ToString()).Success, "Expected : Feed's NextLink doesn't contain trailing when Key is Int64/float/double/decimal");
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        private void PrimitiveValueInSkipTokenInURL(string skipTokenQuery)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

            foreach (var mimeType in mimeTypes)
            {
                var requestMessage = new Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage(new Uri(this.ServiceUri.AbsoluteUri + skipTokenQuery, UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, RetrieveServiceEdmModel()))
                    {
                        var reader = messageReader.CreateODataResourceSetReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.ResourceEnd)
                            {
                                string pattern = skipTokenQuery.Substring(0, skipTokenQuery.IndexOf("?")) + "/" + dataPattern + "$";
                                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);

                                ODataResource entry = reader.Item as ODataResource;
                                Assert.IsTrue(rgx.Match(entry.Id.ToString()).Success, "Expected : Entry's Id doesn't contain trailing when Key is Int64/float/double/decimal");
                                if (mimeType.Equals(MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata))
                                {
                                    Assert.IsTrue(rgx.Match(entry.EditLink.ToString()).Success, "Expected : Entry's EditLink doesn't contain trailing when Key is Int64/float/double/decimal");
                                    Assert.IsTrue(rgx.Match(entry.ReadLink.ToString()).Success, "Expected : Entry's ReadLink doesn't contain trailing when Key is Int64/float/double/decimal");
                                }
                            }
                            else if (reader.State == ODataReaderState.ResourceSetEnd)
                            {
                                var pattern = skipTokenQuery.Substring(0, skipTokenQuery.IndexOf("?")) + @"\?\$skiptoken=" + dataPattern + "$";
                                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                                ODataResourceSet feed = reader.Item as ODataResourceSet;
                                Assert.IsTrue(rgx.Match(feed.NextPageLink.ToString()).Success, "Expected : Feed's NextLink doesn't contain trailing when Key is Int64/float/double/decimal");
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        private DataServiceContextWrapper<Services.TestServices.PrimitiveKeysServiceReference.TestContext> CreateWrappedContext()
        {
            var contextWrapper = base.CreateWrappedContext<Services.TestServices.PrimitiveKeysServiceReference.TestContext>();
            contextWrapper.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
            return contextWrapper;
        }
    }
}