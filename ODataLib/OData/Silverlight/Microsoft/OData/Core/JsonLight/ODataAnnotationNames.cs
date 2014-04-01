//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;
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
                    ODataContext,
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
                    ODataDeltaLink
                },
                StringComparer.Ordinal);

        /// <summary>The OData Context annotation name.</summary>
        internal const string ODataContext = "odata.context";

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

        /// <summary>
        /// Returns true if the <paramref name="annotationName"/> starts with "odata.", false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true if the <paramref name="annotationName"/> starts with "odata.", false otherwise.</returns>
        internal static bool IsODataAnnotationName(string annotationName)
        {
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
                throw new ODataException(ODataErrorStrings.ODataJsonLightPropertyAndValueDeserializer_UnexpectedAnnotationProperties(annotationName));
            }

            Debug.Assert(!IsODataAnnotationName(annotationName), "Unknown names under the odata. namespace should be skipped by ODataJsonLightDeserializer.ParseProperty().");
        }

        /// <summary>
        /// Get the string without the instance annotation prefix @
        /// </summary>
        /// <param name="annotationName">the origin annotation name from reader</param>
        /// <returns>the annotation name without prefix @ </returns>
        internal static string RemoveAnnotationPrefix(string annotationName)
        {
            if (!String.IsNullOrEmpty(annotationName) && annotationName[0] == JsonLightConstants.ODataPropertyAnnotationSeparatorChar)
            {
                return annotationName.Substring(1);
            }

            return annotationName;
        }
    }
}
