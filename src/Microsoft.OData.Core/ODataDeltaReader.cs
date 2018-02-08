//---------------------------------------------------------------------
// <copyright file="ODataDeltaReader.cs" company="Microsoft">
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
    /// Base class for OData delta readers.
    /// </summary>
    public abstract class ODataDeltaReader
    {
        /// <summary>Gets the current state of the reader. </summary>
        /// <returns>The current state of the reader.</returns>
        public abstract ODataDeltaReaderState State { get; }

        /// <summary>Gets the current sub state of the reader. </summary>
        /// <returns>The current sub state of the reader.</returns>
        /// <remarks>
        /// The sub state is a complement to the current state if the current state itself is not enough to determine
        /// the real state of the reader. The sub state is only meaningful in ExpandedNavigationProperty state.
        /// </remarks>
        public abstract ODataReaderState SubState { get; }

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
