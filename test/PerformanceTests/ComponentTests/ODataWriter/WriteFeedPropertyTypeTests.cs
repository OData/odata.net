//---------------------------------------------------------------------
// <copyright file="WriteFeedPropertyTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::Xunit;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Xunit.Performance;

    public class WriteFeedPropertyTypeTests
    {
        private static readonly IEdmModel Model = TestUtils.GetEntityWithDifferentPropertyTypeModel();
        private const int NumberOfEntries = 10000;
        private static readonly int MaxStreamSize = 220000000;
        private static Stream WriteStream = new MemoryStream(MaxStreamSize);

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithStringSet()
        {
            TestAndMeasure("EntityStringSet");
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithInt32Set()
        {
            TestAndMeasure("EntityInt32Set");
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithInt64Set()
        {
            TestAndMeasure("EntityInt64Set");
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithDecimalSet()
        {
            TestAndMeasure("EntityDecimalSet");
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithDateTimeOffsetSet()
        {
            TestAndMeasure("EntityDateTimeOffsetSet");
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithGuidSet()
        {
            TestAndMeasure("EntityGuidSet");
        }

        [Benchmark]
        [MeasureGCAllocations]
        public void WriteFeedWithMixedSet()
        {
            TestAndMeasure("EntityMixedSet");
        }

        private void TestAndMeasure(string propertyType)
        {
            var entry = GenerateEntry(propertyType);
            var entitySet = Model.EntityContainer.FindEntitySet(propertyType);

            foreach (var iteration in Benchmark.Iterations)
            {
                // Reuse the same stream
                WriteStream.Seek(0, SeekOrigin.Begin);

                using (iteration.StartMeasurement())
                {
                    using (var messageWriter = ODataMessageHelper.CreateMessageWriter(WriteStream, Model, ODataMessageKind.Response, isFullValidation: true))
                    {
                        ODataWriter writer = messageWriter.CreateODataResourceSetWriter(entitySet, entitySet.EntityType());
                        writer.WriteStart(new ODataResourceSet { Id = new Uri("http://www.odata.org/Perf.svc") });
                        for (int i = 0; i < NumberOfEntries; ++i)
                        {
                            writer.WriteStart(entry);
                            writer.WriteEnd();
                        }

                        writer.WriteEnd();
                        writer.Flush();
                    }
                }
            }
        }

        private ODataResource GenerateEntry(string name)
        {
            var entitySet = Model.EntityContainer.FindEntitySet(name);

            ODataResource entry = new ODataResource();

            List<ODataProperty> properties = new List<ODataProperty>();

            foreach (IEdmProperty prop in entitySet.EntityType().Properties())
            {
                if (prop.PropertyKind == EdmPropertyKind.Structural)
                {
                    properties.Add(new ODataProperty()
                    {
                        Name = prop.Name,
                        Value = GetValue(prop.Type.ShortQualifiedName()),
                    });
                }
            }
            entry.Properties = properties;

            return entry;
        }

        private object GetValue(string typeName)
        {
            switch (typeName)
            {
                case "String":
                    return "abcdefghijklmnopqrstuvwxyz1234567";
                case "Int32":
                    return Int32.MaxValue;
                case "Int64":
                    return Int64.MaxValue;
                case "Decimal":
                    return Decimal.MaxValue;
                case "DateTimeOffset":
                    return DateTimeOffset.Now;
                case "Guid":
                    return Guid.NewGuid();
                default:
                    return null;
            }
        }
    }
}
