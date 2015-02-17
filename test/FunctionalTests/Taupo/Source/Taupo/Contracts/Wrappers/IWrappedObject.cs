//---------------------------------------------------------------------
// <copyright file="IWrappedObject.cs" company="Microsoft">
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
    /// Basic interface implemented by all wrapped objects
    /// </summary>
    public interface IWrappedObject
    {
        /// <summary>
        /// Gets the product instance wrapped by this wrapper.
        /// </summary>
        object Product { get; }

        /// <summary>
        /// Gets the wrapper scope.
        /// </summary>
        IWrapperScope Scope { get; }
    }
}