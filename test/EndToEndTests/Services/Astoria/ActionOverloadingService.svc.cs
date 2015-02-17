//---------------------------------------------------------------------
// <copyright file="ActionOverloadingService.svc.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ActionOverloadingService
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Microsoft.Test.OData.Framework.TestProviders.Common;
    using Microsoft.Test.OData.Framework.TestProviders.Contracts.DataOracle;
    using Microsoft.Test.OData.Framework.TestProviders.OptionalProviders;
    using Microsoft.Test.OData.Framework.TestProviders.Reflection;
    using Microsoft.Test.OData.Services.AstoriaDefaultService;

    [System.ServiceModel.ServiceBehaviorAttribute(IncludeExceptionDetailInFaults = true)]
    public class ActionOverloadingService : DataService<DefaultContainer>, IDataServiceDataSourceCreator, System.IServiceProvider
    {
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.AcceptSpatialLiteralsInQuery = false;
            config.DataServiceBehavior.MaxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;
            config.SetEntitySetAccessRule("*", Microsoft.OData.Service.EntitySetRights.All);
            config.SetServiceActionAccessRule("RetrieveProduct", Microsoft.OData.Service.ServiceActionRights.Invoke);
            config.SetServiceActionAccessRule("IncreaseSalaries", Microsoft.OData.Service.ServiceActionRights.Invoke);
            config.SetServiceActionAccessRule("UpdatePersonInfo", Microsoft.OData.Service.ServiceActionRights.Invoke);
            config.SetServiceActionAccessRule("IncreaseEmployeeSalary", Microsoft.OData.Service.ServiceActionRights.Invoke);

            config.SetServiceOperationAccessRule("*", Microsoft.OData.Service.ServiceOperationRights.All);
        }

        public virtual object GetService(System.Type serviceType)
        {
            if (((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceStreamProvider2)) || (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceStreamProvider))))
            {
                return new InMemoryStreamProvider<ReferenceEqualityComparer>();
            }

            if (((serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider)) ||
                 (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider2)) ||
                 (serviceType == typeof(Microsoft.OData.Service.IUpdatable))))
            {
                return this.CurrentDataSource;
            }

            if (serviceType == typeof(Microsoft.OData.Service.Providers.IDataServiceActionProvider))
            {
                return new ActionOverloadingTestActionProvider(this);
            }

            return null;
        }

        //
        // Service operation, actions with the same name and different binding types
        //
        [WebInvoke()]
        public int RetrieveProduct()
        {
            return this.CurrentDataSource.Product.First().ProductId;
        }

        public int RetrieveProduct(OrderLine orderLine)
        {
            return orderLine.ProductId;
        }

        public int RetrieveProduct(Product product)
        {
            return product.ProductId;
        }

        //
        // Collection bound actions with the same name
        //
        public void IncreaseSalaries(IEnumerable<Employee> employees, int n)
        {
            foreach (var employee in employees)
            {
                employee.Salary += n;
            }
        }

        public void IncreaseSalaries(IEnumerable<SpecialEmployee> specialEmployees, int n)
        {
            foreach (var specialEmployee in specialEmployees)
            {
                specialEmployee.Salary += n;
            }
        }

        //
        // Actions with the same name and base/derived type binding parameters
        //
        public void UpdatePersonInfo()
        {
            this.CurrentDataSource.Person.First().Name += "[UpdataPersonName]";
        }

        public void UpdatePersonInfo(Person person)
        {
            person.Name += "[UpdataPersonName]";
        }

        public void UpdatePersonInfo(Employee employee)
        {
            employee.Title += "[UpdateEmployeeTitle]";
        }

        public void UpdatePersonInfo(SpecialEmployee specialEmployee)
        {
            specialEmployee.Title += "[UpdateSpecialEmployeeTitle]";
        }

        public void UpdatePersonInfo(Contractor contractor)
        {
            contractor.JobDescription += "[UpdateContractorJobDescriptor]";
        }

        //
        // Actions with the same name, base/derived type binding parameters, different non-binding parameters
        //
        public bool IncreaseEmployeeSalary(Employee employee, int n)
        {
            employee.Salary += n;
            return true;
        }

        public int IncreaseEmployeeSalary(SpecialEmployee specialEmployee)
        {
            specialEmployee.Salary += 1;
            return specialEmployee.Salary;
        }

        object IDataServiceDataSourceCreator.CreateDataSource()
        {
            return this.CreateDataSource();
        }
    }
}
