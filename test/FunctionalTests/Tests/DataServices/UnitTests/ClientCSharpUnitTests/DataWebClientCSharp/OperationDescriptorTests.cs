//---------------------------------------------------------------------
// <copyright file="OperationDescriptorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using System.Text;
    using AstoriaUnitTests.Stubs;
    using AstoriaUnitTests.Stubs.DataServiceProvider;
    using AstoriaUnitTests.Tests;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AstoriaTest = System.Data.Test.Astoria;

    #endregion

    /// <summary>
    /// Test action and function descriptors.
    /// </summary>
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/881
    [Ignore] // Remove Atom
    // [TestClass]
    public class OperationDescriptorTests
    {
        private const string ServiceName = "TestService";
        private const string ServiceNamespace = "TestNamespace";

        private const string AtomId = "http://host/TheTest/Customers(1)";

        private static DSPMetadata Metadata;
        private static ResourceType CustomerResourceType;

        #region Class CustomerEntity
        private class CustomerEntity
        {
            public int ID
            {
                get;
                set;
            }
            public string Name
            {
                get;
                set;
            }
        }
        #endregion

        #region Class TestCase
        private class TestCase
        {
            /// <summary>
            /// Cache of the test case's expected descriptors for each format. Caching this information for the test case is required so that individual cases can initialize
            /// the descriptors before the test is run, the test runner can then make possible modifications which may be different for each format, then can later access the modified
            /// data in order to do the necessary verification.
            /// </summary>
            private readonly Dictionary<ODataFormat, List<List<MyOperationDescriptor>>> expectedDescriptors = new Dictionary<ODataFormat, List<List<MyOperationDescriptor>>>();

            public IEnumerable<Operation> Operations
            {
                get;
                set;
            }

            /// <summary>
            /// Delegate used to initialize the expected descriptors for a format. After the descriptors are initialized by the test case, they may 
            /// be modified when the test runs in order to handle cases where we need to create absolute URIs by adding the test service's base URI to
            /// a relative URI specified by the test case. Because of that modification step, this delegate is used outside of this class for 
            /// initialization only, and can not be used to later access the actual set of descriptors. Outside of this class the GetExpectedDescriptors
            /// method should be used to get the final set of expected descriptors, which will include any modifications that were made after initialization.
            /// </summary>
            public Func<ODataFormat, List<List<MyOperationDescriptor>>> InitializeExpectedDescriptors
            {
                private get;
                set;
            }

            /// <summary>
            /// Gets the test case's expected descriptors for the specified format. The set of descriptors may include
            /// modifications that were made after the descriptors were initially created for the test case.
            /// </summary>
            public List<List<MyOperationDescriptor>> GetExpectedDescriptors(ODataFormat format)
            {
                List<List<MyOperationDescriptor>> expectedDescriptorsForFormat = null;
                if (!this.expectedDescriptors.TryGetValue(format, out expectedDescriptorsForFormat))
                {
                    expectedDescriptorsForFormat = this.InitializeExpectedDescriptors(format);
                    this.expectedDescriptors.Add(format, expectedDescriptorsForFormat);
                }

                return expectedDescriptorsForFormat;
            }

            public string RequestUriString
            {
                get;
                set;
            }

            public bool? AddBaseUriToTarget
            {
                get;
                set;
            }

            public bool? AddBaseUriToMetadata
            {
                get;
                set;
            }

            public void AddBaseUriStringToExpectedDescriptors(string baseUri, ODataFormat format)
            {
                if (this.GetExpectedDescriptors(format) != null)
                {
                    foreach (var expectedDescriptorsForFormat in this.GetExpectedDescriptors(format))
                    {
                        foreach (MyOperationDescriptor od in expectedDescriptorsForFormat)
                        {
                            if (format == ODataFormat.Json && od.Metadata[0] == '#')
                            {
                                od.Metadata = baseUri + "/$metadata" + od.Metadata;
                            }
                            else if (this.AddBaseUriToMetadata == null || this.AddBaseUriToMetadata.Value)
                            {
                                od.Metadata = baseUri + "/" + od.Metadata;
                            }

                            if (this.AddBaseUriToTarget == null || this.AddBaseUriToTarget.Value)
                            {
                                od.Target = baseUri + "/" + od.Target;
                            }
                        }
                    }
                }
            }

            public MyDSPActionProvider.AdvertiseServiceActionDelegate SubstituteAdvertiseServiceAction
            {
                get;
                set;
            }

            public string ExpectedErrorMessage
            {
                get;
                set;
            }

            public PayloadBuilder ResponsePayloadBuilder
            {
                get;
                set;
            }
        }
        #endregion

        #region Class MyOperationDescriptor
        private class MyOperationDescriptor
        {
            public string Title
            {
                set;
                get;
            }

            public string Target
            {
                set;
                get;
            }

            public string Metadata
            {
                set;
                get;
            }

            public MyOperationDescriptor()
            {
            }

            public MyOperationDescriptor(OperationDescriptor od)
            {
                this.Title = od.Title;
                this.Metadata = od.Metadata.IsAbsoluteUri ? od.Metadata.AbsoluteUri : od.Metadata.OriginalString;
                this.Target = od.Target.AbsoluteUri;
            }
        }
        #endregion

        #region MyDSPActionProvider
        /// <summary>
        /// Repesents a metadata provider.
        /// </summary>
        private class MyDSPActionProvider : DSPActionProvider
        {
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
        }
        #endregion

        #region private methods
        /// <summary>
        /// Creates a datasource from the given metadata.
        /// </summary>
        /// <param name="metadata">The metadata against which the datasource is created.</param>
        /// <returns>DSPContext representing the data source.</returns>
        private static DSPContext CreateDataSource(DSPMetadata metadata)
        {
            DSPContext context = new DSPContext();
            ResourceType customerType = metadata.GetResourceType("CustomerEntity");
            DSPResource entity1 = new DSPResource(customerType);
            entity1.SetValue("ID", 1);
            entity1.SetValue("Name", "Vineet");

            DSPResource entity2 = new DSPResource(customerType);
            entity2.SetValue("ID", 2);
            entity2.SetValue("Name", "Jimmy");

            context.GetResourceSetEntities("CustomerEntities").Add(entity1);
            context.GetResourceSetEntities("CustomerEntities").Add(entity2);
            return context;
        }

        private ServiceAction GetServiceAction(string title, ResourceType bindingType)
        {
            return new ServiceAction(title, /*returnType*/null, /*resultSet*/null, OperationParameterBindingKind.Sometimes, new ServiceActionParameter[] { new ServiceActionParameter("p1", bindingType) });
        }

        private MyOperationDescriptor GetMyOperationDescriptor(ODataFormat format, string title, string uri)
        {
            if (format == ODataFormat.Json)
            {
                // JSON Light builds the title and target with entity container name in front of the simple title name. Atom doesn't include the container name.
                title = String.Format("TestNamespace.{0}", title);
                return new MyOperationDescriptor() { Title = title, Metadata = String.Format("$metadata#{0}", title), Target = uri + "/" + title };
            }

            return new MyOperationDescriptor() { Title = title, Metadata = String.Format("$metadata#TestNamespace.{0}", title), Target = uri + "/TestNamespace." + title };
        }

        private static void TestEquality(IEnumerable<OperationDescriptor> actualDescriptors, IEnumerable<MyOperationDescriptor> expectedDescriptors)
        {
            IEnumerable<OperationDescriptor> orderedActualDescriptors = actualDescriptors.OrderBy(od => od.Title);
            IEnumerable<MyOperationDescriptor> orderedExpectedDescriptors = expectedDescriptors.OrderBy(od => od.Title);

            IEnumerator<OperationDescriptor> actualEnumerator = orderedActualDescriptors.GetEnumerator();
            IEnumerator<MyOperationDescriptor> expectedEnumerator = orderedExpectedDescriptors.GetEnumerator();

            while (expectedEnumerator.MoveNext())
            {
                Assert.IsTrue(actualEnumerator.MoveNext());
                MyOperationDescriptor actualDescriptor = new MyOperationDescriptor((OperationDescriptor)actualEnumerator.Current);
                MyOperationDescriptor expectedDescriptor = expectedEnumerator.Current;

                Assert.AreEqual(expectedDescriptor.Metadata, actualDescriptor.Metadata);
                Assert.AreEqual(expectedDescriptor.Target, actualDescriptor.Target);
                Assert.AreEqual(expectedDescriptor.Title, actualDescriptor.Title);
            }
        }

        /// <summary>
        /// Make final changes to the testcase before sending the request.
        /// </summary>
        private static void MakeFinalChangesToTestCase(TestCase testCase, ODataFormat format, MyDSPActionProvider actionProvider, TestWebRequest request)
        {
            foreach (var action in testCase.Operations)
            {
                actionProvider.AddAction((ServiceAction)action);
            }

            testCase.AddBaseUriStringToExpectedDescriptors(request.ServiceRoot.OriginalString, format);

            if (testCase.SubstituteAdvertiseServiceAction != null)
            {
                actionProvider.SubstituteAdvertiseServiceAction = testCase.SubstituteAdvertiseServiceAction;
            }
        }

        #endregion

        #region Test Class Initialize

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Metadata = new DSPMetadata(ServiceName, ServiceNamespace);
            CustomerResourceType = Metadata.AddEntityType("CustomerEntity", null, null, false);
            Metadata.AddKeyProperty(CustomerResourceType, "ID", typeof(int));
            Metadata.AddPrimitiveProperty(CustomerResourceType, "Name", typeof(string), false);
            Metadata.AddResourceSet("CustomerEntities", CustomerResourceType);
            Metadata.SetReadOnly();
        }

        #endregion

        #region Test Methods

        #region ActionDescriptorPositiveTests

        [TestMethod]
        public void SingleEntityWithSingleAction()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                {
                    this.GetMyOperationDescriptor(format, "Action1", "CustomerEntities(1)")
                }},
                RequestUriString = "CustomerEntities(1)",
            };

            RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void SingleEntityWithMultipleActions()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType), this.GetServiceAction("Action2", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                {
                    this.GetMyOperationDescriptor(format, "Action1", "CustomerEntities(1)"), this.GetMyOperationDescriptor(format, "Action2", "CustomerEntities(1)")
                }},
                RequestUriString = "CustomerEntities(1)",
            };

            RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void SingleEntityWithNoActions()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { },
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor> { } },
                RequestUriString = "CustomerEntities(1)",
            };

            this.RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void CollectionOfEntitiesWithOneActionEach()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>>
                {
                    new List<MyOperationDescriptor>()
                    {
                        this.GetMyOperationDescriptor(format, "Action1", "CustomerEntities(1)"),
                    },
                    new List<MyOperationDescriptor>()
                    {
                        this.GetMyOperationDescriptor(format, "Action1", "CustomerEntities(2)"),
                    }
                },
                RequestUriString = "CustomerEntities",
            };

            this.RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void RelativeUriInActionTarget()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) =>
                {
                    var title = string.Format("{0}Action1", format == ODataFormat.Json ? "TestNamespace." : string.Empty);
                    return new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                    {
                        new MyOperationDescriptor() { Title = title , Metadata = "$metadata#TestNamespace.Action1", Target = "CustomerEntities(1)/Action1" }
                    }};
                },
                RequestUriString = "CustomerEntities(1)",
                SubstituteAdvertiseServiceAction = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                {
                    // Note that the relative Uri should not starts with a '/'. If it does, the output absolute uri will be incorrect. This behavior is defined by 
                    // the call to new Uri(baseUri, relativeUri)).
                    od.Target = new Uri("CustomerEntities(1)/Action1", UriKind.Relative);
                    return true;
                }),
            };

            RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void MetadataStartsWithHash()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) =>
                {
                    var title = string.Format("{0}Action1", format == ODataFormat.Json ? "TestNamespace." : string.Empty);
                    return new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                    {
                        new MyOperationDescriptor() { Title = title, Metadata = "#TestNamespace.Action1", Target = "CustomerEntities(1)/TestNamespace.Action1" }
                    }};
                },
                RequestUriString = "CustomerEntities(1)",
                AddBaseUriToMetadata = false,
                SubstituteAdvertiseServiceAction = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                {
                    od.Metadata = new Uri("#TestNamespace.Action1", UriKind.RelativeOrAbsolute);
                    return true;
                })
            };

            this.RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void TargetAndRelationValuesSetInAdvertiseServiceAction()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>()
                {
                    new MyOperationDescriptor() { Title="SomeTitle", Target="http://sometarget/", Metadata="#TestNamespace.Action1" }
                }},
                RequestUriString = "/CustomerEntities(1)",
                AddBaseUriToMetadata = false,
                AddBaseUriToTarget = false,
                SubstituteAdvertiseServiceAction = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                {
                    od.Metadata = new Uri("#TestNamespace.Action1", UriKind.RelativeOrAbsolute);
                    od.Target = new Uri("http://sometarget", UriKind.Absolute);
                    od.Title = "SomeTitle";
                    return true;
                }),
            };

            this.RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void ActionWithNullTitle()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) =>
                {
                    var target = string.Format("CustomerEntities(1)/{0}Action1", "TestNamespace.");

                    // With JSON Light, if the title is not on the wire (which it's not, if it's null), then the metadata builder kicks in and a non-null value is reported
                    // There is no real scenario where the title would need to actually be null, so we have decided to not do anything to make this match the Atom behavior.
                    var title = format == ODataFormat.Json ? "TestNamespace.Action1" : null;

                    return new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>()
                    {
                        new MyOperationDescriptor() { Title = title, Target= target, Metadata = String.Format("$metadata#TestNamespace.{0}", "Action1")}
                    }};
                },
                RequestUriString = "CustomerEntities(1)",
                SubstituteAdvertiseServiceAction = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                {
                    od.Title = null;
                    return true;
                }),
            };

            this.RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void ActionWithEmptyTitle()
        {
            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction("Action1", CustomerResourceType) },
                InitializeExpectedDescriptors = (format) =>
                {
                    var target = string.Format("CustomerEntities(1)/{0}Action1", "TestNamespace.");
                    return new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>()
                    {
                        new MyOperationDescriptor() {Title = "", Target = target, Metadata = String.Format("$metadata#TestNamespace.{0}", "Action1") }
                    }};
                },
                RequestUriString = "CustomerEntities(1)",
                SubstituteAdvertiseServiceAction = new MyDSPActionProvider.AdvertiseServiceActionDelegate((DataServiceOperationContext oc, ServiceAction so, object o, bool inFeed, ref ODataAction od) =>
                {
                    od.Title = "";
                    return true;
                }),
            };

            this.RunPositiveTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void ActionWithReservedURICharactersInTitle()
        {
            // '#' = %23; '$' = %24. 
            var unescaped = "Action1#Re$erved";
            var escapedFragment = "Action1%23Re%24erved";
            var escapedRelativeUri = "Action1%23Re%24erved";

            var testCase = new TestCase()
            {
                Operations = new ServiceAction[] { this.GetServiceAction(unescaped, CustomerResourceType) },
                InitializeExpectedDescriptors = (format) =>
                {
                    var target = string.Format("CustomerEntities(1)/TestNamespace.{0}", format == ODataFormat.Json ? unescaped : escapedRelativeUri);
                    var title = string.Format("{0}{1}", format == ODataFormat.Json ? "TestNamespace." : String.Empty, unescaped);
                    return new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>()
                    {
                        new MyOperationDescriptor() { Title = title, Target = target, Metadata = String.Format("$metadata#TestNamespace.{0}", format == ODataFormat.Json ? escapedFragment : escapedRelativeUri) }
                    }};
                },
                RequestUriString = "CustomerEntities(1)",
            };

            RunPositiveTestWithAllFormats(testCase);
        }

        private void RunPositiveTestWithAllFormats(TestCase testCase)
        {
            RunPositiveTest(ODataFormat.Json, testCase);
        }

        private static void RunPositiveTest(ODataFormat format, TestCase testCase)
        {
            MyDSPActionProvider actionProvider = new MyDSPActionProvider();
            DSPServiceDefinition service = new DSPServiceDefinition() { Metadata = Metadata, CreateDataSource = CreateDataSource, ActionProvider = actionProvider };
            service.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;

            using (TestWebRequest request = service.CreateForInProcessWcf())
            {
                request.StartService();
                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //ctx.EnableAtom = true;

                Uri uri = new Uri(request.ServiceRoot + "/" + testCase.RequestUriString);

                MakeFinalChangesToTestCase(testCase, format, actionProvider, request);

                if (format == ODataFormat.Json)
                {
                    JsonLightTestUtil.ConfigureContextForJsonLight(ctx, null);
                }
                else
                {
                    //ctx.Format.UseAtom();
                }

                QueryOperationResponse<CustomerEntity> qor = (QueryOperationResponse<CustomerEntity>)ctx.Execute<CustomerEntity>(uri);
                Assert.IsNotNull(qor);
                Assert.IsNull(qor.Error);

                IEnumerator<CustomerEntity> entities = qor.GetEnumerator();

                int expectedDescriptorsPerEntity = 0;

                while (entities.MoveNext())
                {
                    CustomerEntity c = entities.Current;
                    EntityDescriptor ed = ctx.GetEntityDescriptor(c);
                    IEnumerable<OperationDescriptor> actualDescriptors = ed.OperationDescriptors;
                    TestEquality(actualDescriptors, testCase.GetExpectedDescriptors(format)[expectedDescriptorsPerEntity++]);
                }
            }
        }

        #endregion

        #region FunctionDescriptorTests

        private void RunPositiveFunctionTestWithAllFormats(TestCase testCase)
        {
            this.RunPositiveFunctionTest(ODataFormat.Json, testCase);
        }

        [TestMethod]
        public void SingleEntityWithSingleFunction()
        {
            var testCase = new TestCase()
            {
                ResponsePayloadBuilder = GetPayloadBuilder()
                                            .AddFunction("#TestNamespace.Function1", "TestNamespace.Function1", "http://sometarget"),
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                    {
                        new MyOperationDescriptor() { Title="TestNamespace.Function1", Target="http://sometarget/", Metadata="#TestNamespace.Function1" }
                    }},
                RequestUriString = "CustomerEntities(1)",
            };

            this.RunPositiveFunctionTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void SingleEntityWithMultipleFunctions()
        {
            var testCase = new TestCase()
            {
                ResponsePayloadBuilder = GetPayloadBuilder()
                                            .AddFunction("#TestNamespace.Function1", "Function1", "http://sometarget")
                                            .AddFunction("#TestNamespace.Function2", "TestNamespace.Function2", "http://sometarget2"),
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                {
                    new MyOperationDescriptor() { Title="Function1", Target="http://sometarget/", Metadata="#TestNamespace.Function1" },
                    new MyOperationDescriptor() { Title="TestNamespace.Function2", Target="http://sometarget2/", Metadata="#TestNamespace.Function2" },
                }},
                RequestUriString = "CustomerEntities(1)",
            };

            this.RunPositiveFunctionTestWithAllFormats(testCase);
        }

        [TestMethod]
        public void SingleEntityWithBothActionsAndFunctions()
        {
            var testCase = new TestCase()
            {
                ResponsePayloadBuilder = GetPayloadBuilder()
                                            .AddAction("#TestNamespace.Action1", "TestNamespace.Action1", "http://sometarget1")
                                            .AddAction("#TestNamespace.Action2", "Action2", "http://sometarget2")
                                            .AddFunction("#TestNamespace.Function1", "Function1", "http://sometarget3"),
                InitializeExpectedDescriptors = (format) => new List<List<MyOperationDescriptor>> { new List<MyOperationDescriptor>
                    {
                        new MyOperationDescriptor() { Title="TestNamespace.Action1", Target="http://sometarget1/", Metadata="#TestNamespace.Action1" },
                        new MyOperationDescriptor() { Title="Action2", Target="http://sometarget2/", Metadata="#TestNamespace.Action2" },
                        new MyOperationDescriptor() { Title="Function1", Target="http://sometarget3/", Metadata="#TestNamespace.Function1" }
                    }},
                RequestUriString = "CustomerEntities(1)",
            };

            this.RunPositiveFunctionTestWithAllFormats(testCase);
        }

        private void RunPositiveFunctionTest(ODataFormat format, TestCase testCase)
        {
            // All of the functions tests use the PlaybackService since the WCF Data Services server doesn't support functions
            // The PlaybackService itself will not automatically turn Metadata into an absolute URI, so set that to false on all tests.
            // The tests also use absolute URIs for Target, so suppress that as well.
            testCase.AddBaseUriToMetadata = false;
            testCase.AddBaseUriToTarget = false;

            using (TestWebRequest request = TestWebRequest.CreateForInProcessWcf())
            using (PlaybackService.ProcessRequestOverride.Restore())
            {
                request.ServiceType = typeof(AstoriaUnitTests.Stubs.PlaybackService);
                request.StartService();

                var payloadBuilder = testCase.ResponsePayloadBuilder;
                PlaybackService.ProcessRequestOverride.Value = (req) =>
                {
                    string contentType;
                    if (format == ODataFormat.Json)
                    {
                        contentType = UnitTestsUtil.JsonLightMimeType;
                        payloadBuilder.Metadata = request.BaseUri + "/$metadata#TestService.CustomerEntities/$entity";
                    }
                    else
                    {
                        contentType = UnitTestsUtil.AtomFormat;
                    }

                    req.SetResponseStreamAsText(PayloadGenerator.Generate(payloadBuilder, format));
                    req.ResponseHeaders.Add("Content-Type", contentType);
                    req.SetResponseStatusCode(200);
                    return req;
                };

                testCase.AddBaseUriStringToExpectedDescriptors(request.ServiceRoot.OriginalString, format);

                Uri uri = new Uri(request.ServiceRoot + "/" + testCase.RequestUriString);
                DataServiceContext ctx = new DataServiceContext(request.ServiceRoot, ODataProtocolVersion.V4);
                //ctx.EnableAtom = true;

                if (format == ODataFormat.Json)
                {
                    string serviceEdmx = GetServiceEdmxWithOperations(payloadBuilder);
                    JsonLightTestUtil.ConfigureContextForJsonLight(ctx, serviceEdmx);
                }

                QueryOperationResponse<CustomerEntity> qor = (QueryOperationResponse<CustomerEntity>)ctx.Execute<CustomerEntity>(uri);
                Assert.IsNotNull(qor);
                Assert.IsNull(qor.Error);

                IEnumerator<CustomerEntity> entities = qor.GetEnumerator();

                int expectedDescriptorsPerEntity = 0;

                while (entities.MoveNext())
                {
                    CustomerEntity c = entities.Current;
                    EntityDescriptor ed = ctx.GetEntityDescriptor(c);
                    IEnumerable<OperationDescriptor> actualDescriptors = ed.OperationDescriptors;
                    TestEquality(actualDescriptors, testCase.GetExpectedDescriptors(format)[expectedDescriptorsPerEntity++]);
                }
            }
        }

        #endregion

        private static PayloadBuilder GetPayloadBuilder()
        {
            return new PayloadBuilder() { Id = AtomId }.AddProperty("ID", 1);
        }

        private static string GetServiceEdmxWithOperations(PayloadBuilder builder)
        {
            string serviceEdmx =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
    <Schema Namespace=""TestNamespace"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
      <EntityType Name=""CustomerEntity"">
        <Key>
          <PropertyRef Name=""ID"" />
        </Key>
        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />
        <Property Name=""Name"" Type=""Edm.String"" />
      </EntityType>
      {0}
      <EntityContainer Name=""TestService"">
        <EntitySet Name=""CustomerEntities"" EntityType=""TestNamespace.CustomerEntity"" />
      </EntityContainer>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

            StringBuilder operationBuilder = new StringBuilder();

            const string actionEdmx =
@"<Action Name=""{0}"" IsBound=""true"">
  <Parameter Name=""p1"" Type=""TestNamespace.CustomerEntity"" />
</Action>";

            const string composableFunctionEdmx =
@"<Function Name=""{0}"" IsBound=""true"" IsComposable=""{1}"">
  <Parameter Name=""p1"" Type=""TestNamespace.CustomerEntity"" />
</Function>";

            foreach (var operation in builder.Operations)
            {

                bool isComposable = operation.PropertyKind == PayloadBuilderPropertyKind.Function;
                var title = operation.Title;
                var dotIndex = title.IndexOf('.');
                if (dotIndex >= 0)
                {
                    title = title.Substring(dotIndex + 1);
                }

                if (operation.PropertyKind == PayloadBuilderPropertyKind.Action)
                {
                    operationBuilder.Append(String.Format(actionEdmx, title));
                }
                else
                {
                    operationBuilder.Append(String.Format(composableFunctionEdmx, title, isComposable));
                }
            }

            return string.Format(serviceEdmx, operationBuilder.ToString());

        }

        #endregion
    }
}
