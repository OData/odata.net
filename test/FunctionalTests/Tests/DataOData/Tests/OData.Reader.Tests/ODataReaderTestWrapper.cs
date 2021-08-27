//---------------------------------------------------------------------
// <copyright file="ODataReaderTestWrapper.cs" company="Microsoft">
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
    /// Wrapper for the ODataReader which allows for transparent monitoring and changing
    /// of the reader behavior.
    /// </summary>
    public sealed class ODataReaderTestWrapper : ODataReader
    {
        /// <summary>
        /// The underlying reader to wrap.
        /// </summary>
        private readonly ODataReader reader;

        /// <summary>
        /// Test configuration to use.
        /// </summary>
        private readonly ReaderTestConfiguration testConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">The reader to wrap.</param>
        /// <param name="testConfiguration">The test configuration to use.</param>
        public ODataReaderTestWrapper(ODataReader reader, ReaderTestConfiguration testConfiguration)
        {
            ExceptionUtilities.CheckArgumentNotNull(reader, "reader");
            ExceptionUtilities.CheckArgumentNotNull(testConfiguration, "testConfiguration");

            this.reader = reader;
            this.testConfiguration = testConfiguration;
        }

        /// <summary>
        /// Returns the underlying reader.
        /// </summary>
        public ODataReader Reader
        {
            get { return this.reader; }
        }

        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public override ODataReaderState State
        {
            get { return this.reader.State; }
        }

        /// <summary>
        /// The most recent <see cref="ODataItem"/> that has been read.
        /// </summary>
        public override ODataItem Item
        {
            get { return this.reader.Item; }
        }

        /// <summary>
        /// Reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public override bool Read()
        {
            if (this.testConfiguration.Synchronous)
            {
                return this.reader.Read();
            }
            else
            {
                return this.reader.ReadAsync().WaitForResult();
            }
        }

        /// <summary>
        /// Asynchronously reads the next <see cref="ODataItem"/> from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public override Task<bool> ReadAsync()
        {
            throw new NotImplementedException("Tests should always use synchronous APIs.");
        }

        /// <summary>
        /// Reads the current value using a TextReader.
        /// </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public override System.IO.TextReader CreateTextReader()
        {
            return this.reader.CreateTextReader();
        }

        /// <summary>
        /// Reads the current value as a stream.
        /// </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public override System.IO.Stream CreateReadStream()
        {
            return this.reader.CreateReadStream();
        }

    }
}
