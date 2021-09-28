//---------------------------------------------------------------------
// <copyright file="ODataTextStreamReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Threading.Tasks;

    #endregion Namespaces

    /// <summary>
    /// A textreader for reading a text value.
    /// </summary>
    internal sealed class ODataTextStreamReader : TextReader
    {
        private Func<char[], int, int, int> reader;

        private Func<char[], int, int, Task<int>> asyncReader;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">A function from which to read character values.</param>
        internal ODataTextStreamReader(Func<char[], int, int, int> reader)
        {
            Debug.Assert(reader != null, "reader cannot be null");
            this.reader = reader;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">An async delegate to invoke to read character values.</param>
        internal ODataTextStreamReader(Func<char[], int, int, Task<int>> reader)
        {
            Debug.Assert(reader != null, "reader cannot be null");
            this.asyncReader = reader;
        }

        public override int Read(char[] buffer, int offset, int count)
        {
            this.AssertSynchronous();

            return this.reader(buffer, offset, count);
        }

        public override Task<int> ReadAsync(char[] buffer, int index, int count)
        {
            this.AssertAsynchronous();

            return this.asyncReader(buffer, index, count);
        }

        /// <summary>
        /// Asserts that the stream reader was created for a synchronous operation.
        /// </summary>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        private void AssertSynchronous()
        {
            Debug.Assert(this.reader != null, "The method should only be called on a synchronous stream reader.");
        }

        /// <summary>
        /// Asserts that the stream reader was created for an asynchronous operation.
        /// </summary>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        private void AssertAsynchronous()
        {
            Debug.Assert(this.asyncReader != null, "The method should only be called on an asynchronous stream reader.");
        }
    }
}
