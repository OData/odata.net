//---------------------------------------------------------------------
// <copyright file="PluggableFormatService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PluggableFormat
{
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.Handlers;

    /// <summary>
    /// The class implements an OData service sample using pluggable format.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class PluggableFormatService : ODataService<PluggableFormatServiceDataSource>
    {
        public override RootRequestHandler CreateRootRequestHandler(HttpMethod method, PluggableFormatServiceDataSource dataSource)
        {
            return new PluggableFormatRequestHandler(method, dataSource);
        }
    }
}
