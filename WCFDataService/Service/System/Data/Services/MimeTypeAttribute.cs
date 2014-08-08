//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
