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
