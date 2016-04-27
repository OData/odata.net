//---------------------------------------------------------------------
// <copyright file="StreamingWriteReadTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.CombinatorialEngine;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming;
    using Microsoft.Test.Taupo.OData.Writer.Tests;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StreamingWriteReadTests : ODataWriterTestCase
    {
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        [InjectDependency(IsRequired = true)]
        public StreamingPayloadWriterTestDescriptor<ODataPayloadElement>.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [InjectDependency(IsRequired = true)]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [Ignore] // Remove Atom
        [TestMethod]
        public void StreamWriteReadFeed()
        {
            var payloadDescriptors = Test.OData.Utils.ODataLibTest.TestFeeds.GetFeeds(new EdmModel(), true /*withTypeNames*/);

            var testDescriptors = this.PayloadDescriptorsToStreamDescriptors(payloadDescriptors);

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
               (testDescriptor, testConfiguration) =>
               {
                   testConfiguration = testConfiguration.Clone();
                   testConfiguration.MessageWriterSettings.ODataUri = new Microsoft.OData.ODataUri()
                   {
                       ServiceRoot = ServiceDocumentUri
                   };
                   testDescriptor.RunTest(testConfiguration);
               });
        }

        [Ignore]
        [TestMethod]
        public void StreamWriteReadEntry()
        {
            EdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            ComplexInstance complexValue = ODataStreamingTestCase.GetComplexInstanceWithManyPrimitiveProperties(model);

            var payloadDescriptors = new PayloadTestDescriptor[]
            { 
                // Multiple nesting of Complex Values and Multiple Values.
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.Property("propertyName", complexValue),
                    PayloadEdmModel = model.Clone(),
                    SkipTestConfiguration = (tc) => tc.Version < ODataVersion.V4
                }.InComplexValue().InCollection().InProperty().InComplexValue().InCollection().InProperty().InComplexValue()
                .InCollection().InProperty().InComplexValue().InCollection().InProperty().InEntity(),
                
                // Multiple nesting of Complex Values.
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.Property("propertyName", complexValue),
                    PayloadEdmModel = model.Clone(),
                }.InComplexValue().InProperty().InComplexValue().InProperty().InComplexValue().InProperty()
                .InComplexValue().InProperty().InComplexValue().InProperty().InComplexValue().InProperty().InEntity(1,0),
                
                // Entry With an Expanded Link which is an entry containing a Complex collection.
                new PayloadTestDescriptor()
                {
                    PayloadElement = complexValue,
                    PayloadEdmModel = model.Clone(),
                    SkipTestConfiguration = (tc) => tc.Version < ODataVersion.V4
                }.InCollection(1,1).InProperty().InComplexValue().InCollection().InProperty().InEntity().InEntryWithExpandedLink(/*singletonRelationship*/ true),
                
                // Entry With an Expanded Link which is a Feed containing an Entry with Complex collection properties.
                new PayloadTestDescriptor()
                {
                    PayloadElement = complexValue,
                    PayloadEdmModel = model.Clone(),
                    SkipTestConfiguration = (tc) => tc.Version < ODataVersion.V4
                }.InCollection(1,2).InProperty().InComplexValue(1, 1).InCollection(1, 0).InProperty().InEntity(1,1).InFeed(2).InEntryWithExpandedLink(),
                
                // Entry With Nested Expanded Links which contain Entries.
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.Property("propertyName", complexValue),
                    PayloadEdmModel = model.Clone(),
                }.InEntity(1, 1, ODataVersion.V4).InEntryWithExpandedLink(/*singletonRelationship*/ true).InEntryWithExpandedLink(/*singletonRelationship*/ true)
                .InEntryWithExpandedLink(/*singletonRelationship*/ true).InEntryWithExpandedLink(/*singletonRelationship*/ true)
                .InEntryWithExpandedLink(/*singletonRelationship*/ true).InEntryWithExpandedLink(/*singletonRelationship*/ true),

                // Entry with inline expanded feed association to an arbitrary depth (7) where the expanded feed has no entries
                new PayloadTestDescriptor()
                {
                    PayloadElement = PayloadBuilder.EntitySet().WithTypeAnnotation(model.FindDeclaredType("TestModel.OfficeType")),
                    PayloadEdmModel = model.Clone(),
                }.InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink()
                .InFeed(2).InEntryWithExpandedLink(),
            };

            var testDescriptors = this.PayloadDescriptorsToStreamDescriptors(payloadDescriptors);

            //ToDo: Fix places where we've lost JsonVerbose coverage to add JsonLight
            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.WriterTestConfigurationProvider.ExplicitFormatConfigurationsWithIndent.Where(tc => false),
               (testDescriptor, testConfiguration) =>
               {
                   testDescriptor.RunTest(testConfiguration);
               });
        }

        private List<StreamingPayloadWriterTestDescriptor<ODataPayloadElement>> PayloadDescriptorsToStreamDescriptors(IEnumerable<PayloadTestDescriptor> payloadDescriptors)
        {
            var testDescriptors = new List<StreamingPayloadWriterTestDescriptor<ODataPayloadElement>>();

            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement.DeepCopy();
                testDescriptors.Add(new StreamingPayloadWriterTestDescriptor<ODataPayloadElement>(this.Settings)
                {
                    PayloadDescriptor = payloadDescriptor,
                    PayloadElement = (payload),
                    SkipTestConfiguration = payloadDescriptor.SkipTestConfiguration,
                    ExpectedResultCallback = (tc) =>
                    {
                        return new StreamingWriterTestExpectedResults(this.ExpectedResultSettings)
                        {
                            ExpectedPayload = payload
                        };
                    }

                });
            }
            return testDescriptors;
        }
    }
}
