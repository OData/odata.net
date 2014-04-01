//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    #endregion Namespaces

    /// <summary>
    /// Writes text indented as per the indentation level setting
    /// </summary>
    internal sealed class NonIndentedTextWriter : TextWriterWrapper
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="writer">The underlying writer to wrap.</param>
        public NonIndentedTextWriter(TextWriter writer)
            : base(writer.FormatProvider)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes the given string value to the underlying writer.
        /// </summary>
        /// <param name="s">String value to be written.</param>
        public override void Write(string s)
        {
            this.writer.Write(s);
        }

        /// <summary>
        /// Writes the given char value to the underlying writer.
        /// </summary>
        /// <param name="value">Char value to be written.</param>
        public override void Write(char value)
        {
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        public override void WriteLine()
        {
        }
    }
}
