//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.Test.OData.DependencyInjection;
using Xunit;
using System.Threading.Tasks;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightDeltaReaderTests
    {
        private IEdmModel model;

        private const string payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"Address\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":{\"CityName\":\"Tsawassen\"},\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}],\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"}";

        private const string payloadWithNavigationLinks = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@odata.associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref\",\"Orders@odata.navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order\",\"Parent@odata.associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref\",\"Parent@odata.navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.type\":\"MyNS.Order\",\"@odata.id\":\"Orders(10643)\",\"Address\":{\"@odata.type\":\"MyNS.Address\",\"Street\":\"23 Tsawassen Blvd.\",\"City\":{\"CityName\":\"Tsawassen\"},\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}],\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"}";

        private const string payloadWithSimplifiedAnnotations = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref\",\"Orders@navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order\",\"Parent@associationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref\",\"Parent@navigationLink\":\"http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person\"},{\"@context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@type\":\"MyNS.Order\",\"@id\":\"Orders(10643)\",\"Address\":{\"@type\":\"MyNS.Address\",\"Street\":\"23 Tsawassen Blvd.\",\"City\":{\"CityName\":\"Tsawassen\"},\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}],\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"}";

        #region Entities

        private readonly ODataDeltaResourceSet feed = new ODataDeltaResourceSet
        {
            Count = 5,
            DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
        };

        private readonly ODataResource customerUpdated = new ODataResource
        {
            Id = new Uri("Customers('BOTTM')", UriKind.Relative),
            Properties = new List<ODataProperty>
            {
                new ODataProperty { Name = "ContactName", Value = "Susan Halvenstern" }
            }
        };

        private readonly ODataDeltaDeletedLink linkToOrder10643 = new ODataDeltaDeletedLink(new Uri("Customers('ALFKI')", UriKind.Relative), new Uri("Orders('10643')", UriKind.Relative), "Orders");

        private readonly ODataDeltaLink linkToOrder10645 = new ODataDeltaLink(new Uri("Customers('BOTTM')", UriKind.Relative), new Uri("Orders('10645')", UriKind.Relative), "Orders");

        private readonly ODataResource order10643 = new ODataResource
        {
            Id = new Uri("Orders(10643)", UriKind.Relative),
            Properties = new List<ODataProperty>
            {
            },
        };

        private readonly ODataResource complexPropertyInOrder10643 = new ODataResource
        {
            Properties = new List<ODataProperty>
            {
                new ODataProperty { Name = "Street", Value = "23 Tsawassen Blvd." },
                new ODataProperty { Name = "City", Value = "Tsawassen" },
                new ODataProperty { Name = "Region", Value = "BC" },
                new ODataProperty { Name = "PostalCode", Value = "T2F 8M4" }
            }
        };

        private readonly ODataDeltaDeletedEntry customerDeletedEntry = new ODataDeltaDeletedEntry("Customers('ANTON')", DeltaDeletedEntryReason.Deleted);
        private readonly ODataDeletedResource customerDeleted = new ODataDeletedResource(new Uri("Customers('ANTON')", UriKind.Relative), DeltaDeletedEntryReason.Deleted);

        private EdmEntitySet customers;
        private EdmEntitySet orders;

        private EdmEntityType customer;
        private EdmEntityType order;

        #endregion

        #region ODataV4 tests

        [Fact]
        public void ReadExample30FromV4Spec()
        {
            var tuples = this.ReadItem(payload, Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public async void ReadExample30FromV4SpecAsync()
        {
            var tuples = await this.ReadItemAsync(payload, Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        #region ReadExample30FromV4SpecWithNavigationLinks

        [Fact]
        public void ReadExample30FromV4SpecWithNavigationLinks()
        {
            var tuples = this.ReadItem(payloadWithNavigationLinks, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExample30FromV4SpecWithFullODataAnnotationsODataSimplified()
        {
            // cover "@odata.deltaLink"
            var tuples = this.ReadItem(payloadWithNavigationLinks, this.Model, customers, customer, enableReadingODataAnnotationWithoutPrefix: true);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExample30FromV4SpecWithSimplifiedODataAnnotationsODataSimplified()
        {
            // cover "@deltaLink"
            var tuples = this.ReadItem(payloadWithSimplifiedAnnotations, this.Model, customers, customer, enableReadingODataAnnotationWithoutPrefix: true);
            this.ValidateTuples(tuples);
        }

        #endregion

        [Fact]
        public void ReadODataType()
        {
            var payloadWithODataType = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.type\":\"MyNS.Order\",\"@odata.id\":\"Orders(10643)\",\"Address\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":{\"CityName\":\"Tsawassen\"},\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}}]}";
            var tuples = this.ReadItem(payloadWithODataType, Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadNextLinkAtStart()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.nextLink\":\"http://tempuri.org/\",\"value\":[]}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadNextLinkAtEnd()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[],\"@odata.nextLink\":\"http://tempuri.org/\"}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadDeltaLinkAtStart()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.deltaLink\":\"http://tempuri.org/\",\"value\":[]}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples, null, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadDeltaLinkAtEnd()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[],\"@odata.deltaLink\":\"http://tempuri.org/\"}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples, null, new Uri("http://tempuri.org/"));
        }

        #region Expanded Navigation Property

        private string expandedPayload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders\":" +
                            "[" +
                                "{" +
                                    "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                                    "\"Id\":10643," +
                                    "\"Address\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":" +
                                        "{" +
                                            "\"CityName\":\"Tsawassen\"" +
                                        "}," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}";

        [Fact]
        public void ReadExpandedFeed()
        {
            var tuples = this.ReadItem(expandedPayload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public async void ReadExpandedFeedAsync()
        {
            var tuples = await this.ReadItemAsync(expandedPayload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExpandedFeedWithNavigationLink()
        {
            var payload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.context\":\"http://host/service/$metadata#Customers/$entity\"," +
                            "\"@odata.type\":\"#MyNS.Customer\"," +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"@odata.editLink\":\"Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders@odata.associationLink\":\"http://host/service/Customers('BOTTM')/Orders/$ref\"," +
                            "\"Orders@odata.navigationLink\":\"http://host/service/Customers('BOTTM')/Orders\"," +
                            "\"Orders\":" +
                            "[" +
                                "{" +
                                    "\"@odata.type\":\"#MyNS.Order\"," +
                                    "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                                    "\"@odata.editLink\":\"http://host/service/Orders(10643)\"," +
                                    "\"Id\":10643," +
                                    "\"Address\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":" +
                                        "{" +
                                            "\"CityName\":\"Tsawassen\"" +
                                        "}," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}" +
                            "]," +
                            "\"FavouriteProducts@odata.associationLink\":\"http://host/service/Customers('BOTTM')/FavouriteProducts/$ref\"," +
                            "\"FavouriteProducts@odata.navigationLink\":\"http://host/service/Customers('BOTTM')/FavouriteProducts\"," +
                            "\"ProductBeingViewed@odata.associationLink\":\"http://host/service/Customers('BOTTM')/ProductBeingViewed/$ref\"," +
                            "\"ProductBeingViewed@odata.navigationLink\":\"http://host/service/Customers('BOTTM')/ProductBeingViewed\"" +
                        "}" +
                    "]" +
                "}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        private string multipleExpandedPayload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders\":" +
                            "[" +
                                "{" +
                                    "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                                    "\"Id\":10643," +
                                    "\"Address\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":" +
                                        "{" +
                                            "\"CityName\":\"Tsawassen\"" +
                                        "}," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}" +
                            "]," +
                            "\"FavouriteProducts\":" +
                            "[" +
                                "{" +
                                    "\"@odata.id\":\"http://host/service/Product(1)\"," +
                                    "\"Id\":1," +
                                    "\"Name\":\"Car\"" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}";

        [Fact]
        public void ReadMutlipleExpandedFeeds()
        {
            var tuples = this.ReadItem(multipleExpandedPayload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public async void ReadMutlipleExpandedFeedsAsync()
        {
            var tuples = await this.ReadItemAsync(multipleExpandedPayload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadContainmentExpandedFeed()
        {
            var payload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"FavouriteProducts\":" +
                            "[" +
                                "{" +
                                    "\"@odata.id\":\"http://host/service/Products(1)\"," +
                                    "\"Id\":1," +
                                    "\"Name\":\"Car\"," +
                                    "\"Details\":" +
                                    "[" +
                                        "{" +
                                            "\"@odata.type\":\"#MyNS.ProductDetail\"," +
                                            "\"Id\":1," +
                                            "\"Detail\":\"made in china\"" +
                                        "}" +
                                    "]" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExpandedSingleton()
        {
            var payload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"ProductBeingViewed\":" +
                            "{" +
                                "\"@odata.id\":\"http://host/service/Products(1)\"," +
                                "\"Id\":1," +
                                "\"Name\":\"Car\"," +
                                "\"Details\":" +
                                "[" +
                                    "{" +
                                        "\"@odata.type\":\"#MyNS.ProductDetail\"," +
                                        "\"Id\":1," +
                                        "\"Detail\":\"made in china\"" +
                                    "}" +
                                "]" +
                            "}" +
                        "}" +
                    "]" +
                "}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExpandedFeedException()
        {
            var payload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders\":" +
                            "[" +
                                "{" +
                                    "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                                    "\"Id\":\"Id shouldn't be a string\"," +
                                    "\"Address\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":" +
                                        "{" +
                                            "\"CityName\":\"Tsawassen\"" +
                                        "}," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}";

            Action readAction = () =>
            {
                var tuples = this.ReadItem(payload, this.Model, customers, customer);
                this.ValidateTuples(tuples);
            };
            readAction.ShouldThrow<ODataException>().Where(e => e.Message.Contains("Id shouldn't be a string"));
        }

        #endregion

        #region ComplexProperty

        [Fact]
        public void ReadNestedComplexProperty()
        {
            var payload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Orders/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                            "\"Id\":10643," +
                            "\"Address\":" +
                            "{" +
                                "\"Street\":\"23 Tsawassen Blvd.\"," +
                                "\"City\":" +
                                "{" +
                                    "\"CityName\":\"Tsawassen\"" +
                                "}," +
                                "\"Region\":\"BC\"," +
                                "\"PostalCode\":\"T2F 8M4\"" +
                            "}" +
                        "}" +
                    "]" +
                "}";
            var tuples = this.ReadItem(payload, this.Model, orders, order);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadNestedOpenCollectionOfComplexProperty()
        {
            var payload =
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Orders/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                            "\"Id\":10643," +
                            "\"Addresses@odata.type\":\"#Collection(MyNS.Address)\"," +
                            "\"Addresses\":" +
                            "[" +
                                "{" +
                                    "\"@odata.type\":\"#MyNS.Address\"," +
                                    "\"Street\":\"23 Tsawassen Blvd.\"," +
                                    "\"City\":" +
                                    "{" +
                                        "\"CityName\":\"Tsawassen\"" +
                                    "}," +
                                    "\"Region\":\"BC\"," +
                                    "\"PostalCode\":\"T2F 8M4\"" +
                                "}," +
                                "{" +
                                    "\"@odata.type\":\"#MyNS.Address\"," +
                                    "\"Street\":\"ZixingRoad.\"," +
                                    "\"City\":" +
                                    "{" +
                                        "\"CityName\":\"Shanghai\"" +
                                    "}," +
                                    "\"PostalCode\":\"200001\"" +
                                "}" +

                            "]" +
                        "}" +
                    "]" +
                "}";
            var tuples = this.ReadItem(payload, this.Model, orders, order);
            this.ValidateTuples(tuples);
        }

        #endregion

        #endregion ODataV4 tests

        #region ODataV401 tests

        [Fact]
        public void Read41DeletedEntryWithId()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customers/1\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("Customers/1", UriKind.Relative));
        }

        [Fact]
        public void Read41DeletedEntryWithKeyProperties()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
            deletedResource.Properties.Count().Should().Be(1);
            deletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
        }

        [Fact]
        public void Read41DeletedEntryRemovedAtEnd()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"@removed\":{\"reason\":\"changed\"}}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
        }

        [Fact]
        public void Read41DeletedEntryWithEmptyRemoved()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
        }

        [Fact]
        public void Read41DeletedEntryWithNullRemoved()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":null,\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
        }

        [Fact]
        public void Read41DeletedEntryWithExtraContentInRemoved()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\",\"extraProperty\":\"value\",\"@extra.annotation\":\"annotationValue\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
        }

        [Fact]
        public void ReadPropertiesIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ContactName\":\"Samantha Stones\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while(reader.Read())
            {
                switch(reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
            deletedResource.Properties.Count().Should().Be(2);
            deletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            deletedResource.Properties.First(p=>p.Name=="ContactName").Value.Should().Be("Samantha Stones");
            deletedResource.Reason.Should().Be(DeltaDeletedEntryReason.Changed);
        }

        [Fact]
        public void ReadIgnorePropertiesIn40DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"id\":\"Customers('BOTTM')\",\"reason\":\"deleted\",\"ContactName\":\"Susan Halvenstern\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("Customers('BOTTM')", UriKind.Relative));
            deletedResource.Reason.Should().Be(DeltaDeletedEntryReason.Deleted);
        }

        [Fact]
        public void ReadNestedResourceIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":{\"Name\":\"Scissors\",\"Id\":10}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                        nestedResource = reader.Item as ODataResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("ProductBeingViewed");
            Assert.NotNull(nestedResource);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Scissors");
        }

        [Fact]
        public void ReadNullResourceIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":null}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                        nestedResource = reader.Item as ODataResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("ProductBeingViewed");
            Assert.Null(nestedResource);
        }

        [Fact]
        public void ReadNestedResourceIn41DeltaResource()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"ProductBeingViewed\":{\"Name\":\"Scissors\",\"Id\":10},\"ContactName\":\"Samantha Stones\"}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataResource deltaResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (nestedResource == null)
                        {
                            nestedResource = reader.Item as ODataResource;
                        }
                        else
                        {
                            deltaResource = reader.Item as ODataResource;
                        }
                        break;
                }
            }

            Assert.NotNull(deltaResource);
            deltaResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
            deltaResource.Properties.Count().Should().Be(2);
            deltaResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            deltaResource.Properties.First(p => p.Name == "ContactName").Value.Should().Be("Samantha Stones");
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("ProductBeingViewed");
            Assert.NotNull(nestedResource);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Scissors");
        }

        [Fact]
        public void ReadNestedDeletedEntryIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":10}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataDeletedResource nestedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        if (nestedResource == null)
                        {
                            nestedResource = reader.Item as ODataDeletedResource;
                        }
                        else
                        {
                            deletedResource = reader.Item as ODataDeletedResource;
                        }
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("ProductBeingViewed");
            Assert.NotNull(nestedResource);
            nestedResource.Reason.Should().Be(DeltaDeletedEntryReason.Deleted);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Scissors");
        }

        [Fact]
        public void ReadNestedDerivedDeletedEntryIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":10,\"Name\":\"car\",\"Material\":\"gold\"}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataDeletedResource nestedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        if (nestedResource == null)
                        {
                            nestedResource = reader.Item as ODataDeletedResource;
                        }
                        else
                        {
                            deletedResource = reader.Item as ODataDeletedResource;
                        }
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("ProductBeingViewed");
            Assert.NotNull(nestedResource);
            nestedResource.TypeName.Should().Be("MyNS.PhysicalProduct");
            nestedResource.Reason.Should().Be(DeltaDeletedEntryReason.Deleted);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedResource.Properties.Count().Should().Be(3);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("car");
            nestedResource.Properties.First(p => p.Name == "Material").Value.Should().Be("gold");
        }

        [Fact]
        public void ReadNestedDeletedEntryIn41DeltaResource()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":10}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataResource deltaResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataDeletedResource nestedDeletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        deltaResource = reader.Item as ODataResource;
                        break;
                    case ODataReaderState.DeletedResourceEnd:
                        nestedDeletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                }
            }

            Assert.NotNull(deltaResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("ProductBeingViewed");
            Assert.NotNull(nestedDeletedResource);
            nestedDeletedResource.Reason.Should().Be(DeltaDeletedEntryReason.Deleted);
            nestedDeletedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedDeletedResource.Properties.Count().Should().Be(2);
            nestedDeletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
            nestedDeletedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Scissors");
        }

        [Fact]
        public void ReadNestedDeltaResourceSetIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            ODataDeletedResource nestedDeletedResource = null;
            ODataDeltaResourceSet nestedDeltaResourceSet = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        if (nestedDeletedResource == null)
                        {
                            nestedDeletedResource = reader.Item as ODataDeletedResource;
                        }
                        else
                        {
                            deletedResource = reader.Item as ODataDeletedResource;
                        }
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                            nestedResource = reader.Item as ODataResource;
                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        if(nestedDeltaResourceSet == null)
                        {
                            nestedDeltaResourceSet = reader.Item as ODataDeltaResourceSet;
                        }
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("FavouriteProducts");
            Assert.NotNull(nestedDeltaResourceSet);
            nestedDeltaResourceSet.Count.Should().Be(5);
            nestedDeltaResourceSet.NextPageLink.Should().Be("http://host/service/Customers?$skipToken=5");
            Assert.NotNull(nestedResource);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/1"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Car");
            Assert.NotNull(nestedDeletedResource);
            nestedDeletedResource.Reason.Should().Be(DeltaDeletedEntryReason.Deleted);
            nestedDeletedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedDeletedResource.Properties.Count().Should().Be(1);
            nestedDeletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
        }


        [Fact]
        public void ReadEmptyDeltaResourceSetIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@count\":2,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataDeltaResourceSet nestedDeltaResourceSet = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        if (nestedDeltaResourceSet == null)
                        {
                            nestedDeltaResourceSet = reader.Item as ODataDeltaResourceSet;
                        }
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("FavouriteProducts");
            Assert.NotNull(nestedDeltaResourceSet);
            nestedDeltaResourceSet.Count.Should().Be(2);
            nestedDeltaResourceSet.NextPageLink.Should().Be("http://host/service/Customers?$skipToken=5");
        }

        [Fact]
        public void ReadNestedDeltaResourceSetIn41DeltaResource()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataResource deltaResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            ODataDeletedResource nestedDeletedResource = null;
            ODataDeltaResourceSet nestedDeltaResourceSet = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        nestedDeletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (nestedResource == null)
                        {
                            nestedResource = reader.Item as ODataResource;
                        }
                        else
                        {
                            deltaResource = reader.Item as ODataResource;
                        }
                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        if (nestedDeltaResourceSet == null)
                        {
                            nestedDeltaResourceSet = reader.Item as ODataDeltaResourceSet;
                        }
                        break;
                }
            }

            Assert.NotNull(deltaResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("FavouriteProducts");
            Assert.NotNull(nestedDeltaResourceSet);
            nestedDeltaResourceSet.Count.Should().Be(5);
            nestedDeltaResourceSet.NextPageLink.Should().Be("http://host/service/Customers?$skipToken=5");
            Assert.NotNull(nestedResource);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/1"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Car");
            Assert.NotNull(nestedDeletedResource);
            nestedDeletedResource.Reason.Should().Be(DeltaDeletedEntryReason.Deleted);
            nestedDeletedResource.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedDeletedResource.Properties.Count().Should().Be(1);
            nestedDeletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
        }

        [Fact]
        public void ReadNestedResourceSetIn41DeletedEntry()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            ODataResource nestedResource2 = null;
            ODataResourceSet nestedResourceSet = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (nestedResource == null)
                        {
                            nestedResource = reader.Item as ODataResource;
                        }
                        else
                        {
                            nestedResource2 = reader.Item as ODataResource;
                        }
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        nestedResourceSet = reader.Item as ODataResourceSet;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("FavouriteProducts");
            Assert.NotNull(nestedResourceSet);
            nestedResourceSet.Count.Should().Be(5);
            nestedResourceSet.NextPageLink.Should().Be("http://host/service/Customers?$skipToken=5");
            Assert.NotNull(nestedResource);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/1"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Car");
            Assert.NotNull(nestedResource2);
            nestedResource2.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedResource2.Properties.Count().Should().Be(1);
            nestedResource2.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
        }

        [Fact]
        public void ReadNestedResourceSetIn41DeltaResource()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataResource deltaResource = null;
            ODataNestedResourceInfo nestedResourceInfo = null;
            ODataResource nestedResource = null;
            ODataResource nestedResource2 = null;
            ODataResourceSet nestedResourceSet = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.NestedResourceInfoStart:
                        nestedResourceInfo = reader.Item as ODataNestedResourceInfo;
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (nestedResource == null)
                        {
                            nestedResource = reader.Item as ODataResource;
                        }
                        else if (nestedResource2 == null)
                        {
                            nestedResource2 = reader.Item as ODataResource;
                        }
                        else
                        {
                            deltaResource = reader.Item as ODataResource;
                        }
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        nestedResourceSet = reader.Item as ODataResourceSet;
                        break;
                }
            }

            Assert.NotNull(deltaResource);
            Assert.NotNull(nestedResourceInfo);
            nestedResourceInfo.Name.Should().Be("FavouriteProducts");
            Assert.NotNull(nestedResourceSet);
            nestedResourceSet.Count.Should().Be(5);
            nestedResourceSet.NextPageLink.Should().Be("http://host/service/Customers?$skipToken=5");
            Assert.NotNull(nestedResource);
            nestedResource.Id.Should().Be(new Uri("http://host/service/Products/1"));
            nestedResource.Properties.Count().Should().Be(2);
            nestedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            nestedResource.Properties.First(p => p.Name == "Name").Value.Should().Be("Car");
            Assert.NotNull(nestedResource2);
            nestedResource2.Id.Should().Be(new Uri("http://host/service/Products/10"));
            nestedResource2.Properties.Count().Should().Be(1);
            nestedResource2.Properties.First(p => p.Name == "Id").Value.Should().Be(10);
        }

        [Fact]
        public void ReadDeletedEntryFromDifferentSetIn41()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Orders/1"));
            deletedResource.Properties.Count().Should().Be(1);
            deletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
        }

        [Fact]
        public void ReadDerivedDeletedResourceIn41()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@odata.type\":\"#MyNS.PreferredCustomer\",\"Id\":1,\"HonorLevel\":\"Gold\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            deletedResource.Id.Should().Be(new Uri("http://host/service/Customers/1"));
            deletedResource.Properties.Count().Should().Be(2);
            deletedResource.TypeName.Should().Be("MyNS.PreferredCustomer");
            deletedResource.Properties.First(p => p.Name == "Id").Value.Should().Be(1);
            deletedResource.Properties.First(p => p.Name == "HonorLevel").Value.Should().Be("Gold");
        }

        [Fact]
        public void ReadNestedDeletedEntryFromDifferentSetShouldFail()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders\":[{\"@context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer);

            Action readAction = () =>
            {
                while (reader.Read())
                {
                }
            };

            readAction.ShouldThrow<ODataException>().WithMessage(Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet("http://host/service/$metadata#Customers/$deletedEntity","MyNS.Example30.Customers", "MyNS.Example30.Customers.Orders"));
        }

        #endregion

        #region Private Methods

        private IEdmModel Model
        {
            get
            {
                if (this.model == null)
                {
                    EdmModel myModel = new EdmModel();

                    EdmComplexType city = new EdmComplexType("MyNS", "City");
                    city.AddStructuralProperty("CityName", EdmPrimitiveTypeKind.String);
                    myModel.AddElement(city);

                    EdmComplexType address = new EdmComplexType("MyNS", "Address");
                    address.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
                    address.AddStructuralProperty("City", new EdmComplexTypeReference(city, false));
                    address.AddStructuralProperty("Region", EdmPrimitiveTypeKind.String);
                    address.AddStructuralProperty("PostalCode", EdmPrimitiveTypeKind.String);
                    myModel.AddElement(address);

                    EdmComplexType homeAddress = new EdmComplexType("MyNS", "HomeAddress", address);
                    homeAddress.AddStructuralProperty("IsHomeAddress", EdmPrimitiveTypeKind.Boolean);
                    myModel.AddElement(address);

                    EdmComplexTypeReference AddressReference = new EdmComplexTypeReference(address, true);

                    order = new EdmEntityType("MyNS", "Order", null, false, true);
                    order.AddKeys(order.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                    order.AddStructuralProperty("Address", AddressReference);
                    myModel.AddElement(order);

                    EdmEntityType person = new EdmEntityType("MyNS", "Person");
                    myModel.AddElement(person);

                    customer = new EdmEntityType("MyNS", "Customer");
                    customer.AddKeys(customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
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

                    var preferredCustomer = new EdmEntityType("MyNS", "PreferredCustomer", customer);
                    preferredCustomer.AddStructuralProperty("HonorLevel", EdmPrimitiveTypeKind.String);
                    myModel.AddElement(preferredCustomer);

                    EdmEntityType product = new EdmEntityType("MyNS", "Product");
                    product.AddKeys(product.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                    product.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
                    myModel.AddElement(product);

                    var physicalProductType = new EdmEntityType("MyNS", "PhysicalProduct", product);
                    physicalProductType.AddStructuralProperty("Material", EdmPrimitiveTypeKind.String);
                    myModel.AddElement(physicalProductType);

                    EdmEntityType productDetail = new EdmEntityType("MyNS", "ProductDetail");
                    productDetail.AddKeys(productDetail.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                    productDetail.AddStructuralProperty("Detail", EdmPrimitiveTypeKind.String);
                    myModel.AddElement(productDetail);

                    product.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                    {
                        Name = "Details",
                        Target = productDetail,
                        TargetMultiplicity = EdmMultiplicity.Many,
                        ContainsTarget = true,
                    });

                    EdmNavigationProperty favouriteProducts = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                    {
                        Name = "FavouriteProducts",
                        Target = product,
                        TargetMultiplicity = EdmMultiplicity.Many,

                    });
                    EdmNavigationProperty productBeingViewed = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                    {
                        Name = "ProductBeingViewed",
                        Target = product,
                        TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    });

                    EdmEntityContainer container = new EdmEntityContainer("MyNS", "Example30");
                    customers = container.AddEntitySet("Customers", customer);
                    orders = container.AddEntitySet("Orders", order);
                    EdmEntitySet products = container.AddEntitySet("Products", product);
                    customers.AddNavigationTarget(favouriteProducts, products);
                    customers.AddNavigationTarget(productBeingViewed, products);

                    myModel.AddElement(container);
                    this.model = myModel;
                }

                return this.model;
            }
        }

        private IEnumerable<Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>> ReadItem(string payload, IEdmModel model = null, IEdmNavigationSource navigationSource = null, IEdmEntityType entityType = null, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            var settings = new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = s => true,
            };

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = model ?? new EdmModel(),
                Container = ContainerBuilderHelper.BuildContainer(null)
            };

            using (var inputContext = new ODataJsonLightInputContext(
                new StringReader(payload), messageInfo, settings))
            {
                inputContext.Container.GetRequiredService<ODataSimplifiedOptions>()
                    .EnableReadingODataAnnotationWithoutPrefix = enableReadingODataAnnotationWithoutPrefix;
                var jsonLightReader = new ODataJsonLightDeltaReader(inputContext, navigationSource, entityType);
                while (jsonLightReader.Read())
                {
                    yield return new Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>(jsonLightReader.Item, jsonLightReader.State, jsonLightReader.SubState);
                }
            }
        }

        private async Task<IEnumerable<Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>>> ReadItemAsync(string payload, IEdmModel model = null, IEdmNavigationSource navigationSource = null, IEdmEntityType entityType = null, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            List<Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>> tuples = new List<Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>>();
            var settings = new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = s => true,
            };

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = true,
                Model = model ?? new EdmModel(),
                Container = ContainerBuilderHelper.BuildContainer(null)
            };

            using (var inputContext = new ODataJsonLightInputContext(
                new StringReader(payload), messageInfo, settings))
            {
                inputContext.Container.GetRequiredService<ODataSimplifiedOptions>()
                    .EnableReadingODataAnnotationWithoutPrefix = enableReadingODataAnnotationWithoutPrefix;
                var jsonLightReader = new ODataJsonLightDeltaReader(inputContext, navigationSource, entityType);
                while (await jsonLightReader.ReadAsync())
                {
                    tuples.Add(new Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>(jsonLightReader.Item, jsonLightReader.State, jsonLightReader.SubState));
                }
            }

            return tuples;
        }

        private void ValidateTuples(IEnumerable<Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>> tuples, Uri nextLink = null, Uri feedDeltaLink = null)
        {
            foreach (var tuple in tuples)
            {
                switch (tuple.Item2)
                {
                    case ODataDeltaReaderState.DeltaResourceSetStart:
                        ODataDeltaResourceSet deltaFeed = tuple.Item1 as ODataDeltaResourceSet;
                        Assert.NotNull(deltaFeed);
                        if (deltaFeed.Count.HasValue)
                        {
                            Assert.Equal(deltaFeed.Count, feed.Count);
                        }
                        break;
                    case ODataDeltaReaderState.DeltaResourceSetEnd:
                        Assert.NotNull(tuple.Item1 as ODataDeltaResourceSet);
                        if (nextLink != null)
                        {
                            Assert.Equal(nextLink, ((ODataDeltaResourceSet)tuple.Item1).NextPageLink);
                        }
                        if (feedDeltaLink != null)
                        {
                            Assert.Equal(feedDeltaLink, ((ODataDeltaResourceSet)tuple.Item1).DeltaLink);
                        }
                        break;
                    case ODataDeltaReaderState.DeltaResourceStart:
                        Assert.True(tuple.Item1 is ODataResource);
                        break;
                    case ODataDeltaReaderState.DeltaResourceEnd:
                        var deltaResource = tuple.Item1 as ODataResource;
                        Assert.NotNull(deltaResource);
                        Assert.NotNull(deltaResource.Id);
                        if (this.IdEqual(deltaResource.Id, customerUpdated.Id))
                        {
                            Assert.True(PropertiesEqual(deltaResource.Properties, customerUpdated.Properties));
                        }
                        else if (this.IdEqual(deltaResource.Id, order10643.Id))
                        {
                            Assert.True(this.PropertiesEqual(deltaResource.Properties, order10643.Properties));
                        }
                        else
                        {
                            Assert.True(this.PropertiesEqual(deltaResource.Properties, complexPropertyInOrder10643.Properties));
                        }
                        break;
                    case ODataDeltaReaderState.DeltaDeletedEntry:
                        var deltaDeletedEntry = tuple.Item1 as ODataDeltaDeletedEntry;
                        Assert.NotNull(deltaDeletedEntry);
                        Assert.True(deltaDeletedEntry.Id.EndsWith(customerDeletedEntry.Id));
                        Assert.Equal(deltaDeletedEntry.Reason, customerDeletedEntry.Reason);
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
                    case ODataDeltaReaderState.NestedResource:
                        switch (tuple.Item3)
                        {
                            case ODataReaderState.Completed:
                            case ODataReaderState.Start:
                                ODataNestedResourceInfo nestedResource = tuple.Item1 as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResource);
                                break;
                            case ODataReaderState.EntityReferenceLink:
                                break;
                            case ODataReaderState.ResourceEnd:
                                ODataResource entry = tuple.Item1 as ODataResource;
                                Assert.NotNull(entry);
                                if (entry.TypeName == "MyNS.Order")
                                {
                                    Assert.Equal(10643, entry.Properties.Single(p => p.Name == "Id").Value);
                                }
                                else if (entry.TypeName == "MyNS.Product")
                                {
                                    Assert.Equal(1, entry.Properties.Single(p => p.Name == "Id").Value);
                                }
                                else if (entry.TypeName == "MyNS.ProductDetail")
                                {
                                    Assert.Equal(1, entry.Properties.Single(p => p.Name == "Id").Value);
                                }
                                else if (entry.TypeName == "MyNS.Address")
                                {
                                    Assert.NotNull(entry.Properties.Single(p => p.Name == "Street").Value);
                                }
                                else if (entry.TypeName == "MyNS.City")
                                {
                                    Assert.NotNull(entry.Properties.Single(p => p.Name == "CityName").Value);
                                }
                                break;
                            case ODataReaderState.ResourceStart:
                                Assert.NotNull(tuple.Item1 as ODataResource);
                                break;
                            case ODataReaderState.ResourceSetEnd:
                                Assert.NotNull(tuple.Item1 as ODataResourceSet);
                                break;
                            case ODataReaderState.ResourceSetStart:
                                Assert.NotNull(tuple.Item1 as ODataResourceSet);
                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                Assert.NotNull(tuple.Item1 as ODataNestedResourceInfo);
                                break;
                            case ODataReaderState.NestedResourceInfoStart:
                                ODataNestedResourceInfo nestedResourceInfo = tuple.Item1 as ODataNestedResourceInfo;
                                Assert.NotNull(nestedResourceInfo);
                                Assert.True(nestedResourceInfo.Name.Equals("Details") || nestedResourceInfo.Name.Equals("Address") || nestedResourceInfo.Name.Equals("City"));
                                break;
                            default:
                                Assert.True(false, "Wrong reader sub state.");
                                break;
                        }
                        break;
                    default:
                        Assert.True(false, "Wrong reader state.");
                        break;
                }
            }
        }

        private ODataReader GetODataReader(string deltaPayload, IEdmModel model, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool keyAsSegment = true)
        {
            var settings = new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = s => true,
            };

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = true,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = model ?? new EdmModel(),
                Container = ContainerBuilderHelper.BuildContainer(null)
            };

            var inputContext = new ODataJsonLightInputContext(
                new StringReader(deltaPayload), messageInfo, settings);
            inputContext.Container.GetRequiredService<ODataSimplifiedOptions>()
                   .EnableReadingKeyAsSegment = keyAsSegment;
            inputContext.ODataSimplifiedOptions.EnableReadingODataAnnotationWithoutPrefix = true;
            return new ODataJsonLightReader(inputContext, navigationSource, entityType, /*readingResourceSet*/true, /*readingParameter*/false, /*readingDelta*/ true);
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
