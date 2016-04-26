//---------------------------------------------------------------------
// <copyright file="ErrorReaderTests.cs" company="Microsoft">
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
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Atom;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various error payloads.
    /// </summary>
    [TestClass, TestCase]
    public class ErrorReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Errors"), Variation(Description = "Verifies correct reading of error payloads.")]
        public void TopLevelErrorReaderTest()
        {
            IEdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            // Use some standard complex value payloads first
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateErrorReaderTestDescriptors(this.Settings);

            // make sure reading errors works with and without model
            testDescriptors = testDescriptors.Concat(testDescriptors.Select(td => new PayloadReaderTestDescriptor(td) { PayloadEdmModel = model }));

            // add the normalizer for ATOM error payloads
            testDescriptors = testDescriptors.Select(td =>
            {
                td.ExpectedResultNormalizers.Add(
                    tc => (Func<ODataPayloadElement, ODataPayloadElement>)null);
                return td;
            });

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        [TestMethod, TestCategory("Reader.Errors"), Variation(Description = "Verifies failure of reading too deeply recursive top-level error payloads.")]
        public void TopLevelDeeplyRecursiveErrorTest()
        {
            int depthLimit = 5;

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = PayloadReaderTestDescriptorGenerator.CreateErrorDeeplyNestedReaderTestDescriptors(this.Settings, depthLimit);

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations.Where(tc => !tc.IsRequest),
                (testDescriptor, testConfiguration) =>
                {
                    // Copy the test configuration so we can modify the depth limit.
                    testConfiguration = new ReaderTestConfiguration(testConfiguration);
                    testConfiguration.MessageReaderSettings.MessageQuotas.MaxNestingDepth = depthLimit;

                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}