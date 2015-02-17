//---------------------------------------------------------------------
// <copyright file="IODataEntryMetadataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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