//-----------------------------------------------------------------------------
// <copyright file="DeltaTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Net;
using System.Text;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.E2E.TestCommon;
using Microsoft.OData.E2E.TestCommon.Common;
using Microsoft.OData.E2E.TestCommon.Common.Client.Default.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.Default;
using Microsoft.OData.E2E.TestCommon.Common.Server.DeltaTests;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;

namespace Microsoft.OData.Core.E2E.Tests.DeltaTests
{
    public class DeltaTests : EndToEndTestBase<DeltaTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private readonly IEdmModel _model;
        private static readonly string NameSpacePrefix = "Microsoft.OData.E2E.TestCommon.Common.Server.Default.";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(MetadataController), typeof(DeltaTestsController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", DefaultEdmModel.GetEdmModel()));
            }
        }

        public DeltaTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = DefaultEdmModel.GetEdmModel();
            ResetDefaultDataSource();
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task ADeltaResponse_IsWittenAndReadSuccessfully_WithMinimalAndFullMimeTypes(string mimeType)
        {
            var customersSet = _model.FindDeclaredEntitySet("Customers");
            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var requestUri = new Uri(_baseUri, "Customers?$expand=Orders");

            var settings = CreateODataMessageWriterSettings(requestUri);

            var responseMessage = new TestStreamRequestMessage(new MemoryStream(), requestUri, "GET");
            responseMessage.SetHeader("Content-Type", mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings, _model);
            var odataWriter = await messageWriter.CreateODataDeltaResourceSetWriterAsync(customersSet, customerType);

            var deltaFeed = new ODataDeltaResourceSet();

            await odataWriter.WriteStartAsync(deltaFeed);

            // Modified Entity
            var deltaEntry = new ODataResource
            {
                Id = new Uri(_baseUri, customersSet.Name + "(1)"),
                Properties = new[]
                {
                    new ODataProperty { Name = "FirstName", Value = "GGGG" }
                }
            };

            await odataWriter.WriteStartAsync(deltaEntry);
            await odataWriter.WriteEndAsync();

            // Deleted Link
            var deletedLink = new ODataDeltaDeletedLink(
                new Uri(_baseUri, customersSet.Name + "(1)"),
                new Uri(_baseUri, "Orders(8)"),
                "Orders"
            );

            await odataWriter.WriteDeltaDeletedLinkAsync(deletedLink);

            // Added Link
            var addedLink = new ODataDeltaLink(
                new Uri(_baseUri, customersSet.Name + "(1)"),
                new Uri(_baseUri, "Orders(7)"),
                "Orders"
            );

            await odataWriter.WriteDeltaLinkAsync(addedLink);

            // Navigation Entry (Order)
            var navigationEntry = new ODataResource
            {
                Id = new Uri(_baseUri, "Orders(100)"),
                TypeName = NameSpacePrefix + "Order",
                Properties = new[]
                {
                    new ODataProperty { Name = "OrderID", Value = 100 },
                    new ODataProperty { Name = "OrderDate", Value = new DateTimeOffset(2025, 3, 6, 8, 30, 0, TimeSpan.Zero) }
                }
            };

            navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = NameSpacePrefix + "Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            });

            await odataWriter.WriteStartAsync(navigationEntry);
            await odataWriter.WriteEndAsync();

            // Deleted Entity
            var deletedEntry = new ODataDeletedResource()
            {
                Id = new Uri(_baseUri, customersSet.Name + "(2)"),
                Reason = DeltaDeletedEntryReason.Deleted
            };

            await odataWriter.WriteStartAsync(deletedEntry);
            await odataWriter.WriteEndAsync();

            await odataWriter.WriteEndAsync(); // Close deltaFeed
            await odataWriter.FlushAsync();

            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(stream, leaveOpen: true);
            var content = await streamReader.ReadToEndAsync();

            Assert.Equal("{\"@odata.context\":\"http://localhost/odata/$metadata#Customers(Orders())/$delta\",\"value\":[{\"@odata.id\":\"http://localhost/odata/Customers(1)\",\"FirstName\":\"GGGG\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Customers/$deletedLink\",\"source\":\"http://localhost/odata/Customers(1)\",\"relationship\":\"Orders\",\"target\":\"http://localhost/odata/Orders(8)\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Customers/$link\",\"source\":\"http://localhost/odata/Customers(1)\",\"relationship\":\"Orders\",\"target\":\"http://localhost/odata/Orders(7)\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Orders/$entity\",\"@odata.id\":\"http://localhost/odata/Orders(100)\",\"OrderID\":100,\"OrderDate\":\"2025-03-06T08:30:00Z\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Customers/$deletedEntity\",\"id\":\"http://localhost/odata/Customers(2)\",\"reason\":\"deleted\"}]}", content);

            stream.Seek(0, SeekOrigin.Begin);

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var deltaReader = await messageReader.CreateODataDeltaResourceSetReaderAsync(customersSet, customerType);

            while (await deltaReader.ReadAsync())
            {
                switch (deltaReader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        var resource = deltaReader.Item as ODataResource;
                        Assert.NotNull(resource);

                        if (resource.Id == new Uri(_baseUri, customersSet.Name + "(1)"))
                        {
                            var firstNameProp = resource.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "FirstName");
                            Assert.NotNull(firstNameProp);
                            Assert.Equal("GGGG", firstNameProp.Value);
                        }
                        else if (resource.Id == new Uri(_baseUri, "Orders(100)"))
                        {
                            var orderIdProp = resource.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "OrderID");
                            Assert.NotNull(orderIdProp);
                            Assert.Equal(100, orderIdProp.Value);
                        }

                        break;

                    case ODataReaderState.DeletedResourceEnd:
                        var deletedResource = deltaReader.Item as ODataDeletedResource;
                        Assert.NotNull(deletedResource);
                        Assert.Equal(new Uri(_baseUri, customersSet.Name + "(2)"), deletedResource.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);

                        break;

                    case ODataReaderState.DeltaLink:
                        var deltaLink = deltaReader.Item as ODataDeltaLink;
                        Assert.NotNull(deltaLink);
                        Assert.Equal(new Uri(_baseUri, customersSet.Name + "(1)"), deltaLink.Source);
                        Assert.Equal(new Uri(_baseUri, "Orders(7)"), deltaLink.Target);
                        Assert.Equal("Orders", deltaLink.Relationship);

                        break;

                    case ODataReaderState.DeltaDeletedLink:
                        var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                        Assert.NotNull(deltaDeletedLink);
                        Assert.Equal(new Uri(_baseUri, customersSet.Name + "(1)"), deltaDeletedLink.Source);
                        Assert.Equal(new Uri(_baseUri, "Orders(8)"), deltaDeletedLink.Target);
                        Assert.Equal("Orders", deltaDeletedLink.Relationship);

                        break;
                }
            }

            Assert.Equal(ODataReaderState.Completed, deltaReader.State);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task ADeltaResponseWith_Containment_IsWrittenAndReadSuccessfully(string mimeType)
        {
            // Arrange: Set up OData model, navigation properties, and request URI
            var accountsSet = _model.FindDeclaredEntitySet("Accounts");
            var accountType = _model.FindDeclaredType(NameSpacePrefix + "Account") as IEdmEntityType;
            var myPisNav = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var piSet = accountsSet.FindNavigationTarget(myPisNav) as IEdmEntitySetBase;
            var piType = _model.FindDeclaredType(NameSpacePrefix + "PaymentInstrument") as IEdmEntityType;
            var requestUri = new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments?$expand=BillingStatements");

            var settings = CreateODataMessageWriterSettings(requestUri);
            var responseMessage = new TestStreamRequestMessage(new MemoryStream(), requestUri, "GET");
            responseMessage.SetHeader("Content-Type", mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings, _model);
            var odataWriter = await messageWriter.CreateODataDeltaResourceSetWriterAsync(piSet as IEdmContainedEntitySet, piType);

            var deltaFeed = new ODataDeltaResourceSet();
            await odataWriter.WriteStartAsync(deltaFeed);

            // Modified Entity
            var deltaEntry = new ODataResource
            {
                Id = new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)"),
                Properties = new[] { new ODataProperty { Name = "FriendlyName", Value = "GGGG" } }
            };
            await odataWriter.WriteStartAsync(deltaEntry);
            await odataWriter.WriteEndAsync();

            // Deleted Link
            var deletedLink = new ODataDeltaDeletedLink(
                new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)"),
                new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"),
                "BillingStatements"
            );

            await odataWriter.WriteDeltaDeletedLinkAsync(deletedLink);

            // Added Link
            var addedLink = new ODataDeltaLink(
                new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)"),
                new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)"),
                "BillingStatements"
            );

            await odataWriter.WriteDeltaLinkAsync(addedLink);

            // Navigation Entry (Billing Statement)
            var navigationEntry = new ODataResource
            {
                Id = new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)"),
                TypeName = NameSpacePrefix + "Statement",
                Properties = new[]
                {
                    new ODataProperty { Name = "TransactionType", Value = "OnlinePurchase" },
                    new ODataProperty { Name = "TransactionDescription", Value = "unknown purchase" },
                    new ODataProperty { Name = "Amount", Value = 32.1 }
                }
            };

            navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = NameSpacePrefix + "Statement",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                NavigationSourceName = "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements"
            });

            await odataWriter.WriteStartAsync(navigationEntry);
            await odataWriter.WriteEndAsync();

            // Deleted Entity
            var deletedEntry = new ODataDeletedResource
            {
                Id = new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"),
                Reason = DeltaDeletedEntryReason.Deleted
            };

            await odataWriter.WriteStartAsync(deletedEntry);
            await odataWriter.WriteEndAsync();

            await odataWriter.WriteEndAsync(); // Close deltaFeed
            await odataWriter.FlushAsync();

            // Act: Read the response message stream
            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(stream, leaveOpen: true);
            var content = await streamReader.ReadToEndAsync();

            Assert.Equal("{\"@odata.context\":\"http://localhost/odata/$metadata#Accounts(103)/MyPaymentInstruments(BillingStatements())/$delta\",\"value\":[{\"@odata.id\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)\",\"FriendlyName\":\"GGGG\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Accounts(103)/MyPaymentInstruments/$deletedLink\",\"source\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)\",\"relationship\":\"BillingStatements\",\"target\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Accounts(103)/MyPaymentInstruments/$link\",\"source\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)\",\"relationship\":\"BillingStatements\",\"target\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Accounts(103)/MyPaymentInstruments(103901)/BillingStatements/$entity\",\"@odata.id\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)\",\"TransactionType\":\"OnlinePurchase\",\"TransactionDescription\":\"unknown purchase\",\"Amount\":32.1},{\"@odata.context\":\"http://localhost/odata/$metadata#Accounts(103)/MyPaymentInstruments/$deletedEntity\",\"id\":\"http://localhost/odata/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)\",\"reason\":\"deleted\"}]}", content);

            stream.Seek(0, SeekOrigin.Begin);

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false
            };
            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var deltaReader = await messageReader.CreateODataDeltaResourceSetReaderAsync(piSet, piType);

            while (await deltaReader.ReadAsync())
            {
                switch (deltaReader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        var entry = deltaReader.Item as ODataResource;

                        if (entry.Id == new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)"))
                        {
                            var friendlyNameProp = entry.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "FriendlyName");
                            Assert.Equal("GGGG", friendlyNameProp.Value);
                        }
                        else if (entry.Id == new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)"))
                        {
                            var transationTypeProp = entry.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "TransactionType");
                            Assert.Equal("OnlinePurchase", transationTypeProp.Value);
                        }

                        break;

                    case ODataReaderState.DeltaDeletedLink:
                        var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                        Assert.NotNull(deltaDeletedLink);
                        Assert.Equal(new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)"), deltaDeletedLink.Source);
                        Assert.Equal(new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"), deltaDeletedLink.Target);
                        Assert.Equal("BillingStatements", deltaDeletedLink.Relationship);

                        break;

                    case ODataReaderState.DeltaLink:
                        var deltaLink = deltaReader.Item as ODataDeltaLink;
                        Assert.NotNull(deltaLink);
                        Assert.Equal(new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)"), deltaLink.Source);
                        Assert.Equal(new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)"), deltaLink.Target);
                        Assert.Equal("BillingStatements", deltaLink.Relationship);

                        break;

                    case ODataReaderState.DeletedResourceEnd:
                        var dEntry = deltaReader.Item as ODataDeletedResource;
                        Assert.NotNull(dEntry);
                        Assert.Equal(new Uri(_baseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"), dEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, dEntry.Reason);

                        break;
                }
            }

            Assert.Equal(ODataReaderState.Completed, deltaReader.State);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task ADeltaResponseWith_DerivedTypes_IsWrittenAndReadSuccessfully(string mimeType)
        {
            var peopleSet = _model.FindDeclaredEntitySet("People");
            var personType = _model.FindDeclaredType(NameSpacePrefix + "Person") as IEdmEntityType;
            var requestUri = new Uri(_baseUri, "People?$expand=Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer/Orders");

            var settings = CreateODataMessageWriterSettings(requestUri);

            var responseMessage = new TestStreamRequestMessage(new MemoryStream(), requestUri, "GET");
            responseMessage.SetHeader("Content-Type", mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings, _model);
            var deltaWriter = await messageWriter.CreateODataDeltaResourceSetWriterAsync(peopleSet, personType);

            var deltaFeed = new ODataDeltaResourceSet();

            await deltaWriter.WriteStartAsync(deltaFeed);

            // Modified Entity
            var deltaEntry = new ODataResource
            {
                Id = new Uri(_baseUri, "People(1)"),
                TypeName = NameSpacePrefix + "Customer",
                Properties = new[] { new ODataProperty { Name = "City", Value = "GGGG" } }
            };

            await deltaWriter.WriteStartAsync(deltaEntry);
            await deltaWriter.WriteEndAsync();

            // Deleted Link
            var deletedLink = new ODataDeltaDeletedLink(
                new Uri(_baseUri, "People(1)"),
                new Uri(_baseUri, "Orders(8)"),
                "Orders"
            );

            await deltaWriter.WriteDeltaDeletedLinkAsync(deletedLink);

            // Added Link
            var addedLink = new ODataDeltaLink(
                new Uri(_baseUri, "People(1)"),
                new Uri(_baseUri, "Orders(7)"),
                "Orders"
            );

            await deltaWriter.WriteDeltaLinkAsync(addedLink);

            // Navigation Entry (Order)
            var navigationEntry = new ODataResource
            {
                Id = new Uri(_baseUri, "Orders(100)"),
                TypeName = NameSpacePrefix + "Order",
                Properties = new[]
                {
                    new ODataProperty { Name = "OrderID", Value = 100 },
                    new ODataProperty { Name = "OrderDate", Value = new DateTimeOffset(2025, 3, 6, 8, 30, 0, TimeSpan.Zero) }
                }
            };

            navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = NameSpacePrefix + "Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            });

            await deltaWriter.WriteStartAsync(navigationEntry);
            await deltaWriter.WriteEndAsync();

            // Deleted Entity
            var deletedEntry = new ODataDeletedResource()
            {
                Id = new Uri(_baseUri, "People(2)"),
                Reason = DeltaDeletedEntryReason.Changed
            };

            await deltaWriter.WriteStartAsync(deletedEntry);
            await deltaWriter.WriteEndAsync();

            await deltaWriter.WriteEndAsync(); // Close deltaFeed
            await deltaWriter.FlushAsync();

            // Read the response message stream
            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(stream, leaveOpen: true);
            var content = await streamReader.ReadToEndAsync();

            Assert.Equal("{\"@odata.context\":\"http://localhost/odata/$metadata#People(Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer/Orders())/$delta\",\"value\":[{\"@odata.type\":\"#Microsoft.OData.E2E.TestCommon.Common.Server.Default.Customer\",\"@odata.id\":\"http://localhost/odata/People(1)\",\"City\":\"GGGG\"},{\"@odata.context\":\"http://localhost/odata/$metadata#People/$deletedLink\",\"source\":\"http://localhost/odata/People(1)\",\"relationship\":\"Orders\",\"target\":\"http://localhost/odata/Orders(8)\"},{\"@odata.context\":\"http://localhost/odata/$metadata#People/$link\",\"source\":\"http://localhost/odata/People(1)\",\"relationship\":\"Orders\",\"target\":\"http://localhost/odata/Orders(7)\"},{\"@odata.context\":\"http://localhost/odata/$metadata#Orders/$entity\",\"@odata.id\":\"http://localhost/odata/Orders(100)\",\"OrderID\":100,\"OrderDate\":\"2025-03-06T08:30:00Z\"},{\"@odata.context\":\"http://localhost/odata/$metadata#People/$deletedEntity\",\"id\":\"http://localhost/odata/People(2)\",\"reason\":\"changed\"}]}", content);

            stream.Seek(0, SeekOrigin.Begin);

            var readerSettings = new ODataMessageReaderSettings { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var deltaReader = await messageReader.CreateODataDeltaResourceSetReaderAsync(peopleSet, personType);

            while (await deltaReader.ReadAsync())
            {
                switch (deltaReader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        var entry = deltaReader.Item as ODataResource;

                        if (entry.Id == new Uri(_baseUri, "People(1)"))
                        {
                            var cityNameProp = entry.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "City");
                            Assert.Equal("GGGG", cityNameProp.Value);
                        }
                        else if (entry.Id == new Uri(_baseUri, "Orders(100)"))
                        {
                            var orderIdProp = entry.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "OrderID");
                            Assert.Equal(100, orderIdProp.Value);
                        }

                        break;

                    case ODataReaderState.DeltaDeletedLink:
                        var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                        Assert.NotNull(deltaDeletedLink);
                        Assert.Equal(new Uri(_baseUri, "People(1)"), deltaDeletedLink.Source);
                        Assert.Equal(new Uri(_baseUri, "Orders(8)"), deltaDeletedLink.Target);
                        Assert.Equal("Orders", deltaDeletedLink.Relationship);

                        break;

                    case ODataReaderState.DeltaLink:
                        var deltaLink = deltaReader.Item as ODataDeltaLink;
                        Assert.NotNull(deltaLink);
                        Assert.Equal(new Uri(_baseUri, "People(1)"), deltaLink.Source);
                        Assert.Equal(new Uri(_baseUri, "Orders(7)"), deltaLink.Target);
                        Assert.Equal("Orders", deltaLink.Relationship);

                        break;

                    case ODataReaderState.DeletedResourceEnd:
                        var dEntry = deltaReader.Item as ODataDeletedResource;
                        Assert.NotNull(dEntry);
                        Assert.Equal(new Uri(_baseUri, "People(2)"), dEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, dEntry.Reason);

                        break;
                }
            }

            Assert.Equal(ODataReaderState.Completed, deltaReader.State);
        }

        [Fact]
        public async Task ADeltaResponseWith_ExpandedNavProperties_IsWrittenAndReadSuccessfully()
        {
            var customerSet = _model.FindDeclaredEntitySet("Customers");
            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;
            var orderSet = _model.FindDeclaredEntitySet("Orders");
            var peopleSet = _model.FindDeclaredEntitySet("People");
            var requestUri = new Uri(_baseUri, "Customers?$expand=Orders");

            var settings = CreateODataMessageWriterSettings(requestUri);

            var responseMessage = new TestStreamRequestMessage(new MemoryStream(), requestUri, "GET");
            responseMessage.SetHeader("Content-Type", MimeTypeODataParameterMinimalMetadata);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings, _model);

            ODataWriter deltaWriter = await messageWriter.CreateODataDeltaResourceSetWriterAsync(customerSet, customerType);

            // Delta feed and entry
            var deltaFeed = new ODataDeltaResourceSet();

            var deltaEntry = new ODataResource
            {
                Id = new Uri(_baseUri, customerSet.Name + "(1)"),
                Properties = new[] { new ODataProperty { Name = "FirstName", Value = "GGGG" } }
            };

            var nestedResourceInfoIndeltaEntry = new ODataNestedResourceInfo
            {
                Name = "HomeAddress",
                IsCollection = false,
            };

            var nestedResource = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty{ Name = "Street", Value = "Zixing Road" },
                    new ODataProperty{ Name = "City", Value = "Shanghai" },
                    new ODataProperty{ Name = "PostalCode", Value = "200001" }
                }
            };

            var nestedResourceInfoInExpanded = new ODataNestedResourceInfo
            {
                Name = "InfoFromCustomer",
                IsCollection = false,
            };

            var nestedResourceInExpanded = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty{ Name = "CustomerMessage", Value = "XXL" },
                }
            };

            // Expanded feed
            var navigationLink = new ODataNestedResourceInfo()
            {
                Name = "Orders",
                IsCollection = true,
            };
            var expandedFeed = new ODataResourceSet();
            var expandedEntry = new ODataResource
            {
                Id = new Uri(_baseUri, orderSet.Name + "(8)"),
                Properties = new[]
                {
                    new ODataProperty { Name = "OrderDate", Value = new DateTimeOffset(2011, 3, 4, 16, 03, 57, TimeSpan.FromHours(-8)) },
                    new ODataProperty { Name = "OrderID", Value = 8 },
                    new ODataProperty { Name = "OrderShelfLifes", Value = new ODataCollectionValue { Items = new object[] { new TimeSpan(1) } } },
                    new ODataProperty { Name = "ShelfLife", Value = new TimeSpan(1) },
                    new ODataProperty { Name = "ShipDate", Value = new Date(2014, 8, 12) },
                    new ODataProperty { Name = "ShipTime", Value = new TimeOfDay(6, 5, 30, 0) },
                }
            };

            // Expanded entry
            var navigationLinkSingle = new ODataNestedResourceInfo()
            {
                Name = "Parent",
                IsCollection = false,
            };
            var expandedEntrySingle = new ODataResource
            {
                Id = new Uri(_baseUri, peopleSet.Name + "(2)"),
                Properties = new[]
                {
                    new ODataProperty { Name = "FirstName", Value = "Jill" },
                    new ODataProperty { Name = "LastName", Value = "Jones" },
                    new ODataProperty { Name = "Numbers", Value = new ODataCollectionValue() },
                    new ODataProperty { Name = "Emails", Value = new ODataCollectionValue() },
                    new ODataProperty { Name = "PersonID", Value = 2 },
                    new ODataProperty { Name = "Home", Value = GeographyPoint.Create(15.0, 161.8) },
                }
            };

            // Delta feed and entry
            await deltaWriter.WriteStartAsync(deltaFeed);
            await deltaWriter.WriteStartAsync(deltaEntry);
            await deltaWriter.WriteStartAsync(nestedResourceInfoIndeltaEntry);
            await deltaWriter.WriteStartAsync(nestedResource);
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();

            // Expanded feed
            await deltaWriter.WriteStartAsync(navigationLink);
            await deltaWriter.WriteStartAsync(expandedFeed);
            await deltaWriter.WriteStartAsync(expandedEntry);
            await deltaWriter.WriteStartAsync(nestedResourceInfoInExpanded);
            await deltaWriter.WriteStartAsync(nestedResourceInExpanded);
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();

            // Expanded entry
            await deltaWriter.WriteStartAsync(navigationLinkSingle);
            await deltaWriter.WriteStartAsync(expandedEntrySingle);
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();

            // Delta feed and entry
            await deltaWriter.WriteEndAsync();
            await deltaWriter.WriteEndAsync();

            await deltaWriter.FlushAsync();

            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(stream, leaveOpen: true);
            var content = await streamReader.ReadToEndAsync();

            stream.Seek(0, SeekOrigin.Begin);

            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var deltaReader = await messageReader.CreateODataDeltaResourceSetReaderAsync(customerSet, customerType);

            while (await deltaReader.ReadAsync())
            {
                if (deltaReader.State == ODataReaderState.DeltaResourceSetEnd)
                {
                    Assert.NotNull(deltaReader.Item as ODataDeltaResourceSet);
                }
                else if (deltaReader.State == ODataReaderState.ResourceEnd)
                {
                    var resource = deltaReader.Item as ODataResource;
                    Assert.NotNull(resource);
                }
                else if (deltaReader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    switch (deltaReader.State)
                    {
                        case ODataReaderState.Start:
                            Assert.NotNull(deltaReader.Item as ODataNestedResourceInfo);
                            break;
                        case ODataReaderState.ResourceSetEnd:
                            Assert.NotNull(deltaReader.Item as ODataResourceSet);
                            break;
                        case ODataReaderState.ResourceEnd:
                            var resource = deltaReader.Item as ODataResource;
                            Assert.NotNull(resource);

                            break;
                        case ODataReaderState.NestedResourceInfoEnd:
                            var nestedResourceInfo = deltaReader.Item as ODataNestedResourceInfo;
                            Assert.NotNull(nestedResourceInfo);
                            Assert.Contains(nestedResourceInfo.Name, new[] { "HomeAddress", "InfoFromCustomer", "Orders", "Parent"});

                            // Check if it is a single resource or collection
                            if (nestedResourceInfo.Name == "HomeAddress")
                            {
                                Assert.False(nestedResourceInfo.IsCollection);
                            }
                            else if (nestedResourceInfo.Name == "InfoFromCustomer")
                            {
                                Assert.False(nestedResourceInfo.IsCollection);
                            }
                            else if (nestedResourceInfo.Name == "Orders")
                            {
                                Assert.True(nestedResourceInfo.IsCollection);
                            }
                            else if (nestedResourceInfo.Name == "Parent")
                            {
                                Assert.False(nestedResourceInfo.IsCollection);
                            }

                            break;
                        case ODataReaderState.Completed:
                            Assert.NotNull(deltaReader.Item);

                            break;
                    }
                }
            }

            Assert.Equal(ODataReaderState.Completed, deltaReader.State);
        }

        [Theory]
        [InlineData(MimeTypeODataParameterFullMetadata)]
        [InlineData(MimeTypeODataParameterMinimalMetadata)]
        public async Task ADeltaResponse_WithSelectedAndExpandedNavProperties_IsWrittenAndReadSuccessfully(string mimeType)
        {
            var customersSet = _model.FindDeclaredEntitySet("Customers");
            var customerType = _model.FindDeclaredType(NameSpacePrefix + "Customer") as IEdmEntityType;

            var requestUri = new Uri(_baseUri, "Customers?$select=PersonID,FirstName,LastName&$expand=Orders($select=OrderID,OrderDate)");

            var settings = CreateODataMessageWriterSettings(requestUri);

            var responseMessage = new TestStreamRequestMessage(new MemoryStream(), requestUri, "GET");
            responseMessage.SetHeader("Content-Type", mimeType);

            using var messageWriter = new ODataMessageWriter(responseMessage, settings, _model);

            ODataWriter deltaWriter = await messageWriter.CreateODataDeltaResourceSetWriterAsync(customersSet, customerType);

            var deltaFeed = new ODataDeltaResourceSet();
            var deltaEntry = new ODataResource
            {
                Properties = new[]
                {
                    new ODataProperty { Name = "PersonID", Value = 1 },
                    new ODataProperty { Name = "FirstName", Value = "FFFF" },
                    new ODataProperty { Name = "LastName", Value = "LLLL" },
                    new ODataProperty { Name = "City", Value = "Beijing" }
                }
            };

            var ordersNavigationInfo = new ODataNestedResourceInfo
            {
                Name = "Orders",
                IsCollection = true
            };

            var ordersResourceSet = new ODataResourceSet();

            var navigationEntry = new ODataResource
            {
                Id = new Uri(_baseUri, "Orders(100)"),
                TypeName = NameSpacePrefix + "Order",
                Properties = new[]
                {
                    new ODataProperty { Name = "OrderID", Value = 100 },
                    new ODataProperty { Name = "OrderDate", Value = new DateTimeOffset(2025, 3, 6, 8, 30, 0, TimeSpan.Zero) }
                }
            };

            navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = NameSpacePrefix + "Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            });

            await deltaWriter.WriteStartAsync(deltaFeed);
            await deltaWriter.WriteStartAsync(deltaEntry);

            await deltaWriter.WriteStartAsync(ordersNavigationInfo);
            await deltaWriter.WriteStartAsync(ordersResourceSet);
            await deltaWriter.WriteStartAsync(navigationEntry);
            await deltaWriter.WriteEndAsync(); // End navigationEntry
            await deltaWriter.WriteEndAsync(); // End ordersResourceSet
            await deltaWriter.WriteEndAsync(); // End ordersNavigationInfo

            await deltaWriter.WriteEndAsync(); // End deltaEntry1
            await deltaWriter.WriteEndAsync(); // End deltaFeed

            await deltaWriter.FlushAsync();

            Stream stream = await responseMessage.GetStreamAsync();
            stream.Seek(0, SeekOrigin.Begin);

            var streamReader = new StreamReader(stream, leaveOpen: true);
            var content = await streamReader.ReadToEndAsync();

            Assert.Equal("{\"@odata.context\":\"http://localhost/odata/$metadata#Customers(PersonID,FirstName,LastName,Orders(OrderID,OrderDate))/$delta\",\"value\":[{\"PersonID\":1,\"FirstName\":\"FFFF\",\"LastName\":\"LLLL\",\"City\":\"Beijing\",\"Orders\":[{\"@odata.id\":\"http://localhost/odata/Orders(100)\",\"OrderID\":100,\"OrderDate\":\"2025-03-06T08:30:00Z\"}]}]}", content);

            stream.Seek(0, SeekOrigin.Begin);

            ODataMessageReaderSettings readerSettings = new() { BaseUri = _baseUri, EnableMessageStreamDisposal = false };

            using var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model);
            var deltaReader = await messageReader.CreateODataDeltaResourceSetReaderAsync(customersSet, customerType);

            while (await deltaReader.ReadAsync())
            {
                switch (deltaReader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        ODataResource entry = deltaReader.Item as ODataResource;
                        Assert.NotNull(entry);

                        var personIdProp = entry.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "PersonID");
                        if (personIdProp != null)
                        {
                            Assert.Equal(1, personIdProp.Value);
                            Assert.Equal("FFFF", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "FirstName").Value);
                            Assert.Equal("LLLL", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "LastName").Value);
                            Assert.Equal("Beijing", entry.Properties.OfType<ODataProperty>().Single(p => p.Name == "City").Value);
                        }

                        var orderIdProp = entry.Properties.OfType<ODataProperty>().SingleOrDefault(p => p.Name == "OrderID");
                        if (orderIdProp != null)
                        {
                            Assert.Equal(100, orderIdProp.Value);
                        }

                        break;
                }
            }

            Assert.Equal(ODataReaderState.Completed, deltaReader.State);
        }

        [Theory]
        [InlineData("Customers(1)", "{\"Orders@delta\": [{\"OrderID\": 8, \"@removed\": { \"reason\": \"changed\" }}]}", 8)]
        [InlineData("Customers(1)", "{\"FirstName\": \"Jane\", \"Orders@delta\": [{\"@removed\": { \"reason\": \"changed\" }, \"OrderID\": 8 }]}", 8)]
        [InlineData("Customers(1)", "{\"FirstName\": \"Jane\", \"Orders@delta\": [{\"OrderID\": 8, \"@removed\": { \"reason\": \"changed\" }}]}", 8)]
        [InlineData("Customers(2)", "{\"FirstName\": \"Doe\", \"Orders@delta\": [{\"@removed\": { \"reason\": \"changed\" }, \"OrderID\": 7 }, {\"OrderID\": 9, \"@removed\": { \"reason\": \"changed\" }}]}", 9)]
        [InlineData("Customers(1)", "{\"Orders@delta\": [{\"OrderID\": 8, \"@removed\": { \"reason\": \"deleted\" }}]}", 8)]
        [InlineData("Customers(2)", "{\"Orders@delta\": [{\"OrderID\": 9, \"@removed\": { \"reason\": \"changed\" }}, {\"@removed\": { \"reason\": \"changed\" }, \"OrderID\": 7 }]}", 9)]
        [InlineData("Customers(2)", "{\"Orders@delta\": [{\"OrderID\": 9, \"@removed\": { \"reason\": \"deleted\" }}, {\"@removed\": { \"reason\": \"deleted\" }, \"OrderID\": 7 }]}", 9)]
        [InlineData("Customers(2)", "{\"Orders@delta\": [{\"OrderID\": 9, \"@removed\": { \"reason\": \"deleted\" }}, {\"@removed\": { \"reason\": \"changed\" }, \"OrderID\": 7 }]}", 7)]
        [InlineData("Customers(1)", "{\"FirstName\": \"Jane\", \"Orders@delta\": [{\"@removed\": { \"reason\": \"deleted\" }, \"OrderID\": 8 }]}", 8)]
        [InlineData("Customers(1)", "{\"FirstName\": \"Jane\", \"Orders@delta\": [{\"OrderID\": 8, \"@removed\": { \"reason\": \"deleted\" }}]}", 8)]
        [InlineData("Customers(2)", "{\"FirstName\": \"Doe\", \"Orders@delta\": [{\"@removed\": { \"reason\": \"deleted\" }, \"OrderID\": 7 }, {\"OrderID\": 9, \"@removed\": { \"reason\": \"deleted\" }}]}", 9)]
        [InlineData("Customers(2)", "{\"FirstName\": \"Doe\", \"Orders@delta\": [{\"@removed\": { \"reason\": \"changed\" }, \"OrderID\": 7 }, {\"OrderID\": 9, \"@removed\": { \"reason\": \"deleted\" }}]}", 7)]
        [InlineData("Customers(2)", "{\"FirstName\": \"Doe\", \"Orders@delta\": [{\"@removed\": { \"reason\": \"changed\" }, \"OrderID\": 7 }, {\"@removed\": { \"reason\": \"deleted\" }, \"OrderID\": 9}]}", 7)]
        public async Task DeltaDeleteWithOrderedAndUnorderedPayload_WorksAsExpected_Async(string query, string payload, int removedID)
        {
            // Arrange
            var requestUri = new Uri(_baseUri, query);
            var orderQuery = requestUri.AbsoluteUri + $"/Orders({removedID})";

            // Ensure the order to be removed exists
            var existOrderResponse = await Client.GetAsync(orderQuery);
            Assert.Equal(HttpStatusCode.OK, existOrderResponse.StatusCode);

            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PatchAsync(requestUri, content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Ensure the order is removed
            var removedOrderResponse = await Client.GetAsync(orderQuery);
            Assert.Equal(HttpStatusCode.NotFound, removedOrderResponse.StatusCode);
        }

        private ODataMessageWriterSettings CreateODataMessageWriterSettings(Uri requestUri)
        {
            ODataUriParser uriParser = new(_model, _baseUri, requestUri);
            var settings = new ODataMessageWriterSettings
            {
                BaseUri = _baseUri,
                EnableMessageStreamDisposal = false,
                ODataUri = new ODataUri()
                {
                    RequestUri = requestUri,
                    ServiceRoot = _baseUri,
                    Path = uriParser.ParsePath(),
                    SelectAndExpand = uriParser.ParseSelectAndExpand()
                }
            };

            return settings;
        }

        private void ResetDefaultDataSource()
        {
            var actionUri = new Uri(_baseUri + "deltatests/Default.ResetDefaultDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
