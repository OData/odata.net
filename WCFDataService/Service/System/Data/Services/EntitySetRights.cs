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

    /// <summary>
    /// Provides values to describe the kind of thing targetted by a 
    /// client request.
    /// </summary>
    [Flags]
    public enum EntitySetRights
    {
        /// <summary>Specifies no rights on this resource.</summary>
        None = 0,

        /// <summary>Specifies the right to read one resource per request.</summary>
        ReadSingle = 1,

        /// <summary>Specifies the right to read multiple resources per request.</summary>
        ReadMultiple = 2,

        /// <summary>Specifies the right to append new resources to the container.</summary>
        WriteAppend = 4,

        /// <summary>Specifies the right to update existing resource in the container.</summary>
        WriteReplace = 8,

        /// <summary>Specifies the right to delete existing resource in the container.</summary>
        WriteDelete = 16,

        /// <summary>Specifies the right to update existing resource in the container.</summary>
        WriteMerge = 32,

        /// <summary>Specifies the right to read single or multiple resources in a single request.</summary>
        AllRead = ReadSingle | ReadMultiple,

        /// <summary>Specifies the right to append, delete or update resources in the container.</summary>
        AllWrite = WriteAppend | WriteDelete | WriteReplace | WriteMerge,

        /// <summary>Specifies all rights to the container.</summary>
        All = AllRead | AllWrite,
    }
}
