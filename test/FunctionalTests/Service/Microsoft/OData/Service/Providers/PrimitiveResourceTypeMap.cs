//---------------------------------------------------------------------
// <copyright file="PrimitiveResourceTypeMap.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Xml.Linq;
    using Microsoft.Spatial;

    #endregion

    /// <summary>
    /// Mapping between primitive CLR types, EDM type names, and ResourceTypes
    /// </summary>
    internal class PrimitiveResourceTypeMap
    {
        /// <summary>Set of ResourceTypes for this instance of the map.</summary>
        private readonly ResourceType[] primitiveResourceTypes;

        /// <summary>Set of ResourceTypes that can be inherted.</summary>
        private readonly ResourceType[] inheritablePrimitiveResourceTypes;

        /// <summary>List of primitive types supported by WCF Data Services and their corresponding EDM names.</summary>
        private static readonly KeyValuePair<Type, string>[] builtInTypesMapping =
            new KeyValuePair<Type, string>[]
            {
                new KeyValuePair<Type, string>(typeof(string), XmlConstants.EdmStringTypeName),
                new KeyValuePair<Type, string>(typeof(Boolean), XmlConstants.EdmBooleanTypeName),
                new KeyValuePair<Type, string>(typeof(Boolean?), XmlConstants.EdmBooleanTypeName),
                new KeyValuePair<Type, string>(typeof(Byte), XmlConstants.EdmByteTypeName),
                new KeyValuePair<Type, string>(typeof(Byte?), XmlConstants.EdmByteTypeName),
                new KeyValuePair<Type, string>(typeof(DateTime), XmlConstants.EdmDateTimeOffsetTypeName),
                new KeyValuePair<Type, string>(typeof(DateTime?), XmlConstants.EdmDateTimeOffsetTypeName),
                new KeyValuePair<Type, string>(typeof(Decimal), XmlConstants.EdmDecimalTypeName),
                new KeyValuePair<Type, string>(typeof(Decimal?), XmlConstants.EdmDecimalTypeName),
                new KeyValuePair<Type, string>(typeof(Double), XmlConstants.EdmDoubleTypeName),
                new KeyValuePair<Type, string>(typeof(Double?), XmlConstants.EdmDoubleTypeName),
                new KeyValuePair<Type, string>(typeof(Guid), XmlConstants.EdmGuidTypeName),
                new KeyValuePair<Type, string>(typeof(Guid?), XmlConstants.EdmGuidTypeName),
                new KeyValuePair<Type, string>(typeof(Int16), XmlConstants.EdmInt16TypeName),
                new KeyValuePair<Type, string>(typeof(Int16?), XmlConstants.EdmInt16TypeName),
                new KeyValuePair<Type, string>(typeof(Int32), XmlConstants.EdmInt32TypeName),
                new KeyValuePair<Type, string>(typeof(Int32?), XmlConstants.EdmInt32TypeName),
                new KeyValuePair<Type, string>(typeof(Int64), XmlConstants.EdmInt64TypeName),
                new KeyValuePair<Type, string>(typeof(Int64?), XmlConstants.EdmInt64TypeName),
                new KeyValuePair<Type, string>(typeof(SByte), XmlConstants.EdmSByteTypeName),
                new KeyValuePair<Type, string>(typeof(SByte?), XmlConstants.EdmSByteTypeName),
                new KeyValuePair<Type, string>(typeof(Single), XmlConstants.EdmSingleTypeName),
                new KeyValuePair<Type, string>(typeof(Single?), XmlConstants.EdmSingleTypeName),
                new KeyValuePair<Type, string>(typeof(byte[]), XmlConstants.EdmBinaryTypeName),
                new KeyValuePair<Type, string>(typeof(Stream), XmlConstants.EdmStreamTypeName),
                new KeyValuePair<Type, string>(typeof(Geography), XmlConstants.EdmGeographyTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyPoint), XmlConstants.EdmPointTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyLineString), XmlConstants.EdmLineStringTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyPolygon), XmlConstants.EdmPolygonTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyCollection), XmlConstants.EdmGeographyCollectionTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyMultiLineString), XmlConstants.EdmMultiLineStringTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyMultiPoint), XmlConstants.EdmMultiPointTypeName),
                new KeyValuePair<Type, string>(typeof(GeographyMultiPolygon), XmlConstants.EdmMultiPolygonTypeName),
                new KeyValuePair<Type, string>(typeof(Geometry), XmlConstants.EdmGeometryTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryPoint), XmlConstants.EdmGeometryPointTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryLineString), XmlConstants.EdmGeometryLineStringTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryPolygon), XmlConstants.EdmGeometryPolygonTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryCollection), XmlConstants.EdmGeometryCollectionTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryMultiLineString), XmlConstants.EdmGeometryMultiLineStringTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryMultiPoint), XmlConstants.EdmGeometryMultiPointTypeName),
                new KeyValuePair<Type, string>(typeof(GeometryMultiPolygon), XmlConstants.EdmGeometryMultiPolygonTypeName),
                new KeyValuePair<Type, string>(typeof(TimeSpan), XmlConstants.EdmDurationTypeName),
                new KeyValuePair<Type, string>(typeof(TimeSpan?), XmlConstants.EdmDurationTypeName),
                new KeyValuePair<Type, string>(typeof(DateTimeOffset), XmlConstants.EdmDateTimeOffsetTypeName),
                new KeyValuePair<Type, string>(typeof(DateTimeOffset?), XmlConstants.EdmDateTimeOffsetTypeName),
#if !EF6Provider
                // Keep the Binary and XElement in the end, since there are not the default mappings for Edm.Binary and Edm.String.
                new KeyValuePair<Type, string>(typeof(XElement), XmlConstants.EdmStringTypeName),
                new KeyValuePair<Type, string>(typeof(Binary), XmlConstants.EdmBinaryTypeName),
#endif
            };

        /// <summary> List of primitive type that can be derived from.</summary>
        private static readonly Type[] inheritablePrimitiveClrTypes = new Type[]
            {
                typeof(Geography), typeof(GeographyPoint), typeof(GeographyLineString), typeof(GeographyPolygon), typeof(GeographyCollection), typeof(GeographyMultiPoint), typeof(GeographyMultiLineString), typeof(GeographyMultiPolygon),
                typeof(Geometry), typeof(GeometryPoint), typeof(GeometryLineString), typeof(GeometryPolygon), typeof(GeometryCollection), typeof(GeometryMultiPoint), typeof(GeometryMultiLineString), typeof(GeometryMultiPolygon),
            };

        /// <summary>Mapping between primitive CLR types, EDM type names, and ResourceTypes.</summary>
        private static PrimitiveResourceTypeMap primitiveResourceTypeMapping;

        /// <summary>
        /// Creates a new instance of the type map using the set of all primitive types supported by WCF Data Services.
        /// </summary>
        internal PrimitiveResourceTypeMap() :
            this(builtInTypesMapping)
        {
        }

        /// <summary>
        /// Creates a new instance of the type map using the specified set of types.
        /// </summary>
        /// <param name="primitiveTypesEdmNameMapping">Primitive CLR type-to-string mapping information to use to build the type map.</param>
        internal PrimitiveResourceTypeMap(KeyValuePair<Type, string>[] primitiveTypesEdmNameMapping)
        {
            Debug.Assert(primitiveTypesEdmNameMapping != null && primitiveTypesEdmNameMapping.Length != 0, "Can't create a primitive resource type map based on a null or empty set of EDM mappings.");

            int typeMapLength = primitiveTypesEdmNameMapping.Length;
            this.primitiveResourceTypes = new ResourceType[typeMapLength];
            List<ResourceType> resourceTypesThatCanBeInherited = new List<ResourceType>(inheritablePrimitiveClrTypes.Length);
            for (int i = 0; i < typeMapLength; i++)
            {
                string fullName = primitiveTypesEdmNameMapping[i].Value;
                Debug.Assert(fullName.StartsWith(XmlConstants.EdmNamespace, StringComparison.Ordinal), "fullName.StartsWith(XmlConstants.EdmNamespace, StringComparison.Ordinal)");
                string name = fullName.Substring(XmlConstants.EdmNamespace.Length + 1);
#if EF6Provider
                this.primitiveResourceTypes[i] = ResourceType.GetPrimitiveResourceType(primitiveTypesEdmNameMapping[i].Key);
#else
                this.primitiveResourceTypes[i] = new ResourceType(primitiveTypesEdmNameMapping[i].Key, ResourceTypeKind.Primitive, XmlConstants.EdmNamespace, name);
#endif
                if (inheritablePrimitiveClrTypes.Contains(primitiveTypesEdmNameMapping[i].Key))
                {
                    resourceTypesThatCanBeInherited.Add(this.primitiveResourceTypes[i]);
                }
            }

            this.inheritablePrimitiveResourceTypes = resourceTypesThatCanBeInherited.ToArray();
        }

        /// <summary>
        /// Mapping between primitive CLR types, EDM type names, and ResourceTypes.
        /// </summary>
        internal static PrimitiveResourceTypeMap TypeMap
        {
            get
            {
                if (primitiveResourceTypeMapping == null)
                {
                    PrimitiveResourceTypeMap mapping = new PrimitiveResourceTypeMap();
                    Interlocked.CompareExchange<PrimitiveResourceTypeMap>(ref primitiveResourceTypeMapping, mapping, null);
                }

                Debug.Assert(primitiveResourceTypeMapping != null, "primitiveResourceTypeMapping should have been initialized");
                return primitiveResourceTypeMapping;
            }
        }

        /// <summary>
        /// Returns all ResourceTypes for this type map.
        /// </summary>
        internal ResourceType[] AllPrimitives
        {
            get
            {
                return this.primitiveResourceTypes;
            }
        }

        /// <summary>
        /// Returns the primitive ResourceType for the specified CLR type.
        /// </summary>
        /// <param name="type">CLR type to use for lookup.</param>
        /// <returns>Primitive ResourceType that maps to <paramref name="type"/> or null if the type is not mapped.</returns>
        internal ResourceType GetPrimitive(Type type)
        {
            WebUtil.CheckArgumentNull(type, "type");
            ResourceType primitiveResourceType = this.primitiveResourceTypes.FirstOrDefault(rt => rt.InstanceType == type);
            if (primitiveResourceType == null)
            {
                foreach (ResourceType possibleMatch in this.inheritablePrimitiveResourceTypes)
                {
                    // If the primitive type is a sub class of the one of the known primitive types
                    // then its a possible match
                    if (possibleMatch.InstanceType.IsAssignableFrom(type))
                    {
                        // In order to find the most derived type for the best match, we need to check
                        // if the current match is a more derived type than the previous match
                        if (primitiveResourceType == null || primitiveResourceType.InstanceType.IsAssignableFrom(possibleMatch.InstanceType))
                        {
                            primitiveResourceType = possibleMatch;
                        }
                    }
                }
            }

            return primitiveResourceType;
        }

        /// <summary>
        /// Returns the primitive ResourceType for the specified EDM type name.
        /// </summary>
        /// <param name="fullEdmTypeName">Fully-qualified EDM type name to use for lookup.</param>
        /// <returns>Primitive ResourceType that maps to <paramref name="fullEdmTypeName"/> or null if the type is not mapped.</returns>
        internal ResourceType GetPrimitive(string fullEdmTypeName)
        {
            Debug.Assert(!String.IsNullOrEmpty(fullEdmTypeName), "Can't look up a primitive resource type based on a null or empty EDM type name.");
            return this.primitiveResourceTypes.FirstOrDefault(rt => rt.FullName == fullEdmTypeName);
        }

        /// <summary>
        /// Whether or not the specified CLR type maps to a primitive ResourceType.
        /// </summary>
        /// <param name="type">CLR type to use for lookup</param>
        /// <returns>True if <paramref name="type"/> maps to a primitive ResourceType, otherwise false.</returns>
        internal bool IsPrimitive(Type type)
        {
            return this.GetPrimitive(type) != null;
        }
    }
}
