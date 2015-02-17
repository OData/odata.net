//---------------------------------------------------------------------
// <copyright file="ResourceSetPathExpressionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    #region Namespaces.
    using System;
    using Microsoft.OData.Service.Providers;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces.

    /// <summary>
    /// Tests for the ResourceSetPathExpression class.
    /// </summary>
    [TestClass, TestCase]
    public class ResourceSetPathExpressionTests
    {
        [TestMethod, Variation("Verifies that the constructor fails when received bad arguments.")]
        public void ConstructorNegativeTest()
        {
            ExceptionUtils.ExpectedException<ArgumentException>(
                () => new ResourceSetPathExpression(null),
                "Value cannot be null or empty.\r\nParameter name: pathExpression",
                "Cannot use null or empty path expression.");
            
            ExceptionUtils.ExpectedException<ArgumentException>(
                () => new ResourceSetPathExpression(string.Empty),
                "Value cannot be null or empty.\r\nParameter name: pathExpression",
                "Cannot use null or empty path expression.");
        }

        [TestMethod, Variation("Verifies the PathExpression property returns the correct value.")]
        public void PathExpressionTest()
        {
            var expression = new ResourceSetPathExpression("param1");
            Assert.AreEqual("param1", expression.PathExpression, "Invalid value in PathExpression.");

            expression = new ResourceSetPathExpression("param1/P1");
            Assert.AreEqual("param1/P1", expression.PathExpression, "Invalid value in PathExpression.");

            expression = new ResourceSetPathExpression("param1/P1/P2");
            Assert.AreEqual("param1/P1/P2", expression.PathExpression, "Invalid value in PathExpression.");

            expression = new ResourceSetPathExpression("param1/FQNS.T1/P1/FQNS.T2/P2");
            Assert.AreEqual("param1/FQNS.T1/P1/FQNS.T2/P2", expression.PathExpression, "Invalid value in PathExpression.");
        }
    }
}
