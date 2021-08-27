//---------------------------------------------------------------------
// <copyright file="ODataCollectionReaderTestWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Wrapper for the ODataCollectionReader which allows for transparent monitoring and changing
    /// of the reader behavior.
    /// </summary>
    public sealed class ODataCollectionReaderTestWrapper : ODataCollectionReader
    {
        /// <summary>
        /// The underlying reader to wrap.
        /// </summary>
        private readonly ODataCollectionReader collectionReader;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly ReaderTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="collectionReader">The reader to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataCollectionReaderTestWrapper(ODataCollectionReader collectionReader, ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(collectionReader, "collectionReader");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.collectionReader = collectionReader;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// The underlying reader.
        /// </summary>
        public ODataCollectionReader CollectionReader
        {
            get { return this.collectionReader; }
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public override ODataCollectionReaderState State
        {
            get { return this.collectionReader.State; }
        }

        /// <summary>
        /// The most recent item that has been read.
        /// </summary>
        public override object Item
        {
            get { return this.collectionReader.Item; }
        }

        /// <summary>
        /// Reads the next item from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override bool Read()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.collectionReader.Read();
            }
            else
            {
                return this.collectionReader.ReadAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously reads the next item from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public override Task<bool> ReadAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }
    }
}
