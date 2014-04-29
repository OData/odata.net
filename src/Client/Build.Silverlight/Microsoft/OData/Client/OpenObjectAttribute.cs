//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if ASTORIA_OPEN_OBJECT
namespace Microsoft.OData.Client
{
    using System;

    /// <summary>
    /// Attribute to be annotated on class to designate the name of the instance property to store name-value pairs.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class OpenObjectAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the instance property returning an IDictionary&lt;string,object&gt;.
        /// </summary>
        private readonly string openObjectPropertyName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="openObjectPropertyName">The name of the instance property returning an IDictionary&lt;string,object&gt;.</param>
        public OpenObjectAttribute(string openObjectPropertyName)
        {
            Util.CheckArgumentNotEmpty(openObjectPropertyName, "openObjectPropertyName");
            this.openObjectPropertyName = openObjectPropertyName;
        }

        /// <summary>
        /// The name of the instance property returning an IDictionary&lt;string,object&gt;.
        /// </summary>
        public string OpenObjectPropertyName
        {
            get { return this.openObjectPropertyName; }
        }
    }
}
#endif
