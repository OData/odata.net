//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Evaluation
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Interface used for substitutability of the metadata-centric responsibilities of an entry.
    /// Metadata may come from a user-provided model or from the SetSerializationInfo() method on a feed or entry. The latter is considered the "no-model" case since only strings
    /// are provided, and there is no interconnectedness. The goal of this interface is to provide a way to query the metadata information available on an entry without
    /// needing to know where the metadata originated from.
    /// </summary>
    internal interface IODataEntryMetadataContext
    {
        /// <summary>
        /// The entry instance.
        /// </summary>
        ODataEntry Entry { get; }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry.
        /// </summary>
        IODataFeedAndEntryTypeContext TypeContext { get; }

        /// <summary>
        /// The actual entity type of the entry, i.e. ODataEntry.TypeName.
        /// </summary>
        string ActualEntityTypeName { get; }

        /// <summary>
        /// The key property name and value pairs of the entry.
        /// </summary>
        ICollection<KeyValuePair<string, object>> KeyProperties { get; }

        /// <summary>
        /// The ETag property name and value pairs of the entry.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> ETagProperties { get; }

        /// <summary>
        /// The selected navigation properties.
        /// </summary>
        IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties { get; }

        /// <summary>
        /// The selected stream properties.
        /// </summary>
        IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties { get; }

        /// <summary>
        /// The selected bindable operations.
        /// </summary>
        IEnumerable<IEdmOperation> SelectedBindableOperations { get; }
    }
}
