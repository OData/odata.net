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

    /// <summary>
    /// Use this attribute on a DataService method to indicate than this method should be invoked to intercept queries.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class QueryInterceptorAttribute : Attribute
    {
        /// <summary>Entity set name that the method filters.</summary>
        private readonly string entitySetName;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Service.QueryInterceptorAttribute" /> class for the entity set specified by the <paramref name="entitySetName" /> parameter.</summary>
        /// <param name="entitySetName">The name of the entity set that contains the entity to which the interceptor applies.</param>
        public QueryInterceptorAttribute(string entitySetName)
        {
            this.entitySetName = WebUtil.CheckArgumentNull(entitySetName, "entitySetName");
        }

        /// <summary>Gets the name of the entity set that contains the entity to which the interceptor applies.</summary>
        /// <returns>A string that indicates the name of the entity set that contains the entity to which the interceptor applies.</returns>
        public string EntitySetName
        {
            [DebuggerStepThrough]
            get { return this.entitySetName; }
        }
    }
}
