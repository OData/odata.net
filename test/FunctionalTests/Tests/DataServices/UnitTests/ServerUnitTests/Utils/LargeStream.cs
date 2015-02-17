//---------------------------------------------------------------------
// <copyright file="LargeStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System.IO;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class LargeStream : Stream
    {
        private bool isClosed;
        private bool isDisposed;
        private long length;
        private long position;
        private byte? content;

        private void ThrowIfClosedOrDisposed()
        {
            if (isClosed)
            {
                throw new ObjectDisposedException("LargeStream already Closed.");
            }

            if (isDisposed)
            {
                throw new ObjectDisposedException("LargeStream already Disposed.");
            }
        }

        public LargeStream()
        {
        }

        /// <summary>Initializes a new <see cref="LargeStream"/> with the specified values.</summary>
        /// <param name="content">Single byte to be used as repeating content.</param>
        /// <param name="length">Number of times the byte should be repeated.</param>
        public LargeStream(byte content, long length)
        {
            this.content = content;
            this.length = length;
        }

        public override bool CanRead
        {
            get
            {
                this.ThrowIfClosedOrDisposed();
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                this.ThrowIfClosedOrDisposed();
                return true;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                this.ThrowIfClosedOrDisposed();
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                this.ThrowIfClosedOrDisposed();
                return true;
            }
        }

        public override long Length
        {
            get
            {
                this.ThrowIfClosedOrDisposed();
                return this.length;
            }
        }

        public override long Position
        {
            get
            {
                this.ThrowIfClosedOrDisposed();
                return this.position;
            }

            set
            {
                this.ThrowIfClosedOrDisposed();
                if (value < 0 || value >= this.length)
                {
                    throw new InvalidOperationException();
                }

                this.position = value;
            }
        }

        public override int ReadTimeout
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        public override int WriteTimeout
        {
            get { throw new InvalidOperationException(); }
            set { throw new InvalidOperationException(); }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            this.isClosed = true;
        }

        public new void Dispose()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException("LargeStream.");
            }

            this.isDisposed = true;
        }

        public void ReOpen()
        {
            this.isClosed = false;
            this.isDisposed = false;
            this.position = 0;
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            this.ThrowIfClosedOrDisposed();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Assert.IsNotNull(buffer);
            this.ThrowIfClosedOrDisposed();

            byte b = this.content.Value;
            long remaining = this.length - this.position;
            int read = (remaining > count) ? count : (int)remaining;
            for (int i = offset; i < (offset + read); i++)
            {
                buffer[i] = b;
            }

            this.position += read;
            return read;
        }

        private byte[] singleByte = new byte[1];

        public override int ReadByte()
        {
            this.ThrowIfClosedOrDisposed();
            if (this.Read(singleByte, 0, 1) == 0)
            {
                return -1;
            }

            return this.singleByte[0];
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.ThrowIfClosedOrDisposed();
            long position = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = 0;
                    break;
                case SeekOrigin.Current:
                    position = this.position;
                    break;
                case SeekOrigin.End:
                    position = this.length;
                    break;
            }

            position += offset;

            if (position < 0 || position >= this.length)
            {
                throw new IOException();
            }

            this.position = position;
            return this.position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Assert.IsNotNull(buffer);
            this.ThrowIfClosedOrDisposed();
            if (count <= 0)
            {
                return;
            }

            if (!this.content.HasValue)
            {
                this.content = buffer[offset];
            }

            Assert.AreEqual(this.content, buffer[offset + count - 1]);
            //while (offset < count)
            //{
            //   Assert.AreEqual(this.content, buffer[offset++]);
            //}

            this.length += count;
            this.position += count;
        }

        public override void WriteByte(byte value)
        {
            this.ThrowIfClosedOrDisposed();
            singleByte[0] = value;
            Write(singleByte, 0, 1);
        }

        public bool Compare(LargeStream other)
        {
            if (this.content == other.content && this.length == other.length)
            {
                return true;
            }

            return false;
        }
    }
}