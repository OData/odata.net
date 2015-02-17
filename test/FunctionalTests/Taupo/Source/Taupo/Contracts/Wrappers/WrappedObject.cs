//---------------------------------------------------------------------
// <copyright file="WrappedObject.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base class for all wrapper objects.
    /// </summary>
    public class WrappedObject : IWrappedObject
    {
        /// <summary>
        /// Initializes a new instance of the WrappedObject class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="product">The product.</param>
        public WrappedObject(IWrapperScope wrapperScope, object product)
        {
            ExceptionUtilities.CheckObjectNotNull(wrapperScope, "wrapperScope");
            ExceptionUtilities.CheckObjectNotNull(product, "product");

            this.Scope = wrapperScope;
            this.Product = product;
        }

        /// <summary>
        /// Gets the product instance wrapped by this wrapper.
        /// </summary>
        public object Product { get; private set; }

        /// <summary>
        /// Gets the wrapper scope.
        /// </summary>
        public IWrapperScope Scope { get; private set; }

        /// <summary>
        /// Casts this instance to another wrapper type.
        /// </summary>
        /// <typeparam name="TWrapper">The type of the wrapper.</typeparam>
        /// <returns>New wrapper instance representing the same wrapped product instance.</returns>
        public TWrapper Cast<TWrapper>()
           where TWrapper : IWrappedObject
        {
            return this.Scope.Wrap<TWrapper>(this.Product);
        }
    }
}