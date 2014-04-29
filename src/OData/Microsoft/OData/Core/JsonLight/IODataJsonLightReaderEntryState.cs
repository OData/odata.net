//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
