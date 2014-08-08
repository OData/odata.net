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

namespace Microsoft.Data.OData.Evaluation
{
    using System.Collections.Generic;
    using Microsoft.Data.Edm;

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
        /// The selected always bindable operations.
        /// </summary>
        IEnumerable<IEdmFunctionImport> SelectedAlwaysBindableOperations { get; }
    }
}
