//---------------------------------------------------------------------
// <copyright file="XmlODataPayloadElementExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Contracts.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Atom;
    #endregion Namespaces

    /// <summary>
    /// Xml specific extension methods for ODataPayloadElement.
    /// </summary>
    public static class XmlODataPayloadElementExtensions
    {
        /// <summary>
        /// String which holds xml namespace declaration attributes for the standard Atom and Data Services namespaces.
        /// </summary>
        private const string StandardAtomNamespaceDeclarations =
            "xmlns:" + ODataConstants.DataServicesNamespaceDefaultPrefix + "=\"" + ODataConstants.DataServicesNamespaceName + "\" " +
            "xmlns:" + ODataConstants.DataServicesMetadataNamespaceDefaultPrefix + "=\"" + ODataConstants.DataServicesMetadataNamespaceName + "\" " +
            "xmlns=\"" + ODataConstants.AtomNamespaceName + "\"";

        /// <summary>
        /// Annotates payload element representing a value with the replacement for the value.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to annotate.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="propertyValueXml">The XML to use as the property value.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the annotation.</returns>
        public static T XmlValueRepresentation<T>(this T payloadElement, IEnumerable<XNode> propertyValueXml)
            where T : ODataPayloadElement, ITypedValue
        {
            return payloadElement.WithXmlRewriteFunction(valueElement =>
            {
                valueElement.RemoveNodes();
                valueElement.Add(propertyValueXml);
                return valueElement;
            });
        }

        /// <summary>
        /// Annotates payload element representing a value with the replacement for the value.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to annotate.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="propertyValueXml">The XML to use as the property value.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the annotation.</returns>
        public static T XmlValueRepresentation<T>(this T payloadElement, string propertyValueXml)
            where T : ODataPayloadElement, ITypedValue
        {
            return payloadElement.XmlValueRepresentation(ParseXmlContent(propertyValueXml));
        }

        /// <summary>
        /// Annotates payload element representing a value with the replacement for the value.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to annotate.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="propertyValueXml">The XML to use as the property value.</param>
        /// <param name="propertyAttributes">The attributes to put on the property.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the annotation.</returns>
        public static T XmlValueRepresentation<T>(this T payloadElement, IEnumerable<XNode> propertyValueXml, IEnumerable<XAttribute> propertyAttributes)
            where T : ODataPayloadElement, ITypedValue
        {
            return payloadElement.WithXmlRewriteFunction(valueElement =>
            {
                valueElement.RemoveAll();
                valueElement.Add(propertyValueXml);
                valueElement.Add(propertyAttributes);
                return valueElement;
            });
        }

        /// <summary>
        /// Annotates payload element representing a value with the replacement for the value and its attributes.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to annotate.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="propertyValueXml">The XML to use as the property value.</param>
        /// <param name="attributes">The XML attributes to put on the property value.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the annotation.</returns>
        public static T XmlValueRepresentation<T>(this T payloadElement, string propertyValueXml, string attributes)
            where T : ODataPayloadElement, ITypedValue
        {
            return payloadElement.XmlValueRepresentation(ParseXmlContent(propertyValueXml), ParseXmlAttributes(attributes));
        }

        /// <summary>
        /// Annotates payload element representing a value with the replacement for the value, and the typename and null attributes.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to annotate.</typeparam>
        /// <param name="payloadElement">The payload element to annotate.</param>
        /// <param name="propertyValueXml">The XML to use as the property value.</param>
        /// <param name="typeName">The type name to use for the m:type attribute.</param>
        /// <param name="nullAttributeValue">The value of the m:null attribute.</param>
        /// <returns>The <paramref name="payloadElement"/> after adding the annotation.</returns>
        public static T XmlValueRepresentation<T>(this T payloadElement, string propertyValueXml, string typeName, string nullAttributeValue)
            where T : ODataPayloadElement, ITypedValue
        {
            return payloadElement.XmlValueRepresentation(
                ParseXmlContent(propertyValueXml),
                new XAttribute[] { 
                    typeName == null ? null : new XAttribute(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomTypeAttributeName, typeName),
                    nullAttributeValue == null ? null : new XAttribute(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataNullAttributeName, nullAttributeValue)
                });
        }

        /// <summary>
        /// Parses the specified xml with predefined well known namespaces and returns the fragment.
        /// </summary>
        /// <param name="xml">The XML to parse.</param>
        /// <returns>Enumeration of XNodes which represent the fragment.</returns>
        private static IEnumerable<XNode> ParseXmlContent(string xml)
        {
            if (xml == null)
            {
                return new XNode[0];
            }
            else if (xml.Length == 0)
            {
                return new[] { new XText(string.Empty) };
            }
            else
            {
                string tempElementString = string.Format(
                    CultureInfo.InvariantCulture,
                    "<temp {0}>{1}</temp>",
                    StandardAtomNamespaceDeclarations,
                    xml);

                return XElement.Parse(tempElementString, LoadOptions.PreserveWhitespace).Nodes();
            }
        }

        /// <summary>
        /// Parses the specified xml attributes with predefined well known namespaces and returns the fragment.
        /// </summary>
        /// <param name="xml">The XML attributes to parse.</param>
        /// <returns>Enumeration of XAttributes.</returns>
        private static IEnumerable<XAttribute> ParseXmlAttributes(string xml)
        {
            string tempElement = string.Format(
                CultureInfo.InvariantCulture,
                "<tempElement {0} xmlns:__c='tempElementUri'><__c:attributes {1}/></tempElement>",
                StandardAtomNamespaceDeclarations,
                xml);
            return XElement.Parse(tempElement).Element(XNamespace.Get("tempElementUri").GetName("attributes")).Attributes();
        }
    }
}
