//---------------------------------------------------------------------
// <copyright file="StreamingReadWriteTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Contracts.Json;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Scenario.Tests.Streaming;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Testing that ODataLib streams correctly in a read first then write scenario.
    /// </summary>
    [TestClass]
    public class StreamingReadWriteTests : ODataStreamingTestCase
    {
        [InjectDependency(IsRequired = true)]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [InjectDependency(IsRequired = true)]
        public PayloadWriterTestExpectedResults.Settings ExpectedResultSettings { get; set; }

        [Ignore]
        [TestMethod]
        public void StreamReadWriteFeed()
        {
            var payloadDescriptors = Test.OData.Utils.ODataLibTest.TestFeeds.GetFeeds(new EdmModel(), true /*withTypeNames*/);

            var testDescriptors = this.PayloadDescriptorsToStreamDescriptors(payloadDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations.Where(
                tc => tc.Synchronous),
               (testDescriptor, testConfiguration) =>
               {
                   testDescriptor.RunTest(testConfiguration);
               });
        }

        [Ignore]
        [TestMethod]
        public void StreamReadWriteEntry()
        {
            EdmModel model = Microsoft.Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            ComplexInstance complexValue = ODataStreamingTestCase.GetComplexInstanceWithManyPrimitiveProperties(model);

            var payloadDescriptors = new[]
                    {
                        // Multiple nesting of Complex Values and Multiple Values.
                        new PayloadTestDescriptor()
                            {
                                PayloadElement = PayloadBuilder.Property("propertyName", complexValue),
                                PayloadEdmModel = model.Clone(),
                                SkipTestConfiguration = (tc) => tc.Version < ODataVersion.V4
                            }.InComplexValue().InCollection().InProperty().InComplexValue().InCollection().InProperty().
                            InComplexValue()
                            .InCollection().InProperty().InComplexValue().InCollection().InProperty().InEntity(),

                        // Multiple nesting of Complex Values.
                        new PayloadTestDescriptor()
                            {
                                PayloadElement = PayloadBuilder.Property("propertyName", complexValue),
                                PayloadEdmModel = model.Clone(),
                            }.InComplexValue().InProperty().InComplexValue().InProperty().InComplexValue().InProperty()
                            .InComplexValue().InProperty().InComplexValue().InProperty().InComplexValue().InProperty().
                            InEntity(1, 0),

                        // Entry With an Expanded Link which is an entry containing a Complex collection.
                        new PayloadTestDescriptor()
                            {
                                PayloadElement = complexValue,
                                PayloadEdmModel = model.Clone(),
                                SkipTestConfiguration = (tc) => tc.Version < ODataVersion.V4
                            }.InCollection(1, 1).InProperty().InComplexValue().InCollection().InProperty().InEntity().
                            InEntryWithExpandedLink( /*singletonRelationship*/ true),

                        // Entry With an Expanded Link which is a Feed containing an Entry with Complex collection properties.
                        new PayloadTestDescriptor()
                            {
                                PayloadElement = complexValue,
                                PayloadEdmModel = model.Clone(),
                                SkipTestConfiguration = (tc) => tc.Version < ODataVersion.V4
                            }.InCollection(1, 2).InProperty().InComplexValue(1, 1).InCollection(1, 0).InProperty().
                            InEntity(1, 1).InFeed(2).InEntryWithExpandedLink(),

                        // Entry With Nested Expanded Links which contain Entries.
                        new PayloadTestDescriptor()
                            {
                                PayloadElement = PayloadBuilder.Property("propertyName", complexValue),
                                PayloadEdmModel = model.Clone(),
                            }.InEntity(1, 1, ODataVersion.V4).InEntryWithExpandedLink( /*singletonRelationship*/ true).
                            InEntryWithExpandedLink( /*singletonRelationship*/ true)
                            .InEntryWithExpandedLink( /*singletonRelationship*/ true).InEntryWithExpandedLink( /*singletonRelationship*/
                            true)
                            .InEntryWithExpandedLink( /*singletonRelationship*/ true).InEntryWithExpandedLink( /*singletonRelationship*/
                            true),

                        // Entry with inline expanded feed association to an arbitrary depth (7) where the expanded feed has no entries
                        new PayloadTestDescriptor()
                            {
                                PayloadElement =
                                    PayloadBuilder.EntitySet().WithTypeAnnotation(
                                        model.FindDeclaredType("TestModel.OfficeType")),
                                PayloadEdmModel = model.Clone(),
                            }.InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink().InFeed(2).
                            InEntryWithExpandedLink().InFeed(2).InEntryWithExpandedLink()
                            .InFeed(2).InEntryWithExpandedLink(),
                    };

            var testDescriptors = this.PayloadDescriptorsToStreamDescriptors(payloadDescriptors);

            this.CombinatorialEngineProvider.RunCombinations(
               testDescriptors,
               this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations.Where(
                tc => tc.Synchronous),
               (testDescriptor, testConfiguration) =>
               {
                   if (testConfiguration.Format == ODataFormat.Json)
                   {
                       testDescriptor.PayloadElement.Accept(new JsonDateTimeV3RepresentationFixup());
                   }

                   testDescriptor.RunTest(testConfiguration);
               });
        }

        private List<StreamingPayloadReaderTestDescriptor> PayloadDescriptorsToStreamDescriptors(IEnumerable<PayloadTestDescriptor> payloadDescriptors)
        {
            var testDescriptors = new List<StreamingPayloadReaderTestDescriptor>();

            foreach (var payloadDescriptor in payloadDescriptors)
            {
                var payload = payloadDescriptor.PayloadElement.DeepCopy();

                testDescriptors.Add(new StreamingPayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadDescriptor = payloadDescriptor,
                    PayloadElement = payload,
                    PayloadEdmModel = payloadDescriptor.PayloadEdmModel,
                    SkipTestConfiguration = payloadDescriptor.SkipTestConfiguration,
                    ExpectedResultCallback = (tc) =>
                     {
                         var payloadCopy = payload.DeepCopy();

                         return new PayloadWriterTestExpectedResults(this.ExpectedResultSettings)
                         {
                             ExpectedPayload = payloadCopy,
                         };
                     }
                });
            }

            return testDescriptors;
        }

        private class JsonDateTimeV3RepresentationFixup : ODataPayloadElementVisitorBase
        {
            public override void Visit(Astoria.Contracts.OData.PrimitiveValue payloadElement)
            {
                if (payloadElement.ClrValue is DateTime)
                {
                    // In V3, JSON DateTimes use ISO format - use XmlConvert to overwrite the default.
                    var dateTimePayload = (DateTime)payloadElement.ClrValue;
                    payloadElement.JsonRepresentation(
                        new JsonPrimitiveValue(XmlConvert.ToString(dateTimePayload, XmlDateTimeSerializationMode.RoundtripKind)));
                }

                base.Visit(payloadElement);
            }
        }

        private class JsonDateTimePreV3ClrValueFixup : ODataPayloadElementVisitorBase
        {
            public override void Visit(PrimitiveValue payloadElement)
            {
                if (payloadElement.ClrValue is DateTime)
                {
                    // In V1 and V2, DateTimes surface with Utc Kind, regardless of original source Kind.
                    var dateTimePayload = (DateTime)payloadElement.ClrValue;
                    if (dateTimePayload.Kind == DateTimeKind.Local)
                    {
                        dateTimePayload = dateTimePayload.ToUniversalTime();
                    }

                    // In V1 and V2, a certain amount of precision is lost when deserializing the old JSON format,
                    // so strip out the last four digits on the Ticks property.
                    long newTicks = Convert.ToInt64(Math.Truncate((decimal)dateTimePayload.Ticks / 10000) * 10000);
                    payloadElement.ClrValue = new DateTime(newTicks, DateTimeKind.Utc);
                }

                base.Visit(payloadElement);
            }
        }
    }
}
