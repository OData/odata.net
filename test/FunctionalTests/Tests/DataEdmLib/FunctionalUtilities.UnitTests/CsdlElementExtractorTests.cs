//---------------------------------------------------------------------
// <copyright file="CsdlElementExtractorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities.UnitTests
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CsdlElementExtractorTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoInput_Should_Throw()
        {
            CsdlElementExtractor.ExtractElementByName(Enumerable.Empty<XElement>(), "EntityContainer");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NullInput_Should_Throw()
        {
            CsdlElementExtractor.ExtractElementByName(null, "EntityContainer");
        }

        [TestMethod]
        public void Containers_Should_BeExtracted()
        {
            XElement input1 = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <EntityType Name='Person' IsAbstract='true' other:Name='foobaz' ZZZ='zzz'>
    <Property Type='Int32' Name='Id' />
  </EntityType>
  <EntityContainer Name='a' />
  <EntityContainer Name='b' />
</Schema>
");
            XElement input2 = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns'>
  <EntityContainer Name='c' />
</Schema>
");

            XElement actual = CsdlElementExtractor.ExtractElementByName(new XElement[] { input1, input2 }, "EntityContainer");

            XElement expectedResult = XElement.Parse(@"
<Schema Namespace='ExtractedElements' xmlns='fakeEdmXmlns'>
  <EntityContainer Name='a' />
  <EntityContainer Name='b' />
  <EntityContainer Name='c' />
</Schema>
");
            Assert.AreEqual(expectedResult.ToString(), actual.ToString());

            XElement expectedInput1 = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <EntityType Name='Person' IsAbstract='true' other:Name='foobaz' ZZZ='zzz'>
    <Property Type='Int32' Name='Id' />
  </EntityType>
</Schema>
");
            Assert.AreEqual(expectedInput1.ToString(), input1.ToString());

            XElement expectedInput2 = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' />
");
            Assert.AreEqual(expectedInput2.ToString(), input2.ToString());
        }

        [TestMethod]
        public void NoContainers_Should_ReturnEmpty()
        {
            XElement input = XElement.Parse(@"
<Schema xmlns='fakeEdmXmlns' xmlns:other='annotationXmlns'>
  <EntityType Name='Person' IsAbstract='true' other:Name='foobaz' ZZZ='zzz'>
    <Property Type='Int32' Name='Id' />
  </EntityType>
</Schema>
");
            XElement copyOfInput = new XElement(input);

            XElement actual = CsdlElementExtractor.ExtractElementByName(new XElement[] { input }, "EntityContainer");

            XElement expectedResult = XElement.Parse(@"
<Schema Namespace='ExtractedElements' xmlns='fakeEdmXmlns' />
");
            Assert.AreEqual(expectedResult.ToString(), actual.ToString());

            Assert.AreEqual(copyOfInput.ToString(), input.ToString());
        }
    }
}
