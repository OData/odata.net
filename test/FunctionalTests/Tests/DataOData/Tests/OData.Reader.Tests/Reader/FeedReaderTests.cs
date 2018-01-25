//---------------------------------------------------------------------
// <copyright file="FeedReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various feed payloads.
    /// </summary>
    [TestClass, TestCase]
    public class FeedReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Feeds"), Variation(Description = "Test the the reading of feed payloads without metadata.")]
        public void FeedReadingTest()
        {
            EdmModel model = Test.OData.Utils.Metadata.TestModels.BuildTestModel() as EdmModel;

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateEntitySetInstanceDescriptors(this.Settings, model, true);

            IEnumerable<PayloadReaderTestDescriptor> payloadDescriptors = testDescriptors
                .SelectMany(feed => this.PayloadGenerator.GenerateReaderPayloads(feed));

            this.CombinatorialEngineProvider.RunCombinations(
                payloadDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfig) =>
                {
                    testDescriptor.RunTest(testConfig);
                });
        }

        private IEnumerable<PayloadReaderTestDescriptor> CreateFeedValidatorDescriptors(IEdmModel model)
        {
            var cityType = model.FindType("TestModel.CityType");
            var personType = model.FindType("TestModel.Person");
            var employeeType = model.FindType("TestModel.Employee");

            var testCases = new[]
                {
                    new
                    {
                        Description = "Homogenous collection (no inheritance)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithTypeAnnotation(personType).IgnoreEntitySet()
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(personType))
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(personType)),
                        ExpectedException = (ExpectedException)null,
                        Model = model
                    },
                    new
                    {
                        Description = "Homogenous collection (inheritance, base type first)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithTypeAnnotation(personType).IgnoreEntitySet()
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(personType))
                            .Append(PayloadBuilder.Entity("TestModel.Employee")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(employeeType)),
                        ExpectedException = (ExpectedException)null,
                        Model = model
                    },
                    new
                    {
                        Description = "Homogenous collection (inheritance, derived type first)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithTypeAnnotation(personType).IgnoreEntitySet()
                            .Append(PayloadBuilder.Entity("TestModel.Employee")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(employeeType))
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(personType)),
                        ExpectedException = (ExpectedException)null,
                        Model = model
                    },
                    new
                    {
                        Description = "Heterogeneous collection",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().WithTypeAnnotation(personType).IgnoreEntitySet()
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1).WithTypeAnnotation(personType))
                            .Append(PayloadBuilder.Entity("TestModel.CityType")
                                .PrimitiveProperty("Id", 2).WithTypeAnnotation(cityType)),
                        ExpectedException = ODataExpectedExceptions.ODataException("ResourceSetWithoutExpectedTypeValidator_IncompatibleTypes", "TestModel.CityType", "TestModel.Person"),
                        Model = model
                    },
                    new
                    {
                        Description = "Heterogeneous collection (no model)",
                        Feed = (ODataPayloadElement)PayloadBuilder.EntitySet().IgnoreEntitySet()
                            .Append(PayloadBuilder.Entity("TestModel.Person")
                                .PrimitiveProperty("Id", 1))
                            .Append(PayloadBuilder.Entity("TestModel.CityType")
                                .PrimitiveProperty("Id", 2)),
                        ExpectedException = (ExpectedException)null,
                        Model = (IEdmModel)null
                    },
                };

            // Create the tests for a top-level feed
            return testCases.Select(testCase =>
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    DebugDescription = testCase.Description,
                    PayloadDescriptor = new PayloadTestDescriptor()
                    {
                        DebugDescription = testCase.Description,
                        PayloadElement = testCase.Feed,
                    },
                    PayloadEdmModel = testCase.Model,
                    ExpectedException = testCase.ExpectedException,
                });
        }
    }
}
