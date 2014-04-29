//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
