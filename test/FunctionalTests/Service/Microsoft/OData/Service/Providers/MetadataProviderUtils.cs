//---------------------------------------------------------------------
// <copyright file="MetadataProviderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Client;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Edm.Vocabularies;
    using CommonUtil = Microsoft.OData.Service.CommonUtil;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for the IDSMP-to-EdmLib bridge.
    /// </summary>
    internal static class MetadataProviderUtils
    {
        /// <summary>
        /// Maps a given data service version with the highest supported Edm version.
        /// New entry should be created for each new WCF DS version.
        /// </summary>
        internal static readonly Dictionary<Version, Version> DataServiceEdmVersionMap = new Dictionary<Version, Version>(EqualityComparer<Version>.Default)
        {
            { ODataProtocolVersion.V4.ToVersion(), new Version(4, 0) },
        };

        /// <summary>
        /// Validation rule used when serializing metadata documents that ensures that property names don't include
        /// characters that are reserved in OData.
        /// </summary>
        internal static readonly ValidationRule<IEdmProperty> PropertyNameIncludesReservedODataCharacters =
            new ValidationRule<IEdmProperty>(
                (context, item) =>
                {
                    string propertyName = item.Name;
                    if (propertyName != null && propertyName.IndexOfAny(InvalidCharactersInPropertyNames) >= 0)
                    {
                        string invalidChars = string.Join(
                            ", ",
                            InvalidCharactersInPropertyNames.Select(c => string.Format(CultureInfo.InvariantCulture, "'{0}'", c)).ToArray());
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidName,
                            Service.Strings.MetadataProviderUtils_PropertiesMustNotContainReservedChars(propertyName, invalidChars));
                    }
                });

        /// <summary>Version number for CSDL 4.0.</summary>
        private static readonly Version Version4Dot0 = new Version(4, 0);

        /// <summary>Xml writer settings for writing element annotations.</summary>
        private static readonly XmlWriterSettings xmlWriterSettingsForElementAnnotations = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = true,
        };

        /// <summary>The set of characters that are invalid in property names.</summary>
        /// <remarks>Keep this array in sync with ValidationUtils.InvalidCharactersInPropertyNames in ODataLib.</remarks>
        private static readonly char[] InvalidCharactersInPropertyNames = new char[] { ':', '.', '@' };

        /// <summary>
        /// Compute the entity set name from the resource set.
        /// </summary>
        /// <param name="resourceSet">Resource set instance whose name needs to be returned.</param>
        /// <returns>Local name of the entity set.</returns>
        internal static string GetEntitySetName(ResourceSet resourceSet)
        {
            Debug.Assert(resourceSet != null, "resourceSet != null");

            string entityContainerName = resourceSet.EntityContainerName;
            if (entityContainerName != null && 
                resourceSet.Name.StartsWith(entityContainerName, StringComparison.Ordinal) &&
                resourceSet.Name[entityContainerName.Length] == '.')
            {
                Debug.Assert(resourceSet.Name.Count(c => c == '.') == 1, "The resourceSet name should have exactly one period character.");
                return resourceSet.Name.Substring(resourceSet.EntityContainerName.Length + 1);
            }

            return resourceSet.Name;
        }

        /// <summary>
        /// Given the association set, generate the association name.
        /// </summary>
        /// <param name="associationSet">The association set to generate the association name for.</param>
        /// <returns>The generated association name.</returns>
        internal static string GetAssociationName(ResourceAssociationSet associationSet)
        {
            Debug.Assert(associationSet != null, "associationSet != null");

            // end1 is always the end with the navigation property
            ResourceAssociationSetEnd end1 = associationSet.End1.ResourceProperty != null ? associationSet.End1 : associationSet.End2;

            // end2 points to the other end and is null if the other end has null navigation property
            ResourceAssociationSetEnd end2 = (end1 == associationSet.End1 ? (associationSet.End2.ResourceProperty != null ? associationSet.End2 : null) : null);

            string associationName = end1.ResourceType.Name + '_' + end1.ResourceProperty.Name;
            if (end2 != null)
            {
                associationName = associationName + '_' + end2.ResourceType.Name + '_' + end2.ResourceProperty.Name;
            }

            return associationName;
        }

        /// <summary>
        /// Get the string key to look up an association from the namespace.
        /// </summary>
        /// <param name="resourceType">The resource type contributing to the association name.</param>
        /// <param name="resourceProperty">The navigation property contributing to the association name.</param>
        /// <returns>The lookup key to find the association by name.</returns>
        internal static string GetAssociationEndName(ResourceType resourceType, ResourceProperty resourceProperty)
        {
            Debug.Assert(resourceType != null, "resourceType != null");

            string lookupName = resourceType.Name;
            if (resourceProperty != null)
            {
                lookupName = lookupName + '_' + resourceProperty.Name;
            }

            return lookupName;
        }

        /// <summary>Returns the version number given an <see cref="MetadataEdmSchemaVersion"/>.</summary>
        /// <param name="schemaVersion">EDM schema version.</param>
        /// <returns>Version number corresponding to the <paramref name="schemaVersion"/>.</returns>
        internal static Version ToVersion(this MetadataEdmSchemaVersion schemaVersion)
        {
            return Version4Dot0;
        }

        /// <summary>
        /// Computes the multiplicity of a navigation property.
        /// </summary>
        /// <param name="property">The navigation to compute the multiplicity for.</param>
        /// <returns>The <see cref="EdmMultiplicity"/> value of the multiplicity.</returns>
        internal static EdmMultiplicity GetMultiplicity(ResourceProperty property)
        {
            Debug.Assert(
                property == null || property.IsOfKind(ResourcePropertyKind.ResourceReference) || property.IsOfKind(ResourcePropertyKind.ResourceSetReference),
                "If a property is specified it must be a navigation property.");

            return property != null && property.Kind == ResourcePropertyKind.ResourceReference
                       ? EdmMultiplicity.ZeroOrOne
                       : EdmMultiplicity.Many;
        }

        /// <summary>
        /// Converts the string representation of a multiplicity to the corresponding enumeration value.
        /// </summary>
        /// <param name="multiplicity">The string representation of the multiplicity to convert.</param>
        /// <returns>The <see cref="EdmMultiplicity"/> representation of the <paramref name="multiplicity"/>.</returns>
        internal static EdmMultiplicity ConvertMultiplicity(string multiplicity)
        {
            switch (multiplicity)
            {
                case XmlConstants.Many: return EdmMultiplicity.Many;
                case XmlConstants.One: return EdmMultiplicity.One;
                case XmlConstants.ZeroOrOne: return EdmMultiplicity.ZeroOrOne;
                default: return EdmMultiplicity.Unknown;
            }
        }

        /// <summary>
        /// Creates a primitive type reference for the specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to create a primitive type reference for.</param>
        /// <returns>An <see cref="IEdmPrimitiveTypeReference"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>This method will remove all processed facets from the annotations.</remarks>
        internal static IEdmPrimitiveTypeReference CreatePrimitiveTypeReference(ResourceType resourceType)
        {
            return CreatePrimitiveTypeReference(resourceType, /*annotations*/null);
        }
        
        /// <summary>
        /// Creates a primitive type reference for the specified <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="resourceType">The resource type to create a primitive type reference for.</param>
        /// <param name="annotations">The optional annotations for the resource type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>An <see cref="IEdmPrimitiveTypeReference"/> instance for the <paramref name="resourceType"/>.</returns>
        /// <remarks>This method will remove all processed facets from the annotations.</remarks>
        internal static IEdmPrimitiveTypeReference CreatePrimitiveTypeReference(ResourceType resourceType, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(resourceType != null, "resourceType != null");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.Primitive, "resourceType.ResourceTypeKind == ResourceTypeKind.Primitive");

            // EdmLib does not support Linq.Binary and XElement; we have to convert them first
            Type instanceType = resourceType.InstanceType;
            if (instanceType == typeof(Binary))
            {
                instanceType = typeof(byte[]);
            }
            else if (instanceType == typeof(XElement))
            {
                instanceType = typeof(string);
            }
            else if (instanceType == typeof(DateTime))
            {
                instanceType = typeof(DateTimeOffset);
            }
            else if (instanceType == typeof(DateTime?))
            {
                instanceType = typeof(DateTimeOffset?);
            }

            return GetPrimitiveTypeReferenceFromTypeAndFacets(instanceType, annotations);
        }

        /// <summary>
        /// Get the value of the default annotation (if present).
        /// </summary>
        /// <param name="annotations">The annotations that optionally hold the default annotation.</param>
        /// <returns>The string representation of the default value if present; otherwise null.</returns>
        /// <remarks>This method will remove the default annotation from the annotations if found.</remarks>
        internal static string GetAndRemoveDefaultValue(List<KeyValuePair<string, object>> annotations)
        {
            if (annotations == null || annotations.Count == 0)
            {
                return null;
            }
            
            object defaultValueAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlDefaultValueAttributeName, out defaultValueAnnotationValue))
            {
                return ConvertDefaultValue(defaultValueAnnotationValue);
            }

            return null;
        }

        /// <summary>
        /// Get the value of the nullable annotation (if present).
        /// </summary>
        /// <param name="annotations">The annotations that optionally hold the nullability annotation.</param>
        /// <returns>The boolean value of the nullable facet if it is present; otherwise returns null.</returns>
        internal static bool? GetAndRemoveNullableFacet(List<KeyValuePair<string, object>> annotations)
        {
            if (annotations == null || annotations.Count == 0)
            {
                return null;
            }

            object nullableAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlNullableAttributeName, out nullableAnnotationValue))
            {
                return ConvertAnnotationValue<bool>(nullableAnnotationValue, XmlConstants.CsdlNullableAttributeName);
            }

            return null;
        }

        /// <summary>
        /// Convert the custom resource annotations to serializable EDM annotations.
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="customAnnotations">The list of custom annotations to convert.</param>
        /// <param name="target">The target <see cref="IEdmElement"/> to add the converted annotations to.</param>
        internal static void ConvertCustomAnnotations(MetadataProviderEdmModel model, IEnumerable<KeyValuePair<string, object>> customAnnotations, IEdmElement target)
        {
            Debug.Assert(model != null, "edmModel != null");
            Debug.Assert(target != null, "target != null");
            foreach (IEdmDirectValueAnnotation annotation in ConvertCustomAnnotations(model, customAnnotations))
            {
                model.SetAnnotationValue(target, annotation.NamespaceUri, annotation.Name, annotation.Value);
            }
        }

        /// <summary>
        /// Converts custom annotations in to IEdmImediateValueAnnotations
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="customAnnotations">The list of custom annotations to convert.</param>
        /// <returns>The converted annotations.</returns>
        internal static IEnumerable<IEdmDirectValueAnnotation> ConvertCustomAnnotations(MetadataProviderEdmModel model, IEnumerable<KeyValuePair<string, object>> customAnnotations)
        {
            if (customAnnotations == null)
            {
                yield break;
            }

            foreach (KeyValuePair<string, object> annotation in customAnnotations)
            {
                object annotationValue = annotation.Value;
                Type annotationType = annotationValue == null ? null : annotationValue.GetType();
                bool isXElement = annotationType == typeof(XElement);

                int index = annotation.Key.LastIndexOf(":", StringComparison.Ordinal);
                if (index == -1)
                {
                    // this is an attribute annotation
                    if (!isXElement)
                    {
                        string annotationStringValue = ConvertAttributeAnnotationValue(annotation.Value);
                        yield return new EdmDirectValueAnnotation(string.Empty, annotation.Key, new EdmStringConstant(EdmCoreModel.Instance.GetString(/*nullable*/true), annotationStringValue));
                    }
                }
                else
                {
                    string xmlNamespace = annotation.Key.Substring(0, index);
                    string localname = annotation.Key.Substring(index + 1);

                    if (annotationValue == null || !isXElement)
                    {
                        string annotationStringValue = ConvertAttributeAnnotationValue(annotation.Value);
                        yield return new EdmDirectValueAnnotation(xmlNamespace, localname, new EdmStringConstant(EdmCoreModel.Instance.GetString(/*nullable*/true), annotationStringValue));
                    }
                    else if (annotationValue != null && annotationType == typeof(XElement))
                    {
                        XElement xmlElement = (XElement)annotationValue;
                        Debug.Assert(
                            xmlElement.Name.NamespaceName == xmlNamespace && xmlElement.Name.LocalName == localname,
                            "Local name and namespace name of the Xml element annotation have to match the name of the annotation.");
                        string xmlElementString = CreateElementAnnotationStringRepresentation(xmlElement);
                        EdmStringConstant elementAnnotation = new EdmStringConstant(EdmCoreModel.Instance.GetString(/*nullable*/false), xmlElementString);
                        elementAnnotation.SetIsSerializedAsElement(model, true);
                        yield return new EdmDirectValueAnnotation(xmlNamespace, localname, elementAnnotation);
                    }
                }
            }
        }

        /// <summary>
        /// Updates a navigation property with information that will create associations and association sets
        /// </summary>
        /// <param name="navigationProperty">The navigation property to update.</param>
        /// <param name="partner">The partner of the navigation property.</param>
        /// <param name="isPrinciple">A value indicating whether this navigation property is the principle end of the association.</param>
        /// <param name="dependentProperties">Dependent properties of the association.</param>
        /// <param name="deleteAction">Action to execute on the deletion of this end of a bidirectional relationship.</param>
        /// <param name="opposingMultiplicity">Multiplicity of the other end of a relationship.</param>
        internal static void FixUpNavigationPropertyWithAssociationSetData(
            IEdmNavigationProperty navigationProperty, 
            IEdmNavigationProperty partner, 
            bool isPrinciple, 
            List<IEdmStructuralProperty> dependentProperties, 
            EdmOnDeleteAction deleteAction, 
            EdmMultiplicity opposingMultiplicity)
        {
            MetadataProviderEdmNavigationProperty metadataProviderNavigationProperty = navigationProperty as MetadataProviderEdmNavigationProperty;
            if (metadataProviderNavigationProperty != null)
            {
                metadataProviderNavigationProperty.FixUpNavigationProperty(partner, isPrinciple, deleteAction);

                // If the other ends' multiplicity is one we need to fix up the type as non-nullable
                if (opposingMultiplicity == EdmMultiplicity.One)
                {
                    metadataProviderNavigationProperty.Type = metadataProviderNavigationProperty.Type.Definition.ToTypeReference(false);
                }

                if (dependentProperties != null && !isPrinciple)
                {
                    metadataProviderNavigationProperty.SetDependentProperties(dependentProperties);
                }
            }
            else
            {
                if (dependentProperties != null && !isPrinciple)
                {
                    ((MetadataProviderEdmSilentNavigationProperty)navigationProperty).SetDependentProperties(dependentProperties);
                }
            }
        }

        /// <summary>
        /// Checks if null value validation should be disallowed for primitive properties.
        /// </summary>
        /// <param name="resourceProperty">The primitive property</param>
        /// <param name="primitiveTypeReference">The primitive type reference to check for </param>
        /// <returns>True if null validation should be disabled.</returns>
        internal static bool ShouldDisablePrimitivePropertyNullValidation(ResourceProperty resourceProperty, IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            Debug.Assert(
                resourceProperty.IsOfKind(ResourcePropertyKind.Primitive) || resourceProperty.IsOfKind(ResourcePropertyKind.Stream),
                "Resource property should be a primitive property or a stream property.");
            Debug.Assert(primitiveTypeReference != null, "Primtive type reference should not be null.");

            // WCF DS readers used only the instance type of the resource property for validation of null values
            // but ODataLib now uses the nullability of the primitive type reference. The primitive type reference
            // can have different nullability than the backing CLR type, since that nullability can come from the underlying EF model.
            // So for example we can have a String property which is marked as non-nullable.
            // WCF DS used to accept null for such property (since the CLR type is nullable), and then later the actual
            // IUpdatable would fail on it when trying to apply the null value to the EF (where it's not nullable).
            // If we leave the type marked as not-nullable for ODataLib, it will fail much sooner and with different status code.
            // So mark the property as "disable validation of nulls" so that the null value passes through ODataLib.
            if (WebUtil.TypeAllowsNull(resourceProperty.ResourceType.InstanceType) && !primitiveTypeReference.IsNullable)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets an instance of <see cref="ResourceType"/> for the given <see cref="IEdmType"/>, assuming it was originally created from the resource type.
        /// </summary>
        /// <param name="edmType">The edm type to get the resource type for.</param>
        /// <returns>The resource type.</returns>
        internal static ResourceType GetResourceType(IEdmType edmType)
        {
            if (edmType == null)
            {
                return null;
            }

            var resourceTypeBasedType = edmType as IResourceTypeBasedEdmType;
            if (resourceTypeBasedType != null)
            {
                return resourceTypeBasedType.ResourceType;
            }

            if (edmType.TypeKind == EdmTypeKind.Collection)
            {
                // When parsing entity-set's in the path, the URI parser has to fake up an entity-collection type, so we cannot just 
                // pull the original resource type from it. Instead, resolve the element type, then create an EntityCollectionResourceType
                IEdmType elementTypeDefinition = ((IEdmCollectionType)edmType).ElementType.Definition;
                Debug.Assert(elementTypeDefinition.TypeKind == EdmTypeKind.Entity, "Should only get here for entity collection types");
                return new EntityCollectionResourceType(GetResourceType(elementTypeDefinition));
            }

            ResourceType primitiveResourceType = PrimitiveResourceTypeMap.TypeMap.GetPrimitive(edmType.FullTypeName());
            Debug.Assert(primitiveResourceType != null, "Could not determine a resource type for edm type: " + edmType.FullTypeName());
            return primitiveResourceType;
        }

        /// <summary>
        /// Gets an instance of <see cref="ResourceType"/> for the given <see cref="TypeSegment"/>.
        /// </summary>
        /// <param name="typeSegment">The type segment to get the resource type for.</param>
        /// <returns>The resource type.</returns>
        internal static ResourceType GetResourceType(TypeSegment typeSegment)
        {
            Debug.Assert(typeSegment != null, "typeSegment != null");
            Debug.Assert(typeSegment.EdmType != null, "typeSegment.EdmType != null");

            IEdmType edmType = typeSegment.EdmType;
            if (edmType.TypeKind == EdmTypeKind.Collection)
            {
                edmType = ((IEdmCollectionType)edmType).ElementType.Definition;
            }

            return GetResourceType(edmType);
        }

        /// <summary>
        /// Creates the string representation of an element annotation represented as <see cref="XElement"/>.
        /// </summary>
        /// <param name="xmlElement">The <see cref="XElement"/> to convert to string.</param>
        /// <returns>The string representation of the <paramref name="xmlElement"/>.</returns>
        private static string CreateElementAnnotationStringRepresentation(XElement xmlElement)
        {
            Debug.Assert(xmlElement != null, "xmlElement != null");

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, xmlWriterSettingsForElementAnnotations))
            {
                xmlElement.WriteTo(writer);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the value for an attribute annotation to the corresponding serializable string value
        /// </summary>
        /// <param name="annotationValue">The value of the annotation to convert.</param>
        /// <returns>The string representation of the <paramref name="annotationValue"/>.</returns>
        private static string ConvertAttributeAnnotationValue(object annotationValue)
        {
            if (annotationValue == null)
            {
                // NOTE: this is the behavior of WriteAttributeString that was used in Astoria metadata serialization
                //       before EdmLib integration.
                return string.Empty;
            }

            Type annotationType = annotationValue.GetType();

            // We need to special case the boolean facets, since ToString returns True and False as values
            // and for xml, they need to be lower case
            if (annotationType == typeof(bool))
            {
                return (bool)annotationValue ? XmlConstants.XmlTrueLiteral : XmlConstants.XmlFalseLiteral;
            }
            
            if (annotationType == typeof(int))
            {
                return XmlConvert.ToString((int)annotationValue);
            }
            
            if (annotationType.IsEnum)
            {
                return annotationValue.ToString();
            }
            
            if (annotationType == typeof(byte))
            {
                return XmlConvert.ToString((byte)annotationValue);
            }
            
            if (annotationType == typeof(DateTime))
            {
                return XmlConvert.ToString((DateTime)annotationValue, "yyyy-MM-dd HH:mm:ss.fffZ");
            }
            
            if (annotationType == typeof(byte[]))
            {
                // Convert the bytes to a hex string in the format, e.g. "0x123"
                string bytesAsHexString = string.Concat(((byte[])annotationValue).Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
                return "0x" + bytesAsHexString;
            }
            
            return annotationValue.ToString();
        }

        /// <summary>
        /// Returns the primitive type reference for the given Clr type using facet annotations if available.
        /// </summary>
        /// <param name="clrType">The Clr type to resolve.</param>
        /// <param name="annotations">The optional annotations for the type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>The primitive type reference for the given Clr type including (optional) facets.</returns>
        /// <remarks>This method will remove all processed facets from the annotations.</remarks>
        private static IEdmPrimitiveTypeReference GetPrimitiveTypeReferenceFromTypeAndFacets(Type clrType, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(clrType != null, "type != null");

            // First get the primitive type reference ignoring facets.
            IEdmPrimitiveTypeReference primitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(clrType);

            if (primitiveTypeReference.IsSpatial())
            {
                // force the metadata to use SRID="Variable" by explicitly setting the spatialReferenceIdentifier to null
                primitiveTypeReference = new EdmSpatialTypeReference(primitiveTypeReference.PrimitiveDefinition(), primitiveTypeReference.IsNullable, /*SRID*/ null);
            }

            // Then apply the facets
            return primitiveTypeReference.ApplyFacetAnnotations(annotations);
        }

        /// <summary>
        /// Applies (optional) facet annotations to the primitive type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The type reference to apply the facets to.</param>
        /// <param name="annotations">The optional annotations for the type; the annotations can contain facets that need to be applied to the type reference.</param>
        /// <returns>The primitive type reference including (optional) facets.</returns>
        /// <remarks>This method will remove all processed facets from the annotations.</remarks>
        private static IEdmPrimitiveTypeReference ApplyFacetAnnotations(this IEdmPrimitiveTypeReference primitiveTypeReference, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            if (annotations == null || annotations.Count == 0)
            {
                return primitiveTypeReference;
            }

            IEdmPrimitiveTypeReference resultTypeReference = primitiveTypeReference;

            bool isNullable = primitiveTypeReference.IsNullable;

            // If we find an annotation, it overrides the default nullability that was created from the Clr type.
            object isNullableAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlNullableAttributeName, out isNullableAnnotationValue))
            {
                isNullable = ConvertAnnotationValue<bool>(isNullableAnnotationValue, XmlConstants.CsdlNullableAttributeName);
            }

            EdmPrimitiveTypeKind kind = primitiveTypeReference.PrimitiveKind();
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
                    // if the nullability from the annotation is different, it wins.
                    if (primitiveTypeReference.IsNullable != isNullable)
                    {
                        resultTypeReference = new EdmPrimitiveTypeReference(primitiveTypeReference.PrimitiveDefinition(), isNullable);
                    }

                    break;
                case EdmPrimitiveTypeKind.Binary:
                    Debug.Assert(primitiveTypeReference.IsNullable, "Binary type reference must always be nullable.");
                    resultTypeReference = CreateBinaryTypeReference(primitiveTypeReference, isNullable, annotations);
                    break;

                case EdmPrimitiveTypeKind.String:
                    resultTypeReference = CreateStringTypeReference(primitiveTypeReference, isNullable, annotations);
                    break;

                case EdmPrimitiveTypeKind.Decimal:
                    resultTypeReference = CreateDecimalTypeReference(primitiveTypeReference, isNullable, annotations);
                    break;

                case EdmPrimitiveTypeKind.DateTimeOffset:
                case EdmPrimitiveTypeKind.Duration:
                    resultTypeReference = CreateTemporalTypeReference(primitiveTypeReference, isNullable, annotations);
                    break;

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
                    resultTypeReference = CreateSpatialTypeReference(primitiveTypeReference, isNullable, annotations);
                    break;

                default:
                    throw new InvalidOperationException(Service.Strings.MetadataProviderUtils_UnsupportedPrimitiveTypeKind(kind.ToString()));
            }

            return resultTypeReference;
        }

        /// <summary>
        /// Create a binary type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type reference.</param>
        /// <param name="nullableFacet">true if the type reference should be nullable; otherwise false.</param>
        /// <param name="annotations">The (optional) annotations with facets for the type reference.</param>
        /// <returns>The created type reference.</returns>
        private static IEdmPrimitiveTypeReference CreateBinaryTypeReference(IEdmPrimitiveTypeReference primitiveTypeReference, bool nullableFacet, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveType != null");
            Debug.Assert(primitiveTypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.Binary, "primitiveType.PrimitiveKind() == EdmPrimitiveTypeKind.Binary");
            Debug.Assert(annotations != null, "annotations != null");
            Debug.Assert(annotations.Where(kvp => kvp.Key == XmlConstants.CsdlNullableAttributeName).Count() == 0, "The nullable facet should have been removed from the annotations by now.");

            //// Facets supported by binary references
            //// * Nullable
            //// * IsFixedLength
            //// * IsMaxMaxLength
            //// * MaxLength

            if (annotations.Count == 0)
            {
                return primitiveTypeReference.IsNullable == nullableFacet ? primitiveTypeReference : new EdmBinaryTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet);
            }
            
            bool isMaxMaxLength;
            int? maxLength = GetMaxLengthAnnotation(annotations, out isMaxMaxLength);
            bool? isFixedLength = GetFixedLengthAnnotation(annotations);
            return new EdmBinaryTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet, isMaxMaxLength, maxLength);
        }

        /// <summary>
        /// Create a string type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type reference.</param>
        /// <param name="nullableFacet">true if the type reference should be nullable; otherwise false.</param>
        /// <param name="annotations">The (optional) annotations with facets for the type reference.</param>
        /// <returns>The created type reference.</returns>
        private static IEdmPrimitiveTypeReference CreateStringTypeReference(IEdmPrimitiveTypeReference primitiveTypeReference, bool nullableFacet, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");
            Debug.Assert(annotations != null, "annotations != null");
            Debug.Assert(annotations.Where(kvp => kvp.Key == XmlConstants.CsdlNullableAttributeName).Count() == 0, "The nullable facet should have been removed from the annotations by now.");

            //// Facets supported by string references
            //// * Nullable
            //// * IsFixedLength
            //// * IsMaxMaxLength
            //// * MaxLength
            //// * IsUnicode

            if (annotations.Count == 0)
            {
                return primitiveTypeReference.IsNullable == nullableFacet ? primitiveTypeReference : new EdmStringTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet);
            }
            
            bool isMaxMaxLength;
            int? maxLength = GetMaxLengthAnnotation(annotations, out isMaxMaxLength);
            bool? isFixedLength = GetFixedLengthAnnotation(annotations);
            bool? isUnicode = GetIsUnicodeAnnotation(annotations);
            return new EdmStringTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet, isMaxMaxLength, maxLength, isUnicode);
        }

        /// <summary>
        /// Create a decimal type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type reference.</param>
        /// <param name="nullableFacet">true if the type reference should be nullable; otherwise false.</param>
        /// <param name="annotations">The (optional) annotations with facets for the type reference.</param>
        /// <returns>The created type reference.</returns>
        private static IEdmPrimitiveTypeReference CreateDecimalTypeReference(IEdmPrimitiveTypeReference primitiveTypeReference, bool nullableFacet, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");
            Debug.Assert(annotations != null, "annotations != null");
            Debug.Assert(annotations.Where(kvp => kvp.Key == XmlConstants.CsdlNullableAttributeName).Count() == 0, "The nullable facet should have been removed from the annotations by now.");

            //// Facets supported by decimal references
            //// * Nullable
            //// * Precision
            //// * Scale

            if (annotations.Count == 0)
            {
                return primitiveTypeReference.IsNullable == nullableFacet ? primitiveTypeReference : new EdmDecimalTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet);
            }
            
            int? precision = GetPrecisionAnnotation(annotations);
            int? scale = GetScaleAnnotation(annotations);
            return new EdmDecimalTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet, precision, scale);
        }

        /// <summary>
        /// Create a temporal type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type reference.</param>
        /// <param name="nullableFacet">true if the type reference should be nullable; otherwise false.</param>
        /// <param name="annotations">The (optional) annotations with facets for the type reference.</param>
        /// <returns>The created type reference.</returns>
        private static IEdmPrimitiveTypeReference CreateTemporalTypeReference(IEdmPrimitiveTypeReference primitiveTypeReference, bool nullableFacet, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");
            Debug.Assert(annotations != null, "annotations != null");
            Debug.Assert(annotations.Where(kvp => kvp.Key == XmlConstants.CsdlNullableAttributeName).Count() == 0, "The nullable facet should have been removed from the annotations by now.");

            //// Facets supported by temporal references
            //// * Nullable
            //// * Precision

            if (annotations.Count == 0)
            {
                return primitiveTypeReference.IsNullable == nullableFacet ? primitiveTypeReference : new EdmTemporalTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet);
            }
            
            int? precision = GetPrecisionAnnotation(annotations);
            return new EdmTemporalTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet, precision);
        }

        /// <summary>
        /// Create a spatial type reference.
        /// </summary>
        /// <param name="primitiveTypeReference">The primitive type reference.</param>
        /// <param name="nullableFacet">true if the type reference should be nullable; otherwise false.</param>
        /// <param name="annotations">The (optional) annotations with facets for the type reference.</param>
        /// <returns>The created type reference.</returns>
        private static IEdmPrimitiveTypeReference CreateSpatialTypeReference(IEdmPrimitiveTypeReference primitiveTypeReference, bool nullableFacet, List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");
            Debug.Assert(annotations != null, "annotations != null");
            Debug.Assert(annotations.Where(kvp => kvp.Key == XmlConstants.CsdlNullableAttributeName).Count() == 0, "The nullable facet should have been removed from the annotations by now.");

            //// Facets supported by spatial references
            //// * SpatialReferenceIdentifier

            object spatialReferenceIdentifierAnnotationValue;
            if (annotations.Count == 0 || !TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlSridAttributeName, out spatialReferenceIdentifierAnnotationValue))
            {
                return primitiveTypeReference.IsNullable == nullableFacet ? primitiveTypeReference : new EdmSpatialTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet, null);
            }

            int spatialReferenceIdentifier = ConvertAnnotationValue<int>(spatialReferenceIdentifierAnnotationValue, XmlConstants.CsdlSridAttributeName);
            return new EdmSpatialTypeReference(primitiveTypeReference.PrimitiveDefinition(), nullableFacet, spatialReferenceIdentifier);
        }

        /// <summary>
        /// Gets the 'FixedLength' annotation if it exists.
        /// </summary>
        /// <param name="annotations">The annotations to check.</param>
        /// <returns>The value of the annotation or a default value if the annotation is not specified.</returns>
        private static bool? GetFixedLengthAnnotation(List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(annotations != null, "annotations != null");

            object isFixedLengthAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlFixedLengthAttributeName, out isFixedLengthAnnotationValue))
            {
                return ConvertAnnotationValue<bool>(isFixedLengthAnnotationValue, XmlConstants.CsdlFixedLengthAttributeName);
            }

            return null;
        }

        /// <summary>
        /// Gets the 'MaxLength' annotation if it exists.
        /// </summary>
        /// <param name="annotations">The annotations to check.</param>
        /// <param name="isMaxMaxLength">true if the annotation has the special 'Max' value; otherwise false.</param>
        /// <returns>The value of the annotation or a default value if the annotation is not specified.</returns>
        private static int? GetMaxLengthAnnotation(List<KeyValuePair<string, object>> annotations, out bool isMaxMaxLength)
        {
            Debug.Assert(annotations != null, "annotations != null");

            object maxLengthAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlMaxLengthAttributeName, out maxLengthAnnotationValue))
            {
                // the MaxLength facet can either be 'Max' or an actual int?
                int maxLength;
                if (TryConvertAnnotationValue(maxLengthAnnotationValue, out maxLength))
                {
                    isMaxMaxLength = false;
                    return maxLength;
                }

                // The 'Max' value can either be a string or a special (internal) EDM class that 
                // will return 'Max' when ToString() is called on it.
                string maxMaxLengthString;
                if (!TryConvertAnnotationValue(maxLengthAnnotationValue, out maxMaxLengthString))
                {
                    maxMaxLengthString = maxLengthAnnotationValue == null ? null : maxLengthAnnotationValue.ToString();
                }

                if (string.CompareOrdinal(XmlConstants.CsdlMaxLengthAttributeMaxValue, maxMaxLengthString) == 0)
                {
                    isMaxMaxLength = true;
                    return null;
                }

                string typeName = maxLengthAnnotationValue == null ? "null" : maxLengthAnnotationValue.GetType().FullName;
                throw new FormatException(Service.Strings.MetadataProviderUtils_ConversionError(XmlConstants.CsdlMaxLengthAttributeName, typeName, typeof(string).FullName));
            }

            isMaxMaxLength = false;
            return null;
        }

        /// <summary>
        /// Gets the 'IsUnicode' annotation if it exists.
        /// </summary>
        /// <param name="annotations">The annotations to check.</param>
        /// <returns>The value of the annotation or a default value if the annotation is not specified.</returns>
        private static bool? GetIsUnicodeAnnotation(List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(annotations != null, "annotations != null");

            object isUnicodeAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlUnicodeAttributeName, out isUnicodeAnnotationValue))
            {
                return ConvertAnnotationValue<bool>(isUnicodeAnnotationValue, XmlConstants.CsdlUnicodeAttributeName);
            }

            return null;
        }

        /// <summary>
        /// Gets the 'Precision' annotation if it exists.
        /// </summary>
        /// <param name="annotations">The annotations to check.</param>
        /// <returns>The value of the annotation or a default value if the annotation is not specified.</returns>
        private static int? GetPrecisionAnnotation(List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(annotations != null, "annotations != null");

            object precisionAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlPrecisionAttributeName, out precisionAnnotationValue))
            {
                return ConvertAnnotationValue<int>(precisionAnnotationValue, XmlConstants.CsdlPrecisionAttributeName);
            }

            return null;
        }

        /// <summary>
        /// Gets the 'Precision' annotation if it exists.
        /// </summary>
        /// <param name="annotations">The annotations to check.</param>
        /// <returns>The value of the annotation or a default value if the annotation is not specified.</returns>
        private static int? GetScaleAnnotation(List<KeyValuePair<string, object>> annotations)
        {
            Debug.Assert(annotations != null, "annotations != null");

            object scaleAnnotationValue;
            if (TryFindAndRemoveAnnotation(annotations, XmlConstants.CsdlScaleAttributeName, out scaleAnnotationValue))
            {
                return ConvertAnnotationValue<int>(scaleAnnotationValue, XmlConstants.CsdlScaleAttributeName);
            }

            return null;
        }

        /// <summary>
        /// Find an annotation with the specified key and return its value as object.
        /// </summary>
        /// <param name="annotations">The annotations to search.</param>
        /// <param name="key">The name of the annotation to search for.</param>
        /// <param name="value">The value of the annotation with the specified key; 'null' if no annotation is found.</param>
        /// <returns>true if an annotation with the specified key was found and removed; otherwise false.</returns>
        private static bool TryFindAndRemoveAnnotation(List<KeyValuePair<string, object>> annotations, string key, out object value)
        {
            Debug.Assert(annotations != null, "annotations != null");
            Debug.Assert(!string.IsNullOrEmpty(key), "!string.IsNullOrEmpty(key)");

            for (int i = 0; i < annotations.Count; ++i)
            {
                if (string.CompareOrdinal(annotations[i].Key, key) == 0)
                {
                    value = annotations[i].Value;
                    annotations.RemoveAt(i);
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Tries to convert an annotation value to the specified target type.
        /// </summary>
        /// <typeparam name="T">The target type to convert the <paramref name="annotationValue"/> to.</typeparam>
        /// <param name="annotationValue">The annotation value to convert.</param>
        /// <param name="convertedValue">The converted annotation value or 'null' if no conversion was possible.</param>
        /// <returns>true if the value could be converted to the target type; otherwise false.</returns>
        /// <remarks>We only support the target types needed by the facets; this is not a general purpose conversion method.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intentionally catching all exceptions since this method should never throw but return false instead.")]
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "We're calling this correctly")]
        private static bool TryConvertAnnotationValue<T>(object annotationValue, out T convertedValue)
        {
            Type targetType = typeof(T);
            bool targetTypeAllowsNullable = WebUtil.TypeAllowsNull(targetType);
#if DEBUG
            Type nonNullableTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            Debug.Assert(nonNullableTargetType == targetType, "nonNullableTargetType == targetType");
#endif

            // Handle nullability first
            if (annotationValue == null)
            {
                if (targetTypeAllowsNullable)
                {
                    convertedValue = (T)annotationValue;
                    return true;
                }
                else
                {
                    convertedValue = default(T);
                    return false;
                }
            }

            IConvertible convertible = annotationValue as IConvertible;
            if (convertible != null)
            {
                try
                {
                    switch (Type.GetTypeCode(targetType))
                    {
                        case TypeCode.Boolean:  // fall through
                        case TypeCode.String:   // fall through
                        case TypeCode.Int32:
                            convertedValue = (T)Convert.ChangeType(convertible, targetType, CultureInfo.CurrentCulture);
                            return true;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    if (!CommonUtil.IsCatchableExceptionType(e))
                    {
                        throw;
                    }

                    // ignore the exception because we will report the failed attempt
                    // to convert the value below.
                }
            }

            convertedValue = default(T);
            return false;
        }

        /// <summary>
        /// Converts an annotation value to the specified target type.
        /// </summary>
        /// <typeparam name="T">The target type to convert the <paramref name="annotationValue"/> to.</typeparam>
        /// <param name="annotationValue">The annotation value to convert.</param>
        /// <param name="facetName">The name of the facet for which the value is converted; for error reporting only.</param>
        /// <returns>The converted annotation value.</returns>
        private static T ConvertAnnotationValue<T>(object annotationValue, string facetName)
        {
            T convertedValue;
            if (!TryConvertAnnotationValue(annotationValue, out convertedValue))
            {
                string typeName = annotationValue == null ? "null" : annotationValue.GetType().FullName;
                throw new FormatException(Service.Strings.MetadataProviderUtils_ConversionError(facetName, typeName, typeof(T).FullName));
            }

            return convertedValue;
        }

        /// <summary>
        /// Converts an annotation value to a string representation for the default value facet.
        /// </summary>
        /// <param name="annotationValue">The annotation value to convert.</param>
        /// <returns>The converted annotation value.</returns>
        private static string ConvertDefaultValue(object annotationValue)
        {
            if (annotationValue == null)
            {
                return null;
            }

            Type type = annotationValue.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    // We need to special case the boolean facets, since ToString returns True and False as values
                    // and for xml, they need to be lower case
                    return (bool)annotationValue ? XmlConstants.XmlTrueLiteral : XmlConstants.XmlFalseLiteral;

                case TypeCode.Int32:
                    return XmlConvert.ToString((int)annotationValue);

                case TypeCode.Byte:
                    return XmlConvert.ToString((byte)annotationValue);

                case TypeCode.DateTime:
                    return XmlConvert.ToString((DateTime)annotationValue, "yyyy-MM-dd HH:mm:ss.fffZ");

                default:
                    if (type.IsEnum)
                    {
                        return annotationValue.ToString();
                    }
                    else if (type == typeof(byte[]))
                    {
                        // Convert the bytes to a hex string in the format, e.g. "0x123"
                        string bytesAsHexString = string.Concat(((byte[])annotationValue).Select(b => b.ToString("X2", CultureInfo.InvariantCulture)));
                        return "0x" + bytesAsHexString;
                    }
                    else
                    {
                        return annotationValue.ToString();
                    }
            }
        }
    }
}
