//---------------------------------------------------------------------
// <copyright file="XmlUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    #endregion Namespaces

    /// <summary>
    /// Helper methods to work with XML
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        /// Reusable name table for tests.
        /// </summary>
        private static XmlNameTable nameTable;

        /// <summary>
        /// Reusable namespace manager for tests.
        /// </summary>
        private static XmlNamespaceManager namespaceManager;

        /// <summary>
        /// Xml writer settings that do not force character checking and do *not* indent when writing.
        /// </summary>
        private static XmlWriterSettings nonCharacterCheckingNonIndentingWriterSettings;

        /// <summary>
        /// Xml writer settings that do not force character checking and indent when writing.
        /// </summary>
        private static XmlWriterSettings nonCharacterCheckingWriterSettings;

        /// <summary>
        /// Xml reader settings that do not force character checking and ignore whitespace.
        /// </summary>
        private static XmlReaderSettings nonCharacterCheckingIgnoreWhitespaceReaderSettings;

        /// <summary>
        /// Xml reader settings that do not force character checking and preserve whitespace.
        /// </summary>
        private static XmlReaderSettings nonCharacterCheckingPreserveWhitespaceReaderSettings;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static XmlUtils()
        {
            nonCharacterCheckingNonIndentingWriterSettings = new XmlWriterSettings();
            nonCharacterCheckingNonIndentingWriterSettings.CheckCharacters = false;
            nonCharacterCheckingNonIndentingWriterSettings.Indent = false;

            nonCharacterCheckingWriterSettings = new XmlWriterSettings();
            nonCharacterCheckingWriterSettings.CheckCharacters = false;
            nonCharacterCheckingWriterSettings.Indent = true;

            nonCharacterCheckingIgnoreWhitespaceReaderSettings = new XmlReaderSettings();
            nonCharacterCheckingIgnoreWhitespaceReaderSettings.CheckCharacters = false;
            // NOTE: must ignore whitespace when reading since otherwise our string-based comparison would not work
            //       when using the WriterPayloads approach of testing the same payload fragment in different places in the payload.
            nonCharacterCheckingIgnoreWhitespaceReaderSettings.IgnoreWhitespace = true;

            nonCharacterCheckingPreserveWhitespaceReaderSettings = new XmlReaderSettings();
            nonCharacterCheckingPreserveWhitespaceReaderSettings.CheckCharacters = false;
            nonCharacterCheckingPreserveWhitespaceReaderSettings.IgnoreWhitespace = false;
        }

        /// <summary>
        /// Reusable name table for tests.
        /// </summary>
        public static XmlNameTable NameTable
        {
            get
            {
                if (nameTable == null)
                {
                    nameTable = new NameTable();
                }

                return nameTable;
            }
        }

        /// <summary>
        /// Reusable namespace manager for tests.
        /// </summary>
        public static XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (namespaceManager == null)
                {
                    namespaceManager = new XmlNamespaceManager(NameTable);

                    // Some common namespaces used by legacy tests.
                    namespaceManager.AddNamespace("dw", "http://docs.oasis-open.org/odata/ns/data");

                    namespaceManager.AddNamespace("csdl1", "http://schemas.microsoft.com/ado/2006/04/edm");
                    namespaceManager.AddNamespace("csdl11", "http://schemas.microsoft.com/ado/2007/05/edm");
                    namespaceManager.AddNamespace("csdl12", "http://schemas.microsoft.com/ado/2008/01/edm");
                    namespaceManager.AddNamespace("csdl2", "http://schemas.microsoft.com/ado/2008/09/edm");
                    namespaceManager.AddNamespace("ads", "http://docs.oasis-open.org/odata/ns/data");
                    namespaceManager.AddNamespace("adsm", "http://docs.oasis-open.org/odata/ns/metadata");
                    namespaceManager.AddNamespace("csdl", "http://docs.oasis-open.org/odata/ns/edm");
                    namespaceManager.AddNamespace("ssdl", "http://schemas.microsoft.com/ado/2006/04/edm/ssdl");
                    namespaceManager.AddNamespace("msl", "urn:schemas-microsoft-com:windows:storage:mapping:CS");
                    namespaceManager.AddNamespace("app", "http://www.w3.org/2007/app");
                    namespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                    namespaceManager.AddNamespace("edmx", "http://docs.oasis-open.org/odata/ns/edmx");
                    namespaceManager.AddNamespace("cy", "http://www.currency.org");
                    namespaceManager.AddNamespace("geo", "http://www.georss.org/georss");
                    namespaceManager.AddNamespace("ad", "http://www.address.org");
                    namespaceManager.AddNamespace("tmpNs", "http://tempuri.org");
                    namespaceManager.AddNamespace("c", "http://www.customer.org");
                }

                return namespaceManager;
            }
        }

        /// <summary>
        /// Verifies that the specified XPath (or more) return at least one result.
        /// </summary>
        /// <param name="node">Node to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExists(AssertionHandler assert, XNode node, params string[] xpaths)
        {
            VerifyXPathExists(assert, node.CreateNavigator(NameTable), xpaths);
        }


        /// <summary>
        /// Verifies that the specified XPath (or more) return at least one result.
        /// </summary>
        /// <param name="navigable">Document to look in.</param>
        /// <param name="xpaths">The xpaths to verify.</param>
        public static void VerifyXPathExists(AssertionHandler assert, IXPathNavigable navigable, params string[] xpaths)
        {
            foreach (string xpath in xpaths)
            {
                int count = navigable.CreateNavigator().Select(xpath, NamespaceManager).Count;
                if (count == 0)
                {
                    Trace.WriteLine(navigable.CreateNavigator().OuterXml);
                    assert.Fail("Failed to find specified xpath in the document: " + xpath);
                }
            }
        }

        /// <summary>
        /// Compare two <see cref="XElement"/> instances for equality using XNode.DeepEquals. 
        /// Optionally normalize namespace declarations prior to comparison.
        /// </summary>
        /// <param name="expected">The expected <see cref="XElement"/> instance.</param>
        /// <param name="actual">The observed <see cref="XElement"/> instance.</param>
        /// <param name="normalize">Normalize the XElement by removing all explicit namespace declarations.</param>
        /// <param name="error">The error message if the <paramref name="expected"/> and <paramref name="actual"/> elements do not compare equal; otherwise null.</param>
        /// <returns>A flag indicating whether the two elements are considered equal.</returns>
        public static bool CompareXml(XElement expected, XElement actual, bool normalize, out string error)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
            ExceptionUtilities.CheckArgumentNotNull(actual, "actual");

            error = null;

            if (normalize)
            {
                expected.DescendantNodesAndSelf().OfType<XElement>().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
                actual.DescendantNodesAndSelf().OfType<XElement>().Attributes().Where(a => a.IsNamespaceDeclaration).Remove();
            }

            if (!XNode.DeepEquals(expected, actual))
            {
                error = string.Format(
                    "Different XML.{0}Expected:{0}-->{1}<--{0}Actual:{0}-->{2}<--{0}",
                    Environment.NewLine,
                    expected.ToString(),
                    actual.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Expands an Xml template by resolving all variables, parsing it into an XElement and then getting its string representation.
        /// </summary>
        /// <param name="xmlTemplate">The Xml template to expand.</param>
        /// <param name="variables">The set of variables to expand.</param>
        /// <param name="indent">Flag to indicate whether to use indentation during writing.</param>
        /// <returns>The string representation of the <paramref name="xmlTemplate"/> with resolved variables.</returns>
        public static string GetComparableXmlString(string xmlTemplate, Dictionary<string, string> variables, bool indent)
        {
            XElement expectedXml = xmlTemplate.ToXElement(variables);
            return expectedXml.ToComparableString(indent);
        }

        /// <summary>
        /// Converts an <see cref="XElement"/> into a string usable for comparisons.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> to convert.</param>
        /// <param name="indent">Flag to indicate whether to use indentation during writing.</param>
        /// <returns>The string representaiton of the <paramref name="element"/>.</returns>
        public static string ToComparableString(this XElement element, bool indent)
        {
            XmlWriterSettings settings = indent ? nonCharacterCheckingWriterSettings : nonCharacterCheckingNonIndentingWriterSettings;

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                element.WriteTo(writer);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Checks if an <see cref="XElement"/> has an attribute with a given name.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> to check.</param>
        /// <param name="propertyName">The name of the property to check for.</param>
        /// <returns>
        /// true if the <paramref name="element"/> has a property with name 
        /// <paramref name="propertyName"/>.
        /// Otherwise false.
        /// </returns>
        public static bool HasAttribute(this XElement element, string propertyName)
        {
            XAttribute attribute = element.Attributes(propertyName).FirstOrDefault();
            return attribute != null;
        }

        /// <summary>
        /// Checks if an <see cref="XElement"/> has an attribute with a given name and value.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> to check.</param>
        /// <param name="propertyName">The name of the property to check for.</param>
        /// <param name="propertyValue">The value of the property to check for.</param>
        /// <returns>
        /// true if the <paramref name="element"/> has a property with name 
        /// <paramref name="propertyName"/> and the value <paramref name="propertyValue"/>.
        /// Otherwise false.
        /// </returns>
        public static bool HasAttribute(this XElement element, string propertyName, string propertyValue)
        {
            XAttribute attribute = element.Attributes(propertyName).FirstOrDefault();

            if (attribute == null)
            {
                return false;
            }

            return string.CompareOrdinal(propertyValue, attribute.Value) == 0;
        }

        /// <summary>
        /// Parse a string into an <see cref="XElement"/>.
        /// </summary>
        /// <param name="xmlString">The string to parse.</param>
        /// <param name="preserveWhitespace">True if insignificant whitespace should be preserved during parsing.</param>
        /// <returns>The <see cref="XElement"/> created from the <paramref name="xmlString"/>.</returns>
        public static XElement ToXElement(this string xmlString, bool preserveWhitespace = false)
        {
            XElement element;
            XmlReaderSettings xmlReaderSettings =
                preserveWhitespace
                    ? nonCharacterCheckingPreserveWhitespaceReaderSettings
                    : nonCharacterCheckingIgnoreWhitespaceReaderSettings;

            using (StringReader stringReader = new StringReader(xmlString))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader, xmlReaderSettings))
                {
                    element = XElement.Load(xmlReader, LoadOptions.PreserveWhitespace);
                }
            }

            return element;
        }

        /// <summary>
        /// Parse a string into an <see cref="XElement"/>.
        /// </summary>
        /// <param name="xmlString">The string to parse.</param>
        /// <param name="variables">The variables to be resolved in the <paramref name="xmlString"/> prior to parsing.</param>
        /// <param name="preserveWhitespace">True if insignificant whitespace should be preserved during parsing.</param>
        /// <returns>The <see cref="XElement"/> created from the <paramref name="xmlString"/>.</returns>
        public static XElement ToXElement(this string xmlString, Dictionary<string, string> variables, bool preserveWhitespace = false)
        {
            xmlString = StringUtils.ResolveVariables(xmlString, variables);
            return xmlString.ToXElement(preserveWhitespace);
        }

        /// <summary>
        /// Convert the contents of a memory stream to an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="preserveWhitespace">True if insignificant whitespace should be preserved during parsing.</param>
        /// <returns>The <see cref="XElement"/> created from the <paramref name="stream"/>.</returns>
        public static XElement ToXElement(this Stream stream, bool preserveWhitespace = false)
        {
            XElement element;

            XmlReaderSettings xmlReaderSettings =
                preserveWhitespace
                    ? nonCharacterCheckingPreserveWhitespaceReaderSettings
                    : nonCharacterCheckingIgnoreWhitespaceReaderSettings;

            using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
            {
                element = XElement.Load(xmlReader, LoadOptions.PreserveWhitespace);
            }

            return element;
        }
    }
}
