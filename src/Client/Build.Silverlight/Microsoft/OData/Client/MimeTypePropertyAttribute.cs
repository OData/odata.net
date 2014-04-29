//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client
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
