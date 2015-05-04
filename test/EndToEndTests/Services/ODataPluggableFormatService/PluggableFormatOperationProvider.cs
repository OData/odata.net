//---------------------------------------------------------------------
// <copyright file="PluggableFormatOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System;
    using System.Linq;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    internal class PluggableFormatOperationProvider<T> : ODataReflectionOperationProvider
        where T : class, IODataDataSource, new()
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<T>();
            dataSource.Reset();
            dataSource.Initialize();
        }

        public void AddProduct(EntityCollection<Product> products, Product Value, bool Override)
        {
            var exist = products.SingleOrDefault(p => p.Id == Value.Id);
            if (exist != null && !Override)
            {
                throw new Exception("Same product id exists");
            }

            products.Remove(exist);
            products.Add(Value);
        }
    }
}
