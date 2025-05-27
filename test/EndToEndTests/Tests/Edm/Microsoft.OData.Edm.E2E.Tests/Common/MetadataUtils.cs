//---------------------------------------------------------------------
// <copyright file="MetadataUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Reflection;
using System.Xml.Linq;
using Microsoft.OData.Edm.Vocabularies;

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
