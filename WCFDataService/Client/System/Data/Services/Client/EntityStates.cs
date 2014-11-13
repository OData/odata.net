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

namespace System.Data.Services.Client
{
    /// <summary>
    /// Describes the insert/update/delete state of an entity or link.
    /// </summary>
    /// <remarks>
    /// Deleting an inserted resource will detach it.
    /// After SaveChanges, deleted resources will become detached and Added &amp; Modified resources will become unchanged.
    /// </remarks>
    [System.Flags()] 
    public enum EntityStates
    {
        /// <summary>
        /// The resource is not tracked by the context.
        /// </summary>
        Detached = 0x00000001,

        /// <summary>
        /// The resource is tracked by a context with no changes.
        /// </summary>
        Unchanged = 0x00000002,

        /// <summary>
        /// The resource is tracked by a context for insert.
        /// </summary>
        Added = 0x00000004,

        /// <summary>
        /// The resource is tracked by a context for deletion.
        /// </summary>
        Deleted = 0x00000008,

        /// <summary>
        /// The resource is tracked by a context for update.
        /// </summary>
        Modified = 0x00000010
    }
}
