//---------------------------------------------------------------------
// <copyright file="ODataReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// Base class for OData readers.
    /// </summary>
    public abstract class ODataReader
    {
        /// <summary>Gets the current state of the reader. </summary>
        /// <returns>The current state of the reader.</returns>
        public abstract ODataReaderState State { get; }

        /// <summary>Gets the most recent <see cref="Microsoft.OData.ODataItem" /> that has been read. </summary>
        /// <returns>The most recent <see cref="Microsoft.OData.ODataItem" /> that has been read.</returns>
        public abstract ODataItem Item { get; }

        /// <summary> Reads the next <see cref="Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public abstract bool Read();

        /// <summary>Creates a stream for reading an inline stream property. </summary>
        /// <returns>A stream for reading the stream property.</returns>
        public virtual Stream CreateReadStream()
        {
            throw new NotImplementedException();
        }

        /// <summary>Creates a TextReader for reading an inline string property. </summary>
        /// <returns>A TextReader for reading the text property.</returns>
        public virtual TextReader CreateTextReader()
        {
            throw new NotImplementedException();
        }

        /// <summary> Asynchronously reads the next <see cref="Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public abstract Task<bool> ReadAsync();

        /// <summary>Asynchronously creates a stream for reading an inline stream property. </summary>
        /// <returns>A stream for reading the stream property.</returns>
        public virtual Task<Stream> CreateReadStreamAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateReadStream());
        }

        /// <summary>Asynchronously creates a stream for reading an inline stream property. </summary>
        /// <returns>A stream for reading the stream property.</returns>
        public virtual Task<TextReader> CreateTextReaderAsync()
        {
            return TaskUtils.GetTaskForSynchronousOperation(() => this.CreateTextReader());
        }
    }
}
