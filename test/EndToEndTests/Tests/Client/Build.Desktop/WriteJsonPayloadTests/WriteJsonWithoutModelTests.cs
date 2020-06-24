//---------------------------------------------------------------------
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
    using System.Text.RegularExpressions;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Write various payload with/without EDM model in Atom/VerboseJosn/JsonLight formats.
    /// </summary>
    public class WritePayloadTests : EndToEndTestBase
    {
        protected const string NameSpace = "Microsoft.Test.OData.Services.AstoriaDefaultService.";

        protected List<string> mimeTypes = new List<string>()
        {
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        };

        public WritePayloadTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        public override void CustomTestInitialize()
        {
            WritePayloadHelper.CustomTestInitialize(this.ServiceUri);
        }


        /// <summary>
        /// Write a feed with multiple order entries. The feed/entry contains properties, navigation & association links, next link.
        /// </summary>
        [Fact]
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
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    outputWithModel = this.WriteAndVerifyOrderFeed(responseMessageWithModel, odataWriter, true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    outputWithoutModel = this.WriteAndVerifyOrderFeed(responseMessageWithoutModel, odataWriter, false,
                                                                      mimeType);
                }

                //var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                //outputWithoutModel = rex.Replace(outputWithoutModel, "");
                //WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");
                    var outputWithModel2 = rex.Replace(outputWithModel, "");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel2, outputWithoutModel2, mimeType);
                }
                else if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel2, mimeType);
                }
                else
                {
                    //NoMetadata with/out model should result in same output
                    Assert.Equal(outputWithModel, outputWithoutModel);
                }
            }
        }

        /// <summary>
        /// Write an expanded customer entry containing primitive, complex, collection of primitive/complex properties.
        /// </summary>
        [Fact]
        public void ExpandedCustomerEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutModel = this.WriteAndVerifyExpandedCustomerEntry(responseMessageWithoutModel,
                                                                                  odataWriter, false, mimeType);
                }
                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType);
                    outputWithModel = this.WriteAndVerifyExpandedCustomerEntry(responseMessageWithModel, odataWriter,
                                                                               false, mimeType);
                }

                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");
                    var outputWithModel2 = rex.Replace(outputWithModel, "");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel2, outputWithoutModel2, mimeType);
                }
                else if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel2, mimeType);
                }
                else
                {
                    //NoMetadata with/out model should result in same output
                    Assert.Equal(outputWithModel, outputWithoutModel);
                }
            }
        }

        /// <summary>
        /// Write an entry containing stream, named stream
        /// </summary>
        [Fact]
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
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.CarSet, WritePayloadHelper.CarType);
                    outputWithModel = this.WriteAndVerifyCarEntry(responseMessageWithModel, odataWriter, true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                responseMessageWithoutModel.PreferenceAppliedHeader().AnnotationFilter = "*";
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutModel = this.WriteAndVerifyCarEntry(responseMessageWithoutModel, odataWriter, false,
                                                                     mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }

        /// <summary>
        /// Write a feed containing actions, derived type entry instance
        /// </summary>
        [Fact]
        public void PersonFeedTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
                {
                    continue;
                }

                var settings = new ODataMessageWriterSettings() { BaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType);
                    outputWithModel = this.WriteAndVerifyPersonFeed(responseMessageWithModel, odataWriter, false,
                                                                   mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    outputWithoutModel = this.WriteAndVerifyPersonFeed(responseMessageWithoutModel, odataWriter, false,
                                                                       mimeType);
                }

                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
                }
                else
                {
                    //NoMetadata with/out model should result in same output
                    Assert.Equal(outputWithModel, outputWithoutModel);
                }

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    Assert.True(outputWithoutModel.Contains(this.ServiceUri + "$metadata#Person\""));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // odata.type is included in json light payload only if entry typename is different than serialization info
                    Assert.False(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "Person\","), "odata.type Person");
                    Assert.True(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "Employee\","), "odata.type Employee");
                    Assert.True(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "SpecialEmployee\","), "odata.type SpecialEmployee");
                }
            }
        }

        /// <summary>
        /// Write an employee entry with/without ExpectedTypeName in serialization info
        /// </summary>
        [Fact]
        public void EmployeeEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings() { BaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithTypeCast = null;
                string outputWithoutTypeCast = null;

                // employee entry as response of person(1)
                var responseMessageWithoutTypeCast = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutTypeCast.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutTypeCast, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutTypeCast = this.WriteAndVerifyEmployeeEntry(responseMessageWithoutTypeCast, odataWriter,
                                                                             false, mimeType);
                }

                // employee entry as response of person(1)/EmployeeTyeName, in this case the test sets ExpectedTypeName as Employee in Serialization info
                var responseMessageWithTypeCast = new StreamResponseMessage(new MemoryStream());
                responseMessageWithTypeCast.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithTypeCast, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithTypeCast = this.WriteAndVerifyEmployeeEntry(responseMessageWithTypeCast, odataWriter, true,
                                                                          mimeType);
                }

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    // expect type cast in odata.metadata if EntitySetElementTypeName != ExpectedTypeName
                    Assert.True(outputWithoutTypeCast.Contains(this.ServiceUri + "$metadata#Person/$entity"));
                    Assert.True(
                        outputWithTypeCast.Contains(this.ServiceUri + "$metadata#Person/" + NameSpace +
                                                    "Employee/$entity"));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // write odata.type if entry TypeName != ExpectedTypeName
                    Assert.True(outputWithoutTypeCast.Contains("odata.type"));
                    Assert.False(outputWithTypeCast.Contains("odata.type"));
                }
            }
        }

        /// <summary>
        /// Write complex collection response
        /// </summary>
        [Fact]
        public void ComplexCollectionTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                string testMimeType = mimeType.Contains("xml") ? MimeTypes.ApplicationXml : mimeType;

                var settings = new ODataMessageWriterSettings() { BaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(null, WritePayloadHelper.ContactDetailType);
                    outputWithModel = this.WriteAndVerifyCollection(responseMessageWithModel, odataWriter, true,
                                                                    testMimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(null, WritePayloadHelper.ContactDetailType);
                    outputWithoutModel = this.WriteAndVerifyCollection(responseMessageWithoutModel, odataWriter, false,
                                                                       testMimeType);
                }

                Assert.Equal(outputWithModel, outputWithoutModel);
            }
        }

        /// <summary>
        /// Write $ref response
        /// </summary>
        [Fact]
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
        [Fact]
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
        [Fact]
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
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    outputWithModel = this.WriteAndVerifyRequestMessage(requestMessageWithModel, odataWriter,
                                                                        true, mimeType);
                }

                var requestMessageWithoutModel = new StreamRequestMessage(new MemoryStream(),
                                                                          new Uri(this.ServiceUri + "Order"), "POST");
                requestMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(requestMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutModel = this.WriteAndVerifyRequestMessage(requestMessageWithoutModel,
                                                                           odataWriter, false, mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }


        private string WriteAndVerifyOrderFeed(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                               bool hasModel, string mimeType)
        {
            var orderFeed = new ODataResourceSet()
            {
                Id = new Uri(this.ServiceUri + "Order"),
                NextPageLink = new Uri(this.ServiceUri + "Order?$skiptoken=-9"),
            };
            if (!hasModel)
            {
                orderFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
            }

            odataWriter.WriteStart(orderFeed);

            var orderEntry1 = WritePayloadHelper.CreateOrderEntry1(hasModel);
            odataWriter.WriteStart(orderEntry1);

            var orderEntry1Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-10)/Customer")
            };
            odataWriter.WriteStart(orderEntry1Navigation1);
            odataWriter.WriteEnd();

            var orderEntry1Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-10)/Login")
            };
            odataWriter.WriteStart(orderEntry1Navigation2);
            odataWriter.WriteEnd();

            // Finish writing orderEntry1.
            odataWriter.WriteEnd();

            var orderEntry2Wrapper = WritePayloadHelper.CreateOrderEntry2(hasModel);


            var orderEntry2Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-9)/Customer")
            };

            var orderEntry2Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-9)/Login")
            };
            orderEntry2Wrapper.NestedResourceInfoWrappers = orderEntry2Wrapper.NestedResourceInfoWrappers.Concat(
                new[]
                {
                    new ODataNestedResourceInfoWrapper() { NestedResourceInfo = orderEntry1Navigation1 },
                    new ODataNestedResourceInfoWrapper() { NestedResourceInfo = orderEntry2Navigation2 }
                });

            ODataWriterHelper.WriteResource(odataWriter, orderEntry2Wrapper);

            // Finish writing the feed.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                Assert.NotNull(feed.NextPageLink);
                verifyFeedCalled = true;
            };
            Action<ODataResource> verifyEntry = (entry) =>
            {
                if (entry.TypeName.Contains("Order"))
                {
                    //entry.Properties.Count
                    Assert.Equal(2, entry.Properties.Count());
                }
                else
                {
                    Assert.True(entry.TypeName.Contains("ConcurrencyInfo"), "complex Property Concurrency should be read into ODataResource");
                }
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.True(navigation.Name == "Customer" || navigation.Name == "Login" || navigation.Name == "Concurrency", "navigation.Name");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.True(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyExpandedCustomerEntry(StreamResponseMessage responseMessage,
                                                           ODataWriter odataWriter, bool hasModel, string mimeType)
        {
            ODataResourceWrapper customerEntry = WritePayloadHelper.CreateCustomerEntry(hasModel);

            var loginFeed = new ODataResourceSet() { Id = new Uri(this.ServiceUri + "Customer(-9)/Logins") };
            if (!hasModel)
            {
                loginFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Login", NavigationSourceEntityTypeName = NameSpace + "Login", NavigationSourceKind = Microsoft.OData.Edm.EdmNavigationSourceKind.EntitySet });
            }

            var loginEntry = WritePayloadHelper.CreateLoginEntry(hasModel);


            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(WritePayloadHelper.CreateCustomerNavigationLinks());
            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(new[]{  new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Logins",
                    IsCollection = true,
                    Url = new Uri(this.ServiceUri + "Customer(-9)/Logins")
                },
                NestedResourceOrResourceSet = new ODataResourceSetWrapper()
                {
                    ResourceSet = loginFeed,
                    Resources = new List<ODataResourceWrapper>()
                    {
                        new ODataResourceWrapper()
                        {
                            Resource = loginEntry,
                            NestedResourceInfoWrappers = WritePayloadHelper.CreateLoginNavigationLinksWrapper().ToList()
                        }
                    }
                }
            }});

            ODataWriterHelper.WriteResource(odataWriter, customerEntry);

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            int verifyEntryCalled = 0;
            bool verifyNavigationCalled = false;
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                verifyFeedCalled = true;
            };

            Action<ODataResource> verifyEntry = (entry) =>
            {
                if (entry.TypeName.Contains("Customer"))
                {
                    Assert.Equal(4, entry.Properties.Count());
                    verifyEntryCalled++;
                }

                if (entry.TypeName.Contains("Login"))
                {
                    Assert.Equal(2, entry.Properties.Count());
                    verifyEntryCalled++;
                }
            };

            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.NotNull(navigation.Name);
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType,
                                                   verifyFeed, verifyEntry, verifyNavigation);
                Assert.True(verifyFeedCalled && verifyEntryCalled == 2 && verifyNavigationCalled,
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
            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.Equal(4, entry.Properties.Count());
                Assert.NotNull(entry.MediaResource);
                Assert.True(entry.EditLink.AbsoluteUri.Contains("Car(11)"), "entry.EditLink");
                Assert.True(entry.ReadLink == null || entry.ReadLink.AbsoluteUri.Contains("Car(11)"), "entry.ReadLink");
                Assert.Equal(1, entry.InstanceAnnotations.Count);

                verifyEntryCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CarSet, WritePayloadHelper.CarType, null, verifyEntry,
                                                   null);
                Assert.True(verifyEntryCalled, "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyPersonFeed(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasModel, string mimeType)
        {
            var personFeed = new ODataResourceSet()
            {
                Id = new Uri(this.ServiceUri + "Person"),
                DeltaLink = new Uri(this.ServiceUri + "Person")
            };
            if (!hasModel)
            {
                personFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });
            }

            odataWriter.WriteStart(personFeed);

            ODataResource personEntry = WritePayloadHelper.CreatePersonEntry(hasModel);
            odataWriter.WriteStart(personEntry);

            var personNavigation = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-5)/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(personNavigation);
            odataWriter.WriteEnd();

            // Finish writing personEntry.
            odataWriter.WriteEnd();

            ODataResource employeeEntry = WritePayloadHelper.CreateEmployeeEntry(hasModel);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation2);
            odataWriter.WriteEnd();

            // Finish writing employeeEntry.
            odataWriter.WriteEnd();

            ODataResource specialEmployeeEntry = WritePayloadHelper.CreateSpecialEmployeeEntry(hasModel);
            odataWriter.WriteStart(specialEmployeeEntry);

            var specialEmployeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation1);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation2);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation3 = new ODataNestedResourceInfo()
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
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                if (mimeType != MimeTypes.ApplicationAtomXml)
                {
                    Assert.True(feed.DeltaLink.AbsoluteUri.Contains("Person"));
                }
                verifyFeedCalled = true;
            };
            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.True(entry.EditLink.AbsoluteUri.EndsWith("Person(-5)") ||
                              entry.EditLink.AbsoluteUri.EndsWith("Person(-3)/" + NameSpace + "Employee") ||
                              entry.EditLink.AbsoluteUri.EndsWith("Person(-10)/" + NameSpace + "SpecialEmployee"));
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.True(navigation.Name == "PersonMetadata" || navigation.Name == "Manager" || navigation.Name == "Car");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.True(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyEmployeeEntry(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasExpectedType, string mimeType)
        {

            ODataResource employeeEntry = WritePayloadHelper.CreateEmployeeEntry(false);
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo()
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

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNestedResourceInfo()
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

            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.True(entry.EditLink.AbsoluteUri.Contains("Person"), "entry.EditLink");
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.True(navigation.Name == "PersonMetadata" || navigation.Name == "Manager", "navigation.Name");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, null,
                                                   verifyEntry, verifyNavigation);
                Assert.True(verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyCollection(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasModel, string mimeType)
        {
            var resourceSet = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet
                {
                    Count = 12,
                    NextPageLink = new Uri("http://localhost")
                },
                Resources = new List<ODataResourceWrapper>()
                {
                    WritePayloadHelper.CreatePrimaryContactODataWrapper()
                }
            };

            if (!hasModel)
            {
                resourceSet.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo()
                {
                    ExpectedTypeName = NameSpace + "ContactDetails"
                });
            }

            ODataWriterHelper.WriteResourceSet(odataWriter, resourceSet);

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);
                ODataReader reader = messageReader.CreateODataResourceSetReader(WritePayloadHelper.ContactDetailType);
                bool collectionRead = false;
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        collectionRead = true;
                    }
                }

                Assert.True(collectionRead, "collectionRead");
                Assert.Equal(ODataReaderState.Completed, reader.State);
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
                Assert.Equal(2, linksRead.Links.Count());
                Assert.NotNull(linksRead.NextPageLink);
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
                Assert.True(linkRead.Url.AbsoluteUri.Contains("Order(-10)"), "linkRead.Url");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyRequestMessage(StreamRequestMessage requestMessageWithoutModel,
                                                    ODataWriter odataWriter, bool hasModel, string mimeType)
        {
            var order = new ODataResource()
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
                order.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
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
                ODataReader reader = messageReader.CreateODataResourceReader(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                ODataResource entry = null;
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = reader.Item as ODataResource;
                    }
                }

                Assert.True(entry.Id.ToString().Contains("Order(-10)"), "entry.Id");
                Assert.Equal(2, entry.Properties.Count());
                Assert.Equal(ODataReaderState.Completed, reader.State);
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }
    }
}