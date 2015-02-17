//---------------------------------------------------------------------
// <copyright file="AtomCollectionWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Atom
{
    using System.Linq;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.CollectionWriter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for writing ATOM collections with the ODataMessageWriter.
    /// </summary>
    [TestClass, TestCase]
    public class AtomCollectionWriterTests : ODataWriterTestCase
    {
        [InjectDependency(IsRequired = true)]
        public CollectionWriterTestDescriptor.Settings Settings { get; set; }

        [TestMethod, Variation(Description = "Test error conditions when writing collections.")]
        public void CollectionNoErroronEmptyCollectionnTest()
        {
            var testCases = new[]
                {
                    new
                    {
                        CollectionName = (string)null,
                        PayloadItems = new CollectionWriterTestDescriptor.ItemDescription[0],
                        ExpectedException = (ExpectedException)null,
                    },
                };

            var testDescriptors = testCases.Select(tc =>
                new CollectionWriterTestDescriptor(
                    this.Settings,
                    tc.CollectionName,
                    tc.PayloadItems, 
                    tc.ExpectedException,
                    /*model*/null));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.WriterTestConfigurationProvider.AtomFormatConfigurationsWithIndent,
                (testDescriptor, testConfig) =>
                {
                    CollectionWriterUtils.WriteAndVerifyCollectionPayload(testDescriptor, testConfig, this.Assert, this.Logger);
                });

        }
    }
}
