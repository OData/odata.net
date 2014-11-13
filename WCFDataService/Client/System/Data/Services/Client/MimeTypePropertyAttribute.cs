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

namespace System.Data.Services.Client
{
    using System;

    /// <summary>
    /// This attribute indicates another property in the same type that
    /// contains the MIME type that should be used for the data contained
    /// in the property this attribute is applied to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MimeTypePropertyAttribute : Attribute
    {
        /// <summary>The name of the property that contains the data</summary>
        private readonly string dataPropertyName;

        /// <summary>The name of the property that contains the mime type</summary>
        private readonly string mimeTypePropertyName;

        /// <summary>Creates a new instance of the MimeTypePropertyAttribute.</summary>
        /// <param name="dataPropertyName">A string that contains the name of the new property attribute.</param>
        /// <param name="mimeTypePropertyName">A string that contains the Mime type of the new property attribute.</param>
        public MimeTypePropertyAttribute(string dataPropertyName, string mimeTypePropertyName)
        {
            this.dataPropertyName = dataPropertyName;
            this.mimeTypePropertyName = mimeTypePropertyName;
        }

        /// <summary>Gets the name of the MimeTypePropertyAttribute.</summary>
        /// <returns>A string that contains the name of the property attribute. </returns>
        public string DataPropertyName
        {
            get { return this.dataPropertyName; }
        }

        /// <summary>Gets the Mime type of the MimeTypePropertyAttribute</summary>
        /// <returns>A string that contains the Mime type of the property attribute. </returns>
        public string MimeTypePropertyName
        {
            get { return this.mimeTypePropertyName; }
        }
    }
}
