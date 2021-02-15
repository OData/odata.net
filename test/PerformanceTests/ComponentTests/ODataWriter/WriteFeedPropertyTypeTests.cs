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
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using BenchmarkDotNet.Attributes;

    /// <summary>
    /// Measures the performance of the ODataWriter when handling
    /// different types of properties.
    /// </summary>
    [MemoryDiagnoser]
    public class WriteFeedPropertyTypeTests
    {
        private static readonly IEdmModel Model = TestUtils.GetEntityWithDifferentPropertyTypeModel();
        private const int NumberOfEntries = 10000;
        private static readonly int MaxStreamSize = 220000000;
        private static Stream WriteStream = new MemoryStream(MaxStreamSize);

        [Params("EntityStringSet",
            "EntityInt32Set",
            "EntityInt64Set",
            "EntityDecimalSet",
            "EntityDateTimeOffsetSet",
            "EntityGuidSet",
            "EntityMixedSet")]
        public string propertyType;
        public ODataResource entry;
        public IEdmEntitySet entitySet;

        [GlobalSetup]
        public void SetupEntryAndEntitySet()
        {
            entry = GenerateEntry(propertyType);
            entitySet = Model.EntityContainer.FindEntitySet(propertyType);
        }

        [IterationSetup]
        public void RewindStream()
        {
            // Reuse the same stream
            WriteStream.Seek(0, SeekOrigin.Begin);
        }

        [Benchmark]
        public void WriteFeed()
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
