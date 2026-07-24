//---------------------------------------------------------------------
// <copyright file="WellKnownBinaryWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// The writer setting for Well Known Binary (WKB)
    /// </summary>
    public class WellKnownBinaryWriterSettings
    {
        /// <summary>
        ///  Gets/sets a the byte order.
        /// </summary>
        public ByteOrder Order { get; set; } = ByteOrder.LittleEndian;

        /// <summary>
        /// Gets/sets a boolean value indicating whether to support the ISO WKB.
        /// In ISO WKB, it simply adds a round number to the type number to indicate extra dimensions.
        /// +1000 a flag for Z
        /// +2000 a flag for M
        /// +3000 a flag for ZM
        /// </summary>
        public bool IsoWKB { get; set; } = true;

        /// <summary>
        /// Gets/sets a boolean value indicating whether the SRID values, present or not, should be emitted.
        /// To back compatibility for the extended WKB, let's use a flag as:
        /// 0x20000000
        /// </summary>
        public bool HandleSRID { get; set; } = true;

        /// <summary>
        /// Gets/sets a boolean value indicating whether the Z values, present or not, should be emitted.
        /// 0x80000000 a flag for Z
        /// </summary>
        public bool HandleZ { get; set; } = true;

        /// <summary>
        /// Gets/sets a boolean value indicating whether the M values, present or not, should be emitted.
        /// 0x40000000 a flag for M
        /// </summary>
        public bool HandleM { get; set; } = true;
    }
}
