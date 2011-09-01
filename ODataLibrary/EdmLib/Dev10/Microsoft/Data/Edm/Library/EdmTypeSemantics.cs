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
using System.Text;
using Microsoft.Data.Edm.Validation;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Provides semantics of the predefined EDM types.
    /// </summary>
    public static class EdmTypeSemantics
    {
        #region IsCollection, IsEntity, IsComplex, ...

        /// <summary>
        /// Returns true if this reference refers to a collection.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a collection.</returns>
        public static bool IsCollection(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Collection;
        }

        /// <summary>
        /// Returns true if this reference refers to an entity type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an entity type.</returns>
        public static bool IsEntity(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Entity;
        }

        /// <summary>
        /// Returns true if this reference refers to an entity type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an entity type.</returns>
        public static bool IsEntityReference(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.EntityReference;
        }

        /// <summary>
        /// Returns true if this reference refers to a complex type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a complex type.</returns>
        public static bool IsComplex(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Complex;
        }

        /// <summary>
        /// Returns true if this reference refers to an enumerationtype.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an enumerationtype.</returns>
        public static bool IsEnum(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Enum;
        }

        /// <summary>
        /// Returns true if this reference refers to a row type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a row type.</returns>
        public static bool IsRow(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Row;
        }

        /// <summary>
        /// Returns true if this reference refers to a structured type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a structured type.</returns>
        public static bool IsStructured(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.TypeKind())
            {
                case EdmTypeKind.Entity:
                case EdmTypeKind.Complex:
                case EdmTypeKind.Row:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this reference refers to a primitive type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a primitive type.</returns>
        public static bool IsPrimitive(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Primitive;
        }

        /// <summary>
        /// Returns true if this reference refers to a binary type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a binary type.</returns>
        public static bool IsBinary(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Binary;
        }

        /// <summary>
        /// Returns true if this reference refers to a boolean type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a boolean type.</returns>
        public static bool IsBoolean(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Boolean;
        }

        /// <summary>
        /// Returns true if this reference refers to a temporal type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a temporal type.</returns>
        public static bool IsTemporal(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Time:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this reference refers to a DateTime type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a DateTime type.</returns>
        public static bool IsDateTime(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.DateTime;
        }

        /// <summary>
        /// Returns true if this reference refers to a time type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a time type.</returns>
        public static bool IsTime(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Time;
        }

        /// <summary>
        /// Returns true if this reference refers to a DateTimeOffset type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a DateTimeOffset type.</returns>
        public static bool IsDateTimeOffset(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.DateTimeOffset;
        }
        
        /// <summary>
        /// Returns true if this reference refers to a decimal type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a decimal type.</returns>
        public static bool IsDecimal(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Decimal;
        }

        /// <summary>
        /// Returns true if this reference refers to a floating point type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a floating point type.</returns>
        public static bool IsFloating(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Single:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this reference refers to a single type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a single type.</returns>
        public static bool IsSingle(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Single;
        }

        /// <summary>
        /// Returns true if this reference refers to a double type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a double type.</returns>
        public static bool IsDouble(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Double;
        }

        /// <summary>
        /// Returns true if this reference refers to a GUID type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a GUID type.</returns>
        public static bool IsGuid(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Guid;
        }

        /// <summary>
        /// Returns true if this reference refers to a signed integral type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a signed integral type.</returns>
        public static bool IsSignedIntegral(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this reference refers to an SByte type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an SByte type.</returns>
        public static bool IsSByte(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.SByte;
        }

        /// <summary>
        /// Returns true if this reference refers to an Int16 type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an Int16 type.</returns>
        public static bool IsInt16(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int16;
        }

        /// <summary>
        /// Returns true if this reference refers to an Int32 type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an Int32 type.</returns>
        public static bool IsInt32(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int32;
        }

        /// <summary>
        /// Returns true if this reference refers to an Int64 type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to an Int64 type.</returns>
        public static bool IsInt64(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int64;
        }

        /// <summary>
        /// Returns true if this reference refers to a byte type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a byte type.</returns>
        public static bool IsByte(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Byte;
        }

        /// <summary>
        /// Returns true if this reference refers to a string type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a string type.</returns>
        public static bool IsString(this IEdmTypeReference type)
        {
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.String;
        }

        /// <summary>
        /// Returns true if this reference refers to a stream type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a stream type.</returns>
        public static bool IsStream(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Stream;
        }

        /// <summary>
        /// Returns true if this reference refers to a spatial type.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>This reference refers to a spatial type.</returns>
        public static bool IsSpatial(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.PrimitiveKind())
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
                    return true;
            }

            return false;
        }
        
        #endregion

        // The As*** functions never return null -- if the supplied type does not have the appropriate shape, an encoding of a bad type is returned.
        #region AsPrimitive, AsCollection, AsStructured, AsAssociation, ...
        /// <summary>
        /// If this reference is of a primitive type, this will return a valid primitive type reference to the type definition. Otherwise, it will return a bad primitive type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid primitive type reference if the definition of the reference is of a primitive type. Otherwise a bad primitive type reference.</returns>
        public static IEdmPrimitiveTypeReference AsPrimitive(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveTypeReference reference = type as IEdmPrimitiveTypeReference;
            if (reference != null)
            {
                return reference;
            }

            if (type.TypeKind() == EdmTypeKind.Primitive)
            {
                var primitiveDefinition = type.Definition as IEdmPrimitiveType;
                if (primitiveDefinition != null)
                {
                    switch (primitiveDefinition.PrimitiveKind)
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
                            return new EdmPrimitiveTypeReference(primitiveDefinition, type.IsNullable);
                        case EdmPrimitiveTypeKind.Binary:
                            return type.AsBinary();
                        case EdmPrimitiveTypeKind.Decimal:
                            return type.AsDecimal();
                        case EdmPrimitiveTypeKind.String:
                            return type.AsString();
                        case EdmPrimitiveTypeKind.Time:
                        case EdmPrimitiveTypeKind.DateTime:
                        case EdmPrimitiveTypeKind.DateTimeOffset:
                            return type.AsTemporal();
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
                            return type.AsSpatial();
                        case EdmPrimitiveTypeKind.None:
                            break;
                    }
                }
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Primitive);
            return new BadPrimitiveTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of a collection type, this will return a valid collection type reference to the type definition. Otherwise, it will return a bad collection type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid collection type reference if the definition of the reference is of a collection type. Otherwise a bad collection type reference.</returns>
        public static IEdmCollectionTypeReference AsCollection(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmCollectionTypeReference reference = type as IEdmCollectionTypeReference;
            if (reference != null)
            {
                return reference;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Collection);
            return new EdmCollectionTypeReference(
                type.TypeKind() == EdmTypeKind.Collection ? (IEdmCollectionType)type.Definition : new BadCollectionType(EdmConstants.Collection_IsAtomic, errors),
                type.IsNullable);
        }
        
        /// <summary>
        /// If this reference is of a structured type, this will return a valid structured type reference to the type definition. Otherwise, it will return a bad structured type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid structured type reference if the definition of the reference is of a structured type. Otherwise a bad structured type reference.</returns>
        public static IEdmStructuredTypeReference AsStructured(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmStructuredTypeReference reference = type as IEdmStructuredTypeReference;
            if (reference != null)
            {
                return reference;
            }

            switch (type.TypeKind())
            {
                case EdmTypeKind.Entity:
                    return type.AsEntity();
                case EdmTypeKind.Complex:
                    return type.AsComplex();
                case EdmTypeKind.Row:
                    return type.AsRow();
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Structured);
            return new BadEntityTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of an enumeration type, this will return a valid enumeration type reference to the type definition. Otherwise, it will return a bad enumeration type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid enumeration type reference if the definition of the reference is of an enumeration type. Otherwise a bad enumeration type reference.</returns>
        public static IEdmEnumTypeReference AsEnum(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmEnumTypeReference reference = type as IEdmEnumTypeReference;
            if (reference != null)
            {
                return reference;
            }

            return new EdmEnumTypeReference(
                type.TypeKind() == EdmTypeKind.Enum ? (IEdmEnumType)type.Definition : new BadEnumType(type.FullName(), ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Enum)),
                type.IsNullable);
        }

        /// <summary>
        /// If this reference is of an entity type, this will return a valid entity type reference to the type definition. Otherwise, it will return a bad entity type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid entity type reference if the definition of the reference is of an entity type. Otherwise a bad entity type reference.</returns>
        public static IEdmEntityTypeReference AsEntity(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmEntityTypeReference reference = type as IEdmEntityTypeReference;
            if (reference != null)
            {
                return reference;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Entity);
            return type.TypeKind() == EdmTypeKind.Entity ? 
                new EdmEntityTypeReference((IEdmEntityType)type.Definition, type.IsNullable) :
                new BadEntityTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of an entity reference type, this will return a valid entity reference type reference to the type definition. Otherwise, it will return a bad entity reference type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid entity reference type reference if the definition of the reference is of an entity reference type. Otherwise a bad entity reference type reference.</returns>
        public static IEdmEntityReferenceTypeReference AsEntityReference(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmEntityReferenceTypeReference reference = type as IEdmEntityReferenceTypeReference;
            if (reference != null)
            {
                return reference;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_EntityReference);
            return new EdmEntityReferenceTypeReference(
                type.TypeKind() == EdmTypeKind.EntityReference ? (IEdmEntityReferenceType)type.Definition : new BadEntityReferenceType(errors),
                type.IsNullable);
        }

        /// <summary>
        /// If this reference is of a complex type, this will return a valid complex type reference to the type definition. Otherwise, it will return a bad complex type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid complex type reference if the definition of the reference is of a complex type. Otherwise a bad complex type reference.</returns>
        public static IEdmComplexTypeReference AsComplex(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmComplexTypeReference reference = type as IEdmComplexTypeReference;
            if (reference != null)
            {
                return reference;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Complex);
            return type.TypeKind() == EdmTypeKind.Complex ? 
                new EdmComplexTypeReference((IEdmComplexType)type.Definition, type.IsNullable) : 
                new BadComplexTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of a row type, this will return a valid row type reference to the type definition. Otherwise, it will return a bad row type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid row type reference if the definition of the reference is of a row type. Otherwise a bad row type reference.</returns>
        public static IEdmRowTypeReference AsRow(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmRowTypeReference reference = type as IEdmRowTypeReference;
            if (reference != null)
            {
                return reference;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Row);
            return new EdmRowTypeReference(
                type.TypeKind() == EdmTypeKind.Row ? (IEdmRowType)type.Definition : new BadRowType(errors),
                type.IsNullable);
        }

        /// <summary>
        /// If this reference is of a spatial type, this will return a valid spatial type reference to the type definition. Otherwise, it will return a bad spatial type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid spatial type reference if the definition of the reference is of a spatial type. Otherwise a bad spatial type reference.</returns>
        public static IEdmSpatialTypeReference AsSpatial(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmSpatialTypeReference spatial = type as IEdmSpatialTypeReference;
            if (spatial != null)
            {
                return spatial;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Spatial);
            return new BadSpatialTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of a temporal type, this will return a valid temporal type reference to the type definition. Otherwise, it will return a bad temporal type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid temporal type reference if the definition of the reference is of a temporal type. Otherwise a bad temporal type reference.</returns>
        public static IEdmTemporalTypeReference AsTemporal(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmTemporalTypeReference temporal = type as IEdmTemporalTypeReference;
            if (temporal != null)
            {
                return temporal;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Temporal);
            return new BadTemporalTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of a decimal type, this will return a valid decimal type reference to the type definition. Otherwise, it will return a bad decimal type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid decimal type reference if the definition of the reference is of a decimal type. Otherwise a bad decimal type reference.</returns>
        public static IEdmDecimalTypeReference AsDecimal(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmDecimalTypeReference decimalType = type as IEdmDecimalTypeReference;
            if (decimalType != null)
            {
                return decimalType;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Decimal);
            return new BadDecimalTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of a string type, this will return a valid string type reference to the type definition. Otherwise, it will return a bad string type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid string type reference if the definition of the reference is of a string type. Otherwise a bad string type reference.</returns>
        public static IEdmStringTypeReference AsString(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmStringTypeReference stringType = type as IEdmStringTypeReference;
            if (stringType != null)
            {
                return stringType;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_String);
            return new BadStringTypeReference(type.FullName(), type.IsNullable, errors);
        }

        /// <summary>
        /// If this reference is of a binary type, this will return a valid binary type reference to the type definition. Otherwise, it will return a bad binary type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid binary type reference if the definition of the reference is of a binary type. Otherwise a bad binary type reference.</returns>
        public static IEdmBinaryTypeReference AsBinary(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmBinaryTypeReference binaryType = type as IEdmBinaryTypeReference;
            if (binaryType != null)
            {
                return binaryType;
            }

            IEnumerable<EdmError> errors = (type.IsBad()) ? type.Errors() : ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Binary);
            return new BadBinaryTypeReference(type.FullName(), type.IsNullable, errors);
        }

        private static IEnumerable<EdmError> ConversionError(EdmLocation location, string typeName, string typeKindName)
        {
            return new[]{new EdmError(location, EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, Edm.Strings.TypeSemantics_CouldNotConvertTypeReference(typeName ?? EdmConstants.Value_UnnamedType, typeKindName))};
        }
        #endregion

        #region IsEquivalentTo

        /// <summary>
        /// Returns true if the compared type is semantically equivalent to this type.
        /// </summary>
        /// <param name="thisType">Reference to the calling object.</param>
        /// <param name="otherType">Type being compared to.</param>
        /// <returns>Equivalence of the two types.</returns>
        public static bool IsEquivalentTo(this IEdmType thisType, IEdmType otherType)
        {
            if (thisType == otherType)
            {
                return true;
            }

            if (thisType == null || otherType == null)
            {
                return false;
            }

            if (thisType.TypeKind != otherType.TypeKind)
            {
                return false;
            }

            switch(thisType.TypeKind)
            {
                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    return ((IEdmSchemaType)thisType).IsEquivalentTo(((IEdmSchemaType)otherType));
                case EdmTypeKind.Collection:
                    return ((IEdmCollectionType)thisType).IsEquivalentTo(((IEdmCollectionType)otherType));
                case EdmTypeKind.EntityReference:
                    return ((IEdmEntityReferenceType)thisType).IsEquivalentTo(((IEdmEntityReferenceType)otherType));
                case EdmTypeKind.Primitive:
                    return ((IEdmPrimitiveType)thisType).IsEquivalentTo(((IEdmPrimitiveType)otherType));
                case EdmTypeKind.Row:
                    return ((IEdmRowType)thisType).IsEquivalentTo(((IEdmRowType)otherType));
                default:
                    return false;
            }
        }

        private static bool IsEquivalentTo(this IEdmSchemaType thisType, IEdmSchemaType otherType)
        {
            return thisType.FullName() == otherType.FullName();
        }

        private static bool IsEquivalentTo(this IEdmCollectionType thisType, IEdmCollectionType otherType)
        {
            return thisType.ElementType.IsEquivalentTo(otherType.ElementType);
        }

        private static bool IsEquivalentTo(this IEdmEntityReferenceType thisType, IEdmEntityReferenceType otherType)
        {
            return thisType.EntityType.IsEquivalentTo(otherType.EntityType);
        }

        private static bool IsEquivalentTo(this IEdmRowType thisType, IEdmRowType otherType)
        {
            if (thisType.DeclaredProperties.Count() != otherType.DeclaredProperties.Count())
            {
                return false;
            }

            IEnumerator<IEdmProperty> thisTypePropertyEnumerator = thisType.DeclaredProperties.GetEnumerator();
            foreach (IEdmProperty otherTypeProperty in otherType.DeclaredProperties)
            {
                thisTypePropertyEnumerator.MoveNext();
                if (!(thisTypePropertyEnumerator.Current.Name.EqualsOrdinal(otherTypeProperty.Name)) || !thisTypePropertyEnumerator.Current.Type.IsEquivalentTo(otherTypeProperty.Type))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsEquivalentTo(this IEdmPrimitiveType thisType, IEdmPrimitiveType otherType)
        {
            EdmUtil.CheckArgumentNull(thisType, "thisType");
            return thisType.PrimitiveKind == otherType.PrimitiveKind;
        }

        /// <summary>
        /// Returns true if the compared type is semantically equivalent to this type.
        /// </summary>
        /// <param name="thisType">Reference to the calling object.</param>
        /// <param name="otherType">Type being compared to.</param>
        /// <returns>Equivalence of the two types.</returns>
        public static bool IsEquivalentTo(this IEdmTypeReference thisType, IEdmTypeReference otherType)
        {
            if (thisType == otherType)
            {
                return true;
            }

            if (thisType == null || otherType == null)
            {
                return false;
            }

            if (thisType.TypeKind() != otherType.TypeKind())
            {
                return false;
            }

            IEdmPrimitiveTypeReference primitiveType = thisType as IEdmPrimitiveTypeReference;
            if (primitiveType != null)
            {
                return primitiveType.IsEquivalentTo(otherType as IEdmPrimitiveTypeReference);
            }

            return thisType.IsNullable == otherType.IsNullable &&
                   thisType.Definition.IsEquivalentTo(otherType.Definition);
        }

        private static bool IsEquivalentTo(this IEdmPrimitiveTypeReference thisType, IEdmPrimitiveTypeReference otherType)
        {
            if (thisType.PrimitiveKind() != otherType.PrimitiveKind())
            {
                return false;
            }

            switch (thisType.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    return ((IEdmBinaryTypeReference)thisType).IsEquivalentTo((IEdmBinaryTypeReference)otherType);
                case EdmPrimitiveTypeKind.Decimal:
                    return ((IEdmDecimalTypeReference)thisType).IsEquivalentTo((IEdmDecimalTypeReference)otherType);
                case EdmPrimitiveTypeKind.String:
                    return ((IEdmStringTypeReference)thisType).IsEquivalentTo((IEdmStringTypeReference)otherType);
                case EdmPrimitiveTypeKind.Time:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return ((IEdmTemporalTypeReference)thisType).IsEquivalentTo((IEdmTemporalTypeReference)otherType);
            }

            return thisType.IsNullable == otherType.IsNullable &&
                thisType.Definition.IsEquivalentTo(otherType.Definition);
        }

        private static bool IsEquivalentTo(this IEdmBinaryTypeReference thisType, IEdmBinaryTypeReference otherType)
        {
            return thisType.IsNullable == otherType.IsNullable &&
                thisType.IsFixedLength == otherType.IsFixedLength &&
                thisType.IsMaxMaxLength == otherType.IsMaxMaxLength &&
                thisType.MaxLength == otherType.MaxLength;
        }

        private static bool IsEquivalentTo(this IEdmDecimalTypeReference thisType, IEdmDecimalTypeReference otherType)
        {
            return thisType.IsNullable == otherType.IsNullable &&
                thisType.Precision == otherType.Precision &&
                thisType.Scale == otherType.Scale;
        }

        private static bool IsEquivalentTo(this IEdmTemporalTypeReference thisType, IEdmTemporalTypeReference otherType)
        {
            return thisType.TypeKind() == otherType.TypeKind() &&
                thisType.IsNullable == otherType.IsNullable &&
                thisType.Precision == otherType.Precision;
        }

        private static bool IsEquivalentTo(this IEdmStringTypeReference thisType, IEdmStringTypeReference otherType)
        {
            return thisType.IsNullable == otherType.IsNullable &&
                thisType.IsFixedLength == otherType.IsFixedLength &&
                thisType.IsMaxMaxLength == otherType.IsMaxMaxLength &&
                thisType.MaxLength == otherType.MaxLength &&
                thisType.IsUnicode == otherType.IsUnicode &&
                thisType.Collation == otherType.Collation;
        }

        /// <summary>
        /// Returns true if the compared function is semantically equivalent to this function.
        /// </summary>
        /// <param name="thisFunction">Reference to the calling object.</param>
        /// <param name="otherFunction">Function being compared to.</param>
        /// <returns>Equivalence of the two functions.</returns>
        public static bool IsEquivalentTo(this IEdmFunction thisFunction, IEdmFunction otherFunction)
        {
            EdmUtil.CheckArgumentNull(thisFunction, "thisFunction");
            EdmUtil.CheckArgumentNull(otherFunction, "otherFunction");
            if (thisFunction == otherFunction)
            {
                return true;
            }

            return thisFunction.Namespace == otherFunction.Namespace && ((IEdmFunctionBase)thisFunction).IsEquivalentTo((IEdmFunctionBase)otherFunction);
        }

        /// <summary>
        /// Returns true if the compared function import is semantically equivalent to this function import.
        /// </summary>
        /// <param name="thisFunction">Reference to the calling object.</param>
        /// <param name="otherFunction">Function import being compared to.</param>
        /// <returns>Equivalence of the two function imports.</returns>
        public static bool IsEquivalentTo(this IEdmFunctionImport thisFunction, IEdmFunctionImport otherFunction)
        {
            if (thisFunction == otherFunction)
            {
                return true;
            }

            EdmUtil.CheckArgumentNull(thisFunction, "thisFunction");
            EdmUtil.CheckArgumentNull(otherFunction, "otherFunction");

            return (thisFunction.EntitySet.IsEquivalentTo(otherFunction.EntitySet)) && ((IEdmFunctionBase)thisFunction).IsEquivalentTo((IEdmFunctionBase)otherFunction);
        }

        private static bool IsEquivalentTo(this IEdmFunctionBase thisFunction, IEdmFunctionBase otherFunction)
        {
            if (thisFunction.Name != otherFunction.Name)
            {
                return false;
            }

            if (!thisFunction.ReturnType.IsEquivalentTo(otherFunction.ReturnType))
            {
                return false;
            }

            IEnumerator<IEdmFunctionParameter> otherFunctionParameterEnumerator = otherFunction.Parameters.GetEnumerator();
            foreach (IEdmFunctionParameter parameter in thisFunction.Parameters)
            {
                otherFunctionParameterEnumerator.MoveNext();
                if (!parameter.IsEquivalentTo(otherFunctionParameterEnumerator.Current))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsEquivalentTo(this IEdmFunctionParameter thisParameter, IEdmFunctionParameter otherParameter)
        {
            if (thisParameter == otherParameter)
            {
                return true;
            }

            return thisParameter.Name == otherParameter.Name && thisParameter.Type.IsEquivalentTo(otherParameter.Type);
        }

        private static bool IsEquivalentTo(this IEdmEntitySet thisEntitySet, IEdmEntitySet otherEntitySet)
        {
            if (thisEntitySet == otherEntitySet)
            {
                return true;
            }

            return thisEntitySet.Name == otherEntitySet.Name && thisEntitySet.ElementType.IsEquivalentTo(otherEntitySet.ElementType);
        }

        #endregion

        #region ToTraceString

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.TypeKind)
            {
                case EdmTypeKind.Collection:
                    return ((IEdmCollectionType)type).ToTraceString();
                case EdmTypeKind.EntityReference:
                    return ((IEdmEntityReferenceType)type).ToTraceString();
                case EdmTypeKind.Row:
                    return ((IEdmRowType)type).ToTraceString();
                default:
                    var schemaType = type as IEdmSchemaType;
                    return schemaType != null ? schemaType.ToTraceString() : EdmConstants.Value_UnknownType;
            }
        }

        private static string ToTraceString(this IEdmSchemaType type)
        {
            return type.FullName();
        }

        private static string ToTraceString(this IEdmEntityReferenceType type)
        {
            return EdmTypeKind.EntityReference.ToString() + '(' + type.EntityType + ')';
        }

        private static string ToTraceString(this IEdmCollectionType type)
        {
            return EdmTypeKind.Collection.ToString() + '(' + type.ElementType.ToString() + ')';
        }

        private static string ToTraceString(this IEdmRowType type)
        {
            StringBuilder sb = new StringBuilder(EdmTypeKind.Row.ToString());
            sb.Append('(');
            IEdmProperty lastProperty = type.Properties().Last();
            foreach (IEdmProperty prop in type.Properties())
            {
                sb.Append(prop.Name);
                sb.Append('=');
                sb.Append(prop.Type.ToString());
                if (!prop.Equals(lastProperty))
                {
                    sb.Append(", ");
                }
            }

            sb.Append(')');
            return sb.ToString();
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The text representation of the current object.</returns>
        public static string ToTraceString(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(type.Definition.ToString());
            sb.AppendKeyValue(EdmConstants.FacetName_Nullable, type.IsNullable.ToString());
            if (type.IsPrimitive())
            {
                sb.AppendFacets(type.AsPrimitive());
            }

            sb.Append(']');
            return sb.ToString();
        }

        private static void AppendFacets(this StringBuilder sb, IEdmPrimitiveTypeReference type)
        {
            switch(type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary:
                    sb.AppendBinaryFacets(type.AsBinary());
                    break;
                case EdmPrimitiveTypeKind.Decimal:
                    sb.AppendDecimalFacets(type.AsDecimal());
                    break;
                case EdmPrimitiveTypeKind.String:
                    sb.AppendStringFacets(type.AsString());
                    break;
                case EdmPrimitiveTypeKind.Time:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    sb.AppendTemporalFacets(type.AsTemporal());
                    break;
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
                    sb.AppendSpatialFacets(type.AsSpatial());
                    break;
            }
        }

        private static void AppendBinaryFacets(this StringBuilder sb, IEdmBinaryTypeReference type)
        {
            sb.AppendKeyValue(EdmConstants.FacetName_FixedLength, type.IsFixedLength.ToString());
            if (type.IsMaxMaxLength || type.MaxLength != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_MaxLength, (type.IsMaxMaxLength) ? EdmConstants.Value_Max : type.MaxLength.ToString());
            }
        }

        private static void AppendStringFacets(this StringBuilder sb, IEdmStringTypeReference type)
        {
            sb.AppendKeyValue(EdmConstants.FacetName_FixedLength, type.IsFixedLength.ToString());
            if (type.IsMaxMaxLength == true || type.MaxLength != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_MaxLength, (type.IsMaxMaxLength) ? EdmConstants.Value_Max : type.MaxLength.ToString());
            }

            sb.AppendKeyValue(EdmConstants.FacetName_Unicode, type.IsUnicode.ToString());
            if (type.Collation != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Collation, type.Collation.ToString());
            }
        }

        private static void AppendTemporalFacets(this StringBuilder sb, IEdmTemporalTypeReference type)
        {
            if (type.Precision != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Precision, type.Precision.ToString());
            }
        }

        private static void AppendDecimalFacets(this StringBuilder sb, IEdmDecimalTypeReference type)
        {
            if (type.Precision != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Precision, type.Precision.ToString());
            }

            if (type.Scale != null)
            {
                sb.AppendKeyValue(EdmConstants.FacetName_Scale, type.Scale.ToString());
            }
        }

        private static void AppendSpatialFacets(this StringBuilder sb, IEdmSpatialTypeReference type)
        {
            sb.AppendKeyValue(EdmConstants.FacetName_Srid, type.SpatialReferenceIdentifier != null ? type.SpatialReferenceIdentifier.ToString() : EdmConstants.Value_SridVariable);
        }

        private static void AppendKeyValue(this StringBuilder sb, string key, string value)
        {
            sb.Append(' ');
            sb.Append(key);
            sb.Append('=');
            sb.Append(value);
        }

        #endregion

        /// <summary>
        /// Returns the primitive kind of the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The primitive kind of the definition of this reference.</returns>
        public static EdmPrimitiveTypeKind PrimitiveKind(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Primitive ? type.AsPrimitive().PrimitiveDefinition().PrimitiveKind : EdmPrimitiveTypeKind.None;
        }

        /// <summary>
        /// Returns a reference to this row type definition.
        /// </summary>
        /// <param name="rowType">Reference to the calling object.</param>
        /// <param name="isNullable">Flag specifying if the referenced type should be nullable.</param>
        /// <returns>A reference to this row type definition.</returns>
        public static IEdmRowTypeReference ApplyType(this IEdmRowType rowType, bool isNullable)
        {
            EdmUtil.CheckArgumentNull(rowType, "type");
            return new EdmRowTypeReference(rowType, isNullable);
        }

        /// <summary>
        /// Determines if the potential base type is in the inheritance hierarchy of the type being tested.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <param name="potentialBaseType">The potential base type of the type being tested.</param>
        /// <returns>A value indicating whether the type inherits from the potential base type.</returns>
        public static bool InheritsFrom(this IEdmStructuredType type, IEdmStructuredType potentialBaseType)
        {
            while (type != null)
            {
                if(type == potentialBaseType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
