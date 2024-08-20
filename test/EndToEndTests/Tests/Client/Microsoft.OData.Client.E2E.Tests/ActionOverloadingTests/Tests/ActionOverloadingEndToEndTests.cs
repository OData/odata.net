//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingEndToEndTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Tests
{
    public class ActionOverloadingEndToEndTests : EndToEndTestBase<ActionOverloadingEndToEndTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ActionOverloadingTestsController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }
        public ActionOverloadingEndToEndTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
            ResetDataSource();
        }

        [Fact]
        public void An_OverloadedOperation_Executes_Successfully()
        {
            // Arrange
            var productUri = new Uri(_baseUri + "Products(-10)", UriKind.Absolute);
            var expectedProductTargetUri = "http://localhost/odata/Products(-10)/Default.RetrieveProduct";
            var expectedProductMetadataUri = "http://localhost/odata/$metadata#Default.RetrieveProduct";

            var orderLineUri = new Uri(_baseUri + "OrderLines(OrderId=-7,ProductId=-7)", UriKind.Absolute);
            var expectedOrderLineTargetUri = "http://localhost/odata/OrderLines(OrderId=-7,ProductId=-7)/Default.RetrieveProduct";
            var expectedOrderLineMetadataUri = "http://localhost/odata/$metadata#Default.RetrieveProduct";

            var serviceOperationUri = new Uri(_baseUri + "RetrieveProduct", UriKind.Absolute);

            // Act
            var product = _context.Execute<Common.Clients.EndToEnd.Product>(productUri).Single();
            var productOperationDescriptor = _context.GetEntityDescriptor(product).OperationDescriptors
                .Single(op => op.Target.AbsoluteUri == expectedProductTargetUri);

            var orderLine = _context.Execute<Common.Clients.EndToEnd.OrderLine>(orderLineUri).Single();
            var orderLineOperationDescriptor = _context.GetEntityDescriptor(orderLine).OperationDescriptors
                .Single(op => op.Target.AbsoluteUri == expectedOrderLineTargetUri);

            var serviceOperationResult = _context.Execute<int>(serviceOperationUri, "POST", true).Single();

            var productId = _context.Execute<int>(productOperationDescriptor.Target, "POST", true).Single();
            var orderLineId = _context.Execute<int>(orderLineOperationDescriptor.Target, "POST", true).Single();

            // Assert
            Assert.Equal(expectedProductTargetUri, productOperationDescriptor.Target.AbsoluteUri, ignoreCase: true);
            Assert.Equal(expectedProductMetadataUri, productOperationDescriptor.Metadata.AbsoluteUri, ignoreCase: true);

            Assert.Equal(expectedOrderLineTargetUri, orderLineOperationDescriptor.Target.AbsoluteUri, ignoreCase: true);
            Assert.Equal(expectedOrderLineMetadataUri, orderLineOperationDescriptor.Metadata.AbsoluteUri, ignoreCase: true);

            Assert.Equal(-9, serviceOperationResult);

            Assert.Equal(-10, productId);
            Assert.Equal(-7, orderLineId);
        }

        [Fact]
        public void An_UnboundOverloadedAction_Executes_Successfully()
        {
            // Arrange
            _context.MergeOption = MergeOption.OverwriteChanges;
            var personUri = new Uri(_baseUri + "People(-10)", UriKind.Absolute);
            var actionUri = new Uri(_baseUri + "Default.UpdatePersonInfo", UriKind.Absolute);
            var expectedPersonNameBeforeAction = "ぺソぞ弌タァ匚タぽひハ欲ぴほ匚せまたバボチマ匚ぁゾソチぁЯそぁミя暦畚ボ歹ひЯほダチそЯせぽゼポЯチａた歹たをタマせをせ匚ミタひぜ畚暦グクひほそたグせяチ匚ｦぺぁ";
            var expectedPersonNameAfterAction = expectedPersonNameBeforeAction + "[UpdataPersonName]";

            // Act
            var person = _context.Execute<Common.Clients.EndToEnd.Person>(personUri).Single();
            Assert.Equal(expectedPersonNameBeforeAction, person.Name); // Verify initial state

            _context.Execute(actionUri, "POST");  // Perform the action

            var personAfterAction = _context.Execute<Common.Clients.EndToEnd.Person>(personUri).Single();  // Get the person after action

            // Assert
            Assert.Equal(expectedPersonNameAfterAction, personAfterAction.Name);  // Verify the result of the action
        }

        [Fact]
        public void An_ActionImport_Executes_Successfully()
        {
            var productId = _context.RetrieveProduct().GetValue();
            Assert.Equal(-9, productId);
        }

        [Fact]
        public void A_BoundAction_Executes_Successfully()
        {
            _context.MergeOption = MergeOption.OverwriteChanges;

            // Retrieve a special employee with PersonId -7 and verify their initial salary
            Common.Clients.EndToEnd.SpecialEmployee specialEmployee = (Common.Clients.EndToEnd.SpecialEmployee)_context.People.Where(p => p.PersonId == -7).Single();
            Assert.Equal(2016141256, specialEmployee.Salary);

            // Create a DataServiceQuerySingle instance for the special employee and increase their salary
            var singleSpecialEmployee = new DataServiceQuerySingle<Common.Clients.EndToEnd.SpecialEmployee>(_context, "People(-7)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee");

            int salary = singleSpecialEmployee.IncreaseEmployeeSalary().GetValue();
            Assert.Equal(2016141257, salary);

            // Increase the special employee's salary again and verify the updated salary
            singleSpecialEmployee.IncreaseEmployeeSalary().GetValue();
            specialEmployee = (Common.Clients.EndToEnd.SpecialEmployee)_context.People.Where(p => p.PersonId == -7).Single();
            Assert.Equal(2016141258, specialEmployee.Salary);
        }

        [Fact]
        public void OverloadedActionsOnBaseAndDerivedTypes_Execute_Successfully()
        {
            string actionName = "UpdatePersonInfo";
            string actionTitle = "Default." + actionName;
            string metadataPrefix = "$metadata#Default.";

            _context.MergeOption = MergeOption.OverwriteChanges;

            // Action bound with person instance
            var peopleUri = new Uri(_baseUri + "People(-1)", UriKind.Absolute);
            var people = _context.Execute<Common.Clients.EndToEnd.Person>(peopleUri).Single();
            var peopleDescriptors = _context.GetEntityDescriptor(people).OperationDescriptors
                .Where(od => od.Metadata.AbsoluteUri.EndsWith(actionName));
            var expectedPeopleLinks = new Dictionary<string, string>
            {
                { metadataPrefix + actionName, "People(-1)/" + actionTitle },
            };

            // Verify and execute actions for person
            VerifyLinks(expectedPeopleLinks, peopleDescriptors);
            ExecuteActions(_context, peopleDescriptors);

            // Action bound with employee instance
            var employeeUri = new Uri(_baseUri + "People(0)", UriKind.Absolute);
            var employee = _context.Execute<Common.Clients.EndToEnd.Employee>(employeeUri).Single();
            var employeeDescriptors = _context.GetEntityDescriptor(employee).OperationDescriptors
                .Where(od => od.Title.Equals(actionTitle));
            var expectedEmployeeLinks = new List<(string, string)>
            {
                (metadataPrefix + actionName, "People(0)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Person/" + actionTitle),
                (metadataPrefix + actionName, "People(0)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/" + actionTitle)
            };

            // Verify and execute actions for employee
            VerifyLinks(expectedEmployeeLinks, employeeDescriptors);
            ExecuteActions(_context, employeeDescriptors);

            // Action bound with special employee instance
            var specialEmployeeUri = new Uri(_baseUri + "People(-7)", UriKind.Absolute);
            var specialEmployee = _context.Execute<Common.Clients.EndToEnd.SpecialEmployee>(specialEmployeeUri).Single();
            var specialEmployeeDescriptors = _context.GetEntityDescriptor(specialEmployee).OperationDescriptors
                .Where(od => od.Title.Equals(actionTitle));
            var expectedSpecialEmployeeLinks = new List<(string, string)>
            {
                (metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Person/" + actionTitle),
                (metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/" + actionTitle),
                (metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/" + actionTitle),
            };

            // Verify and execute actions for special employee
            VerifyLinks(expectedSpecialEmployeeLinks, specialEmployeeDescriptors);
            ExecuteActions(_context, specialEmployeeDescriptors);

            // Action bound with contractor instance
            var contractorUri = new Uri(_baseUri + "People(1)", UriKind.Absolute);
            var contractor = _context.Execute<Common.Clients.EndToEnd.Contractor>(contractorUri).Single();
            var contractorDescriptors = _context.GetEntityDescriptor(contractor).OperationDescriptors
                .Where(od => od.Title.Equals(actionTitle));
            var expectedContractorLinks = new List<(string, string)>
            {
                (metadataPrefix + actionName, "People(1)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Person/" + actionTitle),
            };

            // Verify and execute actions for contractor
            VerifyLinks(expectedContractorLinks, contractorDescriptors);
            ExecuteActions(_context, contractorDescriptors);
        }

        [Fact]
        public void OverloadedActionsWithDifferentParameters_Execute_Successfully()
        {
            string actionName = "IncreaseEmployeeSalary";
            string actionTitle = "Default." + actionName;
            string metadataPrefix = "$metadata#Default.";

            _context.MergeOption = MergeOption.OverwriteChanges;

            // Action bound with a regular employee instance
            var employeeUri = new Uri(_baseUri + "People(0)", UriKind.Absolute);
            Common.Clients.EndToEnd.Employee employee = _context.Execute<Common.Clients.EndToEnd.Employee>(employeeUri).Single();
            Assert.Equal(85, employee.Salary);

            var employeeDescriptors = _context
                .GetEntityDescriptor(employee)
                .OperationDescriptors
                .Where(od => od.Title.Equals(actionTitle));

            var expectedLinkValues = new Dictionary<string, string>
            {
                { metadataPrefix + actionName, "People(0)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/" + actionTitle },
            };

            VerifyLinks(expectedLinkValues, employeeDescriptors);

            // Execute action
            _context.Execute<bool>(employeeDescriptors.Single().Target, "POST", true, new BodyOperationParameter("n", 123)).Single();

            // Verify salary after action
            Common.Clients.EndToEnd.Employee employeeAfterAction = _context.Execute<Common.Clients.EndToEnd.Employee>(employeeUri).Single();
            Assert.Equal(208, employeeAfterAction.Salary);

            // Action bound with a special employee instance
            var specialEmployeeUri = new Uri(_baseUri + "People(-7)", UriKind.Absolute);
            Common.Clients.EndToEnd.SpecialEmployee specialEmployee = _context.Execute<Common.Clients.EndToEnd.SpecialEmployee>(specialEmployeeUri).Single();
            Assert.Equal(2016141256, specialEmployee.Salary);

            var specialEmployeeDescriptors = _context
                .GetEntityDescriptor(specialEmployee)
                .OperationDescriptors
                .Where(od => od.Title.Equals(actionTitle));

            var expectedLinkValuesList = new List<(string, string)>
            {
                (metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/" + actionTitle),
                (metadataPrefix + actionName, "People(-7)/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/" + actionTitle),
            };

            VerifyLinks(expectedLinkValuesList, specialEmployeeDescriptors);

            _context.Execute<bool>(
                specialEmployeeDescriptors.Single(d => d.Target.AbsoluteUri.Contains(".Employee")).Target,
                "POST",
                true,
                new BodyOperationParameter("n", 123)
            ).Single();

            Common.Clients.EndToEnd.SpecialEmployee specialEmployeeAfterActionWithParam = _context.Execute<Common.Clients.EndToEnd.SpecialEmployee>(specialEmployeeUri).Single();
            Assert.Equal(2016141379, specialEmployeeAfterActionWithParam.Salary);

            _context.Execute<int>(
                specialEmployeeDescriptors.Single(d => d.Target.AbsoluteUri.Contains(".SpecialEmployee")).Target,
                "POST",
                true
            ).Single();

            Common.Clients.EndToEnd.SpecialEmployee specialEmployeeAfterActionWithoutParam = _context.Execute<Common.Clients.EndToEnd.SpecialEmployee>(specialEmployeeUri).Single();
            Assert.Equal(2016141380, specialEmployeeAfterActionWithoutParam.Salary);
        }

        [Fact]
        public void EntitySetBoundOverloadedOperations_Executes_Successfully()
        {
            // Set the merge option to overwrite changes in the context
            _context.MergeOption = MergeOption.OverwriteChanges;

            // Define the action parameter for increasing salaries
            var actionParameter = new BodyOperationParameter("n", 3);

            // Define URIs for the IncreaseSalaries action for both employees and special employees
            var employeesUri = new Uri($"{_baseUri}People/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee/Default.IncreaseSalaries", UriKind.Absolute);
            var specialEmployeesUri = new Uri($"{_baseUri}People/Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee/Default.IncreaseSalaries", UriKind.Absolute);

            // Define URIs to fetch specific employee and special employee data
            var employeeUri = new Uri($"{_baseUri}People(-3)", UriKind.Absolute);
            var specialEmployeeUri = new Uri($"{_baseUri}People(-10)", UriKind.Absolute);

            // Fetch the initial salary of the employee
            var employee = _context.Execute<Common.Clients.EndToEnd.Employee>(employeeUri).Single();
            Assert.Equal(0, employee.Salary); // Verify initial salary is 0

            // Fetch the initial salary of the special employee
            var specialEmployee = _context.Execute<Common.Clients.EndToEnd.SpecialEmployee>(specialEmployeeUri).Single();
            Assert.Equal(4091, specialEmployee.Salary); // Verify initial salary is 4094

            // Execute the IncreaseSalaries action for employees ~ This will increase salaries for all employees
            // including special employees.
            _context.Execute(employeesUri, "POST", actionParameter);

            // Fetch and verify the employee's salary after the action
            var employeeAfterAction = _context.Execute<Common.Clients.EndToEnd.Employee>(employeeUri).Single();
            Assert.Equal(3, employeeAfterAction.Salary); // Verify the salary increased by 3

            // Fetch and verify the special employee's salary after the action
            var specialEmployeeAfterAction = _context.Execute<Common.Clients.EndToEnd.SpecialEmployee>(specialEmployeeUri).Single();
            Assert.Equal(4094, specialEmployeeAfterAction.Salary); // Verify the salary increased by 3
        }


        [Fact]
        public void OverloadedActionsProjection()
        {
            Common.Clients.EndToEnd.Product product = _context.Execute<Common.Clients.EndToEnd.Product>(new Uri(_baseUri + "Products(-10)?$select=Default.*", UriKind.Absolute)).Single();
            OperationDescriptor productOperationDescriptor = _context.GetEntityDescriptor(product).OperationDescriptors.Single();
            Assert.Equal(_baseUri + "$metadata#Default.RetrieveProduct", productOperationDescriptor.Metadata.AbsoluteUri, true);
            Assert.Equal(_baseUri + "Products(-10)/Default.RetrieveProduct", productOperationDescriptor.Target.AbsoluteUri, true);

            Common.Clients.EndToEnd.OrderLine orderLine = _context.Execute<Common.Clients.EndToEnd.OrderLine>(new Uri(_baseUri + "OrderLines(OrderId=-10,ProductId=-10)?$select=*")).Single();
            Assert.Empty(_context.GetEntityDescriptor(orderLine).OperationDescriptors);
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

        private void VerifyLinks(List<(string, string)> expectedValues, IEnumerable<OperationDescriptor> actualDescriptors)
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

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "actionoverloading/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
