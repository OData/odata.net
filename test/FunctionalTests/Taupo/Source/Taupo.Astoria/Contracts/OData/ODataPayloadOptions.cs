//---------------------------------------------------------------------
// <copyright file="ODataPayloadOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;

    /// <summary>
    /// Enum for representing different options that can affect how OData payloads appear
    /// </summary>
    [Flags]
    public enum ODataPayloadOptions
    {
        /// <summary>
        /// Indicates that no options are enabled
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that entity payloads should include identifiers.
        /// </summary>
        IncludeEntityIdentifier = 1 << 0,

        /// <summary>
        /// Indicates that type names should be present in general. The more specific type-name omission options supercede this.
        /// </summary>
        IncludeTypeNames = 1 << 1,

        /// <summary>
        /// Indicates that stream edit links should be present for media-resources. This is specifically for non-named media-resources.
        /// </summary>
        IncludeMediaResourceEditLinks = 1 << 2,

        /// <summary>
        /// Indicates that stream source links should be present for media-resources. This is specifically for non-named media-resources.
        /// </summary>
        IncludeMediaResourceSourceLinks = 1 << 3,

        /// <summary>
        /// Indicates that stream edit links should be present for named media-resources
        /// </summary>
        IncludeNamedMediaResourceEditLinks = 1 << 4,

        /// <summary>
        /// Indicates that stream source links should be present for named media-resources
        /// </summary>
        IncludeNamedMediaResourceSourceLinks = 1 << 5,

        /// <summary>
        /// Indicates that identifiers will be based on convention
        /// </summary>
        UseConventionBasedIdentifiers = 1 << 6,

        /// <summary>
        /// Indicates that links (edit, navigation, relationship, etc) will be based on convention
        /// </summary>
        UseConventionBasedLinks = 1 << 7,

        /// <summary>
        /// Indicates that type names will be omiitted from the individual elements of a multivalue property
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Matches product API, but is explicitly blocked by FxCop")]
        OmitTypeNamesWithinMultiValues = 1 << 8,

        /// <summary>
        /// Indicates that type names will be omitted for primitive string values
        /// </summary>
        OmitTypeNamesForStrings = 1 << 9,

        /// <summary>
        /// Indicates that type names will be omitted for all primitive values
        /// </summary>
        OmitTypeNamesForAllPrimitives = 1 << 10,

        /// <summary>
        /// Indicates that spatial primitives will always have type information
        /// </summary>
        AlwaysIncludeTypeNamesForSpatialPrimitives = 1 << 11,

        /// <summary>
        /// Indicates that null values will still have type information
        /// </summary>
        IncludeTypeNamesForNullValues = 1 << 12,

        /// <summary>
        /// Indicates that payloads should include ETags (JSON-Light does not include ETags in payload, while ATOM and JSON-Verbose do)
        /// </summary>
        IncludeETags = 1 << 13,

        /// <summary>
        /// Indicates whether payloads should include at least one of Self and Edit links (JSON-Light allows for bot fo them to be missing, but ATOM and JSON Verbose don't)
        /// </summary>
        IncludeSelfOrEditLink = 1 << 14,

        /// <summary>
        /// Indicates whether payloads will have a self link that may be wrong but is constructed
        /// </summary>
        ConventionallyProducedNamedStreamSelfLink = 1 << 15,

        /// <summary>
        /// Indicates that DateTime values in the payload are in the old 'Date(...)' format rather than the ISO one. 
        /// </summary>
        UseOldDateTimeFormat = 1 << 16,
    }
}
