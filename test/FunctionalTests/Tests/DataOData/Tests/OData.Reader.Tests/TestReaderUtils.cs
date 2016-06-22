//---------------------------------------------------------------------
// <copyright file="TestReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.PayloadTransformation;
    using ODataConstants = Microsoft.OData.ODataConstants;

    #endregion Namespaces

    /// <summary>
    /// Utility methods for reader tests.
    /// </summary>
    public static class TestReaderUtils
    {
        /// <summary>
        /// List of all the <see cref="ODataPayloadKind"/> values; we can't use Enum.GetValues since this is not supported on all platforms.
        /// </summary>
        public static readonly ODataPayloadKind[] ODataPayloadKinds = new ODataPayloadKind[]
            {
                ODataPayloadKind.ResourceSet,
                ODataPayloadKind.Resource,
                ODataPayloadKind.Property,
                ODataPayloadKind.EntityReferenceLink,
                ODataPayloadKind.EntityReferenceLinks,
                ODataPayloadKind.Value,
                ODataPayloadKind.BinaryValue,
                ODataPayloadKind.Collection,
                ODataPayloadKind.ServiceDocument,
                ODataPayloadKind.MetadataDocument,
                ODataPayloadKind.Error,
                ODataPayloadKind.Batch,
                ODataPayloadKind.Parameter,
                ODataPayloadKind.IndividualProperty,
                ODataPayloadKind.Delta,
                ODataPayloadKind.Asynchronous,
                ODataPayloadKind.Unsupported,
            };

        /// <summary>
        /// List of all behavior kinds.
        /// </summary>
        public static TestODataBehaviorKind[] ODataBehaviorKinds = new[] { TestODataBehaviorKind.Default, TestODataBehaviorKind.WcfDataServicesClient, TestODataBehaviorKind.WcfDataServicesServer };

        /// <summary>
        /// Returns a text description of message reader settings.
        /// </summary>
        /// <param name="messageReaderSettings">The reader settings to get text description of.</param>
        /// <returns>Humanly readable description of <paramref name="messageReaderSettings"/>, used for debugging.</returns>
        public static string ToDebugString(this ODataMessageReaderSettings messageReaderSettings)
        {
            return string.Format("EnablePrimitiveTypeConversion: {0}, EnableCharactersCheck: {1}, EnableMessageStreamDisposal: {2}",
                messageReaderSettings.EnablePrimitiveTypeConversion,
                messageReaderSettings.EnableCharactersCheck,
                messageReaderSettings.EnableMessageStreamDisposal);
        }

        /// <summary>
        /// Helper method to create a test message from its content.
        /// </summary>
        /// <param name="messageContent">Stream with the content of the message.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>Newly created test message.</returns>
        public static TestMessage CreateInputMessageFromStream(TestStream messageContent, ReaderTestConfiguration testConfiguration)
        {
            return CreateInputMessageFromStream(messageContent, testConfiguration, null, null, null);
        }

        /// <summary>
        /// Helper method to create a test message from its content.
        /// </summary>
        /// <param name="messageContent">Stream with the content of the message.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <param name="payloadKind">The payload kind to use to compute and set the content type header; or null if no content type header should be set.</param>
        /// <param name="customContentTypeHeader">A custom content type header to be used in the message.</param>
        /// <param name="urlResolver">Url resolver to add to the test message created.</param>
        /// <returns>Newly created test message.</returns>
        public static TestMessage CreateInputMessageFromStream(
            TestStream messageContent,
            ReaderTestConfiguration testConfiguration,
            ODataPayloadKind? payloadKind,
            string customContentTypeHeader,
            IODataPayloadUriConverter urlResolver)
        {
            TestMessage message;
            if (testConfiguration.IsRequest)
            {
                if (urlResolver != null)
                {
                    message = new TestRequestMessageWithUrlResolver(messageContent, urlResolver, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
                else
                {
                    message = new TestRequestMessage(messageContent, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
            }
            else
            {
                if (urlResolver != null)
                {
                    message = new TestResponseMessageWithUrlResolver(messageContent, urlResolver, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
                else
                {
                    message = new TestResponseMessage(messageContent, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
            }

            // set the OData-Version header
            message.SetHeader(ODataConstants.ODataVersionHeader, testConfiguration.Version.ToText());

            if (customContentTypeHeader != null)
            {
                message.SetHeader(ODataConstants.ContentTypeHeader, customContentTypeHeader);
            }
            else if (payloadKind.HasValue)
            {
                message.SetContentType(testConfiguration.Format, payloadKind.Value);
            }

            return message;
        }

        /// <summary>
        /// Helper method to create <see cref="ODataMessageReader"/> instance.
        /// </summary>
        /// <param name="message">The test message to use.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>The test wrapper for the newly created message reader.</returns>
        public static ODataMessageReaderTestWrapper CreateMessageReader(
            TestMessage message,
            IEdmModel model,
            ReaderTestConfiguration testConfiguration)
        {
            ODataMessageReader messageReader;
            if (testConfiguration.IsRequest)
            {
                TestRequestMessage requestMessage = (TestRequestMessage)message;
                messageReader = new ODataMessageReader(requestMessage, testConfiguration.MessageReaderSettings, model);
            }
            else
            {
                TestResponseMessage responseMessage = (TestResponseMessage)message;
                messageReader = new ODataMessageReader(responseMessage, testConfiguration.MessageReaderSettings, model);
            }

            return new ODataMessageReaderTestWrapper(messageReader, testConfiguration.MessageReaderSettings, testConfiguration, message);
        }

        /// <summary>
        /// Returns the payload to be used for this test case and the specified test configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <returns>The payload to use for testing.</returns>
        public static byte[] GetPayload(
            ReaderTestConfiguration testConfiguration,
            List<Func<ReaderTestConfiguration, Func<ODataPayloadElement, ODataPayloadElement>>> payloadNormalizers,
            PayloadReaderTestDescriptor.Settings settings,
            ODataPayloadElement payloadElement)
        {
            IPayloadSerializer payloadSerializer = null;

            // Apply all payload element transforms before serialization.
            IPayloadTransform<ODataPayloadElement> payloadElementTransform = settings.PayloadTransformFactory.GetTransform<ODataPayloadElement>();
            ODataPayloadElement transformedODataElement = null;
            if (payloadElementTransform.TryTransform(payloadElement, out transformedODataElement))
            {
                payloadElement = transformedODataElement;
            }

            ODataPayloadElement payloadElementToSerialize = payloadElement;

            // Apply all normalizers/fixups before serialization
            if (payloadNormalizers != null)
            {
                ODataPayloadElement payloadElementCopy = null;
                foreach (var getPayloadNormalizerFunc in payloadNormalizers)
                {
                    var normalizer = getPayloadNormalizerFunc(testConfiguration);
                    if (normalizer != null)
                    {
                        if (payloadElementCopy == null)
                        {
                            payloadElementCopy = payloadElementToSerialize.DeepCopy();
                        }

                        payloadElementCopy = normalizer(payloadElementCopy);
                    }
                }

                payloadElementToSerialize = payloadElementCopy ?? payloadElementToSerialize;
            }

            if (testConfiguration.Format == ODataFormat.Json)
            {
                // Create a copy of the payload element so that we can add annotations to it.
                payloadElementToSerialize = payloadElementToSerialize.DeepCopy();

                // Annotate elements with version and response/request as appropriate
                PayloadFormatVersionAnnotatingVisitor.AnnotateJsonLight(
                    payloadElementToSerialize,
                    testConfiguration.Version.ToDataServiceProtocolVersion(),
                    testConfiguration.IsRequest);

                payloadSerializer = new JsonPayloadSerializer(settings.PayloadElementToJsonLightConverter.ConvertToJsonLight);
            }
            else if (testConfiguration.Format == null)
            {
                if (payloadElementToSerialize.ElementType == ODataPayloadElementType.PrimitiveValue)
                {
                    PrimitiveValue primitiveValue = (PrimitiveValue)payloadElementToSerialize;
                    if (primitiveValue.ClrValue == null)
                    {
                        throw new NotSupportedException("Reading null values is not supported (since we don't support writing null values).");
                    }
                    else if (primitiveValue.ClrValue.GetType() == typeof(byte[]))
                    {
                        payloadSerializer = settings.BinaryValuePayloadElementConverter;
                    }
                    else
                    {
                        payloadSerializer = settings.TextValuePayloadElementConverter;
                    }
                }
                else if (payloadElementToSerialize.ElementType == ODataPayloadElementType.BatchRequestPayload || payloadElementToSerialize.ElementType == ODataPayloadElementType.BatchResponsePayload)
                {
                    return SerializeBatchPayload(payloadElementToSerialize, settings);
                }
                else
                {
                    throw new NotImplementedException("Default format not yet implemented for payload test descriptor and payload element type '" + payloadElementToSerialize.ElementType + "'.");
                }
            }
            else
            {
                throw new NotSupportedException("Unexpected format.");
            }

            // Default encoding is UTF8
            return payloadSerializer.SerializeToBinary(payloadElementToSerialize, null);
        }

        /// <summary>
        /// Returns the serialized batch payload 
        /// </summary>
        /// <param name="batchPayload">batch payload</param>
        /// <returns>bytes representing batch payload</returns>
        private static byte[] SerializeBatchPayload(ODataPayloadElement batchPayload, PayloadReaderTestDescriptor.Settings settings)
        {
            string boundary = null;
            Byte[] bytes = null;

            Func<ODataPayloadElement, string> getBoundaryAnnotation = (batchPayloadElement) =>
            {
                var boundaryAnn = batchPayloadElement.Annotations.OfType<BatchBoundaryAnnotation>().Single();
                ExceptionUtilities.CheckObjectNotNull(boundaryAnn, "bounday annotation cannot be null");
                return boundaryAnn.BatchBoundaryInPayload;
            };

            var batchRequestPayload = batchPayload as BatchRequestPayload;
            if (batchRequestPayload != null)
            {
                boundary = getBoundaryAnnotation(batchRequestPayload);
                bytes = settings.BatchSerializer.SerializeBatchPayload(batchRequestPayload, boundary, Encoding.UTF8.WebName); // encoding assumed to be UTF8
            }
            else
            {
                var batchResponsePayload = batchPayload as BatchResponsePayload;
                boundary = getBoundaryAnnotation(batchResponsePayload);
                ExceptionUtilities.CheckObjectNotNull(batchResponsePayload, "the specified batch payload is neither a request payload nor a response");
                bytes = settings.BatchSerializer.SerializeBatchPayload(batchResponsePayload, boundary, Encoding.UTF8.WebName); // encoding assumed to be UTF8
            }

            return bytes;
        }

        /// <summary>
        /// Creates the input message for the test descriptor
        /// </summary>
        /// <param name="testConfiguration">the test configuration to use</param>
        /// <param name="readerTestDescriptor">The test descriptor</param>
        /// <param name="settings">The test descriptor settings</param>
        /// <param name="applyPayloadTransformations">Whether or not to apply payload transformations</param>
        /// <returns>The message for the test</returns>
        public static TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration,
            PayloadReaderTestDescriptor readerTestDescriptor,
            PayloadReaderTestDescriptor.Settings settings,
            bool? applyPayloadTransformations)
        {
            TestMessage testMessage;
            bool originalApplyTransformValue = false;
            var odataTransformFactory = settings.PayloadTransformFactory as ODataLibPayloadTransformFactory;

            try
            {
                if (applyPayloadTransformations.HasValue && odataTransformFactory != null)
                {
                    originalApplyTransformValue = odataTransformFactory.ApplyTransform;
                    odataTransformFactory.ApplyTransform = applyPayloadTransformations.Value;
                }

                if (readerTestDescriptor.TestDescriptorNormalizers != null)
                {
                    foreach (var testDescriptorNormalizer in readerTestDescriptor.TestDescriptorNormalizers)
                    {
                        var normalizerAction = testDescriptorNormalizer(testConfiguration);
                        if (normalizerAction != null)
                        {
                            normalizerAction(readerTestDescriptor);
                        }
                    }
                }

                MemoryStream memoryStream = new MemoryStream(GetPayload(testConfiguration, readerTestDescriptor.PayloadNormalizers, settings, readerTestDescriptor.PayloadElement));
                TestStream messageStream = new TestStream(memoryStream);
                if (testConfiguration.Synchronous)
                {
                    messageStream.FailAsynchronousCalls = true;
                }
                else
                {
                    messageStream.FailSynchronousCalls = true;
                }

                testMessage = TestReaderUtils.CreateInputMessageFromStream(
                    messageStream,
                    testConfiguration,
                    readerTestDescriptor.PayloadElement.GetPayloadKindFromPayloadElement(),
                    readerTestDescriptor.PayloadElement.GetCustomContentTypeHeader(), readerTestDescriptor.UrlResolver);

                if (readerTestDescriptor.TestMessageWrapper != null)
                {
                    testMessage = readerTestDescriptor.TestMessageWrapper(testMessage);
                }

                return testMessage;
            }
            finally
            {
                if (applyPayloadTransformations.HasValue && odataTransformFactory != null)
                {
                    odataTransformFactory.ApplyTransform = originalApplyTransformValue;
                }
            }
        }
    }
}
