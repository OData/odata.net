//---------------------------------------------------------------------
// <copyright file="DataSourceManager.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Caching;
    using System.ServiceModel;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Hosting;

    public static class DataSourceManager
    {
        private const string CurrentDataSourceTypeRequestKey = "MicrosoftODataDataSourceType";
        private const string CurrentDataSourceInstanceSessionKeyFormat = "MicrosoftODataDataSource_{0}";

        public static IODataDataSource GetCurrentDataSource()
        {
            var dataSourceType = default(Type);

            if (HostingEnvironment.IsHosted)
            {
                dataSourceType = (Type)HttpContext.Current.Items[CurrentDataSourceTypeRequestKey];
            }
            else
            {
                dataSourceType = (Type)Thread.GetData(Thread.GetNamedDataSlot(CurrentDataSourceTypeRequestKey));
            }

            return GetCurrentDataSource(dataSourceType, () => (IODataDataSource)Utility.QuickCreateInstance(dataSourceType));
        }

        public static TDataSource GetCurrentDataSource<TDataSource>()
            where TDataSource : class, IODataDataSource, new()
        {
            return (TDataSource)GetCurrentDataSource(typeof(TDataSource), () => new TDataSource());
        }

        public static void EnsureCurrentDataSource<TDataSource>()
            where TDataSource : class, IODataDataSource, new()
        {
            if (HostingEnvironment.IsHosted)
            {
                HttpContext.Current.Items[CurrentDataSourceTypeRequestKey] = typeof(TDataSource);
            }
            else
            {
                if (Thread.GetData(Thread.GetNamedDataSlot(CurrentDataSourceTypeRequestKey)) == null)
                {
                    Thread.SetData(Thread.GetNamedDataSlot(CurrentDataSourceTypeRequestKey), typeof(TDataSource));
                    OperationContext.Current.OperationCompleted += delegate { Thread.SetData(Thread.GetNamedDataSlot(CurrentDataSourceTypeRequestKey), null); };
                }
            }

            // call this method to ensure data source creation
            GetCurrentDataSource(typeof(TDataSource), () => new TDataSource());
        }

        private static IODataDataSource GetCurrentDataSource(Type dataSourceType, Func<IODataDataSource> dataSourceCreator)
        {
            var result = default(IODataDataSource);

            var dataSourceSessionKey = string.Format(CurrentDataSourceInstanceSessionKeyFormat, dataSourceType.AssemblyQualifiedName);

            if (HostingEnvironment.IsHosted)
            {
                // here is deployed on IIS based host, e.g. Azure or IISExpress

                var session = HttpContext.Current.Session;
                var dataSource = (IODataDataSource)session[dataSourceSessionKey];
                if (dataSource == null)
                {
                    dataSource = dataSourceCreator();
                    session[dataSourceSessionKey] = dataSource;
                    dataSource.Reset();
                    dataSource.Initialize();
                }
                result = dataSource;
            }
            else
            {
                // here is deployed on Non-IIS based host, e.g. unit test

                if (MemoryCache.Default.Contains(dataSourceSessionKey))
                {
                    result = (IODataDataSource)MemoryCache.Default.Get(dataSourceSessionKey);
                }
                else
                {
                    var dataSource = dataSourceCreator();
                    MemoryCache.Default.Add(dataSourceSessionKey, dataSource, new CacheItemPolicy());
                    dataSource.Reset();
                    dataSource.Initialize();
                    result = dataSource;
                }
            }

            return result;
        }
    }
}
