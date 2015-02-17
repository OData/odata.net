//---------------------------------------------------------------------
// <copyright file="TypeDefinitionOperationProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using microsoft.odata.sampleService.models.typedefinition;

    public class TypeDefinitionOperationProvider : ODataReflectionOperationProvider
    {
        public void ResetDataSource()
        {
            var dataSource = DataSourceManager.GetCurrentDataSource<TypeDefinitionDataSource>();
            dataSource.Reset();
            dataSource.Initialize();
        }

        string GetFullName(Person person, string nickname)
        {
            return person.FirstName + " (" + nickname + ") " + person.LastName;
        }

        UInt64 ExtendLifeTime(Product product, UInt32 seconds)
        {
            product.LifeTimeInSeconds += seconds;
            return product.LifeTimeInSeconds;
        }
    }
}
