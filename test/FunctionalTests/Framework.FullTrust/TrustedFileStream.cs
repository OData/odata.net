//---------------------------------------------------------------------
// <copyright file="TrustedFileStream.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria.FullTrust
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// Wraps System.IO.FileStream
    /// </summary>
    public class TrustedFileStream : Stream
    {
        private FileStream underlying;

        /// <summary>
        /// Initializes a new instance of the TrustedFileStream class
        /// </summary>
        /// <param name="path">The path for the file stream</param>
        /// <param name="mode">The mode for the file stream</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public TrustedFileStream(string path, FileMode mode)
            : base()
        {
            underlying = new FileStream(path, mode);
        }

        /// <summary>
        /// Initializes a new instance of the TrustedFileStream class
        /// </summary>
        /// <param name="path">The path for the file stream</param>
        /// <param name="mode">The mode for the file stream</param>
        /// <param name="access">The access for the file stream</param>
        /// <param name="share">The share for the file stream</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public TrustedFileStream(string path, FileMode mode, FileAccess access, FileShare share)
            : base()
        {
            underlying = new FileStream(path, mode, access, share);
        }

        /// <summary>
        /// Gets the value of FileStream.CanRead
        /// </summary>
        public override bool CanRead
        {
            get { return underlying.CanRead; }
        }

        /// <summary>
        /// Gets the value of FileStream.CanSeek
        /// </summary>
        public override bool CanSeek
        {
            get { return underlying.CanSeek; }
        }

        /// <summary>
        /// Gets the value of FileStream.CanWrite
        /// </summary>
        public override bool CanWrite
        {
            get { return underlying.CanWrite; }
        }

        /// <summary>
        /// Gets the value of FileStream.Length
        /// </summary>
        public override long Length
        {
            get { return underlying.Length; }
        }

        /// <summary>
        /// Gets or sets the value of FileStream.Position
        /// </summary>
        public override long Position
        {
            get
            {
                return underlying.Position;
            }
            set
            {
                underlying.Position = value;
            }
        }

        /// <summary>
        /// Closes the underlying FileStream
        /// </summary>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public override void Close()
        {
            base.Close();
            underlying.Close();
        }

        /// <summary>
        /// Disposes the underlying FileStream
        /// </summary>
        /// <param name="disposing">Whether or not to release managed resources</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                underlying.Dispose();
            }
        }
        
        /// <summary>
        /// Invokes FileStream.Flush
        /// </summary>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public override void Flush()
        {
            underlying.Flush();
        }

        /// <summary>
        /// Invokes FileStream.Read
        /// </summary>
        /// <param name="array">The buffer to read into</param>
        /// <param name="offset">The offset to start at</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The number of bytes read</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public override int Read(byte[] array, int offset, int count)
        {
            return underlying.Read(array, offset, count);
        }

        /// <summary>
        /// Invokes FileStream.Seek
        /// </summary>
        /// <param name="offset">The seek offset</param>
        /// <param name="origin">The seek origin</param>
        /// <returns>The return value of FileStream.Seek</returns>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public override long Seek(long offset, SeekOrigin origin)
        {
            return underlying.Seek(offset, origin);
        }

        /// <summary>
        /// Invokes FileStream.SetLength
        /// </summary>
        /// <param name="value">The new length</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public override void SetLength(long value)
        {
            underlying.SetLength(value);
        }

        /// <summary>
        /// Invokes FileStream.Write
        /// </summary>
        /// <param name="array">The buffer to write from</param>
        /// <param name="offset">The offset to start at</param>
        /// <param name="count">The number of bytes to write</param>
        [FileIOPermission(SecurityAction.Assert, Unrestricted = true)]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public override void Write(byte[] array, int offset, int count)
        {
            underlying.Write(array, offset, count);
        }
    }
}
