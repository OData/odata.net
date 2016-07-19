using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
    }
}
