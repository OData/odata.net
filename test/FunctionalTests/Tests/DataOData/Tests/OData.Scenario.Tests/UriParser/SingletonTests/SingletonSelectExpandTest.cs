//---------------------------------------------------------------------
// <copyright file="SingletonSelectExpandTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser
{
    using System.Runtime.CompilerServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SingletonSelectExpandTest : UriParserTestsBase
    {
        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SingleNavPropSelectExpandTest()
        {
            this.ApprovalVerifySelectAndExpandParser(specialOrderBase, "", "CustomerForOrder");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void TwoNapPropsExpanded()
        {
            this.ApprovalVerifySelectAndExpandParser(specialOrderBase, "", "CustomerForOrder,OrderDetails");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void ExpandWhenPathSelected()
        {
            this.ApprovalVerifySelectAndExpandParser(specialOrderBase, "", "CustomerForOrder($select=*)");
        }

        public void ExpandWhenItemPrimitiveAndNavPropSelected()
        {
            this.ApprovalVerifySelectAndExpandParser(specialOrderBase, "OrderDate,OrderDetails", "OrderDetails");
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void SelectPrimitiveOnExpandedNavProp()
        {
            this.ApprovalVerifySelectAndExpandParser(specialOrderBase, "", "OrderDetails($expand=ProductOrdered($select=QuantityInStock))");
        }

        

    }
}
