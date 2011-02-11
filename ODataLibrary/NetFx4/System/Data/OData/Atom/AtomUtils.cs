//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Diagnostics;

    #endregion Namespaces.

    /// <summary>
    /// Helper methods related to the ATOM Format
    /// </summary>
    internal static class AtomUtils
    {
        /// <summary>
        /// Creates the value for the navigation property's link relation attribute.
        /// </summary>
        /// <param name="link">The link representing the navigation property for which the relation value is created.</param>
        /// <returns>The relation attribute value for the navigation property's link relation.</returns>
        internal static string ComputeODataLinkRelation(ODataLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link.Name != null, "link.Name != null");

            return string.Join("/", new string[] { AtomConstants.ODataNamespace, AtomConstants.ODataNavigationPropertiesRelatedSegmentName, link.Name });
        }

        /// <summary>
        /// Creates the value for the navigation property's type attribute.
        /// </summary>
        /// <param name="link">The link representing the navigation property for which the type value is created.</param>
        /// <returns>The type attribute value for the navigation property.</returns>
        internal static string ComputeODataLinkType(ODataLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(link != null, "link != null");

            // "application/atom+xml;type=entry" or type="application/atom+xml;type=feed"
            return link.IsCollection ? MimeConstants.MimeApplicationAtomXmlTypeFeed : MimeConstants.MimeApplicationAtomXmlTypeEntry;
        }

        /// <summary>
        /// Creates the value for the navigation property's association link relation attribute.
        /// </summary>
        /// <param name="link">The link representing the navigation property's association for which the relation value is created.</param>
        /// <returns>The relation attribute value for the navigation property's association link relation.</returns>
        internal static string ComputeODataAssociationLinkRelation(ODataAssociationLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(link != null, "link != null");
            Debug.Assert(link.Name != null, "link.Name != null");

            return string.Join("/", new string[] { AtomConstants.ODataNamespace, AtomConstants.ODataNavigationPropertiesAssociationRelatedSegmentName, link.Name });
        }

        /// <summary>
        /// Creates the value for the named stream's link relation attribute.
        /// </summary>
        /// <param name="namedStream">The named stream to create the relation for.</param>
        /// <param name="forEditLink">'true' if the relation is computed for an edit link; otherwise 'false'.</param>
        /// <returns>The relation attribute value for the named stream's link relation.</returns>
        internal static string ComputeNamedStreamRelation(ODataMediaResource namedStream, bool forEditLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(namedStream != null, "namedStream != null");
            Debug.Assert(!string.IsNullOrEmpty(namedStream.Name), "!string.IsNullOrEmpty(namedStream.Name)");

            string segmentName = forEditLink ? AtomConstants.ODataNamedStreamsEditMediaSegmentName : AtomConstants.ODataNamedStreamsMediaResourceSegmentName;
            return string.Join("/", new string[] { AtomConstants.ODataNamespace, segmentName, namedStream.Name });
        }

        /// <summary>
        /// Converts the given <paramref name="uri"/> Uri to a string. 
        /// If the provided baseUri is not null and is a base Uri of the <paramref name="uri"/> Uri 
        /// the method returns the string form of the relative Uri.
        /// </summary>
        /// <param name="uri">The Uri to convert.</param>
        /// <param name="baseUri">An optional base Uri</param>
        /// <returns>The string form of the <paramref name="uri"/> Uri. If the Uri is absolute and 
        /// the <paramref name="baseUri"/> is the base Uri of the <paramref name="uri"/> Uri it returns a relative Uri; 
        /// otherwise the string form of the absolute Uri. If the <paramref name="uri"/> Uri is not absolute 
        /// it returns the original string of the Uri.</returns>
        internal static string ToUrlAttributeValue(this Uri uri, Uri baseUri)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            if (baseUri == null)
            {
                if (!uri.IsAbsoluteUri)
                {
                    throw new ODataException(Strings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtils.UriToString(uri)));
                }
            }
            else
            {
                if (uri.IsAbsoluteUri && UriUtils.UriInvariantInsensitiveIsBaseOf(baseUri, uri))
                {
                    uri = baseUri.MakeRelativeUri(uri);
                }
            }

            return UriUtils.UriToString(uri);
        }
    }
}
