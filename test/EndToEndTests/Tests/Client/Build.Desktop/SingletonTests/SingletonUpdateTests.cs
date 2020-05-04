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
    using Xunit;

    public class SingletonUpdateTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        private const string NameSpacePrefix = "Microsoft.Test.OData.Services.ODataWCFService.";

        public SingletonUpdateTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        [Fact]
        public void UpdateSingletonProperty()
        {
            string[] cities = { "London", "Seattle", "Paris", "New York", "Washington" };

            for (int i = 0; i < mimeTypes.Length; i++)
            {
                List<ODataResource> entries = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(cities[i], entries[1].Properties.Single(p => p.Name == "City").Value);
                }

                var properties = new[] { new ODataProperty { Name = "City", Value = cities[i + 1] } };
                this.UpdateEntry("Customer", "VipCustomer", mimeTypes[i], properties);

                List<ODataResource> updatedEntries = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(cities[i + 1], updatedEntries[1].Properties.Single(p => p.Name == "City").Value);
                }
            }
        }

        [Fact]
        public void UpdateSingletonComplexProperty()
        {
            ODataResource complex0 = new ODataResource()
            {
                TypeName = NameSpacePrefix + "Address",
                Properties = new[]
                {
                    new ODataProperty() {Name = "Street", Value = "1 Microsoft Way"}, 
                    new ODataProperty() {Name = "City", Value = "London"}, 
                    new ODataProperty() {Name = "PostalCode", Value = "98052"}
                }
            };

            ODataNestedResourceInfo homeAddress0 = new ODataNestedResourceInfo() { Name = "HomeAddress", IsCollection = false };
            homeAddress0.SetAnnotation(complex0);

            ODataResource complex1 = new ODataResource()
            {
                TypeName = NameSpacePrefix + "Address",
                Properties = new[]
                {
                    new ODataProperty() {Name = "Street", Value = "Zixing 999"}, 
                    new ODataProperty() {Name = "City", Value = "Seattle"}, 
                    new ODataProperty() {Name = "PostalCode", Value = "1111"}
                }
            };
            ODataNestedResourceInfo homeAddress1 = new ODataNestedResourceInfo() { Name = "HomeAddress", IsCollection = false };
            homeAddress1.SetAnnotation(complex1);

            for (int i = 0; i < mimeTypes.Length; i++)
            {
                ODataResource currentHomeAddress;
                ODataResource updatedHomeAddress;

                object[] properties;
                if (i % 2 == 0)
                {
                    currentHomeAddress = complex0;
                    updatedHomeAddress = complex1;
                    properties = new[] { homeAddress1 };
                }
                else
                {
                    currentHomeAddress = complex1;
                    updatedHomeAddress = complex0;
                    properties = new[] { homeAddress0 };
                }
                List<ODataResource> entries = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertyAndResourceEqual(currentHomeAddress, entries[0] );
                }

                this.UpdateEntry("Customer", "VipCustomer", mimeTypes[i], properties);

                List<ODataResource> updatedentries = this.QueryEntry("VipCustomer", mimeTypes[i]);
                if (!mimeTypes[i].Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    ODataValueAssertEqualHelper.AssertODataPropertyAndResourceEqual(updatedHomeAddress, updatedentries[0]);
                }
            }
        }

        #region Help function

        private List<ODataResource> QueryEntry(string requestUri, string mimeType)
        {
            List<ODataResource> entries = new List<ODataResource>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var reader = messageReader.CreateODataResourceReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            entries.Add(reader.Item as ODataResource);
                        }
                    }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }

        private void UpdateEntry(string singletonType, string singletonName, string mimeType, IEnumerable<object> properties)
        {
            ODataResource entry = new ODataResource() { TypeName = NameSpacePrefix + singletonType };
            var elementType = properties != null && properties.Count() > 0 ? properties.ElementAt(0).GetType() : null;
            if (elementType == typeof(ODataProperty))
            {
                entry.Properties = properties.Cast<ODataProperty>();
            }

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
                if (elementType == typeof(ODataNestedResourceInfo))
                {
                    foreach (var p in properties)
                    {
                        var nestedInfo = (ODataNestedResourceInfo)p;
                        odataWriter.WriteStart(nestedInfo);
                        odataWriter.WriteStart(nestedInfo.GetAnnotation<ODataResource>());
                        odataWriter.WriteEnd();
                        odataWriter.WriteEnd();
                    }
                }
                odataWriter.WriteEnd();
            }

            var responseMessage = requestMessage.GetResponse();

            // verify the update
            Assert.Equal(204, responseMessage.StatusCode);
        }

        #endregion
    }
}
