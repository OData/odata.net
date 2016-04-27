//---------------------------------------------------------------------
// <copyright file="JsonPayloadErrorTestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Json
{
    #region Namespaces
    using System;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Class to represent a JSON error payload test case.
    /// </summary>
    internal sealed class JsonPayloadErrorTestCase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public JsonPayloadErrorTestCase()
        {
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The <see cref="JsonPayloadErrorTestCase"/> to clone.</param>
        public JsonPayloadErrorTestCase(JsonPayloadErrorTestCase other)
        {
            this.Json = other.Json;
            this.EdmModel = other.EdmModel;
            this.ExpectedExceptionFunc = other.ExpectedExceptionFunc;
        }

        /// <summary>
        /// The input JSON payload.
        /// </summary>
        public ODataPayloadElement Json { get; set; }

        /// <summary>
        /// The model to use for reading the input payload or null if no metadata is available.
        /// </summary>
        public EdmModel EdmModel { get; set; }

        /// <summary>
        /// A function that, given a test configuration, returns an expected exception or 'null' if no error is expected.
        /// </summary>
        public Func<JsonPayloadErrorTestCase, ReaderTestConfiguration, ExpectedException> ExpectedExceptionFunc { get; set; }

        /// <summary>
        /// Func which determines if the test configuration should be skipped.
        /// </summary>
        public Func<ReaderTestConfiguration, bool> SkipTestConfiguration { get; set; }

        /// <summary>
        /// Converts the error test case into a <see cref="PayloadReaderTestDescriptor"/> that can be used to run the test.
        /// </summary>
        /// <param name="payloadTestDescriptorSettings">The settings for the test descriptor.</param>
        /// <param name="payloadExpectedResultSettings">The settings for the expected results.</param>
        /// <returns>A <see cref="PayloadReaderTestDescriptor"/> that can be used to run the test.</returns>
        internal PayloadReaderTestDescriptor ToPayloadReaderTestDescriptor(PayloadReaderTestDescriptor.Settings payloadTestDescriptorSettings, PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings payloadExpectedResultSettings)
        {
            return new PayloadReaderTestDescriptor(payloadTestDescriptorSettings)
            {
                PayloadElement = this.Json,
                PayloadEdmModel = this.EdmModel,
                SkipTestConfiguration = this.SkipTestConfiguration,
                ExpectedResultCallback = (testConfig) =>
                    new PayloadReaderTestExpectedResult(payloadExpectedResultSettings)
                    {
                        ExpectedException = this.ExpectedExceptionFunc == null ? null : this.ExpectedExceptionFunc(this, testConfig),
                    }
            };
        }

        internal PayloadReaderTestDescriptor ToEdmPayloadReaderTestDescriptor(PayloadReaderTestDescriptor.Settings payloadTestDescriptorSettings, PayloadReaderTestExpectedResult.PayloadReaderTestExpectedResultSettings payloadExpectedResultSettings)
        {
            return new PayloadReaderTestDescriptor(payloadTestDescriptorSettings)
            {
                PayloadElement = this.Json,
                PayloadEdmModel = this.EdmModel,
                SkipTestConfiguration = this.SkipTestConfiguration,
                ExpectedResultCallback = (testConfig) =>
                    new PayloadReaderTestExpectedResult(payloadExpectedResultSettings)
                    {
                        ExpectedException = this.ExpectedExceptionFunc == null ? null : this.ExpectedExceptionFunc(this, testConfig),
                    }
            };
        }
    }
}
