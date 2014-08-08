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
    using System.Reflection;

    /// <summary>
    /// Use this attribute on a DataService service operation method 
    /// to indicate than the IQueryable returned should contain a single element.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SingleResultAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.SingleResultAttribute" /> class. </summary>
        public SingleResultAttribute()
        {
        }
    }
}
