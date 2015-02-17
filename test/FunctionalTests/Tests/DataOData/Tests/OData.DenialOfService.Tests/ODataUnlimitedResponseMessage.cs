//---------------------------------------------------------------------
// <copyright file="ODataUnlimitedResponseMessage.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Reliability.ODataSecurityTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.OData.Core;

    public class ODataUnlimitedResponseMessage : IODataResponseMessage
    {
        private Stream stream;
        private Dictionary<string, string> headers;

        public ODataUnlimitedResponseMessage(string startingString, string cycleString)
        {
            this.stream = new TamperedStream(startingString, cycleString);
            this.headers = new Dictionary<string, string>();
        }

        public string GetHeader(string headerName)
        {
            return this.headers[headerName];
        }

        public Stream GetStream()
        {
            return this.stream;
        }

        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get 
            { 
                return this.headers; 
            }
        }

        public void SetHeader(string headerName, string headerValue)
        {
            this.headers[headerName] = headerValue;
        }

        public int StatusCode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private class TamperedStream : Stream
        {
            private bool firstRead = true;
            private long cycleCount = 0;
            private int cycleBufferPosition = 0;
            private string startingString;
            private string cycleString;

            public TamperedStream(string startingString, string cycleString)
            {
                this.startingString = startingString;
                this.cycleString = cycleString;
            }

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
                get { throw new NotImplementedException(); }
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override long Length
            {
                get { throw new NotImplementedException(); }
            }

            public override long Position
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if (firstRead)
                {
                    firstRead = false;
                    var startingBytes = Encoding.UTF8.GetBytes(startingString);
                    startingBytes.CopyTo(buffer, 0);
                    this.WriteCycles(buffer, startingBytes.Length);
                }
                else
                {
                    this.WriteCycles(buffer, 0);
                }
                  
                return count;
            }

            private void WriteCycles(byte[] buffer, int bufferPosition)
            {
                byte[] cycleBuffer = null;
                while (bufferPosition < buffer.Length)
                {
                    if (cycleBuffer == null)
                    {
                        var numberedCycleString = this.cycleString.Replace("[Count]", this.cycleCount.ToString());
                        cycleBuffer = Encoding.UTF8.GetBytes(numberedCycleString);
                    }

                    buffer[bufferPosition] = cycleBuffer[this.cycleBufferPosition];
                    bufferPosition++;
                    cycleBufferPosition++;
                    if (cycleBufferPosition >= cycleBuffer.Length)
                    {
                        cycleBuffer = null;
                        this.cycleBufferPosition = 0;
                        this.cycleCount++;
                    }
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }
    }
}
