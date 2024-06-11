//-----------------------------------------------------------------------------
// <copyright file="ActionOverloadingQueryTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Client.E2E.TestCommon;
using Microsoft.OData.Client.E2E.TestCommon.Common;
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Client.Default;
using Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Tests
{
    public class ActionOverloadingQueryTests : EndToEndTestBase<ActionOverloadingQueryTests.TestsStartup>
    {
        private readonly Uri _baseUri;
        private readonly Container _context;
        private IEdmModel _model = null;
        public const string PersonTypeName = "Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Person";
        public const string EmployeeTypeName = "Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Employee";
        public const string SpecialEmployeeTypeName = "Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.SpecialEmployee";
        public const string ContractorTypeName = "Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Contractor";
        public const string ProductTypeName = "Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.Product";
        public const string OrderLineTypeName = "Microsoft.OData.Client.E2E.Tests.ActionOverloadingTests.Server.OrderLine";
        public const string MetadataPrefix = "$metadata#Default.";
        public const string ContainerPrefix = "#Default.";

        public class TestsStartup : TestStartupBase
        {
            public override void ConfigureServices(IServiceCollection services)
            {
                services.ConfigureControllers(typeof(ProductsController), typeof(OrderLinesController), typeof(PeopleController));

                services.AddControllers().AddOData(opt => opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(null)
                    .AddRouteComponents("odata", ActionOverloadingEdmModel.GetEdmModel()));
            }
        }

        public ActionOverloadingQueryTests(TestWebApplicationFactory<TestsStartup> fixture)
            : base(fixture)
        {
            _baseUri = new Uri(Client.BaseAddress, "odata/");
            _context = new Container(_baseUri);
            _context.HttpClientFactory = HttpClientFactory;
            _model = ActionOverloadingEdmModel.GetEdmModel();
        }

        /// <summary>
        /// Verify metadata of the operations defined in the test service.
        /// </summary>
        [Fact]
        public void QueryMetadataTest()
        {
            Dictionary<string, string> expectedUpdatePersonInfoOperations = new Dictionary<string, string>()
            {
                { "", null }, // the unbound action
            };
            Dictionary<string, string> expectedRetrieveProductOperations = new Dictionary<string, string>()
            {
                { "", null }, // the service operation
            };

            IEnumerable<IEdmOperationImport> actionImports = _model.EntityContainer.OperationImports();

            this.VerifyOperationsInMetadata(expectedUpdatePersonInfoOperations, actionImports.Where(fi => fi.Name == "UpdatePersonInfo"));
            this.VerifyOperationsInMetadata(expectedRetrieveProductOperations, actionImports.Where(fi => fi.Name == "RetrieveProduct"));
        }

        /// <summary>
        /// Verify actions in entry payload format atom, json verbose, and json fullmetadata.
        /// </summary>
        [Fact]
        public void QueryEntryTest()
        {
            List<string> mimeTypes = new List<string>()
            {
                //MimeTypes.ApplicationAtomXml,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            };

            string updatePersonInfo = "UpdatePersonInfo";
            string updatePersonInfoName = "Default.UpdatePersonInfo";
            string retrieveProduct = "RetrieveProduct";
            string retrieveProductName = "Default.RetrieveProduct";
            string increaseEmployeeSalary = "IncreaseEmployeeSalary";
            string increaseEmployeeSalaryName = "Default.IncreaseEmployeeSalary";

            foreach (string mimeType in mimeTypes)
            {
                List<Tuple<string, string>> expectedActionsOnPerson = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(-1)/" + updatePersonInfoName),
                };
                List<Tuple<string, string>> expectedActionsOnEmployee = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(0)/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(0)/" + EmployeeTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + increaseEmployeeSalary, "People(0)/" + EmployeeTypeName + "/" + increaseEmployeeSalaryName),
                };
                List<Tuple<string, string>> expectedActionsOnSpecialEmployee = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(-7)/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(-7)/" + EmployeeTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(-7)/" + SpecialEmployeeTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + increaseEmployeeSalary, "People(-7)/" + SpecialEmployeeTypeName + "/" + increaseEmployeeSalaryName),
                    new Tuple<string, string>(MetadataPrefix + increaseEmployeeSalary, "People(-7)/" + EmployeeTypeName + "/" + increaseEmployeeSalaryName),
                };
                List<Tuple<string, string>> expectedActionsOnContractor = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(1)/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "People(1)/" + ContractorTypeName + "/" + updatePersonInfoName),
                };
                List<Tuple<string, string>> expectedActionsOnProduct = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + retrieveProduct, "Products(-10)/" + retrieveProductName),
                };
                List<Tuple<string, string>> expectedActionsOnOrderLine = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + retrieveProduct, "OrderLines(OrderId=-10,ProductId=-10)/" + retrieveProductName),
                };

                this.VerifyActionInEntityPayload("People(-1)", expectedActionsOnPerson, mimeType);
                this.VerifyActionInEntityPayload("People(0)", expectedActionsOnEmployee, mimeType);
                this.VerifyActionInEntityPayload("People(-7)", expectedActionsOnSpecialEmployee, mimeType);
                this.VerifyActionInEntityPayload("People(1)", expectedActionsOnContractor, mimeType);
                this.VerifyActionInEntityPayload("Products(-10)", expectedActionsOnProduct, mimeType);
                this.VerifyActionInEntityPayload("OrderLines(OrderId=-10,ProductId=-10)", expectedActionsOnOrderLine, mimeType);
            }
        }

        private void VerifyOperationsInMetadata(Dictionary<string, string> expectedOperations, IEnumerable<IEdmOperationImport> actualActionImports)
        {
            //Wrong number of ActionImport
            Assert.Equal(expectedOperations.Count, actualActionImports.Count());

            // Verify the binding type of the ActionImport in metadata.
            foreach (KeyValuePair<string, string> operation in expectedOperations)
            {
                if (operation.Key == string.Empty)
                {
                    //Action not found
                    Assert.NotNull(actualActionImports.Where(fi => !fi.Operation.Parameters.Any()).SingleOrDefault());
                }
                else
                {
                    IEdmOperationImport actionImport = actualActionImports.Single(fi => fi.Operation.Parameters.Any() && fi.Operation.Parameters.First().Name == operation.Key);
                    IEdmTypeReference bindingParameterType = actionImport.Operation.Parameters.First().Type;
                    if (bindingParameterType.IsCollection())
                    {
                        Assert.Equal(operation.Value, bindingParameterType.AsCollection().ElementType().FullName());
                    }
                    else
                    {
                        Assert.Equal(operation.Value, bindingParameterType.FullName());
                    }
                }
            }
        }

        private void VerifyActionInEntityPayload(string queryUri, List<Tuple<string, string>> expectedActions, string acceptMimeType)
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
                    }
                }
                Assert.True(matched);
            }
        }
    }
}
