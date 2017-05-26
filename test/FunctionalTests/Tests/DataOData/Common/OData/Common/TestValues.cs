//---------------------------------------------------------------------
// <copyright file="TestValues.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;

    /// <summary>
    /// Helper class to create all interesting payload values to be used in properties.
    /// </summary>
    public static class TestValues
    {
        private static string NamespaceName = "TestModel";

        /// <summary>
        /// Array of supported primitive CLR types (non-nullable variants only)
        /// </summary>
        public static Type[] PrimitiveTypes = new Type[]
        {
            typeof(string),
            typeof(Boolean),
            typeof(Byte),
            typeof(Decimal),
            typeof(Double),
            typeof(Guid),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(SByte),
            typeof(Single),
            typeof(byte[]),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            // Stream - not yet supported
        };

        /// <summary>
        /// List of interesting primitive values which carry metadata.
        /// </summary>
        /// <param name="fullSet">true if all available primitive values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting primitive values.</returns>
        public static IList<PrimitiveValue> CreatePrimitiveValuesWithMetadata(bool fullSet = true)
        {
            List<PrimitiveValue> primitiveValues = new List<PrimitiveValue>();

            // null
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(EdmCoreModel.Instance.GetString(true)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(true)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, true)));

            // Edm.String
            primitiveValues.Add(PayloadBuilder.PrimitiveValue("stringvalue").WithTypeAnnotation(EdmCoreModel.Instance.GetString(true)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue("").WithTypeAnnotation(EdmCoreModel.Instance.GetString(true)));
            }

            // Edm.Boolean
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(true).WithTypeAnnotation(EdmCoreModel.Instance.GetBoolean(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(false).WithTypeAnnotation(EdmCoreModel.Instance.GetBoolean(false)));
            }

            // Edm.Byte
            primitiveValues.Add(PayloadBuilder.PrimitiveValue((byte)33).WithTypeAnnotation(EdmCoreModel.Instance.GetByte(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(byte.MinValue).WithTypeAnnotation(EdmCoreModel.Instance.GetByte(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(byte.MaxValue).WithTypeAnnotation(EdmCoreModel.Instance.GetByte(false)));
            }

            // Edm.SByte
            primitiveValues.Add(PayloadBuilder.PrimitiveValue((sbyte)22).WithTypeAnnotation(EdmCoreModel.Instance.GetSByte(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue((sbyte)(-22)).WithTypeAnnotation(EdmCoreModel.Instance.GetSByte(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(sbyte.MinValue).WithTypeAnnotation(EdmCoreModel.Instance.GetSByte(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(sbyte.MaxValue).WithTypeAnnotation(EdmCoreModel.Instance.GetSByte(false)));
            }

            // Edm.Int16
            primitiveValues.Add(PayloadBuilder.PrimitiveValue((Int16)123).WithTypeAnnotation(EdmCoreModel.Instance.GetInt16(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue((Int16)(-123)).WithTypeAnnotation(EdmCoreModel.Instance.GetInt16(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Int16.MinValue).WithTypeAnnotation(EdmCoreModel.Instance.GetInt16(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Int16.MaxValue).WithTypeAnnotation(EdmCoreModel.Instance.GetInt16(false)));
            }

            // Edm.Decimal
            primitiveValues.Add(PayloadBuilder.PrimitiveValue((decimal)123.456).WithTypeAnnotation(EdmCoreModel.Instance.GetDecimal(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue((decimal)(-123.456)).WithTypeAnnotation(EdmCoreModel.Instance.GetDecimal(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(decimal.MinValue).WithTypeAnnotation(EdmCoreModel.Instance.GetDecimal(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(decimal.MaxValue).WithTypeAnnotation(EdmCoreModel.Instance.GetDecimal(false)));
            }

            // Edm.Single
            primitiveValues.Add(PayloadBuilder.PrimitiveValue((float)42.42).WithTypeAnnotation(EdmCoreModel.Instance.GetSingle(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Single.PositiveInfinity).WithTypeAnnotation(EdmCoreModel.Instance.GetSingle(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Single.NegativeInfinity).WithTypeAnnotation(EdmCoreModel.Instance.GetSingle(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Single.NaN).WithTypeAnnotation(EdmCoreModel.Instance.GetSingle(false)));
            }

            // Edm.Int32
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(-42).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(42).WithTypeAnnotation(EdmCoreModel.Instance.GetInt32(false)));
            }

            // Edm.Int64
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(Int64.MaxValue).WithTypeAnnotation(EdmCoreModel.Instance.GetInt64(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue((Int64)456).WithTypeAnnotation(EdmCoreModel.Instance.GetInt64(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue((Int64)(-456)).WithTypeAnnotation(EdmCoreModel.Instance.GetInt64(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Int64.MinValue).WithTypeAnnotation(EdmCoreModel.Instance.GetInt64(false)));
            }

            // Edm.Double
            primitiveValues.Add(PayloadBuilder.PrimitiveValue((double)Int32.MaxValue + 1).WithTypeAnnotation(EdmCoreModel.Instance.GetDouble(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(42.42).WithTypeAnnotation(EdmCoreModel.Instance.GetDouble(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Double.PositiveInfinity).WithTypeAnnotation(EdmCoreModel.Instance.GetDouble(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Double.NegativeInfinity).WithTypeAnnotation(EdmCoreModel.Instance.GetDouble(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(Double.NaN).WithTypeAnnotation(EdmCoreModel.Instance.GetDouble(false)));
            }

            // Edm.Binary
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(new byte[] { 1, 2, 3 }).WithTypeAnnotation(EdmCoreModel.Instance.GetBinary(false)));

            // Edm.Guid
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(new Guid("{38CF68C2-4010-4CCC-8922-868217F03DDC}")).WithTypeAnnotation(EdmCoreModel.Instance.GetGuid(false)));

            // Geography types
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.Point(32.0, -100.0).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.Collection().Point(-19.99, -12.0).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, false)));

            // Geometry types
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.Point(32.0, -10.0).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.LineString(33.1, -11.5).LineTo(35.97, -11).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.Polygon().Ring(33.1, -13.6).LineTo(35.97, -11.15).LineTo(11.45, 87.75).Ring(35.97, -11).LineTo(36.97, -11.15).LineTo(45.23, 23.18).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.Collection().Point(-19.99, -12.0).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, false)));
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, false)));

            if (fullSet)
            {
                // Geography types
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.Point().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.LineString().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.Polygon().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.Collection().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.MultiPoint().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.MultiLineString().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeographyFactory.MultiPolygon().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, false)));

                // Geometry types
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.Point().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.LineString().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.Polygon().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.Collection().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.MultiPoint().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.MultiLineString().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(GeometryFactory.MultiPolygon().Build()).WithTypeAnnotation(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, false)));
            }

            // Edm.DateTimeOffset
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(new DateTimeOffset(new DateTime(2011, 2, 26), new TimeSpan(3, 0, 0))).WithTypeAnnotation(EdmCoreModel.Instance.GetDateTimeOffset(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(new DateTimeOffset(new DateTime(2011, 2, 26), new TimeSpan(3, 0, 0))).WithTypeAnnotation(EdmCoreModel.Instance.GetDateTimeOffset(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(new DateTimeOffset(new DateTime(2011, 2, 26), TimeSpan.Zero)).WithTypeAnnotation(EdmCoreModel.Instance.GetDateTimeOffset(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(new DateTimeOffset(new DateTime(2011, 2, 26, 0, 0, 0, DateTimeKind.Utc))).WithTypeAnnotation(EdmCoreModel.Instance.GetDateTimeOffset(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(new DateTimeOffset(new DateTime(2011, 2, 26), new TimeSpan(3, 0, 0)).ToUniversalTime()).WithTypeAnnotation(EdmCoreModel.Instance.GetDateTimeOffset(false)));
            }

            // Edm.Duration
            primitiveValues.Add(PayloadBuilder.PrimitiveValue(new TimeSpan(5, 0, 0)).WithTypeAnnotation(EdmCoreModel.Instance.GetDuration(false)));
            if (fullSet)
            {
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(TimeSpan.Zero).WithTypeAnnotation(EdmCoreModel.Instance.GetDuration(false)));
                primitiveValues.Add(PayloadBuilder.PrimitiveValue(new TimeSpan(-1, -4, -10, -40, -35)).WithTypeAnnotation(EdmCoreModel.Instance.GetDuration(false)));
            }

            return primitiveValues;
        }

        /// <summary>
        /// Creates a set of interesting complex values along with metadata.
        /// </summary>
        /// <param name="model">If non-null, the method creates complex types for the complex values and adds them to the model.</param>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="fullSet">true if all available complex values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting complex values.</returns>
        public static IEnumerable<ComplexInstance> CreateComplexValues(EdmModel model, bool withTypeNames, bool fullSet = true)
        {
            var complexValuesList = new List<ComplexInstance>();

            EdmComplexType complexType = null;
            EdmComplexType innerComplexType = null;
            string typeName;
            ComplexInstance complexValue;

            //// Null complex value
            //if (fullSet)
            //{
            //    typeName = "NullComplexType";

            //    // Can't specify type name for a null complex value since the reader will not read it (in JSON it's not even in the payload anywhere)
            //    complexValue = new ComplexInstance(null, true);
            //    complexValuesList.Add(complexValue);
            //    if (model != null)
            //    {

            //        complexType = (EdmComplexType)model.FindDeclaredType(GetFullTypeName(typeName));
            //        if (complexType == null)
            //        {
            //            complexType = new EdmComplexType(NamespaceName, typeName);
            //            model.AddElement(complexType);
            //        }

            //        complexValue.WithTypeAnnotation(new EdmComplexTypeReference(complexType, true));
            //    }
            //}

            if (fullSet)
            {
                // Complex value with one number property
                typeName = "ComplexTypeWithNumberProperty";
                complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                        .PrimitiveProperty("numberProperty", 42);
                complexValuesList.Add(complexValue);
                if (model != null)
                {
                    complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                    if (complexType == null)
                    {
                        complexType = new EdmComplexType(NamespaceName, typeName);
                        complexType.AddStructuralProperty("numberProperty", EdmPrimitiveTypeKind.Int32);
                        model.AddElement(complexType);
                    }
                    complexValue.WithTypeAnnotation(complexType);
                }
            }

            // Complex value with three properties
            typeName = "ComplexTypeWithNumberStringAndNullProperty";
            complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                .PrimitiveProperty("number", 42)
                .PrimitiveProperty("string", "some")
                .PrimitiveProperty("null", null);
            complexValuesList.Add(complexValue);
            if (model != null)
            {
                complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                if (complexType == null)
                {
                    complexType = new EdmComplexType(NamespaceName, typeName);
                    complexType.AddStructuralProperty("number", EdmPrimitiveTypeKind.Int32);
                    complexType.AddStructuralProperty("string", EdmPrimitiveTypeKind.String);
                    complexType.AddStructuralProperty("null", EdmPrimitiveTypeKind.String);
                    model.AddElement(complexType);
                }

                complexValue.WithTypeAnnotation(complexType);
            }

            // Complex value with primitive and complex property
            typeName = "ComplexTypeWithPrimitiveAndComplexProperty";
            if (model != null)
            {
                innerComplexType = model.FindDeclaredType(GetFullTypeName("InnerComplexTypeWithStringProperty")) as EdmComplexType;
                if (innerComplexType == null)
                {
                    innerComplexType = new EdmComplexType(NamespaceName, "InnerComplexTypeWithStringProperty");
                    innerComplexType.AddStructuralProperty("foo", EdmPrimitiveTypeKind.String);
                    model.AddElement(innerComplexType);
                }
                complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                if (complexType == null)
                {
                    complexType = new EdmComplexType(NamespaceName, typeName);
                    complexType.AddStructuralProperty("number", EdmPrimitiveTypeKind.Int32);
                    complexType.AddStructuralProperty("complex", innerComplexType.ToTypeReference());
                    model.AddElement(complexType);
                }

            }

            complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                .PrimitiveProperty("number", 42)
                .Property("complex", (withTypeNames ?
                                        PayloadBuilder.ComplexValue("TestModel.InnerComplexTypeWithStringProperty")
                                        : (model != null ?
                                                PayloadBuilder.ComplexValue(null).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null })
                                                : PayloadBuilder.ComplexValue(null)))
                    .PrimitiveProperty("foo", "bar")
                    .WithTypeAnnotation(innerComplexType));
            complexValuesList.Add(complexValue);
            complexValue.WithTypeAnnotation(complexType);

            // Complex value with spatial properties
            typeName = "ComplexTypeWithSpatialProperties";
            if (model != null)
            {
                complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                if (complexType == null)
                {
                    complexType = new EdmComplexType(NamespaceName, typeName);
                    complexType.AddStructuralProperty("geographyPoint", EdmPrimitiveTypeKind.GeographyPoint);
                    complexType.AddStructuralProperty("geometryPoint", EdmPrimitiveTypeKind.GeometryPoint);
                    model.AddElement(complexType);
                }
            }

            complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                .PrimitiveProperty("geographyPoint", GeographyFactory.Point(32.0, -200.0).Build())
                .PrimitiveProperty("geometryPoint", GeometryFactory.Point(60.5, -50.0).Build());
            complexValuesList.Add(complexValue);
            complexValue.WithTypeAnnotation(complexType);

            // Complex value with deeply nested complex types
            if (fullSet)
            {
                int nesting = 7;
                var nestedComplexTypes = new List<EdmComplexType>();
                string typeNameBase = "NestedComplexType";

                if (model != null)
                {
                    for (int i = 0; i < nesting; ++i)
                    {
                        typeName = typeNameBase + i;
                        complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                        if (complexType == null)
                        {
                            complexType = new EdmComplexType(NamespaceName, typeName);
                            complexType.AddStructuralProperty("Property1", EdmPrimitiveTypeKind.Int32);
                            if (nestedComplexTypes.Any())
                            {
                                complexType.AddStructuralProperty("complex", nestedComplexTypes.Last().ToTypeReference());
                            }
                            model.AddElement(complexType);
                        }

                        nestedComplexTypes.Add(complexType);
                    }
                }

                complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeNameBase) + "0" : null)
                    .PrimitiveProperty("Property1", 0)
                    .WithTypeAnnotation(nestedComplexTypes.ElementAt(0));

                for (int j = 1; j < nesting; ++j)
                {
                    if (!withTypeNames && model != null)
                    {
                        complexValue.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                    }

                    complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeNameBase) + j : null)
                        .PrimitiveProperty("Property1", 0)
                        .Property("complex", complexValue);
                    complexValue.WithTypeAnnotation(nestedComplexTypes.ElementAt(j));
                }
                complexValuesList.Add(complexValue);
            }

            // Complex value with many primitive properties
            if (fullSet)
            {
                var primitiveValues = TestValues.CreatePrimitiveValuesWithMetadata(true);

                typeName = "ComplexTypeWithManyPrimitiveProperties";
                if (model != null)
                {
                    complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                    if (complexType == null)
                    {
                        complexType = new EdmComplexType(NamespaceName, typeName);
                        for (int i = 0; i < primitiveValues.Count; ++i)
                        {
                            complexType.AddStructuralProperty("property" + i, primitiveValues[i].GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
                        }

                        model.AddElement(complexType);
                    }
                }

                complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null).WithTypeAnnotation(complexType);
                for (int j = 0; j < primitiveValues.Count; ++j)
                {
                    complexValue.PrimitiveProperty("property" + j, primitiveValues[j].ClrValue);
                }

                complexValuesList.Add(complexValue);
            }

            // Complex value with collection properties
            if (fullSet)
            {
                var primitiveCollections = TestValues.CreatePrimitiveCollections(withTypeNames, true);
                var complexPropertyValue = complexValuesList.Last().DeepCopy();
                IEdmTypeReference complexPropertyType = null;

                typeName = "ComplexTypeWithCollectionProperties";
                if (model != null)
                {
                    complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                    if (complexType == null)
                    {
                        complexType = new EdmComplexType(NamespaceName, typeName);
                        int i = 0;
                        for (; i < primitiveCollections.Count(); ++i)
                        {
                            complexType.AddStructuralProperty("property" + i, primitiveCollections.ElementAt(i).GetAnnotation<EntityModelTypeAnnotation>().EdmModelType);
                        }

                        complexPropertyType = complexPropertyValue.GetAnnotation<EntityModelTypeAnnotation>().EdmModelType;
                        complexType.AddStructuralProperty("property" + i, EdmCoreModel.GetCollection(complexPropertyType));
                        model.AddElement(complexType);
                    }
                }

                complexValue = PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null).WithTypeAnnotation(complexType);
                int k = 0;
                for (; k < primitiveCollections.Count(); ++k)
                {
                    complexValue.Property("property" + k, primitiveCollections.ElementAt(k));
                }

                string complexCollectionTypeName = withTypeNames ? EntityModelUtils.GetCollectionTypeName(complexPropertyValue.FullTypeName) : null;
                var complexCollectionItem = PayloadBuilder.ComplexMultiValue(complexCollectionTypeName)
                        .Item(complexPropertyValue)
                        .WithTypeAnnotation(EdmCoreModel.GetCollection(complexPropertyType));
                if (!withTypeNames && model != null)
                {
                    complexPropertyValue.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                    complexCollectionItem.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                }
                complexValue.Property(
                    "property" + k,
                    complexCollectionItem);
                complexValuesList.Add(complexValue);
            }

            if (model != null)
            {
                if (!withTypeNames)
                {
                    foreach (var item in complexValuesList)
                    {
                        item.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                    }
                }
                if (model.EntityContainer == null)
                {
                    model.AddElement(new EdmEntityContainer(NamespaceName, "DefaultContainer"));
                }
            }

            return complexValuesList;
        }

        /// <summary>
        /// Creates a set of interesting collections with primitive items.
        /// </summary>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="fullSet">true if all available primitive collections should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting collections.</returns>
        public static IEnumerable<PrimitiveMultiValue> CreatePrimitiveCollections(bool withTypeNames, bool fullSet = true)
        {
            List<PrimitiveMultiValue> results = new List<PrimitiveMultiValue>();

            // Empty collection of integers
            results.Add(PayloadBuilder.PrimitiveMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName("Edm.Int32") : null)
                .WithTypeAnnotation(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false))));

            // Collection of integers with one item
            if (fullSet)
            {
                results.Add(PayloadBuilder.PrimitiveMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName("Edm.Int32") : null)
                    .Item(42)
                    .WithTypeAnnotation(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false))));
            }

            // Collection of strings with multiple items
            results.Add(PayloadBuilder.PrimitiveMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName("Edm.String") : null)
                .Item("Bart")
                .Item("Homer")
                .Item("Marge")
                .WithTypeAnnotation(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetString(true))));

            // Collection of booleans
            results.Add(PayloadBuilder.PrimitiveMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName("Edm.Boolean") : null)
                .Item(true)
                .Item(false)
                .WithTypeAnnotation(EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetBoolean(false))));

            if (!withTypeNames)
            {
                foreach (var result in results)
                {
                    result.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                }
            }

            return results;
        }

        /// <summary>
        /// Creates a set of interesting collections with complex items.
        /// </summary>
        /// <param name="model">The model to add complex types to.</param>
        /// <param name="withTypeNames">true if the complex value payloads should specify type names.</param>
        /// <param name="fullSet">true if all available complex collections should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting collections.</returns>
        public static IEnumerable<ComplexMultiValue> CreateComplexCollections(EdmModel model, bool withTypeNames, bool fullSet = true)
        {
            List<ComplexMultiValue> results = new List<ComplexMultiValue>();

            string typeName;
            ComplexMultiValue complexCollection;
            EdmComplexType complexType = null;

            // Empty complex collection without model or typename cannot be recognized and will be treated as a primitive collection
            // (note that this is done by test infrastructure, not by the product, the product will simply report empty collection).
            // TODO: What about empty complex collection with type name but without model?
            typeName = "ComplexTypeForEmptyCollection";
            if (model != null)
            {
                complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                if (complexType == null)
                {
                    complexType = new EdmComplexType(NamespaceName, typeName);
                    model.AddElement(complexType);
                }

                complexCollection = PayloadBuilder.ComplexMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName(GetFullTypeName(typeName)) : null);
                complexCollection.WithTypeAnnotation(EdmCoreModel.GetCollection(complexType.ToTypeReference()).CollectionDefinition());
                if (!withTypeNames)
                {
                    complexCollection.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                }

                results.Add(complexCollection);
            }

            // Collection with multiple complex items with property
            typeName = "ComplexTypeForMultipleItemsCollection";
            if (model != null)
            {
                complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                if (complexType == null)
                {
                    complexType = new EdmComplexType(NamespaceName, typeName);
                    complexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
                    model.AddElement(complexType);
                }
            }

            List<ComplexInstance> instanceList = new List<ComplexInstance>()
            {
                PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                    .PrimitiveProperty("Name", "Bart")
                    .WithTypeAnnotation(complexType),
                PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                    .PrimitiveProperty("Name", "Homer")
                    .WithTypeAnnotation(complexType),
                PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                    .PrimitiveProperty("Name", "Marge")
                    .WithTypeAnnotation(complexType)
            };

            if (!withTypeNames)
            {
                foreach (var item in instanceList)
                {
                    item.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                }
            }

            complexCollection = PayloadBuilder.ComplexMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName(GetFullTypeName(typeName)) : null);
            complexCollection.Items(instanceList);
            complexCollection.WithTypeAnnotation(complexType == null ? null : EdmCoreModel.GetCollection(new EdmComplexTypeReference(complexType, false)));
            if (!withTypeNames)
            {
                complexCollection.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
            }

            results.Add(complexCollection);

            // Collection with multiple complex items with properties of various types
            if (fullSet)
            {
                typeName = "ComplexTypeForMultipleComplexItemsCollection";
                string innerTypeName = "InnerComplexTypeForMultipleComplexItemsCollection";

                EdmComplexType innerComplexType = null;
                if (model != null)
                {
                    innerComplexType = model.FindDeclaredType(GetFullTypeName(innerTypeName)) as EdmComplexType;
                    if (innerComplexType == null)
                    {
                        innerComplexType = new EdmComplexType(NamespaceName, innerTypeName);
                        innerComplexType.AddStructuralProperty("Include", EdmPrimitiveTypeKind.Boolean);
                        model.AddElement(innerComplexType);
                    }

                    complexType = model.FindDeclaredType(GetFullTypeName(typeName)) as EdmComplexType;
                    if (complexType == null)
                    {
                        complexType = new EdmComplexType(NamespaceName, typeName);
                        complexType.AddStructuralProperty("Rating", EdmPrimitiveTypeKind.Int32);
                        complexType.AddStructuralProperty("Complex", new EdmComplexTypeReference(innerComplexType, true));
                        model.AddElement(complexType);
                    }
                }

                //instanceList = new List<ComplexInstance>()
                //{
                //    //PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                //    //    .PrimitiveProperty("Rating", 1)
                //    //    .Property("Complex", (withTypeNames ? 
                //    //                            PayloadBuilder.ComplexValue(GetFullTypeName(innerTypeName))
                //    //                            : PayloadBuilder.ComplexValue(null).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                //    //                    .PrimitiveProperty("Include", false)
                //    //                    .WithTypeAnnotation(innerComplexType))
                //    //    .WithTypeAnnotation(complexType),
                //    //PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                //    //    .PrimitiveProperty("Rating", 2)
                //    //    .Property("Complex", (withTypeNames ? 
                //    //                            PayloadBuilder.ComplexValue(GetFullTypeName(innerTypeName))
                //    //                            : PayloadBuilder.ComplexValue(null).AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null }))
                //    //                    .PrimitiveProperty("Include", true)
                //    //                    .WithTypeAnnotation(innerComplexType))
                //    //    .WithTypeAnnotation(complexType),
                //    //PayloadBuilder.ComplexValue(withTypeNames ? GetFullTypeName(typeName) : null)
                //    //    .PrimitiveProperty("Rating", 1)
                //    //    .Property("Complex", new ComplexInstance(null, true))
                //    //    .WithTypeAnnotation(complexType)
                //};
                //if (!withTypeNames)
                //{
                //    foreach (var item in instanceList)
                //    {
                //        item.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                //    }
                //}

                //complexCollection = PayloadBuilder.ComplexMultiValue(withTypeNames ? EntityModelUtils.GetCollectionTypeName(GetFullTypeName(typeName)) : null)
                //    .Items(instanceList);
                //complexCollection.WithTypeAnnotation(complexType == null ? null : new EdmCollectionType(complexType.ToTypeReference()));
                //if (!withTypeNames)
                //{
                //    complexCollection.AddAnnotation(new SerializationTypeNameTestAnnotation() { TypeName = null });
                //}

                //results.Add(complexCollection);
            }

            return results;
        }

        /// <summary>
        /// Creates a set of interesting stream reference (named stream) values.
        /// </summary>
        /// <param name="fullSet">true if all the stream reference values should be created; false if only a subset should be created.</param>
        /// <returns>List of interesting stream reference values (named streams).</returns>
        public static IEnumerable<NamedStreamInstance> CreateStreamReferenceValues(bool fullSet = true)
        {
            // read link only
            yield return new NamedStreamInstance()
            {
                Name = "StreamReference0",
                SourceLink = "http://odata.org/namedstream/sourcelink"
            };

            if (fullSet)
            {
                // read link and content type
                yield return new NamedStreamInstance()
                {
                    Name = "StreamReference1",
                    SourceLink = "http://odata.org/namedstream/sourcelink",
                    SourceLinkContentType = "namedstream:sourcelink:contenttype",
                };

                // edit link only
                yield return new NamedStreamInstance()
                {
                    Name = "StreamReference2",
                    EditLink = "http://odata.org/namedstream/editlink"
                };
            }

            // read link and content type
            yield return new NamedStreamInstance()
            {
                Name = "StreamReference3",
                EditLink = "http://odata.org/namedstream/editlink",
                EditLinkContentType = "namedstream:editlink:contenttype",
            };

            if (fullSet)
            {
                // read link and edit link
                yield return new NamedStreamInstance()
                {
                    Name = "StreamReference4",
                    SourceLink = "http://odata.org/namedstream/sourcelink",
                    EditLink = "http://odata.org/namedstream/editlink",
                };

                // read link, edit link and ETag
                yield return new NamedStreamInstance()
                {
                    Name = "StreamReference5",
                    SourceLink = "http://odata.org/namedstream/sourcelink",
                    EditLink = "http://odata.org/namedstream/editlink",
                    ETag = "namedstream:etag",
                };
            }

            // read link, edit link, content type and ETag
            yield return new NamedStreamInstance()
            {
                Name = "StreamReference6",
                SourceLink = "http://odata.org/namedstream/sourcelink",
                EditLink = "http://odata.org/namedstream/editlink",
                EditLinkContentType = "namedstream:editlink:contenttype",
                ETag = "namedstream:etag",
            };
        }

        /// <summary>
        /// Creates a set of interesting homogeneous collection values with primitive and complex items.
        /// </summary>
        /// <param name="model">The model to add complex types to.</param>
        /// <param name="withTypeNames">true if the collection and complex value payloads should specify type names.</param>
        /// <param name="withExpectedType">true if an expected type annotation should be added to the generated payload element; otherwise false.</param>
        /// <param name="withcollectionName">true if the collection is not in the top level, otherwise false</param>
        /// <param name="fullSet">true if all available collection values should be returned, false if only the most interesting subset should be returned.</param>
        /// <returns>List of interesting collection values.</returns>
        public static IEnumerable<ODataPayloadElementCollection> CreateHomogeneousCollectionValues(
            EdmModel model,
            bool withTypeNames,
            bool withExpectedType,
            bool withcollectionName,
            bool fullset = true)
        {
            IEdmTypeReference itemTypeAnnotationType = null;
            IEdmTypeReference collectionTypeAnnotationType = null;
            EdmOperationImport primitiveCollectionFunctionImport = null;
            EdmEntityContainer defaultContainer = null;

            if (model != null)
            {
                defaultContainer = model.FindEntityContainer("TestModel.TestContainer") as EdmEntityContainer;
                if (defaultContainer == null)
                {
                    defaultContainer = new EdmEntityContainer("TestModel", "TestContainer");
                    model.AddElement(defaultContainer);
                }

                itemTypeAnnotationType = EdmCoreModel.Instance.GetString(true);
                collectionTypeAnnotationType = EdmCoreModel.GetCollection(itemTypeAnnotationType);

                var function = new EdmFunction("TestModel", "PrimitiveCollectionFunctionImport", collectionTypeAnnotationType);
                model.AddElement(function);
                primitiveCollectionFunctionImport = defaultContainer.AddFunctionImport("PrimitiveCollectionFunctionImport", function);
            }

            // primitive collection with single null item
            yield return new PrimitiveCollection(PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(itemTypeAnnotationType))
                .WithTypeAnnotation(collectionTypeAnnotationType)
                .ExpectedCollectionItemType(withExpectedType ? itemTypeAnnotationType : null)
                .ExpectedFunctionImport(withExpectedType ? primitiveCollectionFunctionImport : null)
                .CollectionName(withcollectionName ? "PrimitiveCollectionFunctionImport" : null);

            // primitive collection with multiple items (same type)
            yield return new PrimitiveCollection(
                PayloadBuilder.PrimitiveValue("Vienna").WithTypeAnnotation(itemTypeAnnotationType),
                PayloadBuilder.PrimitiveValue("Prague").WithTypeAnnotation(itemTypeAnnotationType),
                PayloadBuilder.PrimitiveValue("Redmond").WithTypeAnnotation(itemTypeAnnotationType)
                )
                .WithTypeAnnotation(collectionTypeAnnotationType)
                .ExpectedCollectionItemType(withExpectedType ? itemTypeAnnotationType : null)
                .ExpectedFunctionImport(withExpectedType ? primitiveCollectionFunctionImport : null)
                .CollectionName(withcollectionName ? "PrimitiveCollectionFunctionImport" : null);

            if (fullset)
            {
                // empty primitive collection
                yield return new PrimitiveCollection()
                    .WithTypeAnnotation(collectionTypeAnnotationType)
                    .ExpectedCollectionItemType(withExpectedType ? itemTypeAnnotationType : null)
                    .ExpectedFunctionImport(withExpectedType ? primitiveCollectionFunctionImport : null)
                    .CollectionName(withcollectionName ? "PrimitiveCollectionFunctionImport" : null);

                // primitive collection with a single item
                yield return new PrimitiveCollection(
                    PayloadBuilder.PrimitiveValue("Vienna").WithTypeAnnotation(itemTypeAnnotationType)
                    ).WithTypeAnnotation(collectionTypeAnnotationType)
                    .ExpectedCollectionItemType(withExpectedType ? itemTypeAnnotationType : null)
                    .ExpectedFunctionImport(withExpectedType ? primitiveCollectionFunctionImport : null)
                    .CollectionName(withcollectionName ? "PrimitiveCollectionFunctionImport" : null);

                // primitive collection with multiple null items
                yield return new PrimitiveCollection(
                    PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(itemTypeAnnotationType),
                    PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(itemTypeAnnotationType),
                    PayloadBuilder.PrimitiveValue(null).WithTypeAnnotation(itemTypeAnnotationType)
                    ).WithTypeAnnotation(collectionTypeAnnotationType)
                    .ExpectedCollectionItemType(withExpectedType ? itemTypeAnnotationType : null)
                    .ExpectedFunctionImport(withExpectedType ? primitiveCollectionFunctionImport : null)
                    .CollectionName(withcollectionName ? "PrimitiveCollectionFunctionImport" : null);
            }
        }

        /// <summary>
        /// Create a set of interesting deferred navigation links.
        /// </summary>
        /// <returns>Enumeration of interesting navigation links.</returns>
        public static IEnumerable<NavigationPropertyInstance> CreateDeferredNavigationLinks()
        {
            yield return PayloadBuilder.NavigationProperty("NavPropWithAssociationUri", "http://odata.org/NavProp").IsCollection(false);
            yield return PayloadBuilder.NavigationProperty("NavPropWithAssociationUri", "http://odata.org/NavProp", "http://odata.org/NavPropWithAssociationUri");

            // TODO: Add test cases that use relative URIs (if possible)
        }
        /// <summary>
        /// Returns a new complex property with different primitive values but the same property types
        /// </summary>
        /// <param name="complexProperty">Complex property as a base to build the new one</param>
        /// <returns>New complex property with the same property types but different primitive values</returns>
        public static ComplexProperty GetComplexPropertyWithDifferentPrimitiveValues(ComplexProperty complexProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexProperty, "complexProperty");
            if (complexProperty.Value == null)
            {
                // return a copy of the same since we don't know how to provide different primitive values since the value of the original property is null
                return complexProperty.DeepCopy();
            }
            else
            {
                var newProp = complexProperty.DeepCopy();
                newProp.Value = GetComplexValueWithDifferentValues(complexProperty.Value);
                return newProp;
            }
        }

        /// <summary>
        /// Create Complex values with the same type as the given complex Value but different property values
        /// </summary>
        /// <param name="complexValue">Complex value to create other values</param>
        /// <returns>An IEnumerable of complex values</returns>
        public static ComplexInstance GetComplexValueWithDifferentValues(ComplexInstance complexValue)
        {
            if (complexValue == null)
            {
                return null;
            }
            List<PropertyInstance> newPropertyValues = new List<PropertyInstance>();
            foreach (var prop in complexValue.Properties)
            {
                var primProp = prop as PrimitiveProperty;
                if (primProp != null)
                {
                    var newProp = primProp.DeepCopy();
                    newProp.Value = GetDifferentPrimitiveValue(primProp.Value);
                    newPropertyValues.Add(newProp);
                }

                var complexProperty = prop as ComplexProperty;
                if (complexProperty != null)
                {
                    newPropertyValues.Add(GetComplexPropertyWithDifferentPrimitiveValues(complexProperty));
                }

                var complexCollectionProp = prop as ComplexMultiValueProperty;
                if (complexCollectionProp != null)
                {
                    var newProp = complexCollectionProp.DeepCopy();
                    if (complexCollectionProp.Value != null)
                    {
                        newProp.Value.Clear();
                        foreach (var complexPropValue in complexCollectionProp.Value)
                        {
                            newProp.Value.Add(GetComplexValueWithDifferentValues(complexPropValue));
                        }
                    }
                    newPropertyValues.Add(newProp);
                }

                var primCollectionProperty = prop as PrimitiveMultiValueProperty;
                if (primCollectionProperty != null)
                {
                    var newProp = primCollectionProperty.DeepCopy();
                    if (primCollectionProperty.Value != null)
                    {
                        newProp.Value.Clear();
                        List<PrimitiveValue> usedValues = new List<PrimitiveValue>();
                        foreach (var primitiveVal in primCollectionProperty.Value)
                        {
                            var newPrimValue = GetDifferentPrimitiveValue(primitiveVal, usedValues);
                            newProp.Value.Add(newPrimValue);
                            usedValues.Add(newPrimValue);
                        }
                    }
                    newPropertyValues.Add(newProp);
                }
            }

            var newValue = complexValue.DeepCopy();
            newValue.Properties = newPropertyValues;

            return newValue;
        }

        /// <summary>
        /// Finds a different(not guaranteed) primitive value of the same type as the value passed
        /// </summary>
        /// <param name="currentValue">primitive value for which to provide a different value</param>
        /// <param name="usedValues">Optional used values to exclude when returning a new value</param>
        /// <returns>A new primitive value with different value(not guaranteed) but the same type</returns>
        public static PrimitiveValue GetDifferentPrimitiveValue(PrimitiveValue currentValue, IEnumerable<PrimitiveValue> usedValues = null)
        {
            if (currentValue == null)
            {
                return null;
            }
            if (currentValue.ClrValue == null)
            {
                return currentValue.DeepCopy();
            }

            Type expectedClrType = currentValue.ClrValue.GetType();
            var valuesOfExpectedType = TestValues.CreatePrimitiveValuesWithMetadata(true).Where(v => v.ClrValue != null && v.ClrValue.GetType() == expectedClrType && v.ClrValue != currentValue.ClrValue);
            // Make sure that our list of primitives has atleast one value with the expected type
            ExceptionUtilities.Assert(valuesOfExpectedType.Count() > 0,
                "CreatePrimitiveValuesWithMetadata does not have a primitve value with the clr type: " + expectedClrType.ToString() + " that is different from " + currentValue.ClrValue.ToString());

            // Find unused values
            var unUsedValues = valuesOfExpectedType;
            if (usedValues != null)
            {
                unUsedValues = unUsedValues.Where(v => !usedValues.Any(usedVal => usedVal.ClrValue == currentValue.ClrValue));
            }
            var newPrimitiveValue = unUsedValues.Count() > 0 ? unUsedValues.First() : valuesOfExpectedType.First();

            // Make a deep copy of the original and replace only the clr value so that we won't touch annotations etc.
            var newValueToBeReturned = currentValue.DeepCopy();
            newValueToBeReturned.ClrValue = newPrimitiveValue.ClrValue;
            return newValueToBeReturned;
        }

        /// <summary>
        /// Returns the namespace qualified type name.
        /// </summary>
        /// <param name="typeName">type name without the namespace.</param>
        /// <returns>Returns the namespace qualified type name.</returns>
        private static string GetFullTypeName(string typeName)
        {
            return NamespaceName + "." + typeName;
        }
    }
}
