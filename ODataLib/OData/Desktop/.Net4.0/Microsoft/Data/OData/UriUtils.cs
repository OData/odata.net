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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.OData.JsonLight;

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
            Debug.Assert(fragmentString[0] == JsonLightConstants.MetadataUriFragmentIndicator, "fragmentString[0] == JsonLightConstants.MetadataUriFragmentIndicator");

            // Note if fragmentString contains characters that need escaping, the Uri constructor will escape it.
            // Uri.AbsoluteUri and Uri.Fragment will return the escaped characters and Uri.OriginalString
            // will contain the unescaped characters in the fragment.
            return (new Uri(ExampleMetadataAbsoluteUri, fragmentString)).Fragment;
        }
    }
}
