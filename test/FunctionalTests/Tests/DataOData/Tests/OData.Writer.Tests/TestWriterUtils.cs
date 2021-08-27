//---------------------------------------------------------------------
// <copyright file="TestWriterUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Json;
    using Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine;
    #endregion Namespaces

    /// <summary>
    /// Utility methods for creating and using OData writers.
    /// </summary>
    public static class TestWriterUtils
    {
        /// <summary>
        /// All permutations of invalid settings where one selector is set to invalid.
        /// </summary>
        /// <remarks>
        /// We currently support the following invalid behaviors:
        /// array[0] - GetStreamAsync() fails
        /// array[1] - stream is closed after creation
        /// </remarks>
        public static IEnumerable<bool[]> InvalidSettingSelectors
        {
            get
            {
                bool[] invalidSettingSelector = new bool[] { true, false };
                return invalidSettingSelector.Permutations();
            }
        }

        /// <summary>
        /// List of all behavior kinds.
        /// </summary>
        public static TestODataBehaviorKind[] ODataBehaviorKinds = new[] { TestODataBehaviorKind.Default, TestODataBehaviorKind.WcfDataServicesClient, TestODataBehaviorKind.WcfDataServicesServer };

        /// <summary>
        /// Creates an ODataWriter for the specified format and the specified version and
        /// writes the payload in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        public static void WriteAndVerifyODataEdmPayload<T>(PayloadWriterTestDescriptor<T> descriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert, BaselineLogger baselineLogger)
        {
            IEdmEntitySet edmEntitySet = null;
            IEdmEntityType edmEntityType = null;

            EdmEntitySet modelEntitySet = descriptor.PayloadEdmElementContainer as EdmEntitySet;
            EdmEntityType modelEntityType = descriptor.PayloadEdmElementType as EdmEntityType;
            if (modelEntitySet != null || modelEntityType != null)
            {
                IEdmModel edmModel = descriptor.GetMetadataProvider();
                if (modelEntitySet != null)
                {
                    edmEntitySet = edmModel.FindEntityContainer(modelEntitySet.Container.FullName()).FindEntitySet(modelEntitySet.Name);
                }

                if (modelEntityType != null)
                {
                    edmEntityType = (IEdmEntityType)edmModel.FindType(modelEntityType.FullName());
                }
            }

            TestWriterUtils.WriteActionAndVerifyODataPayload<T>(
                (messageWriter, writerDescriptor, feedWriter) =>
                {
                    ODataWriter writer = messageWriter.CreateODataWriter(feedWriter, edmEntitySet, edmEntityType);
                    WritePayload(messageWriter, writer, true, writerDescriptor.PayloadItems, writerDescriptor.ThrowUserExceptionAt);
                },
                descriptor,
                testConfiguration,
                assert,
                baselineLogger);
        }

        /// <summary>
        /// Creates an ODataWriter for the specified format and the specified version and
        /// writes the payload in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        public static void WriteAndVerifyODataPayload<T>(PayloadWriterTestDescriptor<T> descriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert, BaselineLogger baselineLogger)
        {
            IEdmEntitySet edmEntitySet = descriptor.PayloadEdmElementContainer as IEdmEntitySet;
            IEdmEntityType edmEntityType = descriptor.PayloadEdmElementType as IEdmEntityType;

            TestWriterUtils.WriteActionAndVerifyODataPayload<T>(
                (messageWriter, writerDescriptor, feedWriter) =>
                {
                    ODataWriter writer = messageWriter.CreateODataWriter(feedWriter, edmEntitySet, edmEntityType);
                    WritePayload(messageWriter, writer, true, writerDescriptor.PayloadItems, writerDescriptor.ThrowUserExceptionAt);
                },
                descriptor,
                testConfiguration,
                assert,
                baselineLogger);
        }

        /// <summary>
        /// Performs the specified action to write the payload in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <typeparam name="TPayloadItem">Payload item type.</typeparam>
        /// <param name="writeAction">Action to write the payload.</param>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        internal static void WriteActionAndVerifyODataPayload<TPayloadItem>(Action<ODataMessageWriterTestWrapper, PayloadWriterTestDescriptor<TPayloadItem>, bool> writeAction, PayloadWriterTestDescriptor<TPayloadItem> descriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert, BaselineLogger baselineLogger)
        {
            if (descriptor.SkipTestConfiguration != null && descriptor.SkipTestConfiguration(testConfiguration))
            {
                return;
            }

            baselineLogger.LogConfiguration(testConfiguration);
            baselineLogger.LogModelPresence(descriptor.Model);

            // serialize to a memory stream
            using (var memoryStream = new MemoryStream())
            using (var testMemoryStream = CreateTestStream(testConfiguration, memoryStream, ignoreDispose: true))
            {
                bool feedWriter = descriptor.PayloadItems[0] is ODataResourceSet;
                TestMessage testMessage = null;
                Exception exception = TestExceptionUtils.RunCatching(() =>
                {
                    using (var messageWriter = CreateMessageWriter(testMemoryStream, testConfiguration, assert, out testMessage, null, descriptor.GetMetadataProvider(), descriptor.UrlResolver))
                    {
                        writeAction(messageWriter, descriptor, feedWriter);
                    }
                });
                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
                WriterTestExpectedResults expectedResults = descriptor.ExpectedResultCallback(testConfiguration);
                ValidateExceptionOrLogResult(testMessage, testConfiguration, expectedResults, exception, assert, descriptor.TestDescriptorSettings.ExpectedResultSettings.ExceptionVerifier, baselineLogger);

                if (exception == null)
                {
                    ValidateContentType(testMessage, expectedResults, true, assert);
                }
            }
        }

        /// <summary>
        /// Creates an ODataWriter for the specified format and the specified version and
        /// writes the parameter payload in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        /// <param name="functionImport">Function import whose parameters are to be written.</param>
        internal static void WriteAndVerifyODataParameterPayload(PayloadWriterTestDescriptor<ODataParameters> descriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert, BaselineLogger baselineLogger, IEdmOperationImport functionImport = null)
        {
            TestWriterUtils.WriteActionAndVerifyODataPayload<ODataParameters>(
                (messageWriter, writerDescriptor, feedWriter) =>
                {
                    writerDescriptor.TestDescriptorSettings.ObjectModelToMessageWriter.WriteMessage(
                        messageWriter,
                        ODataPayloadKind.Parameter,
                        writerDescriptor.PayloadItems.First(),
                        /*model*/ null,
                        functionImport);
                },
                descriptor,
                testConfiguration,
                assert,
                baselineLogger);
        }

        /// <summary>
        /// Creates an ODataWriter for the specified format and the specified version and
        /// writes the payload in the descriptor to an in-memory stream. It then parses
        /// the written Xml and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        internal static void WriteAndVerifyODataPayloadElement(PayloadWriterTestDescriptor<ODataPayloadElement> descriptor, WriterTestConfiguration testConfiguration, AssertionHandler assert, IPayloadElementODataWriter payloadElementWriter, IODataPayloadElementComparer comparer, IXmlToPayloadElementConverter xmlConverter)
        {
            bool feedWriter = descriptor.PayloadItems[0].ElementType == ODataPayloadElementType.EntitySetInstance;

            // serialize to a memory stream
            using (var memoryStream = CreateTestStream(testConfiguration))
            using (var messageWriter = CreateMessageWriter(memoryStream, testConfiguration, assert, null, descriptor.Model))
            {
                ODataWriter writer = messageWriter.CreateODataWriter(feedWriter);
                assert.IsTrue(descriptor.PayloadItems.Count == 1, "there should be exactly one payloadElement");
                Exception exception = TestExceptionUtils.RunCatching(() => payloadElementWriter.WritePayload(writer, descriptor.PayloadItems.First()));
                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
                if (exception != null)
                {
                    assert.IsNull(exception, "Unexpected exception: " + exception.ToString());
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                XElement rootElement = memoryStream.ToXElement();
                ODataPayloadElement newPayload = xmlConverter.ConvertToPayloadElement(rootElement);
                EntityInstance observedEntity = newPayload as EntityInstance;
                assert.IsNotNull(observedEntity, "ObservedEntity cannot be null");
                comparer.Compare(descriptor.PayloadItems.First(), newPayload);
            }
        }

        /// <summary>
        /// Creates an ODataMessageWriter for the specified format and the specified version and
        /// calls the <param name="writerAction" /> to write a payload to an in-memory stream. It then parses
        /// the written Xml/JSON and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="writerAction">An action to write a payload to the in-memory stream.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="settings">The ODataMessageWriterSettings instance.</param>
        internal static void WriteAndVerifyTopLevelContent(
            PayloadWriterTestDescriptor descriptor,
            WriterTestConfiguration testConfiguration,
            Action<ODataMessageWriterTestWrapper> writerAction,
            AssertionHandler assert,
            ODataMessageWriterSettings settings = null,
            BaselineLogger baselineLogger = null)
        {
            baselineLogger.LogConfiguration(testConfiguration);
            baselineLogger.LogModelPresence(descriptor.Model);
            // serialize to a memory stream
            using (var memoryStream = new MemoryStream())
            using (var testMemoryStream = CreateTestStream(testConfiguration, memoryStream, ignoreDispose: true))
            {
                TestMessage testMessage;
                Exception exception;
                using (ODataMessageWriterTestWrapper messageWriter = CreateMessageWriter(testMemoryStream, testConfiguration, assert, out testMessage, settings, descriptor.GetMetadataProvider()))
                {
                    exception = TestExceptionUtils.RunCatching(() => writerAction(messageWriter));
                }

                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
                WriterTestExpectedResults expectedResults = descriptor.ExpectedResultCallback(testConfiguration);
                ValidateExceptionOrLogResult(testMessage, testConfiguration, expectedResults, exception, assert, descriptor.TestDescriptorSettings.ExpectedResultSettings.ExceptionVerifier, baselineLogger);
                ValidateContentType(testMessage, expectedResults, true, assert);
            }
        }

        /// <summary>
        /// Creates an ODataMessageWriter for the specified format and the specified version and writer a top-level property payload.
        /// It then parses the written Xml/JSON and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="alwaysSpecifyOwningContainer">If true the owning container (type, ...) will be used always if specified by the test,
        /// otherwise it will only be used in formats which require it.</param>
        internal static void RunTopLevelPropertyPayload<T>(
            this PayloadWriterTestDescriptor<T> testDescriptor,
            WriterTestConfiguration testConfiguration,
            bool alwaysSpecifyOwningContainer = false,
            BaselineLogger baselineLogger = null, 
            Action<ODataMessageWriterTestWrapper> writeAction = null)
        {
            T property = testDescriptor.PayloadItems.Single();

            WriteAndVerifyTopLevelContent(
                testDescriptor,
                testConfiguration,
                writeAction ?? ((messageWriter) => messageWriter.WriteProperty(property as ODataProperty)),
                testDescriptor.TestDescriptorSettings.Assert,
                testConfiguration.MessageWriterSettings,
                baselineLogger: baselineLogger);
        }

        /// <summary>
        /// Creates an ODataMessageWriter for the specified format and the specified version and
        /// calls the <param name="writerAction" /> to write a payload to an in-memory stream. It then parses
        /// the written Xml/JSON and compares it to the expected result as specified in the descriptor.
        /// </summary>
        /// <param name="descriptor">The test descriptor to process.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="writerAction">An action to write a payload to the in-memory stream.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        internal static void WriteAndVerifyRawContent(
            PayloadWriterTestDescriptor<object> descriptor,
            WriterTestConfiguration testConfiguration,
            AssertionHandler assert,
            BaselineLogger baselineLogger)
        {
            baselineLogger.LogConfiguration(testConfiguration);
            baselineLogger.LogModelPresence(descriptor.Model);

            // serialize to a memory stream
            using (var memoryStream = new MemoryStream())
            using (var testMemoryStream = new TestStream(memoryStream, ignoreDispose: true))
            {
                TestMessage testMessage;
                Exception exception;
                using (ODataMessageWriterTestWrapper messageWriter = CreateMessageWriter(testMemoryStream, testConfiguration, assert, out testMessage, null, descriptor.Model))
                {
                    exception = TestExceptionUtils.RunCatching(() => messageWriter.WriteValue(descriptor.PayloadItems.Single()));
                }

                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);
                WriterTestExpectedResults expectedResults = descriptor.ExpectedResultCallback(testConfiguration);
                ValidateExceptionOrLogResult(testMessage, testConfiguration, expectedResults, exception, assert, descriptor.TestDescriptorSettings.ExpectedResultSettings.ExceptionVerifier, baselineLogger);
                ValidateContentType(testMessage, expectedResults, true, assert);
            }
        }

        /// <summary>
        /// Creates an ODataMessageWriter instance for the specified format and protocol version.
        /// </summary>
        /// <param name="stream">The underlying stream to create the writer over.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <param name="assert">Assertion handler for the test.</param>
        /// <param name="settings">The settings to use.</param>
        /// <param name="model">The model.</param>
        /// <param name="skipMessageStreamDisposalValidation">Set true to skip validation on whether the message stream was disposed.</param>
        /// <returns>The <see cref="ODataMessageWriter"/> created for the given stream and configuration.</returns>
        internal static ODataMessageWriterTestWrapper CreateMessageWriter(
            Stream stream,
            WriterTestConfiguration testConfiguration,
            AssertionHandler assert,
            ODataMessageWriterSettings settings = null,
            IEdmModel model = null)
        {
            TestMessage message;
            return CreateMessageWriter(stream, testConfiguration, assert, out message, settings, model);
        }

        /// <summary>
        /// Creates an ODataMessageWriter instance for the specified format and protocol version.
        /// </summary>
        /// <param name="stream">The underlying stream to create the writer over.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        /// <param name="assert">Assertion handler for the test.</param>
        /// <param name="testMessage">Returns the test wrapper for an OData request or response message.</param>
        /// <param name="settings">The settings to use.</param>
        /// <param name="model">The model.</param>
        /// <param name="urlResolver">The URL resolver to use for the messages, if null the message will not implement that interface.</param>
        /// <returns>The <see cref="ODataMessageWriter"/> created for the given stream and configuration.</returns>
        internal static ODataMessageWriterTestWrapper CreateMessageWriter(
            Stream stream,
            WriterTestConfiguration testConfiguration,
            AssertionHandler assert,
            out TestMessage testMessage,
            ODataMessageWriterSettings settings = null,
            IEdmModel model = null,
            IODataPayloadUriConverter urlResolver = null)
        {
            testMessage = CreateOutputMessageFromStream(stream, testConfiguration, null, null, urlResolver);
            return CreateMessageWriter(testMessage, model, testConfiguration, assert, settings);
        }

        /// <summary>
        /// Helper method to create <see cref="ODataMessageWriter"/> instance.
        /// </summary>
        /// <param name="message">The test message to use.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <param name="assert">The assertion handler to use.</param>
        /// <returns>The test wrapper for the newly created message writer.</returns>
        public static ODataMessageWriterTestWrapper CreateMessageWriter(
            TestMessage message,
            IEdmModel model,
            WriterTestConfiguration testConfiguration,
            AssertionHandler assert,
            ODataMessageWriterSettings settings = null)
        {
            ODataMessageWriterSettings messageWriterSettings = settings ?? testConfiguration.MessageWriterSettings;

            ODataMessageWriter messageWriter;

            if (testConfiguration.IsRequest)
            {
                TestRequestMessage requestMessage = (TestRequestMessage)message;
                messageWriter = new ODataMessageWriter(requestMessage, messageWriterSettings, model);
            }
            else
            {
                TestResponseMessage responseMessage = (TestResponseMessage)message;
                messageWriter = new ODataMessageWriter(responseMessage, messageWriterSettings, model);
            }

            return new ODataMessageWriterTestWrapper(messageWriter, testConfiguration, message, assert);
        }

        /// <summary>
        /// Creates a stream for a test message based on the specified configuration.
        /// </summary>
        /// <param name="testConfiguration">The test configuration to use for the stream.</param>
        /// <param name="innerStream">The inner stream to wrap the test stream around. If this is null a new MemoryStream will be used.</param>
        /// <param name="ignoreDispose">If the test stream should ignore dispose calls.</param>
        /// <returns>The newly created test stream.</returns>
        internal static TestStream CreateTestStream(WriterTestConfiguration testConfiguration, Stream innerStream = null, bool ignoreDispose = false)
        {
            TestStream stream = new TestStream(innerStream == null ? new MemoryStream() : innerStream, ignoreDispose);
            if (testConfiguration.Synchronous)
            {
                stream.FailAsynchronousCalls = true;
            }
            else
            {
                stream.FailSynchronousCalls = true;
            }

            return stream;
        }

        /// <summary>
        /// Helper method to create a test message to write to.
        /// </summary>
        /// <param name="messageContent">Stream to write the content to.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <returns>Newly created test message.</returns>
        internal static TestMessage CreateOutputMessageFromStream(Stream messageContent, WriterTestConfiguration testConfiguration)
        {
            return CreateOutputMessageFromStream(messageContent, testConfiguration, null, null, null);
        }

        /// <summary>
        /// Helper method to create a test message to write to.
        /// </summary>
        /// <param name="messageContent">Stream to write to.</param>
        /// <param name="testConfiguration">The test configuration.</param>
        /// <param name="payloadKind">The payload kind to use to compute and set the content type header; or null if no content type header should be set.</param>
        /// <param name="urlResolver">The URL resolver to use for the messages, if null the message will not implement that interface.</param>
        /// <returns>Newly created test message.</returns>
        public static TestMessage CreateOutputMessageFromStream(
            Stream messageContent,
            WriterTestConfiguration testConfiguration,
            ODataPayloadKind? payloadKind,
            string customContentTypeHeader,
            IODataPayloadUriConverter urlResolver)
        {
            TestMessage message;

            if (testConfiguration.IsRequest)
            {
                TestRequestMessage requestMessage;
                if (urlResolver != null)
                {
                    requestMessage = new TestRequestMessageWithUrlResolver(messageContent, urlResolver, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
                else
                {
                    requestMessage = new TestRequestMessage(messageContent, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }

                // TODO: do we need to set the method and request Url properties?
                message = requestMessage;
            }
            else
            {
                TestResponseMessage responseMessage;
                if (urlResolver != null)
                {
                    responseMessage = new TestResponseMessageWithUrlResolver(messageContent, urlResolver, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }
                else
                {
                    responseMessage = new TestResponseMessage(messageContent, testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous);
                }

                responseMessage.StatusCode = 200;
                message = responseMessage;
            }

            // set the OData-Version header
            message.SetHeader(Microsoft.OData.ODataConstants.ODataVersionHeader, testConfiguration.Version.ToText());
            if (customContentTypeHeader != null)
            {
                message.SetHeader(Microsoft.OData.ODataConstants.ContentTypeHeader, customContentTypeHeader);
            }
            else if (payloadKind.HasValue)
            {
                message.SetContentType(testConfiguration.Format, payloadKind.Value);
            }

            return message;
        }

        /// <summary>
        /// Writes a list of OData payload items using the specified writer.
        /// </summary>
        /// <param name="messageWriter">The test wrapper for ODataMessageWriter to use.</param>
        /// <param name="writer">The OData writer to use.</param>
        /// <param name="flush">A boolean flag indicating whether to flush the writer after writing.</param>
        /// <param name="items">The payload items to write.</param>
        internal static void WritePayload<T>(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, bool flush, params T[] items)
        {
            WritePayload(messageWriter, writer, flush, (IList<T>)items, /*throwAtUserException*/-1);
        }

        /// <summary>
        /// Writes a list of OData payload items using the specified writer.
        /// </summary>
        /// <param name="messageWriter">The test wrapper for ODataMessageWriter to use.</param>
        /// <param name="writer">The OData writer to use.</param>
        /// <param name="flush">A boolean flag indicating whether to flush the writer after writing.</param>
        /// <param name="item">The payload items to write.</param>
        internal static void WritePayload<T>(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, bool flush, T item)
        {
            Debug.Assert(messageWriter != null, "messageWriter != null");
            Debug.Assert(item != null, "item != null");
            Debug.Assert(writer != null || item is ODataAnnotatedError, "Writer must have a value unless writing an error");

            try
            {
                var feed = item as ODataResourceSet;
                if (feed != null)
                {
                    WritePayload(messageWriter, writer, feed);
                }

                var entry = item as ODataResource;
                if (entry != null)
                {
                    WritePayload(messageWriter, writer, entry);
                }

                var navLink = item as ODataNestedResourceInfo;
                if (navLink != null)
                {
                    WritePayload(messageWriter, writer, navLink);
                }

                var error = item as ODataAnnotatedError;
                if (error != null)
                {
                    WritePayload(messageWriter, error);
                }
            }
            finally
            {
                // Flush always since this flush will be sync/async as per test configuration
                // while the auto-flush in Dispose will be always sync, which might cause our test streams to fail
                // since those are set to not support sync operations when async test config is in use.
                if (flush && writer != null)
                {
                    writer.Flush();
                }
            }
        }

        private static void WritePayload(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, ODataResource entry)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer");
            ExceptionUtilities.CheckArgumentNotNull(entry, "entry");

            WriteEntryCallbacksAnnotation callbacksAnnotation = entry.GetAnnotation<WriteEntryCallbacksAnnotation>();
            if (callbacksAnnotation != null && callbacksAnnotation.BeforeWriteStartCallback != null)
            {
                callbacksAnnotation.BeforeWriteStartCallback(entry);
            }

            if (entry.IsNullEntry())
            {
                writer.WriteStart((ODataResource)null);
            }
            else
            {
                writer.WriteStart(entry);
            }

            //Write Navigation Links
            var expandedLinksAnnotation = entry.GetAnnotation<ODataEntryNavigationLinksObjectModelAnnotation>();
            ODataNestedResourceInfo link = null;
            if (expandedLinksAnnotation != null)
            {
                for (int i = 0; i < expandedLinksAnnotation.Count; ++i)
                {
                    bool linkExists = expandedLinksAnnotation.TryGetNavigationLinkAt(i, out link);
                    ExceptionUtilities.Assert(linkExists, "Links for writing must be numbered sequentially");
                    WritePayload(messageWriter, writer, link);
                }
            }

            writer.WriteEnd();
        }

        private static void WritePayload(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, ODataNestedResourceInfo link)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer");
            ExceptionUtilities.CheckArgumentNotNull(link, "link");

            writer.WriteStart(link);
            var expandedLinksAnnotation = link.GetAnnotation<ODataNavigationLinkExpandedItemObjectModelAnnotation>();
            if (expandedLinksAnnotation != null && expandedLinksAnnotation.ExpandedItem != null)
            {
                List<ODataItem> items = expandedLinksAnnotation.ExpandedItem as List<ODataItem>;
                if (items != null)
                {
                    foreach (ODataItem item in items)
                    {
                        WritePayload(messageWriter, writer, false, item);
                    }
                }
                else
                {
                    WritePayload(messageWriter, writer, false, (ODataItem)expandedLinksAnnotation.ExpandedItem);
                }
            }

            writer.WriteEnd();
        }

        private static void WritePayload(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, ODataResourceSet feed)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer");
            ExceptionUtilities.CheckArgumentNotNull(feed, "feed");

            WriteFeedCallbacksAnnotation callbacksAnnotation = feed.GetAnnotation<WriteFeedCallbacksAnnotation>();
            if (callbacksAnnotation != null && callbacksAnnotation.BeforeWriteStartCallback != null)
            {
                callbacksAnnotation.BeforeWriteStartCallback(feed);
            }

            writer.WriteStart(feed);

            if (callbacksAnnotation != null && callbacksAnnotation.AfterWriteStartCallback != null)
            {
                callbacksAnnotation.AfterWriteStartCallback(feed);
            }

            var entries = feed.GetAnnotation<ODataFeedEntriesObjectModelAnnotation>();
            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    WritePayload(messageWriter, writer, false, entry);
                }
            }

            writer.WriteEnd();
        }

        private static void WritePayload(ODataMessageWriterTestWrapper messageWriter, ODataAnnotatedError error)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(error, "error");

            messageWriter.WriteError(error.Error, error.IncludeDebugInformation);
        }

        /// <summary>
        /// Writes a list of OData payload items using the specified writer.
        /// </summary>
        /// <param name="messageWriter">The test wrapper for ODataMessageWriter to use.</param>
        /// <param name="writer">The OData writer to use.</param>
        /// <param name="flush">A boolean flag indicating whether to flush the writer after writing.</param>
        /// <param name="items">The payload items to write.</param>
        /// <remarks>
        ///     The list of <paramref name="items"/> is interpreted in the following way: 
        ///     for every non-null item we call WriteStart; for every null item we call WriteEnd.
        ///     null items can be omitted at the end of the list, e.g., a list of a feed and an entry 
        ///     item would be 'auto-completed' with two null entries.
        /// </remarks>
        internal static void WritePayload<T>(ODataMessageWriterTestWrapper messageWriter, ODataWriter writer, bool flush, IList<T> items, int throwUserExceptionAt = -1)
        {
            Debug.Assert(messageWriter != null, "messageWriter != null");
            Debug.Assert(items != null, "items != null");
            Debug.Assert(writer != null || (items.Count == 1 && items[0] is ODataAnnotatedError), "Expected either an ODataWriter or a top-level error payload.");

            Stack<object> itemsStack = new Stack<object>();
            try
            {
                int i = 0;
                for (; i < items.Count; ++i)
                {
                    if (i == throwUserExceptionAt)
                    {
                        throw new ODataTestException("User code triggered an exception.");
                    }

                    object item = items[i];
                    if (item == null)
                    {
                        Debug.Assert(itemsStack.Count > 0, "itemsStack.Count > 0");
                        WritePayloadEndItem(writer, itemsStack.Pop());
                    }
                    else
                    {
                        ODataResource entry = item as ODataResource;
                        if (entry != null)
                        {
                            WriteEntryCallbacksAnnotation callbacksAnnotation = entry.GetAnnotation<WriteEntryCallbacksAnnotation>();
                            if (callbacksAnnotation != null && callbacksAnnotation.BeforeWriteStartCallback != null)
                            {
                                callbacksAnnotation.BeforeWriteStartCallback(entry);
                            }

                            if (entry.IsNullEntry())
                            {
                                writer.WriteStart((ODataResource)null);
                            }
                            else
                            {
                                writer.WriteStart(entry);
                            }

                            itemsStack.Push(entry);
                        }
                        else
                        {
                            ODataResourceSet resourceCollection = item as ODataResourceSet;
                            if (resourceCollection != null)
                            {
                                WriteFeedCallbacksAnnotation callbacksAnnotation = resourceCollection.GetAnnotation<WriteFeedCallbacksAnnotation>();
                                if (callbacksAnnotation != null && callbacksAnnotation.BeforeWriteStartCallback != null)
                                {
                                    callbacksAnnotation.BeforeWriteStartCallback(resourceCollection);
                                }

                                writer.WriteStart(resourceCollection);
                                if (callbacksAnnotation != null && callbacksAnnotation.AfterWriteStartCallback != null)
                                {
                                    callbacksAnnotation.AfterWriteStartCallback(resourceCollection);
                                }

                                itemsStack.Push(resourceCollection);
                            }
                            else
                            {
                                ODataNestedResourceInfo navigationLink = item as ODataNestedResourceInfo;
                                if (navigationLink != null)
                                {
                                    writer.WriteStart(navigationLink);
                                    itemsStack.Push(navigationLink);
                                }
                                else
                                {
                                    ODataEntityReferenceLink entityReferenceLink = item as ODataEntityReferenceLink;
                                    if (entityReferenceLink != null)
                                    {
                                        writer.WriteEntityReferenceLink(entityReferenceLink);
                                    }
                                    else
                                    {
                                        ODataAnnotatedError error = item as ODataAnnotatedError;
                                        if (error != null)
                                        {
                                            messageWriter.WriteError(error.Error, error.IncludeDebugInformation);
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("Invalid OData item type found: " + item.GetType().FullName + ".");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (i == throwUserExceptionAt)
                {
                    throw new ODataTestException("User code triggered an exception.");
                }

                while (itemsStack.Count > 0)
                {
                    WritePayloadEndItem(writer, itemsStack.Pop());
                }
            }
            finally
            {
                // Flush always since this flush will be sync/async as per test configuration
                // while the auto-flush in Dispose will be always sync, which might cause our test streams to fail
                // since those are set to not support sync operations when async test config is in use.
                if (flush && writer != null)
                {
                    writer.Flush();
                }
            }
        }

        internal static void WriteBatchPayload(
            ODataMessageWriterTestWrapper messageWriter,
            ODataPayloadElement batchPayload,
            ODataPayloadElementToObjectModelConverter converter,
            ObjectModelToMessageWriter writer,
            IEdmModel model,
            AssertionHandler assert,
            WriterTestConfiguration config,
            bool flush)
        {
            ExceptionUtilities.CheckArgumentNotNull(messageWriter, "messageWriter");
            ExceptionUtilities.CheckArgumentNotNull(batchPayload, "batchPayload");
            ExceptionUtilities.Assert(batchPayload is BatchRequestPayload || batchPayload is BatchResponsePayload, "Payload must be a batch payload.");

            var batchWriter = messageWriter.CreateODataBatchWriter();

            var batchRequest = batchPayload as BatchRequestPayload;
            batchWriter.WriteStartBatch();
            if (batchRequest != null)
            {
                foreach (var part in batchRequest.Parts)
                {
                    var operation = part as MimePartData<IHttpRequest>;
                    if (operation != null)
                    {
                        var message = batchWriter.CreateOperationRequestMessage(operation.Body.Verb.ToString().ToUpper(), operation.Body.GetRequestUri());
                        foreach (var header in operation.Headers)
                        {
                            message.SetHeader(header.Key, header.Value);
                        }
                        // We don't write a body because we should only have gets here.
                    }
                    else
                    {
                        var changeset = part as BatchRequestChangeset;
                        ExceptionUtilities.CheckObjectNotNull(changeset, "All parts must be IHttpRequest or BatchRequestChangeset");
                        batchWriter.WriteStartChangeset();
                        foreach (var changesetOperation in changeset.Operations)
                        {
                            var op = changesetOperation as ODataRequest;
                            ExceptionUtilities.CheckObjectNotNull(op, "ODataRequest Expected.");
                            var message = batchWriter.CreateOperationRequestMessage(op.Verb.ToString().ToUpper(), op.GetRequestUri());
                            foreach (var header in op.Headers)
                            {
                                message.SetHeader(header.Key, header.Value);
                            }

                            if (op.Body != null && op.Body.RootElement != null)
                            {
                                var contentType = changesetOperation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
                                ODataFormat format = TestMediaTypeUtils.GetODataFormat(contentType, op.Body.RootElement.GetPayloadKindFromPayloadElement());

                                var messageWriterSettings = config.MessageWriterSettings.Clone();
                                messageWriterSettings.SetContentType(format);
                                var messageConfig = new WriterTestConfiguration(format, messageWriterSettings, config.IsRequest, config.Synchronous);

                                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(TestWriterUtils.GetStream(message, messageConfig), messageConfig, assert, messageWriterSettings, model))
                                {
                                    writer.WriteMessage(messageWriterWrapper, op.Body.RootElement.GetPayloadKindFromPayloadElement(), converter.Convert(op.Body.RootElement));
                                }
                            }
                        }

                        batchWriter.WriteEndChangeset();
                    }
                }
            }
            else
            {
                var batchResponse = batchPayload as BatchResponsePayload;

                foreach (var part in batchResponse.Parts)
                {
                    var fragment = part as MimePartData<HttpResponseData>;
                    if (fragment != null)
                    {
                        var operation = fragment.Body as ODataResponse;
                        var message = batchWriter.CreateOperationResponseMessage();
                        message.StatusCode = (int)operation.StatusCode;

                        foreach (var header in operation.Headers)
                        {
                            message.SetHeader(header.Key, header.Value);
                        }

                        if (operation.RootElement != null)
                        {
                            var contentType = operation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
                            ODataFormat format = TestMediaTypeUtils.GetODataFormat(contentType, operation.RootElement.GetPayloadKindFromPayloadElement());
                            var messageWriterSettings = config.MessageWriterSettings.Clone();
                            messageWriterSettings.SetContentType(format);
                            var messageConfig = new WriterTestConfiguration(format, messageWriterSettings, config.IsRequest, config.Synchronous);
                            using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(TestWriterUtils.GetStream(message, messageConfig), messageConfig, assert, messageWriterSettings, model))
                            {
                                writer.WriteMessage(messageWriterWrapper, operation.RootElement.GetPayloadKindFromPayloadElement(), converter.Convert(operation.RootElement));
                            }
                        }
                    }
                    else
                    {
                        var changeset = part as BatchResponseChangeset;
                        ExceptionUtilities.CheckObjectNotNull(changeset, "All parts must be IHttpRequest or BatchRequestChangeset");
                        batchWriter.WriteStartChangeset();
                        foreach (var changesetOperation in changeset.Operations)
                        {
                            var op = changesetOperation as ODataResponse;
                            ExceptionUtilities.CheckObjectNotNull(op, "ODataRequest Expected.");

                            var message = batchWriter.CreateOperationResponseMessage();
                            message.StatusCode = (int)op.StatusCode;

                            foreach (var header in op.Headers)
                            {
                                message.SetHeader(header.Key, header.Value);
                            }

                            if (op.RootElement != null)
                            {
                                var contentType = op.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
                                ODataFormat format = TestMediaTypeUtils.GetODataFormat(contentType, op.RootElement.GetPayloadKindFromPayloadElement());
                                var messageWriterSettings = config.MessageWriterSettings.Clone();
                                messageWriterSettings.SetContentType(format);
                                var messageConfig = new WriterTestConfiguration(format, messageWriterSettings, config.IsRequest, config.Synchronous);

                                using (ODataMessageWriterTestWrapper messageWriterWrapper = TestWriterUtils.CreateMessageWriter(TestWriterUtils.GetStream(message, messageConfig), messageConfig, assert, messageWriterSettings, model))
                                {
                                    writer.WriteMessage(messageWriterWrapper, op.RootElement.GetPayloadKindFromPayloadElement(), converter.Convert(op.RootElement));
                                }
                            }
                        }

                        batchWriter.WriteEndChangeset();
                    }
                }
            }

            batchWriter.WriteEndBatch();

            if (flush)
            {
                batchWriter.Flush();
            }
        }

        /// <summary>
        /// Execute the specified <paramref name="writeAction"/> against a message writer that exposes several error scenarios.
        /// </summary>
        /// <typeparam name="T">The type of the payload items.</typeparam>
        /// <param name="testCase">The test case to run.</param>
        /// <param name="invalidSettingSelector">The array of flags controlling the invalid behavior of the stream.</param>
        /// <param name="testConfiguration">The configuration settings for the test.</param>
        /// <param name="writeAction">The action to execute for each test case.</param>
        /// <param name="assert">The assertion handler to report assertion violations.</param>
        internal static void WriteWithStreamErrors<T>(
            PayloadWriterTestDescriptor<T> testCase,
            bool[] invalidSettingSelector,
            WriterTestConfiguration testConfiguration,
            Action<ODataMessageWriterTestWrapper> writeAction,
            AssertionHandler assert)
        {
            bool getAsyncStreamFails = invalidSettingSelector[0];
            bool streamClosed = invalidSettingSelector[1];

            TestMessageFlags flags = testConfiguration.Synchronous ? TestMessageFlags.NoAsynchronous : TestMessageFlags.NoSynchronous;
            if (getAsyncStreamFails)
            {
                flags |= TestMessageFlags.GetStreamFailure;
            }

            // serialize to a memory stream
            using (var stream = CreateTestStream(testConfiguration))
            {
                ODataMessageWriterSettings settings = testConfiguration.MessageWriterSettings.Clone();
                settings.Version = testConfiguration.Version;

                ODataMessageWriterTestWrapper messageWriter = null;
                Exception exception = null;
                try
                {
                    if (testConfiguration.IsRequest)
                    {
                        var requestMessage = new TestRequestMessage(stream, flags);
                        messageWriter = new ODataMessageWriterTestWrapper(new ODataMessageWriter(requestMessage, settings, testCase.GetMetadataProvider()), testConfiguration, requestMessage, assert);
                    }
                    else
                    {
                        var responseMessage =
                            new TestResponseMessage(stream, flags);
                        responseMessage.StatusCode = 404;
                        messageWriter = new ODataMessageWriterTestWrapper(new ODataMessageWriter(responseMessage, settings, testCase.GetMetadataProvider()), testConfiguration, responseMessage, assert);
                    }

                    if (streamClosed)
                    {
                        stream.CloseInner();
                    }

                    exception = TestExceptionUtils.RunCatching(() => writeAction(messageWriter));
                }
                finally
                {
                    if (messageWriter != null)
                    {
                        messageWriter.Dispose();
                    }
                }

                assert.IsNotNull(exception, "Expected exception but none was thrown.");

                ExpectedException expectedException = testCase.ExpectedResultCallback(testConfiguration).ExpectedException2;
                if (expectedException == null)
                {
                    if (getAsyncStreamFails)
                    {
                        // TODO: Support raw error message verification for non-product exceptions
                        expectedException = new ExpectedException(typeof(InvalidOperationException));
                    }

                    if (streamClosed)
                    {
                        if (testConfiguration.Synchronous)
                        {
                            if (streamClosed)
                            {
                                // This can fail in two different ways
                                // Sometimes this will fail with ObjectDisposedException because we try to write into the closed stream
                                // othertimes this will fail with ArgumentException since we try to create a StreamWriter over the closed stream
                                // and that fails verifying that the stream can write (which the closed stream can't).
                                if (exception is ArgumentException)
                                {
                                    expectedException = new ExpectedException(typeof(ArgumentException));
                                }
                                else
                                {
                                    expectedException = new ExpectedException(typeof(ObjectDisposedException));
                                }
                            }
                        }
                        else
                        {
                            AggregateException aggregate = exception as AggregateException;
                            assert.IsNotNull(aggregate, "Expected AggregateException");
                            aggregate = aggregate.Flatten();
                            assert.AreEqual(1, aggregate.InnerExceptions.Count, "Expected a single exception.");
                            exception = aggregate.InnerExceptions.Single();

                            if (streamClosed)
                            {
                                // TODO: Support raw error message verification for non-product exceptions
                                expectedException = new ExpectedException(typeof(NotSupportedException));
                            }
                        }
                    }
                }

                testCase.TestDescriptorSettings.ExpectedResultSettings.ExceptionVerifier.VerifyExceptionResult(expectedException, exception);
            }
        }

        /// <summary>
        /// Depending on the <paramref name="testConfiguration"/> this method modifies the <paramref name="testDescriptor"/>
        /// to use entity reference links instead of deferred links.
        /// </summary>
        /// <param name="testDescriptor">The test descriptor to inspect.</param>
        /// <param name="testConfiguration">The current test configuration to use.</param>
        /// <returns>If necessary a new test descriptor which is a copy of the source one with the deferred links turned into entity reference links.
        /// Otherwise it returns the source test descriptor.</returns>
        internal static PayloadWriterTestDescriptor<ODataItem> DeferredLinksToEntityReferenceLinksInRequest(
            this PayloadWriterTestDescriptor<ODataItem> testDescriptor,
            WriterTestConfiguration testConfiguration)
        {
            List<ODataItem> newItems = new List<ODataItem>(testDescriptor.PayloadItems);
            bool changed = false;
            if (testConfiguration.IsRequest)
            {
                for (int i = 0; i < newItems.Count; i++)
                {
                    ODataNestedResourceInfo navigationLink = newItems[i] as ODataNestedResourceInfo;
                    if (navigationLink != null)
                    {
                        if (i == newItems.Count - 1)
                        {
                            // Last node - need to replace since this is a deferred link
                            changed = true;
                            newItems.Insert(i + 1, new ODataEntityReferenceLink { Url = navigationLink.Url });
                            break;
                        }
                        else
                        {
                            if (newItems[i + 1] == null)
                            {
                                // Nav link followed by null == end -> deferred link -> replace
                                changed = true;
                                newItems.Insert(i + 1, new ODataEntityReferenceLink { Url = navigationLink.Url });
                                i = i + 2;
                            }
                        }
                    }
                }
            }

            if (changed)
            {
                return new PayloadWriterTestDescriptor<ODataItem>(testDescriptor) { PayloadItems = newItems.AsReadOnly() };
            }
            else
            {
                return testDescriptor;
            }
        }

        /// <summary>
        /// Validate that the expected error was reported or no error was reported if none was expected.
        /// </summary>
        /// <param name="reportedException">The exception from writing the payload; or null if no exception was thrown.</param>
        /// <param name="expectedResults">The expected results specifying the expected exception and error message (if any).</param>
        /// <param name="assert">The assertion handler.</param>
        /// <returns>True if an error was expected and successfully validated. False if no error was expected and none was reported.</returns>
        internal static bool ValidateReportedError(Exception reportedException, WriterTestExpectedResults expectedResults, AssertionHandler assert, IExceptionVerifier exceptionVerifier)
        {
            if (expectedResults.ExpectedException2 != null)
            {
                assert.IsNotNull(reportedException,
                    "The test was expected to fail with an exception of type " +
                    expectedResults.ExpectedException2.ExpectedExceptionType.ToString() +
                    " and message resource ID " +
                    (expectedResults.ExpectedException2.ExpectedMessage == null ? "<null>" : expectedResults.ExpectedException2.ExpectedMessage.ResourceIdentifier));
                exceptionVerifier.VerifyExceptionResult(expectedResults.ExpectedException2, reportedException);
                return true;
            }

            Exception expectedException = expectedResults.ExpectedException;
            if (expectedException != null)
            {
                assert.IsNotNull(reportedException, "The test was expected to fail with an error of type " + expectedException.GetType().FullName + " and error message: " + expectedResults.ExpectedException.Message);
                assert.IsTrue(expectedException.GetType().IsAssignableFrom(reportedException.GetType()), "Expected exception type compatible to " + expectedException.GetType().FullName + " but found exception of type " + reportedException.GetType().FullName + ".");
                assert.AreEqual(expectedResults.ExpectedException.Message, reportedException.Message, "Unexpected error message.");
                return true;
            }

            if (expectedResults.ExpectedODataExceptionMessage != null)
            {
                assert.IsNotNull(reportedException, "The test was expected to fail with error message: " + expectedResults.ExpectedODataExceptionMessage);
                assert.IsTrue(reportedException is ODataException, "Expected an ODataException instance but got a " + reportedException.GetType().FullName + ".");
                assert.AreEqual(expectedResults.ExpectedODataExceptionMessage, reportedException.Message, "Unexpected error message.");
                return true;
            }

            if (reportedException != null)
            {
                assert.IsNull(reportedException, "Unexpected exception: " + reportedException.ToString());
            }

            return false;
        }

        /// <summary>
        /// Returns the set of variables to be resolved for a given result template.
        /// </summary>
        /// <param name="testConfiguration">The test configuration used to create the variables.</param>
        /// <returns>The variables for the given test configuration.</returns>
        internal static Dictionary<string, string> GetPayloadVariablesForTestConfiguration(WriterTestConfiguration testConfiguration)
        {
            // a data wrapper is required for all responses
            bool requiresDataWrapper = !testConfiguration.IsRequest;
            // for v3 responses a results wrapper is injected as well
            bool requiresResultsWrapper = requiresDataWrapper;

            var expansions = new Dictionary<string, string>()
                {
                    { "NL", string.Empty },
                    { "Indent", string.Empty },
                    { "JsonDataWrapperIndent", string.Empty },
                    { "JsonResultsWrapperIndent", string.Empty },
                };
            return expansions;
        }

        /// <summary>
        /// Validate the result of writing a payload.
        /// </summary>
        /// <param name="stream">The (seekable) stream the payload was written to.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="expectedResults">The expected result description.</param>
        /// <param name="exception">An (optional) exception if one was thrown during writing.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        internal static void ValidateResult(Stream stream, WriterTestConfiguration testConfiguration, WriterTestExpectedResults expectedResults, Exception exception, AssertionHandler assert, IExceptionVerifier exceptionVerifier)
        {
            if (expectedResults == null)
            {
                return;
            }

            if (ValidateReportedError(exception, expectedResults, assert, exceptionVerifier))
            {
                return;
            }

            stream.Seek(0, SeekOrigin.Begin);

            bool success;
            string error;
            if (testConfiguration.Format == ODataFormat.Json)
            {
                var jsonExpectedResults = expectedResults as JsonWriterTestExpectedResults;
                ExceptionUtilities.CheckObjectNotNull(jsonExpectedResults, "The expected result should be for Json format.");
                success = CompareJsonResults(jsonExpectedResults, stream, testConfiguration, out error);
            }
            else if (testConfiguration.Format == null || testConfiguration.Format == ODataFormat.RawValue)
            {
                var rawExpectedResults = expectedResults as RawValueWriterTestExpectedResults;
                ExceptionUtilities.CheckObjectNotNull(rawExpectedResults, "The expected result should be for the raw format.");
                success = CompareRawResults(rawExpectedResults, stream, testConfiguration, out error);
            }
            else
            {
                throw new NotSupportedException("Format '" + testConfiguration.Format.ToString() + "' is not supported.");
            }

            assert.IsTrue(success, error);
        }

        /// <summary>
        /// Validate the exception of writing a payload or log the payload for baseline when no exception happen.
        /// </summary>
        /// <param name="message">The message used to write the payload.</param>
        /// <param name="testConfiguration">The configuration of the test.</param>
        /// <param name="expectedResults">The expected result description.</param>
        /// <param name="exception">An (optional) exception if one was thrown during writing.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        /// <param name="baselineLogger">Logger to log baseline.</param>
        internal static void ValidateExceptionOrLogResult(TestMessage message, WriterTestConfiguration testConfiguration, WriterTestExpectedResults expectedResults, Exception exception, AssertionHandler assert, IExceptionVerifier exceptionVerifier, BaselineLogger baselineLogger)
        {
            if (expectedResults == null)
            {
                return;
            }

            if (ValidateReportedError(exception, expectedResults, assert, exceptionVerifier))
            {
                return;
            }
            var payload = ReadToString(message);
            baselineLogger.LogPayload(payload);
        }

        /// <summary>
        /// Validates that the content type header of the response message is the expected one.
        /// </summary>
        /// <param name="testMessage">The message used to write the payload.</param>
        /// <param name="expectedResults">The expected results for the test.</param>
        internal static void ValidateContentType(TestMessage testMessage, WriterTestExpectedResults expectedResults, bool exactMatch, AssertionHandler assert)
        {
            if (expectedResults == null)
            {
                return;
            }

            string expectedContentType = expectedResults.ExpectedContentType;
            if (expectedContentType != null)
            {
                string actualContentType = testMessage.GetHeader(Microsoft.OData.ODataConstants.ContentTypeHeader);
                assert.IsNotNull(actualContentType, "Expected to find non-null 'Content-Type' header on a response message.");
                if (exactMatch)
                {
                    assert.IsTrue(string.Compare(expectedContentType, actualContentType, StringComparison.OrdinalIgnoreCase) == 0,
                        "Expected content type '" + expectedContentType + "' but found content type '" + actualContentType + "'!");
                }
                else
                {
                    assert.IsTrue(actualContentType.StartsWith(expectedContentType, StringComparison.OrdinalIgnoreCase),
                        "Expected content type '" + actualContentType + "' to start with content type '" + expectedContentType + "'!");
                }
            }
        }

        /// <summary>
        /// Validate that the <paramref name="testMessage"/> contains all the expected headers passed in <paramref name="expectedHeaders"/>.
        /// </summary>
        /// <param name="testMessage">The test message to check the headers for.</param>
        /// <param name="expectedHeaders">The set of expected header names and header values.</param>
        /// <param name="assert">The assertion handler.</param>
        /// <remarks>Note that this method ignores the Content-Type header since we treat the content type differently.</remarks>
        internal static void ValidateHeaders(TestMessage testMessage, Dictionary<string, string> expectedHeaders, AssertionHandler assert)
        {
            IEnumerable<KeyValuePair<string, string>> actualHeaders = testMessage.Headers;
            if (actualHeaders == null)
            {
                assert.IsTrue(expectedHeaders == null || expectedHeaders.Count == 0, "Did not find any headers but expected " + expectedHeaders.Count + ".");
            }
            else
            {
                int actualHeaderCount = 0;
                foreach (var kvp in actualHeaders)
                {
                    if (kvp.Key == "Content-Type" || kvp.Key == "OData-Version")
                    {
                        // the content type and data service version headers are treated separately
                        continue;
                    }

                    actualHeaderCount++;

                    string expectedValue;
                    if (expectedHeaders == null || !expectedHeaders.TryGetValue(kvp.Key, out expectedValue))
                    {
                        assert.IsFalse(true, "Did not find header " + kvp.Key + " in the expected headers.");
                        return;
                    }

                    assert.AreEqual(expectedValue, kvp.Value, "Header values don't match.");
                }

                int expectedHeadersCount = expectedHeaders == null ? 0 : expectedHeaders.Count;
                if (actualHeaderCount != expectedHeadersCount)
                {
                    assert.IsFalse(true, "Did not find all " + expectedHeaders.Count + " headers; found only " + actualHeaderCount + ".");
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="ODataMessageWriter"/> for the specified format and the specified version and
        /// sets the payload kind on it. It then compares the content-type header to the expected content-type.
        /// </summary>
        /// <param name="payloadKind">The payload kind to be written.</param>
        /// <param name="expectedContentType">The expected content type.</param>
        /// <param name="testConfiguration">The configuration of the message writer.</param>
        /// <param name="assert">The assertion handler to report errors to.</param>
        internal static void SetPayloadKindAndVerifyContentType(ODataPayloadKind payloadKind, WriterTestExpectedResults expectedResults, WriterTestConfiguration testConfiguration, AssertionHandler assert)
        {
            if (expectedResults.ExpectedODataExceptionMessage != null || expectedResults.ExpectedException != null || expectedResults.ExpectedException2 != null)
            {
                // ignore error cases
                return;
            }

            // serialize to a memory stream
            using (var memoryStream = CreateTestStream(testConfiguration))
            {
                TestMessage testMessage = null;
                Exception exception = TestExceptionUtils.RunCatching(() =>
                {
                    // We skip message stream disposal validation because this test doesn't write to the stream, so GetStream() would not be called on the message.
                    using (ODataMessageWriterTestWrapper messageWriter = TestWriterUtils.CreateMessageWriter(memoryStream, testConfiguration, assert, out testMessage, testConfiguration.MessageWriterSettings, null))
                    {
                        messageWriter.SetHeadersForPayload(payloadKind);
                    }
                });
                exception = TestExceptionUtils.UnwrapAggregateException(exception, assert);

                assert.IsNull(exception, "No exception expected.");

                TestWriterUtils.ValidateContentType(testMessage, expectedResults, false, assert);
            }
        }

        /// <summary>
        /// Sets whether the stream should fail on asynchronous calls
        /// </summary>
        /// <param name="testConfiguration"></param>
        /// <param name="messageStream"></param>
        internal static void SetFailAsynchronousCalls(TestStream messageStream, bool synchronous)
        {
            if (synchronous)
            {
                messageStream.FailAsynchronousCalls = true;
            }
            else
            {
                messageStream.FailSynchronousCalls = true;
            }
        }

        /// <summary>
        /// Compares two strings of Json and returns whether they are equal. If not, it sets the <paramref name="error"/>
        /// text to a formatted error message.
        /// </summary>
        /// <param name="expectedJsonString">Expected Json string.</param>
        /// <param name="observedJsonString">Observed Json string.</param>
        /// <param name="error">Formatted text set in the case of a failure.</param>
        /// <returns>True if the strings are equal.</returns>
        internal static bool CompareJsonStrings(string expectedJsonString, string observedJsonString, out string error)
        {
            error = null;

            if (expectedJsonString != observedJsonString)
            {
                error = string.Format(
                    "Different JSON.{0}Expected:{0}-->{1}<--{0}Actual:{0}-->{2}<--{0}",
                    Environment.NewLine,
                    expectedJsonString,
                    observedJsonString);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compares the Xml written by an ATOM writer with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="observedText">The observed payload text.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        internal static bool CompareAtomResults(AtomWriterTestExpectedResults expectedResults, string observedText, WriterTestConfiguration testConfiguration, out string error)
        {
            Func<XElement> observedXml = null;
            if (observedText.Length > 0)
            {
                observedXml = () => observedText.ToXElement(expectedResults.PreserveWhitespace);
            }

            return CompareAtomResults(expectedResults, observedXml, testConfiguration, out error);
        }

        /// <summary>
        /// Compares the JSON written by a JSON writer with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="observedText">The observed result.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        internal static bool CompareJsonResults(JsonWriterTestExpectedResults expectedResults, string observedText, WriterTestConfiguration testConfiguration, out string error)
        {
            Func<JsonValue> observedJson = null;
            if (observedText.Length > 0)
            {
                observedJson = () => JsonTextPreservingParser.ParseValue(new StringReader(observedText));
            }

            return CompareJsonResults(expectedResults, observedJson, testConfiguration, out error);
        }

        /// <summary>
        /// Compares the Xml written by an ATOM writer with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="stream">The memory stream to read the observed result from.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        private static bool CompareAtomResults(AtomWriterTestExpectedResults expectedResults, Stream stream, WriterTestConfiguration testConfiguration, out string error)
        {
            Func<XElement> observedXml = null;
            if (stream.Length > 0)
            {
                observedXml = () => stream.ToXElement(expectedResults.PreserveWhitespace);
            }

            return CompareAtomResults(expectedResults, observedXml, testConfiguration, out error);
        }

        /// <summary>
        /// Compares the Xml written by an ATOM writer with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="getObservedXml">The observed payload XML.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        private static bool CompareAtomResults(AtomWriterTestExpectedResults expectedResults, Func<XElement> getObservedXml, WriterTestConfiguration testConfiguration, out string error)
        {
            error = null;

            if (getObservedXml == null)
            {
                if (!string.IsNullOrEmpty(expectedResults.Xml))
                {
                    error = "Nothing was written to the stream but the following Xml was expected: " + expectedResults.Xml + ".";
                    return false;
                }

                return true;
            }

            // if we do not have an expected result ignore the comparison
            if (string.IsNullOrEmpty(expectedResults.Xml))
            {
                return true;
            }

            XElement observedXml = getObservedXml();
            if (expectedResults.FragmentExtractor != null)
            {
                observedXml = expectedResults.FragmentExtractor(observedXml);
            }

            var variables = GetPayloadVariablesForTestConfiguration(testConfiguration);
            XElement expectedXml = expectedResults.Xml.ToXElement(variables, expectedResults.PreserveWhitespace);

            return XmlUtils.CompareXml(expectedXml, observedXml, !expectedResults.DisableNamespaceNormalization, out error);
        }

        /// <summary>
        /// Compares the JSON written by a JSON writer with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="stream">The memory stream to read the observed result from.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        private static bool CompareJsonResults(JsonWriterTestExpectedResults expectedResults, Stream stream, WriterTestConfiguration testConfiguration, out string error)
        {
            Func<JsonValue> observedJson = null;
            if (stream.Length > 0)
            {
                observedJson = () => JsonTextPreservingParser.ParseValue(stream);
            }

            return CompareJsonResults(expectedResults, observedJson, testConfiguration, out error);
        }

        /// <summary>
        /// Compares the JSON written by a JSON writer with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="getObservedJson">The observed result.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        private static bool CompareJsonResults(JsonWriterTestExpectedResults expectedResults, Func<JsonValue> getObservedJson, WriterTestConfiguration testConfiguration, out string error)
        {
            error = null;

            if (getObservedJson == null)
            {
                if (!string.IsNullOrEmpty(expectedResults.Json))
                {
                    error = "Nothing was written to the stream but the following JSON was expected: " + expectedResults.Json + ".";
                    return false;
                }

                return true;
            }

            // if we do not have an expected result ignore the comparison
            if (string.IsNullOrEmpty(expectedResults.Json))
            {
                return true;
            }

            JsonValue observedJson = getObservedJson();
            if (expectedResults.FragmentExtractor != null)
            {
                observedJson = expectedResults.FragmentExtractor(observedJson);
            }

            var variables = GetPayloadVariablesForTestConfiguration(testConfiguration);
            string expectedJsonString = JsonUtils.GetComparableJsonString(expectedResults.Json, variables);

            // TODO: More involved JSON comparison. We could parse the expected JSON into the JsonValue and compare the trees here
            // For now just compare the strings as they are; we trim the leading/trailing whitespace here again since the fragment extractor
            // trims it in some cases but not in others. For comparison we strip leading/trailing whitespace from the expected and actual results.
            string observedJsonText = observedJson.ToText(testConfiguration.Format == ODataFormat.Json).Trim();
            return CompareJsonStrings(expectedJsonString, observedJsonText, out error);
        }

        /// <summary>
        /// Compares the raw value written by an ODataMessageWriter with the expected result.
        /// </summary>
        /// <param name="expectedResults">The expected results.</param>
        /// <param name="stream">The memory stream to read the observed result from.</param>
        /// <param name="testConfiguration">Configuration for the current test.</param>
        /// <param name="error">An error message indicating how the expected and the observed results differ.</param>
        /// <returns>True if the expected and observed results are the same; otherwise false.</returns>
        private static bool CompareRawResults(RawValueWriterTestExpectedResults expectedResults, Stream stream, WriterTestConfiguration testConfiguration, out string error)
        {
            error = null;

            if (stream.Length == 0)
            {
                if (!string.IsNullOrEmpty(expectedResults.RawValueAsText) && expectedResults.RawBytes == null)
                {
                    error = "Nothing was written to the stream but the following raw value was expected: " + expectedResults.ToText() + ".";
                    return false;
                }

                return true;
            }

            // if we do not have an expected result ignore the comparison
            if (expectedResults.RawBytes == null && string.IsNullOrEmpty(expectedResults.RawValueAsText))
            {
                return true;
            }

            if (expectedResults.RawBytes != null)
            {
                byte[] readBytes = new byte[stream.Length];
                stream.Read(readBytes, 0, (int)stream.Length);

                if (!expectedResults.RawBytes.IsEqualTo(readBytes))
                {
                    string readBytesAsText = RawValueWriterTestExpectedResults.RawBytesToText(readBytes);
                    error = string.Format(
                        "Different binary RAW value.{0}Expected:{0}-->{1}<--{0}Actual:{0}-->{2}<--{0}",
                        Environment.NewLine,
                        expectedResults.ToText(),
                        readBytesAsText);
                    return false;
                }
            }
            else
            {
                string readString;
                using (StreamReader streamReader = new StreamReader(stream))
                {
                    readString = streamReader.ReadToEnd();
                }

                var variables = GetPayloadVariablesForTestConfiguration(testConfiguration);
                string expectedRawValue = StringUtils.ResolveVariables(expectedResults.RawValueAsText, variables);

                if (expectedRawValue != readString)
                {
                    error = string.Format(
                        "Different RAW value.{0}Expected:{0}-->{1}<--{0}Actual:{0}-->{2}<--{0}",
                        Environment.NewLine,
                        expectedRawValue,
                        readString);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads from the stream using a test deserializer, then compares the result to the given payload
        /// </summary>
        /// <param name="expectedPayload">the expected payload</param>
        /// <param name="memoryStream">the seekable stream to read from</param>
        /// <param name="formatSelector">the format selector to use to get a test deserializer</param>
        /// <param name="contentType">the content type of the payload</param>
        /// <param name="comparer">payload element comparer to use</param>
        public static ODataPayloadElement Read(ODataPayloadElement expectedPayload, TestMessage message, IProtocolFormatStrategySelector formatSelector, string contentType, IBatchPayloadDeserializer deserializer = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedPayload, "expectedPayload");
            ExceptionUtilities.CheckArgumentNotNull(message, "message");
            ExceptionUtilities.CheckArgumentNotNull(formatSelector, "formatSelector");
            var memoryStream = message.TestStream.InnerStream;
            memoryStream.Seek(0, SeekOrigin.Begin);
            byte[] arr = new byte[memoryStream.Length];
            int numOfBytesRead = (int)memoryStream.Read(arr, 0, (int)memoryStream.Length);
            ExceptionUtilities.Assert(numOfBytesRead == arr.Length, "all the bytes from the memory stream should be read");
            if (contentType != MimeTypes.MultipartMixed)
            {
                return ReadNonBatch(formatSelector, contentType, arr);
            }
            else
            {
                return ReadBatch(expectedPayload, message, deserializer, arr);
            }
        }

        /// <summary>
        /// Reads from the stream using a test deserializer, then compares the result to the given payload
        /// </summary>
        /// <param name="expectedPayload">the expected payload</param>
        /// <param name="memoryStream">the seekable stream to read from</param>
        /// <param name="formatSelector">the format selector to use to get a test deserializer</param>
        /// <param name="contentType">the content type of the payload</param>
        /// <param name="comparer">payload element comparer to use</param>
        public static string ReadToString(TestMessage message)
        {
            ExceptionUtilities.CheckArgumentNotNull(message, "message");
            var memoryStream = message.TestStream.InnerStream;
            memoryStream.Seek(0, SeekOrigin.Begin);
            byte[] arr = new byte[memoryStream.Length];
            int numOfBytesRead = (int)memoryStream.Read(arr, 0, (int)memoryStream.Length);
            ExceptionUtilities.Assert(numOfBytesRead == arr.Length, "all the bytes from the memory stream should be read");
            return System.Text.Encoding.UTF8.GetString(arr);
        }

        /// <summary>
        /// Reads the non batch payload based on the content type
        /// </summary>
        /// <param name="formatSelector">Selector that returns the appropriate deserializer for the content type.</param>
        /// <param name="contentType">The content type to provide to the selector</param>
        /// <param name="arr">The bytes to be deserialized.</param>
        /// <returns>ODataPayloadElement representing the bytes in arr.</returns>
        private static ODataPayloadElement ReadNonBatch(IProtocolFormatStrategySelector formatSelector, string contentType, byte[] arr)
        {
            var strategy = formatSelector.GetStrategy(contentType, null);
            ExceptionUtilities.CheckObjectNotNull(strategy, "Could not get strategy for content-type: '{0}'", contentType);
            var deserializer = strategy.GetDeserializer();
            return deserializer.DeserializeFromBinary(arr, new ODataPayloadContext());
        }

        /// <summary>
        /// Reads the batch payload.
        /// </summary>
        /// <param name="expectedPayload">The expected payload used to get the associated request if this is a response.</param>
        /// <param name="message">The message containing the payload header information</param>
        /// <param name="formatSelector">Used to get the batch strategy.</param>
        /// <param name="body">The payload to deserialize</param>
        /// <returns>The deseriealized batch payload.</returns>
        private static ODataPayloadElement ReadBatch(ODataPayloadElement expectedPayload, TestMessage message, IBatchPayloadDeserializer deserializer, byte[] body)
        {
            ExceptionUtilities.CheckArgumentNotNull(expectedPayload, "expectedPayload");
            ExceptionUtilities.CheckArgumentNotNull(message, "message");
            ExceptionUtilities.CheckArgumentNotNull(deserializer, "deserializer");
            ExceptionUtilities.CheckArgumentNotNull(body, "body");

            bool isRequest = expectedPayload is BatchRequestPayload;

            if (isRequest)
            {
                var requestMessage = message as TestRequestMessage;
                var request = new HttpRequestData()
                {
                    Uri = requestMessage.Url,
                    Verb = (HttpVerb)Enum.Parse(typeof(HttpVerb), requestMessage.Method.ToString(), true),
                    Body = body
                };

                foreach (var header in requestMessage.Headers)
                {
                    request.Headers.Add(header);
                }

                return deserializer.DeserializeBatchRequest(request);
            }
            else
            {
                // response
                var responseMessage = message as TestResponseMessage;
                var response = new HttpResponseData()
                {
                    StatusCode = (HttpStatusCode)responseMessage.StatusCode,
                    Body = body
                };

                foreach (var header in responseMessage.Headers)
                {
                    response.Headers.Add(header);
                }

                var request = expectedPayload.GetAnnotation<ODataBatchResponseRequestAnnotation>().BatchRequest;
                return deserializer.DeserializeBatchResponse(request, response);
            }
        }

        /// <summary>
        /// Writes the end of a given item to the specified writer.
        /// </summary>
        /// <param name="writer">The OData writer to write to.</param>
        /// <param name="item">The item to write the end for.</param>
        private static void WritePayloadEndItem(ODataWriter writer, object item)
        {
            if (item != null)
            {
                ODataResource entry = item as ODataResource;
                if (entry != null)
                {
                    WriteEntryCallbacksAnnotation callbacksAnnotation = entry.GetAnnotation<WriteEntryCallbacksAnnotation>();
                    if (callbacksAnnotation != null && callbacksAnnotation.BeforeWriteEndCallback != null)
                    {
                        callbacksAnnotation.BeforeWriteEndCallback(entry);
                    }
                }

                ODataResourceSet feed = item as ODataResourceSet;
                if (feed != null)
                {
                    WriteFeedCallbacksAnnotation callbacksAnnotation = feed.GetAnnotation<WriteFeedCallbacksAnnotation>();
                    if (callbacksAnnotation != null && callbacksAnnotation.BeforeWriteEndCallback != null)
                    {
                        callbacksAnnotation.BeforeWriteEndCallback(feed);
                    }
                }
            }

            writer.WriteEnd();
        }

        /// <summary>
        /// Wrapper class which uses private reflection to access internal properties on the ODataMessageWriterSettings class in the product.
        /// </summary>
        internal sealed class ODataMessageWriterSettingsTestWrapper
        {
            private static readonly Type messageWriterSettingsType = typeof(ODataMessageWriterSettings);

            private readonly ODataMessageWriterSettings settings;

            public ODataMessageWriterSettingsTestWrapper(ODataMessageWriterSettings settings)
            {
                this.settings = settings;
            }

            public ODataVersion? Version { get { return this.settings.Version; } }
            public bool CheckCharacters { get { return this.settings.EnableCharactersCheck; } }
            public Uri BaseUri { get { return this.settings.BaseUri; } }
            public string AcceptableMediaTypes { get { return (string)ReflectionUtils.GetProperty(this.settings, "AcceptableMediaTypes"); } }
            public string AcceptableCharsets { get { return (string)ReflectionUtils.GetProperty(this.settings, "AcceptableCharsets"); } }
            public ODataFormat Format { get { return (ODataFormat)ReflectionUtils.GetProperty(this.settings, "Format"); } }
            public bool UseFormat { get { return (bool)ReflectionUtils.GetProperty(this.settings, "UseFormat"); } }
        }

        /// <summary>
        /// Gets the stream from the message according to whether synchronous or asynchronous.
        /// </summary>
        /// <param name="message">Message to retrieve stream from</param>
        /// <param name="messageConfig">The config to determine whether synchronous or not</param>
        /// <returns>A stream</returns>
        private static Stream GetStream(ODataBatchOperationRequestMessage message, WriterTestConfiguration messageConfig)
        {
            if (messageConfig.Synchronous)
            {
                return message.GetStream();
            }
            else
            {
                return message.GetStreamAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Gets the stream from the message according to whether synchronous or asynchronous.
        /// </summary>
        /// <param name="message">Message to retrieve stream from</param>
        /// <param name="messageConfig">The config to determine whether synchronous or not</param>
        /// <returns>A stream</returns>
        private static Stream GetStream(ODataBatchOperationResponseMessage message, WriterTestConfiguration messageConfig)
        {
            if (messageConfig.Synchronous)
            {
                return message.GetStream();
            }
            else
            {
                return message.GetStreamAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Fix random data from payload;
        /// </summary>
        public static string BaseLineFixup(string payload)
        {
            //Remove datatime, use different patterns to keep the data format in the result view.
            //<updated>9999-12-31T23:59:59Z</updated>
            //string pattern = @"\d{4}-\d\d-\d\d\w\d\d:\d\d:\d\d";
            //string replacement = "0000-00-00T00:00:00";
            //payload = Regex.Replace(payload, pattern, (match) => replacement);

            //pattern = @"\d{4}-\d\d-\d\d\w\d\d:\d\d:\d\d\.\d{7}";
            //replacement = "0000-00-00T00:00:00.0000000";
            //payload = Regex.Replace(payload, pattern, (match) => replacement);

            //pattern = @"\d{4}-\d\d-\d\d\w\d\d:\d\d:\d\d\.\d{7}[\+\-]\d\d:\d\d";
            //replacement = "0000-00-00T00:00:00.0000000-00:00";
            //payload = Regex.Replace(payload, pattern, (match) => replacement);

            string pattern = @"<updated>\d{4}-\d\d-\d\d\w\d\d:\d\d:\d\d\w</updated>";
            string replacement = "<updated>0000-00-00T00:00:00Z</updated>";
            payload = Regex.Replace(payload, pattern, (match) => replacement);

            pattern = @"<updated>\d{4}-\d\d-\d\d\w\d\d:\d\d:\d\d[\+\-]\d\d:\d\d</updated>";
            replacement = "<updated>0000-00-00T00:00:00+00:00</updated>";
            payload = Regex.Replace(payload, pattern, (match) => replacement);

            //Remove Id and Guid
            pattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
            replacement = "00000000-0000-0000-0000-000000000000";
            payload = Regex.Replace(payload, pattern, (match) => replacement);

            return payload;
        }

    }
}
