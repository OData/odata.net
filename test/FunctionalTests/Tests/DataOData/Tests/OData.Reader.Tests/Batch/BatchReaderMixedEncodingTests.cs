//---------------------------------------------------------------------
// <copyright file="BatchReaderMixedEncodingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestMessage = Microsoft.Test.Taupo.OData.Common.TestMessage;
    using TestStream = Microsoft.Test.Taupo.OData.Common.TestStream;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of batch payloads 
    /// </summary>
    [TestClass, TestCase]
    public class BatchReaderMixedEncodingTests : ODataReaderTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestDescriptor.Settings PayloadReaderSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataRequestManager RequestManager { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadElementToJsonConverter PayloadElementToJsonConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadElementToXmlConverter PayloadElementToXmlConverter { get; set; }

        [InjectDependency(IsRequired = true)]
        public IODataUriToStringConverter UriConverter { get; set; }

        /// <summary>
        /// Generates the raw test message with mixed encodings defined by the test case, as well as the expected ODataPayloadElement.
        /// </summary>
        /// <param name="testCase">The test case defining the structure and encodings of the batch payload.</param>
        /// <param name="payload">The payload to use for all generated batch operations.</param>
        /// <param name="payloadUri">The URI to use for all generated batch operations.</param>
        /// <param name="isRequest">If true, generates a batch request, otherwise a batch response.</param>
        /// <returns>The test descriptor for this test case/configuration.</returns>
        private BatchReaderMixedEncodingTestDescriptor CreateTestDescriptor(BatchReaderMixedEncodingTestCase testCase, ODataPayloadElement payload, ODataUri payloadUri, bool isRequest)
        {
            ExceptionUtilities.Assert(testCase.BatchEncoding != null, "Batch encoding has not been specified.");

            string batchBoundary = "batch_" + Guid.NewGuid().ToString();
            string payloadUriString = this.UriConverter.ConvertToString(payloadUri);

            var batchPayload = isRequest ? (ODataPayloadElement)new BatchRequestPayload() : (ODataPayloadElement)new BatchResponsePayload();
            batchPayload.AddAnnotation(new BatchBoundaryAnnotation(batchBoundary));

            var rawMessage = new List<byte>();

            // TODO: Batch reader does not support multi codepoint encodings
            Encoding unsupportedEncoding = AsUnsupportedEncoding(testCase.BatchEncoding);

            foreach (var changeset in testCase.Changesets)
            {
                string changesetBoundary = "change_" + Guid.NewGuid().ToString();
                Encoding changesetEncoding = changeset.ChangesetEncoding ?? testCase.BatchEncoding;

                // TODO: Batch reader does not support multi codepoint encodings
                unsupportedEncoding = unsupportedEncoding ?? AsUnsupportedEncoding(changesetEncoding);

                string changesetContentType = HttpUtilities.BuildContentType(
                    MimeTypes.MultipartMixed,
                    changeset.ChangesetEncoding == null ? string.Empty : changeset.ChangesetEncoding.WebName,
                    changesetBoundary);

                rawMessage.AddRange(
                    WriteMessagePart(
                        testCase.BatchEncoding,
                        (writer) =>
                        {
                            writer.Write("--");
                            writer.WriteLine(batchBoundary);
                            writer.WriteLine(HttpHeaders.ContentType + ": " + changesetContentType);
                            writer.WriteLine();
                        }));

                var mimeParts = new List<IMimePart>();
                int contentId = 0;

                foreach (var operation in changeset.Operations)
                {
                    ExceptionUtilities.Assert(operation.PayloadFormat == ODataFormat.Json, "Payload format must be JSON.");
                    string formatType = MimeTypes.ApplicationAtomXml + ";type=entry";
                    Encoding payloadEncoding = operation.OperationEncoding ?? changesetEncoding;
                    string payloadContentType = HttpUtilities.BuildContentType(
                        formatType,
                        operation.OperationEncoding == null ? string.Empty : operation.OperationEncoding.WebName,
                        string.Empty);

                    string httpStatus = isRequest ? "POST " + payloadUriString + " HTTP/1.1" : "HTTP/1.1 201 Created";

                    rawMessage.AddRange(
                        WriteMessagePart(
                            changesetEncoding,
                            (writer) =>
                            {
                                writer.WriteLine();
                                writer.Write("--");
                                writer.WriteLine(changesetBoundary);
                                writer.WriteLine(HttpHeaders.ContentType + ": application/http");
                                writer.WriteLine(HttpHeaders.ContentTransferEncoding + ": binary");
                                writer.WriteLine(HttpHeaders.ContentId + ": " + (++contentId).ToString());
                                writer.WriteLine();
                                writer.WriteLine(httpStatus);
                                writer.WriteLine(HttpHeaders.ContentType + ": " + payloadContentType);
                                writer.WriteLine();
                            }));

                    IPayloadSerializer payloadSerializer = (IPayloadSerializer)new JsonPayloadSerializer(this.PayloadElementToJsonConverter.ConvertToJson);

                    byte[] payloadBytes = payloadSerializer.SerializeToBinary(payload, payloadEncoding.WebName);
                    rawMessage.AddRange(payloadBytes.Skip(payloadEncoding.GetPreamble().Length));

                    if (isRequest)
                    {
                        var request = this.RequestManager.BuildRequest(payloadUri, HttpVerb.Post, new Dictionary<string, string> { { HttpHeaders.ContentType, payloadContentType } });
                        request.Body = new ODataPayloadBody(payloadBytes, payload);
                        mimeParts.Add(request);
                    }
                    else
                    {
                        var httpResponseData = new HttpResponseData { StatusCode = HttpStatusCode.Created, };
                        httpResponseData.Headers.Add(HttpHeaders.ContentType, payloadContentType);
                        var response = new ODataResponse(httpResponseData) { Body = payloadBytes, RootElement = payload };
                        mimeParts.Add(response);
                    }
                }

                rawMessage.AddRange(
                    WriteMessagePart(
                        changesetEncoding,
                        (writer) =>
                        {
                            writer.WriteLine();
                            writer.Write("--");
                            writer.Write(changesetBoundary);
                            writer.WriteLine("--");
                        }));

                if (isRequest)
                {
                    ((BatchRequestPayload)batchPayload).Add(BatchPayloadBuilder.RequestChangeset(changesetBoundary, changesetEncoding.WebName, mimeParts.ToArray()));
                }
                else
                {
                    ((BatchResponsePayload)batchPayload).Add(BatchPayloadBuilder.ResponseChangeset(changesetBoundary, changesetEncoding.WebName, mimeParts.ToArray()));
                }
            }

            rawMessage.AddRange(
                WriteMessagePart(
                    testCase.BatchEncoding,
                    (writer) =>
                    {
                        writer.WriteLine();
                        writer.Write("--");
                        writer.Write(batchBoundary);
                        writer.WriteLine("--");
                    }));

            return new BatchReaderMixedEncodingTestDescriptor(this.PayloadReaderSettings)
            {
                BatchContentTypeHeader = HttpUtilities.BuildContentType(MimeTypes.MultipartMixed, testCase.BatchEncoding.WebName, batchBoundary),
                RawMessage = rawMessage.ToArray(),
                PayloadElement = batchPayload,
                ExpectedException = unsupportedEncoding == null ? null : ODataExpectedExceptions.ODataException("ODataBatchReaderStream_MultiByteEncodingsNotSupported", unsupportedEncoding.WebName)
            };
        }

        private static IEnumerable<byte> WriteMessagePart(Encoding encoding, Action<StreamWriter> writeAction)
        {
            using (var writer = new StreamWriter(new MemoryStream(), encoding))
            {
                writeAction(writer);
                writer.Flush();
                // Don't write any preamble that the StreamWriter may have introduced
                return ((MemoryStream)writer.BaseStream).ToArray().Skip(encoding.GetPreamble().Length);
            }
        }

        // TODO: Batch reader does not support multi codepoint encodings
        // We decided to not support multi-byte encodings other than UTF8 for now.
        private static Encoding AsUnsupportedEncoding(Encoding encoding)
        {
#if SILVERLIGHT || WINDOWS_PHONE
            if (string.CompareOrdinal(Encoding.UTF8.WebName, encoding.WebName) == 0)
#else
            if (encoding.IsSingleByte || Encoding.UTF8.CodePage == encoding.CodePage)
#endif
            {
                return null;
            }

            return encoding;
        }


        private class BatchReaderMixedEncodingTestCase
        {
            internal Encoding BatchEncoding { get; set; }
            internal BatchReaderMixedEncodingChangeset[] Changesets { get; set; }
        }

        private class BatchReaderMixedEncodingChangeset
        {
            internal Encoding ChangesetEncoding { get; set; }
            internal BatchReaderMixedEncodingOperation[] Operations { get; set; }
        }

        private class BatchReaderMixedEncodingOperation
        {
            internal Encoding OperationEncoding { get; set; }
            internal ODataFormat PayloadFormat { get; set; }
        }

        private class BatchReaderMixedEncodingTestDescriptor : PayloadReaderTestDescriptor
        {
            internal byte[] RawMessage { get; set; }

            internal string BatchContentTypeHeader { get; set; }

            internal BatchReaderMixedEncodingTestDescriptor(PayloadReaderTestDescriptor.Settings settings)
                : base(settings)
            {
            }

            protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
            {
                MemoryStream memoryStream = new MemoryStream(this.RawMessage);
                TestStream messageStream = new TestStream(memoryStream);
                if (testConfiguration.Synchronous)
                {
                    messageStream.FailAsynchronousCalls = true;
                }
                else
                {
                    messageStream.FailSynchronousCalls = true;
                }

                TestMessage testMessage = TestReaderUtils.CreateInputMessageFromStream(
                    messageStream,
                    testConfiguration,
                    this.PayloadElement.GetPayloadKindFromPayloadElement(),
                    this.BatchContentTypeHeader,
                    this.UrlResolver);

                return testMessage;
            }

            protected override string DumpInputMessageContent(ReaderTestConfiguration testConfiguration)
            {
                return Encoding.UTF8.GetString(this.RawMessage, 0, this.RawMessage.Length);
            }
        }
    }
}
