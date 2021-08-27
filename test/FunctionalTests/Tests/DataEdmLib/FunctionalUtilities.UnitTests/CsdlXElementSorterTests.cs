//---------------------------------------------------------------------
// <copyright file="CsdlXElementSorterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsdlXElementSorterTests
    {
        [TestMethod]
        public void ElementInOtherNamespaces_ShouldNotBeSorted()
        {
            XElement original = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <other:EntityType Name='Person' IsAbstract='true' other:Name='foobaz' ZZZ='zzz'>
    <other:Property Type='Int32' Name='Id' />
    <other:Property Type='Int32' Name='AAA' />
  </other:EntityType>
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <other:EntityType Name='Person' IsAbstract='true' other:Name='foobaz' ZZZ='zzz'>
    <other:Property Type='Int32' Name='Id' />
    <other:Property Type='Int32' Name='AAA' />
  </other:EntityType>
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortDocumentation()
        {
            XElement original = XElement.Parse(@"
<Documentation>
  <Summary />
  <LongDescription />
</Documentation>
");
            XElement expected = XElement.Parse(@"
<Documentation>
  <Summary />
  <LongDescription />
</Documentation>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortDocumentation_WrongOrder()
        {
            XElement original = XElement.Parse(@"
<Documentation>
  <LongDescription />
  <Summary />
</Documentation>
");
            XElement expected = XElement.Parse(@"
<Documentation>
  <LongDescription />
  <Summary />
</Documentation>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortAttributes()
        {
            XElement original = XElement.Parse(@"
<EntityType Name='Person' BaseType='Foo' IsAbstract='true'>
  <Property Name='Id' Type='Int32' />
</EntityType>
");
            XElement expected = XElement.Parse(@"
<EntityType BaseType='Foo' IsAbstract='true' Name='Person'>
  <Property Name='Id' Type='Int32' />
</EntityType>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortAttributes_WithAttributeAnnoation()
        {
            XElement original = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <EntityType Name='Person' IsAbstract='true' other:Name='foobaz' ZZZ='zzz'>
    <Property Type='Int32' Name='Id' />
  </EntityType>
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <EntityType IsAbstract='true' Name='Person' ZZZ='zzz' other:Name='foobaz'>
    <Property Name='Id' Type='Int32' />
  </EntityType>
</Schema>
");            
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortSchemaElements()
        {
            XElement original = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <EntityContainer />
  <ComplexType />
  <EntityType />
  <Function />
  <other:ZZZ />
  <other:Function />
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <ComplexType />
  <EntityContainer />
  <EntityType />
  <Function />
  <other:ZZZ />
  <other:Function />
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortSchemaElements_WrongOrder()
        {
            XElement original = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <other:ZZZ />
  <EntityType />
  <EntityContainer />
  <other:Function />
  <ComplexType />
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <other:ZZZ />
  <EntityContainer />
  <EntityType />
  <other:Function />
  <ComplexType />
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortComplexType()
        {
            XElement original = XElement.Parse(@"
<Schema>
  <ComplexType Name='b'>
    <Documentation />
    <Property Name='cc' />
    <Property Name='bb' />
  </ComplexType>
  <ComplexType Name='a' />
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema>
  <ComplexType Name='a' />
  <ComplexType Name='b'>
    <Documentation />
    <Property Name='bb' />
    <Property Name='cc' />
  </ComplexType>
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortEntityType()
        {
            XElement original = XElement.Parse(@"
<Schema>
  <EntityType Name='b'>
    <Documentation />
    <Key>
      <PropertyRef Name='cc' />
      <PropertyRef Name='aa' />
    </Key>
    <Property Name='cc' />
    <NavigationProperty Name='ee' />
    <Property Name='bb' />
    <NavigationProperty Name='dd' />
  </EntityType>
  <EntityType Name='a' />
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema>
  <EntityType Name='a' />
  <EntityType Name='b'>
    <Documentation />
    <Key>
      <PropertyRef Name='aa' />
      <PropertyRef Name='cc' />
    </Key>
    <NavigationProperty Name='dd' />
    <NavigationProperty Name='ee' />
    <Property Name='bb' />
    <Property Name='cc' />
  </EntityType>
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortEntityType_WrongOrder()
        {
            XElement original = XElement.Parse(@"
<Schema>
  <EntityType Name='b'>
    <Property Name='cc' />
    <Key />
    <Property Name='bb' />
    <Documentation />
    <NavigationProperty Name='dd' />
  </EntityType>
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema>
  <EntityType Name='b'>
    <Property Name='cc' />
    <Key />
    <Property Name='bb' />
    <Documentation />
    <NavigationProperty Name='dd' />
  </EntityType>
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortFunction()
        {
            XElement original = XElement.Parse(@"
<Schema>
  <Function Name='b'>
    <Documentation />
    <Parameter Name='zz' />
    <Parameter Name='cc' />
    <ReturnType />
  </Function>
  <Function Name='a' />
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema>
  <Function Name='a' />
  <Function Name='b'>
    <Documentation />
    <Parameter Name='cc' />
    <Parameter Name='zz' />
    <ReturnType />
  </Function>
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        [TestMethod]
        public void SortUnknownChild_shouldKeepOriginalOrder()
        {
            XElement original = XElement.Parse(@"
<Schema>
  <EntityContainer Name='1' />
  <Unknown Name='1' />
  <EntityContainer Name='4' />
  <EntityContainer Name='2' />
  <Unknown Name='2'>
    <Giberish Order='1' />
    <Giberish Order='2' />
  </Unknown>
  <Unknown Name='3' />
  <Unknown Name='4' />
  <EntityContainer Name='5' />
</Schema>
");
            XElement expected = XElement.Parse(@"
<Schema>
  <EntityContainer Name='1' />
  <Unknown Name='1' />
  <EntityContainer Name='2' />
  <EntityContainer Name='4' />
  <Unknown Name='2'>
    <Giberish Order='1' />
    <Giberish Order='2' />
  </Unknown>
  <Unknown Name='3' />
  <Unknown Name='4' />
  <EntityContainer Name='5' />
</Schema>
");
            this.SortAndCompareToExpected(original, expected);
        }

        private void SortAndCompareToExpected(XElement original, XElement expected)
        {
            XElement actual = new CsdlXElementSorter().SortCsdl(original);

            Assert.AreEqual(expected.ToString(), actual.ToString());
        }
    }
}
