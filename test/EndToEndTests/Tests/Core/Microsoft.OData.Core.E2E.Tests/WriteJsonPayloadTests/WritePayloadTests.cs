//-----------------------------------------------------------------------------
// <copyright file="WritePayloadTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd;
using Microsoft.OData.Edm;
using System.Text.RegularExpressions;

namespace Microsoft.OData.Core.E2E.Tests.WriteJsonPayloadTests
{
    public class WritePayloadTests : EndToEndTestBase<WritePayloadTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly IEdmModel _model;
        private static string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.EndToEnd.";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public WritePayloadTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _model = CommonEndToEndEdmModel.GetEdmModel();
        }

        public static IEnumerable<object[]> MimeTypesData
        {
            get
            {
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata };
                yield return new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata };
            }
        }

        /// <summary>
        /// Write a feed with multiple order entries. The feed/entry contains properties, navigation & association links, next link.
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void OrderFeedTest(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };
            string outputWithModel = null;
            string outputWithoutModel = null;

            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);

            var orderType = _model.FindDeclaredType(NameSpacePrefix + "Order") as IEdmEntityType;
            var orderSet = _model.EntityContainer.FindEntitySet("Orders");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter(orderSet, orderType);
                outputWithModel = WriteAndVerifyOrderFeed(responseMessageWithModel, odataWriter, true, mimeType);
            }

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceSetWriter();
                outputWithoutModel = WriteAndVerifyOrderFeed(responseMessageWithoutModel, odataWriter, false,
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
                TestsHelper.VerifyPayloadString(outputWithModel2, outputWithoutModel2, mimeType);
            }
            else if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
            {
                var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                TestsHelper.VerifyPayloadString(outputWithModel, outputWithoutModel2, mimeType);
            }
            else
            {
                //NoMetadata with/out model should result in same output
                Assert.Equal(outputWithModel, outputWithoutModel);
            }
        }

        /// <summary>
        /// Write an expanded customer entry containing primitive, complex, collection of primitive/complex properties.
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void ExpandedCustomerEntryTest(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };
            string outputWithModel = null;
            string outputWithoutModel = null;

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                outputWithoutModel = WriteAndVerifyExpandedCustomerEntry(responseMessageWithoutModel,
                                                                              odataWriter, false, mimeType);
            }
            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);

            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var customerSet = _model.EntityContainer.FindEntitySet("Customers");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(customerSet, customerType);
                outputWithModel = WriteAndVerifyExpandedCustomerEntry(responseMessageWithModel, odataWriter,
                                                                           false, mimeType);
            }

            if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
            {
                var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");
                var outputWithModel2 = rex.Replace(outputWithModel, "");
                var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                TestsHelper.VerifyPayloadString(outputWithModel2, outputWithoutModel2, mimeType);
            }
            else if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
            {
                var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                TestsHelper.VerifyPayloadString(outputWithModel, outputWithoutModel2, mimeType);
            }
            else
            {
                //NoMetadata with/out model should result in same output
                Assert.Equal(outputWithModel, outputWithoutModel);
            }
        }

        /// <summary>
        /// Write an entry containing stream, named stream
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void CarEntryTest(string mimeType)
        {
            var settings = new ODataMessageWriterSettings();
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };
            string outputWithModel = null;
            string outputWithoutModel = null;

            var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithModel.SetHeader("Content-Type", mimeType);
            responseMessageWithModel.PreferenceAppliedHeader().AnnotationFilter = "*";

            var carType = _model.FindDeclaredType(NameSpacePrefix + "Car") as IEdmEntityType;
            var carSet = _model.EntityContainer.FindEntitySet("Cars");

            using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter(carSet, carType);
                outputWithModel = WriteAndVerifyCarEntry(responseMessageWithModel, odataWriter, true, mimeType);
            }

            var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
            responseMessageWithoutModel.PreferenceAppliedHeader().AnnotationFilter = "*";
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                outputWithoutModel = WriteAndVerifyCarEntry(responseMessageWithoutModel, odataWriter, false,
                                                                 mimeType);
            }

            TestsHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
        }

        /// <summary>
        /// Write a feed containing actions, derived type entry instance
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void PersonFeedTest(string mimeType)
        {
            if (mimeType != MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
            {
                var settings = new ODataMessageWriterSettings() { BaseUri = _baseUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };

                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new TestStreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);

                var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
                var personSet = _model.EntityContainer.FindEntitySet("People");

                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, _model))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(personSet, personType);
                    outputWithModel = WriteAndVerifyPersonFeed(responseMessageWithModel, odataWriter, false,
                                                                   mimeType);
                }

                var responseMessageWithoutModel = new TestStreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    outputWithoutModel = WriteAndVerifyPersonFeed(responseMessageWithoutModel, odataWriter, false,
                                                                       mimeType);
                }

                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    TestsHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
                }
                else
                {
                    //NoMetadata with/out model should result in same output
                    Assert.Equal(outputWithModel, outputWithoutModel);
                }

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    Assert.True(outputWithoutModel.Contains(_baseUri + "$metadata#People\""));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // odata.type is included in json light payload only if entry typename is different than serialization info
                    Assert.False(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpacePrefix + "Person\","), "odata.type Person");
                    Assert.True(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpacePrefix + "Employee\","), "odata.type Employee");
                    Assert.True(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpacePrefix + "SpecialEmployee\","), "odata.type SpecialEmployee");
                }
            }
        }

        /// <summary>
        /// Write an employee entry with/without ExpectedTypeName in serialization info
        /// </summary>
        [Theory]
        [MemberData(nameof(MimeTypesData))]
        public void EmployeeEntryTest(string mimeType)
        {
            var settings = new ODataMessageWriterSettings() { BaseUri = _baseUri };
            settings.ODataUri = new ODataUri() { ServiceRoot = _baseUri };
            string outputWithTypeCast = null;
            string outputWithoutTypeCast = null;

            // employee entry as response of person(1)
            var responseMessageWithoutTypeCast = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithoutTypeCast.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithoutTypeCast, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                outputWithoutTypeCast = WriteAndVerifyEmployeeEntry(responseMessageWithoutTypeCast, odataWriter,
                                                                         false, mimeType);
            }

            // employee entry as response of person(1)/EmployeeTyeName, in this case the test sets ExpectedTypeName as Employee in Serialization info
            var responseMessageWithTypeCast = new TestStreamResponseMessage(new MemoryStream());
            responseMessageWithTypeCast.SetHeader("Content-Type", mimeType);
            using (var messageWriter = new ODataMessageWriter(responseMessageWithTypeCast, settings))
            {
                var odataWriter = messageWriter.CreateODataResourceWriter();
                outputWithTypeCast = WriteAndVerifyEmployeeEntry(responseMessageWithTypeCast, odataWriter, true,
                                                                      mimeType);
            }

            if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
            {
                // expect type cast in odata.metadata if EntitySetElementTypeName != ExpectedTypeName
                Assert.True(outputWithoutTypeCast.Contains(_baseUri + "$metadata#People/$entity"));
                Assert.True(
                    outputWithTypeCast.Contains(_baseUri + "$metadata#People/" + NameSpacePrefix +
                                                "Employee/$entity"));
            }

            if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
            {
                // write odata.type if entry TypeName != ExpectedTypeName
                Assert.True(outputWithoutTypeCast.Contains("odata.type"));
                Assert.False(outputWithTypeCast.Contains("odata.type"));
            }
        }

        private string WriteAndVerifyOrderFeed(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            var orderFeed = new ODataResourceSet()
            {
                Id = new Uri(_baseUri + "Orders"),
                NextPageLink = new Uri(_baseUri + "Orders?$skiptoken=-9"),
            };

            if (!hasModel)
            {
                orderFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Orders", NavigationSourceEntityTypeName = NameSpacePrefix + "Order" });
            }

            odataWriter.WriteStart(orderFeed);

            var orderEntry1 = TestsHelper.CreateOrderEntry1(hasModel);
            odataWriter.WriteStart(orderEntry1);

            var orderEntry1Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-10)/Customer")
            };

            odataWriter.WriteStart(orderEntry1Navigation1);
            odataWriter.WriteEnd();

            var orderEntry1Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-10)/Login")
            };

            odataWriter.WriteStart(orderEntry1Navigation2);
            odataWriter.WriteEnd();

            // Finish writing orderEntry1.
            odataWriter.WriteEnd();

            var orderEntry2Wrapper = TestsHelper.CreateOrderEntry2(hasModel);


            var orderEntry2Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-9)/Customer")
            };

            var orderEntry2Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(_baseUri + "Orders(-9)/Login")
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

            var orderType = _model.FindDeclaredType(NameSpacePrefix + "Order") as IEdmEntityType;
            var orderSet = _model.EntityContainer.FindEntitySet("Orders");

            Stream stream = responseMessage.GetStream();

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                TestsHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, orderSet, orderType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.True(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return TestsHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyExpandedCustomerEntry(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            ODataResourceWrapper customerEntry = TestsHelper.CreateCustomerEntry(hasModel);

            var loginFeed = new ODataResourceSet() { Id = new Uri(_baseUri + "Customers(-9)/Logins") };
            if (!hasModel)
            {
                loginFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Logins", NavigationSourceEntityTypeName = NameSpacePrefix + "Login", NavigationSourceKind = EdmNavigationSourceKind.EntitySet });
            }

            var loginEntry = TestsHelper.CreateLoginEntry(hasModel);


            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(TestsHelper.CreateCustomerNavigationLinks());
            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(new[]{  new ODataNestedResourceInfoWrapper()
            {
                NestedResourceInfo = new ODataNestedResourceInfo()
                {
                    Name = "Logins",
                    IsCollection = true,
                    Url = new Uri(_baseUri + "Customers(-9)/Logins")
                },
                NestedResourceOrResourceSet = new ODataResourceSetWrapper()
                {
                    ResourceSet = loginFeed,
                    Resources = new List<ODataResourceWrapper>()
                    {
                        new ODataResourceWrapper()
                        {
                            Resource = loginEntry,
                            NestedResourceInfoWrappers = TestsHelper.CreateLoginNavigationLinksWrapper().ToList()
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

            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var customerSet = _model.EntityContainer.FindEntitySet("Customers");

            Stream stream = responseMessage.GetStream();

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                TestsHelper.ReadAndVerifyFeedEntryMessage(
                    false,
                    responseMessage,
                    customerSet,
                    customerType,
                    verifyFeed,
                    verifyEntry,
                    verifyNavigation);
                Assert.True(verifyFeedCalled && verifyEntryCalled == 2 && verifyNavigationCalled, "Verification action not called.");
            }

            return TestsHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyCarEntry(
            TestStreamResponseMessage responseMessage,
            ODataWriter odataWriter,
            bool hasModel,
            string mimeType)
        {
            var carEntry = TestsHelper.CreateCarEntry(hasModel);

            odataWriter.WriteStart(carEntry);

            // Finish writing the entry.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyEntryCalled = false;
            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.Equal(4, entry.Properties.Count());
                Assert.NotNull(entry.MediaResource);
                Assert.True(entry.EditLink.AbsoluteUri.Contains("Cars(11)"), "entry.EditLink");
                Assert.True(entry.ReadLink == null || entry.ReadLink.AbsoluteUri.Contains("Cars(11)"), "entry.ReadLink");
                Assert.Equal(1, entry.InstanceAnnotations.Count);

                verifyEntryCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            var carType = _model.FindDeclaredType(NameSpacePrefix + "Car") as IEdmEntityType;
            var carSet = _model.EntityContainer.FindEntitySet("Cars");

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                TestsHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, carSet, carType, null, verifyEntry,
                                                   null);
                Assert.True(verifyEntryCalled, "Verification action not called.");
            }

            return TestsHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyPersonFeed(TestStreamResponseMessage responseMessage, ODataWriter odataWriter,
                                        bool hasModel, string mimeType)
        {
            var personFeed = new ODataResourceSet()
            {
                Id = new Uri(_baseUri + "People"),
                DeltaLink = new Uri(_baseUri + "People")
            };
            if (!hasModel)
            {
                personFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "People", NavigationSourceEntityTypeName = NameSpacePrefix + "Person" });
            }

            odataWriter.WriteStart(personFeed);

            ODataResource personEntry = TestsHelper.CreatePersonEntry(hasModel);
            odataWriter.WriteStart(personEntry);

            var personNavigation = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-5)/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(personNavigation);
            odataWriter.WriteEnd();

            // Finish writing personEntry.
            odataWriter.WriteEnd();

            ODataResource employeeEntry = TestsHelper.CreateEmployeeEntry(hasModel);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation2);
            odataWriter.WriteEnd();

            // Finish writing employeeEntry.
            odataWriter.WriteEnd();

            ODataResource specialEmployeeEntry = TestsHelper.CreateSpecialEmployeeEntry(hasModel);
            odataWriter.WriteStart(specialEmployeeEntry);

            var specialEmployeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-10)/" + NameSpacePrefix + "SpecialEmployee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation1);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("People(-10)/" + NameSpacePrefix + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation2);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation3 = new ODataNestedResourceInfo()
            {
                Name = "Car",
                IsCollection = false,
                Url = new Uri("People(-10)/" + NameSpacePrefix + "SpecialEmployee" + "/Manager", UriKind.Relative)
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
                    Assert.True(feed.DeltaLink.AbsoluteUri.Contains("People"));
                }
                verifyFeedCalled = true;
            };
            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.True(entry.EditLink.AbsoluteUri.EndsWith("People(-5)") ||
                              entry.EditLink.AbsoluteUri.EndsWith("People(-3)/" + NameSpacePrefix + "Employee") ||
                              entry.EditLink.AbsoluteUri.EndsWith("People(-10)/" + NameSpacePrefix + "SpecialEmployee"));
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.True(navigation.Name == "PersonMetadata" || navigation.Name == "Manager" || navigation.Name == "Car");
                verifyNavigationCalled = true;
            };

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = _model.EntityContainer.FindEntitySet("People");

            Stream stream = responseMessage.GetStream();

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);

                TestsHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, peopleSet, personType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.True(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return TestsHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyEmployeeEntry(TestStreamResponseMessage responseMessage, ODataWriter odataWriter,
                                        bool hasExpectedType, string mimeType)
        {

            ODataResource employeeEntry = TestsHelper.CreateEmployeeEntry(false);
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo()
            {
                NavigationSourceName = "People",
                NavigationSourceEntityTypeName = NameSpacePrefix + "Person",
            };

            if (hasExpectedType)
            {
                serializationInfo.ExpectedTypeName = NameSpacePrefix + "Employee";
            }

            employeeEntry.SetSerializationInfo(serializationInfo);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("People(-3)/" + NameSpacePrefix + "Employee" + "/Manager", UriKind.Relative)
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
                Assert.True(entry.EditLink.AbsoluteUri.Contains("People"), "entry.EditLink");
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.True(navigation.Name == "PersonMetadata" || navigation.Name == "Manager", "navigation.Name");
                verifyNavigationCalled = true;
            };

            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var peopleSet = _model.EntityContainer.FindEntitySet("People");

            Stream stream = responseMessage.GetStream();

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                TestsHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, peopleSet, personType, null,
                                                   verifyEntry, verifyNavigation);
                Assert.True(verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return TestsHelper.ReadStreamContent(stream);
        }

        private WritePayloadTestsHelper TestsHelper
        {
            get
            {
                return new WritePayloadTestsHelper(_baseUri, _model);
            }
        }
    }
}
