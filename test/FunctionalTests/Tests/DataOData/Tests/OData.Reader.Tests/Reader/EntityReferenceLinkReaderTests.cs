//---------------------------------------------------------------------
// <copyright file="EntityReferenceLinkReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.ODataLibTest;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests reading of association links (in deferred and expanded navigation links).
    /// </summary>
    [TestClass, TestCase]
    public class EntityReferenceLinkReaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        [TestMethod, TestCategory("Reader.Links"), Variation(Description = "Test the the reading of top-level (singleton) entity reference links.")]
        public void ReadEntityReferenceLinkTest()
        {
            // NOTE: No need for the payload generator here since entity reference links can only appear at the top level.
            IEnumerable<PayloadReaderTestDescriptor> testDescriptors = TestEntityReferenceLinks.CreateEntityReferenceLinkDescriptors(this.Settings);

            // make sure this also works with metadata
            EdmModel model = (EdmModel)Test.OData.Utils.Metadata.TestModels.BuildTestModel();
            EdmEntityType cityType = (EdmEntityType)model.FindDeclaredType("TestModel.CityType");
            EdmEntitySet citySet = (EdmEntitySet)model.EntityContainersAcrossModels().First().FindEntitySet("Cities");

            testDescriptors = testDescriptors.Concat(
                testDescriptors.Select(td =>
                    new PayloadReaderTestDescriptor(td)
                    {
                        PayloadElement = ((DeferredLink)td.PayloadElement.DeepCopy()).ExpectedNavigationProperty(citySet, cityType, "PoliceStation"),
                        PayloadEdmModel = model
                    }));

            // add some error cases
            IEnumerable<PayloadReaderTestDescriptor> errorDescriptors = new PayloadReaderTestDescriptor[]
            {
                // null link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.DeferredLink(null).ExpectedNavigationProperty(citySet, cityType, "PoliceStation"),
                    PayloadEdmModel = model,
                    ExpectedResultCallback = tc =>
                            new PayloadReaderTestExpectedResult (this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException = ODataExpectedExceptions.ODataException("ODataJsonLightEntityReferenceLinkDeserializer_EntityReferenceLinkUrlCannotBeNull")
                            },
                },

                // empty link
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.DeferredLink(string.Empty).ExpectedNavigationProperty(citySet, cityType, "PoliceStation"),
                    PayloadEdmModel = model,
                    ExpectedResultCallback = tc =>
                            new PayloadReaderTestExpectedResult (this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException = tc.Format == ODataFormat.Json
                                ? null
                                : ODataExpectedExceptions.ODataException("ODataJsonDeserializer_RelativeUriUsedWithoutBaseUriSpecified")
                            },
                },

                // invalid Uri format
                new PayloadReaderTestDescriptor(this.Settings)
                {
                    PayloadElement = PayloadBuilder.DeferredLink("foo-is-not-a-link").ExpectedNavigationProperty(citySet, cityType, "PoliceStation"),
                    PayloadEdmModel = model,
                    ExpectedResultCallback = tc =>
                            new PayloadReaderTestExpectedResult (this.Settings.ExpectedResultSettings)
                            {
                                ExpectedException = tc.Format == ODataFormat.Json
                                ? null
                                : ODataExpectedExceptions.ODataException("ODataJsonDeserializer_RelativeUriUsedWithoutBaseUriSpecified")
                            },
                },
            };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors.Concat(errorDescriptors),
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    if (testConfiguration.Format == ODataFormat.Json && testDescriptor.PayloadEdmModel == null)
                    {
                        // Ignore test cases without model in JSON Light
                        return;
                    }

                    ReaderTestConfiguration testConfigClone = new ReaderTestConfiguration(testConfiguration);
                    if (testConfiguration.Format == ODataFormat.Json)
                    {
                        testConfigClone.MessageReaderSettings.BaseUri = null;
                    }

                    testDescriptor.RunTest(testConfigClone);
                });
        }
    }
}
