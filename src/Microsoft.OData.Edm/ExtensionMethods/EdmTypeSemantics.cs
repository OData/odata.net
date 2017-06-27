//---------------------------------------------------------------------
// <copyright file="EdmTypeSemantics.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Validation;

namespace Microsoft.OData.Edm
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
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a collection.</returns>
        public static bool IsCollection(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Collection;
        }

        /// <summary>
        /// Returns true if this reference refers to an entity type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an entity type.</returns>
        public static bool IsEntity(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Entity;
        }

        /// <summary>
        /// Returns true if this reference refers to an entity type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an entity type.</returns>
        public static bool IsEntityReference(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.EntityReference;
        }

        /// <summary>
        /// Returns true if this reference refers to a complex type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a complex type.</returns>
        public static bool IsComplex(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Complex;
        }

        /// <summary>
        /// Returns true if this reference refers to a Edm.Untyped type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a Edm.Untyped type.</returns>
        public static bool IsUntyped(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Untyped;
        }

        /// <summary>
        /// Returns true if this reference refers to an enumeration type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an enumeration type.</returns>
        public static bool IsEnum(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Enum;
        }

        /// <summary>
        /// Returns true if this reference refers to a type definition.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a type definition.</returns>
        public static bool IsTypeDefinition(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.TypeDefinition;
        }

        /// <summary>
        /// Returns true if this reference refers to a structured type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a structured type.</returns>
        public static bool IsStructured(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.TypeKind())
            {
                case EdmTypeKind.Entity:
                case EdmTypeKind.Complex:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this type kind represents a structured type.
        /// </summary>
        /// <param name="typeKind">Reference to the calling object.</param>
        /// <returns>This kind refers to a structured type.</returns>
        public static bool IsStructured(this EdmTypeKind typeKind)
        {
            switch (typeKind)
            {
                case EdmTypeKind.Entity:
                case EdmTypeKind.Complex:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this reference refers to a primitive type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a primitive type.</returns>
        public static bool IsPrimitive(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.TypeKind() == EdmTypeKind.Primitive;
        }

        /// <summary>
        /// Returns true if this reference refers to a binary type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a binary type.</returns>
        public static bool IsBinary(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Binary;
        }

        /// <summary>
        /// Returns true if this reference refers to a boolean type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a boolean type.</returns>
        public static bool IsBoolean(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Boolean;
        }

        /// <summary>
        /// Returns true if this reference refers to a temporal type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a temporal type.</returns>
        public static bool IsTemporal(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.IsTemporal();
        }

        /// <summary>
        /// Returns true if this definition refers to a temporal type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This definition refers to a temporal type.</returns>
        public static bool IsTemporal(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind.IsTemporal();
        }

        /// <summary>
        /// Returns true if this type kind represents a temporal type.
        /// </summary>
        /// <param name="typeKind">Reference to the calling object.</param>
        /// <returns>This kind refers to a temporal type.</returns>
        public static bool IsTemporal(this EdmPrimitiveTypeKind typeKind)
        {
            switch (typeKind)
            {
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this reference refers to a duration type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a duration type.</returns>
        public static bool IsDuration(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Duration;
        }

        /// <summary>
        /// Returns true if this reference refers to a Date type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a Date type.</returns>
        public static bool IsDate(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Date;
        }

        /// <summary>
        /// Returns true if this reference refers to a DateTimeOffset type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a DateTimeOffset type.</returns>
        public static bool IsDateTimeOffset(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.DateTimeOffset;
        }

        /// <summary>
        /// Returns true if this reference refers to a decimal type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a decimal type.</returns>
        public static bool IsDecimal(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.IsDecimal();
        }

        /// <summary>
        /// Returns true if this definition refers to a decimal type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This definition refers to a decimal type.</returns>
        public static bool IsDecimal(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Decimal;
        }

        /// <summary>
        /// Returns true if this reference refers to a floating point type.
        /// </summary>
        /// <param name="type">Type reference.</param>
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
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a single type.</returns>
        public static bool IsSingle(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Single;
        }

        /// <summary>
        /// Returns true if this reference refers to a TimeOfDay type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a TimeOfDay type.</returns>
        public static bool IsTimeOfDay(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.TimeOfDay;
        }

        /// <summary>
        /// Returns true if this reference refers to a double type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a double type.</returns>
        public static bool IsDouble(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Double;
        }

        /// <summary>
        /// Returns true if this reference refers to a GUID type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a GUID type.</returns>
        public static bool IsGuid(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Guid;
        }

        /// <summary>
        /// Returns true if this reference refers to a signed integral type.
        /// </summary>
        /// <param name="type">Type reference.</param>
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
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an SByte type.</returns>
        public static bool IsSByte(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.SByte;
        }

        /// <summary>
        /// Returns true if this reference refers to an Int16 type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an Int16 type.</returns>
        public static bool IsInt16(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int16;
        }

        /// <summary>
        /// Returns true if this reference refers to an Int32 type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an Int32 type.</returns>
        public static bool IsInt32(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int32;
        }

        /// <summary>
        /// Returns true if this reference refers to an Int64 type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an Int64 type.</returns>
        public static bool IsInt64(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Int64;
        }

        /// <summary>
        /// Returns true if this reference refers to an integer type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to an integer type.</returns>
        public static bool IsIntegral(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.SByte:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if this primitive type kind represents an integer type.
        /// </summary>
        /// <param name="primitiveTypeKind">Type reference.</param>
        /// <returns>This kind refers to an integer type.</returns>
        public static bool IsIntegral(this EdmPrimitiveTypeKind primitiveTypeKind)
        {
            switch (primitiveTypeKind)
            {
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.SByte:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if this reference refers to a byte type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a byte type.</returns>
        public static bool IsByte(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.PrimitiveKind() == EdmPrimitiveTypeKind.Byte;
        }

        /// <summary>
        /// Returns true if this reference refers to a string type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a string type.</returns>
        public static bool IsString(this IEdmTypeReference type)
        {
            return type.Definition.IsString();
        }

        /// <summary>
        /// Returns true if this definition refers to a string type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This definition refers to a string type.</returns>
        public static bool IsString(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.String;
        }

        /// <summary>
        /// Returns true if this reference refers to a stream type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a stream type.</returns>
        public static bool IsStream(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.IsStream();
        }

        /// <summary>
        /// Returns true if this reference refers to a stream type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a stream type.</returns>
        public static bool IsStream(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream;
        }

        /// <summary>
        /// Returns true if this reference refers to a spatial type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a spatial type.</returns>
        public static bool IsSpatial(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.IsSpatial();
        }

        /// <summary>
        /// Returns true if this definition refers to a spatial type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This definition refers to a spatial type.</returns>
        public static bool IsSpatial(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind.IsSpatial();
        }

        /// <summary>
        /// Returns true if this type kind represents a spatial type.
        /// </summary>
        /// <param name="typeKind">Type reference.</param>
        /// <returns>This kind refers to a spatial type.</returns>
        public static bool IsSpatial(this EdmPrimitiveTypeKind typeKind)
        {
            switch (typeKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this definition refers to a geography type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This definition refers to a geography type.</returns>
        public static bool IsGeography(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind.IsGeography();
        }

        /// <summary>
        /// Returns true if this reference refers to a geography type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a geography type.</returns>
        public static bool IsGeography(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.IsGeography();
        }

        /// <summary>
        /// Returns true if this type kind represents a geography type.
        /// </summary>
        /// <param name="typeKind">Type reference.</param>
        /// <returns>This kind refers to a geography type.</returns>
        public static bool IsGeography(this EdmPrimitiveTypeKind typeKind)
        {
            switch (typeKind)
            {
            case EdmPrimitiveTypeKind.Geography:
            case EdmPrimitiveTypeKind.GeographyPoint:
            case EdmPrimitiveTypeKind.GeographyLineString:
            case EdmPrimitiveTypeKind.GeographyPolygon:
            case EdmPrimitiveTypeKind.GeographyCollection:
            case EdmPrimitiveTypeKind.GeographyMultiPolygon:
            case EdmPrimitiveTypeKind.GeographyMultiLineString:
            case EdmPrimitiveTypeKind.GeographyMultiPoint:
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if this definition refers to a geometry type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This definition refers to a geometry type.</returns>
        public static bool IsGeometry(this IEdmType type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                return false;
            }

            return primitiveType.PrimitiveKind.IsGeometry();
        }

        /// <summary>
        /// Returns true if this reference refers to a geometry type.
        /// </summary>
        /// <param name="type">Type reference.</param>
        /// <returns>This reference refers to a geometry type.</returns>
        public static bool IsGeometry(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            return type.Definition.IsGeometry();
        }

        /// <summary>
        /// Returns true if this type kind represents a geometry type.
        /// </summary>
        /// <param name="typeKind">Type reference.</param>
        /// <returns>This kind refers to a geometry type.</returns>
        public static bool IsGeometry(this EdmPrimitiveTypeKind typeKind)
        {
            switch (typeKind)
            {
            case EdmPrimitiveTypeKind.Geometry:
            case EdmPrimitiveTypeKind.GeometryPoint:
            case EdmPrimitiveTypeKind.GeometryLineString:
            case EdmPrimitiveTypeKind.GeometryPolygon:
            case EdmPrimitiveTypeKind.GeometryCollection:
            case EdmPrimitiveTypeKind.GeometryMultiPolygon:
            case EdmPrimitiveTypeKind.GeometryMultiLineString:
            case EdmPrimitiveTypeKind.GeometryMultiPoint:
                return true;
            }

            return false;
        }

        #endregion

        // The As*** functions never return null -- if the supplied type does not have the appropriate shape, an encoding of a bad type is returned.
        #region AsPrimitive, AsCollection, AsStructured, ...
        /// <summary>
        /// If this reference is of a primitive type, this will return a valid primitive type reference to the type definition. Otherwise, it will return a bad primitive type reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid primitive type reference if the definition of the reference is of a primitive type. Otherwise a bad primitive type reference.</returns>
        public static IEdmPrimitiveTypeReference AsPrimitive(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmPrimitiveTypeReference primitiveReference = type as IEdmPrimitiveTypeReference;
            if (primitiveReference != null)
            {
                return primitiveReference;
            }

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.Primitive)
            {
                var primitiveDefinition = typeDefinition as IEdmPrimitiveType;
                if (primitiveDefinition != null)
                {
                    switch (primitiveDefinition.PrimitiveKind)
                    {
                        case EdmPrimitiveTypeKind.Boolean:
                        case EdmPrimitiveTypeKind.Byte:
                        case EdmPrimitiveTypeKind.Date:
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
                        case EdmPrimitiveTypeKind.Duration:
                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.TimeOfDay:
                            return type.AsTemporal();
                        case EdmPrimitiveTypeKind.Geography:
                        case EdmPrimitiveTypeKind.GeographyPoint:
                        case EdmPrimitiveTypeKind.GeographyLineString:
                        case EdmPrimitiveTypeKind.GeographyPolygon:
                        case EdmPrimitiveTypeKind.GeographyCollection:
                        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                        case EdmPrimitiveTypeKind.GeographyMultiLineString:
                        case EdmPrimitiveTypeKind.GeographyMultiPoint:
                        case EdmPrimitiveTypeKind.Geometry:
                        case EdmPrimitiveTypeKind.GeometryPoint:
                        case EdmPrimitiveTypeKind.GeometryLineString:
                        case EdmPrimitiveTypeKind.GeometryPolygon:
                        case EdmPrimitiveTypeKind.GeometryCollection:
                        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiLineString:
                        case EdmPrimitiveTypeKind.GeometryMultiPoint:
                            return type.AsSpatial();
                        case EdmPrimitiveTypeKind.None:
                            break;
                    }
                }
            }
            else if (typeDefinition.TypeKind == EdmTypeKind.TypeDefinition)
            {
                IEdmPrimitiveType underlyingType = typeDefinition.UnderlyingType();
                IEdmTypeDefinitionReference reference = type as IEdmTypeDefinitionReference;
                if (reference == null)
                {
                    // No facet available if not IEdmTypeDefinitionReference.
                    return new EdmPrimitiveTypeReference(underlyingType, type.IsNullable);
                }

                switch (underlyingType.PrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Binary:
                        return new EdmBinaryTypeReference(underlyingType, reference.IsNullable, reference.IsUnbounded, reference.MaxLength);

                    case EdmPrimitiveTypeKind.Decimal:
                        return new EdmDecimalTypeReference(underlyingType, reference.IsNullable, reference.Precision, reference.Scale);

                    case EdmPrimitiveTypeKind.String:
                        return new EdmStringTypeReference(underlyingType, reference.IsNullable, reference.IsUnbounded, reference.MaxLength, reference.IsUnicode);

                    case EdmPrimitiveTypeKind.Duration:
                    case EdmPrimitiveTypeKind.DateTimeOffset:
                    case EdmPrimitiveTypeKind.TimeOfDay:
                        return new EdmTemporalTypeReference(underlyingType, reference.IsNullable, reference.Precision);

                    case EdmPrimitiveTypeKind.Geography:
                    case EdmPrimitiveTypeKind.GeographyPoint:
                    case EdmPrimitiveTypeKind.GeographyLineString:
                    case EdmPrimitiveTypeKind.GeographyPolygon:
                    case EdmPrimitiveTypeKind.GeographyCollection:
                    case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    case EdmPrimitiveTypeKind.Geometry:
                    case EdmPrimitiveTypeKind.GeometryPoint:
                    case EdmPrimitiveTypeKind.GeometryLineString:
                    case EdmPrimitiveTypeKind.GeometryPolygon:
                    case EdmPrimitiveTypeKind.GeometryCollection:
                    case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    case EdmPrimitiveTypeKind.GeometryMultiPoint:
                        return new EdmSpatialTypeReference(underlyingType, reference.IsNullable, reference.SpatialReferenceIdentifier);

                   default:
                        return new EdmPrimitiveTypeReference(underlyingType, reference.IsNullable);
                }
            }

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Primitive));
            }

            return new BadPrimitiveTypeReference(typeFullName, type.IsNullable, errors);
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

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.Collection)
            {
                return new EdmCollectionTypeReference((IEdmCollectionType)typeDefinition);
            }

            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), type.FullName(), EdmConstants.Type_Collection));
            }

            return new EdmCollectionTypeReference(new BadCollectionType(errors));
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
            }

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.TypeErrors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Structured));
            }

            return new BadEntityTypeReference(typeFullName, type.IsNullable, errors);
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

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.Enum)
            {
                return new EdmEnumTypeReference((IEdmEnumType)typeDefinition, type.IsNullable);
            }

            string typeFullName = type.FullName();
            return new EdmEnumTypeReference(
                new BadEnumType(typeFullName, ConversionError(type.Location(), typeFullName, EdmConstants.Type_Enum)),
                type.IsNullable);
        }


        /// <summary>
        /// If this reference is of a type definition, this will return a valid type definition reference to the type definition. Otherwise, it will return a bad type definition reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>A valid type definition reference if the definition of the reference is of a type definition. Otherwise a bad type definition reference.</returns>
        public static IEdmTypeDefinitionReference AsTypeDefinition(this IEdmTypeReference type)
        {
            EdmUtil.CheckArgumentNull(type, "type");
            IEdmTypeDefinitionReference reference = type as IEdmTypeDefinitionReference;
            if (reference != null)
            {
                return reference;
            }

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.TypeDefinition)
            {
                return new EdmTypeDefinitionReference((IEdmTypeDefinition)typeDefinition, type.IsNullable);
            }

            string typeFullName = type.FullName();
            return new EdmTypeDefinitionReference(
                new BadTypeDefinition(typeFullName, ConversionError(type.Location(), typeFullName, EdmConstants.Type_TypeDefinition)),
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

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.Entity)
            {
                return new EdmEntityTypeReference((IEdmEntityType)typeDefinition, type.IsNullable);
            }

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Entity));
            }

            return new BadEntityTypeReference(typeFullName, type.IsNullable, errors);
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

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.EntityReference)
            {
                return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)typeDefinition, type.IsNullable);
            }

            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), type.FullName(), EdmConstants.Type_EntityReference));
            }

            return new EdmEntityReferenceTypeReference(new BadEntityReferenceType(errors), type.IsNullable);
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

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind == EdmTypeKind.Complex)
            {
                return new EdmComplexTypeReference((IEdmComplexType)typeDefinition, type.IsNullable);
            }

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Complex));
            }

            return new BadComplexTypeReference(typeFullName, type.IsNullable, errors);
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

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Spatial));
            }

            return new BadSpatialTypeReference(typeFullName, type.IsNullable, errors);
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

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Temporal));
            }

            return new BadTemporalTypeReference(typeFullName, type.IsNullable, errors);
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

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Decimal));
            }

            return new BadDecimalTypeReference(typeFullName, type.IsNullable, errors);
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

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_String));
            }

            return new BadStringTypeReference(typeFullName, type.IsNullable, errors);
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

            string typeFullName = type.FullName();
            List<EdmError> errors = new List<EdmError>(type.Errors());
            if (errors.Count == 0)
            {
                errors.AddRange(ConversionError(type.Location(), typeFullName, EdmConstants.Type_Binary));
            }

            return new BadBinaryTypeReference(typeFullName, type.IsNullable, errors);
        }
        #endregion

        /// <summary>
        /// Returns the primitive kind of the definition of this reference.
        /// </summary>
        /// <param name="type">Reference to the calling object.</param>
        /// <returns>The primitive kind of the definition of this reference.</returns>
        public static EdmPrimitiveTypeKind PrimitiveKind(this IEdmTypeReference type)
        {
            if (type == null)
            {
                return EdmPrimitiveTypeKind.None;
            }

            IEdmType typeDefinition = type.Definition;
            if (typeDefinition.TypeKind != EdmTypeKind.Primitive)
            {
                return EdmPrimitiveTypeKind.None;
            }

            return ((IEdmPrimitiveType)typeDefinition).PrimitiveKind;
        }

        /// <summary>
        /// Determines if the potential base type is in the inheritance hierarchy of the type being tested.
        /// </summary>
        /// <param name="type">Type to be tested for derivation from the other type.</param>
        /// <param name="potentialBaseType">The potential base type of the type being tested.</param>
        /// <returns>True if and only if the type inherits from the potential base type.</returns>
        public static bool InheritsFrom(this IEdmStructuredType type, IEdmStructuredType potentialBaseType)
        {
            do
            {
                type = type.BaseType;
                if (type != null && type.IsEquivalentTo(potentialBaseType))
                {
                    return true;
                }
            }
            while (type != null);

            return false;
        }

        /// <summary>
        /// Determines if a type is equivalent to or derived from another type.
        /// </summary>
        /// <param name="thisType">Type to be tested for equivalence to or derivation from the other type.</param>
        /// <param name="otherType">Type that is the other type.</param>
        /// <returns>True if and only if the thisType is equivalent to or inherits from otherType.</returns>
        public static bool IsOrInheritsFrom(this IEdmType thisType, IEdmType otherType)
        {
            if (thisType == null || otherType == null)
            {
                return false;
            }

            if (thisType.IsEquivalentTo(otherType))
            {
                return true;
            }

            EdmTypeKind thisKind = thisType.TypeKind;
            if (thisKind != otherType.TypeKind || !(thisKind == EdmTypeKind.Entity || thisKind == EdmTypeKind.Complex))
            {
                return false;
            }

            return ((IEdmStructuredType)thisType).InheritsFrom((IEdmStructuredType)otherType);
        }

        /// <summary>
        /// Determines whether thisType is the same as otherType, or thisType derives from otherType, or
        /// otherType derives from thisType.
        /// </summary>
        /// <param name="thisType">This EDM type.</param>
        /// <param name="otherType">The other EDM type.</param>
        /// <returns>true if thisType and otherType are along the same line in the type hierarchy; false otherwise.</returns>
        public static bool IsOnSameTypeHierarchyLineWith(this IEdmType thisType, IEdmType otherType)
        {
            return thisType.IsOrInheritsFrom(otherType) || otherType.IsOrInheritsFrom(thisType);
        }

        /// <summary>
        /// Returns the actual type of the given type.
        /// If the given type is type definition, the actual type is its underlying type;
        /// otherwise, return the given type itself.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <returns>The actual type of the given type.</returns>
        public static IEdmType AsActualType(this IEdmType type)
        {
            IEdmPrimitiveType underlyingType = type.UnderlyingType();

            return underlyingType ?? type;
        }

        internal static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(this IEdmPrimitiveType type, bool isNullable)
        {
            switch (type.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.Date:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Stream:
                    return new EdmPrimitiveTypeReference(type, isNullable);
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmBinaryTypeReference(type, isNullable);
                case EdmPrimitiveTypeKind.String:
                    return new EdmStringTypeReference(type, isNullable);
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalTypeReference(type, isNullable);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return new EdmTemporalTypeReference(type, isNullable);
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryCollection:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return new EdmSpatialTypeReference(type, isNullable);
                default:
                    throw new InvalidOperationException(Edm.Strings.EdmPrimitive_UnexpectedKind);
            }
        }

        internal static IEdmTypeReference GetTypeReference(this IEdmType type, bool isNullable)
        {
            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType != null)
            {
                return primitiveType.GetPrimitiveTypeReference(isNullable);
            }

            IEdmComplexType complexType = type as IEdmComplexType;
            if (complexType != null)
            {
                return new EdmComplexTypeReference(complexType, isNullable);
            }

            IEdmEntityType entityType = type as IEdmEntityType;
            if (entityType != null)
            {
                return new EdmEntityTypeReference(entityType, isNullable);
            }

            IEdmEnumType enumType = type as IEdmEnumType;
            if (enumType != null)
            {
                return new EdmEnumTypeReference(enumType, isNullable);
            }

            throw new InvalidOperationException(Edm.Strings.EdmType_UnexpectedEdmType);
        }

        internal static IEdmPrimitiveType UnderlyingType(this IEdmType type)
        {
            if (type == null || type.TypeKind != EdmTypeKind.TypeDefinition)
            {
                return null;
            }

            return ((IEdmTypeDefinition)type).UnderlyingType;
        }

        internal static IEdmPrimitiveType UnderlyingType(this IEdmTypeReference type)
        {
            if (type == null)
            {
                return null;
            }

            return type.Definition.UnderlyingType();
        }

        internal static IEdmTypeReference AsActualTypeReference(this IEdmTypeReference type)
        {
            if (type == null || type.TypeKind() != EdmTypeKind.TypeDefinition)
            {
                return type;
            }

            return type.AsPrimitive();
        }

        internal static bool CanSpecifyMaxLength(this IEdmPrimitiveType type)
        {
            switch (type.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.String:
                    return true;
            }

            return false;
        }

        private static IEnumerable<EdmError> ConversionError(EdmLocation location, string typeName, string typeKindName)
        {
            return new[] { new EdmError(location, EdmErrorCode.TypeSemanticsCouldNotConvertTypeReference, Edm.Strings.TypeSemantics_CouldNotConvertTypeReference(typeName ?? EdmConstants.Value_UnnamedType, typeKindName)) };
        }
    }
}
