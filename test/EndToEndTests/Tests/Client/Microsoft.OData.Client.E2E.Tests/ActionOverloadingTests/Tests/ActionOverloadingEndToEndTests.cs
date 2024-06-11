//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingEndToEndTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using System.Xml;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Client.Default;
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Tests
{
    [TestCaseOrderer("Microsoft.OData.Client.E2E.TestCommon.PriorityOrderer", "Microsoft.OData.Client.E2E.TestCommon")]
    public class ActionOverloadingEndToEndTests : EndToEndTestBase<ActionOverloadingEndToEndTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ProductsController), typeof(OrderLinesController), typeof(PeopleController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", ActionOverloadingEdmModel.GetEdmModel()));
            }
        }
        public ActionOverloadingEndToEndTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
        }

        [Fact]
        public void ExecuteOverloadedOperations()
        {
            var product = _context.Execute<Client.Product>(new Uri(_baseUri + "Products(-10)", UriKind.Absolute)).Single();
            OperationDescriptor productOperationDescriptor = _context.GetEntityDescriptor(product).OperationDescriptors.Single();

            Assert.Equal("http://localhost/odata/Products(-10)/Default.RetrieveProduct", productOperationDescriptor.Target.AbsoluteUri, true);
            Assert.Equal("http://localhost/odata/$metadata#Default.RetrieveProduct", productOperationDescriptor.Metadata.AbsoluteUri, true);

            Client.OrderLine orderLine = _context.Execute<Client.OrderLine>(new Uri(_baseUri + "OrderLines(OrderId=-10,ProductId=-10)", UriKind.Absolute)).Single();

            OperationDescriptor orderLineOperationDescriptor = _context.GetEntityDescriptor(orderLine).OperationDescriptors.Single();
            Assert.Equal("http://localhost/odata/OrderLines(OrderId=-10,ProductId=-10)/Default.RetrieveProduct", orderLineOperationDescriptor.Target.AbsoluteUri, true);
            Assert.Equal("http://localhost/odata/$metadata#Default.RetrieveProduct", orderLineOperationDescriptor.Metadata.AbsoluteUri, true);

            // service operation
            int serviceOperationResult = _context.Execute<int>(new Uri(_baseUri + "RetrieveProduct", UriKind.Absolute), "POST", true).Single();
            Assert.Equal(-10, serviceOperationResult);

            // actions
            int productodata = _context.Execute<int>(productOperationDescriptor.Target, "POST", true).Single();
            Assert.Equal(-10, productodata);

            int orderLineodata = _context.Execute<int>(orderLineOperationDescriptor.Target, "POST", true).Single();
            Assert.Equal(-10, orderLineodata);
        }

        [Fact]
        public void ExecuteOverloadedActionNotBound()
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 1)
                {
                    _context.Format.UseJson();
                }

                // unbound action
                _context.Execute(new Uri(_baseUri + "Default.UpdatePersonInfo", UriKind.Absolute), "POST");
            }
        }

        [Fact]
        public void ExecuteActionImport()
        {
            var productId = _context.RetrieveProduct().GetValue();
            Assert.Equal(-10, productId);
            _context.UpdatePersonInfo();
        }

        [Fact, TestPriority(1)]
        public void ExcuteBoundAction()
        {
            _context.MergeOption = MergeOption.OverwriteChanges;

            Client.Employee employee = (Client.Employee)_context.People.Where(p => p.PersonId == 0).Single();
            Assert.Equal(85, employee.Salary);

            DataServiceQuerySingle<Client.Employee> singleEmployee = new DataServiceQuerySingle<Client.Employee>(_context, "People(0)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee");
            singleEmployee.UpdatePersonInfo().Execute();

            Client.SpecialEmployee specialEmployee = (Client.SpecialEmployee)_context.People.Where(p => p.PersonId == -7).Single();

            DataServiceQuerySingle<Client.SpecialEmployee> singleSpecialEmployee = new DataServiceQuerySingle<Client.SpecialEmployee>(_context, "People(-7)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.SpecialEmployee");
            int salary = singleSpecialEmployee.IncreaseEmployeeSalary().GetValue();
            Assert.Equal(2016141257, salary);

            singleSpecialEmployee.IncreaseEmployeeSalary().GetValue();
            specialEmployee = (Client.SpecialEmployee)_context.People.Where(p => p.PersonId == -7).Single();
            Assert.Equal(2016141258, specialEmployee.Salary);
        }

        [Fact]
        public void ExecuteOverloadedActionsOnBaseAndDerivedTypes()
        {
            string actionName = "UpdatePersonInfo";
            string actionTitle = "Default." + actionName;
            string metadataPrefix = "$metadata#Default.";

            // action bound with person instance
            var people = _context.Execute<Client.Person>(new Uri(_baseUri + "People(-1)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> peopleDescriptors = _context.GetEntityDescriptor(people).OperationDescriptors.Where(od => od.Metadata.AbsoluteUri.EndsWith(actionName));
            Dictionary<string, string> expectedLinkValues = new Dictionary<string, string>()
            {
                { metadataPrefix + actionName, "People(-1)/" + actionTitle },
            };
            this.VerifyLinks(expectedLinkValues, peopleDescriptors);
            this.ExecuteActions(_context, peopleDescriptors);

            // action bound with employee instance
            var employee = _context.Execute<Client.Employee>(new Uri(_baseUri + "People(0)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> employeeDescriptors = _context.GetEntityDescriptor(employee).OperationDescriptors.Where(od => od.Title.Equals(actionTitle));
            List<Tuple<string, string>> expectedLinkValuesList = new List<Tuple<string, string>>
            {
                new(metadataPrefix + actionName, "People(0)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Person/" + actionTitle),
                new(metadataPrefix + actionName, "People(0)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/" + actionTitle),
            };

            this.VerifyLinks(expectedLinkValuesList, employeeDescriptors);
            this.ExecuteActions(_context, employeeDescriptors);

            // action bound with special employee instance
            var specialEmployee = _context.Execute<Client.SpecialEmployee>(new Uri(_baseUri + "People(-7)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> specialEmployeeDescriptors = _context.GetEntityDescriptor(specialEmployee).OperationDescriptors.Where(od => od.Title.Equals(actionTitle));
            expectedLinkValuesList = new List<Tuple<string, string>>
            {
                new(metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Person/" + actionTitle),
                new(metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/" + actionTitle),
                new(metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.SpecialEmployee/" + actionTitle),
            };
            this.VerifyLinks(expectedLinkValuesList, specialEmployeeDescriptors);
            this.ExecuteActions(_context, specialEmployeeDescriptors);

            // action bound with contractor instance
            var contractor = _context.Execute<Client.Contractor>(new Uri(_baseUri + "People(1)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> contractorDescriptors = _context.GetEntityDescriptor(contractor).OperationDescriptors.Where(od => od.Title.Equals(actionTitle));
            expectedLinkValuesList = new List<Tuple<string, string>>
            {
                new(metadataPrefix + actionName, "People(1)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Person/" + actionTitle),
            };
            this.VerifyLinks(expectedLinkValuesList, contractorDescriptors);
            this.ExecuteActions(_context, contractorDescriptors);
        }

        [Fact, TestPriority(3)]
        public void ExecuteOverloadedActionsWithDifferentParameter()
        {
            string actionName = "IncreaseEmployeeSalary";
            string actionTitle = "Default." + actionName;
            string metadataPrefix = "$metadata#Default.";

            // action bound with special employee instance
            Client.Employee employee = _context.Execute<Client.Employee>(new Uri(_baseUri + "People(0)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> employeeDescriptors = _context.GetEntityDescriptor(employee).OperationDescriptors.Where(od => od.Title.Equals(actionTitle));
            Dictionary<string, string> expectedLinkValues = new Dictionary<string, string>()
            {
                { metadataPrefix + actionName, "People(0)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/" + actionTitle },
            };
            this.VerifyLinks(expectedLinkValues, employeeDescriptors);
            _context.Execute<bool>(employeeDescriptors.Single().Target, "POST", true, new BodyOperationParameter("n", 123)).Single();

            // action bound with special special employee instance
            Client.SpecialEmployee specialEmployee = _context.Execute<Client.SpecialEmployee>(new Uri(_baseUri + "People(-7)", UriKind.Absolute)).Single();
            IEnumerable<OperationDescriptor> specialEmployeeDescriptors = _context.GetEntityDescriptor(specialEmployee).OperationDescriptors.Where(od => od.Title.Equals(actionTitle));
            List<Tuple<string, string>> expectedLinkValuesList = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>(metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/" + actionTitle),
                new Tuple<string, string>(metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.SpecialEmployee/" + actionTitle),
            };

            this.VerifyLinks(expectedLinkValuesList, specialEmployeeDescriptors);

            _context.Execute<bool>(specialEmployeeDescriptors.Where(d => d.Target.AbsoluteUri.Contains(".Employee")).Single().Target, "POST", true, new BodyOperationParameter("n", 123)).Single();
            _context.Execute<int>(specialEmployeeDescriptors.Where(d => d.Target.AbsoluteUri.Contains(".SpecialEmployee")).Single().Target, "POST", true).Single();
        }

        [Fact, TestPriority(2)]
        public void ExecuteEntitySetBoundOverloadedOperations()
        {
            WriteModelToCsdl(ActionOverloadingEdmModel.GetEdmModel(), "csdl.xml");

            for (int i = 1; i < 2; i++)
            {
                // action bound with collection of employees
                _context.Execute(
                    new Uri(_baseUri + "People/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee/Default.IncreaseSalaries", UriKind.Absolute),
                    "POST",
                    new BodyOperationParameter("n", 3));

                // action bound with collection of special employees
                _context.Execute(
                    new Uri(_baseUri + "People/Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.SpecialEmployee/Default.IncreaseSalaries", UriKind.Absolute),
                    "POST",
                    new BodyOperationParameter("n", 3));
            }
        }

        [Fact]
        public void OverloadedActionsProjection()
        {
            for (int i = 1; i < 2; i++)
            {
                Client.Product product = _context.Execute<Client.Product>(new Uri(_baseUri + "Products(-10)?$select=Default.*", UriKind.Absolute)).Single();
                OperationDescriptor productOperationDescriptor = _context.GetEntityDescriptor(product).OperationDescriptors.Single();
                Assert.Equal(_baseUri + "$metadata#Default.RetrieveProduct", productOperationDescriptor.Metadata.AbsoluteUri, true);
                Assert.Equal(_baseUri + "Products(-10)/Default.RetrieveProduct", productOperationDescriptor.Target.AbsoluteUri, true);

                Client.OrderLine orderLine = _context.Execute<Client.OrderLine>(new Uri(_baseUri + "OrderLines(OrderId=-10,ProductId=-10)?$select=*")).Single();
                Assert.Empty(_context.GetEntityDescriptor(orderLine).OperationDescriptors);
            }
        }

        private void VerifyLinks(Dictionary<string, string> expectedValues, IEnumerable<OperationDescriptor> actualDescriptors)
        {
            foreach (KeyValuePair<string, string> expected in expectedValues)
            {
                OperationDescriptor od;
                od = actualDescriptors.Where(d => d.Metadata.AbsoluteUri.Equals(_baseUri + expected.Key, StringComparison.OrdinalIgnoreCase)).First();
                Assert.Equal(_baseUri + expected.Value, od.Target.AbsoluteUri, true);
            }
        }

        private void VerifyLinks(IEnumerable<Tuple<string, string>> expectedValues, IEnumerable<OperationDescriptor> actualDescriptors)
        {
            foreach (var expected in expectedValues)
            {
                IEnumerable<OperationDescriptor> ods;
                ods = actualDescriptors.Where(d => d.Metadata.AbsoluteUri.Equals(_baseUri + expected.Item1, StringComparison.OrdinalIgnoreCase));

                bool matched = false;
                foreach (var od in ods)
                {
                    if (_baseUri + expected.Item2 == od.Target.AbsoluteUri)
                    {
                        matched = true;
                        break;
                    }
                }

                Assert.True(matched);
            }
        }

        private void ExecuteActions(Container context, IEnumerable<OperationDescriptor> operationDescriptors)
        {
            foreach (OperationDescriptor od in operationDescriptors)
            {
                context.Execute(od.Target, "POST");
            }
        }

        private static void WriteModelToCsdl(IEdmModel model, string fileName)
        {
            using (var writer = XmlWriter.Create(fileName))
            {
                IEnumerable<EdmError> errors;
                CsdlWriter.TryWriteCsdl(model, writer, CsdlTarget.OData, out errors);
            }
        }
    }
}
