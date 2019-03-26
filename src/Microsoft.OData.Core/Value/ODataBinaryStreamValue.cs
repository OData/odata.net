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
        public ODataBinaryStreamValue(Stream stream)
        {
            ExceptionUtils.CheckArgumentNotNull(stream, "stream");

            this.Stream = stream;
        }

        /// <summary>
        /// The Stream wrapped by the ODataBinaryStreamValue
        /// </summary>
        public Stream Stream { get; private set; }
    }
}
