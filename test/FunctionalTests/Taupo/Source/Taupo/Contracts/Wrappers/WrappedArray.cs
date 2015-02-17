//---------------------------------------------------------------------
// <copyright file="WrappedArray.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    /// <summary>
    /// Wraps the array.
    /// </summary>
    public abstract class WrappedArray : WrappedObject
    {
        /// <summary>
        /// Initializes a new instance of the WrappedArray class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="wrappedInstance">The wrapped instance.</param>
        protected WrappedArray(IWrapperScope wrapperScope, object wrappedInstance)
            : base(wrapperScope, wrappedInstance)
        {
        }
    }
}