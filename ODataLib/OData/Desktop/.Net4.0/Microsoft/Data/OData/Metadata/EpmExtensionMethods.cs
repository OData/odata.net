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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;
    using Microsoft.Data.Edm.Library.Values;
    using Microsoft.Data.Edm.Values;
    using Microsoft.Data.OData.Atom;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Extension methods to make it easier to work with EPM.
    /// </summary>
    internal static class EpmExtensionMethods
    {
        /// <summary>
        /// All supported base names for serializable EPM annotations.
        /// </summary>
        private static readonly string[] EpmAnnotationBaseNames = new string[]
        {
            EpmConstants.ODataEpmTargetPath,
            EpmConstants.ODataEpmSourcePath,
            EpmConstants.ODataEpmKeepInContent,
            EpmConstants.ODataEpmContentKind,
            EpmConstants.ODataEpmNsUri,
            EpmConstants.ODataEpmNsPrefix,
        };

        /// <summary>
        /// FC_TargetPath to <see cref="SyndicationItemProperty"/> enum mapping.
        /// </summary>
        private static readonly Dictionary<string, SyndicationItemProperty> TargetPathToSyndicationItemMap = 
            new Dictionary<string, SyndicationItemProperty>(StringComparer.OrdinalIgnoreCase)
        {
            { EpmConstants.ODataSyndItemAuthorEmail, SyndicationItemProperty.AuthorEmail },
            { EpmConstants.ODataSyndItemAuthorName, SyndicationItemProperty.AuthorName },
            { EpmConstants.ODataSyndItemAuthorUri, SyndicationItemProperty.AuthorUri },
            { EpmConstants.ODataSyndItemContributorEmail, SyndicationItemProperty.ContributorEmail },
            { EpmConstants.ODataSyndItemContributorName, SyndicationItemProperty.ContributorName },
            { EpmConstants.ODataSyndItemContributorUri, SyndicationItemProperty.ContributorUri },
            { EpmConstants.ODataSyndItemUpdated, SyndicationItemProperty.Updated },
            { EpmConstants.ODataSyndItemPublished, SyndicationItemProperty.Published },
            { EpmConstants.ODataSyndItemRights, SyndicationItemProperty.Rights },
            { EpmConstants.ODataSyndItemSummary, SyndicationItemProperty.Summary },
            { EpmConstants.ODataSyndItemTitle, SyndicationItemProperty.Title },
        };

        /// <summary>
        /// Ensures that an up-to-date EPM cache exists for the specified <paramref name="entityType"/>. 
        /// If no cache exists, a new one will be created based on the public mappings (if any).
        /// If the public mappings have changed (and the cache is thus dirty), the method re-constructs the cache.
        /// If all public mappings have been removed, the method also removes the EPM cache.
        /// </summary>
        /// <param name="model">IEdmModel containing the annotations.</param>
        /// <param name="entityType">IEdmEntityType instance for which to ensure the EPM cache.</param>
        /// <param name="maxMappingCount">The maximum allowed number of entity property mappings 
        /// for a given entity type (on the type itself and all its base types).</param>
        /// <returns>An instance of <see cref="ODataEntityPropertyMappingCache"/>, if there are any EPM mappings for the given entity type, otherwise returns null.</returns>
        internal static ODataEntityPropertyMappingCache EnsureEpmCache(this IEdmModel model, IEdmEntityType entityType, int maxMappingCount)
        {
            DebugUtils.CheckNoExternalCallers();
            bool cacheModified;
            return EnsureEpmCacheInternal(model, entityType, maxMappingCount, out cacheModified);
        }

        /// <summary>
        /// Determines if the <paramref name="entityType"/> has any EPM defined on it (or its base types).
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="entityType">The entity type to test for presence of EPM.</param>
        /// <returns>true if the <paramref name="entityType"/> has EPM; false otherwise.</returns>
        internal static bool HasEntityPropertyMappings(this IEdmModel model, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityType != null, "entityType != null");

            // Walk the base types and determine if there's EPM anywhere in there.
            IEdmEntityType entityTypeToTest = entityType;
            while (entityTypeToTest != null)
            {
                if (model.GetEntityPropertyMappings(entityTypeToTest) != null)
                {
                    return true;
                }

                entityTypeToTest = entityTypeToTest.BaseEntityType();
            }

            return false;
        }

        /// <summary>
        /// Returns the EPM information for an entity type.
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="entityType">The entity type to get the EPM information for.</param>
        /// <returns>Returns the EPM information for an entity type. If there's no such information, this returns null.</returns>
        internal static ODataEntityPropertyMappingCollection GetEntityPropertyMappings(this IEdmModel model, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityType != null, "entityType != null");
           
            return model.GetAnnotationValue<ODataEntityPropertyMappingCollection>(entityType);
        }

        /// <summary>
        /// Returns the cached EPM information for an entity type.
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="entityType">The entity type to get the cached EPM information for.</param>
        /// <returns>Returns the cached EPM information for an entity type. If there's no cached information, this returns null.</returns>
        internal static ODataEntityPropertyMappingCache GetEpmCache(this IEdmModel model, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityType != null, "entityType != null");

            return model.GetAnnotationValue<ODataEntityPropertyMappingCache>(entityType);
        }

        /// <summary>
        /// Gets all the annotations bindings in order to remove all EPM related annotations from a given <see cref="IEdmElement"/>.
        /// </summary>
        /// <param name="model">The model containing the annotations.</param>
        /// <param name="annotatable">The annotatable to get the EPM annotations for.</param>
        /// <returns>A dictionary of local annotation name to annotation binding mappings for all serializable EPM annotations on <paramref name="annotatable"/>.</returns>
        internal static Dictionary<string, IEdmDirectValueAnnotationBinding> GetAnnotationBindingsToRemoveSerializableEpmAnnotations(
            this IEdmModel model, 
            IEdmElement annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");

            Dictionary<string, IEdmDirectValueAnnotationBinding> annotationBindingsToRemove = new Dictionary<string, IEdmDirectValueAnnotationBinding>(StringComparer.Ordinal);
            IEnumerable<IEdmDirectValueAnnotation> annotations = model.GetODataAnnotations(annotatable);
            if (annotations != null)
            {
                foreach (IEdmDirectValueAnnotation annotation in annotations)
                {
                    if (annotation.IsEpmAnnotation())
                    {
                        annotationBindingsToRemove.Add(
                            annotation.Name,
                            new EdmDirectValueAnnotationBinding(annotatable, annotation.NamespaceUri, annotation.Name, /*value*/ null));
                    }
                }
            }

            return annotationBindingsToRemove;
        }

        /// <summary>
        /// Removes the in-memory EPM annotations from an entity type; potentially also drops an existing EPM cache.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to remove the EPM annotation from.</param>
        internal static void ClearInMemoryEpmAnnotations(this IEdmModel model, IEdmElement annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");

            IEdmDirectValueAnnotationBinding[] annotationSetters = new IEdmDirectValueAnnotationBinding[2];

            // remove the in-memory annotation
            annotationSetters[0] = new EdmTypedDirectValueAnnotationBinding<ODataEntityPropertyMappingCollection>(annotatable, null);

            // remove the cache
            annotationSetters[1] = new EdmTypedDirectValueAnnotationBinding<ODataEntityPropertyMappingCache>(annotatable, null);

            model.SetAnnotationValues(annotationSetters);
        }

        /// <summary>
        /// Saves the EPM annotations on the given <paramref name="property"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="property">The <see cref="IEdmProperty"/> to save the EPM annotations for.</param>
        /// <param name="epmCache">The EPM cache for the owning entity type.</param>
        internal static void SaveEpmAnnotationsForProperty(this IEdmModel model, IEdmProperty property, ODataEntityPropertyMappingCache epmCache)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(epmCache != null, "epmCache != null");

            // get the mappings for the current property; this is based on the source path of the mapping
            string propertyName = property.Name;
            IEnumerable<EntityPropertyMappingAttribute> propertyMappings = epmCache.MappingsForDeclaredProperties.Where(
                m => m.SourcePath.StartsWith(propertyName, StringComparison.Ordinal) &&
                    (m.SourcePath.Length == propertyName.Length || m.SourcePath[propertyName.Length] == '/'));

            bool skipSourcePath;
            bool removePrefix;
            if (property.Type.IsODataPrimitiveTypeKind())
            {
                // Since only a single mapping from a primitive property can exist, it is fine to not write the source path.
                Debug.Assert(propertyMappings.Count() <= 1, "At most one entity property mapping can exist from a primitive property.");
                skipSourcePath = true;
                removePrefix = false;
            }
            else
            {
                Debug.Assert(
                    property.Type.IsODataComplexTypeKind() || property.Type.IsNonEntityCollectionType(),
                    "Only primitive, complex or collectionValue properties can have EPM defined on them.");

                removePrefix = true;
                skipSourcePath = propertyMappings.Any(m => m.SourcePath == propertyName);
                Debug.Assert(
                    !skipSourcePath || propertyMappings.Count() == 1,
                    "We must not have multiple mappings for a property if one of them matches the property name exactly (the other ones would not make sense).");
            }

            model.SaveEpmAnnotations(property, propertyMappings, skipSourcePath, removePrefix);
        }

        /// <summary>
        /// Saves the EPM annotations on the given <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to save the EPM annotations on.</param>
        /// <param name="mappings">All the EPM annotations to be saved.</param>
        /// <param name="skipSourcePath">true if the source path should be saved explicitly; otherwise false.</param>
        /// <param name="removePrefix">true if the prefix of the source path should be removed; otherwise false.</param>
        internal static void SaveEpmAnnotations(this IEdmModel model, IEdmElement annotatable, IEnumerable<EntityPropertyMappingAttribute> mappings, bool skipSourcePath, bool removePrefix)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(mappings != null, "mappings != null");
            Debug.Assert(!skipSourcePath || mappings.Count() <= 1, "Skipping the source path is only valid if at most a single mapping exists.");

            EpmAttributeNameBuilder epmAttributeNameBuilder = new EpmAttributeNameBuilder();

            // Get the annotation bindings for all serializable EPM annotations with a null value (which will remove them).
            // NOTE: we use a dictionary here and replace existing annotation bindings below to ensure we have at most
            //       one binding per annotation name; the default annotation manager will process annotations in order and thus
            //       the last binding for an annotation would win but custom annotation managers might not.
            Dictionary<string, IEdmDirectValueAnnotationBinding> epmAnnotationBindings = 
                GetAnnotationBindingsToRemoveSerializableEpmAnnotations(model, annotatable);

            string localName;
            foreach (EntityPropertyMappingAttribute mapping in mappings)
            {
                // Check whether the mapping represents a custom mapping
                if (mapping.TargetSyndicationItem == SyndicationItemProperty.CustomProperty)
                {
                    localName = epmAttributeNameBuilder.EpmTargetPath;
                    epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, mapping.TargetPath);

                    localName = epmAttributeNameBuilder.EpmNsUri;
                    epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, mapping.TargetNamespaceUri);

                    string targetNamespacePrefix = mapping.TargetNamespacePrefix;
                    if (!string.IsNullOrEmpty(targetNamespacePrefix))
                    {
                        localName = epmAttributeNameBuilder.EpmNsPrefix;
                        epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, targetNamespacePrefix);
                    }
                }
                else
                {
                    localName = epmAttributeNameBuilder.EpmTargetPath;
                    epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, mapping.TargetSyndicationItem.ToAttributeValue());

                    localName = epmAttributeNameBuilder.EpmContentKind;
                    epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, mapping.TargetTextContentKind.ToAttributeValue());
                }

                if (!skipSourcePath)
                {
                    string sourcePath = mapping.SourcePath;
                    if (removePrefix)
                    {
                        sourcePath = sourcePath.Substring(sourcePath.IndexOf('/') + 1);
                    }

                    localName = epmAttributeNameBuilder.EpmSourcePath;
                    epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, sourcePath);
                }

                string keepInContent = mapping.KeepInContent ? AtomConstants.AtomTrueLiteral : AtomConstants.AtomFalseLiteral;
                localName = epmAttributeNameBuilder.EpmKeepInContent;
                epmAnnotationBindings[localName] = GetODataAnnotationBinding(annotatable, localName, keepInContent);

                epmAttributeNameBuilder.MoveNext();
            }

            model.SetAnnotationValues(epmAnnotationBindings.Values);
        }

        /// <summary>
        /// Returns the cached keep-in-content annotation for the primitive properties of a complex type.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="complexType">The complex type to get the cached keep-in-content annotation for.</param>
        /// <returns>Returns the keep-in-content annotation for a type. If there's no such annotation this returns null.</returns>
        internal static CachedPrimitiveKeepInContentAnnotation EpmCachedKeepPrimitiveInContent(this IEdmModel model, IEdmComplexType complexType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(complexType != null, "complexType != null");
           
            return model.GetAnnotationValue<CachedPrimitiveKeepInContentAnnotation>(complexType);
        }

        /// <summary>
        /// Maps the enumeration of allowed <see cref="SyndicationItemProperty"/> values to their string representations.
        /// </summary>
        /// <param name="targetSyndicationItem">Value of the <see cref="SyndicationItemProperty"/> given in 
        /// the <see cref="EntityPropertyMappingAttribute"/> contstructor.</param>
        /// <returns>String representing the xml element path in the syndication property.</returns>
        internal static string ToTargetPath(this SyndicationItemProperty targetSyndicationItem)
        {
            DebugUtils.CheckNoExternalCallers();

            switch (targetSyndicationItem)
            {
                case SyndicationItemProperty.AuthorEmail: return EpmConstants.PropertyMappingTargetPathAuthorEmail;
                case SyndicationItemProperty.AuthorName: return EpmConstants.PropertyMappingTargetPathAuthorName;
                case SyndicationItemProperty.AuthorUri: return EpmConstants.PropertyMappingTargetPathAuthorUri;
                case SyndicationItemProperty.ContributorEmail: return EpmConstants.PropertyMappingTargetPathContributorEmail;
                case SyndicationItemProperty.ContributorName: return EpmConstants.PropertyMappingTargetPathContributorName;
                case SyndicationItemProperty.ContributorUri: return EpmConstants.PropertyMappingTargetPathContributorUri;
                case SyndicationItemProperty.Updated: return EpmConstants.PropertyMappingTargetPathUpdated;
                case SyndicationItemProperty.Published: return EpmConstants.PropertyMappingTargetPathPublished;
                case SyndicationItemProperty.Rights: return EpmConstants.PropertyMappingTargetPathRights;
                case SyndicationItemProperty.Summary: return EpmConstants.PropertyMappingTargetPathSummary;
                case SyndicationItemProperty.Title: return EpmConstants.PropertyMappingTargetPathTitle;
                default:
                    throw new ArgumentException(ODataErrorStrings.EntityPropertyMapping_EpmAttribute("targetSyndicationItem"));
            }
        }

        /// <summary>
        /// Loads the serializable EPM annotations on the given <paramref name="entityType"/> into their in-memory representation.
        /// </summary>
        /// <param name="model">The model the entity type belongs to.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to load the EPM annotations for.</param>
        private static void LoadEpmAnnotations(IEdmModel model, IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityType != null, "entityType != null");

            string entityTypeName = entityType.ODataFullName();
            ODataEntityPropertyMappingCollection mappings = new ODataEntityPropertyMappingCollection();

            // Load the annotations from the type (these are the mappings for properties not explicitly declared on this type)
            model.LoadEpmAnnotations(entityType, mappings, entityTypeName, null /*property*/);

            IEnumerable<IEdmProperty> declaredProperties = entityType.DeclaredProperties;
            if (declaredProperties != null)
            {
                foreach (IEdmProperty property in declaredProperties)
                {
                    // Load the EPM annotations for all properties independent of their kind in order to fail on invalid property kinds.
                    model.LoadEpmAnnotations(property, mappings, entityTypeName, property);
                }
            }

            // Set the new EPM annotation on the entity type only at the very end to not leave a 
            // inconsistent annotation if building it fails.
            model.SetAnnotationValue(entityType, mappings);
        }

        /// <summary>
        /// Loads the serializable EPM annotations on the given <paramref name="annotatable"/> into their in-memory representation.
        /// </summary>
        /// <param name="model">The model the annotatable belongs to.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to load the EPM annotations for.</param>
        /// <param name="mappings">The collection of EPM annotations to add newly loaded annotations to.</param>
        /// <param name="typeName">The name of the type for which to load the annotations or that declares the <paramref name="property"/>. Only used in error messages.</param>
        /// <param name="property">The property to parse the EPM annotations for.</param>
        private static void LoadEpmAnnotations(this IEdmModel model, IEdmElement annotatable, ODataEntityPropertyMappingCollection mappings, string typeName, IEdmProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(mappings != null, "mappings != null");
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");

            IEnumerable<EpmAnnotationValues> allAnnotationValues = ParseSerializableEpmAnnotations(model, annotatable, typeName, property);

            if (allAnnotationValues != null)
            {
                foreach (EpmAnnotationValues annotationValues in allAnnotationValues)
                {
                    // check whether we found a valid EPM configuration
                    EntityPropertyMappingAttribute mapping = ValidateAnnotationValues(annotationValues, typeName, property);
                    mappings.Add(mapping);
                }
            }
        }

        /// <summary>
        /// Given a <paramref name="targetPath"/> gets the corresponding syndication property.
        /// </summary>
        /// <param name="targetPath">Target path in the form of a syndication property name.</param>
        /// <returns>
        /// Enumeration value of a <see cref="SyndicationItemProperty"/> or SyndicationItemProperty.CustomProperty 
        /// if the <paramref name="targetPath"/> does not map to any syndication property name.
        /// </returns>
        private static SyndicationItemProperty MapTargetPathToSyndicationProperty(string targetPath)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(targetPath != null, "targetPath != null");

            SyndicationItemProperty targetSyndicationItem;
            return TargetPathToSyndicationItemMap.TryGetValue(targetPath, out targetSyndicationItem) ? targetSyndicationItem : SyndicationItemProperty.CustomProperty;
        }

        /// <summary>
        /// Translates a content kind enumeration value to the corresponding string attribute value for serialization to CSDL.
        /// </summary>
        /// <param name="contentKind">The content kind to translate.</param>
        /// <returns>A string corresponding to the <paramref name="contentKind"/> value.</returns>
        private static string ToAttributeValue(this SyndicationTextContentKind contentKind)
        {
            switch (contentKind)
            {
                case SyndicationTextContentKind.Xhtml:
                    return EpmConstants.ODataSyndContentKindXHtml;
                case SyndicationTextContentKind.Html:
                    return EpmConstants.ODataSyndContentKindHtml;
                default:
                    return EpmConstants.ODataSyndContentKindPlaintext;
            }
        }

        /// <summary>
        /// Translates a syndication item property enumeration value to the corresponding string attribute value for serialization to CSDL.
        /// </summary>
        /// <param name="syndicationItemProperty">The syndication item property to translate.</param>
        /// <returns>A string corresponding to the <paramref name="syndicationItemProperty"/> value.</returns>
        private static string ToAttributeValue(this SyndicationItemProperty syndicationItemProperty)
        {
            switch (syndicationItemProperty)
            {
                case SyndicationItemProperty.AuthorEmail: return EpmConstants.ODataSyndItemAuthorEmail;
                case SyndicationItemProperty.AuthorName: return EpmConstants.ODataSyndItemAuthorName;
                case SyndicationItemProperty.AuthorUri: return EpmConstants.ODataSyndItemAuthorUri;
                case SyndicationItemProperty.ContributorEmail: return EpmConstants.ODataSyndItemContributorEmail;
                case SyndicationItemProperty.ContributorName: return EpmConstants.ODataSyndItemContributorName;
                case SyndicationItemProperty.ContributorUri: return EpmConstants.ODataSyndItemContributorUri;
                case SyndicationItemProperty.Updated: return EpmConstants.ODataSyndItemUpdated;
                case SyndicationItemProperty.Published: return EpmConstants.ODataSyndItemPublished;
                case SyndicationItemProperty.Rights: return EpmConstants.ODataSyndItemRights;
                case SyndicationItemProperty.Summary: return EpmConstants.ODataSyndItemSummary;
                case SyndicationItemProperty.Title: return EpmConstants.ODataSyndItemTitle;
                case SyndicationItemProperty.CustomProperty: // fall through
                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.EpmExtensionMethods_ToAttributeValue_SyndicationItemProperty));
            }
        }

        /// <summary>
        /// Maps the <paramref name="contentKind"/> string to an enumeration value of the <see cref="SyndicationTextContentKind"/> enumeration.
        /// </summary>
        /// <param name="contentKind">The content kind string to map.</param>
        /// <param name="attributeSuffix">The suffix of the attribute name currently being parsed or validated.Only used in error messages.</param>
        /// <param name="typeName">The name of the type for which to load the annotations or that declares the <paramref name="propertyName"/>. Only used in error messages.</param>
        /// <param name="propertyName">The name of the property to parse the EPM annotations for. Only used in error messages.</param>
        /// <returns>An <see cref="SyndicationTextContentKind"/> value if the <paramref name="contentKind"/> could be successfully mapped; otherwise throws.</returns>
        private static SyndicationTextContentKind MapContentKindToSyndicationTextContentKind(
            string contentKind, 
            string attributeSuffix,
            string typeName, 
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(contentKind != null, "contentKind != null");
            Debug.Assert(attributeSuffix != null, "attributeSuffix != null");
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");

            switch (contentKind)
            {
                case EpmConstants.ODataSyndContentKindPlaintext: return SyndicationTextContentKind.Plaintext;
                case EpmConstants.ODataSyndContentKindHtml: return SyndicationTextContentKind.Html;
                case EpmConstants.ODataSyndContentKindXHtml: return SyndicationTextContentKind.Xhtml;
                default:
                    string errorMessage = propertyName == null
                        ? ODataErrorStrings.EpmExtensionMethods_InvalidTargetTextContentKindOnType(EpmConstants.ODataEpmContentKind + attributeSuffix, typeName)
                        : ODataErrorStrings.EpmExtensionMethods_InvalidTargetTextContentKindOnProperty(EpmConstants.ODataEpmContentKind + attributeSuffix, propertyName, typeName);
                    throw new ODataException(errorMessage);
            }
        }

        /// <summary>
        /// Parses the serializable EPM annotations of the <paramref name="annotatable"/>, groups them by suffix
        /// and translates them into a set of structs.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to parse the EPM annotations for.</param>
        /// <param name="typeName">The name of the type for which the annotations are parsed or that declares the <paramref name="property"/>. Only used in error messages.</param>
        /// <param name="property">The property to parse the EPM annotations for.</param>
        /// <returns>An enumerable of <see cref="EpmAnnotationValues"/> that represents all the parsed annotations grouped by their suffix.</returns>
        private static IEnumerable<EpmAnnotationValues> ParseSerializableEpmAnnotations(this IEdmModel model, IEdmElement annotatable, string typeName, IEdmProperty property)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(typeName != null, "typeName != null");

            Dictionary<string, EpmAnnotationValues> allAnnotationValues = null;

            IEnumerable<IEdmDirectValueAnnotation> annotations = model.GetODataAnnotations(annotatable);
            if (annotations != null)
            {
                foreach (IEdmDirectValueAnnotation annotation in annotations)
                {
                    string suffix;
                    string baseName;
                    if (annotation.IsEpmAnnotation(out baseName, out suffix))
                    {
                        Debug.Assert(baseName != null, "baseName != null");
                        Debug.Assert(suffix != null, "suffix != null");

                        string annotationValue = ConvertEdmAnnotationValue(annotation);

                        if (allAnnotationValues == null)
                        {
                            allAnnotationValues = new Dictionary<string, EpmAnnotationValues>(StringComparer.Ordinal);
                        }

                        EpmAnnotationValues annotationValues;
                        if (!allAnnotationValues.TryGetValue(suffix, out annotationValues))
                        {
                            annotationValues = new EpmAnnotationValues
                            {
                                AttributeSuffix = suffix
                            };
                            allAnnotationValues[suffix] = annotationValues;
                        }

                        // NOTE: we don't have to check for duplicate definitions since the Xml attribute
                        //       nature of the annotations prevents that.
                        if (NamesMatchByReference(EpmConstants.ODataEpmTargetPath, baseName))
                        {
                            Debug.Assert(annotationValues.TargetPath == null, "Can have only a single target path annotation per suffix.");
                            annotationValues.TargetPath = annotationValue;
                        }
                        else if (NamesMatchByReference(EpmConstants.ODataEpmSourcePath, baseName))
                        {
                            Debug.Assert(annotationValues.SourcePath == null, "Can have only a single source path annotation per suffix.");
                            annotationValues.SourcePath = annotationValue;
                        }
                        else if (NamesMatchByReference(EpmConstants.ODataEpmKeepInContent, baseName))
                        {
                            Debug.Assert(annotationValues.KeepInContent == null, "Can have only a single keep-in-content annotation per suffix.");
                            annotationValues.KeepInContent = annotationValue;
                        }
                        else if (NamesMatchByReference(EpmConstants.ODataEpmContentKind, baseName))
                        {
                            Debug.Assert(annotationValues.ContentKind == null, "Can have only a single content kind annotation per suffix.");
                            annotationValues.ContentKind = annotationValue;
                        }
                        else if (NamesMatchByReference(EpmConstants.ODataEpmNsUri, baseName))
                        {
                            Debug.Assert(annotationValues.NamespaceUri == null, "Can have only a single namespace URI annotation per suffix.");
                            annotationValues.NamespaceUri = annotationValue;
                        }
                        else if (NamesMatchByReference(EpmConstants.ODataEpmNsPrefix, baseName))
                        {
                            Debug.Assert(annotationValues.NamespacePrefix == null, "Can have only a single namespace prefix annotation per suffix.");
                            annotationValues.NamespacePrefix = annotationValue;
                        }
                        else
                        {
                            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataUtils_ParseSerializableEpmAnnotations_UnreachableCodePath));
                        }
                    }
                }

                // Fix up the source path values
                if (allAnnotationValues != null)
                {
                    foreach (EpmAnnotationValues annotationValues in allAnnotationValues.Values)
                    {
                        string sourcePath = annotationValues.SourcePath;

                        // set the default source path based on the property name if none was found in the annotations
                        if (sourcePath == null)
                        {
                            if (property == null)
                            {
                                string attributeName = EpmConstants.ODataEpmSourcePath + annotationValues.AttributeSuffix;
                                throw new ODataException(ODataErrorStrings.EpmExtensionMethods_MissingAttributeOnType(attributeName, typeName));
                            }

                            annotationValues.SourcePath = property.Name;
                        }
                        else
                        {
                            // For non-primitive properties we strip the property name from the source path if the 
                            // source path is not the same as the property name; re-add the property name here.
                            if (property != null && !property.Type.IsODataPrimitiveTypeKind())
                            {
                                annotationValues.SourcePath = property.Name + "/" + sourcePath;
                            }
                        }
                    }
                }
            }

            return allAnnotationValues == null ? null : allAnnotationValues.Values;
        }

        /// <summary>
        /// Validates the annotation values parsed for an EPM mapping.
        /// </summary>
        /// <param name="annotationValues">The <see cref="EpmAnnotationValues"/> to validate.</param>
        /// <param name="typeName">The name of the type for which the annotations are validated or that declares the <paramref name="property"/>. Only used in error messages.</param>
        /// <param name="property">The property for which the annotations are validated; null if the annotations are for a type.</param>
        /// <returns>An <see cref="EntityPropertyMappingAttribute"/> instance that represents the mapping created from the <paramref name="annotationValues"/>.</returns>
        private static EntityPropertyMappingAttribute ValidateAnnotationValues(EpmAnnotationValues annotationValues, string typeName, IEdmProperty property)
        {
            Debug.Assert(annotationValues != null, "annotationValues != null");
            Debug.Assert(annotationValues.AttributeSuffix != null, "annotationValues.AttributeSuffix != null");
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");

            //// Conditions for EPM annotation values to represent a valid mapping:
            ////   1. must have target path
            ////   2. can have keep-in-content (default is 'true')
            ////   3a. if custom mapping: target path must map to custom property, content kind must be null
            ////   3b. if syndication mapping: content kind (optional; default: plain text), no ns uri, no ns prefix

            if (annotationValues.TargetPath == null)
            {
                string attributeName = EpmConstants.ODataEpmTargetPath + annotationValues.AttributeSuffix;
                string errorMessage = property == null
                    ? ODataErrorStrings.EpmExtensionMethods_MissingAttributeOnType(attributeName, typeName)
                    : ODataErrorStrings.EpmExtensionMethods_MissingAttributeOnProperty(attributeName, property.Name, typeName);
                throw new ODataException(errorMessage);
            }

            EntityPropertyMappingAttribute mapping;

            bool keepInContent = true;
            if (annotationValues.KeepInContent != null)
            {
                if (!bool.TryParse(annotationValues.KeepInContent, out keepInContent))
                {
                    string attributeName = EpmConstants.ODataEpmKeepInContent + annotationValues.AttributeSuffix;
                    throw new InvalidOperationException(property == null 
                        ? ODataErrorStrings.EpmExtensionMethods_InvalidKeepInContentOnType(attributeName, typeName) 
                        : ODataErrorStrings.EpmExtensionMethods_InvalidKeepInContentOnProperty(attributeName, property.Name, typeName));
                }
            }

            // figure out whether this is a custom mapping or not
            SyndicationItemProperty targetSyndicationItem = MapTargetPathToSyndicationProperty(annotationValues.TargetPath);
            if (targetSyndicationItem == SyndicationItemProperty.CustomProperty)
            {
                if (annotationValues.ContentKind != null)
                {
                    string attributeName = EpmConstants.ODataEpmContentKind + annotationValues.AttributeSuffix;
                    string errorMessage = property == null
                        ? ODataErrorStrings.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType(attributeName, typeName)
                        : ODataErrorStrings.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty(attributeName, property.Name, typeName);
                    throw new ODataException(errorMessage);
                }

                mapping = new EntityPropertyMappingAttribute(
                    annotationValues.SourcePath,
                    annotationValues.TargetPath,
                    annotationValues.NamespacePrefix,
                    annotationValues.NamespaceUri,
                    keepInContent);
            }
            else
            {
                if (annotationValues.NamespaceUri != null)
                {
                    string attributeName = EpmConstants.ODataEpmNsUri + annotationValues.AttributeSuffix;
                    string errorMessage = property == null
                        ? ODataErrorStrings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType(attributeName, typeName)
                        : ODataErrorStrings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty(attributeName, property.Name, typeName);
                    throw new ODataException(errorMessage);
                }

                if (annotationValues.NamespacePrefix != null)
                {
                    string attributeName = EpmConstants.ODataEpmNsPrefix + annotationValues.AttributeSuffix;
                    string errorMessage = property == null
                        ? ODataErrorStrings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType(attributeName, typeName)
                        : ODataErrorStrings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty(attributeName, property.Name, typeName);
                    throw new ODataException(errorMessage);
                }

                SyndicationTextContentKind contentKind = SyndicationTextContentKind.Plaintext;
                if (annotationValues.ContentKind != null)
                {
                    contentKind = MapContentKindToSyndicationTextContentKind(
                        annotationValues.ContentKind, 
                        annotationValues.AttributeSuffix, 
                        typeName, 
                        property == null ? null : property.Name);
                }

                mapping = new EntityPropertyMappingAttribute(
                    annotationValues.SourcePath,
                    targetSyndicationItem,
                    contentKind,
                    keepInContent);
            }

            Debug.Assert(mapping != null, "mapping != null");
            return mapping;
        }
        
        /// <summary>
        /// Removes an existing EPM cache annotation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to remove the EPM cache from.</param>
        private static void RemoveEpmCache(this IEdmModel model, IEdmEntityType entityType)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityType != null, "entityType != null");
            
            model.SetAnnotationValue<ODataEntityPropertyMappingCache>(entityType, null);
        }

        /// <summary>
        /// Checks whether a given OData annotation is an EPM related annotation.
        /// </summary>
        /// <param name="annotation">The <see cref="IEdmDirectValueAnnotation"/> instance to check.</param>
        /// <returns>true if the annotation is EPM related; otherwise false.</returns>
        private static bool IsEpmAnnotation(this IEdmDirectValueAnnotation annotation)
        {
            Debug.Assert(annotation != null, "annotation != null");
            Debug.Assert(annotation.NamespaceUri == AtomConstants.ODataMetadataNamespace, "Expected only OData annotations in the metadata namespace.");

            string baseName;
            string suffix;
            return annotation.IsEpmAnnotation(out baseName, out suffix);
        }

        /// <summary>
        /// Checks whether a given serializable annotation represents part of an EPM mapping.
        /// </summary>
        /// <param name="annotation">The annotation to check.</param>
        /// <param name="baseName">The base name of the EPM annotation.</param>
        /// <param name="suffix">The suffix of the EPM annotation or null if not an EPM annotation.</param>
        /// <returns>true if the <paramref name="annotation"/> is an EPM annotation; otherwise false.</returns>
        private static bool IsEpmAnnotation(this IEdmDirectValueAnnotation annotation, out string baseName, out string suffix)
        {
            Debug.Assert(annotation != null, "annotation != null");
            Debug.Assert(annotation.NamespaceUri == AtomConstants.ODataMetadataNamespace, "Annotation must be OData-specific.");

            string localName = annotation.Name;
            for (int i = 0; i < EpmAnnotationBaseNames.Length; ++i)
            {
                string annotationBaseName = EpmAnnotationBaseNames[i];
                if (localName.StartsWith(annotationBaseName, StringComparison.Ordinal))
                {
                    baseName = annotationBaseName;
                    suffix = localName.Substring(annotationBaseName.Length);
                    return true;
                }
            }

            baseName = null;
            suffix = null;

            return false;
        }

        /// <summary>
        /// Converts the value of the <paramref name="annotation"/> to a string.
        /// </summary>
        /// <param name="annotation">The <see cref="IEdmDirectValueAnnotation"/> to convert.</param>
        /// <returns>The string representation of the converted annotation value.</returns>
        private static string ConvertEdmAnnotationValue(IEdmDirectValueAnnotation annotation)
        {
            Debug.Assert(annotation != null, "annotation != null");

            object annotationValue = annotation.Value;
            if (annotationValue == null)
            { 
                return null;
            }

            IEdmStringValue stringValue = annotationValue as IEdmStringValue;
            if (stringValue != null)
            {
                return stringValue.Value;
            }

            throw new ODataException(ODataErrorStrings.EpmExtensionMethods_CannotConvertEdmAnnotationValue(annotation.NamespaceUri, annotation.Name, annotation.GetType().FullName));
        }

        /// <summary>
        /// Checks that two strings are the same references (and asserts that if they are not they also
        /// don't have the same value).
        /// </summary>
        /// <param name="first">The first string to compare.</param>
        /// <param name="second">The second string to compare.</param>
        /// <returns>true if the <paramref name="first"/> and <paramref name="second"/> are the same reference; otherwise false;</returns>
        private static bool NamesMatchByReference(string first, string second)
        {
            Debug.Assert(
                object.ReferenceEquals(first, second) || string.CompareOrdinal(first, second) != 0,
                "Either the references are the same or the values must also be differnt.");

            return object.ReferenceEquals(first, second);
        }

        /// <summary>
        /// Checks whether the <paramref name="entityType"/> has EPM defined for it (either directly
        /// on the type or on one of the base types).
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to check.</param>
        /// <returns>true if the <paramref name="entityType"/> has EPM defined; otherwise false.</returns>
        private static bool HasOwnOrInheritedEpm(this IEdmModel model, IEdmEntityType entityType)
        {
            if (entityType == null)
            {
                return false;
            }

            Debug.Assert(model != null, "model != null");

            if (model.GetAnnotationValue<ODataEntityPropertyMappingCollection>(entityType) != null)
            {
                return true;
            }

            // If we don't have an in-memory annotation, try to load the serializable EPM annotations
            LoadEpmAnnotations(model, entityType);
            if (model.GetAnnotationValue<ODataEntityPropertyMappingCollection>(entityType) != null)
            {
                return true;
            }

            return model.HasOwnOrInheritedEpm(entityType.BaseEntityType());
        }

        /// <summary>
        /// Gets the annotation binding with the OData metadata namespace and the specified <paramref name="localName" /> for the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to set the annotation on.</param>
        /// <param name="localName">The local name of the annotation to set.</param>
        /// <param name="value">The value of the annotation to set.</param>
        /// <returns>An <see cref="IEdmDirectValueAnnotationBinding"/> instance that represnets the annotation with the specified name and value.</returns>
        private static IEdmDirectValueAnnotationBinding GetODataAnnotationBinding(IEdmElement annotatable, string localName, string value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            IEdmStringValue stringValue = null;
            if (value != null)
            {
                IEdmStringTypeReference typeReference = EdmCoreModel.Instance.GetString(/*nullable*/true);
                stringValue = new EdmStringConstant(typeReference, value);
            }

            return new EdmDirectValueAnnotationBinding(annotatable, AtomConstants.ODataMetadataNamespace, localName, stringValue);
        }

        /// <summary>
        /// Ensures that an up-to-date EPM cache exists for the specified <paramref name="entityType"/>. 
        /// If no cache exists, a new one will be created based on the public mappings (if any).
        /// If the public mappings have changed (and the cache is thus dirty), the method re-constructs the cache.
        /// If all public mappings have been removed, the method also removes the EPM cache.
        /// </summary>
        /// <param name="model">IEdmModel instance containing the annotations.</param>
        /// <param name="entityType">IEdmEntityType instance for which to ensure the EPM cache.</param>
        /// <param name="maxMappingCount">The maximum allowed number of entity property mappings 
        /// for a given entity type (on the type itself and all its base types).</param>
        /// <param name="cacheModified">true if the cache was modified; otherwise false.</param>
        /// <returns>An instance of <see cref="ODataEntityPropertyMappingCache"/>, if there are any EPM mappings for the given entity type, otherwise returns null.</returns>
        private static ODataEntityPropertyMappingCache EnsureEpmCacheInternal(
            IEdmModel model, 
            IEdmEntityType entityType, 
            int maxMappingCount, 
            out bool cacheModified)
        {
            cacheModified = false;

            if (entityType == null)
            {
                return null;
            }

            // Make sure the EPM of the base type is initialized.
            IEdmEntityType baseEntityType = entityType.BaseEntityType();
            ODataEntityPropertyMappingCache baseCache = null;
            if (baseEntityType != null)
            {
                baseCache = EnsureEpmCacheInternal(model, baseEntityType, maxMappingCount, out cacheModified);
            }

            ODataEntityPropertyMappingCache epmCache = model.GetEpmCache(entityType);

            if (model.HasOwnOrInheritedEpm(entityType))
            {
                ODataEntityPropertyMappingCollection mappings = model.GetEntityPropertyMappings(entityType);
                bool needToBuildCache = epmCache == null || cacheModified || epmCache.IsDirty(mappings);
                if (needToBuildCache)
                {
                    cacheModified = true;
                    int totalMappingCount = ValidationUtils.ValidateTotalEntityPropertyMappingCount(baseCache, mappings, maxMappingCount);
                    epmCache = new ODataEntityPropertyMappingCache(mappings, model, totalMappingCount);

                    // Build the EPM tree and validate it
                    try
                    {
                        epmCache.BuildEpmForType(entityType, entityType);
                        epmCache.EpmSourceTree.Validate(entityType);
                        epmCache.EpmTargetTree.Validate();

                        // We set the annotation here, so if anything fails during
                        // building of the cache the annotation will not even be set so
                        // not leaving the type in an inconsistent state.
                        model.SetAnnotationValue(entityType, epmCache);
                    }
                    catch
                    {
                        // Remove an existing EPM cache if it is dirty to make sure we don't leave
                        // stale caches in case building of the cache fails.
                        // NOTE: we do this in the catch block to ensure that we always make a single
                        //       SetAnnotation call to either set or clear the existing annotation
                        //       since the SetAnnotation method is thread-safe
                        model.RemoveEpmCache(entityType);

                        throw;
                    }
                }
            }
            else
            {
                if (epmCache != null)
                {
                    // remove an existing EPM cache if the mappings have been removed from the type
                    cacheModified = true;
                    model.RemoveEpmCache(entityType);
                }
            }

            return epmCache;
        }

        /// <summary>
        /// Private struct to store the values of the serializable EPM annotations during loading.
        /// </summary>
        private sealed class EpmAnnotationValues
        {
            /// <summary>The string value of the FC_SourcePath attribute (or null if not present).</summary>
            internal string SourcePath { get; set; }

            /// <summary>The string value of the FC_TargetPath attribute (or null if not present).</summary>
            internal string TargetPath { get; set; }

            /// <summary>The string value of the FC_KeepInContent attribute (or null if not present).</summary>
            internal string KeepInContent { get; set; }

            /// <summary>The string value of the FC_ContentKind attribute (or null if not present).</summary>
            internal string ContentKind { get; set; }

            /// <summary>The string value of the FC_NsUri attribute (or null if not present).</summary>
            internal string NamespaceUri { get; set; }

            /// <summary>The string value of the FC_NsPrefix attribute (or null if not present).</summary>
            internal string NamespacePrefix { get; set; }

            /// <summary>The attribute suffix used for the attribute names.</summary>
            internal string AttributeSuffix { get; set; }
        }
    }
}
