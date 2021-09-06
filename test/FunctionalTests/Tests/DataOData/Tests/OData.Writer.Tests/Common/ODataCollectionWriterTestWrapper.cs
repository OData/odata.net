//---------------------------------------------------------------------
// <copyright file="ODataCollectionWriterTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Common
{
    #region Namespaces
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataCollectionWriter which allows for transparent monitoring and changing
    /// of the writer behavior.
    /// </summary>
    public sealed class ODataCollectionWriterTestWrapper : ODataCollectionWriter
    {
        /// <summary>
        /// The underlying writer to wrap.
        /// </summary>
        private readonly ODataCollectionWriter collectionWriter;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly WriterTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="collectionWriter">The writer to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataCollectionWriterTestWrapper(ODataCollectionWriter collectionWriter, WriterTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(collectionWriter, "collectionWriter");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.collectionWriter = collectionWriter;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// The underlying writer.
        /// </summary>
        public ODataCollectionWriter CollectionWriter
        {
            get { return this.collectionWriter; }
        }

        /// <summary>
        /// Start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        public override void WriteStart(ODataCollectionStart collection)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.collectionWriter.WriteStart(collection);
            }
            else
            {
                this.collectionWriter.WriteStartAsync(collection).Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing a collection.
        /// </summary>
        /// <param name="collection">The <see cref="ODataCollectionStart"/> representing the collection.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteStartAsync(ODataCollectionStart collection)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        public override void WriteItem(object item)
        {
            if (this.testConfiguration.Synchronous)
            {
                this.collectionWriter.WriteItem(item);
            }
            else
            {
                this.collectionWriter.WriteItemAsync(item).Wait();
            }
        }

        /// <summary>
        /// Asynchronously start writing a collection item.
        /// </summary>
        /// <param name="item">The collection item to write.</param>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteItemAsync(object item)
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Finish writing a collection.
        /// </summary>
        public override void WriteEnd()
        {
            if (this.testConfiguration.Synchronous)
            {
                this.collectionWriter.WriteEnd();
            }
            else
            {
                this.collectionWriter.WriteEndAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously finish writing a collection.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public override Task WriteEndAsync()
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
                this.collectionWriter.Flush();
            }
            else
            {
                this.collectionWriter.FlushAsync().Wait();
            }
        }

        /// <summary>
        /// Asynchronously flushes the write buffer to the underlying stream.
        /// </summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public override Task FlushAsync()
        {
            throw new NotImplementedException("Tests should only use synchronous APIs.");
        }
    }
}
