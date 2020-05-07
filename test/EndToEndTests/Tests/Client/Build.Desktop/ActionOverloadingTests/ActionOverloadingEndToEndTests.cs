//---------------------------------------------------------------------
// <copyright file="ActionOverloadingEndToEndTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ActionOverloadingTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.ActionOverloadingServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionOverloadingEndToEndTests : EndToEndTestBase
    {
        public ActionOverloadingEndToEndTests()
            : base(ServiceDescriptors.ActionOverloadingService)
        {
        }

        [TestMethod]
        public void ExecuteOverloadedOperations()
        {
            string actionName = "RetrieveProduct";
            string actionTitle = "Microsoft.Test.OData.Services.AstoriaDefaultService." + actionName;

            var contextWrapper = this.CreateWrappedContext();

            Product product = contextWrapper.Execute<Product>(new Uri(this.ServiceUri + "Product(-10)", UriKind.Absolute)).Single();
            OperationDescriptor productOperationDescriptor = contextWrapper.GetEntityDescriptor(product).OperationDescriptors.Single();
            Assert.AreEqual(this.ServiceUri + "Product(-10)/" + actionTitle, productOperationDescriptor.Target.AbsoluteUri, true);
            Assert.AreEqual(this.ServiceUri + ActionOverloadingQueryTests.MetadataPrefix + actionName, productOperationDescriptor.Metadata.AbsoluteUri, true);

            OrderLine orderLine = contextWrapper.Execute<OrderLine>(new Uri(this.ServiceUri + "OrderLine(OrderId=-10,ProductId=-10)", UriKind.Absolute)).Single();
            OperationDescriptor orderLineOperationDescriptor = contextWrapper.GetEntityDescriptor(orderLine).OperationDescriptors.Single();
            Assert.AreEqual(this.ServiceUri + "OrderLine(OrderId=-10,ProductId=-10)/" + actionTitle, orderLineOperationDescriptor.Target.AbsoluteUri, true);
            Assert.AreEqual(this.ServiceUri + ActionOverloadingQueryTests.MetadataPrefix + actionName, orderLineOperationDescriptor.Metadata.AbsoluteUri, true);

            // service operation
            int serviceOperationResult = contextWrapper.Execute<int>(new Uri(this.ServiceUri + actionName, UriKind.Absolute), "POST", true).Single();

            // actions
            int productActionResult = contextWrapper.Execute<int>(productOperationDescriptor.Target, "POST", true).Single();
            int orderLineActionResult = contextWrapper.Execute<int>(orderLineOperationDescriptor.Target, "POST", true).Single();
        }

        [TestMethod]
        public void ExecuteOverloadedActionNotBound()
        {
            for (int i = 0; i < 2; i++)
            {
                var contextWrapper = this.CreateWrappedContext();
                if (i == 1)
                {
                    contextWrapper.Format.UseJson();
                }

                // unbound action
                contextWrapper.Execute(new Uri(this.ServiceUri + "UpdatePersonInfo", UriKind.Absolute), "POST");
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        [TestMethod]
        public void ExecuteActionImport()
        {
            var contextWrapper = this.CreateWrappedContext();
            var productId = contextWrapper.Context.RetrieveProduct().GetValue();
            Assert.AreEqual(-10, productId);
            contextWrapper.Context.UpdatePersonInfo();
        }
#endif

        [TestMethod]
        public void ExcuteBoundAction()
        {
            var contextWrapper = this.CreateWrappedContext();
            contextWrapper.Context.MergeOption = MergeOption.OverwriteChanges;
            
            Employee employee = (Employee)contextWrapper.Context.Person.Where(p => p.PersonId == 0).Single();
            Assert.AreEqual(85, employee.Salary);

            employee.UpdatePersonInfo();

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
            SpecialEmployee specialEmployee = (SpecialEmployee)contextWrapper.Context.Person.Where(p => p.PersonId == -7).Single();
            int salary = specialEmployee.IncreaseEmployeeSalary().GetValue();
            Assert.AreEqual(2016141257, salary);

            specialEmployee.IncreaseEmployeeSalary().GetValue();
            specialEmployee = (SpecialEmployee)contextWrapper.Context.Person.Where(p => p.PersonId == -7).Single();
            Assert.AreEqual(2016141258, specialEmployee.Salary);
#endif
        }

        [TestMethod] 
        public void ExecuteOverloadedActionsOnBaseAndDerivedTypes()
        {
            string actionName = "UpdatePersonInfo";
            string actionTitle = "Microsoft.Test.OData.Services.AstoriaDefaultService." + actionName;

            var contextWrapper = this.CreateWrappedContext();

            // action bound with person instance
            Person people = contextWrapper.Execute<Person>(new Uri(this.ServiceUri + "Person(-1)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> peopleDescriptors = contextWrapper.GetEntityDescriptor(people).OperationDescriptors.Where(od => od.Metadata.AbsoluteUri.EndsWith(actionName));
            Dictionary<string, string> expectedLinkValues = new Dictionary<string, string>()
            {
                { ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-1)/" + actionTitle },
            };
            this.VerifyLinks(expectedLinkValues, peopleDescriptors);
            this.ExecuteActions(contextWrapper, peopleDescriptors);

            // action bound with employee instance
            Employee employee = contextWrapper.Execute<Employee>(new Uri(this.ServiceUri + "Person(0)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> employeeDescriptors = contextWrapper.GetEntityDescriptor(employee).OperationDescriptors.Where(od => od.Title.Equals(actionName));
            List<Tuple<string, string>> expectedLinkValuesList = new List<Tuple<string, string>> 
            {
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(0)/" + ActionOverloadingQueryTests.PersonTypeName + "/" + actionTitle),
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(0)/" + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionTitle),
            };
            this.VerifyLinks(expectedLinkValuesList, employeeDescriptors);
            this.ExecuteActions(contextWrapper, employeeDescriptors);

            // action bound with special employee instance
            SpecialEmployee specialEmployee = contextWrapper.Execute<SpecialEmployee>(new Uri(this.ServiceUri + "Person(-7)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> specialEmployeeDescriptors = contextWrapper.GetEntityDescriptor(specialEmployee).OperationDescriptors.Where(od => od.Title.Equals(actionName));
            expectedLinkValuesList = new List<Tuple<string, string>> 
            {
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-7)/" + ActionOverloadingQueryTests.PersonTypeName + "/" + actionTitle),
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-7)/" + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionTitle),
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-7)/" + ActionOverloadingQueryTests.SpecialEmployeeTypeName + "/" + actionTitle),
            };
            this.VerifyLinks(expectedLinkValuesList, specialEmployeeDescriptors);
            this.ExecuteActions(contextWrapper, specialEmployeeDescriptors);

            // action bound with contractor instance
            Contractor contractor = contextWrapper.Execute<Contractor>(new Uri(this.ServiceUri + "Person(1)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> contractorDescriptors = contextWrapper.GetEntityDescriptor(contractor).OperationDescriptors.Where(od => od.Title.Equals(actionName));
            expectedLinkValuesList = new List<Tuple<string, string>> 
            {
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(1)/" + ActionOverloadingQueryTests.PersonTypeName + "/" + actionTitle),
            };
            this.VerifyLinks(expectedLinkValuesList, contractorDescriptors);
            this.ExecuteActions(contextWrapper, contractorDescriptors);
        }

        [TestMethod]
        public void ExecuteEntitySetBoundOverloadedOperations()
        {
            for (int i = 1; i < 2; i++)
            {
                var contextWrapper = this.CreateWrappedContext();
                if (i == 0)
                {
                    //contextWrapper.Format.UseAtom();
                }

                // action bound with collection of employees
                contextWrapper.Execute(
                    new Uri(this.ServiceUri + "Person/" + ActionOverloadingQueryTests.EmployeeTypeName + "/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries", UriKind.Absolute),
                    "POST",
                    new BodyOperationParameter("n", 3));

                // action bound with collection of special employees
                contextWrapper.Execute(
                    new Uri(this.ServiceUri + "Person/" + ActionOverloadingQueryTests.SpecialEmployeeTypeName + "/Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseSalaries", UriKind.Absolute),
                    "POST",
                    new BodyOperationParameter("n", 3));
            }
        }

        [TestMethod]
        public void ExecuteOverloadedActionsWithDifferentParameter()
        {
            string actionName = "IncreaseEmployeeSalary";
            string actionTitle = "Microsoft.Test.OData.Services.AstoriaDefaultService." + actionName;

            var contextWrapper = this.CreateWrappedContext();

            // action bound with special employee instance
            Employee employee = contextWrapper.Execute<Employee>(new Uri(this.ServiceUri + "Person(0)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> employeeDescriptors = contextWrapper.GetEntityDescriptor(employee).OperationDescriptors.Where(od => od.Title.Equals(actionTitle));
            Dictionary<string, string> expectedLinkValues = new Dictionary<string, string>()
            {
                { ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(0)/" + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionTitle },
            };
            this.VerifyLinks(expectedLinkValues, employeeDescriptors);
            contextWrapper.Execute<bool>(employeeDescriptors.Single().Target, "POST", true, new BodyOperationParameter("n", 123)).Single();

            // action bound with special special employee instance
            SpecialEmployee specialEmployee = contextWrapper.Execute<SpecialEmployee>(new Uri(this.ServiceUri + "Person(-7)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> specialEmployeeDescriptors = contextWrapper.GetEntityDescriptor(specialEmployee).OperationDescriptors.Where(od => od.Title.Equals(actionName));
            List<Tuple<string, string>> expectedLinkValuesList = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-7)/" + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionTitle),
                new Tuple<string, string>(ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-7)/" + ActionOverloadingQueryTests.SpecialEmployeeTypeName + "/" + actionTitle),
            };

            this.VerifyLinks(expectedLinkValuesList, specialEmployeeDescriptors);

            contextWrapper.Execute<bool>(specialEmployeeDescriptors.Where(d => d.Target.AbsoluteUri.Contains(".Employee")).Single().Target, "POST", true, new BodyOperationParameter("n", 123)).Single();
            contextWrapper.Execute<int>(specialEmployeeDescriptors.Where(d => d.Target.AbsoluteUri.Contains(".SpecialEmployee")).Single().Target, "POST", true).Single();
        }

        [TestMethod]
        public void OverloadedActionsProjection()
        {
            for (int i = 1; i < 2; i++)
            {
                var contextWrapper = this.CreateWrappedContext();
                if (i == 0)
                {
                    //contextWrapper.Format.UseAtom();
                }

                Product product = contextWrapper.Execute<Product>(new Uri(this.ServiceUri + "Product(-10)?$select=Microsoft.Test.OData.Services.AstoriaDefaultService.*", UriKind.Absolute)).Single();
                OperationDescriptor productOperationDescriptor = contextWrapper.GetEntityDescriptor(product).OperationDescriptors.Single();
                Assert.AreEqual(this.ServiceUri + ActionOverloadingQueryTests.MetadataPrefix + "RetrieveProduct", productOperationDescriptor.Metadata.AbsoluteUri, true);
                Assert.AreEqual(this.ServiceUri + "Product(-10)/Microsoft.Test.OData.Services.AstoriaDefaultService.RetrieveProduct", productOperationDescriptor.Target.AbsoluteUri, true);
                
                OrderLine orderLine = contextWrapper.Execute<OrderLine>(new Uri(this.ServiceUri + "OrderLine(OrderId=-10,ProductId=-10)?$select=*")).Single();
                Assert.AreEqual(0, contextWrapper.GetEntityDescriptor(orderLine).OperationDescriptors.Count);
            }
        }

        // Inconsistent behavior when selecting namespace.* and action name
        // [TestMethod] // github issuse: #896
        public void BaseDerivedTypeOverloadedActionsProjection()
        {
            string actionName = "UpdatePersonInfo";
            for (int i = 0; i < 2; i++)
            {
                var contextWrapper = this.CreateWrappedContext();
                if (i == 1)
                {
                    contextWrapper.Format.UseJson();
                }

                // base type instance, $select=ActionNmae
                Person person = contextWrapper.Execute<Person>(new Uri(this.ServiceUri + "Person(-1)?$select=PersonId," + actionName, UriKind.Absolute)).Single();
                OperationDescriptor personOperationDescriptor = contextWrapper.GetEntityDescriptor(person).OperationDescriptors.Single();
                Assert.AreEqual(this.ServiceUri + ActionOverloadingQueryTests.MetadataPrefix + actionName, personOperationDescriptor.Metadata.AbsoluteUri, true);
                Assert.AreEqual(this.ServiceUri + "Person(-1)/UpdatePersonInfo", personOperationDescriptor.Target.AbsoluteUri, true);

                // base type instance, $select=DerivedType/ActionName
                contextWrapper.Detach(person);
                person = contextWrapper.Execute<Person>(new Uri(this.ServiceUri + "Person(-1)?$select=PersonId," + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionName, UriKind.Absolute)).Single();
                Assert.AreEqual(0, contextWrapper.GetEntityDescriptor(person).OperationDescriptors.Count);

                // derived type instance, $select=ActionName
                Employee employee = contextWrapper.Execute<Employee>(new Uri(this.ServiceUri + "Person(0)?$select=PersonId," + actionName, UriKind.Absolute)).Single();
                Dictionary<string, string> expectedLinkValues = new Dictionary<string, string>()
                {
                    { ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(0)/" + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionName },
                };
                IEnumerable<OperationDescriptor> employeeDescriptors = contextWrapper.GetEntityDescriptor(employee).OperationDescriptors;
                this.VerifyLinks(expectedLinkValues, employeeDescriptors);

                // derived type instance, $select=DerivedType/ActionName
                contextWrapper.Detach(employee);
                employee = contextWrapper.Execute<Employee>(new Uri(this.ServiceUri + "Person(0)?$select=PersonId," + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionName, UriKind.Absolute)).Single();
                employeeDescriptors = contextWrapper.GetEntityDescriptor(employee).OperationDescriptors;
                this.VerifyLinks(expectedLinkValues, employeeDescriptors);

                // derived type instance, $select=Microsoft.Test.OData.Services.AstoriaDefaultService.*
                SpecialEmployee specialEmployee = contextWrapper.Execute<SpecialEmployee>(new Uri(this.ServiceUri + "Person(-7)?$select=PersonId," + "Microsoft.Test.OData.Services.AstoriaDefaultService.*", UriKind.Absolute)).Single();
                expectedLinkValues = new Dictionary<string, string>()
                {
                    { ActionOverloadingQueryTests.MetadataPrefix + actionName, "Person(-7)/" + ActionOverloadingQueryTests.SpecialEmployeeTypeName + "/" + actionName }
                };
                IEnumerable<OperationDescriptor> specialEmployeeDescriptors = contextWrapper.GetEntityDescriptor(specialEmployee).OperationDescriptors;
                this.VerifyLinks(expectedLinkValues, specialEmployeeDescriptors);

                // derived type instance, $select=BaseType/ActionName
                contextWrapper.Detach(specialEmployee);
                specialEmployee = contextWrapper.Execute<SpecialEmployee>(new Uri(this.ServiceUri + "Person(-7)?$select=PersonId," + ActionOverloadingQueryTests.EmployeeTypeName + "/" + actionName, UriKind.Absolute)).Single();
                specialEmployeeDescriptors = contextWrapper.GetEntityDescriptor(specialEmployee).OperationDescriptors;
                this.VerifyLinks(expectedLinkValues, specialEmployeeDescriptors);
            }
        }

        private DataServiceContextWrapper<DefaultContainer> CreateWrappedContext()
        {
            return this.CreateWrappedContext<DefaultContainer>();
        }

        private void VerifyLinks(Dictionary<string, string> expectedValues, IEnumerable<OperationDescriptor> actualDescriptors)
        {
            foreach (KeyValuePair<string, string> expected in expectedValues)
            {
                OperationDescriptor od;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
                od = actualDescriptors.Where(d => d.Metadata.AbsoluteUri.Equals(this.ServiceUri + expected.Key, StringComparison.OrdinalIgnoreCase)).First();
#else
                od = actualDescriptors.Where(d => d.Metadata.AbsoluteUri.Equals(this.ServiceUri + expected.Key, StringComparison.InvariantCultureIgnoreCase)).First();
#endif
                Assert.AreEqual(this.ServiceUri + expected.Value, od.Target.AbsoluteUri, true);
            }
        }

        private void VerifyLinks(IEnumerable<Tuple<string, string>> expectedValues, IEnumerable<OperationDescriptor> actualDescriptors)
        {
            foreach (var expected in expectedValues)
            {
                IEnumerable<OperationDescriptor> ods;
#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
                ods = actualDescriptors.Where(d => d.Metadata.AbsoluteUri.Equals(this.ServiceUri + expected.Item1, StringComparison.OrdinalIgnoreCase));
#else
                ods = actualDescriptors.Where(d => d.Metadata.AbsoluteUri.Equals(this.ServiceUri + expected.Item1, StringComparison.InvariantCultureIgnoreCase));
#endif

                bool matched = false;
                foreach (var od in ods)
                {
                    if (this.ServiceUri + expected.Item2 == od.Target.AbsoluteUri)
                    {
                        matched = true;
                        break;
                    }
                }

                Assert.IsTrue(matched);
            }
        }

        private void ExecuteActions(DataServiceContextWrapper<DefaultContainer> context, IEnumerable<OperationDescriptor> operationDescriptors)
        {
            foreach (OperationDescriptor od in operationDescriptors)
            {
                context.Execute(od.Target, "POST");
            }
        }
    }
}
