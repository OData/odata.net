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

    #endregion Namespaces

    /// <summary>
    /// A textreader for reading a text value.
    /// </summary>
    internal sealed class ODataTextStreamReader : TextReader
    {
        private Func<char[], int, int, int> reader;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="reader">A function from which to read character values.</param>
        internal ODataTextStreamReader(Func<char[], int, int, int> reader)
        {
            Debug.Assert(reader != null, "reader cannot be null");
            this.reader = reader;
        }

        public override int Read(char[] buffer, int offset, int count)
        {
            return reader(buffer, offset, count);
        }
    }
}
