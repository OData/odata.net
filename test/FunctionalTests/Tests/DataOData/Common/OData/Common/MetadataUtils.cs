//---------------------------------------------------------------------
// <copyright file="MetadataUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using System.Xml.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.OData.Edm.Vocabularies;
    using TypeCode = System.TypeCode;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for testing metadata and their OM
    /// </summary>
    public static class MetadataUtils
    {
        public static readonly Version EdmxVersion4 = new Version(4, 0);

        /// The namespace for Oasis verion of Edmx
        /// </summary>
        public const string EdmxOasisNamespace = "http://docs.oasis-open.org/odata/ns/edmx";

        private static readonly XName EdmxVersionAttributeName = XName.Get("Version");

        /// <summary>
        /// Adds the function and function import.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="model">The model.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <returns>Returns the function import created.</returns>
        public static EdmActionImport AddActionAndActionImport(this EdmEntityContainer container, EdmModel model, string name, IEdmTypeReference returnType, IEdmExpression entitySet, bool isBound)
        {
            EdmAction action = new EdmAction(container.Namespace, name, returnType, isBound, null);
            model.AddElement(action);
            return container.AddActionImport(name, action, entitySet);
        }

        /// <summary>
        /// As an edm function.
        /// </summary>
        /// <param name="action">The function.</param>
        /// <returns>EdmAction</returns>
        public static EdmAction AsEdmAction(this IEdmAction action)
        {
            var edmAction = action as EdmAction;
            ExceptionUtilities.CheckObjectNotNull(edmAction, "function");

            return edmAction;
        }

        /// <summary>
        /// Adds the function and function import.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="model">The model.</param>
        /// <param name="name">The name.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="isComposable">if set to <c>true</c> [is composable].</param>
        /// <param name="isBound">if set to <c>true</c> [is bound].</param>
        /// <returns></returns>
        public static EdmFunctionImport AddFunctionAndFunctionImport(this EdmEntityContainer container, EdmModel model, string name, IEdmTypeReference returnType, IEdmExpression entitySet = null, bool isComposable = false, bool isBound = false)
        {
            EdmFunction function = new EdmFunction(container.Namespace, name, returnType, isBound, null, isComposable);
            model.AddElement(function);
            return container.AddFunctionImport(name, function, entitySet);
        }

        /// <summary>
        /// Ases the edm function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static EdmFunction AsEdmFunction(this IEdmFunction function)
        {
            var edmAction = function as EdmFunction;
            ExceptionUtilities.CheckObjectNotNull(edmAction, "function");

            return edmAction;
        }

        /// <summary>
        /// Indicates whether the member property in Primitive
        /// </summary>
        /// <param name="memberProperty"> member property check</param>
        /// <returns>true is property is Primitive otherwise false</returns>
        public static bool IsPrimitive(this MemberProperty memberProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");
            ExceptionUtilities.CheckObjectNotNull(memberProperty.PropertyType, "PropertyType of memberProperty cannot be null");
            return memberProperty.PropertyType is PrimitiveDataType;
        }

        /// <summary>
        /// Indicates whether the member property in Complex
        /// </summary>
        /// <param name="memberProperty"> member property check</param>
        /// <returns>true is property is Complex otherwise false</returns>
        public static bool IsComplex(this MemberProperty memberProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");
            ExceptionUtilities.CheckObjectNotNull(memberProperty.PropertyType, "PropertyType of memberProperty cannot be null");
            return memberProperty.PropertyType is ComplexDataType;
        }

        /// <summary>
        /// Gets the inner most property represented by the path
        /// </summary>
        /// <param name="entityType">entity type to search for properties on</param>
        /// <param name="propertyPath">property path to search for</param>
        /// <returns>Inner most property as indicated by the propertyPath</returns>
        public static MemberProperty GetProperty(this EntityType entityType, string propertyPath)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckArgumentNotNull(propertyPath, "propertyPath");

            var props = entityType.GetPropertiesForPath(propertyPath.Split("/".ToCharArray()));
            ExceptionUtilities.CheckObjectNotNull(props, "No properties matching the path:{0}", propertyPath);
            ExceptionUtilities.Assert(props.Any(), "Zero properties matching the path:{0} expected one or more", propertyPath);
            return props.Last();
        }

        /// <summary>
        /// Gets the navigation property with the specified name.
        /// </summary>
        /// <param name="entityType">The entity type to search for navigation properties on.</param>
        /// <param name="navigationPropertyName">The navigation property name.</param>
        /// <returns>The navigation property with the specified name or null if none exists.</returns>
        public static NavigationProperty GetNavigationProperty(this EntityType entityType, string navigationPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            return entityType.AllNavigationProperties.Where(np => np.Name == navigationPropertyName).SingleOrDefault();
        }

        /// <summary>
        /// Wraps the specified <paramref name="payload"/> into and Edmx envelope.
        /// </summary>
        /// <param name="payload">The <see cref="XElement"/> payload to wrap.</param>
        /// <param name="edmVersion">The Edm Version for the edmx envelope.</param>
        /// <returns>
        /// The Edmx wrapper that contains the <paramref name="payload"/>.
        /// </returns>
        public static XElement WrapInEdmxEnvelope(this XElement payload, EdmVersion edmVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(payload, "payload");

            XAttribute metadataNamespaceAttr = null;

            Version edmxVersion = edmVersion.ToEdmxVersion();
            XNamespace edmxNamespace = EdmxXNamespace(edmxVersion);
            return new XElement(edmxNamespace + "Edmx", 
                new XAttribute(XNamespace.Xmlns + "edmx", edmxNamespace),
                new XAttribute(EdmxVersionAttributeName, edmxVersion.ToString()),
                new XElement(edmxNamespace + "DataServices",
                    metadataNamespaceAttr,
                    payload));
        }

        /// <summary>
        /// Unwraps the <paramref name="envelope"/> Edmx envelope and returns the actual payload.
        /// </summary>
        /// <param name="envelope">The <see cref="XElement"/> reprsenting the Edmx envelope.</param>
        /// <param name="edmVersion">The Edm Version for the edmx envelope.</param>
        /// <returns>The payload that was wrapped by the Edmx envelope.</returns>
        public static XElement UnwrapEdmxEnvelope(this XElement envelope, EdmVersion edmVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(envelope, "envelope");

            Version edmxVersion = edmVersion.ToEdmxVersion();
            XNamespace edmxNamespace = EdmxXNamespace(edmxVersion);
            ExceptionUtilities.Assert(envelope.Name.Namespace == edmxNamespace && envelope.Name.LocalName == "Edmx", "Expected <edmx:Edmx> as the top-level element.");

            XAttribute versionAttribute = envelope.Attribute(EdmxVersionAttributeName);
            ExceptionUtilities.Assert(versionAttribute != null && versionAttribute.Value == edmxVersion.ToString(), "Edmx version attribute not valid.");

            XElement dataServicesElement = envelope.Element(edmxNamespace + "DataServices");
            ExceptionUtilities.Assert(dataServicesElement != null, "<DataServices> element not found.");

            return dataServicesElement.Elements().Single();
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
            IEdmStructuredType type = model.ResolveType(typeName) as IEdmStructuredType;
            ExceptionUtilities.Assert(type != null, "Could not resolve name '" + typeName + "' to a structured type.");
            return type.ResolveProperty(propertyName);
        }

        /// <summary>Helper method to get a property from the type.</summary>
        /// <param name="structuredType">The structured type to get the property from.</param>
        /// <param name="name">The name of the property to get.</param>
        /// <returns>The property found or null otherwise.</returns>
        public static IEdmProperty ResolveProperty(this IEdmStructuredType structuredType, string name)
        {
            IEdmProperty property = structuredType.FindProperty(name);
            ExceptionUtilities.CheckObjectNotNull(property, "Can't find property '" + name + "' on type '" + structuredType.TestFullName() + "'.");
            return property;
        }

        /// <summary>Helper method to get a type by its name.</summary>
        /// <param name="model">The model.</param>
        /// <param name="fullName">The name of the type to find. For the namespace the namespace of the model is used.</param>
        /// <param name="nullable">true if the type reference should be nullable; otherwise false.</param>
        /// <returns>The type found.</returns>
        public static IEdmTypeReference ResolveTypeReference(this IEdmModel model, string fullName, bool nullable)
        {
            IEdmType type = ResolveType(model, fullName);
            ExceptionUtilities.Assert(type != null, "type != null");

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
                throw new TaupoInvalidOperationException("A fully qualified name is required: " + fullName + ".");
            }

            IEdmType type = model.FindType(fullName);
            if (type == null)
            {
                type = EdmCoreModel.Instance.FindType(fullName);
            }
            if (type == null)
            {
                ExceptionUtilities.Assert(false, "Can't find type '" + fullName + "'.");
                return null;
            }

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
            ExceptionUtilities.CheckArgumentNotNull(typeReference, "typeReference");

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
        public static string TestFullName(this IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            // Handle collection type names here since for EdmLib collections are functions
            // that do not have a full name
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            if (collectionType != null)
            {
                string elementTypeName = collectionType.ElementType.TestFullName();
                if (elementTypeName == null)
                {
                    return null;
                }

                return "Collection(" + elementTypeName + ")";
            }

            var namedDefinition = type as IEdmSchemaElement;
            if (namedDefinition != null)
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
        public static Version ToEdmxVersion(this EdmVersion edmVersion)
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
        /// Returns all the entity types in a model.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to get the entity types for (must not be null).</param>
        /// <returns>An enumerable of all <see cref="IEdmEntityType"/> instances in the <paramref name="model"/>.</returns>
        public static IEnumerable<IEdmEntityType> EntityTypes(this IEdmModel model)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");

            IEnumerable<IEdmSchemaElement> schemaElements = model.SchemaElements;
            if (schemaElements != null)
            {
                return schemaElements.OfType<IEdmEntityType>();
            }

            return null;
        }

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding non-nullable <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <returns>A non-nullable type reference for the <paramref name="type"/>.</returns>
        public static IEdmTypeReference ToTypeReference(this IEdmType type)
        {
            return ToTypeReference(type, false /*nullable*/);
        }

        /// <summary>
        /// Turns a <see cref="IEdmType"/> into the corresponding <see cref="IEdmTypeReference"/>.
        /// </summary>
        /// <param name="type">The type to convert.</param>
        /// <param name="nullable">true if the returned type reference should be nullable; otherwise false.</param>
        /// <returns>A type reference for the <paramref name="type"/>.</returns>
        public static IEdmTypeReference ToTypeReference(this IEdmType type, bool nullable)
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
                case EdmTypeKind.None:
                default:
                    throw new TaupoNotSupportedException("Unsupported type kind: " + type.TypeKind.ToString());
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
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Returns the primitive type reference for the given Clr type.
        /// </summary>
        /// <param name="clrType">The Clr type to resolve.</param>
        /// <returns>The primitive type reference for the given Clr type.</returns>
        public static IEdmPrimitiveTypeReference GetPrimitiveTypeReference(Type clrType)
        {
            Debug.Assert(clrType != null, "clrType != null");

            Type targetType = TestTypeUtils.GetNonNullableType(clrType);
            TypeCode typeCode = Type.GetTypeCode(targetType);
            bool nullable = TestTypeUtils.TypeAllowsNull(clrType);

            IEdmPrimitiveType primitiveType = null;
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

                    if (typeof(GeographyPoint).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPoint);
                        break;
                    }

                    if (typeof(GeographyLineString).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyLineString);
                        break;
                    }

                    if (typeof(GeographyPolygon).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyPolygon);
                        break;
                    }

                    if (typeof(GeographyMultiPoint).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPoint);
                        break;
                    }

                    if (typeof(GeographyMultiLineString).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiLineString);
                        break;
                    }

                    if (typeof(GeographyMultiPolygon).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyMultiPolygon);
                        break;
                    }

                    if (typeof(GeographyCollection).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeographyCollection);
                        break;
                    }

                    if (typeof(Geography).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geography);
                        break;
                    }

                    if (typeof(GeometryPoint).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPoint);
                        break;
                    }

                    if (typeof(GeometryLineString).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryLineString);
                        break;
                    }

                    if (typeof(GeometryPolygon).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryPolygon);
                        break;
                    }

                    if (typeof(GeometryMultiPoint).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPoint);
                        break;
                    }

                    if (typeof(GeometryMultiLineString).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiLineString);
                        break;
                    }

                    if (typeof(GeometryMultiPolygon).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryMultiPolygon);
                        break;
                    }

                    if (typeof(GeometryCollection).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.GeometryCollection);
                        break;
                    }

                    if (typeof(Geometry).IsAssignableFrom(targetType))
                    {
                        primitiveType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Geometry);
                        break;
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
        /// Clones the specified type reference.
        /// </summary>
        /// <param name="typeReference">The type reference to clone.</param>
        /// <param name="nullable">true to make the cloned type reference nullable; false to make it non-nullable.</param>
        /// <returns>The cloned <see cref="IEdmTypeReference"/> instance.</returns>
        public static IEdmTypeReference Clone(this IEdmTypeReference typeReference, bool nullable)
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
                            throw new TaupoNotSupportedException("Invalid primitive type kind: " + typeKind.ToString());
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

                case EdmTypeKind.None:  // fall through
                default:
                    throw new TaupoNotSupportedException("Invalid type kind: " + typeKind.ToString());
            }
        }

        /// <summary>
        /// Creates a collection value type for the specified <paramref name="itemTypeReference"/>.
        /// </summary>
        /// <param name="itemTypeReference">The <see cref="IEdmPrimitiveTypeReference"/> for the item type.</param>
        /// <returns>The created <see cref="IEdmCollectionTypeReference"/>.</returns>
        public static IEdmCollectionTypeReference ToCollectionTypeReference(this IEdmPrimitiveTypeReference itemTypeReference)
        {
            IEdmCollectionType collectionType = new EdmCollectionType(itemTypeReference);
            return (IEdmCollectionTypeReference)ToTypeReference(collectionType);
        }

        /// <summary>
        /// Get the <see cref="IEdmEntityTypeReference"/> of the item type of the <paramref name="typeReference"/>.
        /// </summary>
        /// <param name="typeReference">The collection type to get the item type for.</param>
        /// <returns>The item type of the <paramref name="typeReference"/>.</returns>
        public static IEdmTypeReference GetCollectionItemType(this IEdmTypeReference typeReference)
        {
            IEdmCollectionTypeReference collectionType = typeReference.AsCollection();
            return collectionType == null ? null : collectionType.ElementType();
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
                    throw new TaupoNotSupportedException("Invalid primitive type kind: " + kind.ToString());
            }
        }

        /// <summary>
        /// Returns the primitive CLR type for the specified primitive type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type to resolve.</param>
        /// <returns>The CLR type for the primitive type reference.</returns>
        public static Type GetPrimitiveClrType(IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            ExceptionUtilities.CheckArgumentNotNull(primitiveTypeReference, "primitiveTypeReference");

            var primitiveType = (IEdmPrimitiveType)primitiveTypeReference.Definition;
            var isNullable = primitiveTypeReference.IsNullable;

            switch (primitiveType.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    return typeof(byte[]);
                case EdmPrimitiveTypeKind.Boolean:
                    return isNullable ? typeof(Boolean?) : typeof(Boolean);
                case EdmPrimitiveTypeKind.Byte:
                    return isNullable ? typeof(Byte?) : typeof(Byte);
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
                default:
                    return null;
            }
        }

    }
}
