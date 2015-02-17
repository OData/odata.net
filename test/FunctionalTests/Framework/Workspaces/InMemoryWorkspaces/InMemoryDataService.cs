//---------------------------------------------------------------------
// <copyright file="InMemoryDataService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Data.Test.Astoria.InMemoryLinq;

namespace System.Data.Test.Astoria
{
    using System.Linq;
    using System.ServiceModel.Web;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Reflection;

    public abstract class InMemoryTestWebService<T> : TestDataWebService<T> where T : InMemoryContext
    {
        [WebGet]
        public virtual void RestoreData()
        {
            InMemoryContext context = (InMemoryContext)this.CurrentDataSource;
            context.RestoreData();
        }
    }
}
