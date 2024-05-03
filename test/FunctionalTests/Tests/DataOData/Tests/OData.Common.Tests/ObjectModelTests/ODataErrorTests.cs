//---------------------------------------------------------------------
// <copyright file="ODataErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataErrorTests object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataErrorTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of ODataError.")]
        public void DefaultValuesTest()
        {
            ODataError error = new ODataError();
            this.Assert.IsNull(error.Code, "Expected null default value for property 'Code'.");
            this.Assert.IsNull(error.Message, "Expected null default value for property 'Message'.");
            this.Assert.IsNull(error.Target, "Expected null default value for property 'Target'.");
            this.Assert.IsNull(error.Details, "Expected null default value for property 'Details'.");
            this.Assert.IsNull(error.InnerError, "Expected null default value for property 'InnerError'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataError.")]
        public void PropertyGettersAndSettersTest()
        {
            string errorCode = "500";
            string message = "Fehler! Bitte kontaktieren Sie den Administrator!";
            var target = "any target";
            var details = new List<ODataErrorDetail>
            {
                new ODataErrorDetail { Code = "401", Message = "any msg", Target = "another target" }
            };
            ODataInnerError innerError = new ODataInnerError { Message = "No inner error" };

            ODataError error = new ODataError()
            {
                Code = errorCode,
                Message = message,
                Target = target,
                Details = details,
                InnerError = innerError
            };

            this.Assert.AreEqual(errorCode, error.Code, "Expected equal error code values.");
            this.Assert.AreEqual(message, error.Message, "Expected equal message values.");
            this.Assert.AreEqual(target, error.Target, "Expected equal target values.");
            this.Assert.AreSame(details, error.Details, "Expected equal error detail values.");
            this.Assert.AreSame(innerError, error.InnerError, "Expected equal inner error values.");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataError error = new ODataError()
            {
                Code = "500",
                Message = "Fehler! Bitte kontaktieren Sie den Administrator!",
                Target = "any target",
                Details =
                    new List<ODataErrorDetail>
                    {
                        new ODataErrorDetail { Code = "401", Message = "any msg", Target = "another target" }
                    },
                InnerError = new ODataInnerError { Message = "No inner error" },
            };

            error.Code = null;
            error.Message = null;
            error.Target = null;
            error.Details = null;
            error.InnerError = null;

            this.Assert.IsNull(error.Code, "Expected null default value for property 'Code'.");
            this.Assert.IsNull(error.Message, "Expected null default value for property 'Message'.");
            this.Assert.IsNull(error.Target, "Expected null default value for property 'Target'.");
            this.Assert.IsNull(error.Details, "Expected null default value for property 'Details'.");
            this.Assert.IsNull(error.InnerError, "Expected null default value for property 'InnerError'.");
        }
    }
}
