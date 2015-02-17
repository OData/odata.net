//---------------------------------------------------------------------
// <copyright file="IProxyObject.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Wrappers
{
    using System;

    /// <summary>
    /// Contract for wrapping an object in a proxy class which implements the same API makes appropriate calls to the wrapping scope
    /// </summary>
    public interface IProxyObject : IWrappedObject, IDisposable
    {
    }
}