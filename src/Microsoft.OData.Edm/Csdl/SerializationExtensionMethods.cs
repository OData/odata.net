//---------------------------------------------------------------------
// <copyright file="SerializationExtensionMethods.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl
{
    /// <summary>
    /// Represents whether a vocabulary annotation should be serialized within the element it applies to or in a separate section of the CSDL.
    /// </summary>
    public enum EdmVocabularyAnnotationSerializationLocation
    {
        /// <summary>
        /// The annotation should be serialized within the element being annotated.
        /// </summary>
        Inline,

        /// <summary>
        /// The annotation should be serialized in a separate section.
        /// </summary>
        OutOfLine
    }

    /// <summary>
    /// Contains extension methods for <see cref="IEdmModel"/> interfaces that are useful to serialization.
    /// </summary>
    public static class SerializationExtensionMethods
    {
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
        /// Sets the serialization alias for a given namespace(including current model's schemas namespace-alias, and referenced models' schemas namespace-alias)
        /// TODO: REF make sure no duplicated alias.
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

            if (EdmUtil.IsNullOrWhiteSpaceInternal(alias))
            {
                string val;
                if (mappings.TryGetValue(namespaceName, out val))
                {
                    mappings = mappings.Remove(namespaceName);
                }
            }
            else
            {
                mappings = mappings.Set(namespaceName, alias);
            }

            model.SetAnnotationValue(model, EdmConstants.InternalUri, CsdlConstants.NamespaceAliasAnnotation, mappings);

            var list = model.GetAnnotationValue<VersioningList<string>>(model, EdmConstants.InternalUri, CsdlConstants.UsedNamespacesAnnotation);
            if (list == null)
            {
                list = VersioningList<string>.Create();
            }

            if (!string.IsNullOrEmpty(namespaceName) && !list.Contains(namespaceName))
            {
                list = list.Add(namespaceName);
            }

            model.SetAnnotationValue(model, EdmConstants.InternalUri, CsdlConstants.UsedNamespacesAnnotation, list);
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

        /// <summary>
        /// Gets the namespaces in all schemas having alias, excluding those without alias.
        /// </summary>
        /// <param name="model">The IEdmModel.</param>
        /// <returns>The namespaces in all schemas having alias.</returns>
        internal static VersioningList<string> GetUsedNamespacesHavingAlias(this IEdmModel model)
        {
            EdmUtil.CheckArgumentNull(model, "model");
            return model.GetAnnotationValue<VersioningList<string>>(model, EdmConstants.InternalUri, CsdlConstants.UsedNamespacesAnnotation);
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
    }
}
