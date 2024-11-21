//---------------------------------------------------------------------
// <copyright file="ODataJsonWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Core.Tests.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Json;
using Microsoft.OData.UriParser;
using Microsoft.OData.Tests;
using Xunit;

namespace Microsoft.OData.Core.Tests.Json
{
    public class ODataJsonWriterTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private EdmModel model;
        private MemoryStream stream;
        private ODataMessageWriterSettings settings;

        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntityType productEntityType; // Test containment
        private EdmComplexType addressComplexType;
        private EdmEnumType customerTypeEnumType;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;

        public ODataJsonWriterTests()
        {
            InitializeEdmModel();

            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                Version = ODataVersion.V4,
                ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*")
            };
            this.settings.SetServiceDocumentUri(new Uri(ServiceUri));
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.customerTypeEnumType = new EdmEnumType("NS", "CustomerType");
            this.addressComplexType = new EdmComplexType("NS", "Address", baseType: null, isAbstract: true, isOpen: true);
            this.orderEntityType = new EdmEntityType("NS", "Order");
            this.customerEntityType = new EdmEntityType("NS", "Customer");
            this.productEntityType = new EdmEntityType("NS", "Product");

            this.customerTypeEnumType.AddMember(new EdmEnumMember(this.customerTypeEnumType, "Retail", new EdmEnumMemberValue(0)));
            this.customerTypeEnumType.AddMember(new EdmEnumMember(this.customerTypeEnumType, "Wholesale", new EdmEnumMemberValue(1)));
            this.model.AddElement(this.customerTypeEnumType);

            this.addressComplexType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String);
            this.addressComplexType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String);
            this.model.AddElement(this.addressComplexType);

            var customerIdProperty = this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.customerEntityType.AddKeys(customerIdProperty);
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.customerEntityType.AddStructuralProperty("Type", new EdmEnumTypeReference(this.customerTypeEnumType, false));
            this.model.AddElement(this.customerEntityType);

            var orderIdProperty = this.orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddKeys(orderIdProperty);
            this.orderEntityType.AddStructuralProperty("CustomerId", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddStructuralProperty("ShippingAddress", new EdmComplexTypeReference(this.addressComplexType, true));
            this.orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            this.model.AddElement(this.orderEntityType);

            var productIdProperty = this.productEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.productEntityType.AddKeys(productIdProperty);
            this.productEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.model.AddElement(this.productEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.customerEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);
            this.orderEntitySet = entityContainer.AddEntitySet("Orders", this.orderEntityType);

            this.customerEntitySet.AddNavigationTarget(
                this.customerEntityType.AddUnidirectionalNavigation(
                    new EdmNavigationPropertyInfo
                    {
                        Name = "Orders",
                        Target = this.orderEntityType,
                        TargetMultiplicity = EdmMultiplicity.Many
                    }),
                this.orderEntitySet);

            this.orderEntitySet.AddNavigationTarget(
                this.orderEntityType.AddUnidirectionalNavigation(
                    new EdmNavigationPropertyInfo
                    {
                        Name = "Customer",
                        Target = this.customerEntityType,
                        TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                    }),
                this.customerEntitySet);

            this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Products",
                    Target = this.productEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = true
                });
        }

        public static IEnumerable<object[]> GetWriteTopLevelResourceSetTestData()
        {
            ODataResourceSet customerResourceSet;

            // Base case
            customerResourceSet = CreateCustomerResourceSet();
            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\",\"value\":[]}"
            };

            // Set Count
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.Count = 5;
            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"@odata.count\":5," +
                "\"value\":[]}"
            };

            // Set NextPageLink
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");
            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"@odata.nextLink\":\"http://tempuri.org/Customers/nextLink\"," +
                "\"value\":[]}"
            };

            // Set DeltaLink
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.DeltaLink = new Uri($"{ServiceUri}/Customers/deltaLink");
            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"value\":[]}"
            };

            // Set Action
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Customers"), "RateCustomers"));
            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"#Action\":{\"title\":\"RateCustomers\",\"target\":\"http://tempuri.org/Customers/RateCustomers\"}," +
                "\"value\":[]}"
            };

            // Set Function
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Customers"), "GetTopCustomer"));
            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"#Function\":{\"title\":\"GetTopCustomer\",\"target\":\"http://tempuri.org/Customers/GetTopCustomer\"}," +
                "\"value\":[]}"
            };

            // Set InstanceAnnotation
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.InstanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.ResourceSet", new ODataPrimitiveValue(true))
            };

            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"@Is.ResourceSet\":true," +
                "\"value\":[]}"
            };

            // Set Multiple
            customerResourceSet = CreateCustomerResourceSet();
            customerResourceSet.Count = 5;
            // Not allowed to set both NextPageLink and DeltaLink
            customerResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");
            customerResourceSet.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Customers"), "RateCustomers"));
            customerResourceSet.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Customers"), "GetTopCustomer"));
            customerResourceSet.InstanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.ResourceSet", new ODataPrimitiveValue(true))
            };

            yield return new object[]
            {
                customerResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"#Action\":{\"title\":\"RateCustomers\",\"target\":\"http://tempuri.org/Customers/RateCustomers\"}," +
                "\"#Function\":{\"title\":\"GetTopCustomer\",\"target\":\"http://tempuri.org/Customers/GetTopCustomer\"}," +
                "\"@odata.count\":5," +
                "\"@odata.nextLink\":\"http://tempuri.org/Customers/nextLink\"," +
                "\"@Is.ResourceSet\":true,\"value\":[]}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteTopLevelResourceSetTestData))]
        public async Task WriteTopLevelResourceSetAsync(ODataResourceSet customerResourceSet, string expected)
        {
            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResourceSet);
                    await jsonWriter.WriteEndAsync(); // Flushes the stream
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetWriteTopLevelDeltaResourceSetTestData()
        {
            ODataDeltaResourceSet customerDeltaResourceSet;

            // Base case
            customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            yield return new object[]
            {
                customerDeltaResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"value\":[]}"
            };

            // Set Count
            customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.Count = 5;
            yield return new object[]
            {
                customerDeltaResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.count\":5," +
                "\"value\":[]}"
            };

            // Set DeltaLink
            customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.DeltaLink = new Uri($"{ServiceUri}/Customers/deltaLink");
            yield return new object[]
            {
                customerDeltaResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"value\":[]}"
            };

            // Set NextPageLink
            customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");
            yield return new object[]
            {
                customerDeltaResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.nextLink\":\"http://tempuri.org/Customers/nextLink\"," +
                "\"value\":[]}"
            };

            // Set InstanceAnnotation
            customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.SetInstanceAnnotations(new Collection<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.DeltaResourceSet", new ODataPrimitiveValue(true))
            });

            yield return new object[]
            {
                customerDeltaResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@Is.DeltaResourceSet\":true," +
                "\"value\":[]}"
            };

            // Set Multiple
            customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.Count = 5;
            // Not allowed to set both DeltaLink and NextPageLink
            customerDeltaResourceSet.DeltaLink = new Uri($"{ServiceUri}/Customers/deltaLink");
            customerDeltaResourceSet.SetInstanceAnnotations(new Collection<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.DeltaResourceSet", new ODataPrimitiveValue(true))
            });

            yield return new object[]
            {
                customerDeltaResourceSet,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.count\":5," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"@Is.DeltaResourceSet\":true," +
                "\"value\":[]}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteTopLevelDeltaResourceSetTestData))]
        public async Task WriteTopLevelDeltaResourceSetAsync(ODataDeltaResourceSet customerDeltaResourceSet, string expected)
        {
            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerDeltaResourceSet);
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: true);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteV401NestedDeltaResourceSetAsync()
        {
            // Nested DeltaResourceSet only allowed in 401
            this.settings.Version = ODataVersion.V401;

            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();
            var orderDeltaResourceSet = CreateOrderDeltaResourceSet();
            orderDeltaResourceSet.Count = 1;
            orderDeltaResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");
            var orderNestedResource = CreateOrderResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonWriter.WriteStartAsync(orderNestedResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: false,
                writingDelta: true);

            Assert.Equal(
                "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders@navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"," +
                "\"Orders@count\":1," +
                "\"Orders@nextLink\":\"http://tempuri.org/Customers/nextLink\"," +
                "\"Orders@delta\":[{\"Id\":1,\"CustomerId\":1,\"Amount\":100}]}",
                result);
        }

        public static IEnumerable<object[]> GetWriteTopLevelResourceTestData()
        {
            ODataResource customerResource;

            // Resource with no properties
            yield return new object[]
            {
                new ODataResource
                {
                    TypeName = "NS.Customer"
                },
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"}"
            };

            // Resource with properties
            customerResource = CreateCustomerResource();
            yield return new object[]
            {
                customerResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}"
            };

            // Resource start metadata properties - ETag
            customerResource = CreateCustomerResource();
            customerResource.ETag = "resource-etag";
            yield return new object[]
            {
                customerResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.etag\":\"resource-etag\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}"
            };

            // Resource metadata properties - editLink, readLink, MediaResource
            customerResource = CreateCustomerResource();
            customerResource.EditLink = new Uri($"{ServiceUri}/Customers(1)/editLink");
            customerResource.ReadLink = new Uri($"{ServiceUri}/Customers(1)/readLink");
            customerResource.MediaResource = new ODataStreamReferenceValue
            {
                EditLink = new Uri($"{ServiceUri}/Customers(1)/blob/editLink"),
                ReadLink = new Uri($"{ServiceUri}/Customers(1)/blob/readLink"),
                ContentType = "image/png",
                ETag = "media-etag"
            };

            yield return new object[]
            {
                customerResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.editLink\":\"http://tempuri.org/Customers(1)/editLink\"," +
                "\"@odata.readLink\":\"http://tempuri.org/Customers(1)/readLink\"," +
                "\"@odata.mediaEditLink\":\"http://tempuri.org/Customers(1)/blob/editLink\"," +
                "\"@odata.mediaReadLink\":\"http://tempuri.org/Customers(1)/blob/readLink\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"media-etag\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}"
            };

            // Resource end metadata properties - actions & functions
            customerResource = CreateCustomerResource();
            customerResource.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Customers(1)"), "RateCustomer"));
            customerResource.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Customers(1)"), "MostRecentOrder"));
            yield return new object[]
            {
                customerResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"#Action\":{\"title\":\"RateCustomer\",\"target\":\"http://tempuri.org/Customers(1)/RateCustomer\"}," +
                "\"#Function\":{\"title\":\"MostRecentOrder\",\"target\":\"http://tempuri.org/Customers(1)/MostRecentOrder\"}}"
            };

            // Instance annotations 
            customerResource = CreateCustomerResource();
            customerResource.InstanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.Resource", new ODataPrimitiveValue(true))
            };

            yield return new object[]
            {
                customerResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@Is.Resource\":true," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteTopLevelResourceTestData))]
        public async Task WriteTopLevelResourceAsync(ODataResource resource, string expected)
        {
            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(resource);
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType);

            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetWriteNestedResourceTestData()
        {
            // Nested EntityType
            yield return new object[]
            {
                CreateCustomerNestedResourceInfo(),
                CreateCustomerResource(),
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer\":{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}}",
            };

            // Nested ComplexType
            yield return new object[]
            {
                CreateAddressNestedResourceInfo(),
                CreateAddressResource(),
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"ShippingAddress\":{\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\"}}"
            };

            // Nested Resource null
            yield return new object[]
            {
                CreateCustomerNestedResourceInfo(),
                null,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer\":null}"
            };

            ODataNestedResourceInfo nestedResourceInfo;

            // Nested Resource null + TypeAnnotation specified on NestedResourceInfo
            nestedResourceInfo = CreateCustomerNestedResourceInfo();
            nestedResourceInfo.TypeAnnotation = new ODataTypeAnnotation("NS.Customer");
            yield return new object[]
            {
                nestedResourceInfo,
                null,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer@odata.type\":\"#NS.Customer\",\"Customer\":null}"
            };

            // Nested Resource null + Instance annotations specified on NestedResourceInfo
            nestedResourceInfo = CreateCustomerNestedResourceInfo();
            nestedResourceInfo.SetInstanceAnnotations(new Collection<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.NestedResource", new ODataPrimitiveValue(true))
            });
            yield return new object[]
            {
                nestedResourceInfo,
                null,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer@Is.NestedResource\":true,\"Customer\":null}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteNestedResourceTestData))]
        public async Task WriteNestedResourceAsync(ODataNestedResourceInfo nestedResourceInfo, ODataResource nestedResource, string expected)
        {
            var orderResource = CreateOrderResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(nestedResourceInfo);
                    await jsonWriter.WriteStartAsync(nestedResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(null)] // Auto-detection should apply
        public async Task WriteNestedResourceSetAsync(bool? isCollection)
        {
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();
            orderCollectionNestedResourceInfo.IsCollection = isCollection;
            var orderResourceSet = CreateOrderResourceSet();
            var orderNestedResource = CreateOrderResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(orderResourceSet);
                    await jsonWriter.WriteStartAsync(orderNestedResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"," +
                "\"Orders\":[{\"Id\":1,\"CustomerId\":1,\"Amount\":100}]}",
                result);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted, ",\"reason\":\"deleted\"")]
        [InlineData(DeltaDeletedEntryReason.Changed, ",\"reason\":\"changed\"")]
        [InlineData(null, "")]
        public async Task WriteDeletedResourceAsync(DeltaDeletedEntryReason? reason, string expected)
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerDeltaResourceSet);
                    await jsonWriter.WriteStartAsync(customerDeletedResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"value\":[{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$deletedEntity\"," +
                "\"id\":\"http://tempuri.org/Customers(1)\"" + expected + "}]}",
                result);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted, "{\"reason\":\"deleted\"}")]
        [InlineData(DeltaDeletedEntryReason.Changed, "{\"reason\":\"changed\"}")]
        [InlineData(null, "{}")]
        public async Task WriteV401DeletedResourceAsync(DeltaDeletedEntryReason? reason, string expected)
        {
            this.settings.Version = ODataVersion.V401;

            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerDeltaResourceSet);
                    await jsonWriter.WriteStartAsync(customerDeletedResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: true);

            Assert.Equal(
                "{\"@context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"value\":[" +
                "{\"@removed\":" + expected + "," +
                "\"@id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]}",
                result);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted, "deleted")]
        [InlineData(DeltaDeletedEntryReason.Changed, "changed")]
        public async Task WriteV401NestedDeletedResourceAsync(DeltaDeletedEntryReason reason, string expected)
        {
            this.settings.Version = ODataVersion.V401;

            var orderResource = CreateOrderResource();
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(customerDeletedResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: false,
                writingDelta: true);

            Assert.Equal(
                "{\"@context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer@navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer\":{\"@removed\":{\"reason\":\"" + expected + "\"}," +
                "\"@id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}}",
                result);
        }

        [Fact]
        public async Task WriteEntityReferenceLinkAsync()
        {
            var customerResource = CreateOrderResource();
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();
            var customerEntityReferenceLink = CreateCustomerEntityReferenceLink(1);

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonWriter.WriteEntityReferenceLinkAsync(customerEntityReferenceLink);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer\":{\"@odata.id\":\"Customers(1)\"," +
                "\"@Is.EntityReferenceLink\":true}}",
                result);
        }

        /// <summary>
        /// Gets the name of the caller method of this method
        /// </summary>
        /// <param name="caller">The string that the method name of the caller will be written into</param>
        /// <returns>The name of the caller method of this method</returns>
        public static string GetCurrentMethodName([System.Runtime.CompilerServices.CallerMemberName] string caller = null)
        {
            return caller;
        }

        /// <summary>
        /// A <see cref="IEdmNavigationSource"/> that pretends to be the "products" contained navigation collection for the purposes of computing a context URL
        /// </summary>
        private sealed class MockNavigationSource : IEdmNavigationSource, IEdmContainedEntitySet, IEdmUnknownEntitySet
        {
            public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings => throw new NotImplementedException();

            public IEdmPathExpression Path => throw new NotImplementedException();

            public IEdmType Type => new EdmEntityType("ns", "products");

            public IEdmEntityType EntityType => this.Type.AsElementType() as IEdmEntityType;

            public string Name => "products";

            public IEdmNavigationSource ParentNavigationSource => throw new NotImplementedException();

            public IEdmNavigationProperty NavigationProperty => throw new NotImplementedException();

            public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
            {
                throw new NotImplementedException();
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
            {
                throw new NotImplementedException();
            }

            public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
            {
                throw new NotImplementedException();
            }
        }

#if !NETCOREAPP1_1
        /// <summary>
        /// Generates a context URL from a <see cref="ODataUriSlim"/> that ends with cast and key segments
        /// </summary>
        /// <returns><see cref="void"/></returns>
        [Fact]
        public static void GenerateContextUrlFromSlimUriWithDerivedTypeCastAndKeySegment()
        {
            var domain = new Uri("http://tempuri.org");
            var requestUrl = new Uri(domain, "/orders('1')/products/ns.derivedProduct('2')");

            // load the CSDL from the embedded resources
            var assembly = Assembly.GetExecutingAssembly();
            var currentMethod = GetCurrentMethodName();
            var csdlResourceName = assembly.GetManifestResourceNames().Where(name => name.EndsWith($"{currentMethod}.xml")).Single();

            // parse the CSDL
            IEdmModel model;
            using (var csdlResourceStream = assembly.GetManifestResourceStream(csdlResourceName))
            {
                using (var xmlReader = XmlReader.Create(csdlResourceStream))
                {
                    if (!CsdlReader.TryParse(xmlReader, out model, out var errors))
                    {
                        Assert.True(false, string.Join(Environment.NewLine, errors));
                    }
                }
            }

            var uriParser = new ODataUriParser(model, domain, requestUrl);
            var slimUri = new ODataUriSlim(uriParser.ParseUri());
            var contextUrlInfo = ODataContextUrlInfo.Create(new MockNavigationSource(), "ns.product", true, slimUri, ODataVersion.V4);
            Assert.Equal(@"orders('1')/products", contextUrlInfo.NavigationPath);
        }

        /// <summary>
        /// Writes a resource as the response to a request where the URL ends with a combined cast and key segment
        /// </summary>
        /// <returns><see cref="void"/></returns>
        [Fact]
        public static async Task WriteContextWithDerivedTypeCastAndKeySegmentAsync()
        {
            var domain = new Uri("http://tempuri.org");
            var requestUrl = new Uri(domain, "/orders('1')/products/ns.derivedProduct('2')");
            var serviceSideResponseResource = new ODataResource
            {
                TypeName = "ns.product",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "id",
                        Value = "1",
                        SerializationInfo = new ODataPropertySerializationInfo
                        {
                            PropertyKind = ODataPropertyKind.Key
                        },
                    },
                    new ODataProperty
                    {
                        Name = "name",
                        Value = "somename",
                    },
                },
            };
            var expectedResponsePayload =
                "{" +
                    "\"@odata.context\":\"http://tempuri.org/$metadata#orders('1')/products/$entity\"," +
                    "\"id\":\"1\"," +
                    "\"name\":\"somename\"" +
                "}";

            // load the CSDL from the embedded resources
            var assembly = Assembly.GetExecutingAssembly();
            var currentMethod = GetCurrentMethodName();
            var csdlResourceName = assembly.GetManifestResourceNames().Where(name => name.EndsWith($"{currentMethod}.xml")).Single();

            // parse the CSDL
            IEdmModel model;
            using (var csdlResourceStream = assembly.GetManifestResourceStream(csdlResourceName))
            {
                using (var xmlReader = XmlReader.Create(csdlResourceStream))
                {
                    if (!CsdlReader.TryParse(xmlReader, out model, out var errors))
                    {
                        Assert.True(false, string.Join(Environment.NewLine, errors));
                    }
                }
            }

            using (var memoryStream = new MemoryStream())
            {
                // initialize the json response writer
                var uriParser = new ODataUriParser(model, domain, requestUrl);
                var odataMessageWriterSettings = new ODataMessageWriterSettings
                {
                    EnableMessageStreamDisposal = false,
                    Version = ODataVersion.V4,
                    ShouldIncludeAnnotationInternal = ODataUtils.CreateAnnotationFilter("*"),
                    ODataUri = uriParser.ParseUri(),
                };
                var messageInfo = new ODataMessageInfo
                {
                    MessageStream = memoryStream,
                    MediaType = new ODataMediaType("application", "json"),
                    Encoding = Encoding.Default,
                    IsResponse = true,
                    IsAsync = true,
                    Model = model,
                };
                var jsonOutputContext = new ODataJsonOutputContext(messageInfo, odataMessageWriterSettings);
                var jsonWriter = new ODataJsonWriter(
                    jsonOutputContext,
                    null,
                    null,
                    false);

                // write the response
                await jsonWriter.WriteStartAsync(serviceSideResponseResource);
                await jsonWriter.WriteEndAsync();

                // confirm that the written response was the expected response
                memoryStream.Position = 0;
                using (var streamReader = new StreamReader(memoryStream))
                {
                    var actualResponsePayload = await streamReader.ReadToEndAsync();
                    Assert.Equal(expectedResponsePayload, actualResponsePayload);
                }
            }
        }
#endif

        [Fact]
        public async Task WriteEntityReferenceLinkForCollectionNavigationPropertyAsync()
        {
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();
            var orderResourceSet = CreateOrderResourceSet();
            var orderEntityReferenceLink1 = new ODataEntityReferenceLink
            {
                Url = new Uri($"{ServiceUri}/Orders(1)")
            };
            var orderEntityReferenceLink2 = new ODataEntityReferenceLink
            {
                Url = new Uri($"{ServiceUri}/Orders(2)")
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(orderResourceSet);
                    await jsonWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                    await jsonWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"," +
                "\"Orders\":[{\"@odata.id\":\"Orders(1)\"},{\"@odata.id\":\"Orders(2)\"}]}",
                result);
        }

        [Fact]
        public async Task WriteDeltaLinkAsync()
        {
            var orderDeltaResourceSet = CreateOrderDeltaResourceSet();
            var deltaLink = new ODataDeltaLink(
                new Uri($"{ServiceUri}/Orders(1)"),
                new Uri($"{ServiceUri}/Customers(1)"),
                "Customer");

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonWriter.WriteDeltaLinkAsync(deltaLink);
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: true,
                writingDelta: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"value\":[{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$link\"," +
                "\"source\":\"http://tempuri.org/Orders(1)\"," +
                "\"relationship\":\"Customer\"," +
                "\"target\":\"http://tempuri.org/Customers(1)\"}]}",
                result);
        }

        [Fact]
        public async Task WriteDeltaDeletedLinkAsync()
        {
            var orderDeltaResourceSet = CreateOrderDeltaResourceSet();
            var deltaDeletedLink = new ODataDeltaDeletedLink(
                new Uri($"{ServiceUri}/Orders(1)"),
                new Uri($"{ServiceUri}/Customers(1)"),
                "Customer");

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonWriter.WriteDeltaDeletedLinkAsync(deltaDeletedLink);
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: true,
                writingDelta: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"value\":[{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$deletedLink\"," +
                "\"source\":\"http://tempuri.org/Orders(1)\"," +
                "\"relationship\":\"Customer\"," +
                "\"target\":\"http://tempuri.org/Customers(1)\"}]}",
                result);
        }

        [Fact]
        public async Task WritePropertyWithValueAsync()
        {
            var addressResource = CreateAddressResource();
            var stateProperty = new ODataProperty { Name = "State", Value = "Washington" };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(stateProperty);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\",\"State\":\"Washington\"}",
                result);
        }

        public static IEnumerable<object[]> GetWritePrimitiveTestData()
        {
            // Base case
            yield return new object[]
            {
                new ODataPrimitiveValue("Washington"),
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\",\"State\":\"Washington\"}"
            };

            // ODataPrimitiveValue null
            yield return new object[]
            {
                null,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\",\"State\":null}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWritePrimitiveTestData))]
        public async Task WritePrimitiveAsync(ODataPrimitiveValue primitiveValue, string expected)
        {
            var addressResource = CreateAddressResource();
            var stateProperty = new ODataPropertyInfo { Name = "State" };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(stateProperty);
                    await jsonWriter.WritePrimitiveAsync(primitiveValue);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteResourceSetAsync_ForNavigationSourceNotSpecified()
        {
            var customerResourceSet = CreateCustomerResourceSet();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResourceSet);
                    await jsonWriter.WriteEndAsync();
                },
                navigationSource: null,
                resourceType: null,
                writingResourceSet: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\",\"value\":[]}",
                result);
        }

        [Fact]
        public async Task WriteResourceAsync_ForResourceTypeNotSpecified()
        {
            var customerResourceSet = CreateCustomerResourceSet();
            var customerResource = CreateCustomerResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResourceSet);
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                navigationSource: null,
                resourceType: null,
                writingResourceSet: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]}",
                result);
        }

        [Fact]
        public async Task WriteResourceAsync_ForTypeNameNotSpecified()
        {
            var customerResource = new ODataResource
            {
                // TypeName not specified
                Properties = CreateCustomerResourceProperties(),
                SerializationInfo = CreateCustomerResourceSerializationInfo()
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteEndAsync();
                },
                navigationSource: null,
                resourceType: null);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}",
                result);
        }

        [Fact]
        public async Task WriteResourceAsync_ForNavigationSourceNotMatchedToParentDeltaResourceSet()
        {
            var orderDeltaResourceSet = CreateOrderDeltaResourceSet();
            var customerResource = CreateCustomerResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: true,
                writingDelta: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"value\":[" +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]}",
                result);
        }

        [Fact]
        public async Task WriteStringValueToTextWriterAsync()
        {
            var addressResource = CreateAddressResource();
            var pangramProperty = new ODataPropertyInfo { Name = "Pangram" };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(pangramProperty);

                    using (var textWriter = await jsonWriter.CreateTextWriterAsync())
                    {
                        await textWriter.WriteAsync("The quick brown fox jumps over the lazy dog");
                        await textWriter.FlushAsync();
                    }

                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\"," +
                "\"Pangram\":\"The quick brown fox jumps over the lazy dog\"}",
                result);
        }

        [Fact]
        public async Task WriteBinaryValueToStreamAsync()
        {
            var addressResource = CreateAddressResource();
            var streamProperty = new ODataStreamPropertyInfo
            {
                Name = "Stream",
                EditLink = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress/Stream/edit", UriKind.Absolute),
                ReadLink = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress/Stream/read", UriKind.Absolute),
                ContentType = "text/plain"
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(streamProperty);

                    await using (var stream = await jsonWriter.CreateBinaryWriteStreamAsync())
                    {
                        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                        await stream.WriteAsync(bytes, 0, 4);
                        await stream.WriteAsync(bytes, 4, 4);
                        await stream.WriteAsync(bytes, 8, 2);
                        await stream.FlushAsync();
                    }

                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\"," +
                "\"Stream@odata.mediaEditLink\":\"http://tempuri.org/Orders(1)/ShippingAddress/Stream/edit\"," +
                "\"Stream@odata.mediaReadLink\":\"http://tempuri.org/Orders(1)/ShippingAddress/Stream/read\"," +
                "\"Stream@odata.mediaContentType\":\"text/plain\"," +
                "\"Stream\":\"AQIDBAUGBwgJAA==\"}",
                result);
        }

        [Fact]
        public async Task WriteBinaryValueToStream_WithODataUtf8JsonWriter_Async()
        {
            var addressResource = CreateAddressResource();
            var streamProperty = new ODataStreamPropertyInfo
            {
                Name = "Stream",
                EditLink = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress/Stream/edit", UriKind.Absolute),
                ReadLink = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress/Stream/read", UriKind.Absolute),
                ContentType = "text/plain"
            };

            Action<IServiceCollection> configureWriter = (builder) =>
            {
                builder.AddSingleton<IJsonWriterFactory>(_ => ODataUtf8JsonWriterFactory.Default);
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(streamProperty);

                    using (var stream = await jsonWriter.CreateBinaryWriteStreamAsync())
                    {
                        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                        await stream.WriteAsync(bytes, 0, 4, CancellationToken.None);
                        await stream.WriteAsync(bytes, 4, 4, CancellationToken.None);
                        await stream.WriteAsync(bytes, 8, 2, CancellationToken.None);
                        await stream.FlushAsync(CancellationToken.None);
                    }

                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                configAction: configureWriter);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\"," +
                "\"Stream@odata.mediaEditLink\":\"http://tempuri.org/Orders(1)/ShippingAddress/Stream/edit\"," +
                "\"Stream@odata.mediaReadLink\":\"http://tempuri.org/Orders(1)/ShippingAddress/Stream/read\"," +
                "\"Stream@odata.mediaContentType\":\"text/plain\"," +
                "\"Stream\":\"AQIDBAUGBwgJAA==\"}",
                result);
        }

        [Fact]
        public async Task WriteStringValueToTextWriter_WithODataUtf8JsonWriter_Async()
        {
            var addressResource = CreateAddressResource();
            var pangramProperty = new ODataPropertyInfo { Name = "Pangram" };

            Action<IServiceCollection> configureWriter = (builder) =>
            {
                builder.AddSingleton<IJsonWriterFactory>(_ => ODataUtf8JsonWriterFactory.Default);
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(pangramProperty);

                    using (var textWriter = await jsonWriter.CreateTextWriterAsync())
                    {
                        string value = "The quick brown";
                        string value1 = " fox jumps over";
                        string value2 = " the lazy dog";
                        char[] charArray = value.ToCharArray();
                        char[] charArray1 = value1.ToCharArray();
                        char[] charArray2 = value2.ToCharArray();

                        // Call WriteAsync passing in the byte array
                        await textWriter.WriteAsync(charArray, 0, charArray.Length);
                        await textWriter.WriteAsync(charArray1, 0, charArray1.Length);
                        await textWriter.WriteAsync(charArray2, 0, charArray2.Length);
                        await textWriter.FlushAsync();
                    }

                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                configAction: configureWriter);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/NS.Address/$entity\"," +
                "\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\"," +
                "\"Pangram\":\"The quick brown fox jumps over the lazy dog\"}",
                result);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync()
        {
            var orderResourceSet = CreateOrderResourceSet();
            var orderResource = CreateOrderResource();
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();
            var customerResource = CreateCustomerResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResourceSet);
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: true,
                writingDelta: false,
                writingParameter: false,
                writingRequest: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders\"," +
                "\"value\":[" +
                "{\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                "\"Customer\":{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}}]}",
                result);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ContainingNestedResourceSet()
        {
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();
            var orderResourceSet = CreateOrderResourceSet();
            var orderResource = CreateOrderResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(orderResourceSet);
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: false,
                writingDelta: false,
                writingParameter: false,
                writingRequest: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders\":[{\"Id\":1,\"CustomerId\":1,\"Amount\":100}]}",
                result);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ContainingEntityReferenceLinks()
        {
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();
            var orderEntityReferenceLink1 = CreateOrderEntityReferenceLink(1);
            var orderEntityReferenceLink2 = CreateOrderEntityReferenceLink(2);

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                    await jsonWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: false,
                writingDelta: false,
                writingParameter: false,
                writingRequest: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders@odata.bind\":[\"http://tempuri.org/Orders(1)\",\"http://tempuri.org/Orders(2)\"]}",
                result);
        }

        [Fact]
        public async Task WriteV401RequestPayloadAsync_ContainingEntityReferenceLinks()
        {
            this.settings.Version = ODataVersion.V401;

            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();
            var orderEntityReferenceLink1 = CreateOrderEntityReferenceLink(1);
            var orderEntityReferenceLink2 = CreateOrderEntityReferenceLink(2);

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                    await jsonWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: false,
                writingDelta: false,
                writingParameter: false,
                writingRequest: true);

            Assert.Equal(
                "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders\":[" +
                "{\"@id\":\"Orders(1)\",\"@Is.EntityReferenceLink\":true}," +
                "{\"@id\":\"Orders(2)\",\"@Is.EntityReferenceLink\":true}]}",
                result);
        }

        [Fact]
        public async Task WriteResourceSetUriParameterAsync()
        {
            var customerResourceSet = CreateCustomerResourceSet();
            var customerResource = CreateCustomerResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResourceSet);
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: false,
                writingParameter: true);

            Assert.Equal("[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.Version6, "\"Products@odata.context\":\"http://tempuri.org/$metadata#Orders(1)/Products\",")]
        [InlineData(ODataLibraryCompatibility.Version7, "")]
        [InlineData(ODataLibraryCompatibility.None, "")]
        public async Task WriteContainmentAsync(ODataLibraryCompatibility libraryCompatilibity, string containmentContextUrl)
        {
            this.settings.LibraryCompatibility = libraryCompatilibity;

            var orderResource = CreateOrderResource();
            var productCollectionNestedResourceInfo = CreateProductCollectionNestedResourceInfo();
            var productResourceSet = CreateProductResourceSet();
            var productResource = CreateProductResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(productCollectionNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(productResourceSet);
                    await jsonWriter.WriteStartAsync(productResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1,\"CustomerId\":1,\"Amount\":100," +
                containmentContextUrl +
                "\"Products@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Products\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Pencil\"}]}",
                result);
        }

        [Fact]
        public async Task WriteDeferredNestedResourceInfoAsync()
        {
            var customerResourceSet = CreateCustomerResourceSet();
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResourceSet);
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"}]}",
                result);
        }


        [Fact]
        public async Task WriteDeltaResourceSetAsync_NoLongerThrowsExceptionForWriterNotConfiguredForWritingDelta()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();

            var result = await SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(customerDeltaResourceSet);
                        await jsonWriter.WriteEndAsync();
                    },
                    this.customerEntitySet,
                    this.customerEntityType,
                    writingResourceSet: true,
                    writingDelta: false);

            Assert.Equal(
               "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\",\"value\":[]}",
                result);
        }

        [Fact]
        public async Task WriteDeletedResourceAsync_WorksForDeletedResourceAtTopLevel()
        {
            var customerDeletedResource = CreateCustomerDeletedResource();

            var result = await SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(customerDeletedResource);
                        await jsonWriter.WriteEndAsync();
                    },
                    this.customerEntitySet,
                    this.customerEntityType,
                    writingResourceSet: false,
                    writingDelta: false);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$deletedEntity\",\"id\":\"http://tempuri.org/Customers(1)\",\"reason\":\"changed\"}",
                result);
        }

        [Fact]
        public async Task WriteTransientComplexResourceAsync_ODataIdAnnotationIsNotWritten()
        {
            var orderResource = new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>(),
                IsTransient = true,
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };

            var addressNestedResourceInfo = CreateAddressNestedResourceInfo();
            var addressResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "City", Value = "Redmond" }
                },
                IsTransient = true,
                SerializationInfo = CreateAddressResourceSerializationInfo()
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(addressNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                customerEntitySet,
                customerEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"@odata.id\":null,\"ShippingAddress\":{\"City\":\"Redmond\"}}",
                result);
        }

        [Fact]
        public void WriteTransientComplexResource_ODataIdAnnotationIsNotWritten()
        {
            var orderResource = new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>(),
                IsTransient = true,
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };

            var addressNestedResourceInfo = CreateAddressNestedResourceInfo();
            var addressResource = new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "City", Value = "Redmond" }
                },
                IsTransient = true,
                SerializationInfo = CreateAddressResourceSerializationInfo()
            };

            var result = SetupJsonWriterAndRunTest(
                (jsonWriter) =>
                {
                    jsonWriter.WriteStart(orderResource);
                    jsonWriter.WriteStart(addressNestedResourceInfo);
                    jsonWriter.WriteStart(addressResource);
                    jsonWriter.WriteEnd();
                    jsonWriter.WriteEnd();
                    jsonWriter.WriteEnd();
                },
                customerEntitySet,
                customerEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"@odata.id\":null,\"ShippingAddress\":{\"City\":\"Redmond\"}}",
                result);
        }

        [Fact]
        public async Task WriteResourceWithPropertyWithoutValueAsync()
        {
            var orderResource = new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataPropertyInfo>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataPropertyInfo
                    {
                        Name = "Amount",
                        InstanceAnnotations = new List<ODataInstanceAnnotation>
                        {
                            new ODataInstanceAnnotation("Has.Value", new ODataPrimitiveValue(false))
                        }
                    }
                },
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":1,\"Amount@Has.Value\":false}",
                result);
        }

        [Fact]
        public async Task WriteResourceWithUndeclaredPropertyWithoutValueAsync()
        {
            var orderResource = CreateOrderResource();
            var addressNestedResourceInfo = CreateAddressNestedResourceInfo();
            var addressResource = CreateAddressResource();
            var stateProperty = new ODataPropertyInfo
            {
                Name = "State",
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Has.Value", new ODataPrimitiveValue(false))
                }
            };

            var result = await SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(addressNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(stateProperty);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"CustomerId\":1," +
                "\"Amount\":100," +
                "\"ShippingAddress\":{\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\",\"State@Has.Value\":false}}",
                result);
        }

        [Fact]
        public void WriteResourceWithPropertyWithoutValue()
        {
            var orderResource = new ODataResource
            {
                TypeName = "NS.Order",
                Properties = new List<ODataPropertyInfo>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataPropertyInfo
                    {
                        Name = "Amount",
                        InstanceAnnotations = new List<ODataInstanceAnnotation>
                        {
                            new ODataInstanceAnnotation("Has.Value", new ODataPrimitiveValue(false))
                        }
                    }
                },
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };

            var result = SetupJsonWriterAndRunTest(
                (jsonWriter) =>
                {
                    jsonWriter.WriteStart(orderResource);
                    jsonWriter.WriteEnd();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\",\"Id\":1,\"Amount@Has.Value\":false}",
                result);
        }

        [Fact]
        public void WriteResourceWithUndeclaredPropertyWithoutValue()
        {
            var orderResource = CreateOrderResource();
            var addressNestedResourceInfo = CreateAddressNestedResourceInfo();
            var addressResource = CreateAddressResource();
            var stateProperty = new ODataPropertyInfo
            {
                Name = "State",
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Has.Value", new ODataPrimitiveValue(false))
                }
            };

            var result = SetupJsonWriterAndRunTest(
                (jsonWriter) =>
                {
                    jsonWriter.WriteStart(orderResource);
                    jsonWriter.WriteStart(addressNestedResourceInfo);
                    jsonWriter.WriteStart(addressResource);
                    jsonWriter.WriteStart(stateProperty);
                    jsonWriter.WriteEnd();
                    jsonWriter.WriteEnd();
                    jsonWriter.WriteEnd();
                    jsonWriter.WriteEnd();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"CustomerId\":1," +
                "\"Amount\":100," +
                "\"ShippingAddress\":{\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\",\"State@Has.Value\":false}}",
                result);
        }

        #region Exception Cases

        [Fact]
        public async Task WriteNestedResourceInfoAsync_ThrowsExceptionForNestedResourceInfoAtTopLevel()
        {
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    (jsonWriter) => jsonWriter.WriteStartAsync(customerNestedResourceInfo),
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "NestedResourceInfo"),
                exception.Message);
        }

        [Fact]
        public async Task WritePropertyInfoAsync_ThrowsExceptionForPropertyInfoAtTopLevel()
        {
            var stateProperty = new ODataPropertyInfo { Name = "State" };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    (jsonWriter) => jsonWriter.WriteStartAsync(stateProperty),
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "Property"), exception.Message);
        }

        [Fact]
        public async Task WritePropertyInfoAsync_ThrowsExceptionForPriorWritePropertyOperationNotEnded()
        {
            var addressResource = CreateAddressResource();
            var stateProperty = new ODataProperty { Name = "State", Value = "Washington" };
            var countryProperty = new ODataProperty { Name = "Country", Value = "USA" };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(stateProperty);
                    // Missing: await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteStartAsync(countryProperty);
                },
                this.orderEntitySet,
                this.orderEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_PropertyValueAlreadyWritten(stateProperty.Name),
                exception.Message);
        }

        [Fact]
        public async Task WritePropertyInfoAsync_ThrowsExceptionForDisallowedPropertyValue()
        {
            var addressResource = CreateAddressResource();
            var stateProperty = new ODataPropertyInfo { Name = "Resource" };
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(stateProperty);
                    await jsonWriter.WriteStartAsync(customerResource);
                },
                this.orderEntitySet,
                this.orderEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidStateTransition("Property", "Resource"),
                exception.Message);
        }

        [Fact]
        public async Task WriteTopLevelResourceSetAsync_ThrowsExceptionForWriterNotConfiguredForWritingResourceSet()
        {
            var customerResourceSet = CreateCustomerResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    (jsonWriter) => jsonWriter.WriteStartAsync(customerResourceSet),
                    this.customerEntitySet,
                    this.customerEntityType,
                    writingResourceSet: false));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter, exception.Message);
        }

        [Fact]
        public async Task WriteTopLevelResourceAsync_ThrowsExceptionForWriterConfiguredForWritingResourceSet()
        {
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    (jsonWriter) => jsonWriter.WriteStartAsync(customerResource),
                    this.customerEntitySet,
                    this.customerEntityType,
                    writingResourceSet: true));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter, exception.Message);
        }

        [Fact]
        public async Task WriteDeltaResourceSetAsync_ThrowsExceptionForWriterNotConfiguredForWritingResourceSet()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    (jsonWriter) => jsonWriter.WriteStartAsync(customerDeltaResourceSet),
                    this.customerEntitySet,
                    this.customerEntityType,
                    writingResourceSet: false,
                    writingDelta: true));

            Assert.Equal(
                Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter,
                exception.Message);
        }

        [Fact]
        public async Task WriteDeltaLinkAsync_ThrowsExceptionForWritedNotConfiguredForWritingDelta()
        {
            var deltaLink = new ODataDeltaLink(
                new Uri($"{ServiceUri}/Orders(1)"),
                new Uri($"{ServiceUri}/Customers(1)"),
                "Customer");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    // Shouldn't be written at top-level, either way but we validate writingDelta flag first
                    (jsonWriter) => jsonWriter.WriteDeltaLinkAsync(deltaLink),
                    this.orderEntitySet,
                    this.orderEntityType,
                    writingResourceSet: false,
                    writingDelta: false));

            Assert.Equal(Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "DeltaLink"),
                exception.Message);
        }

        [Fact]
        public async Task WriteDeltaDeletedLinkAsync_ThrowsExceptionForWriterNotConfiguredForWritingDelta()
        {
            var deltaLink = new ODataDeltaDeletedLink(
                new Uri($"{ServiceUri}/Orders(1)"),
                new Uri($"{ServiceUri}/Customers(1)"),
                "Customer");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    // Shouldn't be written at top-level, either way but we validate writingDelta flag first
                    (jsonWriter) => jsonWriter.WriteDeltaDeletedLinkAsync(deltaLink),
                    this.orderEntitySet,
                    this.orderEntityType,
                    writingResourceSet: false,
                    writingDelta: false));

            Assert.Equal(Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "DeltaDeletedLink"),
                exception.Message);
        }

        [Fact]
        public async Task WriteNestedResourceInfoAsync_ThrowsExceptionForParentResourceIsNull()
        {
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync((ODataResource)null);
                        await jsonWriter.WriteStartAsync(customerNestedResourceInfo);
                    },
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromNullResource("Resource", "NestedResourceInfo"),
                exception.Message);
        }

        [Fact]
        public async Task WriteNestedResourceAsync_ThrowsExceptionForResourceNotNestedInNestedResourceInfo()
        {
            var orderResource = CreateOrderResource();
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(orderResource);
                        await jsonWriter.WriteStartAsync(customerResource);
                    },
                    this.orderEntitySet,
                    this.orderEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromResource("Resource", "Resource"),
                exception.Message);
        }

        [Fact]
        public async Task WriteNestedResourceSetAsync_ThrowsExceptionForResourceSetNotNestedInCollectionNestedResourceInfo()
        {
            var customerResource = CreateCustomerResource();
            var orderResourceSet = CreateOrderResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteStartAsync(orderResourceSet);
                    },
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromResource("Resource", "ResourceSet"),
                exception.Message);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted)]
        [InlineData(DeltaDeletedEntryReason.Changed)]
        public async Task WriteNestedDeletedResourceAsync_ThrowsException(DeltaDeletedEntryReason reason)
        {
            //NOTE: Allowed for V401 but not V1
            var orderResource = CreateOrderResource();
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();
            var deletedResource = CreateCustomerDeletedResource();
            deletedResource.Reason = reason;

            var exception = await Assert.ThrowsAsync<ODataException>
                (() => SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(orderResource);
                    await jsonWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonWriter.WriteStartAsync(deletedResource);
                },
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: false,
                writingDelta: true));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeletedResource"),
                exception.Message);
        }

        [Fact]
        public async Task WriteDeletedResourceAsync_ThrowsExceptionWhenWrittenWithinTypedResourceSet()
        {
            var orderResourceSet = CreateOrderResourceSet();
            var orderDeletedResource = CreateOrderDeletedResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(orderResourceSet);
                        await jsonWriter.WriteStartAsync(orderDeletedResource);
                    },
                    this.orderEntitySet,
                    this.orderEntityType,
                    writingResourceSet: true,
                    writingDelta: true));

            Assert.Equal(
                Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter,
                exception.Message);
        }

        [Fact]
        public async Task WriteNestedDeltaResourceSetAsync_ThrowsException()
        {
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResource = CreateOrderCollectionNestedResourceInfo();
            var orderDeltaResourceSet = CreateOrderDeltaResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteStartAsync(orderCollectionNestedResource);
                        await jsonWriter.WriteStartAsync(orderDeltaResourceSet);
                    },
                    this.customerEntitySet,
                    this.customerEntityType,
                    writingResourceSet: false,
                    writingDelta: true));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeltaResourceSet"),
                exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForCountInResourceSet()
        {
            var orderResourceSet = CreateOrderResourceSet();
            orderResourceSet.Count = 5;

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                (jsonWriter) => jsonWriter.WriteStartAsync(orderResourceSet),
                this.orderEntitySet,
                this.orderEntityType,
                writingResourceSet: true,
                writingDelta: false,
                writingParameter: false,
                writingRequest: true));

            Assert.Equal(Strings.ODataWriterCore_QueryCountInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForNextPageLinkInDeltaResourceSet()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                (jsonWriter) => jsonWriter.WriteStartAsync(customerDeltaResourceSet),
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: true,
                writingParameter: false,
                writingRequest: true));

            Assert.Equal(Strings.ODataWriterCore_QueryNextLinkInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForDeltaLinkInDeltaResourceSet()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.DeltaLink = new Uri($"{ServiceUri}/Customers/deltaLink");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                (jsonWriter) => jsonWriter.WriteStartAsync(customerDeltaResourceSet),
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: true,
                writingParameter: false,
                writingRequest: true));

            Assert.Equal(Strings.ODataWriterCore_QueryDeltaLinkInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForDeferredNestedResourceInfoInPayload()
        {
            var customerResourceSet = CreateCustomerResourceSet();
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(customerResourceSet);
                    await jsonWriter.WriteStartAsync(customerResource);
                    await jsonWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                writingResourceSet: true,
                writingDelta: false,
                writingParameter: false,
                writingRequest: true));

            Assert.Equal(Strings.ODataWriterCore_DeferredLinkInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteStartAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                        // WriterState equals Completed at this point
                        // Try to start writing again
                        await jsonWriter.WriteStartAsync(customerResource);
                    },
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromCompleted("Completed", "Resource"),
                exception.Message);
        }

        [Fact]
        public async Task WriteEndAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                    async (jsonWriter) =>
                    {
                        await jsonWriter.WriteStartAsync(customerResource);
                        await jsonWriter.WriteEndAsync();
                        // WriterState equals Completed at this point
                        // Try to end writing again
                        await jsonWriter.WriteEndAsync();
                    },
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(Strings.ODataWriterCore_WriteEndCalledInInvalidState("Completed"), exception.Message);
        }

        [Fact]
        public async Task WriteEndAsync_ThrowsExceptionForBinaryStreamNotDisposed()
        {
            var addressResource = CreateAddressResource();
            var streamProperty = new ODataStreamPropertyInfo
            {
                Name = "Stream",
                EditLink = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress/Stream/edit", UriKind.Absolute),
                ReadLink = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress/Stream/read", UriKind.Absolute),
                ContentType = "text/plain"
            };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonWriterAndRunTestAsync(
                async (jsonWriter) =>
                {
                    await jsonWriter.WriteStartAsync(addressResource);
                    await jsonWriter.WriteStartAsync(streamProperty);

                    // `using` intentionally not used so as not to trigger dispose
                    var stream = await jsonWriter.CreateBinaryWriteStreamAsync();
                    var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                    await stream.WriteAsync(bytes, 0, 10);
                    await stream.FlushAsync();

                    await jsonWriter.WriteEndAsync();
                    await jsonWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType));

            Assert.Equal(Strings.ODataWriterCore_StreamNotDisposed, exception.Message);
        }

        #endregion Exception Cases

        /// <summary>
        /// Sets up an ODataJsonWriter,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonWriterAndRunTestAsync(
            Func<ODataJsonWriter, Task> func,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType resourceType,
            bool writingResourceSet = false,
            bool writingDelta = false,
            bool writingParameter = false,
            bool writingRequest = false,
            IODataReaderWriterListener writerListener = null,
            Action<IServiceCollection> configAction = null)
        {
            var jsonOutputContext = CreateJsonOutputContext(writingRequest, true, configAction);

            var jsonWriter = new ODataJsonWriter(
                jsonOutputContext,
                navigationSource,
                resourceType,
                writingResourceSet,
                writingParameter,
                writingDelta,
                writerListener);

            await func(jsonWriter);

            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        /// <summary>
        /// Sets up an ODataJsonWriter,
        /// then runs the given test code,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private string SetupJsonWriterAndRunTest(
            Action<ODataJsonWriter> action,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType resourceType,
            bool writingResourceSet = false,
            bool writingDelta = false,
            bool writingParameter = false,
            bool writingRequest = false,
            IODataReaderWriterListener writerListener = null,
            Action<IServiceCollection> configAction = null)
        {
            var jsonOutputContext = CreateJsonOutputContext(writingRequest, false, configAction);

            var jsonWriter = new ODataJsonWriter(
                jsonOutputContext,
                navigationSource,
                resourceType,
                writingResourceSet,
                writingParameter,
                writingDelta,
                writerListener);

            action(jsonWriter);

            this.stream.Position = 0;
            return new StreamReader(this.stream).ReadToEnd();
        }

        private ODataJsonOutputContext CreateJsonOutputContext(
            bool writingRequest = false,
            bool isAsync = true,
            Action<IServiceCollection> configAction = null)
        {
            IServiceProvider serviceProvider = null;

            if (configAction != null)
            {
                serviceProvider = ServiceProviderHelper.BuildServiceProvider(configAction);
            }

            Stream messageStream = this.stream;
            if (isAsync)
            {
                messageStream = new AsyncStream(messageStream);
            }

            var messageInfo = new ODataMessageInfo
            {
                MessageStream = messageStream,
                MediaType = new ODataMediaType("application", "json"),
                Encoding = Encoding.Default,
                IsResponse = !writingRequest,
                IsAsync = isAsync,
                Model = this.model,
                ServiceProvider = serviceProvider
            };

            return new ODataJsonOutputContext(messageInfo, this.settings);
        }

        #region Helper Methods

        private static ODataResourceSet CreateCustomerResourceSet()
        {
            return new ODataResourceSet
            {
                TypeName = "Collection(NS.Customer)",
                SerializationInfo = CreateCustomerResourceSerializationInfo()
            };
        }

        private static ODataResourceSet CreateOrderResourceSet()
        {
            return new ODataResourceSet
            {
                TypeName = "Collection(NS.Order)",
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };
        }

        private static ODataResourceSet CreateProductResourceSet()
        {
            return new ODataResourceSet
            {
                TypeName = "Collection(NS.Product)",
                SerializationInfo = CreateProductResourceSerializationInfo()
            };
        }

        private static ODataDeltaResourceSet CreateCustomerDeltaResourceSet()
        {
            return new ODataDeltaResourceSet
            {
                TypeName = "Collection(NS.Customer)",
                SerializationInfo = CreateCustomerResourceSerializationInfo()
            };
        }

        private static ODataDeltaResourceSet CreateOrderDeltaResourceSet()
        {
            return new ODataDeltaResourceSet
            {
                TypeName = "Collection(NS.Order)",
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };
        }

        private static ODataResource CreateCustomerResource()
        {
            return new ODataResource
            {
                TypeName = "NS.Customer",
                Properties = CreateCustomerResourceProperties(),
                SerializationInfo = CreateCustomerResourceSerializationInfo()
            };
        }

        private static ODataResource CreateOrderResource()
        {
            return new ODataResource
            {
                TypeName = "NS.Order",
                Properties = CreateOrderResourceProperties(),
                SerializationInfo = CreateOrderResourceSerializationInfo()
            };
        }

        private static ODataResource CreateProductResource()
        {
            return new ODataResource
            {
                TypeName = "NS.Product",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo{ PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Name", Value = "Pencil" }
                },
                SerializationInfo = CreateProductResourceSerializationInfo()
            };
        }

        private static ODataResource CreateAddressResource()
        {
            return new ODataResource
            {
                TypeName = "NS.Address",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Street", Value = "One Microsoft Way" },
                    new ODataProperty { Name = "City", Value = "Redmond" }
                },
                SerializationInfo = CreateAddressResourceSerializationInfo()
            };
        }

        private static ODataNestedResourceInfo CreateCustomerNestedResourceInfo()
        {
            return new ODataNestedResourceInfo
            {
                Name = "Customer",
                Url = new Uri($"{ServiceUri}/Orders(1)/Customer"),
                IsCollection = false,
                SerializationInfo = new ODataNestedResourceInfoSerializationInfo
                {
                    IsComplex = false,
                    IsUndeclared = false
                }
            };
        }

        private static ODataNestedResourceInfo CreateOrderCollectionNestedResourceInfo()
        {
            return new ODataNestedResourceInfo
            {
                Name = "Orders",
                Url = new Uri($"{ServiceUri}/Customers(1)/Orders"),
                IsCollection = true,
                SerializationInfo = new ODataNestedResourceInfoSerializationInfo
                {
                    IsComplex = false,
                    IsUndeclared = false
                }
            };
        }

        private static ODataNestedResourceInfo CreateProductCollectionNestedResourceInfo()
        {
            return new ODataNestedResourceInfo
            {
                Name = "Products",
                Url = new Uri($"{ServiceUri}/Orders(1)/Products"),
                IsCollection = true,
                SerializationInfo = new ODataNestedResourceInfoSerializationInfo
                {
                    IsComplex = false,
                    IsUndeclared = false
                }
            };
        }

        private static ODataNestedResourceInfo CreateAddressNestedResourceInfo()
        {
            return new ODataNestedResourceInfo
            {
                Name = "ShippingAddress",
                Url = new Uri($"{ServiceUri}/Orders(1)/ShippingAddress"),
                IsCollection = false,
                SerializationInfo = new ODataNestedResourceInfoSerializationInfo
                {
                    IsComplex = true,
                    IsUndeclared = false
                }
            };
        }

        private static ODataDeletedResource CreateCustomerDeletedResource()
        {
            return new ODataDeletedResource()
            {
                TypeName = "NS.Customer",
                Id = new Uri($"{ServiceUri}/Customers(1)"),
                Properties = CreateCustomerResourceProperties(),
                SerializationInfo = CreateCustomerResourceSerializationInfo(),
                Reason = DeltaDeletedEntryReason.Changed
            };
        }

        private static ODataDeletedResource CreateOrderDeletedResource()
        {
            return new ODataDeletedResource()
            {
                TypeName = "NS.Order",
                Id = new Uri($"{ServiceUri}/Orders(1)"),
                Properties = CreateOrderResourceProperties(),
                SerializationInfo = CreateOrderResourceSerializationInfo(),
                Reason = DeltaDeletedEntryReason.Changed
            };
        }

        private static ODataEntityReferenceLink CreateCustomerEntityReferenceLink(int customerId)
        {
            return new ODataEntityReferenceLink
            {
                Url = new Uri($"{ServiceUri}/Customers({customerId})"),
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.EntityReferenceLink", new ODataPrimitiveValue(true))
                }
            };
        }

        private static ODataEntityReferenceLink CreateOrderEntityReferenceLink(int orderId)
        {
            return new ODataEntityReferenceLink
            {
                Url = new Uri($"{ServiceUri}/Orders({orderId})"),
                InstanceAnnotations = new Collection<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.EntityReferenceLink", new ODataPrimitiveValue(true))
                }
            };
        }

        private static ODataResourceSerializationInfo CreateCustomerResourceSerializationInfo()
        {
            return new ODataResourceSerializationInfo
            {
                NavigationSourceName = "Customers",
                ExpectedTypeName = "NS.Customer",
                NavigationSourceEntityTypeName = "NS.Customer",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };
        }

        private static ODataResourceSerializationInfo CreateOrderResourceSerializationInfo()
        {
            return new ODataResourceSerializationInfo
            {
                ExpectedTypeName = "NS.Order",
                NavigationSourceName = "Orders",
                NavigationSourceEntityTypeName = "NS.Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };
        }

        private static ODataResourceSerializationInfo CreateProductResourceSerializationInfo()
        {
            return new ODataResourceSerializationInfo
            {
                ExpectedTypeName = "NS.Product",
                NavigationSourceName = "Orders",
                NavigationSourceEntityTypeName = "NS.Order",
                NavigationSourceKind = EdmNavigationSourceKind.ContainedEntitySet
            };
        }

        private static ODataResourceSerializationInfo CreateAddressResourceSerializationInfo()
        {
            return new ODataResourceSerializationInfo
            {
                ExpectedTypeName = "NS.Address",
                NavigationSourceName = "Orders",
                NavigationSourceEntityTypeName = "NS.Order",
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet
            };
        }

        private static List<ODataProperty> CreateCustomerResourceProperties()
        {
            return new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "Id",
                    Value = 1,
                    SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                },
                new ODataProperty { Name = "Name", Value = "Sue" },
                new ODataProperty { Name = "Type", Value = new ODataEnumValue("Retail") }
            };
        }

        private static List<ODataProperty> CreateOrderResourceProperties()
        {
            return new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "Id",
                    Value = 1,
                    SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                },
                new ODataProperty { Name = "CustomerId", Value = 1 },
                new ODataProperty { Name = "Amount", Value = 100M }
            };
        }

        private static ODataAction CreateODataAction(Uri uri, string actionName)
        {
            return new ODataAction
            {
                Title = actionName,
                Target = new Uri($"{uri}/{actionName}"),
                Metadata = new Uri($"{ServiceUri}/$metadata/#Action")
            };
        }

        private static ODataFunction CreateODataFunction(Uri uri, string functionName)
        {
            return new ODataFunction
            {
                Title = functionName,
                Target = new Uri($"{uri}/{functionName}"),
                Metadata = new Uri($"{ServiceUri}/$metadata/#Function")
            };
        }

        #endregion Helper Methods
    }
}
