//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightDeltaReaderTests
    {
        private const string payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}],\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"}";

        private const string payloadWithNavigationLinks = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@odata.associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref\",\"Orders@odata.navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order\",\"Parent@odata.associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref\",\"Parent@odata.navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.type\":\"MyNS.Order\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"@odata.type\":\"MyNS.ShippingAddress\",\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}],\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"}";

        private const string payloadWithSimplifiedAnnotations = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref\",\"Orders@navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order\",\"Parent@associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref\",\"Parent@navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person\"},{\"@context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@type\":\"MyNS.Order\",\"@id\":\"Orders(10643)\",\"ShippingAddress\":{\"@type\":\"MyNS.ShippingAddress\",\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}],\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"}";

        #region Entities

        private readonly ODataDeltaFeed feed = new ODataDeltaFeed
        {
            Count = 5,
            DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
        };

        private readonly ODataEntry customerUpdated = new ODataEntry
        {
            Id = new Uri("Customers('BOTTM')", UriKind.Relative),
            Properties = new List<ODataProperty>
            {
                new ODataProperty { Name = "ContactName", Value = "Susan Halvenstern" }
            }
        };

        private readonly ODataDeltaDeletedLink linkToOrder10643 = new ODataDeltaDeletedLink(new Uri("Customers('ALFKI')", UriKind.Relative), new Uri("Orders('10643')", UriKind.Relative), "Orders");

        private readonly ODataDeltaLink linkToOrder10645 = new ODataDeltaLink(new Uri("Customers('BOTTM')", UriKind.Relative), new Uri("Orders('10645')", UriKind.Relative), "Orders");

        private readonly ODataEntry order10643 = new ODataEntry
        {
            Id = new Uri("Orders(10643)", UriKind.Relative),
            Properties = new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "ShippingAddress",
                    Value = new ODataComplexValue
                    {
                        Properties = new List<ODataProperty>
                        {
                            new ODataProperty { Name = "Street", Value = "23 Tsawassen Blvd." },
                            new ODataProperty { Name = "City", Value = "Tsawassen" },
                            new ODataProperty { Name = "Region", Value = "BC" },
                            new ODataProperty { Name = "PostalCode", Value = "T2F 8M4" }
                        }
                    }
                }
            },
        };

        private readonly ODataDeltaDeletedEntry customerDeleted = new ODataDeltaDeletedEntry("Customers('ANTON')", DeltaDeletedEntryReason.Deleted);

        private IEdmNavigationSource customers;

        private EdmEntityType customer;

        #endregion

        [Fact]
        public void ReadExample30FromV4Spec()
        {
            var tuples = this.ReadItem(payload, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples);
        }

        #region ReadExample30FromV4SpecWithNavigationLinks

        [Fact]
        public void ReadExample30FromV4SpecWithNavigationLinks()
        {
            var tuples = this.ReadItem(payloadWithNavigationLinks, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExample30FromV4SpecWithFullODataAnnotationsODataSimplified()
        {
            // cover "@odata.deltaLink"
            var tuples = this.ReadItem(payloadWithNavigationLinks, this.GetModel(), customers, customer, odataSimplified: true);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExample30FromV4SpecWithSimplifiedODataAnnotationsODataSimplified()
        {
            // cover "@deltaLink"
            var tuples = this.ReadItem(payloadWithSimplifiedAnnotations, this.GetModel(), customers, customer, odataSimplified: true);
            this.ValidateTuples(tuples);
        }

        #endregion

        [Fact]
        public void ReadODataType()
        {
            var payloadWithODataType = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.type\":\"MyNS.Order\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}}]}";
            var tuples = this.ReadItem(payloadWithODataType, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadNextLinkAtStart()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.nextLink\":\"http://tempuri.org/\",\"value\":[]}";
            var tuples = this.ReadItem(payload, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadNextLinkAtEnd()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[],\"@odata.nextLink\":\"http://tempuri.org/\"}";
            var tuples = this.ReadItem(payload, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadDeltaLinkAtStart()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.deltaLink\":\"http://tempuri.org/\",\"value\":[]}";
            var tuples = this.ReadItem(payload, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples, null, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadDeltaLinkAtEnd()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[],\"@odata.deltaLink\":\"http://tempuri.org/\"}";
            var tuples = this.ReadItem(payload, this.GetModel(), customers, customer);
            this.ValidateTuples(tuples, null, new Uri("http://tempuri.org/"));
        }

        #region Private Methods

        private IEdmModel GetModel()
        {
            EdmModel myModel = new EdmModel();

            EdmComplexType shippingAddress = new EdmComplexType("MyNS", "ShippingAddress");
            shippingAddress.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            shippingAddress.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            shippingAddress.AddStructuralProperty("Region", EdmPrimitiveTypeKind.String);
            shippingAddress.AddStructuralProperty("PostalCode", EdmPrimitiveTypeKind.String);
            myModel.AddElement(shippingAddress);

            EdmComplexTypeReference shippingAddressReference = new EdmComplexTypeReference(shippingAddress, true);

            EdmEntityType order = new EdmEntityType("MyNS", "Order");
            order.AddStructuralProperty("ShippingAddress", shippingAddressReference);
            myModel.AddElement(order);

            EdmEntityType person = new EdmEntityType("MyNS", "Person");
            myModel.AddElement(person);

            customer = new EdmEntityType("MyNS", "Customer");
            customer.AddStructuralProperty("ContactName", EdmPrimitiveTypeKind.String);
            EdmNavigationPropertyInfo orderLinks = new EdmNavigationPropertyInfo
            {
                Name = "Orders",
                Target = order,
                TargetMultiplicity = EdmMultiplicity.Many
            };
            EdmNavigationPropertyInfo personLinks = new EdmNavigationPropertyInfo
            {
                Name = "Parent",
                Target = person,
                TargetMultiplicity = EdmMultiplicity.Many
            };
            customer.AddUnidirectionalNavigation(orderLinks);
            customer.AddUnidirectionalNavigation(personLinks);
            myModel.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("MyNS", "Example30");
            customers = container.AddEntitySet("Customers", customer);
            container.AddEntitySet("Orders", order);
            myModel.AddElement(container);

            return myModel;
        }

        private IEnumerable<Tuple<ODataItem, ODataDeltaReaderState>> ReadItem(string payload, IEdmModel model = null, IEdmNavigationSource navigationSource = null, IEdmEntityType entityType = null, bool odataSimplified = false)
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.ShouldIncludeAnnotation = s => true;
            settings.ODataSimplified = odataSimplified;

            using (var inputContext = new ODataJsonLightInputContext(
                ODataFormat.Json,
                stream,
                new ODataMediaType("application", "json"),
                Encoding.UTF8,
                settings,
                /*readingResponse*/ true,
                /*synchronous*/ true,
                model ?? new EdmModel(),
                /*urlResolver*/ null))
            {
                var jsonLightReader = new ODataJsonLightDeltaReader(inputContext, navigationSource, entityType);
                while (jsonLightReader.Read())
                {
                    yield return new Tuple<ODataItem, ODataDeltaReaderState>(jsonLightReader.Item, jsonLightReader.State);
                }
            }
        }

        private void ValidateTuples(IEnumerable<Tuple<ODataItem, ODataDeltaReaderState>> tuples, Uri nextLink = null, Uri feedDeltaLink = null)
        {
            foreach (var tuple in tuples)
            {
                switch (tuple.Item2)
                {
                    case ODataDeltaReaderState.DeltaFeedStart:
                        ODataDeltaFeed deltaFeed = tuple.Item1 as ODataDeltaFeed;
                        Assert.NotNull(deltaFeed);
                        if (deltaFeed.Count.HasValue)
                        {
                            Assert.Equal(deltaFeed.Count, feed.Count);
                        }
                        break;
                    case ODataDeltaReaderState.FeedEnd:
                        Assert.NotNull(tuple.Item1 as ODataDeltaFeed);
                        if (nextLink != null)
                        {
                            Assert.Equal(nextLink, ((ODataDeltaFeed)tuple.Item1).NextPageLink);
                        }
                        if (feedDeltaLink != null)
                        {
                            Assert.Equal(feedDeltaLink, ((ODataDeltaFeed)tuple.Item1).DeltaLink);
                        }
                        break;
                    case ODataDeltaReaderState.DeltaEntryStart:
                        Assert.True(tuple.Item1 is ODataEntry);
                        break;
                    case ODataDeltaReaderState.DeltaEntryEnd:
                        var deltaEntry = tuple.Item1 as ODataEntry;
                        Assert.NotNull(deltaEntry);
                        Assert.NotNull(deltaEntry.Id);
                        if (this.IdEqual(deltaEntry.Id, customerUpdated.Id))
                        {
                            Assert.True(PropertiesEqual(deltaEntry.Properties, customerUpdated.Properties));
                        }
                        else if (this.IdEqual(deltaEntry.Id, order10643.Id))
                        {
                            Assert.True(this.PropertiesEqual(deltaEntry.Properties, order10643.Properties));
                        }
                        else
                        {
                            Assert.True(false, "Invalid id read.");
                        }
                        break;
                    case ODataDeltaReaderState.DeltaDeletedEntry:
                        var deltaDeletedEntry = tuple.Item1 as ODataDeltaDeletedEntry;
                        Assert.NotNull(deltaDeletedEntry);
                        Assert.True(deltaDeletedEntry.Id.EndsWith(customerDeleted.Id));
                        Assert.Equal(deltaDeletedEntry.Reason, customerDeleted.Reason);
                        break;
                    case ODataDeltaReaderState.DeltaLink:
                        var deltaLink = tuple.Item1 as ODataDeltaLink;
                        Assert.NotNull(deltaLink);
                        Assert.True(this.IdEqual(deltaLink.Source, linkToOrder10645.Source));
                        Assert.Equal(deltaLink.Relationship, linkToOrder10645.Relationship);
                        Assert.True(this.IdEqual(deltaLink.Target, linkToOrder10645.Target));
                        break;
                    case ODataDeltaReaderState.DeltaDeletedLink:
                        var deltaDeletedLink = tuple.Item1 as ODataDeltaDeletedLink;
                        Assert.NotNull(deltaDeletedLink);
                        Assert.True(this.IdEqual(deltaDeletedLink.Source, linkToOrder10643.Source));
                        Assert.Equal(deltaDeletedLink.Relationship, linkToOrder10643.Relationship);
                        Assert.True(this.IdEqual(deltaDeletedLink.Target, linkToOrder10643.Target));
                        break;
                    default:
                        Assert.True(false, "Wrong reader state.");
                        break;
                }
            }
        }

        private bool IdEqual(Uri first, Uri second)
        {
            return this.GetId(first).Equals(this.GetId(second));
        }

        private string GetId(Uri uri)
        {
            string uriString = UriUtils.UriToString(uri);
            int lastSegment = uriString.LastIndexOf('/') + 1;
            return lastSegment == 0 ? uriString : uriString.Substring(lastSegment);
        }

        private bool PropertiesEqual(IEnumerable<ODataProperty> first, IEnumerable<ODataProperty> second)
        {
            var i = first.GetEnumerator();
            var j = second.GetEnumerator();

            while (i.MoveNext() && j.MoveNext())
            {
                if (!i.Current.Name.Equals(j.Current.Name))
                {
                    return false;
                }

                if (i.Current.Value is ODataComplexValue && j.Current.Value is ODataComplexValue)
                {
                    return this.PropertiesEqual(((ODataComplexValue)i.Current.Value).Properties, ((ODataComplexValue)j.Current.Value).Properties);
                }
                
                if (!i.Current.Value.Equals(j.Current.Value))
                {
                    return false;
                }
            }

            if (i.MoveNext() || j.MoveNext())
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
