namespace NewStuff._Design._0_Convention.Sample
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class HttpClientExtensions
    {
        public static WriteStream Patch(this HttpClient httpClient, Uri uri)
        {
            return new WriteStream(async content => await httpClient.PatchAsync(uri, content).ConfigureAwait(false));
        }
    }



    public sealed class WriteStream : Stream
    {
        private readonly object @lock;

        private readonly Stream underlyingStream;

        private readonly ReadStream readStream;

        private readonly StreamContent streamContent;

        private readonly Task<HttpResponseMessage> responseFuture;

        private bool disposed;

        public WriteStream(Func<HttpContent, Task<HttpResponseMessage>> send)
        {
            this.@lock = new object();
            this.underlyingStream = new MemoryStream(); //// TODO this doesn't actually throw away the data that's already been written
            this.readStream = new ReadStream(this.underlyingStream, this.@lock);
            this.streamContent = new StreamContent(this.readStream);
            this.responseFuture = send(this.streamContent);

            this.disposed = false;
        }

        public async Task<HttpResponseMessage> Final()
        {
            if (this.disposed)
            {
                throw new Exception("TODO");
            }

            this.readStream.Final();
            return await this.responseFuture.ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            //// TODO i don't remember if this is the correct pattern
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                // this.responseFuture.Dispose(); //// TODO i don't think you're supposed to dispose async tasks
                this.streamContent.Dispose();
                this.readStream.Dispose();
                this.underlyingStream.Dispose();
            }

            base.Dispose(disposing);
            this.disposed = true;
        }

        public override bool CanRead { get; } = false;

        public override bool CanSeek { get; } = false;

        public override bool CanWrite { get; } = true;

        public override long Length
        {
            get
            {
                throw new NotSupportedException("TODO");
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException("tODO");
            }
            set
            {
                throw new NotSupportedException("TODO");
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("TODO");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("TODO");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("TODO");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        private sealed class ReadStream : Stream
        {
            public ReadStream(Stream stream, object @lock)
            {
            }

            public void Final()
            {
            }

            public override bool CanRead { get; } = true; //// TODO have you ever figured out if it's better for this to be an autoproperty with a single value versus having a body that always returns false?

            public override bool CanSeek { get; } = false;

            public override bool CanWrite { get; } = false;

            public override long Length
            {
                get
                {
                    throw new NotSupportedException("TODO");
                }
            }

            public override long Position
            {
                get
                {
                    throw new NotSupportedException("TODO");
                }
                set
                {
                    throw new NotSupportedException("TODO");
                }
            }

            public override void Flush()
            {
                throw new NotSupportedException("TODO");
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException("TODO");
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException("TODO");
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException("TODO");
            }
        }
    }
}
