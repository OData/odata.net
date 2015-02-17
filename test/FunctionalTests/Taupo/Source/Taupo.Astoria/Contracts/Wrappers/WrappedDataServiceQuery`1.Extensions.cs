//---------------------------------------------------------------------
// <copyright file="WrappedDataServiceQuery`1.Extensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Wrappers
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <content>
    /// Hand-written extensions for the generated <see cref="WrappedDataServiceQuery"/> provided for better usability.
    /// </content>
    public partial class WrappedDataServiceQuery<TElement>
      where TElement : WrappedObject
    {
        /// <summary>
        /// Wrapper for BeginExecute() which automatically catches all exceptions in the action
        /// which processes results.
        /// </summary>
        /// <param name="continuation">The asynchronous continuation to invoke when there is a failure.</param>
        /// <param name="action">The action which processes results.</param>
        /// <returns>The original <see cref="IAsyncResult" /> returned by <see cref="DataServiceQuery{TElement}.BeginExecute"/></returns>
        public IAsyncResult BeginExecute(IAsyncContinuation continuation, Action<IAsyncResult> action)
        {
            return this.BeginExecute(result => AsyncHelpers.CatchErrors(continuation, () => action(result)), null);
        }
    }
}
