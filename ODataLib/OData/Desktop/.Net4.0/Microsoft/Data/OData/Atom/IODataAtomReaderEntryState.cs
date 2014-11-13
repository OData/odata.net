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
