//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Edm.Validation.Internal;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl
{
    using System.Diagnostics;

    /// <summary>
    /// Represents whether a vocabulary annotation should be serialized within the element it applies to or in a seperate section of the CSDL.
    /// </summary>
    public enum EdmVocabularyAnnotationSerializationLocation
    {
        /// <summary>
        /// The annotation should be serialized within the element being annotated.
        /// </summary>
        Inline,

        /// <summary>
        /// The annotation should be serialized in a seperate section.
        /// </summary>
        OutOfLine
    }

    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces that are useful to serialization.
    /// </summary>
    public static class SerializationExtensionMethods
    {
        #region Private Constants

        private const char AssociationNameEscapeChar = '_';
        private const string AssociationNameEscapeString = "_";
        private const string AssociationNameEscapeStringEscaped = "__";

        #endregion

        #region EdmxVersion

        /// <summary>
        /// Gets the value for the EDMX version of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">Model the version has been set for.</param>
        /// <returns>The version.</returns>
        public static Version GetEdmxVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<Version>(model, EdmConstants.InternalUri, CsdlConstants.EdmxVersionAnnotation);
        }

        /// <summary>
        /// Sets a value of EDMX version attribute of the <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model the version should be set for.</param>
        /// <param name="version">The version.</param>
        public static void SetEdmxVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(model, EdmConstants.InternalUri, CsdlConstants.EdmxVersionAnnotation, version);
        }

        #endregion

        #region NamespacePrefixMappings

        /// <summary>
        /// Sets an annotation on the IEdmModel to notify the serializer of preferred prefix mappings for xml namespaces.
        /// </summary>
        /// <param name="model">Reference to the calling object.</param>
        /// <param name="mappings">XmlNamespaceManage containing mappings between namespace prefixes and xml namespaces.</param>
        public static void SetNamespacePrefixMappings(this IEdmModel model, IEnumerable<KeyValuePair<string, string>> mappings)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(model, EdmConstants.InternalUri, CsdlConstants.NamespacePrefixAnnotation, mappings);
        }
        
        /// <summary>
        /// Gets the preferred prefix mappings for xml namespaces from an IEdmModel
        /// </summary>
        /// <param name="model">Reference to the calling object.</param>
        /// <returns>Namespace prefixes that exist on the model.</returns>
        public static IEnumerable<KeyValuePair<string, string>> GetNamespacePrefixMappings(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<IEnumerable<KeyValuePair<string, string>>>(model, EdmConstants.InternalUri, CsdlConstants.NamespacePrefixAnnotation);
        }

        #endregion

        #region DataServicesVersion

        /// <summary>
        /// Sets a value for the DataServiceVersion attribute in an EDMX artifact.
        /// </summary>
        /// <param name="model">The model the attribute should be set for.</param>
        /// <param name="version">The value of the attribute.</param>
        public static void SetDataServiceVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(model, EdmConstants.InternalUri, EdmConstants.DataServiceVersion, version);
        }

        /// <summary>
        /// Gets the value for the DataServiceVersion attribute used during EDMX serialization.
        /// </summary>
        /// <param name="model">Model the attribute has been set for.</param>
        /// <returns>Value of the attribute.</returns>
        public static Version GetDataServiceVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<Version>(model, EdmConstants.InternalUri, EdmConstants.DataServiceVersion);
        }

        #endregion

        #region MaxDataServicesVersion

        /// <summary>
        /// Sets a value for the MaxDataServiceVersion attribute in an EDMX artifact.
        /// </summary>
        /// <param name="model">The model the attribute should be set for.</param>
        /// <param name="version">The value of the attribute.</param>
        public static void SetMaxDataServiceVersion(this IEdmModel model, Version version)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(model, EdmConstants.InternalUri, EdmConstants.MaxDataServiceVersion, version);
        }

        /// <summary>
        /// Gets the value for the MaxDataServiceVersion attribute used during EDMX serialization.
        /// </summary>
        /// <param name="model">Model the attribute has been set for</param>
        /// <returns>Value of the attribute.</returns>
        public static Version GetMaxDataServiceVersion(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<Version>(model, EdmConstants.InternalUri, EdmConstants.MaxDataServiceVersion);
        }

        #endregion

        #region SerializationLocation

        /// <summary>
        /// Sets the location an annotation should be serialized in.
        /// </summary>
        /// <param name="annotation">The annotation the location is being specified for.</param>
        /// <param name="model">Model containing the annotation.</param>
        /// <param name="location">The location the annotation should appear.</param>
        public static void SetSerializationLocation(this IEdmVocabularyAnnotation annotation, IEdmModel model, EdmVocabularyAnnotationSerializationLocation? location)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            EdmUtil.CheckArgumentNull(model, "model");

            model.SetAnnotationValue(annotation, EdmConstants.InternalUri, CsdlConstants.AnnotationSerializationLocationAnnotation, (object)location);
        }

        /// <summary>
        /// Gets the location an annotation should be serialized in.
        /// </summary>
        /// <param name="annotation">Reference to the calling annotation.</param>
        /// <param name="model">Model containing the annotation.</param>
        /// <returns>The location the annotation should be serialized at.</returns>
        public static EdmVocabularyAnnotationSerializationLocation? GetSerializationLocation(this IEdmVocabularyAnnotation annotation, IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            EdmUtil.CheckArgumentNull(model, "model");

            return model.GetAnnotationValue(annotation, EdmConstants.InternalUri, CsdlConstants.AnnotationSerializationLocationAnnotation) as EdmVocabularyAnnotationSerializationLocation?;
        }

        #endregion

        #region SchemaNamespace

        /// <summary>
        /// Sets the schema an annotation should appear in.
        /// </summary>
        /// <param name="annotation">The annotation the schema should be set for.</param>
        /// <param name="model">Model containing the annotation.</param>
        /// <param name="schemaNamespace">The schema the annotation belongs in.</param>
        public static void SetSchemaNamespace(this IEdmVocabularyAnnotation annotation, IEdmModel model, string schemaNamespace)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(annotation, EdmConstants.InternalUri, CsdlConstants.SchemaNamespaceAnnotation, schemaNamespace);
        }

        /// <summary>
        /// Gets the schema an annotation should be serialized in.
        /// </summary>
        /// <param name="annotation">Reference to the calling annotation.</param>
        /// <param name="model">Model containing the annotation.</param>
        /// <returns>Name of the schema the annotation belongs to.</returns>
        public static string GetSchemaNamespace(this IEdmVocabularyAnnotation annotation, IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(annotation, "annotation");
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<string>(annotation, EdmConstants.InternalUri, CsdlConstants.SchemaNamespaceAnnotation);
        }

        #endregion

        #region Model

        /// <summary>
        /// Sets the name used for the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="associationName">The association name.</param>
        public static void SetAssociationName(this IEdmModel model, IEdmNavigationProperty property, string associationName)
        {
            EdmUtil.CheckArgumentNull(model, "model"); 
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(associationName, "associationName");

            model.SetAnnotationValue(property, EdmConstants.InternalUri, CsdlConstants.AssociationNameAnnotation, associationName);
        }

        /// <summary>
        /// Gets the name used for the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <returns>The association name.</returns>
        public static string GetAssociationName(this IEdmModel model, IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");

            property.PopulateCaches();
            string associationName = model.GetAnnotationValue<string>(property, EdmConstants.InternalUri, CsdlConstants.AssociationNameAnnotation);
            if (associationName == null)
            {
                IEdmNavigationProperty fromPrincipal = property.GetPrimary();
                IEdmNavigationProperty toPrincipal = fromPrincipal.Partner;

                associationName =
                    GetQualifiedAndEscapedPropertyName(toPrincipal) +
                    AssociationNameEscapeChar +
                    GetQualifiedAndEscapedPropertyName(fromPrincipal);
            }
            
            return associationName;
        }

        /// <summary>
        /// Sets the namespace used for the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="associationNamespace">The association namespace.</param>
        public static void SetAssociationNamespace(this IEdmModel model, IEdmNavigationProperty property, string associationNamespace)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");
            EdmUtil.CheckArgumentNull(associationNamespace, "associationNamespace");

            model.SetAnnotationValue(property, EdmConstants.InternalUri, CsdlConstants.AssociationNamespaceAnnotation, associationNamespace);
        }

        /// <summary>
        /// Gets the namespace used for the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <returns>The association namespace.</returns>
        public static string GetAssociationNamespace(this IEdmModel model, IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");
            
            property.PopulateCaches();
            string associationNamespace = model.GetAnnotationValue<string>(property, EdmConstants.InternalUri, CsdlConstants.AssociationNamespaceAnnotation);
            if (associationNamespace == null)
            {
                associationNamespace = property.GetPrimary().DeclaringEntityType().Namespace;
            }

            return associationNamespace;
        }

        /// <summary>
        /// Gets the fully-qualified name used for the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <returns>The fully-qualified association name.</returns>
        public static string GetAssociationFullName(this IEdmModel model, IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");
            
            property.PopulateCaches();
            return model.GetAssociationNamespace(property) + "." + model.GetAssociationName(property);
        }

        /// <summary>
        /// Sets the annotations for the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="annotations">The association annotations.</param>
        /// <param name="end1Annotations">The annotations for association end 1.</param>
        /// <param name="end2Annotations">The annotations for association end 2.</param>
        /// <param name="constraintAnnotations">The annotations for the referential constraint.</param>
        public static void SetAssociationAnnotations(this IEdmModel model, IEdmNavigationProperty property, IEnumerable<IEdmDirectValueAnnotation> annotations, IEnumerable<IEdmDirectValueAnnotation> end1Annotations, IEnumerable<IEdmDirectValueAnnotation> end2Annotations, IEnumerable<IEdmDirectValueAnnotation> constraintAnnotations)
        {
            EdmUtil.CheckArgumentNull(model, "model"); 
            EdmUtil.CheckArgumentNull(property, "property");

            if ((annotations != null && annotations.FirstOrDefault() != null) || (end1Annotations != null && end1Annotations.FirstOrDefault() != null) || (end2Annotations != null && end2Annotations.FirstOrDefault() != null) || (constraintAnnotations != null && constraintAnnotations.FirstOrDefault() != null))
            {
                model.SetAnnotationValue(property, EdmConstants.InternalUri, CsdlConstants.AssociationAnnotationsAnnotation, new AssociationAnnotations { Annotations = annotations, End1Annotations = end1Annotations, End2Annotations = end2Annotations, ConstraintAnnotations = constraintAnnotations });
            }
        }

        /// <summary>
        /// Gets the annotations associated with the association serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="annotations">The association annotations.</param>
        /// <param name="end1Annotations">The annotations for association end 1.</param>
        /// <param name="end2Annotations">The annotations for association end 2.</param>
        /// <param name="constraintAnnotations">The annotations for the referential constraint.</param>
        public static void GetAssociationAnnotations(this IEdmModel model, IEdmNavigationProperty property, out IEnumerable<IEdmDirectValueAnnotation> annotations, out IEnumerable<IEdmDirectValueAnnotation> end1Annotations, out IEnumerable<IEdmDirectValueAnnotation> end2Annotations, out IEnumerable<IEdmDirectValueAnnotation> constraintAnnotations)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");
            
            property.PopulateCaches();
            AssociationAnnotations associationAnnotations = model.GetAnnotationValue<AssociationAnnotations>(property, EdmConstants.InternalUri, CsdlConstants.AssociationAnnotationsAnnotation);
            if (associationAnnotations != null)
            {
                annotations = associationAnnotations.Annotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
                end1Annotations = associationAnnotations.End1Annotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
                end2Annotations = associationAnnotations.End2Annotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
                constraintAnnotations = associationAnnotations.ConstraintAnnotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
            }
            else
            {
                IEnumerable<IEdmDirectValueAnnotation> empty = Enumerable.Empty<IEdmDirectValueAnnotation>();
                annotations = empty;
                end1Annotations = empty;
                end2Annotations = empty;
                constraintAnnotations = empty;
            }
        }

        /// <summary>
        /// Sets the name used for the association end serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="association">The association end name.</param>
        public static void SetAssociationEndName(this IEdmModel model, IEdmNavigationProperty property, string association)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");
            
            model.SetAnnotationValue(property, EdmConstants.InternalUri, CsdlConstants.AssociationEndNameAnnotation, association);
        }

        /// <summary>
        /// Gets the name used for the association end serialized for a navigation property.
        /// </summary>
        /// <param name="model">Model containing the navigation property.</param>
        /// <param name="property">The navigation property.</param>
        /// <returns>The association end name.</returns>
        public static string GetAssociationEndName(this IEdmModel model, IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(property, "property");
            
            property.PopulateCaches();
            return model.GetAnnotationValue<string>(property, EdmConstants.InternalUri, CsdlConstants.AssociationEndNameAnnotation) ?? property.Partner.Name;
        }

        /// <summary>
        /// Sets the name used for the association set serialized for a navigation property of an entity set.
        /// </summary>
        /// <param name="model">Model containing the entity set.</param>
        /// <param name="entitySet">The entity set</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="associationSet">The association set name.</param>
        public static void SetAssociationSetName(this IEdmModel model, IEdmEntitySet entitySet, IEdmNavigationProperty property, string associationSet)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(entitySet, "entitySet");
            EdmUtil.CheckArgumentNull(property, "property");
            
            Dictionary<IEdmNavigationProperty, string> navigationPropertyMappings = model.GetAnnotationValue<Dictionary<IEdmNavigationProperty, string>>(entitySet, EdmConstants.InternalUri, CsdlConstants.AssociationSetNameAnnotation);
            if (navigationPropertyMappings == null)
            {
                navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, string>(EdmNavigationPropertyHashComparer.Instance);
                model.SetAnnotationValue(entitySet, EdmConstants.InternalUri, CsdlConstants.AssociationSetNameAnnotation, navigationPropertyMappings);
            }

            navigationPropertyMappings[property] = associationSet;
        }

        /// <summary>
        /// Gets the name used for the association set serialized for a navigation property of an entity set.
        /// </summary>
        /// <param name="model">Model containing the entity set.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="property">The navigation property.</param>
        /// <returns>The association set name.</returns>
        public static string GetAssociationSetName(this IEdmModel model, IEdmEntitySet entitySet, IEdmNavigationProperty property)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(entitySet, "entitySet");
            EdmUtil.CheckArgumentNull(property, "property");
            
            string associationSetName;
            Dictionary<IEdmNavigationProperty, string> navigationPropertyMappings = model.GetAnnotationValue<Dictionary<IEdmNavigationProperty, string>>(entitySet, EdmConstants.InternalUri, CsdlConstants.AssociationSetNameAnnotation);
            if (navigationPropertyMappings == null || !navigationPropertyMappings.TryGetValue(property, out associationSetName))
            {
                associationSetName = model.GetAssociationName(property) + "Set";
            }

            return associationSetName;
        }

        /// <summary>
        /// Sets the annotations for the association set serialized for a navigation target of an entity set.
        /// </summary>
        /// <param name="model">Model containing the entity set.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="annotations">The association set annotations.</param>
        /// <param name="end1Annotations">The annotations for association set end 1.</param>
        /// <param name="end2Annotations">The annotations for association set end 2.</param>
        public static void SetAssociationSetAnnotations(this IEdmModel model, IEdmEntitySet entitySet, IEdmNavigationProperty property, IEnumerable<IEdmDirectValueAnnotation> annotations, IEnumerable<IEdmDirectValueAnnotation> end1Annotations, IEnumerable<IEdmDirectValueAnnotation> end2Annotations)
        {
            EdmUtil.CheckArgumentNull(model, "model"); 
            EdmUtil.CheckArgumentNull(entitySet, "property");
            EdmUtil.CheckArgumentNull(property, "property");

            if ((annotations != null && annotations.FirstOrDefault() != null) || (end1Annotations != null && end1Annotations.FirstOrDefault() != null) || (end2Annotations != null && end2Annotations.FirstOrDefault() != null))
            {
                Dictionary<IEdmNavigationProperty, AssociationSetAnnotations> navigationPropertyMappings = model.GetAnnotationValue<Dictionary<IEdmNavigationProperty, AssociationSetAnnotations>>(entitySet, EdmConstants.InternalUri, CsdlConstants.AssociationSetAnnotationsAnnotation);
                if (navigationPropertyMappings == null)
                {
                    navigationPropertyMappings = new Dictionary<IEdmNavigationProperty, AssociationSetAnnotations>(EdmNavigationPropertyHashComparer.Instance);
                    model.SetAnnotationValue(entitySet, EdmConstants.InternalUri, CsdlConstants.AssociationSetAnnotationsAnnotation, navigationPropertyMappings);
                }

                navigationPropertyMappings[property] = new AssociationSetAnnotations { Annotations = annotations, End1Annotations = end1Annotations, End2Annotations = end2Annotations };
            }
        }

        /// <summary>
        /// Gets the annotations associated with the association serialized for a navigation target of an entity set.
        /// </summary>
        /// <param name="model">Model containing the entity set.</param>
        /// <param name="entitySet">The entity set.</param>
        /// <param name="property">The navigation property.</param>
        /// <param name="annotations">The association set annotations.</param>
        /// <param name="end1Annotations">The annotations for association set end 1.</param>
        /// <param name="end2Annotations">The annotations for association set end 2.</param>
        public static void GetAssociationSetAnnotations(this IEdmModel model, IEdmEntitySet entitySet, IEdmNavigationProperty property, out IEnumerable<IEdmDirectValueAnnotation> annotations, out IEnumerable<IEdmDirectValueAnnotation> end1Annotations, out IEnumerable<IEdmDirectValueAnnotation> end2Annotations)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            EdmUtil.CheckArgumentNull(entitySet, "entitySet");
            EdmUtil.CheckArgumentNull(property, "property");
            
            AssociationSetAnnotations associationSetAnnotations;
            Dictionary<IEdmNavigationProperty, AssociationSetAnnotations> navigationPropertyMappings = model.GetAnnotationValue<Dictionary<IEdmNavigationProperty, AssociationSetAnnotations>>(entitySet, EdmConstants.InternalUri, CsdlConstants.AssociationSetAnnotationsAnnotation);
            if (navigationPropertyMappings != null && navigationPropertyMappings.TryGetValue(property, out associationSetAnnotations))
            {
                annotations = associationSetAnnotations.Annotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
                end1Annotations = associationSetAnnotations.End1Annotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
                end2Annotations = associationSetAnnotations.End2Annotations ?? Enumerable.Empty<IEdmDirectValueAnnotation>();
            }
            else
            {
                IEnumerable<IEdmDirectValueAnnotation> empty = Enumerable.Empty<IEdmDirectValueAnnotation>();
                annotations = empty;
                end1Annotations = empty;
                end2Annotations = empty;
            }
        }

        #endregion

        #region NavigationProperty
        
        /// <summary>
        /// Gets the primary end of a pair of partnered navigation properties, selecting the principal end if there is one and making a stable, arbitrary choice otherwise.
        /// </summary>
        /// <param name="property">The navigation property.</param>
        /// <returns>The primary end between the navigation property and its partner.</returns>
        public static IEdmNavigationProperty GetPrimary(this IEdmNavigationProperty property)
        {
            if (property.IsPrincipal)
            {
                return property;
            }

            IEdmNavigationProperty partner = property.Partner;
            if (partner.IsPrincipal)
            {
                return partner;
            }

            // There is no meaningful basis for determining which of the two partners is principal, so break the tie with an arbitrary, stable comparision.
            int nameComparison = string.Compare(property.Name, partner.Name, StringComparison.Ordinal);
            if (nameComparison == 0)
            {
                nameComparison = string.Compare(property.DeclaringEntityType().FullName(), partner.DeclaringEntityType().FullName(), StringComparison.Ordinal);
            }

            return nameComparison > 0 ? property : partner;
        }

        #endregion

        #region IsValueExplicit

        /// <summary>
        /// Sets an annotation indicating whether the value of an enum member should be explicitly serialized.
        /// </summary>
        /// <param name="member">Member to set the annotation on.</param>
        /// <param name="model">Model containing the member.</param>
        /// <param name="isExplicit">If the value of the enum member should be explicitly serialized</param>
        public static void SetIsValueExplicit(this IEdmEnumMember member, IEdmModel model, bool? isExplicit)
        {
            EdmUtil.CheckArgumentNull(member, "member");
            EdmUtil.CheckArgumentNull(model, "model");
            model.SetAnnotationValue(member, EdmConstants.InternalUri, CsdlConstants.IsEnumMemberValueExplicitAnnotation, (object)isExplicit);
        }

        /// <summary>
        /// Gets an annotation indicating whether the value of an enum member should be explicitly serialized.
        /// </summary>
        /// <param name="member">The member the annotation is on.</param>
        /// <param name="model">Model containing the member.</param>
        /// <returns>Whether the member should have its value serialized.</returns>
        public static bool? IsValueExplicit(this IEdmEnumMember member, IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(member, "member");
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue(member, EdmConstants.InternalUri, CsdlConstants.IsEnumMemberValueExplicitAnnotation) as bool?;
        }

        #endregion

        #region IsSerializedAsElement

        /// <summary>
        /// Sets an annotation indicating if the value should be serialized as an element.
        /// </summary>
        /// <param name="value">Value to set the annotation on.</param>
        /// <param name="model">Model containing the value.</param>
        /// <param name="isSerializedAsElement">Value indicating if the value should be serialized as an element.</param>
        public static void SetIsSerializedAsElement(this IEdmValue value, IEdmModel model, bool isSerializedAsElement)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            EdmUtil.CheckArgumentNull(model, "model");
            if (isSerializedAsElement)
            {
                EdmError error;
                if (!ValidationHelper.ValidateValueCanBeWrittenAsXmlElementAnnotation(value, null, null, out error))
                {
                     throw new InvalidOperationException(error.ToString());
                }
            }

            model.SetAnnotationValue(value, EdmConstants.InternalUri, CsdlConstants.IsSerializedAsElementAnnotation, (object)isSerializedAsElement);
        }

        /// <summary>
        /// Gets an annotation indicating if the value should be serialized as an element.
        /// </summary>
        /// <param name="value">Value the annotation is on.</param>
        /// <param name="model">Model containing the value.</param>
        /// <returns>Value indicating if the string should be serialized as an element.</returns>
        public static bool IsSerializedAsElement(this IEdmValue value, IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(value, "value");
            EdmUtil.CheckArgumentNull(model, "model");
            return (model.GetAnnotationValue(value, EdmConstants.InternalUri, CsdlConstants.IsSerializedAsElementAnnotation) as bool?) ?? false;
        }

        #endregion

        #region NamespaceAliases

        /// <summary>
        /// Sets the serialization alias for a given namespace
        /// </summary>
        /// <param name="model">Model that will be serialized.</param>
        /// <param name="namespaceName">The namespace to set the alias for.</param>
        /// <param name="alias">The alias for that namespace.</param>
        public static void SetNamespaceAlias(this IEdmModel model, string namespaceName, string alias)
        {
            VersioningDictionary<string, string> mappings = model.GetAnnotationValue<VersioningDictionary<string, string>>(model, EdmConstants.InternalUri, CsdlConstants.NamespaceAliasAnnotation);
            if (mappings == null)
            {
                mappings = VersioningDictionary<string, string>.Create(string.CompareOrdinal);
            }

            if (alias == null)
            {
                mappings = mappings.Remove(namespaceName);
            }
            else
            {
                mappings = mappings.Set(namespaceName, alias);
            }

            model.SetAnnotationValue(model, EdmConstants.InternalUri, CsdlConstants.NamespaceAliasAnnotation, mappings);
        }

        /// <summary>
        /// Gets the serialization alias for a given namespace.
        /// </summary>
        /// <param name="model">Model that will be serialized.</param>
        /// <param name="namespaceName">Namespace the alias is needed for.</param>
        /// <returns>The alias of the given namespace, or null if one does not exist.</returns>
        public static string GetNamespaceAlias(this IEdmModel model, string namespaceName)
        {
            VersioningDictionary<string, string> mappings = model.GetAnnotationValue<VersioningDictionary<string, string>>(model, EdmConstants.InternalUri, CsdlConstants.NamespaceAliasAnnotation);
            return mappings.Get(namespaceName);
        }

        // This internal method exists so we can get a consistent view of the mappings through the entire serialization process. 
        // Otherwise, changes to the dictionary durring serialization would result in an invalid or inconsistent output.
        internal static VersioningDictionary<string, string> GetNamespaceAliases(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<VersioningDictionary<string, string>>(model, EdmConstants.InternalUri, CsdlConstants.NamespaceAliasAnnotation);
        }

        #endregion

        internal static bool IsInline(this IEdmVocabularyAnnotation annotation, IEdmModel model)
        {
            return annotation.GetSerializationLocation(model) == EdmVocabularyAnnotationSerializationLocation.Inline || annotation.TargetString() == null;
        }

        internal static string TargetString(this IEdmVocabularyAnnotation annotation)
        {
            return EdmUtil.FullyQualifiedName(annotation.Target);
        }

        private static void PopulateCaches(this IEdmNavigationProperty property)
        {
            // Force computation that can apply annotations to the navigation property.
            IEdmNavigationProperty partner = property.Partner;
            bool isPrincipal = property.IsPrincipal;
            IEnumerable<IEdmStructuralProperty> dependentProperties = property.DependentProperties;
        }

        private static string GetQualifiedAndEscapedPropertyName(IEdmNavigationProperty property)
        {
            return
                EscapeName(property.DeclaringEntityType().Namespace).Replace('.', AssociationNameEscapeChar) +
                AssociationNameEscapeChar +
                EscapeName(property.DeclaringEntityType().Name) +
                AssociationNameEscapeChar +
                EscapeName(property.Name);
        }

        private static string EscapeName(string name)
        {
            return name.Replace(AssociationNameEscapeString, AssociationNameEscapeStringEscaped);
        }

        private class AssociationAnnotations
        {
            public IEnumerable<IEdmDirectValueAnnotation> Annotations { get; set; }

            public IEnumerable<IEdmDirectValueAnnotation> End1Annotations { get; set; }

            public IEnumerable<IEdmDirectValueAnnotation> End2Annotations { get; set; }

            public IEnumerable<IEdmDirectValueAnnotation> ConstraintAnnotations { get; set; }
        }

        private class AssociationSetAnnotations
        {
            public IEnumerable<IEdmDirectValueAnnotation> Annotations { get; set; }

            public IEnumerable<IEdmDirectValueAnnotation> End1Annotations { get; set; }

            public IEnumerable<IEdmDirectValueAnnotation> End2Annotations { get; set; }
        }

        private class EdmNavigationPropertyHashComparer : EqualityComparer<IEdmNavigationProperty>
        {
            private static EdmNavigationPropertyHashComparer instance = new EdmNavigationPropertyHashComparer();

            private EdmNavigationPropertyHashComparer()
            {
            }

            internal static EdmNavigationPropertyHashComparer Instance
            {
                get { return instance; }
            }

            public override bool Equals(IEdmNavigationProperty left, IEdmNavigationProperty right)
            {
                Debug.Assert(right != null, "right != null");
                Debug.Assert(left != null, "left != null");

                string rightHash = GenerateHash(right);
                string leftHash = GenerateHash(left);
                return rightHash == leftHash;
            }

            public override int GetHashCode(IEdmNavigationProperty obj)
            {
                Debug.Assert(obj != null, "obj != null");

                string hash = GenerateHash(obj);
                return hash.GetHashCode();
            }

            private static string GenerateHash(IEdmNavigationProperty prop)
            {
                Debug.Assert(prop.DeclaringType is IEdmEntityType, "DeclaringType must be EntityType to get its name.");
                return prop.Name + "_" + prop.DeclaringEntityType().FullName();
            }
        }
    }
}
