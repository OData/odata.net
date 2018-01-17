using System;
using System.Runtime.CompilerServices;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Test.Taupo.OData.Scenario.Tests.UriParser.MultiBindingTests
{
    [TestClass]
    public class MultiBindingTests : UriParserTestsBase
    {
        private readonly Uri entitySetBase = new Uri("http://host/EntitySet");
        private readonly Uri complexProp1Base = new Uri("http://host/EntitySet('abc')/complexProp1");
        private readonly Uri navUnderComplexBase = new Uri("http://host/EntitySet('abc')/complexProp1/CollectionOfNavOnComplex");
        private readonly Uri containedNav1Base = new Uri("http://host/EntitySet('abc')/ContainedNav1");
        private readonly IEdmModel bindigModel = MultiBindingModel.GetModel();

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void NestedExpandUnderComplex()
        {
            // http://host/EntitySet?$expand=complexProp1/CollectionOfNavOnComplex($expand=NavNested)
            this.ApprovalVerifyExpandParser(entitySetBase, "complexProp1/CollectionOfNavOnComplex($expand=NavNested)", bindigModel);
            this.TestAllInOneExtensionExpand(entitySetBase, "complexProp1/collectionofnavoncomplex($expand=NavNested)", "complexProp1/CollectionOfNavOnComplex($expand=NavNested)", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void NestedExpandUnderSplitComplex()
        {
            // http://host/EntitySet('abc')/complexProp1?$expand=CollectionOfNavOnComplex($expand=NavNested)
            this.ApprovalVerifyExpandParser(complexProp1Base, "CollectionOfNavOnComplex($expand=NavNested)", bindigModel);
            this.TestAllInOneExtensionExpand(complexProp1Base, "CollectionOfNavOnComplex($expand=navnested)", "CollectionOfNavOnComplex($expand=NavNested)", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void ExpandNested()
        {
            // http://host/EntitySet('abc')/complexProp1/CollectionOfNavOnComplex?$expand=NavNested
            this.ApprovalVerifyExpandParser(navUnderComplexBase, "NavNested", bindigModel);
            this.TestAllInOneExtensionExpand(navUnderComplexBase, "navNested", "NavNested", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void OrderByContained()
        {
            // http://host/EntitySet('abc')?$orderby=ContainedNav1/NavOnContained/ID
            this.ApprovalVerifyOrderByParser(entitySetBase, "ContainedNav1/NavOnContained/ID", bindigModel);
            this.TestAllInOneExtensionOrderBy(entitySetBase, "containednav1/NavOnContained/ID", "ContainedNav1/NavOnContained/ID", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void FilterContained()
        {
            // http://host/EntitySet('abc')/ContainedNav1?$filter=NavOnContained/ID eq 'abc'
            this.ApprovalVerifyFilterParser(containedNav1Base, "NavOnContained/ID eq 'abc'", bindigModel);
            this.TestAllInOneExtensionFilter(containedNav1Base, "navOnContained/ID eq 'abc'", "NavOnContained/ID eq 'abc'", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void FilterOnNavigationUnderComplex()
        {
            // http://host/EntitySet?$filter=complexProp2/CollectionOfNavOnComplex/any(t:t/ID eq 'abc')
            this.ApprovalVerifyFilterParser(entitySetBase, "complexProp1/CollectionOfNavOnComplex/any(t:t/ID eq 'abc')", bindigModel);
            this.TestAllInOneExtensionFilter(entitySetBase, "complexProp1/collectionofnavOnComplex/any(t:t/ID eq 'abc')", "complexProp1/CollectionOfNavOnComplex/any(t:t/ID eq 'abc')", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void FilterOnNavigationUnderComplexAndComplexIsInPath()
        {
            // http://host/EntitySet/complexProp1?$filter=CollectionOfNavOnComplex/any(t:t/ID eq 'abc')
            this.ApprovalVerifyFilterParser(complexProp1Base, "CollectionOfNavOnComplex/any(t:t/ID eq 'abc')", bindigModel);
            this.TestAllInOneExtensionFilter(complexProp1Base, "collectionofNavOnComplex/any(t:t/ID eq 'abc')", "CollectionOfNavOnComplex/any(t:t/ID eq 'abc')", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void OrderByOnNavigationUnderComplex()
        {
            // http://host/EntitySet?$orderby=complexProp1/CollectionOfNavOnComplex/$count
            this.ApprovalVerifyOrderByParser(entitySetBase, "complexProp1/CollectionOfNavOnComplex/$count", bindigModel);
            this.TestAllInOneExtensionOrderBy(entitySetBase, "complexprop1/CollectionOfNavOnComplex/$count", "complexProp1/CollectionOfNavOnComplex/$count", bindigModel);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void OrderByOnNavigationUnderComplexAndComplexIsInPath()
        {
            // http://host/EntitySet('abc')/complexProp1?$orderby=CollectionOfNavOnComplex/$count
            this.ApprovalVerifyOrderByParser(complexProp1Base, "CollectionOfNavOnComplex/$count", bindigModel);
            this.TestAllInOneExtensionOrderBy(complexProp1Base, "collectionOfnavOnComplex/$count", "CollectionOfNavOnComplex/$count", bindigModel);
        }
    }
}
