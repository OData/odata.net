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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Well known OData annotation names reserved for OData Lib.
    /// </summary>
    internal static class ODataAnnotationNames
    {
        /// <summary>
        /// Hash set of known odata annotation names that have special meanings to OData Lib.
        /// </summary>
        internal static readonly HashSet<string> KnownODataAnnotationNames =
            new HashSet<string>(
                new[]
                {
                    ODataMetadata,
                    ODataNull,
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
                    ODataAnnotationGroup, 
                    ODataAnnotationGroupReference, 
                    ODataError,
                    ODataDeltaLink
                },
                StringComparer.Ordinal);

        /// <summary>The OData Metadata annotation name.</summary>
        internal const string ODataMetadata = "odata.metadata";

        /// <summary>The OData 'null' annotation name.</summary>
        internal const string ODataNull = "odata.null";

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
        internal const string ODataMediaETag = "odata.mediaETag";

        /// <summary>The 'odata.count' annotation name.</summary>
        internal const string ODataCount = "odata.count";

        /// <summary>The 'odata.nextLink' annotation name.</summary>
        internal const string ODataNextLink = "odata.nextLink";

        /// <summary>The 'odata.navigationLinkUrl' annotation name.</summary>
        internal const string ODataNavigationLinkUrl = "odata.navigationLinkUrl";

        /// <summary>The 'odata.bind' annotation name.</summary>
        internal const string ODataBind = "odata.bind";

        /// <summary>The 'odata.associationLinkUrl' annotation name.</summary>
        internal const string ODataAssociationLinkUrl = "odata.associationLinkUrl";

        /// <summary>The 'odata.annotationGroup' annotation name.</summary>
        internal const string ODataAnnotationGroup = "odata.annotationGroup";

        /// <summary>The 'odata.annotationGroupReference' annotation name.</summary>
        internal const string ODataAnnotationGroupReference = "odata.annotationGroupReference";

        /// <summary>The 'odata.error' annotation name.</summary>
        internal const string ODataError = "odata.error";

        /// <summary>The 'odata.deltaLink' annotation name.</summary>
        internal const string ODataDeltaLink = "odata.deltaLink";

        /// <summary>
        /// Returns true if the <paramref name="annotationName"/> starts with "odata.", false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true if the <paramref name="annotationName"/> starts with "odata.", false otherwise.</returns>
        internal static bool IsODataAnnotationName(string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (annotationName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal))
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            // All other reserved OData instance annotations should fail.
            if (KnownODataAnnotationNames.Contains(annotationName))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(annotationName));
            }

            Debug.Assert(!IsODataAnnotationName(annotationName), "Unknown names under the odata. namespace should be skipped by ODataJsonLightDeserializer.ParseProperty().");
        }
    }
}
