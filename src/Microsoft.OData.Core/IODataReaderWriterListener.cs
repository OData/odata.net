//---------------------------------------------------------------------
// <copyright file="IODataReaderWriterListener.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary>
    /// An interface that allows the creator of a reader/writer to listen for status changes of the created reader/writer.
    /// </summary>
    internal interface IODataReaderWriterListener
    {
        /// <summary>
        /// This method notifies the implementer of this interface that the created reader is in Exception state.
        /// </summary>
        void OnException();

        /// <summary>
        /// This method notifies the implementer of this interface that the created reader is in Completed state.
        /// </summary>
        void OnCompleted();
    }
}
