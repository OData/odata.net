//---------------------------------------------------------------------
// <copyright file="ActionOverloadingQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ActionOverloadingTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ActionOverloadingQueryTests : EndToEndTestBase
    {
        public const string PersonTypeName = "Microsoft.Test.OData.Services.AstoriaDefaultService.Person";
        public const string EmployeeTypeName = "Microsoft.Test.OData.Services.AstoriaDefaultService.Employee";
        public const string SpecialEmployeeTypeName = "Microsoft.Test.OData.Services.AstoriaDefaultService.SpecialEmployee";
        public const string ContractorTypeName = "Microsoft.Test.OData.Services.AstoriaDefaultService.Contractor";
        public const string ProductTypeName = "Microsoft.Test.OData.Services.AstoriaDefaultService.Product";
        public const string OrderLineTypeName = "Microsoft.Test.OData.Services.AstoriaDefaultService.OrderLine";
        public const string MetadataPrefix = "$metadata#Microsoft.Test.OData.Services.AstoriaDefaultService.";
        public const string ContainerPrefix = "#Microsoft.Test.OData.Services.AstoriaDefaultService.";

        private IEdmModel model = null;

        public ActionOverloadingQueryTests()
            : base(ServiceDescriptors.ActionOverloadingService)
        {
        }

        public override void CustomTestInitialize()
        {
            // retrieve IEdmModel of the test service
            HttpWebRequestMessage message = new HttpWebRequestMessage(new Uri(this.ServiceUri.AbsoluteUri + "$metadata", UriKind.Absolute));
            message.SetHeader("Accept", MimeTypes.ApplicationXml);

            using (var messageReader = new ODataMessageReader(message.GetResponse()))
            {
                this.model = messageReader.ReadMetadataDocument();
            }
        }

        /// <summary>
        /// Verify metadata of the operations defined in the test service.
        /// </summary>
        [TestMethod]
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

            IEnumerable<IEdmOperationImport> actionImports = this.model.EntityContainer.OperationImports();

            this.VerifyOperationsInMetadata(expectedUpdatePersonInfoOperations, actionImports.Where(fi => fi.Name == "UpdatePersonInfo"));
            this.VerifyOperationsInMetadata(expectedRetrieveProductOperations, actionImports.Where(fi => fi.Name == "RetrieveProduct"));
        }

        /// <summary>
        /// Verify actions in entry payload format atom, json verbose, and json fullmetadata.
        /// </summary>
        [TestMethod]
        public void QueryEntryTest()
        {
            List<string> mimeTypes = new List<string>()
            {
                //MimeTypes.ApplicationAtomXml,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            };

            string updatePersonInfo = "UpdatePersonInfo";
            string updatePersonInfoName = "Microsoft.Test.OData.Services.AstoriaDefaultService.UpdatePersonInfo";
            string retrieveProduct = "RetrieveProduct";
            string retrieveProductName = "Microsoft.Test.OData.Services.AstoriaDefaultService.RetrieveProduct";
            string increaseEmployeeSalary = "IncreaseEmployeeSalary";
            string increaseEmployeeSalaryName = "Microsoft.Test.OData.Services.AstoriaDefaultService.IncreaseEmployeeSalary";

            foreach (string mimeType in mimeTypes)
            {
                List<Tuple<string, string>> expectedActionsOnPerson = new List<Tuple<string, string>>()
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(-1)/" + updatePersonInfoName),
                };
                List<Tuple<string, string>> expectedActionsOnEmployee = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(0)/" + PersonTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(0)/" + EmployeeTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + increaseEmployeeSalary, "Person(0)/" + EmployeeTypeName + "/" + increaseEmployeeSalaryName),
                };
                List<Tuple<string, string>> expectedActionsOnSpecialEmployee = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(-7)/" + PersonTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(-7)/" + EmployeeTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(-7)/" + SpecialEmployeeTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + increaseEmployeeSalary, "Person(-7)/" + SpecialEmployeeTypeName + "/" + increaseEmployeeSalaryName),
                    new Tuple<string, string>(MetadataPrefix + increaseEmployeeSalary, "Person(-7)/" + EmployeeTypeName + "/" + increaseEmployeeSalaryName),
                };
                List<Tuple<string, string>> expectedActionsOnContractor = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(1)/" + PersonTypeName + "/" + updatePersonInfoName),
                    new Tuple<string, string>(MetadataPrefix + updatePersonInfo, "Person(1)/" + ContractorTypeName + "/" + updatePersonInfoName),
                };
                List<Tuple<string, string>> expectedActionsOnProduct = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + retrieveProduct, "Product(-10)/" + retrieveProductName),
                };
                List<Tuple<string, string>> expectedActionsOnOrderLine = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>(MetadataPrefix + retrieveProduct, "OrderLine(OrderId=-10,ProductId=-10)/" + retrieveProductName),
                };

                this.VerifyActionInEntityPayload("Person(-1)", expectedActionsOnPerson, mimeType);
                this.VerifyActionInEntityPayload("Person(0)", expectedActionsOnEmployee, mimeType);
                this.VerifyActionInEntityPayload("Person(-7)", expectedActionsOnSpecialEmployee, mimeType);
                this.VerifyActionInEntityPayload("Person(1)", expectedActionsOnContractor, mimeType);
                this.VerifyActionInEntityPayload("Product(-10)", expectedActionsOnProduct, mimeType);
                this.VerifyActionInEntityPayload("OrderLine(OrderId=-10,ProductId=-10)", expectedActionsOnOrderLine, mimeType);
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        /// <summary>
        /// Verify actions in entry payload format json minimalmetadata, and json nometadata.
        /// </summary>
        [TestMethod]
        public void QueryEntryJsonLightTest()
        {
            List<string> mimeTypes = new List<string>()
            {
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
                MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
            };

            string updatePersonInfo = "UpdatePersonInfo";
            string retrieveProduct = "RetrieveProduct";
            string increaseEmployeeSalary = "IncreaseEmployeeSalary";

            foreach (string mimeType in mimeTypes)
            {
                List<string> expectedActionsOnPerson = new List<string>()
                {
                    "\"" + ContainerPrefix + updatePersonInfo,
                };
                List<string> expectedActionsOnEmployee = new List<string>()
                {
                    "\"" + ContainerPrefix + updatePersonInfo,
                    "\"" + ContainerPrefix + increaseEmployeeSalary,
                };
                List<string> expectedActionsOnSpecialEmployee = new List<string>()
                {
                    "\"" + ContainerPrefix + updatePersonInfo,
                    "\"" + ContainerPrefix + increaseEmployeeSalary,
                };

                List<string> expectedActionsOnContractor = new List<string>()
                {
                    "\"" + ContainerPrefix + updatePersonInfo,
                };

                List<string> expectedRetrieveProductAction = new List<string>()
                {
                    "\"" + ContainerPrefix + retrieveProduct,
                };

                this.VerifyActionInJsonLightPayload("Person(-1)", expectedActionsOnPerson, mimeType);
                this.VerifyActionInJsonLightPayload("Person(0)", expectedActionsOnEmployee, mimeType);
                this.VerifyActionInJsonLightPayload("Person(-7)", expectedActionsOnSpecialEmployee, mimeType);
                this.VerifyActionInJsonLightPayload("Person(1)", expectedActionsOnContractor, mimeType);
                this.VerifyActionInJsonLightPayload("Product(-10)", expectedRetrieveProductAction, mimeType);
                this.VerifyActionInJsonLightPayload("OrderLine(OrderId=-10,ProductId=-10)", expectedRetrieveProductAction, mimeType);
            }
        }
#endif

        private void VerifyOperationsInMetadata(Dictionary<string, string> expectedOperations, IEnumerable<IEdmOperationImport> actualActionImports)
        {
            Assert.AreEqual(expectedOperations.Count, actualActionImports.Count(), "Wrong number of ActionImport");

            // Verify the binding type of the ActionImport in metadata.
            foreach (KeyValuePair<string, string> operation in expectedOperations)
            {
                if (operation.Key == string.Empty)
                {
                    Assert.IsNotNull(actualActionImports.Where(fi => !fi.Operation.Parameters.Any()).SingleOrDefault(), "Action not found.");
                }
                else
                {
                    IEdmOperationImport actionImport = actualActionImports.Single(fi => fi.Operation.Parameters.Any() && fi.Operation.Parameters.First().Name == operation.Key);
                    IEdmTypeReference bindingParameterType = actionImport.Operation.Parameters.First().Type;
                    if (bindingParameterType.IsCollection())
                    {
                        Assert.AreEqual(operation.Value, bindingParameterType.AsCollection().ElementType().FullName());
                    }
                    else
                    {
                        Assert.AreEqual(operation.Value, bindingParameterType.FullName());
                    }
                }
            }
        }

        private void VerifyActionInEntityPayload(string queryUri, List<Tuple<string, string>> expectedActions, string acceptMimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings();
            var requestMessage = new HttpWebRequestMessage(new Uri(this.ServiceUri.AbsoluteUri + queryUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", acceptMimeType);

            var responseMessage = requestMessage.GetResponse();

            ODataResource entry = null;
            using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, this.model))
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
            Assert.AreEqual(expectedActions.Count, entry.Actions.Count());
            foreach (var expected in expectedActions)
            {
                var actions = entry.Actions.Where(a => a.Metadata.AbsoluteUri == this.ServiceUri + expected.Item1);
                bool matched = false;
                foreach (var action in actions)
                {
                    if (this.ServiceUri + expected.Item2 == action.Target.AbsoluteUri)
                    {
                        matched = true;
                    }
                }
                Assert.IsTrue(matched);
            }
        }

#if !(NETCOREAPP1_0 || NETCOREAPP2_0)
        private void VerifyActionInJsonLightPayload(string queryUri, List<string> expectedActionPayload, string acceptMimeType)
        {
            var verifyActionNotInPayload = (acceptMimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.ServiceUri.AbsoluteUri + queryUri);
            request.Accept = acceptMimeType;
            string responseString = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    responseString = reader.ReadToEnd();
                }
            }

            // Since ODL does not read nometadata payload and minimalmetadata action payload is the same as nometadata, we verify that the expected action payload exists (or does not exist) in the response string.
            foreach (string action in expectedActionPayload)
            {
                if (verifyActionNotInPayload)
                {
                    Assert.IsFalse(responseString.Contains(action));
                }
                else
                {
                    Assert.IsTrue(responseString.Contains(action));
                }
            }
        }
#endif
    }
}
