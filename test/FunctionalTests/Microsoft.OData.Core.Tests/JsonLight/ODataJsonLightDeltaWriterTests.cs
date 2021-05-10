//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightDeltaWriterTests
    {
        private ODataJsonLightOutputContext V4ResponseOutputContext;
        private ODataJsonLightOutputContext V4RequestOutputContext;
        private ODataJsonLightOutputContext V401ResponseOutputContext;
        private ODataJsonLightOutputContext V401RequestOutputContext;
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

        private readonly ODataDeltaResourceSet requestFeed = new ODataDeltaResourceSet
        {
            SerializationInfo = new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Customers",
                NavigationSourceEntityTypeName = "MyNS.Customer",
                ExpectedTypeName = "MyNS.Customer"
            },
        };

        private readonly ODataDeltaResourceSet feedWithoutInfo = new ODataDeltaResourceSet
        {
            Count = 5,
            DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
        };

        private readonly ODataDeltaResourceSet requestFeedWithoutInfo = new ODataDeltaResourceSet
        {
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteExample30FromV4SpecWithModel(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(isResponse ? feedWithoutInfo : requestFeedWithoutInfo);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}" :
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteExample30FromV4SpecWithoutModel(bool isResponse)
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), null, null);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}" :
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteEmptyDeltaFeed(bool isResponse)
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), null, null);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[]}" :
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteContainedEntityInDeltaFeed(bool isResponse)
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

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(containedInContainedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/1/Items/$entity\",\"ItemId\":1,\"Description\":\"made by HCC\"}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteContainedEntityUsingKeyAsSegmentInDeltaFeed(bool isResponse)
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

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), false, null, ODataVersion.V4, isResponse);
            outputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment = true;
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(containedInContainedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products/1/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"@odata.context\":\"http://host/service/$metadata#Products/1/Details/1/Items/$entity\",\"ItemId\":1,\"Description\":\"made by HCC\"}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteContainedEntityInDeltaFeedWithSelectExpand(bool isResponse)
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

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), false, odataUri, ODataVersion.V4, isResponse);
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(product);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Products(Name,Details(Detail))/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"Id\":1,\"Name\":\"Car\"}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteEntityInDeltaFeedWithSelectExpand(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataDeltaResourceSet feed = new ODataDeltaResourceSet();

            ODataResource orderEntry = new ODataResource()
            {
                Id = new Uri("Orders(1)", UriKind.Relative),
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

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), false, odataUri, ODataVersion.V4, isResponse);
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

            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Products(ContactName,Orders(ShippingAddress))/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(1)\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDerivedEntity(bool isResponse)
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

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(derivedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDerivedDeletedResource(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetProducts(), this.GetProductType(), true, false, true);
            writer.WriteStart(feed);
            writer.WriteStart(derivedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDerivedEntityOfWrongTypeShouldFail(bool isResponse)
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
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeedWithInfo);
                writer.WriteStart(derivedEntity);
            };

            writeAction.Throws<ODataException>(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes("MyNS.PhysicalProduct", "MyNS.Customer"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDerivedDeletedResourceOfWrongTypeShouldFail(bool isResponse)
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
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(deltaFeedWithInfo);
                writer.WriteStart(derivedEntity);
            };

            writeAction.Throws<ODataException>(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes("MyNS.PhysicalProduct", "MyNS.Customer"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDerivedEntityWithSerilizationInfo(bool isResponse)
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

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(derivedEntity);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products/$entity\",\"@odata.type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true, /*keyAsSegment*/false)]
        [InlineData(/*isResponse*/false, /*keyAsSegment*/false)]
        [InlineData(/*isResponse*/true, /*keyAsSegment*/true)]
        [InlineData(/*isResponse*/false, /*keyAsSegment*/true)]
        [Theory]
        public void Write40DeletedEntryWithKeyValues(bool isResponse, bool keyAsSegment)
        {
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource()
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="Id", Value = 1},
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                },
                Reason = DeltaDeletedEntryReason.Changed
            };

            string payload = WriteDeletedResource(deletedCustomerWithContent, isResponse, keyAsSegment, ODataVersion.V4);

            string id = keyAsSegment ? "http://host/service/Customers/1" : "http://host/service/Customers(1)";
            Assert.Equal(payload, isResponse ?
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"" + id + "\",\"reason\":\"changed\"}]}" :
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"" + id + "\",\"reason\":\"changed\"}]}"
                );
        }

        [InlineData(/*isResponse*/true, /*keyAsSegment*/false)]
        [InlineData(/*isResponse*/false, /*keyAsSegment*/false)]
        [InlineData(/*isResponse*/true, /*keyAsSegment*/true)]
        [InlineData(/*isResponse*/false, /*keyAsSegment*/true)]
        [Theory]
        public void Write40DeletedEntryWithKeyValuesMatchesId(bool isResponse, bool keyAsSegment)
        {
            ODataDeletedResource deletedCustomerWithKeys = new ODataDeletedResource()
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="Id", Value = 1},
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                },
                Reason = DeltaDeletedEntryReason.Changed
            };

            string id = keyAsSegment ? "http://host/service/Customers/1" : "http://host/service/Customers(1)";
            ODataDeletedResource deletedCustomerWithId = new ODataDeletedResource(new Uri(id, UriKind.Absolute), DeltaDeletedEntryReason.Changed);

            string deletedCustomerWithKeysPayload = WriteDeletedResource(deletedCustomerWithKeys, isResponse, keyAsSegment, ODataVersion.V4);
            string deletedCustomerWithIdPayload = WriteDeletedResource(deletedCustomerWithId, isResponse, keyAsSegment, ODataVersion.V4);

            Assert.Equal(deletedCustomerWithIdPayload, deletedCustomerWithKeysPayload);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void Write40DeletedEntryWithKeyAndIdUsesId(bool isResponse)
        {
            ODataDeletedResource deletedCustomerWithId = new ODataDeletedResource(new Uri("Customers(2)", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="Id", Value = 1},
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                },
                Reason = DeltaDeletedEntryReason.Changed
            };

            string deletedCustomerWithKeysPayload = WriteDeletedResource(deletedCustomerWithId, isResponse, /*keyAsSegment*/false, ODataVersion.V4);

            Assert.Equal(deletedCustomerWithKeysPayload, isResponse ?
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers(2)\",\"reason\":\"changed\"}]}" :
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers(2)\",\"reason\":\"changed\"}]}"
                );
        }

        private string WriteDeletedResource(ODataDeletedResource deletedResource, bool isResponse, bool keyAsSegment, ODataVersion version)
        {
            MemoryStream stream = new MemoryStream();
            ODataJsonLightOutputContext outputContext = CreateJsonLightOutputContext(stream, this.GetModel(), false, null, version, isResponse);
            outputContext.ODataSimplifiedOptions.EnableWritingKeyAsSegment = keyAsSegment;

            ODataJsonLightWriter writer = new ODataJsonLightWriter(outputContext, this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(deletedResource);
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);
            string result = new StreamReader(stream).ReadToEnd();
            stream.Dispose();
            return result;
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
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(/*isResponse*/ true), this.GetCustomers(), this.GetCustomerType());
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

        private void WriteNestedDeltaFeedImplementation(bool isResponse)
        {
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
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

            Assert.Equal(this.TestPayload(),
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

            Assert.Equal(this.TestPayload(),
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteExpandedFeedWithSerializationInfoMinimalMetadata(bool isResponse)
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), null, null);
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

            Assert.Equal(this.TestPayload(),
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteMultipleExpandedFeeds(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
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

            Assert.Equal(this.TestPayload(),
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteContainmentExpandedFeeds(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
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

            Assert.Equal(this.TestPayload(),
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeltaWithModelMinimalMetadataV4_01(bool isResponse)
        {
            this.TestInit(this.GetModel());

            this.WriteNestedDeltaFeedImplementation(isResponse);

            Assert.Equal(this.TestPayload(),
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CannotWriteExpandedNavigationPropertyOutsideDeltaEntry(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(ordersNavigationLink);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "NestedResourceInfo"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CantWriteDeletedtemFromDifferentSetInNestedDelta(bool isResponse)
        {
            this.TestInit(this.GetModel());

            var writeAction = new Action(() =>
               { ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                   writer.WriteStart(deltaFeed);
                   writer.WriteStart(customerEntry);
                   writer.WriteStart(ordersNavigationLink);
                   writer.WriteStart(ordersFeed);
                   writer.WriteDeltaDeletedEntry(orderDeletedEntry);
               });

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CanWriteDeletedEntryInNestedDeltaV4_01(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
            writer.WriteStart(deltaFeed);
            writer.WriteStart(customerEntry);
            writer.WriteStart(ordersNavigationLink);
            writer.WriteStart(ordersDeltaFeed);
            writer.WriteDeltaDeletedEntry(orderDeletedEntry);
            writer.WriteEnd(); //ordersFeed
            writer.WriteEnd(); //ordersNavigationLink
            writer.WriteEnd(); //customerEntry
            writer.WriteEnd(); //deltaFeed

            Assert.Equal(this.TestPayload(), V4_01DeltaResponseWithNoKeys);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void V4_01DoesntIncludeAtODataId(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
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

            Assert.Equal(this.TestPayload(), V4_01DeltaResponse);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CanWriteStartEndDeletedResourceInNestedDeltaV4_01(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
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

            Assert.Equal(this.TestPayload(), V4_01DeltaResponse);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CannotWriteDeltaItemOfDifferentTypeWhileWritingExpandedNavigationProperty(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(customerEntry);
                writer.WriteStart(ordersNavigationLink);
                writer.WriteStart(ordersFeed);
                writer.WriteDeltaDeletedEntry(customerDeletedEntry);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CannotWriteExpandedFeedOutsideNavigationLink(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(customerEntry);
                writer.WriteStart(ordersFeed);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResource("Resource", "ResourceSet"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void CannotWriteExpandedFeedOutsideDeltaEntry(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(deltaFeed);
                writer.WriteStart(ordersFeed);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "ResourceSet"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteExpandedSingleton(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
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

            Assert.Equal(this.TestPayload(),
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

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteContentIn41DeletedEntry(bool isResponse)
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed",
                IsCollection = false
            });
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"Id\":1,\"Name\":\"Car\"}}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"Id\":1,\"Name\":\"Car\"}}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntryWithoutKeyOrIdShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(new ODataDeletedResource());
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntryWithNoReason(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{},\"Id\":1}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{},\"Id\":1}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteResourceInDeltaSetResponseWithoutKeyOrIdShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(new ODataResource());
            };

            if (isResponse)
            {
                writeAction.Throws<ODataException>(Strings.ODataWriterCore_DeltaResourceWithoutIdOrKeyProperties);
            }
            else
            {
                writeAction.DoesNotThrow();
            }
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteContentIn40DeletedEntryShouldFail(bool isResponse)
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
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(deletedCustomerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed",
                    IsCollection = false
                });
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFrom40DeletedResource("DeletedResource", "NestedResourceInfo"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeletedEntryInDeletedEntry(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed",
                IsCollection = false
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":1}}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"Id\":1,\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":1}}]}");
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeletedEntryInResource(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(customerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "ProductBeingViewed",
                IsCollection = false
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"Id\":1,\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":1}}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"Id\":1,\"ContactName\":\"Samantha Stones\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"deleted\"},\"Name\":\"Scissors\",\"Id\":1}}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeletedEntryFromWrongSetShouldFail(bool isResponse)
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

                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerWithContent);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed",
                    IsCollection = false
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

            writeAction.Throws<ODataException>(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedSingletonResourceFromWrongSetShouldFail(bool isResponse)
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

                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
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

            writeAction.Throws<ODataException>(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedSingletonDeltaResourceSetInDeletedEntry(bool isResponse)
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(deletedCustomerWithContent);
            writer.WriteStart(new ODataNestedResourceInfo()
            {
                Name = "FavouriteProducts"
            });
            writer.WriteStart(
                isResponse ? 
                new ODataDeltaResourceSet()
                {
                    Count = 2,
                    NextPageLink = new Uri("Customers/1/FavouriteProducts?$skipToken=123", UriKind.Relative)
                } : 
                new ODataDeltaResourceSet()
            );
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteStart(new ODataDeletedResource(new Uri("Products/1", UriKind.Relative), DeltaDeletedEntryReason.Deleted));
            writer.WriteEnd(); // deleted product
            writer.WriteEnd(); // delta resource set
            writer.WriteEnd(); // nested info
            writer.WriteEnd(); // customer
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(),
                isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"FavouriteProducts@count\":2,\"FavouriteProducts@nextLink\":\"Customers/1/FavouriteProducts?$skipToken=123\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Products/1\"}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Products/1\"}]}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedSingletonDeletedEntryFromWrongSetShouldFail(bool isResponse)
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

                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
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

            writeAction.Throws<ODataException>(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedResourceFromWrongSetShouldFail(bool isResponse)
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

                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
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

            writeAction.Throws<ODataException>(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Order", "MyNS.Product"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedResourceSetInDeletedEntry(bool isResponse)
        {
            this.TestInit(this.GetModel());
            ODataDeletedResource deletedCustomerWithContent = new ODataDeletedResource(new Uri("Customer/1", UriKind.Relative), DeltaDeletedEntryReason.Changed)
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty() {Name="ContactName", Value="Samantha Stones" }
                }
            };

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"Customer/1\",\"ContactName\":\"Samantha Stones\",\"FavouriteProducts\":[{\"Id\":1,\"Name\":\"Car\"}]}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntityInDeltaFeedWithSelectExpand(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(orderDeletedEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"@id\":\"orders/1\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntityShouldIgnoreSelectExpand(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(orderDeletedEntry);
            writer.WriteStart(shippingAddressInfo);
            writer.WriteStart(shippingAddress);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            Assert.Equal("{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"changed\"},\"@id\":\"orders/1\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}", this.TestPayload());
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteRelatedEntityIn41(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"Id\":1,\"Name\":\"Car\"}}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"Id\":1,\"Name\":\"Car\"}}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteRelatedDerivedEntityIn41(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteRelatedDerivedDeletedResourceIn41(bool isResponse)
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

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"changed\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":{\"@removed\":{\"reason\":\"changed\"},\"@type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeltaResourceSetIn40ShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "FavouriteProducts",
                    IsCollection = true
                });
                writer.WriteStart(new ODataDeltaResourceSet());
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeltaResourceSet"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeletedResourceIn40ShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(GetV4OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType());
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "ProductBeingViewed",
                    IsCollection = false
                });
                writer.WriteDeltaDeletedEntry(new ODataDeltaDeletedEntry("Products/1", DeltaDeletedEntryReason.Deleted));
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeletedResource"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNullRelatedEntityIn41(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":null}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"ProductBeingViewed\":null}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteRelatedEntitiesIn41(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders\":[{\"@id\":\"Orders(10643)\"},{\"@id\":\"Orders(10643)\"}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders\":[{\"@id\":\"Orders(10643)\"},{\"@id\":\"Orders(10643)\"}]}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteWithTypeDifferentThanWriter(bool isResponse)
        {
            this.TestInit(this.GetModel());
            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetProductType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(product);
            writer.WriteEnd(); // product
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Products/$entity\",\"Id\":1,\"Name\":\"Car\"}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Products/$entity\",\"Id\":1,\"Name\":\"Car\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteNestedDeltasIn41(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@delta\":[{\"@id\":\"Orders(10643)\"},{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@delta\":[{\"@id\":\"Orders(10643)\"},{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}]}",
                this.TestPayload());
        }

        [Fact]
        public void WriteResourceWithNestedDeltaRequestIn41()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(false), this.GetCustomers(), this.GetCustomerType(), false);
            writer.WriteStart(new ODataResource
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty{ Name = "Id", Value = 1}
                }
            });
            writer.WriteStart(new ODataNestedResourceInfo
            {
                Name = "FavouriteProducts",
                IsCollection = true,
            });
            writer.WriteStart(new ODataDeltaResourceSet());
            writer.WriteStart(new ODataResource
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty{ Name = "Id", Value = 1},
                    new ODataProperty{ Name = "Name", Value = "Car"}
                }
            });
            writer.WriteEnd(); // resource
            writer.WriteStart(new ODataDeletedResource
            {
                Reason = DeltaDeletedEntryReason.Deleted,
                Properties = new ODataProperty[]
                {
                    new ODataProperty{ Name = "Id", Value = 10}
                }
            });
            writer.WriteEnd(); // deleted resource
            writer.WriteEnd(); // delta resourceSet
            writer.WriteEnd(); // nestedInfo
            writer.WriteEnd(); // customer
            writer.Flush();

            Assert.Equal("{\"@context\":\"http://host/service/$metadata#Customers/$entity\",\"Id\":1,\"FavouriteProducts@delta\":[{\"Id\":1,\"Name\":\"Car\"},{\"@removed\":{\"reason\":\"deleted\"},\"Id\":10}]}",
                this.TestPayload());
        }

        [Fact]
        public void WriteResourceWithNestedDeltaResponseShouldFail()
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(true), this.GetCustomers(), this.GetCustomerType(), false);
                writer.WriteStart(new ODataResource
                {
                    Properties = new ODataProperty[]
                    {
                        new ODataProperty{ Name = "Id", Value = 1}
                    }
                });
                writer.WriteStart(new ODataNestedResourceInfo
                {
                    Name = "FavouriteProducts",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataDeltaResourceSet());
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter);
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteTopLevelEntityFromDifferentSet(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@id\":\"Orders(10643)\"}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@id\":\"Orders(10643)\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteTopLevelEntityFromDifferentSetWithoutInfo(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feedWithoutInfo : requestFeedWithoutInfo);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(order10643);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@id\":\"Orders(10643)\"}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$entity\",\"@id\":\"Orders(10643)\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteTopLevelDeletedEntityFromDifferentSet(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(orderDeleted);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteTopLevelDeletedEntityFromDifferentSetWithoutInfo(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feedWithoutInfo : requestFeedWithoutInfo);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd(); // customer
            writer.WriteStart(orderDeleted);
            writer.WriteEnd(); // order
            writer.WriteEnd(); // delta resource set
            writer.Flush();

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@context\":\"http://host/service/$metadata#Orders/$deletedEntity\",\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Orders(10643)\"}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteEntityFromDifferentSetToEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, false);
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(product);
            };

            writeAction.Throws<ODataException>(Strings.ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes("MyNS.Product", "MyNS.Customer"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteEntityFromDifferentSetToNestedEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
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

            writeAction.Throws<ODataException>(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Product", "MyNS.Order"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteEntityFromDifferentSetToNestedDeltaSetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
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

            writeAction.Throws<ODataException>(Strings.WriterValidationUtils_NestedResourceTypeNotCompatibleWithParentPropertyType("MyNS.Customer","MyNS.Order"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntityToNestedDeltaSet(bool isResponse)
        {
            this.TestInit(this.GetModel());

            ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
            writer.WriteStart(isResponse ? feed : requestFeed);
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

            Assert.Equal(this.TestPayload(), isResponse ?
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"@count\":5,\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@delta\":[{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Customers('ANTON')\"}]}]}" :
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\",\"Orders@delta\":[{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Customers('ANTON')\"}]}]}"
                );
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntityToEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(new ODataResourceSet());
                writer.WriteStart(customerDeleted);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeletedEntityToNestedEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
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

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeltaLinkToNestedDeltaSetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataDeltaResourceSet());
                writer.Write(linkToOrder10645);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "DeltaLink"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeltaLinkToEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10645);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaLink"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeltaLinkToNestedEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10645);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaLink"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeltaDeletedLinkToNestedDeltaSetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataDeltaResourceSet());
                writer.Write(linkToOrder10643);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("DeltaResourceSet", "DeltaDeletedLink"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeltaDeletedLinkToEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10643);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaDeletedLink"));
        }

        [InlineData(/*isResponse*/true)]
        [InlineData(/*isResponse*/false)]
        [Theory]
        public void WriteDeltaDeletedLinkToNestedEntitySetShouldFail(bool isResponse)
        {
            this.TestInit(this.GetModel());

            Action writeAction = () =>
            {
                ODataJsonLightWriter writer = new ODataJsonLightWriter(GetV401OutputContext(isResponse), this.GetCustomers(), this.GetCustomerType(), true, false, true);
                writer.WriteStart(isResponse ? feed : requestFeed);
                writer.WriteStart(customerUpdated);
                writer.WriteStart(new ODataNestedResourceInfo()
                {
                    Name = "Orders",
                    IsCollection = true,
                });
                writer.WriteStart(new ODataResourceSet());
                writer.Write(linkToOrder10643);
            };

            writeAction.Throws<ODataException>(Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeltaDeletedLink"));
        }

        #endregion 4.01 Tests

        #region Async Tests

        public static IEnumerable<object[]> GetWriteDeltaPayloadTestData()
        {
            var valueAsJson = "\"value\":[" +
                "{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"}," +
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\"," +
                "\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"}," +
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\"," +
                "\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"}," +
                "{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\"," +
                "\"@odata.id\":\"Orders(10643)\"," +
                "\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},";

            yield return new object[]
            {
                new ODataDeltaResourceSet
                {
                    Count = 5,
                    DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
                },
                false, // Writing request
                ODataVersion.V4,
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"@odata.count\":5," +
                "\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"," +
                valueAsJson +
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}"
            };

            yield return new object[]
            {
                new ODataDeltaResourceSet(),
                true, // Writing request
                ODataVersion.V4,
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                valueAsJson +
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}"
            };

            yield return new object[]
            {
                new ODataDeltaResourceSet
                {
                    Count = 5,
                    DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
                },
                false, // Writing request
                ODataVersion.V401,
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"@count\":5," +
                "\"@deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\"," +
                valueAsJson.Replace("@odata.", "@") +
                "{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Customers('ANTON')\"}"
            };

            yield return new object[]
            {
                new ODataDeltaResourceSet(),
                true, // Writing request
                ODataVersion.V401,
                "{\"@context\":\"http://host/service/$metadata#Customers/$delta\"," +
                valueAsJson.Replace("@odata.", "@") +
                "{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"Customers('ANTON')\"}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteDeltaPayloadTestData))]
        public async Task WriteDeltaPayloadAsync(ODataDeltaResourceSet deltaResourceSet, bool writingRequest, ODataVersion odataVersion, string expected)
        {
            var result = await SetupJsonLightDeltaWriterAndRunTestAsync(
                async (jsonLightDeltaWriter) =>
                {
                    await jsonLightDeltaWriter.WriteStartAsync(deltaResourceSet);
                    await jsonLightDeltaWriter.WriteStartAsync(customerUpdated);
                    await jsonLightDeltaWriter.WriteEndAsync();
                    await jsonLightDeltaWriter.WriteDeltaDeletedLinkAsync(linkToOrder10643);
                    await jsonLightDeltaWriter.WriteDeltaLinkAsync(linkToOrder10645);
                    await jsonLightDeltaWriter.WriteStartAsync(order10643);
                    await jsonLightDeltaWriter.WriteStartAsync(shippingAddressInfo);
                    await jsonLightDeltaWriter.WriteStartAsync(shippingAddress);
                    await jsonLightDeltaWriter.WriteEndAsync(); // shippingAddress
                    await jsonLightDeltaWriter.WriteEndAsync(); // shippingAddressInfo
                    await jsonLightDeltaWriter.WriteEndAsync(); // order10643
                    await jsonLightDeltaWriter.WriteDeltaDeletedEntryAsync(customerDeletedEntry);
                    await jsonLightDeltaWriter.WriteEndAsync(); // deltaResourceSet
                    await jsonLightDeltaWriter.FlushAsync();
                },
                this.GetCustomers(),
                this.GetCustomerType(),
                odataVersion,
                /*writingRequest*/ writingRequest);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteExpandedResourceSetAsync()
        {
            var result = await SetupJsonLightDeltaWriterAndRunTestAsync(
                async (jsonLightDeltaWriter) =>
                {
                    await jsonLightDeltaWriter.WriteStartAsync(deltaFeed);
                    await jsonLightDeltaWriter.WriteStartAsync(customerEntry);
                    await jsonLightDeltaWriter.WriteStartAsync(ordersNavigationLink);
                    await jsonLightDeltaWriter.WriteStartAsync(ordersFeed);
                    await jsonLightDeltaWriter.WriteStartAsync(orderEntry);
                    await jsonLightDeltaWriter.WriteStartAsync(shippingAddressInfo);
                    await jsonLightDeltaWriter.WriteStartAsync(shippingAddress);
                    await jsonLightDeltaWriter.WriteEndAsync(); // shippingAddress
                    await jsonLightDeltaWriter.WriteEndAsync(); // shippingAddressInfo
                    await jsonLightDeltaWriter.WriteEndAsync(); // orderEntry
                    await jsonLightDeltaWriter.WriteEndAsync(); // ordersFeed
                    await jsonLightDeltaWriter.WriteEndAsync(); // ordersNavigationLink
                    await jsonLightDeltaWriter.WriteStartAsync(favouriteProductsNavigationLink);
                    await jsonLightDeltaWriter.WriteStartAsync(favouriteProductsFeed);
                    await jsonLightDeltaWriter.WriteStartAsync(productEntry);
                    await jsonLightDeltaWriter.WriteEndAsync(); // productEntry
                    await jsonLightDeltaWriter.WriteEndAsync(); // favouriteProductsFeed
                    await jsonLightDeltaWriter.WriteEndAsync(); // favouriteProductsNavigationLink
                    await jsonLightDeltaWriter.WriteEndAsync(); // customerEntry
                    await jsonLightDeltaWriter.WriteEndAsync(); // deltaFeed
                    await jsonLightDeltaWriter.FlushAsync();
                }, this.GetCustomers(),
                this.GetCustomerType());

            Assert.Equal(
                "{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@odata.id\":\"http://host/service/Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"," +
                "\"Orders\":[{\"@odata.id\":\"http://host/service/Orders(10643)\",\"Id\":10643," +
                "\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}}]," +
                "\"FavouriteProducts\":[{\"@odata.id\":\"http://host/service/Product(1)\",\"Id\":1,\"Name\":\"Car\"}]}]}",
                result);
        }

        #endregion Async Tests

        #region Test Helper Methods

        private void TestInit(IEdmModel userModel = null, bool fullMetadata = false)
        {
            this.stream = new MemoryStream();
            this.V4RequestOutputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata, null, ODataVersion.V4, false);
            this.V4ResponseOutputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata, null, ODataVersion.V4, true);
            this.V401RequestOutputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata, null, ODataVersion.V401, false);
            this.V401ResponseOutputContext = CreateJsonLightOutputContext(this.stream, userModel, fullMetadata, null, ODataVersion.V401, true);
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
                var ordersNavigation = customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo
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
                var orders = container.AddEntitySet("Orders", orderType);
                customers.AddNavigationTarget(ordersNavigation, orders);
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

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, IEdmModel userModel, bool fullMetadata = false, ODataUri uri = null, ODataVersion version = ODataVersion.V4, bool isResponse = true, bool isAsync = false)
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
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = userModel ?? EdmCoreModel.Instance
            };

            return new ODataJsonLightOutputContext(messageInfo, settings);
        }

        private ODataJsonLightOutputContext GetV401OutputContext(bool isResponse)
        {
            return isResponse ? V401ResponseOutputContext : V401RequestOutputContext;
        }

        private ODataJsonLightOutputContext GetV4OutputContext(bool isResponse)
        {
            return isResponse ? V4ResponseOutputContext : V4RequestOutputContext;
        }

        /// <summary>
        /// Sets up an ODataJsonLightDeltaWriter,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonLightDeltaWriterAndRunTestAsync(
            Func<ODataJsonLightDeltaWriter, Task> func,
            IEdmNavigationSource navigationSource,
            IEdmEntityType resourceType,
            ODataVersion odataVersion = ODataVersion.V4,
            bool writingRequest = false)
        {
            this.stream = new MemoryStream();
            var jsonLightOutputContext = CreateJsonLightOutputContext(
                    this.stream,
                    this.GetModel(),
                    /*fullMetadata*/ false,
                    /*uri*/ null,
                    odataVersion,
                    !writingRequest,
                    /*isAsync*/ true);
            var jsonLightDeltaWriter = new ODataJsonLightDeltaWriter(
                jsonLightOutputContext,
                navigationSource,
                resourceType);

            await func(jsonLightDeltaWriter);

            this.stream.Seek(0, SeekOrigin.Begin);
            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        #endregion
    }
}
