//---------------------------------------------------------------------
// <copyright file="PerfServiceOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PerfService
{
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class PerfServiceOperationProvider : ODataReflectionOperationProvider
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<PerfServiceDataSource>();
            dataSource.Reset();
            dataSource.Initialize();
        }
    }
}
