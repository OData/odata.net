//---------------------------------------------------------------------
// <copyright file="ODataWriterTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataWriter which allows for transparent monitoring and changing
    /// of the writer behavior.
    /// </summary>
    public sealed class ODataWriterTestWrapper : ODataWriter
    {
        /// <summary>
        /// The underlying writer to wrap.
        /// </summary>
        private readonly ODataWriter writer;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly WriterTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="writer">The writer to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataWriterTestWrapper(ODataWriter writer, WriterTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(writer, "writer");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.writer = writer;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// Returns the underlying writer.
        /// </summary>
        public ODataWriter Writer
        {
            get { return this.writer; }
        }

        /// <summary>
        /// Start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        public override void WriteStart(ODataResourceSet feed)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.writer.WriteStart(feed);
            }
            else
            {
                this.writer.WriteStartAsync(feed).Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing a feed.
        /// </summary>
        /// <param name="feed">Feed/collection to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataResourceSet resourceCollection)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        public override void WriteStart(ODataResource entry)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.writer.WriteStart(entry);
            }
            else
            {
                this.writer.WriteStartAsync(entry).Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing an entry.
        /// </summary>
        /// <param name="entry">Entry/item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataResource entry)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">Navigation link to write.</param>
        public override void WriteStart(ODataNestedResourceInfo navigationLink)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.writer.WriteStart(navigationLink);
            }
            else
            {
                this.writer.WriteStartAsync(navigationLink).Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing a navigation link.
        /// </summary>
        /// <param name="navigationLink">Navigation link to writer.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataNestedResourceInfo navigationLink)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Finish writing a feed/entry/navigation link.
        /// </summary>
        public override void WriteEnd()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.writer.WriteEnd();
            }
            else
            {
                this.writer.WriteEndAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously finish writing a feed/entry/navigation link.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteEndAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Writes an entity reference link, which is used to represent binding to an existing resource in a request payload.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNestedResourceInfo.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public override void WriteEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.writer.WriteEntityReferenceLink(entityReferenceLink);
            }
            else
            {
                this.writer.WriteEntityReferenceLinkAsync(entityReferenceLink).Wait();
            }
        }

        /// <summary>
        /// Asynchronously writes an entity reference link, which is used to represent binding to an existing resource in a request payload.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <remarks>
        /// This method can only be called for writing request messages. The entity reference link must be surrounded
        /// by a navigation link written through WriteStart/WriteEnd.
        /// The <see cref="ODataNestedResourceInfo.Url"/> will be ignored in that case and the Uri from the <see cref="ODataEntityReferenceLink.Url"/> will be used
        /// as the binding URL to be written.
        /// </remarks>
        public override Task WriteEntityReferenceLinkAsync(ODataEntityReferenceLink entityReferenceLink)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Flushes the write buffer to the underlying stream.
        /// </summary>
        public override void Flush()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.writer.Flush();
            }
            else
            {
                this.writer.FlushAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public override Task FlushAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }
    }
}
