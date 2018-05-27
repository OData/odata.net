//---------------------------------------------------------------------
// <copyright file="ActionTestsWithLargePayload.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using System.Linq;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Provider = Microsoft.OData.Service.Providers;
    using t = System.Data.Test.Astoria;
    #endregion

    /// <summary>
    /// End-to-end test for actions with large number of parameters or advertisement of large number of actions.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class ActionTestsWithLargePayload
    {
        #region Initialize Class
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            service = SetupService();
        }
        #endregion

        #region TestSetup
        private static DSPServiceDefinition service;
        private static int ParameterPayloadSize = 1000;
        private static int TotalParameterPayloadSize = ParameterPayloadSize * 5 + 1;  // 5 different parameter types types and one binding parameter
        private static int CollectionParameterPayloadSize = 5000;
        private static int NumberOfActions = 1000;
        private static int TotalNumberOfActions = NumberOfActions + 2;
        private static Provider.ResourceType ComplexType = null;

        private class Customer
        {
            public int ID
            {
                get;
                set;
            }
        }

        private class AddressComplexType
        {
            public int ZipCode
            {
                set;
                get;
            }

            public string City
            {
                set;
                get;
            }
        }

        private static DSPServiceDefinition SetupService()
        {
            DSPMetadata metadata = new DSPMetadata("ActionsWithLargePayload", "AstoriaUnitTests.ActionTestsWithLargePayload");
            var customer = metadata.AddEntityType("Customer", null, null, false);
            metadata.AddKeyProperty(customer, "ID", typeof(int));
            var customers = metadata.AddResourceSet("Customers", customer);

            ComplexType = metadata.AddComplexType("AddressComplexType", null, null, false);
            metadata.AddPrimitiveProperty(ComplexType, "ZipCode", typeof(int));
            metadata.AddPrimitiveProperty(ComplexType, "City", typeof(string));

            DSPContext context = new DSPContext();
            metadata.SetReadOnly();

            DSPResource customer1 = new DSPResource(customer);
            customer1.SetValue("ID", 1);
            context.GetResourceSetEntities("Customers").Add(customer1);

            MyDSPActionProvider actionProvider = new MyDSPActionProvider();
            SetUpActionWithLargeParameterPayload(actionProvider, metadata, customer);
            SetUpActionWithLargeCollectionParameterPayload(actionProvider, metadata, customer);
            SetupLargeNumberOfActions(actionProvider, metadata, customer);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                Writable = true,
                ActionProvider = actionProvider,
            };

            return service;
        }

        private static void SetUpActionWithLargeCollectionParameterPayload(MyDSPActionProvider actionProvider, Provider.IDataServiceMetadataProvider metadata, Provider.ResourceType bindingParameterType)
        {
            List<Provider.ServiceActionParameter> parameters = new List<Provider.ServiceActionParameter>();

            if (bindingParameterType != null)
            {
                parameters.Add(new Provider.ServiceActionParameter("bindingParam", bindingParameterType));
            }

            parameters.Add(new Provider.ServiceActionParameter("collectionParameter", Provider.ResourceType.GetCollectionResourceType(Provider.ResourceType.GetPrimitiveResourceType(typeof(string)))));
            actionProvider.AddAction("ActionWithLargeCollectionParameterPayload", null, null, parameters, Provider.OperationParameterBindingKind.Sometimes, null, null);
        }

        private static void SetupLargeNumberOfActions(MyDSPActionProvider actionProvider, Provider.IDataServiceMetadataProvider metadata, Provider.ResourceType bindingParameterType)
        {
            List<Provider.ServiceActionParameter> parameters = new List<Provider.ServiceActionParameter>();

            if (bindingParameterType != null)
            {
                parameters.Add(new Provider.ServiceActionParameter("bindingParam", bindingParameterType));
            }

            for (int i = 0; i < NumberOfActions; ++i)
            {
                actionProvider.AddAction("Action_" + i, null, null, parameters, Provider.OperationParameterBindingKind.Sometimes, null, null);
            }
        }

        private static void SetUpActionWithLargeParameterPayload(MyDSPActionProvider actionProvider, Provider.IDataServiceMetadataProvider metadata, Provider.ResourceType bindingParameterType)
        {    
            List<Provider.ServiceActionParameter> parameters = new List<Provider.ServiceActionParameter>();
            if(bindingParameterType != null) 
            {
                parameters.Add(new Provider.ServiceActionParameter("bindingParam", bindingParameterType));
            }

            int j = 0;
            for (int i = 0; i < ActionTestsWithLargePayload.ParameterPayloadSize; ++i)
            {
                string pName = "p_int_" + j++;
                parameters.Add(new Provider.ServiceActionParameter(pName, Provider.ResourceType.GetPrimitiveResourceType(typeof(Int32))));

                pName = "p_string_" + j++;
                parameters.Add(new Provider.ServiceActionParameter(pName, Provider.ResourceType.GetPrimitiveResourceType(typeof(string))));

                pName = "p_double_" + j++;
                parameters.Add(new Provider.ServiceActionParameter(pName, Provider.ResourceType.GetPrimitiveResourceType(typeof(double))));

                pName = "p_datetimeoffset_" + j++;
                parameters.Add(new Provider.ServiceActionParameter(pName, Provider.ResourceType.GetPrimitiveResourceType(typeof(DateTimeOffset))));

                pName = "p_complex_" + j++;
                parameters.Add(new Provider.ServiceActionParameter(pName, ActionTestsWithLargePayload.ComplexType));
            }

            actionProvider.AddAction("ActionWithLargeParameterPayload", null, null, parameters, Provider.OperationParameterBindingKind.Sometimes, null, null);
        }

        private OperationParameter[] GetParametersWithLargePayload()
        {
            List<OperationParameter> parameters = new List<OperationParameter>();
            int j =0;
            for (int i = 0; i < ActionTestsWithLargePayload.ParameterPayloadSize; ++i)
            {
                parameters.Add(new BodyOperationParameter("p_int_" + j++, 5));
                parameters.Add(new BodyOperationParameter("p_string_" + j++, "chararrayvalue".ToCharArray()));
                parameters.Add(new BodyOperationParameter("p_double_" + j++, 4.5555));
                parameters.Add(new BodyOperationParameter("p_datetimeoffset_" + j++, DateTimeOffset.Now));
                parameters.Add(new BodyOperationParameter("p_complex_" + j++, new AddressComplexType() { City = "Redmond", ZipCode = 98052 }));   
            }

            return parameters.ToArray();
        }

        private OperationParameter[] GetCollectionParameterWithLargePayload()
        {
            List<OperationParameter> parameters = new List<OperationParameter>();
            List<char[]> collectionParameter = new List<char[]>();

            for (int i = 0; i < CollectionParameterPayloadSize; ++i)
            {
                collectionParameter.Add(("string_" + i).ToCharArray());
            }

            parameters.Add(new BodyOperationParameter("collectionParameter", collectionParameter));
            return parameters.ToArray();
        }

        public class MyDSPInvokableAction : Provider.IDataServiceInvokable
        {
            private Provider.ServiceAction action;
            private object[] parameters;

            #region IDataServiceInvokable Members

            public MyDSPInvokableAction(DataServiceOperationContext operationContext, Provider.ServiceAction action, object[] parameters)
            {
                this.action = action;
                this.parameters = parameters;
            }

            void Provider.IDataServiceInvokable.Invoke()
            {
                if (this.action.Name.Equals("ActionWithLargeCollectionParameterPayload"))
                {
                    IEnumerator enumerator = ((IEnumerable)this.parameters[1]).GetEnumerator();
                    int cnt = 0;
                    while (enumerator.MoveNext())
                    {
                        cnt++;
                    }
                    Assert.IsTrue(cnt == CollectionParameterPayloadSize, "unexpected collection parameter item count.");
                }
                else
                {
                    Assert.IsTrue(this.parameters.Count() == ActionTestsWithLargePayload.TotalParameterPayloadSize, "unexpected parameter count.");
                }
            }

            object Provider.IDataServiceInvokable.GetResult()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class MyDSPActionProvider : DSPActionProvider
        {
            public override Provider.IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, Provider.ServiceAction serviceAction, object[] parameterTokens)
            {
                return new MyDSPInvokableAction(operationContext, serviceAction, parameterTokens);
            }
        }

        #endregion

        #region TestMethods

        [TestMethod]
        public void ActionWithLargeParameterPayloadTests()
        {
            // Test action with large number of parameters.
            var testCases = new []
            {   
                new 
                {
                    RequestUri = "/Customers(1)/AstoriaUnitTests.ActionTestsWithLargePayload.ActionWithLargeParameterPayload",
                    ExpectedResults = new object[] { },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    OperationParameters = this.GetParametersWithLargePayload(),
                },
                new 
                {
                    RequestUri = "/Customers(1)/AstoriaUnitTests.ActionTestsWithLargePayload.ActionWithLargeCollectionParameterPayload",
                    ExpectedResults = new object[] { },
                    StatusCode = 204,
                    ExpectedReturnType = typeof(void),
                    OperationParameters = this.GetCollectionParameterWithLargePayload(),
                },
            };

            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                t.TestUtil.RunCombinations(testCases, (testCase) =>
                    {
                        DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                        Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);
                        OperationResponse operationResponse = ctx.Execute(uri, "POST", testCase.OperationParameters);
                        Assert.IsNotNull(operationResponse);
                        Assert.AreEqual(testCase.StatusCode, operationResponse.StatusCode);
                        Assert.IsNull(operationResponse.Error);
                    });
            }
        }

        [Ignore] // Remove Atom
        [TestMethod]
        public void AdvertiseLargeNumberOfActionsTests()
        {
            // Test advertising large number of actions.
            var testCases = new[]
            {   
                new 
                {
                    RequestUri = "/Customers(1)",
                },
            };

            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                t.TestUtil.RunCombinations(testCases, (testCase) =>
                {
                    DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                    //ctx.EnableAtom = true;
                    //ctx.Format.UseAtom();
                    ctx.ResolveType = name => typeof(Customer);
                    Uri uri = new Uri(request.ServiceRoot + testCase.RequestUri);
                    QueryOperationResponse<object> qor = (QueryOperationResponse<object>)ctx.Execute<object>(uri);
                    Assert.IsNotNull(qor);
                    IEnumerator<object> entities = qor.GetEnumerator();
                    entities.MoveNext();
                    Assert.IsNotNull(entities.Current);
                    EntityDescriptor ed = ctx.GetEntityDescriptor(entities.Current);
                    Assert.IsNotNull(ed);
                    Assert.IsNotNull(ed.OperationDescriptors);
                    Assert.AreEqual(ed.OperationDescriptors.Count(), TotalNumberOfActions, "Invalid count of total number of advertised actions.");
                });
            }
        }

        #endregion
    }
}
