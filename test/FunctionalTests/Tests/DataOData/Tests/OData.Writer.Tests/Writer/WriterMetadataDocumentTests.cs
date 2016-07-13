//---------------------------------------------------------------------
// <copyright file="WriterMetadataDocumentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Writer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MetadataUtils = Microsoft.Test.OData.Utils.Metadata.MetadataUtils;

    /// <summary>
    /// Tests for writing metadata with the ODatamessage writer.
    /// </summary>
    [TestClass, TestCase]
    public class WriterMetadataDocumentTests : ODataWriterTestCase
    {
        //EntityDataModelSchema Generator used to get the base model for other generators
        [InjectDependency(IsRequired = true)]
        public IModelGenerator ModelGenerator { get; set; }

        //CSDL Generator used to generate expected results
        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaSerializer EntityModelSchemaSerializer { get; set; }

        [InjectDependency]
        public MetadataWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Test the writing of metadata document payloads.")]
        public void MetadataDocumentWriterTest()
        {
            // TODO: add more interesting metadata payloads
            IEnumerable<MetadataWriterTestDescriptor> metadataDescriptors = MetadataWriterTestDescriptorGenerator.CreateMetadataDocumentWriterDescriptors(this.Settings);
            MetadataWriterTestDescriptor[] manualDescriptors = new MetadataWriterTestDescriptor[]
            {
                new MetadataWriterTestDescriptor(this.Settings)
                {
                    EdmVersion = EdmVersion.V40,
                    Model = CreateTestModel(MetadataUtils.EdmxVersion4)
                },
            };
            
            IEnumerable<MetadataWriterTestDescriptor> testDescriptors = metadataDescriptors.Concat(manualDescriptors);
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent
                    .Where(tc => tc.Synchronous && !tc.IsRequest),
                (testDescriptor, testConfiguration) => 
                    testDescriptor.RunTest(testConfiguration, this.Logger));
        }

        [TestMethod, Variation(Description = "Test the writing of metadata document payloads with errors.")]
        public void MetadataDocumentWriterErrorTest()
        {
            MetadataWriterTestDescriptor[] errorDescriptors = new MetadataWriterTestDescriptor[]
            {
                // EdmLib writes entity types without key
                //new MetadataWriterTestDescriptor(this.Settings)
                //{
                //    PayloadModel = TestModels.InvalidModels.EntityTypeWithoutKey,
                //    ExpectedODataExceptionMessage = "foo",
                //},
                // EdmLib writes duplicate types and test infrastructure can't handle them
                //new MetadataWriterTestDescriptor(this.Settings)
                //{
                //    PayloadModel = TestModels.InvalidModels.DuplicateEntityTypes,
                //    ExpectedODataExceptionMessage = "foo2",
                //},
                // EdmLib writes duplicate types and test infrastructure can't handle them
                //new MetadataWriterTestDescriptor(this.Settings)
                //{
                //    PayloadModel = TestModels.InvalidModels.DuplicateComplexTypes,
                //    ExpectedODataExceptionMessage = "foo3",
                //},
                // EdmLib writes duplicate properties and test infrastructure can't handle them
                //new MetadataWriterTestDescriptor(this.Settings)
                //{
                //    PayloadModel = TestModels.InvalidModels.ComplexTypeWithDuplicateProperties,
                //    ExpectedODataExceptionMessage = "foo4",
                //},
                // EdmLib writes duplicate properties and test infrastructure can't handle them
                //new MetadataWriterTestDescriptor(this.Settings)
                //{
                //    PayloadModel = TestModels.InvalidModels.EntityTypeWithDuplicateProperties,
                //    ExpectedODataExceptionMessage = "foo5",
                //},
            };

            IEnumerable<MetadataWriterErrorTestCase> testCases = errorDescriptors.Select(desc =>
                new MetadataWriterErrorTestCase
                {
                    TestDescriptor = desc,
                });

            IEnumerable<MetadataWriterErrorTestCase> manualTestCases = new MetadataWriterErrorTestCase[]
            {
                new MetadataWriterErrorTestCase
                {
                    TestDescriptor = new MetadataWriterTestDescriptor(this.Settings)
                    {
                        Model = new EdmModel().Fixup(),
                        ExpectedException = new TaupoNotSupportedException("Asynchronous metadata writing is not supported.")
                    },
                    RunInAsync = true,
                },
                new MetadataWriterErrorTestCase
                {
                    TestDescriptor = new MetadataWriterTestDescriptor(this.Settings)
                    {
                        Model = new EdmModel().Fixup(),
                        ExpectedODataExceptionMessage = "A metadata document cannot be written to request payloads. Metadata documents are only supported in responses.",
                    },
                    RunInRequest = true,
                },
            };

            var allTestCases = testCases.Concat(manualTestCases);

            this.CombinatorialEngineProvider.RunCombinations(
                allTestCases,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent,
                (testCase, testConfiguration) =>
                {
                    if (testCase.RunInAsync == testConfiguration.Synchronous)
                    {
                        return;
                    }

                    if (testCase.RunInRequest != testConfiguration.IsRequest)
                    {
                        return;
                    }

                    testCase.TestDescriptor.RunTest(testConfiguration, this.Logger);
                });
        }

        [TestMethod, Variation(Description = "Test the writing of metadata document payloads with OData-specific annotations using the ODataLib API.")]
        public void MetadataDocumentWithODataAnnotationsWriterTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildODataAnnotationTestModel(false);

            // NamedStream
            IEdmEntityType personType = model.FindType("TestModel.PersonType") as IEdmEntityType;
            this.Assert.IsNotNull(personType, "Expected to find the person type.");

            // MIME types
            IEdmProperty nameProperty = personType.DeclaredProperties.Where(p => p.Name == "Name").Single();
            model.SetMimeType(nameProperty, "text/plain");

            IEdmProperty addressProperty = personType.DeclaredProperties.Where(p => p.Name == "Address").Single();
            IEdmComplexType addressType = addressProperty.Type.Definition as IEdmComplexType;
            this.Assert.IsNotNull(addressType, "Expected address to have a complex type.");
            IEdmProperty zipProperty = addressType.DeclaredProperties.Where(p => p.Name == "Zip").Single();
            model.SetMimeType(zipProperty, "text/plain");

            IEnumerable<IEdmOperationImport> functionGroup = model.EntityContainer.FindOperationImports("ServiceOperation1");
            IEdmOperationImport functionImport = functionGroup.Single() as IEdmOperationImport;
            this.Assert.IsNotNull(functionImport, "Expected to find the function import.");
            model.SetMimeType(functionImport.Operation, "img/jpeg");

            MetadataWriterTestDescriptor[] testDescriptors = new MetadataWriterTestDescriptor[]
            {
                new MetadataWriterTestDescriptor(this.Settings)
                {
                    EdmVersion = EdmVersion.V40,
                    Model = model,
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent
                    .Where(tc => tc.Synchronous && !tc.IsRequest),
                (testDescriptor, testConfiguration) => 
                    {
                        testDescriptor.RunTest(testConfiguration, this.Logger);
                    });
        }

        [TestMethod, Variation(Description = "Test the writing of function import metadata annotations using ODataLib API.")]
        public void MetadataFunctionImportAnnotationsWriterTest()
        {
            // HttpMethod 
            var interestingValues = new [] 
            {
                new { HttpMethod = "GET" },
                new { HttpMethod= "POST" },
                new { HttpMethod= string.Empty },
                new { HttpMethod = "45" },
            };

            EdmModel model = (EdmModel) Microsoft.Test.OData.Utils.Metadata.TestModels.BuildODataAnnotationTestModel(false);
            var container = (EdmEntityContainer) model.EntityContainer;

            foreach (var value in interestingValues)
            {
                var action = new EdmAction("TestModel", "ActionImport_" + value.HttpMethod, null, true, null);
                model.AddElement(action);
                IEdmActionImport actionImport = container.AddActionImport(action);
                this.Assert.IsNotNull(actionImport, "Expected to find the function import.");
            }

            MetadataWriterTestDescriptor[] testDescriptors = new MetadataWriterTestDescriptor[]
            {
                new MetadataWriterTestDescriptor(this.Settings)
                {
                    EdmVersion = EdmVersion.V40,
                    Model = model,
                }
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.DefaultFormatConfigurationsWithIndent
                    .Where(tc => tc.Synchronous && !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration, this.Logger);
                });
        }

        // Metadata serialization: add tests for reading/writing in-stream errors

        /// <summary>
        /// Creates a test model in the specified EDM(X) version for manual testing.
        /// </summary>
        /// <param name="edmxVersion">The EDM(X) version to use.</param>
        /// <returns>The created <see cref="IEdmModel"/>.</returns>
        private IEdmModel CreateTestModel(Version edmxVersion)
        {
            string ns = edmxVersion == MetadataUtils.EdmxVersion4
                          ? Microsoft.Test.Taupo.Contracts.EntityModel.Edm.EdmConstants.CsdlOasisNamespace.NamespaceName
                          : null;
            this.Assert.IsNotNull(ns, "Could not determine EDMX namespace for version " + edmxVersion + ".");

            string csdl = @"
<Schema Namespace='TestModel' p1:UseStrongSpatialTypes='false' xmlns:p1='http://schemas.microsoft.com/ado/2009/02/edm/annotation' xmlns='" + ns + @"'>
  <EntityContainer Name='DefaultContainer'>
    <EntitySet Name='EPMEntityType' EntityType='TestModel.EPMEntityType' />
  </EntityContainer>
  <EntityType Name='EPMEntityType'>
    <Key>
      <PropertyRef Name='ID' />
    </Key>
    <Property Name='ID' Type='Int32' Nullable='false' />
  </EntityType>
  <ComplexType Name='EPMComplexType'>
    <Property Name='Name' Type='String' />
  </ComplexType>
</Schema>";

            IEdmModel model;
            IEnumerable<EdmError> errors;
            using (XmlReader reader = XmlReader.Create(new StringReader(csdl)))
            {
                if (!SchemaReader.TryParse(new[] { reader }, out model, out errors))
                {
                    string errorStrings = string.Join(", ", errors.Select(e => e.ToString()).ToArray());
                    this.Assert.Fail("Could not parse CSDL: " + System.Environment.NewLine + errorStrings);
                }
            }

            model.SetEdmxVersion(edmxVersion);
            return model;
        }

        private sealed class MetadataWriterErrorTestCase
        {
            public MetadataWriterTestDescriptor TestDescriptor { get; set; }
            public bool RunInAsync { get; set; }
            public bool RunInRequest { get; set; }
        }
    }
}
