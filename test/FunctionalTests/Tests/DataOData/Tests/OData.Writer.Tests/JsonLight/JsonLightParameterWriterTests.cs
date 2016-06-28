//---------------------------------------------------------------------
// <copyright file="JsonLightParameterWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.JsonLight
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing parameter payloads in JSON Lite format.
    /// </summary>
    [TestClass, TestCase]
    public class JsonLightParameterWriterTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [TestMethod, Variation(Description = "Error case for writing a complex parameters in JSON Lite without type information (in the API or the OM).")]
        public void WriteComplexParameterWithoutTypeInformationErrorTest()
        {
            EdmModel edmModel = new EdmModel();
            var container = new EdmEntityContainer("DefaultNamespace", "DefaultContainer");
            edmModel.AddElement(container);

            var testDescriptors = new PayloadWriterTestDescriptor<ODataParameters>[]
            {
                new PayloadWriterTestDescriptor<ODataParameters>(
                    this.Settings,
                    new ODataParameters()
                    {
                        new KeyValuePair<string, object>("p1", new ODataResource())
                    },
                    tc => new WriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedException2 = ODataExpectedExceptions.ODataException("WriterValidationUtils_MissingTypeNameWithMetadata")
                        })
                    {
                        DebugDescription = "Complex value without expected type or type name.",
                        Model = edmModel
                    },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.JsonLightFormatConfigurations.Where(tc => tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    TestWriterUtils.WriteAndVerifyODataParameterPayload(testDescriptor, testConfiguration, this.Assert, this.Logger);
                });
        }
    }
}
