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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using Microsoft.Data.OData.JsonLight;
    using Microsoft.Data.OData.Json;

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
        /// <summary>Name of the navigation link for which we were asked to check duplication on its start.</summary>
        /// <remarks>If this is set, the next call must be a real check for the same navigation link.</remarks>
        private string startNavigationLinkName;
#endif

        /// <summary>
        /// A cache of property names to detect duplicate property names. The <see cref="DuplicationKind"/> value stored
        /// for a given property name indicates what should happen if another property with the same name is found.
        /// See the comments on <see cref="DuplicationKind"/> for more details.
        /// </summary>
        private Dictionary<string, DuplicationRecord> propertyNameCache;

        /// <summary>
        /// The annotation collector.
        /// </summary>
        private PropertyAnnotationCollector annotationCollector = new PropertyAnnotationCollector();

        /// <summary>
        /// Constructor.
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
        /// This enumeration is used to determine what should happen if two properties with the same name are detected on an entry or complex value.
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
        /// The raw annotation collector.
        /// </summary>
        public PropertyAnnotationCollector AnnotationCollector
        {
            get
            {
                return this.annotationCollector;
            }
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(property != null, "property != null");
#if DEBUG
            Debug.Assert(this.startNavigationLinkName == null, "CheckForDuplicatePropertyNamesOnNavigationLinkStart was followed by a CheckForDuplicatePropertyNames(ODataProperty).");
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
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.AssociationLink != null) ||
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
        /// Checks the <paramref name="navigationLink"/> for duplicate property names in an entry when the navigation link
        /// has started but we don't know yet if it's expanded or not.
        /// </summary>
        /// <param name="navigationLink">The navigation link to be checked.</param>
        internal void CheckForDuplicatePropertyNamesOnNavigationLinkStart(ODataNavigationLink navigationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");
#if DEBUG
            this.startNavigationLinkName = navigationLink.Name;
#endif

            // Just check for duplication without modifying anything in the caches - this is to allow callers to choose whether they want to call this method first
            // or just call the CheckForDuplicatePropertyNames(ODataNavigationLink) directly.
            string propertyName = navigationLink.Name;
            DuplicationRecord existingDuplicationRecord;
            if (this.propertyNameCache != null && this.propertyNameCache.TryGetValue(propertyName, out existingDuplicationRecord))
            {
                this.CheckNavigationLinkDuplicateNameForExistingDuplicationRecord(propertyName, existingDuplicationRecord);
            }
        }

        /// <summary>
        /// Check the <paramref name="navigationLink"/> for duplicate property names in an entry or complex value.
        /// If not explicitly allowed throw when duplicate properties are detected.
        /// If duplicate properties are allowed see the comment on ODataWriterBehavior.AllowDuplicatePropertyNames 
        /// or ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </summary>
        /// <param name="navigationLink">The navigation link to be checked.</param>
        /// <param name="isExpanded">true if the link is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the navigation link is a collection, false if it's a singleton or null if we don't know.</param>
        /// <returns>The association link with the same name if there already was one.</returns>
        internal ODataAssociationLink CheckForDuplicatePropertyNames(ODataNavigationLink navigationLink, bool isExpanded, bool? isCollection)
        {
            DebugUtils.CheckNoExternalCallers();
#if DEBUG
            this.startNavigationLinkName = null;
#endif

            string propertyName = navigationLink.Name;
            DuplicationRecord existingDuplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out existingDuplicationRecord))
            {
                DuplicationRecord duplicationRecord = new DuplicationRecord(DuplicationKind.NavigationProperty);
                ApplyNavigationLinkToDuplicationRecord(duplicationRecord, navigationLink, isExpanded, isCollection);
                this.propertyNameCache.Add(propertyName, duplicationRecord);
                return null;
            }
            else
            {
                // First check for duplication without expansion knowledge.
                this.CheckNavigationLinkDuplicateNameForExistingDuplicationRecord(propertyName, existingDuplicationRecord);

                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.PropertyAnnotationSeen ||
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty &&
                    existingDuplicationRecord.AssociationLink != null &&
                    existingDuplicationRecord.NavigationLink == null))
                {
                    // If the existing one is just an association link, update it to include the navigation link portion as well
                    ApplyNavigationLinkToDuplicationRecord(existingDuplicationRecord, navigationLink, isExpanded, isCollection);
                }
                else if (this.allowDuplicateProperties)
                {
                    Debug.Assert(
                        (existingDuplicationRecord.DuplicationKind == DuplicationKind.PotentiallyAllowed || existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty),
                        "We should have already taken care of prohibit duplication.");

                    // If the configuration explicitly allows duplication, then just turn the existing property into a nav link with all the information we have
                    existingDuplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
                    ApplyNavigationLinkToDuplicationRecord(existingDuplicationRecord, navigationLink, isExpanded, isCollection);
                }
                else
                {
                    // We've found two navigation links in a request
                    Debug.Assert(
                        existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.NavigationLink != null && !this.isResponse,
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

                return existingDuplicationRecord.AssociationLink;
            }
        }

        /// <summary>
        /// Check the <paramref name="associationLink"/> for duplicate property names in an entry or complex value.
        /// If not explicitly allowed throw when duplicate properties are detected.
        /// If duplicate properties are allowed see the comment on ODataWriterBehavior.AllowDuplicatePropertyNames  
        /// or ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </summary>
        /// <param name="associationLink">The association link to be checked.</param>
        /// <returns>The navigation link with the same name as the association link if there's one.</returns>
        internal ODataNavigationLink CheckForDuplicateAssociationLinkNames(ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(associationLink != null, "associationLink != null");
#if DEBUG
            Debug.Assert(this.startNavigationLinkName == null, "CheckForDuplicatePropertyNamesOnNavigationLinkStart was followed by a CheckForDuplicatePropertyNames(ODataProperty).");
#endif

            DuplicationRecord existingDuplicationRecord;
            string associationLinkName = associationLink.Name;
            if (!this.TryGetDuplicationRecord(associationLinkName, out existingDuplicationRecord))
            {
                this.propertyNameCache.Add(
                    associationLinkName,
                    new DuplicationRecord(DuplicationKind.NavigationProperty)
                    {
                        AssociationLink = associationLink
                    });

                return null;
            }
            else
            {
                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.PropertyAnnotationSeen ||
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.AssociationLink == null))
                {
                    // The only valid case for a duplication with association link is if the existing record is for a navigation property
                    // which doesn't have its association link yet.
                    // In this case just mark the navigation property as having found its association link.
                    existingDuplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
                    existingDuplicationRecord.AssociationLink = associationLink;
                }
                else
                {
                    // In all other cases fail.
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(associationLinkName));
                }

                return existingDuplicationRecord.NavigationLink;
            }
        }

        /// <summary>
        /// Clear the internal data structures of the checker so it can be reused.
        /// </summary>
        internal void Clear()
        {
            DebugUtils.CheckNoExternalCallers();

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
        /// <param name="annotationValue">The valud of the annotation to add.</param>
        internal void AddODataPropertyAnnotation(string propertyName, string annotationName, object annotationValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
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
                if (ODataJsonLightReaderUtils.IsAnnotationProperty(propertyName))
                {
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationForInstanceAnnotationNotAllowed(annotationName, propertyName));
                }

                throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed(annotationName, propertyName));
            }

            odataAnnotations.Add(annotationName, annotationValue);
        }

        /// <summary>
        /// Adds a custom annotation to a property.
        /// </summary>
        /// <param name="propertyName">The name of the property to add annotation to. string.empty means the annotation is for the current scope.</param>
        /// <param name="annotationName">The name of the annotation to add.</param>
        internal void AddCustomPropertyAnnotation(string propertyName, string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");
            Debug.Assert(!JsonLight.ODataJsonLightReaderUtils.IsODataAnnotationName(annotationName), "annotationName must not be an OData annotation.");

            DuplicationRecord duplicationRecord = this.GetDuplicationRecordToAddPropertyAnnotation(propertyName, annotationName);
            HashSet<string> customAnnotations = duplicationRecord.PropertyCustomAnnotations;
            if (customAnnotations == null)
            {
                customAnnotations = new HashSet<string>(StringComparer.Ordinal);
                duplicationRecord.PropertyCustomAnnotations = customAnnotations;
            }
            else if (customAnnotations.Contains(annotationName))
            {
                throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicateAnnotationForPropertyNotAllowed(annotationName, propertyName));
            }

            customAnnotations.Add(annotationName);
        }

        /// <summary>
        /// Returns OData annotations for the specified property with name <paramref name="propertyName"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return the annotations for.</param>
        /// <returns>Enumeration of pairs of OData annotation name and and the annotation value, or null if there are no OData annotations for the property.</returns>
        internal Dictionary<string, object> GetODataPropertyAnnotations(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
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
        /// Marks the <paramref name="propertyName"/> property to note that all its annotations were already processed.
        /// </summary>
        /// <param name="propertyName">The property name to mark.</param>
        /// <remarks>
        /// Properties marked like this will fail if there are more annotations found for them in the payload.
        /// </remarks>
        internal void MarkPropertyAsProcessed(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
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
        /// Returns the names of all properties which have not been marked as processed through <see cref="MarkPropertyAsProcessed"/>.
        /// </summary>
        /// <returns>A set of property names.</returns>
        internal IEnumerable<string> GetAllUnprocessedProperties()
        {
            DebugUtils.CheckNoExternalCallers();

            if (this.propertyNameCache == null)
            {
                return Enumerable.Empty<string>();
            }

            return this.propertyNameCache
                .Where(IsPropertyUnprocessed)
                .Select(property => property.Key);
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
        /// Indicates whether a property's annotations have not yet been processed.
        /// </summary>
        /// <param name="property">The name of the property to check.</param>
        /// <returns>true if the property associated with the given name has unprocessed annotations.</returns>
        private static bool IsPropertyUnprocessed(KeyValuePair<string, DuplicationRecord> property)
        {
            return !string.IsNullOrEmpty(property.Key) && !object.ReferenceEquals(property.Value.PropertyODataAnnotations, propertyAnnotationsProcessedToken);
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
        /// <param name="isExpanded">true if the navigation link is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the navigation link is marked as collection, false if it's marked as singletong or null if we don't know.</param>
        /// <returns>The effective value of the isCollection flag. Note that we can't rely on singleton links which are not expanded since
        /// those can appear even in cases where the actual navigation property is a collection.
        /// We allow singleton deferred links for collection properties in requests, as that is one way of expressing a bind operation.</returns>
        private static bool? GetIsCollectionEffectiveValue(bool isExpanded, bool? isCollection)
        {
            return isExpanded ? isCollection : (isCollection == true ? (bool?)true : null);
        }

        /// <summary>
        /// Sets the properties on a duplication record for a navigation link.
        /// </summary>
        /// <param name="duplicationRecord">The duplication record to modify.</param>
        /// <param name="navigationLink">The navigation link found for this property.</param>
        /// <param name="isExpanded">true if the navigation link is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the navigation link is marked as collection, false if it's marked as singletong or null if we don't know.</param>
        private static void ApplyNavigationLinkToDuplicationRecord(DuplicationRecord duplicationRecord, ODataNavigationLink navigationLink, bool isExpanded, bool? isCollection)
        {
            duplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
            duplicationRecord.NavigationLink = navigationLink;

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
        /// Checks for duplication of a navigation link against an existing duplication record.
        /// </summary>
        /// <param name="propertyName">The name of the navigation link.</param>
        /// <param name="existingDuplicationRecord">The existing duplication record.</param>
        /// <remarks>This only performs checks possible without the knowledge of whether the link was expanded or not.</remarks>
        private void CheckNavigationLinkDuplicateNameForExistingDuplicationRecord(string propertyName, DuplicationRecord existingDuplicationRecord)
        {
            if (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty &&
                existingDuplicationRecord.AssociationLink != null &&
                existingDuplicationRecord.NavigationLink == null)
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
        /// An independent annotation collector to collect the raw json annotations.
        /// </summary>
        internal sealed class PropertyAnnotationCollector
        {
            /// <summary>
            /// The raw annotations.
            /// </summary>
            private Dictionary<string, Dictionary<string, string>> propertyAnnotations =
                new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);

            /// <summary>
            /// If should collect annotation.
            /// </summary>
            private bool shouldCollectAnnotation;

            /// <summary>
            /// Gets or sets if should collect annotation;
            /// </summary>
            public bool ShouldCollectAnnotation
            {
                get
                {
                    return this.shouldCollectAnnotation;
                }

                set
                {
                    this.shouldCollectAnnotation = value;
                }
            }

            /// <summary>
            /// Tries to peek and collect a raw annotation value from BufferingJsonReader.
            /// </summary>
            /// <param name="jsonReader">The BufferingJsonReader.</param>
            /// <param name="propertyName">The property name.</param>
            /// <param name="annotationName">The annotation name.</param>
            public void TryPeekAndCollectAnnotationRawJson(
                BufferingJsonReader jsonReader,
                string propertyName,
                string annotationName)
            {
                if (this.shouldCollectAnnotation)
                {
                    this.PeekAndCollectAnnotationRawJson(jsonReader, propertyName, annotationName);
                }
            }

            /// <summary>
            /// Tries to add property annotation raw json.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            /// <param name="annotationName">The annotation name.</param>
            /// <param name="rawJson">The raw json string.</param>
            public void TryAddPropertyAnnotationRawJson(string propertyName, string annotationName, string rawJson)
            {
                if (this.shouldCollectAnnotation)
                {
                    this.AddPropertyAnnotationRawJson(propertyName, annotationName, rawJson);
                }
            }

            /// <summary>
            /// Gets an ODataJsonLightRawAnnotationSet that can be attached to ODataValue or ODataUntypedValue.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            /// <returns>An ODataJsonLightRawAnnotationSet instance.</returns>
            public ODataJsonLightRawAnnotationSet GetPropertyRawAnnotationSet(string propertyName)
            {
                Dictionary<string, string> annotations = null;
                if (!this.propertyAnnotations.TryGetValue(propertyName, out annotations))
                {
                    return null;
                }

                ODataJsonLightRawAnnotationSet ret = new ODataJsonLightRawAnnotationSet();
                ret.Annotations = annotations;
                return ret;
            }

            /// <summary>
            /// Peeks and collects a raw annotation value from BufferingJsonReader.
            /// </summary>
            /// <param name="jsonReader">The BufferingJsonReader.</param>
            /// <param name="propertyName">The property name.</param>
            /// <param name="annotationName">The annotation name.</param>
            private void PeekAndCollectAnnotationRawJson(
                BufferingJsonReader jsonReader,
                string propertyName,
                string annotationName)
            {
                if (jsonReader.IsBuffering)
                {
                    return; // only need to collect annotation for not-buffering pass reading.
                }

                try
                {
                    jsonReader.StartBuffering();
                    if (jsonReader.NodeType == JsonNodeType.Property)
                    {
                        jsonReader.Read(); // read over annotation name
                    }

                    StringBuilder builder = new StringBuilder();
                    jsonReader.SkipValue(builder);
                    string annotationRawJson = builder.ToString();
                    this.AddPropertyAnnotationRawJson(propertyName, annotationName, annotationRawJson);
                }
                finally
                {
                    jsonReader.StopBuffering();
                }
            }

            /// <summary>
            /// Add property annotation raw json.
            /// </summary>
            /// <param name="propertyName">The property name.</param>
            /// <param name="annotationName">The annotation name.</param>
            /// <param name="rawJson">The raw json string.</param>
            private void AddPropertyAnnotationRawJson(string propertyName, string annotationName, string rawJson)
            {
                Debug.Assert(annotationName != null, "annotationName != null");
                Dictionary<string, string> annotations = null;
                if (!this.propertyAnnotations.TryGetValue(propertyName, out annotations))
                {
                    annotations = new Dictionary<string, string>(StringComparer.Ordinal);
                    this.propertyAnnotations[propertyName] = annotations;
                }

                annotations[annotationName] = rawJson;
            }
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
            /// The navigation link if it was already found for this property.
            /// </summary>
            public ODataNavigationLink NavigationLink { get; set; }

            /// <summary>
            /// The association link if it was already found for this property.
            /// </summary>
            public ODataAssociationLink AssociationLink { get; set; }

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
            /// Hashset of custom annotations for the property for which the duplication record is stored.
            /// </summary>
            /// <remarks>
            /// This is just a hashset for now since we don't read custom annotations, we just need to check for duplicates.
            /// </remarks>
            public HashSet<string> PropertyCustomAnnotations { get; set; }
        }
    }
}
