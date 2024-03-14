//---------------------------------------------------------------------
// <copyright file="DeltaTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.DeltaTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class DeltaTests : ODataWCFServiceTestsBase<InMemoryEntities>, IDisposable
    {
        public DeltaTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {

        }

        public static readonly IEnumerable<object[]> DeltaMimeTypes = new List<object[]>
        {
            new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata },
            new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata },
            new object[] { MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata },
        };

        [Theory]
        [MemberData(nameof(DeltaMimeTypes))]
        public void RequestDeltaLink(string mimeType)
        {
            var customersSet = Model.FindDeclaredEntitySet("Customers");
            var customerType = customersSet.EntityType;

            var readerSettings = new ODataMessageReaderSettings { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta", UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                var resourceSetCounter = 0;
                var deltaResourceSetCounter = 0;
                var resourceCounter = 0;
                var nestedResourceInfoCounter = 0;
                var deletedResourceCounter = 0;
                var deltaLinkCounter = 0;
                var deltaDeletedLinkCounter = 0;

                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var deltaReader = messageReader.CreateODataDeltaResourceSetReader(customersSet, customerType);

                    while (deltaReader.Read())
                    {
                        switch (deltaReader.State)
                        {
                            case ODataReaderState.ResourceSetEnd:
                                resourceSetCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetEnd:
                                var deltaFeed = deltaReader.Item as ODataDeltaResourceSet;
                                Assert.NotNull(deltaFeed);
                                deltaResourceSetCounter++;

                                break;
                            case ODataReaderState.ResourceEnd:
                                var entry = deltaReader.Item as ODataResource;
                                Assert.NotNull(entry);
                                var properties = entry.Properties.ToArray();
                                switch (resourceCounter)
                                {
                                    case 0:
                                        var property = Assert.IsType<ODataProperty>(Assert.Single(properties));
                                        Assert.Equal("FirstName", property.Name);
                                        Assert.Equal("GGGG", property.Value);
                                        Assert.EndsWith("/Customers(1)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Customers(1)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 1:
                                        Assert.Equal(2, properties.Length);
                                        Assert.Equal("OrderID", properties[0].Name);
                                        Assert.Equal(100, Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("OrderDate", properties[1].Name);
                                        Assert.EndsWith("/Orders(100)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Orders(100)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    default:
                                        Assert.True(false, "Expected only 2 resources");

                                        break;
                                }

                                resourceCounter++;

                                break;
                            case ODataReaderState.DeletedResourceEnd:
                                var deletedResource = deltaReader.Item as ODataDeletedResource;
                                Assert.NotNull(deletedResource);
                                Assert.EndsWith("/Customers(2)", deletedResource.Id.AbsoluteUri);
                                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                                deletedResourceCounter++;

                                break;
                            case ODataReaderState.DeltaLink:
                                var deltaLink = deltaReader.Item as ODataDeltaLink;
                                Assert.NotNull(deltaLink);
                                Assert.EndsWith("/Customers(1)", deltaLink.Source.AbsoluteUri);
                                Assert.EndsWith("/Orders(7)", deltaLink.Target.AbsoluteUri);
                                Assert.Equal("Orders", deltaLink.Relationship);
                                deltaLinkCounter++;

                                break;
                            case ODataReaderState.DeltaDeletedLink:
                                var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                                Assert.EndsWith("/Customers(1)", deltaDeletedLink.Source.AbsoluteUri);
                                Assert.EndsWith("Orders(8)", deltaDeletedLink.Target.AbsoluteUri);
                                Assert.Equal("Orders", deltaDeletedLink.Relationship);
                                Assert.NotNull(deltaDeletedLink);
                                deltaDeletedLinkCounter++;

                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                Assert.EndsWith("odata.metadata=full", mimeType);
                                var nestedResourceInfo = deltaReader.Item as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResourceInfo);
                                switch (nestedResourceInfo.Name)
                                {
                                    case "Parent":
                                    case "Orders":
                                    case "Company":
                                        break;
                                    default:
                                        Assert.True(false, $"Unexpected nested resource: {nestedResourceInfo.Name}");

                                        break;
                                }

                                Assert.EndsWith($"/Customers(1)/{nestedResourceInfo.Name}/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                                Assert.EndsWith($"/Customers(1)/{nestedResourceInfo.Name}", nestedResourceInfo.Url.AbsoluteUri);
                                nestedResourceInfoCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetStart:
                            case ODataReaderState.ResourceStart:
                            case ODataReaderState.DeletedResourceStart:
                            case ODataReaderState.NestedResourceInfoStart:

                                break;
                            default:
                                Assert.True(false, $"Unexpected ODataReaderState: {deltaReader.State}");

                                break;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, deltaReader.State);
                }

                Assert.Equal(0, resourceSetCounter);
                Assert.Equal(1, deltaResourceSetCounter);
                Assert.Equal(2, resourceCounter);
                Assert.Equal(1, deletedResourceCounter);
                Assert.Equal(1, deltaLinkCounter);
                Assert.Equal(1, deltaDeletedLinkCounter);
                if (mimeType.EndsWith("odata.metadata=full"))
                {
                    Assert.Equal(3, nestedResourceInfoCounter);
                }
                else if (mimeType.EndsWith("odata.metadata=minimal"))
                {
                    Assert.Equal(0, nestedResourceInfoCounter);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DeltaMimeTypes))]
        public void RequestDeltaLink_Containment(string mimeType)
        {
            var accountsSet = this.Model.FindDeclaredEntitySet("Accounts");
            var accountType = accountsSet.EntityType;
            var myPisNav = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
            var piSet = accountsSet.FindNavigationTarget(myPisNav) as IEdmEntitySetBase;
            var piType = piSet.EntityType;

            var readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=containment", UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                var resourceSetCounter = 0;
                var deltaResourceSetCounter = 0;
                var resourceCounter = 0;
                var nestedResourceInfoCounter = 0;
                var deletedResourceCounter = 0;
                var deltaLinkCounter = 0;
                var deltaDeletedLinkCounter = 0;

                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var deltaReader = messageReader.CreateODataDeltaResourceSetReader(piSet, piType);

                    while (deltaReader.Read())
                    {
                        switch (deltaReader.State)
                        {
                            case ODataReaderState.ResourceSetEnd:
                                resourceSetCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetEnd:
                                var deltaFeed = deltaReader.Item as ODataDeltaResourceSet;
                                Assert.NotNull(deltaFeed);
                                deltaResourceSetCounter++;

                                break;
                            case ODataReaderState.ResourceEnd:
                                var entry = deltaReader.Item as ODataResource;
                                Assert.NotNull(entry);
                                var properties = entry.Properties.ToArray();
                                switch (resourceCounter)
                                {
                                    case 0:
                                        var property = Assert.IsType<ODataProperty>(Assert.Single(properties));
                                        Assert.Equal("FriendlyName", property.Name);
                                        Assert.Equal("GGGG", property.Value);
                                        Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Accounts(103)/MyPaymentInstruments(103901)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 1:
                                        Assert.Equal(3, properties.Length);
                                        Assert.Equal("TransactionType", properties[0].Name);
                                        Assert.Equal("OnlinePurchase", Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("TransactionDescription", properties[1].Name);
                                        Assert.Equal("unknown purchase", Assert.IsType<ODataProperty>(properties[1]).Value);
                                        Assert.Equal("Amount", properties[2].Name);
                                        Assert.Equal(32.1, Assert.IsType<ODataProperty>(properties[2]).Value);
                                        Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    default:
                                        Assert.True(false, "Expected only 2 resources");

                                        break;
                                }

                                resourceCounter++;

                                break;
                            case ODataReaderState.DeletedResourceEnd:
                                var deletedResource = deltaReader.Item as ODataDeletedResource;
                                Assert.NotNull(deletedResource);
                                Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)", deletedResource.Id.AbsoluteUri);
                                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                                deletedResourceCounter++;

                                break;
                            case ODataReaderState.DeltaLink:
                                var deltaLink = deltaReader.Item as ODataDeltaLink;
                                Assert.NotNull(deltaLink);
                                Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)", deltaLink.Source.AbsoluteUri);
                                Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)", deltaLink.Target.AbsoluteUri);
                                Assert.Equal("BillingStatements", deltaLink.Relationship);
                                deltaLinkCounter++;

                                break;
                            case ODataReaderState.DeltaDeletedLink:
                                var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                                Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)", deltaDeletedLink.Source.AbsoluteUri);
                                Assert.EndsWith("/Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)", deltaDeletedLink.Target.AbsoluteUri);
                                Assert.Equal("BillingStatements", deltaDeletedLink.Relationship);
                                Assert.NotNull(deltaDeletedLink);
                                deltaDeletedLinkCounter++;

                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                Assert.EndsWith("odata.metadata=full", mimeType);
                                var nestedResourceInfo = deltaReader.Item as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResourceInfo);
                                switch (nestedResourceInfo.Name)
                                {
                                    case "TheStoredPI":
                                    case "BillingStatements":
                                    case "BackupStoredPI":
                                        break;
                                    default:
                                        Assert.True(false, $"Unexpected nested resource: {nestedResourceInfo.Name}");

                                        break;
                                }

                                Assert.EndsWith($"/Accounts(103)/MyPaymentInstruments(103901)/{nestedResourceInfo.Name}/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                                Assert.EndsWith($"/Accounts(103)/MyPaymentInstruments(103901)/{nestedResourceInfo.Name}", nestedResourceInfo.Url.AbsoluteUri);
                                nestedResourceInfoCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetStart:
                            case ODataReaderState.ResourceStart:
                            case ODataReaderState.DeletedResourceStart:
                            case ODataReaderState.NestedResourceInfoStart:
                                break;
                            default:
                                Assert.True(false, $"Unexpected ODataReaderState: {deltaReader.State}");

                                break;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, deltaReader.State);
                }

                Assert.Equal(0, resourceSetCounter);
                Assert.Equal(1, deltaResourceSetCounter);
                Assert.Equal(2, resourceCounter);
                Assert.Equal(1, deletedResourceCounter);
                Assert.Equal(1, deltaLinkCounter);
                Assert.Equal(1, deltaDeletedLinkCounter);
                if (mimeType.EndsWith("odata.metadata=full"))
                {
                    Assert.Equal(3, nestedResourceInfoCounter);
                }
                else if (mimeType.EndsWith("odata.metadata=minimal"))
                {
                    Assert.Equal(0, nestedResourceInfoCounter);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DeltaMimeTypes))]
        public void RequestDeltaLink_Derived(string mimeType)
        {
            var peopleSet = Model.FindDeclaredEntitySet("People");
            var personType = peopleSet.EntityType;

            var readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=derived", UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                var resourceSetCounter = 0;
                var deltaResourceSetCounter = 0;
                var resourceCounter = 0;
                var nestedResourceInfoCounter = 0;
                var deletedResourceCounter = 0;
                var deltaLinkCounter = 0;
                var deltaDeletedLinkCounter = 0;

                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var deltaReader = messageReader.CreateODataDeltaResourceSetReader(peopleSet, personType);

                    while (deltaReader.Read())
                    {
                        switch (deltaReader.State)
                        {
                            case ODataReaderState.ResourceSetEnd:
                                resourceSetCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetEnd:
                                var deltaFeed = deltaReader.Item as ODataDeltaResourceSet;
                                Assert.NotNull(deltaFeed);
                                deltaResourceSetCounter++;

                                break;
                            case ODataReaderState.ResourceEnd:
                                var entry = deltaReader.Item as ODataResource;
                                Assert.NotNull(entry);
                                var properties = entry.Properties.ToArray();
                                switch (resourceCounter)
                                {
                                    case 0:
                                        var property = Assert.IsType<ODataProperty>(Assert.Single(properties));
                                        Assert.Equal("City", property.Name);
                                        Assert.Equal("GGGG", property.Value);
                                        Assert.EndsWith("/People(1)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("People(1)/Microsoft.Test.OData.Services.ODataWCFService.Customer", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 1:
                                        Assert.Equal(2, properties.Length);
                                        Assert.Equal("OrderID", properties[0].Name);
                                        Assert.Equal(100, Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("OrderDate", properties[1].Name);
                                        Assert.EndsWith("/Orders(100)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Orders(100)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    default:
                                        Assert.True(false, "Expected only 2 resources");

                                        break;
                                }

                                resourceCounter++;

                                break;
                            case ODataReaderState.DeletedResourceEnd:
                                var deletedResource = deltaReader.Item as ODataDeletedResource;
                                Assert.NotNull(deletedResource);
                                Assert.EndsWith("/People(2)", deletedResource.Id.AbsoluteUri);
                                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                                deletedResourceCounter++;

                                break;
                            case ODataReaderState.DeltaLink:
                                var deltaLink = deltaReader.Item as ODataDeltaLink;
                                Assert.NotNull(deltaLink);
                                Assert.EndsWith("/People(1)", deltaLink.Source.AbsoluteUri);
                                Assert.EndsWith("/Orders(7)", deltaLink.Target.AbsoluteUri);
                                Assert.Equal("Orders", deltaLink.Relationship);
                                deltaLinkCounter++;

                                break;
                            case ODataReaderState.DeltaDeletedLink:
                                var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                                Assert.EndsWith("/People(1)", deltaDeletedLink.Source.AbsoluteUri);
                                Assert.EndsWith("Orders(8)", deltaDeletedLink.Target.AbsoluteUri);
                                Assert.Equal("Orders", deltaDeletedLink.Relationship);
                                Assert.NotNull(deltaDeletedLink);
                                deltaDeletedLinkCounter++;

                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                Assert.EndsWith("odata.metadata=full", mimeType);
                                var nestedResourceInfo = deltaReader.Item as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResourceInfo);
                                switch (nestedResourceInfo.Name)
                                {
                                    case "Parent":
                                    case "Orders":
                                    case "Company":
                                        break;
                                    default:
                                        Assert.True(false, $"Unexpected nested resource: {nestedResourceInfo.Name}");

                                        break;
                                }

                                Assert.EndsWith($"/People(1)/Microsoft.Test.OData.Services.ODataWCFService.Customer/{nestedResourceInfo.Name}/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                                Assert.EndsWith($"/People(1)/Microsoft.Test.OData.Services.ODataWCFService.Customer/{nestedResourceInfo.Name}", nestedResourceInfo.Url.AbsoluteUri);
                                nestedResourceInfoCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetStart:
                            case ODataReaderState.ResourceStart:
                            case ODataReaderState.DeletedResourceStart:
                            case ODataReaderState.NestedResourceInfoStart:
                                break;
                            default:
                                Assert.True(false, $"Unexpected ODataReaderState: {deltaReader.State}");

                                break;
                        }

                    }

                    Assert.Equal(ODataReaderState.Completed, deltaReader.State);
                }

                Assert.Equal(0, resourceSetCounter);
                Assert.Equal(1, deltaResourceSetCounter);
                Assert.Equal(2, resourceCounter);
                Assert.Equal(1, deletedResourceCounter);
                Assert.Equal(1, deltaLinkCounter);
                Assert.Equal(1, deltaDeletedLinkCounter);
                if (mimeType.EndsWith("odata.metadata=full"))
                {
                    Assert.Equal(3, nestedResourceInfoCounter);
                }
                else if (mimeType.EndsWith("odata.metadata=minimal"))
                {
                    Assert.Equal(0, nestedResourceInfoCounter);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DeltaMimeTypes))]
        public void RequestDeltaLink_Expanded(string mimeType)
        {
            var customersSet = Model.FindDeclaredEntitySet("Customers");
            var customerType = customersSet.EntityType;

            var readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=expanded", UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                var resourceSetCounter = 0;
                var deltaResourceSetCounter = 0;
                var resourceCounter = 0;
                var nestedResourceInfoCounter = 0;
                var deletedResourceCounter = 0;
                var deltaLinkCounter = 0;
                var deltaDeletedLinkCounter = 0;


                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var deltaReader = messageReader.CreateODataDeltaResourceSetReader(customersSet, customerType);

                    while (deltaReader.Read())
                    {
                        switch (deltaReader.State)
                        {
                            case ODataReaderState.ResourceSetEnd:
                                var feed = deltaReader.Item as ODataResourceSet;
                                Assert.NotNull(feed);
                                resourceSetCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetEnd:
                                var deltaFeed = deltaReader.Item as ODataDeltaResourceSet;
                                Assert.NotNull(deltaFeed);
                                deltaResourceSetCounter++;

                                break;
                            case ODataReaderState.ResourceEnd:
                                var entry = deltaReader.Item as ODataResource;
                                Assert.NotNull(entry);
                                var properties = entry.Properties.ToArray();
                                switch (resourceCounter)
                                {
                                    case 0: // HomeAddress complex property
                                        Assert.Equal(3, properties.Length);
                                        Assert.Equal("Street", properties[0].Name);
                                        Assert.Equal("Zixing Road", Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("City", properties[1].Name);
                                        Assert.Equal("Shanghai", Assert.IsType<ODataProperty>(properties[1]).Value);
                                        Assert.Equal("PostalCode", properties[2].Name);
                                        Assert.Equal("200001", Assert.IsType<ODataProperty>(properties[2]).Value);

                                        break;
                                    case 1: // InfoFromCustomer complex property
                                        var customerMessageProperty = Assert.IsType<ODataProperty>(Assert.Single(properties));
                                        Assert.Equal("CustomerMessage", customerMessageProperty.Name);
                                        Assert.Equal("XXL", customerMessageProperty.Value);

                                        break;
                                    case 2:
                                        Assert.Equal(6, properties.Length);
                                        Assert.Equal("OrderDate", properties[0].Name);
                                        Assert.Equal("OrderID", properties[1].Name);
                                        Assert.Equal(8, Assert.IsType<ODataProperty>(properties[1]).Value);
                                        Assert.Equal("OrderShelfLifes", properties[2].Name);
                                        var orderShelfLifesValue = Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(properties[2]).Value);
                                        Assert.Single(orderShelfLifesValue.Items);
                                        Assert.Equal("ShelfLife", properties[3].Name);
                                        Assert.Equal("ShipDate", properties[4].Name);
                                        Assert.Equal("ShipTime", properties[5].Name);
                                        Assert.EndsWith("/Orders(8)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Orders(8)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 3:
                                        Assert.Equal(6, properties.Length);
                                        Assert.Equal("FirstName", properties[0].Name);
                                        Assert.Equal("Jill", Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("LastName", properties[1].Name);
                                        Assert.Equal("Jones", Assert.IsType<ODataProperty>(properties[1]).Value);
                                        Assert.Equal("Numbers", properties[2].Name);
                                        Assert.Empty(Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(properties[2]).Value).Items);
                                        Assert.Equal("Emails", properties[3].Name);
                                        Assert.Empty(Assert.IsType<ODataCollectionValue>(Assert.IsType<ODataProperty>(properties[3]).Value).Items);
                                        Assert.Equal("PersonID", properties[4].Name);
                                        Assert.Equal(2, Assert.IsType<ODataProperty>(properties[4]).Value);
                                        Assert.Equal("Home", properties[5].Name);
                                        Assert.IsAssignableFrom<GeographyPoint>(Assert.IsType<ODataProperty>(properties[5]).Value);
                                        Assert.EndsWith("People(2)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("People(2)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 4:
                                        var firstNameProperty = Assert.IsType<ODataProperty>(Assert.Single(properties));
                                        Assert.Equal("FirstName", firstNameProperty.Name);
                                        Assert.Equal("GGGG", firstNameProperty.Value);
                                        Assert.EndsWith("/Customers(1)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Customers(1)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    default:
                                        Assert.True(false, "Expected only 5 resources");

                                        break;
                                }

                                resourceCounter++;

                                break;
                            case ODataReaderState.DeletedResourceEnd:
                                deletedResourceCounter++;

                                break;
                            case ODataReaderState.DeltaLink:
                                deltaLinkCounter++;

                                break;
                            case ODataReaderState.DeltaDeletedLink:
                                deltaDeletedLinkCounter++;

                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                var nestedResourceInfo = deltaReader.Item as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResourceInfo);
                                switch (nestedResourceInfo.Name)
                                {
                                    case "HomeAddress":
                                        // Complex nested resource info
                                        Assert.Null(nestedResourceInfo.AssociationLinkUrl);
                                        Assert.Null(nestedResourceInfo.Url);

                                        break;
                                    case "InfoFromCustomer":
                                        // Complex nested resource info
                                        Assert.Null(nestedResourceInfo.AssociationLinkUrl);
                                        Assert.Null(nestedResourceInfo.Url);

                                        break;
                                    case "LoggedInEmployee":
                                        // Reading of annotations on expanded navigation properties currently not supported
                                        Assert.Null(nestedResourceInfo.AssociationLinkUrl);
                                        Assert.Null(nestedResourceInfo.Url);

                                        break;
                                    case "CustomerForOrder":
                                        // Reading of annotations on expanded navigation properties currently not supported
                                        Assert.Null(nestedResourceInfo.AssociationLinkUrl);
                                        Assert.Null(nestedResourceInfo.Url);

                                        break;
                                    case "OrderDetails":
                                        // Reading of annotations on expanded navigation properties currently not supported
                                        Assert.Null(nestedResourceInfo.AssociationLinkUrl);
                                        Assert.Null(nestedResourceInfo.Url);

                                        break;
                                    case "Orders":
                                        Assert.EndsWith("/Customers(1)/Orders/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                                        Assert.EndsWith("/Customers(1)/Orders", nestedResourceInfo.Url.AbsoluteUri);

                                        break;
                                    case "Parent":
                                        var associationLink = nestedResourceInfo.AssociationLinkUrl.AbsoluteUri;
                                        var navigationLink = nestedResourceInfo.Url.AbsoluteUri;
                                        Assert.True(associationLink.EndsWith("/People(2)/Parent/$ref") || associationLink.EndsWith("/Customers(1)/Parent/$ref"));
                                        Assert.True(navigationLink.EndsWith("/People(2)/Parent") || navigationLink.EndsWith("/Customers(1)/Parent"));

                                        break;
                                    case "Company":
                                        Assert.EndsWith("/Customers(1)/Company/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                                        Assert.EndsWith("/Customers(1)/Company", nestedResourceInfo.Url.AbsoluteUri);

                                        break;
                                    default:
                                        Assert.True(false, $"Unexpected nested resource: {nestedResourceInfo.Name}");

                                        break;
                                }

                                nestedResourceInfoCounter++;

                                break;
                            case ODataReaderState.ResourceSetStart:
                            case ODataReaderState.DeltaResourceSetStart:
                            case ODataReaderState.ResourceStart:
                            case ODataReaderState.DeletedResourceStart:
                            case ODataReaderState.NestedResourceInfoStart:
                                break;
                            default:
                                Assert.True(false, $"Unexpected ODataReaderState: {deltaReader.State}");

                                break;
                        }

                    }

                    Assert.Equal(ODataReaderState.Completed, deltaReader.State);
                }

                Assert.Equal(1, resourceSetCounter);
                Assert.Equal(1, deltaResourceSetCounter);
                Assert.Equal(5, resourceCounter);
                Assert.Equal(0, deletedResourceCounter);
                Assert.Equal(0, deltaLinkCounter);
                Assert.Equal(0, deltaDeletedLinkCounter);
                if (mimeType.EndsWith("odata.metadata=full"))
                {
                    Assert.Equal(9, nestedResourceInfoCounter);
                }
                else if (mimeType.EndsWith("odata.metadata=minimal"))
                {
                    Assert.Equal(4, nestedResourceInfoCounter);
                }
            }
        }

        [Theory]
        [MemberData(nameof(DeltaMimeTypes))]
        public void RequestDeltaLink_Projection(string mimeType)
        {
            var customersSet = Model.FindDeclaredEntitySet("Customers");
            var cutomerType = customersSet.EntityType;

            var readerSettings = new ODataMessageReaderSettings() { BaseUri = ServiceBaseUri };

            var requestMessage = new HttpWebRequestMessage(new Uri(ServiceBaseUri.AbsoluteUri + "$delta?$token=projection", UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                var resourceSetCounter = 0;
                var deltaResourceSetCounter = 0;
                var resourceCounter = 0;
                var nestedResourceInfoCounter = 0;
                var deletedResourceCounter = 0;
                var deltaLinkCounter = 0;
                var deltaDeletedLinkCounter = 0;

                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, Model))
                {
                    var deltaReader = messageReader.CreateODataDeltaResourceSetReader(customersSet, cutomerType);

                    while (deltaReader.Read())
                    {
                        switch (deltaReader.State)
                        {
                            case ODataReaderState.ResourceSetEnd:
                                resourceSetCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetEnd:
                                var deltaFeed = deltaReader.Item as ODataDeltaResourceSet;
                                Assert.NotNull(deltaFeed);
                                deltaResourceSetCounter++;

                                break;
                            case ODataReaderState.ResourceEnd:
                                var entry = deltaReader.Item as ODataResource;
                                Assert.NotNull(entry);
                                var properties = entry.Properties.ToArray();
                                switch (resourceCounter)
                                {
                                    case 0:
                                        Assert.Equal(4, properties.Length);
                                        Assert.Equal("PersonID", properties[0].Name);
                                        Assert.Equal(1, Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("FirstName", properties[1].Name);
                                        Assert.Equal("FFFF", Assert.IsType<ODataProperty>(properties[1]).Value);
                                        Assert.Equal("LastName", properties[2].Name);
                                        Assert.Equal("LLLL", Assert.IsType<ODataProperty>(properties[2]).Value);
                                        Assert.Equal("City", Assert.IsType<ODataProperty>(properties[3]).Name);
                                        Assert.Equal("Beijing", Assert.IsType<ODataProperty>(properties[3]).Value);
                                        Assert.EndsWith("/Customers(1)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Customers(1)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 1:
                                        Assert.Equal(2, properties.Length);
                                        Assert.Equal("OrderID", properties[0].Name);
                                        Assert.Equal(100, Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("OrderDate", properties[1].Name);
                                        Assert.EndsWith("/Orders(100)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Orders(100)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    case 2:
                                        Assert.Equal(2, properties.Length);
                                        Assert.Equal("PersonID", properties[0].Name);
                                        Assert.Equal(2, Assert.IsType<ODataProperty>(properties[0]).Value);
                                        Assert.Equal("FirstName", properties[1].Name);
                                        Assert.Equal("AAAA", Assert.IsType<ODataProperty>(properties[1]).Value);
                                        Assert.EndsWith("/Customers(2)", entry.Id.AbsoluteUri);
                                        if (mimeType.EndsWith("odata.metadata=full"))
                                        {
                                            Assert.NotNull(entry.EditLink);
                                            Assert.EndsWith("Customers(2)", entry.EditLink.OriginalString);
                                        }

                                        break;
                                    default:
                                        Assert.True(false, "Expected only 3 resources");

                                        break;
                                }

                                resourceCounter++;

                                break;
                            case ODataReaderState.DeletedResourceEnd:
                                var deletedResource = deltaReader.Item as ODataDeletedResource;
                                Assert.NotNull(deletedResource);
                                Assert.EndsWith("/Orders(20)", deletedResource.Id.AbsoluteUri);
                                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                                deletedResourceCounter++;

                                break;
                            case ODataReaderState.DeltaLink:
                                var deltaLink = deltaReader.Item as ODataDeltaLink;
                                Assert.NotNull(deltaLink);
                                Assert.EndsWith("/Customers(1)", deltaLink.Source.AbsoluteUri);
                                Assert.EndsWith("/Orders(7)", deltaLink.Target.AbsoluteUri);
                                Assert.Equal("Orders", deltaLink.Relationship);
                                deltaLinkCounter++;

                                break;
                            case ODataReaderState.DeltaDeletedLink:
                                var deltaDeletedLink = deltaReader.Item as ODataDeltaDeletedLink;
                                Assert.EndsWith("/Customers(1)", deltaDeletedLink.Source.AbsoluteUri);
                                Assert.EndsWith("Orders(8)", deltaDeletedLink.Target.AbsoluteUri);
                                Assert.Equal("Orders", deltaDeletedLink.Relationship);
                                Assert.NotNull(deltaDeletedLink);
                                deltaDeletedLinkCounter++;

                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                Assert.EndsWith("odata.metadata=full", mimeType);
                                var nestedResourceInfo = deltaReader.Item as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResourceInfo);
                                switch (nestedResourceInfo.Name)
                                {
                                    case "Orders":
                                        var associationLink = nestedResourceInfo.AssociationLinkUrl.AbsoluteUri;
                                        var navigationLink = nestedResourceInfo.Url.AbsoluteUri;
                                        Assert.True(associationLink.EndsWith("Customers(1)/Orders/$ref") || associationLink.EndsWith("Customers(2)/Orders/$ref"));
                                        Assert.True(navigationLink.EndsWith("Customers(1)/Orders") || navigationLink.EndsWith("Customers(2)/Orders"));

                                        break;
                                    default:
                                        Assert.True(false, $"Unexpected nested resource: {nestedResourceInfo.Name}");

                                        break;
                                }

                                nestedResourceInfoCounter++;

                                break;
                            case ODataReaderState.DeltaResourceSetStart:
                            case ODataReaderState.ResourceStart:
                            case ODataReaderState.DeletedResourceStart:
                            case ODataReaderState.NestedResourceInfoStart:
                                break;
                            default:
                                Assert.True(false, $"Unexpected ODataReaderState: {deltaReader.State}");

                                break;
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, deltaReader.State);
                }

                Assert.Equal(0, resourceSetCounter);
                Assert.Equal(1, deltaResourceSetCounter);
                Assert.Equal(3, resourceCounter);
                Assert.Equal(1, deletedResourceCounter);
                Assert.Equal(1, deltaLinkCounter);
                Assert.Equal(1, deltaDeletedLinkCounter);
                if (mimeType.EndsWith("odata.metadata=full"))
                {
                    Assert.Equal(2, nestedResourceInfoCounter);
                }
                else if (mimeType.EndsWith("odata.metadata=minimal"))
                {
                    Assert.Equal(0, nestedResourceInfoCounter);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}
