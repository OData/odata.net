//---------------------------------------------------------------------
// <copyright file="ODataJsonLightDeltaWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.JsonLight;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightDeltaWriterTests
    {
        private ODataJsonLightOutputContext outputContext;
        private MemoryStream stream;
        private EdmModel myModel;

        #region Entities

        private readonly ODataDeltaFeed feed = new ODataDeltaFeed
        {
            Count = 5,
            SerializationInfo = new ODataDeltaFeedSerializationInfo
            {
                EntitySetName = "Customers",
                EntityTypeName = "MyNS.Customer",
                ExpectedTypeName = "MyNS.Customer"
            },
            DeltaLink = new Uri("Customers?$expand=Orders&$deltatoken=8015", UriKind.Relative)
        };

        private readonly ODataDeltaFeed feedWithoutInfo = new ODataDeltaFeed
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
            },
            SerializationInfo = new ODataFeedAndEntrySerializationInfo
            {
                NavigationSourceEntityTypeName = "Customer",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Customers"
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
            SerializationInfo = new ODataFeedAndEntrySerializationInfo
            {
                NavigationSourceEntityTypeName = "Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                NavigationSourceName = "Orders"
            },
        };

        private readonly ODataDeltaDeletedEntry customerDeleted = new ODataDeltaDeletedEntry("Customers('ANTON')", DeltaDeletedEntryReason.Deleted);

        private IEdmNavigationSource customers;

        private EdmEntityType customer;

        #endregion

        [Fact]
        public void WriteExample30FromV4SpecWithModel()
        {
            this.TestInit(this.GetModel());

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, customers, customer);
            writer.WriteStart(feedWithoutInfo);
            writer.WriteStart(customerUpdated);
            writer.WriteEnd();
            writer.WriteDeltaDeletedLink(linkToOrder10643);
            writer.WriteDeltaLink(linkToOrder10645);
            writer.WriteStart(order10643);
            writer.WriteEnd();
            writer.WriteDeltaDeletedEntry(customerDeleted);
            writer.WriteEnd();
            writer.Flush();

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}");
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
            writer.WriteEnd();
            writer.WriteDeltaDeletedEntry(customerDeleted);
            writer.WriteEnd();
            writer.Flush();

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[{\"@odata.id\":\"Customers('BOTTM')\",\"ContactName\":\"Susan Halvenstern\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedLink\",\"source\":\"Customers('ALFKI')\",\"relationship\":\"Orders\",\"target\":\"Orders('10643')\"},{\"@odata.context\":\"http://host/service/$metadata#Customers/$link\",\"source\":\"Customers('BOTTM')\",\"relationship\":\"Orders\",\"target\":\"Orders('10645')\"},{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"@odata.id\":\"Orders(10643)\",\"ShippingAddress\":{\"Street\":\"23 Tsawassen Blvd.\",\"City\":\"Tsawassen\",\"Region\":\"BC\",\"PostalCode\":\"T2F 8M4\"}},{\"@odata.context\":\"http://host/service/$metadata#Customers/$deletedEntity\",\"id\":\"Customers('ANTON')\",\"reason\":\"deleted\"}]}");
        }

        [Fact]
        public void WriteEmptyDeltaFeed()
        {
            this.TestInit();

            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, null, null);
            writer.WriteStart(feed);
            writer.WriteEnd();
            writer.Flush();

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"@odata.count\":5,\"@odata.deltaLink\":\"Customers?$expand=Orders&$deltatoken=8015\",\"value\":[]}");
        }

        [Fact]
        public void WriteContainedEntityInDeltaFeed()
        {
            this.TestInit(this.GetModel());

            ODataDeltaFeed feed = new ODataDeltaFeed();
            ODataEntry containedEntry = new ODataEntry()
            {
                TypeName = "MyNS.ProductDetail",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)}, 
                    new ODataProperty {Name = "Detail", Value = new ODataPrimitiveValue("made in china")},
                },
            };

            containedEntry.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo()
            {
                NavigationSourceEntityTypeName = "MyNS.ProductDetail",
                NavigationSourceName = "Products(1)/Details",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            });

            ODataEntry containedInContainedEntity = new ODataEntry()
            {
                TypeName = "MyNS.ProductDetailItem",
                Properties = new[]
                {
                    new ODataProperty {Name = "ItemId", Value = new ODataPrimitiveValue(1)}, 
                    new ODataProperty {Name = "Description", Value = new ODataPrimitiveValue("made by HCC")},
                },
            };

            containedInContainedEntity.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo()
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

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details(1)/Items/$entity\",\"ItemId\":1,\"Description\":\"made by HCC\"}]}"); 
        }

        [Fact]
        public void WriteContainedEntityInDeltaFeedWithSelectExpand()
        {
            this.TestInit(this.GetModel());

            ODataDeltaFeed feed = new ODataDeltaFeed();

            ODataEntry entry = new ODataEntry()
            {
                TypeName = "MyNS.Product",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)}, 
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("Car")},
                },
            };

            ODataEntry containedEntry = new ODataEntry()
            {
                TypeName = "MyNS.ProductDetail",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)}, 
                    new ODataProperty {Name = "Detail", Value = new ODataPrimitiveValue("made in china")},
                },
            };

            containedEntry.SetSerializationInfo(new ODataFeedAndEntrySerializationInfo()
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

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), odataUri);
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(containedEntry);
            writer.WriteEnd();
            writer.WriteStart(entry);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products(Name,Details,Details(Detail))/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products(1)/Details/$entity\",\"Id\":1,\"Detail\":\"made in china\"},{\"Id\":1,\"Name\":\"Car\"}]}");
        }

        [Fact]
        public void WriteEntityInDeltaFeedWithSelectExpand()
        {
            this.TestInit(this.GetModel());

            ODataDeltaFeed feed = new ODataDeltaFeed();

            ODataEntry orderEntry= new ODataEntry()
            {
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "ShippingAddress",
                        Value = new ODataComplexValue
                        {
                            Properties = new List<ODataProperty>
                            {
                                new ODataProperty { Name = "City", Value = "Shanghai" },
                            }
                        }
                    }
                },
                SerializationInfo = new ODataFeedAndEntrySerializationInfo
                {
                    NavigationSourceEntityTypeName = "Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                    NavigationSourceName = "Orders"
                },
            };


           var result = new ODataQueryOptionParser(this.GetModel(), this.GetCustomerType(), this.GetCustomers(), new Dictionary<string, string> { { "$expand", "Orders($select=ShippingAddress)" }, { "$select", "ContactName" } }).ParseSelectAndExpand();

            ODataUri odataUri = new ODataUri()
            {
                ServiceRoot = new Uri("http://host/service"),
                SelectAndExpand = result
            };

            var outputContext = CreateJsonLightOutputContext(this.stream, this.GetModel(), odataUri);
            ODataJsonLightDeltaWriter writer = new ODataJsonLightDeltaWriter(outputContext, this.GetProducts(), this.GetProductType());
            writer.WriteStart(feed);
            writer.WriteStart(orderEntry);
            writer.WriteEnd();
            writer.WriteEnd();
            writer.Flush();

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products(ContactName,Orders,Orders(ShippingAddress))/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Orders/$entity\",\"ShippingAddress\":{\"City\":\"Shanghai\"}}]}");
        }

        [Fact]
        public void WriteDerivedEntity()
        {
            this.TestInit(this.GetModel());

            ODataDeltaFeed feed = new ODataDeltaFeed();
            ODataEntry derivedEntity = new ODataEntry()
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

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Products/$delta\",\"value\":[{\"@odata.type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}");
        }

        [Fact]
        public void WriteDerivedEntityWithSerilizationInfo()
        {
            this.TestInit(this.GetModel());

            ODataDeltaFeed feed = new ODataDeltaFeed();
            ODataEntry derivedEntity = new ODataEntry()
            {
                TypeName = "MyNS.PhysicalProduct",
                Properties = new[]
                {
                    new ODataProperty {Name = "Id", Value = new ODataPrimitiveValue(1)}, 
                    new ODataProperty {Name = "Name", Value = new ODataPrimitiveValue("car")},
                    new ODataProperty {Name = "Material", Value = new ODataPrimitiveValue("gold")},
                },

                SerializationInfo = new ODataFeedAndEntrySerializationInfo()
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

            string payload = this.TestFinish();
            payload.Should().Be("{\"@odata.context\":\"http://host/service/$metadata#Customers/$delta\",\"value\":[{\"@odata.context\":\"http://host/service/$metadata#Products/$entity\",\"@odata.type\":\"#MyNS.PhysicalProduct\",\"Id\":1,\"Name\":\"car\",\"Material\":\"gold\"}]}");
        }

        #region Private Methods

        private void TestInit(IEdmModel userModel = null)
        {
            this.stream = new MemoryStream();
            this.outputContext = CreateJsonLightOutputContext(this.stream, userModel);
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

                EdmEntityType order = new EdmEntityType("MyNS", "Order");
                order.AddStructuralProperty("ShippingAddress", shippingAddressReference);
                myModel.AddElement(order);

                customer = new EdmEntityType("MyNS", "Customer");
                customer.AddStructuralProperty("ContactName", EdmPrimitiveTypeKind.String);
                var customerId = customer.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
                customer.AddKeys(customerId);
                customer.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
                {
                    Name = "Orders",
                    Target = order,
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

                productType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() {Name = "Details", Target = productDetailType, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true,});
                productDetailType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo() { Name = "Items", Target = productDetailItemType, TargetMultiplicity = EdmMultiplicity.Many, ContainsTarget = true, });

                EdmEntityContainer container = new EdmEntityContainer("MyNS", "Example30");
                customers = container.AddEntitySet("Customers", customer);
                container.AddEntitySet("Orders", order);
                myModel.AddElement(container);

                container.AddEntitySet("Products", productType);
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

        private string TestFinish()
        {
            stream.Seek(0, SeekOrigin.Begin);
            return (new StreamReader(stream)).ReadToEnd();
        }

        private static ODataJsonLightOutputContext CreateJsonLightOutputContext(MemoryStream stream, IEdmModel userModel, ODataUri uri = null)
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings { Version = ODataVersion.V4, AutoComputePayloadMetadataInJson = true, ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*") };
            settings.SetServiceDocumentUri(new Uri("http://host/service"));
            if (uri != null)
            {
                settings.ODataUri = uri;
            }

            ODataMediaType mediaType = new ODataMediaType("application", "json", new List<KeyValuePair<string, string>>());
            //mediaType.Parameters.Add(new KeyValuePair<string, string>("odata.metadata", "full"));
            return new ODataJsonLightOutputContext(
                ODataFormat.Json,
                new NonDisposingStream(stream),
                mediaType,
                Encoding.UTF8,
                settings,
                /*writingResponse*/ true,
                /*synchronous*/ true,
                userModel ?? EdmCoreModel.Instance,
                /*urlResolver*/ null);
        }

        #endregion
    }
}
