//---------------------------------------------------------------------
// <copyright file="IDataServiceDataSourceCreator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Contracts.DataOracle
{
    /// <summary>
    /// Contract allowing the data-oracle implementations access to the data source of a data-service
    /// </summary>
    public interface IDataServiceDataSourceCreator
    {
        /// <summary>
        /// Creates the data source
        /// </summary>
        /// <returns>The data source</returns>
        object CreateDataSource();
    }
}
