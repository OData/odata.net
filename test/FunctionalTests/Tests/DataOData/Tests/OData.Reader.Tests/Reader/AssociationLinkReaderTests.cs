//---------------------------------------------------------------------
// <copyright file="AssociationLinkReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.Test.Taupo.OData.Reader.Tests.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TestModels = Microsoft.Test.OData.Utils.Metadata.TestModels;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of association links (in deferred and expanded navigation links).
    /// </summary>
    [TestClass, TestCase]
    public class AssociationLinksReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public IPayloadGenerator PayloadGenerator { get; set; }

        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Links"), Variation(Description = "Test that the reading of association links in deferred and expanded navigation links.")]
        public void ReadAssociationLinkTest()
        {
            IEdmModel model = TestModels.BuildTestModel();

            // TODO: add a payload with a relative association Uri

            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Association link with nav. link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder
                        .NavigationProperty("NavPropWithAssociationUri", "http://odata.org/NavProp", "http://odata.org/NavPropWithAssociationUri")
                        .IsCollection(true),
                    PayloadEdmModel = model
                },

                // No need to add expanded nav links since those will be generated for us by the payload generator below.
            }.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td));

            // Association links with nav. link
            IEnumerable<PayloadReaderTestDescriptor> associationLinkTestDescriptors = new PayloadReaderTestDescriptor[]
            {
                // Association links without a nav. link
                // Association link for a singleton nav. property.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    PayloadEdmModel = model
                },
                // Association link for a collection nav. property.
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1),
                    PayloadEdmModel = model
                },
                // Association link which is not declared
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).Property(
                        PayloadBuilder.NavigationProperty("Nonexistant", null, "http://odata.org/CityHallLink").IsCollection(true)),
                    PayloadEdmModel = model,
                    ExpectedException = ODataExpectedExceptions.ODataException("ValidationUtils_PropertyDoesNotExistOnType", "Nonexistant", "TestModel.CityType")
                },
                // Association link which is not declared on an open type
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityOpenType")
                        .PrimitiveProperty("Id", 1)
                        .Property(
                            PayloadBuilder.NavigationProperty("Nonexistant", null, "http://odata.org/CityHallLink").IsCollection(true)),
                    PayloadEdmModel = model,
                    ExpectedResultCallback = 
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = null
                                },
                },
                // Association link which is declared but of wrong kind
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1).Property(
                        PayloadBuilder.NavigationProperty("Name", null, "http://odata.org/CityHallLink").IsCollection(true)),
                    PayloadEdmModel = model,
                    ExpectedResultCallback = 
                        (tc) => new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                                {
                                    ExpectedException = 
                                        (tc.Format == ODataFormat.Json)
                                        ? ODataExpectedExceptions.ODataException("ODataJsonLightResourceDeserializer_PropertyWithoutValueWithWrongType", "Name", "Edm.String")
                                        : ODataExpectedExceptions.ODataException("ValidationUtils_NavigationPropertyExpected", "Name", "TestModel.CityType", "Structural"),
                                },
                },
            };

            // Generate interesting payloads around the navigation property - this will skip failure cases like request payloads or wrong versions.
            testDescriptors = testDescriptors.Concat(associationLinkTestDescriptors.SelectMany(td => this.PayloadGenerator.GenerateReaderPayloads(td)));

            // Add the same cases again, but without skipping interesting configurations.
            testDescriptors = testDescriptors.Concat(associationLinkTestDescriptors.Select(td =>
                {
                    PayloadReaderTestDescriptor result = new PayloadReaderTestDescriptor(td);
                    var originalResultCallback = result.ExpectedResultCallback;
                    result.ExpectedResultCallback = tc =>
                        new PayloadReaderTestExpectedResult(this.Settings.ExpectedResultSettings)
                        {
                            ExpectedException = tc.IsRequest
                                ? null
                                : originalResultCallback == null ? result.ExpectedException : originalResultCallback(tc).ExpectedException,
                            ExpectedPayloadElement = tc.IsRequest
                                ? RemoveAssociationLinkPayloadElementNormalizer.Normalize(result.PayloadElement.DeepCopy())
                                : result.PayloadElement
                        };

                    // Setting the ExpectedResultCallback prevents normalizers from being run.
                    result.SkipTestConfiguration = tc => tc.Format == ODataFormat.Json;

                    return result;
                }));

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                ODataVersionUtils.AllSupportedVersions,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, maxProtocolVersion, testConfiguration) =>
                {
                    if (maxProtocolVersion < testConfiguration.Version)
                    {
                        return;
                    }

                    testDescriptor.RunTest(testConfiguration.CloneAndApplyMaxProtocolVersion(maxProtocolVersion));
                });
        }
        
    }
}