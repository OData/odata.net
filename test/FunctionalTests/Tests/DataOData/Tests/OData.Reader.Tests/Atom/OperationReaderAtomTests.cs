//---------------------------------------------------------------------
// <copyright file="OperationReaderAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of m:action and m:function in ATOM.
    /// </summary>
    [TestClass, TestCase]
    public class OperationReaderAtomTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies correct reading of m:action and m:function without metadata.")]
        public void ODataOperationReadTest()
        {
            EntityInstance entryWithOperations = PayloadBuilder.Entity();
            entryWithOperations.Add(new ServiceOperationDescriptor { IsAction = true, Metadata = "http://odata.org/operationMetadata", Title = "operation title", Target = "http://odata.org/operationTarget" });
            entryWithOperations.Add(new ServiceOperationDescriptor { IsFunction = true, Metadata = "http://odata.org/operationMetadata", Title = "operation title", Target = "http://odata.org/operationTarget" });
            
            var validCases = new[]
                {
                    new
                    {
                        // action and function in atom namespace
                        payloadElement = PayloadBuilder.Entity(),
                        Xml = "<$operation metadata='http://odata.org/operationMetadata' title='operation title' target='http://odata.org/operationTarget'/>"
                    },
                    new
                    {
                        // action and function with additional attributes
                        payloadElement = entryWithOperations,
                        Xml = "<m:$operation metadata='http://odata.org/operationMetadata' title='operation title' target='http://odata.org/operationTarget' nonamespace='nonsattribute' m:random='random'/>"
                    },
                    new
                    {
                        // action and function with content
                        payloadElement = entryWithOperations,
                        Xml = "<m:$operation metadata='http://odata.org/operationMetadata' title='operation title' target='http://odata.org/operationTarget'><randomElement/> random text</m:$operation>"
                    },
                    new
                    {
                        // action and function with additional attributes in with the same local name but in a different namespace
                        payloadElement = entryWithOperations,
                        Xml = @"<m:$operation metadata='http://odata.org/operationMetadata' title='operation title' target='http://odata.org/operationTarget'
                                            m:rel='ignored' m:target='ignored' m:title='ignored' m:metadata='ignored' />"
                    },
                    new
                    {
                        // action and function after extra XML element
                        payloadElement = entryWithOperations,
                        Xml = "<randomElement>SomeText</randomElement><m:$operation metadata='http://odata.org/operationMetadata' title='operation title' target='http://odata.org/operationTarget' />"
                    },
                    new
                    {
                        // action and function before extra XML element
                        payloadElement = entryWithOperations,
                        Xml = "<m:$operation metadata='http://odata.org/operationMetadata' title='operation title' target='http://odata.org/operationTarget' /><randomElement>SomeText</randomElement>"
                    }
                };

            var errorCases = new[]
                {
                    new OperationReaderErrorTestCase
                    {
                        Xml = "<m:$operation/>",
                        GetExpectedException = (arg) => ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_OperationMissingMetadataAttribute", arg),
                    },
                    new OperationReaderErrorTestCase
                    {
                        Xml = "<m:$operation target='http://odata.org'/>",
                        GetExpectedException = (arg) => ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_OperationMissingMetadataAttribute", arg),
                    },
                    new OperationReaderErrorTestCase
                    {
                        Xml = "<m:$operation m:metadata='/operationMetadata' target='http://odata.org'/>",
                        GetExpectedException = (arg) => ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_OperationMissingMetadataAttribute", arg),
                    },
                    new OperationReaderErrorTestCase
                    {
                        Xml = "<m:$operation metadata='/operationMetadata'/>",
                        GetExpectedException = (arg) => ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute", arg),
                    },
                    new OperationReaderErrorTestCase
                    {
                        Xml = "<m:$operation metadata='/operationMetadata' m:target='random text'/>",
                        GetExpectedException = (arg) => ODataExpectedExceptions.ODataException("ODataAtomEntryAndFeedDeserializer_OperationMissingTargetAttribute", arg),
                    },
                };

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = validCases.Select(
                vc => new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = vc.payloadElement.DeepCopy()
                        .XmlRepresentation("<entry xmlns:m='http://docs.oasis-open.org/odata/ns/metadata'>" +
                                            vc.Xml.Replace("$operation", "action") +
                                            vc.Xml.Replace("$operation", "function") +
                                            "</entry>")
                })
                .Concat(errorCases.Select( // concat error cases for m:action
                    ec => new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                                    .XmlRepresentation("<entry>" + 
                                                        ec.Xml.Replace("$operation", "action") + 
                                                        "</entry>"),
                        ExpectedException = ec.GetExpectedException("action"),
                    }))
                .Concat(errorCases.Select( // concat error cases for m:function
                    ec => new PayloadReaderTestDescriptor(this.Settings)
                    {
                        PayloadElement = PayloadBuilder.Entity()
                                    .XmlRepresentation("<entry>" +
                                                        ec.Xml.Replace("$operation", "function") +
                                                        "</entry>"),
                        ExpectedException = ec.GetExpectedException("function"),
                    }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                ODataVersionUtils.AllSupportedVersions,
                this.ReaderTestConfigurationProvider.AtomFormatConfigurations,
                (testDescriptor, maxProtocolVersion, testConfiguration) =>
                {
                    if (maxProtocolVersion < testConfiguration.Version)
                    {
                        // This would fail anyway (no need to test it).
                        return;
                    }

                    if (testConfiguration.IsRequest)
                    {
                        testDescriptor = new PayloadReaderTestDescriptor(testDescriptor);
                        testDescriptor.ExpectedException = null;
                        testDescriptor.ExpectedResultNormalizers.Add(tc => RemoveOperationsNormalizer.Normalize);
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyMaxProtocolVersion(maxProtocolVersion));
                });
        }

        private class OperationReaderErrorTestCase
        {
            public string Xml { get; set; }
            public Func<string, ExpectedException> GetExpectedException { get; set; }
        }
    }
}
