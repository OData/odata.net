//---------------------------------------------------------------------
// <copyright file="AutoComputePayloadMetadataInJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Script.Serialization;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.OData.DependencyInjection;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AutoComputePayloadMetadataInJsonTests : EndToEndTestBase
    {
        protected const string NameSpace = "Microsoft.Test.OData.Services.AstoriaDefaultService.";

        private readonly string[] mimeTypes = new string[]
        {
            //MimeTypes.ApplicationAtomXml,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        };

        private readonly bool[] hasModelFlagBools = new bool[] { true, false, };

        public AutoComputePayloadMetadataInJsonTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        public override void CustomTestInitialize()
        {
            WritePayloadHelper.CustomTestInitialize(this.ServiceUri);
        }

        /// <summary>
        /// User tries to write a feed with two entries, one with all metadata and the other without metadata.
        /// </summary>
        [TestMethod]
        public void FeedTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                foreach (var hasModel in this.hasModelFlagBools)
                {
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                    settings.BaseUri = this.ServiceUri;

                    settings.AutoComputePayloadMetadata = false;
                    string defaultModeResult = this.WriteAndVerifyOrderFeed(settings, mimeType, hasModel);

                    settings.AutoComputePayloadMetadata = true;
                    string autoComputeMetadataModeResult = this.WriteAndVerifyOrderFeed(settings, mimeType, hasModel);

                    // For Atom/VerboseJson, verify that the result is the same for AutoComputePayloadMetadata=true/false
                    if (mimeType == MimeTypes.ApplicationAtomXml)
                    {
                        WritePayloadHelper.VerifyPayloadString(defaultModeResult, autoComputeMetadataModeResult, mimeType);
                    }
                }
            }
        }

        private string WriteAndVerifyOrderFeed(ODataMessageWriterSettings settings, string mimeType, bool hasModel)
        {
            // create a feed with two entries
            var orderFeed = new ODataResourceSet()
            {
                NextPageLink = new Uri(this.ServiceUri + "Order?$skiptoken=-9"),
                Count = 9999
            };

            if (mimeType == MimeTypes.ApplicationAtomXml)
            {
                orderFeed.Id = new Uri(this.ServiceUri + "Order");
            }

            if (!hasModel)
            {
                orderFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
            }

            var orderEntry1 = WritePayloadHelper.CreateOrderEntry1NoMetadata(hasModel);
            Dictionary<string, object> expectedOrderObject1 = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.OrderType, "Order(-10)", orderEntry1, hasModel);

            var orderEntry2 = WritePayloadHelper.CreateOrderEntry2NoMetadata(hasModel);
            Dictionary<string, object> expectedOrderObject2 = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.OrderType, "Order(-9)", orderEntry2, hasModel);
            var orderEntry2Navigation = WritePayloadHelper.AddOrderEntryCustomNavigation(orderEntry2, expectedOrderObject2, hasModel);

            // write the response message and read using ODL reader
            var responseMessage = new StreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);
            string result = string.Empty;
            using (var messageWriter = this.CreateODataMessageWriter(responseMessage, settings, hasModel))
            {
                var odataWriter = this.CreateODataResourceSetWriter(messageWriter, WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType, hasModel);

                odataWriter.WriteStart(orderFeed);

                odataWriter.WriteStart(orderEntry1);
                odataWriter.WriteEnd();

                odataWriter.WriteStart(orderEntry2);
                odataWriter.WriteStart(orderEntry2Navigation);
                odataWriter.WriteEnd();
                odataWriter.WriteEnd();

                // Finish writing the feed.
                odataWriter.WriteEnd();

                result = this.ReadFeedEntryMessage(true, responseMessage, mimeType, WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
            }

            if (mimeType != MimeTypes.ApplicationAtomXml)
            {
                JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, object> resultObject = jScriptSerializer.DeserializeObject(result) as Dictionary<string, object>;

                Assert.AreEqual(this.ServiceUri + "Order?$skiptoken=-9", resultObject.Single(e => e.Key == (JsonLightConstants.ODataNextLinkAnnotationName)).Value, "Feed next link");
                Assert.AreEqual(9999, resultObject.Single(e => e.Key == (JsonLightConstants.ODataCountAnnotationName)).Value, "Feed count");
                resultObject.Remove(JsonLightConstants.ODataNextLinkAnnotationName);
                resultObject.Remove(JsonLightConstants.ODataCountAnnotationName);

                VerifyODataContextAnnotation(this.ServiceUri + "$metadata#Order", resultObject, mimeType);

                VerifyEntry(expectedOrderObject1, orderEntry1, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).First() as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
                VerifyEntry(expectedOrderObject2, orderEntry2, new ODataNestedResourceInfo[] { orderEntry2Navigation }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).Last() as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
            }

            return result;
        }

        /// <summary>
        /// Write an expanded customer entry containing primitive, complex, collection of primitive/complex properties. Covers selection result.
        /// </summary>
        [TestMethod]
        public void ExpandedEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                foreach (var hasModel in this.hasModelFlagBools)
                {
                    const string selectClause = "CustomerId,Video,Orders";
                    const string expandClause = "Logins($select=Username;$expand=SentMessages)";
                    const string expectedClause = "CustomerId,Video,Orders,Logins,Logins(Username,SentMessages)";
                    var uriParser = new ODataUriParser(WritePayloadHelper.Model, WritePayloadHelper.ServiceUri, new Uri(this.ServiceUri, "Customer?$select=" + selectClause + "&$expand=" + expandClause));
                    var result = uriParser.ParseSelectAndExpand();
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri, SelectAndExpand = result };

                    settings.BaseUri = this.ServiceUri;

                    settings.AutoComputePayloadMetadata = false;
                    string defaultModeResult = this.WriteAndVerifyExpandedCustomerEntry(settings, mimeType, expectedClause, hasModel);

                    settings.AutoComputePayloadMetadata = true;
                    string autoComputeMetadataModeResult = this.WriteAndVerifyExpandedCustomerEntry(settings, mimeType, expectedClause, hasModel);

                    // For Atom, verify that the result is the same for AutoComputePayloadMetadata=true/false
                    if (mimeType == MimeTypes.ApplicationAtomXml)
                    {
                        WritePayloadHelper.VerifyPayloadString(defaultModeResult, autoComputeMetadataModeResult, mimeType);
                    }
                }
            }
        }

        private string WriteAndVerifyExpandedCustomerEntry(ODataMessageWriterSettings settings, string mimeType, string expectedProjectionClause, bool hasModel)
        {
            ODataResource customerEntry = WritePayloadHelper.CreateCustomerEntryNoMetadata(hasModel);
            Dictionary<string, object> expectedCustomerObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.CustomerType, "Customer(-9)", customerEntry, hasModel);
            var thumbnailProperty = WritePayloadHelper.AddCustomerMediaProperty(customerEntry, expectedCustomerObject);

            // order navigation
            var orderNavigation = WritePayloadHelper.CreateCustomerOrderNavigation(expectedCustomerObject);

            // expanded logins navigation containing a Login instance
            var expandedLoginsNavigation = WritePayloadHelper.CreateExpandedCustomerLoginsNavigation(expectedCustomerObject);
            var loginFeed = new ODataResourceSet();
            if (mimeType == MimeTypes.ApplicationAtomXml)
            {
                loginFeed.Id = new Uri(this.ServiceUri + "Customer(-9)/Logins");
            }

            if (!hasModel)
            {
                loginFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Login", NavigationSourceEntityTypeName = NameSpace + "Login" });
            }

            var loginEntry = WritePayloadHelper.CreateLoginEntryNoMetadata(hasModel);
            Dictionary<string, object> expectedLoginObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.LoginType, "Login('2')", loginEntry, hasModel);

            this.RemoveNonSelectedMetadataFromExpected(expectedCustomerObject, expectedLoginObject, hasModel);

            // write the response message and read using ODL reader
            var responseMessage = new StreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);
            string result = string.Empty;
            using (var messageWriter = this.CreateODataMessageWriter(responseMessage, settings, hasModel))
            {
                var odataWriter = this.CreateODataEntryWriter(messageWriter, WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType, hasModel);

                odataWriter.WriteStart(customerEntry);

                odataWriter.WriteStart(orderNavigation);
                odataWriter.WriteEnd();

                // write expanded navigation
                odataWriter.WriteStart(expandedLoginsNavigation);

                odataWriter.WriteStart(loginFeed);
                odataWriter.WriteStart(loginEntry);
                odataWriter.WriteEnd();
                odataWriter.WriteEnd();

                // Finish writing expandedNavigation.
                odataWriter.WriteEnd();

                // Finish writing customerEntry.
                odataWriter.WriteEnd();

                result = this.ReadFeedEntryMessage(false, responseMessage, mimeType, WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType);
            }

            // For Json light, verify the resulting metadata is as expected
            if (mimeType != MimeTypes.ApplicationAtomXml)
            {
                JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, object> resultObject = jScriptSerializer.DeserializeObject(result) as Dictionary<string, object>;

                VerifyODataContextAnnotation(this.ServiceUri + "$metadata#Customer(" + expectedProjectionClause + ")/$entity", resultObject, mimeType);

                VerifyEntry(expectedCustomerObject, customerEntry, new ODataNestedResourceInfo[] { orderNavigation, expandedLoginsNavigation }, new ODataProperty[] { thumbnailProperty }, resultObject, mimeType, settings.AutoComputePayloadMetadata);
                VerifyEntry(expectedLoginObject, loginEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject["Logins"] as object[]).Single() as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
            }

            return result;
        }

        private void RemoveNonSelectedMetadataFromExpected(Dictionary<string, object> expectedCustomerObject, Dictionary<string, object> expectedLoginObject, bool hasModel)
        {
            if (hasModel)
            {
                // Video property is selected, so the fullmetadata result should auto-compute and inclue it
                expectedCustomerObject.Add("Video" + JsonLightConstants.ODataMediaEditLinkAnnotationName, ServiceUri + "Customer(-9)/Video");
                expectedCustomerObject.Add("Video" + JsonLightConstants.ODataMediaReadLinkAnnotationName, ServiceUri + "Customer(-9)/Video");
            }

            // navigations that are not slected or specified by user are not expected to be auto-computed/included
            expectedCustomerObject.Remove("Husband" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedCustomerObject.Remove("Husband" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);
            expectedCustomerObject.Remove("Wife" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedCustomerObject.Remove("Wife" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);
            expectedCustomerObject.Remove("Info" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedCustomerObject.Remove("Info" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);

            expectedLoginObject.Remove("Customer" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedLoginObject.Remove("Customer" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);
            expectedLoginObject.Remove("LastLogin" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedLoginObject.Remove("LastLogin" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);
            expectedLoginObject.Remove("ReceivedMessages" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedLoginObject.Remove("ReceivedMessages" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);
            expectedLoginObject.Remove("Orders" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName);
            expectedLoginObject.Remove("Orders" + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName);

            // actions/functions that are not selected or specified by user are not expected to be auto-computed/included
            expectedCustomerObject.Remove("#Microsoft.Test.OData.Services.AstoriaDefaultService.ChangeCustomerAuditInfo");
        }

        /// <summary>
        /// Write a feed containing actions, derived type entry instances
        /// </summary>
        [TestMethod]
        public void FeedWithDerivedTypeInstanceTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                foreach (var hasModel in this.hasModelFlagBools)
                {
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                    settings.BaseUri = this.ServiceUri;

                    settings.AutoComputePayloadMetadata = false;
                    string defaultModeResult = this.WriteAndVerifyPersonFeed(settings, mimeType, hasModel);

                    settings.AutoComputePayloadMetadata = true;
                    string autoComputeMetadataModeResult = this.WriteAndVerifyPersonFeed(settings, mimeType, hasModel);

                    // For Atom, verify that the result is the same for AutoComputePayloadMetadata=true/false
                    if (mimeType == MimeTypes.ApplicationAtomXml)
                    {
                        WritePayloadHelper.VerifyPayloadString(defaultModeResult, autoComputeMetadataModeResult, mimeType);
                    }
                }
            }
        }

        private string WriteAndVerifyPersonFeed(ODataMessageWriterSettings settings, string mimeType, bool hasModel)
        {
            // create a Person feed containing a person, an employee, a special employee
            var personFeed = new ODataResourceSet();
            if (mimeType == MimeTypes.ApplicationAtomXml)
            {
                personFeed.Id = new Uri(this.ServiceUri + "Person");
            }

            if (!hasModel)
            {
                personFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });
            }

            ODataResource personEntry = WritePayloadHelper.CreatePersonEntryNoMetadata(hasModel);
            Dictionary<string, object> expectedPersonObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.PersonType, "Person(-5)", personEntry, hasModel);

            ODataResource employeeEntry = WritePayloadHelper.CreateEmployeeEntryNoMetadata(hasModel);
            Dictionary<string, object> expectedEmployeeObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.EmployeeType, "Person(-3)", employeeEntry, hasModel, true);

            ODataResource specialEmployeeEntry = WritePayloadHelper.CreateSpecialEmployeeEntryNoMetadata(hasModel);
            Dictionary<string, object> expectedSpecialEmployeeObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.SpecialEmployeeType, "Person(-10)", specialEmployeeEntry, hasModel, true);

            // write the response message and read using ODL reader
            var responseMessage = new StreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);
            string result = string.Empty;
            using (var messageWriter = this.CreateODataMessageWriter(responseMessage, settings, hasModel))
            {
                var odataWriter = this.CreateODataResourceSetWriter(messageWriter, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, hasModel);
                odataWriter.WriteStart(personFeed);

                odataWriter.WriteStart(personEntry);
                odataWriter.WriteEnd();

                odataWriter.WriteStart(employeeEntry);
                odataWriter.WriteEnd();

                odataWriter.WriteStart(specialEmployeeEntry);
                odataWriter.WriteEnd();

                // Finish writing the feed.
                odataWriter.WriteEnd();

                result = this.ReadFeedEntryMessage(true, responseMessage, mimeType, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType);
            }

            // For Json light, verify the resulting metadata is as expected
            if (mimeType != MimeTypes.ApplicationAtomXml)
            {
                JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, object> resultObject = jScriptSerializer.DeserializeObject(result) as Dictionary<string, object>;

                VerifyODataContextAnnotation(this.ServiceUri + "$metadata#Person", resultObject, mimeType);

                VerifyEntry(expectedPersonObject, personEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).ElementAt(0) as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
                VerifyEntry(expectedEmployeeObject, employeeEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).ElementAt(1) as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
                VerifyEntry(expectedSpecialEmployeeObject, specialEmployeeEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).ElementAt(2) as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
            }

            return result;
        }

        /// <summary>
        /// Write an entry containing stream, named stream
        /// </summary>
        [TestMethod]
        public void EntryWithMediaTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                foreach (var hasModel in this.hasModelFlagBools)
                {
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                    settings.BaseUri = this.ServiceUri;

                    settings.AutoComputePayloadMetadata = false;
                    string defaultModeResult = this.WriteAndVerifyCarEntry(settings, mimeType, hasModel);

                    settings.AutoComputePayloadMetadata = true;
                    string autoComputeMetadataModeResult = this.WriteAndVerifyCarEntry(settings, mimeType, hasModel);

                    // For Atom, verify that the result is the same for AutoComputePayloadMetadata=true/false
                    if (mimeType == MimeTypes.ApplicationAtomXml)
                    {
                        WritePayloadHelper.VerifyPayloadString(defaultModeResult, autoComputeMetadataModeResult, mimeType);
                    }
                }
            }
        }

        private string WriteAndVerifyCarEntry(ODataMessageWriterSettings settings, string mimeType, bool hasModel)
        {
            // create a car entry
            var carEntry = WritePayloadHelper.CreateCarEntryNoMetadata(hasModel);
            carEntry.MediaResource = new ODataStreamReferenceValue();

            Dictionary<string, object> expectedCarObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.CarType, "Car(11)", carEntry, hasModel);
            WritePayloadHelper.ComputeDefaultExpectedFullMetadataEntryMedia(WritePayloadHelper.CarType, "Car(11)", carEntry, expectedCarObject, true /*hasStream*/, hasModel);

            // write the response message and read using ODL reader
            var responseMessage = new StreamResponseMessage(new MemoryStream());
            responseMessage.SetHeader("Content-Type", mimeType);
            responseMessage.PreferenceAppliedHeader().AnnotationFilter = "foo.*";
            string result = string.Empty;
            using (var messageWriter = this.CreateODataMessageWriter(responseMessage, settings, hasModel))
            {
                var odataWriter = this.CreateODataEntryWriter(messageWriter, WritePayloadHelper.CarSet, WritePayloadHelper.CarType, hasModel);
                odataWriter.WriteStart(carEntry);
                odataWriter.WriteEnd();

                result = this.ReadFeedEntryMessage(false, responseMessage, mimeType, WritePayloadHelper.CarSet, WritePayloadHelper.CarType);
            }

            // For Json light, verify the resulting metadata is as expected
            if (mimeType != MimeTypes.ApplicationAtomXml)
            {
                JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, object> resultObject = jScriptSerializer.DeserializeObject(result) as Dictionary<string, object>;

                VerifyODataContextAnnotation(this.ServiceUri + "$metadata#Car/$entity", resultObject, mimeType);

                VerifyEntry(expectedCarObject, carEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, resultObject, mimeType, settings.AutoComputePayloadMetadata);
            }

            return result;
        }

        /// <summary>
        /// Write a request message with an entry
        /// </summary>
        [TestMethod]
        public void RequestMessageTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                foreach (var hasModel in this.hasModelFlagBools)
                {
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                    settings.BaseUri = this.ServiceUri;

                    settings.AutoComputePayloadMetadata = false;
                    string defaultModeResult = this.WriteAndVerifyRequestMessage(settings, mimeType, hasModel);

                    settings.AutoComputePayloadMetadata = true;
                    string autoComputeMetadataModeResult = this.WriteAndVerifyRequestMessage(settings, mimeType, hasModel);
                }
            }
        }

        private string WriteAndVerifyRequestMessage(ODataMessageWriterSettings settings, string mimeType, bool hasModel)
        {
            // create an order entry to POST
            var order = new ODataResource()
            {
                TypeName = NameSpace + "Order"
            };

            var orderP1 = new ODataProperty { Name = "OrderId", Value = -10 };
            var orderp2 = new ODataProperty { Name = "CustomerId", Value = 8212 };
            var orderp3 = new ODataProperty { Name = "Concurrency", Value = null };
            order.Properties = new[] { orderP1, orderp2, orderp3 };
            if (!hasModel)
            {
                order.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
                orderP1.SetSerializationInfo(new ODataPropertySerializationInfo() { PropertyKind = ODataPropertyKind.Key });
            }

            Dictionary<string, object> expectedOrderObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.OrderType, "Order(-10)", order, hasModel);

            // write the request message and read using ODL reader
            var requestMessage = new StreamRequestMessage(new MemoryStream(), new Uri(this.ServiceUri + "Order"), "POST");
            requestMessage.SetHeader("Content-Type", mimeType);
            string result = string.Empty;
            using (var messageWriter = this.CreateODataMessageWriter(requestMessage, settings, hasModel))
            {
                var odataWriter = this.CreateODataEntryWriter(messageWriter, WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType, hasModel);
                odataWriter.WriteStart(order);
                odataWriter.WriteEnd();

                Stream stream = requestMessage.GetStream();
                result = WritePayloadHelper.ReadStreamContent(stream);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var readerSetting = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };
                    ODataMessageReader messageReader = new ODataMessageReader(requestMessage, readerSetting, WritePayloadHelper.Model);
                    ODataReader reader = messageReader.CreateODataResourceReader(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    bool verifyEntryCalled = false;
                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            ODataResource entry = reader.Item as ODataResource;
                            if (entry != null)
                            {
                                Assert.AreEqual(2, entry.Properties.Count(), "entry.Properties.Count");
                                verifyEntryCalled = true;
                            }
                            else
                            {
                                Assert.IsNull(entry);
                            }
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                    Assert.IsTrue(verifyEntryCalled, "verifyEntryCalled");
                }
            }

            // For Json light, verify the resulting metadata is as expected
            if (mimeType != MimeTypes.ApplicationAtomXml)
            {
                JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, object> resultObject = jScriptSerializer.DeserializeObject(result) as Dictionary<string, object>;

                // AutoComputePayloadMetadata has no effect on request message metadata
                Assert.AreEqual(this.ServiceUri + "$metadata#Order/$entity", resultObject.Single(e => e.Key == JsonLightConstants.ODataContextAnnotationName).Value);
                resultObject.Remove(JsonLightConstants.ODataContextAnnotationName);

                foreach (var pair in resultObject)
                {
                    Assert.IsFalse(pair.Key.Contains("odata.") || pair.Key.StartsWith("#"));
                }
            }

            return result;
        }

        /// <summary>
        /// Write derived type entries with AutoGeneratedUrlsShouldPutKeyValueInDedicatedSegment, attempt to reset AutoComputePayloadMetadata when writing
        /// </summary>
        [TestMethod]
        public void DerivedTypeFeedUrlConventionTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                foreach (var hasModel in this.hasModelFlagBools)
                {
                    var settings = new ODataMessageWriterSettings();
                    settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                    settings.BaseUri = this.ServiceUri;

                    settings.AutoComputePayloadMetadata = false;
                    string defaultModeResult = this.WriteAndVerifyEmployeeFeed(settings, mimeType, hasModel, true);

                    settings.AutoComputePayloadMetadata = true;
                    string autoComputeMetadataModeResult = this.WriteAndVerifyEmployeeFeed(settings, mimeType, hasModel, true);

                    // For Atom, verify that the result is the same for AutoComputePayloadMetadata=true/false
                    if (mimeType == MimeTypes.ApplicationAtomXml)
                    {
                        WritePayloadHelper.VerifyPayloadString(defaultModeResult, autoComputeMetadataModeResult, mimeType);
                    }
                }
            }
        }

        private string WriteAndVerifyEmployeeFeed(ODataMessageWriterSettings settings, string mimeType, bool hasModel, bool useKeyAsSegment)
        {
            // create a feed with two entries
            var employeeFeed = new ODataResourceSet();

            if (mimeType == MimeTypes.ApplicationAtomXml)
            {
                employeeFeed.Id = new Uri(this.ServiceUri + "Person/" + NameSpace + "Employee");
            }

            if (!hasModel)
            {
                employeeFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person", ExpectedTypeName = NameSpace + "Employee" });
            }

            ODataResource employeeEntry = WritePayloadHelper.CreateEmployeeEntryNoMetadata(false);
            ODataResource specialEmployeeEntry = WritePayloadHelper.CreateSpecialEmployeeEntryNoMetadata(false);

            // expected result with AutoGeneratedUrlsShouldPutKeyValueInDedicatedSegment
            Dictionary<string, object> expectedEmployeeObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.EmployeeType, "Person/-3", employeeEntry, hasModel, true);
            Dictionary<string, object> expectedSpecialEmployeeObject = WritePayloadHelper.ComputeExpectedFullMetadataEntryObject(WritePayloadHelper.SpecialEmployeeType, "Person/-10", specialEmployeeEntry, hasModel, true);

            var container = ContainerBuilderHelper.BuildContainer(null);
            ((ODataSimplifiedOptions)container.GetService(typeof(ODataSimplifiedOptions))).EnableWritingKeyAsSegment =
                useKeyAsSegment;
            // write the response message and read using ODL reader
            var responseMessage = new StreamResponseMessage(new MemoryStream())
            {
                Container = container
            };

            responseMessage.SetHeader("Content-Type", mimeType);

            string result = string.Empty;
            using (var messageWriter = this.CreateODataMessageWriter(responseMessage, settings, hasModel))
            {
                var odataWriter = this.CreateODataResourceSetWriter(messageWriter, WritePayloadHelper.PersonSet, WritePayloadHelper.EmployeeType, hasModel);
                odataWriter.WriteStart(employeeFeed);

                odataWriter.WriteStart(employeeEntry);
                odataWriter.WriteEnd();

                // toggle AutoComputePayloadMetadata, this should not affect the writing result
                settings.AutoComputePayloadMetadata = !settings.AutoComputePayloadMetadata;

                odataWriter.WriteStart(specialEmployeeEntry);
                odataWriter.WriteEnd();

                odataWriter.WriteEnd();

                result = this.ReadFeedEntryMessage(true, responseMessage, mimeType, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType);
            }

            // For Json light, verify the resulting metadata is as expected
            if (mimeType != MimeTypes.ApplicationAtomXml)
            {
                JavaScriptSerializer jScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, object> resultObject = jScriptSerializer.DeserializeObject(result) as Dictionary<string, object>;

                VerifyODataContextAnnotation(this.ServiceUri + "$metadata#Person/" + NameSpace + "Employee", resultObject, mimeType);

                settings.AutoComputePayloadMetadata = !settings.AutoComputePayloadMetadata;
                VerifyEntry(expectedEmployeeObject, employeeEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).First() as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
                VerifyEntry(expectedSpecialEmployeeObject, specialEmployeeEntry, new ODataNestedResourceInfo[] { }, new ODataProperty[] { }, (resultObject.Last().Value as object[]).Last() as Dictionary<string, object>, mimeType, settings.AutoComputePayloadMetadata);
            }

            return result;
        }

        private ODataMessageWriter CreateODataMessageWriter(StreamResponseMessage responseMessage, ODataMessageWriterSettings settings, bool hasModel)
        {
            if (hasModel)
            {
                return new ODataMessageWriter(responseMessage, settings, WritePayloadHelper.Model);
            }
            else
            {
                return new ODataMessageWriter(responseMessage, settings);
            }
        }

        private ODataMessageWriter CreateODataMessageWriter(StreamRequestMessage requestMessage, ODataMessageWriterSettings settings, bool hasModel)
        {
            if (hasModel)
            {
                return new ODataMessageWriter(requestMessage, settings, WritePayloadHelper.Model);
            }
            else
            {
                return new ODataMessageWriter(requestMessage, settings);
            }
        }

        private ODataWriter CreateODataResourceSetWriter(ODataMessageWriter messageWriter, IEdmEntitySet entitySet, IEdmEntityType entityType, bool hasModel)
        {
            if (hasModel)
            {
                return messageWriter.CreateODataResourceSetWriter(entitySet, entityType);
            }
            else
            {
                return messageWriter.CreateODataResourceSetWriter();
            }
        }

        private ODataWriter CreateODataEntryWriter(ODataMessageWriter messageWriter, IEdmEntitySet entitySet, IEdmEntityType entityType, bool hasModel)
        {
            if (hasModel)
            {
                return messageWriter.CreateODataResourceWriter(entitySet, entityType);
            }
            else
            {
                return messageWriter.CreateODataResourceWriter();
            }
        }

        private string ReadFeedEntryMessage(bool isFeed, StreamResponseMessage responseMessage, string mimeType, IEdmEntitySet edmEntitySet, IEdmEntityType edmEntityType)
        {
            Stream stream = responseMessage.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                // read the response message using ODL reader, verify the read completed successfully
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(isFeed, responseMessage, edmEntitySet, edmEntityType, null, null, null);
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private static void VerifyEntry(
            Dictionary<string, object> expectedFullMetadataObject,
            ODataResource entry,
            ODataNestedResourceInfo[] userSpecifiedNavigationLinks,
            ODataProperty[] userSpecifiedMediaProperties,
            Dictionary<string, object> metadataNotYetVerified,
            string mimeType,
            bool autoComputePayloadMetadata)
        {
            if (autoComputePayloadMetadata && !mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata))
            {
                // Verify fullmedata result against fullmetadata expected object, for nometadata, verify metadata does not exist in the result
                foreach (var pair in expectedFullMetadataObject)
                {
                    // whether the expected metadata is in the result
                    bool existInEntry = metadataNotYetVerified.Contains(pair);

                    bool isMetadata = pair.Key.Contains("odata.");

                    // special handling for actions since each action is a Dictionary<string, object> in the expect/actual with key value "#Namespace.ActionName"
                    if (pair.Key.StartsWith("#" + WritePayloadHelper.Model.EntityContainer.Namespace))
                    {
                        isMetadata = true;
                        var expectedActionObject = pair.Value as Dictionary<string, object>;
                        if (metadataNotYetVerified.ContainsKey(pair.Key))
                        {
                            var actualActionObject = metadataNotYetVerified[pair.Key] as Dictionary<string, object>;
                            existInEntry = true;
                            foreach (var item in expectedActionObject)
                            {
                                // verify action title/target
                                existInEntry = existInEntry && actualActionObject.Contains(item);
                            }
                        }
                    }

                    // whether expectedFullMetadataObject property is in the result
                    if (!isMetadata)
                    {
                        existInEntry = metadataNotYetVerified.ContainsKey(pair.Key);
                    }

                    if (mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                    {
                        // nometadata result should contain properties but not metadata
                        Assert.IsTrue(isMetadata != existInEntry, "Unexpected " + pair.Key);
                    }
                    else if (mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                    {
                        // fullmetadata result should contain properties and metadata
                        Assert.IsTrue(existInEntry, "Expected item not found " + pair.Key);
                    }

                    metadataNotYetVerified.Remove(pair.Key);
                }
            }
            else
            {
                VerifyAgainstODataEntry(entry, userSpecifiedNavigationLinks, userSpecifiedMediaProperties, metadataNotYetVerified);
            }

            // additional verification for mimimalmetadata, only include odata.type in derived type instances
            if (!autoComputePayloadMetadata || mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata))
            {
                object value = null;
                bool typeAnnotationFound = metadataNotYetVerified.TryGetValue(JsonLightConstants.ODataTypeAnnotationName, out value);
                Assert.IsTrue(!typeAnnotationFound || (typeAnnotationFound && (value as string).Contains("Employee")));

                metadataNotYetVerified.Remove(JsonLightConstants.ODataTypeAnnotationName);
            }

            // make sure all metadata in the actual object is verified
            foreach (var pair in metadataNotYetVerified)
            {
                Assert.IsFalse(pair.Key.Contains("odata.") && !(pair.Key.Contains("odata.associationLink") || pair.Key.Contains("odata.type")));
                Assert.IsFalse(pair.Key.StartsWith("#"));
            }
        }

        private static void VerifyAgainstODataEntry(ODataResource entry, ODataNestedResourceInfo[] userSpecifiedNavigationLinks, ODataProperty[] userSpecifiedMediaProperties, Dictionary<string, object> metadataNotYetVerified)
        {
            // for non-autoComputePayloadMetadata case (and minimal metatadata), writer should include user specified metadata
            object value = null;
            Assert.AreEqual(entry.Id != null, metadataNotYetVerified.TryGetValue(JsonLightConstants.ODataIdAnnotationName, out value));
            Assert.AreEqual(entry.Id, value);
            Assert.AreEqual(entry.EditLink != null, metadataNotYetVerified.TryGetValue(JsonLightConstants.ODataEditLinkAnnotationName, out value));
            Assert.IsTrue(entry.EditLink == null || entry.EditLink.AbsoluteUri == value as string);
            Assert.AreEqual(entry.ReadLink != null, metadataNotYetVerified.TryGetValue(JsonLightConstants.ODataReadLinkAnnotationName, out value));
            Assert.IsTrue(entry.ReadLink == null || entry.ReadLink.OriginalString == value as string || (value as string).EndsWith(entry.ReadLink.OriginalString));
            Assert.AreEqual(entry.ETag != null, metadataNotYetVerified.TryGetValue(JsonLightConstants.ODataETagAnnotationName, out value));
            Assert.AreEqual(entry.ETag, value);

            metadataNotYetVerified.Remove(JsonLightConstants.ODataIdAnnotationName);
            metadataNotYetVerified.Remove(JsonLightConstants.ODataEditLinkAnnotationName);
            metadataNotYetVerified.Remove(JsonLightConstants.ODataReadLinkAnnotationName);
            metadataNotYetVerified.Remove(JsonLightConstants.ODataETagAnnotationName);

            foreach (var nl in userSpecifiedNavigationLinks)
            {
                var nlKey = nl.Name + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataNavigationLinkUrlAnnotationName;
                Assert.IsTrue(metadataNotYetVerified.TryGetValue(nlKey, out value));
                Assert.IsTrue(nl.Url.OriginalString == value as string || (value as string).EndsWith(nl.Url.OriginalString));
                metadataNotYetVerified.Remove(nlKey);
                if (nl.AssociationLinkUrl != null)
                {
                    var alKey = nl.Name + JsonLightConstants.ODataPropertyAnnotationSeparator + JsonLightConstants.ODataAssociationLinkUrlAnnotationName;
                    Assert.IsTrue(metadataNotYetVerified.TryGetValue(alKey, out value));
                    Assert.IsTrue(nl.AssociationLinkUrl.OriginalString == value as string || (value as string).EndsWith(nl.AssociationLinkUrl.OriginalString));
                    metadataNotYetVerified.Remove(alKey);
                }
            }

            foreach (var mp in userSpecifiedMediaProperties)
            {
                var elKey = mp.Name + JsonLightConstants.ODataMediaEditLinkAnnotationName;
                Assert.IsTrue(metadataNotYetVerified.TryGetValue(elKey, out value));
                Assert.AreEqual((mp.Value as ODataStreamReferenceValue).EditLink.AbsoluteUri, value as string);
                metadataNotYetVerified.Remove(elKey);

                var rlKey = mp.Name + JsonLightConstants.ODataMediaReadLinkAnnotationName;
                Assert.IsTrue(metadataNotYetVerified.TryGetValue(rlKey, out value));
                Assert.AreEqual((mp.Value as ODataStreamReferenceValue).ReadLink.AbsoluteUri, value as string);
                metadataNotYetVerified.Remove(rlKey);
            }
        }

        private static void VerifyODataContextAnnotation(string expectedValue, Dictionary<string, object> metadataNotYetVerified, string mimeType)
        {
            // verify existance/value of odata.metadata annotation
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                Assert.AreEqual(expectedValue, metadataNotYetVerified.Single(e => e.Key == JsonLightConstants.ODataContextAnnotationName).Value);
                metadataNotYetVerified.Remove(JsonLightConstants.ODataContextAnnotationName);
            }
            else
            {
                Assert.IsFalse(metadataNotYetVerified.ContainsKey(JsonLightConstants.ODataContextAnnotationName));
            }
        }
    }
}
