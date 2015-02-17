//---------------------------------------------------------------------
// <copyright file="XmlBaseReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm.Library;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests handling of xml:base and URIs in payloads.
    /// </summary>
    [TestClass, TestCase]
    public class XmlBaseReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        // Note - no reason to have "missing" xml:base tests as that is tested by UriHandlingTests.MissingBaseUri tests.

        /// <summary>
        /// A test case class that specifies a relative payload URI, an xml:base attribute value,
        /// a base URI for the message reader settings and an expected result URI.
        /// It then has methods to compute the expected result URI and the expected exception
        /// for a set of test variation parameters.
        /// </summary>
        private sealed class XmlBaseUriTestCase
        {
            /// <summary>The relative URI used in the payload.</summary>
            public string RelativeUri { get; set; }

            /// <summary>The value of the xml:base attribute.</summary>
            public string XmlBaseUri { get; set; }

            /// <summary>The base URI value specified in the message reader settings.</summary>
            public string SettingsBaseUri { get; set; }

            /// <summary>The result URI from processing the relative URI in the payload.</summary>
            public string ResultUri { get; set; }

            /// <summary>
            /// Computes the expected exception for this test case.
            /// </summary>
            /// <param name="behaviorKind">The <see cref="TestODataBehaviorKind"/> used by this test variation.</param>
            /// <param name="version">The <see cref="ODataVersion"/> used by this test variation.</param>
            /// <param name="ignoredOnServer">true if the payload value is ignored on the server; otherwise false.</param>
            /// <returns>The expected exception for a test variation using the specified parameter values; null if no exception is expected.</returns>
            public ExpectedException ComputeExpectedException(TestODataBehaviorKind behaviorKind, ODataVersion version, bool ignoredOnServer)
            {
                bool settingsBaseIsNullOrRelative = this.SettingsBaseUri == null || !(new Uri(this.SettingsBaseUri, UriKind.RelativeOrAbsolute).IsAbsoluteUri);
                bool xmlBaseIsNullOrRelative = this.XmlBaseUri == null || !(new Uri(this.XmlBaseUri, UriKind.RelativeOrAbsolute).IsAbsoluteUri);
                bool ignoreXmlBase = false;

                // If both the settings base URI and an xml:base are relative we will fail when we detect the xml:base.
                if (settingsBaseIsNullOrRelative && xmlBaseIsNullOrRelative && !ignoreXmlBase)
                {
                    string relativeBase = this.XmlBaseUri == null ? string.Empty : this.XmlBaseUri;
                    return ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", relativeBase);
                }

                // Special rules for WCF DS server behavior.
                if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer)
                {
                    // If the payload is ignored on the server, we expect no exception.
                    if (ignoredOnServer)
                    {
                        return null;
                    }
                }

                return null;
            }

            /// <summary>
            /// Computes the expected result URI for this test case and a specified test variation.
            /// </summary>
            /// <param name="behaviorKind">The <see cref="TestODataBehaviorKind"/> used by this test variation.</param>
            /// <param name="version">The <see cref="ODataVersion"/> used by this test variation.</param>
            /// <param name="ignoredOnServer">true if the payload value is ignored on the server; otherwise false.</param>
            /// <returns>The expected result URI from processing the relative URI in the payload.</returns>
            public string ComputeExpectedResultUri(TestODataBehaviorKind behaviorKind, ODataVersion version, bool ignoredOnServer)
            {
                // Special rules apply for the WCF DS server behavior.
                if (behaviorKind == TestODataBehaviorKind.WcfDataServicesServer)
                {
                    // If the payload URI is not even read in the server, we expect a null value.
                    if (ignoredOnServer)
                    {
                        return null;
                    }

                    return this.ResultUri;
                }

                return this.ResultUri;
            }
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verify the correct handling of valid xml:base values.")]
        public void XmlBaseUriHandlingTest()
        {
            var testCases = new XmlBaseUriTestCase[]
            {
                new XmlBaseUriTestCase
                {
                    RelativeUri = "relative",
                    XmlBaseUri = "http://odata.org/base",
                    SettingsBaseUri = (string)null,
                    ResultUri = "http://odata.org/relative",
                },
                new XmlBaseUriTestCase
                {
                    RelativeUri = "relative",
                    XmlBaseUri = (string)null,
                    SettingsBaseUri = (string)null,
                    ResultUri = "http://odata.org/relative",
                },
                new XmlBaseUriTestCase
                {
                    RelativeUri = "relative",
                    XmlBaseUri = "/baserelative/",
                    SettingsBaseUri = "http://odata.org/base",
                    ResultUri = "http://odata.org/baserelative/relative"
                },
                new XmlBaseUriTestCase
                {
                    RelativeUri = "relative",
                    XmlBaseUri = string.Empty,
                    SettingsBaseUri = "http://odata.org/",
                    ResultUri = "http://odata.org/relative"
                },
                new XmlBaseUriTestCase
                {
                    RelativeUri = "relative",
                    XmlBaseUri = "/baserelative/",
                    SettingsBaseUri = "http://odata.org/base/",
                    ResultUri = "http://odata.org/baserelative/relative"
                },
                new XmlBaseUriTestCase
                {
                    RelativeUri = "relative",
                    XmlBaseUri = string.Empty,
                    SettingsBaseUri = "http://odata.org/base/",
                    ResultUri = "http://odata.org/base/relative"
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                TestReaderUtils.ODataBehaviorKinds,
                (testCase, testConfiguration, behavior) =>
                {
                    IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
                    {
                        // MLE with relative media source link
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink(testCase.ComputeExpectedResultUri(behavior, testConfiguration.Version, /*ignoredOnServer*/ true))
                                .XmlRepresentation("<entry xml:base='" + testCase.XmlBaseUri + "'><content src='" + testCase.RelativeUri +"'/></entry>"),
                            ExpectedException = testCase.ComputeExpectedException(behavior, testConfiguration.Version, /*ignoredOnServer*/ true)
                        },

                        // Navigation link with relative href
                        new PayloadReaderTestDescriptor(this.Settings)
                        {
                            PayloadElement = PayloadBuilder.Entity()
                                .NavigationProperty("NavLink", testCase.ComputeExpectedResultUri(behavior, testConfiguration.Version, /*ignoredOnServer*/ false))
                                .XmlRepresentation("<entry xml:base='" + testCase.XmlBaseUri + "'><content /><link rel='http://docs.oasis-open.org/odata/ns/related/NavLink' type='application/atom+xml;type=entry' title='NavLink' href='" + testCase.RelativeUri + "' /></entry>"),
                            ExpectedException = testCase.ComputeExpectedException(behavior, testConfiguration.Version, /*ignoredOnServer*/ false)
                        },

                        // TODO: Add tests like this for all URLs we can process, including ATOM Metadata ones!
                    };

                    this.CombinatorialEngineProvider.RunCombinations(
                        testDescriptors,
                        testDescriptor =>
                        {
                            // Clone the test configuration and set base URI and behavior.
                            testConfiguration = new ReaderTestConfiguration(testConfiguration);
                            testConfiguration.MessageReaderSettings.BaseUri = testCase.SettingsBaseUri == null ? null : new Uri(testCase.SettingsBaseUri);
                            testConfiguration = testConfiguration.CloneAndApplyBehavior(behavior);

                            testDescriptor.RunTest(testConfiguration);
                        });
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verify correct handling of invalid xml:base values.")]
        public void InvalidXmlBaseUriHandlingTest()
        {
            string absoluteUri = "http://odata.org/relative";

            var testCases = new[]
            {
                new
                {
                    BaseUriString = "relativeUri",
                    SettingsBaseUri = (string)null,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", "relativeUri")
                },
                new
                {
                    BaseUriString = string.Empty,
                    SettingsBaseUri = (string)null,
                    ExpectedException = ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", string.Empty)
                },
                new
                {
                    BaseUriString = "http://invalid:uri:value",
                    SettingsBaseUri = (string)null,
                    ExpectedException = new ExpectedException(typeof(UriFormatException))
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                new TestODataBehaviorKind[] { TestODataBehaviorKind.Default, TestODataBehaviorKind.WcfDataServicesServer },
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testCase, behaviorKind, testConfiguration) =>
                {
                    var td = new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity().AsMediaLinkEntry().StreamSourceLink(absoluteUri)
                            .XmlRepresentation("<entry xml:base='" + testCase.BaseUriString + "'><content src='" + absoluteUri + "'/></entry>"),
                        ExpectedException = testCase.ExpectedException
                    };

                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    // In server mode we need to normalize payload to not expect information that the server does not read
                    if(behaviorKind == TestODataBehaviorKind.WcfDataServicesServer)
                    {
                        td.ExpectedResultNormalizers.Add((tc) => (payloadElement => WcfDsServerPayloadElementNormalizer.Normalize(payloadElement, tc.Format, (EdmModel)null)));
                    }


                    testConfiguration = testConfiguration.CloneAndApplyBehavior(behaviorKind);
                    testConfiguration.MessageReaderSettings.BaseUri = testCase.SettingsBaseUri == null ? null : new Uri(testCase.SettingsBaseUri);

                    td.RunTest(testConfiguration);
                });
        }
    }
}