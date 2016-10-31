//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if !INTERNAL_DROP
namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
#if ASTORIA_SERVER
    using System.Data.Services;
#endif
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Spatial;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Validation;
#if ASTORIA_SERVER
    using ErrorStrings = System.Data.Services.Strings;
    using PlatformHelper = System.Type;
#endif
#if ASTORIA_CLIENT
    using ErrorStrings = System.Data.Services.Client.Strings;
#endif
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
    using Microsoft.Data.OData.JsonLight;
    using ErrorStrings = Microsoft.Data.OData.Strings;
    using PlatformHelper = Microsoft.Data.OData.PlatformHelper;
#endif
    #endregion Namespaces

    /// <summary>
    /// Class with code that will eventually live in EdmLib.
    /// </summary>
    /// <remarks>This class should go away completely when the EdmLib integration is fully done.</remarks>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The class coupling is due to mapping primitive types, lot of different types there.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Edm", Justification = "Following EdmLib standards.")]
    internal static class EdmLibraryExtensions
    {
        /// <summary>
        /// Map of CLR primitive type to EDM primitive type reference. Doesn't include spatial types since they need assignability and not equality.
        /// </summary>
        private static readonly Dictionary<Type, IEdmPrimitiveTypeReference> PrimitiveTypeReferenceMap = new Dictionary<Type, IEdmPrimitiveTypeReference>(EqualityComparer<Type>.Default);

        /// <summary>Type reference for Edm.Boolean.</summary>
        private static readonly EdmPrimitiveTypeReference BooleanTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), false);

        /// <summary>Type reference for Edm.Byte.</summary>
        private static readonly EdmPrimitiveTypeReference ByteTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), false);

        /// <summary>Type reference for Edm.DateTime.</summary>
        private static readonly EdmPrimitiveTypeReference DateTimeTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTime), false);

        /// <summary>Type reference for Edm.Decimal.</summary>
        private static readonly EdmPrimitiveTypeReference DecimalTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), false);
        
        /// <summary>Type reference for Edm.Double.</summary>
        private static readonly EdmPrimitiveTypeReference DoubleTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), false);

        /// <summary>Type reference for Edm.Int16.</summary>
        private static readonly EdmPrimitiveTypeReference Int16TypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), false);

        /// <summary>Type reference for Edm.Int32.</summary>
        private static readonly EdmPrimitiveTypeReference Int32TypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), false);

        /// <summary>Type reference for Edm.Int64.</summary>
        private static readonly EdmPrimitiveTypeReference Int64TypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), false);

        /// <summary>Type reference for Edm.SByte.</summary>
        private static readonly EdmPrimitiveTypeReference SByteTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), false);

        /// <summary>Type reference for Edm.String.</summary>
        private static readonly EdmPrimitiveTypeReference StringTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String), true);

        /// <summary>Type reference for Edm.Float.</summary>
        private static readonly EdmPrimitiveTypeReference SingleTypeReference = ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), false);

        #region Edm Collection constants

        /// <summary>The qualifier to turn a type name into a Collection type name.</summary>
        private const string CollectionTypeQualifier = "Collection";

        /// <summary>Format string to describe a Collection of a given type.</summary>
        private const string CollectionTypeFormat = CollectionTypeQualifier + "({0})";

        #endregion Edm Collection constants

        /// <summary>
        /// Constructor.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Need to use the static constructor for the phone platform.")]
        static EdmLibraryExtensions()
        {
            PrimitiveTypeReferenceMap.Add(typeof(Boolean), BooleanTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Byte), ByteTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(DateTime), DateTimeTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Decimal), DecimalTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Double), DoubleTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Int16), Int16TypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Int32), Int32TypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Int64), Int64TypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(SByte), SByteTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(String), StringTypeReference);
            PrimitiveTypeReferenceMap.Add(typeof(Single), SingleTypeReference);

            PrimitiveTypeReferenceMap.Add(typeof(DateTimeOffset), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), false));
            PrimitiveTypeReferenceMap.Add(typeof(Guid), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), false));
            PrimitiveTypeReferenceMap.Add(typeof(TimeSpan), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Time), false));
            PrimitiveTypeReferenceMap.Add(typeof(byte[]), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary), true));
            PrimitiveTypeReferenceMap.Add(typeof(Stream), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream), false));

            PrimitiveTypeReferenceMap.Add(typeof(Boolean?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean), true));
            PrimitiveTypeReferenceMap.Add(typeof(Byte?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte), true));
            PrimitiveTypeReferenceMap.Add(typeof(DateTime?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTime), true));
            PrimitiveTypeReferenceMap.Add(typeof(DateTimeOffset?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset), true));
            
            PrimitiveTypeReferenceMap.Add(typeof(Decimal?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal), true));
            PrimitiveTypeReferenceMap.Add(typeof(Double?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double), true));
            PrimitiveTypeReferenceMap.Add(typeof(Int16?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16), true));
            PrimitiveTypeReferenceMap.Add(typeof(Int32?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), true));
            PrimitiveTypeReferenceMap.Add(typeof(Int64?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64), true));
            PrimitiveTypeReferenceMap.Add(typeof(SByte?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte), true));
            PrimitiveTypeReferenceMap.Add(typeof(Single?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single), true));
            PrimitiveTypeReferenceMap.Add(typeof(Guid?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid), true));
            PrimitiveTypeReferenceMap.Add(typeof(TimeSpan?), ToTypeReference(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Time), true));
        }
        
        #region Internal methods
        #region ODataLib only
#if !ODATALIB_QUERY && !ASTORIA_SERVER && !ASTORIA_CLIENT
        /// <summary>
        /// A method that determines whether a given model is a user model or one of the built-in core models
        /// that can only used for primitive type resolution.
        /// </summary>
        /// <param name="model">The model to check.</param>
        /// <returns>true if the <paramref name="model"/> is a user model; otherwise false.</returns>
        internal static bool IsUserModel(this IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");

            return !(model is EdmCoreModel);
        }

        /// <summary>
        /// Checks whether the provided <paramref name="clrType"/> is a supported primitive type.
        /// </summary>
        /// <param name="clrType">The CLR type to check.</param>
        /// <returns>true if the <paramref name="clrType"/> is a supported primitive type; otherwise false.</returns>
        internal static bool IsPrimitiveType(Type clrType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(clrType != null, "clrType != null");

            // PERF
            switch (PlatformHelper.GetTypeCode(clrType))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.String:
                case TypeCode.Single:
                    return true;

                default:
                    return PrimitiveTypeReferenceMap.ContainsKey(clrType) || typeof(ISpatial).IsAssignableFrom(clrType);
            }
        }

        /// <summary>
        /// Returns all the entity types in a model.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to get the entity types for (must not be null).</param>
        /// <returns>An enumerable of all <see cref="IEdmEntityType"/> instances in the <paramref name="model"/>.</returns>
        internal static IEnumerable<IEdmEntityType> EntityTypes(this IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");

            IEnumerable<IEdmSchemaElement> schemaElements = model.SchemaElements;
            if (schemaElements != null)
            {
                return schemaElements.OfType<IEdmEntityType>();
            }

            return null;
        }

        /// <summary>
        /// Creates a collection value type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmPrimitiveTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for primitive type references only.")]
        internal static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmPrimitiveTypeReference itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Creates a collection type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmComplexTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for complex type references only.")]
        internal static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmComplexTypeReference itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> type reference is assignable from the <paramref name="subtype"/> type reference.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        /// <remarks>Note that this method only checks the type definition for assignability; it does not consider nullability
        /// or any other facets of the type reference.</remarks>
        internal static bool IsAssignableFrom(this IEdmTypeReference baseType, IEdmTypeReference subtype)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            // We only consider the type definition but no facets (incl. nullability) here.
            return baseType.Definition.IsAssignableFrom(subtype.Definition);
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> type is assignable from the <paramref name="subtype"/> type.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        internal static bool IsAssignableFrom(this IEdmType baseType, IEdmType subtype)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            EdmTypeKind baseTypeKind = baseType.TypeKind;
            EdmTypeKind subTypeKind = subtype.TypeKind;

            // If the type kinds don't match, the types are not assignable.
            if (baseTypeKind != subTypeKind)
            {
                return false;
            }

            switch (baseTypeKind)
            {
                case EdmTypeKind.Primitive:
                    return ((IEdmPrimitiveType)baseType).IsAssignableFrom((IEdmPrimitiveType)subtype);

                case EdmTypeKind.Entity:    // fall through
                case EdmTypeKind.Complex:
                    return ((IEdmStructuredType)baseType).IsAssignableFrom((IEdmStructuredType)subtype);

                case EdmTypeKind.Collection:
                    // NOTE: we do allow inheritance (or co-/contra-variance) in collection types.
                    return ((IEdmCollectionType)baseType).ElementType.Definition.IsAssignableFrom(((IEdmCollectionType)subtype).ElementType.Definition);

                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_IsAssignableFrom_Type));
            }
        }

        /// <summary>
        /// Checks if the <paramref name="firstType"/> structured type and the <paramref name="secondType"/> structured type
        /// have a common base type.
        /// In other words, if <paramref name="secondType"/> is a subtype of <paramref name="firstType"/> or not.
        /// </summary>
        /// <param name="firstType">Type of the base type.</param>
        /// <param name="secondType">Type of the sub type.</param>
        /// <returns>The common base type or null if no common base type exists.</returns>
        internal static IEdmStructuredType GetCommonBaseType(this IEdmStructuredType firstType, IEdmStructuredType secondType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(firstType != null, "firstType != null");
            Debug.Assert(secondType != null, "secondType != null");

            if (firstType.IsEquivalentTo(secondType))
            {
                return firstType;
            }

            IEdmStructuredType commonBaseType = firstType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(secondType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType;
            }

            commonBaseType = secondType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(firstType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Checks if the <paramref name="firstType"/> primitive type and the <paramref name="secondType"/> primitive type
        /// have a common base type.
        /// In other words, if <paramref name="secondType"/> is a subtype of <paramref name="firstType"/> or not.
        /// </summary>
        /// <param name="firstType">Type of the base type.</param>
        /// <param name="secondType">Type of the sub type.</param>
        /// <returns>The common base type or null if no common base type exists.</returns>
        internal static IEdmPrimitiveType GetCommonBaseType(this IEdmPrimitiveType firstType, IEdmPrimitiveType secondType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(firstType != null, "firstType != null");
            Debug.Assert(secondType != null, "secondType != null");

            if (firstType.IsEquivalentTo(secondType))
            {
                return firstType;
            }

            IEdmPrimitiveType commonBaseType = firstType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(secondType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType();
            }

            commonBaseType = secondType;
            while (commonBaseType != null)
            {
                if (commonBaseType.IsAssignableFrom(firstType))
                {
                    return commonBaseType;
                }

                commonBaseType = commonBaseType.BaseType();
            }

            return null;
        }

        /// <summary>
        /// Returns the base type of a primitive type.
        /// </summary>
        /// <param name="type">The <see cref="IEdmPrimitiveType"/> to get the base type for.</param>
        /// <returns>The base type of the <paramref name="type"/> or null if no base type exists.</returns>
        internal static IEdmPrimitiveType BaseType(this IEdmPrimitiveType type)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");

            switch (type.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.None:
                case EdmPrimitiveTypeKind.Binary:
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Decimal:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Time:
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.Geometry:
                    return null;

                case EdmPrimitiveTypeKind.GeographyPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyCollection:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);

                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);

                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);

                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);

                case EdmPrimitiveTypeKind.GeometryPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryCollection:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);

                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);

                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);

                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);

                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_BaseType));
            }
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmComplexTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmComplexTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmComplexTypeReference AsComplexOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            return typeReference.TypeKind() == EdmTypeKind.Complex ? typeReference.AsComplex() : null;
        }

        /// <summary>
        /// Casts an <see cref="IEdmTypeReference"/> to a <see cref="IEdmCollectionTypeReference"/> or returns null if this is not supported.
        /// </summary>
        /// <param name="typeReference">The type reference to convert.</param>
        /// <returns>An <see cref="IEdmCollectionTypeReference"/> instance or null if the <paramref name="typeReference"/> cannot be converted.</returns>
        internal static IEdmCollectionTypeReference AsCollectionOrNull(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            if (typeReference.TypeKind() != EdmTypeKind.Collection)
            {
                return null;
            }

            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollection();
            if (!collectionTypeReference.IsNonEntityCollectionType())
            {
                return null;
            }

            return collectionTypeReference;
        }

        /// <summary>
        /// Resolves the name of a primitive type.
        /// </summary>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmSchemaType ResolvePrimitiveTypeName(string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            return EdmCoreModel.Instance.FindDeclaredType(typeName);
        }

        /// <summary>
        /// Get the <see cref="IEdmEntityTypeReference"/> of the item type of the <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The collection type to get the item type for.</param>
        /// <returns>The item type of the <paramref name="typeReference"/>.</returns>
        internal static IEdmTypeReference GetCollectionItemType(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmCollectionTypeReference collectionType = typeReference.AsCollectionOrNull();
            return collectionType == null ? null : collectionType.ElementType();
        }

        /// <summary>
        /// Returns the IEdmCollectionType implementation with the given IEdmType as element type.
        /// </summary>
        /// <param name="itemType">IEdmType instance which is the element type.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance using the <paramref name="itemType"/> as Collection item type.</returns>
        internal static IEdmCollectionType GetCollectionType(IEdmType itemType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(itemType != null, "itemType != null");

            IEdmTypeReference itemTypeReference = itemType.ToTypeReference();
            return GetCollectionType(itemTypeReference);
        }

        /// <summary>
        /// Returns the IEdmCollectionType implementation with the given IEdmTypeReference as element type.
        /// </summary>
        /// <param name="itemTypeReference">IEdmTypeReference instance which is the element type.</param>
        /// <returns>An <see cref="IEdmCollectionType"/> instance using the <paramref name="itemTypeReference"/> as Collection item type.</returns>
        internal static IEdmCollectionType GetCollectionType(IEdmTypeReference itemTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(itemTypeReference != null, "itemTypeReference != null");

            if (!itemTypeReference.IsODataPrimitiveTypeKind() && !itemTypeReference.IsODataComplexTypeKind())
            {
                throw new ODataException(ErrorStrings.EdmLibraryExtensions_CollectionItemCanBeOnlyPrimitiveOrComplex);
            }

            return new EdmCollectionType(itemTypeReference);
        }

        /// <summary>
        /// Checks whether a type reference is a Geography type.
        /// </summary>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is a Geography type; otherwise false.</returns>
        internal static bool IsGeographyType(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
            if (primitiveTypeReference == null)
            {
                return false;
            }

            switch (primitiveTypeReference.PrimitiveKind())
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
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether a type reference is a Geometry type.
        /// </summary>
        /// <param name="typeReference">The <see cref="IEdmTypeReference"/> to check.</param>
        /// <returns>true if the <paramref name="typeReference"/> is a Geometry type; otherwise false.</returns>
        internal static bool IsGeometryType(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
            if (primitiveTypeReference == null)
            {
                return false;
            }

            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns CollectionValue item type name or null if the provided type name is not a collectionValue.
        /// </summary>
        /// <param name="typeName">CollectionValue type name read from payload.</param>
        /// <returns>CollectionValue element type name or null if not a collectionValue.</returns>
        internal static string GetCollectionItemTypeName(string typeName)
        {
            DebugUtils.CheckNoExternalCallers();

            return GetCollectionItemTypeName(typeName, false);
        }
        
        /// <summary>
        /// Gets the name of a function import group.
        /// </summary>
        /// <param name="functionImportGroup">The function import group in question.</param>
        /// <returns>The name of the function import group.</returns>
        internal static string FunctionImportGroupName(this IEnumerable<IEdmFunctionImport> functionImportGroup)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functionImportGroup != null && functionImportGroup.Any(), "functionImportGroup != null && functionImportGroup.Any()");

            string name = functionImportGroup.First().Name;
            Debug.Assert(functionImportGroup.All(f => f.Name == name), "functionImportGroup.All(f => f.Name == name)");
            return name;
        }

        /// <summary>
        /// Gets the full name of a function import group.
        /// </summary>
        /// <param name="functionImportGroup">The function import group in question.</param>
        /// <returns>The full name of the function import group.</returns>
        internal static string FunctionImportGroupFullName(this IEnumerable<IEdmFunctionImport> functionImportGroup)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functionImportGroup != null && functionImportGroup.Any(), "functionImportGroup != null && functionImportGroup.Any()");

            string fullName = functionImportGroup.First().FullName();
            Debug.Assert(functionImportGroup.All(f => f.FullName() == fullName), "functionImportGroup.All(f => f.FullName() == fullName)");
            return fullName;
        }

        /// <summary>
        /// Name of the function import with parameters.
        /// </summary>
        /// <param name="functionImport">Function import in question.</param>
        /// <returns>Name of the function import with parameters.</returns>
        internal static string NameWithParameters(this IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functionImport != null, "functionImport != null");

            return functionImport.Name + functionImport.ParameterTypesToString();
        }

        /// <summary>
        /// Full name of the function import with parameters.
        /// </summary>
        /// <param name="functionImport">Function import in question.</param>
        /// <returns>Full name of the function import with parameters.</returns>
        internal static string FullNameWithParameters(this IEdmFunctionImport functionImport)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(functionImport != null, "functionImport != null");

            return functionImport.FullName() + functionImport.ParameterTypesToString();
        }

        /// <summary>
        /// Determines whether operations bound to this type must be qualified with the operation they belong to when appearing in a $select clause.
        /// </summary>
        /// <param name="entityType">The entity type the operations are bound to.</param>
        /// <returns>True if the operations must be container qualified, otherwise false.</returns>
        internal static bool OperationsBoundToEntityTypeMustBeContainerQualified(this IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityType != null, "entityType != null");
            return entityType.IsOpen;
        }
#endif
        #endregion

        #region ODataLib and WCF DS Server
#if !ODATALIB_QUERY && !ASTORIA_CLIENT
        /// <summary>
        /// Gets the full name of the definition referred to by the type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to get the full name for.</param>
        /// <returns>The full name of this <paramref name="typeReference"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib FullName extension method in that it also returns
        /// names for collection types. For EdmLib, collection types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataFullName(this IEdmTypeReference typeReference)
        {
#if !ASTORIA_SERVER
            DebugUtils.CheckNoExternalCallers();
#endif
            Debug.Assert(typeReference != null, "typeReference != null");
            Debug.Assert(typeReference.Definition != null, "typeReference.Definition != null");
            return typeReference.Definition.ODataFullName();
        }

        /// <summary>
        /// Gets the full name of the type.
        /// </summary>
        /// <param name="type">The type to get the full name for.</param>
        /// <returns>The full name of the <paramref name="type"/>.</returns>
        /// <remarks>
        /// Note that this method is different from the EdmLib FullName extension method in that it also returns
        /// names for collection types. For EdmLib, collection types are functions and thus don't have a full name.
        /// The name/string they use in CSDL is just shorthand for them.
        /// </remarks>
        internal static string ODataFullName(this IEdmType type)
        {
#if !ASTORIA_SERVER
            DebugUtils.CheckNoExternalCallers();
#endif
            Debug.Assert(type != null, "type != null");

            // Handle collection type names here since for EdmLib collection values are functions
            // that do not have a full name
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType != null)
            {
                string elementTypeName = collectionType.ElementType.ODataFullName();
                if (elementTypeName == null)
                {
                    return null;
                }

                return GetCollectionTypeName(elementTypeName);
            }

            var namedDefinition = type as IEdmSchemaElement;
            return namedDefinition != null ? namedDefinition.FullName() : null;
        }

        /// <summary>
        /// Clones the specified type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to clone.</param>
        /// <param name="nullable">true to make the cloned type reference nullable; false to make it non-nullable.</param>
        /// <returns>The cloned <see cref="IEdmTypeReference"/> instance.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The clone logic should stay in one place.")]
        internal static IEdmTypeReference Clone(this IEdmTypeReference typeReference, bool nullable)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeReference == null)
            {
                return null;
            }

            EdmTypeKind typeKind = typeReference.TypeKind();
            switch (typeKind)
            {
                case EdmTypeKind.Primitive:
                    EdmPrimitiveTypeKind kind = typeReference.PrimitiveKind();
                    IEdmPrimitiveType primitiveType = (IEdmPrimitiveType)typeReference.Definition;
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
                            return new EdmPrimitiveTypeReference(primitiveType, nullable);
                        case EdmPrimitiveTypeKind.Binary:
                            IEdmBinaryTypeReference binaryTypeReference = (IEdmBinaryTypeReference)typeReference;
                            return new EdmBinaryTypeReference(
                                primitiveType,
                                nullable,
                                binaryTypeReference.IsUnbounded,
                                binaryTypeReference.MaxLength,
                                binaryTypeReference.IsFixedLength);

                        case EdmPrimitiveTypeKind.String:
                            IEdmStringTypeReference stringTypeReference = (IEdmStringTypeReference)typeReference;
                            return new EdmStringTypeReference(
                                primitiveType,
                                nullable,
                                stringTypeReference.IsUnbounded,
                                stringTypeReference.MaxLength,
                                stringTypeReference.IsFixedLength,
                                stringTypeReference.IsUnicode,
                                stringTypeReference.Collation);

                        case EdmPrimitiveTypeKind.Decimal:
                            IEdmDecimalTypeReference decimalTypeReference = (IEdmDecimalTypeReference)typeReference;
                            return new EdmDecimalTypeReference(primitiveType, nullable, decimalTypeReference.Precision, decimalTypeReference.Scale);

                        case EdmPrimitiveTypeKind.DateTime:
                        case EdmPrimitiveTypeKind.DateTimeOffset:
                        case EdmPrimitiveTypeKind.Time:
                            IEdmTemporalTypeReference temporalTypeReference = (IEdmTemporalTypeReference)typeReference;
                            return new EdmTemporalTypeReference(primitiveType, nullable, temporalTypeReference.Precision);

                        case EdmPrimitiveTypeKind.Geography:
                        case EdmPrimitiveTypeKind.GeographyPoint:
                        case EdmPrimitiveTypeKind.GeographyLineString:
                        case EdmPrimitiveTypeKind.GeographyPolygon:
                        case EdmPrimitiveTypeKind.GeographyCollection:
                        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                        case EdmPrimitiveTypeKind.GeographyMultiLineString:
                        case EdmPrimitiveTypeKind.GeographyMultiPoint:
                        case EdmPrimitiveTypeKind.Geometry:
                        case EdmPrimitiveTypeKind.GeometryCollection:
                        case EdmPrimitiveTypeKind.GeometryPoint:
                        case EdmPrimitiveTypeKind.GeometryLineString:
                        case EdmPrimitiveTypeKind.GeometryPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                        case EdmPrimitiveTypeKind.GeometryMultiLineString:
                        case EdmPrimitiveTypeKind.GeometryMultiPoint:
                            IEdmSpatialTypeReference spatialTypeReference = (IEdmSpatialTypeReference)typeReference;
                            return new EdmSpatialTypeReference(primitiveType, nullable, spatialTypeReference.SpatialReferenceIdentifier);

                        default:
                            throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_Clone_PrimitiveTypeKind));
                    }

                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)typeReference.Definition, nullable);

                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)typeReference.Definition, nullable);

                case EdmTypeKind.Row:
                    return new EdmRowTypeReference((IEdmRowType)typeReference.Definition, nullable);

                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference((IEdmCollectionType)typeReference.Definition, nullable);

                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)typeReference.Definition, nullable);

                case EdmTypeKind.Enum:
                    return new EdmEnumTypeReference((IEdmEnumType)typeReference.Definition, nullable);

                case EdmTypeKind.None:  // fall through
                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_Clone_TypeKind));
            }
        }
#endif
        #endregion

        #region ODataLib Contrib only (stuff that is not yet released)
#if !ASTORIA_SERVER && !ASTORIA_CLIENT

        /// <summary>
        /// Gets the multiplicity of a navigation property.
        /// </summary>
        /// <param name="property">Reference to the calling object.</param>
        /// <returns>The multiplicity of the navigation property in question.</returns>
        /// <remarks>This has been added to EdmLib, but EdmLib won't be released for a while.
        /// If you need to use this functionality before we release EdmLib, then use this method. Change your calls
        /// to use the real method whenever we release EdmLib again.</remarks>
        internal static EdmMultiplicity TargetMultiplicityTemporary(this IEdmNavigationProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(property != null, "property != null");

            IEdmTypeReference type = property.Type;
            if (type.IsCollection())
            {
                return EdmMultiplicity.Many;
            }

            return type.IsNullable ? EdmMultiplicity.ZeroOrOne : EdmMultiplicity.One;
        }

#endif
        #endregion

        #region ODataLib and Query project
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
        /// <summary>
        /// Checks if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Extension method for structured types only.")]
        internal static bool IsAssignableFrom(this IEdmStructuredType baseType, IEdmStructuredType subtype)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            if (baseType.TypeKind != subtype.TypeKind)
            {
                return false;
            }

            if (!baseType.IsODataEntityTypeKind() && !baseType.IsODataComplexTypeKind())
            {
                // we only support complex and entity type inheritance.
                return false;
            }

            IEdmStructuredType structuredSubType = subtype;
            while (structuredSubType != null)
            {
                if (structuredSubType.IsEquivalentTo(baseType))
                {
                    return true;
                }

                structuredSubType = structuredSubType.BaseType;
            }

            return false;
        }
        
        /// <summary>
        /// Checks if the primitive type is a geography or geometry type.
        /// </summary>
        /// <param name="primitiveType">The type to check.</param>
        /// <returns>true, if the <paramref name="primitiveType"/> is a geography or geometry type.</returns>
        internal static bool IsSpatialType(this IEdmPrimitiveType primitiveType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(primitiveType != null, "primitiveType != null");

            switch (primitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                case EdmPrimitiveTypeKind.GeographyPoint:
                case EdmPrimitiveTypeKind.GeographyLineString:
                case EdmPrimitiveTypeKind.GeographyPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                case EdmPrimitiveTypeKind.GeographyCollection:
                case EdmPrimitiveTypeKind.Geometry:
                case EdmPrimitiveTypeKind.GeometryPoint:
                case EdmPrimitiveTypeKind.GeometryLineString:
                case EdmPrimitiveTypeKind.GeometryPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return true;

                case EdmPrimitiveTypeKind.None:
                case EdmPrimitiveTypeKind.Binary:
                case EdmPrimitiveTypeKind.Boolean:
                case EdmPrimitiveTypeKind.Byte:
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Decimal:
                case EdmPrimitiveTypeKind.Double:
                case EdmPrimitiveTypeKind.Guid:
                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                case EdmPrimitiveTypeKind.Int64:
                case EdmPrimitiveTypeKind.SByte:
                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.String:
                case EdmPrimitiveTypeKind.Stream:
                case EdmPrimitiveTypeKind.Time:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the <paramref name="baseType"/> primitive type is assignable to <paramref name="subtype"/> primitive type.
        /// In other words, if <paramref name="subtype"/> is a subtype of <paramref name="baseType"/> or not.
        /// </summary>
        /// <param name="baseType">Type of the base type.</param>
        /// <param name="subtype">Type of the sub type.</param>
        /// <returns>true, if the <paramref name="baseType"/> is assignable to <paramref name="subtype"/>. Otherwise returns false.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Need to keep code together.")]
        internal static bool IsAssignableFrom(this IEdmPrimitiveType baseType, IEdmPrimitiveType subtype)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseType != null, "baseType != null");
            Debug.Assert(subtype != null, "subtype != null");

            if (baseType.IsEquivalentTo(subtype))
            {
                return true;
            }

            // Only spatial types are assignable
            if (!baseType.IsSpatialType() || !subtype.IsSpatialType())
            {
                return false;
            }

            // For two spatial types, test for assignability
            EdmPrimitiveTypeKind baseTypeKind = baseType.PrimitiveKind;
            EdmPrimitiveTypeKind subTypeKind = subtype.PrimitiveKind;

            switch (baseTypeKind)
            {
                case EdmPrimitiveTypeKind.Geography:
                    return subTypeKind == EdmPrimitiveTypeKind.Geography ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPolygon ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyPolygon;

                case EdmPrimitiveTypeKind.GeographyPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyPoint;

                case EdmPrimitiveTypeKind.GeographyLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyLineString;

                case EdmPrimitiveTypeKind.GeographyPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyPolygon;

                case EdmPrimitiveTypeKind.GeographyCollection:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPolygon;

                case EdmPrimitiveTypeKind.GeographyMultiPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPolygon;

                case EdmPrimitiveTypeKind.GeographyMultiLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyMultiLineString;

                case EdmPrimitiveTypeKind.GeographyMultiPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeographyMultiPoint;

                case EdmPrimitiveTypeKind.Geometry:
                    return subTypeKind == EdmPrimitiveTypeKind.Geometry ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPolygon ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryPolygon;

                case EdmPrimitiveTypeKind.GeometryPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryPoint;

                case EdmPrimitiveTypeKind.GeometryLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryLineString;

                case EdmPrimitiveTypeKind.GeometryPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryPolygon;

                case EdmPrimitiveTypeKind.GeometryCollection:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryCollection ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiLineString ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPoint ||
                        subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPolygon;

                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPolygon;

                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryMultiLineString;

                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                    return subTypeKind == EdmPrimitiveTypeKind.GeometryMultiPoint;

                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_IsAssignableFrom_Primitive));
            }
        }

        /// <summary>
        /// Returns the primitive CLR type for the specified primitive type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type to resolve.</param>
        /// <returns>The CLR type for the primitive type reference.</returns>
        internal static Type GetPrimitiveClrType(IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            return GetPrimitiveClrType(primitiveTypeReference.PrimitiveDefinition(), primitiveTypeReference.IsNullable); 
        }

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding non-nullable <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>A non-nullable type reference for the <paramref name="type"/>.</returns>
        internal static IEdmTypeReference ToTypeReference(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            return ToTypeReference(type, false /*nullable*/);
        }

        /// <summary>
        /// Determines whether the provided <paramref name="type"/> is an open type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type"/> is an open type; otherwise false.</returns>
        internal static bool IsOpenType(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");

            IEdmStructuredType structuredType = type as IEdmStructuredType;
            if (structuredType != null)
            {
                return structuredType.IsOpen;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the provided <paramref name="type"/> is a stream.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>true if the <paramref name="type"/> represents a stream; otherwise false.</returns>
        internal static bool IsStream(this IEdmType type)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");

            IEdmPrimitiveType primitiveType = type as IEdmPrimitiveType;
            if (primitiveType == null)
            {
                Debug.Assert(type.TypeKind != EdmTypeKind.Primitive, "Invalid type kind.");
                return false;
            }

            Debug.Assert(primitiveType.TypeKind == EdmTypeKind.Primitive, "Expected primitive type kind.");
            return primitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Stream;
        }

#if !ODATALIB_QUERY || DEBUG
        /// <summary>
        /// Checks whether the specified <paramref name="property"/> is defined for the type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to check the properties on.</param>
        /// <param name="property">The property to check for.</param>
        /// <returns>true if the <paramref name="property"/> is defined for the <paramref name="type"/>; otherwise false.</returns>
        internal static bool ContainsProperty(this IEdmType type, IEdmProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "type != null");
            Debug.Assert(property != null, "property != null");

            IEdmComplexType complexType = type as IEdmComplexType;
            if (complexType != null)
            {
                // NOTE: using Any() instead of Contains() since Contains() does not exist on all platforms
                return complexType.Properties().Any(p => p == property);
            }

            IEdmEntityType entityType = type as IEdmEntityType;
            if (entityType != null)
            {
                // NOTE: using Any() instead of Contains() since Contains() does not exist on all platforms
                return entityType.Properties().Any(p => p == property) ||
                       entityType.NavigationProperties().Any(p => p == property);
            }

            // we only support complex and entity types with properties so far
            return false;
        }

        /// <summary>
        /// Checks whether the specified <paramref name="property"/> is defined for the type <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The type to check the properties on.</param>
        /// <param name="property">The property to check for.</param>
        /// <returns>true if the <paramref name="property"/> is defined for the <paramref name="typeReference"/>; otherwise false.</returns>
        internal static bool ContainsProperty(this IEdmTypeReference typeReference, IEdmProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(typeReference != null, "typeReference != null");
            Debug.Assert(property != null, "property != null");

            IEdmStructuredTypeReference structuredTypeReference = typeReference.AsStructuredOrNull();
            if (structuredTypeReference == null)
            {
                return false;
            }

            return ContainsProperty(structuredTypeReference.Definition, property);
        }
#endif
#endif
        #endregion

        #region Everyone
        /// <summary>
        /// Returns the fully qualified name of an entity container element.
        /// </summary>
        /// <param name="containerElement">The container element to get the full name for.</param>
        /// <returns>The full name of the owning entity container, slash, name of the container element.</returns>
#if ASTORIA_CLIENT || ODATALIB_QUERY
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Will be used in a later change")]
#endif
        internal static string FullName(this IEdmEntityContainerElement containerElement)
        {
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
            DebugUtils.CheckNoExternalCallers();
#endif
            Debug.Assert(containerElement != null, "containerElement != null");

            return containerElement.Container.Name + "." + containerElement.Name;
        }

#if !ASTORIA_CLIENT
        /// <summary>
        /// Returns the primitive type reference for the given Clr type.
        /// </summary>
        /// <param name="clrType">The Clr type to resolve.</param>
        /// <returns>The primitive type reference for the given Clr type.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "cyclomatic complexity")]
        internal static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(Type clrType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(clrType != null, "clrType != null");

            TypeCode typeCode = PlatformHelper.GetTypeCode(clrType);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return BooleanTypeReference;

                case TypeCode.Byte:
                    return ByteTypeReference;

                case TypeCode.DateTime:
                    return DateTimeTypeReference;

                case TypeCode.Decimal:
                    return DecimalTypeReference;

                case TypeCode.Double:
                    return DoubleTypeReference;

                case TypeCode.Int16:
                    return Int16TypeReference;

                case TypeCode.Int32:
                    return Int32TypeReference;

                case TypeCode.Int64:
                    return Int64TypeReference;

                case TypeCode.SByte:
                    return SByteTypeReference;

                case TypeCode.String:
                    return StringTypeReference;

                case TypeCode.Single:
                    return SingleTypeReference;
            }

            // Try to lookup the type in our map.
            IEdmPrimitiveTypeReference primitiveTypeReference;
            if (PrimitiveTypeReferenceMap.TryGetValue(clrType, out primitiveTypeReference))
            {
                return primitiveTypeReference;
            }

            // If it didn't work, try spatial types which need assignability.
            IEdmPrimitiveType primitiveType = null;
            if (typeof(GeographyPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint);
            }
            else if (typeof(GeographyLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString);
            }
            else if (typeof(GeographyPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon);
            }
            else if (typeof(GeographyMultiPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint);
            }
            else if (typeof(GeographyMultiLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString);
            }
            else if (typeof(GeographyMultiPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon);
            }
            else if (typeof(GeographyCollection).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
            }
            else if (typeof(Geography).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
            }
            else if (typeof(GeometryPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint);
            }
            else if (typeof(GeometryLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString);
            }
            else if (typeof(GeometryPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon);
            }
            else if (typeof(GeometryMultiPoint).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint);
            }
            else if (typeof(GeometryMultiLineString).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString);
            }
            else if (typeof(GeometryMultiPolygon).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon);
            }
            else if (typeof(GeometryCollection).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
            }
            else if (typeof(Geometry).IsAssignableFrom(clrType))
            {
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
            }

            if (primitiveType == null)
            {
                return null;
            }

            // All spatial CLR types are inherently nullable
            return ToTypeReference(primitiveType, true);
        }
#endif

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <param name="nullable">true if the returned type reference should be nullable; otherwise false.</param>
        /// <returns>A type reference for the <paramref name="type"/>.</returns>
        internal static IEdmTypeReference ToTypeReference(this IEdmType type, bool nullable)
        {
#if !ASTORIA_CLIENT
            DebugUtils.CheckNoExternalCallers();
#endif

            if (type == null)
            {
                return null;
            }

            switch (type.TypeKind)
            {
                case EdmTypeKind.Primitive:
                    return ToTypeReference((IEdmPrimitiveType)type, nullable);
                case EdmTypeKind.Complex:
                    return new EdmComplexTypeReference((IEdmComplexType)type, nullable);
                case EdmTypeKind.Entity:
                    return new EdmEntityTypeReference((IEdmEntityType)type, nullable);
                case EdmTypeKind.Collection:
                    return new EdmCollectionTypeReference((IEdmCollectionType)type, nullable);
                case EdmTypeKind.Row:
                    return new EdmRowTypeReference((IEdmRowType)type, nullable);
                case EdmTypeKind.EntityReference:
                    return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)type, nullable);
                case EdmTypeKind.None:
                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_ToTypeReference));
            }
        }
        
        /// <summary>
        /// Creates the EDM type name for a collection of the specified item type name. E.g. Collection(Edm.String)
        /// </summary>
        /// <param name="itemTypeName">Type name of the items in the collection.</param>
        /// <returns>Type name for a collection of the specified item type name.</returns>
        internal static string GetCollectionTypeName(string itemTypeName)
        {
#if !ASTORIA_SERVER && !ASTORIA_CLIENT
            DebugUtils.CheckNoExternalCallers();
#endif
            return string.Format(CultureInfo.InvariantCulture, CollectionTypeFormat, itemTypeName);
        }

        /// <summary>
        /// Finds the entity set with the specified container and name.
        /// </summary>
        /// <param name="model">The model to find the entity set in.</param>
        /// <param name="containerQualifiedEntitySetName">The container qualified name of the entity set.</param>
        /// <returns>The <see cref="IEdmEntitySet"/> with the specified name or null if no such entity set exists.</returns>
        internal static IEdmEntitySet ResolveEntitySet(this IEdmModel model, string containerQualifiedEntitySetName)
        {
#if !ASTORIA_CLIENT
            DebugUtils.CheckNoExternalCallers();
#endif

            return ResolveContainerQualifiedElementName(
                model,
                containerQualifiedEntitySetName,
                (container, entitySetName) =>
                {
                    IEdmEntitySet entitySet = container.FindEntitySet(entitySetName);
                    return entitySet == null ? Enumerable.Empty<IEdmEntityContainerElement>() : new[] { entitySet };
                })
                .Cast<IEdmEntitySet>().SingleOrDefault();
        }

#if !ASTORIA_CLIENT
        /// <summary>
        /// Finds the function import group with the specified container and name. If the name contains the function parameters, this
        /// method will return the function import with matching parameters.
        /// </summary>
        /// <param name="model">The model to find the function import in.</param>
        /// <param name="containerQualifiedFunctionImportName">The container qualified name of the function import.</param>
        /// <returns>The <see cref="IEdmFunctionImport"/> group with the specified name or null if no such function import exists.</returns>
        internal static IEnumerable<IEdmFunctionImport> ResolveFunctionImports(this IEdmModel model, string containerQualifiedFunctionImportName)
        {
            DebugUtils.CheckNoExternalCallers();
            return ResolveFunctionImports(model, containerQualifiedFunctionImportName, true);
        }

        /// <summary>
        /// Finds the function import group with the specified container and name. If the name contains the function parameters, this
        /// method will return the function import with matching parameters.
        /// </summary>
        /// <param name="model">The model to find the function import in.</param>
        /// <param name="containerQualifiedFunctionImportName">The container qualified name of the function import. May contain parameter type names, e.g. Function1(P1Type,P2Type) only if <paramref name="allowParameterTypeNames"/> is true.</param>
        /// <param name="allowParameterTypeNames">Whether parameter type names are allowed to appear in the function import name to resolve.</param>
        /// <returns>The <see cref="IEdmFunctionImport"/> group with the specified name or null if no such function import exists.</returns>
        internal static IEnumerable<IEdmFunctionImport> ResolveFunctionImports(this IEdmModel model, string containerQualifiedFunctionImportName, bool allowParameterTypeNames)
        {
            DebugUtils.CheckNoExternalCallers();

            return ResolveContainerQualifiedElementName(
                model,
                containerQualifiedFunctionImportName,
                /*Silverlight and pre-DotNet40 doesn't support covariance and contravariance. We need to explicitly cast for those platforms to compile.*/
                (container, functionImportName) => ResolveFunctionImports(container, functionImportName, allowParameterTypeNames).Cast<IEdmEntityContainerElement>())
                .Cast<IEdmFunctionImport>();
        }

        /// <summary>
        /// Resolves a function import or function import group.
        /// </summary>
        /// <param name="container">The entity container.</param>
        /// <param name="functionImportName">The function import name to resolve. May contain parameter type names, e.g. Function1(P1Type,P2Type)</param>
        /// <returns>The resolved function import or function import group.</returns>
        internal static IEnumerable<IEdmFunctionImport> ResolveFunctionImports(this IEdmEntityContainer container, string functionImportName)
        {
            DebugUtils.CheckNoExternalCallers();
            return ResolveFunctionImports(container, functionImportName, true);
        }

        /// <summary>
        /// Resolves a function import or function import group.
        /// </summary>
        /// <param name="container">The entity container.</param>
        /// <param name="functionImportName">The function import name to resolve. May contain parameter type names, e.g. Function1(P1Type,P2Type) only if <paramref name="allowParameterTypeNames"/> is true.</param>
        /// <param name="allowParameterTypeNames">Whether parameter type names are allowed to appear in the function import name to resolve.</param>
        /// <returns>The resolved function import or function import group.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "allowParameterTypeNames", Justification = "Used in the ODL version of the method.")]
        internal static IEnumerable<IEdmFunctionImport> ResolveFunctionImports(this IEdmEntityContainer container, string functionImportName, bool allowParameterTypeNames)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(container != null, "container != null");

            if (string.IsNullOrEmpty(functionImportName))
            {
                return Enumerable.Empty<IEdmFunctionImport>();
            }

#if !ASTORIA_SERVER && !ASTORIA_CLIENT && !ODATALIB_QUERY
            int indexOfParameterStart = functionImportName.IndexOf(JsonLightConstants.FunctionParameterStart);
            string functionImportNameWithoutParameterTypes;
            if (indexOfParameterStart > 0)
            {
                if (!allowParameterTypeNames)
                {
                    return Enumerable.Empty<IEdmFunctionImport>();
                }

                functionImportNameWithoutParameterTypes = functionImportName.Substring(0, indexOfParameterStart);
            }
            else
            {
                functionImportNameWithoutParameterTypes = functionImportName;
            }
#else
            Debug.Assert(!allowParameterTypeNames, "Parameter type names not supported in the server version of this method.");
            string functionImportNameWithoutParameterTypes = functionImportName;
#endif

            IEnumerable<IEdmFunctionImport> functionImports = container.FindFunctionImports(functionImportNameWithoutParameterTypes);
            Debug.Assert(functionImports != null, "functionImports != null");
#if !ASTORIA_SERVER && !ASTORIA_CLIENT && !ODATALIB_QUERY
            if (indexOfParameterStart > 0)
            {
                return functionImports.Where(f => f.NameWithParameters().Equals(functionImportName, StringComparison.Ordinal));
            }
#endif

            return functionImports;
        }

        /// <summary>
        /// Finds all function imports with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="model">The model to find the function import in.</param>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="functionImportName">The name of the function imports to find. May be qualified with an entity container name.</param>
        /// <returns>The function imports that match the search criteria.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used during URI parsing to resolve operations in either the path or $select. It does not need to use the resolver.")]
        internal static IEnumerable<IEdmFunctionImport> FindFunctionImportsByBindingParameterTypeHierarchy(this IEdmModel model, IEdmEntityType bindingType, string functionImportName)
        {
            DebugUtils.CheckNoExternalCallers();
            return model.ResolveFunctionImports(functionImportName, false /*allowParameterTypeNames*/)
                        .Where(f => f.IsBindable
                                    && f.Parameters.Any()
                                    && f.Parameters.First().Type.Definition.IsOrInheritsFrom(bindingType));
        }

        /// <summary>
        /// Finds all function imports with the given name which are bindable to an instance of the giving binding type or a more derived type.
        /// </summary>
        /// <param name="model">The model to find the function import in.</param>
        /// <param name="bindingType">The binding entity type.</param>
        /// <param name="functionImportName">The name of the function imports to find. May be qualified with an entity container name.</param>
        /// <returns>The function imports that match the search criteria.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used during URI parsing to resolve operations in either the path or $select. It does not need to use the resolver.")]
        internal static IEnumerable<IEdmFunctionImport> FindFunctionImportsBySpecificBindingParameterType(this IEdmModel model, IEdmType bindingType, string functionImportName)
        {
            DebugUtils.CheckNoExternalCallers();

            // TODO: throw on collision.
            if (bindingType != null)
            {
                return model.ResolveFunctionImports(functionImportName, false /*allowParameterTypeNames*/).Where(f => f.IsBindable && f.Parameters.Any() && f.Parameters.First().Type.Definition.ODataFullName() == bindingType.ODataFullName());
            }

            return model.ResolveFunctionImports(functionImportName, false /*allowParameterTypeNames*/).Where(f => !f.IsBindable);
        }

        /// <summary>
        /// Returns the primitive CLR type for the specified primitive type reference.
        /// </summary>
        /// <param name="primitiveType">The primitive type to resolve.</param>
        /// <param name="isNullable">Whether the returned type should be a nullable variant or not.</param>
        /// <returns>The CLR type for the primitive type reference.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not too complex for what this method does.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Class coupling is with all the primitive Clr types only.")]
        internal static Type GetPrimitiveClrType(IEdmPrimitiveType primitiveType, bool isNullable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(primitiveType != null, "primitiveType != null");

            switch (primitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return typeof(byte[]);
                case EdmPrimitiveTypeKind.Boolean:
                    return isNullable ? typeof(Boolean?) : typeof(Boolean);
                case EdmPrimitiveTypeKind.Byte:
                    return isNullable ? typeof(Byte?) : typeof(Byte);
                case EdmPrimitiveTypeKind.DateTime:
                    return isNullable ? typeof(DateTime?) : typeof(DateTime);
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
                case EdmPrimitiveTypeKind.Time:
                    return isNullable ? typeof(TimeSpan?) : typeof(TimeSpan);
                default:
                    return null;
            }
        }
#endif
        #endregion
        #endregion

        #region Private methods
        #region ODataLib only
#if ODATALIB
        /// <summary>
        /// Returns Collection item type name or null if the provided type name is not a collection.
        /// </summary>
        /// <param name="typeName">Collection type name.</param>
        /// <param name="isNested">Whether it is a nested (recursive) call.</param>
        /// <returns>Collection element type name or null if not a collection.</returns>
        /// <remarks>
        /// The following rules are used for collection type names:
        /// - it has to start with "Collection(" and end with ")" - trailing and leading whitespaces make the type not to be recognized as collection.
        /// - there is to be no characters (including whitespaces) between "Collection" and "(" - otherwise it won't berecognized as collection
        /// - collection item type name has to be a non-empty string - i.e. "Collection()" won't be recognized as collection
        /// - nested collection - e.g. "Collection(Collection(Edm.Int32))" - are not supported - we will throw
        /// Note the following are examples of valid type names which are not collection:
        /// - "Collection()"
        /// - " Collection(Edm.Int32)"
        /// - "Collection (Edm.Int32)"
        /// - "Collection("
        /// </remarks>
        private static string GetCollectionItemTypeName(string typeName, bool isNested)
        {
            int collectionTypeQualifierLength = CollectionTypeQualifier.Length;

            // to be recognized as a collection wireTypeName must not be null, has to start with "Collection(" and end with ")" and must not be "Collection()"
            if (typeName != null &&
                typeName.StartsWith(CollectionTypeQualifier + "(", StringComparison.Ordinal) &&
                typeName[typeName.Length - 1] == ')' &&
                typeName.Length != collectionTypeQualifierLength + 2)
            {
                if (isNested)
                {
                    throw new ODataException(ErrorStrings.ValidationUtils_NestedCollectionsAreNotSupported);
                }

                string innerTypeName = typeName.Substring(collectionTypeQualifierLength + 1, typeName.Length - (collectionTypeQualifierLength + 2));

                // Check if it is not a nested collection and throw if it is
                GetCollectionItemTypeName(innerTypeName, true);

                return innerTypeName;
            }

            return null;
        }

        /// <summary>
        /// Gets the function import parameter types in string.
        /// </summary>
        /// <param name="functionImport">Function import in question.</param>
        /// <returns>Comma separated function import parameter types enclosed in parantheses.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed",
            Justification = "This method is used for matching the name of the function import to something written by the server. So using the name is safe without resolving the type from the client.")]
        private static string ParameterTypesToString(this IEdmFunctionImport functionImport)
        {
            return JsonLightConstants.FunctionParameterStart +
                string.Join(JsonLightConstants.FunctionParameterSeparator, functionImport.Parameters.Select(p => p.Type.ODataFullName()).ToArray()) +
                JsonLightConstants.FunctionParameterEnd;
        }

#endif
        #endregion

        #region Everyone
        /// <summary>
        /// Gets a reference to a primitive kind definition of the appropriate kind.
        /// </summary>
        /// <param name="primitiveType">Primitive type to create a reference for.</param>
        /// <param name="nullable">Flag specifying if the referenced type should be nullable per default.</param>
        /// <returns>A new primitive type reference.</returns>
        private static EdmPrimitiveTypeReference ToTypeReference(IEdmPrimitiveType primitiveType, bool nullable)
        {
            EdmPrimitiveTypeKind kind = primitiveType.PrimitiveKind;
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
                    return new EdmPrimitiveTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.Binary:
                    return new EdmBinaryTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.String:
                    return new EdmStringTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.Decimal:
                    return new EdmDecimalTypeReference(primitiveType, nullable);
                case EdmPrimitiveTypeKind.DateTime:
                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Time:
                    return new EdmTemporalTypeReference(primitiveType, nullable);
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
                case EdmPrimitiveTypeKind.GeometryMultiPolygon:
                case EdmPrimitiveTypeKind.GeometryMultiLineString:
                case EdmPrimitiveTypeKind.GeometryMultiPoint:
                case EdmPrimitiveTypeKind.GeometryCollection:
                    return new EdmSpatialTypeReference(primitiveType, nullable);
                default:
                    throw new ODataException(ErrorStrings.General_InternalError(InternalErrorCodesCommon.EdmLibraryExtensions_PrimitiveTypeReference));
            }
        }

        /// <summary>
        /// Gets the single entity container or the default one if there are multiple containers.
        /// </summary>
        /// <param name="model">Model to find entity containers in.</param>
        /// <param name="foundContainer">Single container that was found.</param>
        /// <returns>Entity Container found in the model</returns>
        private static bool TryGetSingleOrDefaultEntityContainer(this IEdmModel model, out IEdmEntityContainer foundContainer)
        {
            IEdmEntityContainer[] entityContainers = model.EntityContainers().ToArray();
            foundContainer = null;

            Debug.Assert(entityContainers.Length != 0, "Can't get an entity container from a model that has none");

            if (entityContainers.Length > 1)
            {
                // Get the default entity container if there are more than one
                IEdmEntityContainer defaultContainer = entityContainers.SingleOrDefault(model.IsDefaultEntityContainer);
                if (defaultContainer != null)
                {
                    foundContainer = defaultContainer;
                }
            }
            else if (entityContainers.Length == 1)
            {
                foundContainer = entityContainers[0];
            }

            return foundContainer != null;
        }

        /// <summary>
        /// Finds the container elements with the specified container and name.
        /// </summary>
        /// <param name="model">The model to find the element in.</param>
        /// <param name="containerQualifiedElementName">The container qualified name of the elements.</param>
        /// <param name="resolver">The resolver method to resolve the elements.</param>
        /// <returns>The enumeration of <see cref="IEdmEntityContainerElement"/> with the specified name or null if no such element exists.</returns>
        private static IEnumerable<IEdmEntityContainerElement> ResolveContainerQualifiedElementName(IEdmModel model, string containerQualifiedElementName, Func<IEdmEntityContainer, string, IEnumerable<IEdmEntityContainerElement>> resolver)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(resolver != null, "resolver != null");

            if (string.IsNullOrEmpty(containerQualifiedElementName))
            {
                return Enumerable.Empty<IEdmEntityContainerElement>();
            }

            IEdmEntityContainer container = null;
            string elementName = null;

            // If there is no namespace or . in the name this may just be an entitySet in the default container, hand back the default container
            if (containerQualifiedElementName.IndexOf('.') < 0)
            {
                if (TryGetSingleOrDefaultEntityContainer(model, out container))
                {
                    elementName = containerQualifiedElementName;
                }
                else
                {
                    return Enumerable.Empty<IEdmEntityContainerElement>();
                }
            }
            else
            {
                // Find the last index of '.' before '('
#if !ASTORIA_CLIENT && !ASTORIA_SERVER && !ODATALIB_QUERY
                const char FunctionParameterStart = JsonLightConstants.FunctionParameterStart;
#else
                const char FunctionParameterStart = '(';
#endif

                int indexOfParameterStart = containerQualifiedElementName.IndexOf(FunctionParameterStart);
                string nameBeforeParameterStart = indexOfParameterStart < 0 ? containerQualifiedElementName : containerQualifiedElementName.Substring(0, indexOfParameterStart);
                int indexOfContainerNameAndElementNameSeparator = nameBeforeParameterStart.LastIndexOf('.');
                if (indexOfContainerNameAndElementNameSeparator < 0)
                {
                    return Enumerable.Empty<IEdmEntityContainerElement>();
                }

                string containerName = containerQualifiedElementName.Substring(0, indexOfContainerNameAndElementNameSeparator);
                elementName = containerQualifiedElementName.Substring(indexOfContainerNameAndElementNameSeparator + 1);
                if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(elementName))
                {
                    return Enumerable.Empty<IEdmEntityContainerElement>();
                }

                container = model.FindEntityContainer(containerName);
                if (container == null)
                {
                    return Enumerable.Empty<IEdmEntityContainerElement>();
                }
            }

            return resolver(container, elementName);
        }
        #endregion
        #endregion
    }
}
#endif
