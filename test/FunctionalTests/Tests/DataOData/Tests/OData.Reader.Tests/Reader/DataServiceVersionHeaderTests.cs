//---------------------------------------------------------------------
// <copyright file="DataServiceVersionHeaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Reader
{
    #region Namespaces
    using System;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Reader.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests different values for the DataServiceVersion (DSV) header.
    /// </summary>
    [TestClass, TestCase]
    public class DataServiceVersionHeaderTests : ODataReaderTestCase
    {
        [InjectDependency]
        public PayloadReaderTestDescriptor.Settings Settings { get; set; }

        
        [TestMethod, TestCategory("Reader.DataServiceVersion"), Variation(Description = "Tests the correct behavior of the reader with various DSV values and payloads.")]
        public void DataServiceVersionHeaderTest()
        {
            EdmModel model = (EdmModel)Test.OData.Utils.Metadata.TestModels.BuildTestModel();

            var entry = PayloadBuilder.Entity("TestModel.CityType").PrimitiveProperty("Id", 1)
                .ExpandedNavigationProperty("CityHall", PayloadBuilder.EntitySet());

            DataServiceVersionTestDescriptor[] testDescriptors = new DataServiceVersionTestDescriptor[]
                {
                    // valid entry with DSV 4.0
                    new DataServiceVersionTestDescriptor(this.Settings)
                    {
                        PayloadElement = entry,
                        PayloadEdmModel = model,
                        DataServiceVersion = ODataVersion.V4.ToText(),
                        SkipTestConfiguration = tc => tc.Version != ODataVersion.V4,
                    },
                    // invalid DSV (negative number)
                    new DataServiceVersionTestDescriptor(this.Settings)
                    {
                        PayloadElement = entry,
                        PayloadEdmModel = model,
                        DataServiceVersion = "-1.0",
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", "-1.0"),
                    },
                    // invalid DSV (unsupported positive number)
                    new DataServiceVersionTestDescriptor(this.Settings)
                    {
                        PayloadElement = entry,
                        PayloadEdmModel = model,
                        DataServiceVersion = "12345.0",
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", "12345.0"),
                    },
                    // invalid DSV (NaN)
                    new DataServiceVersionTestDescriptor(this.Settings)
                    {
                        PayloadElement = entry,
                        PayloadEdmModel = model,
                        DataServiceVersion = "foo",
                        ExpectedException = ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", "foo"),
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testDescriptors,
                this.ReaderTestConfigurationProvider.ExplicitFormatConfigurations,
                (testDescriptor, testConfiguration) =>
                {
                    testDescriptor.RunTest(testConfiguration);
                });
        }

        /// <summary>
        /// A test descriptor based on the <see cref="PayloadReaderTestDescriptor"/> that overwrites the
        /// DataServiceVersion header for test purposes.
        /// </summary>
        private sealed class DataServiceVersionTestDescriptor : PayloadReaderTestDescriptor
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="settings">Settings class to use.</param>
            public DataServiceVersionTestDescriptor(Settings settings)
                : base(settings)
            {
            }

            /// <summary>The DSV to set into the headers.</summary>
            public string DataServiceVersion
            {
                get;
                set;
            }

            /// <summary>
            /// Called to create the input message for the reader test. Replaces the default DSV header with the
            /// value specified in the constructor.
            /// </summary>
            /// <param name="testConfiguration">The test configuration.</param>
            /// <returns>The newly created test message to use.</returns>
            protected override TestMessage CreateInputMessage(ReaderTestConfiguration testConfiguration)
            {
                TestMessage testMessage = base.CreateInputMessage(testConfiguration);

                // now overwrite the DSV header
                testMessage.SetHeader(Microsoft.OData.ODataConstants.ODataVersionHeader, this.DataServiceVersion);

                return testMessage;
            }
        }
    }
}
