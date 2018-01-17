//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.KeyAsSegmentService
{
    using Microsoft.OData.Service;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    [System.ServiceModel.ServiceBehaviorAttribute(IncludeExceptionDetailInFaults = true)]
    public class KeyAsSegmentService : Service
    {
        public new static void InitializeService(DataServiceConfiguration config)
        {
            Service.InitializeService(config);
            config.DataServiceBehavior.UrlKeyDelimiter = Microsoft.OData.Client.DataServiceUrlKeyDelimiter.Slash;
        }
    }
}
