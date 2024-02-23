//---------------------------------------------------------------------
// <copyright file="ODataBinaryStreamValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.IO;
    #endregion

    /// <summary>
    /// A class to represent a binary stream value
    /// </summary>
    public sealed class ODataBinaryStreamValue : ODataValue
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Input stream</param>
        public ODataBinaryStreamValue(Stream stream) :
            this(stream, leaveOpen: false)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Input stream</param>
        /// <param name="leaveOpen">Whether the provided stream should be left open.</param>
        public ODataBinaryStreamValue(Stream stream, bool leaveOpen)
        {
            ExceptionUtils.CheckArgumentNotNull(stream, "stream");

            this.Stream = stream;
            this.LeaveOpen = leaveOpen;
        }

        /// <summary>
        /// The Stream wrapped by the ODataBinaryStreamValue
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Whether this instance's <see cref="Stream"/> should be left open.
        /// </summary>
        internal bool LeaveOpen { get; }
    }
}
