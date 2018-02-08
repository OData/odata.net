//---------------------------------------------------------------------
// <copyright file="IODataJsonLightReaderResourceState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Evaluation;
    #endregion Namespaces

    /// <summary>
    /// Interface representing a state of the JSON reader for resource.
    /// </summary>
    internal interface IODataJsonLightReaderResourceState
    {
        /// <summary>
        /// The resource being read.
        /// </summary>
        ODataResourceBase Resource { get; }

        /// <summary>
        /// The structured type for the resource (if available)
        /// </summary>
        IEdmStructuredType ResourceType { get; }

        /// <summary>
        /// The expected type defined in the model for the resource (if available)
        /// </summary>
        IEdmStructuredType ResourceTypeFromMetadata { get; set; }

        /// <summary>
        /// The navigation source for the resource (if available)
        /// </summary>
        IEdmNavigationSource NavigationSource { get; }

        /// <summary>
        /// The metadata builder instance for the resource.
        /// </summary>
        ODataResourceMetadataBuilder MetadataBuilder { get; set; }

        /// <summary>
        /// Flag which indicates that during parsing of the resource represented by this state,
        /// any property which is not an instance annotation was found. This includes property annotations
        /// for property which is not present in the payload.
        /// </summary>
        /// <remarks>
        /// This is used to detect incorrect ordering of the payload (for example odata.id must not come after the first property).
        /// </remarks>
        bool AnyPropertyFound { get; set; }

        /// <summary>
        /// If the reader finds a nested resource info to report, but it must first report the parent resource
        /// it will store the nested resource info in this property. So this will only ever store the first nested resource info of a resource.
        /// </summary>
        ODataJsonLightReaderNestedResourceInfo FirstNestedResourceInfo { get; set; }

        /// <summary>
        /// The duplicate property names checker for the resource represented by the current state. May be null.
        /// </summary>
        PropertyAndAnnotationCollector PropertyAndAnnotationCollector { get; }

        /// <summary>
        /// The selected properties that should be expanded during template evaluation.
        /// </summary>
        SelectedPropertiesNode SelectedProperties { get; }

        /// <summary>
        /// The set of names of the navigation properties we have read so far while reading the resource.
        /// </summary>
        List<string> NavigationPropertiesRead { get; }

        /// <summary>
        /// true if we have started processing missing projected navigation links, false otherwise.
        /// </summary>
        bool ProcessingMissingProjectedNestedResourceInfos { get; set; }
    }
}
