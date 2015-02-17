//---------------------------------------------------------------------
// <copyright file="ODataErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using Microsoft.OData.Core;
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
            this.Assert.IsNull(error.ErrorCode, "Expected null default value for property 'ErrorCode'.");
            this.Assert.IsNull(error.Message, "Expected null default value for property 'Message'.");
            this.Assert.IsNull(error.InnerError, "Expected null default value for property 'InnerError'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataError.")]
        public void PropertyGettersAndSettersTest()
        {
            string errorCode = "500";
            string message = "Fehler! Bitte kontaktieren Sie den Administrator!";
            ODataInnerError innerError = new ODataInnerError { Message = "No inner error" };

            ODataError error = new ODataError()
            {
                ErrorCode = errorCode,
                Message = message,
                InnerError = innerError
            };

            this.Assert.AreEqual(errorCode, error.ErrorCode, "Expected equal error code values.");
            this.Assert.AreEqual(message, error.Message, "Expected equal message values.");
            this.Assert.AreSame(innerError, error.InnerError, "Expected equal inner error values.");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataError error = new ODataError()
                {
                    ErrorCode = "500",
                    Message = "Fehler! Bitte kontaktieren Sie den Administrator!",
                    InnerError = new ODataInnerError { Message = "No inner error" },
                };

            error.ErrorCode = null;
            error.Message = null;
            error.InnerError = null;

            this.Assert.IsNull(error.ErrorCode, "Expected null default value for property 'ErrorCode'.");
            this.Assert.IsNull(error.Message, "Expected null default value for property 'Message'.");
            this.Assert.IsNull(error.InnerError, "Expected null default value for property 'InnerError'.");
        }
    }
}
