//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    using System;
    using System.Diagnostics;

    /// <summary>The <see cref="T:Microsoft.OData.Service.ChangeInterceptorAttribute" /> on a method is used to process updates on the specified entity set name.</summary>
    /// <remarks>Use this attribute on a DataService method to indicate that this method should be invoked with data changes.</remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ChangeInterceptorAttribute : Attribute
    {
        /// <summary>Container name that the method filters.</summary>
        private readonly string entitySetName;

        /// <summary>Creates a new change interceptor for an entity set specified by the parameter <paramref name="entitySetName" />.</summary>
        /// <param name="entitySetName">The name of the entity set that contains the entity to which the interceptor applies.</param>
        public ChangeInterceptorAttribute(string entitySetName)
        {
            if (entitySetName == null)
            {
                throw Error.ArgumentNull("entitySetName");
            }

            this.entitySetName = entitySetName;
        }

        /// <summary>Gets the name of the entity set to which the interceptor applies.</summary>
        /// <returns>The string value that represents entity set name.</returns>
        public string EntitySetName
        {
            [DebuggerStepThrough]
            get { return this.entitySetName; }
        }
    }
}
