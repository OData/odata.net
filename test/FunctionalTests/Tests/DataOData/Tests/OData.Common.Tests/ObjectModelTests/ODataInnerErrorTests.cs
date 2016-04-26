//---------------------------------------------------------------------
// <copyright file="ODataInnerErrorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataInnerErrorTests object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataInnerErrorTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of ODataInnerError.")]
        public void DefaultValuesTest()
        {
            ODataInnerError innerError = new ODataInnerError();
            this.Assert.IsNull(innerError.Message, "Expected null default value for property 'Message'.");
            this.Assert.IsNull(innerError.TypeName, "Expected null default value for property 'TypeName'.");
            this.Assert.IsNull(innerError.StackTrace, "Expected null default value for property 'StackTrace'.");
            this.Assert.IsNull(innerError.InnerError, "Expected null default value for property 'InnerError'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataInnerError.")]
        public void PropertyGettersAndSettersTest()
        {
            string message = "Fehler! Bitte kontaktieren Sie den Administrator!";
            string typeName = "System.InvalidOperationException";
            string stackTrace = "Stack trace.";
            ODataInnerError innerInnerError = new ODataInnerError();

            ODataInnerError innerError = new ODataInnerError()
            {
                Message = message,
                TypeName = typeName,
                StackTrace = stackTrace,
                InnerError = innerInnerError
            };

            this.Assert.AreEqual(message, innerError.Message, "Expected equal message values.");
            this.Assert.AreEqual(typeName, innerError.TypeName, "Expected equal type name values.");
            this.Assert.AreEqual(stackTrace, innerError.StackTrace, "Expected equal stack trace values.");
            this.Assert.AreSame(innerInnerError, innerError.InnerError, "Expected reference equal inner error values.");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataInnerError innerError = new ODataInnerError()
                {
                    Message = "Fehler! Bitte kontaktieren Sie den Administrator!",
                    TypeName = "System.InvalidOperationException",
                    StackTrace = "Stack trace.",
                    InnerError = new ODataInnerError()
                };

            innerError.Message = null;
            innerError.TypeName = null;
            innerError.StackTrace = null;
            innerError.InnerError = null;

            this.Assert.IsNull(innerError.Message, "Expected null value for property 'Message'.");
            this.Assert.IsNull(innerError.TypeName, "Expected null value for property 'TypeName'.");
            this.Assert.IsNull(innerError.StackTrace, "Expected null value for property 'StackTrace'.");
            this.Assert.IsNull(innerError.InnerError, "Expected null value for property 'InnerError'.");
        }
    }
}
