//---------------------------------------------------------------------
// <copyright file="TestServiceHost_IServiceProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Data;
using System.Data.Objects;
using System.Threading;
using System.Linq.Expressions;
using System.Data.Test.Astoria.CallOrder;
using [[Namespace]];

namespace TestServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string uriBaseAddress = args[0];
            string waitHandleName = args[1];           

            Uri[] uriArray = { new Uri(uriBaseAddress) };

            using(WebServiceHost host = new WebServiceHost(typeof([[ServiceName]]), uriArray))
            {
                try
                {
                    EventWaitHandle waitHandle = EventWaitHandle.OpenExisting(waitHandleName);

                    host.Open();
                    waitHandle.WaitOne();
                    host.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An exception occurred:");
                    Console.WriteLine(ex.ToString());
                    host.Abort();
                    Console.ReadLine();
                }
            }
        }
    }

    [System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public partial class [[ServiceName]] : [[DataServiceClass]]<[[Namespace]].[[Context]]>, IServiceProvider
    {
        private [[Namespace]].[[Context]] service;

	    object IServiceProvider.GetService(Type serviceType)
	    {
            APICallLog.Current.ServiceProvider.GetService(serviceType);

            try
            {
    		    if( serviceType == typeof(IDataServiceMetadataProvider) || serviceType == typeof(IDataServiceQueryProvider) || serviceType == typeof(IUpdatable) || serviceType == typeof(IProjectionProvider) )
                {
                    if( service == null )
			            service = new [[Namespace]].[[Context]](this);

                    return service;
                }   
                else if(this is IDataServiceStreamProvider && serviceType == typeof(IDataServiceStreamProvider))
                    return this;
		
                return null;
            }
            finally
            {
                APICallLog.Current.Pop();
            }
	    }

        // [[Additional Code]]
    }

    // [[Global Additional Code]]
}
