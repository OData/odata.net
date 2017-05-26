//---------------------------------------------------------------------
// <copyright file="RequestDescriptionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using AstoriaUnitTests.Tests.Server.Simulators;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RequestDescriptionTests
    {
        [TestMethod]
        public void RequestDescriptionResponseFormat_Metadata()
        {
            RunSimpleFormatTest(RequestTargetKind.Metadata, ODataFormat.Metadata);
        }

        [TestMethod]
        public void RequestDescriptionResponseFormat_Batch()
        {
            RunSimpleFormatTest(RequestTargetKind.Batch, ODataFormat.Batch);
        }

        [TestMethod]
        public void RequestDescriptionResponseFormat_RawValue()
        {
            RunSimpleFormatTest(RequestTargetKind.PrimitiveValue, ODataFormat.RawValue);
            RunSimpleFormatTest(RequestTargetKind.MediaResource, ODataFormat.RawValue);
            RunSimpleFormatTest(RequestTargetKind.OpenPropertyValue, ODataFormat.RawValue);
        }

        [TestMethod]
        public void RequestDescriptionResponseFormat_VoidOperation()
        {
            RunSimpleFormatTest(RequestTargetKind.VoidOperation, null);
        }

        [TestMethod]
        public void RequestDescriptionResponseFormat_JsonLite()
        {
            RunNegotiatedFormatTest("application/json", "4.0", ODataProtocolVersion.V4, ODataFormat.Json);
            RunNegotiatedFormatTest("application/json", null, ODataProtocolVersion.V4, ODataFormat.Json);
            RunNegotiatedFormatTest(null, null, ODataProtocolVersion.V4, ODataFormat.Json);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_Property()
        {
            RunPayloadKindTest(RequestTargetKind.Nothing, RequestTargetSource.Property, null, false, false, ODataPayloadKind.Property);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_Metadata()
        {
            RunPayloadKindTest(RequestTargetKind.Metadata, RequestTargetSource.None, null, false, false, ODataPayloadKind.MetadataDocument);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_ServiceDoc()
        {
            RunPayloadKindTest(RequestTargetKind.ServiceDirectory, RequestTargetSource.None, null, false, false, ODataPayloadKind.ServiceDocument);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_Batch()
        {
            RunPayloadKindTest(RequestTargetKind.Batch, RequestTargetSource.None, null, false, false, ODataPayloadKind.Batch);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_MediaResource()
        {
            RunPayloadKindTest(RequestTargetKind.MediaResource, RequestTargetSource.None, null, false, false, ODataPayloadKind.BinaryValue);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_BinaryValue()
        {
            RunPayloadKindTest(RequestTargetKind.PrimitiveValue, RequestTargetSource.None, ResourceType.GetPrimitiveResourceType(typeof(byte[])), false, false, ODataPayloadKind.BinaryValue);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_RawValue()
        {
            RunPayloadKindTest(RequestTargetKind.PrimitiveValue, RequestTargetSource.None, ResourceType.GetPrimitiveResourceType(typeof(string)), false, false, ODataPayloadKind.Value);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_ServiceOperation()
        {
            RunPayloadKindTest(RequestTargetKind.Primitive, RequestTargetSource.ServiceOperation, null, true, false, ODataPayloadKind.Property);
            RunPayloadKindTest(RequestTargetKind.ComplexObject, RequestTargetSource.ServiceOperation, null, true, false, ODataPayloadKind.Property);
            RunPayloadKindTest(RequestTargetKind.Collection, RequestTargetSource.ServiceOperation, null, true, false, ODataPayloadKind.Property);
            RunPayloadKindTest(RequestTargetKind.Primitive, RequestTargetSource.ServiceOperation, null, false, false, ODataPayloadKind.Collection);
            RunPayloadKindTest(RequestTargetKind.ComplexObject, RequestTargetSource.ServiceOperation, null, false, false, ODataPayloadKind.Collection);
            RunPayloadKindTest(RequestTargetKind.Collection, RequestTargetSource.ServiceOperation, null, false, false, ODataPayloadKind.Collection);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_Links()
        {
            RunPayloadKindTest(RequestTargetKind.Resource, RequestTargetSource.EntitySet, null, true, true, ODataPayloadKind.EntityReferenceLink);
            RunPayloadKindTest(RequestTargetKind.Resource, RequestTargetSource.EntitySet, null, false, true, ODataPayloadKind.EntityReferenceLinks);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_Resource()
        {
            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "fake", "fake", false);
            RunPayloadKindTest(RequestTargetKind.Resource, RequestTargetSource.EntitySet, resourceType, true, false, ODataPayloadKind.Resource);
            RunPayloadKindTest(RequestTargetKind.Resource, RequestTargetSource.EntitySet, resourceType, false, false, ODataPayloadKind.ResourceSet);

            resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityCollection, "fake", "fake");
            RunPayloadKindTest(RequestTargetKind.Resource, RequestTargetSource.EntitySet, resourceType, false, false, ODataPayloadKind.ResourceSet);
        }

        [TestMethod]
        public void RequestDescriptionPayloadKind_VoidOperation()
        {
            RunPayloadKindTest(RequestTargetKind.VoidOperation, RequestTargetSource.EntitySet, null, false, false, ODataPayloadKind.Unsupported);
        }

        private static void RunSimpleFormatTest(RequestTargetKind requestTargetKind, ODataFormat expectedFormat)
        {
            var requestDescription = new RequestDescription(requestTargetKind, RequestTargetSource.None, new Uri("http://temp.org/"));
            requestDescription.DetermineResponseFormat(new DataServiceSimulator());
            if (expectedFormat == null)
            {
                requestDescription.ResponseFormat.Should().BeNull();
            }
            else
            {
                requestDescription.ResponseFormat.Should().NotBeNull();
                requestDescription.ResponseFormat.Format.Should().BeSameAs(expectedFormat);
            }
        }

        private static void RunNegotiatedFormatTest(string requestAccept, string requestMaxVersion, Microsoft.OData.Client.ODataProtocolVersion maxProtocolVersion, ODataFormat expectedFormat)
        {
            DataServiceHostSimulator host = new DataServiceHostSimulator
            {
                RequestHttpMethod = "GET",
                RequestAccept = requestAccept,
                RequestMaxVersion = requestMaxVersion,
                RequestVersion = "4.0",
            };
            DataServiceSimulator service = new DataServiceSimulator
            {
                OperationContext = new DataServiceOperationContext(host),
                Configuration = new DataServiceConfiguration(new DataServiceProviderSimulator()),
            };

            service.Configuration.DataServiceBehavior.MaxProtocolVersion = maxProtocolVersion;
            service.OperationContext.InitializeAndCacheHeaders(service);
            service.OperationContext.RequestMessage.InitializeRequestVersionHeaders(VersionUtil.ToVersion(maxProtocolVersion));

            var d = new RequestDescription(RequestTargetKind.Primitive, RequestTargetSource.Property, new Uri("http://temp.org/"));

            d.DetermineWhetherResponseBodyOrETagShouldBeWritten(service.OperationContext.RequestMessage.HttpVerb);
            d.DetermineWhetherResponseBodyShouldBeWritten(service.OperationContext.RequestMessage.HttpVerb);
            d.DetermineResponseFormat(service);
            d.ResponseFormat.Should().NotBeNull();
            d.ResponseFormat.Format.Should().BeSameAs(expectedFormat);
        }

        private static void RunPayloadKindTest(RequestTargetKind requestTargetKind, RequestTargetSource requestTargetSource, ResourceType targetResourceType, bool singleResult, bool isLinkUri, ODataPayloadKind expectedKind)
        {
            var segment = new SegmentInfo
            {
                TargetKind = requestTargetKind,
                TargetSource = requestTargetSource,
                TargetResourceType = targetResourceType,
                SingleResult = singleResult,
                Identifier = "Fake",
            };

            var operation = new ServiceOperation("Fake", ServiceOperationResultKind.Void, null, null, "GET", null);
            operation.SetReadOnly();
            segment.Operation = new OperationWrapper(operation);
            segment.ProjectedProperty = new ResourceProperty("Fake", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int)));

            SegmentInfo[] segmentInfos;
            if (isLinkUri)
            {
                segmentInfos = new[]
                {
                    new SegmentInfo(),
                    new SegmentInfo
                    {
                       TargetKind = RequestTargetKind.Link,
                    },
                    segment
                };
            }
            else
            {
                segmentInfos = new[]
                {
                    new SegmentInfo
                    {
                        Identifier = "Fake",
                    },
                    new SegmentInfo(),
                    segment
                };
            }

            var requestDescription = new RequestDescription(segmentInfos, new Uri("http://temp.org/"));
            requestDescription.ResponsePayloadKind.Should().Be(expectedKind);
        }
    }
}
