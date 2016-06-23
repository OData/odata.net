//---------------------------------------------------------------------
// <copyright file="CollectionReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of various collection payloads.
    /// </summary>
    [TestClass, TestCase]
    public class CollectionReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Collections"), Variation(Description = "Test the the reading of homogeneous collection payloads.")]
        public void HomogeneousCollectionReaderTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors =
                PayloadReaderTestDescriptorGenerator.CreateHomogeneousCollectionValueTestDescriptors(this.Settings, withMetadata: true, withTypeNames: true, withExpectedType: true, withcollectionName:false, fullSet: true)
                .Concat(PayloadReaderTestDescriptorGenerator.CreateHomogeneousCollectionValueTestDescriptors(this.Settings, withMetadata: true, withTypeNames: false, withExpectedType: true, withcollectionName: false, fullSet: true))
                .Concat(PayloadReaderTestDescriptorGenerator.CreateHomogeneousCollectionValueTestDescriptors(this.Settings, withMetadata: true, withTypeNames: true, withExpectedType: false, withcollectionName: false, fullSet: true));

            // Generate interesting payloads around the collection
            testDescriptors = testDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));
            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }
    }
}
