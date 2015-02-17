//---------------------------------------------------------------------
// <copyright file="StreamPipe.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading;

    public class StreamPipe
    {
        PipeWriteStream writeStream;
        PipeReadStream readStream;

        public StreamPipe()
        {
            this.Queue = new ConcurrentQueue<byte>();
            this.IsWriteStreamClosed = false;
            this.IsReadStreamClosed = false;
        }

        public Stream WriteStream
        {
            get
            {
                if (this.writeStream == null)
                {
                    this.writeStream = new PipeWriteStream(this);
                }

                return this.writeStream;
            }
        }

        public Stream ReadStream
        {
            get
            {
                if (this.readStream == null)
                {
                    this.readStream = new PipeReadStream(this);
                }

                return this.readStream;
            }
        }

        public bool IsReadStreamClosed
        {
            get;
            private set;
        }

        public bool IsWriteStreamClosed
        {
            get;
            private set;
        }

        private ConcurrentQueue<byte> Queue
        {
            get;
            set;
        }

        private class PipeReadStream : Stream
        {
            private StreamPipe pipe;

            public PipeReadStream(StreamPipe pipe)
            {
                this.pipe = pipe;
            }

            public override void Close()
            {
                this.pipe.IsReadStreamClosed = true;
                base.Close();
            }

            #region Stream abstract interface

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void Flush()
            {
                throw this.CreateNotSupportedException("Flush");
            }

            public override long Length
            {
                get { throw this.CreateNotSupportedException("Length"); }
            }

            public override long Position
            {
                get
                {
                    throw this.CreateNotSupportedException("Get_Position");
                }
                set
                {
                    throw this.CreateNotSupportedException("Set_Position");
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (offset != 0)
                {
                    throw new NotSupportedException("Read with offset!=0 in the PipeWriteStream is not supported.");
                }

                if (this.pipe.IsReadStreamClosed)
                {
                    throw new InvalidOperationException("Cannot read a PipeReadStream after it is closed.");
                }

                int index = 0;

                while ((!this.pipe.IsWriteStreamClosed || !this.pipe.Queue.IsEmpty) && index < count)
                {
                    byte result;
                    if (this.pipe.Queue.TryDequeue(out result))
                    {
                        buffer[index++] = result;
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }

                return index;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw this.CreateNotSupportedException("Seek");
            }

            public override void SetLength(long value)
            {
                throw this.CreateNotSupportedException("SetLength");
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw this.CreateNotSupportedException("Write");
            }

            #endregion

            private NotSupportedException CreateNotSupportedException(string operationName)
            {
                throw new NotSupportedException(string.Format("The method/property '{0}' is not supported.", operationName));
            }
        }

        private class PipeWriteStream : Stream
        {
            StreamPipe pipe;

            public PipeWriteStream(StreamPipe pipe)
            {
                this.pipe = pipe;
            }

            public override void Close()
            {
                this.pipe.IsWriteStreamClosed = true;
                base.Close();
            }

            #region Stream abstract interface

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return true; }
            }

            public override void Flush()
            {
                // All the bytes will push to queue directly.
            }

            public override long Length
            {
                get { throw this.CreateNotSupportedException("Length"); }
            }

            public override long Position
            {
                get
                {
                    throw this.CreateNotSupportedException("Get_Position");
                }
                set
                {
                    throw this.CreateNotSupportedException("Set_Position");
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw this.CreateNotSupportedException("Read");
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw this.CreateNotSupportedException("Seek");
            }

            public override void SetLength(long value)
            {
                throw this.CreateNotSupportedException("SetLength");
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (this.pipe.IsWriteStreamClosed)
                {
                    throw new InvalidOperationException("Cannot write a PipeWriteStream after it is closed.");
                }

                for (int i = 0; i < count; i++)
                {
                    pipe.Queue.Enqueue(buffer[i + offset]);
                }
            }

            #endregion

            private NotSupportedException CreateNotSupportedException(string operationName)
            {
                throw new NotSupportedException(string.Format("The method/property '{0}' is not supported.", operationName));
            }
        }
    }
}