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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Helper class to verify that no duplicate properties are specified for entries and complex values.
    /// </summary>
    internal sealed class DuplicatePropertyNamesChecker
    {
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
            else
            {
                // If either of them prohibits duplication, fail
                // If the existing one is an association link, fail (association links don't allow duplicates with simple properties)
                // If we don't allow duplication in the first place, fail, since there is no valid case where a simple property coexists with anything else with the same name.
                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.Prohibited ||
                    duplicationKind == DuplicationKind.Prohibited ||
                    (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.AssociationLinkFound) ||
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
        internal void CheckForDuplicatePropertyNames(ODataNavigationLink navigationLink, bool isExpanded, bool? isCollection)
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
                ApplyNavigationLinkToDuplicationRecord(duplicationRecord, isExpanded, isCollection);
                this.propertyNameCache.Add(propertyName, duplicationRecord);
            }
            else
            {
                // First check for duplication without expansion knowledge.
                this.CheckNavigationLinkDuplicateNameForExistingDuplicationRecord(propertyName, existingDuplicationRecord);

                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty &&
                    existingDuplicationRecord.AssociationLinkFound &&
                    !existingDuplicationRecord.NavigationLinkFound)
                {
                    // If the existing one is just an association link, update it to include the navigation link portion as well
                    ApplyNavigationLinkToDuplicationRecord(existingDuplicationRecord, isExpanded, isCollection);
                }
                else if (this.allowDuplicateProperties)
                {
                    Debug.Assert(
                        (existingDuplicationRecord.DuplicationKind == DuplicationKind.PotentiallyAllowed || existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty),
                        "We should have already taken care of prohibit duplication.");

                    // If the configuration explicitly allows duplication, then just turn the existing property into a nav link with all the information we have
                    existingDuplicationRecord.DuplicationKind = DuplicationKind.NavigationProperty;
                    ApplyNavigationLinkToDuplicationRecord(existingDuplicationRecord, isExpanded, isCollection);
                }
                else
                {
                    // We've found two navigation links in a request
                    Debug.Assert(
                        existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && existingDuplicationRecord.NavigationLinkFound && !this.isResponse,
                        "We can only get here if we've found two navigation links in a request.");

                    // If both of them are expanded, fail.
                    if (isExpanded && existingDuplicationRecord.ExpandedNavigationLinkFound)
                    {
                        // This is the case where two expanded links are found.
                        throw new ODataException(Strings.DuplicatePropertyNamesChecker_MultipleExpandedLinks(propertyName));
                    }

                    bool? isCollectionEffectiveValue = GetIsCollectionEffectiveValue(isExpanded, isCollection);

                    // If one of them is a definitive singleton, then we fail.
                    if (isCollectionEffectiveValue == false || existingDuplicationRecord.NavigationPropertyIsCollection == false)
                    {
                        // This is the case where an expanded singleton is followed by a deferred link for example.
                        // Once we know for sure that the nav. prop. is a singleton we can't allow more than one link for it.
                        throw new ODataException(Strings.DuplicatePropertyNamesChecker_MultipleLinksForSingleton(propertyName));
                    }

                    // Otherwise allow it, but update the link with the new information
                    if (isExpanded)
                    {
                        existingDuplicationRecord.ExpandedNavigationLinkFound = true;
                    }

                    if (isCollectionEffectiveValue.HasValue)
                    {
                        existingDuplicationRecord.NavigationPropertyIsCollection = isCollectionEffectiveValue;
                    }
                }
            }
        }

        /// <summary>
        /// Check the <paramref name="associationLink"/> for duplicate property names in an entry or complex value.
        /// If not explicitly allowed throw when duplicate properties are detected.
        /// If duplicate properties are allowed see the comment on ODataWriterBehavior.AllowDuplicatePropertyNames  
        /// or ODataReaderBehavior.AllowDuplicatePropertyNames for further details.
        /// </summary>
        /// <param name="associationLink">The association link to be checked.</param>
        internal void CheckForDuplicatePropertyNames(ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(associationLink != null, "associationLink != null");
#if DEBUG
            Debug.Assert(this.startNavigationLinkName == null, "CheckForDuplicatePropertyNamesOnNavigationLinkStart was followed by a CheckForDuplicatePropertyNames(ODataProperty).");
#endif

            string propertyName = associationLink.Name;
            DuplicationRecord existingDuplicationRecord;
            if (!this.TryGetDuplicationRecord(propertyName, out existingDuplicationRecord))
            {
                this.propertyNameCache.Add(
                    propertyName,
                    new DuplicationRecord(DuplicationKind.NavigationProperty)
                    {
                        AssociationLinkFound = true
                    });
            }
            else
            {
                if (existingDuplicationRecord.DuplicationKind == DuplicationKind.NavigationProperty && !existingDuplicationRecord.AssociationLinkFound)
                {
                    // The only valid case for a duplication with association link is if the existing record is for a navigation property
                    // which doesn't have its association link yet.
                    // In this case just mark the navigation property as having found its association link.
                    existingDuplicationRecord.AssociationLinkFound = true;
                }
                else
                {
                    // In all other cases fail.
                    throw new ODataException(Strings.DuplicatePropertyNamesChecker_DuplicatePropertyNamesNotAllowed(propertyName));
                }
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
        /// Decides whether a the given <paramref name="property"/> supports duplicates (if allowed by the settings).
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>true if the <paramref name="property"/> supports duplicates (if allowed by the settings); otherwise false.</returns>
        private static DuplicationKind GetDuplicationKind(ODataProperty property)
        {
            object value = property.Value;
            return value != null && (value is ODataStreamReferenceValue || value is ODataMultiValue)
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
        /// <param name="isExpanded">true if the navigation link is expanded, false otherwise.</param>
        /// <param name="isCollection">true if the navigation link is marked as collection, false if it's marked as singletong or null if we don't know.</param>
        private static void ApplyNavigationLinkToDuplicationRecord(DuplicationRecord duplicationRecord, bool isExpanded, bool? isCollection)
        {
            duplicationRecord.NavigationLinkFound = true;
            duplicationRecord.ExpandedNavigationLinkFound = isExpanded;

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
                this.propertyNameCache = new Dictionary<string, DuplicationRecord>(EqualityComparer<string>.Default);
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
                existingDuplicationRecord.AssociationLinkFound &&
                !existingDuplicationRecord.NavigationLinkFound)
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
            /// true if the navigation link for this property name was already found; false if it was not found (yet).
            /// </summary>
            public bool NavigationLinkFound { get; set; }

            /// <summary>
            /// true if the association link for this property name was already found; false if it was not found (yet).
            /// </summary>
            public bool AssociationLinkFound { get; set; }

            /// <summary>
            /// true if an expanded navigation link for this property name was already found; false if it was not found (yet);
            /// </summary>
            public bool ExpandedNavigationLinkFound { get; set; }

            /// <summary>
            /// true if we know for sure that the navigation property with the property name is a collection,
            /// false if we know for sure that the navigation property with the property name is a singleton,
            /// null if we don't know the cardinality of the navigation property for sure (yet).
            /// </summary>
            public bool? NavigationPropertyIsCollection { get; set; }
        }
    }
}
