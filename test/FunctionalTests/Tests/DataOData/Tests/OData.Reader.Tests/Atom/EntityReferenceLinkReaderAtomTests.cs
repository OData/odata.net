//---------------------------------------------------------------------
// <copyright file="EntityReferenceLinkReaderAtomTests.cs" company="Microsoft">
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
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for correct handling of invalid entity reference link payloads.
    /// </summary>
    [TestClass, TestCase]
    public class EntityReferenceLinkReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings PayloadExpectedResultSettings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates correct error behavior of the ODataAtomEntityReferenceLinkDeserializer for invalid top-level (singleton) entity reference link payloads.")]
        public void EntityReferenceLinkReaderAtomTest()
        {
            // extra attributes on the <uri> element
            var linkElementPayloads = new[]
            {
                // The spec behavior - the element is in the OData namespace
                new
                {
                    PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/link"),
                    Template = "<m:ref id=\"http://odata.org/link\"/>",
                },
                // The backward compatibility behavior - the element is in the OData metadata namespace
                new
                {
                    PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/link"),
                    Template = "<m:ref id=\"http://odata.org/link\" />",
                },
            };

            string[] attributes = new string[]
            {
                "foo='bar'",
                "m:foo='bar'",
                "foo=''",
                "lang='invalid'",
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = linkElementPayloads.SelectMany(linkPayload =>
                attributes.Select(attribute =>
                {
                    var payloadElement = linkPayload.PayloadElement.DeepCopy();
                    string xmlRepresentation = string.Format(CultureInfo.InvariantCulture, linkPayload.Template, attribute);
                    payloadElement = payloadElement.XmlRepresentation(xmlRepresentation);

                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement
                    };
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates correct error behavior of the ODataAtomEntityReferenceLinkDeserializer for invalid top-level entity reference link (singleton) payloads.")]
        public void InvalidEntityReferenceLinkReaderAtomTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region Wrong name and namespace
                // Wrong name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/link").XmlRepresentation("<m:Ref id=\"http://odata.org/link\" />"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinkStartElement", "Ref", "http://docs.oasis-open.org/odata/ns/metadata"),
                },
                // Wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.DeferredLink("http://odata.org/link").XmlRepresentation("<ref id=\"http://odata.org/link\" />"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinkStartElement", "ref", "http://www.w3.org/2005/Atom"),
                },
                #endregion Wrong name and namespace
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates correct error behavior of the ODataAtomEntityReferenceLinkDeserializer for top-level entity reference link (collection) payloads.")]
        public void EntityReferenceLinksReaderAtomTest()
        {
            const string link1String = "http://odata.org/deferred1";
            const string link2String = "http://odata.org/deferred2";
            const string nextString = "http://odata.org/next";

            DeferredLink link1 = PayloadBuilder.DeferredLink(link1String);
            DeferredLink link2 = PayloadBuilder.DeferredLink(link2String);

            #region Extra attributes on the <feed> element
            var linksElementPayloads = new[]
            {
                // extra attributes on <links> element without content
                new { PayloadElement = PayloadBuilder.LinkCollection(), Template = "<feed {0} />", },

                // extra attributes on <m:count> element
                new { PayloadElement = PayloadBuilder.LinkCollection().InlineCount(1), Template = "<feed><m:count {0}>1</m:count></feed>", },

                // extra attributes on <d:next> element
                new { PayloadElement = PayloadBuilder.LinkCollection().NextLink("http://odata.org/next"), Template = "<feed><d:next {0}>http://odata.org/next</d:next></feed>", },
            };

            string[] attributes = new string[]
            {
                "foo='bar'",
                "m:foo='bar'",
                "foo=''",
                "lang='invalid'",
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = linksElementPayloads.SelectMany(linkPayload =>
                attributes.Select(attribute =>
                {
                    var payloadElement = linkPayload.PayloadElement.DeepCopy();
                    string xmlRep = string.Format(CultureInfo.InvariantCulture, linkPayload.Template, attribute);
                    payloadElement = payloadElement.XmlRepresentation(xmlRep);

                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement,
                    };
                }));
            #endregion Extra attributes on the <links> element

            #region Extra padding between the elements
            var extraPaddingPayloads = new[]
            {
                new { PayloadElement = PayloadBuilder.LinkCollection(), Template = "<feed>{0}</feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().InlineCount(1), Template = "<feed>{0}<m:count>1</m:count></feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().InlineCount(1), Template = "<feed><m:count>1</m:count>{0}</feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().NextLink(nextString), Template = "<feed>{0}<d:next>http://odata.org/next</d:next></feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().NextLink(nextString), Template = "<feed><d:next>http://odata.org/next</d:next>{0}</feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().InlineCount(1).NextLink(nextString), Template = "<feed><m:count>1</m:count>{0}<d:next>http://odata.org/next</d:next></feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().Item(link1).InlineCount(1).NextLink(nextString), Template = "<feed><m:count>1</m:count>{0}<m:ref id =\"" + link1String + "\"/><d:next>" + nextString + "</d:next></feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().Item(link1).InlineCount(1).NextLink(nextString), Template = "<feed><m:count>1</m:count><m:ref id=\"" + link1String + "\"/>{0}<d:next>" + nextString + "</d:next></feed>", },
                new { PayloadElement = PayloadBuilder.LinkCollection().Item(link1).Item(link2).InlineCount(2).NextLink(nextString), Template = "<feed><m:count>2</m:count><m:ref id=\"" + link1String + "\"/>{0}<m:ref id=\"" + link2String + "\"/><d:next>" + nextString + "</d:next></feed>", },
            };

            string[] xmlPaddingToIgnore = new string[]
            {
                string.Empty,  // Nothing
                "  \r\n\t", // Whitespace only
                "<!--s--> <?value?>", // Insignificant nodes
                "some text <![CDATA[cdata]]>", // Significant nodes to be ignored
                "<foo xmlns=''/>", // Element in no namespace
                "<c:foo xmlns:c='ref' attr='1'><c:child/>text</c:foo>", // Element in custom namespace
                "<d:properties/>", // Element in data namespace (should be ignored as well)
                "<entry/>", // Element in atom namespace (should also be ignored)
            };

            IEnumerable<PayloadReaderTestDescriptor> extraPaddingTestDescriptors = extraPaddingPayloads.SelectMany(extraPaddingPayload =>
                xmlPaddingToIgnore.Select(xmlPadding =>
                {
                    var payloadElement = extraPaddingPayload.PayloadElement.DeepCopy();
                    string xmlRep = string.Format(CultureInfo.InvariantCulture, extraPaddingPayload.Template, xmlPadding);
                    payloadElement = payloadElement.XmlRepresentation(xmlRep);

                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement,
                    };
                }));
            testDescriptors = testDescriptors.Concat(extraPaddingTestDescriptors);
            #endregion Extra padding between the elements

            #region Extra elements in the <feed> content
            LinkCollection links = PayloadBuilder.LinkCollection()
                .InlineCount(2)
                .Item(PayloadBuilder.DeferredLink(link1String))
                .Item(PayloadBuilder.DeferredLink(link2String))
                .NextLink(nextString);

            XElement ref1Element = new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataRefElementName);
            ref1Element.SetAttributeValue(TestAtomConstants.AtomIdElementName, link1String);

            XElement ref2Element = new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataRefElementName);
            ref1Element.SetAttributeValue(TestAtomConstants.AtomIdElementName, link2String);

            XElement xmlRepresentation = new XElement(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName,
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataCountElementName, "2"),
                ref1Element,
                ref2Element,
                new XElement(TestAtomConstants.ODataXNamespace + TestAtomConstants.ODataNextLinkElementName, nextString));

            XElement[] extraElements = new XElement[]
            {
                new XElement(XName.Get("foo"), "bar"),
                new XElement(XName.Get("foo"), new XElement(XName.Get("bar"))),
                new XElement(XName.Get("foo"), new XAttribute(XName.Get("bar"), "attribute-value")),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + "foo", "bar"),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + "foo", new XElement(TestAtomConstants.ODataMetadataXNamespace + "bar")),
                new XElement(XName.Get("foo"), new XAttribute(TestAtomConstants.ODataMetadataXNamespace + "bar", "attribute-value")),
                // "Ref" element in the OData metadaata namespace, should be ignored, "ref" is expected
                new XElement(TestAtomConstants.ODataMetadataXNamespace + "Ref"),
                // "uri" element in the OData namespace, should be ignored, OData metadata namespace is expected
                new XElement(TestAtomConstants.ODataXNamespace + "ref"),
            };

            IEnumerable<PayloadReaderTestDescriptor> extraElementTestDescriptors = extraElements.SelectMany(extraElement =>
            {
                return InjectElement(extraElement, xmlRepresentation).Select(linksWithInjectedElement =>
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = links.DeepCopy().XmlRepresentation(linksWithInjectedElement),
                        }
                    );
            });
            testDescriptors = testDescriptors.Concat(extraElementTestDescriptors);
            #endregion Extra elements in the <links> content

            #region Various payload orders for an error element
            links = PayloadBuilder.LinkCollection()
                .InlineCount(1)
                .Item(PayloadBuilder.DeferredLink(link1String))
                .NextLink(nextString);

            ref1Element = new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataRefElementName);
            ref1Element.SetAttributeValue(TestAtomConstants.AtomIdElementName, link1String);

            xmlRepresentation = new XElement(TestAtomConstants.AtomXNamespace + TestAtomConstants.AtomFeedElementName,
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataCountElementName, "1"),
                ref1Element,
                new XElement(TestAtomConstants.ODataXNamespace + TestAtomConstants.ODataNextLinkElementName, nextString));

            IEnumerable<PayloadReaderTestDescriptor> payloadOrderTestDescriptors = GeneratePayloadOrders(xmlRepresentation).Select(xmlRep =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = links.DeepCopy().XmlRepresentation(xmlRep),
                }
            );

            testDescriptors = testDescriptors.Concat(payloadOrderTestDescriptors);
            #endregion Various payload orders for an error element

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates correct error behavior of the ODataAtomEntityReferenceLinkDeserializer for invalid top-level entity reference link (collection) payloads.")]
        public void InvalidEntityReferenceLinksReaderAtomTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                #region Wrong name and namespace
                // Wrong name
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.LinkCollection().XmlRepresentation("<m:Ref></m:Ref>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksStartElement", "Ref", TestAtomConstants.ODataMetadataNamespace)
                },
                // Wrong namespace
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.LinkCollection().XmlRepresentation("<m:ref></m:ref>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomEntityReferenceLinkDeserializer_InvalidEntityReferenceLinksStartElement", "ref", TestAtomConstants.ODataMetadataNamespace)
                },
                #endregion

                #region Duplicate error elements
                // duplicate 'm:count' element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.LinkCollection().InlineCount(1).XmlRepresentation("<feed><m:count>1</m:count><m:count>2</m:count><m:ref id=\"http://odata.org/refid\"></m:ref></feed>"),
                    ExpectedResultCallback = tc =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedPayloadElement = PayloadBuilder.LinkCollection(),
                            ExpectedException =  ODataExpectedExceptions.ODataException("ODataAtomEntityReferenceLinkDeserializer_MultipleEntityReferenceLinksElementsWithSameName", TestAtomConstants.ODataMetadataNamespace, TestAtomConstants.ODataCountElementName)
                        }
                },

                // duplicate 'd:next' element
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.LinkCollection().NextLink("http://odata.org/next1").XmlRepresentation("<feed><d:next>http://odata.org/next1</d:next><d:next>http://odata.org/next2</d:next></feed>"),
                    ExpectedResultCallback = tc =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedPayloadElement = PayloadBuilder.LinkCollection(),
                            ExpectedException =  ODataExpectedExceptions.ODataException("ODataAtomEntityReferenceLinkDeserializer_MultipleEntityReferenceLinksElementsWithSameName", TestAtomConstants.ODataNamespace, TestAtomConstants.ODataNextLinkElementName)
                        }
                },
                #endregion Duplicate error elements

                #region Element content in string elements
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.LinkCollection().InlineCount(1).XmlRepresentation("<feed><m:count><foo></foo>1<bar /></m:count></feed>"),
                    ExpectedResultCallback = tc =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedPayloadElement = PayloadBuilder.LinkCollection(),
                            ExpectedException =  ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                        }
                },
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.LinkCollection().NextLink("http://odata.org/next1").XmlRepresentation("<feed><d:next><foo></foo>http://odata.org/next1<bar /></d:next></feed>"),
                    ExpectedResultCallback = tc =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedPayloadElement = PayloadBuilder.LinkCollection(),
                            ExpectedException =  ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                        }
                },
                #endregion Element content in string elements
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        /// <summary>
        /// Injects the specified element in all interesting places of the target entity reference link payload.
        /// </summary>
        /// <param name="elementToInject">The <see cref="XElement"/> to inject.</param>
        /// <param name="target">The target element to inject into.</param>
        /// <returns>An enumerable of cloned target elements with the <paramref name="elementToInject"/> injected in interesting places.</returns>
        /// <remarks>The method assumes that the target elements has a certain shape; it does now work for arbitrary elements.</remarks>
        private static IEnumerable<XElement> InjectElement(XElement elementToInject, XElement target)
        {
            Action<XElement>[] injectors = new Action<XElement>[]
            {
                element => element.FirstNode.AddBeforeSelf(elementToInject),
                element => element.LastNode.AddAfterSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataCountElementName).AddBeforeSelf(elementToInject),
                element => element.Elements(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataRefElementName).First().AddBeforeSelf(elementToInject),
                element => element.Elements(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataRefElementName).Last().AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataXNamespace + TestAtomConstants.ODataNextLinkElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataXNamespace + TestAtomConstants.ODataNextLinkElementName).AddAfterSelf(elementToInject),
            };

            for (int i = 0; i < injectors.Length; ++i)
            {
                var clonedTarget = new XElement(target);
                injectors[i](clonedTarget);
                yield return clonedTarget;
            }
        }

        /// <summary>
        /// Modifies the payload order of the specified element.
        /// </summary>
        /// <param name="linksElement">The entity reference links element to modify the payload order for.</param>
        /// <returns>An enumerable of cloned error elements with a modified payload order.</returns>
        /// <remarks>The method assumes that the target elements has a certain shape; it does now work for arbitrary elements.</remarks>
        private static IEnumerable<XElement> GeneratePayloadOrders(XElement linksElement)
        {
            // generate all payload orders of the immediate children of the error element
            IList<XElement> children = linksElement.Elements().ToList();
            IEnumerable<XElement[]> permutations = children.Permutations();

            // clone the error element and remove all children
            XElement errorElementWithoutChildren = new XElement(linksElement);
            errorElementWithoutChildren.RemoveAll();

            foreach (XElement[] permutation in permutations)
            {
                XElement targetElement = new XElement(errorElementWithoutChildren);
                targetElement.Add(permutation);
                yield return targetElement;
            }
        }
    }
}
