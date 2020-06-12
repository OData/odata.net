//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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
