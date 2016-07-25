//---------------------------------------------------------------------
// <copyright file="NullValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.JsonLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.JsonLight;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests reading of collection values in JSON.
    /// </summary>
    [TestClass, TestCase]
    public class NullValueJsonLightTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Json"), Variation(Description = "Verifies correct reading of null property values")]
        // TODO: Change the payload of null top-level properties #645
        public void NullValuePropertyTests()
        {
            EdmModel model = new EdmModel();

            var testCases = new []
            {
                new
                {
                    Payload = "{{\"@odata.context\":\"http://odata.org/test/$metadata#Edm.String\",\"value\":null{0}}}",
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => false)
                },
                new
                {
                    Payload = "{{\"value\":null{0}}}",
                    SkipTestConfiguration = (Func<ReaderTestConfiguration, bool>)(tc => tc.IsRequest == false)
                },
            };

            var testDescriptors = testCases.SelectMany(testCase =>
                new[]
                {
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = "null property value - should pass.",
                        PayloadEdmModel = model,
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).JsonRepresentation(string.Format(testCase.Payload, string.Empty)),
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    },
                    new PayloadReaderTestDescriptor(this.Settings)
                    {
                        DebugDescription = "null property value with custom annotation - should pass.",
                        PayloadEdmModel = model,
                        PayloadElement = PayloadBuilder.PrimitiveProperty(null, null).JsonRepresentation(string.Format(testCase.Payload, ",\"@Custom.Annotation\":\"foo\"")),
                        SkipTestConfiguration = testCase.SkipTestConfiguration
                    },
                });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.JsonLightFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    // These descriptors are already tailored specifically for Json Light and 
                    // do not require normalization.
                    testDescriptor.TestDescriptorNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
