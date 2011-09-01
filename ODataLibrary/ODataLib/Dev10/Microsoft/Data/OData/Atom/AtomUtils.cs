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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    #endregion Namespaces

    /// <summary>
    /// Helper methods related to the ATOM Format
    /// </summary>
    internal static class AtomUtils
    {
        /// <summary>
        /// Creates the value for the navigation property's link relation attribute.
        /// </summary>
        /// <param name="navigationLink">The link representing the navigation property for which the relation value is created.</param>
        /// <returns>The relation attribute value for the navigation property's link relation.</returns>
        internal static string ComputeODataNavigationLinkRelation(ODataNavigationLink navigationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationLink.Name != null, "navigationLink.Name != null");

            return string.Join("/", new string[] { AtomConstants.ODataNamespace, AtomConstants.ODataNavigationPropertiesRelatedSegmentName, navigationLink.Name });
        }

        /// <summary>
        /// Creates the value for the navigation property's type attribute.
        /// </summary>
        /// <param name="navigationLink">The link representing the navigation property for which the type value is created.</param>
        /// <returns>The type attribute value for the navigation property.</returns>
        internal static string ComputeODataNavigationLinkType(ODataNavigationLink navigationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(navigationLink.IsCollection.HasValue, "navigationLink.IsCollection.HasValue");

            // "application/atom+xml;type=entry" or type="application/atom+xml;type=feed"
            return navigationLink.IsCollection.Value ? MimeConstants.MimeApplicationAtomXmlTypeFeed : MimeConstants.MimeApplicationAtomXmlTypeEntry;
        }

        /// <summary>
        /// Creates the value for the navigation property's association link relation attribute.
        /// </summary>
        /// <param name="associationLink">The link representing the navigation property's association for which the relation value is created.</param>
        /// <returns>The relation attribute value for the navigation property's association link relation.</returns>
        internal static string ComputeODataAssociationLinkRelation(ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(associationLink != null, "link != null");
            Debug.Assert(associationLink.Name != null, "link.Name != null");

            return string.Join("/", new string[] { AtomConstants.ODataNamespace, AtomConstants.ODataNavigationPropertiesAssociationRelatedSegmentName, associationLink.Name });
        }

        /// <summary>
        /// Creates the value for the named stream's link relation attribute.
        /// </summary>
        /// <param name="namedStreamProperty">The named stream property to create the relation for.</param>
        /// <param name="forEditLink">'true' if the relation is computed for an edit link; otherwise 'false'.</param>
        /// <returns>The relation attribute value for the named stream's link relation.</returns>
        internal static string ComputeNamedStreamRelation(ODataProperty namedStreamProperty, bool forEditLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(namedStreamProperty != null, "namedStreamProperty != null");
            Debug.Assert(!string.IsNullOrEmpty(namedStreamProperty.Name), "!string.IsNullOrEmpty(namedStreamProperty.Name)");

            string segmentName = forEditLink ? AtomConstants.ODataStreamPropertyEditMediaSegmentName : AtomConstants.ODataStreamPropertyMediaResourceSegmentName;
            return string.Join("/", new string[] { AtomConstants.ODataNamespace, segmentName, namedStreamProperty.Name });
        }

        /// <summary>
        /// Converts the given <paramref name="uri"/> Uri to a string. 
        /// If the provided baseUri is not null and is a base Uri of the <paramref name="uri"/> Uri 
        /// the method returns the string form of the relative Uri.
        /// </summary>
        /// <param name="uri">The Uri to convert.</param>
        /// <param name="baseUri">An optional base Uri</param>
        /// <param name="urlResolver">An optional custom URL resolver to resolve URLs for writing them into the payload.</param>
        /// <returns>The string form of the <paramref name="uri"/> Uri. If the Uri is absolute it returns the
        /// string form of the <paramref name="uri"/>. If the <paramref name="uri"/> Uri is not absolute 
        /// it returns the original string of the Uri.</returns>
        internal static string ToUrlAttributeValue(this Uri uri, Uri baseUri, IODataUrlResolver urlResolver)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(uri != null, "uri != null");

            if (urlResolver != null)
            {
                // The resolver returns 'null' if no custom resolution is desired.
                Uri resultUri = urlResolver.ResolveUrl(baseUri, uri);
                if (resultUri != null)
                {
                    return UriUtilsCommon.UriToString(resultUri);
                }
            }

            if (baseUri == null && !uri.IsAbsoluteUri)
            {
                throw new ODataException(
                    Strings.ODataWriter_RelativeUriUsedWithoutBaseUriSpecified(UriUtilsCommon.UriToString(uri)));
            }
            
            return UriUtilsCommon.UriToString(uri);
        }

        /// <summary>
        /// Parse the <paramref name="relation"/> attribute value for ATOM link element and return name following the specified <paramref name="namespacePrefix"/>.
        /// </summary>
        /// <param name="relation">ATOM link relation attribute value.</param>
        /// <param name="namespacePrefix">Value which the rel attribute should start with.</param>
        /// <returns>
        /// The name if the <paramref name="relation"/> starts with the given <paramref name="namespacePrefix"/>.
        /// If the <paramref name="relation"/> value does not start with the <paramref name="namespacePrefix"/> a null value is returned.
        /// </returns>
        internal static string GetNameFromAtomLinkRelationAttribute(string relation, string namespacePrefix)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(namespacePrefix != null, "namespacePrefix != null");

            if (!string.IsNullOrEmpty(relation))
            {
                Uri uri;
                if (Uri.TryCreate(relation, UriKind.RelativeOrAbsolute, out uri) && uri.IsAbsoluteUri)
                {
                    string unescaped = uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
                    if (unescaped.StartsWith(namespacePrefix, StringComparison.Ordinal))
                    {
                        return unescaped.Substring(namespacePrefix.Length);
                    }
                }
            }

            return null;
        }
    }
}
