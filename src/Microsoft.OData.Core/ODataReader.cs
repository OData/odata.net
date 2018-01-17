//---------------------------------------------------------------------
// <copyright file="ODataReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for OData readers.
    /// </summary>
    public abstract class ODataReader
    {
        /// <summary>Gets the current state of the reader. </summary>
        /// <returns>The current state of the reader.</returns>
        public abstract ODataReaderState State { get; }

        /// <summary>Gets the most recent <see cref="T:Microsoft.OData.ODataItem" /> that has been read. </summary>
        /// <returns>The most recent <see cref="T:Microsoft.OData.ODataItem" /> that has been read.</returns>
        public abstract ODataItem Item { get; }

        /// <summary> Reads the next <see cref="T:Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public abstract bool Read();

#if PORTABLELIB
        /// <summary> Asynchronously reads the next <see cref="T:Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public abstract Task<bool> ReadAsync();
#endif
    }
}
