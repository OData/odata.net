//---------------------------------------------------------------------
// <copyright file="WrappedArray`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    /// <summary>
    /// Wraps the array instance.
    /// </summary>
    /// <typeparam name="TElement">Type of the wrapper representing array element</typeparam>
    public class WrappedArray<TElement> : WrappedArray
        where TElement : WrappedObject
    {
        /// <summary>
        /// Initializes a new instance of the WrappedArray class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="product">The product.</param>
        public WrappedArray(IWrapperScope wrapperScope, object product)
            : base(wrapperScope, product)
        {
        }
    }
}