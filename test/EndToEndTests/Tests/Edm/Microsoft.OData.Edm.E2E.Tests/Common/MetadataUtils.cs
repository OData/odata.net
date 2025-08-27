//---------------------------------------------------------------------
// <copyright file="MetadataUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.Spatial;

namespace Microsoft.OData.Edm.E2E.Tests.Common;

/// <summary>
/// Helper methods for testing metadata and their OM
/// </summary>
public static class MetadataUtils
{
    public static readonly Version EdmxVersion4 = new Version(4, 0);

    /// The namespace for Oasis verion of Edmx
    public const string EdmxOasisNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

    /// The namespace for Oasis verion of Edm
    private const string EdmOasisNamespace = "http://docs.oasis-open.org/odata/ns/edm";

    /// <summary>
    /// Indicates whether the member property in Primitive
    /// </summary>
    /// <param name="property"> member property check</param>
    /// <returns>true is property is Primitive otherwise false</returns>
    public static bool IsPrimitive(this IEdmProperty property)
    {
        Assert.NotNull(property);
        Assert.NotNull(property.Type);
        return property.Type.IsPrimitive();
    }

    /// <summary>
    /// Indicates whether the member property in Complex
    /// </summary>
    /// <param name="property"> member property check</param>
    /// <returns>true is property is Complex otherwise false</returns>
    public static bool IsComplex(this IEdmProperty property)
    {
        Assert.NotNull(property);
        Assert.NotNull(property.Type);
        return property.Type.IsComplex();
    }

    /// <summary>
    /// Gets the inner most property represented by the path
    /// </summary>
    /// <param name="entityType">entity type to search for properties on</param>
    /// <param name="propertyPath">property path to search for</param>
    /// <returns>Inner most property as indicated by the propertyPath</returns>
    public static IEdmProperty GetProperty(this EdmEntityType entityType, string propertyPath)
    {
        Assert.NotNull(entityType);
        Assert.NotNull(propertyPath);

        var props = entityType.GetPropertiesForPath(propertyPath.Split("/".ToCharArray()));
        Assert.NotNull(props);
        
        return props.Last();
    }

    /// <summary>
    /// Gets the navigation property with the specified name.
    /// </summary>
    /// <param name="entityType">The entity type to search for navigation properties on.</param>
    /// <param name="navigationPropertyName">The navigation property name.</param>
    /// <returns>The navigation property with the specified name or null if none exists.</returns>
    public static IEdmNavigationProperty? GetNavigationProperty(this IEdmEntityType entityType, string navigationPropertyName)
    {
        Assert.NotNull(entityType);
        Assert.NotEmpty(navigationPropertyName);
        Assert.NotNull(navigationPropertyName);

        return entityType.NavigationProperties().SingleOrDefault(np => np.Name == navigationPropertyName);
    }

    /// <summary>Helper method to resolve a property.</summary>
    /// <param name="model">The model.</param>
    /// <param name="typeAndPropertyName">The name of the type and property to find, like 'Type.Property'.</param>
    /// <returns>The property found or null otherwise.</returns>
    public static IEdmProperty ResolveProperty(this IEdmModel model, string typeAndPropertyName)
    {
        int lastDot = typeAndPropertyName.LastIndexOf('.');
        string propertyName = typeAndPropertyName.Substring(lastDot + 1);
        string typeName = typeAndPropertyName.Substring(0, lastDot);
        IEdmStructuredType? type = model.ResolveType(typeName) as IEdmStructuredType;
        Assert.NotNull(type);

        return type.ResolveProperty(propertyName);
    }

    /// <summary>Helper method to get a property from the type.</summary>
    /// <param name="structuredType">The structured type to get the property from.</param>
    /// <param name="name">The name of the property to get.</param>
    /// <returns>The property found or null otherwise.</returns>
    public static IEdmProperty ResolveProperty(this IEdmStructuredType structuredType, string name)
    {
        IEdmProperty property = structuredType.FindProperty(name);
        Assert.NotNull(property);
        return property;
    }

    /// <summary>Helper method to get a type by its name.</summary>
    /// <param name="model">The model.</param>
    /// <param name="fullName">The name of the type to find. For the namespace the namespace of the model is used.</param>
    /// <param name="nullable">true if the type reference should be nullable; otherwise false.</param>
    /// <returns>The type found.</returns>
    public static IEdmTypeReference? ResolveTypeReference(this IEdmModel model, string fullName, bool nullable)
    {
        IEdmType type = model.ResolveType(fullName);
        Assert.NotNull(type);
        return type.ToTypeReference(nullable);
    }

    /// <summary>Helper method to get a type by its name.</summary>
    /// <param name="model">The model.</param>
    /// <param name="fullName">The name of the type to find. For the namespace the namespace of the model is used.</param>
    /// <returns>The type found.</returns>
    public static IEdmType ResolveType(this IEdmModel model, string fullName)
    {
        if (!fullName.Contains("."))
        {
            throw new InvalidOperationException("A fully qualified name is required: " + fullName + ".");
        }

        IEdmType type = model.FindType(fullName) ?? EdmCoreModel.Instance.FindType(fullName);
        Assert.NotNull(type);
        return type;
    }

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
    public static string TestFullName(this IEdmTypeReference typeReference)
    {
        Assert.NotNull(typeReference);

        return typeReference.Definition.TestFullName();
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
    public static string? TestFullName(this IEdmType type)
    {
        Assert.NotNull(type);
        // Handle collection type names here since for EdmLib collections are functions
        // that do not have a full name
        if (type is IEdmCollectionType collectionType)
        {
            string elementTypeName = collectionType.ElementType.TestFullName();
            if (elementTypeName == null)
            {
                return null;
            }

            return "Collection(" + elementTypeName + ")";
        }

        if (type is IEdmSchemaElement namedDefinition)
        {
            return namedDefinition.FullName();
        }

        return null;
    }

    /// <summary>
    /// Returns the EDMX version for a given EDM version.
    /// </summary>
    /// <param name="edmVersion">The EDM version to get the EDMX version for.</param>
    /// <returns>The EDMX version for the <paramref name="edmVersion"/>.</returns>
    public static Version ToEdmxVersion(this Version edmVersion)
    {
        return EdmxVersion4;
    }

    /// <summary>
    /// Gets the EDMX namespace for a specified version.
    /// </summary>
    /// <param name="edmxVersion">The EDMX version to get the namespace for.</param>
    /// <returns>The <see cref="XNamespace"/> for the specified <paramref name="edmxVersion"/>.</returns>
    public static XNamespace EdmxXNamespace(Version edmxVersion)
    {
        return EdmxOasisNamespace;
    }

    /// <summary>
    /// Returns all the complex types in a model.
    /// </summary>
    /// <param name="model">The <see cref="IEdmModel"/> to get the complex types for (must not be null).</param>
    /// <returns>An enumerable of all <see cref="IEdmComplexType"/> instances in the <paramref name="model"/>.</returns>
    public static IEnumerable<IEdmComplexType> ComplexTypes(this IEdmModel model)
    {
        Assert.NotNull(model);

        IEnumerable<IEdmSchemaElement> schemaElements = model.SchemaElements;
        if (schemaElements != null)
        {
            return schemaElements.OfType<IEdmComplexType>();
        }

        return Enumerable.Empty<IEdmComplexType>();
    }

    /// <summary>
    /// Returns all the entity types in a model.
    /// </summary>
    /// <param name="model">The <see cref="IEdmModel"/> to get the entity types for (must not be null).</param>
    /// <returns>An enumerable of all <see cref="IEdmEntityType"/> instances in the <paramref name="model"/>.</returns>
    public static IEnumerable<IEdmEntityType> EntityTypes(this IEdmModel model)
    {
        Assert.NotNull(model);

        IEnumerable<IEdmSchemaElement> schemaElements = model.SchemaElements;
        if (schemaElements != null)
        {
            return schemaElements.OfType<IEdmEntityType>();
        }

        return Enumerable.Empty<IEdmEntityType>();
    }

    /// <summary>
    /// Turns a <see cref="IEdmType"/> into the corresponding non-nullable <see cref="IEdmTypeReference"/>.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <returns>A non-nullable type reference for the <paramref name="type"/>.</returns>
    public static IEdmTypeReference? ToTypeReference(this IEdmType type)
    {
        return type.ToTypeReference(false /*nullable*/);
    }

    /// <summary>
    /// Turns a <see cref="IEdmType"/> into the corresponding <see cref="IEdmTypeReference"/>.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <param name="nullable">true if the returned type reference should be nullable; otherwise false.</param>
    /// <returns>A type reference for the <paramref name="type"/>.</returns>
    public static IEdmTypeReference? ToTypeReference(this IEdmType type, bool nullable)
    {
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
                return new EdmCollectionTypeReference((IEdmCollectionType)type);
            case EdmTypeKind.EntityReference:
                return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)type, nullable);
            case EdmTypeKind.Enum:
                return new EdmEnumTypeReference((IEdmEnumType)type, nullable);
            default:
                throw new NotSupportedException("Unsupported type kind: " + type.TypeKind.ToString());
        }
    }

    /// <summary>
    /// Creates a collection type for the specified <paramref name="itemTypeReference"/>.
    /// </summary>
    /// <param name="itemTypeReference">The <see cref="IEdmComplexTypeReference"/> for the item type.</param>
    /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
    public static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmComplexTypeReference itemTypeReference)
    {
        IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
        return (IEdmCollectionTypeReference)collectionType.ToTypeReference();
    }

    /// <summary>
    /// Creates a collection value type for the specified <paramref name="itemTypeReference"/>.
    /// </summary>
    /// <param name="itemTypeReference">The <see cref="IEdmPrimitiveTypeReference"/> for the item type.</param>
    /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
    public static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmPrimitiveTypeReference itemTypeReference)
    {
        IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
        return (IEdmCollectionTypeReference)collectionType.ToTypeReference();
    }

    /// <summary>
    /// Returns the primitive type reference for the given Clr type.
    /// </summary>
    /// <param name="clrType">The Clr type to resolve.</param>
    /// <returns>The primitive type reference for the given Clr type.</returns>
    public static IEdmPrimitiveTypeReference? GetPrimitiveTypeReference(Type clrType)
    {
        Assert.NotNull(clrType);

        Type targetType = GetNonNullableType(clrType);
        TypeCode typeCode = Type.GetTypeCode(targetType);
        bool nullable = TypeAllowsNull(clrType);

        IEdmPrimitiveType? primitiveType = null;
        switch (typeCode)
        {
            case TypeCode.Boolean:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean);
                break;
            case TypeCode.Byte:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte);
                break;
            case TypeCode.Decimal:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal);
                break;
            case TypeCode.Double:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double);
                break;
            case TypeCode.Int16:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16);
                break;
            case TypeCode.Int32:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
                break;
            case TypeCode.Int64:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64);
                break;
            case TypeCode.SByte:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte);
                break;
            case TypeCode.String:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String);
                break;
            case TypeCode.Single:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single);
                break;
            default:
                if (targetType == typeof(byte[]))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary);
                    break;
                }

                if (targetType == typeof(Stream))
                {
                    // stream is always non-nullable
                    nullable = false;
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream);
                    break;
                }

                if (targetType == typeof(DateTimeOffset))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset);
                    break;
                }

                if (targetType == typeof(Guid))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid);
                    break;
                }

                if (targetType == typeof(TimeSpan))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration);
                    break;
                }

                if (typeof(GeographyPoint).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint);
                    break;
                }

                if (typeof(GeographyLineString).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString);
                    break;
                }

                if (typeof(GeographyPolygon).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon);
                    break;
                }

                if (typeof(GeographyMultiPoint).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint);
                    break;
                }

                if (typeof(GeographyMultiLineString).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString);
                    break;
                }

                if (typeof(GeographyMultiPolygon).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon);
                    break;
                }

                if (typeof(GeographyCollection).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
                    break;
                }

                if (typeof(Geography).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
                    break;
                }

                if (typeof(GeometryPoint).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint);
                    break;
                }

                if (typeof(GeometryLineString).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString);
                    break;
                }

                if (typeof(GeometryPolygon).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon);
                    break;
                }

                if (typeof(GeometryMultiPoint).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint);
                    break;
                }

                if (typeof(GeometryMultiLineString).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString);
                    break;
                }

                if (typeof(GeometryMultiPolygon).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon);
                    break;
                }

                if (typeof(GeometryCollection).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
                    break;
                }

                if (typeof(Geometry).GetTypeInfo().IsAssignableFrom(targetType))
                {
                    primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
                }

                break;
        }

        if (primitiveType == null)
        {
            return null;
        }

        return ToTypeReference(primitiveType, nullable);
    }

    /// <summary>
    /// Returns the primitive type reference for the given edm type name.
    /// </summary>
    /// <param name="edmTypeName">The edm type name to resolve.</param>
    /// <returns>The primitive type reference for the given edm type name.</returns>
    public static IEdmPrimitiveTypeReference? GetPrimitiveTypeReference(string edmTypeName)
    {
        IEdmPrimitiveType? primitiveType = null;
        bool nullable = true;
        switch (edmTypeName)
        {
            case EdmStringConstants.EdmBooleanTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Boolean);
                break;
            case EdmStringConstants.EdmByteTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Byte);
                break;
            case EdmStringConstants.EdmDecimalTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Decimal);
                break;
            case EdmStringConstants.EdmDoubleTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Double);
                break;
            case EdmStringConstants.EdmInt16TypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int16);
                break;
            case EdmStringConstants.EdmInt32TypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32);
                break;
            case EdmStringConstants.EdmInt64TypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int64);
                break;
            case EdmStringConstants.EdmSByteTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.SByte);
                break;
            case EdmStringConstants.EdmStringTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String);
                break;
            case EdmStringConstants.EdmSingleTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Single);
                break;
            case EdmStringConstants.EdmBinaryTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Binary);
                break;
            case EdmStringConstants.EdmStreamTypeName:
                nullable = false;
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Stream);
                break;
            case EdmStringConstants.EdmDateTimeOffsetTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.DateTimeOffset);
                break;
            case EdmStringConstants.EdmGuidTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Guid);
                break;
            case EdmStringConstants.EdmDurationTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Duration);
                break;
            case EdmStringConstants.EdmPointTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint);
                break;
            case EdmStringConstants.EdmLineStringTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString);
                break;
            case EdmStringConstants.EdmPolygonTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon);
                break;
            case EdmStringConstants.EdmMultiPointTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint);
                break;
            case EdmStringConstants.EdmMultiLineStringTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString);
                break;
            case EdmStringConstants.EdmMultiPolygonTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon);
                break;
            case EdmStringConstants.EdmCollectionTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
                break;
            case EdmStringConstants.EdmGeographyTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
                break;
            case EdmStringConstants.EdmGeometryPointTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint);
                break;
            case EdmStringConstants.EdmGeometryLineStringTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString);
                break;
            case EdmStringConstants.EdmGeometryPolygonTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon);
                break;
            case EdmStringConstants.EdmGeometryMultiPointTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint);
                break;
            case EdmStringConstants.EdmGeometryMultiLineStringTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString);
                break;
            case EdmStringConstants.EdmGeometryMultiPolygonTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon);
                break;
            case EdmStringConstants.EdmGeometryCollectionTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
                break;
            case EdmStringConstants.EdmGeometryTypeName:
                primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
                break;
        }

        if (primitiveType == null)
        {
            return null;
        }

        return ToTypeReference(primitiveType, nullable);
    }

    /// <summary>
    /// Clones the specified type reference.
    /// </summary>
    /// <param name="typeReference">The type reference to clone.</param>
    /// <param name="nullable">true to make the cloned type reference nullable; false to make it non-nullable.</param>
    /// <returns>The cloned <see cref="IEdmTypeReference"/> instance.</returns>
    public static IEdmTypeReference? Clone(this IEdmTypeReference typeReference, bool nullable)
    {
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
                            binaryTypeReference.MaxLength);

                    case EdmPrimitiveTypeKind.String:
                        IEdmStringTypeReference stringTypeReference = (IEdmStringTypeReference)typeReference;
                        return new EdmStringTypeReference(
                            primitiveType,
                            nullable,
                            stringTypeReference.IsUnbounded,
                            stringTypeReference.MaxLength,
                            stringTypeReference.IsUnicode);

                    case EdmPrimitiveTypeKind.Decimal:
                        IEdmDecimalTypeReference decimalTypeReference = (IEdmDecimalTypeReference)typeReference;
                        return new EdmDecimalTypeReference(primitiveType, nullable, decimalTypeReference.Precision, decimalTypeReference.Scale);

                    case EdmPrimitiveTypeKind.DateTimeOffset:
                    case EdmPrimitiveTypeKind.Duration:
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
                        throw new NotSupportedException("Invalid primitive type kind: " + typeKind.ToString());
                }

            case EdmTypeKind.Entity:
                return new EdmEntityTypeReference((IEdmEntityType)typeReference.Definition, nullable);

            case EdmTypeKind.Complex:
                return new EdmComplexTypeReference((IEdmComplexType)typeReference.Definition, nullable);

            case EdmTypeKind.Collection:
                return new EdmCollectionTypeReference((IEdmCollectionType)typeReference.Definition);

            case EdmTypeKind.EntityReference:
                return new EdmEntityReferenceTypeReference((IEdmEntityReferenceType)typeReference.Definition, nullable);

            case EdmTypeKind.Enum:
                return new EdmEnumTypeReference((IEdmEnumType)typeReference.Definition, nullable);

            default:
                throw new NotSupportedException("Invalid type kind: " + typeKind.ToString());
        }
    }

    /// <summary>
    /// Get the <see cref="IEdmEntityTypeReference"/> of the item type of the <paramref name="typeReference"/>.
    /// </summary>
    /// <param name="typeReference">The collection type to get the item type for.</param>
    /// <returns>The item type of the <paramref name="typeReference"/>.</returns>
    public static IEdmTypeReference? GetCollectionItemType(this IEdmTypeReference typeReference)
    {
        IEdmCollectionTypeReference collectionType = typeReference.AsCollection();
        return collectionType?.ElementType();
    }

    /// <summary>
    /// Checks whether the specified type is a generic nullable type.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>true if <paramref name="type"/> is nullable; false otherwise.</returns>
    public static bool IsNullableType(Type type)
    {
        Assert.NotNull(type);
        bool isGenericType = type.IsGenericType;
        return isGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Gets a non-nullable version of the specified type.
    /// </summary>
    /// <param name="type">Type to get non-nullable version for.</param>
    /// <returns>
    /// <paramref name="type"/> if type is a reference type or a 
    /// non-nullable type; otherwise, the underlying value type.
    /// </returns>
    public static Type GetNonNullableType(Type type)
    {
        Assert.NotNull(type);
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    /// <summary>
    /// Checks whether the specified <paramref name='type' /> can be assigned null.
    /// </summary>
    /// <param name='type'>Type to check.</param>
    /// <returns>true if type is a reference type or a Nullable type; false otherwise.</returns>
    public static bool TypeAllowsNull(Type type)
    {
        Assert.NotNull(type);
        bool isValueType = type.IsValueType;
        return !isValueType || IsNullableType(type);
    }

    /// <summary>
    /// Gets the element type for enumerable
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>If the <paramref name="type"/> was IEnumerable then it returns the type of a single element
    /// otherwise it returns null.</returns>
    public static Type? GetIEnumerableElementType(Type type)
    {
        Assert.NotNull(type);
        var iEnum = FindIEnumerable(type);
        if (iEnum == null)
        {
            return null;
        }

        return iEnum.GetTypeInfo().GetGenericArguments()[0];
    }

    /// <summary>
    /// Finds type that implements IEnumerable so can get element type
    /// </summary>
    /// <param name="seqType">The Type to check</param>
    /// <returns>returns the type which implements IEnumerable</returns>
    public static Type? FindIEnumerable(Type seqType)
    {
        Assert.NotNull(seqType);

        if (seqType.IsArray)
        {
            return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
        }

        bool isGenericType = seqType.IsGenericType;
        if (isGenericType)
        {
            foreach (Type arg in seqType.GetTypeInfo().GetGenericArguments())
            {
                Type iEnum = typeof(IEnumerable<>).MakeGenericType(arg);
                if (iEnum.GetTypeInfo().IsAssignableFrom(seqType))
                {
                    return iEnum;
                }
            }
        }

        Type[] interfaces = seqType.GetTypeInfo().GetInterfaces();
        foreach (Type iface in interfaces)
        {
            var iEnum = FindIEnumerable(iface);
            if (iEnum != null)
            {
                return iEnum;
            }
        }

        var baseType = seqType.BaseType;
        if (baseType != null && baseType != typeof(object))
        {
            return FindIEnumerable(baseType);
        }

        return null;
    }

    /// <summary>
    /// Clone the EdmModel based on Edmlib test util -- EdmToStockModelConverter
    /// </summary>
    /// <param name="model">The model to use.</param>
    /// <returns>returns the EdmModel cloned from input IEdmModel</returns>
    public static EdmModel Clone(this IEdmModel model)
    {
        return new EdmToStockModelConverter().ConvertToStockModel(model);
    }

    /// <summary>
    /// Retrieves all child elements, with the given name, in the EDM namespace.
    /// </summary>
    /// <param name="parentElement">The parent XML element.</param>
    /// <param name="elementName">The EDM name of the child elements to retrieve.</param>
    /// <returns>A collection of child XML elements with the given EDM namespace based name.</returns>
    public static IEnumerable<XElement> EdmElements(this XElement parentElement, string elementName)
    {
        Assert.NotEmpty(elementName);
        Assert.NotNull(parentElement);
        return parentElement.Elements(XName.Get(elementName, EdmOasisNamespace));
    }

    /// <summary>
    /// Retrieves the value of the specified attribute.
    /// </summary>
    /// <param name="element">The XML element.</param>
    /// <param name="attributeName">The name of the attribute to retrieve.</param>
    /// <returns>The value of the attribute as a string. Throws an exception if attribute not found.</returns>
    public static string GetAttributeValue(this XElement element, string attributeName)
    {
        Assert.NotEmpty(attributeName);
        Assert.NotNull(attributeName);

        var attribute = element.Attribute(attributeName);
        Assert.NotNull(attribute);

        return attribute.Value;
    }

    /// <summary>
    /// Try to retrieve the value of the specified attribute.
    /// </summary>
    /// <param name="element">The XML element.</param>
    /// <param name="attributeName">The name of the attribute to retrieve.</param>
    /// <param name="attributeValue">The value of the attribute as a string, or null if not found.</param>
    /// <returns>Whether or not the attribute was found.</returns>
    public static bool TryGetAttributeValue(this XElement element, string attributeName, out string? attributeValue)
    {
        Assert.NotEmpty(attributeName);
        Assert.NotNull(attributeName);

        var attribute = element.Attribute(attributeName);
        if (attribute == null)
        {
            attributeValue = null;
            return false;
        }
        else
        {
            attributeValue = attribute.Value;
            return true;
        }
    }

    public static EdmFunctionImport AddFunctionAndFunctionImport(this EdmEntityContainer container, EdmModel model, string name, IEdmTypeReference returnType)
    {
        EdmFunction function = new EdmFunction(container.Namespace, name, returnType, false /*isBound*/, null, false /*isComposable*/);
        model.AddElement(function);
        return container.AddFunctionImport(name, function);
    }

    public static EdmFunctionImport AddFunctionAndFunctionImport(this EdmEntityContainer container, EdmModel model, string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool isComposable, bool isBound)
    {
        EdmFunction function = new EdmFunction(container.Namespace, name, returnType, isBound, null, isComposable);
        model.AddElement(function);
        return container.AddFunctionImport(name, function, entitySet);
    }

    /// <summary>
    /// As an edm action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>EdmAction</returns>
    public static EdmFunction AsEdmFunction(this IEdmFunction function)
    {
        var edmFunction = function as EdmFunction;
        Assert.NotNull(edmFunction);

        return edmFunction;
    }

    /// <summary>
    /// Adds the action and action import.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="model">The model.</param>
    /// <param name="name">The name.</param>
    /// <param name="returnType">Type of the return.</param>
    /// <param name="entitySet">The entity set.</param>
    /// <param name="isBound">if set to <c>true</c> [is bound].</param>
    /// <returns>Returns the action import created.</returns>
    public static EdmActionImport AddActionAndActionImport(this EdmEntityContainer container, EdmModel model, string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool isBound)
    {
        EdmAction action = new EdmAction(container.Namespace, name, returnType, isBound, null);
        model.AddElement(action);
        return container.AddActionImport(name, action, entitySet);
    }

    /// <summary>
    /// As an edm action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>EdmAction</returns>
    public static EdmAction AsEdmAction(this IEdmAction action)
    {
        var edmAction = action as EdmAction;
        Assert.NotNull(edmAction);

        return edmAction;
    }

    /// <summary>
    /// Gets the member properties that correspond to a given path for the entity type
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <param name="pathPieces">The property names from the path</param>
    /// <returns>The series of properties for the path</returns>
    public static IEnumerable<IEdmProperty> GetPropertiesForPath(this IEdmEntityType entityType, string[] pathPieces)
    {
        var currentProperties = entityType.Properties();
        foreach (string propertyName in pathPieces)
        {
            IEdmProperty property = currentProperties.Single(p => p.Name == propertyName);

            var complexDataType = property.Type as IEdmComplexTypeReference;
            if (complexDataType != null)
            {
                currentProperties = complexDataType.ComplexDefinition().Properties();
            }

            var collectionDataType = property.Type as IEdmCollectionTypeReference;
            if (collectionDataType != null)
            {
                var complexElementType = collectionDataType.GetCollectionItemType() as IEdmComplexTypeReference;
                if (complexElementType != null)
                {
                    currentProperties = complexElementType.ComplexDefinition().Properties();
                }
            }

            yield return property;
        }
    }

    /// <summary>
    /// Gets the <see cref="IEdmValue"/> of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="expressionEvaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or null if no relevant annotation exists.</returns>
    public static IEdmValue? GetPropertyValue(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, EdmExpressionEvaluator expressionEvaluator)
    {
        return model.GetPropertyValue(context, context.Type.AsEntity().EntityDefinition(), term, property, null, expressionEvaluator.Evaluate);
    }

    /// <summary>
    /// Gets the CLR value of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or default(<typeparamref name="T"/>) if no relevant annotation exists.</returns>
    public static T? GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, EdmToClrEvaluator evaluator)
    {
        return model.GetPropertyValue(context, context.Type.AsEntity().EntityDefinition(), term, property, null, evaluator.EvaluateToClrValue<T>);
    }

    /// <summary>
    /// Gets the CLR value of a property of a term type that has been applied to the type of a value.
    /// </summary>
    /// <typeparam name="T">The CLR type of the value to be returned.</typeparam>
    /// <param name="model">Model to search for annotations.</param>
    /// <param name="context">Value to use as context in evaluation.</param>
    /// <param name="term">Term to search for annotations.</param>
    /// <param name="property">Property to evaluate.</param>
    /// <param name="qualifier">Qualifier to apply.</param>
    /// <param name="evaluator">Evaluator to use to perform expression evaluation.</param>
    /// <returns>Value of the property evaluated against the supplied value, or default(<typeparamref name="T"/>) if no relevant annotation exists.</returns>
    public static T? GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmTerm term, IEdmProperty property, string qualifier, EdmToClrEvaluator evaluator)
    {
        return model.GetPropertyValue(context, context.Type.AsEntity().EntityDefinition(), term, property, qualifier, evaluator.EvaluateToClrValue<T>);
    }

    private static T? GetPropertyValue<T>(this IEdmModel model, IEdmStructuredValue context, IEdmEntityType contextType, IEdmTerm term, IEdmProperty property, string? qualifier, Func<IEdmExpression, IEdmStructuredValue, T> evaluator)
    {
        IEnumerable<IEdmVocabularyAnnotation> annotations = model.FindVocabularyAnnotations<IEdmVocabularyAnnotation>(contextType, term, qualifier);

        if (annotations.Count() != 1)
        {
            throw new InvalidOperationException("Type " + contextType.ToTraceString() + " must have a single annotation with term " + term.ToTraceString());
        }

        var annotationValue = annotations.Single().Value as IEdmRecordExpression;

        if (annotationValue == null)
        {
            throw new InvalidOperationException("Type " + contextType.ToTraceString() + " must have a single annotation containing a record expression with term " + term.ToTraceString());
        }

        var propertyConstructor = annotationValue.FindProperty(property.Name);
        return propertyConstructor != null ? evaluator(propertyConstructor.Value, context) : default;
    }

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
            case EdmPrimitiveTypeKind.DateTimeOffset:
            case EdmPrimitiveTypeKind.Duration:
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
                throw new NotSupportedException("Invalid primitive type kind: " + kind.ToString());
        }
    }
}
