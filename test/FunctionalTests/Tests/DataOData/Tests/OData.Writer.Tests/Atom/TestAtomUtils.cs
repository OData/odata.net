//---------------------------------------------------------------------
// <copyright file="TestAtomUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.OData.Atom;
    #endregion Namespaces

    /// <summary>
    /// Utility methods related to the ATOM format.
    /// </summary>
    internal static class TestAtomUtils
    {
        /// <summary>
        /// Returns the &lt;m:properties&gt; element given an &lt;atom:entry&gt; element.
        /// </summary>
        /// <param name="entry">The entry element to get the properties from.</param>
        /// <returns>The properties element for the given entry.</returns>
        internal static XElement ExtractPropertiesFromEntry(XElement entry)
        {
            XElement propertiesElement = entry
                .Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomContentElementName).Single()
                .Elements(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomPropertiesElementName).Single();
            // Detach the element from its parent (needed to not pick up namespace declaration from the parent later on)
            propertiesElement.Remove();
            return propertiesElement;
        }

        /// <summary>
        /// Returns an element containing the read link and edit link for the named stream in question.
        /// </summary>
        /// <param name="entry">The entry element to get the named stream links from.</param>
        /// <param name="streamName">Name of the named stream.</param>
        /// <returns>An element containing the read link and edit link for the given named stream.</returns>
        internal static XElement ExtractNamedStreamLinksFromEntry(XElement entry, string streamName)
        {
            var readLink = entry.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                .SingleOrDefault(l => l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName).Value == TestAtomConstants.ODataStreamPropertyMediaResourceRelatedLinkRelationPrefix + streamName);
            var editLink = entry.Elements(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomLinkElementName)
                .SingleOrDefault(l => l.Attribute(TestAtomConstants.AtomLinkRelationAttributeName).Value == TestAtomConstants.ODataStreamPropertyEditMediaRelatedLinkRelationPrefix + streamName);

            Debug.Assert(readLink != null || editLink != null, string.Format("The NamedStream '{0}' is not found!", streamName));
            var result = new XElement("NamedStream");
            if (readLink != null)
            {
                result.Add(readLink);
            }

            if (editLink != null)
            {
                result.Add(editLink);
            }

            return result;
        }

        /// <summary>
        /// Converts the DateTimeOffset to a string.
        /// </summary>
        /// <param name="dateTime">The date time offset value to convert.</param>
        /// <returns>The string value.</returns>
        public static string ToAtomString(this DateTimeOffset dateTime)
        {
            if (dateTime.Offset == new TimeSpan(0, 0, 0))
            {
                return dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }

            return dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the XML in string form of the attributes for a navigation link in ATOM.
        /// </summary>
        /// <param name="propertyName">The name of the navigation property for the link.</param>
        /// <param name="contentType">The content type of the link.</param>
        /// <param name="title">The title of the link.</param>
        /// <param name="url">The URL of the navigation link.</param>
        /// <returns>The XML representation attributes of the link as string.</returns>
        public static string GetExpectedAtomNavigationLinkAttributesAsString(string propertyName, string contentType, string title, string url)
        {
            return
                "rel=\"" + TestAtomConstants.ODataNavigationPropertiesRelatedLinkRelationPrefix + propertyName + "\" " +
                "type=\"" + contentType + "\" " +
                "title=\"" + propertyName + "\" " +
                "href=\"" + url + "\" " +
                "xmlns=\"" + TestAtomConstants.AtomNamespace + "\"";
        }
    }
}