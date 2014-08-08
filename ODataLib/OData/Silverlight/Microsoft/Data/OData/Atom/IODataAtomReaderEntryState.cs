//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a state of the ATOM reader for entry.
    /// </summary>
    internal interface IODataAtomReaderEntryState
    {
        /// <summary>
        /// The entry being read.
        /// </summary>
        ODataEntry Entry { get; }

        /// <summary>
        /// The entity type for the entry (if available)
        /// </summary>
        IEdmEntityType EntityType { get; }

        /// <summary>
        /// Flag which indicates that the ATOM entry element representing the entry is empty.
        /// </summary>
        bool EntryElementEmpty { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found a read link for this empty (even if it had a null URI value).
        /// </summary>
        bool HasReadLink { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found an edit link for this empty (even if it had a null URI value).
        /// </summary>
        bool HasEditLink { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found an edit-media link for this empty (even if it had a null URI value).
        /// </summary>
        bool HasEditMediaLink { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found an id element.
        /// </summary>
        bool HasId { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found a content element .
        /// </summary>
        bool HasContent { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found a category element with the required type name.
        /// </summary>
        bool HasTypeNameCategory { get; set; }

        /// <summary>
        /// Flag which indicates whether we have found a m:properties element.
        /// </summary>
        bool HasProperties { get; set; }

        /// <summary>
        /// Flag indicating if we have already made a decision about the current entry and its being MLE or not.
        /// If this property has a null value, we don't know for sure yet (both are possible), if it has non-null value
        /// then we already know for sure and if we find something different we should fail.
        /// </summary>
        bool? MediaLinkEntry { get; set; }

        /// <summary>
        /// If the reader finds a navigation link to report, but it must first report the parent entry
        /// it will store the navigation link descriptor in this property. So this will only ever store the first navigation link of an entry.
        /// </summary>
        ODataAtomReaderNavigationLinkDescriptor FirstNavigationLinkDescriptor { get; set; }

        /// <summary>
        /// The duplicate property names checker for the entry represented by the current state.
        /// </summary>
        DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; }

        /// <summary>
        /// The EPM information for the entry, or null if there's no EPM for this entry.
        /// </summary>
        ODataEntityPropertyMappingCache CachedEpm { get; }

        /// <summary>
        /// The ATOM entry metadata to fill as we read the content of the entry.
        /// </summary>
        AtomEntryMetadata AtomEntryMetadata { get; }

        /// <summary>
        /// The cache for values read from custom EPM.
        /// </summary>
        /// <remarks>
        /// This should only be accessed if there's CachedEpm available for this entry.
        /// </remarks>
        EpmCustomReaderValueCache EpmCustomReaderValueCache { get; }
    }
}
