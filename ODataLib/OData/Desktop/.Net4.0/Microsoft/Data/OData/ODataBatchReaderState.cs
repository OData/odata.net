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

namespace Microsoft.Data.OData
{
    /// <summary>Enumeration with all the states the batch reader can be in.</summary>
    public enum ODataBatchReaderState
    {
        /// <summary>The state the batch reader is in after having been created.</summary>
        Initial,

        /// <summary>The batch reader detected an operation.</summary>
        /// <remarks>In this state the start boundary, the request/response line 
        /// and the operation headers have already been read.</remarks>
        Operation,

        /// <summary>The batch reader detected the start of a change set.</summary>
        /// <remarks>In this state the start boundary and the change set 
        /// headers have already been read.</remarks>
        ChangesetStart,

        /// <summary>The batch reader completed reading a change set.</summary>
        ChangesetEnd,

        /// <summary>The batch reader completed reading the batch payload.</summary>
        /// <remarks>The batch reader cannot be used in this state anymore.</remarks>
        Completed,

        /// <summary>The batch reader encountered an error reading the batch payload.</summary>
        /// <remarks>The batch reader cannot be used in this state anymore.</remarks>
        Exception,
    }
}
