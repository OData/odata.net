//---------------------------------------------------------------------
// <copyright file="MetadataWriterTestExpectedResult.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;
    #endregion Namespaces

    /// <summary>
    /// Writer test expected result which specified the expected metadata document.
    /// </summary>
    public class MetadataWriterTestExpectedResult : WriterTestExpectedResults
    {
        /// <summary>
        /// Settings to be used.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public MetadataWriterTestExpectedResult(Settings settings)
            : base(settings)
        {
            ExceptionUtilities.CheckArgumentNotNull(settings, "settings");

            this.settings = settings;
            this.EdmVersion = EdmVersion.Latest;
        }

        /// <summary>
        /// The metadata in the form of entity model for the payload.
        /// </summary>
        public EdmVersion EdmVersion { get; set; }

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
            this.settings.Assert.IsTrue(payloadKind == ODataPayloadKind.MetadataDocument, "Only metadata payload kind is supported.");

            // read the message content using the Taupo infrastructure
            ExceptionUtilities.CheckArgumentNotNull(message.TestStream, "stream != null");
            var observed = TestWriterUtils.ReadToString(message);

            if (logger != null) logger.LogPayload(TestWriterUtils.BaseLineFixup(observed));
        }
    }
}
