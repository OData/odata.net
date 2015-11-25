//---------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TestService
{
    using System;
    using System.ServiceModel.Activation;
    using System.Web;
    using System.Web.Routing;
    using Microsoft.Test.OData.Services.ODataWCFService.Extensions;
    using Microsoft.Test.OData.Services.ODataWCFService.Services;

    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var factories = ExtensionManager.Container.GetExportedValues<IODataServiceDescriptor>();

          foreach (var factory in factories)
            {
                RouteTable.Routes.Add(new ServiceRoute(factory.ServiceName, new WebServiceHostFactory(), factory.ServiceType));
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}