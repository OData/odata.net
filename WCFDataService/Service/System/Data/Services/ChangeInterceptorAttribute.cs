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

    /// <summary>The <see cref="T:System.Data.Services.ChangeInterceptorAttribute" /> on a method is used to process updates on the specified entity set name.</summary>
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
