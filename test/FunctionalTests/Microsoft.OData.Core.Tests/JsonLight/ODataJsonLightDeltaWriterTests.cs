//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Microsoft.OData.JsonLight;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightDeltaWriterTests
    {
        private ODataJsonLightOutputContext outputContext;
        private MemoryStream stream;
        private EdmModel myModel;

        #region Entities

        private readonly ODataDeltaResourceSet feed = new ODataDeltaResourceSet
        {
            Count = 5,
            SerializationInfo = new ODataDeltaResourceSetSerializationInfo
            {
                EntitySetName = "Customers",
                EntityTypeName = "MyNS.Customer",
                ExpectedTypeName = "MyNS.Customer"
            },
            DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
        };

        private readonly ODataDeltaResourceSet feedWithoutInfo = new ODataDeltaResourceSet
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
            },
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = "Customer",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Customers"
            }
        };

        private readonly ODataDeltaDeletedLink linkToOrder10643 = new ODataDeltaDeletedLink(new Uri("Customers('ALFKI')", UriKind.Relative), new Uri("Orders('10643')", UriKind.Relative), "Orders");

        private readonly ODataDeltaLink linkToOrder10645 = new ODataDeltaLink(new Uri("Customers('BOTTM')", UriKind.Relative), new Uri("Orders('10645')", UriKind.Relative), "Orders");

        private readonly ODataResource order10643 = new ODataResource
        {
            Id = new Uri("Orders(10643)", UriKind.Relative),
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = "Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            },
        };

        private readonly ODataDeltaDeletedEntry customerDeleted = new ODataDeltaDeletedEntry("Customers('ANTON')", DeltaDeletedEntryReason.Deleted);

        #endregion

        [Fact]
        public void WriteExample30FromV4SpecWithModel()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(feedWithoutInfo);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd();
            writer.WriteDeltaDeletedLink(linkToOrder10643);
            writer.WriteDeltaLink(linkToOrder10645);
            writer.WriteStart(order10643);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd(); // shippingAddress
            writer.WriteEnd(); // shippingAddressInfo
            writer.WriteEnd();
            writer.WriteDeltaDeletedEntry(customerDeleted);
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}");
        }

        [Fact]
        public void WriteExample30FromV4SpecWithoutModel()
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, null, null);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd();
            writer.WriteDeltaDeletedLink(linkToOrder10643);
            writer.WriteDeltaLink(linkToOrder10645);
            writer.WriteStart(order10643);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd(); // shippingAddress
            writer.WriteEnd(); // shippingAddressInfo
            writer.WriteEnd();
            writer.WriteDeltaDeletedEntry(customerDeleted);
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}");
        }

        [Fact]
        public void WriteEmptyDeltaFeed()
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, null, null);
            writer.WriteStart(feed);
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[]}");
        }

        [Fact]
        public void WriteContainedEntityInDeltaFeed()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();
            ODataResource containedEntry = new ODataResource()
            {
                TypeName = "MyNS.ProductDetail",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Detail", Value = new ODataPrimitiveValue("made in china")},
                },
            };

            containedEntry.SetSerializationInfo(new ODataResourceSerializationInfo()
            {
                NavigationSourceEntityTypeName = "MyNS.ProductDetail",
                NavigationSourceName = "Products(1)/Details",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });

            ODataResource containedInContainedEntity = new ODataResource()
            {
                TypeName = "MyNS.ProductDetailItem",
                Properties = new[]
                {
                    new ODataProperty {Name = "ItemId", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Description", Value = new ODataPrimitiveValue("made by HCC")},
                },
            };

            containedInContainedEntity.SetSerializationInfo(new ODataResourceSerializationInfo()
            {
                NavigationSourceEntityTypeName = "MyNS.ProductDetailItem",
                NavigationSourceName = "Products(1)/Details(1)/Items",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(containedInContainedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details(1)/Items/$entity\",\"ItemId\":1,\"Description\":\"made by HCC\"}]}");
        }

        [Fact]
        public void WriteContainedEntityInDeltaFeedWithSelectExpand()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();

            ODataResource entry = new ODataResource()
            {
                TypeName = "MyNS.Product",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("Car")},
                },
            };

            ODataResource containedEntry = new ODataResource()
            {
                TypeName = "MyNS.ProductDetail",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Detail", Value = new ODataPrimitiveValue("made in china")},
                },
            };

            containedEntry.SetSerializationInfo(new ODataResourceSerializationInfo()
            {
                NavigationSourceEntityTypeName = "MyNS.ProductDetail",
                NavigationSourceName = "Products(1)/Details",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });


            var result = new ODataQueryOptionParser(this.GetModel(), this.GetProductType(), this.GetProducts(), new Dictionary<string, string> { { "$expand", "Details($select=Detail)" }, { "$select", "Name" } }).ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://host/service"),
                SelectAndExpand = result
            };

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), false, odataUri);
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(entry);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products(Name,Details(Detail))/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"Id\":1,\"Name\":\"Car\"}]}");
        }

        [Fact]
        public void WriteEntityInDeltaFeedWithSelectExpand()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();

            ODataResource orderEntry = new ODataResource()
            {
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceEntityTypeName = "Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                    NavigationSourceName = "Orders"
                },
            };

            ODataNestedResourceInfo shippingAddressInfo = new ODataNestedResourceInfo
            {
                Name = "ShippingAddress",
                IsCollection = false
            };

            ODataResource shippingAddress = new ODataResource
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "City", Value = "Shanghai" },
                }
            };

            var result = new ODataQueryOptionParser(this.GetModel(), this.GetCustomerType(), this.GetCustomers(), new Dictionary<string, string> { { "$expand", "Orders($select=ShippingAddress)" }, { "$select", "ContactName" } }).ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://host/service"),
                SelectAndExpand = result
            };

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), false, odataUri);
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(orderEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products(ContactName,Orders(ShippingAddress))/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}");
        }

        [Fact]
        public void WriteDerivedEntity()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();
            ODataResource derivedEntity = new ODataResource()
            {
                TypeName = "MyNS.PhysicalProduct",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("car")},
                    new ODataProperty {Name = "Material", Value = new ODataPrimitiveValue("gold")},
                },
            };

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(derivedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}");
        }

        [Fact]
        public void WriteDerivedEntityWithSerilizationInfo()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();
            ODataResource derivedEntity = new ODataResource()
            {
                TypeName = "MyNS.PhysicalProduct",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("car")},
                    new ODataProperty {Name = "Material", Value = new ODataPrimitiveValue("gold")},
                },

                SerializationInfo = new ODataResourceSerializationInfo()
                {
                    ExpectedTypeName = "MyNS.Product",
                    NavigationSourceEntityTypeName = "MyNS.Product",
                    NavigationSourceName = "Products",
                }
            };

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(feed);
            writer.WriteStart(derivedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products/$entity\",\"@odata.type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}");
        }

        [Fact]
        public void JsonLightDeltaResourceScopePropertyAccessors()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Assembly assembly = typeof(ODataAction).GetTypeInfo().Assembly;
            Type typeWriter = assembly.GetType("Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter");

            writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(feedWithoutInfo);
            writer.WriteStart(customerUpdated);

            // Get the CurrentDeltaResourceScope obj and type, and then exercise the setters and getters for its properties.
            PropertyInfo propCurrentDeltaResourceScope = typeWriter.GetProperty("CurrentDeltaResourceScope", bindFlags);
            object objCurrentDeltaResourceScope = propCurrentDeltaResourceScope.GetValue(writer);
            Type typeCurrentDeltaResourceScope = typeWriter.GetNestedType("JsonLightDeltaResourceScope", bindFlags);

            //ODataJsonLightDeltaWriter.JsonLightDeltaResourceScope properties
            string[] propertyNames = new string[]
            {
                "ReadLinkWritten",
                "MediaEditLinkWritten",
                "MediaReadLinkWritten",
                "MediaContentTypeWritten",
                "MediaETagWritten",
            };

            foreach (string propertyName in propertyNames)
            {
                PropertyInfo currentPropertyInfo = typeCurrentDeltaResourceScope.GetProperty(propertyName, bindFlags);

                object currentProperty = currentPropertyInfo.GetValue(objCurrentDeltaResourceScope);
                currentProperty.Should().Be(false);

                // value should only be set from false to true.
                currentPropertyInfo.SetValue(objCurrentDeltaResourceScope, true);
                currentProperty = currentPropertyInfo.GetValue(objCurrentDeltaResourceScope);
                currentProperty.Should().Be(true);
            }
        }
        #region Expanded Feeds

        #region Test Data

        private readonly ODataDeltaResourceSet deltaFeed = new ODataDeltaResourceSet();

        private readonly ODataDeltaResourceSet deltaFeedWithInfo = new ODataDeltaResourceSet
        {
            SerializationInfo = new ODataDeltaResourceSetSerializationInfo
            {
                EntitySetName = "Customers",
                EntityTypeName = "MyNS.Customer"
            }
        };

        private readonly ODataResource customerEntry = new ODataResource
        {
            Id = new Uri("http://host/service/Customers('BOTTM')"),
            Properties = new[]
                {
                    new ODataProperty { Name = "ContactName", Value = "Susan Halvenstern" },
                },
            TypeName = "MyNS.Customer",
        };

        private readonly ODataNestedResourceInfo ordersNavigationLink = new ODataNestedResourceInfo
        {
            Name = "Orders",
            IsCollection = true
        };

        private readonly ODataResourceSet ordersFeed = new ODataResourceSet();

        private readonly ODataResource orderEntry = new ODataResource
        {
            Id = new Uri("http://host/service/Orders(10643)"),
            Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 10643 },
                },
            TypeName = "MyNS.Order"
        };

        private readonly ODataNestedResourceInfo shippingAddressInfo = new ODataNestedResourceInfo
        {
            Name = "ShippingAddress",
            IsCollection = false
        };

        private readonly ODataResource shippingAddress = new ODataResource
        {
            Properties = new List<ODataProperty>
            {
                new ODataProperty { Name = "Street", Value = "23 Tsawassen Blvd." },
                new ODataProperty { Name = "City", Value = "Tsawassen" },
                new ODataProperty { Name = "Region", Value = "BC" },
                new ODataProperty { Name = "PostalCode", Value = "T2F 8M4" }
            }
        };

        private readonly ODataResource orderEntryWithInfo = new ODataResource
        {
            Id = new Uri("http://host/service/Orders(10643)"),
            Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 10643 },
                },
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = "MyNS.Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            }
        };

        private readonly ODataNestedResourceInfo favouriteProductsNavigationLink = new ODataNestedResourceInfo
        {
            Name = "FavouriteProducts",
            IsCollection = true
        };

        private readonly ODataResourceSet favouriteProductsFeed = new ODataResourceSet();

        private readonly ODataResource productEntry = new ODataResource
        {
            Id = new Uri("http://host/service/Product(1)"),
            Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 1 },
                    new ODataProperty { Name = "Name", Value = "Car" },
                },
            TypeName = "MyNS.Product",
        };

        private readonly ODataResource customerEntryWithInfo = new ODataResource
        {
            Id = new Uri("http://host/service/Customers('BOTTM')"),
            Properties = new[]
                {
                    new ODataProperty { Name = "ContactName", Value = "Susan Halvenstern" },
                },
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = "MyNS.Customer",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Customers"
            }
        };

        private readonly ODataResource productDetailEntry = new ODataResource()
        {
            Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Detail", Value = new ODataPrimitiveValue("made in china")},
                },
            TypeName = "MyNS.ProductDetail",
        };

        private readonly ODataNestedResourceInfo detailsNavigationLink = new ODataNestedResourceInfo
        {
            Name = "Details",
            IsCollection = true
        };

        private readonly ODataResourceSet detailsFeed = new ODataResourceSet();

        private readonly ODataNestedResourceInfo productBeingViewedNavigationLink = new ODataNestedResourceInfo
        {
            Name = "ProductBeingViewed",
            IsCollection = false
        };

        #endregion

        private void WriteExpandedFeedWithModelImplementation()
        {
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersFeed);
            writer.WriteStart(orderEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd(); // shippingAddress
            writer.WriteEnd(); // shippingAddressInfo
            writer.WriteEnd(); // orderEntry
            writer.WriteEnd(); // ordersFeed
            writer.WriteEnd(); // ordersNavigationLink
            writer.WriteEnd(); // customerEntry
            writer.WriteEnd(); // deltaFeed
            writer.Flush();
        }

        [Fact]
        public void WriteExpandedFeedWithModelMinimalMetadata()
        {
            this.TestInit(this.GetModel());

            this.WriteExpandedFeedWithModelImplementation();

            this.TestPayload().Should().Be(
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
                                    "\"ShippingAddress\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":\"Tsawassen\"," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}");
        }

        [Fact]
        public void WriteExpandedFeedWithModelFullMetadata()
        {
            this.TestInit(this.GetModel(), true);

            this.WriteExpandedFeedWithModelImplementation();

            this.TestPayload().Should().Be(
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
                                    "\"ShippingAddress\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":\"Tsawassen\"," +
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
                "}");
        }

        [Fact]
        public void WriteExpandedFeedWithSerializationInfoMinimalMetadata()
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, null, null);
            writer.WriteStart(deltaFeedWithInfo);
            writer.WriteStart(customerEntryWithInfo);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersFeed);
            writer.WriteStart(orderEntryWithInfo);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd(); // shippingAddress
            writer.WriteEnd(); // shippingAddressInfo
            writer.WriteEnd(); // orderEntryWithInfo
            writer.WriteEnd(); // ordersFeed
            writer.WriteEnd(); // ordersNavigationLink
            writer.WriteEnd(); // customerEntry
            writer.WriteEnd(); // deltaFeedWithInfo
            writer.Flush();

            this.TestPayload().Should().Be(
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
                                    "\"ShippingAddress\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":\"Tsawassen\"," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}");
        }

        [Fact]
        public void WriteMultipleExpandedFeeds()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersFeed);
            writer.WriteStart(orderEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd(); // shippingAddress
            writer.WriteEnd(); // shippingAddressInfo
            writer.WriteEnd(); // orderEntry
            writer.WriteEnd(); // ordersFeed
            writer.WriteEnd(); // ordersNavigationLink
            writer.WriteStart(favouriteProductsNavigationLink);
            writer.WriteStart(favouriteProductsFeed);
            writer.WriteStart(productEntry);
            writer.WriteEnd(); // productEntry
            writer.WriteEnd(); // favouriteProductsFeed
            writer.WriteEnd(); // favouriteProductsNavigationLink
            writer.WriteEnd(); // customerEntry
            writer.WriteEnd(); // deltaFeed
            writer.Flush();

            this.TestPayload().Should().Be(
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
                                    "\"ShippingAddress\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":\"Tsawassen\"," +
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
                "}");
        }

        [Fact]
        public void WriteContainmentExpandedFeeds()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(favouriteProductsNavigationLink);
            writer.WriteStart(favouriteProductsFeed);
            writer.WriteStart(productEntry);
            writer.WriteStart(detailsNavigationLink);
            writer.WriteStart(detailsFeed);
            writer.WriteStart(productDetailEntry);
            writer.WriteEnd(); // productDetailEntry
            writer.WriteEnd(); // detailsFeed
            writer.WriteEnd(); // detailsNavigationLink
            writer.WriteEnd(); // productEntry
            writer.WriteEnd(); // favouriteProductsFeed
            writer.WriteEnd(); // favouriteProductsNavigationLink
            writer.WriteEnd(); // customerEntry
            writer.WriteEnd(); // deltaFeed
            writer.Flush();

            this.TestPayload().Should().Be(
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
                                    "\"@odata.id\":\"http://host/service/Product(1)\"," +
                                    "\"Id\":1," +
                                    "\"Name\":\"Car\"," +
                                    "\"Details@odata.context\":\"http://host/service/$metadata#Products(1)/Details\"," +
                                    "\"Details\":" +
                                    "[" +
                                        "{" +
                                            "\"Id\":1," +
                                            "\"Detail\":\"made in china\"" +
                                        "}" +
                                    "]" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}");
        }

        [Fact]
        public void CannotWriteExpandedNavigationPropertyOutsideDeltaEntry()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(ordersNavigationLink);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightDeltaWriter_InvalidTransitionToNestedResource("DeltaResourceSet", "NestedResource"));
        }

        [Fact]
        public void CannotWriteDeltaItemWhileWritingExpandedNavigationProperty()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(customerEntry);
                writer.WriteStart(ordersNavigationLink);
                writer.WriteStart(ordersFeed);
                writer.WriteDeltaDeletedEntry(customerDeleted);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightDeltaWriter_InvalidTransitionFromNestedResource("NestedResource", "DeltaDeletedEntry"));
        }

        [Fact]
        public void CannotWriteExpandedFeedOutsideNavigationLink()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(customerEntry);
                writer.WriteStart(ordersFeed);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState("DeltaResource"));
        }

        [Fact]
        public void CannotWriteExpandedFeedOutsideDeltaEntry()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(ordersFeed);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataJsonLightDeltaWriter_WriteStartExpandedResourceSetCalledInInvalidState("DeltaResourceSet"));
        }

        [Fact]
        public void WriteExpandedSingleton()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(productBeingViewedNavigationLink);
            writer.WriteStart(productEntry);
            writer.WriteStart(detailsNavigationLink);
            writer.WriteStart(detailsFeed);
            writer.WriteStart(productDetailEntry);
            writer.WriteEnd(); // productDetailEntry
            writer.WriteEnd(); // detailsFeed
            writer.WriteEnd(); // detailsNavigationLink
            writer.WriteEnd(); // productEntry
            writer.WriteEnd(); // productBeingViewedNavigationLink
            writer.WriteEnd(); // customerEntry
            writer.WriteEnd(); // deltaFeed
            writer.Flush();

            this.TestPayload().Should().Be(
                "{" +
                    "\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@odata.id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"ProductBeingViewed\":" +
                            "{" +
                                "\"@odata.id\":\"http://host/service/Product(1)\"," +
                                "\"Id\":1," +
                                "\"Name\":\"Car\"," +
                                "\"Details@odata.context\":\"http://host/service/$metadata#Products(1)/Details\"," +
                                "\"Details\":" +
                                "[" +
                                    "{" +
                                        "\"Id\":1," +
                                        "\"Detail\":\"made in china\"" +
                                    "}" +
                                "]" +
                            "}" +
                        "}" +
                    "]" +
                "}");
        }

        #endregion

        #region Test Helper Methods

        private void TestInit(IEdmModel userModel = null, bool fullMetadata = false)
        {
            this.stream = new MemoryStream();
            this.outputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata);
        }

        private IEdmModel GetModel()
        {
            if (myModel == null)
            {
                myModel = new EdmModel();

                EdmComplexType shippingAddress = new EdmComplexType("MyNS", "ShippingAddress");
                shippingAddress.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
                shippingAddress.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
                shippingAddress.AddStructuralProperty("Region", EdmPrimitiveTypeKind.String);
                shippingAddress.AddStructuralProperty("PostalCode", EdmPrimitiveTypeKind.String);
                myModel.AddElement(shippingAddress);

                EdmComplexTypeReference shippingAddressReference = new EdmComplexTypeReference(shippingAddress, true);

                EdmEntityType orderType = new EdmEntityType("MyNS", "Order");
                orderType.AddKeys(orderType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32));
                orderType.AddStructuralProperty("ShippingAddress", shippingAddressReference);
                myModel.AddElement(orderType);

                EdmEntityType customer = new EdmEntityType("MyNS", "Customer");
                customer.AddStructuralProperty("ContactName", EdmPrimitiveTypeKind.String);
                var customerId = customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
                customer.AddKeys(customerId);
                customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    Target = orderType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });
                myModel.AddElement(customer);

                var productType = new EdmEntityType("MyNS", "Product");
                var productId = productType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
                productType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
                productType.AddKeys(productId);
                myModel.AddElement(productType);

                var physicalProductType = new EdmEntityType("MyNS", "PhysicalProduct", productType);
                physicalProductType.AddStructuralProperty("Material", EdmPrimitiveTypeKind.String);
                myModel.AddElement(physicalProductType);

                var productDetailType = new EdmEntityType("MyNS", "ProductDetail");
                var productDetailId = productDetailType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
                productDetailType.AddStructuralProperty("Detail", EdmPrimitiveTypeKind.String);
                productDetailType.AddKeys(productDetailId);
                myModel.AddElement(productDetailType);

                var productDetailItemType = new EdmEntityType("MyNS", "ProductDetailItem");
                var productDetailItemId = productDetailItemType.AddStructuralProperty("ItemId", EdmPrimitiveTypeKind.Int32);
                productDetailItemType.AddStructuralProperty("Description", EdmPrimitiveTypeKind.String);
                productDetailItemType.AddKeys(productDetailItemId);
                myModel.AddElement(productDetailItemType);

                productType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "Details",
                    Target = productDetailType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = true,
                });
                productDetailType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "Items",
                    Target = productDetailItemType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = true,
                });

                var favouriteProducts = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "FavouriteProducts",
                    Target = productType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });
                var productBeingViewed = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
                {
                    Name = "ProductBeingViewed",
                    Target = productType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                });

                EdmEntityContainer container = new EdmEntityContainer("MyNS", "Example30");
                var products = container.AddEntitySet("Products", productType);
                var customers = container.AddEntitySet("Customers", customer);
                customers.AddNavigationTarget(favouriteProducts, products);
                customers.AddNavigationTarget(productBeingViewed, products);
                container.AddEntitySet("Orders", orderType);
                myModel.AddElement(container);
            }
            return myModel;
        }

        private IEdmEntitySet GetProducts()
        {
            return this.GetModel().FindDeclaredEntitySet("Products");
        }

        private IEdmEntityType GetProductType()
        {
            return this.GetModel().FindDeclaredType("MyNS.Product") as IEdmEntityType;
        }

        private IEdmEntitySet GetCustomers()
        {
            return this.GetModel().FindDeclaredEntitySet("Customers");
        }

        private IEdmEntityType GetCustomerType()
        {
            return this.GetModel().FindDeclaredType("MyNS.Customer") as IEdmEntityType;
        }

        private string TestPayload()
        {
            stream.Seek(0, SeekOrigin.Begin);
            return (new StreamReader(stream)).ReadToEnd();
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, IEdmModel userModel, bool fullMetadata = false, ODataUri uri = null)
        {
            var settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            settings.SetServiceDocumentUri(new Uri("http://host/service"));
            if (uri != null)
            {
                settings.ODataUri = uri;
            }

            IEnumerable<KeyValuePair<string, string>> parameters;
            if (fullMetadata)
            {
                parameters = new[] { new KeyValuePair<string, string>("odata.metadata", "full") };
            }
            else
            {
                parameters = new List<KeyValuePair<string, string>>();
            }

            var mediaType = new ODataMediaType("application", "json", parameters);

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = new NonDisposingStream(stream),
                MediaType = mediaType,
                Encoding = Encoding.UTF8,
                IsResponse = true,
                IsAsync = false,
                Model = userModel ?? EdmCoreModel.Instance
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }

        #endregion
    }
}
