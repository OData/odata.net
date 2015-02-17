//---------------------------------------------------------------------
// <copyright file="EntryPayloadOrderReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests payload order reading of entries in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class EntryPayloadOrderReaderAtomTests : ODataPayloadOrderReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct payload order of items in an entry.")]
        public void EntryPayloadOrderTest()
        {
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new[]
            {
                // Nothing
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity()
                        .XmlRepresentation("<entry></entry>")
                        .PayloadOrderItems("__StartEntry__")
                },
                // Just type name after nav. link - type name should be reported first.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("MyType").NavigationProperty("Category", "http://odata.org/Category")
                        .XmlRepresentation(
                            "<entry>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' href='http://odata.org/Category'/>" +
                                "<category term='MyType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                            "</entry>")
                        .PayloadOrderItems(
                            "TypeName",
                            "__StartEntry__",
                            "NavigationLink_Category")
                },
                // Type name properties and other things after link - type name and etag should be reported first, the rest after the link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("MyType")
                        .ETag("bar")
                        .NavigationProperty("Category", "http://odata.org/Category")
                        .WithSelfLink("http://odata.org")
                        .PrimitiveProperty("Name", "Foo")
                        .XmlRepresentation(
                            "<entry m:etag='bar'>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' href='http://odata.org/Category'/>" +
                                "<category term='MyType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                "<link rel='self' href='http://odata.org'/>" +
                            "</entry>")
                        .PayloadOrderItems(
                            "TypeName",
                            "ETag",
                            "__StartEntry__",
                            "NavigationLink_Category",
                            "ReadLink",
                            "Property_Name"),
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    SkipTestConfiguration = config => config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer         
                },
                // Everything before nav. link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("MyType")
                        .ETag("bar")
                        .PrimitiveProperty("Name", "Foo")
                        .NavigationProperty("Category", "http://odata.org/Category")
                        .WithSelfLink("http://odata.org")
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsAction = true, Metadata = "/actionMetadata", Target = "http://odata.org/action" })
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsFunction = true, Metadata = "/functionMetadata", Target = "http://odata.org/function" })
                        .XmlRepresentation(
                            "<entry m:etag='bar'>" +
                                "<category term='MyType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                "<link rel='self' href='http://odata.org'/>" +
                                "<m:action metadata='/actionMetadata' target='http://odata.org/action'/>" +
                                "<m:function metadata='/functionMetadata' target='http://odata.org/function'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/PoliceStation' href='http://odata.org/associationlink' type='application/xml'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' href='http://odata.org/Category'/>" +
                            "</entry>")
                        .PayloadOrderItems(
                            "TypeName",
                            "ReadLink",
                            "ETag",
                            "Action_/actionMetadata",
                            "Function_/functionMetadata",
                            "Property_Name",
                            "__StartEntry__",
                            "NavigationLink_Category"),
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    SkipTestConfiguration = config => config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer || config.IsRequest
                },
                // Everything before nav. link - Server behavior - server ignores most of links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("MyType")
                        .ETag("bar")
                        .PrimitiveProperty("Name", "Foo")
                        .NavigationProperty("Category", "http://odata.org/Category")
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsAction = true, Metadata = "/actionMetadata", Target = "http://odata.org/action" })
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsFunction = true, Metadata = "/functionMetadata", Target = "http://odata.org/function" })
                        .XmlRepresentation(
                            "<entry m:etag='bar'>" +
                                "<category term='MyType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                "<link rel='self' href='http://odata.org'/>" +
                                "<m:action metadata='/actionMetadata' target='http://odata.org/action'/>" +
                                "<m:function metadata='/functionMetadata' target='http://odata.org/function'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/PoliceStation' href='http://odata.org/associationlink' type='application/xml'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' href='http://odata.org/Category'/>" +
                            "</entry>")
                        .PayloadOrderItems(
                            "TypeName",
                            "ETag",
                            "Action_/actionMetadata",
                            "Function_/functionMetadata",
                            "Property_Name",
                            "__StartEntry__",
                            "NavigationLink_Category"),
                    SkipTestConfiguration = config => config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesServer || config.IsRequest
                }, 
                // Everything between nav. links
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("MyType")
                        .ETag("bar")
                        .NavigationProperty("Order", "http://odata.org/Order")
                        .PrimitiveProperty("Name", "Foo")
                        .NavigationProperty("Category", "http://odata.org/Category")
                        .WithSelfLink("http://odata.org")
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsAction = true, Metadata = "/actionMetadata", Target = "http://odata.org/action" })
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsFunction = true, Metadata = "/functionMetadata", Target = "http://odata.org/function" })
                        .XmlRepresentation(
                            "<entry m:etag='bar'>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Order' type='application/atom+xml;type=entry' href='http://odata.org/Order'/>" +
                                "<category term='MyType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                "<link rel='self' href='http://odata.org'/>" +
                                "<m:action metadata='/actionMetadata' target='http://odata.org/action'/>" +
                                "<m:function metadata='/functionMetadata' target='http://odata.org/function'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/PoliceStation' href='http://odata.org/associationlink' type='application/xml'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' href='http://odata.org/Category'/>" +
                            "</entry>")
                        .PayloadOrderItems(
                            "TypeName",
                            "ETag",
                            "__StartEntry__",
                            "NavigationLink_Order",
                            "ReadLink",
                            "Action_/actionMetadata",
                            "Function_/functionMetadata",
                            "Property_Name",
                            "NavigationLink_Category"),
                    // [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't
                    SkipTestConfiguration = config => config.RunBehaviorKind == TestODataBehaviorKind.WcfDataServicesServer || config.IsRequest
                },
                // Everything between nav. links - server behavior
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("MyType")
                        .ETag("bar")
                        .NavigationProperty("Order", "http://odata.org/Order")
                        .PrimitiveProperty("Name", "Foo")
                        .NavigationProperty("Category", "http://odata.org/Category")
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsAction = true, Metadata = "/actionMetadata", Target = "http://odata.org/action" })
                        .OperationDescriptor(new ServiceOperationDescriptor() { IsFunction = true, Metadata = "/functionMetadata", Target = "http://odata.org/function" })
                        .XmlRepresentation(
                            "<entry m:etag='bar'>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Order' type='application/atom+xml;type=entry' href='http://odata.org/Order'/>" +
                                "<category term='MyType' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                                "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                                "<link rel='self' href='http://odata.org'/>" +
                                "<m:action metadata='/actionMetadata' target='http://odata.org/action'/>" +
                                "<m:function metadata='/functionMetadata' target='http://odata.org/function'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/PoliceStation' href='http://odata.org/associationlink' type='application/xml'/>" +
                                "<link rel='http://docs.oasis-open.org/odata/ns/related/Category' type='application/atom+xml;type=entry' href='http://odata.org/Category'/>" +
                            "</entry>")
                        .PayloadOrderItems(
                            "TypeName",
                            "ETag",
                            "__StartEntry__",
                            "NavigationLink_Order",
                            "Action_/actionMetadata",
                            "Function_/functionMetadata",
                            "Property_Name",
                            "NavigationLink_Category"),
                    SkipTestConfiguration = config => config.RunBehaviorKind != TestODataBehaviorKind.WcfDataServicesServer || config.IsRequest
                },
                // EPM
                //new PayloadReaderTestDescriptor(this.Settings)
                //{
                //    PayloadElement = PayloadBuilder.Entity("TestModel.EpmEntity")
                //        .NavigationProperty("Order", "http://odata.org/Order")
                //        .PrimitiveProperty("Name", "Foo")
                //        .NavigationProperty("Orders", "http://odata.org/Orders")
                //        .PrimitiveProperty("Description", "Bart")
                //        .XmlRepresentation(
                //            "<entry>" +
                //                "<link rel='http://docs.oasis-open.org/odata/ns/related/Order' type='application/atom+xml;type=entry' href='http://odata.org/Order'/>" +
                //                "<category term='TestModel.EpmEntity' scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                //                "<content type='application/xml'><m:properties><d:Name>Foo</d:Name></m:properties></content>" + 
                //                "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed' href='http://odata.org/Orders'/>" +
                //                "<author><name>Bart</name></author>" +
                //            "</entry>")
                //        .PayloadOrderItems(
                //            "TypeName",
                //            "__StartEntry__",
                //            "NavigationLink_Order",
                //            "Property_Name",
                //            "NavigationLink_Orders",
                //            "Property_Description"),
                //    PayloadModel = model
                //}
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                TestReaderUtils.ODataBehaviorKinds,
                (testDescriptor, testConfiguration, behavior) =>
                {
                    // In WCF DS Server mode the reader reports the StartEntry as soon as it finds a type name.
                    //   Since the ETag is always on the start element it will be reported in StartEntry as well.
                    if (behavior == TestODataBehaviorKind.WcfDataServicesServer)
                    {
                        ODataPayloadElement element = testDescriptor.PayloadElement.DeepCopy();
                        PayloadOrderODataPayloadElementAnnotation payloadOrderAnnotation = element.GetAnnotation<PayloadOrderODataPayloadElementAnnotation>();
                        int startEntryPosition = 0;
                        if (payloadOrderAnnotation.PayloadItems[0] == "TypeName")
                        {
                            startEntryPosition++;
                        }

                        if (payloadOrderAnnotation.PayloadItems.Contains("ETag"))
                        {
                            payloadOrderAnnotation.PayloadItems.Remove("ETag");
                            payloadOrderAnnotation.PayloadItems.Insert(1, "ETag");
                            startEntryPosition++;
                        }

                        payloadOrderAnnotation.PayloadItems.Remove("__StartEntry__");
                        payloadOrderAnnotation.PayloadItems.Insert(startEntryPosition, "__StartEntry__");

                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor)
                        {
                            PayloadElement = element
                        };
                    }

                    // We must remove any payload normalizers since we are now supersensitive to ordering issues in this test.
                    testDescriptor.ExpectedResultNormalizers.Clear();
                    testDescriptor.RunTest(testConfiguration.CloneAndApplyBehavior(behavior));
                });
        }
    }
}
