//---------------------------------------------------------------------
// <copyright file="DeltaLinkHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;

    /// <summary>
    /// Class for handling incoming client query requests.
    /// </summary>
    public class DeltaLinkHandler : RequestHandler
    {
        private string deltaToken = "common";
        private Uri originalUri = null;

        public DeltaLinkHandler(RequestHandler other, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
            : base(other, HttpMethod.GET, requestUri, headers)
        {
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            var token = HttpUtility.ParseQueryString(RequestUri.Query).Get("$token");
            if (string.IsNullOrEmpty(token))
            {
                deltaToken = "common";
            }
            else
            {
                deltaToken = token;
            }

            if (deltaToken == "common")
            {
                originalUri = new Uri(ServiceConstants.ServiceBaseUri, "Customers?$expand=Orders");
                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    var entitySet = this.DataSource.Model.FindDeclaredEntitySet("Customers");
                    var entityType = entitySet.EntityType();
                    ODataDeltaWriter deltaWriter = messageWriter.CreateODataDeltaWriter(entitySet, entityType);

                    var deltaFeed = new ODataDeltaResourceSet();
                    var deltaEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(1)"),
                        Properties = new[] { new ODataProperty { Name = "FirstName", Value = "GGGG" } }
                    };
                    var deletedLink = new ODataDeltaDeletedLink(
                        new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(1)"), new Uri(ServiceConstants.ServiceBaseUri, "Orders(8)"), "Orders");
                    var addedLink = new ODataDeltaLink(
                        new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(1)"), new Uri(ServiceConstants.ServiceBaseUri, "Orders(7)"), "Orders");
                    var navigationEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, "Orders(100)"),
                        TypeName = "Microsoft.Test.OData.Services.ODataWCFService.Order",
                        Properties = new[]
                        {
                            new ODataProperty {Name = "OrderID", Value = 100},
                            new ODataProperty {Name = "OrderDate", Value = new DateTimeOffset(DateTime.Now)}
                        }
                    };
                    navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
                    {
                        NavigationSourceEntityTypeName = "Microsoft.Test.OData.Services.ODataWCFService.Order",
                        NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                        NavigationSourceName = "Orders"
                    });

                    var deletedEntry = new ODataDeltaDeletedEntry(
                        new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(2)").AbsoluteUri, DeltaDeletedEntryReason.Deleted);

                    deltaFeed.DeltaLink = new Uri(ServiceConstants.ServiceBaseUri, "$delta?$token=common");

                    deltaWriter.WriteStart(deltaFeed);
                    deltaWriter.WriteStart(deltaEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedLink(deletedLink);
                    deltaWriter.WriteDeltaLink(addedLink);
                    deltaWriter.WriteStart(navigationEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedEntry(deletedEntry);
                    deltaWriter.WriteEnd();
                }
            }
            else if (deltaToken == "containment")
            {
                originalUri = new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments?$expand=BillingStatements");
                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    var accountsSet = this.DataSource.Model.FindDeclaredEntitySet("Accounts");
                    var accountType = accountsSet.EntityType();
                    var myPisNav = accountType.FindProperty("MyPaymentInstruments") as IEdmNavigationProperty;
                    var piSet = accountsSet.FindNavigationTarget(myPisNav);
                    var piType = piSet.EntityType();
                    ODataDeltaWriter deltaWriter = messageWriter.CreateODataDeltaWriter(piSet as IEdmContainedEntitySet, piType);

                    var deltaFeed = new ODataDeltaResourceSet();
                    var deltaEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)"),
                        Properties = new[] { new ODataProperty { Name = "FriendlyName", Value = "GGGG" } }
                    };

                    var deletedEntry = new ODataDeltaDeletedEntry(
                        new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)").AbsoluteUri,
                        DeltaDeletedEntryReason.Deleted);
                    deletedEntry.SetSerializationInfo(new ODataDeltaSerializationInfo()
                    {
                        NavigationSourceName = "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements"
                    });

                    var deletedLink = new ODataDeltaDeletedLink(
                        new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)"),
                        new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901001)"),
                        "BillingStatements");

                    var navigationEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)"),
                        TypeName = "Microsoft.Test.OData.Services.ODataWCFService.Statement",
                        Properties = new[]
                        {
                            new ODataProperty { Name = "TransactionType", Value = "OnlinePurchase" },
                            new ODataProperty { Name = "TransactionDescription", Value = "unknown purchase" },
                            new ODataProperty { Name = "Amount", Value = 32.1 }
                        }
                    };
                    navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
                    {
                        NavigationSourceEntityTypeName = "Microsoft.Test.OData.Services.ODataWCFService.Statement",
                        NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet,
                        NavigationSourceName = "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements"
                    });

                    var addedLink = new ODataDeltaLink(
                        new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)"),
                        new Uri(ServiceConstants.ServiceBaseUri, "Accounts(103)/MyPaymentInstruments(103901)/BillingStatements(103901005)"),
                        "BillingStatements");

                    deltaWriter.WriteStart(deltaFeed);
                    deltaWriter.WriteStart(deltaEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedEntry(deletedEntry);
                    deltaWriter.WriteDeltaDeletedLink(deletedLink);
                    deltaWriter.WriteStart(navigationEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaLink(addedLink);
                    deltaWriter.WriteEnd();
                }
            }
            else if (deltaToken == "derived")
            {
                originalUri = new Uri(ServiceConstants.ServiceBaseUri, "People?$expand=Microsoft.Test.OData.Services.ODataWCFService.Customer/Orders");
                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    var peopleSet = this.DataSource.Model.FindDeclaredEntitySet("People");
                    var personType = peopleSet.EntityType();
                    ODataDeltaWriter deltaWriter = messageWriter.CreateODataDeltaWriter(peopleSet, personType);

                    var deltaFeed = new ODataDeltaResourceSet();
                    var deltaEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, "People(1)"),
                        TypeName = "Microsoft.Test.OData.Services.ODataWCFService.Customer",
                        Properties = new[]
                        {
                            new ODataProperty { Name = "City", Value = "GGGG" }
                        }
                    };

                    var addedLink = new ODataDeltaLink(
                        new Uri(ServiceConstants.ServiceBaseUri, "People(1)"), new Uri(ServiceConstants.ServiceBaseUri, "Orders(7)"), "Orders");

                    var deletedEntry = new ODataDeltaDeletedEntry(
                        new Uri(ServiceConstants.ServiceBaseUri, "People(2)").AbsoluteUri,
                        DeltaDeletedEntryReason.Changed);

                    var deletedLink = new ODataDeltaDeletedLink(
                        new Uri(ServiceConstants.ServiceBaseUri, "People(1)"),
                        new Uri(ServiceConstants.ServiceBaseUri, "Orders(8)"),
                        "Orders");

                    var navigationEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, "Orders(100)"),
                        TypeName = "Microsoft.Test.OData.Services.ODataWCFService.Order",
                        Properties = new[]
                        {
                            new ODataProperty {Name = "OrderID", Value = 100},
                            new ODataProperty {Name = "OrderDate", Value = new DateTimeOffset(DateTime.Now)}
                        }
                    };
                    navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
                    {
                        NavigationSourceEntityTypeName = "Microsoft.Test.OData.Services.ODataWCFService.Order",
                        NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                        NavigationSourceName = "Orders"
                    });

                    deltaWriter.WriteStart(deltaFeed);
                    deltaWriter.WriteStart(deltaEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedLink(deletedLink);
                    deltaWriter.WriteDeltaLink(addedLink);
                    deltaWriter.WriteStart(navigationEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedEntry(deletedEntry);
                    deltaWriter.WriteEnd();
                }
            }
            else if (deltaToken == "expanded")
            {
                originalUri = new Uri(ServiceConstants.ServiceBaseUri, "Customers?$expand=Orders");
                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    var customerSet = this.DataSource.Model.FindDeclaredEntitySet("Customers");
                    var orderSet = this.DataSource.Model.FindDeclaredEntitySet("Orders");
                    var peopleSet = this.DataSource.Model.FindDeclaredEntitySet("People");
                    var customerType = customerSet.EntityType();
                    ODataDeltaWriter deltaWriter = messageWriter.CreateODataDeltaWriter(customerSet, customerType);

                    // Delta feed and entry
                    var deltaFeed = new ODataDeltaResourceSet
                    {
                        DeltaLink = new Uri(ServiceConstants.ServiceBaseUri, "$delta?$token=expanded")
                    };
                    var deltaEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, customerSet.Name + "(1)"),
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
                            new ODataProperty{ Name = "PostalCode", Value = "200001" },
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
                        Id = new Uri(ServiceConstants.ServiceBaseUri, orderSet.Name + "(8)"),
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
                        Id = new Uri(ServiceConstants.ServiceBaseUri, peopleSet.Name + "(2)"),
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
                    deltaWriter.WriteStart(deltaFeed);
                    deltaWriter.WriteStart(deltaEntry);
                    deltaWriter.WriteStart(nestedResourceInfoIndeltaEntry);
                    deltaWriter.WriteStart(nestedResource);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();

                    // Expanded feed
                    deltaWriter.WriteStart(navigationLink);
                    deltaWriter.WriteStart(expandedFeed);
                    deltaWriter.WriteStart(expandedEntry);
                    deltaWriter.WriteStart(nestedResourceInfoInExpanded);
                    deltaWriter.WriteStart(nestedResourceInExpanded);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();

                    // Expanded entry
                    deltaWriter.WriteStart(navigationLinkSingle);
                    deltaWriter.WriteStart(expandedEntrySingle);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();

                    // Delta feed and entry
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();
                }
            }
            else if (deltaToken == "projection")
            {
                originalUri = new Uri(ServiceConstants.ServiceBaseUri, "Customers?$select=PersonID,FirstName,LastName&$expand=Orders($select=OrderID,OrderDate)");
                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    var entitySet = this.DataSource.Model.FindDeclaredEntitySet("Customers");
                    var entityType = entitySet.EntityType();
                    ODataDeltaWriter deltaWriter = messageWriter.CreateODataDeltaWriter(entitySet, entityType);

                    var deltaFeed = new ODataDeltaResourceSet();
                    var deltaEntry1 = new ODataResource
                    {
                        Properties = new[]
                        {
                            new ODataProperty { Name = "PersonID", Value = 1 },
                            new ODataProperty { Name = "FirstName", Value = "FFFF" },
                            new ODataProperty { Name = "LastName", Value = "LLLL" },
                            new ODataProperty { Name = "City", Value = "Beijing" }
                        }
                    };
                    var deletedLink = new ODataDeltaDeletedLink(
                        new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(1)"), new Uri(ServiceConstants.ServiceBaseUri, "Orders(8)"), "Orders");
                    var addedLink = new ODataDeltaLink(
                        new Uri(ServiceConstants.ServiceBaseUri, entitySet.Name + "(1)"), new Uri(ServiceConstants.ServiceBaseUri, "Orders(7)"), "Orders");
                    var navigationEntry = new ODataResource
                    {
                        Id = new Uri(ServiceConstants.ServiceBaseUri, "Orders(100)"),
                        TypeName = "Microsoft.Test.OData.Services.ODataWCFService.Order",
                        Properties = new[]
                        {
                            new ODataProperty {Name = "OrderID", Value = 100},
                            new ODataProperty {Name = "OrderDate", Value = new DateTimeOffset(DateTime.Now)}
                        }
                    };
                    navigationEntry.SetSerializationInfo(new ODataResourceSerializationInfo
                    {
                        NavigationSourceEntityTypeName = "Microsoft.Test.OData.Services.ODataWCFService.Order",
                        NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                        NavigationSourceName = "Orders"
                    });

                    var deletedOrderEntry = new ODataDeltaDeletedEntry(
                        new Uri(ServiceConstants.ServiceBaseUri, "Orders(20)").AbsoluteUri, DeltaDeletedEntryReason.Deleted);
                    deletedOrderEntry.SetSerializationInfo(new ODataDeltaSerializationInfo()
                    {
                        NavigationSourceName = "Orders"
                    });

                    var deltaEntry2 = new ODataResource
                    {
                        Properties = new[]
                        {
                            new ODataProperty { Name = "PersonID", Value = 2 },
                            new ODataProperty { Name = "FirstName", Value = "AAAA" },
                        }
                    };

                    deltaWriter.WriteStart(deltaFeed);
                    deltaWriter.WriteStart(deltaEntry1);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedLink(deletedLink);
                    deltaWriter.WriteDeltaLink(addedLink);
                    deltaWriter.WriteStart(navigationEntry);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteDeltaDeletedEntry(deletedOrderEntry);
                    deltaWriter.WriteStart(deltaEntry2);
                    deltaWriter.WriteEnd();
                    deltaWriter.WriteEnd();
                }
            }
        }

        protected override ODataMessageWriterSettings GetWriterSettings()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                BaseUri = this.ServiceRootUri
            };

            ODataUriParser uriParser = new ODataUriParser(this.DataSource.Model, ServiceConstants.ServiceBaseUri, originalUri);
            settings.ODataUri = new ODataUri()
            {
                RequestUri = originalUri,
                ServiceRoot = this.ServiceRootUri,
                Path = uriParser.ParsePath(),
                SelectAndExpand = uriParser.ParseSelectAndExpand()
            };

            // TODO: howang read the encoding from request.
            settings.SetContentType(string.IsNullOrEmpty(this.QueryContext.FormatOption) ? this.RequestAcceptHeader : this.QueryContext.FormatOption, Encoding.UTF8.WebName);
            return settings;
        }
    }
}
