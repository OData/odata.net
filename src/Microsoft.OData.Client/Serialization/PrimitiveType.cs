//---------------------------------------------------------------------
// <copyright file="PrimitiveType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represent a Primitive Type on the client
    /// </summary>
    /// <remarks>
    /// 1) Performance
    /// For performance reasons we use several dictionaries here:
    /// - clrMapping contains well known primitive types. Initialized in the static constructor and never changed
    ///   after initialization. Therefore it is safe to read without locks. Also see comment about Binary type below
    /// - derivedPrimitiveTypeMapping - a map for custom primitive types derived from well known primitive types -
    ///   especially spatial types. New items may be added at runtime so reads and writes must be locked
    /// - knownNonPrimitiveTypes - a HashSet of types we have seen and determined they are not primitive. Used
    ///   to shortcircuit logic for finding derived primitive types for types we know are not primitive.
    /// To get a primitive type one *MUST NOT* clrMapping since clrMapping will not contain custom primitive types
    /// but call TryGetPrimitiveType method that knows how to handle multiple dictionaries.
    /// 2) System.Data.Linq.Binary
    /// We want to avoid static dependency on System.Data.Linq.dll. On the other hand System.Data.Linq.Binary is
    /// a well known primitive type. For performance reasons and to avoid locking the clrMapping is only initialized
    /// in the static ctor when we don't have System.Data.Linq.Binary type handy. Therefore we use the dummy BinaryTypeSub
    /// type during initialization. As a result to get a well known primitive type one *MUST NOT* use the clrMapping
    /// dictionary directly but call TryGetWellKnownPrimitiveType() method which knows how to handle BinaryType.
    /// </remarks>
    internal sealed class PrimitiveType
    {
        /// <summary>
        /// Clr Type - Primitive Type mapping for well known primitive types
        /// </summary>
        /// <remarks>
        /// It is being initialized in the static constructor and must not change
        /// later. This way we can avoid locking it.
        /// </remarks>
        private static readonly Dictionary<Type, PrimitiveType> clrMapping;

        /// <summary>
        /// Clr Type - Primitive Type mapping for custom derived primitive type (e.g. spatial types)
        /// </summary>
        /// <remarks>
        /// This dictionary contains type mapping for custom derived primitive types (e.g. spatial) that
        /// are types discovered at runtime and added as we go. Any access to this dictionary requires locking.
        /// </remarks>
        private static readonly Dictionary<Type, PrimitiveType> derivedPrimitiveTypeMapping;

        /// <summary>
        /// Edm Type - Primitive Type mapping
        /// </summary>
        private static readonly Dictionary<String, PrimitiveType> edmMapping;

        /// <summary>
        /// Cache containing known non-primitive types. Any access to this hashset requires locking.
        /// </summary>
        private static readonly HashSet<Type> knownNonPrimitiveTypes;

        /// <summary>
        /// Static Constructor
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Required")]
        static PrimitiveType()
        {
            clrMapping = new Dictionary<Type, PrimitiveType>(EqualityComparer<Type>.Default);
            edmMapping = new Dictionary<string, PrimitiveType>(StringComparer.Ordinal);
            derivedPrimitiveTypeMapping = new Dictionary<Type, PrimitiveType>(EqualityComparer<Type>.Default);
            knownNonPrimitiveTypes = new HashSet<Type>(EqualityComparer<Type>.Default);

            InitializeTypes();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clrType">The Clr Type</param>
        /// <param name="edmTypeName">The Edm Type Name</param>
        /// <param name="primitiveKind">The Edm Primitive Type Kind</param>
        /// <param name="typeConverter">A PrimitiveXmlConverter that provides convertion between  instances of this type to its Xml representation and back</param>
        /// <param name="hasReverseMapping">Whether this primitive type can be mapped from the Edm type name</param>
        private PrimitiveType(Type clrType, string edmTypeName, EdmPrimitiveTypeKind primitiveKind, PrimitiveTypeConverter typeConverter, bool hasReverseMapping)
        {
            Debug.Assert(clrType != null, "clrType != null");
            Debug.Assert(primitiveKind != EdmPrimitiveTypeKind.None, "primitiveKind != EdmPrimitiveTypeKind.None");
            Debug.Assert(typeConverter != null, "typeConverter != null");

            this.ClrType = clrType;
            this.EdmTypeName = edmTypeName;
            this.PrimitiveKind = primitiveKind;
            this.TypeConverter = typeConverter;
            this.HasReverseMapping = hasReverseMapping;
        }

        /// <summary>
        /// The Clr Type
        /// </summary>
        internal Type ClrType
        {
            get;
            private set;
        }

        /// <summary>
        /// The Edm Type Name
        /// </summary>
        internal String EdmTypeName
        {
            get;
            private set;
        }

        /// <summary>
        /// A PrimitiveXmlConverter that provides convertion between
        /// instances of this type to its Xml representation and back
        /// </summary>
        internal PrimitiveTypeConverter TypeConverter
        {
            get;
            private set;
        }

        /// <summary>
        /// This type has a reverse edm type mapping
        /// </summary>
        /// <remarks>
        /// Some known primitive types have shared edm type mapping
        /// Only one of these shared type can contain a reverse mapping
        /// </remarks>
        internal bool HasReverseMapping
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the types EDM primitive type kind
        /// </summary>
        internal EdmPrimitiveTypeKind PrimitiveKind
        {
            get;
            private set;
        }

        /// <summary>
        /// Try retrieve a primitive type metadata from a clr type
        /// </summary>
        /// <param name="clrType">The Clr Type</param>
        /// <param name="ptype">The returning primitive type</param>
        /// <returns>True if the type is found</returns>
        /// <remarks>
        /// See remarks for the class.
        /// </remarks>
        internal static bool TryGetPrimitiveType(Type clrType, out PrimitiveType ptype)
        {
            Type primitiveClrType = Nullable.GetUnderlyingType(clrType) ?? clrType;

            // Is it a well known primitive type?
            if (!TryGetWellKnownPrimitiveType(primitiveClrType, out ptype))
            {
                lock (knownNonPrimitiveTypes)
                {
                    // Is it a type that we know that is not primitive?
                    if (knownNonPrimitiveTypes.Contains(clrType))
                    {
                        ptype = null;
                        return false;
                    }
                }

                KeyValuePair<Type, PrimitiveType>[] possibleMatches;
                lock (derivedPrimitiveTypeMapping)
                {
                    // is it a derived primitive type?
                    if (derivedPrimitiveTypeMapping.TryGetValue(clrType, out ptype))
                    {
                        return true;
                    }

                    // note that clrMapping contains a substitute type for Binary. We don't really care about this type since you cannot
                    // derive from System.Data.Linq.Binary type as it is sealed. Also to get all other applicable types we need to concatenate
                    // well known primitive types and custom primitive types. We can exclude primitive and sealed types as they cannot be
                    // derived from.
                    possibleMatches = clrMapping.Where(m => !m.Key.IsPrimitive() && !m.Key.IsSealed()).Concat(derivedPrimitiveTypeMapping).ToArray();
                }

                var bestMatch = new KeyValuePair<Type, PrimitiveType>(typeof(object), null);
                foreach (var possibleMatch in possibleMatches)
                {
                    // If the primitive type is a sub class of the one of the known primitive types
                    // then its a possible match
                    if (primitiveClrType.IsSubclassOf(possibleMatch.Key))
                    {
                        // In order to find the most derived type for the best match, we need to check
                        // if the current match is a more derived type than the previous match
                        if (possibleMatch.Key.IsSubclassOf(bestMatch.Key))
                        {
                            bestMatch = possibleMatch;
                        }
                    }
                }

                if (bestMatch.Value == null)
                {
                    // this is not a primitive type - update the hashset accordingly.
                    lock (knownNonPrimitiveTypes)
                    {
                        // Note that this is hashset so it is OK if we try adding the same type more than once.
                        knownNonPrimitiveTypes.Add(clrType);
                    }

                    return false;
                }

                ptype = bestMatch.Value;
                lock (derivedPrimitiveTypeMapping)
                {
                    // this is a derived primitive type - update the dictionary accordingly
                    derivedPrimitiveTypeMapping[primitiveClrType] = ptype;
                }
            }

            return true;
        }

        /// <summary>
        /// Try retrieve a primitive type metadata from a Edm Type Name
        /// </summary>
        /// <param name="edmTypeName">Edm Type Name</param>
        /// <param name="ptype">The returning primitive type</param>
        /// <returns>True if the type is found</returns>
        internal static bool TryGetPrimitiveType(String edmTypeName, out PrimitiveType ptype)
        {
            return edmMapping.TryGetValue(edmTypeName, out ptype);
        }

        /// <summary>
        /// Is this a known primitive type (including string,byte[],uri)
        /// </summary>
        /// <param name="type">type to analyze</param>
        /// <returns>true if known primitive type</returns>
        internal static bool IsKnownType(Type type)
        {
            PrimitiveType primitiveType;
            return TryGetPrimitiveType(type, out primitiveType);
        }

        /// <summary>
        /// Is this a known primitive type or a nullable based on a primitive type (including string,byte[],uri)
        /// </summary>
        /// <param name="type">type to analyze, possibly nullable</param>
        /// <returns>true if known primitive type or a nullable based on a primitive type</returns>
        internal static bool IsKnownNullableType(Type type)
        {
            return IsKnownType(Nullable.GetUnderlyingType(type) ?? type);
        }

        /// <summary>
        /// Delete the type from known type table
        /// </summary>
        /// <param name="clrType">The clr type</param>
        /// <param name="edmTypeName">The edm type name to remove, or null</param>
        /// <remarks>This is a test clean up hook. MUST NOT BE CALLED FROM PRODUCT CODE.</remarks>
        internal static void DeleteKnownType(Type clrType, String edmTypeName)
        {
            // This is not thread safe. Product code should never modify clrMapping dictionary after initialization.
            // This allows for reads without locks.
            clrMapping.Remove(clrType);

            if (edmTypeName != null)
            {
                edmMapping.Remove(edmTypeName);
            }
        }

        /// <summary>
        /// Register a known type as primitive type
        /// </summary>
        /// <param name="clrType">The Clr Type</param>
        /// <param name="edmTypeName">The Edm Type Name</param>
        /// <param name="primitiveKind">The Edm Primitive Type Kind</param>
        /// <param name="converter">The Type Converter</param>
        /// <param name="twoWay">Whether this mapping should have a reverse mapping from Edm</param>
        /// <remarks>
        /// This method is internal only for testing purposes.
        /// IN PRODUCT MUST BE CALLED ONLY FROM THE STATIC CTOR OF THE PrimitiveType CLASS.
        /// </remarks>
        internal static void RegisterKnownType(Type clrType, string edmTypeName, EdmPrimitiveTypeKind primitiveKind, PrimitiveTypeConverter converter, bool twoWay)
        {
            Debug.Assert(!clrMapping.ContainsKey(clrType), "Clr type already registered");
            Debug.Assert(clrType != null, "clrType != null");
            Debug.Assert(primitiveKind != EdmPrimitiveTypeKind.None, "primitiveKind != EdmPrimitiveTypeKind.None");
            Debug.Assert(converter != null, "converter != null");

            PrimitiveType pt = new PrimitiveType(clrType, edmTypeName, primitiveKind, converter, twoWay);
            clrMapping.Add(clrType, pt);

            if (twoWay)
            {
                Debug.Assert(!edmMapping.ContainsKey(edmTypeName), "Edm type name already registered");
                edmMapping.Add(edmTypeName, pt);
            }
        }

        /// <summary>
        /// Creates a new instance of the corresponding IEdmPrimitiveType
        /// </summary>
        /// <returns>Returns a new instance of the corresponding IEdmPrimitiveType</returns>
        internal IEdmPrimitiveType CreateEdmPrimitiveType()
        {
            // Note we always create a new instance of an IEdmPrimitiveType instead of returning the static instance by calling
            // EdmCoreModel.Default.GetPrimitiveType(), for the following reasons:
            // 1. We will annotate the IEdmPrimitiveType with a ClientTypeAnnotation, which has a MaxProtocolVersion property. We can't map
            //    an instance of IEdmPrimitiveType to a CLR type, instead it is mapped to the (CLR type, MaxProtocolVersion) pair.
            // 2. We do have multiple CLR primitive types mapped to a single EDM primitive type (take Edm.String for example). In this case
            //    we need to create multiple instances of the Edm.String type and annotate them with the corresponding CLR type.
            return ClientEdmPrimitiveType.CreateType(this.PrimitiveKind);
        }

        /// <summary>
        /// Populate the mapping table
        /// </summary>
        /// <remarks>
        /// MUST NOT BE CALLED FROM PRODUCT CODE OTHER THAN STATIC CTOR OF PrimitiveType class.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Coupling necessary for type table")]
        private static void InitializeTypes()
        {
            // Following are types that are mapped directly to Edm type
            RegisterKnownType(typeof(Boolean), XmlConstants.EdmBooleanTypeName, EdmPrimitiveTypeKind.Boolean, new BooleanTypeConverter(), true);
            RegisterKnownType(typeof(Byte), XmlConstants.EdmByteTypeName, EdmPrimitiveTypeKind.Byte, new ByteTypeConverter(), true);
            RegisterKnownType(typeof(Byte[]), XmlConstants.EdmBinaryTypeName, EdmPrimitiveTypeKind.Binary, new ByteArrayTypeConverter(), true);
            RegisterKnownType(typeof(DateTimeOffset), XmlConstants.EdmDateTimeOffsetTypeName, EdmPrimitiveTypeKind.DateTimeOffset, new DateTimeOffsetTypeConverter(), true);
            RegisterKnownType(typeof(Decimal), XmlConstants.EdmDecimalTypeName, EdmPrimitiveTypeKind.Decimal, new DecimalTypeConverter(), true);
            RegisterKnownType(typeof(Double), XmlConstants.EdmDoubleTypeName, EdmPrimitiveTypeKind.Double, new DoubleTypeConverter(), true);
            RegisterKnownType(typeof(Guid), XmlConstants.EdmGuidTypeName, EdmPrimitiveTypeKind.Guid, new GuidTypeConverter(), true);
            RegisterKnownType(typeof(Int16), XmlConstants.EdmInt16TypeName, EdmPrimitiveTypeKind.Int16, new Int16TypeConverter(), true);
            RegisterKnownType(typeof(Int32), XmlConstants.EdmInt32TypeName, EdmPrimitiveTypeKind.Int32, new Int32TypeConverter(), true);
            RegisterKnownType(typeof(Int64), XmlConstants.EdmInt64TypeName, EdmPrimitiveTypeKind.Int64, new Int64TypeConverter(), true);
            RegisterKnownType(typeof(Single), XmlConstants.EdmSingleTypeName, EdmPrimitiveTypeKind.Single, new SingleTypeConverter(), true);
            RegisterKnownType(typeof(String), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new StringTypeConverter(), true);
            RegisterKnownType(typeof(SByte), XmlConstants.EdmSByteTypeName, EdmPrimitiveTypeKind.SByte, new SByteTypeConverter(), true);
            RegisterKnownType(typeof(TimeSpan), XmlConstants.EdmDurationTypeName, EdmPrimitiveTypeKind.Duration, new TimeSpanTypeConverter(), true);
            RegisterKnownType(typeof(Geography), XmlConstants.EdmGeographyTypeName, EdmPrimitiveTypeKind.Geography, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyPoint), XmlConstants.EdmPointTypeName, EdmPrimitiveTypeKind.GeographyPoint, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyLineString), XmlConstants.EdmLineStringTypeName, EdmPrimitiveTypeKind.GeographyLineString, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyPolygon), XmlConstants.EdmPolygonTypeName, EdmPrimitiveTypeKind.GeographyPolygon, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyCollection), XmlConstants.EdmGeographyCollectionTypeName, EdmPrimitiveTypeKind.GeographyCollection, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyMultiPoint), XmlConstants.EdmMultiPointTypeName, EdmPrimitiveTypeKind.GeographyMultiPoint, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyMultiLineString), XmlConstants.EdmMultiLineStringTypeName, EdmPrimitiveTypeKind.GeographyMultiLineString, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(GeographyMultiPolygon), XmlConstants.EdmMultiPolygonTypeName, EdmPrimitiveTypeKind.GeographyMultiPolygon, new GeographyTypeConverter(), true);
            RegisterKnownType(typeof(Geometry), XmlConstants.EdmGeometryTypeName, EdmPrimitiveTypeKind.Geometry, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryPoint), XmlConstants.EdmGeometryPointTypeName, EdmPrimitiveTypeKind.GeometryPoint, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryLineString), XmlConstants.EdmGeometryLineStringTypeName, EdmPrimitiveTypeKind.GeometryLineString, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryPolygon), XmlConstants.EdmGeometryPolygonTypeName, EdmPrimitiveTypeKind.GeometryPolygon, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryCollection), XmlConstants.EdmGeometryCollectionTypeName, EdmPrimitiveTypeKind.GeometryCollection, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryMultiPoint), XmlConstants.EdmGeometryMultiPointTypeName, EdmPrimitiveTypeKind.GeometryMultiPoint, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryMultiLineString), XmlConstants.EdmGeometryMultiLineStringTypeName, EdmPrimitiveTypeKind.GeometryMultiLineString, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(GeometryMultiPolygon), XmlConstants.EdmGeometryMultiPolygonTypeName, EdmPrimitiveTypeKind.GeometryMultiPolygon, new GeometryTypeConverter(), true);
            RegisterKnownType(typeof(DataServiceStreamLink), XmlConstants.EdmStreamTypeName, EdmPrimitiveTypeKind.Stream, new NamedStreamTypeConverter(), false);
            RegisterKnownType(typeof(Date), XmlConstants.EdmDateTypeName, EdmPrimitiveTypeKind.Date, new DateTypeConverter(), true);
            RegisterKnownType(typeof(TimeOfDay), XmlConstants.EdmTimeOfDayTypeName, EdmPrimitiveTypeKind.TimeOfDay, new TimeOfDayConvert(), true);

            // Following are known types are mapped to existing Edm type
            RegisterKnownType(typeof(Char), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new CharTypeConverter(), false);
            RegisterKnownType(typeof(Char[]), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new CharArrayTypeConverter(), false);
            RegisterKnownType(typeof(Type), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new ClrTypeConverter(), false);
            RegisterKnownType(typeof(Uri), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new UriTypeConverter(), false);
            RegisterKnownType(typeof(System.Xml.Linq.XDocument), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new XDocumentTypeConverter(), false);
            RegisterKnownType(typeof(System.Xml.Linq.XElement), XmlConstants.EdmStringTypeName, EdmPrimitiveTypeKind.String, new XElementTypeConverter(), false);

            // Following are known types that are not mapped to Edm
            RegisterKnownType(typeof(UInt16), null, EdmPrimitiveTypeKind.String, new UInt16TypeConverter(), false);
            RegisterKnownType(typeof(UInt32), null, EdmPrimitiveTypeKind.String, new UInt32TypeConverter(), false);
            RegisterKnownType(typeof(UInt64), null, EdmPrimitiveTypeKind.String, new UInt64TypeConverter(), false);

#if !PORTABLELIB
            // There is no static dependency on System.Data.Linq so we use a substitute type for the Binary type
            RegisterKnownType(typeof(BinaryTypeSub), XmlConstants.EdmBinaryTypeName, EdmPrimitiveTypeKind.Binary, new BinaryTypeConverter(), false);
#endif
        }

        /// <summary>
        /// Tries to get a well known PrimitiveType for a clr type. Contains logic to handle Binary type.
        /// </summary>
        /// <param name="clrType">The clr type to get well known PrimitiveType for.</param>
        /// <param name="ptype">PrimitiveType for the <paramref name="clrType"/> if exists. Otherwise null.</param>
        /// <returns><c>true</c> if a PrimitiveType for the <paramref name="clrType"/> was found. Otherwise <c>false</c>.</returns>
        private static bool TryGetWellKnownPrimitiveType(Type clrType, out PrimitiveType ptype)
        {
            Debug.Assert(clrType != null, "clrType != null");

            ptype = null;
            if (!clrMapping.TryGetValue(clrType, out ptype))
            {
#if !PORTABLELIB
                if (IsBinaryType(clrType))
                {
                    Debug.Assert(clrMapping.ContainsKey(typeof(BinaryTypeSub)), "BinaryTypeSub missing from the dictionary");
                    ptype = clrMapping[typeof(BinaryTypeSub)];
                }
#endif
            }

            return ptype != null;
        }

#if !PORTABLELIB// System.Data.Linq not available
        /// <summary>
        /// Whether the <paramref name="type"/> is System.Data.Linq.Binary.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns><c>true</c> if <paramref name="type"/> is System.Data.Linq.Binary. Otherwise <c>false</c>.</returns>
        private static bool IsBinaryType(Type type)
        {
            Debug.Assert(type != null, "clrType != null");

            if (BinaryTypeConverter.BinaryType == null && type.Name == "Binary")
            {
                if ((type.Namespace == "System.Data.Linq") &&
                    (System.Reflection.AssemblyName.ReferenceMatchesDefinition(
                        type.Assembly.GetName(), new System.Reflection.AssemblyName("System.Data.Linq"))))
                {
                    BinaryTypeConverter.BinaryType = type;
                }
            }

            return type == BinaryTypeConverter.BinaryType;
        }

        /// <summary>
        /// There is no static dependency on System.Data.Linq where Binary type lives. We
        /// will use this type to Substitute for the missing Binary type.
        /// </summary>
        private sealed class BinaryTypeSub
        {
        }
#endif

        /// <summary>
        /// Represents a definition of an EDM primitive type.
        /// </summary>
        private class ClientEdmPrimitiveType : EdmType, IEdmPrimitiveType
        {
            /// <summary>
            /// Namespace of the type.
            /// </summary>
            private readonly string namespaceName;

            /// <summary>
            /// Name of the type.
            /// </summary>
            private readonly string name;

            /// <summary>
            /// The kind of primitive.
            /// </summary>
            private readonly EdmPrimitiveTypeKind primitiveKind;

            /// <summary>
            /// Creates an instance of the client EDM primitive type.
            /// </summary>
            /// <param name="namespaceName">Namespace of the type.</param>
            /// <param name="name">Name of the type.</param>
            /// <param name="primitiveKind">Kind fo the primitive type.</param>
            private ClientEdmPrimitiveType(string namespaceName, string name, EdmPrimitiveTypeKind primitiveKind)
            {
                this.namespaceName = namespaceName;
                this.name = name;
                this.primitiveKind = primitiveKind;
            }

            /// <summary>
            /// Name of the type.
            /// </summary>
            public string Name
            {
                get { return this.name; }
            }

            /// <summary>
            /// Namespace of the type.
            /// </summary>
            public string Namespace
            {
                get { return this.namespaceName; }
            }

            /// <summary>
            /// Kind of the primitive type.
            /// </summary>
            public EdmPrimitiveTypeKind PrimitiveKind
            {
                get { return this.primitiveKind; }
            }

            /// <summary>
            /// The kind of this schema element.
            /// </summary>
            public EdmSchemaElementKind SchemaElementKind
            {
                get { return EdmSchemaElementKind.TypeDefinition; }
            }

            /// <summary>
            /// Kind of this type.
            /// </summary>
            public override EdmTypeKind TypeKind
            {
                get { return EdmTypeKind.Primitive; }
            }

            /// <summary>
            /// Creates a new instance of the IEdmPrimitiveType
            /// </summary>
            /// <param name="primitiveKind">Kind of primitive type.</param>
            /// <returns>Returns a new instance of the IEdmPrimitiveType</returns>
            public static IEdmPrimitiveType CreateType(EdmPrimitiveTypeKind primitiveKind)
            {
                Debug.Assert(primitiveKind != EdmPrimitiveTypeKind.None, "primitiveKiind != EdmPrimitiveTypeKind.None");
                return new ClientEdmPrimitiveType(XmlConstants.EdmNamespace, primitiveKind.ToString(), primitiveKind);
            }
        }
    }
}
