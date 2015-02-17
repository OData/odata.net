//---------------------------------------------------------------------
// <copyright file="PropertiesElementAtomValues.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    #endregion Namespaces

    /// <summary>
    /// Interesting test values for the properties element (complex value, m:properties, ...)
    /// </summary>
    public class PropertiesElementAtomValues
    {
        /// <summary>
        /// Generates interesting properties element payloads with paddings for ATOM reader.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to use.</typeparam>
        /// <param name="sourceTestDescriptor">The source descriptor to use.
        /// This should have the PayloadElement be the instance to which to add properties, the PayloadModel should be filled if necessary
        /// with model which defines two properties stringProperty (Edm.String) and numberProperty (Edm.Int32).</param>
        /// <param name="applyXmlValue">Func which will apply the generated XML value to the payload element.</param>
        /// <returns>Enumeration of interesting test descriptors.</returns>
        public static IEnumerable<PayloadReaderTestDescriptor> CreatePropertiesElementPaddingPayloads<T>(
            PayloadReaderTestDescriptor sourceTestDescriptor,
            Func<T, string, T> applyXmlValue) where T : ComplexInstance
        {
            var payloadWithPadding = new[]
            {
                // Element with insignificant content is a valid complex value
                new
                {
                    XmlValue = "{0}",
                    Properties = new PropertyInstance[] { }
                },
                // Simple empty string property - padding before
                new
                {
                    XmlValue = "{0}<d:stringProperty/>",
                    Properties = new PropertyInstance[] { PayloadBuilder.PrimitiveProperty("stringProperty", string.Empty) }
                },
                // Simple empty string property - padding after
                new
                {
                    XmlValue = "<d:stringProperty/>{0}",
                    Properties = new PropertyInstance[] { PayloadBuilder.PrimitiveProperty("stringProperty", string.Empty) }
                },
                // Two simple properties - padding between
                new
                {
                    XmlValue = "<d:stringProperty/>{0}<d:numberProperty m:type='Edm.Int32'>42</d:numberProperty>",
                    Properties = new PropertyInstance[] { 
                        PayloadBuilder.PrimitiveProperty("stringProperty", string.Empty),
                        PayloadBuilder.PrimitiveProperty("numberProperty", 42)
                    }
                },
                // Two simple properties - padding after
                new
                {
                    XmlValue = "<d:stringProperty/><d:numberProperty m:type='Edm.Int32'>42</d:numberProperty>{0}",
                    Properties = new PropertyInstance[] { 
                        PayloadBuilder.PrimitiveProperty("stringProperty", string.Empty),
                        PayloadBuilder.PrimitiveProperty("numberProperty", 42)
                    }
                },
            };

            string[] xmlPaddingToIgnore = new string[]
            {
                string.Empty,  // Nothing
                "  \r\n\t", // Whitespace only
                "<!--s--> <?value?>", // Insignificant nodes
                "some text <![CDATA[cdata]]>", // Significant nodes to be ignored
                "<foo xmlns=''/>", // Element in no namespace
                "<c:foo xmlns:c='uri' attr='1'><c:child/>text</c:foo>", // Element in custom namespace
                "<m:properties/>", // Element in metadata namespace (should be ignored as well)
                "<entry/>", // Element in atom namespace (should also be ignored)
            };

            return payloadWithPadding.SelectMany(payload =>
                xmlPaddingToIgnore.Select(xmlPadding =>
                    {
                        var payloadElement = (T)sourceTestDescriptor.PayloadElement.DeepCopy();
                        foreach (var prop in payload.Properties)
                        {
                            payloadElement.Add(prop);
                        }

                        payloadElement = applyXmlValue(payloadElement, string.Format(CultureInfo.InvariantCulture, payload.XmlValue, xmlPadding));

                        return new PayloadReaderTestDescriptor(sourceTestDescriptor)
                        {
                            PayloadElement = payloadElement
                        };
                    }));
        }

        /// <summary>
        /// Generates interesting properties element payloads with ordering of properties for ATOM reader.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to use.</typeparam>
        /// <param name="sourceTestDescriptor">The source descriptor to use.
        /// This should have the PayloadElement be the instance to which to add properties, the PayloadModel should be filled if necessary
        /// with model which defines two properties stringProperty (Edm.String) and numberProperty (Edm.Int32) and nullProperty (Edm.String nullable).</param>
        /// <param name="applyXmlValue">Func which will apply the generated XML value to the payload element.</param>
        /// <returns>Enumeration of interesting test descriptors.</returns>
        public static IEnumerable<PayloadReaderTestDescriptor> CreatePropertiesElementOrderingPayloads<T>(
            PayloadReaderTestDescriptor sourceTestDescriptor,
            Func<T, IEnumerable<XNode>, T> applyXmlValue,
            string odataNamespace = TestAtomConstants.ODataNamespace) where T : ComplexInstance
        {
            XNamespace odataXNamespace = XNamespace.Get(odataNamespace);
            var properties = new[]
            {
                new
                {
                    PropertyXml = new XElement(odataXNamespace + "stringProperty", "some"),
                    PropertyElement = PayloadBuilder.PrimitiveProperty("stringProperty", "some")
                },
                new
                {
                    PropertyXml = new XElement(odataXNamespace + "numberProperty",
                        new XAttribute(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.AtomTypeAttributeName, "Edm.Int32"),
                        "42"),
                    PropertyElement = PayloadBuilder.PrimitiveProperty("numberProperty", 42)
                },
                new
                {
                    PropertyXml = new XElement(odataXNamespace + "nullProperty",
                        new XAttribute(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataNullAttributeName, TestAtomConstants.AtomTrueLiteral)),
                    PropertyElement = PayloadBuilder.PrimitiveProperty("nullProperty", null)
                },
            };

            return properties.Permutations().Select(props =>
            {
                var payloadElement = (T)sourceTestDescriptor.PayloadElement.DeepCopy();
                List<XNode> xmlNodes = new List<XNode>();
                for (int i = 0; i < properties.Length; i++)
                {
                    xmlNodes.Add(props[i].PropertyXml);
                    payloadElement.Add(props[i].PropertyElement);
                }

                return new PayloadReaderTestDescriptor(sourceTestDescriptor)
                {
                    PayloadElement = applyXmlValue(payloadElement, xmlNodes)
                };
            });
        }
    }
}