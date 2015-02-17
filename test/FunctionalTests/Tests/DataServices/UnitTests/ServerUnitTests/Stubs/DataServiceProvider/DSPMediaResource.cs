//---------------------------------------------------------------------
// <copyright file="DSPMediaResource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a single media resource.
    /// </summary>
    public class DSPMediaResource
    {
        /// <summary>The internal stream of the media resource.</summary>
        protected Stream stream;

        /// <summary>
        /// Gets the read stream.
        /// </summary>
        /// <returns>Returns a readable stream.</returns>
        public virtual Stream GetReadStream(out bool isEmptyStream)
        {
            if (this.stream.CanSeek)
            {
                this.stream.Position = 0;
            }

            // The default implementation uses memory stream so .Length will work.
            isEmptyStream = this.stream.Length == 0;
            return this.stream;
        }

        /// <summary>
        /// Gets the write stream.
        /// </summary>
        /// <returns>Returns a writable stream.</returns>
        public virtual Stream GetWriteStream()
        {
            if (this.stream.CanSeek)
            {
                this.stream.Position = 0;
            }

            return this.stream;
        }

        /// <summary>The content type of the media resource.</summary>
        public virtual string ContentType { get; set; }

        /// <summary>The etag of the media resource.</summary>
        public virtual string Etag { get; set; }

        /// <summary>The read stream uri of the media resource.</summary>
        public virtual Uri ReadStreamUri { get; set; }

        /// <summary>Constructs the media resource.</summary>
        public DSPMediaResource()
        {
            this.InitializeMediaResource();
        }

        /// <summary>Generates an etag</summary>
        /// <returns>etag string</returns>
        public static string GenerateStreamETag()
        {
            return '"' + Guid.NewGuid().ToString() + '"';
        }

        /// <summary>
        /// Creates a new media resource
        /// </summary>
        /// <returns>new media resource instance.</returns>
        protected virtual void InitializeMediaResource()
        {
            this.stream = new ReusableStream();
        }

        /// <summary>
        /// Overriding the dispose method so that after the Astoria runtime disposes the stream returned by
        /// the stream provider, we can still reuse the streams for testing purposes.
        /// </summary>
        private class ReusableStream : MemoryStream, IDisposable
        {
            void IDisposable.Dispose()
            {
                // Resets the stream
                this.Position = 0;
            }
        }
    }
}
