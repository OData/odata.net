//---------------------------------------------------------------------
// <copyright file="ODataUtilsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.InfrastructureTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.Metadata;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the OData utility class.
    /// </summary>
    [TestClass, TestCase]
    public class ODataUtilsTests : ODataTestCase
    {
        //version strings
        private string DataServiceVersion4 = "4.0";
        private string DataServiceVersion4_01 = "4.01";

        /// <summary>
        /// Gets or sets the exception verifier.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IExceptionVerifier ExceptionVerifier { get; set; }

        /// <summary>
        /// The combinatorial engine to use in matrix based tests.
        /// </summary>
        [InjectDependency]
        public ICombinatorialEngineProvider CombinatorialEngineProvider { get; set; }

        /// <summary>
        /// Converter from an entity model to schema to an EDM model.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EntityModelSchemaToEdmModelConverter EntityModelSchemaToEdmModelConverter { get; set; }

        /// <summary>
        /// Tests for the ODataVersion utility methods.
        /// </summary>
        [TestMethod, Variation]
        public void GetVersionUtilTest()
        {

            //positive tests
            this.Assert.AreEqual(ODataUtils.StringToODataVersion(DataServiceVersion4), ODataVersion.V4,
                "Failed to parse Data Service Version string");

            this.Assert.AreEqual(ODataUtils.StringToODataVersion(DataServiceVersion4 + ";"), ODataVersion.V4,
                "Failed to parse Data Service Version string");

            this.Assert.AreEqual(ODataUtils.StringToODataVersion(DataServiceVersion4 + ";anything"), ODataVersion.V4,
                "Failed to parse Data Service Version string");

            this.Assert.AreEqual(ODataUtils.ODataVersionToString(ODataVersion.V4), DataServiceVersion4,
                "Failed to parse Data Service Version enum");

            this.Assert.AreEqual(ODataUtils.StringToODataVersion(DataServiceVersion4_01), ODataVersion.V401,
                "Failed to parse Data Service Version string");

            this.Assert.AreEqual(ODataUtils.StringToODataVersion(DataServiceVersion4_01 + ";"), ODataVersion.V401,
                "Failed to parse Data Service Version string");

            this.Assert.AreEqual(ODataUtils.StringToODataVersion(DataServiceVersion4_01 + ";anything"), ODataVersion.V401,
                "Failed to parse Data Service Version string");

            this.Assert.AreEqual(ODataUtils.ODataVersionToString(ODataVersion.V401), DataServiceVersion4_01,
                "Failed to parse Data Service Version enum");

            //negative tests
            string[] invalidVersionStrings = {"randomstring", "V1.0", "1.5", "randomstring;1.0", "5.0", "1"};
            foreach (string s in invalidVersionStrings)
                TestExceptionUtils.ExpectedException(
                    this.Assert,
                    () =>
                        {
                            ODataUtils.StringToODataVersion(s);
                        },
                    ODataExpectedExceptions.ODataException("ODataUtils_UnsupportedVersionHeader", s),
                    this.ExceptionVerifier
                    );
        }
    }
}
