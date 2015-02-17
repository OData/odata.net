//---------------------------------------------------------------------
// <copyright file="StreamHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System.IO;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Collection of helpful utility functions for working with streams.
    /// </summary>
    public static class StreamHelpers
    {
        /// <summary>Copies content from one stream into another.</summary>
        /// <param name="source">Stream to read from.</param>
        /// <param name="destination">Stream to write to.</param>
        /// <remarks>The destination stream will have it's position reset to the beginning.</remarks>
        public static void CopyStream(Stream source, Stream destination)
        {
            if (source == null)
            {
                throw new TaupoArgumentNullException("source stream");
            }

            if (destination == null)
            {
                throw new TaupoArgumentNullException("destination stream");
            }

            source.CopyTo(destination);
            destination.Position = 0;
        }

        /// <summary>
        /// Compare the content of two streams.
        /// </summary>
        /// <param name="left">The stream 1</param>
        /// <param name="right">The stream 2</param>
        /// <returns>true if both streams are identical.</returns>
        public static bool CompareStream(Stream left, Stream right)
        {
            if (left == null || !left.CanRead)
            {
                throw new TaupoArgumentException("Must be a valid readable stream - left");
            }

            if (right == null || !right.CanRead)
            {
                throw new TaupoArgumentException("Must be a valid readable stream - right");
            }

            if (left == right)
            {
                return true;
            }

            int bufferSize = 64 * 1024;
            byte[] leftBuffer = new byte[bufferSize];
            byte[] rightBuffer = new byte[bufferSize];
            int leftReadCount = 0;
            int rightReadCount = 0;

            do
            {
                leftReadCount = FillBuffer(leftBuffer, left);
                rightReadCount = FillBuffer(rightBuffer, right);

                if (leftReadCount != rightReadCount)
                {
                    return false;
                }

                for (int idx = 0; idx < leftReadCount; idx++)
                {
                    if (leftBuffer[idx] != rightBuffer[idx])
                    {
                        return false;
                    }
                }
            }
            while (leftReadCount > 0);

            return true;
        }

        /// <summary>
        /// Set stream associated with entity in V1 stream support style.
        /// </summary>
        /// <param name="objectServices">IEntityModelObjectServices to get entity object adapter</param>
        /// <param name="wrappedEntity">entity to set stream</param>
        /// <param name="clientMediaEntryAnnotation">ClientMediaEntryAnnotation of the entity</param>
        /// <param name="streamValue">stream bytes</param>
        public static void SetSaveV1Stream(IEntityModelObjectServices objectServices, WrappedObject wrappedEntity, ClientMediaEntryAnnotation clientMediaEntryAnnotation, byte[] streamValue)
        {
            var objectAdapter = objectServices.GetObjectAdapter(wrappedEntity.Product.GetType().FullName);
            objectAdapter.SetMemberValue(wrappedEntity.Product, clientMediaEntryAnnotation.MediaEntryName, streamValue);
            objectAdapter.SetMemberValue(wrappedEntity.Product, clientMediaEntryAnnotation.MimeTypePropertyName, MimeTypes.TextPlain);
        }

        private static int FillBuffer(byte[] buffer, Stream stream)
        {
            int bufferSize = buffer.Length;
            int total = 0;
            int count = 0;

            do
            {
                count = stream.Read(buffer, total, bufferSize - total);
                total += count;
            }
            while (count > 0 && total < bufferSize);

            return total;
        }
    }
}