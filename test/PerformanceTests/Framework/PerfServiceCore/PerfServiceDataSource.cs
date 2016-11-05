//---------------------------------------------------------------------
// <copyright file="PerfServiceDataSource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PerfService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    public class PerfServiceDataSource : ODataReflectionDataSource
    {
        public PerfServiceDataSource()
        {
            this.OperationProvider = new PerfServiceOperationProvider();        
        }

        public EntityCollection<Person> SimplePeopleSet
        {
            get;
            private set;
        }

        public EntityCollection<Person> LargePeopleSet
        {
            get;
            private set;
        }

        public EntityCollection<Company> CompanySet
        {
            get;
            private set;
        }

        public override void Reset()
        {
            this.SimplePeopleSet = new EntityCollection<Person>();
            this.LargePeopleSet = new EntityCollection<Person>();
            this.CompanySet = new EntityCollection<Company>();
        }

        public override void Initialize()
        {
            Random rnd = new Random();

            List<Person> simplePeopleSet = Enumerable.Range(1, 100).Select(i =>
                        new Person
                        {
                            PersonID = i,
                            FirstName = "Jill",
                            LastName = "Jones",
                            MiddleName = "Vat",
                            Age = rnd.Next(1, 100),
                        }).ToList();
            this.SimplePeopleSet.AddRange(simplePeopleSet);

            List<Person> largePeopleSet = Enumerable.Range(1, 1000).Select(i =>
                        new Person
                        {
                            PersonID = i,
                            FirstName = "Bob",
                            LastName = "Cat",
                            MiddleName = "Elmo",
                            Age = rnd.Next(1, 100),
                        }).ToList();
            this.LargePeopleSet.AddRange(largePeopleSet);

            List<Company> companySet = Enumerable.Range(1, 100).Select(i =>
                new Company 
                { 
                    CompanyID = i,
                    Name = GenerateRandomString(),
                    Address = new Address() { City = "City", Street = "Street", PostalCode = "0000"},
                    Employees = new EntityCollection<Person>(),
                    Revenue = rnd.Next(100, 10000)
                }).ToList();

            for (int i = 0; i < 10; i++)
            {
                companySet[i].Employees.AddRange(largePeopleSet.GetRange(i * 10, 10));
            }

            this.CompanySet.AddRange(companySet);
        }

        private string GenerateRandomString()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 8);
        }

        protected override IEdmModel CreateModel()
        {
            return PerfServiceEdmModel.CreateServiceEdmModel("Microsoft.Test.OData.Services.PerfService");
        }
    }
}
