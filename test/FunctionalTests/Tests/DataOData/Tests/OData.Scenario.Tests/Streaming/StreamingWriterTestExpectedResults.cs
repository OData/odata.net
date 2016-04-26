//---------------------------------------------------------------------
// <copyright file="StreamingWriterTestExpectedResults.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming
{
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Fixups;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;

    public class StreamingWriterTestExpectedResults : PayloadWriterTestExpectedResults
    {
        /// <summary>
        /// Gets or sets payload element observed during the streaming.
        /// </summary>
        public ODataPayloadElement ObservedElement { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="settings">Settings class to use.</param>
        public StreamingWriterTestExpectedResults(PayloadWriterTestExpectedResults.Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Verifies the result of the write-read.
        /// </summary>
        /// <param name="message">The test message is not used but is required to keep the method signature the same.</param>
        /// <param name="payloadKind">The payload kind is not used but is required to keep the method signature the same.</param>
        /// <param name="testConfiguration">The test configuration is used for some fixups.</param>
        public override void VerifyResult(TestMessage message, ODataPayloadKind payloadKind, WriterTestConfiguration testConfiguration, BaselineLogger logger=null)
        {
            //TODO: Use Logger to verify result, right now this change is only to unblock writer testcase checkin

            Debug.Assert(ObservedElement != null, "ObservedElement not provided");
            // Fixup the expected and get the content type
            ODataPayloadElement expected = this.ExpectedPayload.DeepCopy();
            ODataPayloadElement observed = this.ObservedElement.DeepCopy();

            observed.Accept(new RemoveTypeNameAnnotationFromComplexInCollection());
            expected.Accept(new ReorderProperties());
            expected.Accept(new RemoveComplexWithNoProperties());
            
            // Compare
            this.settings.PayloadElementComparer.Compare(expected, observed);
        }
    }
}
