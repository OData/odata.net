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
    using Microsoft.OData;
    using Microsoft.OData.Edm;
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
    }
}