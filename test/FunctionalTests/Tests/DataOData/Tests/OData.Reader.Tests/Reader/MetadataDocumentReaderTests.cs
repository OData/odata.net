//---------------------------------------------------------------------
// <copyright file="MetadataDocumentReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.PayloadTransformation;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataCommon = Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various metadata document payloads.
    /// </summary>
    [TestClass, TestCase]
    public class MetadataDocumentReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public MetadataReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency]
        public PayloadKindDetectionTestDescriptor.Settings PayloadKindDetectionSettings { get; set; }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test the reading of metadata document payloads.")]
        public void MetadataDocumentReaderTest()
        {
            // TODO: add more interesting metadata payloads
            IEnumerable<MetadataReaderTestDescriptor> metadataDescriptors = MetadataReaderTestDescriptorGenerator.CreateMetadataDocumentReaderDescriptors(this.Settings);
            MetadataReaderTestDescriptor[] manualDescriptors = new MetadataReaderTestDescriptor[]
            {
            };

            IEnumerable<MetadataReaderTestDescriptor> testDescriptors = metadataDescriptors.Concat(manualDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations.Where(tc => !tc.IsRequest && tc.Synchronous),
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test the reading of OData-specific metadata annotations with the ODataLib API.")]
        public void MetadataDocumentWithODataAnnotationsReaderTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildODataAnnotationTestModel(true);

            // Now use the ODataUtils API to make sure the expected annotations exist
            // Default container
            IEdmEntityContainer defaultContainer = model.EntityContainer;

            // Default Stream
            IEdmEntityType personType = model.FindType("TestModel.PersonType") as IEdmEntityType;
            this.Assert.IsNotNull(personType, "Expected to find the person type.");
            this.Assert.IsTrue(personType.HasStream, "Did not find expected HasStream annnotation.");

            // MIME types
            IEdmProperty nameProperty = personType.DeclaredProperties.Where(p => p.Name == "Name").Single();
            this.Assert.AreEqual("text/plain", model.GetMimeType(nameProperty), "MIME types of 'Name' property don't match.");

            IEdmProperty addressProperty = personType.DeclaredProperties.Where(p => p.Name == "Address").Single();
            IEdmComplexType addressType = addressProperty.Type.Definition as IEdmComplexType;
            this.Assert.IsNotNull(addressType, "Expected address to have a complex type.");
            IEdmProperty zipProperty = addressType.DeclaredProperties.Where(p => p.Name == "Zip").Single();
            this.Assert.AreEqual("text/plain", model.GetMimeType(zipProperty), "MIME types of 'Zip' property don't match.");

            IEnumerable<IEdmOperationImport> functionGroup = defaultContainer.FindOperationImports("ServiceOperation1");
            IEdmOperationImport functionImport = functionGroup.Single() as IEdmOperationImport;
            this.Assert.IsNotNull(functionImport, "Expected to find the function import.");
            this.Assert.AreEqual("img/jpeg", model.GetMimeType(functionImport.Operation), "MIME types of function import don't match.");
        }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test the reading of metadata annotations of function import with the ODataLib API.")]
        public void MetadataFunctionImportAnnotationsReaderTest()
        {
            // HttpMethod 
            var interestingValues = new[]
            {
                new { HttpMethod = "GET" },
                new { HttpMethod= "POST" },
                new { HttpMethod= string.Empty },
                new { HttpMethod = "45" },
            };

            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildODataAnnotationTestModel(false);

            foreach (var value in interestingValues)
            {
                var container = model.EntityContainer as EdmEntityContainer;
                EdmFunction expectedFunction = new EdmFunction(container.Namespace, "FunctionImport_" + value.HttpMethod, EdmCoreModel.Instance.GetInt32(true), true /*isBound*/, null, true/*isComposable*/);
                container.AddFunctionImport("FunctionImport_" + value.HttpMethod, expectedFunction);
            }

            foreach (var value in interestingValues)
            {
                IEdmOperationImport actualFunctionImport = model.EntityContainer.FindOperationImports("FunctionImport_" + value.HttpMethod).Single();
                this.Assert.IsNotNull(actualFunctionImport, "Expected to find the function import.");
            }
        }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test for reading the Model used by Astoria Epm Mappings tests")]
        public void MetadataDocumentReaderAstoriaModelTest()
        {
            List<MetadataReaderTestDescriptor> testCases = new List<MetadataReaderTestDescriptor>();
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildDefaultAstoriaTestModel();
            testCases.Add(this.CreateMetadataDescriptor(model));

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations.Where(tc => tc.Synchronous && !tc.IsRequest),
                (testDescriptor, testConfiguration) => testDescriptor.RunTest(testConfiguration));
        }

        [TestMethod, TestCategory("Reader.MetadataDocument"), Variation(Description = "Test for reading metadata documents with element types appearing in different orders.")]
        public void MetadataDocumentElementTypeOrderTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildDefaultAstoriaTestModel();
            IEnumerable<MetadataReaderTestDescriptor> metadataDescriptors = new[] { this.CreateMetadataDescriptor(model) };

            var metadataElementOrderPermutations = DataUtilities.GetEnumValues<MetadataDocumentReorderingTransform.MetadataDocumentElementType>().ToList().Permutations();
            var entityContainerElementOrderPermutations = DataUtilities.GetEnumValues<MetadataDocumentReorderingTransform.EntityContainerElementType>().ToList().Permutations();

            this.CombinatorialEngineProvider.RunCombinations(
                metadataDescriptors,
                this.ReaderTestConfigurationProvider.DefaultFormatConfigurations.Where(tc => !tc.IsRequest && tc.Synchronous),
                metadataElementOrderPermutations,
                entityContainerElementOrderPermutations,
                (testDescriptor, testConfiguration, metadataOrder, entityContainerOrder) =>
                {
                    var testDescriptorCopy = (MetadataReaderTestDescriptor)testDescriptor.Clone();
                    testDescriptorCopy.MetadataDocumentTransform = new MetadataDocumentReorderingTransform(metadataOrder, entityContainerOrder);
                    testDescriptorCopy.RunTest(testConfiguration);
                });
        }

        private MetadataReaderTestDescriptor CreateMetadataDescriptor(IEdmModel testModel, ExpectedException expectedException = null)
        {
            return new MetadataReaderTestDescriptor(this.Settings)
            {
                PayloadEdmModel = testModel,

                // ExpectedPayloadEdmModel is not used anywhere. The test infrastructure should be updated to use this,
                // or the tests that call this method should be replaced with TDD tests.
                ExpectedPayloadEdmModel = testModel,
                ExpectedException = expectedException,
            };
        }
    }
}
