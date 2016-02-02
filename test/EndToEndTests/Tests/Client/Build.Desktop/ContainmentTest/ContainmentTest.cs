//---------------------------------------------------------------------
// <copyright file="ContainmentTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ContainmentTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using HttpWebRequestMessage = Microsoft.Test.OData.Tests.Client.Common.HttpWebRequestMessage;

    /// <summary>
    /// Send query and verify the results from the service implemented using ODataLib and EDMLib.
    /// </summary>
    [TestClass]
    public class ContainmentTest : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        private const string TestModelNameSpace = "Microsoft.Test.OData.Services.ODataWCFService";

        public ContainmentTest()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        private readonly string[] containmentMimeTypes = new string[]
            {
                // MimeTypes.ApplicationAtomXml,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

        #region Query
        [TestMethod]
        public void QueryContainedEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/MyGiftCard", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual(301, entry.Properties.Single(p => p.Name == "GiftCardID").Value);
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void CallFunctionBoundedToContainedEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri +
                "Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.2)", UriKind.Absolute));
            requestMessage.SetHeader("Accept", "*/*");
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
            {
                var amount = messageReader.ReadProperty().Value;
                Assert.AreEqual(23.88, amount);
            }
        }

        [TestMethod]
        public void CallFunctionWhichReturnsContainedEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/Microsoft.Test.OData.Services.ODataWCFService.GetDefaultPI()", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataEntry> entries = new List<ODataEntry>();
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                        }
                        Assert.AreEqual(101901, entries.Single().Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void InvokeActionReturnsContainedEntity()
        {
            var writerSettings = new ODataMessageWriterSettings();
            writerSettings.PayloadBaseUri = ServiceBaseUri;
            var readerSettings = new ODataMessageReaderSettings();
            readerSettings.BaseUri = ServiceBaseUri;

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)/Microsoft.Test.OData.Services.ODataWCFService.RefreshDefaultPI"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "POST";
                DateTimeOffset newDate = new DateTimeOffset(DateTime.Now);
                using (var messageWriter = new ODataMessageWriter(requestMessage, writerSettings, Model))
                {
                    var odataWriter = messageWriter.CreateODataParameterWriter((IEdmOperation)null);
                    odataWriter.WriteStart();
                    odataWriter.WriteValue("newDate", newDate);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();
                        List<ODataEntry> entries = new List<ODataEntry>();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                entries.Add(entry);
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(101901, entries.Single().Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                        Assert.AreEqual(newDate, entries.Single().Properties.Single(p => p.Name == "CreatedDate").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntitySet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataEntry> entries = new List<ODataEntry>();
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                            else if (reader.State == ODataReaderState.FeedEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataFeed);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(4, entries.Count);

                        Assert.AreEqual(103905, entries[2].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                        Assert.AreEqual("103 new PI", entries[2].Properties.Single(p => p.Name == "FriendlyName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QuerySpecificEntityInContainedEntitySet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103902)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual("103 second PI", entry.Properties.Single(p => p.Name == "FriendlyName").Value);
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryLevel2ContainedEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual("Digital goods: App", entry.Properties.Single(p => p.Name == "TransactionDescription").Value);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryLevel2ContainedEntitySet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "StatementID").Value);
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "TransactionType").Value);
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "TransactionDescription").Value);
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "Amount").Value);
                            }
                            else if (reader.State == ODataReaderState.FeedEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataFeed);
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryLeve2NonContainedEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(103)/MyPaymentInstruments(103901)/TheStoredPI", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual(802, entry.Properties.Single(p => p.Name == "StoredPIID").Value);
                                Assert.AreEqual("AliPay", entry.Properties.Single(p => p.Name == "PIType").Value);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryLevel2Singleton()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)/BackupStoredPI", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        List<ODataEntry> entries = new List<ODataEntry>();
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry);
                                entries.Add(entry);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                        Assert.AreEqual(1, entries.Count);

                        Assert.AreEqual(800, entries[0].Properties.Single(p => p.Name == "StoredPIID").Value);
                        Assert.AreEqual("The Default Stored PI", entries[0].Properties.Single(p => p.Name == "PIName").Value);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntityWithDerivedTypeCast()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + string.Format("Accounts(101)/MyPaymentInstruments(101902)/{0}.CreditCardPI", TestModelNameSpace), UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual(101902, entry.Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntitySetWithDerivedTypeCast()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + string.Format("Accounts(101)/MyPaymentInstruments/{0}.CreditCardPI", TestModelNameSpace), UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                            }
                            else if (reader.State == ODataReaderState.FeedEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataFeed);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntitiesInDerivedTypeEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + string.Format("Accounts(101)/MyPaymentInstruments(101902)/{0}.CreditCardPI/CreditRecords", TestModelNameSpace), UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "CreditRecordID").Value);
                            }
                            else if (reader.State == ODataReaderState.FeedEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataFeed);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryEntityContainsContainmentSet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataEntryReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.AreEqual(101, entry.Properties.Single(p => p.Name == "AccountID").Value);
                            }
                        }
                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryFeedContainsContainmentSet()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var reader = messageReader.CreateODataFeedReader();

                        while (reader.Read())
                        {
                            if (reader.State == ODataReaderState.EntryEnd)
                            {
                                ODataEntry entry = reader.Item as ODataEntry;
                                Assert.IsNotNull(entry.Properties.Single(p => p.Name == "AccountID").Value);
                            }
                            else if (reader.State == ODataReaderState.FeedEnd)
                            {
                                Assert.IsNotNull(reader.Item as ODataFeed);
                            }
                        }

                        Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void QueryIndividualPropertyOfContainedEntity()
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101902)/PaymentInstrumentID", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        messageReader.ReadProperty();
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntitiesWithFilter()
        {
            Dictionary<string, bool> testCases = new Dictionary<string, bool>()
            {
                { "Accounts(103)/MyPaymentInstruments?$filter=PaymentInstrumentID gt 103901", false },
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in containmentMimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataEntry> entries = new List<ODataEntry>();
                            var reader = messageReader.CreateODataFeedReader();

                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.EntryEnd)
                                {
                                    ODataEntry entry = reader.Item as ODataEntry;
                                    Assert.IsNotNull(entry);
                                    entries.Add(entry);
                                }
                                else if (reader.State == ODataReaderState.FeedEnd)
                                {
                                    Assert.IsNotNull(reader.Item as ODataFeed);
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(2, entries.Count);
                            Assert.AreEqual(103902, entries[0].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                            Assert.AreEqual(103905, entries[1].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntitiesWithOrderby()
        {
            Dictionary<string, bool> testCases = new Dictionary<string, bool>()
            {
                { "Accounts(103)/MyPaymentInstruments?$orderby=CreatedDate", false },
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in containmentMimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataEntry> entries = new List<ODataEntry>();
                            var reader = messageReader.CreateODataFeedReader();

                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.EntryEnd)
                                {
                                    ODataEntry entry = reader.Item as ODataEntry;
                                    Assert.IsNotNull(entry);
                                    entries.Add(entry);
                                }
                                else if (reader.State == ODataReaderState.FeedEnd)
                                {
                                    Assert.IsNotNull(reader.Item as ODataFeed);
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(4, entries.Count);
                            Assert.AreEqual(103902, entries[0].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                            Assert.AreEqual(101910, entries[1].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                            Assert.AreEqual(103901, entries[2].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                            Assert.AreEqual(103905, entries[3].Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void QueryEntityAndExpandContainedEntity()
        {
            Dictionary<string, int[]> testCases = new Dictionary<string, int[]>()
            {
                { "Accounts(101)?$expand=MyGiftCard", new int[] {2, 4} },
                { "Accounts(101)?$expand=MyPaymentInstruments", new int[] {4, 15} },
                { "Accounts(101)?$select=AccountID&$expand=MyGiftCard($select=GiftCardID)", new int[] {2, 1}  }
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in containmentMimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataEntry> entries = new List<ODataEntry>();
                            List<ODataNavigationLink> navigationLinks = new List<ODataNavigationLink>();

                            var reader = messageReader.CreateODataEntryReader();
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.EntryEnd)
                                {
                                    entries.Add(reader.Item as ODataEntry);
                                }
                                else if (reader.State == ODataReaderState.NavigationLinkEnd)
                                {
                                    navigationLinks.Add(reader.Item as ODataNavigationLink);
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(testCase.Value[0], entries.Count);
                            Assert.AreEqual(testCase.Value[1], navigationLinks.Count);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void QueryContainedEntityWithSelectOption()
        {
            Dictionary<string, int> testCases = new Dictionary<string, int>()
            {
                { "Accounts(101)/MyGiftCard?$select=GiftCardID,GiftCardNO", 2 },
                { "Accounts(101)/MyPaymentInstruments(101901)?$select=PaymentInstrumentID,FriendlyName,CreatedDate", 3 },
            };

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var testCase in testCases)
            {
                foreach (var mimeType in containmentMimeTypes)
                {
                    var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + testCase.Key, UriKind.Absolute));
                    requestMessage.SetHeader("Accept", mimeType);
                    var responseMessage = requestMessage.GetResponse();
                    Assert.AreEqual(200, responseMessage.StatusCode);

                    if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                        {
                            List<ODataEntry> entries = new List<ODataEntry>();
                            List<ODataNavigationLink> navigationLinks = new List<ODataNavigationLink>();

                            var reader = messageReader.CreateODataEntryReader();
                            while (reader.Read())
                            {
                                if (reader.State == ODataReaderState.EntryEnd)
                                {
                                    entries.Add(reader.Item as ODataEntry);
                                }
                                else if (reader.State == ODataReaderState.NavigationLinkEnd)
                                {
                                    navigationLinks.Add(reader.Item as ODataNavigationLink);
                                }
                            }

                            Assert.AreEqual(ODataReaderState.Completed, reader.State);
                            Assert.AreEqual(1, entries.Count);
                            Assert.AreEqual(0, navigationLinks.Count);
                            Assert.AreEqual(testCase.Value, entries[0].Properties.Count());
                        }
                    }
                }
            }
        }

        #endregion

        #region Create/Update/Delete
        [TestMethod]
        public void CreateAndDeleteContainmentEntity()
        {
            // create entry and insert
            var paymentInstrumentEntry = new ODataEntry() { TypeName = TestModelNameSpace + ".PaymentInstrument" };
            var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 101904 };
            var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = "101 new PI" };
            var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 8, 29, 14, 11, 57)) };
            paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

            var accountType = Model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = Model.FindDeclaredType(TestModelNameSpace + ".PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)/MyPaymentInstruments"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "POST";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(myPaymentInstrumentSet, paymentInstrumentType);
                    odataWriter.WriteStart(paymentInstrumentEntry);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // verify the create
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataEntry entry = this.QueryEntityItem("Accounts(101)/MyPaymentInstruments(101904)") as ODataEntry;
                Assert.AreEqual(101904, entry.Properties.Single(p => p.Name == "PaymentInstrumentID").Value);

                // delete the entry
                var deleteRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)/MyPaymentInstruments(101904)"));
                deleteRequestMessage.Method = "DELETE";
                var deleteResponseMessage = deleteRequestMessage.GetResponse();

                // verify the delete
                Assert.AreEqual(204, deleteResponseMessage.StatusCode);
                ODataEntry deletedEntry = this.QueryEntityItem("Accounts(101)/MyPaymentInstruments(101904)", 204) as ODataEntry;
                Assert.IsNull(deletedEntry);
            }
        }

        [TestMethod]
        public void CreateSingleValuedContainedEntity()
        {
            // create entry and insert
            var giftCardEntry = new ODataEntry() { TypeName = TestModelNameSpace + ".GiftCard" };
            var giftCardEntryP1 = new ODataProperty { Name = "GiftCardID", Value = 304 };
            var giftCardEntryP2 = new ODataProperty { Name = "GiftCardNO", Value = "AAGS993A" };
            var giftCardEntryP3 = new ODataProperty { Name = "ExperationDate", Value = new DateTimeOffset(new DateTime(2013, 12, 30)) };
            var giftCardEntryP4 = new ODataProperty { Name = "Amount", Value = 37.0 };
            giftCardEntry.Properties = new[] { giftCardEntryP1, giftCardEntryP2, giftCardEntryP3, giftCardEntryP4 };

            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

            var accountType = Model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");
            var giftCardType = Model.FindDeclaredType(TestModelNameSpace + ".GiftCard") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyGiftCard") as IEdmNavigationProperty;
            var myGiftCardSet = accountSet.FindNavigationTarget(navProp);

            foreach (var mimeType in containmentMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(104)/MyGiftCard"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                // Use PATCH to upsert
                requestMessage.Method = "PATCH";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(myGiftCardSet, giftCardType);
                    odataWriter.WriteStart(giftCardEntry);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // verify the create
                // TODO: [tiano] the response code should be 201
                Assert.AreEqual(204, responseMessage.StatusCode);
                ODataEntry entry = this.QueryEntityItem("Accounts(104)/MyGiftCard") as ODataEntry;
                Assert.AreEqual(304, entry.Properties.Single(p => p.Name == "GiftCardID").Value);
            }
        }

        [TestMethod]
        public void UpdateContainmentEntity()
        {
            // create entry and insert
            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

            var accountType = Model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = Model.FindDeclaredType(TestModelNameSpace + ".PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            foreach (var mimeType in containmentMimeTypes)
            {
                var paymentInstrumentEntry = new ODataEntry() { TypeName = TestModelNameSpace + ".PaymentInstrument" };
                var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = 101903 };
                var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = mimeType };
                var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 8, 29, 14, 11, 57)) };
                paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)/MyPaymentInstruments(101903)"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "PATCH";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(myPaymentInstrumentSet, paymentInstrumentType);
                    odataWriter.WriteStart(paymentInstrumentEntry);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // verify the create
                Assert.AreEqual(204, responseMessage.StatusCode);
                ODataEntry entry = this.QueryEntityItem("Accounts(101)/MyPaymentInstruments(101903)") as ODataEntry;
                Assert.AreEqual(101903, entry.Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.AreEqual(mimeType, entry.Properties.Single(p => p.Name == "FriendlyName").Value);
            }
        }


        [TestMethod]
        public void UpsertContainmentEntity()
        {
            // create entry and insert
            var settings = new ODataMessageWriterSettings();
            settings.PayloadBaseUri = ServiceBaseUri;

            var accountType = Model.FindDeclaredType(TestModelNameSpace + ".Account") as IEdmEntityType;
            var accountSet = Model.EntityContainer.FindEntitySet("Accounts");
            var paymentInstrumentType = Model.FindDeclaredType(TestModelNameSpace + ".PaymentInstrument") as IEdmEntityType;
            IEdmNavigationProperty navProp = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var myPaymentInstrumentSet = accountSet.FindNavigationTarget(navProp);

            int count = 1;

            foreach (var mimeType in containmentMimeTypes)
            {
                int piid = 20000 + count;
                var paymentInstrumentEntry = new ODataEntry() { TypeName = TestModelNameSpace + ".PaymentInstrument" };
                var paymentInstrumentEntryP1 = new ODataProperty { Name = "PaymentInstrumentID", Value = piid };
                var paymentInstrumentEntryP2 = new ODataProperty { Name = "FriendlyName", Value = mimeType };
                var paymentInstrumentEntryP3 = new ODataProperty { Name = "CreatedDate", Value = new DateTimeOffset(new DateTime(2013, 8, 29, 14, 11, 57)) };
                paymentInstrumentEntry.Properties = new[] { paymentInstrumentEntryP1, paymentInstrumentEntryP2, paymentInstrumentEntryP3 };

                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + "Accounts(101)/MyPaymentInstruments(" + piid + ")"));

                requestMessage.SetHeader("Content-Type", mimeType);
                requestMessage.SetHeader("Accept", mimeType);
                requestMessage.Method = "PUT";
                using (var messageWriter = new ODataMessageWriter(requestMessage, settings, Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(myPaymentInstrumentSet, paymentInstrumentType);
                    odataWriter.WriteStart(paymentInstrumentEntry);
                    odataWriter.WriteEnd();
                }

                // send the http request
                var responseMessage = requestMessage.GetResponse();

                // verify the create
                Assert.AreEqual(201, responseMessage.StatusCode);
                ODataEntry entry = this.QueryEntityItem("Accounts(101)/MyPaymentInstruments(" + piid + ")") as ODataEntry;
                Assert.AreEqual(piid, entry.Properties.Single(p => p.Name == "PaymentInstrumentID").Value);
                Assert.AreEqual(mimeType, entry.Properties.Single(p => p.Name == "FriendlyName").Value);

                count++;
            }
        }

        #endregion

        #region OData Client test cases

        [TestMethod]
        public void QueryContainedEntityFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var queryable = TestClientContext.CreateQuery<GiftCard>("Accounts(101)/MyGiftCard");
                Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("Accounts(101)/MyGiftCard", StringComparison.Ordinal));

                List<GiftCard> result = queryable.ToList();
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(301, result[0].GiftCardID);
                Assert.AreEqual("AAA123A", result[0].GiftCardNO);
            }
        }

        [TestMethod]
        public void QueryContainedEntitySetFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var queryable = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments");
                Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("Accounts(103)/MyPaymentInstruments", StringComparison.Ordinal));

                List<PaymentInstrument> result = queryable.ToList();
                Assert.AreEqual(4, result.Count);
                Assert.AreEqual(103902, result[1].PaymentInstrumentID);
                Assert.AreEqual("103 second PI", result[1].FriendlyName);

            }
        }

        [TestMethod]
        public void QuerySpecificEntityInContainedEntitySetFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var queryable = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments(103902)");
                Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("Accounts(103)/MyPaymentInstruments(103902)", StringComparison.Ordinal));

                List<PaymentInstrument> result = queryable.ToList();
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(103902, result[0].PaymentInstrumentID);
                Assert.AreEqual("103 second PI", result[0].FriendlyName);
            }
        }

        [TestMethod]
        public void QueryIndividualPropertyOfContainedEntityFromODataClient()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                var queryable = TestClientContext.CreateQuery<int>("Accounts(103)/MyPaymentInstruments(103902)/PaymentInstrumentID");
                Assert.IsTrue(queryable.RequestUri.OriginalString.EndsWith("Accounts(103)/MyPaymentInstruments(103902)/PaymentInstrumentID", StringComparison.Ordinal));

                List<int> result = queryable.ToList();
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(103902, result[0]);
            }
        }

        [TestMethod]
        public void LinqUriTranslationTest()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                TestClientContext.MergeOption = MergeOption.OverwriteChanges;

                //translate to key
                var q1 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 103901);
                PaymentInstrument q1Result = q1.Single();
                Assert.AreEqual(103901, q1Result.PaymentInstrumentID);

                //$filter
                var q2 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.CreatedDate > new DateTimeOffset(new DateTime(2013, 10, 1)));
                PaymentInstrument q2Result = q2.Single();
                Assert.AreEqual(103905, q2Result.PaymentInstrumentID);

                //$orderby
                var q3 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").OrderBy(pi => pi.CreatedDate).ThenByDescending(pi => pi.FriendlyName);
                List<PaymentInstrument> q3Result = q3.ToList();
                Assert.AreEqual(103902, q3Result[0].PaymentInstrumentID);

                //$expand
                var q4 = TestClientContext.Accounts.Expand(account => account.MyPaymentInstruments).Where(account => account.AccountID == 103);
                Account q4Result = q4.Single();
                Assert.IsNotNull(q4Result.MyPaymentInstruments);

                var q5 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Expand(pi => pi.BillingStatements).Where(pi => pi.PaymentInstrumentID == 103901);
                PaymentInstrument q5Result = q5.Single();
                Assert.IsNotNull(q5Result.BillingStatements);

                //$top
                var q6 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Take(1);
                var q6Result = q6.ToList();

                //$count
                var q7 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Count();

                //$count=true
                var q8 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").IncludeTotalCount();
                var q8Result = q8.ToList();

                //projection
                var q9 = TestClientContext.Accounts.Where(a => a.AccountID == 103).Select(a => new Account() { AccountID = a.AccountID, MyGiftCard = a.MyGiftCard });
                var q9Result = q9.Single();
                Assert.IsNotNull(q9Result.MyGiftCard);

                var q10 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(103)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 103901).Select(p => new PaymentInstrument()
                {
                    PaymentInstrumentID = p.PaymentInstrumentID,
                    BillingStatements = p.BillingStatements
                });
                var q10Result = q10.ToList();
            }
        }

        [TestMethod]
        public void CallFunctionBoundToContainedEntityFromODataClient()
        {
            double result = TestClientContext.Execute<double>(new Uri(ServiceBaseUri.AbsoluteUri +
                "Accounts(101)/MyGiftCard/Microsoft.Test.OData.Services.ODataWCFService.GetActualAmount(bonusRate=0.2)", UriKind.Absolute), "GET", true).Single();

            Assert.AreEqual(23.88, result);

        }

        [TestMethod]
        public void CallFunctionFromODataClientWhichReturnsContainedEntity()
        {
            TestClientContext.Format.UseJson(Model);

            PaymentInstrument result = TestClientContext.Execute<PaymentInstrument>(new Uri(ServiceBaseUri.AbsoluteUri +
                "Accounts(101)/Microsoft.Test.OData.Services.ODataWCFService.GetDefaultPI()", UriKind.Absolute), "GET", true).Single();
            Assert.AreEqual(101901, result.PaymentInstrumentID);

            result.FriendlyName = "Random Name";
            TestClientContext.UpdateObject(result);
            TestClientContext.SaveChanges();

            result = TestClientContext.Execute<PaymentInstrument>(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)", UriKind.Absolute), "GET", true).Single();
            Assert.AreEqual("Random Name", result.FriendlyName);
        }

        [TestMethod]
        public void InvokeActionFromODataClientWhichReturnsContainedEntity()
        {
            TestClientContext.Format.UseJson(Model);

            PaymentInstrument result = TestClientContext.Execute<PaymentInstrument>(new Uri(ServiceBaseUri.AbsoluteUri +
                "Accounts(101)/Microsoft.Test.OData.Services.ODataWCFService.RefreshDefaultPI", UriKind.Absolute), "POST", true,
                new BodyOperationParameter("newDate", new DateTimeOffset(DateTime.Now))).Single();
            Assert.AreEqual(101901, result.PaymentInstrumentID);

            result.FriendlyName = "Random Name";
            TestClientContext.UpdateObject(result);
            TestClientContext.SaveChanges();

            result = TestClientContext.Execute<PaymentInstrument>(new Uri(ServiceBaseUri.AbsoluteUri + "Accounts(101)/MyPaymentInstruments(101901)", UriKind.Absolute), "GET").Single();
            Assert.AreEqual("Random Name", result.FriendlyName);
        }

        [TestMethod]
        public void CreateContainedEntityFromODataClientUsingAddRelatedObject()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                // create an an account entity and a contained PI entity
                Account newAccount = new Account()
                {
                    AccountID = 110,
                    CountryRegion = "CN",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "New",
                        LastName = "Guy"
                    }
                };
                PaymentInstrument newPI = new PaymentInstrument()
                {
                    PaymentInstrumentID = 110901,
                    FriendlyName = "110's first PI",
                    CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 10))
                };
                TestClientContext.AddToAccounts(newAccount);
                TestClientContext.AddRelatedObject(newAccount, "MyPaymentInstruments", newPI);
                TestClientContext.SaveChanges();

                var queryable0 = TestClientContext.Accounts.Where(account => account.AccountID == 110);
                Account accountResult = queryable0.Single();
                Assert.AreEqual("Guy", accountResult.AccountInfo.LastName);

                var queryable1 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(110)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 110901);
                PaymentInstrument piResult = queryable1.Single();
                Assert.AreEqual("110's first PI", piResult.FriendlyName);
            }
        }

        [TestMethod]
        public void DeleteContainedEntityFromODataClientUsingDeleteObject()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                // create an an account entity and a contained PI entity
                Account newAccount = new Account()
                {
                    AccountID = 115,
                    CountryRegion = "CN",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "New",
                        LastName = "Guy"
                    }
                };
                PaymentInstrument newPI = new PaymentInstrument()
                {
                    PaymentInstrumentID = 115901,
                    FriendlyName = "115's first PI",
                    CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 10))
                };
                TestClientContext.AddToAccounts(newAccount);
                TestClientContext.AddRelatedObject(newAccount, "MyPaymentInstruments", newPI);
                TestClientContext.SaveChanges();

                var queryable = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(115)/MyPaymentInstruments");
                PaymentInstrument piResult = queryable.Single();
                Assert.AreEqual("115's first PI", piResult.FriendlyName);

                TestClientContext.DeleteObject(piResult);
                TestClientContext.SaveChanges();

                List<PaymentInstrument> piResult2 = queryable.ToList();
                Assert.AreEqual(0, piResult2.Count);
            }
        }

        [TestMethod]
        public void UpdateContainedEntityFromODataClientUsingUpdateObject()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                // Get a contained PI entity
                var queryable1 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(101)/MyPaymentInstruments").Where(pi => pi.PaymentInstrumentID == 101901);
                PaymentInstrument piResult = queryable1.Single();

                piResult.FriendlyName = "Michael's first PI";
                TestClientContext.UpdateObject(piResult);
                TestClientContext.SaveChanges();

                piResult = queryable1.Single();
                Assert.AreEqual("Michael's first PI", piResult.FriendlyName);
            }
        }

        [TestMethod]
        [Ignore]
        public void CreateContainedEntityFromODataClientUsingAddRelatedObjectUsingBatchRequest()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                // create an an account entity and a contained PI entity
                Account newAccount = new Account()
                {
                    AccountID = 114,
                    CountryRegion = "CN",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "New",
                        LastName = "Guy"
                    }
                };
                PaymentInstrument newPI = new PaymentInstrument()
                {
                    PaymentInstrumentID = 110905,
                    FriendlyName = "110's first PI",
                    CreatedDate = new DateTimeOffset(new DateTime(2012, 12, 10))
                };
                TestClientContext.AddToAccounts(newAccount);
                TestClientContext.AddRelatedObject(newAccount, "MyPaymentInstruments", newPI);
                TestClientContext.SaveChanges(SaveChangesOptions.BatchWithIndependentOperations);

                var queryable0 = TestClientContext.CreateQuery<Account>("Accounts");
                List<Account> accountResult = queryable0.ToList();

                var queryable1 = TestClientContext.CreateQuery<PaymentInstrument>("Accounts(114)/MyPaymentInstruments");
                List<PaymentInstrument> piResult = queryable1.ToList();
            }
        }

        [TestMethod]
        public void CreateContainedNonCollectionEntityFromODataClientUsingUpdateRelatedObject()
        {
            for (int i = 1; i < 2; i++)
            {
                if (i == 0)
                {
                    TestClientContext.Format.UseAtom();
                }
                else
                {
                    TestClientContext.Format.UseJson(Model);
                }

                // create an an account entity and a contained PI entity
                Account newAccount = new Account()
                {
                    AccountID = 120,
                    CountryRegion = "GB",
                    AccountInfo = new AccountInfo()
                    {
                        FirstName = "Diana",
                        LastName = "Spencer"
                    }
                };
                GiftCard giftCard = new GiftCard()
                {
                    GiftCardID = 320,
                    GiftCardNO = "XX120ABCDE",
                    Amount = 76,
                    ExperationDate = new DateTimeOffset(new DateTime(2013, 12, 30))
                };

                TestClientContext.AddToAccounts(newAccount);
                TestClientContext.UpdateRelatedObject(newAccount, "MyGiftCard", giftCard);
                TestClientContext.SaveChanges();

                var queryable1 = TestClientContext.CreateQuery<GiftCard>("Accounts(120)/MyGiftCard");
                List<GiftCard> giftCardResult = queryable1.ToList();
                Assert.AreEqual(1, giftCardResult.Count);
                Assert.AreEqual(76, giftCardResult[0].Amount);
            }
        }

        #endregion

        #region private methods

        private ODataItem QueryEntityItem(string uri, int expectedStatusCode = 200)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", MimeTypes.ApplicationJsonLight);
            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.AreEqual(expectedStatusCode, queryResponseMessage.StatusCode);

            ODataItem item = null;
            if (expectedStatusCode == 200)
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataEntryReader();
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            item = reader.Item;
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }

            return item;
        }
        #endregion
    }
}
