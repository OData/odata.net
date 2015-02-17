//---------------------------------------------------------------------
// <copyright file="BatchReaderStreamReadDescriptor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Batch
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// A class representing a read from the batch stream.
    /// </summary>
    internal sealed class BatchReaderStreamReadDescriptor
    {
        /// <summary>The size of the buffer to read into.</summary>
        public int BufferSize { get; set; }

        /// <summary>The offset into te buffer to read into.</summary>
        public int BufferOffset { get; set; }

        /// <summary>The number of bytes to read.</summary>
        public int NumberOfBytesToRead { get; set; }

        /// <summary>The expected number of bytes read.</summary>
        public int ExpectedNumberOfBytesRead { get; set; }

        /// <summary>The offset in the source stream where to begin reading.</summary>
        public int SourceStreamOffset { get; set; }
    }
}
