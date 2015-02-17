//---------------------------------------------------------------------
// <copyright file="ActionOverloadingTestActionProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ActionOverloadingService
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts;
    using Microsoft.Test.OData.Framework.TestProviders.OptionalProviders;

    public class ActionOverloadingTestActionProvider : TestDataServiceActionProvider, IDataServiceActionResolver
    {
        public ActionOverloadingTestActionProvider(object dataServiceInstance)
            : base(dataServiceInstance)
        {
        }

        public bool TryResolveServiceAction(DataServiceOperationContext context, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction)
        {
            string actionName = resolverArgs.ServiceActionName;
            IEnumerable<ServiceAction> possibleMatches = this.GetServiceActions(context).Where(a => a.Name == actionName);
            if (possibleMatches.Count() == 1)
            {
                serviceAction = possibleMatches.Single();
            }
            else if (resolverArgs.BindingType == null)
            {
                // unbound action
                serviceAction = possibleMatches.SingleOrDefault(a => a.BindingParameter == null);
            }
            else
            {
                serviceAction = possibleMatches.SingleOrDefault(a => a.BindingParameter != null && a.BindingParameter.ParameterType.FullName == resolverArgs.BindingType.FullName);
            }

            return serviceAction != null;
        }

        protected override IEnumerable<ServiceAction> LoadServiceActions(IDataServiceMetadataProvider dataServiceMetadataProvider)
        {
            ResourceType productType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Product", out productType);

            ResourceType orderLineType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.OrderLine", out orderLineType);

            ResourceType personType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Person", out personType);

            ResourceType employeeType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Employee", out employeeType);

            ResourceType specialEmployeeType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.SpecialEmployee", out specialEmployeeType);

            ResourceType contractorType;
            dataServiceMetadataProvider.TryResolveResourceType("Microsoft.Test.OData.Services.AstoriaDefaultService.Contractor", out contractorType);

            //
            // actions with the same name and non-related binding types
            //
            var RetrieveProductActionProduct = new ServiceAction(
                "RetrieveProduct",
                ResourceType.GetPrimitiveResourceType(typeof(int)),
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("product", productType), 
                });
            RetrieveProductActionProduct.SetReadOnly();
            yield return RetrieveProductActionProduct;

            var RetrieveProductActionOrderLine = new ServiceAction(
                "RetrieveProduct",
                ResourceType.GetPrimitiveResourceType(typeof(int)),
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("orderLine", orderLineType), 
                });
            RetrieveProductActionOrderLine.SetReadOnly();
            yield return RetrieveProductActionOrderLine;

            //
            // Collection bound actions with the same name
            //
            var increaseSalariesActionEmployee = new ServiceAction(
                "IncreaseSalaries",
                null,
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("employees", ResourceType.GetEntityCollectionResourceType(employeeType)),
                    new ServiceActionParameter("n", ResourceType.GetPrimitiveResourceType(typeof(int))),
                });
            increaseSalariesActionEmployee.SetReadOnly();
            yield return increaseSalariesActionEmployee;

            var increaseSalariesActionSpecialEmployee = new ServiceAction(
                "IncreaseSalaries",
                null,
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("specialEmployees", ResourceType.GetEntityCollectionResourceType(specialEmployeeType)),
                    new ServiceActionParameter("n", ResourceType.GetPrimitiveResourceType(typeof(int))),
                });
            increaseSalariesActionSpecialEmployee.SetReadOnly();
            yield return increaseSalariesActionSpecialEmployee;

            //
            // Actions with the same name and base/derived type binding parameters
            //
            var updatePersonInfoAction = new ServiceAction(
                "UpdatePersonInfo",
                null,
                null,
                OperationParameterBindingKind.Never,
                null);
            updatePersonInfoAction.SetReadOnly();
            yield return updatePersonInfoAction;

            var updatePersonInfoActionPerson = new ServiceAction(
                "UpdatePersonInfo",
                null,
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("person", personType), 
                });
            updatePersonInfoActionPerson.SetReadOnly();
            yield return updatePersonInfoActionPerson;

            var updatePersonInfoActionEmployee = new ServiceAction(
                "UpdatePersonInfo",
                null,
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("employee", employeeType), 
                });
            updatePersonInfoActionEmployee.SetReadOnly();
            yield return updatePersonInfoActionEmployee;

            var updatePersonInfoActionSpecialEmployee = new ServiceAction(
                "UpdatePersonInfo",
                null,
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("specialEmployee", specialEmployeeType), 
                });
            updatePersonInfoActionSpecialEmployee.SetReadOnly();
            yield return updatePersonInfoActionSpecialEmployee;

            var updatePersonInfoActionContractor = new ServiceAction(
                "UpdatePersonInfo",
                null,
                null,
                OperationParameterBindingKind.Always,
                new[]
                {
                    new ServiceActionParameter("contractor", contractorType), 
                });
            updatePersonInfoActionContractor.SetReadOnly();
            yield return updatePersonInfoActionContractor;

            //
            // Actions with the same name, base/derived type binding parameters, different non-binding parameters
            //
            var increaseEmployeeSalaryActionEmployee = new ServiceAction(
                "IncreaseEmployeeSalary",
                ResourceType.GetPrimitiveResourceType(typeof(bool)),
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("employee", employeeType),
                    new ServiceActionParameter("n", ResourceType.GetPrimitiveResourceType(typeof(int))),
                });
            increaseEmployeeSalaryActionEmployee.SetReadOnly();
            yield return increaseEmployeeSalaryActionEmployee;

            var increaseEmployeeSalaryActionSpecialEmployee = new ServiceAction(
                "IncreaseEmployeeSalary",
                ResourceType.GetPrimitiveResourceType(typeof(int)),
                null,
                OperationParameterBindingKind.Sometimes,
                new[]
                {
                    new ServiceActionParameter("specialEmployee", specialEmployeeType),
                });
            increaseEmployeeSalaryActionSpecialEmployee.SetReadOnly();
            yield return increaseEmployeeSalaryActionSpecialEmployee;
        }
    }
}