//---------------------------------------------------------------------
// <copyright file="TripPinService.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService
{
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    /// <summary>
    /// The class implements an OData service using WCF as the host
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class TripPinService : ODataService<TripPinServiceDataSource>
    {
    }
}
