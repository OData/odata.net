//---------------------------------------------------------------------
// <copyright file="EdmDataTypes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Entity Data Model data types.
    /// </summary>
    public static class EdmDataTypes
    {
        private static EdmFullNameVisitor edmFullNameVisitor = new EdmFullNameVisitor();
        private static EdmShortQualifiedNameVisitor edmShortQalifiedNameVisitor = new EdmShortQualifiedNameVisitor();

        /// <summary>
        /// Gets the EDM Boolean type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static BooleanDataType Boolean
        {
            get
            {
                return DataTypes.Boolean
                    .WithPrimitiveClrType(typeof(bool))
                    .WithEdmName("Edm", "Boolean");
            }
        }

        /// <summary>
        /// Gets the EDM Byte type.
        /// </summary>
        /// <value>The Byte type.</value>
        public static IntegerDataType Byte
        {
            get
            {
                return DataTypes.Integer
                    .WithSize(8)
                    .WithPrimitiveClrType(typeof(byte))
                    .WithEdmName("Edm", "Byte");
            }
        }

        /// <summary>
        /// Gets the EDM SByte type.
        /// </summary>
        /// <value>The SByte type.</value>
        public static IntegerDataType SByte
        {
            get
            {
                return DataTypes.Integer
                    .WithSize(8)
                    .WithPrimitiveClrType(typeof(sbyte))
                    .WithEdmName("Edm", "SByte");
            }
        }

        /// <summary>
        /// Gets the EDM Int16 type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static IntegerDataType Int16
        {
            get
            {
                return DataTypes.Integer
                    .WithSize(16)
                    .WithPrimitiveClrType(typeof(short))
                    .WithEdmName("Edm", "Int16");
            }
        }

        /// <summary>
        /// Gets the EDM Int32 type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static IntegerDataType Int32
        {
            get
            {
                return DataTypes.Integer
                    .WithSize(32)
                    .WithPrimitiveClrType(typeof(int))
                    .WithEdmName("Edm", "Int32");
            }
        }

        /// <summary>
        /// Gets the EDM Int64 type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static IntegerDataType Int64
        {
            get
            {
                return DataTypes.Integer
                    .WithSize(64)
                    .WithPrimitiveClrType(typeof(long))
                    .WithEdmName("Edm", "Int64");
            }
        }

        /// <summary>
        /// Gets the EDM Single type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static FloatingPointDataType Single
        {
            get
            {
                return DataTypes.FloatingPoint
                    .WithPrimitiveClrType(typeof(float))
                    .WithEdmName("Edm", "Single");
            }
        }

        /// <summary>
        /// Gets the EDM Double type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static FloatingPointDataType Double
        {
            get
            {
                return DataTypes.FloatingPoint
                    .WithPrimitiveClrType(typeof(double))
                    .WithEdmName("Edm", "Double");
            }
        }

        /// <summary>
        /// Gets the EDM Geometry type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static SpatialDataType Geometry
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "Geometry");
            }
        }

        /// <summary>
        /// Gets the EDM Geography type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static SpatialDataType Geography
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "Geography");
            }
        }

        /// <summary>
        /// Gets the EDM Guid type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static GuidDataType Guid
        {
            get
            {
                return DataTypes.Guid
                    .WithPrimitiveClrType(typeof(Guid))
                    .WithEdmName("Edm", "Guid");
            }
        }

        /// <summary>
        /// Gets the EDM Stream type.
        /// </summary>
        /// <value>The EDM type.</value>
        public static StreamDataType Stream
        {
            get
            {
                return DataTypes.Stream
                    .WithPrimitiveClrType(typeof(Stream))
                    .WithEdmName("Edm", "Stream");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyPoint type.
        /// </summary>
        public static SpatialDataType GeographyPoint
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyPoint");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyLineString type.
        /// </summary>
        public static SpatialDataType GeographyLineString
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyLineString");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyPolygon type.
        /// </summary>
        public static SpatialDataType GeographyPolygon
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyPolygon");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyCollection type.
        /// </summary>
        public static SpatialDataType GeographyCollection
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyCollection");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyMultiPolygon type.
        /// </summary>
        public static SpatialDataType GeographyMultiPolygon
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyMultiPolygon");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyMultiLineString type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "Spelling is correct")]
        public static SpatialDataType GeographyMultiLineString
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyMultiLineString");
            }
        }

        /// <summary>
        /// Gets the EDM GeographyMultiPoint type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiPoint", Justification = "Spelling is correct")]
        public static SpatialDataType GeographyMultiPoint
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeographyMultiPoint");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryPoint type.
        /// </summary>
        public static SpatialDataType GeometryPoint
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryPoint");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryLineString type.
        /// </summary>
        public static SpatialDataType GeometryLineString
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryLineString");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryPolygon type.
        /// </summary>
        public static SpatialDataType GeometryPolygon
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryPolygon");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryCollection type.
        /// </summary>
        public static SpatialDataType GeometryCollection
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryCollection");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryMultiPolygon type.
        /// </summary>
        public static SpatialDataType GeometryMultiPolygon
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryMultiPolygon");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryMultiLineString type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine", Justification = "Spelling is correct")]
        public static SpatialDataType GeometryMultiLineString
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryMultiLineString");
            }
        }

        /// <summary>
        /// Gets the EDM GeometryMultiPoint type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiPoint", Justification = "Spelling is correct")]
        public static SpatialDataType GeometryMultiPoint
        {
            get
            {
                return DataTypes.Spatial.WithEdmName("Edm", "GeometryMultiPoint");
            }
        }

        /// <summary>
        /// Gets the EDM String type without facets.
        /// </summary>
        /// <returns>The EDM Type.</returns>
        public static StringDataType String()
        {
            return String(null, null);
        }

        /// <summary>
        /// Gets the EDM String type with facets.
        /// </summary>
        /// <param name="maxLength">Maximum length of the string.</param>
        /// <param name="isUnicode">Whether the string can be Unicode.</param>
        /// <returns>The EDM Type.</returns>
        public static StringDataType String(int? maxLength, bool? isUnicode)
        {
            var type = DataTypes.String.WithPrimitiveClrType(typeof(string));
            var debuggerDisplay = new StringBuilder();
            debuggerDisplay.Append("Edm.String(");
            string separator = string.Empty;

            if (maxLength.HasValue)
            {
                type = type.WithMaxLength(maxLength.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("MaxLength=" + maxLength);
                separator = ",";
            }

            if (isUnicode.HasValue)
            {
                type = type.WithUnicodeSupport(isUnicode.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("IsUnicode=" + isUnicode);
            }

            debuggerDisplay.Append(")");

            return type.WithEdmName("Edm", "String").WithDebuggerDisplay(debuggerDisplay.ToString());
        }

        /// <summary>
        /// Gets the EDM Binary type without facets.
        /// </summary>
        /// <returns>The EDM type.</returns>
        public static BinaryDataType Binary()
        {
            return Binary(null);
        }

        /// <summary>
        /// Gets the EDM Binary type with facets.
        /// </summary>
        /// <param name="maxLength">Maximum length of the binary.</param>
        /// <returns>The EDM type.</returns>
        public static BinaryDataType Binary(int? maxLength)
        {
            var type = DataTypes.Binary.WithPrimitiveClrType(typeof(byte[]));
            var debuggerDisplay = new StringBuilder();
            debuggerDisplay.Append("Edm.Binary(");

            if (maxLength.HasValue)
            {
                type = type.WithMaxLength(maxLength.Value);
                debuggerDisplay.Append("MaxLength=");
                debuggerDisplay.Append(maxLength.Value);
            }

            debuggerDisplay.Append(")");

            return type.WithEdmName("Edm", "Binary").WithDebuggerDisplay(debuggerDisplay.ToString());
        }

        /// <summary>
        /// Gets the EDM Decimal type without facets.
        /// </summary>
        /// <returns>The EDM type.</returns>
        public static FixedPointDataType Decimal()
        {
            return Decimal(null, null);
        }

        /// <summary>
        /// Gets the EDM Decimal type with facets.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The EDM type.</returns>
        public static FixedPointDataType Decimal(int? precision, int? scale)
        {
            var type = DataTypes.FixedPoint.WithPrimitiveClrType(typeof(decimal));
            var debuggerDisplay = new StringBuilder();
            debuggerDisplay.Append("Edm.Decimal(");

            string separator = string.Empty;

            if (precision.HasValue)
            {
                type = type.WithPrecision(precision.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("Precision=");
                debuggerDisplay.Append(precision.Value);
                separator = ",";
            }

            if (scale.HasValue)
            {
                type = type.WithScale(scale.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("Scale=");
                debuggerDisplay.Append(scale.Value);
            }

            debuggerDisplay.Append(")");
            return type.WithEdmName("Edm", "Decimal").WithDebuggerDisplay(debuggerDisplay.ToString());
        }

        /// <summary>
        /// Gets the EDM DateTime type without facets.
        /// </summary>
        /// <returns>The EDM type.</returns>
        public static DateTimeDataType DateTime()
        {
            return DateTime(null);
        }

        /// <summary>
        /// Gets the EDM DateTime type with specified precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>The EDM type.</returns>
        public static DateTimeDataType DateTime(int? precision)
        {
            var type = DataTypes.DateTime.WithPrimitiveClrType(typeof(DateTime));

            var debuggerDisplay = new StringBuilder();
            debuggerDisplay.Append("Edm.DateTime(");

            string separator = string.Empty;

            if (precision.HasValue)
            {
                type = type.WithPrecision(precision.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("Precision=");
                debuggerDisplay.Append(precision.Value);
            }

            debuggerDisplay.Append(")");

            return type.WithEdmName("Edm", "DateTime")
                .WithDebuggerDisplay(debuggerDisplay.ToString());
        }

        /// <summary>
        /// Gets the EDM DateTimeOffset type.
        /// </summary>
        /// <returns>The EDM type.</returns>
        public static DateTimeDataType DateTimeOffset()
        {
            return DateTimeOffset(null);
        }

        /// <summary>
        /// Gets the EDM DateTimeOffset type with specified precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>The EDM type.</returns>
        public static DateTimeDataType DateTimeOffset(int? precision)
        {
            var type = DataTypes.DateTime.WithPrimitiveClrType(typeof(DateTimeOffset)).WithTimeZoneOffset(true);

            var debuggerDisplay = new StringBuilder();
            debuggerDisplay.Append("Edm.DateTimeOffset(");

            string separator = string.Empty;

            if (precision.HasValue)
            {
                type = type.WithPrecision(precision.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("Precision=");
                debuggerDisplay.Append(precision.Value);
            }

            debuggerDisplay.Append(")");

            return type.WithEdmName("Edm", "DateTimeOffset")
                .WithDebuggerDisplay(debuggerDisplay.ToString());
        }

        /// <summary>
        /// Gets the EDM Time type.
        /// </summary>
        /// <returns>The EDM type.</returns>
        public static TimeOfDayDataType Time()
        {
            return Time(null);
        }

        /// <summary>
        /// Gets the EDM Time type with specified precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>The EDM type.</returns>
        public static TimeOfDayDataType Time(int? precision)
        {
            var type = DataTypes.TimeOfDay.WithPrimitiveClrType(typeof(TimeSpan));

            var debuggerDisplay = new StringBuilder();
            debuggerDisplay.Append("Edm.Duration(");

            string separator = string.Empty;

            if (precision.HasValue)
            {
                type = type.WithPrecision(precision.Value);
                debuggerDisplay.Append(separator);
                debuggerDisplay.Append("Precision=");
                debuggerDisplay.Append(precision.Value);
            }

            debuggerDisplay.Append(")");

            return type.WithEdmName("Edm", "Duration")
                .WithDebuggerDisplay(debuggerDisplay.ToString());
        }

        /// <summary>
        /// Gets all edm primitive data types.
        /// </summary>
        /// <param name="edmVersion">Edm version.</param>
        /// <returns>all primitive data types in Edm (with defalut facet values).</returns>
        public static IEnumerable<PrimitiveDataType> GetAllPrimitiveTypes(EdmVersion edmVersion)
        {
            var result = new List<PrimitiveDataType>()
            {
                EdmDataTypes.Binary(),
                EdmDataTypes.Boolean,
                EdmDataTypes.Byte,
                EdmDataTypes.DateTimeOffset(),
                EdmDataTypes.Decimal(),
                EdmDataTypes.Double,
                EdmDataTypes.Guid,
                EdmDataTypes.Int16,
                EdmDataTypes.Int32,
                EdmDataTypes.Int64,
                EdmDataTypes.SByte,
                EdmDataTypes.Single,
                EdmDataTypes.Stream,
                EdmDataTypes.String(),
                EdmDataTypes.Time(),
            };

            if (edmVersion >= EdmVersion.V40)
            {
                result.AddRange(new PrimitiveDataType[] 
                {
                    EdmDataTypes.Geography,
                    EdmDataTypes.GeographyPoint,
                    EdmDataTypes.GeographyLineString,
                    EdmDataTypes.GeographyPolygon,
                    EdmDataTypes.GeographyCollection,
                    EdmDataTypes.GeographyMultiPolygon,
                    EdmDataTypes.GeographyMultiLineString,
                    EdmDataTypes.GeographyMultiPoint,
                    EdmDataTypes.Geometry,
                    EdmDataTypes.GeometryPoint,
                    EdmDataTypes.GeometryLineString,
                    EdmDataTypes.GeometryPolygon,
                    EdmDataTypes.GeometryCollection,
                    EdmDataTypes.GeometryMultiPolygon,
                    EdmDataTypes.GeometryMultiLineString,
                    EdmDataTypes.GeometryMultiPoint,
                });
            }

            return result;
        }

        /// <summary>
        /// Gets the full Edm name of a data type (if it's an valid Edm data type).
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>Full Edm name</returns>
        public static string GetEdmFullName(DataType dataType)
        {
            return edmFullNameVisitor.GetEdmFullName(dataType);
        }

        /// <summary>
        /// Gets the short qualified name of a data type (if it's an valid Edm data type).
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>short qualified name</returns>
        public static string GetEdmShortQualifiedName(DataType dataType)
        {
            return edmShortQalifiedNameVisitor.GetEdmShortQualifiedName(dataType);
        }

        /// <summary>
        /// Gets the Edm name of a data type
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>The Edm name</returns>
        public static string GetEdmName(DataType dataType)
        {
            string fullName = GetEdmFullName(dataType);
            return SplitFullName(fullName)[1];
        }

        /// <summary>
        /// Gets the Edm namespace of a data type
        /// </summary>
        /// <param name="dataType">The data type.</param>
        /// <returns>The Edm name</returns>
        public static string GetEdmNamespace(DataType dataType)
        {
            string fullName = GetEdmFullName(dataType);
            return SplitFullName(fullName)[0];
        }

        /// <summary>
        /// Returns try if the data type represents a Geometry or any of its subtypes
        /// </summary>
        /// <param name="spatialDataType">The data type to test</param>
        /// <returns>True if it is Geometry, GeometryPoint, GeometryLineString...</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "This is intended for use with SpatialDataTypes only.")]
        public static bool IsGeometricSpatialType(SpatialDataType spatialDataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(spatialDataType, "spatialDataType");
            string dataTypeName = GetSpatialTypeName(spatialDataType);
            return dataTypeName.StartsWith("Geom", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the type name defined. By defualt returns the value of EdmTypeNameFacet. If it is not defined, returns the vlaue of 
        /// SpatialShapeKindFacet or UnqualifiedDataBaseTypeNameFacet. If none is defined, returns an empty string.
        /// </summary>
        /// <param name="spatialDataType">The spatial data type</param>
        /// <returns>the type name defined in the spatial type.</returns>
        public static string GetSpatialTypeName(PrimitiveDataType spatialDataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(spatialDataType, "spatialDataType");
            string dataTypeName = spatialDataType.GetFacetValue<EdmTypeNameFacet, string>(string.Empty);

            if (string.IsNullOrEmpty(dataTypeName))
            {
                var shapeKind = spatialDataType.GetFacetValue<SpatialShapeKindFacet, SpatialShapeKind>(SpatialShapeKind.Unspecified);
                dataTypeName = (shapeKind == SpatialShapeKind.Unspecified)
                    ? spatialDataType.GetFacetValue<UnqualifiedDatabaseTypeNameFacet, string>(string.Empty)
                    : shapeKind.ToString();
            }

            ExceptionUtilities.Assert(
                !string.IsNullOrEmpty(dataTypeName),
                "the data type is not supported as spatial data type.");

            return dataTypeName;
        }

        private static TType WithEdmName<TType>(this TType type, string edmNamespace, string edmTypeName)
            where TType : PrimitiveDataType
        {
            return type.WithFacet(new EdmNamespaceFacet(edmNamespace))
                .WithFacet(new EdmTypeNameFacet(edmTypeName))
                .WithDebuggerDisplay(edmNamespace + "." + edmTypeName);
        }

        private static string[] SplitFullName(string fullName)
        {
            int index = fullName.LastIndexOf(".", StringComparison.Ordinal);
            ExceptionUtilities.Assert(index > 0, "FullName {0} does not contain '.'", fullName);

            string namespaceName = fullName.Substring(0, index);
            string name = fullName.Substring(index + 1, fullName.Length - index - 1);
            return new string[] { namespaceName, name };
        }

        /// <summary>
        /// Visitor to get Edm full Name from a data type
        /// </summary>
        private class EdmFullNameVisitor : IDataTypeVisitor<string>
        {
            /// <summary>
            /// Gets the full Edm name of a data type
            /// </summary>
            /// <param name="dataType">The data type</param>
            /// <returns>The full edm name</returns>
            public string GetEdmFullName(DataType dataType)
            {
                return dataType.Accept(this);
            }

            /// <summary>
            /// Gets the full Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Full Edm name</returns>
            public string Visit(CollectionDataType dataType)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} does not have Edm full name.", dataType));
            }

            /// <summary>
            /// Gets the full Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Full Edm name</returns>
            public string Visit(ComplexDataType dataType)
            {
                return dataType.Definition.FullName;
            }

            /// <summary>
            /// Gets the full Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Full Edm name</returns>
            public string Visit(EntityDataType dataType)
            {
                return dataType.Definition.FullName;
            }

            /// <summary>
            /// Gets the full Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Full Edm name</returns>
            public string Visit(PrimitiveDataType dataType)
            {
                EnumDataType enumDataType = dataType as EnumDataType;
                if (enumDataType != null)
                {
                    return enumDataType.Definition.FullName;
                }

                bool isValid = dataType.HasFacet<EdmNamespaceFacet>() &&
                               dataType.HasFacet<EdmTypeNameFacet>();
                ExceptionUtilities.Assert(isValid, "{0} is not a valid Edm primitive data type. No required facets.", dataType);

                string namespaceName = dataType.GetFacet<EdmNamespaceFacet>().Value;
                string name = dataType.GetFacet<EdmTypeNameFacet>().Value;
                return namespaceName + "." + name;
            }

            /// <summary>
            /// Gets the full Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>Full Edm name</returns>
            public string Visit(ReferenceDataType dataType)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} does not have Edm full name.", dataType));
            }
        }

        /// <summary>
        /// Visitor to get Edm short qualified Name from a data type
        /// </summary>
        private class EdmShortQualifiedNameVisitor : IDataTypeVisitor<string>
        {
            /// <summary>
            /// Gets the short qualified Edm name of a data type
            /// </summary>
            /// <param name="dataType">The data type</param>
            /// <returns>The short qualified edm name</returns>
            public string GetEdmShortQualifiedName(DataType dataType)
            {
                return dataType.Accept(this);
            }

            /// <summary>
            /// Gets the short qualified Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>short qualified Edm name</returns>
            public string Visit(CollectionDataType dataType)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} does not have Edm short qualified name.", dataType));
            }

            /// <summary>
            /// Gets the short qualified Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>short qualified Edm name</returns>
            public string Visit(ComplexDataType dataType)
            {
                return dataType.Definition.Name;
            }

            /// <summary>
            /// Gets the short qualified Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>short qualified Edm name</returns>
            public string Visit(EntityDataType dataType)
            {
                return dataType.Definition.Name;
            }

            /// <summary>
            /// Gets the short qualified Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>short qualified Edm name</returns>
            public string Visit(PrimitiveDataType dataType)
            {
                EnumDataType enumDataType = dataType as EnumDataType;
                if (enumDataType != null)
                {
                    return enumDataType.Definition.Name;
                }

                bool isValid = dataType.HasFacet<EdmNamespaceFacet>() &&
                               dataType.HasFacet<EdmTypeNameFacet>();
                ExceptionUtilities.Assert(isValid, "{0} is not a valid Edm primitive data type. No required facets.", dataType);

                string name = dataType.GetFacet<EdmTypeNameFacet>().Value;
                return name;
            }

            /// <summary>
            /// Gets the short qualified Edm name
            /// </summary>
            /// <param name="dataType">The data type.</param>
            /// <returns>short qualified Edm name</returns>
            public string Visit(ReferenceDataType dataType)
            {
                throw new TaupoInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} does not have Edm short qualified name.", dataType));
            }
        }
    }
}
