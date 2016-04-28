//---------------------------------------------------------------------
// <copyright file="SingletonUpdateTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.SingletonTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingletonUpdateTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private const string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public SingletonUpdateTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [TestMethod]
        public void UpdateSingletonProperty()
        {
            string[] cities = { "London", "Seattle", "Paris", "New York", "Washington" };

            for (int i = 0; i < mimeTypes.Length; i++)
            {
                ODataResource entry = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(cities[i], entry.Properties.Single(p => p.Name == "City").Value);
                }

                var properties = new[] { new ODataProperty { Name = "City", Value = cities[i + 1] } };
                this.UpdateEntry("Customer", "VipCustomer", mimeTypes[i], properties);

                ODataResource updatedEntry = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(cities[i + 1], updatedEntry.Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [TestMethod]
        public void UpdateSingletonComplexProperty()
        {
            ODataComplexValue complexValue0 = new ODataComplexValue()
            {
                TypeName = NameSpacePrefix + "Address",
                Properties = new[]
                {
                    new ODataProperty() {Name = "Street", Value = "1 Microsoft Way"}, 
                    new ODataProperty() {Name = "City", Value = "London"}, 
                    new ODataProperty() {Name = "PostalCode", Value = "98052"}
                }
            };

            ODataProperty homeAddress0 = new ODataProperty() { Name = "HomeAddress", Value = complexValue0 };

            ODataComplexValue complexValue1 = new ODataComplexValue()
            {
                TypeName = NameSpacePrefix + "Address",
                Properties = new[]
                {
                    new ODataProperty() {Name = "Street", Value = "Zixing 999"}, 
                    new ODataProperty() {Name = "City", Value = "Seattle"}, 
                    new ODataProperty() {Name = "PostalCode", Value = "1111"}
                }
            };
            ODataProperty homeAddress1 = new ODataProperty() { Name = "HomeAddress", Value = complexValue1 };

            for (int i = 0; i < mimeTypes.Length; i++)
            {
                ODataProperty currentHomeAddress;
                ODataProperty updatedHomeAddress;

                if (i % 2 == 0)
                {
                    currentHomeAddress = homeAddress0;
                    updatedHomeAddress = homeAddress1;
                }
                else
                {
                    currentHomeAddress = homeAddress1;
                    updatedHomeAddress = homeAddress0;
                }
                ODataResource entry = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertyAreEqual((ODataProperty)entry.Properties.Single(p => p.Name == "HomeAddress"), currentHomeAddress);
                }

                var properties = new[] { updatedHomeAddress };
                this.UpdateEntry("Customer", "VipCustomer", mimeTypes[i], properties);

                ODataResource updatedentry = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertyAreEqual(updatedHomeAddress, (ODataProperty)updatedentry.Properties.Single(p => p.Name == "HomeAddress"));
                }
            }
        }

        #region Help function

        private ODataResource QueryEntry(string requestUri, string mimeType)
        {
            ODataResource entry = null;

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            entry = reader.Item as ODataResource;
                        }
                    }
                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }
            return entry;
        }

        private void UpdateEntry(string singletonType, string singletonName, string mimeType, IEnumerable<ODataProperty> properties)
        {
            ODataResource entry = new ODataResource() { TypeName = NameSpacePrefix + singletonType };
            entry.Properties = properties;

            var settings = new ODataMessageWriterSettings();
            settings.BaseUri = ServiceBaseUri;

            var customerType = Model.FindDeclaredType(NameSpacePrefix + singletonType) as IEdmEntityType;
            var customerSet = Model.EntityContainer.FindSingleton(singletonName);

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri + singletonName));
            requestMessage.SetHeader("Content-Type", mimeType);
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.Method = "PATCH";

            using (var messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(customerSet, customerType);
                odataWriter.WriteStart(entry);
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the update
            Assert.AreEqual(204, responseMessage.StatusCode);
        }

        #endregion
    }
}
