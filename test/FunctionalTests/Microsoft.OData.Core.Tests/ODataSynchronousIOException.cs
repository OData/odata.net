using System;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// This exception is thrown when synchronous I/O occurs
    /// in asynchronous code paths.
    /// </summary>
    internal class ODataSynchronousIOException: Exception
    {
        public ODataSynchronousIOException():
            this("Synchronous I/O is not allowed in asynchronous code paths.")
        { }
        public ODataSynchronousIOException(string message) : base(message) { }
    }
}
