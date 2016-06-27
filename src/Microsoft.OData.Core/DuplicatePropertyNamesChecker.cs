//---------------------------------------------------------------------
// <copyright file="DuplicatePropertyNamesChecker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Json;
    using Microsoft.OData.JsonLight;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that no duplicate properties are specified for entries and complex values.
    /// </summary>
    internal sealed class DuplicatePropertyNamesChecker
    {
        /// <summary>Special value for the property annotations which is used to mark the annotations as processed.</summary>
        private static readonly Dictionary<string, object> propertyAnnotationsProcessedToken = new Dictionary<string, object>(0, StringComparer.Ordinal);

        /// <summary>true if duplicate properties are allowed; otherwise false.</summary>
        /// <remarks>
        /// See the comment on ODataWriterBehavior.AllowDuplicatePropertyNames or
        /// ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </remarks>
        private readonly bool allowDuplicateProperties;

        /// <summary>true if we're processing a response; false if it's a request.</summary>
        private readonly bool isResponse;

#if DEBUG
        /// <summary>Name of the nested resource info for which we were asked to check duplication on its start.</summary>
        /// <remarks>If this is set, the next call must be a real check for the same nested resource info.</remarks>
        private string startNestedResourceInfoName;
#endif

        /// <summary>
        /// A cache of property names to detect duplicate property names. The <see cref="DuplicationKind"/> value stored
        /// for a given property name indicates what should happen if another property with the same name is found.
        /// See the comments on <see cref="DuplicationKind"/> for more details.
        /// </summary>
        private Dictionary<string, DuplicationRecord> propertyNameCache;

        /// <summary>
        /// Creates a DuplicatePropertyNamesChecker instance.
        /// </summary>
        /// <param name="allowDuplicateProperties">true if duplicate properties are allowed; otherwise false.</param>
        /// <param name="isResponse">true if we're processing a response; false if it's a request.</param>
        public DuplicatePropertyNamesChecker(bool allowDuplicateProperties, bool isResponse)
        {
            this.allowDuplicateProperties = allowDuplicateProperties;
            this.isResponse = isResponse;
        }

        /// <summary>
        /// An enumeration to represent the duplication kind of a given property name.
        /// </summary>
        /// <remarks>
        /// This enumeration is used to determine what should happen if two properties with the same name are detected on a resource or complex value.
        /// When the first property is found, the initial value is set based on the kind of property found and the general setting to allow or disallow duplicate properties.
        /// When a second property with the same name is found, the duplication kind can be 'upgraded' (e.g., from association link to navigation property), 'ignored' (e.g.
        /// when finding the association link for an existing navigation property or when duplicate properties are allowed by the settings) or 'fail'
        /// (e.g., when duplicate properties are not allowed).
        /// </remarks>
        private enum DuplicationKind
        {
            /// <summary>We don't know enough about the property to determine its duplication kind yet, we've just seen a property annotation for it.</summary>
            PropertyAnnotationSeen,

            /// <summary>Duplicates for this property name are not allowed.</summary>
            Prohibited,

            /// <summary>This kind indicates that duplicates are allowed (if the settings allow duplicates).</summary>
            PotentiallyAllowed,

            /// <summary>A navigation link or association link was reported.</summary>
            NavigationProperty,
        }

        /// <summary>
        /// Check the <paramref name="property"/> for duplicate property names in an entry or complex value.
        /// If not explicitly allowed throw when duplicate properties are detected.
        /// If duplicate properties are allowed see the comment on ODataWriterBehavior.AllowDuplicatePropertyNames
        /// or ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </summary>
        /// <param name="property">The property to be checked.</param>
        internal void CheckForDuplicatePropertyNames(ODataProperty property)
        {
            Debug.Assert(property != null, "property != null");
#if DEBUG
            Debug.Assert(this.startNestedResourceInfoName == null, "CheckForDuplicatePropertyNamesOnNestedResourceInfoStart was followed by a CheckForDuplicatePropertyNames(ODataProperty).");
#endif

            string propertyName = property.Name;
            DuplicationKind duplicationKind = GetDuplicationKind(property);
            DuplicationRecord existingDuplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out existingDuplicationRecord))
            {
                this.propertyNameCache.Add(propertyName, new DuplicationRecord(duplicationKind));
            }
            else if (existingDuplicationRecord.DuplicationKind == DuplicationKind.PropertyAnnotationSeen)
            {
                existingDuplicationRecord.DuplicationKind = duplicationKind;
            }
            else
            {
                // If either of them prohibits duplication, fail
                // If the existing one is an association link, fail (association links don't allow duplicates with simple properties)
                // If we don't allow duplication in the first place, fail, since there is no valid case where a simple property coexists with anything else with the same name.
                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.Prohibited ||
                    duplicationKind == DuplicationKind.Prohibited ||
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.AssociationLinkName != null) ||
                    !this.allowDuplicateProperties)
                {
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(propertyName));
                }
                else
                {
                    // Otherwise allow the duplicate.
                    // Note that we don't modify the existing duplication record in any way if the new property is a simple property.
                    // This is because if the existing one is a simple property which allows duplication as well, there's nothing to change.
                    // and if the existing one is a navigation property the navigation property information is more important than the simple property one.
                }
            }
        }

        /// <summary>
        /// Checks the <paramref name="nestedResourceInfo"/> for duplicate property names in a resource when the nested resource info
        /// has started but we don't know yet if it's expanded or not.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to be checked.</param>
        internal void CheckForDuplicatePropertyNamesOnNestedResourceInfoStart(ODataNestedResourceInfo nestedResourceInfo)
        {
            Debug.Assert(nestedResourceInfo != null, "nestedResourceInfo != null");
#if DEBUG
            this.startNestedResourceInfoName = nestedResourceInfo.Name;
#endif

            // Just check for duplication without modifying anything in the caches - this is to allow callers to choose whether they want to call this method first
            // or just call the CheckForDuplicatePropertyNames(ODataNestedResourceInfo) directly.
            string propertyName = nestedResourceInfo.Name;
            DuplicationRecord existingDuplicationRecord;
            if (this.propertyNameCache != null && this.propertyNameCache.TryGetValue(propertyName, out existingDuplicationRecord))
            {
                this.CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(propertyName, existingDuplicationRecord);
            }
        }

        /// <summary>
        /// Check the <paramref name="nestedResourceInfo"/> for duplicate property names in a resource or complex value.
        /// If not explicitly allowed throw when duplicate properties are detected.
        /// If duplicate properties are allowed see the comment on ODataWriterBehavior.AllowDuplicatePropertyNames
        /// or ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to be checked.</param>
        /// <param name="isExpanded">true if the link is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the nested resource info is a collection, false if it's a singleton or null if we don't know.</param>
        /// <returns>The association link uri with the same name if there already was one.</returns>
        internal Uri CheckForDuplicatePropertyNames(ODataNestedResourceInfo nestedResourceInfo, bool isExpanded, bool? isCollection)
        {
#if DEBUG
            this.startNestedResourceInfoName = null;
#endif

            string propertyName = nestedResourceInfo.Name;
            DuplicationRecord existingDuplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out existingDuplicationRecord))
            {
                DuplicationRecord duplicationRecord = new DuplicationRecord(DuplicationKind.NavigationProperty);
                ApplyNestedResourceInfoToDuplicationRecord(duplicationRecord, nestedResourceInfo, isExpanded, isCollection);
                this.propertyNameCache.Add(propertyName, duplicationRecord);
                return null;
            }
            else
            {
                // First check for duplication without expansion knowledge.
                this.CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(propertyName, existingDuplicationRecord);

                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.PropertyAnnotationSeen ||
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty &&
                    existingDuplicationRecord.AssociationLinkName != null &&
                    existingDuplicationRecord.NestedResourceInfo == null))
                {
                    // If the existing one is just an association link, update it to include the nested resource info portion as well
                    ApplyNestedResourceInfoToDuplicationRecord(existingDuplicationRecord, nestedResourceInfo, isExpanded, isCollection);
                }
                else if (this.allowDuplicateProperties)
                {
                    Debug.Assert(
                        (existingDuplicationRecord.DuplicationKind == DuplicationKind.PotentiallyAllowed || existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty),
                        "We should have already taken care of prohibit duplication.");

                    // If the configuration explicitly allows duplication, then just turn the existing property into a nav link with all the information we have
                    existingDuplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
                    ApplyNestedResourceInfoToDuplicationRecord(existingDuplicationRecord, nestedResourceInfo, isExpanded, isCollection);
                }
                else
                {
                    // We've found two navigation links in a request
                    Debug.Assert(
                        existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.NestedResourceInfo != null && !this.isResponse,
                        "We can only get here if we've found two navigation links in a request.");

                    bool? isCollectionEffectiveValue = GetIsCollectionEffectiveValue(isExpanded, isCollection);

                    // If one of them is a definitive singleton, then we fail.
                    if (isCollectionEffectiveValue == false || existingDuplicationRecord.NavigationPropertyIsCollection == false)
                    {
                        // This is the case where an expanded singleton is followed by a deferred link for example.
                        // Once we know for sure that the nav. prop. is a singleton we can't allow more than one link for it.
                        throw new ODataException(Strings.DuplicatePropertyNamesChecker_MultipleLinksForSingleton(propertyName));
                    }

                    // Otherwise allow it, but update the link with the new information
                    if (isCollectionEffectiveValue.HasValue)
                    {
                        existingDuplicationRecord.NavigationPropertyIsCollection = isCollectionEffectiveValue;
                    }
                }

                return existingDuplicationRecord.AssociationLinkUrl;
            }
        }

        /// <summary>
        /// Check the <paramref name="associationLinkName"/> for duplicate property names in a resource or complex value.
        /// If not explicitly allowed throw when duplicate properties are detected.
        /// If duplicate properties are allowed see the comment on ODataWriterBehavior.AllowDuplicatePropertyNames
        /// or ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </summary>
        /// <param name="associationLinkName">The name of association link to be checked.</param>
        /// <param name="associationLinkUrl">The url of association link to be checked.</param>
        /// <returns>The navigation link with the same name as the association link if there's one.</returns>
        internal ODataNestedResourceInfo CheckForDuplicateAssociationLinkNames(string associationLinkName, Uri associationLinkUrl)
        {
            Debug.Assert(associationLinkName != null, "associationLinkName != null");
#if DEBUG
            Debug.Assert(this.startNestedResourceInfoName == null, "CheckForDuplicatePropertyNamesOnNestedResourceInfoStart was followed by a CheckForDuplicatePropertyNames(ODataProperty).");
#endif

            DuplicationRecord existingDuplicationRecord;
            if (!this.TryGetDuplicationRecord(associationLinkName, out existingDuplicationRecord))
            {
                this.propertyNameCache.Add(
                    associationLinkName,
                    new DuplicationRecord(DuplicationKind.NavigationProperty)
                    {
                        AssociationLinkName = associationLinkName,
                        AssociationLinkUrl = associationLinkUrl
                    });

                return null;
            }
            else
            {
                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.PropertyAnnotationSeen ||
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.AssociationLinkName == null))
                {
                    // The only valid case for a duplication with association link is if the existing record is for a navigation property
                    // which doesn't have its association link yet.
                    // In this case just mark the navigation property as having found its association link.
                    existingDuplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
                    existingDuplicationRecord.AssociationLinkName = associationLinkName;
                    existingDuplicationRecord.AssociationLinkUrl = associationLinkUrl;
                }
                else
                {
                    // In all other cases fail.
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(associationLinkName));
                }

                return existingDuplicationRecord.NestedResourceInfo;
            }
        }

        /// <summary>
        /// Clear the internal data structures of the checker so it can be reused.
        /// </summary>
        internal void Clear()
        {
            if (this.propertyNameCache != null)
            {
                this.propertyNameCache.Clear();
            }
        }

        /// <summary>
        /// Adds an OData annotation to a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to add annotation to. string.empty means the annotation is for the current scope.</param>
        /// <param name="annotationName">The name of the annotation to add.</param>
        /// <param name="annotationValue">The value of the annotation to add.</param>
        internal void AddODataPropertyAnnotation(string propertyName, string annotationName, object annotationValue)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName), "annotationName must be an OData annotation.");

            DuplicationRecord duplicationRecord = this.GetDuplicationRecordToAddPropertyAnnotation(propertyName, annotationName);
            Dictionary<string, object> odataAnnotations = duplicationRecord.PropertyODataAnnotations;
            if (odataAnnotations == null)
            {
                odataAnnotations = new Dictionary<string, object>(StringComparer.Ordinal);
                duplicationRecord.PropertyODataAnnotations = odataAnnotations;
            }
            else if (odataAnnotations.ContainsKey(annotationName))
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    if (ODataJsonLightReaderUtils.IsAnnotationProperty(annotationName) && !ODataJsonLightUtils.IsMetadataReferenceProperty(annotationName))
                    {
                        throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed(annotationName));
                    }

                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(annotationName));
                }
                else
                {
                    if (ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName))
                    {
                        throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationForInstanceAnnotationNotAllowed(annotationName, propertyName));
                    }

                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed(annotationName, propertyName));
                }
            }

            odataAnnotations.Add(annotationName, annotationValue);
        }

        /// <summary>
        /// Adds a custom annotation to a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to add annotation to. string.empty means the annotation is for the current scope.</param>
        /// <param name="annotationName">The name of the annotation to add.</param>
        /// <param name="annotationValue">The value of the annotation to add.</param>
        internal void AddCustomPropertyAnnotation(string propertyName, string annotationName, object annotationValue)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(!JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName), "annotationName must not be an OData annotation.");

            DuplicationRecord duplicationRecord = this.GetDuplicationRecordToAddPropertyAnnotation(propertyName, annotationName);
            Dictionary<string, object> customAnnotations = duplicationRecord.PropertyCustomAnnotations;
            if (customAnnotations == null)
            {
                customAnnotations = new Dictionary<string, object>(StringComparer.Ordinal);
                duplicationRecord.PropertyCustomAnnotations = customAnnotations;
            }
            else if (customAnnotations.ContainsKey(annotationName))
            {
                if (string.IsNullOrEmpty(propertyName))
                {
                    if (ODataJsonLightReaderUtils.IsAnnotationProperty(annotationName) && !ODataJsonLightUtils.IsMetadataReferenceProperty(annotationName))
                    {
                        throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed(annotationName));
                    }

                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(annotationName));
                }
                else
                {
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed(annotationName, propertyName));
                }
            }

            customAnnotations.Add(annotationName, annotationValue);
        }

        /// <summary>
        /// Returns OData annotations for the specified property with name <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return the annotations for.</param>
        /// <returns>Enumeration of pairs of OData annotation name and the annotation value, or null if there are no OData annotations for the property.</returns>
        internal Dictionary<string, object> GetODataPropertyAnnotations(string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            DuplicationRecord duplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out duplicationRecord))
            {
                return null;
            }

            // TODO: Refactor the duplicate property names checker and use different implementations for JSON Light
            //      and the other formats (most of the logic is not needed for JSON Light).
            //      Once we create a JSON Light specific duplicate property names checker, we will check for duplicates in
            //      the ParseProperty method and thus detect duplicates before we get here.
            ThrowIfPropertyIsProcessed(propertyName, duplicationRecord);
            return duplicationRecord.PropertyODataAnnotations;
        }

        /// <summary>
        /// Returns custom instance annotations for the specified property with name <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return the annotations for.</param>
        /// <returns>Enumeration of pairs of custom instance annotation name and the annotation value, or null if there are no OData annotations for the property.</returns>
        internal Dictionary<string, object> GetCustomPropertyAnnotations(string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            DuplicationRecord duplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out duplicationRecord))
            {
                return null;
            }

            // TODO: Refactor the duplicate property names checker and use different implementations for JSON Light
            //      and the other formats (most of the logic is not needed for JSON Light).
            //      Once we create a JSON Light specific duplicate property names checker, we will check for duplicates in
            //      the ParseProperty method and thus detect duplicates before we get here.
            ThrowIfPropertyIsProcessed(propertyName, duplicationRecord);
            return duplicationRecord.PropertyCustomAnnotations;
        }

        /// <summary>
        /// Marks the <paramref name="propertyName"/> property to note that all its annotations were already processed.
        /// </summary>
        /// <param name="propertyName">The property name to mark.</param>
        /// <remarks>
        /// Properties marked like this will fail if there are more annotations found for them in the payload.
        /// </remarks>
        internal void MarkPropertyAsProcessed(string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            DuplicationRecord duplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out duplicationRecord))
            {
                duplicationRecord = new DuplicationRecord(DuplicationKind.PropertyAnnotationSeen);
                this.propertyNameCache.Add(propertyName, duplicationRecord);
            }

            ThrowIfPropertyIsProcessed(propertyName, duplicationRecord);
            duplicationRecord.PropertyODataAnnotations = propertyAnnotationsProcessedToken;
        }

        /// <summary>
        /// Throw if property is processed already.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="duplicationRecord">DuplicationRecord of the property.</param>
        private static void ThrowIfPropertyIsProcessed(string propertyName, DuplicationRecord duplicationRecord)
        {
            if (object.ReferenceEquals(duplicationRecord.PropertyODataAnnotations, propertyAnnotationsProcessedToken))
            {
                if (ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName) && !ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName))
                {
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationNotAllowed(propertyName));
                }

                throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(propertyName));
            }
        }

        /// <summary>
        /// Decides whether a the given <paramref name="property"/> supports duplicates (if allowed by the settings).
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>true if the <paramref name="property"/> supports duplicates (if allowed by the settings); otherwise false.</returns>
        private static DuplicationKind GetDuplicationKind(ODataProperty property)
        {
            object value = property.Value;
            return value != null && (value is ODataStreamReferenceValue || value is ODataCollectionValue)
                ? DuplicationKind.Prohibited
                : DuplicationKind.PotentiallyAllowed;
        }

        /// <summary>
        /// Determines the effective value for the isCollection flag.
        /// </summary>
        /// <param name="isExpanded">true if the nested resource info is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the nested resource info is marked as collection, false if it's marked as singleton or null if we don't know.</param>
        /// <returns>The effective value of the isCollection flag. Note that we can't rely on singleton links which are not expanded since
        /// those can appear even in cases where the actual navigation property is a collection.
        /// We allow singleton deferred links for collection properties in requests, as that is one way of expressing a bind operation.</returns>
        private static bool? GetIsCollectionEffectiveValue(bool isExpanded, bool? isCollection)
        {
            return isExpanded ? isCollection : (isCollection == true ? (bool?)true : null);
        }

        /// <summary>
        /// Sets the properties on a duplication record for a nested resource info.
        /// </summary>
        /// <param name="duplicationRecord">The duplication record to modify.</param>
        /// <param name="nestedResourceInfo">The nested resource info found for this property.</param>
        /// <param name="isExpanded">true if the nested resource info is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the nested resource info is marked as collection, false if it's marked as singleton or null if we don't know.</param>
        private static void ApplyNestedResourceInfoToDuplicationRecord(DuplicationRecord duplicationRecord, ODataNestedResourceInfo nestedResourceInfo, bool isExpanded, bool? isCollection)
        {
            duplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
            duplicationRecord.NestedResourceInfo = nestedResourceInfo;

            // We only take the cardinality of the link for granted if it was expanded or if it is a collection.
            // We can't rely on singleton deferred links to know the cardinality since they can be used for binding even if the actual link is a collection.
            duplicationRecord.NavigationPropertyIsCollection = GetIsCollectionEffectiveValue(isExpanded, isCollection);
        }

        /// <summary>
        /// Tries to get an existing duplication record for the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The property name to look for.</param>
        /// <param name="duplicationRecord">The existing duplication if one was already found.</param>
        /// <returns>true if a duplication record already exists, false otherwise.</returns>
        /// <remarks>This method also initializes the cache if it was not initialized yet.</remarks>
        private bool TryGetDuplicationRecord(string propertyName, out DuplicationRecord duplicationRecord)
        {
            if (this.propertyNameCache == null)
            {
                this.propertyNameCache = new Dictionary<string, DuplicationRecord>(StringComparer.Ordinal);
                duplicationRecord = null;
                return false;
            }

            return this.propertyNameCache.TryGetValue(propertyName, out duplicationRecord);
        }

        /// <summary>
        /// Checks for duplication of a nested resource info against an existing duplication record.
        /// </summary>
        /// <param name="propertyName">The name of the nested resource info.</param>
        /// <param name="existingDuplicationRecord">The existing duplication record.</param>
        /// <remarks>This only performs checks possible without the knowledge of whether the link was expanded or not.</remarks>
        private void CheckNestedResourceInfoDuplicateNameForExistingDuplicationRecord(string propertyName, DuplicationRecord existingDuplicationRecord)
        {
            if (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty &&
                existingDuplicationRecord.AssociationLinkUrl != null &&
                existingDuplicationRecord.NestedResourceInfo == null)
            {
                // Existing one is just an association link, so the new one is a navigation link portion which is OK always.
            }
            else if (existingDuplicationRecord.DuplicationKind == DuplicationKind.Prohibited ||
                (existingDuplicationRecord.DuplicationKind == DuplicationKind.PotentiallyAllowed && !this.allowDuplicateProperties) ||
                (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && this.isResponse && !this.allowDuplicateProperties))
            {
                // If the existing one doesn't allow duplication at all,
                // or it's simple property which does allow duplication, but the configuration does not allow it,
                // or it's a duplicate navigation property in a response,
                // fail.
                throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(propertyName));
            }
        }

        /// <summary>
        /// Gets a duplication record to use for adding property annotation.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the duplication record for.</param>
        /// <param name="annotationName">The name of the annotation being added (only for error reporting).</param>
        /// <returns>The duplication record to use. This will never be null.</returns>
        private DuplicationRecord GetDuplicationRecordToAddPropertyAnnotation(string propertyName, string annotationName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            DuplicationRecord duplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out duplicationRecord))
            {
                duplicationRecord = new DuplicationRecord(DuplicationKind.PropertyAnnotationSeen);
                this.propertyNameCache.Add(propertyName, duplicationRecord);
            }

            if (object.ReferenceEquals(duplicationRecord.PropertyODataAnnotations, propertyAnnotationsProcessedToken))
            {
                throw new ODataException(Strings.DuplicatePropertyNamesChecker_PropertyAnnotationAfterTheProperty(annotationName, propertyName));
            }

            Debug.Assert(duplicationRecord != null, "duplicationRecord != null");
            return duplicationRecord;
        }

        /// <summary>
        /// A record of a single property for duplicate property names checking.
        /// </summary>
        private sealed class DuplicationRecord
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="duplicationKind">The duplication kind of the record to create.</param>
            public DuplicationRecord(DuplicationKind duplicationKind)
            {
                this.DuplicationKind = duplicationKind;
            }

            /// <summary>
            /// The duplication kind of the record to create.
            /// </summary>
            public DuplicationKind DuplicationKind { get; set; }

            /// <summary>
            /// The nested resource info if it was already found for this property.
            /// </summary>
            public ODataNestedResourceInfo NestedResourceInfo { get; set; }

            /// <summary>
            /// The association link name if it was already found for this property.
            /// </summary>
            public string AssociationLinkName { get; set; }

            /// <summary>
            /// The association link url if it was already found for this property.
            /// </summary>
            public Uri AssociationLinkUrl { get; set; }

            /// <summary>
            /// true if we know for sure that the navigation property with the property name is a collection,
            /// false if we know for sure that the navigation property with the property name is a singleton,
            /// null if we don't know the cardinality of the navigation property for sure (yet).
            /// </summary>
            public bool? NavigationPropertyIsCollection { get; set; }

            /// <summary>
            /// Dictionary of OData annotations for the property for which the duplication record is stored.
            /// </summary>
            /// <remarks>
            /// The key of the dictionary is the fully qualified annotation name (i.e. odata.type),
            /// the value is the parsed value of the annotation (this is annotation specific).
            /// </remarks>
            public Dictionary<string, object> PropertyODataAnnotations { get; set; }

            /// <summary>
            /// Dictionary of custom annotations for the property for which the duplication record is stored.
            /// </summary>
            /// <remarks>
            /// The key of the dictionary is the fully qualified annotation name ,
            /// the value is the parsed value of the annotation (this is annotation specific).
            /// </remarks>
            public Dictionary<string, object> PropertyCustomAnnotations { get; set; }
        }
    }
}
