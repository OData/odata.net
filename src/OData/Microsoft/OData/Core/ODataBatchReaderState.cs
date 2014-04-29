//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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
