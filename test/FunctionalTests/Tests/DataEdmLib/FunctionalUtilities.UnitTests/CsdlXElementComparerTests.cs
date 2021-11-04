//---------------------------------------------------------------------
// <copyright file="CsdlXElementComparerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsdlXElementComparerTests
    {
        private CsdlXElementComparer comparer;

        public CsdlXElementComparerTests()
        {
            this.comparer = new CsdlXElementComparer();
        }

        [TestMethod]
        public void TheSameXElement_Should_Succeed()
        {
            XElement element1 = XElement.Parse(@"
<Foo Baz='A'>
</Foo>
");
            XElement element2 = XElement.Parse(@"
<Foo Baz='A'>
</Foo>
");
            this.CompareAndAssertSucceed(element1, element2);
        }

        [TestMethod]
        public void TotallyDifferentXElements_Should_Fail()
        {
            XElement element1 = XElement.Parse(@"
<EntityType Name='Person'>
</EntityType>
");
            XElement element2 = XElement.Parse(@"
<ComplexType Name='Address'>
</ComplexType>
");
            this.CompareAndAssertFail(element1, element2);
        }

        [TestMethod]
        public void DifferentAttributes_Should_Fail()
        {
            XElement element1 = XElement.Parse(@"
<EntityType Name='Person'>
</EntityType>
");
            XElement element2 = XElement.Parse(@"
<EntityType Name='NonPerson'>
</EntityType>
");
            this.CompareAndAssertFail(element1, element2);
        }

        [TestMethod]
        public void DifferentChildElements_Should_Fail()
        {
            XElement element1 = XElement.Parse(@"
<EntityType Name='Person'>
  <Extra1 />
</EntityType>
");
            XElement element2 = XElement.Parse(@"
<EntityType Name='Person'>
  <Extra2 />
</EntityType>
");
            this.CompareAndAssertFail(element1, element2);
        }

        [TestMethod]
        public void ExtraChildElement_Should_Fail()
        {
            XElement element1 = XElement.Parse(@"
<EntityType Name='Person'>
</EntityType>
");
            XElement element2 = XElement.Parse(@"
<EntityType Name='Person'>
  <Extra />
</EntityType>
");
            this.CompareAndAssertFail(element1, element2);
        }

        private void CompareAndAssertSucceed(XElement e1, XElement e2)
        {
            this.comparer.Compare(e1, e2);
        }

        private void CompareAndAssertFail(XElement e1, XElement e2)
        {
            try
            {
                this.comparer.Compare(e1, e2);
                Assert.Fail("Expected an error");
            }
            catch (Exception)
            {
            }
        }
    }
}
