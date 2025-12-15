//---------------------------------------------------------------------
// <copyright file="ODataAnnotationNames.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using System;
    using System.Collections.Frozen;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    #endregion Namespaces

    /// <summary>
    /// Well known OData annotation names reserved for OData Lib.
    /// </summary>
    internal static class ODataAnnotationNames
    {
        /// <summary>
        /// Hash set of known odata annotation names that have special meanings to OData Lib.
        /// </summary>
        internal static readonly FrozenSet<string> KnownODataAnnotationNames =
            FrozenSet.ToFrozenSet(
                new[]
                {
                    ODataContext,
                    ODataType,
                    ODataId,
                    ODataETag,
                    ODataEditLink,
                    ODataReadLink,
                    ODataMediaEditLink,
                    ODataMediaReadLink,
                    ODataMediaContentType,
                    ODataMediaETag,
                    ODataCount,
                    ODataNextLink,
                    ODataBind,
                    ODataAssociationLinkUrl,
                    ODataNavigationLinkUrl,
                    ODataDeltaLink,
                    ODataRemoved,
                    ODataDelta,
                    ODataNull
                },
                StringComparer.Ordinal);

        /// <summary>The OData Context annotation name.</summary>
        internal const string ODataContext = "odata.context";

        /// <summary>The OData Type annotation name.</summary>
        internal const string ODataType = "odata.type";

        /// <summary>The OData ID annotation name.</summary>
        internal const string ODataId = "odata.id";

        /// <summary>The OData etag annotation name.</summary>
        internal const string ODataETag = "odata.etag";

        /// <summary>The OData edit link annotation name.</summary>
        internal const string ODataEditLink = "odata.editLink";

        /// <summary>The OData read link annotation name.</summary>
        internal const string ODataReadLink = "odata.readLink";

        /// <summary>The OData media edit link annotation name.</summary>
        internal const string ODataMediaEditLink = "odata.mediaEditLink";

        /// <summary>The OData media read link annotation name.</summary>
        internal const string ODataMediaReadLink = "odata.mediaReadLink";

        /// <summary>The OData media content type annotation name.</summary>
        internal const string ODataMediaContentType = "odata.mediaContentType";

        /// <summary>The OData media etag annotation name.</summary>
        internal const string ODataMediaETag = "odata.mediaEtag";

        /// <summary>The 'odata.count' annotation name.</summary>
        internal const string ODataCount = "odata.count";

        /// <summary>The 'odata.nextLink' annotation name.</summary>
        internal const string ODataNextLink = "odata.nextLink";

        /// <summary>The 'odata.navigationLink' annotation name.</summary>
        internal const string ODataNavigationLinkUrl = "odata.navigationLink";

        /// <summary>The 'odata.bind' annotation name.</summary>
        internal const string ODataBind = "odata.bind";

        /// <summary>The 'odata.associationLink' annotation name.</summary>
        internal const string ODataAssociationLinkUrl = "odata.associationLink";

        /// <summary>The 'odata.deltaLink' annotation name.</summary>
        internal const string ODataDeltaLink = "odata.deltaLink";

        /// <summary>The 'odata.removed' annotation name.</summary>
        internal const string ODataRemoved = "odata.removed";

        /// <summary>The 'odata.delta' annotation name.</summary>
        internal const string ODataDelta = "odata.delta";

        /// <summary>
        /// The OData Null annotation name. This is an OData 3.0 protocol element
        /// used for compatibility with 6.x library version.
        /// </summary>
        internal const string ODataNull = "odata.null";

        /// <summary>The OData Context annotation name with no prefix.</summary>
        internal const string ODataContextNoPrefix = "context";

        /// <summary>The OData Type annotation name with no prefix.</summary>
        internal const string ODataTypeNoPrefix = "type";

        /// <summary>The OData ID annotation name with no prefix.</summary>
        internal const string ODataIdNoPrefix = "id";

        /// <summary>The OData etag annotation name with no prefix.</summary>
        internal const string ODataETagNoPrefix = "etag";

        /// <summary>The OData edit link annotation name with no prefix.</summary>
        internal const string ODataEditLinkNoPrefix = "editLink";

        /// <summary>The OData read link annotation name with no prefix.</summary>
        internal const string ODataReadLinkNoPrefix = "readLink";

        /// <summary>The OData media edit link annotation name with no prefix.</summary>
        internal const string ODataMediaEditLinkNoPrefix = "mediaEditLink";

        /// <summary>The OData media read link annotation name with no prefix.</summary>
        internal const string ODataMediaReadLinkNoPrefix = "mediaReadLink";

        /// <summary>The OData media content type annotation name with no prefix.</summary>
        internal const string ODataMediaContentTypeNoPrefix = "mediaContentType";

        /// <summary>The OData media etag annotation name with no prefix.</summary>
        internal const string ODataMediaETagNoPrefix = "mediaEtag";

        /// <summary>The 'count' annotation name with no prefix.</summary>
        internal const string ODataCountNoPrefix = "count";

        /// <summary>The 'nextLink' annotation name with no prefix.</summary>
        internal const string ODataNextLinkNoPrefix = "nextLink";

        /// <summary>The 'navigationLink' annotation name with no prefix.</summary>
        internal const string ODataNavigationLinkUrlNoPrefix = "navigationLink";

        /// <summary>The 'bind' annotation name with no prefix.</summary>
        internal const string ODataBindNoPrefix = "bind";

        /// <summary>The 'associationLink' annotation name with no prefix.</summary>
        internal const string ODataAssociationLinkUrlNoPrefix = "associationLink";

        /// <summary>The 'deltaLink' annotation name with no prefix.</summary>
        internal const string ODataDeltaLinkNoPrefix = "deltaLink";

        /// <summary>The 'removed' annotation name with no prefix.</summary>
        internal const string ODataRemovedNoPrefix = "removed";

        /// <summary>The 'delta' annotation name with no prefix.</summary>
        internal const string ODataDeltaNoPrefix = "delta";

        /// <summary>
        /// The OData Null annotation name with no prefix. This is an OData 3.0 protocol element
        /// used for compatibility with 6.x library version.
        /// </summary>
        internal const string ODataNullNoPrefix = "null";

        /// <summary>
        /// Returns true if the <paramref name="annotationName"/> starts with "odata.", false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true if the <paramref name="annotationName"/> starts with "odata.", false otherwise.</returns>
        internal static bool IsODataAnnotationName(string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (annotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if the <paramref name="annotationName"/> starts with "odata." and is not one of the reserved odata annotation names; returns false otherwise.
        /// </summary>
        /// <param name="annotationName">The annotation name in question.</param>
        /// <returns>Returns true if the <paramref name="annotationName"/> starts with "odata." and is not one of the reserved odata annotation names; returns false otherwise.</returns>
        internal static bool IsUnknownODataAnnotationName(string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (IsODataAnnotationName(annotationName) && !KnownODataAnnotationNames.Contains(annotationName))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates that the <paramref name="annotationName"/> is not a reserved OData instance annotation.
        /// </summary>
        /// <param name="annotationName">The instance annotation name to check.</param>
        internal static void ValidateIsCustomAnnotationName(string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            // All other reserved OData instance annotations should fail.
            if (KnownODataAnnotationNames.Contains(annotationName))
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, annotationName));
            }

            Debug.Assert(!IsODataAnnotationName(annotationName), "Unknown names under the odata. namespace should be skipped by ODataJsonDeserializer.ParseProperty().");
        }

        /// <summary>
        /// Get the string without the instance annotation prefix @
        /// </summary>
        /// <param name="annotationName">the origin annotation name from reader</param>
        /// <returns>the annotation name without prefix @ </returns>
        internal static string RemoveAnnotationPrefix(string annotationName)
        {
            if (!String.IsNullOrEmpty(annotationName) && annotationName[0] == ODataJsonConstants.ODataPropertyAnnotationSeparatorChar)
            {
                return annotationName.Substring(1);
            }

            return annotationName;
        }
    }
}
