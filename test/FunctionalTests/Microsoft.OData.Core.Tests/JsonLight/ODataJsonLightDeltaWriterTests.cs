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
        private ODataJsonLightOutputContext V401outputContext;
        private MemoryStream stream;
        private EdmModel myModel;

        #region Entities

        private readonly ODataDeltaResourceSet feed = new ODataDeltaResourceSet
        {
            Count = 5,
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Customers",
                NavigationSourceEntityTypeName = "MyNS.Customer",
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
                new ODataProperty { Name = "ContactName", Value = "Susan Halvenstern" },
            },
            TypeName = "MyNS.Customer",
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceEntityTypeName = "MyNS.Customer",
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
                NavigationSourceEntityTypeName = "MyNS.Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            },
        };

        private readonly ODataResource product = new ODataResource()
        {
            Properties = new[]
            {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("Car")},
            },
            TypeName="MyNS.Product",
            SerializationInfo = new ODataResourceSerializationInfo()
            {
                NavigationSourceName="Products",
                NavigationSourceEntityTypeName="MyNS.Product",
                ExpectedTypeName="MyNS.Product"
            }
        };

        private readonly ODataDeltaDeletedEntry customerDeletedEntry = new ODataDeltaDeletedEntry("Customers('ANTON')", DeltaDeletedEntryReason.Deleted);
        private readonly ODataDeletedResource customerDeleted = new ODataDeletedResource(new Uri("Customers('ANTON')", UriKind.Relative), DeltaDeletedEntryReason.Deleted);
        private readonly ODataDeltaDeletedEntry orderDeletedEntry = new ODataDeltaDeletedEntry("Orders(10643)", DeltaDeletedEntryReason.Deleted);
        private readonly ODataDeletedResource orderDeleted = new ODataDeletedResource(new Uri("Orders(10643)", UriKind.Relative), DeltaDeletedEntryReason.Deleted)
        {
            SerializationInfo = new ODataResourceSerializationInfo()
            {
                NavigationSourceName = "Orders",
                NavigationSourceEntityTypeName = "MyNS.Order",
                ExpectedTypeName = "MyNS.Order"
            }
        };
        private readonly ODataDeletedResource orderDeletedWithKeyProperties = new ODataDeletedResource()
        {
            Reason = DeltaDeletedEntryReason.Deleted,
            Properties = new ODataProperty[] { new ODataProperty() { Name = "Id", Value = 10643 } }
        };

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
            writer.WriteDeltaDeletedEntry(customerDeletedEntry);
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
            writer.WriteDeltaDeletedEntry(customerDeletedEntry);
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
                NavigationSourceName = "Products(1)/Details/1/Items",
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

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/1/Items/$entity\",\"ItemId\":1,\"Description\":\"made by HCC\"}]}");
        }

        [Fact]
        public void WriteContainedEntityUsingKeyAsSegmentInDeltaFeed()
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
                NavigationSourceName = "Products/1/Details",
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
                NavigationSourceName = "Products/1/Details/1/Items",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });
            outputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment = true;
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(containedInContainedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products/1/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"@odata.context\":\"http://host/service/$metadata#Products/1/Details/1/Items/$entity\",\"ItemId\":1,\"Description\":\"made by HCC\"}]}");
        }

        [Fact]
        public void WriteContainedEntityInDeltaFeedWithSelectExpand()
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
            writer.WriteStart(product);
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
                    NavigationSourceEntityTypeName = "MyNS.Order",
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
        public void WriteDerivedDeletedResource()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();
            ODataDeletedResource derivedEntity = new ODataDeletedResource()
            {
                Reason = DeltaDeletedEntryReason.Changed,
                TypeName = "MyNS.PhysicalProduct",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("car")},
                    new ODataProperty {Name = "Material", Value = new ODataPrimitiveValue("gold")},
                },
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetProducts(), this.GetProductType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(derivedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}");
        }

        [Fact]
        public void WriteDerivedEntityOfWrongTypeShouldFail()
        {
            this.TestInit(this.GetModel());

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

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeedWithInfo);
                writer.WriteStart(derivedEntity);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes("MyNS.PhysicalProduct", "MyNS.Customer"));
        }

        [Fact]
        public void WriteDerivedDeletedResourceOfWrongTypeShouldFail()
        {
            this.TestInit(this.GetModel());

            ODataDeletedResource derivedEntity = new ODataDeletedResource()
            {
                Reason = DeltaDeletedEntryReason.Changed,
                TypeName = "MyNS.PhysicalProduct",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("car")},
                    new ODataProperty {Name = "Material", Value = new ODataPrimitiveValue("gold")},
                },
            };
            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(deltaFeedWithInfo);
                writer.WriteStart(derivedEntity);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes("MyNS.PhysicalProduct", "MyNS.Customer"));
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

        #region Expanded Feeds

        #region Test Data

        private readonly ODataDeltaResourceSet deltaFeed = new ODataDeltaResourceSet();

        private readonly ODataDeltaResourceSet deltaFeedWithInfo = new ODataDeltaResourceSet
        {
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Customers",
                NavigationSourceEntityTypeName = "MyNS.Customer"
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

        private readonly ODataDeltaResourceSet ordersDeltaFeed = new ODataDeltaResourceSet();

        private readonly ODataResource orderEntry = new ODataResource
        {
            Id = new Uri("http://host/service/Orders(10643)"),
            Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 10643 },
                },
            TypeName = "MyNS.Order"
        };

        private readonly ODataDeletedResource orderDeletedEntryWithProperties = new ODataDeletedResource(new Uri("http://host/service/Orders(10642)", UriKind.Absolute), DeltaDeletedEntryReason.Deleted)
        {
            Properties = new[]
                {
                    new ODataProperty { Name = "Id", Value = 10642 }
                }
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

        private void WriteNestedDeltaFeedImplementation()
        {
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersDeltaFeed);
            writer.WriteStart(orderEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd(); // shippingAddress
            writer.WriteEnd(); // shippingAddressInfo
            writer.WriteEnd(); // orderEntry
            writer.WriteDeltaDeletedEntry(orderDeletedEntry);
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
                                    "\"@odata.editLink\":\"Orders(10643)\"," +
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
        public void WriteNestedDeltaWithModelMinimalMetadataV4_01()
        {
            this.TestInit(this.GetModel());

            this.WriteNestedDeltaFeedImplementation();

            this.TestPayload().Should().Be(
                "{" +
                    "\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders@delta\":" +
                            "[" +
                                "{" +
                                    "\"@id\":\"http://host/service/Orders(10643)\"," +
                                    "\"Id\":10643," +
                                    "\"ShippingAddress\":" +
                                    "{" +
                                        "\"Street\":\"23 Tsawassen Blvd.\"," +
                                        "\"City\":\"Tsawassen\"," +
                                        "\"Region\":\"BC\"," +
                                        "\"PostalCode\":\"T2F 8M4\"" +
                                    "}" +
                                "}," +
                                "{" +
                                    "\"@removed\":" +
                                    "{" +
                                        "\"reason\":\"deleted\"" +
                                    "}," +
                                    "\"@id\":\"Orders(10643)\"" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}"
                );
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

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "NestedResourceInfo"));
        }

        [Fact]
        public void CantWriteDeletedtemFromDifferentSetInNestedDelta()
        {
            this.TestInit(this.GetModel());

            var writeAction = new Action(() =>
               { ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType());
                   writer.WriteStart(deltaFeed);
                   writer.WriteStart(customerEntry);
                   writer.WriteStart(ordersNavigationLink);
                   writer.WriteStart(ordersFeed);
                   writer.WriteDeltaDeletedEntry(orderDeletedEntry);
               });

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
        }

        private static string V4_01DeltaResponse =
            "{" +
                    "\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders@delta\":" +
                            "[" +
                                "{" +
                                    "\"@removed\":" +
                                    "{" +
                                        "\"reason\":\"deleted\"" +
                                    "}," +
                                    "\"Id\":10643" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}"
        ;

        private static string V4_01DeltaResponseWithNoKeys =
            "{" +
                    "\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                    "\"value\":" +
                    "[" +
                        "{" +
                            "\"@id\":\"http://host/service/Customers('BOTTM')\"," +
                            "\"ContactName\":\"Susan Halvenstern\"," +
                            "\"Orders@delta\":" +
                            "[" +
                                "{" +
                                    "\"@removed\":" +
                                    "{" +
                                        "\"reason\":\"deleted\"" +
                                    "}," +
                                    "\"@id\":\"Orders(10643)\"" +
                                "}" +
                            "]" +
                        "}" +
                    "]" +
                "}"
        ;

        [Fact]
        public void CanWriteDeletedEntryInNestedDeltaV4_01()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersDeltaFeed);
            writer.WriteDeltaDeletedEntry(orderDeletedEntry);
            writer.WriteEnd(); //ordersFeed
            writer.WriteEnd(); //ordersNavigationLink
            writer.WriteEnd(); //customerEntry
            writer.WriteEnd(); //deltaFeed

            this.TestPayload().Should().Be(V4_01DeltaResponseWithNoKeys);
        }

        [Fact]
        public void V4_01DoesntIncludeAtODataId()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersDeltaFeed);
            writer.WriteStart(orderDeletedWithKeyProperties);
            writer.WriteEnd(); //deletedOrder
            writer.WriteEnd(); //ordersFeed
            writer.WriteEnd(); //ordersNavigationLink
            writer.WriteEnd(); //customerEntry
            writer.WriteEnd(); //deltaFeed

            this.TestPayload().Should().Be(V4_01DeltaResponse);
        }

        [Fact]
        public void CanWriteStartEndDeletedResourceInNestedDeltaV4_01()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersDeltaFeed);
            writer.WriteStart(orderDeletedWithKeyProperties);
            writer.WriteEnd(); //orderDeleted
            writer.WriteEnd(); //ordersFeed
            writer.WriteEnd(); //ordersNavigationLink
            writer.WriteEnd(); //customerEntry
            writer.WriteEnd(); //deltaFeed

            this.TestPayload().Should().Be(V4_01DeltaResponse);
        }

        [Fact]
        public void CannotWriteDeltaItemOfDifferentTypeWhileWritingExpandedNavigationProperty()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(customerEntry);
                writer.WriteStart(ordersNavigationLink);
                writer.WriteStart(ordersFeed);
                writer.WriteDeltaDeletedEntry(customerDeletedEntry);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
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

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResource("Resource", "ResourceSet"));
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

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "ResourceSet"));
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

        #region 4.01 Tests

        [Fact]
        public void WriteContentIn41DeletedEntry()
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"Id\":1,\"Name\":\"Car\"}}]}");
        }

        [Fact]
        public void WriteDeletedEntryWithoutKeyOrIdShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(new ODataDeletedResource());
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
        }

        [Fact]
        public void WriteDeletedEntryWithNoReason()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(new ODataDeletedResource()
            {
                Properties = new ODataProperty[]
                {
                        new ODataProperty() {Name = "Id", Value = 1 }
                }
            });
            writer.WriteEnd(); // deleted resource
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{},\"Id\":1}]}");
        }

        [Fact]
        public void WriteResourceInDeltaSetWithoutKeyOrIdShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(new ODataResource());
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
        }

        [Fact]
        public void WriteContentIn40DeletedEntryShouldFail()
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(deletedCustomerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed"
                });
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFrom40DeletedResource("DeletedResource", "NestedResourceInfo"));
        }

        [Fact]
        public void WriteNestedDeletedEntryInDeletedEntry()
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource()
            {
                Reason = DeltaDeletedEntryReason.Changed,
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="Id", Value=1 },
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart(
                new ODataDeletedResource()
                {
                    Reason = DeltaDeletedEntryReason.Deleted,
                    Properties = new ODataProperty[] 
                    {
                        new ODataProperty() { Name = "Name", Value = "Scissors" },
                        new ODataProperty() { Name = "Id", Value = 1 }
                    }
                });
            writer.WriteEnd(); // deleted product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":1}}]}");
        }

        [Fact]
        public void WriteNestedDeletedEntryInResource()
        {
            this.TestInit(this.GetModel());
            ODataResource customerWithContent = new ODataResource()
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="Id", Value=1 },
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart(
                new ODataDeletedResource()
                {
                    Reason = DeltaDeletedEntryReason.Deleted,
                    Properties = new ODataProperty[]
                    {
                        new ODataProperty() { Name = "Name", Value = "Scissors" },
                        new ODataProperty() { Name = "Id", Value = 1 }
                    }
                });
            writer.WriteEnd(); // deleted product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"Id\":1,\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":1}}]}");
        }

        [Fact]
        public void WriteNestedDeletedEntryFromWrongSetShouldFail()
        {
            this.TestInit(this.GetModel());
            Action writeAction = () =>
            {
                ODataResource customerWithContent = new ODataResource()
                {
                    Properties = new ODataProperty[]
                    {
                    new ODataProperty() {Name="Id", Value=1 },
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                    }
                };

                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed"
                });
                writer.WriteStart(
                    new ODataDeletedResource()
                    {
                        Reason = DeltaDeletedEntryReason.Deleted,
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty() { Name = "Id", Value = 1 }
                        },
                        SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            NavigationSourceEntityTypeName = "MyNS.Order",
                            NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                            NavigationSourceName = "Orders"
                        }
                    });
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }


        [Fact]
        public void WriteNestedSingletonResourceFromWrongSetShouldFail()
        {
            this.TestInit(this.GetModel());
            Action writeAction = () =>
            {
                ODataResource customerWithContent = new ODataResource()
                {
                    Properties = new ODataProperty[]
                    {
                    new ODataProperty() {Name="Id", Value=1 },
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                    }
                };

                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed"
                });
                writer.WriteStart(
                    new ODataResource()
                    {
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty() { Name = "Id", Value = 1 }
                        },
                        SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            NavigationSourceEntityTypeName = "MyNS.Order",
                            NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                            NavigationSourceName = "Orders"
                        }
                    });
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }

        [Fact]
        public void WriteNestedSingletonDeltaResourceSetInDeletedEntry()
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "FavouriteProducts"
            });
            writer.WriteStart(new ODataDeltaResourceSet()
            {
                Count = 2,
                NextPageLink = new Uri("Customers/1/FavouriteProducts?$skipToken=123", UriKind.Relative)
            });
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteStart(new ODataDeletedResource(new Uri("Products/1", UriKind.Relative), DeltaDeletedEntryReason.Deleted));
            writer.WriteEnd(); // deleted product
            writer.WriteEnd(); // delta resource set
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"FavouriteProducts@count\":2,\"FavouriteProducts@nextLink\":\"Customers/1/FavouriteProducts?$skipToken=123\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Products/1\"}]}]}");
        }

        [Fact]
        public void WriteNestedSingletonDeletedEntryFromWrongSetShouldFail()
        {
            this.TestInit(this.GetModel());
            Action writeAction = () =>
            {
                ODataResource customerWithContent = new ODataResource()
                {
                    Properties = new ODataProperty[]
                    {
                    new ODataProperty() {Name="Id", Value=1 },
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                    }
                };

                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "FavouriteProducts",
                    IsCollection = true
                });
                writer.WriteStart(new ODataDeltaResourceSet());
                writer.WriteStart(
                    new ODataDeletedResource()
                    {
                        Reason = DeltaDeletedEntryReason.Deleted,
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty() { Name = "Id", Value = 1 }
                        },
                        SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            NavigationSourceEntityTypeName = "MyNS.Order",
                            NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                            NavigationSourceName = "Orders"
                        }
                    });
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }


        [Fact]
        public void WriteNestedResourceFromWrongSetShouldFail()
        {
            this.TestInit(this.GetModel());
            Action writeAction = () =>
            {
                ODataResource customerWithContent = new ODataResource()
                {
                    Properties = new ODataProperty[]
                    {
                    new ODataProperty() {Name="Id", Value=1 },
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                    }
                };

                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "FavouriteProducts",
                    IsCollection = true
                });
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(
                    new ODataResource()
                    {
                        Properties = new ODataProperty[]
                        {
                            new ODataProperty() { Name = "Id", Value = 1 }
                        },
                        SerializationInfo = new ODataResourceSerializationInfo()
                        {
                            NavigationSourceEntityTypeName = "MyNS.Order",
                            NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                            NavigationSourceName = "Orders"
                        }
                    });
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }

        [Fact]
        public void WriteNestedResourceSetInDeletedEntry()
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "FavouriteProducts"
            });

            writer.WriteStart(new ODataResourceSet());
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // delta resource set
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"}]}]}");
        }

        [Fact]
        public void WriteDeletedEntityInDeltaFeedWithSelectExpand()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();

            ODataDeletedResource orderDeletedEntry = new ODataDeletedResource(new Uri("orders/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceEntityTypeName = "MyNS.Order",
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
            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(orderDeletedEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"@id\":\"orders/1\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}");
        }

        [Fact]
        public void WriteDeletedEntityShouldIgnoreSelectExpand()
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();

            ODataDeletedResource orderDeletedEntry = new ODataDeletedResource(new Uri("orders/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceEntityTypeName = "MyNS.Order",
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

            var result = new ODataQueryOptionParser(this.GetModel(), this.GetCustomerType(), this.GetCustomers(), new Dictionary<string, string> { { "$expand", "Orders($select=Id)" }, { "$select", "Orders" } }).ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://host/service"),
                SelectAndExpand = result
            };

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), false, odataUri);
            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(orderDeletedEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"@id\":\"orders/1\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}");
        }

        [Fact]
        public void WriteRelatedEntityIn41()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"Id\":1,\"Name\":\"Car\"}}]}");
        }

        [Fact]
        public void WriteRelatedDerivedEntityIn41()
        {
            this.TestInit(this.GetModel());

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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart(derivedEntity);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}}]}");
        }


        [Fact]
        public void WriteRelatedDerivedDeletedResourceIn41()
        {
            this.TestInit(this.GetModel());

            ODataDeletedResource derivedEntity = new ODataDeletedResource()
            {
                Reason = DeltaDeletedEntryReason.Changed,
                TypeName = "MyNS.PhysicalProduct",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)},
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("car")},
                    new ODataProperty {Name = "Material", Value = new ODataPrimitiveValue("gold")},
                },
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart(derivedEntity);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"changed\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}}]}");
        }

        [Fact]
        public void WriteNestedDeltaResourceSetIn40ShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "FavouriteProducts"
                });
                writer.WriteStart(new ODataDeltaResourceSet());
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeltaResourceSet"));
        }

        [Fact]
        public void WriteNestedDeletedResourceIn40ShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed"
                });
                writer.WriteDeltaDeletedEntry(new ODataDeltaDeletedEntry("Products/1", DeltaDeletedEntryReason.Deleted));
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeletedResource"));
        }

        [Fact]
        public void WriteNullRelatedEntityIn41()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed"
            });
            writer.WriteStart((ODataResource)null);
            writer.WriteEnd(); // null product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":null}]}");
        }

        [Fact]
        public void WriteRelatedEntitiesIn41()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "Orders",
                IsCollection = true,
            });
            writer.WriteStart(new ODataResourceSet());
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // resourceSet
            writer.WriteEnd(); // nestedInfo
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders\":[{\"@id\":\"Orders(10643)\"},{\"@id\":\"Orders(10643)\"}]}]}");
        }

        [Fact]
        public void WriteWithTypeDifferentThanWriter()
        {
            this.TestInit(this.GetModel());
            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetProductType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Products/$entity\",\"Id\":1,\"Name\":\"Car\"}]}");
        }

        [Fact]
        public void WriteNestedDeltasIn41()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "Orders",
                IsCollection = true,
            });
            writer.WriteStart(new ODataDeltaResourceSet());
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteStart(orderDeleted);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resourceSet
            writer.WriteEnd(); // nestedInfo
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@delta\":[{\"@id\":\"Orders(10643)\"},{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}]}");
        }

        [Fact]
        public void WriteTopLevelEntityFromDifferentSet()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@id\":\"Orders(10643)\"}]}");
        }

        [Fact]
        public void WriteTopLevelEntityFromDifferentSetWithoutInfo()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feedWithoutInfo);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@id\":\"Orders(10643)\"}]}");
        }

        [Fact]
        public void WriteTopLevelDeletedEntityFromDifferentSet()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(orderDeleted);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}");
        }
        
        [Fact]
        public void WriteTopLevelDeletedEntityFromDifferentSetWithoutInfo()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feedWithoutInfo);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(orderDeleted);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}");
        }

        [Fact]
        public void WriteEntityFromDifferentSetToEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, false);
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(product);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes("MyNS.Product", "MyNS.Customer"));
        }

        [Fact]
        public void WriteEntityFromDifferentSetToNestedEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(order10643);
                writer.WriteEnd(); // order
                writer.WriteStart(product);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Product", "MyNS.Order"));
        }

        [Fact]
        public void WriteEntityFromDifferentSetToNestedDeltaSetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataDeltaResourceSet());
                writer.WriteStart(order10643);
                writer.WriteEnd(); // order
                writer.WriteStart(customerUpdated);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Customer","MyNS.Order"));
        }

        [Fact]
        public void WriteDeletedEntityToNestedDeltaSet()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(customerUpdated);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "Orders",
                IsCollection = true,
            });
            writer.WriteStart(new ODataDeltaResourceSet());
            writer.WriteStart(customerDeleted);
            writer.WriteEnd(); // deletedCustomer
            writer.WriteEnd(); // resourceSet
            writer.WriteEnd(); // nestedInfo
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // feed

            this.TestPayload().Should().Be("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@delta\":[{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Customers('ANTON')\"}]}]}");
        }

        [Fact]
        public void WriteDeletedEntityToEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(customerDeleted);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
        }

        [Fact]
        public void WriteDeletedEntityToNestedEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(customerDeleted);
                writer.WriteEnd(); // order
                writer.WriteStart(customerUpdated);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
        }

        [Fact]
        public void WriteDeltaLinkToNestedDeltaSetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataDeltaResourceSet());
                writer.Write(linkToOrder10645);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "DeltaLink"));
        }

        [Fact]
        public void WriteDeltaLinkToEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10645);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaLink"));
        }

        [Fact]
        public void WriteDeltaLinkToNestedEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10645);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaLink"));
        }

        [Fact]
        public void WriteDeltaDeletedLinkToNestedDeltaSetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataDeltaResourceSet());
                writer.Write(linkToOrder10643);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "DeltaDeletedLink"));
        }

        [Fact]
        public void WriteDeltaDeletedLinkToEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10643);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaDeletedLink"));
        }

        [Fact]
        public void WriteDeltaDeletedLinkToNestedEntitySetShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(V401outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(feed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10643);
            };

            writeAction.ShouldThrow<ODataException>().WithMessage(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaDeletedLink"));
        }

        #endregion 4.01 Tests

        #region Test Helper Methods

        private void TestInit(IEdmModel userModel = null, bool fullMetadata = false)
        {
            this.stream = new MemoryStream();
            this.outputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata, null, ODataVersion.V4);
            this.V401outputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata, null, ODataVersion.V401);
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

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, IEdmModel userModel, bool fullMetadata = false, ODataUri uri = null, ODataVersion version = ODataVersion.V4)
        {
            var settings = new ODataMessageWriterSettings { Version = version, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
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
