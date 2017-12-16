//---------------------------------------------------------------------
// <copyright file="IODataResourceMetadataContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Evaluation
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Interface used for substitutability of the metadata-centric responsibilities of a resource.
    /// Metadata may come from a user-provided model or from the SetSerializationInfo() method on a resource set or resource. The latter is considered the "no-model" case since only strings
    /// are provided, and there is no interconnectedness. The goal of this interface is to provide a way to query the metadata information available on a resource without
    /// needing to know where the metadata originated from.
    /// </summary>
    internal interface IODataResourceMetadataContext
    {
        /// <summary>
        /// The resource instance.
        /// </summary>
        ODataResourceBase Resource { get; }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the resource.
        /// </summary>
        IODataResourceTypeContext TypeContext { get; }

        /// <summary>
        /// The actual structured type of the resource, i.e. ODataResource.TypeName.
        /// </summary>
        string ActualResourceTypeName { get; }

        /// <summary>
        /// The key property name and value pairs of the resource.
        /// </summary>
        ICollection<KeyValuePair<string, object>> KeyProperties { get; }

        /// <summary>
        /// The ETag property name and value pairs of the resource.
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