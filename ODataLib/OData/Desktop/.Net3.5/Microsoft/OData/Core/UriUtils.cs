//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData.Core.JsonLight;

    #endregion Namespaces

    /// <summary>
    /// Uri utility methods.
    /// </summary>
    internal static class UriUtils
    {
        /// <summary>
        /// An absolute Uri to use as the base Uri for escaping a Uri fragment.
        /// </summary>
        private static Uri ExampleMetadataAbsoluteUri = new Uri("http://www.example.com/$metadata", UriKind.Absolute);

        /// <summary>
        /// Returns an absolute URI constructed from the specified base URI and a relative URI
        /// </summary>
        /// <param name="baseUri">The base URI to use.</param>
        /// <param name="relativeUri">The relative URI to use.</param>
        /// <returns>The absolute URI as a result of combining the base URI with the relative URI.</returns>
        internal static Uri UriToAbsoluteUri(Uri baseUri, Uri relativeUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(baseUri.IsAbsoluteUri, "baseUri is not absolute.");
            Debug.Assert(relativeUri != null, "relativeUri != null");
            Debug.Assert(!relativeUri.IsAbsoluteUri, "relativeUri is not relative.");

            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// A method to ensure that the original string of a relative URI is escaped.
        /// </summary>
        /// <param name="uri">The relative <see cref="System.Uri"/> to escape.</param>
        /// <returns>A relative URI instance with guaranteed escaped original string.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0010", Justification = "Usage of OriginalString is safe in this context")]
        internal static Uri EnsureEscapedRelativeUri(Uri uri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null && !uri.IsAbsoluteUri, "uri != null && !uri.IsAbsoluteUri");

            string escapedRelativeUri = uri.GetComponents(UriComponents.SerializationInfoString, UriFormat.UriEscaped);
            if (string.CompareOrdinal(uri.OriginalString, escapedRelativeUri) == 0)
            {
                return uri;
            }

            return new Uri(escapedRelativeUri, UriKind.Relative);
        }

        /// <summary>
        /// Gets the escaped metadata reference property name.
        /// </summary>
        /// <param name="fragmentString">The metadata reference property name in question.</param>
        /// <returns>The Uri escaped metadata reference property name.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0018:SystemUriEscapeDataStringRule", Justification = "Method explicitly escapes the fragment string.")]
        internal static string EnsureEscapedFragment(string fragmentString)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(fragmentString[0] == JsonLightConstants.ContextUriFragmentIndicator, "fragmentString[0] == JsonLightConstants.ContextUriFragmentIndicator");

            // Note if fragmentString contains characters that need escaping, the Uri constructor will escape it.
            // Uri.AbsoluteUri and Uri.Fragment will return the escaped characters and Uri.OriginalString
            // will contain the unescaped characters in the fragment.
            return (new Uri(ExampleMetadataAbsoluteUri, fragmentString)).Fragment;
        }
    }
}
