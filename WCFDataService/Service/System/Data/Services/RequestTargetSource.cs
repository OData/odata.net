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

#if ODATALIB
namespace Microsoft.Data.OData.Query
#else
namespace System.Data.Services
#endif
{
    /// <summary>
    /// Provides values to describe the source of the request results.
    /// </summary>
    internal enum RequestTargetSource
    {
        /// <summary>No source for data.</summary>
        /// <remarks>
        /// This value is seen when a source hasn't been determined yet, or
        /// when the source is intrinsic to the system - eg a metadata request.
        /// </remarks>
        None,

        /// <summary>An entity set provides the data.</summary>
        EntitySet,

        /// <summary>A service operation provides the data.</summary>
        ServiceOperation,

        /// <summary>A property of an entity or a complex object provides the data.</summary>
        Property,
    }
}
