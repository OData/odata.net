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
    /// <summary>Describes an action performed on a resource.</summary>
    /// <remarks>
    /// This enumeration has been patterned after the DataRowAction
    /// (http://msdn2.microsoft.com/en-us/library/system.data.datarowaction.aspx)
    /// enumeration (with a few less values).
    /// </remarks>
    [System.Flags]
    public enum UpdateOperations
    {
        /// <summary>The resource has not changed.</summary>
        None = 0x00,

        /// <summary>The resource has been added to a container.</summary>
        Add = 0x01,

        /// <summary>The resource has changed.</summary>
        Change = 0x02,

        /// <summary>The resource has been deleted from a container.</summary>
        Delete = 0x04,
    }
}
