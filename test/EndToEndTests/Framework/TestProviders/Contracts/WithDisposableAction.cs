//---------------------------------------------------------------------
// <copyright file="WithDisposableAction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts
{
    using System;

    /// <summary>
    /// Used to enable a WithFunc that does something and can revert this on disposal
    /// </summary>
    internal class WithDisposableAction : IDisposable
    {
        private Action action;

        public WithDisposableAction(Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// Disposes via the action delegate
        /// </summary>
        public void Dispose()
        {
            this.action();
        }
    }
}
