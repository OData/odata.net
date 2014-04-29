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
            Debug.Assert(baseUri != null, "baseUri != null");
            Debug.Assert(baseUri.IsAbsoluteUri, "baseUri is not absolute.");
            Debug.Assert(relativeUri != null, "relativeUri != null");
            Debug.Assert(!relativeUri.IsAbsoluteUri, "relativeUri is not relative.");

            return new Uri(baseUri, relativeUri);
        }

        /// <summary>
        /// Create a Uri as entry or feed id
        /// </summary>
        /// <param name="value">Uri value</param>
        /// <param name="kind">UriKind</param>
        /// <param name="swallowEmpty">if swallowEmpty is true, an empty value will be parsed as null Uri, otherwise invalid Uri format exception will throw. </param>
        /// <returns>Created Uri from value</returns>
        internal static Uri CreateUriAsEntryOrFeedId(string value, UriKind kind, bool swallowEmpty = true)
        {
            if (swallowEmpty && value == string.Empty || value == null)
            {
                return null;
            }

            Uri uri;
            try
            {
                uri = new Uri(value, kind);
            }
            catch (FormatException)
            {
                throw new ODataException(Strings.ODataUriUtils_InvalidUriFormatForEntryIdOrFeedId(value));
            }

            return uri;
        }

        /// <summary>
        /// A method to ensure that the original string of a relative URI is escaped.
        /// </summary>
        /// <param name="uri">The relative <see cref="System.Uri"/> to escape.</param>
        /// <returns>A relative URI instance with guaranteed escaped original string.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0010", Justification = "Usage of OriginalString is safe in this context")]
        internal static Uri EnsureEscapedRelativeUri(Uri uri)
        {
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
            Debug.Assert(fragmentString[0] == ODataConstants.ContextUriFragmentIndicator, "fragmentString[0] == " + ODataConstants.ContextUriFragmentIndicator);

            // Note if fragmentString contains characters that need escaping, the Uri constructor will escape it.
            // Uri.AbsoluteUri and Uri.Fragment will return the escaped characters and Uri.OriginalString
            // will contain the unescaped characters in the fragment.
            return (new Uri(ExampleMetadataAbsoluteUri, fragmentString)).Fragment;
        }

        /// <summary>
        /// Returns the unescaped string representation of the Uri; if the Uri is absolute returns the absolute Uri otherwise the original string.
        /// </summary>
        /// <param name="uri">The Uri to convert to a string.</param>
        /// <returns>For absolute Uris the string representation of the absolute Uri; otherwise the Uri's original string.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0010", Justification = "Usage of OriginalString is safe in this context")]
        internal static string UriToString(Uri uri)
        {
            Debug.Assert(uri != null, "uri != null");

            return uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;
        }

        /// <summary>
        /// Ensure the last character of Uri is a "/".
        /// </summary>
        /// <param name="uri">The Uri to deal with.</param>
        /// <returns>The uri with tailling "/".</returns>
        internal static Uri EnsureTaillingSlash(Uri uri)
        {
            if (uri == null)
            {
                return null;
            }

            string baseUriString = UriToString(uri);

            if (baseUriString[baseUriString.Length - 1] != ODataConstants.UriSegmentSeparatorChar)
            {
                return new Uri(baseUriString + ODataConstants.UriSegmentSeparator, UriKind.RelativeOrAbsolute);
            }

            return uri;
        }
    }
}
