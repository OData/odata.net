//---------------------------------------------------------------------
// <copyright file="ErrorReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Atom;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for correct handling of top-level and in-stream error payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ErrorReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings PayloadReaderTestExpectedResultSettings { get; set; }

        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadElementToXmlConverter PayloadElementToXmlConverter { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates the correct behavior of the ODataAtomErrorDeserializer for top-level errors.")]
        public void TopLevelAtomErrorTest()
        {
            #region Extra attributes on the error elements (and sub-elements)
            var extraAttributePayloads = new[]
            {
                new { PayloadElement = PayloadBuilder.Error(), Template = "<m:error {0} />", },
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode"), Template = "<m:error><m:code {0}>ErrorCode</m:code></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode").Message("Message"), Template = "<m:error><m:code>ErrorCode</m:code><m:message {0}>Message</m:message></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError()), Template = "<m:error><m:innererror {0}></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("InnerMessage")), Template = "<m:error><m:innererror><m:message {0}>InnerMessage</m:message></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName("TypeName")), Template = "<m:error><m:innererror><m:type {0}>TypeName</m:type></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().StackTrace("StackTrace")), Template = "<m:error><m:innererror><m:stacktrace {0}>StackTrace</m:stacktrace></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(new ODataInternalExceptionPayload())), Template = "<m:error><m:innererror><m:internalexception {0}></m:internalexception></m:innererror></m:error>" },
            };

            string[] attributes = new string[]
            {
                "foo='bar'",
                "m:foo='bar'",
                "foo=''",
                "lang='invalid'",
            };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = extraAttributePayloads.SelectMany(extraAttributePayload =>
                attributes.Select(attribute =>
                {
                    var payloadElement = extraAttributePayload.PayloadElement.DeepCopy();
                    string xmlRep = string.Format(CultureInfo.InvariantCulture, extraAttributePayload.Template, attribute);
                    payloadElement = payloadElement.XmlRepresentation(xmlRep);

                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement
                    };
                }));
            #endregion Extra attributes on the error elements (and sub-elements)

            #region Extra padding in the error elements' content
            var extraPaddingPayloads = new[]
            {
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode"), Template = "<m:error>{0}<m:code>ErrorCode</m:code></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode"), Template = "<m:error><m:code>ErrorCode</m:code>{0}</m:error>" },
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode").Message("Message"), Template = "<m:error><m:code>ErrorCode</m:code>{0}<m:message>Message</m:message></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode").Message("Message").InnerError(PayloadBuilder.InnerError()), Template = "<m:error><m:code>ErrorCode</m:code><m:message>Message</m:message>{0}<m:innererror></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().Code("ErrorCode").Message("Message").InnerError(PayloadBuilder.InnerError()), Template = "<m:error><m:code>ErrorCode</m:code><m:message>Message</m:message><m:innererror>{0}</m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("InnerMessage")), Template = "<m:error><m:innererror>{0}<m:message>InnerMessage</m:message></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("InnerMessage")), Template = "<m:error><m:innererror><m:message>InnerMessage</m:message>{0}</m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName("TypeName")), Template = "<m:error><m:innererror>{0}<m:type>TypeName</m:type></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName("TypeName")), Template = "<m:error><m:innererror><m:type>TypeName</m:type>{0}</m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName("TypeName").StackTrace("StackTrace")), Template = "<m:error><m:innererror><m:type>TypeName</m:type>{0}<m:stacktrace>StackTrace</m:stacktrace></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName("TypeName").StackTrace("StackTrace")), Template = "<m:error><m:innererror><m:type>TypeName</m:type><m:stacktrace>StackTrace</m:stacktrace>{0}</m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(new ODataInternalExceptionPayload())), Template = "<m:error><m:innererror>{0}<m:internalexception></m:internalexception></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(new ODataInternalExceptionPayload())), Template = "<m:error><m:innererror><m:internalexception>{0}</m:internalexception></m:innererror></m:error>" },
                new { PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().InnerError(new ODataInternalExceptionPayload())), Template = "<m:error><m:innererror><m:internalexception></m:internalexception>{0}</m:innererror></m:error>" },
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

            IEnumerable<PayloadReaderTestDescriptor> extraPaddingTestDescriptors = extraPaddingPayloads.SelectMany(extraPaddingPayload =>
                xmlPaddingToIgnore.Select(xmlPadding =>
                {
                    var payloadElement = extraPaddingPayload.PayloadElement.DeepCopy();
                    string xmlRep = string.Format(CultureInfo.InvariantCulture, extraPaddingPayload.Template, xmlPadding);
                    payloadElement = payloadElement.XmlRepresentation(xmlRep);

                    return new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = payloadElement
                    };
                }));

            testDescriptors = testDescriptors.Concat(extraPaddingTestDescriptors);
            #endregion Extra padding in the error elements' content

            #region Extra elements in various interesting places of an error payload
            ODataErrorPayload errorPayload = PayloadBuilder.Error()
                .Code("ErrorCode")
                .Message("Message")
                .InnerError(
                    PayloadBuilder.InnerError()
                    .Message("InnerMessage")
                    .TypeName("TypeName")
                    .StackTrace("StackTrace")
                    .InnerError(
                        PayloadBuilder.InnerError()
                        .Message("InnerInnerMessage")));

            XElement xmlRepresentation = new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorElementName,
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorCodeElementName, "ErrorCode"),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorMessageElementName, "Message"),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName,
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorMessageElementName, "InnerMessage"),
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorTypeElementName, "TypeName"),
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorStackTraceElementName, "StackTrace"),
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorInnerErrorElementName,
                        new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorMessageElementName, "InnerInnerMessage"))));

            XElement[] extraElements = new XElement[]
            {
                new XElement(XName.Get("foo"), "bar"),
                new XElement(XName.Get("foo"), new XElement(XName.Get("bar"))),
                new XElement(XName.Get("foo"), new XAttribute(XName.Get("bar"), "attribute-value")),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + "foo", "bar"),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + "foo", new XElement(TestAtomConstants.ODataMetadataXNamespace + "bar")),
                new XElement(XName.Get("foo"), new XAttribute(TestAtomConstants.ODataMetadataXNamespace + "bar", "attribute-value")),
            };

            IEnumerable<PayloadReaderTestDescriptor> extraElementTestDescriptors = extraElements.SelectMany(extraElement =>
            {
                return InjectElement(extraElement, xmlRepresentation).Select(errorWithInjectedElement =>
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = errorPayload.DeepCopy().XmlRepresentation(errorWithInjectedElement)
                        }
                    );
            });
            testDescriptors = testDescriptors.Concat(extraElementTestDescriptors);
            #endregion Extra elements in various interesting places of an error payload

            #region Various payload orders for an error element
            errorPayload = PayloadBuilder.Error()
                .Code("ErrorCode")
                .Message("Message")
                .InnerError(
                    PayloadBuilder.InnerError()
                    .Message("InnerMessage")
                    .TypeName("TypeName")
                    .StackTrace("StackTrace")
                    .InnerError(
                        PayloadBuilder.InnerError()
                        .Message("InnerInnerMessage")));

            xmlRepresentation = new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorElementName,
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorCodeElementName, "ErrorCode"),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorMessageElementName, "Message"),
                new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName,
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorMessageElementName, "InnerMessage"),
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorTypeElementName, "TypeName"),
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorStackTraceElementName, "StackTrace"),
                    new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorInnerErrorElementName,
                        new XElement(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorMessageElementName, "InnerInnerMessage"))));

            IEnumerable<PayloadReaderTestDescriptor> payloadOrderTestDescriptors = GeneratePayloadOrders(xmlRepresentation).Select(xmlRep =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = errorPayload.DeepCopy().XmlRepresentation(xmlRep)
                }
            );
            testDescriptors = testDescriptors.Concat(payloadOrderTestDescriptors);
            #endregion Various payload orders for an error element

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates the correct behavior of the ODataAtomErrorDeserializer for invalid top-level errors.")]
        public void TopLevelAtomInvalidErrorTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = CreateInvalidErrorDescriptors(this.Settings);
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates correct behavior of the ODataAtomReader for in-stream errors.")]
        public void InStreamAtomErrorTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateErrorReaderTestDescriptors(this.Settings);

            // convert the payload element to an Xml representation usable for in-stream error testing
            testDescriptors = testDescriptors.Select(td =>
            {
                XElement xmlPayload = this.PayloadElementToXmlConverter.ConvertToXml(td.PayloadElement);
                Debug.Assert(td.ExpectedException == null,  "Don't expect errors for regular payloads (without annotation).");

                return new PayloadReaderTestDescriptor(td)
                {
                    PayloadElement = td.PayloadElement.XmlRepresentation(xmlPayload)
                };
            });

            testDescriptors = testDescriptors.Select(td => td.ToInStreamErrorTestDescriptor(ODataFormat.Atom));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Validates correct behavior of the ODataAtomReader for invalid in-stream errors.")]
        public void InStreamAtomInvalidErrorTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = CreateInvalidErrorDescriptors(this.Settings);
            testDescriptors = testDescriptors.Select(td => td.ToInStreamErrorTestDescriptor(ODataFormat.Atom));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies failure of reading too deeply recursive in-stream error payloads.")]
        public void InStreamAtomDeeplyRecursiveErrorTest()
        {
            int depthLimit = 5;

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateErrorDeeplyNestedReaderTestDescriptors(this.Settings, depthLimit);

            // Convert the payload element to an Xml representation usable for in-stream error testing
            testDescriptors = testDescriptors.Select(td =>
            {
                XElement xmlPayload = this.PayloadElementToXmlConverter.ConvertToXml(td.PayloadElement);

                return new PayloadReaderTestDescriptor(td)
                {
                    PayloadElement = td.PayloadElement.XmlRepresentation(xmlPayload)
                };
            });

            // Convert top-level error test descriptors to in-stream errors.
            testDescriptors = testDescriptors.Select(td => td.ToInStreamErrorTestDescriptor(ODataFormat.Atom));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // Copy the test configuration so we can modify the depth limit.
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.MessageQuotas.MaxNestingDepth = depthLimit;

                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that we correctly read error payload from otherwise invalid XML.")]
        public void InStreamAtomInvalidXmlTest()
        {
            IEnumerable<NativeInputReaderTestDescriptor> testDescriptors = new[]
            {
                // The payload ends right after the </m:error> which is in fact the most typical case.
                new NativeInputReaderTestDescriptor()
                {
                    InputCreator = tc => 
                        "<entry " + ODataAnnotations.StandardAtomNamespaceDeclarations + ">" +
                            "<id>urn:someid</id>" +
                            "<m:error><m:code>123</m:code><m:message>ErrorMessage</m:message></m:error>",
                    PayloadKind = ODataPayloadKind.Entry,
                    ExpectedResultCallback = (tc) =>
                        new PayloadReaderTestExpectedResult(this.PayloadReaderTestExpectedResultSettings)
                        {
                            ExpectedException = ODataExpectedExceptions.ODataErrorException(
                                new ODataError() { ErrorCode = "123", Message = "ErrorMessage" },
                                "ODataErrorException_GeneralError")
                        }
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        /// <summary>
        /// Injects the specified element in all interesting places of the target error payload.
        /// </summary>
        /// <param name="elementToInject">The <see cref="XElement"/> to inject.</param>
        /// <param name="target">The target element to inject into.</param>
        /// <returns>An enumerable of cloned target elements with the <paramref name="elementToInject"/> injected in interesting places.</returns>
        /// <remarks>The method assumes that the target elements has a certain shape; it does not work for arbitrary elements.</remarks>
        private static IEnumerable<XElement> InjectElement(XElement elementToInject, XElement target)
        {
            Action<XElement>[] injectors = new Action<XElement>[]
            {
                element => element.FirstNode.AddBeforeSelf(elementToInject),
                element => element.LastNode.AddAfterSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorCodeElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataErrorMessageElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorMessageElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorTypeElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorStackTraceElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorInnerErrorElementName).AddBeforeSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorInnerErrorElementName).AddAfterSelf(elementToInject),
                element => element.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName).AddAfterSelf(elementToInject),
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
        /// <param name="errorElement">The error element to modify the payload order for.</param>
        /// <returns>An enumerable of cloned error elements with a modified payload order.</returns>
        /// <remarks>The method assumes that the target elements has a certain shape; it does not work for arbitrary elements.</remarks>
        private static IEnumerable<XElement> GeneratePayloadOrders(XElement errorElement)
        {
            // generate all payload orders of the immediate children of the error element
            IList<XElement> children = errorElement.Elements().ToList();
            IEnumerable<XElement[]> permutations = children.Permutations();

            // clone the error element and remove all children
            XElement errorElementWithoutChildren = new XElement(errorElement);
            errorElementWithoutChildren.RemoveAll();

            foreach (XElement[] permutation in permutations)
            {
                XElement targetElement = new XElement(errorElementWithoutChildren);
                targetElement.Add(permutation);
                yield return targetElement;
            }

            // generate all payload orders of the inner error of the error element
            XElement innerErrorElement = errorElement.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName);
            children = innerErrorElement.Elements().ToList();
            permutations = children.Permutations();

            // clone the error element and remove all children of the <m:innererror> element.
            XElement errorElementWithoutInnerErrorChildren = new XElement(errorElement);
            innerErrorElement = errorElementWithoutInnerErrorChildren.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName);
            innerErrorElement.RemoveAll();

            foreach (XElement[] permutation in permutations)
            {
                XElement targetElement = new XElement(errorElementWithoutInnerErrorChildren);
                innerErrorElement = targetElement.Element(TestAtomConstants.ODataMetadataXNamespace + TestAtomConstants.ODataInnerErrorElementName);
                innerErrorElement.Add(permutation);
                yield return targetElement;
            }
        }

        /// <summary>
        /// Creates invalid error representations in ATOM (e.g., extra properties where they are not allowed,
        /// invalid property value types, etc.)
        /// </summary>
        /// <param name="settings">The test descriptor settings to use for the generated <see cref="PayloadReaderTestDescriptor"/>.</param>
        /// <returns>An enumerable of <see cref="PayloadReaderTestDescriptor"/> representing the invalid error payloads.</returns>
        private static IEnumerable<PayloadReaderTestDescriptor> CreateInvalidErrorDescriptors(PayloadReaderTestDescriptor.Settings settings)
        {
            return new PayloadReaderTestDescriptor[]
            {
                #region Duplicate error elements
                // duplicate 'm:code' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().Code("ErrorCode").XmlRepresentation("<m:error><m:code>ErrorCode</m:code><m:code>ErrorCode2</m:code></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName", "code")
                },
                // duplicate 'm:message' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().Message("ErrorMessage").XmlRepresentation("<m:error><m:message>ErrorMessage</m:message><m:message>ErrorMessage2</m:message></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName", "message")
                },
                // duplicate 'm:innererror' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload()).XmlRepresentation("<m:error><m:innererror></m:innererror><m:innererror></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName", "innererror")
                },
                // duplicate (inner) 'm:message' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload().Message("InnerMessage")).XmlRepresentation("<m:error><m:innererror><m:message>InnerMessage</m:message><m:message>InnerMessage2</m:message></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName", "message")
                },
                // duplicate (inner) 'm:type' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload().TypeName("TypeName")).XmlRepresentation("<m:error><m:innererror><m:type>TypeName</m:type><m:type>TypeName2</m:type></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName", "type")
                },
                // duplicate (inner) 'm:stacktrace' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload().StackTrace("StackTrace")).XmlRepresentation("<m:error><m:innererror><m:stacktrace>StackTrace</m:stacktrace><m:stacktrace>StackTrace2</m:stacktrace></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName", "stacktrace")
                },
                // duplicate (inner) 'm:internalexception' element
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(new ODataInternalExceptionPayload().InnerError(new ODataInternalExceptionPayload())).XmlRepresentation("<m:error><m:innererror><m:internalexception></m:internalexception><m:internalexception></m:internalexception></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName", "internalexception")
                },
                #endregion Duplicate error elements

                #region Element content in string elements
                // Element content in <m:code>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().Code("ErrorCode").XmlRepresentation("<m:error><m:code><foo></foo>ErrorCode<bar /></m:code></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                // Element content in <m:message>
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().Message("Message").XmlRepresentation("<m:error><m:message><foo></foo>Message<bar /></m:message></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                // Element content in <m:message> (inner)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().Message("Message"))
                        .XmlRepresentation("<m:error><m:innererror><m:message><foo></foo>Message<bar /></m:message></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                // Element content in <m:type> (inner)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().TypeName("Type"))
                        .XmlRepresentation("<m:error><m:innererror><m:type><foo></foo>TypeName<bar /></m:type></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                // Element content in <m:stacktrace> (inner)
                new PayloadReaderTestDescriptor(settings)
                {
                    PayloadElement = PayloadBuilder.Error().InnerError(PayloadBuilder.InnerError().StackTrace("StackTrace"))
                        .XmlRepresentation("<m:error><m:innererror><m:stacktrace><foo></foo>StackTrace<bar /></m:stacktrace></m:innererror></m:error>"),
                    ExpectedException = ODataExpectedExceptions.ODataException("XmlReaderExtension_InvalidNodeInStringValue", "Element"),
                },
                #endregion Element content in string elements
            };
        }
    }
}
