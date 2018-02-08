//---------------------------------------------------------------------
// <copyright file="NavigationLinkReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of navigation links (deferred and expanded).
    /// </summary>
    [TestClass, TestCase]
    public class NavigationLinksReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Links"), Variation(Description = "Test the the reading of entity reference links on entry payloads.")]
        public void EntityReferenceLinkTest()
        {
            IEdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            var cityType = model.FindType("TestModel.CityType");
            Debug.Assert(cityType != null, "cityType != null");

            // TODO: add test cases that use relative URIs

            // Few hand-crafted payloads
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Single entity reference link for a singleton
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").WithTypeAnnotation(cityType)
                            .Property(PayloadBuilder.NavigationProperty("PoliceStation", "http://odata.org/PoliceStation").IsCollection(false)),
                    PayloadEdmModel = model
                },
                // Single entity reference link for a collection
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").WithTypeAnnotation(cityType)
                            .Property(PayloadBuilder.NavigationProperty("CityHall", "http://odata.org/CityHall").IsCollection(true)),
                    PayloadEdmModel = model
                },

                // Multiple entity reference links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").WithTypeAnnotation(cityType)
                            .Property(PayloadBuilder.NavigationProperty("CityHall", "http://odata.org/CityHall").IsCollection(true))
                            .Property(PayloadBuilder.NavigationProperty("PoliceStation", "http://odata.org/PoliceStation").IsCollection(false)),
                    PayloadEdmModel = model
                },

                // Multiple entity reference links with primitive properties in between
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").WithTypeAnnotation(cityType)
                            .Property("Id", PayloadBuilder.PrimitiveValue(1))
                            .Property(PayloadBuilder.NavigationProperty("CityHall", "http://odata.org/CityHall").IsCollection(true))
                            .Property("Name", PayloadBuilder.PrimitiveValue("Vienna"))
                            .Property(PayloadBuilder.NavigationProperty("DOL", "http://odata.org/DOL").IsCollection(true)),
                    PayloadEdmModel = model
                },
            };

            // Generate interesting payloads around the entry
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                // Entity reference links are request only.
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Links"), Variation(Description = "Test the the reading of expanded links on entry payloads.")]
        public void ExpandedLinkTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            // TODO: add test cases that use relative URIs

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateDeferredNavigationLinkTestDescriptors(this.Settings, true);

            // Generate interesting payloads around the navigation property
            // Note that this will actually expand the deferred nav links as well.
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            IEnumerable<PayloadReaderTestDescriptor> customTestDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Expanded null entry
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("Id", 1)
                        .ExpandedNavigationProperty("PoliceStation", PayloadBuilder.NullEntity()),
                },
                // Expanded null entry after another expanded collection link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("Id", 1)
                        .ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet())
                        .ExpandedNavigationProperty("PoliceStation", PayloadBuilder.NullEntity()),
                },
                // incorrect type at related end
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType")
                        .PrimitiveProperty("Id", 1)
                        .ExpandedNavigationProperty("PoliceStation", PayloadBuilder.Entity("TestModel.CityType")),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_ResourceTypeNotAssignableToExpectedType", "TestModel.CityType", "TestModel.OfficeType"),
                },
                // Nested entry of depth 4 should fail because we set MaxNestingDepth = 3 below
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_MaxDepthOfNestedEntriesExceeded", "3"),
                }.InEntryWithExpandedLink(true /* isSingleton */)
                 .InEntryWithExpandedLink(true)
                 .InEntryWithExpandedLink(true),

                // Nested entry of depth 4 within expanded feeds should fail
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1),
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_MaxDepthOfNestedEntriesExceeded", "3"),
                }.InFeed()
                 .InEntryWithExpandedLink(false /* isSingleton */)
                 .InFeed()
                 .InEntryWithExpandedLink(false)
                 .InFeed()
                 .InEntryWithExpandedLink(false),
                
                // Nested entry of depth 3 should succeed
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1)
                }.InEntryWithExpandedLink(true /* isSingleton */)
                 .InEntryWithExpandedLink(true),
                
                // Nested entry of depth 3 within expanded feeds should succeed
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1)
                }.InFeed()
                 .InEntryWithExpandedLink(false /* isSingleton */)
                 .InFeed()
                 .InEntryWithExpandedLink(false),

                // Expanded feed with a number of child entries greater than recursive depth limit should succeed.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = new PayloadTestDescriptor(),
                    PayloadEdmModel = model,
                    PayloadElement = PayloadBuilder.EntitySet().Append(
                        PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 1),
                        PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 2),
                        PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 3),
                        PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 4),
                        PayloadBuilder.Entity("TestModel.Person").PrimitiveProperty("Id", 5)),
                }.InEntryWithExpandedLink(false /* isSingleton */)
                 .InEntryWithExpandedLink(true),
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.Concat(customTestDescriptors),
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.MessageQuotas.MaxNestingDepth = 3;

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
