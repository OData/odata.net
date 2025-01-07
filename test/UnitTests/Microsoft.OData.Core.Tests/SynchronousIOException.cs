//---------------------------------------------------------------------
// <copyright file="SynchrnousIOException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// This exception is thrown when synchronous I/O occurs
    /// in asynchronous code paths.
    /// </summary>
    internal class SynchronousIOException
        : Exception

    {
 
        public SynchronousIOException():
            this("Synchronous I/O is not allowed in asynchronous code paths.")
        { }
        public SynchronousIOException(string message) : base(message) { }
    }
}
