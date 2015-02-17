//---------------------------------------------------------------------
// <copyright file="ExpressionTreeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Data.Test.Astoria;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Xml.Linq;
    using Microsoft.OData.Service;
    using System.Linq.Expressions;
    
    [TestModule]
    public partial class UnitTestModule : AstoriaTestModule
    {

        // The test hook is only available in debug mode!
        [TestClass, TestCase]
        /// <summary>This is a test class for querying functionality.</summary>
        public class ExpressionTreeTests : AstoriaTestCase
        {
            public static IQueryable LastQueryable;
#if DEBUG
            [TestMethod, Variation("Test the functionality of the IQueryable hook")]
            public void ExpressionHookTest()
            {
                // This is just a constant expression with the resultant array
                var q = ExpressionTreeTestUtils.CreateRequestAndGetQueryable(typeof(CustomDataContext), "/Customers").Expression;
                Assert.AreEqual(q.NodeType, ExpressionType.Constant);

                // this is a call to IQueryable.Where
                q = ExpressionTreeTestUtils.CreateRequestAndGetQueryable(typeof(CustomDataContext), "/Customers?$filter=ID eq 0").Expression;
                Assert.AreEqual(q.NodeType, ExpressionType.Call);

                XmlDocument xDoc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomDataContext), "/Customers?$filter=ID eq 0");
                UnitTestsUtil.VerifyXPaths(xDoc,
                    "/Call[Method='Where']",
                    "/Call/Arguments/Constant[position()=1]");

                using (TestUtil.RestoreStaticValueOnDispose(typeof(ExpressionTreeToXmlSerializer), "UseFullyQualifiedTypeNames"))
                {
                    xDoc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomDataContext), "/Customers?$filter=isof('AstoriaUnitTests.Stubs.Customer')");
                    UnitTestsUtil.VerifyXPaths(xDoc,
                        "/Call/Arguments/Quote/Lambda/Body/TypeIs[TypeOperand='Customer']");

                    ExpressionTreeToXmlSerializer.UseFullyQualifiedTypeNames = true;
                    xDoc = ExpressionTreeTestUtils.CreateRequestAndGetExpressionTreeXml(typeof(CustomDataContext), "/Customers?$filter=isof('AstoriaUnitTests.Stubs.Customer')");
                    UnitTestsUtil.VerifyXPaths(xDoc,
                        "/Call/Arguments/Quote/Lambda/Body/TypeIs[TypeOperand='AstoriaUnitTests.Stubs.Customer']");
                }
            }

            [TestMethod, Variation("Test the functionality of the IQueryable hook for BasicExpandProvider")]
            public void ExpressionExpandProviderHookTest()
            {
                using (TestUtil.MetadataCacheCleaner())
                using (OpenWebDataServiceHelper.PageSizeCustomizer.Restore())
                {
                    OpenWebDataServiceHelper.PageSizeCustomizer.Value = (config, type) =>
                    {
                        config.SetEntitySetAccessRule("*", EntitySetRights.All);
                        config.SetEntitySetPageSize("Customers", 2);
                    };

                    var expanded = ExpressionTreeTestUtils.CreateRequestAndGetQueryable(typeof(CustomDataContext), "/Customers?$expand=Orders&$orderby=ID add 1").Expression;
                    // We had to wrap for $skiptoken in this case
                    Assert.IsTrue(expanded.Type.ToString().StartsWith("System.Linq.IQueryable`1[Microsoft.OData.Service.Internal.ExpandedWrapper`3"));
                }
            }
#endif
        }
    }
}
