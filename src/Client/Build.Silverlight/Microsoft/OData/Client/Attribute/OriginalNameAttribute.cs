//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.OData.Client
{
    using System;

    /// <summary>Denotes the original name of a variable defined in metadata. </summary>
    public sealed class OriginalNameAttribute : Attribute
    {
        /// <summary>The original name.</summary>
        private readonly string originalName;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.OriginalNameAttribute" /> class. </summary>
        /// <param name="originalName">The string that contains original name of the variable.</param>
        public OriginalNameAttribute(string originalName)
        {
            this.originalName = originalName;
        }

        /// <summary>Gets the orginal names of the variable.</summary>
        /// <returns>String value that contains the original name of the variable. </returns>
        public string OriginalName
        {
            get { return this.originalName; }
        }
    }
}
