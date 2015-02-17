//---------------------------------------------------------------------
// <copyright file="SpatialTestUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Service;
using System.Linq;
using Microsoft.Spatial;
using System.Text;
using System.Xml.Linq;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests
{
    using Microsoft.OData.Client;

    internal static class SpatialTestUtil
    {
        private static Dictionary<SpatialType, Type> spatialTypeToGeography = new Dictionary<SpatialType, Type> { 
            { SpatialType.Point, typeof(GeographyPoint) },
            { SpatialType.LineString, typeof(GeographyLineString) } ,
            { SpatialType.Polygon, typeof(GeographyPolygon) } ,
            { SpatialType.MultiPoint, typeof(GeographyMultiPoint) } ,
            { SpatialType.MultiLineString, typeof(GeographyMultiLineString) } ,
            { SpatialType.MultiPolygon, typeof(GeographyMultiPolygon) } ,
            { SpatialType.Collection, typeof(GeographyCollection) },
            { SpatialType.FullGlobe, typeof(GeographyFullGlobe) }
        };

        private static Dictionary<SpatialType, Type> spatialTypeToGeometry = new Dictionary<SpatialType, Type> { 
            { SpatialType.Point, typeof(GeometryPoint) },
            { SpatialType.LineString, typeof(GeometryLineString) } ,
            { SpatialType.Polygon, typeof(GeometryPolygon) } ,
            { SpatialType.MultiPoint, typeof(GeometryMultiPoint) } ,
            { SpatialType.MultiLineString, typeof(GeometryMultiLineString) } ,
            { SpatialType.MultiPolygon, typeof(GeometryMultiPolygon) } ,
            { SpatialType.Collection, typeof(GeometryCollection) }
        };

        private static Dictionary<SpatialType, String> spatialTypeToGeographicEdm = new Dictionary<SpatialType, String> { 
            { SpatialType.Point, "GeographyPoint" },
            { SpatialType.LineString, "GeographyLineString" } ,
            { SpatialType.Polygon, "GeographyPolygon" } ,
            { SpatialType.MultiPoint, "GeographyMultiPoint" } ,
            { SpatialType.MultiLineString, "GeographyMultiLineString" } ,
            { SpatialType.MultiPolygon, "GeographyMultiPolygon" } ,
            { SpatialType.Collection, "GeographyCollection" },            
            { SpatialType.FullGlobe, "Geography" }
        };

        private static Dictionary<SpatialType, String> spatialTypeToGeometricEdm = new Dictionary<SpatialType, String> { 
            { SpatialType.Point, "GeometryPoint" },
            { SpatialType.LineString, "GeometryLineString" } ,
            { SpatialType.Polygon, "GeometryPolygon" } ,
            { SpatialType.MultiPoint, "GeometryMultiPoint" } ,
            { SpatialType.MultiLineString, "GeometryMultiLineString" } ,
            { SpatialType.MultiPolygon, "GeometryMultiPolygon" } ,
            { SpatialType.Collection, "GeometryCollection" }
        };

        internal static readonly int DefaultId = 1;

        internal static Type GeographyTypeFor(SpatialType type)
        {
            return spatialTypeToGeography[type];
        }

        internal static String GeographyEdmNameFor(SpatialType type)
        {
            return spatialTypeToGeographicEdm[type];
        }

        internal static Type GeometryTypeFor(SpatialType type)
        {
            return spatialTypeToGeometry[type];
        }

        internal static String GeometryEdmNameFor(SpatialType type)
        {
            return spatialTypeToGeometricEdm[type];
        }

        internal static DSPMetadata CreateRoadTripMetadata(Type geographyPropertyType, bool useComplexType = false, bool useOpenTypes = false, Action<DSPMetadata> modifyMetadata = null)
        {
            Assert.IsTrue(typeof(ISpatial).IsAssignableFrom(geographyPropertyType), "geographyPropertyType passed to CreateRoadTripMetadata is not a type derived from Geography.");

            string modelName = string.Format("RoadTripModelWith{0}", geographyPropertyType.Name);
            DSPMetadata metadata = new DSPMetadata(modelName, "AstoriaUnitTests.Tests");

            // Geography property followed by another geography property
            KeyValuePair<string, Type>[] tripLegProperties = new KeyValuePair<string, Type>[] {
                new KeyValuePair<string, Type>("GeographyProperty1", geographyPropertyType),
                new KeyValuePair<string, Type>("GeographyProperty2", geographyPropertyType),
            };
            AddEntityType(metadata, "TripLeg", tripLegProperties, useComplexType, useOpenTypes);

            // Geography property followed by another non-geography property
            KeyValuePair<string, Type>[] amusementParkProperties = new KeyValuePair<string, Type>[] {
                new KeyValuePair<string, Type>("GeographyProperty", geographyPropertyType),
                new KeyValuePair<string, Type>("Name", typeof(string)),
            };
            AddEntityType(metadata, "AmusementPark", amusementParkProperties, useComplexType, useOpenTypes);

            // Geography property at the end of the entry
            KeyValuePair<string, Type>[] restStopProperties = new KeyValuePair<string, Type>[] {
                new KeyValuePair<string, Type>("GeographyProperty", geographyPropertyType),
            };
            AddEntityType(metadata, "RestStop", restStopProperties, useComplexType, useOpenTypes);

            if (modifyMetadata != null)
            {
                modifyMetadata(metadata);
            }
            metadata.SetReadOnly();
            return metadata;
        }

        internal static DSPUnitTestServiceDefinition CreateRoadTripServiceDefinition(DSPMetadata roadTripMetadata, GeographyPropertyValues defaultValues, DSPDataProviderKind providerKind, bool useComplexType = false, Action<string, List<KeyValuePair<string, object>>> modifyPropertyValues = null)
        {
            Assert.IsFalse(useComplexType, "Complex type support is not yet added to the property population in DSPUnitTestServiceDefinition.");

            DSPContext defaultData = PopulateRoadTripData(roadTripMetadata, defaultValues, useComplexType, modifyPropertyValues);

            var service = new DSPUnitTestServiceDefinition(roadTripMetadata, providerKind, defaultData);
            service.HostInterfaceType = typeof(IDataServiceHost2);
            service.DataServiceBehavior.AcceptSpatialLiteralsInQuery = true;
            service.Writable = true;
            service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            return service;
        }

        internal static DSPContext PopulateRoadTripData(DSPMetadata roadTripMetadata, GeographyPropertyValues defaultValues, bool useComplexType, Action<string, List<KeyValuePair<string, object>>> modifyPropertyValues = null)
        {
            var context = new DSPContext();

            DSPResource tripLegResource = CreateTripLegResource(roadTripMetadata, DefaultId, defaultValues.TripLegGeography1, defaultValues.TripLegGeography2, useComplexType, modifyPropertyValues);
            PopulateResourceSet(context, tripLegResource);

            DSPResource amusementParkResource = CreateAmusementParkResource(roadTripMetadata, DefaultId, defaultValues.AmusementParkGeography, "Disneyland", useComplexType, modifyPropertyValues);
            PopulateResourceSet(context, amusementParkResource);

            DSPResource restStopResource = CreateRestStopResource(roadTripMetadata, DefaultId, defaultValues.RestStopGeography, useComplexType, modifyPropertyValues);
            PopulateResourceSet(context, restStopResource);

            return context;
        }

        internal static DSPResource CreateTripLegResource(DSPMetadata roadTripMetadata, int id, ITestGeography geography1, ITestGeography geography2, bool useComplexType, Action<string, List<KeyValuePair<string, object>>> modifyPropertyValues)
        {
            List<KeyValuePair<string, object>> tripLegPropertyValues = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("GeographyProperty1", geography1.AsGeography()),
                new KeyValuePair<string, object>("GeographyProperty2", geography2.AsGeography()),
            };

            if (modifyPropertyValues != null)
            {
                modifyPropertyValues("TripLeg", tripLegPropertyValues);
            }
            DSPResource tripLegResource = CreateResource(roadTripMetadata, "TripLeg", id, tripLegPropertyValues.ToArray(), useComplexType);
            return tripLegResource;
        }

        internal static DSPResource CreateAmusementParkResource(DSPMetadata roadTripMetadata, int id, ITestGeography location, string name, bool useComplexType, Action<string, List<KeyValuePair<string, object>>> modifyPropertyValues)
        {
            List<KeyValuePair<string, object>> amusementParkPropertyValues = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("GeographyProperty", location.AsGeography()),
                new KeyValuePair<string, object>("Name", name),
            };

            if (modifyPropertyValues != null)
            {
                modifyPropertyValues("AmusementPark", amusementParkPropertyValues);
            }

            DSPResource amusementParkResource = CreateResource(roadTripMetadata, "AmusementPark", id, amusementParkPropertyValues.ToArray(), useComplexType);
            return amusementParkResource;
        }

        internal static DSPResource CreateRestStopResource(DSPMetadata roadTripMetadata, int id, ITestGeography location, bool useComplexType, Action<string, List<KeyValuePair<string, object>>> modifyPropertyValues)
        {
            List<KeyValuePair<string, object>> restStopPropertyValues = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("GeographyProperty", location.AsGeography()),
            };

            if (modifyPropertyValues != null)
            {
                modifyPropertyValues("RestStop", restStopPropertyValues);
            }

            DSPResource restStopResource = CreateResource(roadTripMetadata, "RestStop", id, restStopPropertyValues.ToArray(), useComplexType);
            return restStopResource;
        }

        private static DSPResource CreateResource(DSPMetadata metadata, string entityTypeName, int idValue, KeyValuePair<string, object>[] propertyValues, bool useComplexType)
        {
            var entityType = metadata.GetResourceType(entityTypeName);

            DSPResource entity;
            if (useComplexType)
            {
                entity = new DSPResource(entityType);
            }
            else
            {
                entity = new DSPResource(entityType, propertyValues);
            }

            entity.SetValue("ID", idValue);

            DSPResource resourceForProperties;
            if (useComplexType)
            {

                string complexTypeName = GetComplexTypeName(entityTypeName);
                var complexType = metadata.GetResourceType(complexTypeName);
                resourceForProperties = new DSPResource(complexType, propertyValues);
                entity.SetValue("ComplexProperty", resourceForProperties);
            }
            else
            {
                resourceForProperties = entity;
            }
            return entity;
        }

        private static void PopulateResourceSet(DSPContext context, DSPResource resource)
        {
            string resourceSetName = GetResourceSetName(resource.ResourceType.Name);
            var resourceSet = context.GetResourceSetEntities(resourceSetName);
            resourceSet.AddRange(new object[] { resource });
        }

        internal static void AddEntityType(DSPMetadata metadata, string entityTypeName, KeyValuePair<string, Type>[] properties, bool useComplexType, bool useOpenTypes)
        {
            var entityType = metadata.AddEntityType(entityTypeName, null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            entityType.IsOpenType = useOpenTypes;

            if (!useOpenTypes)
            {
                Microsoft.OData.Service.Providers.ResourceType resourceTypeForProps;
                if (useComplexType)
                {

                    string complexTypeName = GetComplexTypeName(entityTypeName);
                    resourceTypeForProps = metadata.AddComplexType(complexTypeName, null, null, false);
                    metadata.AddComplexProperty(entityType, "ComplexProperty", resourceTypeForProps);
                }
                else
                {
                    resourceTypeForProps = entityType;
                }

                foreach (KeyValuePair<string, Type> property in properties)
                {
                    metadata.AddPrimitiveProperty(resourceTypeForProps, property.Key, property.Value);
                }
            }

            string resourceSetName = GetResourceSetName(entityTypeName);
            metadata.AddResourceSet(resourceSetName, entityType);
        }

        private static string GetResourceSetName(string entityTypeName)
        {
            return string.Format("{0}s", entityTypeName);
        }

        private static string GetComplexTypeName(string entityTypeName)
        {
            return string.Format("{0}ComplexType", entityTypeName);
        }

        internal static string ToPointString(params TestPoint[] points)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < points.Length; ++i)
            {
                sb.Append(points[i].Latitude);
                sb.Append(' ');
                sb.Append(points[i].Longitude); 
                
                if (i != points.Length - 1)
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }

    internal class GeographyPropertyValues
    {
        public GeographyPropertyValues(ITestGeography tripLegGeography1, ITestGeography tripLegGeography2, ITestGeography amusementParkGeography, string amusementParkName, ITestGeography restStopGeography)
        {
            this.TripLegGeography1 = tripLegGeography1;
            this.TripLegGeography2 = tripLegGeography2;
            this.AmusementParkGeography = amusementParkGeography;
            this.AmusementParkName = amusementParkName;
            this.RestStopGeography = restStopGeography;
        }

        public ITestGeography TripLegGeography1 { get; private set; }
        public ITestGeography TripLegGeography2 { get; private set; }
        public ITestGeography AmusementParkGeography { get; private set; }
        public string AmusementParkName { get; private set; }
        public ITestGeography RestStopGeography { get; private set; }
    }

    #region Test geography types

    // Containers for geography values in the tests, intentionally not using the product classes
    // in order to isolate the behavior of that class from the behavior being tested here.

    internal interface ITestGeography
    {
        Geography AsGeography();
        void VerifyGmlContent(XElement property);
    }

    internal class TestPoint : ITestGeography
    {
        public TestPoint(double latitude, double longitude)
        {
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public Geography AsGeography()
        {
            return GeographyFactory.Point(Latitude, Longitude);
        }

        public void VerifyGmlContent(XElement property)
        {
            XElement point = property.Element(UnitTestsUtil.GmlNamespace + "Point");
            Assert.IsNotNull(point, "Payload does not contain the GML point element.");
            XElement pos = point.Element(UnitTestsUtil.GmlNamespace + "pos");
            Assert.IsNotNull(pos, "Payload does not contain the GML pos element");
            Assert.AreEqual(SpatialTestUtil.ToPointString(this), pos.Value, "Incorrect GML Content");
        }

        public static GeographyPropertyValues DefaultValues
        {
            get
            {
                return new GeographyPropertyValues(
                    tripLegGeography1: new TestPoint(47.6, -122.1),
                    tripLegGeography2: new TestPoint(34.1, -118.2),
                    amusementParkGeography: new TestPoint(33.8, -117.9),
                    amusementParkName: "Disneyland",
                    restStopGeography: new TestPoint(45.5, -122.7)
                );
            }
        }

        public static GeographyPropertyValues NewValues
        {
            get
            {
                return new GeographyPropertyValues(
                    tripLegGeography1: new TestPoint(48.9, -126.5),
                    tripLegGeography2: new TestPoint(46.9, -120.1),
                    amusementParkGeography: new TestPoint(49.27, -124.0),
                    amusementParkName: "Worlds of Fun",
                    restStopGeography: new TestPoint(50.22, -120.50)
                );
            }
        }
    }

    internal class TestLineString : ITestGeography
    {
        public TestLineString(params TestPoint[] points)
        {
            this.Points = points;
        }

        public TestPoint[] Points { get; private set; }

        public Geography AsGeography()
        {
            var factory = GeographyFactory.LineString();

            for (int i = 0; i < this.Points.Length; ++i)
            {
                factory.LineTo(this.Points[i].Latitude, this.Points[i].Longitude);
            }

            return factory.Build();
        }

        public string AsGml()
        {
            return SpatialTestUtil.ToPointString(this.Points);
        }

        public void VerifyGmlContent(XElement property)
        {
            XElement linestring = property.Element(UnitTestsUtil.GmlNamespace + "LineString");
            Assert.IsNotNull(linestring, "Payload does not contain the GML line element.");

            var pos = linestring.Elements(UnitTestsUtil.GmlNamespace + "pos");
            Assert.IsNotNull(pos, "Payload does not contain the GML pos element");
            Assert.AreEqual(this.Points.Count(), pos.Count(), "Number of pos elements does not equal to number of points in the linestring");

            string pointString = pos.Select(p => p.Value).Concatenate(" ");

            Assert.AreEqual(SpatialTestUtil.ToPointString(this.Points), pointString, "Incorrect GML Content");
        }

        public static GeographyPropertyValues DefaultValues
        {
            get
            {
                return new GeographyPropertyValues(
                    tripLegGeography1: new TestLineString(new TestPoint(48.6, -123.1), new TestPoint(50.2, 100.5)),
                    tripLegGeography2: new TestLineString(new TestPoint(60.6, -120.1), new TestPoint(55.88, 101.5), new TestPoint(47.999, 60.8)),
                    amusementParkGeography: new TestLineString(new TestPoint(33.8, -120.9), new TestPoint(41.6, 102.56)),
                    amusementParkName: "Disneyland",
                    restStopGeography: new TestLineString(new TestPoint(45.5, 110.32), new TestPoint(87.654, -129.000))
                );
            }
        }

        public static GeographyPropertyValues NewValues
        {
            get
            {
                return new GeographyPropertyValues(
                    tripLegGeography1: new TestLineString(new TestPoint(41.6, -124.1), new TestPoint(52.2, 93.5), new TestPoint(88.1, -123)),
                    tripLegGeography2: new TestLineString(new TestPoint(70.6, -167.1), new TestPoint(52.8872, 100.5)),
                    amusementParkGeography: new TestLineString(new TestPoint(39.8, -121.9), new TestPoint(45, -127)),
                    amusementParkName: "Worlds of Fun",
                    restStopGeography: new TestLineString(new TestPoint(46.2, 133.91), new TestPoint(89.8, -140.052))
                );
            }
        }
    }

    #endregion
}
