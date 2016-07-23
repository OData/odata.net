//---------------------------------------------------------------------
// <copyright file="ObjectModelUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for creating and modifying OData object model instances.
    /// </summary>
    public static class ObjectModelUtils
    {
        /// <summary>The ID value for the default entry.</summary>
        public static readonly Uri DefaultEntryId = new Uri("http://www.odata.org/entryid");

        /// <summary>The read link value for the default entry.</summary>
        public static readonly Uri DefaultEntryReadLink = new Uri("http://www.odata.org/entry/readlink");

        /// <summary>The updated value for the default entry.</summary>
        public const string DefaultEntryUpdated = "2010-10-12T17:13:00Z";

        /// <summary>The updated value for the default entry.</summary>
        public static readonly DateTimeOffset DefaultEntryUpdatedDateTime = DateTimeOffset.Parse(DefaultEntryUpdated);

        /// <summary>The ID value for the default feed.</summary>
        public static readonly Uri DefaultFeedId = new Uri("http://www.odata.org/feedid");

        /// <summary>The updated value for the default feed.</summary>
        public const string DefaultFeedUpdated = "2010-10-10T10:10:10Z";

        /// <summary>The name value for a default link.</summary>
        public const string DefaultLinkName = "SampleLinkName";

        /// <summary>The Url for a default link.</summary>
        public static readonly Uri DefaultLinkUrl = new Uri("http://odata.org/link");

        /// <summary>Read link for the default stream.</summary>
        public static readonly Uri DefaultStreamReadLink = new Uri("http://odata.org/defaultstream");

        /// <summary>Content type for the default stream.</summary>
        public static readonly string DefaultStreamContentType = "application/binary";

        /// <summary>The Url for a default association link.</summary>
        public static readonly Uri DefaultAssociationLinkUrl = new Uri("http://odata.org/associationlink");

        /// <summary>The Url for a default entity reference link.</summary>
        public static readonly Uri DefaultEntityReferenceLinkUrl = new Uri("http://odata.org/entityreferencelink");

        /// <summary>The default error code.</summary>
        public static readonly string DefaultErrorCode = "Default error code";

        /// <summary>The default error message.</summary>
        public static readonly string DefaultErrorMessage = "Default error message.";

        /// <summary>The default inner error.</summary>
        public static readonly ODataInnerError DefaultInnerError = new ODataInnerError { Message = "Default inner error." };

        /// <summary>The default error edm model namespace.</summary>
        public static readonly string DefaultNamespaceName = "TestModel";

        /// <summary>Default Geography type values.</summary>
        public static readonly Geography GeographyValue;
        public static readonly GeographyPoint GeographyPointValue;
        public static readonly GeographyLineString GeographyLineStringValue;
        public static readonly GeographyPolygon GeographyPolygonValue;
        public static readonly GeographyCollection GeographyCollectionValue;
        public static readonly GeographyMultiPoint GeographyMultiPointValue;
        public static readonly GeographyMultiLineString GeographyMultiLineStringValue;
        public static readonly GeographyMultiPolygon GeographyMultiPolygonValue;

        /// <summary>Default Geometry type values.</summary>
        public static readonly Geometry GeometryValue;
        public static readonly GeometryPoint GeometryPointValue;
        public static readonly GeometryLineString GeometryLineStringValue;
        public static readonly GeometryPolygon GeometryPolygonValue;
        public static readonly GeometryCollection GeometryCollectionValue;
        public static readonly GeometryMultiPoint GeometryMultiPointValue;
        public static readonly GeometryMultiLineString GeometryMultiLineStringValue;
        public static readonly GeometryMultiPolygon GeometryMultiPolygonValue;

        private static readonly ODataResourceSerializationInfo MySerializationInfo = new ODataResourceSerializationInfo()
        {
            NavigationSourceName = "MySet",
        };

        static ObjectModelUtils()
        {
            GeographyValue = GeographyFactory.Point(32.0, -100.0).Build();
            GeographyPointValue = GeographyFactory.Point(33.1, -110.0).Build();
            GeographyLineStringValue = GeographyFactory.LineString(33.1, -110.0).LineTo(35.97, -110).Build();
            GeographyPolygonValue = GeographyFactory.Polygon().Ring(33.1, -110.0).LineTo(35.97, -110.15).LineTo(11.45, 87.75).Ring(35.97, -110).LineTo(36.97, -110.15).LineTo(45.23, 23.18).Build();
            GeographyCollectionValue = GeographyFactory.Collection().Point(-19.99, -12.0).Build();
            GeographyMultiPointValue = GeographyFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build();
            GeographyMultiLineStringValue = GeographyFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build();
            GeographyMultiPolygonValue = GeographyFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build();

            GeometryValue = GeometryFactory.Point(32.0, -10.0).Build();
            GeometryPointValue = GeometryFactory.Point(33.1, -11.0).Build();
            GeometryLineStringValue = GeometryFactory.LineString(33.1, -11.5).LineTo(35.97, -11).Build();
            GeometryPolygonValue = GeometryFactory.Polygon().Ring(33.1, -13.6).LineTo(35.97, -11.15).LineTo(11.45, 87.75).Ring(35.97, -11).LineTo(36.97, -11.15).LineTo(45.23, 23.18).Build();
            GeometryCollectionValue = GeometryFactory.Collection().Point(-19.99, -12.0).Build();
            GeometryMultiPointValue = GeometryFactory.MultiPoint().Point(10.2, 11.2).Point(11.9, 11.6).Build();
            GeometryMultiLineStringValue = GeometryFactory.MultiLineString().LineString(10.2, 11.2).LineTo(11.9, 11.6).LineString(16.2, 17.2).LineTo(18.9, 19.6).Build();
            GeometryMultiPolygonValue = GeometryFactory.MultiPolygon().Polygon().Ring(10.2, 11.2).LineTo(11.9, 11.6).LineTo(11.45, 87.75).Ring(16.2, 17.2).LineTo(18.9, 19.6).LineTo(11.45, 87.75).Build();
        }

        public static ODataProperty[] CreateDefaultPrimitiveProperties(EdmModel model = null)
        {
            if (model != null)
            {
                var entryWithPrimitiveProperties = new EdmEntityType(DefaultNamespaceName, "EntryWithPrimitiveProperties");
                entryWithPrimitiveProperties.AddKeys(entryWithPrimitiveProperties.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(isNullable: false)));
                entryWithPrimitiveProperties.AddStructuralProperty("Null", EdmCoreModel.Instance.GetString(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("Double", EdmCoreModel.Instance.GetDouble(isNullable: false));

                entryWithPrimitiveProperties.AddStructuralProperty("Binary", EdmCoreModel.Instance.GetBinary(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("Single", EdmCoreModel.Instance.GetSingle(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Boolean", EdmCoreModel.Instance.GetBoolean(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Byte", EdmCoreModel.Instance.GetByte(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("DateTimeOffset1", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("DateTimeOffset2", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("DateTimeOffset3", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Decimal", EdmCoreModel.Instance.GetDecimal(isNullable: false));

                entryWithPrimitiveProperties.AddStructuralProperty("Guid", EdmCoreModel.Instance.GetGuid(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("SByte", EdmCoreModel.Instance.GetSByte(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Int16", EdmCoreModel.Instance.GetInt16(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Int32", EdmCoreModel.Instance.GetInt32(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Int64", EdmCoreModel.Instance.GetInt64(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("String", EdmCoreModel.Instance.GetString(isNullable: false));
                entryWithPrimitiveProperties.AddStructuralProperty("Duration", EdmCoreModel.Instance.GetDuration(isNullable: false));

                entryWithPrimitiveProperties.AddStructuralProperty("Geography", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyCollection", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyMultiPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyMultiLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeographyMultiPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, false));

                entryWithPrimitiveProperties.AddStructuralProperty("Geometry", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryCollection", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryMultiPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryMultiLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, false));
                entryWithPrimitiveProperties.AddStructuralProperty("GeometryMultiPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, false));

                entryWithPrimitiveProperties.AddStructuralProperty("NullableDouble", EdmCoreModel.Instance.GetDouble(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableSingle", EdmCoreModel.Instance.GetSingle(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableBoolean", EdmCoreModel.Instance.GetBoolean(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableByte", EdmCoreModel.Instance.GetByte(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableDateTimeOffset1", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableDateTimeOffset2", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableDateTimeOffset3", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableDecimal", EdmCoreModel.Instance.GetDecimal(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableGuid", EdmCoreModel.Instance.GetGuid(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableSByte", EdmCoreModel.Instance.GetSByte(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableInt16", EdmCoreModel.Instance.GetInt16(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableInt32", EdmCoreModel.Instance.GetInt32(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableInt64", EdmCoreModel.Instance.GetInt64(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableString", EdmCoreModel.Instance.GetString(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullableDuration", EdmCoreModel.Instance.GetDuration(isNullable: true));

                entryWithPrimitiveProperties.AddStructuralProperty("NullDouble", EdmCoreModel.Instance.GetDouble(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullBinary", EdmCoreModel.Instance.GetBinary(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullSingle", EdmCoreModel.Instance.GetSingle(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullBoolean", EdmCoreModel.Instance.GetBoolean(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullByte", EdmCoreModel.Instance.GetByte(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullDateTimeOffset1", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullDateTimeOffset2", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullDateTimeOffset3", EdmCoreModel.Instance.GetDateTimeOffset(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullDecimal", EdmCoreModel.Instance.GetDecimal(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGuid", EdmCoreModel.Instance.GetGuid(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullSByte", EdmCoreModel.Instance.GetSByte(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullInt16", EdmCoreModel.Instance.GetInt16(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullInt32", EdmCoreModel.Instance.GetInt32(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullInt64", EdmCoreModel.Instance.GetInt64(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullString", EdmCoreModel.Instance.GetString(isNullable: true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullDuration", EdmCoreModel.Instance.GetDuration(isNullable: true));

                entryWithPrimitiveProperties.AddStructuralProperty("NullGeography", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPoint, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyLineString, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyPolygon, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyCollection", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyCollection, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyMultiPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPoint, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyMultiLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiLineString, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeographyMultiPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeographyMultiPolygon, true));

                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometry", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geometry, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPoint, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryLineString, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryPolygon, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryCollection", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryCollection, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryMultiPoint", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPoint, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryMultiLineString", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiLineString, true));
                entryWithPrimitiveProperties.AddStructuralProperty("NullGeometryMultiPolygon", EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.GeometryMultiPolygon, true));

                model.AddElement(entryWithPrimitiveProperties);
            }

            return new ODataProperty[]
            {
                new ODataProperty() { Name = "Null", Value = null },
                new ODataProperty() { Name = "Double", Value = (double)1 },
                new ODataProperty() { Name = "Binary", Value = new byte[] { 0, 1, 0, 1} },
                new ODataProperty() { Name = "Single", Value = (Single)1 },
                new ODataProperty() { Name = "Boolean", Value = true },
                new ODataProperty() { Name = "Byte", Value = (byte)1 },
                new ODataProperty() { Name = "DateTimeOffset1", Value = DateTimeOffset.Parse("2010-10-10T10:10:10Z") },
                new ODataProperty() { Name = "DateTimeOffset2", Value = DateTimeOffset.Parse("2010-10-10T10:10:10+01:00") },
                new ODataProperty() { Name = "DateTimeOffset3", Value = DateTimeOffset.Parse("2010-10-10T10:10:10-08:00") },
                new ODataProperty() { Name = "Decimal", Value = (decimal)1 },
                new ODataProperty() { Name = "Guid", Value = new Guid("11111111-2222-3333-4444-555555555555") },
                new ODataProperty() { Name = "SByte", Value = (sbyte)1 },
                new ODataProperty() { Name = "Int16", Value = (Int16)1 },
                new ODataProperty() { Name = "Int32", Value = (Int32)1 },
                new ODataProperty() { Name = "Int64", Value = (Int64)1 },
                new ODataProperty() { Name = "String", Value = "1" },
                new ODataProperty() { Name = "Duration", Value = TimeSpan.FromMinutes(12.34) },
                new ODataProperty() { Name = "Geography", Value = GeographyValue },
                new ODataProperty() { Name = "GeographyPoint", Value =  GeographyPointValue},
                new ODataProperty() { Name = "GeographyLineString", Value = GeographyLineStringValue },
                new ODataProperty() { Name = "GeographyPolygon", Value = GeographyPolygonValue },
                new ODataProperty() { Name = "GeographyCollection", Value = GeographyCollectionValue },
                new ODataProperty() { Name = "GeographyMultiPoint", Value = GeographyMultiPointValue },
                new ODataProperty() { Name = "GeographyMultiLineString", Value = GeographyMultiLineStringValue },
                new ODataProperty() { Name = "GeographyMultiPolygon", Value = GeographyMultiPolygonValue },

                new ODataProperty() { Name = "Geometry", Value = GeometryValue },
                new ODataProperty() { Name = "GeometryPoint", Value = GeometryPointValue },
                new ODataProperty() { Name = "GeometryLineString", Value = GeometryLineStringValue },
                new ODataProperty() { Name = "GeometryPolygon", Value = GeometryPolygonValue },
                new ODataProperty() { Name = "GeometryCollection", Value = GeometryCollectionValue },
                new ODataProperty() { Name = "GeometryMultiPoint", Value = GeometryMultiPointValue },
                new ODataProperty() { Name = "GeometryMultiLineString", Value = GeometryMultiLineStringValue },
                new ODataProperty() { Name = "GeometryMultiPolygon", Value = GeometryMultiPolygonValue },

                new ODataProperty() { Name = "NullableDouble", Value = (double?)1 },
                new ODataProperty() { Name = "NullableSingle", Value = (Single?)1 },
                new ODataProperty() { Name = "NullableBoolean", Value = (bool?)true },
                new ODataProperty() { Name = "NullableByte", Value = (byte?)1 },
                new ODataProperty() { Name = "NullableDateTimeOffset1", Value = (DateTimeOffset?)DateTimeOffset.Parse("2010-10-10T10:10:10Z") },
                new ODataProperty() { Name = "NullableDateTimeOffset2", Value = (DateTimeOffset?)DateTimeOffset.Parse("2010-10-10T10:10:10+01:00") },
                new ODataProperty() { Name = "NullableDateTimeOffset3", Value = (DateTimeOffset?)DateTimeOffset.Parse("2010-10-10T10:10:10-08:00") },
                new ODataProperty() { Name = "NullableDecimal", Value = (decimal?)1 },
                new ODataProperty() { Name = "NullableGuid", Value = (Guid?)new Guid("11111111-2222-3333-4444-555555555555") },
                new ODataProperty() { Name = "NullableSByte", Value = (sbyte?)1 },
                new ODataProperty() { Name = "NullableInt16", Value = (Int16?)1 },
                new ODataProperty() { Name = "NullableInt32", Value = (Int32?)1 },
                new ODataProperty() { Name = "NullableInt64", Value = (Int64?)1 },
                new ODataProperty() { Name = "NullableString", Value = "1" },
                new ODataProperty() { Name = "NullableDuration", Value = (TimeSpan?)TimeSpan.FromMinutes(12.34) },

                new ODataProperty() { Name = "NullDouble", Value = null },
                new ODataProperty() { Name = "NullBinary", Value = null },
                new ODataProperty() { Name = "NullSingle", Value = null },
                new ODataProperty() { Name = "NullBoolean", Value = null },
                new ODataProperty() { Name = "NullByte", Value = null },
                new ODataProperty() { Name = "NullDateTimeOffset1", Value = null },
                new ODataProperty() { Name = "NullDateTimeOffset2", Value = null },
                new ODataProperty() { Name = "NullDateTimeOffset3", Value = null },
                new ODataProperty() { Name = "NullDecimal", Value = null },
                new ODataProperty() { Name = "NullGuid", Value = null },
                new ODataProperty() { Name = "NullSByte", Value = null },
                new ODataProperty() { Name = "NullInt16", Value = null },
                new ODataProperty() { Name = "NullInt32", Value = null },
                new ODataProperty() { Name = "NullInt64", Value = null },
                new ODataProperty() { Name = "NullString", Value = null },
                new ODataProperty() { Name = "NullDuration", Value = null },
                new ODataProperty() { Name = "NullGeography", Value = null },
                new ODataProperty() { Name = "NullGeographyPoint", Value = null },
                new ODataProperty() { Name = "NullGeographyLineString", Value = null },
                new ODataProperty() { Name = "NullGeographyPolygon", Value = null },
                new ODataProperty() { Name = "NullGeographyMultiPoint", Value = null },
                new ODataProperty() { Name = "NullGeographyMultiLineString", Value = null },
                new ODataProperty() { Name = "NullGeographyMultiPolygon", Value = null },
                new ODataProperty() { Name = "NullGeographyCollection", Value = null },
                new ODataProperty() { Name = "NullGeometry", Value = null },
                new ODataProperty() { Name = "NullGeometryPoint", Value = null },
                new ODataProperty() { Name = "NullGeometryLineString", Value = null },
                new ODataProperty() { Name = "NullGeometryPolygon", Value = null },
                new ODataProperty() { Name = "NullGeometryMultiPoint", Value = null },
                new ODataProperty() { Name = "NullGeometryMultiLineString", Value = null },
                new ODataProperty() { Name = "NullGeometryMultiPolygon", Value = null },
                new ODataProperty() { Name = "NullGeometryCollection", Value = null },
            };
        }

        public static ODataItem[][] CreateDefaultComplexProperties(EdmModel model = null)
        {
            if (model != null)
            {
                var addressType = model.ComplexType("AddressType", "My")
                    .Property("Street", EdmPrimitiveTypeKind.String)
                    .Property("City", EdmPrimitiveTypeKind.String);
                var streetType = model.ComplexType("StreetType", "My")
                    .Property("StreetName", EdmPrimitiveTypeKind.String)
                    .Property("Number", EdmPrimitiveTypeKind.Int32);
                var nestedAddressType = model.ComplexType("NestedAddressType", "My")
                    .Property("Street", new EdmComplexTypeReference(streetType, true))
                    .Property("City", EdmPrimitiveTypeKind.String);

                model.EntityType("EntryWithComplexProperties", "TestModel")
                    .Property("ComplexAddress", new EdmComplexTypeReference(addressType, true))
                    .Property("NestedComplex", new EdmComplexTypeReference(nestedAddressType, true));
            }

            return new ODataItem[][]
            {
                new ODataItem[]
                {
                    new ODataResource()
                     {
                        TypeName = "My.AddressType",
                        Properties = new []
                        {
                            new ODataProperty() { Name = "Street", Value = "One Redmond Way" },
                            new ODataProperty() { Name = "City", Value = " Redmond" },
                        }
                    }
                },

                new ODataItem[]
                {
                    new ODataResource()
                    {
                        TypeName = "My.NestedAddressType",
                        Properties = new []
                        {
                            new ODataProperty() { Name = "City", Value = "Redmond " }
                        }
                    },
                    new ODataNestedResourceInfo()
                    {
                        Name = "Street",
                        IsCollection = false
                    },
                    new ODataResource
                    {
                        TypeName = "My.StreetType",
                        Properties = new []
                        {
                            new ODataProperty { Name = "StreetName", Value = "One Redmond Way" },
                            new ODataProperty { Name = "Number", Value = 1234 },
                        }
                    }
                }
            };
        }

        public static ODataProperty[] CreateDefaultCollectionProperties(EdmModel model = null)
        {
            if (model != null)
            {
                var addressType = model.ComplexType("AddressType", "My")
                    .Property("Street", EdmPrimitiveTypeKind.String)
                    .Property("City", EdmPrimitiveTypeKind.String);

                model.EntityType("EntryWithCollectionProperties", "TestModel")
                    .Property("EmptyCollection", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))))
                    .Property("PrimitiveCollection", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))))
                    .Property("IntCollectionNoTypeName", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(false))))
                    .Property("StringCollectionNoTypeName", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(false))))
                    .Property("GeographyCollectionNoTypeName", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSpatial(EdmPrimitiveTypeKind.Geography, false))));
                    //.Property("ComplexCollection", new EdmCollectionTypeReference(new EdmCollectionType(new EdmComplexTypeReference(addressType, true))));
            }

            return new ODataProperty[]
            {
                new ODataProperty
                {
                    Name = "EmptyCollection",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String"),
                    }

                },
                new ODataProperty
                {
                    Name = "PrimitiveCollection",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                        Items = new object[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }
                    }
                },
                new ODataProperty
                {
                    Name = "IntCollectionNoTypeName",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Int32"),
                        Items = new object[] { 0, 1, 2 }
                    }
                },
                new ODataProperty
                {
                    Name = "StringCollectionNoTypeName",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("Edm.String"),
                        Items = new string[] { "One", "Two", "Three" }
                    }
                },
                new ODataProperty
                {
                    Name = "GeographyCollectionNoTypeName",
                    Value = new ODataCollectionValue()
                    {
                        TypeName = EntityModelUtils.GetCollectionTypeName("Edm.Geography"),
                        Items = new object[]
                        {
                            ObjectModelUtils.GeographyCollectionValue,
                            ObjectModelUtils.GeographyLineStringValue,
                            ObjectModelUtils.GeographyMultiLineStringValue,
                            ObjectModelUtils.GeographyMultiPointValue,
                            ObjectModelUtils.GeographyMultiPolygonValue,
                            ObjectModelUtils.GeographyPointValue,
                            ObjectModelUtils.GeographyPolygonValue,
                            ObjectModelUtils.GeographyValue
                        }
                    }
                },
                //new ODataProperty
                //{
                //    Name = "ComplexCollection",
                //    Value = new ODataCollectionValue()
                //    {
                //        TypeName = EntityModelUtils.GetCollectionTypeName("My.AddressType"),
                //        Items = new []
                //        {
                //            new ODataComplexValue()
                //            {
                //                TypeName = "My.AddressType",
                //                Properties = new []
                //                {
                //                    new ODataProperty() { Name = "Street", Value = "One Redmond Way" },
                //                    new ODataProperty() { Name = "City", Value = " Redmond" },
                //                }
                //            },
                //            new ODataComplexValue()
                //            {
                //                TypeName = null,
                //                Properties = new []
                //                {
                //                    new ODataProperty() { Name = "Street", Value = "Am Euro Platz 3" },
                //                    new ODataProperty() { Name = "City", Value = "Vienna " },
                //                }
                //            }
                //        }
                //    }
                //},
            };
        }

        /// <summary>
        /// Creates a an ODataResourceSet instance with default values for 'Id'
        /// that can be used and modified in tests.
        /// </summary>
        /// <param name="entitySetName">The (optional) name of the entity set to create.</param>
        /// <param name="entityTypeName">The optional type name for the entries in this feed.</param>
        /// <param name="model">The product model to generate the type in (if not null).</param>
        /// <returns>The created ODataResourceSet instance.</returns>
        public static ODataResourceSet CreateDefaultFeed(string entitySetName = null, string entityTypeName = null, EdmModel model = null)
        {
            if (model != null && entityTypeName != null)
            {
                EdmEntityType entityType = new EdmEntityType(DefaultNamespaceName, entityTypeName);
                entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(isNullable: false)));
                model.AddElement(entityType);

                EdmEntityContainer container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
                model.AddElement(container);

                if (entitySetName != null)
                {
                    container.AddEntitySet(entitySetName, entityType);
                }
            }

            return new ODataResourceSet()
            {
                Id = DefaultFeedId,
                SerializationInfo = MySerializationInfo
            };
        }

        /// <summary>
        /// Creates a an ODataResourceSet instance with default values for 'Id' and 'Updated'
        /// that can be used and modified in tests.
        /// </summary>
        /// <returns>The created ODataResourceSet instance.</returns>
        public static ODataResourceSet CreateDefaultFeedWithAtomMetadata()
        {
            ODataResourceSet feed = new ODataResourceSet()
            {
                Id = DefaultFeedId,
                SerializationInfo = MySerializationInfo
            };
            return feed;
        }

        /// <summary>
        /// Creates a default ODataNestedResourceInfo instance for a collection with only the Name and Url properties set.
        /// </summary>
        /// <returns>The newly created ODataNestedResourceInfo instances.</returns>
        public static ODataNestedResourceInfo CreateDefaultCollectionLink(string name = DefaultLinkName, bool? isCollection = true)
        {
            ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo()
            {
                Name = name,
                Url = DefaultLinkUrl,
                IsCollection = isCollection
            };
            return navigationLink;
        }

        /// <summary>
        /// Creates an ODataNestedResourceInfo instance with the default Name and Url property values and
        /// 'IsCollection' being set to false that can be used and modified in tests.
        /// </summary>
        /// <returns>The newly created ODataNestedResourceInfo instance.</returns>
        public static ODataNestedResourceInfo CreateDefaultSingletonLink(string name = DefaultLinkName)
        {
            ODataNestedResourceInfo navigationLink = CreateDefaultCollectionLink(name);
            navigationLink.IsCollection = false;
            return navigationLink;
        }

        /// <summary>
        /// Creates an ODataNestedResourceInfo instance with the default Name and Url property values and
        /// 'IsCollection' being set to false that can be used and modified in tests.
        /// </summary>
        /// <returns>The newly created ODataNestedResourceInfo instance.</returns>
        public static ODataNestedResourceInfo CreateDefaultNavigationLink(string name = DefaultLinkName, Uri associationLinkUrl = null)
        {
            ODataNestedResourceInfo navigationLink = CreateDefaultSingletonLink(name);
            navigationLink.AssociationLinkUrl = associationLinkUrl;
            return navigationLink;
        }

        /// <summary>
        /// Creates a default <see cref="ODataEntityReferenceLink"/> instance.
        /// </summary>
        /// <returns>The newly created <see cref="ODataEntityReferenceLink"/> instance.</returns>
        public static ODataEntityReferenceLink CreateDefaultEntityReferenceLink()
        {
            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink()
            {
                Url = DefaultEntityReferenceLinkUrl
            };
            return entityReferenceLink;
        }

        /// <summary>
        /// Creates a default <see cref="ODataEntityReferenceLinks"/> instance with a default
        /// entity reference link inside (but no inline count or next link).
        /// </summary>
        /// <returns>The newly created <see cref="ODataEntityReferenceLinks"/> instance.</returns>
        public static ODataEntityReferenceLinks CreateDefaultEntityReferenceLinks()
        {
            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks()
            {
                Links = new ODataEntityReferenceLink[] { CreateDefaultEntityReferenceLink() },
            };
            return entityReferenceLinks;
        }

        /// <summary>
        /// Creates an <see cref="ODataError"/> instance with the default value for 'ErrorCode' and 'Message'
        /// that can be used and modified in tests.
        /// </summary>
        /// <returns>The newly created <see cref="ODataError"/> instance.</returns>
        public static ODataAnnotatedError CreateDefaultError(bool includeDetails = false)
        {
            ODataError error = new ODataError()
            {
                ErrorCode = DefaultErrorCode,
                Message = DefaultErrorMessage,
                InnerError = includeDetails ? DefaultInnerError : null,
            };

            return new ODataAnnotatedError
            {
                Error = error,
                IncludeDebugInformation = includeDetails
            };
        }

        /// <summary>Special ODataResource instance to represent 'null' value to help TestWriterUtils.WritePayload()</summary>
        private static ODataResource nullEntry = new ODataResource();

        /// <summary>Special ODataResource instance to represent 'null' value to help TestWriterUtils.WritePayload()</summary>
        public static ODataResource ODataNullEntry
        {
            get
            {
                return nullEntry;
            }
        }

        /// <summary>Compares entry with the special 'null' instance</summary>
        public static bool IsNullEntry(this ODataResource entry)
        {
            return entry == nullEntry;
        }

        /// <summary>
        /// Creates an ODataResource instance with the default value for 'Id' and 'ReadLink'
        /// that can be used and modified in tests.
        /// </summary>
        /// <param name="typeName">The optional type name for the default entry.</param>
        /// <returns>The newly created ODataResource instance.</returns>
        public static ODataResource CreateDefaultEntry(string typeName = null)
        {
            return new ODataResource()
            {
                Id = DefaultEntryId,
                ReadLink = DefaultEntryReadLink,
                TypeName = typeName,
                SerializationInfo = MySerializationInfo
            };
        }

        /// <summary>
        /// Creates an ODataResource instance with the default values for 'Id', 'ReadLink' and 'Updated'
        /// that can be used and modified in tests.
        /// </summary>
        /// <param name="typeName">The optional type name for the default entry.</param>
        /// <param name="model">The product model to generate the type in (if not null).</param>
        /// <returns>The newly created ODataResource instance.</returns>
        public static ODataResource CreateDefaultEntryWithAtomMetadata(string entitySetName = null, string typeName = null, EdmModel model = null)
        {
            if (model != null && typeName != null)
            {
                EdmEntityType entityType = new EdmEntityType(DefaultNamespaceName, typeName);
                entityType.AddKeys(entityType.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(isNullable: false)));
                model.AddElement(entityType);

                typeName = entityType.FullName();

                EdmEntityContainer container = new EdmEntityContainer(DefaultNamespaceName, "DefaultContainer");
                model.AddElement(container);

                if (entitySetName != null)
                {
                    container.AddEntitySet(entitySetName, entityType);
                }
            }

            ODataResource entry = new ODataResource()
            {
                Id = DefaultEntryId,
                ReadLink = DefaultEntryReadLink,
                TypeName = typeName,
                SerializationInfo = MySerializationInfo
            };
            return entry;
        }

        /// <summary>
        /// Creates a default ODataStreamReferenceValue representing a default stream.
        /// </summary>
        /// <returns>The newly create media resource instance.</returns>
        public static ODataStreamReferenceValue CreateDefaultStream()
        {
            return new ODataStreamReferenceValue()
            {
                ReadLink = DefaultStreamReadLink,
                ContentType = DefaultStreamContentType
            };
        }

        /// <summary>
        /// Creates an (unnamed) <see cref="ODataServiceDocument"/>.
        /// </summary>
        /// <returns>The created <see cref="ODataServiceDocument"/> instance.</returns>
        public static ODataServiceDocument CreateDefaultWorkspace()
        {
            return new ODataServiceDocument();
        }

        /// <summary>
        /// Creates a default parameter payload.
        /// </summary>
        /// <returns>The created <see cref="ODataParameters"/> instance.</returns>
        public static ODataParameters CreateDefaultParameter()
        {
            var complex = new ODataResource()
            {
                TypeName = "My.NestedAddressType",
                Properties = new[]
                {
                    new ODataProperty() { Name = "City", Value = "Redmond " },
                }
            };

            var nestedInfo = new ODataNestedResourceInfo()
            {
                Name = "Street",
                IsCollection = false
            };

            var nestedStreet = new ODataResource()
            {
                TypeName = "My.StreetType",
                Properties = new[]
                {
                    new ODataProperty { Name = "StreetName", Value = "One Redmond Way" },
                    new ODataProperty { Name = "Number", Value = 1234 },
                }
            };

            nestedInfo.SetAnnotation(new ODataNavigationLinkExpandedItemObjectModelAnnotation() { ExpandedItem = nestedStreet });

            var navigationAnnotation = new ODataEntryNavigationLinksObjectModelAnnotation();
            navigationAnnotation.Add(nestedInfo, 0);

            complex.SetAnnotation(navigationAnnotation);

            var primitiveCollection = new ODataCollectionStart();
            primitiveCollection.SetAnnotation(new ODataCollectionItemsObjectModelAnnotation() { "Value1", "Value2", "Value3" });

            var complexCollection = new ODataResourceSet();
            complexCollection.SetAnnotation(new ODataFeedEntriesObjectModelAnnotation() { complex });

            return new ODataParameters()
            {
                new KeyValuePair<string, object>("primitiveParameter", "StringValue"),
                new KeyValuePair<string, object>("complexParameter", complex),
                new KeyValuePair<string, object>("primitiveCollectionParameter", primitiveCollection),
                new KeyValuePair<string, object>("complexCollectionParameter", complexCollection),
            };
        }
    }
}
