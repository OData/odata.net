//---------------------------------------------------------------------
// <copyright file="EdmClrTypeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.IO;
    using Microsoft.OData.Edm;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Services.ODataWCFService.Extensions;
    using Microsoft.Test.OData.Services.ODataWCFService.Services;

    internal static class EdmClrTypeUtils
    {
        public static IEdmType GetEdmType(IEdmModel model, object entity)
        {
            return model.FindType(entity.GetType().FullName);
        }

        /// <summary>
        /// Returns the instance type for the specified Edm type or null if none exists.
        /// </summary>
        /// <param name="type">The type to get the instance type for.</param>
        /// <returns>The instance type for the <paramref name="typeReference"/> or null if no instance type exists.</returns>
        public static Type GetInstanceType(IEdmTypeReference type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Type result = null;

            if (type.TypeKind() == EdmTypeKind.TypeDefinition)
            {
                var td = type.Definition as IEdmTypeDefinition;
                result = GetUnsignedIntInstanceType(td, type.IsNullable);
            }
            else if (type.TypeKind() == EdmTypeKind.Primitive)
            {
                // TODO: Find out if the type is nullable
                result = GetPrimitiveInstanceType(type.Definition as IEdmPrimitiveType, type.IsNullable);
            }
            else
            {
                // TODO: We should load from DataSource assembly.
                result = GetInstanceType(type.Definition, type.Definition.FullTypeName());
            }

            if (result != null)
            {
                return result;
            }
            else
            {
                throw new InvalidOperationException(string.Format("Cannot find instance type for EdmType {0}.", type.Definition.FullTypeName()));
            }
        }

        /// <summary>
        /// Get Instance type from type name.
        /// </summary>
        /// <param name="typeName">Type Name</param>
        /// <returns>Type value</returns>
        public static Type GetInstanceType(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("ConstantExpression Value");
            }

            return TransateEdmType(typeName);
        }

        #region Private Method
        /// <summary>
        /// Getting the instance type from the assembly in name
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Type GetInstanceType(IEdmType type, string typeName)
        {
            Type result = null;

            switch (type.TypeKind)
            {
                case EdmTypeKind.Enum:
                    result = GetTypeFromModels(typeName);

                    if (result != null && !result.IsEnum)
                    {
                        throw new InvalidOperationException(
                            string.Format("The EdmType {0} is not match to the instance type {1}.", type.FullTypeName(), result.FullName));
                    }

                    break;

                case EdmTypeKind.Entity:
                    result = GetTypeFromModels(typeName);

                    // TODO: Validate the type is a entity type.
                    break;

                case EdmTypeKind.Complex:
                    result = GetTypeFromModels(typeName);

                    // TODO: Validate the type is a complex type.
                    break;

                case EdmTypeKind.Collection:
                    var elementType = GetTypeFromModels((type as IEdmCollectionType).ElementType.Definition.FullTypeName());
                    result = typeof (List<>).MakeGenericType(new[] {elementType});
                    break;
                // It seems we never hit here.
                default:
                    throw new InvalidOperationException(string.Format("GetInstanceType for TypeKind {0} is not supported.", type.TypeKind));
            }

            return result;
        }

        /// <summary>
        /// Returns the instance type for primitive Edm types or null if none exists. 
        /// </summary>
        /// <param name="type">The type to get the instance type for.</param>
        /// <returns>The instance type for the <paramref name="typeReference"/> or null if no instance type exists.</returns>
        private static Type GetPrimitiveInstanceType(IEdmPrimitiveType type, bool isNullable)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return GetPrimitiveClrType(type.PrimitiveKind, isNullable);
        }

        private static Type GetUnsignedIntInstanceType(IEdmTypeDefinition type, bool isNullable)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.Name == "UInt32")
            {
                return isNullable ? typeof(UInt32?) : typeof(UInt32);
            }
            else if (type.Name == "UInt16")
            {
                return isNullable ? typeof(UInt16?) : typeof(UInt16);
            }
            else if (type.Name == "UInt64")
            {
                return isNullable ? typeof(UInt64?) : typeof(UInt64);
            }
            else
            {
                return GetPrimitiveInstanceType(type.UnderlyingType, isNullable);
            }
        }


        /// <summary>
        /// Get Clr type
        /// </summary>
        /// <param name="typeKind">Edm Primitive Type Kind</param>
        /// <param name="isNullable">Nullable value</param>
        /// <returns>CLR type</returns>
        private static Type GetPrimitiveClrType(EdmPrimitiveTypeKind typeKind, bool isNullable)
        {
            switch (typeKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return typeof(byte[]);
                case EdmPrimitiveTypeKind.Boolean:
                    return isNullable ? typeof(Boolean?) : typeof(Boolean);
                case EdmPrimitiveTypeKind.Byte:
                    return isNullable ? typeof(Byte?) : typeof(Byte);
                case EdmPrimitiveTypeKind.Date:
                    return isNullable ? typeof(Date?) : typeof(Date);
                case EdmPrimitiveTypeKind.DateTimeOffset:
                    return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);
                case EdmPrimitiveTypeKind.Decimal:
                    return isNullable ? typeof(Decimal?) : typeof(Decimal);
                case EdmPrimitiveTypeKind.Double:
                    return isNullable ? typeof(Double?) : typeof(Double);
                case EdmPrimitiveTypeKind.Geography:
                    return typeof(Geography);
                case EdmPrimitiveTypeKind.GeographyCollection:
                    return typeof(GeographyCollection);
                case EdmPrimitiveTypeKind.GeographyLineString:
                    return typeof(GeographyLineString);
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return typeof(GeographyMultiLineString);
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return typeof(GeographyMultiPoint);
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return typeof(GeographyMultiPolygon);
                case EdmPrimitiveTypeKind.GeographyPoint:
                    return typeof(GeographyPoint);
                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return typeof(GeographyPolygon);
                case EdmPrimitiveTypeKind.Geometry:
                    return typeof(Geometry);
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return typeof(GeometryCollection);
                case EdmPrimitiveTypeKind.GeometryLineString:
                    return typeof(GeometryLineString);
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return typeof(GeometryMultiLineString);
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return typeof(GeometryMultiPoint);
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return typeof(GeometryMultiPolygon);
                case EdmPrimitiveTypeKind.GeometryPoint:
                    return typeof(GeometryPoint);
                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return typeof(GeometryPolygon);
                case EdmPrimitiveTypeKind.Guid:
                    return isNullable ? typeof(Guid?) : typeof(Guid);
                case EdmPrimitiveTypeKind.Int16:
                    return isNullable ? typeof(Int16?) : typeof(Int16);
                case EdmPrimitiveTypeKind.Int32:
                    return isNullable ? typeof(Int32?) : typeof(Int32);
                case EdmPrimitiveTypeKind.Int64:
                    return isNullable ? typeof(Int64?) : typeof(Int64);
                case EdmPrimitiveTypeKind.SByte:
                    return isNullable ? typeof(SByte?) : typeof(SByte);
                case EdmPrimitiveTypeKind.Single:
                    return isNullable ? typeof(Single?) : typeof(Single);
                case EdmPrimitiveTypeKind.Stream:
                    return typeof(Stream);
                case EdmPrimitiveTypeKind.String:
                    return typeof(String);
                case EdmPrimitiveTypeKind.Duration:
                    return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
                case EdmPrimitiveTypeKind.TimeOfDay:
                    return isNullable ? typeof(TimeOfDay?) : typeof(TimeOfDay);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get CLR type from Edm type name
        /// </summary>
        /// <param name="typeName">Edm type name</param>
        /// <returns>CLR type</returns>
        private static Type TransateEdmType(string typeName)
        {
            string collectionString = "Collection(";
            string edmString = "Edm.";
            if (typeName.StartsWith(collectionString, StringComparison.OrdinalIgnoreCase))
            {
                var itemTypeName = typeName.Substring(collectionString.Length, typeName.Length - 1 - collectionString.Length);
                var elementType = TransateEdmType(itemTypeName);
                return typeof(List<>).MakeGenericType(new[] { elementType });
            }
            else if (typeName.StartsWith(edmString, StringComparison.OrdinalIgnoreCase))
            {
                string edmTypeValue = typeName.Substring(edmString.Length);
                EdmPrimitiveTypeKind edmType = EdmPrimitiveTypeKind.None;
                Enum.TryParse<EdmPrimitiveTypeKind>(edmTypeValue, out edmType);
                return GetPrimitiveClrType(edmType, false);
            }
            else
            {
                return GetTypeFromModels(typeName);
            }
        }

        private static Type GetTypeFromModels(string typeName)
        {
            var factories = ExtensionManager.Container.GetExportedValues<IODataServiceDescriptor>();

            foreach (var factory in factories)
            {
                var type = factory.ServiceType.Assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        #endregion
    }
}
