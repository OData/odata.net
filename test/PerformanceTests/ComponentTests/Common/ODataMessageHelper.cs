//---------------------------------------------------------------------
// <copyright file="ODataMessageHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Performance
{
    using System;
    using System.IO;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Performance.Common;

    /// <summary>
    /// Helper class to create ODataMessageReader and ODataMessageWriter
    /// </summary>
    public static class ODataMessageHelper
    {
        private static readonly Uri BaseUri = new Uri("http://odata.org/Perf.svc");
        private static IServiceProvider container;
        private const string ContentType = "application/json;odata.metadata=minimal;odata.streaming=true;";
        private const bool EnablePrimitiveTypeConversion = true;
        private const bool CheckCharacters = false;
        private const bool EnableMessageStreamDisposal = false;
        private const int MaxPartsPerBatch = 16;
        private const int MaxOperationsPerChangeset = 16;
        private const int MaxNestingDepth = 16;
        private const ODataVersion Version = ODataVersion.V4;

        #region Container
        /// <summary>
        /// Gets the shared DI container
        /// </summary>
        /// <returns>Instance of the DI container</returns>
        private static IServiceProvider GetSharedContainer()
        {
            if (container == null)
            {
                var builder = new TestContainerBuilder();
                builder.AddDefaultODataServices();
                container = builder.BuildContainer();
            }

            return container;
        }

        #endregion

        #region Reader
        /// <summary>
        /// Creates ODataMessageReaderSettings
        /// </summary>
        /// <param name="isFullValidation">Whether turn on FullValidation</param>
        /// <returns>Instance of ODataMessageReaderSettings</returns>
        private static ODataMessageReaderSettings CreateMessageReaderSettings(bool isFullValidation)
        {
            var settings = new ODataMessageReaderSettings
            {
                BaseUri = BaseUri,
                EnableCharactersCheck = CheckCharacters,
                EnableMessageStreamDisposal = EnableMessageStreamDisposal,
                EnablePrimitiveTypeConversion = EnablePrimitiveTypeConversion,
                Validations = isFullValidation ? ValidationKinds.All : ValidationKinds.None,
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = MaxPartsPerBatch,
                    MaxOperationsPerChangeset = MaxOperationsPerChangeset,
                    MaxNestingDepth = MaxNestingDepth,
                    MaxReceivedMessageSize = long.MaxValue,
                },
            };

            return settings;
        }

        /// <summary>
        /// Creates ODataMessageReader
        /// </summary>
        /// <param name="messageStream">Message stream</param>
        /// <param name="model">Edm model</param>
        /// <param name="messageKind">Is request or response</param>
        /// <param name="isFullValidation">Whether turn on FullValidation</param>
        /// <returns>Instance of ODataMessageReader</returns>
        public static ODataMessageReader CreateMessageReader(Stream messageStream, IEdmModel model, ODataMessageKind messageKind, bool isFullValidation)
        {
            var settings = CreateMessageReaderSettings(isFullValidation);

            if (messageKind == ODataMessageKind.Request)
            {
                var message = new StreamBasedRequestMessage(messageStream);
                message.Container = GetSharedContainer();
                message.SetHeader(ODataConstants.ContentTypeHeader, ContentType);
                return new ODataMessageReader(message, settings, model);
            }
            else
            {
                var message = new StreamBasedResponseMessage(messageStream);
                message.Container = GetSharedContainer();
                message.SetHeader(ODataConstants.ContentTypeHeader, ContentType);
                return new ODataMessageReader(message, settings, model);
            }
        }

        /// <summary>
        /// Creates ODataMessageReader
        /// </summary>
        /// <param name="messageStream">Message stream</param>
        /// <param name="model">Edm model</param>
        /// <returns>Instance of ODataMessageReader</returns>
        public static ODataMessageReader CreateMessageReader(Stream messageStream, IEdmModel model)
        {
            var settings = CreateMessageReaderSettings(true);
            var message = new StreamBasedRequestMessage(messageStream);
            message.Container = GetSharedContainer();
            message.SetHeader(ODataConstants.ContentTypeHeader, ContentType);
            return new ODataMessageReader(message, settings, model);
        }
        #endregion

        #region Writer
        /// <summary>
        /// Creates ODataMessageWriterSettings
        /// </summary>
        /// <param name="isFullValidation">Whether turn on FullValidation</param>
        /// <returns>Instance of ODataMessageWriterSettings</returns>
        private static ODataMessageWriterSettings CreateMessageWriterSettings(bool isFullValidation)
        {
            var settings = new ODataMessageWriterSettings
            {
                BaseUri = BaseUri,
                EnableCharactersCheck = CheckCharacters,
                EnableMessageStreamDisposal = EnableMessageStreamDisposal,
                Version = Version,
                Validations = isFullValidation ? ValidationKinds.All : ValidationKinds.None,
                MessageQuotas = new ODataMessageQuotas
                {
                    MaxPartsPerBatch = MaxPartsPerBatch,
                    MaxOperationsPerChangeset = MaxOperationsPerChangeset,
                    MaxNestingDepth = MaxNestingDepth,
                },
            };

            settings.ODataUri = new ODataUri() { ServiceRoot = BaseUri };
            settings.SetContentType(ODataFormat.Json);

            return settings;
        }

        /// <summary>
        /// Creates ODataMessageWriter
        /// </summary>
        /// <param name="stream">Message stream</param>
        /// <param name="model">Edm model</param>
        /// <param name="messageKind">Is request or response</param>
        /// <param name="isFullValidation">Whether turn on FullValidation</param>
        /// <returns>Instance of ODataMessageWriter</returns>
        public static ODataMessageWriter CreateMessageWriter(Stream stream, IEdmModel model, ODataMessageKind messageKind, bool isFullValidation)
        {
            var settings = CreateMessageWriterSettings(isFullValidation);

            if (messageKind == ODataMessageKind.Request)
            {
                return new ODataMessageWriter(new StreamBasedRequestMessage(stream) { Container = GetSharedContainer() }, settings, model);
            }

            return new ODataMessageWriter(new StreamBasedResponseMessage(stream) { Container = GetSharedContainer() }, settings, model);
        }

        /// <summary>
        /// Creates ODataMessageWriter
        /// </summary>
        /// <param name="stream">Message stream</param>
        /// <param name="model">Edm model</param>
        /// <returns>Instance of ODataMessageWriter</returns>
        public static ODataMessageWriter CreateMessageWriter(Stream stream, IEdmModel model)
        {
            var settings = CreateMessageWriterSettings(true);
            return new ODataMessageWriter(new StreamBasedRequestMessage(stream) { Container = GetSharedContainer() }, settings, model);
        }
        #endregion
    }
}
