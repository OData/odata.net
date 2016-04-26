//---------------------------------------------------------------------
// <copyright file="ODataActionsAndFunctionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataAction and ODataFunction object model types.
    /// </summary>
    [TestClass, TestCase]
    public class ODataActionsAndFunctionsTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of ODataAction and ODataFunction.")]
        public void DefaultValuesTest()
        {
            ODataAction action = new ODataAction();
            this.Assert.IsNull(action.Metadata, "Expected null default value for property 'Metadata'.");
            this.Assert.IsNull(action.Title, "Expected null default value for property 'Title'.");
            this.Assert.IsNull(action.Target, "Expected null default value for property 'Target'.");

            ODataFunction function = new ODataFunction();
            this.Assert.IsNull(function.Metadata, "Expected null default value for property 'Metadata'.");
            this.Assert.IsNull(function.Title, "Expected null default value for property 'Title'.");
            this.Assert.IsNull(function.Target, "Expected null default value for property 'Target'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataAction and ODataFunction.")]
        public void PropertyGettersAndSettersTest()
        {
            Uri metadata = new Uri("http://odata.org/operationMetadata");
            string title = "OperationTitle";
            Uri target = new Uri("http://odata.org/operationtarget");

            ODataAction action = new ODataAction()
            {
                Metadata = metadata,
                Title = title,
                Target = target,
            };

            this.Assert.AreSame(metadata, action.Metadata, "Expected reference equal values for property 'Metadata'.");
            this.Assert.AreEqual(title, action.Title, "Expected equal Title values.");
            this.Assert.AreSame(target, action.Target, "Expected reference equal values for property 'Target'.");

            ODataFunction function = new ODataFunction()
            {
                Metadata = metadata,
                Title = title,
                Target = target,
            };

            this.Assert.AreSame(metadata, function.Metadata, "Expected reference equal values for property 'Metadata'.");
            this.Assert.AreEqual(title, function.Title, "Expected equal Title values.");
            this.Assert.AreSame(target, function.Target, "Expected reference equal values for property 'Target'.");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            Uri metadata = new Uri("http://odata.org/operationMetadata");
            string title = "OperationTitle";
            Uri target = new Uri("http://odata.org/operationtarget");

            ODataAction action = new ODataAction()
            {
                Metadata = metadata,
                Title = title,
                Target = target,
            };

            action.Metadata = null;
            action.Title = null;
            action.Target = null;

            this.Assert.IsNull(action.Metadata, "Expected null value for property 'Metadata'.");
            this.Assert.IsNull(action.Title, "Expected null value for property 'Title'.");
            this.Assert.IsNull(action.Target, "Expected null value for property 'Target'.");

            ODataFunction function = new ODataFunction()
            {
                Metadata = metadata,
                Title = title,
                Target = target,
            };

            function.Metadata = null;
            function.Title = null;
            function.Target = null;

            this.Assert.IsNull(function.Metadata, "Expected null value for property 'Metadata'.");
            this.Assert.IsNull(function.Title, "Expected null value for property 'Title'.");
            this.Assert.IsNull(function.Target, "Expected null value for property 'Target'.");
        }
    }
}
