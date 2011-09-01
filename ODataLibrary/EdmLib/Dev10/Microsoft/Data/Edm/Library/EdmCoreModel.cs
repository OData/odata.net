//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Provides predefined declarations relevant to EDM semantics.
    /// </summary>
    public class EdmCoreModel : EdmElement, IEdmModel
    {
        private readonly EdmPrimitiveType[] primitiveTypes;
        private const string EdmNamespace = "Edm";

        /// <summary>
        /// The default core EDM model.
        /// </summary>
        public readonly static EdmCoreModel Instance = new EdmCoreModel();
        
        private readonly Dictionary<string, EdmPrimitiveTypeKind> primitiveTypeKinds = new Dictionary<string, EdmPrimitiveTypeKind>();
        private readonly Dictionary<EdmPrimitiveTypeKind, EdmPrimitiveType> primitiveTypesByKind = new Dictionary<EdmPrimitiveTypeKind, EdmPrimitiveType>();
        private readonly Dictionary<string, EdmPrimitiveType> primitiveTypeByName = new Dictionary<string, EdmPrimitiveType>();

        private EdmCoreModel()
        {
            var primitiveDouble = new EdmPrimitiveType(EdmNamespace, "Double", EdmPrimitiveTypeKind.Double);
            var primitiveSingle = new EdmPrimitiveType(EdmNamespace, "Single", EdmPrimitiveTypeKind.Single);

            var primitiveInt64 = new EdmPrimitiveType(EdmNamespace, "Int64", EdmPrimitiveTypeKind.Int64);
            var primitiveInt32 = new EdmPrimitiveType(EdmNamespace, "Int32", EdmPrimitiveTypeKind.Int32);
            var primitiveInt16 = new EdmPrimitiveType(EdmNamespace, "Int16", EdmPrimitiveTypeKind.Int16);
            var primitiveSByte = new EdmPrimitiveType(EdmNamespace, "SByte", EdmPrimitiveTypeKind.SByte);
            var primitiveByte = new EdmPrimitiveType(EdmNamespace, "Byte", EdmPrimitiveTypeKind.Byte);

            var primitiveBoolean = new EdmPrimitiveType(EdmNamespace, "Boolean", EdmPrimitiveTypeKind.Boolean);
            var primitiveGuid = new EdmPrimitiveType(EdmNamespace, "Guid", EdmPrimitiveTypeKind.Guid);

            var primitiveTime = new EdmPrimitiveType(EdmNamespace, "Time", EdmPrimitiveTypeKind.Time);
            var primitiveDateTime = new EdmPrimitiveType(EdmNamespace, "DateTime", EdmPrimitiveTypeKind.DateTime);
            var primitiveDateTimeOffset = new EdmPrimitiveType(EdmNamespace, "DateTimeOffset", EdmPrimitiveTypeKind.DateTimeOffset);

            var primitiveDecimal = new EdmPrimitiveType(EdmNamespace, "Decimal", EdmPrimitiveTypeKind.Decimal);

            var primitiveBinary = new EdmPrimitiveType(EdmNamespace, "Binary", EdmPrimitiveTypeKind.Binary);
            var primitiveString = new EdmPrimitiveType(EdmNamespace, "String", EdmPrimitiveTypeKind.String);
            var primitiveStream = new EdmPrimitiveType(EdmNamespace, "Stream", EdmPrimitiveTypeKind.Stream);

            var primitiveGeography = new EdmPrimitiveType(EdmNamespace, "Geography", EdmPrimitiveTypeKind.Geography);
            var primitivePoint = new EdmPrimitiveType(EdmNamespace, "Point", EdmPrimitiveTypeKind.Point);
            var primitiveLineString = new EdmPrimitiveType(EdmNamespace, "LineString", EdmPrimitiveTypeKind.LineString);
            var primitivePolygon = new EdmPrimitiveType(EdmNamespace, "Polygon", EdmPrimitiveTypeKind.Polygon);
            var primitiveGeographyCollection = new EdmPrimitiveType(EdmNamespace, "GeographyCollection", EdmPrimitiveTypeKind.GeographyCollection);
            var primitiveMultiPolygon = new EdmPrimitiveType(EdmNamespace, "MultiPolygon", EdmPrimitiveTypeKind.MultiPolygon);
            var primitiveMultiLineString = new EdmPrimitiveType(EdmNamespace, "MultiLineString", EdmPrimitiveTypeKind.MultiLineString);
            var primitiveMultiPoint = new EdmPrimitiveType(EdmNamespace, "MultiPoint", EdmPrimitiveTypeKind.MultiPoint);
            var primitiveGeometry = new EdmPrimitiveType(EdmNamespace, "Geometry", EdmPrimitiveTypeKind.Geometry);
            var primitiveGeometricPoint = new EdmPrimitiveType(EdmNamespace, "GeometricPoint", EdmPrimitiveTypeKind.GeometricPoint);
            var primitiveGeometricLineString = new EdmPrimitiveType(EdmNamespace, "GeometricLineString", EdmPrimitiveTypeKind.GeometricLineString);
            var primitiveGeometricPolygon = new EdmPrimitiveType(EdmNamespace, "GeometricPolygon", EdmPrimitiveTypeKind.GeometricPolygon);
            var primitiveGeometryCollection = new EdmPrimitiveType(EdmNamespace, "GeometryCollection", EdmPrimitiveTypeKind.GeometryCollection);
            var primitiveGeometricMultiPolygon = new EdmPrimitiveType(EdmNamespace, "GeometricMultiPolygon", EdmPrimitiveTypeKind.GeometricMultiPolygon);
            var primitiveGeometricMultiLineString = new EdmPrimitiveType(EdmNamespace, "GeometricMultiLineString", EdmPrimitiveTypeKind.GeometricMultiLineString);
            var primitiveGeometricMultiPoint = new EdmPrimitiveType(EdmNamespace, "GeometricMultiPoint", EdmPrimitiveTypeKind.GeometricMultiPoint);

            this.primitiveTypes = new EdmPrimitiveType[]
            {
                primitiveBinary,
                primitiveBoolean,
                primitiveByte,
                primitiveDateTime,
                primitiveDateTimeOffset,
                primitiveDecimal,
                primitiveDouble,
                primitiveGuid,
                primitiveInt16,
                primitiveInt32,
                primitiveInt64,
                primitiveSByte,
                primitiveSingle,
                primitiveStream,
                primitiveString,
                primitiveTime,
                primitiveGeography,
                primitivePoint,
                primitiveLineString,
                primitivePolygon,
                primitiveGeographyCollection,
                primitiveMultiPolygon ,
                primitiveMultiLineString,
                primitiveMultiPoint,
                primitiveGeometry,
                primitiveGeometricPoint,
                primitiveGeometricLineString,
                primitiveGeometricPolygon,
                primitiveGeometryCollection,
                primitiveGeometricMultiPolygon,
                primitiveGeometricMultiLineString,
                primitiveGeometricMultiPoint
            };
            
            foreach (var primitive in this.primitiveTypes)
            {
                this.primitiveTypeKinds[primitive.Name] = primitive.PrimitiveKind;
                this.primitiveTypeKinds[primitive.Namespace + '.' + primitive.Name] = primitive.PrimitiveKind;
                this.primitiveTypesByKind[primitive.PrimitiveKind] = primitive;
                this.primitiveTypeByName[primitive.Namespace + '.' + primitive.Name] = primitive;
            }
        }

        /// <summary>
        /// Gets the types defined in this core model.
        /// </summary>
        public IEnumerable<IEdmSchemaElement> SchemaElements
        {
            get { return this.primitiveTypes; }
        }

        /// <summary>
        /// Searches for a type with the given name in this model and returns null if no such type exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the type being found.</param>
        /// <returns>The requested type, or null if no such type exists.</returns>
        public IEdmSchemaType FindType(string qualifiedName)
        {
            EdmPrimitiveType element;
            return this.primitiveTypeByName.TryGetValue(qualifiedName, out element) ? element : null;
        }

        /// <summary>
        /// Searches for a value term with the given name in this model and returns null if no such value term exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the value term being found.</param>
        /// <returns>The requested value term, or null if no such value term exists.</returns>
        public IEdmValueTerm FindValueTerm(string qualifiedName)
        {
            return null;
        }

        /// <summary>
        /// Searches for an association with the given name in this model and returns null if no such association exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the association being found.</param>
        /// <returns>The requested association, or null if no such association exists.</returns>
        public IEdmAssociation FindAssociation(string qualifiedName)
        {
            return null;
        }

        /// <summary>
        /// Searches for functions with the given name in this model and returns an empty enumerable if no such function exists.
        /// </summary>
        /// <param name="qualifiedName">The qualified name of the function being found.</param>
        /// <returns>A set functions sharing the specified qualified name, or an empty enumerable if no such function exists.</returns>
        public IEnumerable<IEdmFunction> FindFunctions(string qualifiedName)
        {
            return Enumerable.Empty<IEdmFunction>();
        }

        /// <summary>
        /// Gets the collection of entity containers in this model.
        /// </summary>
        public IEnumerable<IEdmEntityContainer> EntityContainers
        {
            get { return Enumerable.Empty<IEdmEntityContainer>(); }
        }

        /// <summary>
        /// Searches for an entity container with the given name in this model and returns null if no such entity container exists.
        /// </summary>
        /// <param name="name">The name of the entity container being found.</param>
        /// <returns>The requested entity container, or null if no such entity container exists.</returns>
        public IEdmEntityContainer FindEntityContainer(string name)
        {
            return null;
        }

        /// <summary>
        /// Gets the namespace of this core model.
        /// </summary>
        public static string Namespace
        {
            get { return "Edm"; }
        }

        /// <summary>
        /// Gets primitive type by kind.
        /// </summary>
        /// <param name="kind">Kind of the primitive type.</param>
        /// <returns>Primitive type definition.</returns>
        public IEdmPrimitiveType GetPrimitiveType(EdmPrimitiveTypeKind kind)
        {
            EdmPrimitiveType definition;
            return this.primitiveTypesByKind.TryGetValue(kind, out definition) ? definition : null;
        }

        /// <summary>
        /// Gets the EdmPrimitiveTypeKind by the type name.
        /// </summary>
        /// <param name="typeName">Name of the type to look up.</param>
        /// <returns>EdmPrimitiveTypeKind of the type.</returns>
        public EdmPrimitiveTypeKind GetPrimitiveTypeKind(string typeName)
        {
            EdmPrimitiveTypeKind kind;
            return this.primitiveTypeKinds.TryGetValue(typeName, out kind) ? kind : EdmPrimitiveTypeKind.None;
        }

        /// <summary>
        /// Gets a reference to a primitive type of the specified kind.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetPrimitive(EdmPrimitiveTypeKind kind, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                    return new EdmPrimitiveTypeReference(this.GetPrimitiveType(kind), isNullable);
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmBinaryTypeReference(this.GetPrimitiveType(kind), isNullable);
                case EdmPrimitiveTypeKind.String:
                    return new EdmStringTypeReference(this.GetPrimitiveType(kind), isNullable);
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalTypeReference(this.GetPrimitiveType(kind), isNullable);
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    return new EdmTemporalTypeReference(this.GetPrimitiveType(kind), isNullable);
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    return new EdmSpatialTypeReference(this.GetPrimitiveType(kind), isNullable);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to the Int16 primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetInt16(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Int32 primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetInt32(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Int64 primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetInt64(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Boolean primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetBoolean(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Byte primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetByte(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), isNullable);
        }

        /// <summary>
        /// Gets a reference to the SByte primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetSByte(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), isNullable);
        }

        /// <summary>
        /// Gets a reference to the Guid primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new primitive type reference.</returns>
        public EdmPrimitiveTypeReference GetGuid(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), isNullable);
        }

        /// <summary>
        /// Gets a reference to a decimal primitive type definition.
        /// </summary>
        /// <param name="precision">Precision of values of this type.</param>
        /// <param name="scale">Scale of values of this type.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public EdmDecimalTypeReference GetDecimal(int? precision, int? scale, bool isNullable)
        {
            return new EdmDecimalTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable, precision, scale);
        }

        /// <summary>
        /// Gets a reference to a decimal primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public EdmDecimalTypeReference GetDecimal(bool isNullable)
        {
            return new EdmDecimalTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), isNullable, null, null);
        }

        /// <summary>
        /// Gets a reference to a single primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public EdmPrimitiveTypeReference GetSingle(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Single), isNullable);
        }

        /// <summary>
        /// Gets a reference to a double primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new decimal type reference.</returns>
        public EdmPrimitiveTypeReference GetDouble(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Double), isNullable);
        }

        /// <summary>
        /// Gets a reference to a stream primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new stream type reference.</returns>
        public EdmPrimitiveTypeReference GetStream(bool isNullable)
        {
            return new EdmPrimitiveTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), isNullable);
        }

        /// <summary>
        /// Gets a reference to a temporal primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="precision">Precision of values of this type.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new temporal type reference.</returns>
        public EdmTemporalTypeReference GetTemporalType(EdmPrimitiveTypeKind kind, int? precision, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    return new EdmTemporalTypeReference(this.GetPrimitiveType(kind), isNullable, precision);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a temporal primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new temporal type reference.</returns>
        public EdmTemporalTypeReference GetTemporalType(EdmPrimitiveTypeKind kind, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    return new EdmTemporalTypeReference(this.GetPrimitiveType(kind), isNullable, null);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a binary primitive type definition.
        /// </summary>
        /// <param name="isMaxMaxLength">Flag specifying if max length is the maximum allowable value.</param>
        /// <param name="maxLength">Maximum length of the type.</param>
        /// <param name="isFixedLength">Flag specifying if the type will have a fixed length.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new binary type reference.</returns>
        public EdmBinaryTypeReference GetBinary(bool isMaxMaxLength, int? maxLength, bool? isFixedLength, bool isNullable)
        {
            return new EdmBinaryTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), isNullable, isMaxMaxLength, maxLength, isFixedLength);
        }

        /// <summary>
        /// Gets a reference to a binary primitive type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new binary type reference.</returns>
        public EdmBinaryTypeReference GetBinary(bool isNullable)
        {
            return new EdmBinaryTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), isNullable);
        }

        /// <summary>
        /// Gets a reference to a spatial primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new spatial type reference.</returns>
        public EdmSpatialTypeReference GetSpatial(EdmPrimitiveTypeKind kind, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    return new EdmSpatialTypeReference(this.GetPrimitiveType(kind), isNullable);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a spatial primitive type definition.
        /// </summary>
        /// <param name="kind">Primitive kind of the type reference being created.</param>
        /// <param name="spatialReferenceIdentifier">Spatial Reference Identifier for the spatial type being created.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new spatial type reference.</returns>
        public EdmSpatialTypeReference GetSpatial(EdmPrimitiveTypeKind kind, int? spatialReferenceIdentifier, bool isNullable)
        {
            switch (kind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Point:
                case EdmPrimitiveTypeKind.LineString:
                case EdmPrimitiveTypeKind.Polygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.MultiPolygon:
                case EdmPrimitiveTypeKind.MultiLineString:
                case EdmPrimitiveTypeKind.MultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometricPoint:
                case EdmPrimitiveTypeKind.GeometricLineString:
                case EdmPrimitiveTypeKind.GeometricPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:
                case EdmPrimitiveTypeKind.GeometricMultiLineString:
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    return new EdmSpatialTypeReference(this.GetPrimitiveType(kind), isNullable, spatialReferenceIdentifier);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// Gets a reference to a string primitive type definition.
        /// </summary>
        /// <param name="isMaxMaxLength">Flag specifying if max length is the maximum allowable value.</param>
        /// <param name="maxLength">Maximum length of the type.</param>
        /// <param name="isFixedLength">Flag specifying if the type will have a fixed length.</param>
        /// <param name="isUnicode">Flag specifying if the type should support unicode encoding.</param>
        /// <param name="collation">String representing how data should be ordered.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new string type reference.</returns>
        public EdmStringTypeReference GetString(bool isMaxMaxLength, int? maxLength, bool? isFixedLength, bool? isUnicode, string collation, bool isNullable)
        {
            return new EdmStringTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.String), isNullable, isMaxMaxLength, maxLength, isFixedLength, isUnicode, collation);
        }

        /// <summary>
        /// Gets a reference to a binary string type definition.
        /// </summary>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A new string type reference.</returns>
        public EdmStringTypeReference GetString(bool isNullable)
        {
            return new EdmStringTypeReference(this.GetPrimitiveType(EdmPrimitiveTypeKind.String), isNullable);
        }

        /// <summary>
        /// Gets a reference to an atomic collection type definition.
        /// </summary>
        /// <param name="elementType">Type of elements in the collection.</param>
        /// <returns>A new atomic collection type reference.</returns>
        static public EdmCollectionTypeReference GetMultiValue(IEdmTypeReference elementType)
        {
            return new EdmCollectionTypeReference(new EdmCollectionType(elementType, EdmConstants.MultiValue_IsAtomic), false);
        }

        /// <summary>
        /// Gets a reference to a non-atomic collection type definition.
        /// </summary>
        /// <param name="elementType">Type of elements in the collection.</param>
        /// <returns>A new non-atomic collection type reference.</returns>
        static public EdmCollectionTypeReference GetCollection(IEdmTypeReference elementType)
        {
            return new EdmCollectionTypeReference(new EdmCollectionType(elementType, EdmConstants.Collection_IsAtomic), false);
        }

        /// <summary>
        /// Searches for vocabulary annotations specified by this model or a referenced model for a given element.
        /// </summary>
        /// <param name="element">The annotated element.</param>
        /// <returns>The vocabulary annotations for the element.</returns>
        public IEnumerable<Annotations.IEdmVocabularyAnnotation> FindVocabularyAnnotations(IEdmAnnotatable element)
        {
            return Enumerable.Empty<Annotations.IEdmVocabularyAnnotation>();
        }
    }
}
