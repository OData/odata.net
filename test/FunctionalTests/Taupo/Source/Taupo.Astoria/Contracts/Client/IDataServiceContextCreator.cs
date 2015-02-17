//---------------------------------------------------------------------
// <copyright file="IDataServiceContextCreator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Interface allows developers to ask for a machine that can be used to 
    /// Host a DataService
    /// </summary>
    [ImplementationSelector("DataServiceContextCreator", DefaultImplementation = "Default")]
    public interface IDataServiceContextCreator
    {
        /// <summary>
        /// Interface to create DSC
        /// </summary>
        /// <param name="scope">DataServiceContext TrackingScope</param>
        /// <param name="dataServiceContextType">The type of the DataServiceContext instance to be created</param>
        /// <param name="serviceBaseUri">service BaseUri</param>
        /// <returns>Wrapped DataServiceContext</returns>
        WrappedDataServiceContext CreateContext(IWrapperScope scope, Type dataServiceContextType, Uri serviceBaseUri);
    }
}