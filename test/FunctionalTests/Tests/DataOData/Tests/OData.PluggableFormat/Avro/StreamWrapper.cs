//---------------------------------------------------------------------
// <copyright file="StreamWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ENABLE_AVRO
namespace Microsoft.Test.OData.PluggableFormat.Avro
{
    using System.IO;

    // This stream is for writer only, as avro serializer would use position property.
    internal class StreamWrapper : Stream
    {
        private Stream innerStream;

        public StreamWrapper(Stream stream)
        {
            this.innerStream = stream;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { throw new System.NotImplementedException(); }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            innerStream.Flush();
        }

        public override long Length
        {
            get { throw new System.NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                if (innerStream.CanSeek)
                {
                    return innerStream.Position;
                }

                return 0;
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.innerStream.Write(buffer, offset, count);
        }
    }
}
#endif