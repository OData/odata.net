//---------------------------------------------------------------------
// <copyright file="PrimitiveResourceTypeMapTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Linq;
using Microsoft.OData.Service.Providers;
using System.IO;
using System.Xml.Linq;
using Microsoft.Spatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class PrimitiveResourceTypeMapTests
    {
        private static Type knownPrimitiveType = typeof(TestPrimitiveType);
        private static Type unknownPrimitiveType = typeof(string);
        private static string edmPrefix = "Edm";
        private static string testTypeEdmName = "TestPrimitiveType";
        private static string testTypeFullName = String.Format("{0}.{1}", edmPrefix, testTypeEdmName);
        private static KeyValuePair<Type, string>[] primitiveTypeMap = new KeyValuePair<Type, string>[] { new KeyValuePair<Type, string>(knownPrimitiveType, testTypeFullName) };
        private PrimitiveResourceTypeMap typeMap;

        public class TestPrimitiveType
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            typeMap = GetTestPrimitiveMap();
        }

        [TestMethod]
        public void GetPrimitiveFromCLRType_KnownType()
        {
            ResourceType resourceType = typeMap.GetPrimitive(knownPrimitiveType);
            VerifyTestPrimitiveResourceType(resourceType);
        }

        [TestMethod]
        public void GetPrimitiveFromCLRType_UnknownType()
        {
            ResourceType resourceType = typeMap.GetPrimitive(unknownPrimitiveType);
            Assert.IsNull(resourceType, "Expected unknown type not to be in the primitive type map.");
        }

        [TestMethod]
        public void GetPrimitiveFromCLRType_Null()
        {
            Action test = () => typeMap.GetPrimitive((Type)null);
            test.ShouldThrow<ArgumentNullException>().Where(e => e.ParamName == "type");
        }

        [TestMethod]
        public void GetPrimitiveFromString_KnownType()
        {
            ResourceType resourceType = typeMap.GetPrimitive(testTypeFullName);
            VerifyTestPrimitiveResourceType(resourceType);
        }

        [TestMethod]
        public void GetPrimitiveFromString_UnknownType()
        {
            ResourceType resourceType = typeMap.GetPrimitive("Edm.String");
            Assert.IsNull(resourceType, "Expected unknown not to be in the primitive type map.");
        }

        [TestMethod]
        public void IsPrimitive()
        {
            Assert.IsTrue(typeMap.IsPrimitive(knownPrimitiveType));
        }

        [TestMethod]
        public void IsNotPrimitive()
        {
            Assert.IsFalse(typeMap.IsPrimitive(unknownPrimitiveType));
        }

        [TestMethod]
        public void AllPrimitivesContainsAllTypesInMap()
        {
            Assert.AreEqual(1, typeMap.AllPrimitives.Length, "Expected type map to contain only one type that was provided by the test.");
            Assert.AreSame(knownPrimitiveType, typeMap.AllPrimitives[0].InstanceType, "Expected type map to return the same type that was provided by the test.");
        }

        [TestMethod]
        public void ResourceTypeStaticMapIsNonNullShouldReturnSameInstanceEachTime()
        {
            PrimitiveResourceTypeMap typeMap1 = PrimitiveResourceTypeMap.TypeMap;
            Assert.IsNotNull(typeMap1, "Expected non-null static type map on ResourceType.");
            PrimitiveResourceTypeMap typeMap2 = PrimitiveResourceTypeMap.TypeMap;
            Assert.AreSame(typeMap1, typeMap2, "Expected static type map to return the same instance every time it is accessed.");
        }

        [TestMethod]
        public void ValidatePrimitiveResourceTypeMapConstructorHasAllBuiltInTypes()
        {
            PrimitiveResourceTypeMap mapping = new PrimitiveResourceTypeMap();

            string edmBinaryTypeName = "Binary";
            string edmBooleanTypeName = "Boolean";
            string edmByteTypeName = "Byte";
            string edmDateTimeOffsetName = "DateTimeOffset";
            string edmDecimalTypeName = "Decimal";
            string edmDoubleTypeName = "Double";
            string edmGuidTypeName = "Guid";
            string edmSingleTypeName = "Single";
            string edmSByteTypeName = "SByte";
            string edmInt16TypeName = "Int16";
            string edmInt32TypeName = "Int32";
            string edmInt64TypeName = "Int64";
            string edmStringTypeName = "String";
            string edmStreamTypeName = "Stream";
            string edmGeographyTypeName = "Geography";
            string edmPointTypeName = "GeographyPoint";
            string edmLineStringTypeName = "GeographyLineString";
            string edmPolygonTypeName = "GeographyPolygon";
            string edmGeographyCollectionTypeName = "GeographyCollection";
            string edmMultiLineStringTypeName = "GeographyMultiLineString";
            string edmMultiPointTypeName = "GeographyMultiPoint";
            string edmMultiPolygonTypeName = "GeographyMultiPolygon";
            string edmGeometryTypeName = "Geometry";
            string edmGeometryPointTypeName = "GeometryPoint";
            string edmGeometryLineStringTypeName = "GeometryLineString";
            string edmGeometryPolygonTypeName = "GeometryPolygon";
            string edmGeometryCollectionTypeName = "GeometryCollection";
            string edmGeometryMultiLineStringTypeName = "GeometryMultiLineString";
            string edmGeometryMultiPointTypeName = "GeometryMultiPoint";
            string edmGeometryMultiPolygonTypeName = "GeometryMultiPolygon";
            string edmDurationTypeName = "Duration";
            
            KeyValuePair<Type, string>[] expectedBuiltInTypes = 
                new KeyValuePair<Type, string>[]
                {
                    new KeyValuePair<Type, string>(typeof(string), edmStringTypeName),
                    new KeyValuePair<Type, string>(typeof(Boolean), edmBooleanTypeName),
                    new KeyValuePair<Type, string>(typeof(Boolean?), edmBooleanTypeName),
                    new KeyValuePair<Type, string>(typeof(Byte), edmByteTypeName),
                    new KeyValuePair<Type, string>(typeof(Byte?), edmByteTypeName),
                    new KeyValuePair<Type, string>(typeof(DateTime), edmDateTimeOffsetName),
                    new KeyValuePair<Type, string>(typeof(DateTime?), edmDateTimeOffsetName),
                    new KeyValuePair<Type, string>(typeof(DateTimeOffset), edmDateTimeOffsetName),
                    new KeyValuePair<Type, string>(typeof(DateTimeOffset?), edmDateTimeOffsetName),
                    new KeyValuePair<Type, string>(typeof(Decimal), edmDecimalTypeName),
                    new KeyValuePair<Type, string>(typeof(Decimal?), edmDecimalTypeName),
                    new KeyValuePair<Type, string>(typeof(Double), edmDoubleTypeName),
                    new KeyValuePair<Type, string>(typeof(Double?), edmDoubleTypeName),
                    new KeyValuePair<Type, string>(typeof(Guid), edmGuidTypeName),
                    new KeyValuePair<Type, string>(typeof(Guid?), edmGuidTypeName),
                    new KeyValuePair<Type, string>(typeof(Int16), edmInt16TypeName),
                    new KeyValuePair<Type, string>(typeof(Int16?), edmInt16TypeName),
                    new KeyValuePair<Type, string>(typeof(Int32), edmInt32TypeName),
                    new KeyValuePair<Type, string>(typeof(Int32?), edmInt32TypeName),
                    new KeyValuePair<Type, string>(typeof(Int64), edmInt64TypeName),
                    new KeyValuePair<Type, string>(typeof(Int64?), edmInt64TypeName),
                    new KeyValuePair<Type, string>(typeof(SByte), edmSByteTypeName),
                    new KeyValuePair<Type, string>(typeof(SByte?), edmSByteTypeName),
                    new KeyValuePair<Type, string>(typeof(Single), edmSingleTypeName),
                    new KeyValuePair<Type, string>(typeof(Single?), edmSingleTypeName),
                    new KeyValuePair<Type, string>(typeof(byte[]), edmBinaryTypeName),
                    new KeyValuePair<Type, string>(typeof(Stream), edmStreamTypeName),
                    new KeyValuePair<Type, string>(typeof(TimeSpan), edmDurationTypeName),
                    new KeyValuePair<Type, string>(typeof(TimeSpan?), edmDurationTypeName),
                    new KeyValuePair<Type, string>(typeof(Geography), edmGeographyTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyPoint), edmPointTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyLineString), edmLineStringTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyPolygon), edmPolygonTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyCollection), edmGeographyCollectionTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyMultiPoint), edmMultiPointTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyMultiLineString), edmMultiLineStringTypeName),
                    new KeyValuePair<Type, string>(typeof(GeographyMultiPolygon), edmMultiPolygonTypeName),
                    new KeyValuePair<Type, string>(typeof(Geometry), edmGeometryTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryPoint), edmGeometryPointTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryLineString), edmGeometryLineStringTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryPolygon), edmGeometryPolygonTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryCollection), edmGeometryCollectionTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryMultiPoint), edmGeometryMultiPointTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryMultiLineString), edmGeometryMultiLineStringTypeName),
                    new KeyValuePair<Type, string>(typeof(GeometryMultiPolygon), edmGeometryMultiPolygonTypeName),
                    new KeyValuePair<Type, string>(typeof(XElement), edmStringTypeName),
                    new KeyValuePair<Type, string>(typeof(Binary), edmBinaryTypeName),
                };

            Assert.AreEqual(expectedBuiltInTypes.Length, mapping.AllPrimitives.Length, "AllPrimitives contains a different number of types than expected for the mapping provided by the default constructor.");
 
            foreach (KeyValuePair<Type, string> expectedType in expectedBuiltInTypes)
            {
                ResourceType resourceType = mapping.GetPrimitive(expectedType.Key);
                PrimitiveResourceTypeMapTests.VerifyPrimitiveResourceType(resourceType, expectedType.Key, expectedType.Value);
            }
        }

        internal static PrimitiveResourceTypeMap GetTestPrimitiveMap()
        {
            return new PrimitiveResourceTypeMap(primitiveTypeMap);
        }

        private static void VerifyTestPrimitiveResourceType(ResourceType resourceType)
        {
            VerifyPrimitiveResourceType(resourceType, knownPrimitiveType, testTypeEdmName);
        }

        internal static void VerifyPrimitiveResourceType(ResourceType resourceType, Type expectedInstanceType, string expectedTypeName)
        {
            Assert.IsNotNull(resourceType, "Failed to get a resource type from the map.");
            Assert.AreEqual(expectedInstanceType, resourceType.InstanceType, "ResourceType.InstanceType is not the expected value.");
            Assert.AreEqual(expectedTypeName, resourceType.Name, "ResourceType.Name is not the expected value.");
            Assert.AreEqual(ResourceTypeKind.Primitive, resourceType.ResourceTypeKind, "PrimitiveResourcetypeMap should only return primitive resource types.");
            Assert.AreEqual(edmPrefix, resourceType.Namespace, "ResourceType.Namespace for a primitive type should be Edm.");
        }
    }
}
