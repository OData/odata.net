//---------------------------------------------------------------------
// <copyright file="IDataServiceContextTrackingScope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.Wrappers;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Wrappers;

    /// <summary>
    /// Contract for a wrapper scope which tracks DataServiceContext.
    /// </summary>
    [ImplementationSelector("DataServiceContextTrackingScope", DefaultImplementation = "Default", HelpText = "The wrapper scope which tracks DataServiceContext operations.")]
    public interface IDataServiceContextTrackingScope : IWrapperScope
    {
        /// <summary>
        /// Gets the data service context data.
        /// </summary>
        /// <param name="context">The context for which to get the data service context data.</param>
        /// <returns><see cref="DataServiceContextData"/> that represents data for the context.</returns>
        DataServiceContextData GetDataServiceContextData(WrappedDataServiceContext context);
    }
}
