//---------------------------------------------------------------------
// <copyright file="SpatialServerIntegration.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Test.Astoria;
using System.IO;
using System.Linq;
using Microsoft.Spatial;
using System.Text;
using System.Xml;
using AstoriaUnitTests.Stubs;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace AstoriaUnitTests.Tests
{
    [TestClass]
    public class SpatialServerIntegration
    {
        private static WellKnownTextSqlFormatter wktFormatter;
        private static GeoJsonObjectFormatter jsonFormatter;
        private static GmlFormatter gmlFormatter;

        private static Dictionary<SpatialType, String[]> testData = new Dictionary<SpatialType, String[]>()
        {
            { SpatialType.Point, new String[] {
                "POINT EMPTY",
                "POINT(10 20)",
                "SRID=1234;POINT(10 20)",
                "POINT(10 20 30)",
                "POINT(10 20 NULL 40)",
                "POINT(10 20 30 40)" }},
            { SpatialType.LineString, new String[] {
                "LINESTRING EMPTY",
                "LINESTRING(10 20 30, 20 30, 30 40 50 60)",
                "LINESTRING(10 20 NULL 30, 20 30 NULL 40, 30 40, 40 50 NULL 60)",
                "LINESTRING(10 20, 20 30)",
                "SRID=1234;LINESTRING(10 20, 30 40)" }},
            { SpatialType.Polygon, new String[] {
                "POLYGON EMPTY",
                "POLYGON((10 20, 20 30 NULL 40, 30 40 NULL 50, 10 20))",
                "POLYGON((10 20, 20 30, 30 40, 10 20))",
                "SRID=1234;POLYGON((10 20, 20 30, 30 40, 10 20))" }},
            { SpatialType.MultiPoint, new String[] {
                "MULTIPOINT EMPTY",
                "MULTIPOINT((10 20), (20 30))",
                "SRID=1234;MULTIPOINT((20 30), (30 40), (40 50))" }},
            { SpatialType.MultiLineString, new String[] {
                "MULTILINESTRING EMPTY",
                "MULTILINESTRING((10 20, 20 30), (20 30, 30 40))",
                "SRID=1234;MULTILINESTRING((20 30, 30 40), (30 40, 40 50))" }},
            { SpatialType.MultiPolygon, new String[] {
                "MULTIPOLYGON EMPTY",
                "MULTIPOLYGON(((10 20, 20 30, 30 40, 10 20)), ((15 25, 25 35, 35 45, 15 25)))",
                "SRID=1234;MULTIPOLYGON (((10 10, 10 20, 20 20, 20 15 , 10 10), (50 40, 50 50, 60 50, 60 40, 50 40)))" }},
            { SpatialType.Collection, new String[] {
                "GEOMETRYCOLLECTION EMPTY",
                "GEOMETRYCOLLECTION(POINT(10 20), LINESTRING(10 20, 20 30), MULTIPOINT((10 20), (20 30)))",
                "SRID=1234; GEOMETRYCOLLECTION(POLYGON((10 20, 20 30, 30 40, 10 20)), MULTILINESTRING((10 20, 20 30), (20 30, 30 40)))" }},
            // JSON does not handle FullGlobe
            /* { SpatialType.FullGlobe, new String[] {
                "FULLGLOBE",
                "SRID=1234;FULLGLOBE" }} */
        };

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            wktFormatter = WellKnownTextSqlFormatter.Create();
            jsonFormatter = GeoJsonObjectFormatter.Create();
            gmlFormatter = GmlFormatter.Create();
        }

        [TestCategory("Partition2"), TestMethod]
        public void SpatialAsKeys()
        {
            var types = testData.Select(kvp => SpatialTestUtil.GeographyTypeFor(kvp.Key))
                .Union(testData.Select(kvp => SpatialTestUtil.GeometryTypeFor(kvp.Key)))
                .Union(new Type[] { typeof(Geography), typeof(Geometry) })
                .ToArray();

            TestUtil.RunCombinations(types, (t) =>
            {
                DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
                var entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddResourceSet("Entities", entityType);
                Exception ex = TestUtil.RunCatching<InvalidOperationException>(
                    () => metadata.AddKeyProperty(entityType, "ID", t));

                Assert.IsNotNull(ex);
                Assert.AreEqual(DataServicesResourceUtil.GetString("ResourceType_SpatialKeyOrETag", "ID", "EntityType"), ex.Message);
            });
        }

        [TestCategory("Partition2"), TestMethod]
        public void SpatialAsETags()
        {
            var types = testData.Select(kvp => SpatialTestUtil.GeographyTypeFor(kvp.Key))
                .Union(testData.Select(kvp => SpatialTestUtil.GeometryTypeFor(kvp.Key)))
                .Union(new Type[] { typeof(Geography), typeof(Geometry) })
                .ToArray();

            TestUtil.RunCombinations(types, (t) =>
            {
                DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
                var entityType = metadata.AddEntityType("EntityType", null, null, false);
                metadata.AddResourceSet("Entities", entityType);
                metadata.AddKeyProperty(entityType, "ID", typeof(int));
                Exception ex = TestUtil.RunCatching<InvalidOperationException>(
                    () => metadata.AddPrimitiveProperty(entityType, "Property", t, true));

                Assert.IsNotNull(ex);
                Assert.AreEqual(DataServicesResourceUtil.GetString("ResourceType_SpatialKeyOrETag", "Property", "EntityType"), ex.Message);
            });
        }

        private void JsonSpatialValueFromDictionary(IDictionary<string, object> dictionary, StringBuilder output)
        {
            output.Append("{");
            bool first = true;
            foreach (var property in dictionary)
            {
                if (!first)
                {
                    output.Append(", ");
                }

                output.AppendFormat("'{0}': ", property.Key);
                JsonSpatialValueFromObject(property.Value, output);
                first = false;
            }

            output.Append("}");
        }

        private void JsonSpatialValueFromObject(object jsonObject, StringBuilder output)
        {
            if (jsonObject == null)
            {
                output.Append("null");
                return;
            }

            var valueAsDictionary = jsonObject as IDictionary<string, object>;
            if (valueAsDictionary != null)
            {
                JsonSpatialValueFromDictionary(valueAsDictionary, output);
                return;
            }

            var valueAsString = jsonObject as string;
            if (valueAsString != null)
            {
                output.AppendFormat("'{0}'", valueAsString);
                return;
            }

            var valueAsArray = jsonObject as IEnumerable;
            if (valueAsArray != null)
            {
                output.Append("[");
                bool first = true;
                foreach (var element in valueAsArray)
                {
                    if (!first)
                    {
                        output.Append(", ");
                    }

                    JsonSpatialValueFromObject(element, output);
                    first = false;
                }

                output.Append("]");
                return;
            }

            output.Append(jsonObject.ToString());
        }

        /// <summary>
        /// Create a service for a specific spatial property type.
        /// </summary>
        /// <remarks>
        /// DEVNOTE(pqian):
        /// The service will populate with properties named after the property type, with a sequence number
        /// indicating which test sample it holds. For example, if the method is called with 3 sample data, 
        /// and the target type is GeographyPoint, then the service will look like
        /// EntityType:
        /// ID
        /// GeographyPoint1
        /// GeographyPoint2
        /// GeographyPoint3
        /// </remarks>
        /// <typeparam name="T">The target type</typeparam>
        /// <param name="data">Sample Data</param>
        /// <param name="overrideType">Use this type instead of typeof(T)</param>
        /// <param name="openProperty">Use Open Type</param>
        /// <param name="writable">Writable service</param>
        /// <returns>The constructed service definition</returns>
        private DSPServiceDefinition CreateSpatialPropertyService<T>(T[] data, Type overrideType = null, bool openProperty = false, bool writable = false)
        {
            Type propertyType = overrideType ?? typeof(T);

            DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddResourceSet("Entities", entityType);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));

            if (!openProperty)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    metadata.AddPrimitiveProperty(entityType, propertyType.Name + i, propertyType);
                }
            }

            entityType.IsOpenType = openProperty;

            var definition = new DSPServiceDefinition()
            {
                Metadata = metadata
            };

            if (writable)
            {
                definition.CreateDataSource = (m) => new DSPContext();
                definition.Writable = true;
            }
            else
            {
                DSPContext dataSource = new DSPContext();
                var entities = dataSource.GetResourceSetEntities("Entities");
                var resource = new DSPResource(entityType);
                resource.SetValue("ID", 0);

                for (int i = 0; i < data.Length; ++i)
                {
                    if (!openProperty)
                    {
                        metadata.AddPrimitiveProperty(entityType, propertyType.Name + i, propertyType);
                    }

                    resource.SetValue(propertyType.Name + i, data[i]);
                }

                entities.Add(resource);
                definition.DataSource = dataSource;
            }

            return definition;
        }

        private DSPServiceDefinition CreateCollectionWriteService(List<Type> types)
        {
            DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddResourceSet("Entities", entityType);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            foreach (Type t in types)
            {
                metadata.AddCollectionProperty(entityType, "Collection" + t.Name, t);
            }

            return new DSPServiceDefinition()
            {
                Metadata = metadata,
                Writable = true,
                CreateDataSource = (m) => new DSPContext()
            };
        }

        private DSPServiceDefinition CreateCollectionReadService(Dictionary<Type, object> data)
        {
            DSPMetadata metadata = new DSPMetadata("SpatialServerIntegration", "AstoriaUnitTests.Tests");
            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddResourceSet("Entities", entityType);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            foreach (Type t in data.Keys)
            {
                metadata.AddCollectionProperty(entityType, "Collection" + t.Name, t);
            }

            DSPContext dataSource = new DSPContext();
            var entities = dataSource.GetResourceSetEntities("Entities");
            var resource = new DSPResource(entityType);
            resource.SetValue("ID", 0);

            foreach (KeyValuePair<Type, object> d in data)
            {
                resource.SetValue("Collection" + d.Key.Name, d.Value);
            }

            entities.Add(resource);

            return new DSPServiceDefinition()
            {
                Metadata = metadata,
                DataSource = dataSource
            };
        }
    }
}

