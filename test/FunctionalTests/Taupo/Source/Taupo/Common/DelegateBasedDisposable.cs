//---------------------------------------------------------------------
// <copyright file="DelegateBasedDisposable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Implements IDisposable based on a delegate. Note that the delegate will only be called once regardless of how many times it is disposed.
    /// </summary>
    public sealed class DelegateBasedDisposable : IDisposable
    {
        private Action disposeFunc;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the DelegateBasedDisposable class
        /// </summary>
        /// <param name="disposeFunc">The delegate to use when disposing</param>
        public DelegateBasedDisposable(Action disposeFunc)
        {
            ExceptionUtilities.CheckArgumentNotNull(disposeFunc, "disposeFunc");
            this.disposeFunc = disposeFunc;
        }

        /// <summary>
        /// Disposes the object by calling the delegate given during initialization
        /// </summary>
        public void Dispose()
        {
            if (!this.disposed)
            {
                this.disposeFunc();
                this.disposed = true;
            }
        }
    }
}