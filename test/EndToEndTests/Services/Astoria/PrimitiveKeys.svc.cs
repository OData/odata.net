//---------------------------------------------------------------------
// <copyright file="PrimitiveKeys.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PrimitiveKeysService
{
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.ServiceModel;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class ReflectionService : DataService<TestContext>
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
            config.DataServiceBehavior.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.SetEntitySetPageSize("*", 2);
            config.DataServiceBehavior.IncludeAssociationLinksInResponse = true;
        }
    }
}
