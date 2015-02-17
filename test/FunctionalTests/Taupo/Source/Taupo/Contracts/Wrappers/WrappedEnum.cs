//---------------------------------------------------------------------
// <copyright file="WrappedEnum.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    /// <summary>
    /// Wraps the enumeration.
    /// </summary>
    public class WrappedEnum : WrappedObject
    {
        /// <summary>
        /// Initializes a new instance of the WrappedEnum class.
        /// </summary>
        /// <param name="wrapperScope">The wrapper scope.</param>
        /// <param name="product">The product.</param>
        public WrappedEnum(IWrapperScope wrapperScope, object product)
            : base(wrapperScope, product)
        {
        }
    }
}