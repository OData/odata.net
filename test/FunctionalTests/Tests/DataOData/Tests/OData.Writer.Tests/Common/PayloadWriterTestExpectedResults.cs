//---------------------------------------------------------------------
// <copyright file="PayloadWriterTestExpectedResults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces.
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;
    #endregion Namespaces.

    /// <summary>
    /// Writer test expected result which specifies the expected payload that the message should deserialize to.
    /// </summary>
    public class PayloadWriterTestExpectedResults : WriterTestExpectedResults
    {
        /// <summary>
        /// Settings class which should be dependency injected and passed to the constructor.
        /// </summary>
        public new class Settings : WriterTestExpectedResults.Settings
        {
            [InjectDependency(IsRequired = true)]
            public IODataPayloadElementComparer PayloadElementComparer { get; set; }
        }

        /// <summary>
        /// Settings to be used.
        /// </summary>
        protected readonly Settings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public PayloadWriterTestExpectedResults(Settings settings)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
        }

        /// <summary>
        /// Expected entity model schema which will be compared to the actual entity model schema created by reading the message content.
        /// </summary>
        public ODataPayloadElement ExpectedPayload { get; set; }

        /// <summary>
        /// Verifies that the result of the test is what the test expected.
        /// </summary>
        /// <param name="stream">The stream after writing the message content. This method should use it 
        /// to read the message content and verify it.</param>
        /// <param name="payloadKind">The payload kind specified in the test descriptor.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public override void VerifyResult(
            TestMessage message,
            ODataPayloadKind payloadKind,
            WriterTestConfiguration testConfiguration,
            BaselineLogger logger)
        {
            // Get observed payload
            var observed = TestWriterUtils.ReadToString(message);

            if (logger != null) logger.LogPayload(TestWriterUtils.BaseLineFixup(observed));
        }
    }
}
