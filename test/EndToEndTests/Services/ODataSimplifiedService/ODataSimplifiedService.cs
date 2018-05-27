//---------------------------------------------------------------------
// <copyright file="ODataSimplifiedService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using Microsoft.Test.OData.Services.ODataWCFService.Handlers;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class ODataSimplifiedService : ODataService<ODataSimplifiedDataSource>
    {
    }
}
