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
    using Microsoft.Data.OData.Atom;
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
            EpmConstants.ODataEpmCriteriaValue,
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
            { EpmConstants.ODataSyndItemCategoryLabel, SyndicationItemProperty.CategoryLabel },
            { EpmConstants.ODataSyndItemCategoryScheme, SyndicationItemProperty.CategoryScheme },
            { EpmConstants.ODataSyndItemCategoryTerm, SyndicationItemProperty.CategoryTerm },
            { EpmConstants.ODataSyndItemLinkHref, SyndicationItemProperty.LinkHref },
            { EpmConstants.ODataSyndItemLinkHrefLang, SyndicationItemProperty.LinkHrefLang },
            { EpmConstants.ODataSyndItemLinkLength, SyndicationItemProperty.LinkLength },
            { EpmConstants.ODataSyndItemLinkRel, SyndicationItemProperty.LinkRel },
            { EpmConstants.ODataSyndItemLinkTitle, SyndicationItemProperty.LinkTitle },
            { EpmConstants.ODataSyndItemLinkType, SyndicationItemProperty.LinkType },
        };

        /// <summary>
        /// Ensures that an up-to-date EPM cache exists for the specified <paramref name="entityType"/>. 
        /// If no cache exists, a new one will be created based on the public mappings (if any).
        /// If the public mappings have changed (and the cache is thus dirty), the method re-constructs the cache.
        /// If all public mappings have been removed, the method also removes the EPM cache.
        /// </summary>
        /// <param name="entityType">IEdmEntityType instance for which to ensure the EPM cache.</param>
        /// <returns>An instance of <see cref="ODataEntityPropertyMappingCache"/>, if there are any EPM mappings for the given entity type, otherwise returns null.</returns>
        internal static ODataEntityPropertyMappingCache EnsureEpmCache(this IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();

            if (entityType == null)
            {
                return null;
            }

            // Make sure the EPM of the base type is initialized.
            IEdmEntityType baseEntityType = entityType.BaseEntityType();
            if (baseEntityType != null)
            {
                baseEntityType.EnsureEpmCache();
            }

            ODataEntityPropertyMappingCache epmCache = entityType.GetEpmCache();

            if (entityType.HasOwnOrInheritedEpm())
            {
                ODataEntityPropertyMappingCollection mappings = entityType.GetEntityPropertyMappings();
                bool needToBuildCache = epmCache == null || epmCache.IsDirty(mappings);
                if (needToBuildCache)
                {
                    if (epmCache != null)
                    {
                        // remove an existing EPM cache if it is dirty to make sure we don't leave
                        // stale caches in case building of the cache fails.
                        entityType.RemoveEpmCache();
                    }

                    epmCache = new ODataEntityPropertyMappingCache(mappings);

                    // Build the EPM tree and validate it
                    epmCache.BuildEpmForType(entityType, entityType);
                    epmCache.EpmSourceTree.Validate(entityType);
                    epmCache.EpmTargetTree.Validate();

                    // we only set the annotation here, so if anything fails during
                    // building of the cache the annotation will not even be set so
                    // not leaving the type in an inconsistent state.
                    entityType.SetAnnotation(epmCache);
                }
            }
            else
            {
                if (epmCache != null)
                {
                    // remove an existing EPM cache if the mappings have been removed from the type
                    entityType.RemoveEpmCache();
                }
            }

            return epmCache;
        }

        /// <summary>
        /// Determines if the <paramref name="entityType"/> has any EPM defined on it (or its base types).
        /// </summary>
        /// <param name="entityType">The entity type to test for presence of EPM.</param>
        /// <returns>true if the <paramref name="entityType"/> has EPM; false otherwise.</returns>
        internal static bool HasEntityPropertyMappings(this IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityType != null, "entityType != null");

            // Walk the base types and determine if there's EPM anywhere in there.
            IEdmEntityType entityTypeToTest = entityType;
            while (entityTypeToTest != null)
            {
                if (entityTypeToTest.GetEntityPropertyMappings() != null)
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
        /// <param name="entityType">The entity type to get the EPM information for.</param>
        /// <returns>Returns the EPM information for an entity type. If there's no such information, this returns null.</returns>
        internal static ODataEntityPropertyMappingCollection GetEntityPropertyMappings(this IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityType != null, "entityType != null");

            return entityType.GetAnnotation<ODataEntityPropertyMappingCollection>();
        }

        /// <summary>
        /// Returns the cached EPM information for an entity type.
        /// </summary>
        /// <param name="entityType">The entity type to get the cached EPM information for.</param>
        /// <returns>Returns the cached EPM information for an entity type. If there's no cached information, this returns null.</returns>
        internal static ODataEntityPropertyMappingCache GetEpmCache(this IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityType != null, "entityType != null");

            return entityType.GetAnnotation<ODataEntityPropertyMappingCache>();
        }

        /// <summary>
        /// Clears all EPM related annotations from a given <see cref="IEdmAnnotatable"/>.
        /// </summary>
        /// <param name="annotatable">The annotatable to clear the EPM annotations on.</param>
        internal static void ClearSerializableEpmAnnotations(this IEdmAnnotatable annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");

            List<IEdmAnnotation> annotationsToRemove = null;
            IEnumerable<IEdmAnnotation> annotations = annotatable.GetODataAnnotations();
            if (annotations != null)
            {
                foreach (IEdmAnnotation annotation in annotations)
                {
                    if (annotation.IsEpmAnnotation())
                    {
                        if (annotationsToRemove == null)
                        {
                            annotationsToRemove = new List<IEdmAnnotation>();
                        }

                        annotationsToRemove.Add(annotation);
                    }
                }
            }

            if (annotationsToRemove != null)
            {
                for (int i = 0; i < annotationsToRemove.Count; ++i)
                {
                    IEdmAnnotation annotation = annotationsToRemove[i];
                    annotatable.SetAnnotation(annotation.Namespace(), annotation.LocalName(), null);
                }
            }
        }

        /// <summary>
        /// Removes the in-memory EPM annotations from an entity type; potentially also drops an existing EPM cache.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to remove the EPM annotation from.</param>
        internal static void ClearInMemoryEpmAnnotations(this IEdmAnnotatable annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");

            // remove the in-memory annotation
            annotatable.SetAnnotation<ODataEntityPropertyMappingCollection>(null);

            // remove the cache
            annotatable.SetAnnotation<ODataEntityPropertyMappingCache>(null);
        }

        /// <summary>
        /// Saves the EPM annotations on the given <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The <see cref="IEdmProperty"/> to save the EPM annotations for.</param>
        /// <param name="epmCache">The EPM cache for the owning entity type.</param>
        internal static void SaveEpmAnnotationsForProperty(this IEdmProperty property, ODataEntityPropertyMappingCache epmCache)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(property != null, "property != null");
            Debug.Assert(epmCache != null, "epmCache != null");

            // remove all existing EPM annotations from the property to make sure that the is in a consistent state after saving
            // the current EPM mappings (no left-overs from previous load or save operations).
            property.ClearSerializableEpmAnnotations();

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
                    property.Type.IsODataComplexTypeKind() || property.Type.IsODataMultiValueTypeKind(),
                    "Only primitive, complex or multiValue properties can have EPM defined on them.");

                removePrefix = true;
                skipSourcePath = propertyMappings.Any(m => m.SourcePath == propertyName);
                Debug.Assert(
                    !skipSourcePath || propertyMappings.Count() == 1,
                    "We must not have multiple mappings for a property if one of them matches the property name exactly (the other ones would not make sense).");
            }

            property.SaveEpmAnnotations(propertyMappings, skipSourcePath, removePrefix);
        }

        /// <summary>
        /// Saves the EPM annotations on the given <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to save the EPM annotations on.</param>
        /// <param name="mappings">All the EPM annotations to be saved.</param>
        /// <param name="skipSourcePath">true if the source path should be saved explicitly; otherwise false.</param>
        /// <param name="removePrefix">true if the prefix of the source path should be removed; otherwise false.</param>
        internal static void SaveEpmAnnotations(this IEdmAnnotatable annotatable, IEnumerable<EntityPropertyMappingAttribute> mappings, bool skipSourcePath, bool removePrefix)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(mappings != null, "mappings != null");
            Debug.Assert(!skipSourcePath || mappings.Count() <= 1, "Skipping the source path is only valid if at most a single mapping exists.");

            EpmAttributeNameBuilder epmAttributeNameBuilder = new EpmAttributeNameBuilder();

            foreach (EntityPropertyMappingAttribute mapping in mappings)
            {
                annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmTargetPath, mapping.TargetPath);

                // Check whether the mapping represents a custom mapping
                if (mapping.TargetSyndicationItem == SyndicationItemProperty.CustomProperty)
                {
                    annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmNsUri, mapping.TargetNamespaceUri);

                    string targetNamespacePrefix = mapping.TargetNamespacePrefix;
                    if (!string.IsNullOrEmpty(targetNamespacePrefix))
                    {
                        annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmNsPrefix, targetNamespacePrefix);
                    }
                }
                else
                {
                    // If there is a criteria specified it means conditional EPM constructor is used to create the mapping.
                    // Conditional EPM constructor does not have a content kind parameter and we should not write it out.
                    if (mapping.CriteriaValue == null)
                    {
                        annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmContentKind, mapping.TargetTextContentKind.ToAttributeValue());
                    }
                    else
                    {
                        // Conditional EPM
                        annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmCriteriaValue, mapping.CriteriaValue);
                    }
                }

                if (!skipSourcePath)
                {
                    string sourcePath = mapping.SourcePath;
                    if (removePrefix)
                    {
                        sourcePath = sourcePath.Substring(sourcePath.IndexOf('/') + 1);
                    }

                    annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmSourcePath, sourcePath);
                }

                string keepInContent = mapping.KeepInContent ? AtomConstants.AtomTrueLiteral : AtomConstants.AtomFalseLiteral;
                annotatable.SetODataAnnotation(epmAttributeNameBuilder.EpmKeepInContent, keepInContent);

                epmAttributeNameBuilder.MoveNext();
            }
        }

        /// <summary>
        /// Loads the serializable EPM annotations on the given <paramref name="annotatable"/> into their in-memory representation.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to load the EPM annotations for.</param>
        /// <param name="mappings">The collection of EPM annotations to add newly loaded annotations to.</param>
        /// <param name="typeName">The name of the type for which to load the annotations or that declares the <paramref name="property"/>. Only used in error messages.</param>
        /// <param name="property">The property to parse the EPM annotations for.</param>
        internal static void LoadEpmAnnotations(this IEdmAnnotatable annotatable, ODataEntityPropertyMappingCollection mappings, string typeName, IEdmProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(mappings != null, "mappings != null");
            Debug.Assert(!string.IsNullOrEmpty(typeName), "!string.IsNullOrEmpty(typeName)");

            IEnumerable<EpmAnnotationValues> allAnnotationValues = ParseSerializableEpmAnnotations(annotatable, typeName, property);

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
        /// Returns the cached keep-in-content annotation for the primitive properties of a complex type.
        /// </summary>
        /// <param name="complexType">The complex type to get the cached keep-in-content annotation for.</param>
        /// <returns>Returns the keep-in-content annotation for a type. If there's no such annotation this returns null.</returns>
        internal static CachedPrimitiveKeepInContentAnnotation EpmCachedKeepPrimitiveInContent(this IEdmComplexType complexType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(complexType != null, "complexType != null");

            return complexType.GetAnnotation<CachedPrimitiveKeepInContentAnnotation>();
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
                case SyndicationItemProperty.CategoryLabel: return EpmConstants.PropertyMappingTargetPathCategoryLabel;
                case SyndicationItemProperty.CategoryScheme: return EpmConstants.PropertyMappingTargetPathCategoryScheme;
                case SyndicationItemProperty.CategoryTerm: return EpmConstants.PropertyMappingTargetPathCategoryTerm;
                case SyndicationItemProperty.LinkHref: return EpmConstants.PropertyMappingTargetPathLinkHref;
                case SyndicationItemProperty.LinkHrefLang: return EpmConstants.PropertyMappingTargetPathLinkHrefLang;
                case SyndicationItemProperty.LinkLength: return EpmConstants.PropertyMappingTargetPathLinkLength;
                case SyndicationItemProperty.LinkRel: return EpmConstants.PropertyMappingTargetPathLinkRel;
                case SyndicationItemProperty.LinkTitle: return EpmConstants.PropertyMappingTargetPathLinkTitle;
                case SyndicationItemProperty.LinkType: return EpmConstants.PropertyMappingTargetPathLinkType;
                default:
                    throw new ArgumentException(Strings.EntityPropertyMapping_EpmAttribute("targetSyndicationItem"));
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
                case SyndicationTextContentKind.Plaintext:
                    return EpmConstants.ODataSyndContentKindPlaintext;
                case SyndicationTextContentKind.Html:
                    return EpmConstants.ODataSyndContentKindHtml;
                default:
                    Debug.Assert(contentKind == SyndicationTextContentKind.Xhtml, "Unexpected syndication text content kind");
                    return EpmConstants.ODataSyndContentKindXHtml;
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
                        ? Strings.EpmExtensionMethods_InvalidTargetTextContentKindOnType(EpmConstants.ODataEpmContentKind + attributeSuffix, typeName)
                        : Strings.EpmExtensionMethods_InvalidTargetTextContentKindOnProperty(EpmConstants.ODataEpmContentKind + attributeSuffix, propertyName, typeName);
                    throw new ODataException(errorMessage);
            }
        }

        /// <summary>
        /// Parses the serializable EPM annotations of the <paramref name="annotatable"/>, groups them by suffix
        /// and translates them into a set of structs.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to parse the EPM annotations for.</param>
        /// <param name="typeName">The name of the type for which the annotations are parsed or that declares the <paramref name="property"/>. Only used in error messages.</param>
        /// <param name="property">The property to parse the EPM annotations for.</param>
        /// <returns>An enumerable of <see cref="EpmAnnotationValues"/> that represents all the parsed annotations grouped by their suffix.</returns>
        private static IEnumerable<EpmAnnotationValues> ParseSerializableEpmAnnotations(this IEdmAnnotatable annotatable, string typeName, IEdmProperty property)
        {
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(typeName != null, "typeName != null");

            Dictionary<string, EpmAnnotationValues> allAnnotationValues = null;

            IEnumerable<IEdmAnnotation> annotations = annotatable.GetODataAnnotations();
            if (annotations != null)
            {
                string suffix;
                string baseName;
                foreach (IEdmAnnotation annotation in annotations)
                {
                    if (annotation.IsEpmAnnotation(out baseName, out suffix))
                    {
                        Debug.Assert(baseName != null, "baseName != null");
                        Debug.Assert(suffix != null, "suffix != null");

                        string annotationValue = ConvertEdmAnnotationValue((IEdmImmediateValueAnnotation)annotation);

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
                        else if (NamesMatchByReference(EpmConstants.ODataEpmCriteriaValue, baseName))
                        {
                            Debug.Assert(annotationValues.CriteriaValue == null, "Can have only a single criteria value annotation per suffix.");
                            annotationValues.CriteriaValue = annotationValue;
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
                            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataUtils_ParseSerializableEpmAnnotations_UnreachableCodePath));
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
                                throw new ODataException(Strings.EpmExtensionMethods_MissingAttributeOnType(attributeName, typeName));
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
            ////   3a. if custom mapping: target path must map to custom property, content kind must be null, criteria value must be null
            ////   3b. if syndication mapping: content kind (optional; default: plain text), criteria value (optional; default: null), no ns uri, no ns prefix
            ////       NOTE: for the criteria value also check the kind of criteria value based on target path/syndication item kind

            if (annotationValues.TargetPath == null)
            {
                string attributeName = EpmConstants.ODataEpmTargetPath + annotationValues.AttributeSuffix;
                string errorMessage = property == null
                    ? Strings.EpmExtensionMethods_MissingAttributeOnType(attributeName, typeName)
                    : Strings.EpmExtensionMethods_MissingAttributeOnProperty(attributeName, property.Name, typeName);
                throw new ODataException(errorMessage);
            }

            EntityPropertyMappingAttribute mapping = null;

            bool keepInContent = true;
            if (annotationValues.KeepInContent != null)
            {
                if (!bool.TryParse(annotationValues.KeepInContent, out keepInContent))
                {
                    string attributeName = EpmConstants.ODataEpmKeepInContent + annotationValues.AttributeSuffix;
                    throw new InvalidOperationException(property == null 
                        ? Strings.EpmExtensionMethods_InvalidKeepInContentOnType(attributeName, typeName) 
                        : Strings.EpmExtensionMethods_InvalidKeepInContentOnProperty(attributeName, property.Name, typeName));
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
                        ? Strings.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType(attributeName, typeName)
                        : Strings.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty(attributeName, property.Name, typeName);
                    throw new ODataException(errorMessage);
                }

                if (annotationValues.CriteriaValue != null)
                {
                    string attributeName = EpmConstants.ODataEpmCriteriaValue + annotationValues.AttributeSuffix;
                    string errorMessage = property == null
                        ? Strings.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnType(attributeName, typeName)
                        : Strings.EpmExtensionMethods_AttributeNotAllowedForCustomMappingOnProperty(attributeName, property.Name, typeName);
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
                        ? Strings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType(attributeName, typeName)
                        : Strings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty(attributeName, property.Name, typeName);
                    throw new ODataException(errorMessage);
                }

                if (annotationValues.NamespacePrefix != null)
                {
                    string attributeName = EpmConstants.ODataEpmNsPrefix + annotationValues.AttributeSuffix;
                    string errorMessage = property == null
                        ? Strings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnType(attributeName, typeName)
                        : Strings.EpmExtensionMethods_AttributeNotAllowedForAtomPubMappingOnProperty(attributeName, property.Name, typeName);
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

                if (annotationValues.CriteriaValue != null)
                {
                    string expectedCriteria = MapSyndicationItemToEpmCriteria(targetSyndicationItem);
                    if (expectedCriteria == null)
                    {
                        // target path is not mappable to a valid EPM criteria
                        string attributeName = EpmConstants.ODataEpmCriteriaValue + annotationValues.AttributeSuffix;
                        string errorMessage = property == null
                            ? Strings.EpmExtensionMethods_ConditionalMappingToNonConditionalSyndicationItemOnType(attributeName, typeName, annotationValues.TargetPath)
                            : Strings.EpmExtensionMethods_ConditionalMappingToNonConditionalSyndicationItemOnProperty(attributeName, property.Name, typeName, annotationValues.TargetPath);
                        throw new ODataException(errorMessage);
                    }
                }

                if (annotationValues.CriteriaValue == null)
                {
                    mapping = new EntityPropertyMappingAttribute(
                        annotationValues.SourcePath,
                        targetSyndicationItem,
                        contentKind,
                        keepInContent);
                }
                else
                {
                    // If a criteria value is specified, the conditional ODataEntityPropertyMapping constructor was used that
                    // does not accept a content kind. We have to make sure that if a content kind is specified it is the expected
                    // PlainText
                    if (contentKind != SyndicationTextContentKind.Plaintext)
                    {
                        string attributeName = EpmConstants.ODataEpmContentKind + annotationValues.AttributeSuffix;
                        string errorMessage = property == null
                            ? Strings.EpmExtensionMethods_ConditionalMappingToInvalidContentKindOnType(attributeName, typeName)
                            : Strings.EpmExtensionMethods_ConditionalMappingToInvalidContentKindOnProperty(attributeName, property.Name, typeName);
                        throw new ODataException(errorMessage);
                    }

                    mapping = new EntityPropertyMappingAttribute(
                        annotationValues.SourcePath,
                        targetSyndicationItem,
                        keepInContent,
                        annotationValues.CriteriaValue);
                }
            }

            Debug.Assert(mapping != null, "mapping != null");
            return mapping;
        }
        
        /// <summary>
        /// Removes an existing EPM cache annotation.
        /// </summary>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to remove the EPM cache from.</param>
        private static void RemoveEpmCache(this IEdmEntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");

            entityType.SetAnnotation<ODataEntityPropertyMappingCache>(null);
        }

        /// <summary>
        /// Checks whether a given OData annotation is an EPM related annotation.
        /// </summary>
        /// <param name="annotation">The <see cref="IEdmAnnotation"/> instance to check.</param>
        /// <returns>true if the annotation is EPM related; otherwise false.</returns>
        private static bool IsEpmAnnotation(this IEdmAnnotation annotation)
        {
            Debug.Assert(annotation != null, "annotation != null");
            Debug.Assert(annotation.Namespace() == AtomConstants.ODataMetadataNamespace, "Expected only OData annotations in the metadata namespace.");

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
        private static bool IsEpmAnnotation(this IEdmAnnotation annotation, out string baseName, out string suffix)
        {
            Debug.Assert(annotation != null, "annotation != null");
            Debug.Assert(annotation.Namespace() == AtomConstants.ODataMetadataNamespace, "Annotation must be OData-specific.");

            string localName = annotation.LocalName();
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
        /// Returns the value for FC_Criteria attribute for <paramref name="targetSyndicationItem"/>.
        /// </summary>
        /// <param name="targetSyndicationItem">The target syndication item to map.</param>
        /// <returns>The value for the FC_Criteria attribute for the <paramref name="targetSyndicationItem"/>.</returns>
        private static string MapSyndicationItemToEpmCriteria(SyndicationItemProperty targetSyndicationItem)
        {
            switch (targetSyndicationItem)
            {
                case SyndicationItemProperty.CategoryLabel:
                case SyndicationItemProperty.CategoryScheme:
                case SyndicationItemProperty.CategoryTerm:
                    return EpmConstants.PropertyMappingTargetPathCategoryScheme;

                case SyndicationItemProperty.LinkHref:
                case SyndicationItemProperty.LinkHrefLang:
                case SyndicationItemProperty.LinkLength:
                case SyndicationItemProperty.LinkRel:
                case SyndicationItemProperty.LinkTitle:
                case SyndicationItemProperty.LinkType:
                    return EpmConstants.PropertyMappingTargetPathLinkRel;
            }

            return null;
        }

        /// <summary>
        /// Converts the value of the <paramref name="annotation"/> to a string.
        /// </summary>
        /// <param name="annotation">The <see cref="IEdmAnnotation"/> to convert.</param>
        /// <returns>The string representation of the converted annotation value.</returns>
        private static string ConvertEdmAnnotationValue(IEdmImmediateValueAnnotation annotation)
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

            throw new ODataException(Strings.EpmExtensionMethods_CannotConvertEdmAnnotationValue(annotation.Namespace(), annotation.LocalName(), annotation.GetType().FullName));
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
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to check.</param>
        /// <returns>true if the <paramref name="entityType"/> has EPM defined; otherwise false.</returns>
        private static bool HasOwnOrInheritedEpm(this IEdmEntityType entityType)
        {
            if (entityType == null)
            {
                return false;
            }

            if (entityType.GetAnnotation<ODataEntityPropertyMappingCollection>() != null)
            {
                return true;
            }

            return entityType.BaseEntityType().HasOwnOrInheritedEpm();
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

            /// <summary>The string value of the FC_CriteriaValue attribute (or null if not present).</summary>
            internal string CriteriaValue { get; set; }

            /// <summary>The string value of the FC_NsUri attribute (or null if not present).</summary>
            internal string NamespaceUri { get; set; }

            /// <summary>The string value of the FC_NsPrefix attribute (or null if not present).</summary>
            internal string NamespacePrefix { get; set; }

            /// <summary>The attribute suffix used for the attribute names.</summary>
            internal string AttributeSuffix { get; set; }
        }
    }
}
