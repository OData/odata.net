//---------------------------------------------------------------------
// <copyright file="TestTextReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Implements a TextReader which returns different sizes from its Read method.
    /// </summary>
    public class TestTextReader : TextReader
    {
        /// <summary>
        /// The reader being wrapped.
        /// </summary>
        private TextReader innerReader;

        /// <summary>
        /// If set to true the Read() method will always fail.
        /// </summary>
        public bool FailOnSingleCharacterRead { get; set; }

        /// <summary>
        /// If set to true the Peek() method will always fail.
        /// </summary>
        public bool FailOnPeek { get; set; }

        /// <summary>
        /// Optional enumerator which gets invoked before every Read call to possibly change the read size.
        /// The returned value is set as the read value value.
        /// </summary>
        public IEnumerator<int> ReadSizesEnumerator { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="innerReader">The text reader to wrap.</param>
        public TestTextReader(TextReader innerReader)
        {
            ExceptionUtilities.CheckArgumentNotNull(innerReader, "innerReader");

            this.innerReader = innerReader;
        }

        /// <summary>
        /// Reads a single character.
        /// </summary>
        /// <returns>The read character or -1 if EOF.</returns>
        public override int Read()
        {
            VerifyNotDisposed();
            ExceptionUtilities.Assert(!this.FailOnSingleCharacterRead, "The single character TextReader.Read method should not be called.");
            return this.innerReader.Read();
        }

        /// <summary>
        /// Returns the next character to be read.
        /// </summary>
        /// <returns>The next character to be read or -1 if EOF.</returns>
        public override int Peek()
        {
            VerifyNotDisposed();
            ExceptionUtilities.Assert(!this.FailOnPeek, "The TextReader.Peek method should not be called.");
            return this.innerReader.Peek();
        }

        /// <summary>
        /// Reads a block of characters.
        /// </summary>
        /// <param name="buffer">The buffer to read to.</param>
        /// <param name="index">The index to start writing to.</param>
        /// <param name="count">The number of characters to read.</param>
        /// <returns>The number of characters read. If this is 0, the EOF was hit.
        /// Otherwise it can be 1 to count, the Read will return as soon as it has at least some characters.</returns>
        public override int Read(char[] buffer, int index, int count)
        {
            VerifyNotDisposed();

            int readSize = Int32.MaxValue;
            if (this.ReadSizesEnumerator != null)
            {
                readSize = this.ReadSizesEnumerator.MoveNext() ? this.ReadSizesEnumerator.Current : Int32.MaxValue;
            }

            if (count < readSize)
            {
                readSize = count;
            }

            return this.innerReader.Read(buffer, index, readSize);
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        /// <param name="disposing">If Dispose was called.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.innerReader.Dispose();
                this.innerReader = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Verifies the object has not been already disposed.
        /// </summary>
        private void VerifyNotDisposed()
        {
            if (this.innerReader == null)
            {
                throw new ObjectDisposedException("TestTextReader");
            }
        }
    }
}
