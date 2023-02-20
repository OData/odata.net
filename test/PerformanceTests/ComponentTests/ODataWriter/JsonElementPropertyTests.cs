//---------------------------------------------------------------------
// <copyright file="JsonElementPropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using Microsoft.OData.Edm;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.OData.Json;

namespace Microsoft.OData.Performance
{
    [MemoryDiagnoser]
    public class JsonElementPropertyTests
    {
        private static readonly IEdmModel Model = TestUtils.GetAdventureWorksModel();
        private static readonly byte[] JsonPayload = TestUtils.ReadTestResource("EntryWithExpansions.json");
        private static readonly IEdmEntitySet entitySet = Model.FindDeclaredEntitySet("Product");
        private const int MaxStreamSize = 220000000;
        private const int NumEntries = 100;
        private JsonDocument ParsedPayload;

        private static readonly Stream WriteStream = new MemoryStream(MaxStreamSize);

        [GlobalSetup]
        public void Setup()
        {
            ParsedPayload = JsonDocument.Parse(JsonPayload);
        }

        [GlobalCleanup]
        public void Teardown()
        {
            ParsedPayload.Dispose();
        }

        [IterationSetup]
        public void ResetStream()
        {
            WriteStream.Seek(0, SeekOrigin.Begin);
        }

        [Benchmark]
        public void WriteParsedJsonWithJsonElementValues_DefaultWriter()
        {
            WriteParsedJsonWithJsonElementValues();
        }

        [Benchmark]
        public void WriteParsedJsonWithJsonElementValues_Utf8JsonWriter()
        {
            WriteParsedJsonWithJsonElementValues(builder =>
            {
                builder.AddService<IStreamBasedJsonWriterFactory>(ServiceLifetime.Singleton, _ => DefaultStreamBasedJsonWriterFactory.Default);
            });
        }

        [Benchmark]
        public void WriteParsedJsonWithoutJsonElementValues_DefaultWriter()
        {
            WriteParsedJsonWithoutJsonElementValues();
        }

        [Benchmark]
        public void WriteParsedJsonWithoutJsonElementValues_Utf8JsonWriter()
        {
            WriteParsedJsonWithoutJsonElementValues(builder =>
            {
                builder.AddService<IStreamBasedJsonWriterFactory>(ServiceLifetime.Singleton, _ => DefaultStreamBasedJsonWriterFactory.Default);
            });
        }

        private void WriteParsedJsonWithJsonElementValues(Action<IContainerBuilder> configureServices = null)
        {
            using (var messageWriter = ODataMessageHelper.CreateMessageWriter(WriteStream, Model, ODataMessageKind.Response, isFullValidation: true, configureServices: configureServices))
            {
                ODataWriter writer = messageWriter.CreateODataResourceSetWriter(entitySet, entitySet.EntityType());
                writer.WriteStart(new ODataResourceSet { Id = new Uri("http://www.odata.org/Perf.svc") });

                for (int i = 0; i < NumEntries; i++)
                {
                    List<ODataProperty> properties = new List<ODataProperty>();
                    foreach (var property in ParsedPayload.RootElement.EnumerateObject())
                    {
                        if (property.Name.Contains('@'))
                        {
                            continue;
                        }
                        properties.Add(new ODataProperty
                        {
                            Name = property.Name,
                            Value = new ODataJsonElementValue(property.Value)
                        });
                    }

                    ODataResource entry = new ODataResource { Properties = properties };
                    writer.WriteStart(entry);
                    writer.WriteEnd();
                }

                writer.WriteEnd();
                writer.Flush();
            }
        }

        private void WriteParsedJsonWithoutJsonElementValues(Action<IContainerBuilder> configureServices = null)
        {
            using (var messageWriter = ODataMessageHelper.CreateMessageWriter(WriteStream, Model, ODataMessageKind.Response, isFullValidation: true, configureServices: configureServices))
            {
                ODataWriter writer = messageWriter.CreateODataResourceSetWriter(entitySet, entitySet.EntityType());
                writer.WriteStart(new ODataResourceSet { Id = new Uri("http://www.odata.org/Perf.svc") });

                for (int i = 0; i < NumEntries; i++)
                {
                    var root = ParsedPayload.RootElement;

                    ODataResource entry = new ODataResource
                    {
                        Properties = new[]
                        {
                            CreateIntProperty("ProductID", root),
                            CreateStringProperty("Name", root),
                            CreateStringProperty("ProductNumber", root),
                            CreateBoolProperty("MakeFlag", root),
                            CreateBoolProperty("FinishedGoodsFlag", root),
                            CreateStringProperty("Color", root),
                            CreateShortProperty("SafetyStockLevel", root),
                            CreateShortProperty("ReorderPoint", root),
                            CreateDecimalProperty("StandardCost", root),
                            CreateDecimalProperty("ListPrice", root),
                            CreateNullableIntProperty("Size", root),
                            CreateStringProperty("SizeUnitMeasureCode", root),
                            CreateStringProperty("WeightUnitMeasureCode", root),
                            CreateDecimalProperty("Weight", root),
                            CreateIntProperty("DaysToManufacture", root),
                            CreateStringProperty("ProductLine", root),
                            CreateStringProperty("Class", root),
                            CreateStringProperty("Style", root),
                            CreateIntProperty("ProductSubcategoryID", root),
                            CreateStringProperty("ProductModelID", root),
                            CreateDateProperty("SellStartDate", root),
                            CreateStringProperty("SellEndDate", root),
                            CreateDateProperty("DiscontinuedDate", root),
                            CreateGuidProperty("rowguid", root),
                            CreateDateProperty("ModifiedDate", root),
                            CreateIntProperty("OpenProperty0", root),
                            CreateIntProperty("OpenProperty1", root),
                            CreateDecimalProperty("OpenProperty2", root),
                            CreateIntProperty("OpenProperty4", root),
                            CreateProperty("LuckyNumbers",
                                new ODataCollectionValue {
                                    TypeName = "Collection(Edm.Int64)",
                                    Items = root.GetProperty("LuckyNumbers").EnumerateArray().Select(value => (object)value.GetInt64())
                                }
                            )
                        }
                    };

                    writer.WriteStart(entry);

                    JsonElement timeZones = root.GetProperty("TimeZones");
                    writer.WriteStart(new ODataNestedResourceInfo { Name = "TimeZones", IsCollection = true, });
                    writer.WriteStart(new ODataResourceSet { });

                    foreach (JsonElement timeZone in root.GetProperty("TimeZones").EnumerateArray())
                    {
                        var timeZoneResource = new ODataResource
                        {
                            TypeName = "PerformanceServices.Edm.AdventureWorks.TimeZone",
                            Properties = new[]
                            {
                                CreateDateProperty("Offset", timeZone),
                                CreateTimeSpanProperty("StartTime", timeZone)
                            }
                        };

                        writer.WriteStart(timeZoneResource);
                        writer.WriteEnd();
                    }

                    writer.WriteEnd(); // Timezones resource set
                    writer.WriteEnd(); // TimeZones nested resource info

                    writer.WriteEnd(); // entry
                }

                writer.WriteEnd();
                writer.Flush();
            }
        }

        private static ODataProperty CreateStringProperty(string name, JsonElement json) =>
            CreateProperty(name, json.GetProperty(name).GetString());

        private static ODataProperty CreateGuidProperty(string name, JsonElement json) =>
           CreateProperty(name, json.GetProperty(name).GetGuid());

        private static ODataProperty CreateBoolProperty(string name, JsonElement json) =>
            CreateProperty(name, json.GetProperty(name).GetBoolean());

        private static ODataProperty CreateIntProperty(string name, JsonElement json) =>
            CreateProperty(name, json.GetProperty(name).GetInt32());
        private static ODataProperty CreateShortProperty(string name, JsonElement json) =>
            CreateProperty(name, json.GetProperty(name).GetInt16());

        private static ODataProperty CreateNullableIntProperty(string name, JsonElement json)
        {
            var property = json.GetProperty(name);
            int? value = property.ValueKind == JsonValueKind.Null ? null : (int?)property.GetInt32();
            return CreateProperty(name, value);
        }

        private static ODataProperty CreateDoubleProperty(string name, JsonElement json) =>
            CreateProperty(name, json.GetProperty(name).GetDouble());

        private static ODataProperty CreateDecimalProperty(string name, JsonElement json) =>
            CreateProperty(name, json.GetProperty(name).GetDecimal());

        private static ODataProperty CreateDateProperty(string name, JsonElement json) =>
            CreateProperty(name, DateTimeOffset.Parse(json.GetProperty(name).GetString()));

        private static ODataProperty CreateTimeSpanProperty(string name, JsonElement json) =>
            CreateProperty(name, XmlConvert.ToTimeSpan(json.GetProperty(name).GetString()));

        private static ODataProperty CreateProperty(string name, object value) => new ODataProperty { Name = name, Value = value };
    }
}
