﻿//---------------------------------------------------------------------
// <copyright file="WriteJsonWithoutModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Write various payload with/without EDM model in Atom/VerboseJosn/JsonLight formats.
    /// </summary>
    [TestClass]
    public class WritePayloadTests : EndToEndTestBase
    {
        protected const string NameSpace = "Microsoft.Test.OData.Services.AstoriaDefaultService.";

        protected List<string> mimeTypes = new List<string>()
        {
            //MimeTypes.ApplicationAtomXml,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        };

        public WritePayloadTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        public override void CustomTestInitialize()
        {
            WritePayloadHelper.CustomTestInitialize(this.ServiceUri);
        }


        /// <summary>
        /// Write a feed with multiple order entries. The feed/entry contains properties, navigation & association links, next link.
        /// </summary>
        [TestMethod]
        public void OrderFeedTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataFeedWriter(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    outputWithModel = this.WriteAndVerifyOrderFeed(responseMessageWithModel, odataWriter, true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataFeedWriter();
                    outputWithoutModel = this.WriteAndVerifyOrderFeed(responseMessageWithoutModel, odataWriter, false,
                                                                      mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }

        /// <summary>
        /// Write an expanded customer entry containing primitive, complex, collection of primitive/complex properties.
        /// </summary>
        [TestMethod]
        public void ExpandedCustomerEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType);
                    outputWithModel = this.WriteAndVerifyExpandedCustomerEntry(responseMessageWithModel, odataWriter,
                                                                               true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter();
                    outputWithoutModel = this.WriteAndVerifyExpandedCustomerEntry(responseMessageWithoutModel,
                                                                                  odataWriter, false, mimeType);
                }

                if (mimeType != MimeTypes.ApplicationAtomXml)
                {
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
                }
            }
        }

        /// <summary>
        /// Write an entry containing stream, named stream
        /// </summary>
        [TestMethod]
        public void CarEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                responseMessageWithModel.PreferenceAppliedHeader().AnnotationFilter = "*";
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(WritePayloadHelper.CarSet, WritePayloadHelper.CarType);
                    outputWithModel = this.WriteAndVerifyCarEntry(responseMessageWithModel, odataWriter, true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                responseMessageWithoutModel.PreferenceAppliedHeader().AnnotationFilter = "*";
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter();
                    outputWithoutModel = this.WriteAndVerifyCarEntry(responseMessageWithoutModel, odataWriter, false,
                                                                     mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }

        /// <summary>
        /// Write a feed containing actions, derived type entry instance
        /// </summary>
        [TestMethod]
        public void PersonFeedTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings() { PayloadBaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataFeedWriter(WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType);
                    outputWithModel = this.WriteAndVerifyPersonFeed(responseMessageWithModel, odataWriter, true,
                                                                   mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataFeedWriter();
                    outputWithoutModel = this.WriteAndVerifyPersonFeed(responseMessageWithoutModel, odataWriter, false,
                                                                       mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    Assert.IsTrue(outputWithoutModel.Contains(this.ServiceUri + "$metadata#Person\""));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // odata.type is included in json light payload only if entry typename is different than serialization info
                    Assert.IsFalse(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "Person\","), "odata.type Person");
                    Assert.IsTrue(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "Employee\","), "odata.type Employee");
                    Assert.IsTrue(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "SpecialEmployee\","), "odata.type SpecialEmployee");
                }
            }
        }

        /// <summary>
        /// Write an employee entry with/without ExpectedTypeName in serialization info
        /// </summary>
        [TestMethod]
        public void EmployeeEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings() { PayloadBaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithTypeCast = null;
                string outputWithoutTypeCast = null;

                // employee entry as response of person(1)
                var responseMessageWithoutTypeCast = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutTypeCast.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutTypeCast, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter();
                    outputWithoutTypeCast = this.WriteAndVerifyEmployeeEntry(responseMessageWithoutTypeCast, odataWriter,
                                                                             false, mimeType);
                }

                // employee entry as response of person(1)/EmployeeTyeName, in this case the test sets ExpectedTypeName as Employee in Serialization info
                var responseMessageWithTypeCast = new StreamResponseMessage(new MemoryStream());
                responseMessageWithTypeCast.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithTypeCast, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter();
                    outputWithTypeCast = this.WriteAndVerifyEmployeeEntry(responseMessageWithTypeCast, odataWriter, true,
                                                                          mimeType);
                }

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    // expect type cast in odata.metadata if EntitySetElementTypeName != ExpectedTypeName
                    Assert.IsTrue(outputWithoutTypeCast.Contains(this.ServiceUri + "$metadata#Person/$entity"));
                    Assert.IsTrue(
                        outputWithTypeCast.Contains(this.ServiceUri + "$metadata#Person/" + NameSpace +
                                                    "Employee/$entity"));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // write odata.type if entry TypeName != ExpectedTypeName
                    Assert.IsTrue(outputWithoutTypeCast.Contains("odata.type"));
                    Assert.IsFalse(outputWithTypeCast.Contains("odata.type"));
                }
            }
        }

        /// <summary>
        /// Write collection response
        /// </summary>
        [TestMethod]
        public void CollectionTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                string testMimeType = mimeType.Contains("xml") ? MimeTypes.ApplicationXml : mimeType;

                var settings = new ODataMessageWriterSettings() { PayloadBaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataCollectionWriter(WritePayloadHelper.ContactDetailType);
                    outputWithModel = this.WriteAndVerifyCollection(responseMessageWithModel, odataWriter, true,
                                                                    testMimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataCollectionWriter();
                    outputWithoutModel = this.WriteAndVerifyCollection(responseMessageWithoutModel, odataWriter, false,
                                                                       testMimeType);
                }

                Assert.AreEqual(outputWithModel, outputWithoutModel);
            }
        }

        /// <summary>
        /// Write $ref response
        /// </summary>
        [TestMethod]
        public void LinksTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                if (mimeType.Equals(MimeTypes.ApplicationXml)) { continue; }
                string testMimeType = mimeType;
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                var responseMessage = new StreamResponseMessage(new MemoryStream());
                responseMessage.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessage, settings, WritePayloadHelper.Model))
                {
                    this.WriteAndVerifyLinks(responseMessage, messageWriter, testMimeType);
                }
            }
        }

        /// <summary>
        /// Write $ref response with a single link
        /// </summary>
        [TestMethod]
        public void SingleLinkTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                string testMimeType = mimeType.Contains("xml") ? MimeTypes.ApplicationXml : mimeType;

                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                var responseMessage = new StreamResponseMessage(new MemoryStream());
                responseMessage.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessage, settings, WritePayloadHelper.Model))
                {
                    this.WriteAndVerifySingleLink(responseMessage, messageWriter, testMimeType);
                }
            }
        }

        /// <summary>
        /// Write a request message with an entry
        /// </summary>
        [TestMethod]
        public void RequestMessageTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var requestMessageWithModel = new StreamRequestMessage(new MemoryStream(),
                                                                       new Uri(this.ServiceUri + "Order"), "POST");
                requestMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(requestMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    outputWithModel = this.WriteAndVerifyRequestMessage(requestMessageWithModel, odataWriter,
                                                                        true, mimeType);
                }

                var requestMessageWithoutModel = new StreamRequestMessage(new MemoryStream(),
                                                                          new Uri(this.ServiceUri + "Order"), "POST");
                requestMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(requestMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataEntryWriter();
                    outputWithoutModel = this.WriteAndVerifyRequestMessage(requestMessageWithoutModel,
                                                                           odataWriter, false, mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }


        private string WriteAndVerifyOrderFeed(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                               bool hasModel, string mimeType)
        {
            var orderFeed = new ODataFeed()
            {
                Id = new Uri(this.ServiceUri + "Order"),
                NextPageLink = new Uri(this.ServiceUri + "Order?$skiptoken=-9"),
            };
            if (!hasModel)
            {
                orderFeed.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
            }

            odataWriter.WriteStart(orderFeed);

            var orderEntry1 = WritePayloadHelper.CreateOrderEntry1(hasModel);
            odataWriter.WriteStart(orderEntry1);

            var orderEntry1Navigation1 = new ODataNavigationLink()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-10)/Customer")
            };
            odataWriter.WriteStart(orderEntry1Navigation1);
            odataWriter.WriteEnd();

            var orderEntry1Navigation2 = new ODataNavigationLink()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-10)/Login")
            };
            odataWriter.WriteStart(orderEntry1Navigation2);
            odataWriter.WriteEnd();

            // Finish writing orderEntry1.
            odataWriter.WriteEnd();

            var orderEntry2 = WritePayloadHelper.CreateOrderEntry2(hasModel);
            odataWriter.WriteStart(orderEntry2);

            var orderEntry2Navigation1 = new ODataNavigationLink()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-9)/Customer")
            };
            odataWriter.WriteStart(orderEntry2Navigation1);
            odataWriter.WriteEnd();

            var orderEntry2Navigation2 = new ODataNavigationLink()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-9)/Login")
            };
            odataWriter.WriteStart(orderEntry2Navigation2);
            odataWriter.WriteEnd();

            // Finish writing orderEntry2.
            odataWriter.WriteEnd();

            // Finish writing the feed.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;
            Action<ODataFeed> verifyFeed = (feed) =>
            {
                Assert.IsNotNull(feed.NextPageLink, "feed.NextPageLink");
                verifyFeedCalled = true;
            };
            Action<ODataEntry> verifyEntry = (entry) =>
            {
                Assert.AreEqual(3, entry.Properties.Count(), "entry.Properties.Count");
                verifyEntryCalled = true;
            };
            Action<ODataNavigationLink> verifyNavigation = (navigation) =>
            {
                Assert.IsTrue(navigation.Name == "Customer" || navigation.Name == "Login", "navigation.Name");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyExpandedCustomerEntry(StreamResponseMessage responseMessage,
                                                           ODataWriter odataWriter, bool hasModel, string mimeType)
        {
            ODataEntry customerEntry = WritePayloadHelper.CreateCustomerEntry(hasModel);
            odataWriter.WriteStart(customerEntry);

            // write non-expanded navigations
            foreach (var navigation in WritePayloadHelper.CreateCustomerNavigationLinks())
            {
                odataWriter.WriteStart(navigation);
                odataWriter.WriteEnd();
            }

            // write expanded navigation
            var expandedNavigation = new ODataNavigationLink()
            {
                Name = "Logins",
                IsCollection = true,
                Url = new Uri(this.ServiceUri + "Customer(-9)/Logins")
            };
            odataWriter.WriteStart(expandedNavigation);

            var loginFeed = new ODataFeed() { Id = new Uri(this.ServiceUri + "Customer(-9)/Logins") };
            if (!hasModel)
            {
                loginFeed.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo() { NavigationSourceName = "Login", NavigationSourceEntityTypeName = NameSpace + "Login" });
            }

            odataWriter.WriteStart(loginFeed);

            var loginEntry = WritePayloadHelper.CreateLoginEntry(hasModel);
            odataWriter.WriteStart(loginEntry);

            foreach (var navigation in WritePayloadHelper.CreateLoginNavigationLinks())
            {
                odataWriter.WriteStart(navigation);
                odataWriter.WriteEnd();
            }

            // Finish writing loginEntry.
            odataWriter.WriteEnd();

            // Finish writing the loginFeed.
            odataWriter.WriteEnd();

            // Finish writing expandedNavigation.
            odataWriter.WriteEnd();

            // Finish writing customerEntry.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            int verifyEntryCalled = 0;
            bool verifyNavigationCalled = false;
            Action<ODataFeed> verifyFeed = (feed) =>
            {
                verifyFeedCalled = true;
            };

            Action<ODataEntry> verifyEntry = (entry) =>
            {
                if (entry.TypeName.Contains("Customer"))
                {
                    Assert.AreEqual(7, entry.Properties.Count());
                }

                if (entry.TypeName.Contains("Login"))
                {
                    Assert.AreEqual(2, entry.Properties.Count());
                }

                verifyEntryCalled++;
            };

            Action<ODataNavigationLink> verifyNavigation = (navigation) =>
            {
                Assert.IsNotNull(navigation.Name);
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType,
                                                   verifyFeed, verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyFeedCalled && verifyEntryCalled == 2 && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyCarEntry(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                              bool hasModel, string mimeType)
        {
            var carEntry = WritePayloadHelper.CreateCarEntry(hasModel);

            odataWriter.WriteStart(carEntry);

            // Finish writing the entry.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyEntryCalled = false;
            Action<ODataEntry> verifyEntry = (entry) =>
            {
                Assert.AreEqual(4, entry.Properties.Count(), "entry.Properties.Count");
                Assert.IsNotNull(entry.MediaResource, "entry.MediaResource");
                Assert.IsTrue(entry.EditLink.AbsoluteUri.Contains("Car(11)"), "entry.EditLink");
                Assert.IsTrue(entry.ReadLink == null || entry.ReadLink.AbsoluteUri.Contains("Car(11)"), "entry.ReadLink");
                Assert.AreEqual(1, entry.InstanceAnnotations.Count, "entry.InstanceAnnotations.Count");

                verifyEntryCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CarSet, WritePayloadHelper.CarType, null, verifyEntry,
                                                   null);
                Assert.IsTrue(verifyEntryCalled, "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyPersonFeed(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasModel, string mimeType)
        {
            var personFeed = new ODataFeed()
            {
                Id = new Uri(this.ServiceUri + "Person"),
                DeltaLink = new Uri(this.ServiceUri + "Person")
            };
            if (!hasModel)
            {
                personFeed.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });
            }

            odataWriter.WriteStart(personFeed);

            ODataEntry personEntry = WritePayloadHelper.CreatePersonEntry(hasModel);
            odataWriter.WriteStart(personEntry);

            var personNavigation = new ODataNavigationLink()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-5)/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(personNavigation);
            odataWriter.WriteEnd();

            // Finish writing personEntry.
            odataWriter.WriteEnd();

            ODataEntry employeeEntry = WritePayloadHelper.CreateEmployeeEntry(hasModel);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNavigationLink()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNavigationLink()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation2);
            odataWriter.WriteEnd();

            // Finish writing employeeEntry.
            odataWriter.WriteEnd();

            ODataEntry specialEmployeeEntry = WritePayloadHelper.CreateSpecialEmployeeEntry(hasModel);
            odataWriter.WriteStart(specialEmployeeEntry);

            var specialEmployeeNavigation1 = new ODataNavigationLink()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation1);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation2 = new ODataNavigationLink()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation2);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation3 = new ODataNavigationLink()
            {
                Name = "Car",
                IsCollection = false,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation3);
            odataWriter.WriteEnd();

            // Finish writing specialEmployeeEntry.
            odataWriter.WriteEnd();

            // Finish writing the feed.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;
            Action<ODataFeed> verifyFeed = (feed) =>
            {
                if (mimeType != MimeTypes.ApplicationAtomXml)
                {
                    Assert.IsTrue(feed.DeltaLink.AbsoluteUri.Contains("Person"));
                }
                verifyFeedCalled = true;
            };
            Action<ODataEntry> verifyEntry = (entry) =>
                {
                    Assert.IsTrue(entry.EditLink.AbsoluteUri.EndsWith("Person(-5)") ||
                                  entry.EditLink.AbsoluteUri.EndsWith("Person(-3)/" + NameSpace + "Employee") ||
                                  entry.EditLink.AbsoluteUri.EndsWith("Person(-10)/" + NameSpace + "SpecialEmployee"));
                    verifyEntryCalled = true;
                };
            Action<ODataNavigationLink> verifyNavigation = (navigation) =>
            {
                Assert.IsTrue(navigation.Name == "PersonMetadata" || navigation.Name == "Manager" || navigation.Name == "Car");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyEmployeeEntry(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasExpectedType, string mimeType)
        {

            ODataEntry employeeEntry = WritePayloadHelper.CreateEmployeeEntry(false);
            ODataFeedAndEntrySerializationInfo serializationInfo = new ODataFeedAndEntrySerializationInfo()
                {
                    NavigationSourceName = "Person",
                    NavigationSourceEntityTypeName = NameSpace + "Person",
                };

            if (hasExpectedType)
            {
                serializationInfo.ExpectedTypeName = NameSpace + "Employee";
            }

            employeeEntry.SetSerializationInfo(serializationInfo);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNavigationLink()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNavigationLink()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation2);
            odataWriter.WriteEnd();

            // Finish writing employeeEntry.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;

            Action<ODataEntry> verifyEntry = (entry) =>
            {
                Assert.IsTrue(entry.EditLink.AbsoluteUri.Contains("Person"), "entry.EditLink");
                verifyEntryCalled = true;
            };
            Action<ODataNavigationLink> verifyNavigation = (navigation) =>
            {
                Assert.IsTrue(navigation.Name == "PersonMetadata" || navigation.Name == "Manager", "navigation.Name");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, null,
                                                   verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyCollection(StreamResponseMessage responseMessage, ODataCollectionWriter odataWriter,
                                                bool hasModel, string mimeType)
        {
            var collectionStart = new ODataCollectionStart() { Name = "BackupContactInfo", Count = 12, NextPageLink = new Uri("http://localhost")};
            if (!hasModel)
            {
                collectionStart.SetSerializationInfo(new ODataCollectionStartSerializationInfo()
                {
                    CollectionTypeName = "Collection(" + NameSpace + "ContactDetails)"
                });
            }

            odataWriter.WriteStart(collectionStart);
            odataWriter.WriteItem(WritePayloadHelper.CreatePrimaryContactODataComplexValue());
            odataWriter.WriteEnd();

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);
                ODataCollectionReader reader = messageReader.CreateODataCollectionReader(WritePayloadHelper.ContactDetailType);
                bool collectionRead = false;
                while (reader.Read())
                {
                    if (reader.State == ODataCollectionReaderState.CollectionEnd)
                    {
                        collectionRead = true;
                    }
                }

                Assert.IsTrue(collectionRead, "collectionRead");
                Assert.AreEqual(ODataCollectionReaderState.Completed, reader.State);
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyLinks(StreamResponseMessage responseMessage, ODataMessageWriter messageWriter, string mimeType)
        {
            var links = new ODataEntityReferenceLinks()
            {
                Links = new[]
                        {
                            new ODataEntityReferenceLink() {Url = new Uri(this.ServiceUri + "Order(-10)")},
                            new ODataEntityReferenceLink() {Url = new Uri(this.ServiceUri + "Order(-7)")},
                        },
                NextPageLink = new Uri(this.ServiceUri + "Customer(-10)/Orders/$ref?$skiptoken=-7")
            };

            messageWriter.WriteEntityReferenceLinks(links);

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);

                ODataEntityReferenceLinks linksRead = messageReader.ReadEntityReferenceLinks();
                Assert.AreEqual(2, linksRead.Links.Count(), "linksRead.Links.Count");
                Assert.IsNotNull(linksRead.NextPageLink, "linksRead.NextPageLink");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifySingleLink(StreamResponseMessage responseMessage, ODataMessageWriter messageWriter, string mimeType)
        {
            var link = new ODataEntityReferenceLink() { Url = new Uri(this.ServiceUri + "Order(-10)") };

            messageWriter.WriteEntityReferenceLink(link);
            var stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);

                ODataEntityReferenceLink linkRead = messageReader.ReadEntityReferenceLink();
                Assert.IsTrue(linkRead.Url.AbsoluteUri.Contains("Order(-10)"), "linkRead.Url");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyRequestMessage(StreamRequestMessage requestMessageWithoutModel,
                                                    ODataWriter odataWriter, bool hasModel, string mimeType)
        {
            var order = new ODataEntry()
                {
                    Id = new Uri(this.ServiceUri + "Order(-10)"),
                    TypeName = NameSpace + "Order"
                };

            var orderP1 = new ODataProperty { Name = "OrderId", Value = -10 };
            var orderp2 = new ODataProperty { Name = "CustomerId", Value = 8212 };
            var orderp3 = new ODataProperty { Name = "Concurrency", Value = null };
            order.Properties = new[] { orderP1, orderp2, orderp3 };
            if (!hasModel)
            {
                order.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
                orderP1.SetSerializationInfo(new ODataPropertySerializationInfo() { PropertyKind = ODataPropertyKind.Key });
            }

            odataWriter.WriteStart(order);
            odataWriter.WriteEnd();

            Stream stream = requestMessageWithoutModel.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };
                ODataMessageReader messageReader = new ODataMessageReader(requestMessageWithoutModel, settings,
                                                                          WritePayloadHelper.Model);
                ODataReader reader = messageReader.CreateODataEntryReader(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                bool verifyEntryCalled = false;
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.EntryEnd)
                    {
                        ODataEntry entry = reader.Item as ODataEntry;
                        Assert.IsTrue(entry.Id.ToString().Contains("Order(-10)"), "entry.Id");
                        Assert.AreEqual(3, entry.Properties.Count(), "entry.Properties.Count");
                        verifyEntryCalled = true;
                    }
                }

                Assert.AreEqual(ODataReaderState.Completed, reader.State);
                Assert.IsTrue(verifyEntryCalled, "verifyEntryCalled");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }
    }
}
