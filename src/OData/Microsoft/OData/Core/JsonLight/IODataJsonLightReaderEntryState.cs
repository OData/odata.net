//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a state of the JSON reader for entry.
    /// </summary>
    internal interface IODataJsonLightReaderEntryState
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
        /// The metadata builder instance for the entry.
        /// </summary>
        ODataEntityMetadataBuilder MetadataBuilder { get; set; }

        /// <summary>
        /// Flag which indicates that during parsing of the entry represented by this state,
        /// any property which is not an instance annotation was found. This includes property annotations
        /// for property which is not present in the payload.
        /// </summary>
        /// <remarks>
        /// This is used to detect incorrect ordering of the payload (for example odata.id must not come after the first property).
        /// </remarks>
        bool AnyPropertyFound { get; set; }

        /// <summary>
        /// If the reader finds a navigation link to report, but it must first report the parent entry
        /// it will store the navigation link info in this property. So this will only ever store the first navigation link of an entry.
        /// </summary>
        ODataJsonLightReaderNavigationLinkInfo FirstNavigationLinkInfo { get; set; }

        /// <summary>
        /// The duplicate property names checker for the entry represented by the current state. May be null.
        /// </summary>
        DuplicatePropertyNamesChecker DuplicatePropertyNamesChecker { get; }

        /// <summary>
        /// The selected properties that should be expanded during template evaluation.
        /// </summary>
        SelectedPropertiesNode SelectedProperties { get; }

        /// <summary>
        /// The set of names of the navigation properties we have read so far while reading the entry.
        /// </summary>
        List<string> NavigationPropertiesRead { get; }

        /// <summary>
        /// true if we have started processing missing projected navigation links, false otherwise.
        /// </summary>
        bool ProcessingMissingProjectedNavigationLinks { get; set; }
    }
}
