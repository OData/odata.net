//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingQueryTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server;
using Microsoft.OData.Client.E2E.Tests.Common.Clients.EndToEnd.Default;
using Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Tests
{
    public class ActionOverloadingQueryTests : EndToEndTestBase<ActionOverloadingQueryTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private IEdmModel _model = null;
        public const string PersonTypeName = "Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Person";
        public const string EmployeeTypeName = "Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Employee";
        public const string SpecialEmployeeTypeName = "Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.SpecialEmployee";
        public const string ContractorTypeName = "Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Contractor";
        public const string ProductTypeName = "Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.Product";
        public const string OrderLineTypeName = "Microsoft.OData.Client.E2E.Tests.Common.Server.EndToEnd.OrderLine";
        public const string MetadataPrefix = "$metadata#Default.";
        public const string ContainerPrefix = "#Default.";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ActionOverloadingQueryTestsController), typeof(MetadataController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", CommonEndToEndEdmModel.GetEdmModel()));
            }
        }

        public ActionOverloadingQueryTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri)
            {
                HttpClientFactory = HttpClientFactory
            };

            _model = CommonEndToEndEdmModel.GetEdmModel();

            ResetDataSource();
        }

        /// <summary>
        /// Verifies that the specified operations are present in the test service model.
        /// </summary>
        [Fact]
        public void VerifyServiceOperationsInModel()
        {
            var expectedOperations = new List<string>
            {
                "UpdatePersonInfo",
                "RetrieveProduct"
            };

            var actionImports = _model.EntityContainer.OperationImports();

            foreach (var operationName in expectedOperations)
            {
                var import = actionImports.FirstOrDefault(fi => fi.Name == operationName);
                Assert.NotNull(import);
            }
        }

        /// <summary>
        /// Verifies that specific actions are included in the payloads of various entities 
        /// in different MIME types (specifically JSON with full metadata).
        /// </summary>
        [Fact]
        public void VerifyActionsInEntryPayloads()
        {
            // Define the MIME types to be tested
            var mimeTypes = new List<string>
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            };

            // Define actions and their corresponding fully qualified names
            var updatePersonInfo = "UpdatePersonInfo";
            var updatePersonInfoName = "Default.UpdatePersonInfo";
            var retrieveProduct = "RetrieveProduct";
            var retrieveProductName = "Default.RetrieveProduct";
            var increaseEmployeeSalary = "IncreaseEmployeeSalary";
            var increaseEmployeeSalaryName = "Default.IncreaseEmployeeSalary";
            var sack = "Sack";
            var sackName = "Default.Sack";

            // Iterate over each MIME type
            foreach (var mimeType in mimeTypes)
            {
                // Define expected actions for different entity types

                // Expected actions for a person entity with ID -1
                var expectedActionsOnPerson = new List<(string, string)>
                {
                    (MetadataPrefix + updatePersonInfo, $"People(-1)/{updatePersonInfoName}")
                };

                // Expected actions for an employee entity with ID 0
                var expectedActionsOnEmployee = new List<(string, string)>
                {
                    (MetadataPrefix + updatePersonInfo, $"People(0)/{updatePersonInfoName}"),
                    (MetadataPrefix + updatePersonInfo, $"People(0)/{EmployeeTypeName}/{updatePersonInfoName}"),
                    (MetadataPrefix + increaseEmployeeSalary, $"People(0)/{EmployeeTypeName}/{increaseEmployeeSalaryName}"),
                    (MetadataPrefix + sack, $"People(0)/{EmployeeTypeName}/{sackName}")
                };

                // Expected actions for a special employee entity with ID -7
                var expectedActionsOnSpecialEmployee = new List<(string, string)>
                {
                    (MetadataPrefix + updatePersonInfo, $"People(-7)/{updatePersonInfoName}"),
                    (MetadataPrefix + updatePersonInfo, $"People(-7)/{EmployeeTypeName}/{updatePersonInfoName}"),
                    (MetadataPrefix + updatePersonInfo, $"People(-7)/{SpecialEmployeeTypeName}/{updatePersonInfoName}"),
                    (MetadataPrefix + increaseEmployeeSalary, $"People(-7)/{SpecialEmployeeTypeName}/{increaseEmployeeSalaryName}"),
                    (MetadataPrefix + increaseEmployeeSalary, $"People(-7)/{EmployeeTypeName}/{increaseEmployeeSalaryName}"),
                    (MetadataPrefix + sack, $"People(-7)/{EmployeeTypeName}/{sackName}")
                };

                // Expected actions for a contractor entity with ID 1
                var expectedActionsOnContractor = new List<(string, string)>
                {
                    (MetadataPrefix + updatePersonInfo, $"People(1)/{updatePersonInfoName}"),
                    (MetadataPrefix + updatePersonInfo, $"People(1)/{ContractorTypeName}/{updatePersonInfoName}")
                };

                // Expected actions for a product entity with ID -10
                var expectedActionsOnProduct = new List<(string, string)>
                {
                    (MetadataPrefix + retrieveProduct, $"Products(-10)/{retrieveProductName}")
                };

                // Expected actions for an order line entity with composite ID (-10, -10)
                var expectedActionsOnOrderLine = new List<(string, string)>
                {
                    (MetadataPrefix + retrieveProduct, $"OrderLines(OrderId=-10,ProductId=-10)/{retrieveProductName}")
                };

                // Verify the actions in the payload for each entity type
                VerifyActionInEntityPayload("People(-1)", expectedActionsOnPerson, mimeType);
                VerifyActionInEntityPayload("People(0)", expectedActionsOnEmployee, mimeType);
                VerifyActionInEntityPayload("People(-7)", expectedActionsOnSpecialEmployee, mimeType);
                VerifyActionInEntityPayload("People(1)", expectedActionsOnContractor, mimeType);
                VerifyActionInEntityPayload("Products(-10)", expectedActionsOnProduct, mimeType);
                VerifyActionInEntityPayload("OrderLines(OrderId=-10,ProductId=-10)", expectedActionsOnOrderLine, mimeType);
            }
        }

        private void VerifyActionInEntityPayload(string queryUri, List<(string, string)> expectedActions, string acceptMimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings();
            var args = new DataServiceClientRequestMessageArgs(
                "GET",
                new Uri(_baseUri.AbsoluteUri + queryUri, UriKind.Absolute),
                usePostTunneling: false,
                new Dictionary<string, string>(),
                HttpClientFactory);

            var requestMessage = new HttpClientRequestMessage(args);
            requestMessage.SetHeader("Accept", acceptMimeType);

            var responseMessage = requestMessage.GetResponse();

            ODataResource entry = null;
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, _model))
            {
                var reader = messageReader.CreateODataResourceReader();

                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = (ODataResource)reader.Item;
                    }
                }
            }

            // Verify the expected action metadata and target against the actual ODataActions in the ODataEntry.
            Assert.Equal(expectedActions.Count, entry.Actions.Count());
            foreach (var expected in expectedActions)
            {
                var actions = entry.Actions.Where(a => a.Metadata.AbsoluteUri == _baseUri + expected.Item1);
                bool matched = false;
                foreach (var action in actions)
                {
                    if (_baseUri + expected.Item2 == action.Target.AbsoluteUri)
                    {
                        matched = true;
                        break;
                    }
                }

                Assert.True(matched, $"Failed to match action with metadata: {_baseUri + expected.Item1} and target: {_baseUri + expected.Item2}");
            }
        }

        private void ResetDataSource()
        {
            var actionUri = new Uri(_baseUri + "actionoverloadingquery/Default.ResetDataSource", UriKind.Absolute);
            _context.Execute(actionUri, "POST");
        }
    }
}
