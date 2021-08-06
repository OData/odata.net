//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;

namespace Microsoft.OData.Core.Tests.JsonLight
{
    public class ODataJsonLightWriterTests
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

        public ODataJsonLightWriterTests()
        {
            InitializeEdmModel();

            this.stream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings
            {
                EnableMessageStreamDisposal = false,
                Version = ODataVersion.V4,
                ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*")
            };
            this.settings.SetServiceDocumentUri(new Uri(ServiceUri));
        }

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.customerTypeEnumType = new EdmEnumType("NS", "CustomerType");
            this.addressComplexType = new EdmComplexType("NS", "Address", /*baseType*/ null, /*isAbstract*/ true, /*isOpen*/ true);
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
            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResourceSet);
                    await jsonLightWriter.WriteEndAsync(); // Flushes the stream
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ true);

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
            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerDeltaResourceSet);
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResultSet*/ true,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonLightWriter.WriteStartAsync(orderNestedResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResultSet*/ false,
                /*writingDelta*/ true);

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
            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(resource);
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteStartAsync(nestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(nestedResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(orderResourceSet);
                    await jsonLightWriter.WriteStartAsync(orderNestedResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerDeltaResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerDeletedResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResultSet*/ true,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerDeltaResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerDeletedResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResultSet*/ true,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(customerDeletedResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResultSet*/ false,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(customerEntityReferenceLink);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(orderResourceSet);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonLightWriter.WriteDeltaLinkAsync(deltaLink);
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResultSet*/ true,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonLightWriter.WriteDeltaDeletedLinkAsync(deltaDeletedLink);
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResultSet*/ true,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(stateProperty);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(stateProperty);
                    await jsonLightWriter.WritePrimitiveAsync(primitiveValue);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteResourceSetAsync_ForNavigationSourceNotSpecified()
        {
            var customerResourceSet = CreateCustomerResourceSet();

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResourceSet);
                    await jsonLightWriter.WriteEndAsync();
                },
                /*navigationSource*/ null,
                /*resourceType*/ null,
                /*writingResourceSet*/ true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\",\"value\":[]}",
                result);
        }

        [Fact]
        public async Task WriteResourceAsync_ForResourceTypeNotSpecified()
        {
            var customerResourceSet = CreateCustomerResourceSet();
            var customerResource = CreateCustomerResource();

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                /*navigationSource*/ null,
                /*resourceType*/ null,
                /*writingResourceSet*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                },
                /*navigationSource*/ null,
                /*resourceType*/ null);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderDeltaResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(pangramProperty);

                    using (var textWriter = await jsonLightWriter.CreateTextWriterAsync())
                    {
                        await textWriter.WriteAsync("The quick brown fox jumps over the lazy dog");
                        await textWriter.FlushAsync();
                    }
                    
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(streamProperty);

                    using (var stream = await jsonLightWriter.CreateBinaryWriteStreamAsync())
                    {
                        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                        await stream.WriteAsync(bytes, 0, 4);
                        await stream.WriteAsync(bytes, 4, 4);
                        await stream.WriteAsync(bytes, 8, 2);
                        await stream.FlushAsync();
                    }

                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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
        public async Task WriteRequestPayloadAsync()
        {
            var orderResourceSet = CreateOrderResourceSet();
            var orderResource = CreateOrderResource();
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();
            var customerResource = CreateCustomerResource();

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderResourceSet);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ false,
                /*writingParameter*/ false,
                /*writingRequest*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(orderResourceSet);
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ false,
                /*writingDelta*/ false,
                /*writingParameter*/ false,
                /*writingRequest*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ false,
                /*writingDelta*/ false,
                /*writingParameter*/ false,
                /*writingRequest*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                    await jsonLightWriter.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ false,
                /*writingDelta*/ false,
                /*writingParameter*/ false,
                /*writingRequest*/ true);

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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ false,
                /*writingParameter*/ true);

            Assert.Equal("[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]", result);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.Version6, "\"Products@odata.context\":\"http://tempuri.org/$metadata#Orders(1)/Products\",")]
        [InlineData(ODataLibraryCompatibility.Version7, "")]
        [InlineData(ODataLibraryCompatibility.Latest, "")]
        public async Task WriteContainmentAsync(ODataLibraryCompatibility libraryCompatilibity, string containmentContextUrl)
        {
            this.settings.LibraryCompatibility = libraryCompatilibity;

            var orderResource = CreateOrderResource();
            var productCollectionNestedResourceInfo = CreateProductCollectionNestedResourceInfo();
            var productResourceSet = CreateProductResourceSet();
            var productResource = CreateProductResource();

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteStartAsync(productCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(productResourceSet);
                    await jsonLightWriter.WriteStartAsync(productResource);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
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

            var result = await SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ true);

            Assert.Equal(
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"}]}",
                result);
        }

        #region Exception Cases

        [Fact]
        public async Task WriteNestedResourceInfoAsync_ThrowsExceptionForNestedResourceInfoAtTopLevel()
        {
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerNestedResourceInfo),
                    this.customerEntitySet,
                    this.customerEntityType));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "NestedResourceInfo"),
                exception.Message);
        }

        [Fact]
        public async Task WriteDeletedResourceAsync_ThrowsExceptionForDeletedResourceAtTopLevel()
        {
            var customerDeletedResource = CreateCustomerDeletedResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerDeletedResource),
                    this.customerEntitySet,
                    this.customerEntityType,
                    /*writingResourceSet*/ false,
                    /*writingDelta*/ true));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "DeletedResource"),
                exception.Message);
        }

        [Fact]
        public async Task WritePropertyInfoAsync_ThrowsExceptionForPropertyInfoAtTopLevel()
        {
            var stateProperty = new ODataPropertyInfo { Name = "State" };

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(stateProperty),
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
                () => SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(stateProperty);
                    // Missing: await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteStartAsync(countryProperty);
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
                () => SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(stateProperty);
                    await jsonLightWriter.WriteStartAsync(customerResource);
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
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerResourceSet),
                    this.customerEntitySet,
                    this.customerEntityType,
                    /*writingResourceSet*/ false));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter, exception.Message);
        }

        [Fact]
        public async Task WriteTopLevelResourceAsync_ThrowsExceptionForWriterConfiguredForWritingResourceSet()
        {
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerResource),
                    this.customerEntitySet,
                    this.customerEntityType,
                    /*writingResourceSet*/ true));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter, exception.Message);
        }

        [Fact]
        public async Task WriteDeltaResourceSetAsync_ThrowsExceptionForWriterNotConfiguredForWritingResourceSet()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerDeltaResourceSet),
                    this.customerEntitySet,
                    this.customerEntityType,
                    /*writingResourceSet*/ false,
                    /*writingDelta*/ true));

            Assert.Equal(
                Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter,
                exception.Message);
        }

        [Fact]
        public async Task WriteDeltaResourceSetAsync_ThrowsExceptionForWriterNotConfiguredForWritingDelta()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerDeltaResourceSet),
                    this.customerEntitySet,
                    this.customerEntityType,
                    /*writingResourceSet*/ true,
                    /*writingDelta*/ false));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, exception.Message);
        }

        [Fact]
        public async Task WriteDeltaLinkAsync_ThrowsExceptionForWritedNotConfiguredForWritingDelta()
        {
            var deltaLink = new ODataDeltaLink(
                new Uri($"{ServiceUri}/Orders(1)"),
                new Uri($"{ServiceUri}/Customers(1)"),
                "Customer");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    // Shouldn't be written at top-level, either way but we validate writingDelta flag first
                    (jsonLightWriter) => jsonLightWriter.WriteDeltaLinkAsync(deltaLink),
                    this.orderEntitySet,
                    this.orderEntityType,
                    /*writingResourceSet*/ false,
                    /*writingDelta*/ false));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, exception.Message);
        }

        [Fact]
        public async Task WriteDeltaDeletedLinkAsync_ThrowsExceptionForWriterNotConfiguredForWritingDelta()
        {
            var deltaLink = new ODataDeltaDeletedLink(
                new Uri($"{ServiceUri}/Orders(1)"),
                new Uri($"{ServiceUri}/Customers(1)"),
                "Customer");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    // Shouldn't be written at top-level, either way but we validate writingDelta flag first
                    (jsonLightWriter) => jsonLightWriter.WriteDeltaDeletedLinkAsync(deltaLink),
                    this.orderEntitySet,
                    this.orderEntityType,
                    /*writingResourceSet*/ false,
                    /*writingDelta*/ false));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, exception.Message);
        }

        [Fact]
        public async Task WriteNestedResourceInfoAsync_ThrowsExceptionForParentResourceIsNull()
        {
            var customerNestedResourceInfo = CreateCustomerNestedResourceInfo();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync((ODataResource)null);
                        await jsonLightWriter.WriteStartAsync(customerNestedResourceInfo);
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
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync(orderResource);
                        await jsonLightWriter.WriteStartAsync(customerResource);
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
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteStartAsync(orderResourceSet);
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
                (() => SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(orderResource);
                    await jsonLightWriter.WriteStartAsync(customerNestedResourceInfo);
                    await jsonLightWriter.WriteStartAsync(deletedResource);
                },
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResultSet*/ false,
                /*writingDelta*/ true));

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
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync(orderResourceSet);
                        await jsonLightWriter.WriteStartAsync(orderDeletedResource);
                    },
                    this.orderEntitySet,
                    this.orderEntityType,
                    /*writingResourceSet*/ true,
                    /*writingDelta*/ true));

            Assert.Equal(
                Strings.ODataWriterCore_InvalidTransitionFromResourceSet("ResourceSet", "DeletedResource"),
                exception.Message);
        }

        [Fact]
        public async Task WriteNestedDeltaResourceSetAsync_ThrowsException()
        {
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResource = CreateOrderCollectionNestedResourceInfo();
            var orderDeltaResourceSet = CreateOrderDeltaResourceSet();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteStartAsync(orderCollectionNestedResource);
                        await jsonLightWriter.WriteStartAsync(orderDeltaResourceSet);
                    },
                    this.customerEntitySet,
                    this.customerEntityType,
                    /*writingResourceSet*/ false,
                    /*writingDelta*/ true));

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
                () => SetupJsonLightWriterAndRunTestAsync(
                (jsonLightWriter) => jsonLightWriter.WriteStartAsync(orderResourceSet),
                this.orderEntitySet,
                this.orderEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ false,
                /*writingParameter*/ false,
                /*writingRequest*/ true));

            Assert.Equal(Strings.ODataWriterCore_QueryCountInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForNextPageLinkInDeltaResourceSet()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerDeltaResourceSet),
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ true,
                /*writingParameter*/ false,
                /*writingRequest*/ true));

            Assert.Equal(Strings.ODataWriterCore_QueryNextLinkInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForDeltaLinkInDeltaResourceSet()
        {
            var customerDeltaResourceSet = CreateCustomerDeltaResourceSet();
            customerDeltaResourceSet.DeltaLink = new Uri($"{ServiceUri}/Customers/deltaLink");

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                (jsonLightWriter) => jsonLightWriter.WriteStartAsync(customerDeltaResourceSet),
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ true,
                /*writingParameter*/ false,
                /*writingRequest*/ true));

            Assert.Equal(Strings.ODataWriterCore_QueryDeltaLinkInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteRequestPayloadAsync_ThrowsExceptionForDeferredNestedResourceInfoInPayload()
        {
            var customerResourceSet = CreateCustomerResourceSet();
            var customerResource = CreateCustomerResource();
            var orderCollectionNestedResourceInfo = CreateOrderCollectionNestedResourceInfo();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(customerResourceSet);
                    await jsonLightWriter.WriteStartAsync(customerResource);
                    await jsonLightWriter.WriteStartAsync(orderCollectionNestedResourceInfo);
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.customerEntitySet,
                this.customerEntityType,
                /*writingResourceSet*/ true,
                /*writingDelta*/ false,
                /*writingParameter*/ false,
                /*writingRequest*/ true));

            Assert.Equal(Strings.ODataWriterCore_DeferredLinkInRequest, exception.Message);
        }

        [Fact]
        public async Task WriteStartAsync_ThrowsExceptionForWriterInCompletedState()
        {
            var customerResource = CreateCustomerResource();

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteEndAsync();
                        // WriterState equals Completed at this point
                        // Try to start writing again
                        await jsonLightWriter.WriteStartAsync(customerResource);
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
                () => SetupJsonLightWriterAndRunTestAsync(
                    async (jsonLightWriter) =>
                    {
                        await jsonLightWriter.WriteStartAsync(customerResource);
                        await jsonLightWriter.WriteEndAsync();
                        // WriterState equals Completed at this point
                        // Try to end writing again
                        await jsonLightWriter.WriteEndAsync();
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
                () => SetupJsonLightWriterAndRunTestAsync(
                async (jsonLightWriter) =>
                {
                    await jsonLightWriter.WriteStartAsync(addressResource);
                    await jsonLightWriter.WriteStartAsync(streamProperty);

                    // `using` intentionally not used so as not to trigger dispose
                    var stream = await jsonLightWriter.CreateBinaryWriteStreamAsync();
                    var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                    await stream.WriteAsync(bytes, 0, 10);
                    await stream.FlushAsync();

                    await jsonLightWriter.WriteEndAsync();
                    await jsonLightWriter.WriteEndAsync();
                },
                this.orderEntitySet,
                this.orderEntityType));

            Assert.Equal(Strings.ODataWriterCore_StreamNotDisposed, exception.Message);
        }

        #endregion Exception Cases

        /// <summary>
        /// Sets up an ODataJsonLightWriter,
        /// then runs the given test code asynchronously,
        /// then flushes and reads the stream back as a string for customized verification.
        /// </summary>
        private async Task<string> SetupJsonLightWriterAndRunTestAsync(
            Func<ODataJsonLightWriter, Task> func,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType resourceType,
            bool writingResourceSet = false,
            bool writingDelta = false,
            bool writingParameter = false,
            bool writingRequest = false,
            IODataReaderWriterListener writerListener = null)
        {
            var jsonLightOutputContext = CreateJsonLightOutputContext(writingRequest);

            var jsonLightWriter = new ODataJsonLightWriter(
                jsonLightOutputContext,
                navigationSource,
                resourceType,
                writingResourceSet,
                writingParameter,
                writingDelta,
                writerListener);

            await func(jsonLightWriter);

            this.stream.Position = 0;
            return await new StreamReader(this.stream).ReadToEndAsync();
        }

        private ODataJsonLightOutputContext CreateJsonLightOutputContext(bool writingRequest = false)
        {
            var messageInfo = new ODataMessageInfo
            {
                MessageStream = this.stream,
                MediaType = new ODataMediaType("application", "json"),
#if NETCOREAPP1_1
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = !writingRequest,
                IsAsync = true,
                Model = this.model
            };

            return new ODataJsonLightOutputContext(messageInfo, this.settings);
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
