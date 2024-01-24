//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Xunit;
using ErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightReaderTests
    {
        private ODataMessageReaderSettings messageReaderSettings;
        private EdmModel model;
        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntityType productEntityType; // Test containment
        private EdmComplexType addressComplexType;
        private EdmEnumType customerTypeEnumType;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;

        public ODataJsonLightReaderTests()
        {
            this.messageReaderSettings = new ODataMessageReaderSettings();
            this.InitializeModel();
        }

        [Fact]
        public async Task ReadTopLevelResourceSetAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"#NS.RateCustomers\":{\"title\":\"RateCustomers\",\"target\":\"http://tempuri.org/Customers/RateCustomers\"}," +
                "\"#NS.Top5Customers\":{\"title\":\"Top5Customers\",\"target\":\"http://tempuri.org/Customers/Top5Customers\"}," +
                "\"@odata.count\":1," +
                "\"@odata.nextLink\":\"http://tempuri.org/Customers/nextLink\"," +
                "\"@Is.ResourceSet\":true," +
                "\"value\":[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceSetAction: (resourceSet) =>
                    {
                        Assert.NotNull(resourceSet);
                        Assert.Equal(1, resourceSet.Count);
                        Assert.Equal("http://tempuri.org/Customers/nextLink", resourceSet.NextPageLink.AbsoluteUri);
                        var instanceAnnotation = Assert.Single(resourceSet.InstanceAnnotations);
                        Assert.Equal("Is.ResourceSet", instanceAnnotation.Name);
                        var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                        Assert.Equal(true, annotationValue.Value);
                        var rateCustomersAction = Assert.Single(resourceSet.Actions);
                        var top5CustomersFunction = Assert.Single(resourceSet.Functions);
                        Assert.Equal("RateCustomers", rateCustomersAction.Title);
                        Assert.Equal("http://tempuri.org/Customers/RateCustomers", rateCustomersAction.Target.AbsoluteUri);
                        Assert.Equal("http://tempuri.org/$metadata#NS.RateCustomers", rateCustomersAction.Metadata.AbsoluteUri);
                        Assert.Equal("Top5Customers", top5CustomersFunction.Title);
                        Assert.Equal("http://tempuri.org/Customers/Top5Customers", top5CustomersFunction.Target.AbsoluteUri);
                        Assert.Equal("http://tempuri.org/$metadata#NS.Top5Customers", top5CustomersFunction.Metadata.AbsoluteUri);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(3, properties.Length);
                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                        Assert.Equal("Type", properties[2].Name);
                        var customerType = Assert.IsType<ODataEnumValue>(properties[2].Value);
                        Assert.Equal("Retail", customerType.Value);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.Equal("Orders", nestedResourceInfo.Name);
                        Assert.NotNull(nestedResourceInfo.Url);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders", nestedResourceInfo.Url.AbsoluteUri);
                        Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                    }),
                readingResourceSet: true);
        }

        [Fact]
        public async Task ReadTopLevelResourceSetWithDeltaLinkAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"@odata.count\":1," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"value\":[{\"Id\":1,\"Name\":\"Sue\",\"Type\":\"Retail\"}]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceSetAction: (resourceSet) =>
                    {
                        Assert.NotNull(resourceSet);
                        Assert.Equal(1, resourceSet.Count);
                        Assert.NotNull(resourceSet.DeltaLink);
                        Assert.Equal("http://tempuri.org/Customers/deltaLink", resourceSet.DeltaLink.AbsoluteUri);
                    }),
                readingResourceSet: true);
        }

        [Fact]
        public async Task ReadTopLevelResourceAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.editLink\":\"http://tempuri.org/Customers(1)\"," +
                "\"@odata.readLink\":\"http://tempuri.org/Customers(1)\"," +
                "\"@odata.mediaEditLink\":\"http://tempuri.org/Customers(1)/Photo\"," +
                "\"@odata.mediaReadLink\":\"http://tempuri.org/Customers(1)/Photo\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"media-etag\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Type\":\"Retail\"," +
                "\"@Is.Resource\":true," +
                "\"#NS.CancelOrder\":{\"title\":\"CancelOrder\",\"target\":\"http://tempuri.org/Customers(1)/CancelOrder\"}," +
                "\"#NS.MostRecentOrder\":{\"title\":\"MostRecentOrder\",\"target\":\"http://tempuri.org/Customers(1)/MostRecentOrder\"}," +
                "\"Orders@odata.count\":0," +
                "\"Orders@odata.nextLink\":\"http://tempuri.org/Customers(1)/Orders/nextLink\"," +
                "\"Orders@odata.associationLink\":\"http://tempuri.org/Customers(1)/Orders/$ref\"," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"," +
                "\"Orders\":[]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceSetAction: (nestedResourceSet) =>
                    {
                        Assert.Equal(0, nestedResourceSet.Count);
                        Assert.NotNull(nestedResourceSet.NextPageLink);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders/nextLink", nestedResourceSet.NextPageLink.AbsoluteUri);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(3, properties.Length);
                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                        Assert.Equal("Type", properties[2].Name);
                        var customerType = Assert.IsType<ODataEnumValue>(properties[2].Value);
                        Assert.Equal("Retail", customerType.Value);
                        Assert.NotNull(resource.ReadLink);
                        Assert.Equal("http://tempuri.org/Customers(1)", resource.ReadLink.AbsoluteUri);
                        Assert.NotNull(resource.EditLink);
                        Assert.Equal("http://tempuri.org/Customers(1)", resource.EditLink.AbsoluteUri);
                        var streamReferenceValue = Assert.IsType<ODataStreamReferenceValue>(resource.MediaResource);
                        Assert.Equal("http://tempuri.org/Customers(1)/Photo", streamReferenceValue.ReadLink.AbsoluteUri);
                        Assert.NotNull(resource.EditLink);
                        Assert.Equal("http://tempuri.org/Customers(1)/Photo", streamReferenceValue.EditLink.AbsoluteUri);
                        Assert.Equal("image/png", streamReferenceValue.ContentType);
                        Assert.Equal("media-etag", streamReferenceValue.ETag);
                        var cancelOrderAction = Assert.Single(resource.Actions);
                        Assert.Equal("CancelOrder", cancelOrderAction.Title);
                        Assert.Equal("http://tempuri.org/Customers(1)/CancelOrder", cancelOrderAction.Target.AbsoluteUri);
                        Assert.Equal("http://tempuri.org/$metadata#NS.CancelOrder", cancelOrderAction.Metadata.AbsoluteUri);
                        var mostRecentOrderFunction = Assert.Single(resource.Functions);
                        Assert.Equal("MostRecentOrder", mostRecentOrderFunction.Title);
                        Assert.Equal("http://tempuri.org/Customers(1)/MostRecentOrder", mostRecentOrderFunction.Target.AbsoluteUri);
                        Assert.Equal("http://tempuri.org/$metadata#NS.MostRecentOrder", mostRecentOrderFunction.Metadata.AbsoluteUri);
                        var instanceAnnotation = Assert.Single(resource.InstanceAnnotations);
                        Assert.Equal("Is.Resource", instanceAnnotation.Name);
                        var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                        Assert.Equal(true, annotationValue.Value);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.Equal("Orders", nestedResourceInfo.Name);
                        Assert.NotNull(nestedResourceInfo.Url);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders", nestedResourceInfo.Url.AbsoluteUri);
                        Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                    }));
        }

        [Fact]
        public async Task ReadTopLevelResource_WithPropertyWithoutValueAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Type@custom.instance\":\"Retail\"" +
              "}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(2, properties.Length);
                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                    },
                    verifyNestedPropertyInfoAction: (item) =>
                    {
                        ODataPropertyInfo propertyInfo = Assert.IsType<ODataPropertyInfo>(item);

                        Assert.Equal("Type", propertyInfo.Name);
                        ODataInstanceAnnotation annotation = Assert.Single(propertyInfo.InstanceAnnotations);
                        Assert.Equal("custom.instance", annotation.Name);
                        Assert.Equal("Retail", annotation.Value.FromODataValue());
                    }));
        }

        [Fact]
        public async Task ReadTopLevelDeltaResourceSetAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.count\":2," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"@Is.DeltaResourceSet\":true," +
                "\"value\":[" +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$deletedEntity\",\"id\":\"http://tempuri.org/Customers(7)\",\"reason\":\"deleted\"}," +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$deletedEntity\",\"id\":\"http://tempuri.org/Customers(13)\",\"reason\":\"changed\"}]}";

            var verifyDeletedResourceActionStack = new Stack<Action<ODataDeletedResource>>();

            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);

                Assert.NotNull(deletedResource.Id);
                Assert.Equal("http://tempuri.org/Customers(13)", deletedResource.Id.AbsoluteUri);
                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
            });

            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);

                Assert.NotNull(deletedResource.Id);
                Assert.Equal("http://tempuri.org/Customers(7)", deletedResource.Id.AbsoluteUri);
                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(2, deltaResourceSet.Count);
                        Assert.Equal("http://tempuri.org/Customers/deltaLink", deltaResourceSet.DeltaLink.AbsoluteUri);
                        var instanceAnnotation = Assert.Single(deltaResourceSet.InstanceAnnotations);
                        Assert.Equal("Is.DeltaResourceSet", instanceAnnotation.Name);
                        var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                        Assert.Equal(true, annotationValue.Value);
                    },
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotEmpty(verifyDeletedResourceActionStack);
                        var innerVerifyDeletedResourceAction = verifyDeletedResourceActionStack.Pop();

                        innerVerifyDeletedResourceAction(deletedResource);
                    }),
                readingResourceSet: true,
                readingDelta: true);

            Assert.Empty(verifyDeletedResourceActionStack);
        }

        [Fact]
        public async Task ReadV401TopLevelDeltaResourceSetAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@count\":2," +
                "\"value\":[" +
                "{\"@removed\":{\"reason\":\"deleted\"},\"@id\":\"http://tempuri.org/Customers(7)\"}," +
                "{\"@removed\":{\"reason\":\"changed\"},\"@id\":\"http://tempuri.org/Customers(13)\"}]}";

            var verifyDeletedResourceActionStack = new Stack<Action<ODataDeletedResource>>();

            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);

                Assert.NotNull(deletedResource.Id);
                Assert.Equal("http://tempuri.org/Customers(13)", deletedResource.Id.AbsoluteUri);
                Assert.Equal(DeltaDeletedEntryReason.Changed, deletedResource.Reason);
            });

            verifyDeletedResourceActionStack.Push((deletedResource) =>
            {
                Assert.NotNull(deletedResource);

                Assert.NotNull(deletedResource.Id);
                Assert.Equal("http://tempuri.org/Customers(7)", deletedResource.Id.AbsoluteUri);
                Assert.Equal(DeltaDeletedEntryReason.Deleted, deletedResource.Reason);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                        Assert.Equal(2, deltaResourceSet.Count);
                    },
                    verifyDeletedResourceAction: (deletedResource) =>
                    {
                        Assert.NotEmpty(verifyDeletedResourceActionStack);
                        var innerVerifyDeletedResourceAction = verifyDeletedResourceActionStack.Pop();

                        innerVerifyDeletedResourceAction(deletedResource);
                    }),
                readingResourceSet: true,
                readingDelta: true);

            Assert.Empty(verifyDeletedResourceActionStack);
        }

        [Fact]
        public async Task ReadNestedResourceAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"Customer\":{\"Id\":1,\"Name\":\"Sue\"}}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Products", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Orders(1)/Products", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Orders(1)/Products/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Orders", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Customers(1)/Orders", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Customers(1)/Orders/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Customer", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Orders(1)/Customer", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Orders(1)/Customer/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var innerVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();

                        innerVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }));

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Fact]
        public async Task ReadNestedResourceSetAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Orders\":[" +
                "{\"Id\":1,\"Amount\":100}," +
                "{\"Id\":2,\"Amount\":130}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(2, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(130M, properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    }));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadNullNestedResourceAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"Customer\":null}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) => Assert.Null(resource));
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Products", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Orders(1)/Products", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Orders(1)/Products/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Customer", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Orders(1)/Customer", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Orders(1)/Customer/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var innerVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();

                        innerVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }));

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Fact]
        public async Task ReadComplexNestedResourceAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"ShippingAddress\":{\"Street\":\"One Microsoft Way\",\"City\":\"Redmond\"}}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Address", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Street", properties[0].Name);
                Assert.Equal("One Microsoft Way", properties[0].Value);
                Assert.Equal("City", properties[1].Name);
                Assert.Equal("Redmond", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            var verifyNestedResourceInfoActionStack = new Stack<Action<ODataNestedResourceInfo>>();

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Products", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Orders(1)/Products", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Orders(1)/Products/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("Customer", nestedResourceInfo.Name);
                Assert.NotNull(nestedResourceInfo.Url);
                Assert.Equal("http://tempuri.org/Orders(1)/Customer", nestedResourceInfo.Url.AbsoluteUri);
                Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                Assert.Equal("http://tempuri.org/Orders(1)/Customer/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
            });

            verifyNestedResourceInfoActionStack.Push((nestedResourceInfo) =>
            {
                Assert.NotNull(nestedResourceInfo);
                Assert.Equal("ShippingAddress", nestedResourceInfo.Name);
                Assert.Null(nestedResourceInfo.Url);
                Assert.Null(nestedResourceInfo.AssociationLinkUrl);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotEmpty(verifyNestedResourceInfoActionStack);
                        var innerVerifyNestedResourceInfoAction = verifyNestedResourceInfoActionStack.Pop();

                        innerVerifyNestedResourceInfoAction(nestedResourceInfo);
                    }));

            Assert.Empty(verifyResourceActionStack);
            Assert.Empty(verifyNestedResourceInfoActionStack);
        }

        [Fact]
        public async Task ReadV401NestedDeltaResourceSetAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Orders@delta\":[{\"Id\":1,\"Amount\":100}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                    },
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    }),
                readingDelta: true);
        }

        [Fact]
        public async Task ReadDeltaLinkAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"value\":[" +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$link\"," +
                "\"source\":\"http://tempuri.org/Orders(1)\"," +
                "\"relationship\":\"Customer\"," +
                "\"target\":\"http://tempuri.org/Customers(1)\"}]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                    },
                    verifyDeltaLinkAction: (deltaLink) =>
                    {
                        Assert.NotNull(deltaLink.Source);
                        Assert.Equal("http://tempuri.org/Orders(1)", deltaLink.Source.AbsoluteUri);
                        Assert.Equal("Customer", deltaLink.Relationship);
                        Assert.NotNull(deltaLink.Target);
                        Assert.Equal("http://tempuri.org/Customers(1)", deltaLink.Target.AbsoluteUri);
                    }),
                readingResourceSet: true,
                readingDelta: true);
        }

        [Fact]
        public async Task ReadDeltaDeletedLinkAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"value\":[" +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$deletedLink\"," +
                "\"source\":\"http://tempuri.org/Orders(1)\"," +
                "\"relationship\":\"Customer\"," +
                "\"target\":\"http://tempuri.org/Customers(1)\"}]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyDeltaResourceSetAction: (deltaResourceSet) =>
                    {
                        Assert.NotNull(deltaResourceSet);
                    },
                    verifyDeltaLinkAction: (deltaDeletedLink) =>
                    {
                        Assert.NotNull(deltaDeletedLink.Source);
                        Assert.Equal("http://tempuri.org/Orders(1)", deltaDeletedLink.Source.AbsoluteUri);
                        Assert.Equal("Customer", deltaDeletedLink.Relationship);
                        Assert.NotNull(deltaDeletedLink.Target);
                        Assert.Equal("http://tempuri.org/Customers(1)", deltaDeletedLink.Target.AbsoluteUri);
                    }),
                readingResourceSet: true,
                readingDelta: true);
        }

        [Fact]
        public async Task ReadResourceWithStreamPropertyAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Photo@odata.mediaEditLink\":\"http://tempuri.org/Customers(1)/Photo\"," +
                "\"Photo@odata.mediaReadLink\":\"http://tempuri.org/Customers(1)/Photo\"," +
                "\"Photo@odata.mediaContentType\":\"image/png\"," +
                "\"Photo@odata.mediaEtag\":\"media-etag\"," +
                "\"Photo\":\"AQIDBAUGBwgJAA==\"}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyNestedPropertyInfoAction: (item) =>
                    {
                        ODataStreamPropertyInfo streamPropertyInfo = Assert.IsType<ODataStreamPropertyInfo>(item);
                        Assert.NotNull(streamPropertyInfo);
                        Assert.NotNull(streamPropertyInfo.EditLink);
                        Assert.Equal("http://tempuri.org/Customers(1)/Photo", streamPropertyInfo.EditLink.AbsoluteUri);
                        Assert.NotNull(streamPropertyInfo.ReadLink);
                        Assert.Equal("http://tempuri.org/Customers(1)/Photo", streamPropertyInfo.ReadLink.AbsoluteUri);
                        Assert.Equal("image/png", streamPropertyInfo.ContentType);
                        Assert.Equal("media-etag", streamPropertyInfo.ETag);
                    },
                    verifyBinaryStreamAction: async (binaryStream) =>
                    {
                        var maxLength = 10;
                        var buffer = new byte[maxLength];

                        var bytesRead = await binaryStream.ReadAsync(buffer, 0, maxLength);
                        Assert.Equal(bytesRead, maxLength);
                        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, buffer);
                    }));
        }

        [Fact]
        public async Task ReadResourceWithStringPropertyReadAsStreamAsync()
        {
            this.messageReaderSettings.ReadAsStreamFunc =
                (primitiveType, isCollection, propertyName, edmProperty) => propertyName.Equals("Name");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyNestedPropertyInfoAction: (item) =>
                    {
                        ODataStreamPropertyInfo streamPropertyInfo = item as ODataStreamPropertyInfo;
                        Assert.Null(streamPropertyInfo);
                    },
                    verifyTextStreamAction: async (textReader) =>
                    {
                        var result = await textReader.ReadToEndAsync();
                        Assert.Equal("Sue", result);
                    }));
        }

        [Fact]
        public async Task ReadResourceWithStringPropertySetToNullReadAsStreamAsync()
        {
            this.messageReaderSettings.ReadAsStreamFunc =
                (primitiveType, isCollection, propertyName, edmProperty) => propertyName.Equals("Name");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":null}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyNestedPropertyInfoAction: (item) =>
                    {
                        ODataStreamPropertyInfo streamPropertyInfo = item as ODataStreamPropertyInfo;
                        Assert.Null(streamPropertyInfo);
                    },
                    verifyTextStreamAction: async (textReader) =>
                    {
                        var result = await textReader.ReadToEndAsync();
                        Assert.Equal("", result);
                    }));
        }

        [Fact]
        public async Task ReadEntityReferenceLinkInRequestPayloadAsync()
        {
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"Customer\":{\"@odata.id\":\"Customers(1)\",\"@Is.EntityReferenceLink\":true}}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);

                Assert.Equal("Customers(1)", resource.Id?.OriginalString);
                var instanceAnnotation = Assert.Single(resource.InstanceAnnotations);
                Assert.Equal("Is.EntityReferenceLink", instanceAnnotation.Name);
                var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                Assert.Equal(true, annotationValue.Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
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
        public async Task ReadEntityReferenceLinksInRequestPayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Type\":\"Retail\"," +
                "\"Orders@odata.bind\":[\"http://tempuri.org/Orders(1)\",\"http://tempuri.org/Orders(2)\"]}";

            var entityReferenceLinksStack = new Stack<string>();
            entityReferenceLinksStack.Push("http://tempuri.org/Orders(2)");
            entityReferenceLinksStack.Push("http://tempuri.org/Orders(1)");

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyEntityReferenceLinkAction: (entityReferenceLink) =>
                    {
                        Assert.NotNull(entityReferenceLink);
                        Assert.NotEmpty(entityReferenceLinksStack);
                        var entityReferenceLinkUrl = entityReferenceLinksStack.Pop();
                        Assert.Equal(entityReferenceLinkUrl, entityReferenceLink.Url.AbsoluteUri);
                    }),
                isResponse: false);

            Assert.Empty(entityReferenceLinksStack);
        }

        [Fact]
        public async Task ReadV401EntityReferenceLinksInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;
            this.messageReaderSettings.ShouldIncludeAnnotation = ODataUtils.CreateAnnotationFilter("*");

            var payload = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Orders\":[" +
                "{\"@id\":\"Orders(1)\",\"@Is.EntityReferenceLink\":true}," +
                "{\"@id\":\"Orders(2)\",\"@Is.EntityReferenceLink\":true}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);

                Assert.Equal("Orders(2)", resource.Id.OriginalString);
                var instanceAnnotation = Assert.Single(resource.InstanceAnnotations);
                Assert.Equal("Is.EntityReferenceLink", instanceAnnotation.Name);
                var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                Assert.Equal(true, annotationValue.Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);

                Assert.Equal("Orders(1)", resource.Id.OriginalString);
                var instanceAnnotation = Assert.Single(resource.InstanceAnnotations);
                Assert.Equal("Is.EntityReferenceLink", instanceAnnotation.Name);
                var annotationValue = Assert.IsType<ODataPrimitiveValue>(instanceAnnotation.Value);
                Assert.Equal(true, annotationValue.Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
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
        public async Task ReadV6ContainmentAsync()
        {
            this.messageReaderSettings.LibraryCompatibility = ODataLibraryCompatibility.Version6;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Pencil\"}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Product", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Pencil", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    }));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadV7ContainmentAsync()
        {
            this.messageReaderSettings.LibraryCompatibility = ODataLibraryCompatibility.Version7;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"Products@odata.context\":\"http://tempuri.org/$metadata#Orders(1)/Products\"," +
                "\"Products\":[{\"Id\":1,\"Name\":\"Pencil\"}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Product", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Pencil", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    }));

            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadDeltaResourceAsync_ForNavigationSourceNotMatchedToParentDeltaResourceSet()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"value\":[" +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\",\"Id\":1,\"Name\":\"Sue\"}]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("NS.Customer", resource.TypeName);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(2, properties.Length);

                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                    }),
                readingResourceSet: true,
                readingDelta: true);
        }

        [Fact]
        public async Task ReadNestedResourceInRequestPayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"Amount\":100," +
                "\"Customer\":{\"Id\":1,\"Name\":\"Sue\"}}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.orderEntitySet,
                this.orderEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    }),
                readingResourceSet: true,
                isResponse: false);
        }

        [Fact]
        public async Task ReadNestedResourceSetInRequestPayloadAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"value\":[{" +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Orders\":[{\"Id\":1,\"Amount\":100}]}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(100M, properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);
                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();

                        innerVerifyResourceAction(resource);
                    }),
                readingResourceSet: true,
                isResponse: false);
        }

        [Fact]
        public async Task ReadV401NestedDeltaResourceSetInRequestPayloadAsync()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Orders@delta\":[{\"Id\":1,\"Amount\":130}]}";

            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Order", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Amount", properties[1].Name);
                Assert.Equal(130M, properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
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
        public async Task ReadDeferredNestedResourceInfoAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Orders@odata.navigationLink\":\"http://tempuri.org/Customers(1)/Orders\"}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("NS.Customer", resource.TypeName);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(2, properties.Length);

                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                    },
                    verifyNestedResourceInfoAction: (nestedResourceInfo) =>
                    {
                        Assert.NotNull(nestedResourceInfo);
                        Assert.Equal("Orders", nestedResourceInfo.Name);
                        Assert.NotNull(nestedResourceInfo.Url);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders", nestedResourceInfo.Url.AbsoluteUri);
                        Assert.NotNull(nestedResourceInfo.AssociationLinkUrl);
                        Assert.Equal("http://tempuri.org/Customers(1)/Orders/$ref", nestedResourceInfo.AssociationLinkUrl.AbsoluteUri);
                    }));
        }

        [Fact]
        public async Task ReadStringCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"UndeclaredCollectionProp@odata.type\":\"#Collection(Edm.String)\"," +
                "\"UndeclaredCollectionProp\":[\"Foo\",\"Bar\",null]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("NS.Customer", resource.TypeName);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(3, properties.Length);

                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                        Assert.Equal("UndeclaredCollectionProp", properties[2].Name);
                        var collectionValue = Assert.IsType<ODataCollectionValue>(properties[2].Value);
                        var collectionItems = collectionValue.Items.ToArray();
                        Assert.Equal("Foo", collectionItems[0]);
                        Assert.Equal("Bar", collectionItems[1]);
                        Assert.Null(collectionItems[2]);
                    }));
        }

        [Fact]
        public async Task ReadStringCollectionAsStreamAsync()
        {
            this.messageReaderSettings.ReadAsStreamFunc =
                (primitiveType, isCollection, propertyName, edmProperty) => true;

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"UndeclaredCollectionProp@odata.type\":\"#Collection(Edm.String)\"," +
                "\"UndeclaredCollectionProp\":[\"Foo\",\"Bar\",null]}";

            var readAsStreamStack = new Stack<string>(new[] { "", "Bar", "Foo", "Sue" });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        var idProperty = Assert.Single<ODataProperty>(resource.Properties);

                        Assert.Equal("Id", idProperty.Name);
                        Assert.Equal(1, idProperty.Value);
                    },
                    verifyTextStreamAction: async (textReader) =>
                    {
                        Assert.NotEmpty(readAsStreamStack);
                        var readAsStream = readAsStreamStack.Pop();

                        var result = await textReader.ReadToEndAsync();
                        Assert.Equal(readAsStream, result);
                    }));

            Assert.Empty(readAsStreamStack);
        }

        [Fact]
        public async Task ReadEnumCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"EnumCollectionProp@odata.type\":\"#Collection(NS.CustomerType)\"," +
                "\"EnumCollectionProp\":[\"Retail\",\"Wholesale\"]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("NS.Customer", resource.TypeName);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(3, properties.Length);

                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                        Assert.Equal("EnumCollectionProp", properties[2].Name);
                        var collectionValue = Assert.IsType<ODataCollectionValue>(properties[2].Value);
                        var collectionItems = collectionValue.Items.ToArray();
                        Assert.Equal(2, collectionItems.Length);
                        Assert.Equal("Retail", Assert.IsType<ODataEnumValue>(collectionItems[0]).Value);
                        Assert.Equal("Wholesale", Assert.IsType<ODataEnumValue>(collectionItems[1]).Value);
                    }));
        }

        [Fact]
        public async Task ReadStreamCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"UndeclaredCollectionProp@odata.type\":\"#Collection(Edm.Stream)\",\"UndeclaredCollectionProp\":[\"AQIDBAUGBwgJAA==\"]}";

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotNull(resource);
                        Assert.Equal("NS.Customer", resource.TypeName);
                        var properties = resource.Properties.ToArray();
                        Assert.Equal(2, properties.Length);

                        Assert.Equal("Id", properties[0].Name);
                        Assert.Equal(1, properties[0].Value);
                        Assert.Equal("Name", properties[1].Name);
                        Assert.Equal("Sue", properties[1].Value);
                    },
                    verifyBinaryStreamAction: async (binaryStream) =>
                    {
                        var maxLength = 10;
                        var buffer = new byte[maxLength];

                        var bytesRead = await binaryStream.ReadAsync(buffer, 0, maxLength);
                        Assert.Equal(bytesRead, maxLength);
                        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }, buffer);
                    }));
        }

        [Fact]
        public async Task ReadUntypedCollectionAsync()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"UndeclaredCollectionProp@odata.type\":\"#Collection(Edm.Untyped)\"," +
                "\"UndeclaredCollectionProp\":[" +
                "\"www.foobar.com\"," +
                "\"AQIDBAUGBwgJAA==\"," +
                "{\"Name\":\"Joe\",\"Age@odata.type\":\"#Edm.Int32\",\"Age\":17}," +
                "{\"@odata.type\":\"#NS.Product\",\"Id\":7,\"Name\":\"Coffee\"}," +
                "null," +
                "[{\"@odata.type\":\"#NS.Customer\",\"Id\":13,\"Name\":\"Joe\"}]," +
                "true," +
                "false," +
                "-13.7]}";

            var primitiveValueStack = new Stack<object>(new object[] { -13.7M, false, true, "AQIDBAUGBwgJAA==", "www.foobar.com" });
            var verifyResourceActionStack = new Stack<Action<ODataResource>>();

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(13, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Joe", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) => Assert.Null(resource));
            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Product", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(7, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Coffee", properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Name", properties[0].Name);
                var untypedValue = Assert.IsType<ODataUntypedValue>(properties[0].Value);
                Assert.Equal("\"Joe\"", untypedValue.RawValue);
                Assert.Equal("Age", properties[1].Name);
                Assert.Equal(17, properties[1].Value);
            });

            verifyResourceActionStack.Push((resource) =>
            {
                Assert.NotNull(resource);
                Assert.Equal("NS.Customer", resource.TypeName);
                var properties = resource.Properties.ToArray();
                Assert.Equal(2, properties.Length);

                Assert.Equal("Id", properties[0].Name);
                Assert.Equal(1, properties[0].Value);
                Assert.Equal("Name", properties[1].Name);
                Assert.Equal("Sue", properties[1].Value);
            });

            await SetupJsonLightReaderAndRunTestAsync(
                payload,
                this.customerEntitySet,
                this.customerEntityType,
                (jsonLightReader) => DoReadAsync(
                    jsonLightReader,
                    verifyResourceAction: (resource) =>
                    {
                        Assert.NotEmpty(verifyResourceActionStack);

                        var innerVerifyResourceAction = verifyResourceActionStack.Pop();
                        innerVerifyResourceAction(resource);
                    },
                    verifyPrimitiveAction: (primitiveValue) =>
                    {
                        Assert.NotEmpty(primitiveValueStack);

                        var expectedPrimitiveValue = primitiveValueStack.Pop();
                        Assert.Equal(expectedPrimitiveValue, primitiveValue.Value);
                    }));

            Assert.Empty(primitiveValueStack);
            Assert.Empty(verifyResourceActionStack);
        }

        [Fact]
        public async Task ReadNestedResourceSetAsync_ThrowsExceptionForInvalidItemsInResourceSet()
        {
            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Type\":\"Retail\"," +
                "\"Orders\":[\"Foo\",\"Bar\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightReaderAndRunTestAsync(
                    payload,
                    this.customerEntitySet,
                    this.customerEntityType,
                    (jsonLightReader) => DoReadAsync(jsonLightReader)));

            Assert.Equal(ErrorStrings.ODataJsonReader_CannotReadResourcesOfResourceSet("PrimitiveValue"),
                exception.Message);
        }

        [Fact]
        public async Task ReadV401NestedDeltaResourceSetAsync_ThrowsExceptionForInvalidItemsInResourceSet()
        {
            this.messageReaderSettings.Version = ODataVersion.V401;

            var payload = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"Type\":\"Retail\"," +
                "\"Orders@delta\":[\"Foo\",\"Bar\"]}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightReaderAndRunTestAsync(
                    payload,
                    this.customerEntitySet,
                    this.customerEntityType,
                    (jsonLightReader) => DoReadAsync(jsonLightReader)));

            Assert.Equal(ErrorStrings.ODataJsonLightResourceDeserializer_InvalidNodeTypeForItemsInResourceSet("PrimitiveValue"),
                exception.Message);
        }

        [Fact]
        public async Task ReadNestedResourceAsync_ThrowsExceptionForNonNullableStructuralPropertyAsNull()
        {
            this.customerEntityType.AddStructuralProperty("BillingAddress", new EdmComplexTypeReference(this.addressComplexType, false));

            var payload = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"Id\":1," +
                "\"Name\":\"Sue\"," +
                "\"BillingAddress\":null}";

            var exception = await Assert.ThrowsAsync<ODataException>(
                () => SetupJsonLightReaderAndRunTestAsync(
                    payload,
                    this.customerEntitySet,
                    this.customerEntityType,
                    (jsonLightReader) => DoReadAsync(jsonLightReader)));

            Assert.Equal(
                ErrorStrings.ReaderValidationUtils_NullNamedValueForNonNullableType("BillingAddress", "NS.Address"),
                exception.Message);
        }

        private async Task DoReadAsync(
            ODataJsonLightReader jsonLightReader,
            Action<ODataResourceSet> verifyResourceSetAction = null,
            Action<ODataResource> verifyResourceAction = null,
            Action<ODataNestedResourceInfo> verifyNestedResourceInfoAction = null,
            Action<ODataDeltaResourceSet> verifyDeltaResourceSetAction = null,
            Action<ODataDeletedResource> verifyDeletedResourceAction = null,
            Action<ODataDeltaLinkBase> verifyDeltaLinkAction = null,
            Action<ODataEntityReferenceLink> verifyEntityReferenceLinkAction = null,
            Action<ODataItem> verifyNestedPropertyInfoAction = null,
            Action<Stream> verifyBinaryStreamAction = null,
            Action<TextReader> verifyTextStreamAction = null,
            Action<ODataPrimitiveValue> verifyPrimitiveAction = null)
        {
            while (await jsonLightReader.ReadAsync())
            {
                switch (jsonLightReader.State)
                {
                    case ODataReaderState.ResourceSetStart:
                        if (verifyResourceSetAction != null)
                        {
                            verifyResourceSetAction(jsonLightReader.Item as ODataResourceSet);
                        }

                        break;
                    case ODataReaderState.ResourceSetEnd:
                        break;
                    case ODataReaderState.ResourceStart:
                        if (verifyResourceAction != null)
                        {
                            verifyResourceAction(jsonLightReader.Item as ODataResource);
                        }

                        break;
                    case ODataReaderState.ResourceEnd:
                        break;
                    case ODataReaderState.NestedResourceInfoStart:
                        if (verifyNestedResourceInfoAction != null)
                        {
                            verifyNestedResourceInfoAction(jsonLightReader.Item as ODataNestedResourceInfo);
                        }

                        break;
                    case ODataReaderState.NestedResourceInfoEnd:
                        break;
                    case ODataReaderState.DeltaResourceSetStart:
                        if (verifyDeltaResourceSetAction != null)
                        {
                            verifyDeltaResourceSetAction(jsonLightReader.Item as ODataDeltaResourceSet);
                        }

                        break;
                    case ODataReaderState.DeltaResourceSetEnd:
                        break;
                    case ODataReaderState.DeletedResourceStart:
                        if (verifyDeletedResourceAction != null)
                        {
                            verifyDeletedResourceAction(jsonLightReader.Item as ODataDeletedResource);
                        }

                        break;
                    case ODataReaderState.DeletedResourceEnd:
                        break;
                    case ODataReaderState.Primitive:
                        if (verifyPrimitiveAction != null)
                        {
                            verifyPrimitiveAction(jsonLightReader.Item as ODataPrimitiveValue);
                        }

                        break;
                    case ODataReaderState.DeltaLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonLightReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.DeltaDeletedLink:
                        if (verifyDeltaLinkAction != null)
                        {
                            verifyDeltaLinkAction(jsonLightReader.Item as ODataDeltaLinkBase);
                        }

                        break;
                    case ODataReaderState.EntityReferenceLink:
                        if (verifyEntityReferenceLinkAction != null)
                        {
                            verifyEntityReferenceLinkAction(jsonLightReader.Item as ODataEntityReferenceLink);
                        }

                        break;
                    case ODataReaderState.Stream:
                        var streamItem = jsonLightReader.Item as ODataStreamItem;
                        Assert.NotNull(streamItem);

                        if (streamItem.PrimitiveTypeKind == EdmPrimitiveTypeKind.String)
                        {
                            using (var textReader = await jsonLightReader.CreateTextReaderAsync())
                            {
                                if (verifyTextStreamAction != null)
                                {
                                    verifyTextStreamAction(textReader);
                                }
                            }
                        }
                        else
                        {
                            using (var stream = await jsonLightReader.CreateReadStreamAsync())
                            {
                                if (verifyBinaryStreamAction != null)
                                {
                                    verifyBinaryStreamAction(stream);
                                }
                            }
                        }

                        break;
                    case ODataReaderState.NestedProperty:
                        if (verifyNestedPropertyInfoAction != null)
                        {
                            verifyNestedPropertyInfoAction(jsonLightReader.Item);
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        private ODataJsonLightInputContext CreateJsonLightInputContext(string payload, bool isAsync = false, bool isResponse = true)
        {
            var messageInfo = new ODataMessageInfo
            {
                MediaType = new ODataMediaType("application", "json"),
#if NETCOREAPP1_1
                Encoding = Encoding.GetEncoding(0),
#else
                Encoding = Encoding.Default,
#endif
                IsResponse = isResponse,
                IsAsync = isAsync,
                Model = this.model
            };

            return new ODataJsonLightInputContext(new StringReader(payload), messageInfo, this.messageReaderSettings);
        }

        /// <summary>
        /// Sets up an ODataJsonLightReader, then runs the given test code asynchronously
        /// </summary>
        private async Task SetupJsonLightReaderAndRunTestAsync(
            string payload,
            IEdmNavigationSource navigationSource,
            IEdmStructuredType expectedResourceType,
            Func<ODataJsonLightReader, Task> func,
            bool readingResourceSet = false,
            bool readingParameter = false,
            bool readingDelta = false,
            bool isResponse = true,
            IODataReaderWriterListener readerWriterListener = null)
        {
            using (var jsonLightInputContext = CreateJsonLightInputContext(payload, isAsync: true, isResponse: isResponse))
            {
                var jsonLightReader = new ODataJsonLightReader(
                    jsonLightInputContext,
                    navigationSource,
                    expectedResourceType,
                    readingResourceSet,
                    readingParameter,
                    readingDelta,
                    listener: readerWriterListener);

                await func(jsonLightReader);
            }
        }

        private void InitializeModel()
        {
            this.model = new EdmModel();

            this.customerTypeEnumType = new EdmEnumType("NS", "CustomerType");
            this.addressComplexType = new EdmComplexType("NS", "Address", baseType: null, isAbstract: false, isOpen: true);
            this.orderEntityType = new EdmEntityType("NS", "Order");
            this.customerEntityType = new EdmEntityType("NS", "Customer", baseType: null, isAbstract: false, isOpen: true, hasStream: true);
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
            this.customerEntityType.AddStructuralProperty("Photo", EdmPrimitiveTypeKind.Stream);
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

            var rateCustomersAction = new EdmAction(namespaceName: "NS", name: "RateCustomers", returnType: null, isBound: true, entitySetPathExpression: null);
            rateCustomersAction.AddParameter("bindingParameter", new EdmEntityTypeReference(this.customerEntityType, true));
            model.AddElement(rateCustomersAction);

            var top5CustomersFunction = new EdmFunction(
                namespaceName: "NS",
                name: "Top5Customers",
                returnType: new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.customerEntityType, true))),
                isBound: true,
                entitySetPathExpression: null,
                isComposable: false);
            top5CustomersFunction.AddParameter("bindingParameter", new EdmEntityTypeReference(this.customerEntityType, true));
            this.model.AddElement(top5CustomersFunction);

            var cancelOrderAction = new EdmAction(namespaceName: "NS", name: "CancelOrder", returnType: null, isBound: true, entitySetPathExpression: null);
            cancelOrderAction.AddParameter("bindingParameter", new EdmEntityTypeReference(this.customerEntityType, true));
            cancelOrderAction.AddParameter("orderId", EdmCoreModel.Instance.GetInt32(false));
            model.AddElement(cancelOrderAction);

            var mostRecentOrderFunction = new EdmFunction(
                namespaceName: "NS",
                name: "MostRecentOrder",
                returnType: new EdmEntityTypeReference(this.customerEntityType, true),
                isBound: true,
                entitySetPathExpression: null,
                isComposable: false);
            mostRecentOrderFunction.AddParameter("bindParameter", new EdmEntityTypeReference(this.customerEntityType, true));
            this.model.AddElement(mostRecentOrderFunction);

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
    }
}
