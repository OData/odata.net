//---------------------------------------------------------------------
// <copyright file="UrlModifyingService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.UrlModifyingService
{
    using System;
    using Microsoft.OData.Service;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    [System.ServiceModel.ServiceBehaviorAttribute(IncludeExceptionDetailInFaults = true)]
    public class UrlModifyingService : Service
    {
        public ProcessRequestArgs StoredArgs { get; private set; }
                
        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            if (args.RequestUri.AbsoluteUri.Contains("RemapPath"))
            {
                args.RequestUri = new Uri(args.ServiceUri.AbsoluteUri + "Customer");
            }
            else if (args.RequestUri.AbsoluteUri.Contains("RemapBase"))
            {
                args.ServiceUri = new Uri("http://potato" + args.ServiceUri.AbsoluteUri.Substring(args.ServiceUri.AbsoluteUri.IndexOf(':', 5)));
                args.RequestUri = new Uri(args.ServiceUri.AbsoluteUri + "Customer");
            }
            else if (args.RequestUri.AbsoluteUri.Contains("RemapBaseAndPathSeparately"))
            {
                // Service Uri and Request Uri have different bases
                args.RequestUri = new Uri(args.ServiceUri.AbsoluteUri + "Customer");
                args.ServiceUri = new Uri("http://potato" + args.ServiceUri.AbsoluteUri.Substring(args.ServiceUri.AbsoluteUri.IndexOf(':', 5)));   
            }
            else if (args.RequestUri.AbsoluteUri.Contains("BasesDontMatchFail"))
            {
                // Service Uri and Request Uri have different bases
                args.RequestUri = new Uri("http://potato:9090/DontFailMeService/Customer");
                args.ServiceUri = new Uri("http://potato:9090/FailMeService");
            }
            else if (args.RequestUri.AbsoluteUri.Contains("$batch"))
            {
                args.ServiceUri = new Uri("http://potato" + args.ServiceUri.AbsoluteUri.Substring(args.ServiceUri.AbsoluteUri.IndexOf(':', 5)));
            }
            else if (args.RequestUri.AbsoluteUri.Contains("BatchRequest1"))
            {
                args.RequestUri = new Uri(args.ServiceUri.AbsoluteUri + "Customer");
            }
            else if (args.RequestUri.AbsoluteUri.Contains("BatchRequest2"))
            {
                args.RequestUri = new Uri(args.ServiceUri.AbsoluteUri + "Person");
            }
            else if (args.RequestUri.AbsoluteUri.Contains("BatchRequest3"))
            {
                args.ServiceUri = new Uri("http://notpotato:9090/yummy");
                args.RequestUri = new Uri("http://notpotato:9090/yummy/Customer");
            }
            else if (args.RequestUri.AbsoluteUri.Contains("Person"))
            {
                args.RequestUri = new Uri(args.RequestUri.AbsoluteUri + "?$top=5");
            }
            
            base.OnStartProcessingRequest(args);            
        }
    }
}
