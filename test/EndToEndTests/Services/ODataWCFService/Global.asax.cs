//---------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Web;

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System;
    using System.ServiceModel.Activation;
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

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, DELETE, OPTIONS, PATCH");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, If-Match");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }
    }
}
