//---------------------------------------------------------------------
// <copyright file="BinaryFormatterExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;
    using System.Buffers.Binary;
    using System.IO;

    internal static class BinaryFormatterExtensions
    {
        /// <summary>
        /// Writes the double value based on the byte order setting.
        /// </summary>
        /// <param name="writer">The binary writer.</param>
        /// <param name="value">The double value.</param>
        /// <param name="order">The byte order.</param>
        public static void Write(this BinaryWriter writer, double value, ByteOrder order)
        {
            Span<byte> buffer = stackalloc byte[8];
            if (order == ByteOrder.LittleEndian)
            {
                BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteDoubleBigEndian(buffer, value);
            }

            writer.Write(buffer);
        }

        /// <summary>
        /// Writes the uint value based on the byte order setting.
        /// </summary>
        /// <param name="writer">The binary writer.</param>
        /// <param name="value">The uint value.</param>
        /// <param name="order">The byte order.</param>
        public static void Write(this BinaryWriter writer, uint value, ByteOrder order)
        {
            Span<byte> buffer = stackalloc byte[4];
            if (order == ByteOrder.LittleEndian)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            }

            writer.Write(buffer);
        }

        /// <summary>
        /// Writes the int value based on the byte order setting.
        /// </summary>
        /// <param name="writer">The binary writer.</param>
        /// <param name="value">The int value.</param>
        /// <param name="order">The byte order.</param>
        public static void Write(this BinaryWriter writer, int value, ByteOrder order)
        {
            Span<byte> buffer = stackalloc byte[4];
            if (order == ByteOrder.LittleEndian)
            {
                BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer, value);
            }

            writer.Write(buffer);
        }

        /// <summary>
        /// Reads the uint value from reader based on the byte order setting.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="order">The byte order.</param>
        /// <returns>The uint value read from the reader.</returns>
        public static uint ReadUInt32(this BinaryReader reader, ByteOrder order)
        {
            Span<byte> buffer = stackalloc byte[4];
            int num = reader.Read(buffer);
            if (num != 4)
            {
                throw new FormatException(Error.Format(SRResources.WellKnownBinary_ByteLengthNotEnough, "UInt32", 4, num));
            }

            if (order == ByteOrder.LittleEndian)
            {
                return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
            }

            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
        }

        /// <summary>
        /// Reads the int value from reader based on the byte order setting.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="order">The byte order.</param>
        /// <returns>The int value read from the reader.</returns>
        public static int ReadInt32(this BinaryReader reader, ByteOrder order)
        {
            Span<byte> buffer = stackalloc byte[4];
            int num = reader.Read(buffer);
            if (num != 4)
            {
                throw new FormatException(Error.Format(SRResources.WellKnownBinary_ByteLengthNotEnough, "Int32", 4, num));
            }

            if (order == ByteOrder.LittleEndian)
            {
                return BinaryPrimitives.ReadInt32LittleEndian(buffer);
            }

            return BinaryPrimitives.ReadInt32BigEndian(buffer);
        }

        /// <summary>
        /// Reads the double value from reader based on the byte order setting.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <param name="order">The byte order.</param>
        /// <returns>The double value read from the reader.</returns>
        public static double ReadDouble(this BinaryReader reader, ByteOrder order)
        {
            Span<byte> buffer = stackalloc byte[8];
            int num = reader.Read(buffer);
            if (num != 8)
            {
                throw new FormatException(Error.Format(SRResources.WellKnownBinary_ByteLengthNotEnough, "Double", 8, num));
            }

            if (order == ByteOrder.LittleEndian)
            {
                return BinaryPrimitives.ReadDoubleLittleEndian(buffer);
            }

            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
    }
}
