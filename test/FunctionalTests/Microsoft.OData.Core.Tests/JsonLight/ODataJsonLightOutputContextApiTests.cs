//---------------------------------------------------------------------
// <copyright file="ODataJsonLightOutputContextApiTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests.JsonLight
{
    public class ODataJsonLightOutputContextApiTests
    {
        private const string ServiceUri = "http://tempuri.org";
        private const string batchBoundary = "batch_aed653ab";// Regex used to replace the random batch boundary before equivalence check
        private const string batchGuidRegex = @"batch[a-z]*_[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}";
        private EdmModel model;
        private MemoryStream asyncStream;
        private MemoryStream syncStream;
        private ODataMessageWriterSettings writerSettings;

        private EdmEntityType superEntityType;
        private EdmEntityType orderEntityType;
        private EdmEntityType customerEntityType;
        private EdmEntityType orderItemEntityType;
        private EdmComplexType coordinateComplexType;
        private EdmEnumType colorEnumType;
        private EdmEntitySet superEntitySet;
        private EdmEntitySet orderEntitySet;
        private EdmEntitySet customerEntitySet;
        private EdmEntityType streamEntityType;
        private EdmEntitySet streamEntitySet;

        public ODataJsonLightOutputContextApiTests()
        {
            InitializeEdmModel();
            this.asyncStream = new MemoryStream();
            this.syncStream = new MemoryStream();
            this.writerSettings = new ODataMessageWriterSettings
            {
                Version = ODataVersion.V4,
                EnableMessageStreamDisposal = false,
                BaseUri = new Uri(ServiceUri),
                ODataUri = new ODataUri { ServiceRoot = new Uri(ServiceUri) }
            };
        }

        [Fact]
        public async Task WriteResourceSet_APIsShouldYieldSameResult()
        {
            this.writerSettings.ODataUri.Path = new ODataUriParser(this.model, new Uri(ServiceUri), new Uri(ServiceUri + "/Orders(1)/Items")).ParsePath();

            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");
            orderResourceSet.Count = 5;
            // Not allowed to set both NextPageLink and DeltaLink
            orderResourceSet.NextPageLink = new Uri($"{ServiceUri}/Orders/nextLink");
            orderResourceSet.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Orders"), "ResourceSetAction1"));
            orderResourceSet.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Orders"), "ResourceSetAction2"));
            orderResourceSet.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Orders"), "ResourceSetFunction1"));
            orderResourceSet.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Orders"), "ResourceSetFunction2"));
            orderResourceSet.InstanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.ResourceSet", new ODataPrimitiveValue(true))
            };

            var orderResource = CreateOrderResource();
            orderResource.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Orders(1)"), "ResourceAction1"));
            orderResource.AddAction(CreateODataAction(new Uri($"{ServiceUri}/Orders(1)"), "ResourceAction2"));
            orderResource.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Orders(1)"), "ResourceFunction1"));
            orderResource.AddFunction(CreateODataFunction(new Uri($"{ServiceUri}/Orders(1)"), "ResourceFunction2"));
            orderResource.InstanceAnnotations = new List<ODataInstanceAnnotation>
            {
                new ODataInstanceAnnotation("Is.Resource", new ODataPrimitiveValue(true))
            };

            var orderItemResourceSet = CreateResourceSet("Orders", "NS.OrderItem", EdmNavigationSourceKind.ContainedEntitySet);
            var orderItemResource = CreateOrderItemResource();
            var customerResource = CreateCustomerResource();
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");
            var orderItemsPropertyNestedResourceInfo = CreateNestedResourceInfo("Items", isCollection: true);

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(orderResourceSet);
                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                await writer.WriteStartAsync(customerResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(orderItemsPropertyNestedResourceInfo);
                await writer.WriteStartAsync(orderItemResourceSet);
                await writer.WriteStartAsync(orderItemResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceSetWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(orderResourceSet);
                        writer.WriteStart(orderResource);
                        writer.WriteStart(customerPropertyNestedResourceInfo);
                        writer.WriteStart(customerResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteStart(orderItemsPropertyNestedResourceInfo);
                        writer.WriteStart(orderItemResourceSet);
                        writer.WriteStart(orderItemResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders\"," +
                "\"#Action\":[" +
                "{\"title\":\"ResourceSetAction1\",\"target\":\"http://tempuri.org/Orders/ResourceSetAction1\"}," +
                "{\"title\":\"ResourceSetAction2\",\"target\":\"http://tempuri.org/Orders/ResourceSetAction2\"}]," +
                "\"#Function\":[" +
                "{\"title\":\"ResourceSetFunction1\",\"target\":\"http://tempuri.org/Orders/ResourceSetFunction1\"}," +
                "{\"title\":\"ResourceSetFunction2\",\"target\":\"http://tempuri.org/Orders/ResourceSetFunction2\"}]," +
                "\"@odata.count\":5," +
                "\"@odata.nextLink\":\"http://tempuri.org/Orders/nextLink\"," +
                "\"value\":[{" +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                "\"Id\":1," +
                "\"Amount\":13," +
                "\"Customer\":{\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}," +
                "\"Items\":[{\"Id\":1,\"Price\":13,\"Quantity\":7}]," +
                "\"#Action\":[" +
                "{\"title\":\"ResourceAction1\",\"target\":\"http://tempuri.org/Orders(1)/ResourceAction1\"}," +
                "{\"title\":\"ResourceAction2\",\"target\":\"http://tempuri.org/Orders(1)/ResourceAction2\"}]," +
                "\"#Function\":[" +
                "{\"title\":\"ResourceFunction1\",\"target\":\"http://tempuri.org/Orders(1)/ResourceFunction1\"}," +
                "{\"title\":\"ResourceFunction2\",\"target\":\"http://tempuri.org/Orders(1)/ResourceFunction2\"}]}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteNestedResourceSet_APIsShouldYieldSameResultForRequestPayload()
        {
            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true, linkUrl: new Uri($"{ServiceUri}/Customer(1)/Orders"));
            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");
            var orderResource = CreateOrderResource();
            var orderItemsPropertyNestedResourceInfo = CreateNestedResourceInfo("Items", isCollection: true);
            var orderItemResourceSet = CreateResourceSet("Orders", "NS.OrderItem", EdmNavigationSourceKind.ContainedEntitySet);
            var orderItemResource = CreateOrderItemResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerResource);
                await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                await writer.WriteStartAsync(orderResourceSet);
                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(orderItemsPropertyNestedResourceInfo);
                await writer.WriteStartAsync(orderItemResourceSet);
                await writer.WriteStartAsync(orderItemResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerResource);
                        writer.WriteStart(ordersPropertyNestedResourceInfo);
                        writer.WriteStart(orderResourceSet);
                        writer.WriteStart(orderResource);
                        writer.WriteStart(orderItemsPropertyNestedResourceInfo);
                        writer.WriteStart(orderItemResourceSet);
                        writer.WriteStart(orderItemResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Customer 1\"," +
                "\"Orders\":[{\"@odata.id\":\"http://tempuri.org/Orders(1)\",\"Id\":1,\"Amount\":13,\"Items\":[{\"Id\":1,\"Price\":13,\"Quantity\":7}]}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteTopLevelResource_APIsShouldYieldSameResult()
        {
            var superEntityResource = CreateSuperEntityResource();
            var coordinate1Resource = CreateCoordinateResource();
            var coordinate2Resource = CreateCoordinateResource(-1.25873495895d, 36.80558172342d);
            var customer1Resource = CreateCustomerResource();
            var customer2Resource = CreateCustomerResource(2);
            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");
            var coordinatePropertyNestedResourceInfo = CreateNestedResourceInfo("CoordinateProperty", isComplex: true);
            var entityPropertyNestedResourceInfo = CreateNestedResourceInfo("EntityProperty");
            var coordinateCollectionPropertyNestedResourceInfo = CreateNestedResourceInfo("CoordinateCollectionProperty", isComplex: true, isCollection: true);
            var entityCollectionPropertyNestedResourceInfo = CreateNestedResourceInfo("EntityCollectionProperty", isCollection: true);
            var dynamicComplexPropertyNestedResourceInfo = CreateNestedResourceInfo("DynamicComplexProperty", isComplex: true, isUndeclared: true);
            var dynamicComplexCollectionPropertyNestedResourceInfo = CreateNestedResourceInfo("DynamicComplexCollectionProperty", isComplex: true, isCollection: true, isUndeclared: true);

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.superEntitySet, this.superEntityType);

                await writer.WriteStartAsync(superEntityResource);
                await writer.WriteStartAsync(new ODataProperty { Name = "DynamicPrimitiveProperty", Value = 3.14159265359d });
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(new ODataPropertyInfo { Name = "DynamicSpatialProperty" });
                await writer.WritePrimitiveAsync(new ODataPrimitiveValue(GeographyPoint.Create(11.1, 11.1)));
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(new ODataPropertyInfo { Name = "DynamicNullProperty" });
                await writer.WritePrimitiveAsync(null);
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(new ODataPropertyInfo { Name = "DynamicStringValueProperty" });
                using (var textWriter = await writer.CreateTextWriterAsync())
                {
                    await textWriter.WriteAsync("The quick brown fox jumps over the lazy dog");
                    await textWriter.FlushAsync();
                }
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(new ODataStreamPropertyInfo
                {
                    Name = "DynamicBinaryValueProperty",
                    EditLink = new Uri($"{ServiceUri}/SuperEntitySet(1)/DynamicBinaryValueProperty/Edit"),
                    ReadLink = new Uri($"{ServiceUri}/SuperEntitySet(1)/DynamicBinaryValueProperty"),
                    ContentType = "text/plain"
                });
                using (var stream = await writer.CreateBinaryWriteStreamAsync())
                {
                    var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                    await stream.WriteAsync(bytes, 0, 4);
                    await stream.WriteAsync(bytes, 4, 4);
                    await stream.WriteAsync(bytes, 8, 2);
                    await stream.FlushAsync();
                }
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(coordinatePropertyNestedResourceInfo);
                await writer.WriteStartAsync(coordinate1Resource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(entityPropertyNestedResourceInfo);
                await writer.WriteStartAsync(customer1Resource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(coordinateCollectionPropertyNestedResourceInfo);
                await writer.WriteStartAsync(new ODataResourceSet { TypeName = "Collection(NS.Coordinate)" });
                await writer.WriteStartAsync(coordinate1Resource);
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(coordinate2Resource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(entityCollectionPropertyNestedResourceInfo);
                await writer.WriteStartAsync(customerResourceSet);
                await writer.WriteStartAsync(customer1Resource);
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(customer2Resource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(dynamicComplexPropertyNestedResourceInfo);
                await writer.WriteStartAsync(coordinate1Resource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(dynamicComplexCollectionPropertyNestedResourceInfo);
                await writer.WriteStartAsync(new ODataResourceSet { TypeName = "Collection(NS.Coordinate)" });
                await writer.WriteStartAsync(coordinate1Resource);
                await writer.WriteEndAsync();
                await writer.WriteStartAsync(coordinate2Resource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.superEntitySet, this.superEntityType);

                        writer.WriteStart(superEntityResource);
                        writer.WriteStart(new ODataProperty { Name = "DynamicPrimitiveProperty", Value = 3.14159265359d });
                        writer.WriteEnd();
                        writer.WriteStart(new ODataPropertyInfo { Name = "DynamicSpatialProperty" });
                        writer.WritePrimitive(new ODataPrimitiveValue(GeographyPoint.Create(11.1, 11.1)));
                        writer.WriteEnd();
                        writer.WriteStart(new ODataPropertyInfo { Name = "DynamicNullProperty" });
                        writer.WritePrimitive(null);
                        writer.WriteEnd();
                        writer.WriteStart(new ODataPropertyInfo { Name = "DynamicStringValueProperty" });
                        using (var textWriter = writer.CreateTextWriter())
                        {
                            textWriter.Write("The quick brown fox jumps over the lazy dog");
                            textWriter.Flush();
                        }
                        writer.WriteEnd();
                        writer.WriteStart(new ODataStreamPropertyInfo
                        {
                            Name = "DynamicBinaryValueProperty",
                            EditLink = new Uri($"{ServiceUri}/SuperEntitySet(1)/DynamicBinaryValueProperty/Edit"),
                            ReadLink = new Uri($"{ServiceUri}/SuperEntitySet(1)/DynamicBinaryValueProperty"),
                            ContentType = "text/plain"
                        });
                        using (var stream = writer.CreateBinaryWriteStream())
                        {
                            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                            stream.Write(bytes, 0, 4);
                            stream.Write(bytes, 4, 4);
                            stream.Write(bytes, 8, 2);
                            stream.Flush();
                        }
                        writer.WriteEnd();
                        writer.WriteStart(coordinatePropertyNestedResourceInfo);
                        writer.WriteStart(coordinate1Resource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteStart(entityPropertyNestedResourceInfo);
                        writer.WriteStart(customer1Resource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteStart(coordinateCollectionPropertyNestedResourceInfo);
                        writer.WriteStart(new ODataResourceSet { TypeName = "Collection(NS.Coordinate)" });
                        writer.WriteStart(coordinate1Resource);
                        writer.WriteEnd();
                        writer.WriteStart(coordinate2Resource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteStart(entityCollectionPropertyNestedResourceInfo);
                        writer.WriteStart(customerResourceSet);
                        writer.WriteStart(customer1Resource);
                        writer.WriteEnd();
                        writer.WriteStart(customer2Resource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteStart(dynamicComplexPropertyNestedResourceInfo);
                        writer.WriteStart(coordinate1Resource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteStart(dynamicComplexCollectionPropertyNestedResourceInfo);
                        writer.WriteStart(new ODataResourceSet { TypeName = "Collection(NS.Coordinate)" });
                        writer.WriteStart(coordinate1Resource);
                        writer.WriteEnd();
                        writer.WriteStart(coordinate2Resource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();

                        this.syncStream.Position = 0;
                        return new StreamReader(this.syncStream).ReadToEnd();
                    }
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#SuperEntitySet/$entity\"," +
                "\"Id\":1," +
                "\"BooleanProperty\":true," +
                "\"Int32Property\":13," +
                "\"SingleProperty\":3.142," +
                "\"Int16Property\":7," +
                "\"Int64Property\":6078747774547," +
                "\"DoubleProperty\":3.14159265359," +
                "\"DecimalProperty\":7654321," +
                "\"GuidProperty\":\"00000017-003b-003b-0001-020304050607\"," +
                "\"DateTimeOffsetProperty\":\"1970-12-31T23:59:59Z\"," +
                "\"TimeSpanProperty\":\"PT23H59M59S\"," +
                "\"ByteProperty\":1,\"SignedByteProperty\":9," +
                "\"StringProperty\":\"foo\"," +
                "\"ByteArrayProperty\":\"AQIDBAUGBwgJAA==\"," +
                "\"DateProperty\":\"1970-01-01\"," +
                "\"TimeOfDayProperty\":\"23:59:59.0000000\"," +
                "\"ColorProperty\":\"Black\"," +
                "\"GeographyPointProperty\":{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "\"GeometryPointProperty\":{\"type\":\"Point\",\"coordinates\":[7.0,13.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}," +
                "\"BooleanCollectionProperty\":[true,false]," +
                "\"Int32CollectionProperty\":[13,31]," +
                "\"SingleCollectionProperty\":[3.142,241.3]," +
                "\"Int16CollectionProperty\":[7,11]," +
                "\"Int64CollectionProperty\":[6078747774547,7454777478706]," +
                "\"DoubleCollectionProperty\":[3.14159265359,95356295141.3]," +
                "\"DecimalCollectionProperty\":[7654321,1234567]," +
                "\"GuidCollectionProperty\":[\"00000017-003b-003b-0001-020304050607\",\"0000000b-001d-001d-0706-050403020100\"]," +
                "\"DateTimeOffsetCollectionProperty\":[\"1970-12-31T23:59:59Z\",\"1858-11-17T11:29:29Z\"]," +
                "\"TimeSpanCollectionProperty\":[\"PT23H59M59S\",\"PT11H29M29S\"]," +
                "\"ByteCollectionProperty\":[1,9]," +
                "\"SignedByteCollectionProperty\":[9,1]," +
                "\"StringCollectionProperty\":[\"foo\",\"bar\"]," +
                "\"ByteArrayCollectionProperty\":[\"AQIDBAUGBwgJAA==\",\"AAkIBwYFBAMCAQ==\"]," +
                "\"DateCollectionProperty\":[\"1970-12-31\",\"1858-11-17\"]," +
                "\"TimeOfDayCollectionProperty\":[\"23:59:59.0000000\",\"11:29:29.0000000\"]," +
                "\"ColorCollectionProperty\":[\"Black\",\"White\"]," +
                "\"GeographyPointCollectionProperty\":[" +
                "{\"type\":\"Point\",\"coordinates\":[22.2,22.2],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "{\"type\":\"Point\",\"coordinates\":[11.6,11.9],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}]," +
                "\"GeometryPointCollectionProperty\":[" +
                "{\"type\":\"Point\",\"coordinates\":[7.0,13.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}," +
                "{\"type\":\"Point\",\"coordinates\":[13.0,7.0],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:0\"}}}]," +
                "\"DynamicPrimitiveProperty\":3.14159265359," +
                "\"DynamicSpatialProperty\":{\"type\":\"Point\",\"coordinates\":[11.1,11.1],\"crs\":{\"type\":\"name\",\"properties\":{\"name\":\"EPSG:4326\"}}}," +
                "\"DynamicNullProperty\":null," +
                "\"DynamicStringValueProperty\":\"The quick brown fox jumps over the lazy dog\"," +
                "\"DynamicBinaryValueProperty@odata.mediaEditLink\":\"http://tempuri.org/SuperEntitySet(1)/DynamicBinaryValueProperty/Edit\"," +
                "\"DynamicBinaryValueProperty@odata.mediaReadLink\":\"http://tempuri.org/SuperEntitySet(1)/DynamicBinaryValueProperty\"," +
                "\"DynamicBinaryValueProperty@odata.mediaContentType\":\"text/plain\"," +
                "\"DynamicBinaryValueProperty\":\"AQIDBAUGBwgJAA==\"," +
                "\"CoordinateProperty\":{\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
                "\"EntityProperty\":{\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}," +
                "\"CoordinateCollectionProperty\":[" +
                "{\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
                "{\"Longitude\":-1.25873495895,\"Latitude\":36.80558172342}]," +
                "\"EntityCollectionProperty\":[" +
                "{\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}," +
                "{\"@odata.id\":\"http://tempuri.org/Customers(2)\",\"Id\":2,\"Name\":\"Customer 2\"}]," +
                "\"DynamicComplexProperty\":{\"@odata.type\":\"#NS.Coordinate\",\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
                "\"DynamicComplexCollectionProperty@odata.type\":\"#Collection(NS.Coordinate)\"," +
                "\"DynamicComplexCollectionProperty\":[" +
                "{\"@odata.type\":\"#NS.Coordinate\",\"Longitude\":47.64229583688,\"Latitude\":-122.13694393057}," +
                "{\"@odata.type\":\"#NS.Coordinate\",\"Longitude\":-1.25873495895,\"Latitude\":36.80558172342}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteNullResource_APIsShouldYieldSameResult()
        {
            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync((ODataResource)null);
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart((ODataResource)null);
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "null";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteNullNestedResource_APIsShouldYieldSameResult()
        {
            var orderResource = CreateOrderResource();
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");
            customerPropertyNestedResourceInfo.TypeAnnotation = new ODataTypeAnnotation("NS.Customer");

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                await writer.WriteStartAsync((ODataResource)null);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(orderResource);
                        writer.WriteStart(customerPropertyNestedResourceInfo);
                        writer.WriteStart((ODataResource)null);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\",\"Id\":1,\"Amount\":13,\"Customer@odata.type\":\"#NS.Customer\",\"Customer\":null}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteDeferredNestedResource_APIsShouldYieldSameResult()
        {
            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");
            customerResourceSet.DeltaLink = new Uri($"{ServiceUri}/Customers/deltaLink");
            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true);

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerResourceSet);
                await writer.WriteStartAsync(customerResource);
                await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerResourceSet);
                        writer.WriteStart(customerResource);
                        writer.WriteStart(ordersPropertyNestedResourceInfo);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"value\":[{\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteRequestPayload_APIsShouldYieldSameResultForResourceWithoutModel()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync();

                await writer.WriteStartAsync(customerResource);
                await writer.WriteStartAsync(new ODataStreamPropertyInfo { Name = "DynamicProperty", ContentType = "text/plain" });
                using (var textWriter = await writer.CreateTextWriterAsync())
                {
                    await textWriter.WriteAsync("cA_Россия");
                    await textWriter.FlushAsync();
                }
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter();

                        writer.WriteStart(customerResource);
                        writer.WriteStart(new ODataStreamPropertyInfo { Name = "DynamicProperty", ContentType = "text/plain" });
                        using (var textWriter = writer.CreateTextWriter())
                        {
                            textWriter.Write("cA_Россия");
                            textWriter.Flush();
                        }
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Customer 1\"," +
                "\"DynamicProperty@odata.mediaContentType\":\"text/plain\"," +
                "\"DynamicProperty\":\"cA_Россия\"}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Changed, "changed")]
        [InlineData(DeltaDeletedEntryReason.Deleted, "deleted")]
        public async Task WriteDeltaResourceSet_APIsShouldYieldSameResult(DeltaDeletedEntryReason reason, string partial)
        {
            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer", new Uri($"{ServiceUri}/Customers/deltaLink"));
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerDeltaResourceSet);
                await writer.WriteStartAsync(customerDeletedResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerDeltaResourceSet);
                        writer.WriteStart(customerDeletedResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"value\":[{" +
                "\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$deletedEntity\"," +
                "\"id\":\"http://tempuri.org/Customers(7)\"," +
                "\"reason\":\"" + partial + "\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted, "\"reason\":\"deleted\"")]
        [InlineData(DeltaDeletedEntryReason.Changed, "\"reason\":\"changed\"")]
        [InlineData(null, "")]
        public async Task WriteDeltaResourceSet_APIsShouldYieldSameResultForV401ResponsePayload(DeltaDeletedEntryReason? reason, string partial)
        {
            this.writerSettings.Version = ODataVersion.V401;

            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer");
            customerDeltaResourceSet.Count = 5;
            customerDeltaResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerDeltaResourceSet);
                await writer.WriteStartAsync(customerDeletedResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerDeltaResourceSet);
                        writer.WriteStart(customerDeletedResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@count\":5,\"@nextLink\":\"http://tempuri.org/Customers/nextLink\"," +
                "\"value\":[{" +
                "\"@removed\":{" + partial + "}," +
                "\"@id\":\"http://tempuri.org/Customers(7)\"," +
                "\"Id\":7," +
                "\"Name\":\"Customer 7\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted, "\"reason\":\"deleted\"")]
        [InlineData(DeltaDeletedEntryReason.Changed, "\"reason\":\"changed\"")]
        [InlineData(null, "")]
        public async Task WriteDeltaResourceSetWithNestedDeletedResource_APIsShouldYieldSameResultForV401ResponsePayload(DeltaDeletedEntryReason? reason, string partial)
        {
            this.writerSettings.Version = ODataVersion.V401;

            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer", new Uri($"{ServiceUri}/Customers/deltaLink"));
            var orderResource = CreateOrderResource();
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(customerDeltaResourceSet);
                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                await writer.WriteStartAsync(customerDeletedResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(customerDeltaResourceSet);
                        writer.WriteStart(orderResource);
                        writer.WriteStart(customerPropertyNestedResourceInfo);
                        writer.WriteStart(customerDeletedResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$delta\"," +
                "\"@deltaLink\":\"http://tempuri.org/Customers/deltaLink\"," +
                "\"value\":[{" +
                "\"@id\":\"http://tempuri.org/Orders(1)\"," +
                "\"Id\":1," +
                "\"Amount\":13," +
                "\"Customer\":{\"@removed\":{" + partial + "},\"@id\":\"http://tempuri.org/Customers(7)\",\"Id\":7,\"Name\":\"Customer 7\"}}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteNestedDeltaResourceSet_APIsShouldYieldSameResultForV401ResponsePayload()
        {
            this.writerSettings.Version = ODataVersion.V401;

            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");
            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true);
            var orderDeltaResourceSet = CreateDeltaResourceSet("Orders", "NS.Order");
            orderDeltaResourceSet.Count = 1;
            var orderNestedResource = CreateOrderResource();

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerResourceSet);
                await writer.WriteStartAsync(customerResource);
                await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                await writer.WriteStartAsync(orderDeltaResourceSet);
                await writer.WriteStartAsync(orderNestedResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerResourceSet);
                        writer.WriteStart(customerResource);
                        writer.WriteStart(ordersPropertyNestedResourceInfo);
                        writer.WriteStart(orderDeltaResourceSet);
                        writer.WriteStart(orderNestedResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@context\":\"http://tempuri.org/$metadata#Customers\"," +
                "\"value\":[{" +
                "\"@id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Customer 1\"," +
                "\"Orders@count\":1," +
                "\"Orders@delta\":[{\"@id\":\"http://tempuri.org/Orders(1)\",\"Id\":1,\"Amount\":13}]}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Theory]
        [InlineData(ODataLibraryCompatibility.Version6, "\"Items@odata.context\":\"http://tempuri.org/$metadata#Orders(1)/Items\",")]
        [InlineData(ODataLibraryCompatibility.Version7, "")]
        [InlineData(ODataLibraryCompatibility.Latest, "")]
        public async Task WriteContainment_APIsShouldYieldSameResult(ODataLibraryCompatibility libraryCompatilibity, string containmentContextUrl)
        {
            this.writerSettings.ODataUri.Path = new ODataUriParser(this.model, new Uri(ServiceUri), new Uri(ServiceUri + "/Orders(1)")).ParsePath();
            this.writerSettings.LibraryCompatibility = libraryCompatilibity;

            var orderResource = CreateOrderResource();
            var orderItemsPropertyNestedResourceInfo = CreateNestedResourceInfo("Items", isCollection: true);
            var orderItemResourceSet = CreateResourceSet("Orders", "NS.OrderItem", EdmNavigationSourceKind.ContainedEntitySet);
            var orderItemResource = CreateOrderItemResource();

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(orderItemsPropertyNestedResourceInfo);
                await writer.WriteStartAsync(orderItemResourceSet);
                await writer.WriteStartAsync(orderItemResource);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(orderResource);
                        writer.WriteStart(orderItemsPropertyNestedResourceInfo);
                        writer.WriteStart(orderItemResourceSet);
                        writer.WriteStart(orderItemResource);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                "\"Id\":1," +
                "\"Amount\":13," +
                containmentContextUrl +
                "\"Items\":[{\"Id\":1,\"Price\":13,\"Quantity\":7}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteCollection_APIsShouldYieldSameResult()
        {
            var collectionStart = new ODataCollectionStart
            {
                SerializationInfo = new ODataCollectionStartSerializationInfo
                {
                    CollectionTypeName = "Collection(Edm.String)"
                }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataCollectionWriterAsync(EdmCoreModel.Instance.GetString(false));

                await writer.WriteStartAsync(collectionStart);
                await writer.WriteItemAsync("Violet");
                await writer.WriteItemAsync("Indigo");
                await writer.WriteItemAsync("Blue");
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataCollectionWriter(EdmCoreModel.Instance.GetString(false));

                        writer.WriteStart(collectionStart);
                        writer.WriteItem("Violet");
                        writer.WriteItem("Indigo");
                        writer.WriteItem("Blue");
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Collection(Edm.String)\",\"value\":[\"Violet\",\"Indigo\",\"Blue\"]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteParameter_APIsShouldYieldSameResult()
        {
            var rateCustomerAction = new EdmAction("NS", "RateCustomer", EdmCoreModel.Instance.GetInt32(false));
            rateCustomerAction.AddParameter("customerId", EdmCoreModel.Instance.GetInt32(false));

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings, this.model))
            {
                var parameterWriter = await messageWriter.CreateODataParameterWriterAsync(rateCustomerAction);

                await parameterWriter.WriteStartAsync();
                await parameterWriter.WriteValueAsync("customerId", 1);
                await parameterWriter.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings, this.model))
                    {
                        var parameterWriter = messageWriter.CreateODataParameterWriter(rateCustomerAction);

                        parameterWriter.WriteStart();
                        parameterWriter.WriteValue("customerId", 1);
                        parameterWriter.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"customerId\":1}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteResourceUriParameter_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings, this.model))
            {
                var parameterWriter = await messageWriter.CreateODataUriParameterResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                await parameterWriter.WriteStartAsync(customerResource);
                await parameterWriter.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings, this.model))
                    {
                        var parameterWriter = messageWriter.CreateODataUriParameterResourceWriter(this.customerEntitySet, this.customerEntityType);

                        parameterWriter.WriteStart(customerResource);
                        parameterWriter.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\",\"Id\":1,\"Name\":\"Customer 1\"}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteResourceSetUriParameter_APIsShouldYieldSameResult()
        {
            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings, this.model))
            {
                var parameterWriter = await messageWriter.CreateODataUriParameterResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                await parameterWriter.WriteStartAsync(customerResourceSet);
                await parameterWriter.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings, this.model))
                    {
                        var parameterWriter = messageWriter.CreateODataUriParameterResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                        parameterWriter.WriteStart(customerResourceSet);
                        parameterWriter.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "[]";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteEntityReferenceLink_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true);
            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");
            orderResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customer(1)/Orders/nextLink");
            var orderEntityReferenceLink1 = new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Orders(1)") };
            var orderEntityReferenceLink2 = new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Orders(2)") };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerResource);
                await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                await writer.WriteStartAsync(orderResourceSet);
                await writer.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                await writer.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerResource);
                        writer.WriteStart(ordersPropertyNestedResourceInfo);
                        writer.WriteStart(orderResourceSet);
                        writer.WriteEntityReferenceLink(orderEntityReferenceLink1);
                        writer.WriteEntityReferenceLink(orderEntityReferenceLink2);
                        writer.WriteEnd();
                        writer.WriteEnd();
                        writer.WriteEnd();

                        this.syncStream.Position = 0;
                        return new StreamReader(this.syncStream).ReadToEnd();
                    }
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Customer 1\"," +
                "\"Orders@odata.nextLink\":\"http://tempuri.org/Customer(1)/Orders/nextLink\"," +
                "\"Orders\":[{\"@odata.id\":\"Orders(1)\"},{\"@odata.id\":\"Orders(2)\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteEntityReferenceLink_APIsShouldYieldSameResultForV401RequestPayload()
        {
            this.writerSettings.Version = ODataVersion.V401;

            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true);
            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");
            var orderEntityReferenceLink1 = new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Orders(1)") };
            var orderEntityReferenceLink2 = new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Orders(2)") };

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                await writer.WriteStartAsync(customerResource);
                await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                await writer.WriteEntityReferenceLinkAsync(orderEntityReferenceLink1);
                await writer.WriteEntityReferenceLinkAsync(orderEntityReferenceLink2);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                        writer.WriteStart(customerResource);
                        writer.WriteStart(ordersPropertyNestedResourceInfo);
                        writer.WriteEntityReferenceLink(orderEntityReferenceLink1);
                        writer.WriteEntityReferenceLink(orderEntityReferenceLink2);
                        writer.WriteEnd();
                        writer.WriteEnd();

                        this.syncStream.Position = 0;
                        return new StreamReader(this.syncStream).ReadToEnd();
                    }
                });

            var expected = "{\"@context\":\"http://tempuri.org/$metadata#Customers/$entity\"," +
                "\"@id\":\"http://tempuri.org/Customers(1)\"," +
                "\"Id\":1," +
                "\"Name\":\"Customer 1\"," +
                "\"Orders\":[{\"@id\":\"Orders(1)\"},{\"@id\":\"Orders(2)\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteEntityReferenceLink_APIsShouldYieldSameResultForSingleNestedResource()
        {
            var orderResource = CreateOrderResource();
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");
            var entityReferenceLink = new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Customers(1)") };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                await writer.WriteEntityReferenceLinkAsync(entityReferenceLink);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(orderResource);
                        writer.WriteStart(customerPropertyNestedResourceInfo);
                        writer.WriteEntityReferenceLink(entityReferenceLink);
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                "\"Id\":1," +
                "\"Amount\":13," +
                "\"Customer\":{\"@odata.id\":\"Customers(1)\"}}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteEntityReferenceLink_APIsShouldYieldSameResultForSingleNestedResourceInRequestPayload()
        {
            var orderResource = CreateOrderResource();
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");
            var entityReferenceLink = new ODataEntityReferenceLink { Url = new Uri($"{ServiceUri}/Customers(1)") };

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(orderResource);
                await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                await writer.WriteEntityReferenceLinkAsync(entityReferenceLink);
                await writer.WriteEndAsync();
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(orderResource);
                        writer.WriteStart(customerPropertyNestedResourceInfo);
                        writer.WriteEntityReferenceLink(entityReferenceLink);
                        writer.WriteEnd();
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                "\"Id\":1," +
                "\"Amount\":13," +
                "\"Customer@odata.bind\":\"http://tempuri.org/Customers(1)\"}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteDeltaLink_APIsShouldYieldSameResult()
        {
            var orderDeltaResourceSet = CreateDeltaResourceSet("Orders", "NS.Order", new Uri($"{ServiceUri}/Orders/deltaLink"));
            var deltaLink = new ODataDeltaLink(new Uri($"{ServiceUri}/Orders(1)"), new Uri($"{ServiceUri}/Customers(1)"), "Customer");
            var deltaDeletedLink = new ODataDeltaDeletedLink(new Uri($"{ServiceUri}/Orders(2)"), new Uri($"{ServiceUri}/Customers(2)"), "Customer");

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                await writer.WriteStartAsync(orderDeltaResourceSet);
                await writer.WriteDeltaLinkAsync(deltaLink);
                await writer.WriteDeltaDeletedLinkAsync(deltaDeletedLink);
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.orderEntitySet, this.orderEntityType);

                        writer.WriteStart(orderDeltaResourceSet);
                        writer.WriteDeltaLink(deltaLink);
                        writer.WriteDeltaDeletedLink(deltaDeletedLink);
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$delta\"," +
                "\"@odata.deltaLink\":\"http://tempuri.org/Orders/deltaLink\"," +
                "\"value\":[" +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$link\"," +
                "\"source\":\"http://tempuri.org/Orders(1)\"," +
                "\"relationship\":\"Customer\"," +
                "\"target\":\"http://tempuri.org/Customers(1)\"}," +
                "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$deletedLink\"," +
                "\"source\":\"http://tempuri.org/Orders(2)\"," +
                "\"relationship\":\"Customer\"," +
                "\"target\":\"http://tempuri.org/Customers(2)\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteResourceWithMetadataBuilder_APIsShouldYieldSameResult()
        {
            var orderResource = CreateOrderResource();
            var nestedResourceInfos = new List<ODataJsonLightReaderNestedResourceInfo>
            {
                ODataJsonLightReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(
                    nestedResourceInfo: new ODataNestedResourceInfo
                    {
                        Name = "Customer",
                        Url = new Uri($"{orderResource.Id}/Customer"),
                        IsCollection = false,
                        AssociationLinkUrl = new Uri($"{orderResource.Id}/Customer")
                    },
                    nestedProperty: this.orderEntityType.FindProperty("Customer"),
                    nestedResourceType: this.customerEntityType)
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                orderResource.MetadataBuilder = new TestODataResourceMetadataBuilder(
                    orderResource.Id,
                    unprocessedNavigationLinksFactory: () => nestedResourceInfos);

                await writer.WriteStartAsync(orderResource);
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                        orderResource.MetadataBuilder = new TestODataResourceMetadataBuilder(
                            orderResource.Id,
                            unprocessedNavigationLinksFactory: () => nestedResourceInfos);

                        writer.WriteStart(orderResource);
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#Orders/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/Orders(1)\"," +
                "\"@odata.etag\":\"resource-etag\"," +
                "\"@odata.editLink\":\"Orders(1)/Edit\"," +
                "\"@odata.readLink\":\"Orders(1)\"," +
                "\"Id\":1," +
                "\"Amount\":13," +
                "\"Customer@odata.associationLink\":\"http://tempuri.org/Orders(1)/Customer\"," +
                "\"Customer@odata.navigationLink\":\"http://tempuri.org/Orders(1)/Customer\"}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        public static IEnumerable<object[]> GetWriteMediaResourceTestData()
        {
            ODataResource streamEntityResource;

            // Resource with stream property
            streamEntityResource = CreateStreamEntityResource();
            streamEntityResource.EditLink = new Uri($"{streamEntityResource.Id}/Edit");
            streamEntityResource.ReadLink = streamEntityResource.Id;
            IList<ODataProperty> properties = streamEntityResource.Properties as IList<ODataProperty>;
            properties.Add(new ODataProperty
            {
                Name = "Img",
                Value = new ODataStreamReferenceValue
                {
                    EditLink = new Uri($"{streamEntityResource.Id}/Img/Edit"),
                    ReadLink = new Uri($"{streamEntityResource.Id}/Img"),
                    ContentType = "image/png",
                    ETag = "img-etag"
                }
            });

            yield return new object[]
            {
                streamEntityResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#StreamEntitySet/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/StreamEntitySet(1)\"," +
                "\"@odata.editLink\":\"http://tempuri.org/StreamEntitySet(1)/Edit\"," +
                "\"@odata.readLink\":\"http://tempuri.org/StreamEntitySet(1)\"," +
                "\"Id\":1," +
                "\"Img@odata.mediaEditLink\":\"http://tempuri.org/StreamEntitySet(1)/Img/Edit\"," +
                "\"Img@odata.mediaReadLink\":\"http://tempuri.org/StreamEntitySet(1)/Img\"," +
                "\"Img@odata.mediaContentType\":\"image/png\"," +
                "\"Img@odata.mediaEtag\":\"img-etag\"}"
            };

            // Media resource
            streamEntityResource = CreateStreamEntityResource();
            streamEntityResource.EditLink = new Uri($"{streamEntityResource.Id}/Edit");
            streamEntityResource.ReadLink = streamEntityResource.Id;
            streamEntityResource.MediaResource = new ODataStreamReferenceValue
            {
                EditLink = new Uri($"{streamEntityResource.Id}/Img/Edit"),
                ReadLink = new Uri($"{streamEntityResource.Id}/Img"),
                ContentType = "image/png",
                ETag = "img-etag"
            };

            yield return new object[]
            {
                streamEntityResource,
                "{\"@odata.context\":\"http://tempuri.org/$metadata#StreamEntitySet/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/StreamEntitySet(1)\"," +
                "\"@odata.editLink\":\"http://tempuri.org/StreamEntitySet(1)/Edit\"," +
                "\"@odata.readLink\":\"http://tempuri.org/StreamEntitySet(1)\"," +
                "\"@odata.mediaEditLink\":\"http://tempuri.org/StreamEntitySet(1)/Img/Edit\"," +
                "\"@odata.mediaReadLink\":\"http://tempuri.org/StreamEntitySet(1)/Img\"," +
                "\"@odata.mediaContentType\":\"image/png\"," +
                "\"@odata.mediaEtag\":\"img-etag\"," +
                "\"Id\":1}"
            };
        }

        [Theory]
        [MemberData(nameof(GetWriteMediaResourceTestData))]
        public async Task WriteMediaResource_APIsShouldYieldSameResult(ODataResource streamEntityResource, string expected)
        {
            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.streamEntitySet, this.streamEntityType);

                await writer.WriteStartAsync(streamEntityResource);
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.streamEntitySet, this.streamEntityType);

                        writer.WriteStart(streamEntityResource);
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteMediaResourceWithMetadataBuilder_APIsShouldYieldSameResult()
        {
            var streamEntityResource = CreateStreamEntityResource();
            var streamProperties = new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "Img",
                    Value = new ODataStreamReferenceValue
                    {
                        EditLink = new Uri($"{streamEntityResource.Id}/Img/Edit"),
                        ReadLink = new Uri($"{streamEntityResource.Id}/Img"),
                        ETag = "img-etag",
                        ContentType = "image/png"
                    }
                }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var writer = await messageWriter.CreateODataResourceWriterAsync(this.streamEntitySet, this.streamEntityType);

                streamEntityResource.MetadataBuilder = new TestODataResourceMetadataBuilder(
                    streamEntityResource.Id,
                    unprocessedStreamPropertiesFactory: () => streamProperties);

                await writer.WriteStartAsync(streamEntityResource);
                await writer.WriteEndAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var writer = messageWriter.CreateODataResourceWriter(this.streamEntitySet, this.streamEntityType);

                        streamEntityResource.MetadataBuilder = new TestODataResourceMetadataBuilder(
                            streamEntityResource.Id,
                            unprocessedStreamPropertiesFactory: () => streamProperties);

                        writer.WriteStart(streamEntityResource);
                        writer.WriteEnd();
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata#StreamEntitySet/$entity\"," +
                "\"@odata.id\":\"http://tempuri.org/StreamEntitySet(1)\"," +
                "\"@odata.etag\":\"resource-etag\"," +
                "\"@odata.editLink\":\"StreamEntitySet(1)/Edit\"," +
                "\"@odata.readLink\":\"StreamEntitySet(1)\"," +
                "\"Id\":1," +
                "\"Img@odata.mediaEditLink\":\"http://tempuri.org/StreamEntitySet(1)/Img/Edit\"," +
                "\"Img@odata.mediaReadLink\":\"http://tempuri.org/StreamEntitySet(1)/Img\"," +
                "\"Img@odata.mediaContentType\":\"image/png\"," +
                "\"Img@odata.mediaEtag\":\"img-etag\"}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Customers"), "1");

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();

                        var operationRequestMessage = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Customers HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteBatchRequestWithChangeset_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();
                await batchWriter.WriteStartChangesetAsync("69028f2c");

                var operationRequestMessage = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Customers"), "1");

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndChangesetAsync();
                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();
                        batchWriter.WriteStartChangeset("69028f2c");

                        var operationRequestMessage = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndChangeset();
                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_69028f2c

--changeset_69028f2c
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Customers HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--changeset_69028f2c--
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteBatchRequestWithChangesetAndDependsOnIds_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();
            var orderResource = CreateOrderResource();
            var changetsetGuidRegex = batchGuidRegex.Replace("batch", "changeset");
            var changesetBoundary = "changeset_69028f2c";

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();
                await batchWriter.WriteStartChangesetAsync();

                var operationRequestMessage1 = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Customers"), "1");

                using (var nestedMessageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                {
                    var writer = await nestedMessageWriter1.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                // Operation request depends on the previous (Content ID: 1)
                var dependsOnIds = new List<string> { "1" };
                var operationRequestMessage2 = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                using (var nestedMessageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                {
                    var writer = await nestedMessageWriter2.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                    await writer.WriteStartAsync(orderResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndChangesetAsync();
                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);
            asyncResult = Regex.Replace(asyncResult, changetsetGuidRegex, changesetBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();
                        batchWriter.WriteStartChangeset();

                        var operationRequestMessage1 = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        using (var nestedMessageWriter1 = new ODataMessageWriter(operationRequestMessage1))
                        {
                            var writer = nestedMessageWriter1.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        // Operation request depends on the previous (Content ID: 1)
                        var dependsOnIds = new List<string> { "1" };
                        var operationRequestMessage2 = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);

                        using (var nestedMessageWriter2 = new ODataMessageWriter(operationRequestMessage2))
                        {
                            var writer = nestedMessageWriter2.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                            writer.WriteStart(orderResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndChangeset();
                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    result = Regex.Replace(result, batchGuidRegex, batchBoundary);
                    return Regex.Replace(result, changetsetGuidRegex, changesetBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changeset_69028f2c

--changeset_69028f2c
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST http://tempuri.org/Customers HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--changeset_69028f2c
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

POST http://tempuri.org/Orders HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.id"":""http://tempuri.org/Orders(1)"",""Id"":1,""Amount"":13}
--changeset_69028f2c--
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteBatchResponse_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();
            var nestedWriterSettings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri { ServiceRoot = new Uri(ServiceUri) }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();

                var operationResponseMessage = await batchWriter.CreateOperationResponseMessageAsync("1");

                using (var nestedMessageWriter = new ODataMessageWriter(operationResponseMessage, nestedWriterSettings))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();

                        var operationResponseMessage = batchWriter.CreateOperationResponseMessage("1");

                        using (var nestedMessageWriter = new ODataMessageWriter(operationResponseMessage, nestedWriterSettings))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 0 Unknown Status Code
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteBatchResponseWithChangeset_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();
            var nestedWriterSettings = new ODataMessageWriterSettings
            {
                ODataUri = new ODataUri { ServiceRoot = new Uri(ServiceUri) }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();
                await batchWriter.WriteStartChangesetAsync("69028f2c");

                var operationResponseMessage = await batchWriter.CreateOperationResponseMessageAsync("1");

                using (var nestedMessageWriter = new ODataMessageWriter(operationResponseMessage, nestedWriterSettings))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndChangesetAsync();
                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();
                        batchWriter.WriteStartChangeset("69028f2c");

                        var operationResponseMessage = batchWriter.CreateOperationResponseMessage("1");

                        using (var nestedMessageWriter = new ODataMessageWriter(operationResponseMessage, nestedWriterSettings))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndChangeset();
                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: multipart/mixed; boundary=changesetresponse_69028f2c

--changesetresponse_69028f2c
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 0 Unknown Status Code
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.context"":""http://tempuri.org/$metadata#Customers/$entity"",""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--changesetresponse_69028f2c--
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteBatchRequestWithAbsoluteUriUsingHostHeader_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/odata/Customers"), "1", BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();

                        var operationRequestMessage = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/odata/Customers"), "1", BatchPayloadUriOption.AbsoluteUriUsingHostHeader);

                        using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST /odata/Customers HTTP/1.1
Host: tempuri.org:80
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        async Task WriteBatchRequestWithRelativeUri_APIsShouldYieldSameResult()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri("/odata/Customers", UriKind.Relative), "1", BatchPayloadUriOption.RelativeUri);

                using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                {
                    var writer = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                    await writer.WriteStartAsync(customerResource);
                    await writer.WriteEndAsync();
                }

                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();

                        var operationRequestMessage = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri("/odata/Customers", UriKind.Relative), "1", BatchPayloadUriOption.RelativeUri);

                        using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                        {
                            var writer = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                        }

                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST odata/Customers HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.id"":""http://tempuri.org/Customers(1)"",""Id"":1,""Name"":""Customer 1""}
--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        async Task WriteBatchRequest_APIsYieldSameResultForReportMessageCompleted()
        {
            var customerResource = CreateCustomerResource();

            IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
            {
                var batchWriter = await messageWriter.CreateODataBatchWriterAsync();
                await batchWriter.WriteStartBatchAsync();

                var operationRequestMessage = await batchWriter.CreateOperationRequestMessageAsync(
                    "POST", new Uri($"{ServiceUri}/Customers"), "1");
                // No writer created for the request message
                await batchWriter.WriteEndBatchAsync();
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();
            asyncResult = Regex.Replace(asyncResult, batchGuidRegex, batchBoundary);

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                    {
                        var batchWriter = messageWriter.CreateODataBatchWriter();
                        batchWriter.WriteStartBatch();

                        var operationRequestMessage = batchWriter.CreateOperationRequestMessage(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");
                        // No writer created for the request message
                        batchWriter.WriteEndBatch();
                    }

                    this.syncStream.Position = 0;
                    var result = new StreamReader(this.syncStream).ReadToEnd();
                    return Regex.Replace(result, batchGuidRegex, batchBoundary);
                });

            var expected = @"--batch_aed653ab
Content-Type: application/http
Content-Transfer-Encoding: binary

POST http://tempuri.org/Customers HTTP/1.1


--batch_aed653ab--
";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteServiceDocument_APIsShouldYieldSameResult()
        {
            var serviceDocument = new ODataServiceDocument
            {
                EntitySets = new List<ODataEntitySetInfo>
                {
                    new ODataEntitySetInfo { Name = "Orders", Title = "Orders", Url = new Uri($"{ServiceUri}/Orders") },
                    new ODataEntitySetInfo { Name = "Customers", Title = "Customers", Url = new Uri($"{ServiceUri}/Customers") },
                    new ODataEntitySetInfo { Name = "SuperEntitySet", Title = "SuperEntitySet", Url = new Uri($"{ServiceUri}/SuperEntitySet") },
                    new ODataEntitySetInfo { Name = "StreamEntitySet", Title = "StreamEntitySet", Url = new Uri($"{ServiceUri}/StreamEntitySet") }
                }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                await messageWriter.WriteServiceDocumentAsync(serviceDocument);
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        messageWriter.WriteServiceDocument(serviceDocument);
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"@odata.context\":\"http://tempuri.org/$metadata\"," +
                "\"value\":[" +
                "{\"name\":\"Orders\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Orders\"}," +
                "{\"name\":\"Customers\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/Customers\"}," +
                "{\"name\":\"SuperEntitySet\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/SuperEntitySet\"}," +
                "{\"name\":\"StreamEntitySet\",\"kind\":\"EntitySet\",\"url\":\"http://tempuri.org/StreamEntitySet\"}]}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteError_APIsShouldYieldSameResult()
        {
            var nullReferenceError = new ODataError
            {
                ErrorCode = "badRequest",
                Message = "Object reference not set to an instance of an object",
                Target = "ConApp",
                InnerError = new ODataInnerError
                {
                    TypeName = "System.NullReferenceException",
                    Message = "Exception thrown due to attempt to access a member on a variable that currently holds a null reference",
                    StackTrace = "   at ConApp.Program.Main(String[] args) in C:\\Projects\\ConApp\\ConApp\\Program.cs:line 10",
                    InnerError = new ODataInnerError()
                },
                InstanceAnnotations = new List<ODataInstanceAnnotation>
                {
                    new ODataInstanceAnnotation("Is.Error", new ODataPrimitiveValue(true))
                }
            };

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                await messageWriter.CreateODataResourceWriterAsync();
                await messageWriter.WriteErrorAsync(nullReferenceError, includeDebugInformation: true);
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        messageWriter.CreateODataResourceWriter();
                        messageWriter.WriteError(nullReferenceError, includeDebugInformation: true);
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "{\"error\":{" +
                "\"code\":\"badRequest\"," +
                "\"message\":\"Object reference not set to an instance of an object\"," +
                "\"target\":\"ConApp\"," +
                "\"innererror\":{" +
                "\"message\":\"Exception thrown due to attempt to access a member on a variable that currently holds a null reference\"," +
                "\"type\":\"System.NullReferenceException\"," +
                "\"stacktrace\":\"   at ConApp.Program.Main(String[] args) in C:\\\\Projects\\\\ConApp\\\\ConApp\\\\Program.cs:line 10\"," +
                "\"internalexception\":{\"message\":\"\",\"type\":\"\",\"stacktrace\":\"\"}}," +
                "\"@Is.Error\":true}}";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        [Fact]
        public async Task WriteResponsePayloadWithJsonPaddingEnabled_APIsShouldYieldSameResult()
        {
            this.writerSettings.JsonPCallback = "fn";

            IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
            using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
            {
                await messageWriter.WritePropertyAsync(new ODataProperty { Name = "Count", Value = 5 });
            }

            this.asyncStream.Position = 0;
            var asyncResult = await new StreamReader(this.asyncStream).ReadToEndAsync();

            var syncResult = await TaskUtils.GetTaskForSynchronousOperation(
                () =>
                {
                    IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                    using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                    {
                        messageWriter.WriteProperty(new ODataProperty { Name = "Count", Value = 5 });
                    }

                    this.syncStream.Position = 0;
                    return new StreamReader(this.syncStream).ReadToEnd();
                });

            var expected = "fn({\"@odata.context\":\"http://tempuri.org/$metadata#Edm.Int32\",\"value\":5})";

            Assert.Equal(expected, asyncResult);
            Assert.Equal(expected, syncResult);
        }

        #region Exception Cases

        [Fact]
        public async Task WriteRequestPayload_APIsThrowExceptionForCountInResourceSet()
        {
            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");
            customerResourceSet.Count = 5;

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResourceSet);
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_QueryCountInRequest, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_QueryCountInRequest, syncException.Message);
        }

        [Fact]
        public async Task WriteRequestPayload_APIsThrowExceptionForNextPageLinkInDeltaResourceSet()
        {
            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer");
            customerDeltaResourceSet.NextPageLink = new Uri($"{ServiceUri}/Customers/nextLink");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerDeltaResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerDeltaResourceSet);
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_QueryNextLinkInRequest, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_QueryNextLinkInRequest, syncException.Message);
        }

        [Fact]
        public async Task WriteRequestPayload_APIsThrowExceptionForDeltaLinkInDeltaResourceSet()
        {
            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer", new Uri($"{ServiceUri}/Customers/deltaLink"));

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerDeltaResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerDeltaResourceSet);
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_QueryDeltaLinkInRequest, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_QueryDeltaLinkInRequest, syncException.Message);
        }

        [Fact]
        public async Task WriteRequestPayload_APIsThrowExceptionForDeferredNestedResource()
        {
            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");
            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true);

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResourceSet);
                        await writer.WriteStartAsync(customerResource);
                        await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                        await writer.WriteEndAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResourceSet);
                            writer.WriteStart(customerResource);
                            writer.WriteStart(ordersPropertyNestedResourceInfo);
                            writer.WriteEnd();
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_DeferredLinkInRequest, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_DeferredLinkInRequest, syncException.Message);
        }

        [Fact]
        public async Task WriteResponsePayload_APIsThrowExceptionForNestedResourceInfoAtTopLevel()
        {
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerPropertyNestedResourceInfo);
                        }
                    }));

            var exceptionMessage = Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "NestedResourceInfo");

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteResponsePayload_APIsThrowExceptionForDeletedResourceAtTopLevel()
        {
            var customerDeletedResource = CreateCustomerDeletedResource();

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerDeletedResource);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerDeletedResource);
                        }
                    }));

            var exceptionMessage = Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "DeletedResource");

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteProperty_APIsThrowExceptionForPropertyAtTopLevel()
        {
            var countProperty = new ODataPropertyInfo { Name = "Count" };

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(countProperty);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(countProperty);
                        }
                    }));

            var exceptionMessage = Strings.ODataWriterCore_InvalidTransitionFromStart("Start", "Property");

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteProperty_APIsThrowExceptionForPriorWritePropertyOperationNotEnded()
        {
            var addressResource = new ODataResource { TypeName = "NS.Address" };
            var streetProperty = new ODataProperty { Name = "Street", Value = "Street 1" };
            var cityProperty = new ODataProperty { Name = "City", Value = "City 1" };

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync();

                        await writer.WriteStartAsync(addressResource);
                        await writer.WriteStartAsync(streetProperty);
                        // Missing: await writer.WriteEndAsync();
                        await writer.WriteStartAsync(cityProperty);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter();

                            writer.WriteStart(addressResource);
                            writer.WriteStart(streetProperty);
                            // Missing: writer.WriteEnd();
                            writer.WriteStart(cityProperty);
                        }
                    }));

            var exceptionMessage = Strings.ODataWriterCore_PropertyValueAlreadyWritten(streetProperty.Name);

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteProperty_APIsThrowExceptionForNonPrimitivePropertyValue()
        {
            var addressResource = new ODataResource { TypeName = "NS.Address" };
            var streetProperty = new ODataPropertyInfo { Name = "Street" };
            var nonPrimitivePropertyValue = new ODataResource
            {
                TypeName = "NS.Location",
                Properties = new List<ODataProperty> { new ODataProperty { Name = "City", Value = "City 1" } }
            };

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync();

                        await writer.WriteStartAsync(addressResource);
                        await writer.WriteStartAsync(streetProperty);
                        await writer.WriteStartAsync(nonPrimitivePropertyValue);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter();

                            writer.WriteStart(addressResource);
                            writer.WriteStart(streetProperty);
                            writer.WriteStart(nonPrimitivePropertyValue);
                        }
                    }));

            var exceptionMessage = Strings.ODataWriterCore_InvalidStateTransition("Property", "Resource");

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteTopLevelResourceSet_APIsThrowExceptionForResourceWriter()
        {
            var customerResourceSet = CreateResourceSet("Customers", "NS.Customer");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResourceSet);
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceSetWithResourceWriter, syncException.Message);
        }

        [Fact]
        public async Task WriteTopLevelDeltaResourceSet_APIsThrowExceptionForResourceSetWriter()
        {
            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer", new Uri($"{ServiceUri}/Customers/deltaLink"));

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerDeltaResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerDeltaResourceSet);
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, syncException.Message);
        }

        [Fact]
        public async Task WriteTopLevelResource_APIsThrowExceptionForResourceSetWriter()
        {
            var customerResource = CreateCustomerResource();

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResource);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_CannotWriteTopLevelResourceWithResourceSetWriter, syncException.Message);
        }

        [Fact]
        public async Task WriteNestedResourceSet_APIsThrowExceptionForResourceSetNotNestedInNestedResourceInfo()
        {
            var customerResource = CreateCustomerResource();
            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResource);
                        await writer.WriteStartAsync(orderResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                        using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteStart(orderResourceSet);
                        }
                    }));

            var expectedMessage = Strings.ODataWriterCore_InvalidTransitionFromResource("Resource", "ResourceSet");

            Assert.Equal(expectedMessage, asyncException.Message);
            Assert.Equal(expectedMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteNestedResource_APIsThrowExceptionForResourceNotNestedInNestedResourceInfo()
        {
            var orderResource = CreateOrderResource();
            var customerResource = CreateCustomerResource();

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        await writer.WriteStartAsync(orderResource);
                        await writer.WriteStartAsync(customerResource);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                        using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                            writer.WriteStart(orderResource);
                            writer.WriteStart(customerResource);
                        }
                    }));

            var expectedMessage = Strings.ODataWriterCore_InvalidTransitionFromResource("Resource", "Resource");

            Assert.Equal(expectedMessage, asyncException.Message);
            Assert.Equal(expectedMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteNestedResourceInfo_ThrowsExceptionForParentResourceIsNull()
        {
            var customerPropertyResourceInfo = CreateNestedResourceInfo("Customer");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.orderEntitySet, this.orderEntityType);

                        await writer.WriteStartAsync((ODataResource)null);
                        await writer.WriteStartAsync(customerPropertyResourceInfo);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                        using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.orderEntitySet, this.orderEntityType);

                            writer.WriteStart((ODataResource)null);
                            writer.WriteStart(customerPropertyResourceInfo);
                        }
                    }));

            var expectedMessage = Strings.ODataWriterCore_InvalidTransitionFromNullResource("Resource", "NestedResourceInfo");

            Assert.Equal(expectedMessage, asyncException.Message);
            Assert.Equal(expectedMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteNestedDeltaResourceSet_APIsThrowException()
        {
            var customerDeltaResourceSet = CreateDeltaResourceSet("Customers", "NS.Customer", new Uri($"{ServiceUri}/Customers/deltaLink"));
            var customerResource = CreateCustomerResource();
            var ordersPropertyNestedResourceInfo = CreateNestedResourceInfo("Orders", isCollection: true);
            var orderDeltaResourceSet = CreateDeltaResourceSet("Orders", "NS.Order");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerDeltaResourceSet);
                        await writer.WriteStartAsync(customerResource);
                        await writer.WriteStartAsync(ordersPropertyNestedResourceInfo);
                        await writer.WriteStartAsync(orderDeltaResourceSet);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerDeltaResourceSet);
                            writer.WriteStart(customerResource);
                            writer.WriteStart(ordersPropertyNestedResourceInfo);
                            writer.WriteStart(orderDeltaResourceSet);
                        }
                    }));

            var expectedMessage = Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeltaResourceSet");

            Assert.Equal(expectedMessage, asyncException.Message);
            Assert.Equal(expectedMessage, syncException.Message);
        }

        [Theory]
        [InlineData(DeltaDeletedEntryReason.Deleted)]
        [InlineData(DeltaDeletedEntryReason.Changed)]
        public async Task WriteNestedDeletedResource_APIsThrowException(DeltaDeletedEntryReason reason)
        {
            var orderDeltaResourceSet = CreateDeltaResourceSet("Orders", "NS.Order", new Uri($"{ServiceUri}/Orders/deltaLink"));
            var orderResource = CreateOrderResource();
            var customerPropertyNestedResourceInfo = CreateNestedResourceInfo("Customer");
            var customerDeletedResource = CreateCustomerDeletedResource();
            customerDeletedResource.Reason = reason;

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataDeltaResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                        await writer.WriteStartAsync(orderDeltaResourceSet);
                        await writer.WriteStartAsync(orderResource);
                        await writer.WriteStartAsync(customerPropertyNestedResourceInfo);
                        await writer.WriteStartAsync(customerDeletedResource);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataDeltaResourceSetWriter(this.orderEntitySet, this.orderEntityType);

                            writer.WriteStart(orderDeltaResourceSet);
                            writer.WriteStart(orderResource);
                            writer.WriteStart(customerPropertyNestedResourceInfo);
                            writer.WriteStart(customerDeletedResource);
                        }
                    }));

            var expectedMessage = Strings.ODataWriterCore_InvalidTransitionFromExpandedLink("NestedResourceInfoWithContent", "DeletedResource");

            Assert.Equal(expectedMessage, asyncException.Message);
            Assert.Equal(expectedMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteDeltaLink_APIsThrowExceptionForResourceSetWriter()
        {
            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");
            var deltaLink = new ODataDeltaLink(new Uri($"{ServiceUri}/Orders(1)"), new Uri($"{ServiceUri}/Customers(1)"), "Customer");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                        await writer.WriteStartAsync(orderResourceSet);
                        await writer.WriteDeltaLinkAsync(deltaLink);
                        await writer.WriteEndAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceSetWriter(this.orderEntitySet, this.orderEntityType);

                            writer.WriteStart(orderResourceSet);
                            writer.WriteDeltaLink(deltaLink);
                            writer.WriteEnd();
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, syncException.Message);
        }

        [Fact]
        public async Task WriteDeletedDeltaLink_APIsThrowExceptionForResourceSetWriter()
        {
            var orderResourceSet = CreateResourceSet("Orders", "NS.Order");
            var deltaDeletedLink = new ODataDeltaDeletedLink(new Uri($"{ServiceUri}/Orders(1)"), new Uri($"{ServiceUri}/Customers(1)"), "Customer");

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceSetWriterAsync(this.orderEntitySet, this.orderEntityType);

                        await writer.WriteStartAsync(orderResourceSet);
                        await writer.WriteDeltaDeletedLinkAsync(deltaDeletedLink);
                        await writer.WriteEndAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceSetWriter(this.orderEntitySet, this.orderEntityType);

                            writer.WriteStart(orderResourceSet);
                            writer.WriteDeltaDeletedLink(deltaDeletedLink);
                            writer.WriteEnd();
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_CannotWriteDeltaWithResourceSetWriter, syncException.Message);
        }

        [Fact]
        public async Task WriteStart_APIsThrowExceptionForResourceWriterInCompletedState()
        {
            var customerResource = CreateCustomerResource();

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResource);
                        await writer.WriteEndAsync();
                        // Try to start writing again
                        await writer.WriteStartAsync(customerResource);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                        using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                            // Try to start writing again
                            writer.WriteStart(customerResource);
                        }
                    }));

            var expected = Strings.ODataWriterCore_InvalidTransitionFromCompleted("Completed", "Resource");

            Assert.Equal(expected, asyncException.Message);
            Assert.Equal(expected, syncException.Message);
        }

        [Fact]
        public async Task WriteEnd_APIsThrowExceptionForResourceWriterInCompletedState()
        {
            var customerResource = CreateCustomerResource();

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                        await writer.WriteStartAsync(customerResource);
                        await writer.WriteEndAsync();
                        // Try to end writing again
                        await writer.WriteEndAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                        using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                            writer.WriteStart(customerResource);
                            writer.WriteEnd();
                            // Try to end writing again
                            writer.WriteEnd();
                        }
                    }));

            var expected = Strings.ODataWriterCore_WriteEndCalledInInvalidState("Completed");

            Assert.Equal(expected, asyncException.Message);
            Assert.Equal(expected, syncException.Message);
        }

        [Fact]
        public async Task WriteEnd_APIsThrowExceptionForBinaryStreamNotDisposed()
        {
            var addressResource = new ODataResource { TypeName = "NS.Address" };
            var mapProperty = new ODataStreamPropertyInfo { Name = "Map" };

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataResourceWriterAsync();

                        await writer.WriteStartAsync(addressResource);
                        await writer.WriteStartAsync(mapProperty);
                        // `using` intentionally not used so as not to trigger dispose
                        var stream = await writer.CreateBinaryWriteStreamAsync();
                        var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                        await stream.WriteAsync(bytes, 0, 10);
                        await stream.FlushAsync();

                        await writer.WriteEndAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataResourceWriter();

                            writer.WriteStart(addressResource);
                            writer.WriteStart(mapProperty);
                            // `using` intentionally not used so as not to trigger dispose
                            var stream = writer.CreateBinaryWriteStream();
                            var bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

                            stream.Write(bytes, 0, 10);
                            stream.Flush();

                            writer.WriteEnd();
                        }
                    }));

            Assert.Equal(Strings.ODataWriterCore_StreamNotDisposed, asyncException.Message);
            Assert.Equal(Strings.ODataWriterCore_StreamNotDisposed, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForChangesetStartedWithinChangeset()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        // Try to start writing a changeset when another is active
                        await writer.WriteStartChangesetAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            // Try to start writing a changeset when another is active
                            writer.WriteStartChangeset();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_CannotStartChangeSetWithActiveChangeSet, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForEndChangesetNotPrececededByStartChangeset()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        // Try to end changeset when there's none active
                        await writer.WriteEndChangesetAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            // Try to end changeset when there's none active
                            writer.WriteEndChangeset();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_CannotCompleteChangeSetWithoutActiveChangeSet, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForEndBatchBeforeEndChangeset()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        // Try to stop writing batch before changeset end
                        await writer.WriteEndBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            // Try to stop writing batch before changeset end
                            writer.WriteEndBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_CannotCompleteBatchWithActiveChangeSet, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForNoStartBatch()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        // Try to start writing changeset before batch start
                        await writer.WriteStartChangesetAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            // Try to start writing changeset before batch start
                            writer.WriteStartChangeset();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromStart, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromStart, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForMultipleStartBatch()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        // Try to start writing batch again
                        await writer.WriteStartBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            // Try to start writing batch again
                            writer.WriteStartBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromBatchStarted, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForStateTransitionAfterEndBatch()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteEndBatchAsync();
                        // Try to start writing batch after batch end
                        await writer.WriteStartBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteEndBatch();
                            // Try to start writing batch after batch end
                            writer.WriteStartBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromBatchCompleted, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromBatchCompleted, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForOperationResponseMessage()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.CreateOperationResponseMessageAsync("1");
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.CreateOperationResponseMessage("1");
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_CannotCreateResponseOperationWhenWritingRequest, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchResponse_APIsThrowExceptionForOperationRequestMessage()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataResponseMessage asyncResponseMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncResponseMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataResponseMessage syncResponseMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncResponseMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.CreateOperationRequestMessage(
                                "POST", new Uri($"{ServiceUri}/Customers"), "1");
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_CannotCreateRequestOperationWhenWritingResponse, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForInvalidTransitionFromEndChangeset()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        await writer.WriteEndChangesetAsync();
                        // Try to start writing batch after changeset end
                        await writer.WriteStartBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            writer.WriteEndChangeset();
                            // Try to start writing batch after changeset end
                            writer.WriteStartBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetCompleted, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForInvalidTransitionFromOperationStreamDisposed()
        {
            var customerResource = CreateCustomerResource();

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();

                        var operationRequestMessage = await writer.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                        {
                            var nestedWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                            await nestedWriter.WriteStartAsync(customerResource);
                            await nestedWriter.WriteEndAsync();
                        }

                        // Try to start writing batch after operation stream disposed
                        await writer.WriteStartBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();

                            var operationRequestMessage = writer.CreateOperationRequestMessage(
                                "POST", new Uri($"{ServiceUri}/Customers"), "1");

                            using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                            {
                                var nestedWriter = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                                nestedWriter.WriteStart(customerResource);
                                nestedWriter.WriteEnd();
                            }

                            // Try to start writing batch after operation stream disposed
                            writer.WriteStartBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamDisposed, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForInvalidTransitionFromOperationStreamRequested()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();

                        var operationRequestMessage = await writer.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                        {
                            var nestedWriter = await nestedMessageWriter.CreateODataResourceWriterAsync(this.customerEntitySet, this.customerEntityType);

                            // Try to end writing batch after operation stream requested
                            await writer.WriteEndBatchAsync();
                        }
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();

                            var operationRequestMessage = writer.CreateOperationRequestMessage(
                                "POST", new Uri($"{ServiceUri}/Customers"), "1");

                            using (var nestedMessageWriter = new ODataMessageWriter(operationRequestMessage))
                            {
                                var nestedWriter = nestedMessageWriter.CreateODataResourceWriter(this.customerEntitySet, this.customerEntityType);

                                // Try to end writing batch after operation stream requested
                                writer.WriteEndBatch();
                            }
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationContentStreamRequested, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForInvalidTransitionFromOperationCreated()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();

                        var operationRequestMessage = await writer.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");
                        // Try to start writing batch after operation created
                        await writer.WriteStartBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();

                            var operationRequestMessage = writer.CreateOperationRequestMessage(
                                "POST", new Uri($"{ServiceUri}/Customers"), "1");
                            // Try to start writing batch after operation created
                            writer.WriteStartBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationCreated, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromOperationCreated, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForInvalidTransitionFromChangesetStarted()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        // Try to start writing batch after changeset started
                        await writer.WriteStartBatchAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            // Try to start writing batch after changeset started
                            writer.WriteStartBatch();
                        }
                    }));

            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetStarted, asyncException.Message);
            Assert.Equal(Strings.ODataBatchWriter_InvalidTransitionFromChangeSetStarted, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForGetMethodInChangeset()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        // Try to create operation request message for a GET
                        await writer.CreateOperationRequestMessageAsync(
                            "GET", new Uri($"{ServiceUri}/Customers(1)"), "1");
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            // Try to create operation request message for a GET
                            writer.CreateOperationRequestMessage(
                                "GET", new Uri($"{ServiceUri}/Customers(1)"), "1");
                        }
                    }));

            var exceptionMessage = Strings.ODataBatch_InvalidHttpMethodForChangeSetRequest("GET");

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequestWithChangeset_APIsThrowExceptionForContentIdIsNull()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        // Try to create operation request message for a GET
                        await writer.CreateOperationRequestMessageAsync(
                            "PUT", new Uri($"{ServiceUri}/Customers(1)"), /*contentId*/ null);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            // Try to create operation request message with null content id
                            writer.CreateOperationRequestMessage(
                                "PUT", new Uri($"{ServiceUri}/Customers(1)"), /*contentId*/ null);
                        }
                    }));

            var exceptionMessage = Strings.ODataBatchOperationHeaderDictionary_KeyNotFound(ODataConstants.ContentIdHeader);

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForMaxPartsPerBatchLimitExceeded()
        {
            this.writerSettings.MessageQuotas.MaxPartsPerBatch = 1;

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        await writer.WriteEndChangesetAsync();
                        await writer.WriteStartChangesetAsync();
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            writer.WriteEndChangeset();
                            writer.WriteStartChangeset();
                        }
                    }));

            var exceptionMessage = Strings.ODataBatchWriter_MaxBatchSizeExceeded(1);

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForMaxOperationsPerChangesetLimitExceeded()
        {
            this.writerSettings.MessageQuotas.MaxOperationsPerChangeset = 1;

            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        await writer.CreateOperationRequestMessageAsync(
                            "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                        await writer.CreateOperationRequestMessageAsync(
                            "PUT", new Uri($"{ServiceUri}/Customers(2)"), "2");
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            writer.CreateOperationRequestMessage(
                                "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                            writer.CreateOperationRequestMessage(
                                "PUT", new Uri($"{ServiceUri}/Customers(2)"), "2");
                        }
                    }));

            var exceptionMessage = Strings.ODataBatchWriter_MaxChangeSetSizeExceeded(1);

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForDuplicateContentId()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync();
                        await writer.CreateOperationRequestMessageAsync(
                            "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                        await writer.CreateOperationRequestMessageAsync(
                            "PUT", new Uri($"{ServiceUri}/Customers(2)"), "1");
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset();
                            writer.CreateOperationRequestMessage(
                                "PUT", new Uri($"{ServiceUri}/Customers(1)"), "1");
                            writer.CreateOperationRequestMessage(
                                "PUT", new Uri($"{ServiceUri}/Customers(2)"), "1");
                        }
                    }));

            var exceptionMessage = Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed(1);

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        [Fact]
        public async Task WriteBatchRequest_APIsThrowExceptionForContentIdNotInTheSameAtomicityGroup()
        {
            var asyncException = await Assert.ThrowsAsync<ODataException>(
                async () =>
                {
                    IODataRequestMessage asyncRequestMessage = new InMemoryMessage { Stream = this.asyncStream };
                    using (var messageWriter = new ODataMessageWriter(asyncRequestMessage, this.writerSettings))
                    {
                        var writer = await messageWriter.CreateODataBatchWriterAsync();

                        await writer.WriteStartBatchAsync();
                        await writer.WriteStartChangesetAsync("fd04fc24");

                        await writer.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Customers"), "1");

                        await writer.WriteEndChangesetAsync();
                        await writer.WriteStartChangesetAsync("b62a2456");

                        // Operation request depends on content id from different atomicity group
                        var dependsOnIds = new List<string> { "1" };
                        await writer.CreateOperationRequestMessageAsync(
                            "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);
                    }
                });

            var syncException = await Assert.ThrowsAsync<ODataException>(
                () => TaskUtils.GetTaskForSynchronousOperation(
                    () =>
                    {
                        IODataRequestMessage syncRequestMessage = new InMemoryMessage { Stream = this.syncStream };
                        using (var messageWriter = new ODataMessageWriter(syncRequestMessage, this.writerSettings))
                        {
                            var writer = messageWriter.CreateODataBatchWriter();

                            writer.WriteStartBatch();
                            writer.WriteStartChangeset("fd04fc24");

                            writer.CreateOperationRequestMessage(
                                "POST", new Uri($"{ServiceUri}/Customers"), "1");

                            writer.WriteEndChangeset();
                            writer.WriteStartChangeset("b62a2456");

                            // Operation request depends on content id from different atomicity group
                            var dependsOnIds = new List<string> { "1" };
                            writer.CreateOperationRequestMessage(
                                "POST", new Uri($"{ServiceUri}/Orders"), "2", BatchPayloadUriOption.AbsoluteUri, dependsOnIds);
                        }
                    }));

            var exceptionMessage = Strings.ODataBatchReader_DependsOnIdNotFound("1", "2");

            Assert.Equal(exceptionMessage, asyncException.Message);
            Assert.Equal(exceptionMessage, syncException.Message);
        }

        #endregion Exception Cases

        private void InitializeEdmModel()
        {
            this.model = new EdmModel();

            this.colorEnumType = new EdmEnumType("NS", "Color");
            this.coordinateComplexType = new EdmComplexType("NS", "Coordinate");
            this.orderItemEntityType = new EdmEntityType("NS", "OrderItem");
            this.orderEntityType = new EdmEntityType("NS", "Order");
            this.customerEntityType = new EdmEntityType("NS", "Customer");
            this.superEntityType = new EdmEntityType("NS", "SuperEntity", baseType: null, isAbstract: false, isOpen: true);
            this.streamEntityType = new EdmEntityType("NS", "StreamEntity");

            this.colorEnumType.AddMember("Black", new EdmEnumMemberValue(1));
            this.colorEnumType.AddMember("White", new EdmEnumMemberValue(2));
            this.model.AddElement(this.colorEnumType);

            this.coordinateComplexType.AddStructuralProperty("Longitude", EdmPrimitiveTypeKind.Double);
            this.coordinateComplexType.AddStructuralProperty("Latitude", EdmPrimitiveTypeKind.Double);
            this.model.AddElement(this.coordinateComplexType);

            var orderItemIdProperty = this.orderItemEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.orderItemEntityType.AddKeys(orderItemIdProperty);
            this.orderItemEntityType.AddStructuralProperty("Price", EdmPrimitiveTypeKind.Decimal);
            this.orderItemEntityType.AddStructuralProperty("Quantity", EdmPrimitiveTypeKind.Int32);
            this.model.AddElement(this.orderItemEntityType);

            var orderIdProperty = this.orderEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.orderEntityType.AddKeys(orderIdProperty);
            this.orderEntityType.AddStructuralProperty("Amount", EdmPrimitiveTypeKind.Decimal);
            var customerNavProperty = this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Customer",
                    Target = this.customerEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });
            this.orderEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Items",
                    Target = this.orderItemEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                    ContainsTarget = true
                });
            this.model.AddElement(this.orderEntityType);

            var customerIdProperty = this.customerEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            this.customerEntityType.AddKeys(customerIdProperty);
            this.customerEntityType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            var ordersNavProperty = this.customerEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Orders",
                    Target = this.orderEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            this.model.AddElement(this.customerEntityType);

            var superEntityIdProperty = this.superEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            this.superEntityType.AddKeys(superEntityIdProperty);
            this.superEntityType.AddStructuralProperty("BooleanProperty", EdmPrimitiveTypeKind.Boolean, false);
            this.superEntityType.AddStructuralProperty("Int32Property", EdmPrimitiveTypeKind.Int32, false);
            this.superEntityType.AddStructuralProperty("SingleProperty", EdmPrimitiveTypeKind.Single, false);
            this.superEntityType.AddStructuralProperty("Int16Property", EdmPrimitiveTypeKind.Int16, false);
            this.superEntityType.AddStructuralProperty("Int64Property", EdmPrimitiveTypeKind.Int64, false);
            this.superEntityType.AddStructuralProperty("DecimalProperty", EdmPrimitiveTypeKind.Decimal, false);
            this.superEntityType.AddStructuralProperty("DoubleProperty", EdmPrimitiveTypeKind.Double, false);
            this.superEntityType.AddStructuralProperty("GuidProperty", EdmPrimitiveTypeKind.Guid, false);
            this.superEntityType.AddStructuralProperty("DateTimeOffsetProperty", EdmPrimitiveTypeKind.DateTimeOffset, false);
            this.superEntityType.AddStructuralProperty("TimeSpanProperty", EdmPrimitiveTypeKind.Duration, false);
            this.superEntityType.AddStructuralProperty("ByteProperty", EdmPrimitiveTypeKind.Byte, false);
            this.superEntityType.AddStructuralProperty("SignedByteProperty", EdmPrimitiveTypeKind.SByte, false);
            this.superEntityType.AddStructuralProperty("StringProperty", EdmPrimitiveTypeKind.String, false);
            this.superEntityType.AddStructuralProperty("ByteArrayProperty", EdmPrimitiveTypeKind.Binary, false);
            this.superEntityType.AddStructuralProperty("DateProperty", EdmPrimitiveTypeKind.Date, false);
            this.superEntityType.AddStructuralProperty("TimeOfDayProperty", EdmPrimitiveTypeKind.TimeOfDay, false);
            this.superEntityType.AddStructuralProperty("ColorProperty", new EdmEnumTypeReference(this.colorEnumType, false));
            this.superEntityType.AddStructuralProperty("CoordinateProperty", new EdmComplexTypeReference(this.coordinateComplexType, true));
            this.superEntityType.AddStructuralProperty("GeographyPointProperty", EdmPrimitiveTypeKind.GeographyPoint, false);
            this.superEntityType.AddStructuralProperty("GeometryPointProperty", EdmPrimitiveTypeKind.GeometryPoint, false);
            var entityNavProperty = this.superEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "EntityProperty",
                    Target = this.customerEntityType,
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne
                });
            this.superEntityType.AddStructuralProperty("BooleanCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetBoolean(false))));
            this.superEntityType.AddStructuralProperty("Int32CollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))));
            this.superEntityType.AddStructuralProperty("SingleCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSingle(false))));
            this.superEntityType.AddStructuralProperty("Int16CollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt16(false))));
            this.superEntityType.AddStructuralProperty("Int64CollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt64(false))));
            this.superEntityType.AddStructuralProperty("DecimalCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDecimal(false))));
            this.superEntityType.AddStructuralProperty("DoubleCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDouble(false))));
            this.superEntityType.AddStructuralProperty("GuidCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetGuid(false))));
            this.superEntityType.AddStructuralProperty("DateTimeOffsetCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDateTimeOffset(false))));
            this.superEntityType.AddStructuralProperty("TimeSpanCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDuration(false))));
            this.superEntityType.AddStructuralProperty("ByteCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetByte(false))));
            this.superEntityType.AddStructuralProperty("SignedByteCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSByte(false))));
            this.superEntityType.AddStructuralProperty("StringCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))));
            this.superEntityType.AddStructuralProperty("ByteArrayCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetBinary(false))));
            this.superEntityType.AddStructuralProperty("DateCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDate(false))));
            this.superEntityType.AddStructuralProperty("TimeOfDayCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetTimeOfDay(false))));
            this.superEntityType.AddStructuralProperty("ColorCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmEnumTypeReference(this.colorEnumType, false))));
            this.superEntityType.AddStructuralProperty("CoordinateCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(this.coordinateComplexType, true))));
            this.superEntityType.AddStructuralProperty("GeographyPointCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmPrimitiveTypeReference(
                    EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint), false))));
            this.superEntityType.AddStructuralProperty("GeometryPointCollectionProperty",
                new EdmCollectionTypeReference(new EdmCollectionType(new EdmPrimitiveTypeReference(
                    EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint), false))));
            var entityCollectionNavProperty = this.superEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "EntityCollectionProperty",
                    Target = this.customerEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many
                });
            this.model.AddElement(this.superEntityType);

            var streamEntityIdProperty = this.streamEntityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32, false);
            this.streamEntityType.AddKeys(streamEntityIdProperty);
            this.streamEntityType.AddStructuralProperty("Img", EdmPrimitiveTypeKind.Stream);
            this.model.AddElement(this.streamEntityType);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            this.model.AddElement(entityContainer);

            this.orderEntitySet = entityContainer.AddEntitySet("Orders", this.orderEntityType);
            this.customerEntitySet = entityContainer.AddEntitySet("Customers", this.customerEntityType);
            this.superEntitySet = entityContainer.AddEntitySet("SuperEntitySet", this.superEntityType);
            this.streamEntitySet = entityContainer.AddEntitySet("StreamEntitySet", this.streamEntityType);

            this.orderEntitySet.AddNavigationTarget(customerNavProperty, this.customerEntitySet);
            this.customerEntitySet.AddNavigationTarget(ordersNavProperty, this.orderEntitySet);
            this.superEntitySet.AddNavigationTarget(entityNavProperty, this.customerEntitySet);
            this.superEntitySet.AddNavigationTarget(entityCollectionNavProperty, this.customerEntitySet);
        }

        #region Helper Methods

        private static ODataResourceSet CreateResourceSet(
            string navigationSourceName,
            string entityTypeName,
            EdmNavigationSourceKind navigationSourceKind = EdmNavigationSourceKind.EntitySet)
        {
            return new ODataResourceSet
            {
                TypeName = $"Collection({entityTypeName})",
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = navigationSourceName,
                    ExpectedTypeName = entityTypeName,
                    NavigationSourceEntityTypeName = entityTypeName,
                    NavigationSourceKind = navigationSourceKind
                }
            };
        }

        private static ODataDeltaResourceSet CreateDeltaResourceSet(
            string navigationSource,
            string entityTypeName,
            Uri deltaLink = null,
            EdmNavigationSourceKind navigationSourceKind = EdmNavigationSourceKind.EntitySet)
        {
            return new ODataDeltaResourceSet
            {
                TypeName = $"Collection({entityTypeName})",
                DeltaLink = deltaLink,
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = navigationSource,
                    ExpectedTypeName = entityTypeName,
                    NavigationSourceEntityTypeName = entityTypeName,
                    NavigationSourceKind = navigationSourceKind
                }
            };
        }

        private static ODataResource CreateOrderResource()
        {
            return new ODataResource
            {
                Id = new Uri($"{ServiceUri}/Orders(1)"),
                TypeName = "NS.Order",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Amount", Value = 13M }
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Orders",
                    ExpectedTypeName = "NS.Order",
                    NavigationSourceEntityTypeName = "NS.Order",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataResource CreateCustomerResource(int id = 1)
        {
            return new ODataResource
            {
                Id = new Uri($"{ServiceUri}/Customers({id})"),
                TypeName = "NS.Customer",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = id,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Name", Value = $"Customer {id}" }
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Customers",
                    ExpectedTypeName = "NS.Customer",
                    NavigationSourceEntityTypeName = "NS.Customer",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private ODataDeletedResource CreateCustomerDeletedResource()
        {
            return new ODataDeletedResource
            {
                Id = new Uri($"{ServiceUri}/Customers(7)"),
                TypeName = "NS.Customer",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 7,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Name", Value = $"Customer 7" }
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "Customers",
                    ExpectedTypeName = "NS.Customer",
                    NavigationSourceEntityTypeName = "NS.Customer",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataResource CreateSuperEntityResource()
        {
            return new ODataResource
            {
                TypeName = "NS.SuperEntity",
                Properties = GetSuperTypeProperties(),
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "SuperEntitySet",
                    ExpectedTypeName = "NS.SuperEntity",
                    NavigationSourceEntityTypeName = "NS.SuperEntity",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataResource CreateCoordinateResource(double longitude = 47.64229583688d, double latitude = -122.13694393057d)
        {
            return new ODataResource
            {
                TypeName = "NS.Coordinate",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty { Name = "Longitude", Value = longitude },
                    new ODataProperty { Name = "Latitude", Value = latitude }
                }
            };
        }

        private static ODataResource CreateOrderItemResource()
        {
            return new ODataResource
            {
                TypeName = "NS.OrderItem",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    },
                    new ODataProperty { Name = "Price", Value = 13M },
                    new ODataProperty { Name = "Quantity", Value = 7 }
                }
            };
        }

        private static ODataResource CreateStreamEntityResource()
        {
            return new ODataResource
            {
                Id = new Uri($"{ServiceUri}/StreamEntitySet(1)"),
                TypeName = "NS.StreamEntity",
                Properties = new List<ODataProperty>
                {
                    new ODataProperty
                    {
                        Name = "Id",
                        Value = 1,
                        SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                    }
                },
                SerializationInfo = new ODataResourceSerializationInfo
                {
                    NavigationSourceName = "StreamEntitySet",
                    ExpectedTypeName = "NS.StreamEntity",
                    NavigationSourceEntityTypeName = "NS.StreamEntity",
                    NavigationSourceKind = EdmNavigationSourceKind.EntitySet
                }
            };
        }

        private static ODataNestedResourceInfo CreateNestedResourceInfo(
            string linkName,
            bool isComplex = false,
            bool isCollection = false,
            bool isUndeclared = false,
            Uri linkUrl = null)
        {
            return new ODataNestedResourceInfo
            {
                Name = linkName,
                Url = linkUrl,
                IsCollection = isCollection,
                SerializationInfo = new ODataNestedResourceInfoSerializationInfo
                {
                    IsComplex = isComplex,
                    IsUndeclared = isUndeclared
                }
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

        private static IList<ODataProperty> GetSuperTypeProperties()
        {
            return new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "Id",
                    Value = 1,
                    SerializationInfo = new ODataPropertySerializationInfo { PropertyKind = ODataPropertyKind.Key }
                },
                new ODataProperty { Name = "BooleanProperty", Value = true },
                new ODataProperty { Name = "Int32Property", Value = 13 },
                new ODataProperty { Name = "SingleProperty", Value = 3.142f },
                new ODataProperty { Name = "Int16Property", Value = (short)7 },
                new ODataProperty { Name = "Int64Property", Value = 6078747774547L },
                new ODataProperty { Name = "DoubleProperty", Value = 3.14159265359d },
                new ODataProperty { Name = "DecimalProperty", Value = 7654321m },
                new ODataProperty { Name = "GuidProperty", Value = new Guid(23, 59, 59, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }) },
                new ODataProperty { Name = "DateTimeOffsetProperty", Value = new DateTimeOffset(new DateTime(1970, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc)) },
                new ODataProperty { Name = "TimeSpanProperty", Value = new TimeSpan(23, 59, 59) },
                new ODataProperty { Name = "ByteProperty", Value = (byte)1 },
                new ODataProperty { Name = "SignedByteProperty", Value = (sbyte)9 },
                new ODataProperty { Name = "StringProperty", Value = "foo" },
                new ODataProperty { Name = "ByteArrayProperty", Value = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 } },
                new ODataProperty { Name = "DateProperty", Value = new Date(1970, 1, 1) },
                new ODataProperty { Name = "TimeOfDayProperty", Value = new TimeOfDay(23, 59, 59, 0) },
                new ODataProperty { Name = "ColorProperty", Value = new ODataEnumValue("Black") },
                new ODataProperty { Name = "GeographyPointProperty", Value = GeographyPoint.Create(22.2, 22.2) },
                new ODataProperty { Name = "GeometryPointProperty", Value = GeometryPoint.Create(7, 13) },
                new ODataProperty { Name = "BooleanCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { true, false } } },
                new ODataProperty { Name = "Int32CollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { 13, 31 } } },
                new ODataProperty { Name = "SingleCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { 3.142f, 241.3f } } },
                new ODataProperty { Name = "Int16CollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { (short)7, (short)11 } } },
                new ODataProperty { Name = "Int64CollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { 6078747774547L, 7454777478706L } } },
                new ODataProperty { Name = "DoubleCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { 3.14159265359d, 95356295141.3d } } },
                new ODataProperty { Name = "DecimalCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { 7654321m, 1234567m } } },
                new ODataProperty { Name = "GuidCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    new Guid(23, 59, 59, new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }),
                    new Guid(11, 29, 29, new byte[] { 7, 6, 5, 4, 3, 2, 1, 0 }),
                } } },
                new ODataProperty { Name = "DateTimeOffsetCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    new DateTimeOffset(new DateTime(1970, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc)),
                    new DateTimeOffset(new DateTime(1858, 11, 17, 11, 29, 29, 0, DateTimeKind.Utc))
                } } },
                new ODataProperty { Name = "TimeSpanCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    new TimeSpan(23, 59, 59),
                    new TimeSpan(11, 29, 29)
                } } },
                new ODataProperty { Name = "ByteCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { (byte)1, (byte)9 } } },
                new ODataProperty { Name = "SignedByteCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { (sbyte)9, (sbyte)1 } } },
                new ODataProperty { Name = "StringCollectionProperty", Value = new ODataCollectionValue { Items = new List<object> { "foo", "bar" } } },
                new ODataProperty { Name = "ByteArrayCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 },
                    new byte[] { 0, 9, 8, 7, 6, 5, 4, 3, 2, 1 }
                } } },
                new ODataProperty { Name = "DateCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    new Date(1970, 12, 31),
                    new Date(1858, 11, 17)
                } } },
                new ODataProperty { Name = "TimeOfDayCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    new TimeOfDay(23, 59, 59, 0),
                    new TimeOfDay(11, 29, 29, 0)
                } } },
                new ODataProperty { Name = "ColorCollectionProperty", Value = new ODataCollectionValue { TypeName = "NS.Color", Items = new List<object>
                {
                    new ODataEnumValue("Black"),
                    new ODataEnumValue("White")
                } } },
                new ODataProperty { Name = "GeographyPointCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    GeographyPoint.Create(22.2, 22.2),
                    GeographyPoint.Create(11.9, 11.6)
                } } },
                new ODataProperty { Name = "GeometryPointCollectionProperty", Value = new ODataCollectionValue { Items = new List<object>
                {
                    GeometryPoint.Create(7, 13),
                    GeometryPoint.Create(13, 7)
                } } },
            };
        }

        #endregion Helper Methods
    }
}
