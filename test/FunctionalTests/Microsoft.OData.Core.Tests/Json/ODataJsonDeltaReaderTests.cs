//---------------------------------------------------------------------
// <copyright file="ODataJsonDeltaReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Core.Tests.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    public class ODataJsonDeltaReaderTests
    {
        private readonly ODataMessageReaderSettings messageReaderSettings;

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

        public ODataJsonDeltaReaderTests()
        {
            this.messageReaderSettings = new ODataMessageReaderSettings();
            var _ = Model; // To trigger early initialization of entity types and entity sets
        }

        #region ODataV4 tests

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadExample30FromV4Spec(bool isResponse)
        {
            var tuples = this.ReadItem(payload, Model, customers, customer, isResponse);
            this.ValidateTuples(tuples);
        }

        #region ReadExample30FromV4SpecWithNavigationLinks

        [Fact]
        public void ReadExample30FromV4SpecWithNavigationLinks()
        {
            var tuples = this.ReadItem(payloadWithNavigationLinks, this.Model, customers, customer, /*isResponse*/ true);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExample30FromV4SpecWithFullODataAnnotationsODataSimplified()
        {
            // cover "@odata.deltaLink"
            var tuples = this.ReadItem(payloadWithNavigationLinks, this.Model, customers, customer, /*isResponse*/ true, enableReadingODataAnnotationWithoutPrefix: true);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadExample30FromV4SpecWithSimplifiedODataAnnotationsODataSimplified()
        {
            // cover "@deltaLink"
            var tuples = this.ReadItem(payloadWithSimplifiedAnnotations, this.Model, customers, customer, /*isResponse*/ true, enableReadingODataAnnotationWithoutPrefix: true);
            this.ValidateTuples(tuples);
        }

        #endregion

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadODataType(bool isResponse)
        {
            var payloadWithODataType = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.type\":\"MyNS.Order\",\"@odata.id\":\"Orders(10643)\",\"Address\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":{\"CityName\":\"Tsawassen\"},\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}}]}";
            var tuples = this.ReadItem(payloadWithODataType, Model, customers, customer, isResponse);
            this.ValidateTuples(tuples);
        }

        [Fact]
        public void ReadNextLinkAtStart()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.nextLink\":\"http://tempuri.org/\",\"value\":[]}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer, /*isResponse*/ true);
            this.ValidateTuples(tuples, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadNextLinkAtEnd()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[],\"@odata.nextLink\":\"http://tempuri.org/\"}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer, /*isResponse*/ true);
            this.ValidateTuples(tuples, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadDeltaLinkAtStart()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.deltaLink\":\"http://tempuri.org/\",\"value\":[]}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer, /*isResponse*/ true);
            this.ValidateTuples(tuples, null, new Uri("http://tempuri.org/"));
        }

        [Fact]
        public void ReadDeltaLinkAtEnd()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[],\"@odata.deltaLink\":\"http://tempuri.org/\"}";
            var tuples = this.ReadItem(payload, this.Model, customers, customer, /*isResponse*/ true);
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadExpandedFeed(bool isResponse)
        {
            var tuples = this.ReadItem(expandedPayload, this.Model, customers, customer, isResponse);
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
            var tuples = this.ReadItem(payload, this.Model, customers, customer, /*isResponse*/ true);
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadMutlipleExpandedFeeds(bool isResponse)
        {
            var tuples = this.ReadItem(multipleExpandedPayload, this.Model, customers, customer, isResponse);
            this.ValidateTuples(tuples);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadContainmentExpandedFeed(bool isResponse)
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
            var tuples = this.ReadItem(payload, this.Model, customers, customer, isResponse);
            this.ValidateTuples(tuples);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadExpandedSingleton(bool isResponse)
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
            var tuples = this.ReadItem(payload, this.Model, customers, customer, isResponse);
            this.ValidateTuples(tuples);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadExpandedFeedException(bool isResponse)
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
                var tuples = this.ReadItem(payload, this.Model, customers, customer, isResponse);
                this.ValidateTuples(tuples);
            };
            var exception = Assert.Throws<ODataException>(readAction);
            Assert.Contains("Id shouldn't be a string", exception.Message);
        }

        #endregion

        #region ComplexProperty

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedComplexProperty(bool isResponse)
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
            var tuples = this.ReadItem(payload, this.Model, orders, order, isResponse);
            this.ValidateTuples(tuples);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedOpenCollectionOfComplexProperty(bool isResponse)
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
            var tuples = this.ReadItem(payload, this.Model, orders, order, isResponse);
            this.ValidateTuples(tuples);
        }

        #endregion

        #endregion ODataV4 tests

        #region ODataV401 tests

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryWithId(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customers/1\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(new Uri("Customers/1", UriKind.Relative), deletedResource.Id);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryWithKeyProperties(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
            Assert.Equal(1, property.Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryWithKeyPropertiesSetsOnlyId(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
            ODataDeletedResource deletedResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.DeletedResourceEnd:
                        deletedResource = reader.Item as ODataDeletedResource;
                        break;
                    case ODataReaderState.Stream:
                    case ODataReaderState.NestedResourceInfoStart:
                        Assert.True(false, "Should not add stream or navigation properties to delta payload");
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.Equal(new Uri("http://host/service/Customers/1", UriKind.Absolute), deletedResource.Id);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryRemovedAtEnd(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"@removed\":{\"reason\":\"changed\"}}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(deletedResource.Properties.FirstOrDefault(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryWithEmptyRemoved(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(deletedResource.Properties.FirstOrDefault(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryWithNullRemoved(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":null,\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(deletedResource.Properties.FirstOrDefault(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Read41DeletedEntryWithExtraContentInRemoved(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\",\"extraProperty\":\"value\",\"@extra.annotation\":\"annotationValue\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(1, Assert.IsType<ODataProperty>(deletedResource.Properties.FirstOrDefault(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadPropertiesIn41DeletedEntry(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ContactName\":\"Samantha Stones\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(2, deletedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(deletedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Samantha Stones", Assert.IsType<ODataProperty>(deletedResource.Properties.First(p => p.Name == "ContactName")).Value);
            Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadIgnorePropertiesIn40DeletedEntry(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"id\":\"Customers('BOTTM')\",\"reason\":\"deleted\",\"ContactName\":\"Susan Halvenstern\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(new Uri("Customers('BOTTM')", UriKind.Relative), deletedResource.Id);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedResourceIn41DeletedEntry(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":{\"Name\":\"Scissors\",\"Id\":10}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Scissors", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNullResourceIn41DeletedEntry(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":null}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
            Assert.Null(nestedResource);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedResourceIn41DeltaResource(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"ProductBeingViewed\":{\"Name\":\"Scissors\",\"Id\":10},\"ContactName\":\"Samantha Stones\"}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(2, deltaResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(deltaResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Samantha Stones", Assert.IsType<ODataProperty>(deltaResource.Properties.First(p => p.Name == "ContactName")).Value);
            Assert.NotNull(nestedResourceInfo);
            Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Scissors", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDeletedEntryIn41DeletedEntry(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":10}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
            Assert.NotNull(nestedResource);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedResource.Reason);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Scissors", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDerivedDeletedEntryIn41DeletedEntry(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":10,\"Name\":\"car\",\"Material\":\"gold\"}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
            Assert.NotNull(nestedResource);
            Assert.Equal("MyNS.PhysicalProduct", nestedResource.TypeName);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedResource.Reason);
            Assert.Equal(3, nestedResource.Properties.Count());
            var properties = nestedResource.Properties.OfType<ODataProperty>();
            Assert.Equal(10, properties.First(p => p.Name == "Id").Value);
            Assert.Equal("car", properties.First(p => p.Name == "Name").Value);
            Assert.Equal("gold", properties.First(p => p.Name == "Material").Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDeletedEntryIn41DeltaResource(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":10}}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
            Assert.NotNull(nestedDeletedResource);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedDeletedResource.Reason);
            Assert.Equal(2, nestedDeletedResource.Properties.Count());
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedDeletedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Scissors", Assert.IsType<ODataProperty>(nestedDeletedResource.Properties.First(p => p.Name == "Name")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDeltaResourceSetIn41DeletedEntry(bool isResponse)
        {
            string payload = isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
                        if (nestedDeltaResourceSet == null)
                        {
                            nestedDeltaResourceSet = reader.Item as ODataDeltaResourceSet;
                        }
                        break;
                }
            }

            Assert.NotNull(deletedResource);
            Assert.NotNull(nestedResourceInfo);
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedDeltaResourceSet);
            if (isResponse)
            {
                Assert.Equal(5, nestedDeltaResourceSet.Count);
                Assert.Equal(new Uri("http://host/service/Customers?$skipToken=5"), nestedDeltaResourceSet.NextPageLink);
            }
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Car", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
            Assert.NotNull(nestedDeletedResource);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedDeletedResource.Reason);
            Assert.Single(nestedDeletedResource.Properties);
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedDeletedResource.Properties.First(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadEmptyDeltaResourceSetIn41DeletedEntry(bool isResponse)
        {
            string payload = isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@count\":2,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@delta\":[]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedDeltaResourceSet);
            if (isResponse)
            {
                Assert.Equal(2, nestedDeltaResourceSet.Count);
                Assert.Equal(new Uri("http://host/service/Customers?$skipToken=5"), nestedDeltaResourceSet.NextPageLink);
            }
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDeltaResourceSetIn41DeltaResource(bool isResponse)
        {
            string payload = isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedDeltaResourceSet);
            if (isResponse)
            {
                Assert.Equal(5, nestedDeltaResourceSet.Count);
                Assert.Equal(new Uri("http://host/service/Customers?$skipToken=5"), nestedDeltaResourceSet.NextPageLink);
            }
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Car", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
            Assert.NotNull(nestedDeletedResource);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedDeletedResource.Reason);
            Assert.Single(nestedDeletedResource.Properties);
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedDeletedResource.Properties.First(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDeltaResourceSetIn41TopLevelDeltaResource(bool isResponse)
        {
            string payload = isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$entity\",\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$entity\",\"Id\":1,\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse, singleResource: true);
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
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedDeltaResourceSet);
            if (isResponse)
            {
                Assert.Equal(5, nestedDeltaResourceSet.Count);
                Assert.Equal(new Uri("http://host/service/Customers?$skipToken=5"), nestedDeltaResourceSet.NextPageLink);
            }
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Car", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
            Assert.NotNull(nestedDeletedResource);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedDeletedResource.Reason);
            Assert.Single(nestedDeletedResource.Properties);
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedDeletedResource.Properties.First(p => p.Name == "Id")).Value);
        }

        [Fact]
        public void ReadNestedDeltaResourceSetIn41CreateODataResourceRequest()
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$entity\",\"Id\":1,\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}";

            ODataReader reader = GetODataResourceReader(payload, this.Model, customers, customer, false);
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
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedDeltaResourceSet);
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Car", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
            Assert.NotNull(nestedDeletedResource);
            Assert.Equal(DeltaDeletedEntryReason.Deleted, nestedDeletedResource.Reason);
            Assert.Single(nestedDeletedResource.Properties);
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedDeletedResource.Properties.First(p => p.Name == "Id")).Value);
        }

        [Fact]
        public void ReadNestedDeltaResourceSetIn41CreateODataResourceResponseFails()
        {
            string payload =
                "{\"@context\":\"http://host/service/$metadata#Customers/$entity\",\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}";
            ODataReader reader = GetODataResourceReader(payload, this.Model, customers, customer, true);

            Action readAction = () =>
            {
                while (reader.Read()) { }
            };

            readAction.Throws<ODataException>(Strings.ODataJsonResourceDeserializer_UnexpectedDeletedEntryInResponsePayload);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedResourceSetIn41DeletedEntry(bool isResponse)
        {
            string payload = isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedResourceSet);
            if (isResponse)
            {
                Assert.Equal(5, nestedResourceSet.Count);
                Assert.Equal(new Uri("http://host/service/Customers?$skipToken=5"), nestedResourceSet.NextPageLink);
            }
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Car", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
            Assert.NotNull(nestedResource2);
            Assert.Single(nestedResource2.Properties);
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedResource2.Properties.First(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedResourceSetIn41DeltaResource(bool isResponse)
        {
            string payload = isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"FavouriteProducts@count\":5,\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
            Assert.NotNull(nestedResourceSet);
            if (isResponse)
            {
                Assert.Equal(5, nestedResourceSet.Count);
                Assert.Equal(new Uri("http://host/service/Customers?$skipToken=5"), nestedResourceSet.NextPageLink);
            }
            Assert.NotNull(nestedResource);
            Assert.Equal(2, nestedResource.Properties.Count());
            Assert.Equal(1, Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Car", Assert.IsType<ODataProperty>(nestedResource.Properties.First(p => p.Name == "Name")).Value);
            Assert.NotNull(nestedResource2);
            Assert.Single(nestedResource2.Properties);
            Assert.Equal(10, Assert.IsType<ODataProperty>(nestedResource2.Properties.First(p => p.Name == "Id")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadDeletedEntryFromDifferentSetIn41(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            var property = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
            Assert.Equal("Id", property.Name);
            Assert.Equal(1, property.Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadDerivedDeletedResourceIn41(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@odata.type\":\"#MyNS.PreferredCustomer\",\"Id\":1,\"HonorLevel\":\"Gold\"}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);
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
            Assert.Equal(2, deletedResource.Properties.Count());
            Assert.Equal("MyNS.PreferredCustomer", deletedResource.TypeName);
            Assert.Equal(1, Assert.IsType<ODataProperty>(deletedResource.Properties.First(p => p.Name == "Id")).Value);
            Assert.Equal("Gold", Assert.IsType<ODataProperty>(deletedResource.Properties.First(p => p.Name == "HonorLevel")).Value);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void ReadNestedDeletedEntryFromDifferentSetShouldFail(bool isResponse)
        {
            string payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders\":[{\"@context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}]}";
            ODataReader reader = GetODataReader(payload, this.Model, customers, customer, isResponse);

            Action readAction = () =>
            {
                while (reader.Read())
                {
                }
            };

            readAction.Throws<ODataException>(Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet("http://host/service/$metadata#Customers/$deletedEntity", "Customers", "Customers.Orders"));
        }

        #endregion

        #region Async Tests

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadAsync(bool isResponse)
        {
            var verifyDeltaLinkActionStack = new Stack<Action<ODataDeltaLinkBase>>();
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('BOTTM')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10645')", deltaLink.Target.OriginalString);
            });
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('ALFKI')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10643')", deltaLink.Target.OriginalString);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("Address", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("City", nestedResourceInfo.Name));

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            var verifyDeltaResourceActionStack = new Stack<Action<ODataResource>>();
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Order", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Orders(10643)", deltaResource.Id.OriginalString);
                Assert.Empty(deltaResource.Properties);
            });
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Customers('BOTTM')", deltaResource.Id.OriginalString);
                var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                Assert.Equal("ContactName", contactNameProperty.Name);
                Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(5, deltaResourceSet.Count);
                        Assert.NotNull(deltaResourceSet.DeltaLink);
                        Assert.EndsWith("Customers?$expand=Orders&$deltatoken=8015", deltaResourceSet.DeltaLink.OriginalString);
                    },
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceActionStack);
                        var internalVerifyDeltaResourceAction = verifyDeltaResourceActionStack.Pop();
                        internalVerifyDeltaResourceAction(deltaResource);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    },
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("Customers('ANTON')", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedEntry.Reason);
                    },
                    verifyDeltaLinkAction: (deltaLink) =>
                    {
                        Assert.NotEmpty(verifyDeltaLinkActionStack);
                        var internalVerifyDeltaLinkAction = verifyDeltaLinkActionStack.Pop();
                        internalVerifyDeltaLinkAction(deltaLink);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var internalVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();
                        internalVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyDeltaResourceActionStack);
            Assert.Empty(verifyDeltaLinkActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Fact]
        public async Task ReadDeltaPayloadWithNavigationLinksAsync()
        {
            var verifyDeltaLinkActionStack = new Stack<Action<ODataDeltaLinkBase>>();
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('BOTTM')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10645')", deltaLink.Target.OriginalString);
            });
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('ALFKI')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10643')", deltaLink.Target.OriginalString);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("Address", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("City", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Parent", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Orders", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            var verifyDeltaResourceActionStack = new Stack<Action<ODataResource>>();
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Order", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Orders(10643)", deltaResource.Id.OriginalString);
                Assert.Empty(deltaResource.Properties);
            });
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Customers('BOTTM')", deltaResource.Id.OriginalString);
                var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                Assert.Equal("ContactName", contactNameProperty.Name);
                Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payloadWithNavigationLinks,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(5, deltaResourceSet.Count);
                        Assert.NotNull(deltaResourceSet.DeltaLink);
                        Assert.EndsWith("Customers?$expand=Orders&$deltatoken=8015", deltaResourceSet.DeltaLink.OriginalString);
                    },
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceActionStack);
                        var internalVerifyDeltaResourceAction = verifyDeltaResourceActionStack.Pop();
                        internalVerifyDeltaResourceAction(deltaResource);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    },
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("Customers('ANTON')", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedEntry.Reason);
                    },
                    verifyDeltaLinkAction: (deltaLink) =>
                    {
                        Assert.NotEmpty(verifyDeltaLinkActionStack);
                        var internalVerifyDeltaLinkAction = verifyDeltaLinkActionStack.Pop();
                        internalVerifyDeltaLinkAction(deltaLink);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var internalVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();
                        internalVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }));

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyDeltaResourceActionStack);
            Assert.Empty(verifyDeltaLinkActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Fact]
        public async Task ReadDeltaPayloadWithSimplifiedODataAnnotationsAysnc()
        {
            var verifyDeltaLinkActionStack = new Stack<Action<ODataDeltaLinkBase>>();
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('BOTTM')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10645')", deltaLink.Target.OriginalString);
            });
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('ALFKI')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10643')", deltaLink.Target.OriginalString);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("Address", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("City", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Parent", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Orders", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            var verifyDeltaResourceActionStack = new Stack<Action<ODataResource>>();
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Order", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Orders(10643)", deltaResource.Id.OriginalString);
                Assert.Empty(deltaResource.Properties);
            });
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Customers('BOTTM')", deltaResource.Id.OriginalString);
                var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                Assert.Equal("ContactName", contactNameProperty.Name);
                Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payloadWithNavigationLinks,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(5, deltaResourceSet.Count);
                        Assert.NotNull(deltaResourceSet.DeltaLink);
                        Assert.EndsWith("Customers?$expand=Orders&$deltatoken=8015", deltaResourceSet.DeltaLink.OriginalString);
                    },
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceActionStack);
                        var internalVerifyDeltaResourceAction = verifyDeltaResourceActionStack.Pop();
                        internalVerifyDeltaResourceAction(deltaResource);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    },
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("Customers('ANTON')", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedEntry.Reason);
                    },
                    verifyDeltaLinkAction: (deltaLink) =>
                    {
                        Assert.NotEmpty(verifyDeltaLinkActionStack);
                        var internalVerifyDeltaLinkAction = verifyDeltaLinkActionStack.Pop();
                        internalVerifyDeltaLinkAction(deltaLink);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var internalVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();
                        internalVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }),
                enableReadingODataAnnotationWithoutPrefix: true);

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyDeltaResourceActionStack);
            Assert.Empty(verifyDeltaLinkActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Fact]
        public async Task ReadDeltaPayloadWithSimplifiedODataAnnotationsAndNavigationLinksAsync()
        {
            var verifyDeltaLinkActionStack = new Stack<Action<ODataDeltaLinkBase>>();
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('BOTTM')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10645')", deltaLink.Target.OriginalString);
            });
            verifyDeltaLinkActionStack.Push((deltaLink) =>
            {
                Assert.NotNull(deltaLink.Source);
                Assert.Equal("Customers('ALFKI')", deltaLink.Source.OriginalString);
                Assert.Equal("Orders", deltaLink.Relationship);
                Assert.NotNull(deltaLink.Target);
                Assert.Equal("Orders('10643')", deltaLink.Target.OriginalString);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("Address", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("City", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Parent", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Person/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Orders", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://ouyang-sqldev:9090/ODL635336926402810015/Customers(1)/Order/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            var verifyDeltaResourceActionStack = new Stack<Action<ODataResource>>();
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Order", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Orders(10643)", deltaResource.Id.OriginalString);
                Assert.Empty(deltaResource.Properties);
            });
            verifyDeltaResourceActionStack.Push((deltaResource) =>
            {
                Assert.NotNull(deltaResource);
                Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                Assert.NotNull(deltaResource.Id);
                Assert.EndsWith("Customers('BOTTM')", deltaResource.Id.OriginalString);
                var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                Assert.Equal("ContactName", contactNameProperty.Name);
                Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payloadWithSimplifiedAnnotations,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(5, deltaResourceSet.Count);
                        Assert.NotNull(deltaResourceSet.DeltaLink);
                        Assert.EndsWith("Customers?$expand=Orders&$deltatoken=8015", deltaResourceSet.DeltaLink.OriginalString);
                    },
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceActionStack);
                        var internalVerifyDeltaResourceAction = verifyDeltaResourceActionStack.Pop();
                        internalVerifyDeltaResourceAction(deltaResource);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    },
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("Customers('ANTON')", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedEntry.Reason);
                    },
                    verifyDeltaLinkAction: (deltaLink) =>
                    {
                        Assert.NotEmpty(verifyDeltaLinkActionStack);
                        var internalVerifyDeltaLinkAction = verifyDeltaLinkActionStack.Pop();
                        internalVerifyDeltaLinkAction(deltaLink);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var internalVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();
                        internalVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }),
                enableReadingODataAnnotationWithoutPrefix: true);

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyDeltaResourceActionStack);
            Assert.Empty(verifyDeltaLinkActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithODataTypeAnnotationAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\"," +
                "\"@odata.type\":\"MyNS.Order\"," +
                "\"@odata.id\":\"Orders(10643)\"," +
                "\"Address\":{" +
                "\"Street\":\"23 Tsawassen Blvd.\"," +
                "\"City\":{\"CityName\":\"Tsawassen\"}," +
                "\"Region\":\"BC\"," +
                "\"PostalCode\":\"T2F 8M4\"}}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Order", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.EndsWith("Orders(10643)", deltaResource.Id.OriginalString);
                        Assert.Empty(deltaResource.Properties);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadDeltaPayloadWithODataNextLinkAnnotationAtStartAsync()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"@odata.nextLink\":\"http://tempuri.org/\",\"value\":[]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.NotNull(deltaResourceSet.NextPageLink);
                        Assert.EndsWith("http://tempuri.org/", deltaResourceSet.NextPageLink.AbsoluteUri);
                    }));
        }

        [Fact]
        public async Task ReadDeltaPayloadWithODataNextLinkAnnotationAtEndAsync()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[],\"@odata.nextLink\":\"http://tempuri.org/\"}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.NotNull(deltaResourceSet.NextPageLink);
                        Assert.EndsWith("http://tempuri.org/", deltaResourceSet.NextPageLink.AbsoluteUri);
                    }));
        }

        [Fact]
        public async Task ReadDeltaPayloadWithODataDeltaLinkAnnotationAtStartAsync()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/\",\"value\":[]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.NotNull(deltaResourceSet.DeltaLink);
                        Assert.EndsWith("http://tempuri.org/", deltaResourceSet.DeltaLink.AbsoluteUri);
                    }));
        }

        [Fact]
        public async Task ReadDeltaPayloadWithODataDeltaLinkAnnotationAtEndAsync()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[],\"@odata.deltaLink\":\"http://tempuri.org/\"}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.NotNull(deltaResourceSet.DeltaLink);
                        Assert.EndsWith("http://tempuri.org/", deltaResourceSet.DeltaLink.AbsoluteUri);
                    }));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithExpandedCollectionNavigationPropertyAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders\":[{" +
                "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                "\"Id\":10643," +
                "\"Address\":{" +
                "\"Street\":\"23 Tsawassen Blvd.\"," +
                "\"City\":{\"CityName\":\"Tsawassen\"}," +
                "\"Region\":\"BC\"," +
                "\"PostalCode\":\"T2F 8M4\"}}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Order", resource.TypeName);
                Assert.NotNull(resource.Id);
                Assert.EndsWith("http://host/service/Orders(10643)", resource.Id.AbsoluteUri);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10643, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.EndsWith("http://host/service/Customers('BOTTM')", deltaResource.Id.AbsoluteUri);
                        var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("ContactName", contactNameProperty.Name);
                        Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadDeltaPayloadWithExpandedCollectionNavigationPropertyAndNavigationLinksAsync()
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.context\":\"http://host/service/$metadata#Customers/$entity\"," +
                "\"@odata.type\":\"#MyNS.Customer\"," +
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"@odata.editLink\":\"Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders@odata.associationLink\":\"http://host/service/Customers('BOTTM')/Orders/$ref\"," +
                "\"Orders@odata.navigationLink\":\"http://host/service/Customers('BOTTM')/Orders\"," +
                "\"Orders\":[{" +
                "\"@odata.type\":\"#MyNS.Order\"," +
                "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                "\"@odata.editLink\":\"http://host/service/Orders(10643)\"," +
                "\"Id\":10643," +
                "\"Address\":{" +
                "\"Street\":\"23 Tsawassen Blvd.\"," +
                "\"City\":{\"CityName\":\"Tsawassen\"}," +
                "\"Region\":\"BC\"," +
                "\"PostalCode\":\"T2F 8M4\"}}]," +
                "\"FavouriteProducts@odata.associationLink\":\"http://host/service/Customers('BOTTM')/FavouriteProducts/$ref\"," +
                "\"FavouriteProducts@odata.navigationLink\":\"http://host/service/Customers('BOTTM')/FavouriteProducts\"," +
                "\"ProductBeingViewed@odata.associationLink\":\"http://host/service/Customers('BOTTM')/ProductBeingViewed/$ref\"," +
                "\"ProductBeingViewed@odata.navigationLink\":\"http://host/service/Customers('BOTTM')/ProductBeingViewed\"}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Order", resource.TypeName);
                Assert.NotNull(resource.Id);
                Assert.Equal("http://host/service/Orders(10643)", resource.Id.AbsoluteUri);
                Assert.NotNull(resource.EditLink);
                Assert.Equal("http://host/service/Orders(10643)", resource.EditLink.AbsoluteUri);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10643, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("ProductBeingViewed", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://host/service/Customers('BOTTM')/ProductBeingViewed", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://host/service/Customers('BOTTM')/ProductBeingViewed/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("FavouriteProducts", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://host/service/Customers('BOTTM')/FavouriteProducts", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://host/service/Customers('BOTTM')/FavouriteProducts/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.Equal("Orders", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://host/service/Customers('BOTTM')/Orders", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://host/service/Customers('BOTTM')/Orders/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("Address", nestedResourceInfo.Name));
            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) => Assert.Equal("City", nestedResourceInfo.Name));

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.Equal("http://host/service/Customers('BOTTM')", deltaResource.Id.AbsoluteUri);
                        Assert.NotNull(deltaResource.EditLink);
                        Assert.EndsWith("Customers('BOTTM')", deltaResource.EditLink.OriginalString);
                        var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("ContactName", contactNameProperty.Name);
                        Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var internalVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();
                        internalVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }));

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithMultipleExpandedCollectionNavigationPropertyAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders\":[{" +
                "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                "\"Id\":10643," +
                "\"Address\":{" +
                "\"Street\":\"23 Tsawassen Blvd.\"," +
                "\"City\":{\"CityName\":\"Tsawassen\"}," +
                "\"Region\":\"BC\"," +
                "\"PostalCode\":\"T2F 8M4\"}}]," +
                "\"FavouriteProducts\":[{\"@odata.id\":\"http://host/service/Products(1)\",\"Id\":1,\"Name\":\"Car\"}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.NotNull(resource.Id);
                Assert.Equal("http://host/service/Products(1)", resource.Id.AbsoluteUri);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Order", resource.TypeName);
                Assert.NotNull(resource.Id);
                Assert.Equal("http://host/service/Orders(10643)", resource.Id.AbsoluteUri);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10643, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.Equal("http://host/service/Customers('BOTTM')", deltaResource.Id.AbsoluteUri);
                        var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("ContactName", contactNameProperty.Name);
                        Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithExpandedContainedCollectionNavigationPropertyAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"FavouriteProducts\":[{" +
                "\"@odata.id\":\"http://host/service/Products(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Car\"," +
                "\"Details\":[{\"@odata.type\":\"#MyNS.ProductDetail\",\"Id\":1,\"Detail\":\"made in china\"}]}]}]}";

            var resourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.NotNull(resource.Id);
                Assert.Equal("http://host/service/Products(1)", resource.Id.AbsoluteUri);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.ProductDetail", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Detail", properties[1].Name);
                Assert.Equal("made in china", properties[1].Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.Equal("http://host/service/Customers('BOTTM')", deltaResource.Id.AbsoluteUri);
                        var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("ContactName", contactNameProperty.Name);
                        Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
                    },
                    verifyResourceSetAction: (resourceSet) => resourceSetCount++,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Equal(2, resourceSetCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithExpandedSingletonNavigationPropertyAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"ProductBeingViewed\":{" +
                "\"@odata.id\":\"http://host/service/Products(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Car\"," +
                "\"Details\":[{\"@odata.type\":\"#MyNS.ProductDetail\",\"Id\":1,\"Detail\":\"made in china\"}]}}]}";

            var resourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.NotNull(resource.Id);
                Assert.Equal("http://host/service/Products(1)", resource.Id.AbsoluteUri);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.ProductDetail", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Detail", properties[1].Name);
                Assert.Equal("made in china", properties[1].Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Customer", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.Equal("http://host/service/Customers('BOTTM')", deltaResource.Id.AbsoluteUri);
                        var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("ContactName", contactNameProperty.Name);
                        Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
                    },
                    verifyResourceSetAction: (resourceSet) => resourceSetCount++,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Equal(1, resourceSetCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithNestedComplexPropertyAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Orders/$delta\"," +
                "\"value\":[{" +
                "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                "\"Id\":10643," +
                "\"Address\":{" +
                "\"Street\":\"23 Tsawassen Blvd.\"," +
                "\"City\":{\"CityName\":\"Tsawassen\"}," +
                "\"Region\":\"BC\"," +
                "\"PostalCode\":\"T2F 8M4\"}}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                orders,
                order,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Order", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.Equal("http://host/service/Orders(10643)", deltaResource.Id.AbsoluteUri);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(10643, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeltaPayloadWithNestedCollectionOfComplexPropertyAsync(bool isResponse)
        {
            var payload = "{\"@odata.context\":\"http://host/service/$metadata#Orders/$delta\"," +
                    "\"value\":[{" +
                    "\"@odata.id\":\"http://host/service/Orders(10643)\"," +
                    "\"Id\":10643," +
                    "\"Addresses@odata.type\":\"#Collection(MyNS.Address)\"," +
                    "\"Addresses\":[" +
                    "{\"@odata.type\":\"#MyNS.Address\"," +
                    "\"Street\":\"23 Tsawassen Blvd.\"," +
                    "\"City\":{\"CityName\":\"Tsawassen\"}," +
                    "\"Region\":\"BC\"," +
                    "\"PostalCode\":\"T2F 8M4\"}," +
                    "{\"@odata.type\":\"#MyNS.Address\"," +
                    "\"Street\":\"ZixingRoad.\"," +
                    "\"City\":{\"CityName\":\"Shanghai\"}," +
                    "\"PostalCode\":\"200001\"}]}]}";

            var resourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("ZixingRoad.", properties[0].Value);
                Assert.Equal("PostalCode", properties[1].Name);
                Assert.Equal("200001", properties[1].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Shanghai", cityNameProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Address", resource.TypeName);
                Assert.Equal(3, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("23 Tsawassen Blvd.", properties[0].Value);
                Assert.Equal("Region", properties[1].Name);
                Assert.Equal("BC", properties[1].Value);
                Assert.Equal("PostalCode", properties[2].Name);
                Assert.Equal("T2F 8M4", properties[2].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.City", resource.TypeName);
                var cityNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("CityName", cityNameProperty.Name);
                Assert.Equal("Tsawassen", cityNameProperty.Value);
            });

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                orders,
                order,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeltaResourceAction: (deltaResource) =>
                    {
                        Assert.NotNull(deltaResource);
                        Assert.Equal("MyNS.Order", deltaResource.TypeName);
                        Assert.NotNull(deltaResource.Id);
                        Assert.Equal("http://host/service/Orders(10643)", deltaResource.Id.AbsoluteUri);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deltaResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(10643, idProperty.Value);
                    },
                    verifyResourceSetAction: (resourceSet) => resourceSetCount++,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var internalVerifyResourceAction = verifyResourceActionStack.Pop();
                        internalVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Equal(1, resourceSetCount);
            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithODataIdAnnotationAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"@id\":\"Customers/1\"}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("Customers/1", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithKeyPropertyAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("http://host/service/Customers(1)", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithRemovedPropertyAtEndAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"@removed\":{\"reason\":\"changed\"}}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("http://host/service/Customers(1)", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithRemovedPropertyEmptyAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{}," +
                "\"Id\":1}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("http://host/service/Customers(1)", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithRemovedPropertyNullAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{\"@removed\":null," +
                "\"Id\":1}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("http://host/service/Customers(1)", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithRemovedPropertyValueContainingExtraPropertiesAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\",\"extraProperty\":\"value\",\"@extra.annotation\":\"annotationValue\"}," +
                "\"Id\":1}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("http://host/service/Customers(1)", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"ContactName\":\"Samantha Stones\"}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(
                    jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("http://host/service/Customers(1)", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadDeletedEntryIgnoresExtraPropertiesAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\"," +
                "\"id\":\"Customers('BOTTM')\"," +
                "\"reason\":\"deleted\"," +
                "\"ContactName\":\"Susan Halvenstern\"}]}";

            await SetupJsonDeltaReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonDeltaReader) => DoReadAsync(
                    jsonDeltaReader,
                    verifyDeletedEntryAction: (deletedEntry) =>
                    {
                        Assert.NotNull(deletedEntry);
                        Assert.NotNull(deletedEntry.Id);
                        Assert.Equal("Customers('BOTTM')", deletedEntry.Id);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedEntry.Reason);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithNestedResourceAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"ProductBeingViewed\":{\"Name\":\"Scissors\",\"Id\":10}}]}";

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("MyNS.Product", resource.TypeName);
                        Assert.Equal(2, resource.Properties.Count());
                        var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                        Assert.Equal("Name", properties[0].Name);
                        Assert.Equal("Scissors", properties[0].Value);
                        Assert.Equal("Id", properties[1].Name);
                        Assert.Equal(10, properties[1].Value);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Read401DeletedEntryWithNestedNullResourceAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"ProductBeingViewed\":null}]}";

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.Null(resource);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeltaPayloadWithNestedResourceAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"ProductBeingViewed\":{\"Name\":\"Scissors\",\"Id\":10},\"ContactName\":\"Samantha Stones\"}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("ContactName", properties[1].Name);
                Assert.Equal("Samantha Stones", properties[1].Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Name", properties[0].Name);
                Assert.Equal("Scissors", properties[0].Value);
                Assert.Equal("Id", properties[1].Name);
                Assert.Equal(10, properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithNestedDeletedEntryAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":10}}]}";

            var verifyDeletedResourceActionStack = new Stack<Action<ODataDeletedResource>>();
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Product", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                Assert.Equal(2, deletedResource.Properties.Count());
                var properties = deletedResource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Name", properties[0].Name);
                Assert.Equal("Scissors", properties[0].Value);
                Assert.Equal("Id", properties[1].Name);
                Assert.Equal(10, properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotEmpty(verifyDeletedResourceActionStack);
                        var innerVerifyDeletedResourceAction = verifyDeletedResourceActionStack.Pop();
                        innerVerifyDeletedResourceAction(deletedResource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyDeletedResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeletedEntryWithNestedDerivedDeletedEntryAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":10,\"Name\":\"car\",\"Material\":\"gold\"}}]}";

            var verifyDeletedResourceActionStack = new Stack<Action<ODataDeletedResource>>();
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.PhysicalProduct", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                Assert.Equal(3, deletedResource.Properties.Count());
                var properties = deletedResource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(10, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("car", properties[1].Value);
                Assert.Equal("Material", properties[2].Name);
                Assert.Equal("gold", properties[2].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotEmpty(verifyDeletedResourceActionStack);
                        var innerVerifyDeletedResourceAction = verifyDeletedResourceActionStack.Pop();
                        innerVerifyDeletedResourceAction(deletedResource);
                    }),
                isResponse: isResponse);

            Assert.Empty(verifyDeletedResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeltaPayloadWithNestedDeletedEntryAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":10}}]}";

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Product", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                        Assert.Equal(2, deletedResource.Properties.Count());
                        var properties = deletedResource.Properties.OfType<ODataProperty>().ToArray();
                        Assert.Equal("Name", properties[0].Name);
                        Assert.Equal("Scissors", properties[0].Value);
                        Assert.Equal("Id", properties[1].Name);
                        Assert.Equal(10, properties[1].Value);

                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("MyNS.Customer", resource.TypeName);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    }),
                isResponse: isResponse);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithNestedDeltaResourceSetInResponsePayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":5," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            var verifyDeltaResourceSetActionStack = new Stack<Action<ODataDeltaResourceSet>>();
            verifyDeltaResourceSetActionStack.Push((deltaResourceSet) => Assert.NotNull(deltaResourceSet));
            verifyDeltaResourceSetActionStack.Push((deltaResourceSet) =>
            {
                Assert.Equal(5, deltaResourceSet.Count);
                Assert.Equal("http://host/service/Customers?$skipToken=5", deltaResourceSet.NextPageLink.AbsoluteUri);
            });

            var verifyDeletedResourceActionStack = new Stack<Action<ODataDeletedResource>>();
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Product", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10, idProperty.Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceSetActionStack);
                        var innerVerifyDeltaResourceSetAction = verifyDeltaResourceSetActionStack.Pop();
                        innerVerifyDeltaResourceSetAction(deltaResourceSet);
                    },
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotEmpty(verifyDeletedResourceActionStack);
                        var innerVerifyDeletedResourceAction = verifyDeletedResourceActionStack.Pop();
                        innerVerifyDeletedResourceAction(deletedResource);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("MyNS.Product", resource.TypeName);
                        Assert.Equal(2, resource.Properties.Count());
                        var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Car", properties[1].Value);
                    }));

            Assert.Empty(verifyDeltaResourceSetActionStack);
            Assert.Empty(verifyDeletedResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithNestedDeltaResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            var deltaResourceSetCount = 0;
            var verifyDeletedResourceActionStack = new Stack<Action<ODataDeletedResource>>();
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);
                Assert.Equal("MyNS.Product", deletedResource.TypeName);
                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10, idProperty.Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => deltaResourceSetCount++,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotEmpty(verifyDeletedResourceActionStack);
                        var innerVerifyDeletedResourceAction = verifyDeletedResourceActionStack.Pop();
                        innerVerifyDeletedResourceAction(deletedResource);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("MyNS.Product", resource.TypeName);
                        Assert.Equal(2, resource.Properties.Count());
                        var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Car", properties[1].Value);
                    }),
                isResponse: false);

            Assert.Equal(2, deltaResourceSetCount);
            Assert.Empty(verifyDeletedResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithEmptyNestedDeltaResourceSetInResponsePayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":2," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts@delta\":[]}]}";

            var verifyDeltaResourceSetActionStack = new Stack<Action<ODataDeltaResourceSet>>();
            verifyDeltaResourceSetActionStack.Push((deltaResourceSet) => Assert.NotNull(deltaResourceSet));
            verifyDeltaResourceSetActionStack.Push((deltaResourceSet) =>
            {
                Assert.Equal(2, deltaResourceSet.Count);
                Assert.Equal("http://host/service/Customers?$skipToken=5", deltaResourceSet.NextPageLink.AbsoluteUri);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceSetActionStack);
                        var innerVerifyDeltaResourceSetAction = verifyDeltaResourceSetActionStack.Pop();
                        innerVerifyDeltaResourceSetAction(deltaResourceSet);
                    },
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    }));

            Assert.Empty(verifyDeltaResourceSetActionStack);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithEmptyNestedDeltaResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"FavouriteProducts@delta\":[]}]}";

            var deltaResourceSetCount = 0;

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => deltaResourceSetCount++,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    }),
                isResponse: false);

            Assert.Equal(2, deltaResourceSetCount);
        }

        [Fact]
        public async Task ReadV401DeltaPayloadWithNestedDeltaResourceSetInResponsePayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":5," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            var verifyDeltaResourceSetActionStack = new Stack<Action<ODataDeltaResourceSet>>();
            verifyDeltaResourceSetActionStack.Push((deltaResourceSet) => Assert.NotNull(deltaResourceSet));
            verifyDeltaResourceSetActionStack.Push((deltaResourceSet) =>
            {
                Assert.Equal(5, deltaResourceSet.Count);
                Assert.Equal("http://host/service/Customers?$skipToken=5", deltaResourceSet.NextPageLink.AbsoluteUri);
            });

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotEmpty(verifyDeltaResourceSetActionStack);
                        var innerVerifyDeltaResourceSetAction = verifyDeltaResourceSetActionStack.Pop();
                        innerVerifyDeltaResourceSetAction(deltaResourceSet);
                    },
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Product", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(10, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }));

            Assert.Empty(verifyDeltaResourceSetActionStack);
            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeltaPayloadWithNestedDeltaResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}]}";

            var deltaResourceSetCount = 0;
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => deltaResourceSetCount++,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Product", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(10, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }),
                isResponse: false);

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401TopLevelResourceWithNestedDeltaResourceSetInResponsePayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":5," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(5, deltaResourceSet.Count);
                        Assert.Equal("http://host/service/Customers?$skipToken=5", deltaResourceSet.NextPageLink.AbsoluteUri);
                    },
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Product", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(10, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }),
                readingResourceSet: false);

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401TopLevelResourceWithNestedDeltaResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Product", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(10, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }),
                readingResourceSet: false,
                isResponse: false);

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithNestedResourceSetInResponsePayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":5," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => Assert.NotNull(deltaResourceSet),
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    },
                    verifyResourceSetAction: (resourceSet) =>
                    {
                        Assert.NotNull(resourceSet);
                        Assert.Equal(5, resourceSet.Count);
                        Assert.Equal("http://host/service/Customers?$skipToken=5", resourceSet.NextPageLink.AbsoluteUri);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeletedEntryWithNestedResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"changed\"}," +
                "\"Id\":1," +
                "\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => Assert.NotNull(deltaResourceSet),
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Customer", deletedResource.TypeName);
                        Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    },
                    verifyResourceSetAction: (resourceSet) => Assert.NotNull(resourceSet),
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }),
                isResponse: false);

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeltaPayloadWithNestedResourceSetInResponsePayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":5," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => Assert.NotNull(deltaResourceSet),
                    verifyResourceSetAction: (resourceSet) =>
                    {
                        Assert.NotNull(resourceSet);
                        Assert.Equal(5, resourceSet.Count);
                        Assert.Equal("http://host/service/Customers?$skipToken=5", resourceSet.NextPageLink.AbsoluteUri);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV401DeltaPayloadWithNestedResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{\"Id\":1,\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"},{\"Id\":10}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Customer", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(1, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                var idProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                Assert.Equal("Id", idProperty.Name);
                Assert.Equal(10, idProperty.Value);
            });
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("MyNS.Product", resource.TypeName);
                Assert.Equal(2, resource.Properties.Count());
                var properties = resource.Properties.OfType<ODataProperty>().ToArray();
                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Car", properties[1].Value);
            });

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) => Assert.NotNull(deltaResourceSet),
                    verifyResourceSetAction: (resourceSet) => Assert.NotNull(resourceSet),
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    }),
                isResponse: false);

            Assert.Empty(verifyResourceActionStack);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeltaPayloadWithNestedDeletedEntryFromDifferentResourceSetAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@id\":\"Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"}," +
                "{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}";

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.Order", deletedResource.TypeName);
                        var idProperty = Assert.IsType<ODataProperty>(Assert.Single(deletedResource.Properties));
                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("MyNS.Customer", resource.TypeName);
                        Assert.NotNull(resource.Id);
                        Assert.EndsWith("Customers('BOTTM')", resource.Id.OriginalString);
                        var contactNameProperty = Assert.IsType<ODataProperty>(Assert.Single(resource.Properties));
                        Assert.Equal("ContactName", contactNameProperty.Name);
                        Assert.Equal("Susan Halvenstern", contactNameProperty.Value);
                    }),
                isResponse: isResponse);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadV401DeltaPayloadWithNestedDerivedDeletedEntryAsync(bool isResponse)
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@odata.type\":\"#MyNS.PreferredCustomer\",\"Id\":1,\"HonorLevel\":\"Gold\"}]}";

            await SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader,
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotNull(deletedResource);
                        Assert.Equal("MyNS.PreferredCustomer", deletedResource.TypeName);
                        var properties = deletedResource.Properties.OfType<ODataProperty>().ToArray();
                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("HonorLevel", properties[1].Name);
                        Assert.Equal("Gold", properties[1].Value);
                    }),
                isResponse: isResponse);
        }

        [Fact]
        public async Task ReadV401TopLevelResourceWithNestedDeltaResourceSetWithNestedDeletedEntryInResponsePayloadAsync_ThrowsException()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"FavouriteProducts@count\":5," +
                "\"FavouriteProducts@nextLink\":\"http://host/service/Customers?$skipToken=5\"," +
                "\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader),
                readingResourceSet: false,
                readingDelta: false));

            Assert.Equal(
                Strings.ODataJsonResourceDeserializer_UnexpectedDeletedEntryInResponsePayload,
                exception.Message);
        }

        [Fact]
        public async Task ReadV401DeltaPayloadWithNestedDeletedEntryFromDifferentResourceSetAsync_ThrowsException()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@id\":\"Customers('BOTTM')\"," +
                "\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders\":[{\"@context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"Id\":1}]}]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonReaderAndRunTestAsync(
                payload,
                customers,
                customer,
                (jsonReader) => DoReadAsync(jsonReader)));

            Assert.Equal(
                Strings.ReaderValidationUtils_ContextUriValidationInvalidExpectedEntitySet(
                    "http://host/service/$metadata#Customers/$deletedEntity",
                    "Customers",
                    "Customers.Orders"),
                exception.Message);
        }

        #endregion Async Tests

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
                    customer.AddStructuralProperty("StreamProperty", EdmPrimitiveTypeKind.Stream);
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

        private IEnumerable<Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>> ReadItem(string payload, IEdmModel model = null, IEdmNavigationSource navigationSource = null, IEdmEntityType entityType = null, bool isResponse = true, bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            var settings = new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = s => true,
                EnableReadingODataAnnotationWithoutPrefix = enableReadingODataAnnotationWithoutPrefix
            };

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = model ?? new EdmModel(),
                ServiceProvider = ServiceProviderHelper.BuildServiceProvider(null)
            };

            using (var inputContext = new ODataJsonInputContext(
                new StringReader(payload), messageInfo, settings))
            {
                var jsonReader = new ODataJsonDeltaReader(inputContext, navigationSource, entityType);
                while (jsonReader.Read())
                {
                    yield return new Tuple<ODataItem, ODataDeltaReaderState, ODataReaderState>(jsonReader.Item, jsonReader.State, jsonReader.SubState);
                }
            }
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
                        Assert.EndsWith(customerDeletedEntry.Id, deltaDeletedEntry.Id);
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
                                    Assert.Equal(10643, Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "Id")).Value);
                                }
                                else if (entry.TypeName == "MyNS.Product")
                                {
                                    Assert.Equal(1, Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "Id")).Value);
                                }
                                else if (entry.TypeName == "MyNS.ProductDetail")
                                {
                                    Assert.Equal(1, Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "Id")).Value);
                                }
                                else if (entry.TypeName == "MyNS.Address")
                                {
                                    Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "Street")).Value);
                                }
                                else if (entry.TypeName == "MyNS.City")
                                {
                                    Assert.NotNull(Assert.IsType<ODataProperty>(entry.Properties.Single(p => p.Name == "CityName")).Value);
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

        private ODataReader GetODataReader(string deltaPayload, IEdmModel model, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool isResponse, bool keyAsSegment = true, bool singleResource = false)
        {
            var settings = new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = s => true,
                EnableReadingKeyAsSegment = keyAsSegment,
            };

            var messageInfo = new ODataMessageInfo
            {
                IsResponse = isResponse,
                MediaType = new ODataMediaType("application", "json"),
                IsAsync = false,
                Model = model ?? new EdmModel(),
                ServiceProvider = ServiceProviderHelper.BuildServiceProvider(null)
            };

            var inputContext = new ODataJsonInputContext(
                new StringReader(deltaPayload), messageInfo, settings);
            inputContext.MessageReaderSettings.EnableReadingODataAnnotationWithoutPrefix = true;
            return new ODataJsonReader(inputContext, navigationSource, entityType, /*readingResourceSet*/!singleResource, /*readingParameter*/false, /*readingDelta*/ true);
        }

        private ODataReader GetODataResourceReader(string deltaPayload, IEdmModel model, IEdmNavigationSource navigationSource, IEdmEntityType entityType, bool isResponse)
        {
            var settings = new ODataMessageReaderSettings
            {
                ShouldIncludeAnnotation = s => true,
                Version = ODataVersion.V401
            };

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(deltaPayload);
            streamWriter.Flush();
            stream.Seek(0, SeekOrigin.Begin);

            InMemoryMessage message = new InMemoryMessage
            {
                Stream = stream,
            };

            ODataMessageReader reader;
            if (isResponse)
            {
                var responseMessage = new ODataResponseMessage(message, true, true, 2048);
                reader = new ODataMessageReader(responseMessage, settings, model ?? new EdmModel());
            }
            else
            {
                var requestMessage = new ODataRequestMessage(message, true, true, 2048);
                reader = new ODataMessageReader(requestMessage, settings, model ?? new EdmModel());
            }

            return reader.CreateODataResourceReader(navigationSource, entityType);
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

        private bool PropertiesEqual(IEnumerable<ODataPropertyInfo> first, IEnumerable<ODataPropertyInfo> second)
        {
            var i = first.GetEnumerator();
            var j = second.GetEnumerator();

            while (i.MoveNext() && j.MoveNext())
            {
                var iCurrent = Assert.IsType<ODataProperty>(i.Current);
                var jCurrent = Assert.IsType<ODataProperty>(j.Current);

                if (!iCurrent.Name.Equals(jCurrent.Name))
                {
                    return false;
                }

                if (!iCurrent.Value.Equals(jCurrent.Value))
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

        private async Task DoReadAsync(
            ODataJsonDeltaReader jsonDeltaReader,
            Action<ODataDeltaResourceSet> verifyDeltaResourceSetAction = null,
            Action<ODataResource> verifyDeltaResourceAction = null,
            Action<ODataResourceSet> verifyResourceSetAction = null,
            Action<ODataResource> verifyResourceAction = null,
            Action<ODataNestedResourceInfo> verifyNestedResourceInfoAction = null,
            Action<ODataDeltaDeletedEntry> verifyDeletedEntryAction = null,
            Action<ODataDeltaLinkBase> verifyDeltaLinkAction = null)
        {
            while (await jsonDeltaReader.ReadAsync())
            {
                switch (jsonDeltaReader.State)
                {
                    case ODataDeltaReaderState.DeltaResourceSetStart:
                        Assert.NotNull(jsonDeltaReader.Item as ODataDeltaResourceSet);
                        break;
                    case ODataDeltaReaderState.DeltaResourceSetEnd:
                        if (verifyDeltaResourceSetAction != null)
                        {
                            verifyDeltaResourceSetAction(jsonDeltaReader.Item as ODataDeltaResourceSet);
                        }

                        break;
                    case ODataDeltaReaderState.DeltaResourceStart:
                        Assert.NotNull(jsonDeltaReader.Item as ODataResource);
                        break;
                    case ODataDeltaReaderState.DeltaResourceEnd:
                        if (verifyDeltaResourceAction != null)
                        {
                            verifyDeltaResourceAction(jsonDeltaReader.Item as ODataResource);
                        }

                        break;
                    case ODataDeltaReaderState.NestedResource:
                        switch (jsonDeltaReader.SubState)
                        {
                            case ODataReaderState.Start:
                                Assert.NotNull(jsonDeltaReader.Item as ODataNestedResourceInfo);
                                break;
                            case ODataReaderState.Completed:
                                if (verifyNestedResourceInfoAction != null)
                                {
                                    verifyNestedResourceInfoAction(jsonDeltaReader.Item as ODataNestedResourceInfo);
                                }

                                break;
                            case ODataReaderState.ResourceStart:
                                Assert.NotNull(jsonDeltaReader.Item as ODataResource);
                                break;
                            case ODataReaderState.ResourceEnd:
                                if (verifyResourceAction != null)
                                {
                                    verifyResourceAction(jsonDeltaReader.Item as ODataResource);
                                }

                                break;
                            case ODataReaderState.ResourceSetStart:
                                Assert.NotNull(jsonDeltaReader.Item as ODataResourceSet);
                                break;
                            case ODataReaderState.ResourceSetEnd:
                                if (verifyResourceSetAction != null)
                                {
                                    verifyResourceSetAction(jsonDeltaReader.Item as ODataResourceSet);
                                }

                                break;
                            case ODataReaderState.NestedResourceInfoStart:
                                Assert.NotNull(jsonDeltaReader.Item as ODataNestedResourceInfo);
                                break;
                            case ODataReaderState.NestedResourceInfoEnd:
                                if (verifyNestedResourceInfoAction != null)
                                {
                                    verifyNestedResourceInfoAction(jsonDeltaReader.Item as ODataNestedResourceInfo);
                                }

                                break;
                            default:
                                break;
                        }

                        break;
                    case ODataDeltaReaderState.DeltaDeletedEntry:
                        if (verifyDeletedEntryAction != null)
                        {
                            verifyDeletedEntryAction(jsonDeltaReader.Item as ODataDeltaDeletedEntry);
                        }

                        break;
                    case ODataDeltaReaderState.DeltaLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonDeltaReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataDeltaReaderState.DeltaDeletedLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonDeltaReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private async Task DoReadAsync(
            ODataJsonReader jsonReader,
            Action<ODataResourceSet> verifyResourceSetAction = null,
            Action<ODataResource> verifyResourceAction = null,
            Action<ODataNestedResourceInfo> verifyNestedResourceInfoAction = null,
            Action<ODataDeltaResourceSet> verifyDeltaResourceSetAction = null,
            Action<ODataDeletedResource> verifyDeletedResourceAction = null,
            Action<ODataDeltaLinkBase> verifyDeltaLinkAction = null)
        {
            while (await jsonReader.ReadAsync())
            {
                switch (jsonReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        break;
                    case ODataReaderState.ResourceSetEnd:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceStart:
                        break;
                    case ODataReaderState.ResourceEnd:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonReader.Item as ODataResource);
                        }

                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        break;
                    case ODataReaderState.NestedResourceInfoEnd:
                        if (verifyNestedResourceInfoAction != null)
                        {
                            verifyNestedResourceInfoAction(jsonReader.Item as ODataNestedResourceInfo);
                        }

                        break;
                    case ODataReaderState.DeltaResourceSetStart:
                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        if (verifyDeltaResourceSetAction != null)
                        {
                            verifyDeltaResourceSetAction(jsonReader.Item as ODataDeltaResourceSet);
                        }

                        break;
                    case ODataReaderState.DeletedResourceStart:
                        break;
                    case ODataReaderState.DeletedResourceEnd:
                        if (verifyDeletedResourceAction != null)
                        {
                            verifyDeletedResourceAction(jsonReader.Item as ODataDeletedResource);
                        }

                        break;
                    case ODataReaderState.DeltaLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.DeltaDeletedLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private ODataJsonInputContext CreateJsonInputContext(string payload, bool isAsync = false, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = this.Model,
                ServiceProvider = ServiceProviderHelper.BuildServiceProvider(null)
            };

            return new ODataJsonInputContext(new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        /// <summary>
        /// Sets up an ODataJsonReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonReaderAndRunTestAsync(
            string payload,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType expectedResourceType,
            Func<ODataJsonReader, Task> func,
            bool readingResourceSet = true,
            bool readingDelta = true,
            bool isResponse = true)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: true, isResponse: isResponse))
            {
                var jsonReader = new ODataJsonReader(
                    jsonInputContext,
                    navigationSource,
                    expectedResourceType,
                    readingResourceSet: readingResourceSet,
                    readingParameter: false,
                    readingDelta: readingDelta,
                    listener: null);

                await func(jsonReader);
            }
        }

        /// <summary>
        /// Sets up an ODataJsonDeltaReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonDeltaReaderAndRunTestAsync(
            string payload,
            IEdmNavigationSource navigationSource,
            IEdmEntityType expectedEntityType,
            Func<ODataJsonDeltaReader, Task> func,
            bool isResponse = true,
            bool enableReadingODataAnnotationWithoutPrefix = false)
        {
            using (var jsonInputContext = CreateJsonInputContext(payload, isAsync: true, isResponse: isResponse))
            {
                jsonInputContext.MessageReaderSettings.EnableReadingODataAnnotationWithoutPrefix = enableReadingODataAnnotationWithoutPrefix;
                var jsonDeltaReader = new ODataJsonDeltaReader(
                    jsonInputContext,
                    navigationSource,
                    expectedEntityType);

                await func(jsonDeltaReader);
            }
        }

        #endregion Private Methods
    }
}
