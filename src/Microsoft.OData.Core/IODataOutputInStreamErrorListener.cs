//---------------------------------------------------------------------
// <copyright file="IODataOutputInStreamErrorListener.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// An interface that allows the implementations of the writers to get notified if an in-stream error is to be written.
    /// </summary>
    internal interface IODataOutputInStreamErrorListener
    {
        /// <summary>
        /// This method notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        void OnInStreamError();

        /// <summary>
        /// This method asynchronously notifies the listener, that an in-stream error is to be written.
        /// </summary>
        /// <remarks>
        /// This listener can choose to fail, if the currently written payload doesn't support in-stream error at this position.
        /// If the listener returns, the writer should not allow any more writing, since the in-stream error is the last thing in the payload.
        /// </remarks>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task OnInStreamErrorAsync();
    }
}
