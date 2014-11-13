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

    /// <summary>
    /// Use this attribute on a DataService method to indicate than this method should be invoked to intercept queries.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class QueryInterceptorAttribute : Attribute
    {
        /// <summary>Entity set name that the method filters.</summary>
        private readonly string entitySetName;

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.Services.QueryInterceptorAttribute" /> class for the entity set specified by the <paramref name="entitySetName" /> parameter.</summary>
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
