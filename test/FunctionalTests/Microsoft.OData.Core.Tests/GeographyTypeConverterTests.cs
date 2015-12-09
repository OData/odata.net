//---------------------------------------------------------------------
// <copyright file="GeographyTypeConverterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.OData.Core.Json;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Core.Tests
{
    public class GeographyTypeConverterTests
    {
        [Fact]
        public void FastGeoJsonWriterShouldSerializeSameGeographyAsGeoJsonObjectWriter()
        {
            var geographys = new Geography[]
            {
                GeographyFactory.Polygon().Ring(33.1, -110).LineTo(1,2).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).Build(),
                GeographyFactory.Point(32, -100).Build(),
                GeographyFactory.LineString(32, -100).LineTo(0, 100).LineTo(0.9, -10.3).LineTo(16.85, 35).Build(),
                GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(0.1, 0.1).Build(),
                GeographyFactory.MultiPolygon().Polygon().Ring(33.1, -110).LineTo(35.97, -110.15).LineTo(11.45, 87.75).LineTo(-1, -0.9).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).LineTo(9.01, 1).Polygon().Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).LineTo(0.9, 100.5).Ring(33.1, -110).LineTo(35.97, -110.15).LineTo(11.45, 87.75).LineTo(88.77, 33.55).Build()
            };

            var converter = new GeographyTypeConverter();
            ValidateSerializationResultShouldBeSame(OriginalWriteJsonLight, (instance, writer) => converter.WriteJsonLight(instance, writer), geographys);
        }

        [Fact]
        public void FastGeoJsonWriterShouldSerializeSameGeometryAsGeoJsonObjectWriter()
        {
            var geometrys = new Geometry[]
            {
                GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(0.1, 0.1).Build(),
                GeometryFactory.Polygon().Ring(33.1, -110).LineTo(1,2).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(0.03, -0.01).LineTo(45.23, 23.10).Build()
            };

            var converter = new GeometryTypeConverter();
            ValidateSerializationResultShouldBeSame(OriginalWriteJsonLight, (instance, writer) => converter.WriteJsonLight(instance, writer), geometrys);
        }

        /// <summary>
        /// Validate if the serialization result by writer equals to the one by baseline writer.
        /// </summary>
        /// <param name="baselineWriter">The baseline Json writer.</param>
        /// <param name="writer">The Json writer to be validated.</param>
        /// <param name="instances">The spatial instances to write.</param>
        private static void ValidateSerializationResultShouldBeSame(Action<object, IJsonWriter> baselineWriter, Action<object, IJsonWriter> writer, ICollection<object> instances)
        {
            var baseline = SerializeSpatialInstances(baselineWriter, instances);
            var output = SerializeSpatialInstances(writer, instances);
            Assert.Equal(baseline, output);
        }

        /// <summary>
        /// Serialize a set of spatial instances using the specified JsonWriter.
        /// </summary>
        /// <param name="writer">The Json writer.</param>
        /// <param name="instances">The spatial instances to write.</param>
        /// <returns>UTF8-Encoded serialization result.</returns>
        private static string SerializeSpatialInstances(Action<object, IJsonWriter> writer, IEnumerable<object> instances)
        {
            var stream = new MemoryStream();
            var textWriter = new StreamWriter(stream, Encoding.UTF8);
            var jsonWriter = new JsonWriter(textWriter, false, ODataFormat.Json, false);
            var textReader = new StreamReader(stream, Encoding.UTF8);

            foreach (var instance in instances)
            {
                writer(instance, jsonWriter);
            }

            jsonWriter.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return textReader.ReadToEnd();
        }

        /// <summary>
        /// Original implementation of writing a spatial instance.
        /// </summary>
        /// <param name="instance">The spatial instance to write.</param>
        /// <param name="jsonWriter">The Json writer.</param>
        private static void OriginalWriteJsonLight(object instance, IJsonWriter jsonWriter)
        {
            IDictionary<string, object> jsonObject = GeoJsonObjectFormatter.Create().Write((ISpatial)instance);
            jsonWriter.WriteJsonObjectValue(jsonObject, /*injectPropertyAction*/ null);
        }
    }
}
