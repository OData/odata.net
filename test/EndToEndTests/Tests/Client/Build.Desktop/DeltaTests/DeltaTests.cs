//---------------------------------------------------------------------
// <copyright file="DeltaTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.DeltaTests
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DeltaTests : ODataWCFServiceTestsBase<InMemoryEntities>
    {
        public DeltaTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        private readonly string[] deltaMimeTypes = new string[]
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

        [TestMethod]
        public void RequestDeltaLink()
        {
            var customersSet = Model.FindDeclaredEntitySet("Customers");
            var customerType = customersSet.EntityType();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in deltaMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var deltaReader = messageReader.CreateODataDeltaReader(customersSet, customerType);

                        while (deltaReader.Read())
                        {
                            if (deltaReader.State == ODataDeltaReaderState.DeltaEntryEnd)
                            {
                                ODataEntry entry = deltaReader.Item as ODataEntry;
                            }
                            else if (deltaReader.State == ODataDeltaReaderState.FeedEnd)
                            {
                                ODataDeltaFeed feed = deltaReader.Item as ODataDeltaFeed;
                            }
                        }
                        Assert.AreEqual(ODataDeltaReaderState.Completed, deltaReader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestDeltaLink_Containment()
        {
            var accountsSet = this.Model.FindDeclaredEntitySet("Accounts");
            var accountType = accountsSet.EntityType();
            var myPisNav = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var piSet = accountsSet.FindNavigationTarget(myPisNav) as IEdmEntitySetBase;
            var piType = piSet.EntityType();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in deltaMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=containment", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var deltaReader = messageReader.CreateODataDeltaReader(piSet, piType);

                        while (deltaReader.Read())
                        {
                            if (deltaReader.State == ODataDeltaReaderState.DeltaEntryEnd)
                            {
                                ODataEntry entry = deltaReader.Item as ODataEntry;

                            }
                        }
                        Assert.AreEqual(ODataDeltaReaderState.Completed, deltaReader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestDeltaLink_Derived()
        {
            var peopleSet = Model.FindDeclaredEntitySet("People");
            var personType = peopleSet.EntityType();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in deltaMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=derived", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var deltaReader = messageReader.CreateODataDeltaReader(peopleSet, personType);

                        while (deltaReader.Read())
                        {
                            if (deltaReader.State == ODataDeltaReaderState.DeltaEntryEnd)
                            {
                                ODataEntry entry = deltaReader.Item as ODataEntry;

                            }
                        }
                        Assert.AreEqual(ODataDeltaReaderState.Completed, deltaReader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestDeltaLink_Expanded()
        {
            var customersSet = Model.FindDeclaredEntitySet("Customers");
            var customerType = customersSet.EntityType();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in deltaMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=expanded", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                // Currently don't test full metadata because the delta reader doesn't support reading
                // annotations on expanded navigation properties, metadata reference properties and paging.
                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var deltaReader = messageReader.CreateODataDeltaReader(customersSet, customerType);

                        while (deltaReader.Read())
                        {
                            if (deltaReader.State == ODataDeltaReaderState.FeedEnd)
                            {
                                Assert.IsNotNull(deltaReader.Item as ODataDeltaFeed);
                            }
                            else if (deltaReader.State == ODataDeltaReaderState.DeltaEntryEnd)
                            {
                                Assert.IsNotNull(deltaReader.Item as ODataEntry);
                            }
                            else if (deltaReader.State == ODataDeltaReaderState.ExpandedNavigationProperty)
                            {
                                switch (deltaReader.SubState)
                                {
                                    case ODataReaderState.FeedEnd:
                                        Assert.IsNotNull(deltaReader.Item as ODataFeed);
                                        break;
                                    case ODataReaderState.EntryEnd:
                                        Assert.IsNotNull(deltaReader.Item as ODataEntry);
                                        break;
                                    case ODataReaderState.NavigationLinkEnd:
                                        Assert.IsNotNull(deltaReader.Item as ODataNavigationLink);
                                        break;
                                }
                            }
                        }

                        Assert.AreEqual(ODataDeltaReaderState.Completed, deltaReader.State);
                    }
                }
            }
        }

        [TestMethod]
        public void RequestDeltaLink_Projection()
        {
            var customersSet = Model.FindDeclaredEntitySet("Customers");
            var cutomerType = customersSet.EntityType();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            foreach (var mimeType in deltaMimeTypes)
            {
                var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=projection", UriKind.Absolute));
                requestMessage.SetHeader("Accept", mimeType);
                var responseMessage = requestMessage.GetResponse();
                Assert.AreEqual(200, responseMessage.StatusCode);

                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                    {
                        var deltaReader = messageReader.CreateODataDeltaReader(customersSet, cutomerType);

                        while (deltaReader.Read())
                        {
                            if (deltaReader.State == ODataDeltaReaderState.DeltaEntryEnd)
                            {
                                ODataEntry entry = deltaReader.Item as ODataEntry;

                            }
                        }
                        Assert.AreEqual(ODataDeltaReaderState.Completed, deltaReader.State);
                    }
                }
            }
        }

    }
}
