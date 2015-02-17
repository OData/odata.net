//---------------------------------------------------------------------
// <copyright file="BatchReaderTestStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    using System;
    using System.IO;
    using Microsoft.Test.Taupo.OData.Common;

    /// <summary>
    /// This class simulates a http stream in that it doesn't always return the number of bytes requested.
    /// </summary>
    public class BatchReaderTestStream : TestStream
    {
        /// <summary>
        /// Determines the number of bytes which will actually be read from the stream.
        /// </summary>
        private Random random;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        /// <param name="ignoreDispose">When set to true, the Dispose method will not dispose the innerStream.
        /// When set to false (default), the innerStream will be disposed by the Dispose() method.</param>
        public BatchReaderTestStream(Stream stream, bool ignoreDispose = false)
            : base(stream, ignoreDispose)
        {
            random = new Random();
        }

        /// <summary>
        /// Reads a random sized subset of the specified count.
        /// </summary>
        /// <param name="buffer">The buffer to read to.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The max number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, this.random.Next(count) + 1);
        }
    }
}
