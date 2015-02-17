//---------------------------------------------------------------------
// <copyright file="AstoriaDefaultActionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.AstoriaDefaultService
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.OData.Framework.TestProviders.OptionalProviders;

    public class AstoriaDefaultActionProvider : TestDataServiceActionProvider
    {
        public AstoriaDefaultActionProvider(object dataServiceInstance) : base(dataServiceInstance)
        {
        }

        protected override IEnumerable<ServiceAction> LoadServiceActions(IDataServiceMetadataProvider dataServiceMetadataProvider)
        {
            ResourceType employeeType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Employee", out employeeType);
            ResourceType computerDetailType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.ComputerDetail", out computerDetailType);
            ResourceType computerType;          
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Computer", out computerType);
            ResourceType customerType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Customer", out customerType);
            ResourceType auditInfoType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.AuditInfo", out auditInfoType);            
            ResourceSet computerSet;
            dataServiceMetadataProvider.TryResolveResourceSet("Computer", out computerSet);
            var increaseSalaryAction = new ServiceAction(
                 "IncreaseSalaries",
                 null,
                 null,
                 OperationParameterBindingKind.Always,
                 new[]
                {
                    new ServiceActionParameter("employees", ResourceType.GetEntityCollectionResourceType(employeeType)),
                    new ServiceActionParameter("n", ResourceType.GetPrimitiveResourceType(typeof(int))),
                });

            increaseSalaryAction.SetReadOnly();

            yield return increaseSalaryAction;

            var sackEmployeeAction = new ServiceAction(
                "Sack",
                null,
                null,
                OperationParameterBindingKind.Always,
                new[]
                {
                    new ServiceActionParameter("employee", employeeType), 
                });

            sackEmployeeAction.SetReadOnly();

            yield return sackEmployeeAction;

            var getComputerAction = new ServiceAction(
                "GetComputer",
                computerType,
                OperationParameterBindingKind.Always,
                new[]
                {
                    new ServiceActionParameter("computer", computerType)
                },
                new ResourceSetPathExpression("computer"));

            getComputerAction.SetReadOnly();

            yield return getComputerAction;
             
            var changeCustomerAuditInfoAction = new ServiceAction(
               "ChangeCustomerAuditInfo",
               null,
               null,
               OperationParameterBindingKind.Always,
               new[]
                {
                
                    new ServiceActionParameter("customer", customerType), 
                    new ServiceActionParameter("auditInfo", auditInfoType),
                });

            changeCustomerAuditInfoAction.SetReadOnly();

            yield return changeCustomerAuditInfoAction;

             var resetComputerDetailsSpecificationsAction = new ServiceAction(
                "ResetComputerDetailsSpecifications",
                null,
                null,
                OperationParameterBindingKind.Always,
                new[]
                {
                    new ServiceActionParameter("computerDetail", computerDetailType), 
                    new ServiceActionParameter("specifications", ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(string)))),
                    new ServiceActionParameter("purchaseTime", ResourceType.GetPrimitiveResourceType(typeof(DateTimeOffset)))
                });

            resetComputerDetailsSpecificationsAction.SetReadOnly();

            yield return resetComputerDetailsSpecificationsAction;
        }
    }
}
