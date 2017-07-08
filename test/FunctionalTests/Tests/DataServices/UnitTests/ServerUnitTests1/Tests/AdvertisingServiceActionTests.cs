//---------------------------------------------------------------------
// <copyright file="AdvertisingServiceActionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTest = System.Data.Test.Astoria;

    #endregion Namespaces

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    /// <summary>This is a test class for Advertising service operations.</summary>
    [TestClass()]
    public class AdvertisingServiceActionsTests
    {
        private const string ServiceName = "TestService";
        private const string ServiceNamespace = "TestNamespace";
        private const string ActionPrefix = "action";
        private const string FunctionPrefix = "function";
        
        /// <summary>
        /// Creates a datasource from the given metadata.
        /// </summary>
        /// <param name="metadata">The metadata against which the datasource is created.</param>
        /// <returns>DSPContext representing the data source.</returns>
        private DSPContext CreateDataSource(DSPMetadata metadata)
        {
            DSPContext context = new DSPContext();
            ResourceType customerType = metadata.GetResourceType("Customer");
            ResourceType childCustomerType = metadata.GetResourceType("ChildCustomer");
            DSPResource entity1 = new DSPResource(customerType);
            entity1.SetValue("ID", 1);
            DSPResource entity2 = new DSPResource(customerType);
            entity2.SetValue("ID", 2);
            DSPResource entity3 = new DSPResource(childCustomerType);
            entity3.SetValue("ID", 3);
            context.GetResourceSetEntities("Customers").Add(entity1);
            context.GetResourceSetEntities("Customers").Add(entity2);
            context.GetResourceSetEntities("Customers").Add(entity3);

            ResourceType person = metadata.GetResourceType("Person");
            ResourceType impPerson = metadata.GetResourceType("ImportantPerson");
            ResourceType veryImpPerson = metadata.GetResourceType("VeryImportantPerson");

            DSPResource p1 = new DSPResource(person);
            p1.SetValue("ID", 4);
            DSPResource ip1 = new DSPResource(impPerson);
            ip1.SetValue("ID", 5);
            DSPResource vip1 = new DSPResource(veryImpPerson);
            vip1.SetValue("ID", 6);

            context.GetResourceSetEntities("AllPersons").Add(p1);
            context.GetResourceSetEntities("AllPersons").Add(ip1);
            context.GetResourceSetEntities("AllPersons").Add(vip1);

            context.GetResourceSetEntities("Persons").Add(p1);
            context.GetResourceSetEntities("ImportantPersons").Add(ip1);
            context.GetResourceSetEntities("VeryImportantPersons").Add(vip1);

            context.GetResourceSetEntities("ImpPersonsAndVeryImpPersons").Add(ip1);
            context.GetResourceSetEntities("ImpPersonsAndVeryImpPersons").Add(vip1);

            // resource to test property and name collision.
            DSPResource movie1 = new DSPResource(metadata.GetResourceType("Movie"));
            movie1.SetValue("ID", 1);
            movie1.SetValue("MovieA1", 1);
            movie1.SetValue("GoodMovieA2", 1);

            DSPResource goodMovie1 = new DSPResource(metadata.GetResourceType("GoodMovie"));
            goodMovie1.SetValue("ID", 2);
            goodMovie1.SetValue("MovieA1", 2);
            goodMovie1.SetValue("GoodMovieA2", 2);
            goodMovie1.SetValue("GoodMovieA1", 2);


            DSPResource veryGoodMovie1 = new DSPResource(metadata.GetResourceType("VeryGoodMovie"));
            veryGoodMovie1.SetValue("ID", 3);
            veryGoodMovie1.SetValue("MovieA1", 3);
            veryGoodMovie1.SetValue("GoodMovieA2", 3);
            veryGoodMovie1.SetValue("GoodMovieA1", 3);
            veryGoodMovie1.SetValue("VeryGoodMovieA1", 3);
            veryGoodMovie1.SetValue("GoodMovieA3", 3);

            context.GetResourceSetEntities("Movies").Add(movie1);
            context.GetResourceSetEntities("Movies").Add(goodMovie1);
            context.GetResourceSetEntities("Movies").Add(veryGoodMovie1);
            context.GetResourceSetEntities("GoodMovies").Add(goodMovie1);
            context.GetResourceSetEntities("GoodMovies").Add(veryGoodMovie1);
            context.GetResourceSetEntities("VeryGoodMovies").Add(veryGoodMovie1);

            return context;
        }

        /// <summary>
        /// Creates metadata for the service based on the provided type.
        /// </summary>
        /// <typeparam name="T">The type of the metadata to be created.</typeparam>
        /// <returns>The created metadata instance.</returns>
        private DSPMetadata CreateMetadata()
        {
            DSPMetadata metadata = new DSPMetadata(ServiceName, ServiceNamespace);
            ResourceType customerType = metadata.AddEntityType("Customer", null, null, false);
            ResourceType childCustomerType = metadata.AddEntityType("ChildCustomer", null, customerType, false);
            metadata.AddKeyProperty(customerType, "ID", typeof(int));
            ResourceSet customerSet = metadata.AddResourceSet("Customers", customerType);

            ResourceType personType = metadata.AddEntityType("Person", null, null, false);
            metadata.AddKeyProperty(personType, "ID", typeof(int));
            ResourceType importantPersonType = metadata.AddEntityType("ImportantPerson", null, personType, false);
            ResourceType veryImportantPersonType = metadata.AddEntityType("VeryImportantPerson", null, importantPersonType, false);

            ResourceSet allPersons = metadata.AddResourceSet("AllPersons", personType);
            ResourceSet persons = metadata.AddResourceSet("Persons", personType);
            ResourceSet importantPersons = metadata.AddResourceSet("ImportantPersons", importantPersonType);
            ResourceSet veryImportantPersons = metadata.AddResourceSet("VeryImportantPersons", veryImportantPersonType);
            ResourceSet impPersonsAndVeryImpPersons = metadata.AddResourceSet("ImpPersonsAndVeryImpPersons", importantPersonType);

            // Resource to test for property name and action name collision
            ResourceType movieType = metadata.AddEntityType("Movie", null, null, false);
            metadata.AddKeyProperty(movieType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(movieType, "MovieA1", typeof(int)); // There is an action with this name in Movie.
            metadata.AddPrimitiveProperty(movieType, "GoodMovieA2", typeof(int)); // There is an action with this name in GoodMovie.
            
            ResourceType goodMovieType = metadata.AddEntityType("GoodMovie", null, movieType, false);
            metadata.AddPrimitiveProperty(goodMovieType, "GoodMovieA1", typeof(int)); // There is an action with this name in GoodMovie.
            
            ResourceType veryGoodMovieType = metadata.AddEntityType("VeryGoodMovie", null, goodMovieType , false);
            metadata.AddPrimitiveProperty(veryGoodMovieType, "VeryGoodMovieA1", typeof(int)); // There is an action with this name in VeryGoodMovie
            metadata.AddPrimitiveProperty(veryGoodMovieType, "GoodMovieA3", typeof(int)); // There is an action with this name in GoodMovie.

            ResourceSet movies = metadata.AddResourceSet("Movies", movieType);
            ResourceSet goodMovies = metadata.AddResourceSet("GoodMovies", goodMovieType);
            ResourceSet veryGoodMovies = metadata.AddResourceSet("VeryGoodMovies", veryGoodMovieType);
            
            return metadata;
        }

        private class MyDSPActionProvider : DSPActionProvider
        {
            /// <summary>Function which implements GetServiceActionByResourceType method.</summary>
            public Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>> SubstituteGetServiceActionsByResourceType { get; set; }

            /// <summary>
            /// Gets a collection of actions having <paramref name="bindingParameterType"/> as the binding parameter type.
            /// </summary>
            /// <param name="operationContext">The data service operation context instance.</param>
            /// <param name="bindingParameterType">Instance of the binding parameter resource type (<see cref="ResourceType"/>) in question.</param>
            /// <returns>A list of actions having <paramref name="bindingParameterType"/> as the binding parameter type.</returns>
            public override IEnumerable<ServiceAction> GetServiceActionsByBindingParameterType(DataServiceOperationContext operationContext, ResourceType bindingParameterType)
            {
                if (this.SubstituteGetServiceActionsByResourceType != null)
                {
                    return this.SubstituteGetServiceActionsByResourceType(operationContext, bindingParameterType);
                }

                return base.GetServiceActionsByBindingParameterType(operationContext, bindingParameterType);
            }

            /// <summary>
            /// Determines whether a given <paramref name="serviceAction"/> should be advertised as bindable to the given <paramref name="resourceInstance"/>.
            /// </summary>
            /// <param name="operationContext">The data service operation context instance.</param>
            /// <param name="serviceAction">Service action to be advertised.</param>
            /// <param name="resourceInstance">Instance of the resource to which the service action is bound.</param>
            /// <param name="resourceInstanceInFeed">true if the resource instance to be serialized is inside a feed; false otherwise. The value true
            /// suggests that this method might be called many times during serialization since it will get called once for every resource instance inside
            /// the feed. If it is an expensive operation to determine whether to advertise the service action for the <paramref name="resourceInstance"/>,
            /// the provider may choose to always advertise in order to optimize for performance.</param>
            /// <param name="actionToSerialize">The <see cref="ODataAction"/> to be serialized. The server constructs 
            /// the version passed into this call, which may be replaced by an implementation of this interface.
            /// This should never be set to null unless returning false.</param>
            /// <returns>true if the service action should be advertised; false otherwise.</returns>
            public delegate bool AdvertiseServiceActionDelegate(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize);

            /// <summary>The AdvertiseServiceActionDelegate delegate.</summary>
            public AdvertiseServiceActionDelegate SubstituteAdvertiseServiceAction { get; set; }

            /// <summary>
            /// Determines whether a given <paramref name="serviceAction"/> should be advertised as bindable to the given <paramref name="resourceInstance"/>.
            /// </summary>
            /// <param name="operationContext">The data service operation context instance.</param>
            /// <param name="serviceAction">Service action to be advertised.</param>
            /// <param name="resourceInstance">Instance of the resource to which the service action is bound.</param>
            /// <param name="resourceInstanceInFeed">true if the resource instance to be serialized is inside a feed; false otherwise. The value true
            /// suggests that this method might be called many times during serialization since it will get called once for every resource instance inside
            /// the feed. If it is an expensive operation to determine whether to advertise the service action for the <paramref name="resourceInstance"/>,
            /// the provider may choose to always advertise in order to optimize for performance.</param>
            /// <param name="actionToSerialize">The <see cref="ODataAction"/> to be serialized. The server constructs 
            /// the version passed into this call, which may be replaced by an implementation of this interface.
            /// This should never be set to null unless returning false.</param>
            /// <returns>true if the service action should be advertised; false otherwise.</returns>
            public override bool AdvertiseServiceAction(DataServiceOperationContext operationContext, ServiceAction serviceAction, object resourceInstance, bool resourceInstanceInFeed, ref ODataAction actionToSerialize)
            {
                if (SubstituteAdvertiseServiceAction != null)
                {
                    return SubstituteAdvertiseServiceAction(operationContext, serviceAction, resourceInstance, resourceInstanceInFeed, ref actionToSerialize);
                }

                return base.AdvertiseServiceAction(operationContext, serviceAction, resourceInstance, resourceInstanceInFeed, ref actionToSerialize);
            }

            /// <summary>Function which implements GetServiceActionByResourceType method.</summary>
            internal Func<DataServiceOperationContext, IEnumerable<ServiceAction>> SubstituteGetServiceActions { get; set; }

            /// <summary>
            /// Returns all service actions in the provider.
            /// </summary>
            /// <param name="operationContext">The data service operation context instance.</param>
            /// <returns>An enumeration of all service actions.</returns>
            public override IEnumerable<ServiceAction> GetServiceActions(DataServiceOperationContext operationContext)
            {
                if (this.SubstituteGetServiceActions != null)
                {
                    return this.SubstituteGetServiceActions(operationContext);
                }

                return base.GetServiceActions(operationContext);
            }

            internal Func<DataServiceOperationContext, ServiceAction, object[], IDataServiceInvokable> SubstituteCreateInvokable { get; set; }

            public override IDataServiceInvokable CreateInvokable(DataServiceOperationContext operationContext, ServiceAction serviceAction, object[] parameterTokens)
            {
                if (this.SubstituteCreateInvokable != null)
                {
                    return this.SubstituteCreateInvokable(operationContext, serviceAction, parameterTokens);
                }

                return base.CreateInvokable(operationContext, serviceAction, parameterTokens);
            }
        }

        private class TestActionProviderWithResolution : MyDSPActionProvider, IDataServiceActionResolver
        {
            /// <summary>Function which implements GetServiceActionByResourceType method.</summary>
            internal Func<DataServiceOperationContext, ServiceActionResolverArgs, ServiceAction> SubstituteTryResolveServiceAction { get; set; }

            public bool TryResolveServiceAction(DataServiceOperationContext operationContext, ServiceActionResolverArgs resolverArgs, out ServiceAction serviceAction)
            {
                if (this.SubstituteTryResolveServiceAction != null)
                {
                    serviceAction = this.SubstituteTryResolveServiceAction(operationContext, resolverArgs);
                    return serviceAction != null;
                }

                throw new NotImplementedException();
            }
        }

        private class DelegatingInvokable : IDataServiceInvokable
        {
            private readonly Action invoke;
            private readonly Func<object> getResult;

            internal DelegatingInvokable(Action invoke, Func<object> getResult)
            {
                this.invoke = invoke;
                this.getResult = getResult;
            }

            public void Invoke()
            {
                this.invoke();
            }

            public object GetResult()
            {
                return this.getResult();
            }
        }

        private static MethodInfo GetMethodInfoFromLambdaBody<TResult>(Expression<Func<TResult>> lambda)
        {
            return ((MethodCallExpression)lambda.Body).Method;
        }

        private static MethodInfo GetMethodInfoFromLambdaBody(Expression<Action> lambda)
        {
            return ((MethodCallExpression)lambda.Body).Method;
        }

        [DSPAction(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { ServiceNamespace + ".Customer" })]
        private static void operation1(DSPResource param1)
        {
        }
        [DSPAction(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { ServiceNamespace + ".Customer" })]
        private static void operation2(DSPResource param1)
        {
        }
        [DSPAction(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { ServiceNamespace + ".Customer" })]
        private static void operation3(DSPResource param1)
        {
        }
        [DSPAction(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { ServiceNamespace + ".Customer" })]
        private static void operation4(DSPResource param1)
        {
        }
        [DSPAction(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { ServiceNamespace + ".Person" })]
        private static void operation5(DSPResource param1)
        {
        }
        [DSPAction(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { ServiceNamespace + ".Customer" })]
        private static void Action1(DSPResource param1)
        {
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Description("Test that actions and functions are correctly advertised in the payload")]
        public void AdvertiseOperationsInPayload()
        {
            DSPMetadata metadata = this.CreateMetadata();

            var testCases = new[]
            {
                new 
                {
                    // single entity
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation1(new DSPResource())) },
                    Outputs = new [] { new { Title = "operation1", Target = "Customers(1)"} },
                    RequestUriString = "/Customers(1)",
                },
                new 
                {
                    // collection
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation2(new DSPResource())) },
                    Outputs = new [] { new {Title = "operation2", Target="Customers(1)" }, new {Title = "operation2", Target="Customers(2)" } },
                    RequestUriString = "/Customers",
                },

                new 
                {
                    // multiple actions and functions
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation3(new DSPResource())), GetMethodInfoFromLambdaBody(() => operation4(new DSPResource())) },
                    Outputs = new [] { new {Title = "operation3", Target="Customers(1)" }, new {Title = "operation4", Target="Customers(1)" } },
                    RequestUriString = "/Customers(1)",
                },
            };

            foreach (var testCase in testCases)
            {
                MyDSPActionProvider actionProvider = new MyDSPActionProvider();
                DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = this.CreateDataSource, ActionProvider = actionProvider };
                service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    List<string> xPaths = new List<String>();

                    foreach(var method in testCase.Methods)
                    {
                        actionProvider.AddAction(method);
                    }

                    foreach (var output in testCase.Outputs)
                    {
                        AddXPath(xPaths, request.BaseUri, output.Title, output.Target);                        
                    }

                    request.RequestUriString = testCase.RequestUriString;
                    request.Accept = "application/atom+xml,application/xml";
                    request.SendRequest();

                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml, xPaths.ToArray());
                }
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Description("JsonLight should return relative uris for a default action, for atom and json verbose output is always absolute")]
        public void ValidateJsonLightRelativeOtherFormatsNot()
        {
            DSPMetadata metadata = this.CreateMetadata();

            var testCases = new[]
            {
                new 
                {
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation1(new DSPResource())) },
                    Output = "<m:action metadata=\"http://host/$metadata#TestNamespace.operation1\" title=\"operation1\" target=\"http://host/Customers(1)/TestNamespace.operation1\" />",
                    Accept = "application/atom+xml",
                    RequestUriString = "/Customers(1)",
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null
                },
                new 
                {
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation1(new DSPResource())) },
                    Output = "\"#TestNamespace.operation1\":{\"title\":\"operation1\",\"target\":\"Customers(1)/TestNamespace.operation1\"}",
                    Accept = "application/json;odata.metadata=full",
                    RequestUriString = "/Customers(1)",
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null
                },
                new 
                {
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation1(new DSPResource())) },
                    Output = "\"#TestNamespace.operation1\":{\"title\":\"Action2\",\"target\":\"http://differenthostbase/Customers(1)/TestNamespace.operation1\"}",
                    Accept = "application/json;odata.metadata=full",
                    RequestUriString = "/Customers(1)",
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        od = new ODataAction();
                        od.Metadata = new Uri("http://host/$metadata#TestNamespace.operation1", UriKind.RelativeOrAbsolute);
                        od.Title = "Action2";
                        od.Target = new Uri("http://differenthostbase/Customers(1)/TestNamespace.operation1");
                        return true;
                    }),
                },
                new 
                {
                    Methods = new [] { GetMethodInfoFromLambdaBody(() => operation1(new DSPResource())) },
                    Output = "\"#TestNamespace.operation1\":{}",
                    Accept = "application/json",
                    RequestUriString = "/Customers(1)",
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null
                },
            };

            foreach (var testCase in testCases)
            {
                MyDSPActionProvider actionProvider = new MyDSPActionProvider();
                DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = this.CreateDataSource, ActionProvider = actionProvider };
                service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

                using (TestWebRequest request = service.CreateForInProcess())
                {
                    foreach (var method in testCase.Methods)
                    {
                        actionProvider.AddAction(method);
                    }

                    actionProvider.SubstituteAdvertiseServiceAction = testCase.SubstituteIsSOAdvertisable;
                    request.Accept = testCase.Accept;
                    request.RequestUriString = testCase.RequestUriString;
                    request.SendRequest();

                    var actualResponseText = request.GetResponseStreamAsText();

                    Assert.IsTrue(actualResponseText.Contains(testCase.Output));
                }
            }
        }

        private Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>> GetSOByBindingParamType = 
            new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        List<ServiceAction> sos = new List<ServiceAction>();
                        ServiceAction sa1 = null;
                        ServiceAction sa2 = null;
                        ServiceAction sa3 = null;

                        switch(resourceType.Name)
                        {
                            case "Person":
                                sa1 = new ServiceAction("PersonAction", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) }); 
                                break;

                            case "ImportantPerson":
                                sa1 = new ServiceAction("ImportantPersonAction", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                break;

                            case "VeryImportantPerson":
                                sa1 = new ServiceAction("VeryImportantPersonAction", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                break;

                            case "Movie":
                                sa1 = new ServiceAction("MovieA1", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                sa2 = new ServiceAction("MovieA2", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                break;

                            case "GoodMovie":
                                sa1 = new ServiceAction("GoodMovieA1", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                sa2 = new ServiceAction("GoodMovieA2", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                sa3 = new ServiceAction("GoodMovieA3", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                break;

                            case "VeryGoodMovie":
                                sa1 = new ServiceAction("VeryGoodMovieA1", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", (ResourceType)resourceType) });
                                break;

                            default:
                                break;
                        }

                        if (sa1 != null)
                        {
                            sa1.SetReadOnly();
                            sos.Add(sa1);
                        }
                        if (sa2 != null)
                        {
                            sa2.SetReadOnly();
                            sos.Add(sa2);
                        }

                        if (sa3 != null)
                        {
                            sa3.SetReadOnly();
                            sos.Add(sa3);
                        }
                        
                        return sos;
                    });
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Description("Test that actions and functions are correctly advertised in the payload")]
        public void AdvertiseOperationsInPayload2()
        {
            DSPMetadata metadata =  this.CreateMetadata();
            ResourceType customerType = metadata.GetResourceType("Customer");
            MyDSPActionProvider actionProvider = new MyDSPActionProvider();
            actionProvider.AddAction(GetMethodInfoFromLambdaBody(() => Action1(new DSPResource())));

            var testCases = new []
            {
                #region multiple inheritance
                                
                // base type (Person)
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/AllPersons(4)", // AllPersons(4) is Person type.
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'PersonAction' and @target='http://host/AllPersons(4)/TestNamespace.PersonAction']", 
                        "//atom:entry[not(adsm:action[@title = 'ImportantPersonAction'])]", 
                        "//atom:entry[not(adsm:action[@title = 'VeryImportantPersonAction'])]"
                    }
                },

                // child type (ImportantPerson -> Person)
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/AllPersons(5)", // AllPersons(5) is ImportantPerson type. // The set type is of 'Person' type.
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'PersonAction' and @target='http://host/AllPersons(5)/TestNamespace.ImportantPerson/TestNamespace.PersonAction']", 
                        "//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/AllPersons(5)/TestNamespace.ImportantPerson/TestNamespace.ImportantPersonAction']", 
                        "//atom:entry[not(adsm:action[@title = 'VeryImportantPersonAction'])]"
                    }
                },

                // child type (VeryImportantPerson -> ImportantPerson -> Person)
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/AllPersons(6)", // AllPersons(6) is VeryImportantPerson type. Set type is of 'Person' type.
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'PersonAction'and @target='http://host/AllPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.PersonAction']", 
                        "//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/AllPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.ImportantPersonAction']", 
                        "//adsm:action[@title = 'VeryImportantPersonAction' and @target='http://host/AllPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.VeryImportantPersonAction']", 
                    }
                },

                // Collection containing only Person types
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/Persons",
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'PersonAction' and @target='http://host/Persons(4)/TestNamespace.PersonAction']", 
                        "//atom:entry[not(adsm:action[@title = 'ImportantPersonAction'])]", 
                        "//atom:entry[not(adsm:action[@title = 'VeryImportantPersonAction'])]"
                    }
                },

                // Collection containing only ImportantPerson types
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/ImportantPersons",
                    XPaths = new string[] { 
                        "//adsm:action[@title = 'PersonAction' and @target='http://host/ImportantPersons(5)/TestNamespace.PersonAction']", 
                        "//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/ImportantPersons(5)/TestNamespace.ImportantPersonAction']",
                        "//atom:entry[not(adsm:action[@title = 'VeryImportantPersonAction'])]"
                    }
                },

                // Collection containing only VeryImportantPerson types
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/VeryImportantPersons",
                    XPaths = new string[] { 
                        "//adsm:action[@title = 'PersonAction' and @target='http://host/VeryImportantPersons(6)/TestNamespace.PersonAction']", 
                        "//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/VeryImportantPersons(6)/TestNamespace.ImportantPersonAction']", 
                        "//adsm:action[@title = 'VeryImportantPersonAction' and @target='http://host/VeryImportantPersons(6)/TestNamespace.VeryImportantPersonAction']", 
                    }
                },

                // Collection containing multiple types (Person, ImportantPerson, VeryImportantPerson)
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/AllPersons", // set type is "Person"
                    XPaths = new string[] { 

                        // Person
                        "(//adsm:action[@title = 'PersonAction' and @target='http://host/AllPersons(4)/TestNamespace.PersonAction'])",
 
                        // ImportantPerson
                        "(//adsm:action[@title = 'PersonAction' and @target='http://host/AllPersons(5)/TestNamespace.ImportantPerson/TestNamespace.PersonAction'])",
                        "(//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/AllPersons(5)/TestNamespace.ImportantPerson/TestNamespace.ImportantPersonAction'])",
                        
                        // VeryImportantPerson
                        "(//adsm:action[@title = 'PersonAction' and @target='http://host/AllPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.PersonAction'])",
                        "(//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/AllPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.ImportantPersonAction'])",
                        "(//adsm:action[@title = 'VeryImportantPersonAction' and @target='http://host/AllPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.VeryImportantPersonAction'])",     
                    }
                },
     
                // Collection containing important persons and very important persons
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/ImpPersonsAndVeryImpPersons", // set type is "ImportantPerson"
                    XPaths = new string[] { 

                        // ImportantPerson
                        "(//adsm:action[@title = 'PersonAction' and @target='http://host/ImpPersonsAndVeryImpPersons(5)/TestNamespace.PersonAction'])",
                        "(//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/ImpPersonsAndVeryImpPersons(5)/TestNamespace.ImportantPersonAction'])",
                        
                        // VeryImportantPerson
                        "(//adsm:action[@title = 'PersonAction' and @target='http://host/ImpPersonsAndVeryImpPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.PersonAction'])",
                        "(//adsm:action[@title = 'ImportantPersonAction' and @target='http://host/ImpPersonsAndVeryImpPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.ImportantPersonAction'])",
                        "(//adsm:action[@title = 'VeryImportantPersonAction' and @target='http://host/ImpPersonsAndVeryImpPersons(6)/TestNamespace.VeryImportantPerson/TestNamespace.VeryImportantPersonAction'])",     
                    }
                 },
       
                #endregion

                // Verify that action names are case-sensitive
                new 
                {
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action1 = new ServiceAction("action1", null, null, OperationParameterBindingKind.Sometimes, parameters);
                        ServiceAction action2= new ServiceAction("Action1", null, null, OperationParameterBindingKind.Sometimes, parameters); 
                        action1.SetReadOnly();
                        action2.SetReadOnly();
                        sas.Add(action1);
                        sas.Add(action2);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//adsm:action[@title='Action1']", "//adsm:action[@title='action1']" }
                },

                // return null at metadata level
                new 
                {
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        return null;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//atom:entry[not(adsm:action)]" },
                },

                // return empty list at metadata level
                new
                {
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        List<ServiceAction> serviceOperations = new List<ServiceAction>();
                        return serviceOperations;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//atom:entry[not(adsm:action)]" }
                },

                // return false at instance level
                new
                {
                    SubstituteGetSOByResourceType = (Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>)null,
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        return false;
                    }),
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//atom:entry[not(adsm:action)]" }
                },

                // return a new ODataOperation at instance level
                new
                {
                    SubstituteGetSOByResourceType = (Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>)null,
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        od = new ODataAction();
                        od.Metadata = new Uri("somemetadata", UriKind.RelativeOrAbsolute);
                        od.Title = "Action2";
                        od.Target = new Uri("http://host/Customers(1)");
                        return true;
                    }),
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//atom:entry[not(adsm:action[@title ='Action1'])]", "//adsm:action[@title = 'Action2']" }  
                },
                                 
                // return null at metadata level
                new 
                {
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        return null;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//atom:entry[not(adsm:action)]" },
                },
                
                // IsAssignable test --> ServiceAction has base type - 'Customer', while request is for child type - 'ChildCustomer'
                new 
                {
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        if(resourceType == customerType)
                        {
                            List<ServiceAction> sos = new List<ServiceAction>();
                            ServiceAction action = new ServiceAction("Action1", null, (ResourceSet)null, OperationParameterBindingKind.Sometimes, 
                                new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) });
                            action.SetReadOnly();
                            sos.Add(action);
                            return sos;
                        }
                        else 
                        {
                            return null;
                        }
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = "/Customers(3)", // Customers(3) is a ChildCustomer
                    XPaths = new string[] { "//adsm:action[@title='Action1']" }
                },

                // change the target of the ODataOperation in the query provider
                new 
                {
                    SubstituteGetSOByResourceType = (Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>)null,
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        od.Target = new Uri("http://host/Customers(2)");
                        return true;
                    }), 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { string.Format(System.Globalization.CultureInfo.InvariantCulture, "//adsm:{0}[@title='{1}' and @target='{2}']", ActionPrefix, "Action1", "http://host/Customers(2)") } 
                },

                // OperationParameterBindingKind = Always and return false from "AdvertiseServiceAction". Action should be advertised since "AdvertiseServiceAction" will not be called.
                new 
                {           
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        List<ServiceAction> sos = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("Action2", null, null, OperationParameterBindingKind.Always, new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) });
                        action.SetReadOnly();
                        sos.Add(action);
                        return sos;
                    }),

                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsFalse(true, "This should have never been called since isAlwaysAvailable was set to true.");
                        return false;
                    }), 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//adsm:action[@title='Action2']" }
                },

                // OperationParameterBindingKind = Sometimes and return true from "AdvertiseServiceAction". Action should be advertised.
                new 
                {           
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        List<ServiceAction> sos = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("Action2", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) });
                        action.SetReadOnly();
                        sos.Add(action);
                        return sos;
                    }),

                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(true, "This should have been called since isAlwaysAvailable was set to false.");
                        return true;
                    }), 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//adsm:action[@title='Action2']" }
                },

                // Action2.OperationParameterBindingKind = "Always", Action3.OperationParameterBindingKind = "Sometimes"
                new 
                {           
                    SubstituteGetSOByResourceType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((operationContext, resourceType) =>
                    {
                        List<ServiceAction> sos = new List<ServiceAction>();
                        ServiceAction action2 = new ServiceAction("Action2", null, null, OperationParameterBindingKind.Always, new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) });
                        ServiceAction action3 = new ServiceAction("Action3", null, null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) });
                        action2.SetReadOnly();
                        action3.SetReadOnly();
                        sos.Add(action2);
                        sos.Add(action3);
                        return sos;
                    }),

                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(!so.Name.Equals("Action2"), "This should not have been called for action2 since OperationParameterBindingKind is set to 'Always'");
                        Assert.IsTrue(so.Name.Equals("Action3"), "This should have been called for action3 since OperationParameterBindingKind is set to 'Sometimes'");
                        return true;
                    }), 
                    RequestUri = "/Customers(1)",
                    XPaths = new string[] { "//adsm:action[@title='Action2']", "//adsm:action[@title='Action3']" }
                },
            };

            DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = this.CreateDataSource, ActionProvider = actionProvider };
            service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            using (TestWebRequest request = service.CreateForInProcess())
            {
                foreach (var testCase in testCases)
                {
                    request.RequestUriString = testCase.RequestUri;
                    request.Accept = "application/atom+xml,application/xml";
                    actionProvider.SubstituteGetServiceActionsByResourceType = (Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>)testCase.SubstituteGetSOByResourceType;
                    actionProvider.SubstituteAdvertiseServiceAction = (MyDSPActionProvider.AdvertiseServiceActionDelegate)testCase.SubstituteIsSOAdvertisable;
                    
                    request.SendRequest();
                    UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml, testCase.XPaths);
                }
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Description("Test that actions are correctly advertised in the payload with property name collision")]
        public void AdvertiseOperationsWithPropertyNameCollision()
        {
            DSPMetadata metadata = this.CreateMetadata();
            MyDSPActionProvider actionProvider = new MyDSPActionProvider();

            var testCases = new[]
            {
                #region property and action name collision

                // Movie         : Properties:  ID, MovieA1, GoodMovieA2
                //                 Actions:     MovieA1, MovieA2
                // GoodMovie     : Properties:  GoodMovieA1 
                //                 Actions:     GoodMovieA1, GoodMovieA2, GoodMovieA3
                // VeryGoodMovie : Properties:  VeryGoodMovieA1, GoodMovieA3
                //                 Actions:     VeryGoodMovieA1

                // Set Type = movie, instance type = movie. 
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/Movies(1)", "/Movies/TestNamespace.Movie(1)", "/Movies(1)?$select=TestNamespace.*", "/Movies/TestNamespace.Movie(1)?$select=TestNamespace.*" }, 
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'MovieA1' and @metadata='http://host/$metadata#TestNamespace.MovieA1' and @target='http://host/Movies(1)/TestNamespace.MovieA1']", 
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/Movies(1)/TestNamespace.MovieA2']",    
                    }
                },

                // Set Type = movie, instance type = GoodMovie. 
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/Movies(2)", "/Movies/TestNamespace.GoodMovie(2)", "/Movies(2)?$select=TestNamespace.*", "/Movies/TestNamespace.GoodMovie(2)?$select=TestNamespace.*" }, 
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'GoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA1' and @target='http://host/Movies(2)/TestNamespace.GoodMovie/TestNamespace.GoodMovieA1']", 
                        "//adsm:action[@title = 'GoodMovieA2' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA2' and @target='http://host/Movies(2)/TestNamespace.GoodMovie/TestNamespace.GoodMovieA2']",    
                        "//adsm:action[@title = 'GoodMovieA3' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA3' and @target='http://host/Movies(2)/TestNamespace.GoodMovie/TestNamespace.GoodMovieA3']",    
                        "//adsm:action[@title = 'MovieA1' and @metadata='http://host/$metadata#TestNamespace.MovieA1' and @target='http://host/Movies(2)/TestNamespace.GoodMovie/TestNamespace.MovieA1']",    
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/Movies(2)/TestNamespace.GoodMovie/TestNamespace.MovieA2']",    
                    }
                },

                // Set Type = movie, instance type = VeryGoodMovie
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/Movies(3)", "/Movies/TestNamespace.VeryGoodMovie(3)", "/Movies(3)?$select=TestNamespace.*", "/Movies/TestNamespace.VeryGoodMovie(3)?$select=TestNamespace.*" }, 
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'VeryGoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.VeryGoodMovieA1' and @target='http://host/Movies(3)/TestNamespace.VeryGoodMovie/TestNamespace.VeryGoodMovieA1']", 
                        "//adsm:action[@title = 'GoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA1' and @target='http://host/Movies(3)/TestNamespace.VeryGoodMovie/TestNamespace.GoodMovieA1']",    
                        "//adsm:action[@title = 'GoodMovieA2' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA2' and @target='http://host/Movies(3)/TestNamespace.VeryGoodMovie/TestNamespace.GoodMovieA2']",    
                        "//adsm:action[@title = 'GoodMovieA3' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA3' and @target='http://host/Movies(3)/TestNamespace.VeryGoodMovie/TestNamespace.GoodMovieA3']",    
                        "//adsm:action[@title = 'MovieA1' and @metadata='http://host/$metadata#TestNamespace.MovieA1' and @target='http://host/Movies(3)/TestNamespace.VeryGoodMovie/TestNamespace.MovieA1']",    
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/Movies(3)/TestNamespace.VeryGoodMovie/TestNamespace.MovieA2']",    
                    }
                },

                // Set Type = GoodMovie, instance type = GoodMovie
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/GoodMovies(2)", "/GoodMovies/TestNamespace.GoodMovie(2)", "/GoodMovies(2)?$select=TestNamespace.*", "/GoodMovies/TestNamespace.GoodMovie(2)?$select=TestNamespace.*" }, 
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'GoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA1' and @target='http://host/GoodMovies(2)/TestNamespace.GoodMovieA1']",    
                        "//adsm:action[@title = 'GoodMovieA2' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA2' and @target='http://host/GoodMovies(2)/TestNamespace.GoodMovieA2']",    
                        "//adsm:action[@title = 'GoodMovieA3' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA3' and @target='http://host/GoodMovies(2)/TestNamespace.GoodMovieA3']",    
                        "//adsm:action[@title = 'MovieA1' and @metadata='http://host/$metadata#TestNamespace.MovieA1' and @target='http://host/GoodMovies(2)/TestNamespace.MovieA1']",    
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/GoodMovies(2)/TestNamespace.MovieA2']",    
                    }
                },

                // Set Type = GoodMovie, instance type = VeryGoodMovie
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/GoodMovies(3)", "/GoodMovies/TestNamespace.VeryGoodMovie(3)", "/GoodMovies(3)?$select=TestNamespace.*", "/GoodMovies/TestNamespace.VeryGoodMovie(3)?$select=TestNamespace.*" }, 
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'VeryGoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.VeryGoodMovieA1' and @target='http://host/GoodMovies(3)/TestNamespace.VeryGoodMovie/TestNamespace.VeryGoodMovieA1']", 
                        "//adsm:action[@title = 'GoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA1' and @target='http://host/GoodMovies(3)/TestNamespace.VeryGoodMovie/TestNamespace.GoodMovieA1']",    
                        "//adsm:action[@title = 'GoodMovieA2' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA2' and @target='http://host/GoodMovies(3)/TestNamespace.VeryGoodMovie/TestNamespace.GoodMovieA2']",    
                        "//adsm:action[@title = 'GoodMovieA3' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA3' and @target='http://host/GoodMovies(3)/TestNamespace.VeryGoodMovie/TestNamespace.GoodMovieA3']",    
                        "//adsm:action[@title = 'MovieA1' and @metadata='http://host/$metadata#TestNamespace.MovieA1' and @target='http://host/GoodMovies(3)/TestNamespace.VeryGoodMovie/TestNamespace.MovieA1']",    
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/GoodMovies(3)/TestNamespace.VeryGoodMovie/TestNamespace.MovieA2']",    
                    }
                },

                // Set Type = VeryGoodMovie, instance type = VeryGoodMovie
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/VeryGoodMovies(3)", "/VeryGoodMovies/TestNamespace.VeryGoodMovie(3)", "/VeryGoodMovies(3)?$select=TestNamespace.*", "/VeryGoodMovies/TestNamespace.VeryGoodMovie(3)?$select=TestNamespace.*" },  
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'VeryGoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.VeryGoodMovieA1' and @target='http://host/VeryGoodMovies(3)/TestNamespace.VeryGoodMovieA1']", 
                        "//adsm:action[@title = 'GoodMovieA1' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA1' and @target='http://host/VeryGoodMovies(3)/TestNamespace.GoodMovieA1']",    
                        "//adsm:action[@title = 'GoodMovieA2' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA2' and @target='http://host/VeryGoodMovies(3)/TestNamespace.GoodMovieA2']",    
                        "//adsm:action[@title = 'GoodMovieA3' and @metadata='http://host/$metadata#TestNamespace.GoodMovieA3' and @target='http://host/VeryGoodMovies(3)/TestNamespace.GoodMovieA3']",    
                        "//adsm:action[@title = 'MovieA1' and @metadata='http://host/$metadata#TestNamespace.MovieA1' and @target='http://host/VeryGoodMovies(3)/TestNamespace.MovieA1']",    
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/VeryGoodMovies(3)/TestNamespace.MovieA2']",    
                    }
                },

                 // Set Type = movie, instance type = movie, Projection = MovieA2 (No collisions)
                new
                {
                    SubstituteGetSOByResourceType = this.GetSOByBindingParamType,
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    RequestUri = new string[] { "/Movies(1)?$Select=TestNamespace.MovieA2", "/Movies/TestNamespace.Movie(1)?$Select=TestNamespace.MovieA2" },
                    XPaths = new string[] 
                    { 
                        "//adsm:action[@title = 'MovieA2' and @metadata='http://host/$metadata#TestNamespace.MovieA2' and @target='http://host/Movies(1)/TestNamespace.MovieA2']",    
                    }
                },

                #endregion
            };

            DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = this.CreateDataSource, ActionProvider = actionProvider };
            service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            List<string> failedTestUris = new List<string>();
            using (TestWebRequest request = service.CreateForInProcess())
            {
                AstoriaTest.TestUtil.RunCombinations(testCases, testCase =>
                {
                    AstoriaTest.TestUtil.RunCombinations(testCase.RequestUri, requestUri =>
                    {
                        try
                        {
                            request.RequestUriString = requestUri;
                            request.Accept = "application/atom+xml,application/xml";
                            actionProvider.SubstituteGetServiceActionsByResourceType = (Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>)testCase.SubstituteGetSOByResourceType;
                            actionProvider.SubstituteAdvertiseServiceAction = (MyDSPActionProvider.AdvertiseServiceActionDelegate)testCase.SubstituteIsSOAdvertisable;
                            request.SendRequest();
                            UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml, testCase.XPaths);
                        }
                        catch (Exception)
                        {
                            failedTestUris.Add(requestUri);
                            throw;
                        }
                   });
                });
            }
        }

        [TestCategory("Partition1"), TestMethod, Description("Error cases when advertising actions and functions in service operation")]
        public void AdvertiseOperationsInPayloadErrorCases()
        {
            DSPMetadata metadata = this.CreateMetadata();
            ResourceType customerType = metadata.GetResourceType("Customer");
            ResourceSet customerSet = metadata.ResourceSets.FirstOrDefault();
            ResourceType childCustomerType = metadata.GetResourceType("ChildCustomer");

            var TestCases = new []
            {
                // isbindable = false, entity = true
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", ResourceType.GetPrimitiveResourceType(typeof(int))) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("action1", null, null, OperationParameterBindingKind.Never, parameters);
                        action.SetReadOnly();
                        sas.Add(action);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    ErrorMessage = "The binding parameter for service action 'action1' returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() is null. The GetServiceActionsByBindingParameterType method must return service actions that are bindable to the given resource type.",
                },

                // ServiceOperationKind.ServiceOperation
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", ResourceType.GetPrimitiveResourceType(typeof(int))) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("operation", null, null, OperationParameterBindingKind.Never, parameters);
                        action.SetReadOnly();
                        sas.Add(action);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    ErrorMessage = "The binding parameter for service action 'operation' returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() is null. The GetServiceActionsByBindingParameterType method must return service actions that are bindable to the given resource type.",
                },

                // IsAssignable test - binding parameter is not assignable from resourceType
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", childCustomerType) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("action1", null, null, OperationParameterBindingKind.Sometimes, parameters);
                        action.SetReadOnly();
                        sas.Add(action);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    ErrorMessage = "The service action 'action1' returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() has a binding parameter of type 'TestNamespace.ChildCustomer' that is not bindable to the resource type 'TestNamespace.Customer'. The GetServiceActionsByBindingParameterType method must return service actions that are bindable to the given resource type.",
                },

                // IsAssignable test - binding parameter is an entity collection.
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", ResourceType.GetEntityCollectionResourceType(customerType)) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("action1", null, null, OperationParameterBindingKind.Sometimes, parameters);
                        action.SetReadOnly();
                        sas.Add(action);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    ErrorMessage = "The service action 'action1' returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() has a binding parameter of type 'Collection(TestNamespace.Customer)' that is not bindable to the resource type 'TestNamespace.Customer'. The GetServiceActionsByBindingParameterType method must return service actions that are bindable to the given resource type.",
                },

                // Another IsAssignable test - entity/entityset = false, isbindable = true
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", ResourceType.GetPrimitiveResourceType(typeof(int))) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("action1", null, null, OperationParameterBindingKind.Sometimes, parameters);
                        action.SetReadOnly();
                        sas.Add(action);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    ErrorMessage = "An action's binding parameter must be of type Entity or EntityCollection.\r\nParameter name: parameters",
                },

                // IDataServiceActionProvider.AdvertiseServiceAction says the action should be advertised but sets it to null
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action = new ServiceAction("action1", null, null, OperationParameterBindingKind.Sometimes, parameters);
                        action.SetReadOnly();
                        sas.Add(action);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        od = null;
                        return true;
                    }),
                    ErrorMessage = "The IDataServiceActionProvider.AdvertiseServiceAction() method must return a non-null value for the actionToSerialize parameter if it returns true.",
                },

                // duplicate service action names.
                new 
                {
                    SubstituteGetSAByBindingParameterType = new Func<DataServiceOperationContext, ResourceType, IEnumerable<ServiceAction>>((metadataProvider, resourceType) =>
                    {
                        var parameters = new ServiceActionParameter[] { new ServiceActionParameter("param1", customerType) };
                        List<ServiceAction> sas = new List<ServiceAction>();
                        ServiceAction action1 = new ServiceAction("action1", null, null, OperationParameterBindingKind.Sometimes, parameters);
                        ServiceAction action2= new ServiceAction("action1", null, null, OperationParameterBindingKind.Always, parameters); 
                        action1.SetReadOnly();
                        action2.SetReadOnly();
                        sas.Add(action1);
                        sas.Add(action2);
                        return sas;
                    }),
                    SubstituteIsSOAdvertisable = (MyDSPActionProvider.AdvertiseServiceActionDelegate)null, 
                    ErrorMessage = "A service action with the name 'action1' already exists. Please make sure that the list returned by IDataServiceActionProvider.GetServiceActionsByBindingParameterType() contains unique service action names.",
                },
            };

            MyDSPActionProvider actionProvider = new MyDSPActionProvider();
            DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = metadata, CreateDataSource = this.CreateDataSource, ActionProvider = actionProvider };
            service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Customers(1)";

                foreach (var tc in TestCases)
                {
                    actionProvider.SubstituteGetServiceActionsByResourceType = tc.SubstituteGetSAByBindingParameterType;
                    actionProvider.SubstituteAdvertiseServiceAction = (MyDSPActionProvider.AdvertiseServiceActionDelegate)tc.SubstituteIsSOAdvertisable;
                    Exception exception = AstoriaTest.TestUtil.RunCatching(delegate() { request.SendRequest(); });
                    Assert.AreEqual(tc.ErrorMessage, exception.InnerException.Message);
                }
            }
        }

        [Ignore]
        // [TestCategory("Partition1"), TestMethod, Description("Test the resourceInstanceInFeed parameter of the AdvertiseServiceAction method.")]
        public void AdvertiseServiceActionInFeedTest()
        {
            var testCases = new[]
            {
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsFalse(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set(1)",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsFalse(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set(1)/ResourceReferenceProperty",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set(1)/ResourceSetReferenceProperty",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set?$expand=ResourceReferenceProperty",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set?$expand=ResourceReferenceProperty($expand=ResourceReferenceProperty)",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        if ((int)((DSPResource)o).GetValue("ID") == 1)
                        {
                            Assert.IsFalse(inFeed);
                        }
                        else
                        {
                            Assert.IsTrue(inFeed);
                        }

                        return true;
                    }), 
                    RequestUri = "/Set(1)?$expand=ResourceSetReferenceProperty",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        if ((int)((DSPResource)o).GetValue("ID") == 1)
                        {
                            Assert.IsFalse(inFeed);
                        }
                        else
                        {
                            Assert.IsTrue(inFeed);
                        }

                        return true;
                    }), 
                    RequestUri = "/Set(1)?$expand=ResourceSetReferenceProperty($expand=ResourceReferenceProperty)",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set?$select=*,AstoriaUnitTests.Tests.Actions.*",
                },
                new 
                {           
                    SubstituteIsSOAdvertisable = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                    {
                        Assert.IsTrue(inFeed);
                        return true;
                    }), 
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                },
            };

            DSPServiceDefinition service = ActionTests.ModelWithActions();
            MyDSPActionProvider actionProvider = new MyDSPActionProvider();
            ActionTests.SetupActions(actionProvider, service.CurrentDataSource);
            service.ActionProvider = actionProvider;
            service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            using (TestWebRequest request = service.CreateForInProcess())
            {
                foreach (var testCase in testCases)
                {
                    request.RequestUriString = testCase.RequestUri;
                    actionProvider.SubstituteAdvertiseServiceAction = (MyDSPActionProvider.AdvertiseServiceActionDelegate)testCase.SubstituteIsSOAdvertisable;
                    request.SendRequest();
                }
            }
        }

        [TestCategory("Partition1"), TestMethod]
        public void MultipleOverloadsShouldSerializeInMetadata()
        {
            var service = this.CreateServiceWithActionOverloads();

            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = "/$metadata";
                request.SendRequest();
                var document = request.GetResponseStreamAsXDocument();

                UnitTestsUtil.VerifyXPathExists(document, 
                    "//csdl:Action[@Name='ActionWithOverload']/csdl:Parameter[@Type='TestNamespace.Customer']",
                    "//csdl:Action[@Name='ActionWithOverload']/csdl:Parameter[@Type='TestNamespace.ChildCustomer']",
                    "//csdl:Action[@Name='ActionWithOverload']/csdl:Parameter[@Type='Collection(TestNamespace.Customer)']",
                    "//csdl:Action[@Name='ActionWithOverload']/csdl:Parameter[@Type='Collection(TestNamespace.ChildCustomer)']",
                    "//csdl:Action[@Name='ActionWithOverload']/csdl:Parameter[@Type='TestNamespace.Movie']",
                    "//csdl:Action[@Name='ActionWithOverload' and not(csdl:Parameter)]");
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void MultipleOverloadsShouldSerializeInEntry()
        {
            var service = this.CreateServiceWithActionOverloads();

            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Customers(3)";
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                var document = request.GetResponseStreamAsXDocument();

                // <m:action metadata="http://host/$metadata#TestNamespace.ActionWithOverload" title="ActionWithOverload" target="http://host/Customers(3)/TestNamespace.Customer/TestNamespace.ActionWithOverload" />
                // <m:action metadata="http://host/$metadata#TestNamespace.ActionWithOverload" title="ActionWithOverload" target="http://host/Customers(3)/TestNamespace.ChildCustomer/TestNamespace.ActionWithOverload" />
                UnitTestsUtil.VerifyXPathExists(document, "//adsm:action[@metadata='http://host/$metadata#TestNamespace.ActionWithOverload' and @title='ActionWithOverload' and @target='http://host/Customers(3)/TestNamespace.Customer/TestNamespace.ActionWithOverload']", "//adsm:action[@metadata='http://host/$metadata#TestNamespace.ActionWithOverload' and @title='ActionWithOverload' and @target='http://host/Customers(3)/TestNamespace.ChildCustomer/TestNamespace.ActionWithOverload']");
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void SingleOverloadsShouldSerializeInEntryWithNormalLinks()
        {
            var service = this.CreateServiceWithActionOverloads();
            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Customers(1)";
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                var document = request.GetResponseStreamAsXDocument();

                UnitTestsUtil.VerifyXPathExists(document, "//adsm:action[@metadata='http://host/$metadata#TestNamespace.ActionWithOverload' and @title='ActionWithOverload' and @target='http://host/Customers(1)/TestNamespace.ActionWithOverload']");
            }
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void CorrectOverloadShouldBeInvoked()
        {
            var service = this.CreateServiceWithActionOverloads();
            RunActionInvokeTest(service, "/Customers(3)/TestNamespace.ActionWithOverload()", "BoundToBase");
            RunActionInvokeTest(service, "/Movies(0)/TestNamespace.ActionWithOverload()", "BoundToMovie");
            RunActionInvokeTest(service, "/Customers(3)/TestNamespace.ChildCustomer/TestNamespace.ActionWithOverload()", "BoundToChild");
            RunActionInvokeTest(service, "/TestNamespace.ActionWithOverload()", "UnBound");
            RunActionInvokeTest(service, "/Customers/TestNamespace.ActionWithOverload()", "BoundToBaseCollection");
            RunActionInvokeTest(service, "/Customers/TestNamespace.Customer/TestNamespace.ActionWithOverload()", "BoundToBaseCollection");
            RunActionInvokeTest(service, "/Customers/TestNamespace.ChildCustomer/TestNamespace.ActionWithOverload()", "BoundToChildCollection");
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void CorrectOverloadShouldBeSelected()
        {
            var service = this.CreateServiceWithActionOverloads();
            RunActionSelectTest(service, "/Customers(3)?$select=TestNamespace.ActionWithOverload", "#TestNamespace.ActionWithOverload");
            RunActionSelectTest(service, "/Customers(3)?$select=TestNamespace.Customer/TestNamespace.ActionWithOverload", "#TestNamespace.ActionWithOverload");
            RunActionSelectTest(service, "/Customers(3)?$select=TestNamespace.ChildCustomer/TestNamespace.ActionWithOverload", "#TestNamespace.ActionWithOverload");
            RunActionSelectTest(service, "/Customers/TestNamespace.ChildCustomer(3)?$select=TestNamespace.ActionWithOverload", "#TestNamespace.ActionWithOverload");

            RunActionSelectTest(service, "/Customers(1)?$select=TestNamespace.ActionWithOverload", "#TestNamespace.ActionWithOverload");
            RunActionSelectTest(service, "/Customers(1)?$select=TestNamespace.Customer/TestNamespace.ActionWithOverload", "#TestNamespace.ActionWithOverload");
            RunActionSelectTest(service, "/Customers(1)?$select=TestNamespace.ChildCustomer/TestNamespace.ActionWithOverload");
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod]
        public void OnlyOverloadsForSpecificTypeShouldBeSelected()
        {
            var service = this.CreateServiceWithActionOverloads();
            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = "/Customers(3)?$select=TestNamespace.*";
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                var document = request.GetResponseStreamAsXDocument();

                UnitTestsUtil.VerifyXPathExists(document, "//adsm:action[@metadata='http://host/$metadata#TestNamespace.ActionWithOverload']");
            }
        }

        private static void RunActionInvokeTest(DSPServiceDefinition service, string requestUriString, string expectedHeaderAndPayloadValue)
        {
            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = requestUriString;
                request.HttpMethod = "POST";
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();

                Assert.AreEqual(expectedHeaderAndPayloadValue, request.ResponseHeaders["WhichAction"]);
                var document = request.GetResponseStreamAsXDocument();
                UnitTestsUtil.VerifyXPathExists(document, string.Format("//adsm:value[text()='{0}']", expectedHeaderAndPayloadValue));
            }
        }

        private static void RunActionSelectTest(DSPServiceDefinition service, string requestUriString, params string[] metadataLinks)
        {
            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.RequestUriString = requestUriString;
                request.Accept = "application/atom+xml,application/xml";
                request.SendRequest();
                var document = request.GetResponseStreamAsXDocument();

                foreach (var metadataLink in metadataLinks)
                {
                    UnitTestsUtil.VerifyXPathExists(document, string.Format("//adsm:action[@metadata='http://host/$metadata{0}']", metadataLink));
                }

                UnitTestsUtil.VerifyXPathResultCount(document, metadataLinks.Length, "//adsm:action");
            }
        }

        private DSPServiceDefinition CreateServiceWithActionOverloads()
        {
            DSPMetadata metadata = this.CreateMetadata();
            var customerType = metadata.GetResourceType("Customer");
            var movieType = metadata.GetResourceType("Movie");
            var childType = metadata.GetResourceType("ChildCustomer");

            var stringType = ResourceType.GetPrimitiveResourceType(typeof(string));
            ServiceAction boundToBase = new ServiceAction("ActionWithOverload", stringType, null, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("param1", customerType) });
            ServiceAction boundToMovie = new ServiceAction("ActionWithOverload", stringType, null, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("param1", movieType) });
            ServiceAction boundToChild = new ServiceAction("ActionWithOverload", stringType, null, OperationParameterBindingKind.Always, new[] { new ServiceActionParameter("param1", childType) });
            ServiceAction unBound = new ServiceAction("ActionWithOverload", stringType, null, OperationParameterBindingKind.Never, new ServiceActionParameter[0]);
            ServiceAction boundToBaseCollection = new ServiceAction("ActionWithOverload", stringType, null, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("param1", ResourceType.GetEntityCollectionResourceType(customerType)) });
            ServiceAction boundToChildCollection = new ServiceAction("ActionWithOverload", stringType, null, OperationParameterBindingKind.Sometimes, new[] { new ServiceActionParameter("param1", ResourceType.GetEntityCollectionResourceType(childType)) });

            boundToBase.SetReadOnly();
            boundToMovie.SetReadOnly();
            boundToChild.SetReadOnly();
            unBound.SetReadOnly();
            boundToBaseCollection.SetReadOnly();
            boundToChildCollection.SetReadOnly();

            MyDSPActionProvider actionProvider = new TestActionProviderWithResolution
            {
                SubstituteGetServiceActions = operationContext => new[] { boundToBase, boundToMovie, boundToChild, unBound, boundToBaseCollection, boundToChildCollection },
                SubstituteGetServiceActionsByResourceType = (operationContext, resourceType) =>
                                                            {
                                                                if (resourceType == customerType)
                                                                {
                                                                    return new[] { boundToBase };
                                                                }

                                                                if (resourceType == movieType)
                                                                {
                                                                    return new[] { boundToMovie };
                                                                }

                                                                if (resourceType == childType)
                                                                {
                                                                    return new[] { boundToChild };
                                                                }

                                                                if (resourceType.FullName == "Collection(" + customerType.FullName + ")")
                                                                {
                                                                    return new[] { boundToBaseCollection };
                                                                }

                                                                if (resourceType.FullName == "Collection(" + childType.FullName + ")")
                                                                {
                                                                    return new[] { boundToChildCollection };
                                                                }

                                                                return new ServiceAction[0];
                                                            },
                SubstituteTryResolveServiceAction = (operationContext, resolutionContext) =>
                {
                                                        Assert.IsTrue(resolutionContext.ServiceActionName.EndsWith("ActionWithOverload"), "Wrong service name");
                                                        if (resolutionContext.BindingType == customerType)
                                                        {
                                                            return boundToBase;
                                                        }

                                                        if (resolutionContext.BindingType == movieType)
                                                        {
                                                            return boundToMovie;
                                                        }

                                                        if (resolutionContext.BindingType == childType)
                                                        {
                                                            return boundToChild;
                                                        }

                                                        if (resolutionContext.BindingType == null)
                                                        {
                                                            return unBound;
                                                        }

                                                        if (resolutionContext.BindingType.FullName == "Collection(" + customerType.FullName + ")")
                                                        {
                                                            return boundToBaseCollection;
                                                        }

                                                        if (resolutionContext.BindingType.FullName == "Collection(" + childType.FullName + ")")
                                                        {
                                                            return boundToChildCollection;
                                                        }

                                                        return null;
                                                    },
                SubstituteCreateInvokable = (operationContext, action, args) =>
                                            {
                                                if(ReferenceEquals(action, boundToBase))
                                                {
                                                    return CreateInvokable("BoundToBase", operationContext);
                                                }

                                                if (ReferenceEquals(action, boundToMovie))
                                                {
                                                    return CreateInvokable("BoundToMovie", operationContext);
                                                }

                                                if (ReferenceEquals(action, boundToChild))
                                                {
                                                    return CreateInvokable("BoundToChild", operationContext);
                                                }

                                                if (ReferenceEquals(action, unBound))
                                                {
                                                    return CreateInvokable("UnBound", operationContext);
                                                }

                                                if (ReferenceEquals(action, boundToBaseCollection))
                                                {
                                                    return CreateInvokable("BoundToBaseCollection", operationContext);
                                                }

                                                if (ReferenceEquals(action, boundToChildCollection))
                                                {
                                                    return CreateInvokable("BoundToChildCollection", operationContext);
                                                }

                                                throw new NotImplementedException();
                                            },
            };

            DSPServiceDefinition service = new DSPServiceDefinition
            {
                Metadata = metadata,
                CreateDataSource = this.CreateDataSource,
                ActionProvider = actionProvider,
                DataServiceBehavior = { MaxProtocolVersion = ODataProtocolVersion.V4 },
                Writable = true,
                HostInterfaceType = typeof(IDataServiceHost2),
            };

            return service;
        }

        private static IDataServiceInvokable CreateInvokable(string actionNameHint, DataServiceOperationContext operationContext)
        {
            return new DelegatingInvokable(() => operationContext.ResponseHeaders["WhichAction"] = actionNameHint, () => actionNameHint);
        }

        /// <summary>
        /// Constructs an xpath string and adds to the provided list of xpath strings.
        /// </summary>
        /// <param name="xPaths">The list of xpath strings</param>
        /// <param name="baseUri">Base uri of the request</param>
        /// <param name="title">Title of the ODataOperation.</param>
        /// <param name="target">Target of the ODataOperation.</param>
        private void AddXPath(List<string> xPaths, string baseUri, string title, string target)
        {
            string metadata = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}$metadata#{1}.{2}", baseUri, ServiceNamespace, title);
            string xPath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "//adsm:action[@metadata='{0}' and @title='{1}' and @target='{2}{3}/{4}.{1}' and not(@rel)]", metadata, title, baseUri, target, ServiceNamespace);
            xPaths.Add(xPath);
        }
   }
}


