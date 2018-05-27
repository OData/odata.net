//---------------------------------------------------------------------
// <copyright file="EntityModelUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Astoria.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Spatial.EntityModel;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with EDM types.
    /// </summary>
    public static class EntityModelUtils
    {
        /// <summary>
        /// Prefix of collection type names.
        /// </summary>
        private const string CollectionTypeNamePrefix = "Collection(";

        /// <summary>
        /// Suffix of collection type names.
        /// </summary>
        private const string CollectionTypeNameSuffix = ")";

        /// <summary>
        /// Maps primitive CLR type to the appropriate EDM type.
        /// </summary>
        private static Dictionary<Type, PrimitiveDataType> primitiveClrToEdmTypeMap;

        /// <summary>
        /// Maps primitive CLR type to the appropriate EDM type.
        /// </summary>
        private static readonly ClrToPrimitiveDataTypeConverter clrToPrimitiveDataTypeConverter;

        /// <summary>
        /// Class constructor.
        /// </summary>
        static EntityModelUtils()
        {
            primitiveClrToEdmTypeMap = new Dictionary<Type, PrimitiveDataType>();

            clrToPrimitiveDataTypeConverter = new ClrToPrimitiveDataTypeConverter() { SpatialResolver = new SpatialClrTypeResolver() };
            foreach (var t in EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest))
            {
                Type clrType = clrToPrimitiveDataTypeConverter.ToClrType(t);
                if (clrType == typeof(string) || clrType == typeof(byte[]))
                {
                    // only add the nullable variants to the map for string and byte[]
                    primitiveClrToEdmTypeMap.Add(clrType, t.Nullable());
                }
                else
                {
                    primitiveClrToEdmTypeMap.Add(clrType, t);
                    if (!TestTypeUtils.TypeAllowsNull(clrType))
                    {
                        primitiveClrToEdmTypeMap.Add(typeof(Nullable<>).MakeGenericType(clrType), t.Nullable());
                    }
                }
            }
        }

        /// <summary>
        /// Converts a variable of type 'DataType' to type 'System.Type'
        /// </summary>
        /// <param name="type">type to convert</param>
        /// <returns>System.Type representation of type</returns>
        public static Type GetPrimitiveClrType(PrimitiveDataType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return clrToPrimitiveDataTypeConverter.ToClrType(type);
        }

        /// <summary>
        /// Given a CLR type this methods returns the EDM primitive type for it.
        /// </summary>
        /// <param name="clrType">The CLR type to get the EDM type for.</param>
        /// <returns>EDM primitive type representing the same type as the specified <paramref name="clrType"/>.</returns>
        public static PrimitiveDataType GetPrimitiveEdmType(Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");

            PrimitiveDataType primitiveDataType = null;
            if (!primitiveClrToEdmTypeMap.TryGetValue(clrType, out primitiveDataType) && typeof(ISpatial).IsAssignableFrom(clrType))
            {
                Type bestMatch = typeof(object);
                foreach (var p in primitiveClrToEdmTypeMap.Where(v => typeof(ISpatial).IsAssignableFrom(v.Key)))
                {
                    if (p.Key.IsAssignableFrom(clrType) && bestMatch.IsAssignableFrom(p.Key))
                    {
                        primitiveDataType = p.Value;
                        bestMatch = p.Key;
                    }
                }

                if (primitiveDataType != null)
                {
                    primitiveClrToEdmTypeMap.Add(clrType, primitiveDataType);
                }
            }

            if (primitiveDataType == null)
            {
                ExceptionUtilities.Assert(false, "Can't find primitive EDM type for CLR type {0}.", clrType.ToString());
            }

            if (clrType == typeof(string) || clrType == typeof(byte[]))
            {
                return primitiveDataType.Nullable();
            }

            return primitiveDataType;
        }

        /// <summary>
        /// Given the full name of an EDM primitive type, returns that type.
        /// </summary>
        /// <param name="edmTypeName">The full name of the EDM primitive type to get.</param>
        /// <returns>The EDM primitive type of the specified name; or null if no EDM primitive type of the specified name was found.</returns>
        public static PrimitiveDataType GetPrimitiveEdmType(string edmTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(edmTypeName, "edmTypeName");

            PrimitiveDataType primitiveDataType = EdmDataTypes.GetAllPrimitiveTypes(EdmVersion.Latest).SingleOrDefault(t => t.FullEdmName() == edmTypeName);

            // OData only supports nullable versions of Edm.String and Edm.Binary
            if (string.CompareOrdinal(EdmConstants.EdmStringTypeName, edmTypeName) == 0 ||
                string.CompareOrdinal(EdmConstants.EdmBinaryTypeName, edmTypeName) == 0)
            {
                return primitiveDataType.Nullable();
            }
            else
            {
                return primitiveDataType;
            }
        }

        /// <summary>
        /// Returns the full EDM name (namespace and type name) for the specified primitive type.
        /// </summary>
        /// <param name="primitiveDataType">The primitive type to get the full name for.</param>
        /// <returns>The full EDM name, namespace and type name for the specified <paramref name="primitiveDataType"/>.</returns>
        public static string FullEdmName(this PrimitiveDataType primitiveDataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(primitiveDataType, "primitiveDataType");

            string edmTypeName = primitiveDataType.GetFacetValue<EdmTypeNameFacet, string>(null);
            string edmNamespace = primitiveDataType.GetFacetValue<EdmNamespaceFacet, string>(null);

            if (!string.IsNullOrEmpty(edmTypeName) && !string.IsNullOrEmpty(edmNamespace))
            {
                edmTypeName = edmNamespace + '.' + edmTypeName;
            }

            return edmTypeName;
        }

        /// <summary>
        /// Returns the item type name of a collection item type name.
        /// </summary>
        /// <param name="collectionTypeName">The collection item type name to parse.</param>
        /// <returns>The item type name or null if the <paramref name="collectionTypeName"/> is not a collection type name.</returns>
        public static string GetCollectionItemTypeName(string collectionTypeName)
        {
            ExceptionUtilities.CheckArgumentNotNull(collectionTypeName, "collectionTypeName");

            if (collectionTypeName.StartsWith(CollectionTypeNamePrefix) && collectionTypeName.EndsWith(CollectionTypeNameSuffix))
            {
                return collectionTypeName.Substring(
                    CollectionTypeNamePrefix.Length,
                    collectionTypeName.Length - CollectionTypeNamePrefix.Length - CollectionTypeNameSuffix.Length);
            }

            return null;
        }

        /// <summary>
        /// Constructs a collection type name from the type name of the item.
        /// </summary>
        /// <param name="itemTypeName">The type name of the item in collection.</param>
        /// <returns>The collection type name.</returns>
        public static string GetCollectionTypeName(string itemTypeName)
        {
            return CollectionTypeNamePrefix + itemTypeName + CollectionTypeNameSuffix;
        }

        /// <summary>
        /// Gets the value of the <see cref="MinimumRequiredVersionAnnotation"/> annotation on the model
        /// or returns V3 if not such annotation exists.
        /// </summary>
        /// <param name="model">The <see cref="EntityModelSchema"/> to annotate.</param>
        /// <returns>The minimum OData version required for the <paramref name="model"/>.</returns>
        public static ODataVersion MinimumVersion(this EntityModelSchema model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            MinimumRequiredVersionAnnotation annotation = model.Annotations.OfType<MinimumRequiredVersionAnnotation>().SingleOrDefault();
            if (annotation != null)
            {
                return annotation.MinimumVersion;
            }

            return ODataVersion.V4;
        }

        /// <summary>
        /// Returns a full name of entity set.
        /// </summary>
        /// <param name="entitySet">The entity set to get the name for.</param>
        /// <returns>The container qualified full name of the entity set.</returns>
        public static string FullEdmName(this EntitySet entitySet)
        {
            return entitySet.Name;
        }
    }
}
