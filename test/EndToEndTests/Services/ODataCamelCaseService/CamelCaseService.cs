//---------------------------------------------------------------------
// <copyright file="CamelCaseService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CamelCaseService : ODataService<CamelCaseDataSource>
    {
    }
}
