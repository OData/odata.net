//   OData .NET Libraries ver. 5.6.3
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

namespace System.Data.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Use this attribute on a DataService service operation method
    /// or a data object property to indicate than the type returned is 
    /// of a specific MIME type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MimeTypeAttribute : Attribute
    {
        /// <summary>Name of the attributed method or property.</summary>
        private readonly string memberName;

        /// <summary>MIME type for the attributed method or property.</summary>
        private readonly string mimeType;

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.MimeTypeAttribute" /> class. </summary>
        /// <param name="memberName">The name of the attribute.</param>
        /// <param name="mimeType">The MIME type of the attribute.</param>
        public MimeTypeAttribute(string memberName, string mimeType)
        {
            this.memberName = memberName;
            this.mimeType = mimeType;
        }

        /// <summary>Gets the name of the attribute.</summary>
        /// <returns>A string value that contains the name of the attribute. </returns>
        public string MemberName
        {
            get { return this.memberName; }
        }

        /// <summary>Gets the MIME type of a request.</summary>
        /// <returns>A string that contains the MIME type.</returns>
        public string MimeType
        {
            get { return this.mimeType; }
        }
    }
}
