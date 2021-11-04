//---------------------------------------------------------------------
// <copyright file="BufferingXmlReaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

    // These tests use Reflection to create & test internal product types,
    // (in the test wrapper of BufferingXmlReader) which cannot be done in Silverlight/Phone. 
    // Running these unit tests on desktop only is sufficient.

namespace Microsoft.Test.Taupo.OData.Reader.Tests.Atom
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for BufferingXmlReader class.
    /// </summary>
    [TestClass, TestCase]
    public class BufferingXmlReaderTests : ODataReaderTestCase
    {
        private static string TestXml =
@"<?xml version='1.0' encoding='utf-8'?>
<root xmlns='atomuri' xml:base='http://odata.org/base' expectedXmlBase='http://odata.org/base'>
  <child/>
  <child xml:base='relative' expectedXmlBase='http://odata.org/relative'/>
  <child>textvalue</child>
  <d:differentchild xmlns:d='duri1'>
    <d:subchild xmlns:d='duri2'/>
  </d:differentchild>
  <text>
  </text>
  <attribs xmlns='attruri' a:a='1' xmlns:a='attruri2' a:b='3' c=''></attribs>
  <?pi value?>
  <preserve xml:space='preserve'>   bar   </preserve>
  <!-- comment -->
  <![CDATA[cdata]]>
  <deep>
    <deep1 xml:base='http://odata2.org/newbase/' expectedXmlBase='http://odata2.org/newbase/'>
      <deep2>
        <deep3>
          <deep4 xml:base='relative' expectedXmlBase='http://odata2.org/newbase/relative'>foo</deep4>
          <deep4></deep4>
          <deep4 xml:base='relative2' expectedXmlBase='http://odata2.org/newbase/relative2'/>
        </deep3>
      </deep2>
      <deep2>
        <deep3></deep3>
      </deep2>
    </deep1>
  </deep>
</root>";

        private Stack<Uri> expectedXmlBaseUriStack = new Stack<Uri>();

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that non-buffering reading works.")]
        public void NonBufferingReadingTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new bool[] { false, true },
                new bool[] { false, true },
                (moveToElementBeforeRead, verifyAttributes) =>
                {
                    using (XmlReader baselineReader = XmlReader.Create(new StringReader(TestXml)))
                    {
                        this.expectedXmlBaseUriStack.Clear();
                        using (XmlReader comparedReader = XmlReader.Create(new StringReader(TestXml)))
                        {
                            BufferingXmlReader bufferingXmlReader = new BufferingXmlReader(
                                comparedReader,
                                /*parentXmlReader*/ null,
                                /*documentBaseUri*/ null,
                                /*disableXmlBase*/ false,
                                ODataConstants.DefaultMaxRecursionDepth,
                                this.Assert);

                            this.VerifyReadersAreEqual(baselineReader, bufferingXmlReader, Int32.MaxValue, moveToElementBeforeRead, verifyAttributes);
                        }
                    }
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that buffering reading works.")]
        public void BufferingReadingTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new bool[] { false, true },
                new bool[] { false, true },
                (moveToElementBeforeRead, verifyAttributes) =>
                {
                    using (XmlReader baselineReader = XmlReader.Create(new StringReader(TestXml)))
                    {
                        this.expectedXmlBaseUriStack.Clear();
                        using (XmlReader comparedReader = XmlReader.Create(new StringReader(TestXml)))
                        {
                            BufferingXmlReader bufferingXmlReader = new BufferingXmlReader(
                                comparedReader,
                                /*parentXmlReader*/ null,
                                /*documentBaseUri*/ null,
                                /*disableXmlBase*/ false,
                                ODataConstants.DefaultMaxRecursionDepth, 
                                this.Assert);
                            bufferingXmlReader.StartBuffering();

                            this.VerifyReadersAreEqual(baselineReader, bufferingXmlReader, Int32.MaxValue, moveToElementBeforeRead, verifyAttributes);
                        }
                    }
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that ReadInnerXml works.")]
        public void ReadInnerXmlTest()
        {
            var testCases = new[]
                {
                    new {
                        Xml = TestXml,
                        InterestingElementName = "deep"
                    },
                    new {
                        Xml = "<foo/>",
                        InterestingElementName = "foo"
                    },
                    new {
                        Xml = "<foo>some text node</foo>",
                        InterestingElementName = "foo"
                    },
                };

            this.CombinatorialEngineProvider.RunCombinations(
                testCases,
                testCase =>
                {
                    using (XmlReader baselineReader = XmlReader.Create(new StringReader(testCase.Xml)))
                    {
                        this.expectedXmlBaseUriStack.Clear();
                        using (XmlReader comparedReader = XmlReader.Create(new StringReader(testCase.Xml)))
                        {
                            BufferingXmlReader bufferingXmlReader = new BufferingXmlReader(
                                comparedReader,
                                /*parentXmlReader*/ null,
                                /*documentBaseUri*/ null,
                                /*disableXmlBase*/ false,
                                ODataConstants.DefaultMaxRecursionDepth,
                                this.Assert);

                            while (true)
                            {
                                this.VerifyReadersAreEqual(baselineReader, bufferingXmlReader, 1, false, false);
                                if (testCase.InterestingElementName == null)
                                {
                                    break;
                                }
                                else if (baselineReader.NodeType == XmlNodeType.Element && baselineReader.LocalName == testCase.InterestingElementName)
                                {
                                    break;
                                }
                            }

                            this.Assert.AreEqual(baselineReader.ReadInnerXml(), bufferingXmlReader.ReadInnerXml(), "ReadInnerXml failed.");

                            this.VerifyReadersAreEqual(baselineReader, bufferingXmlReader, Int32.MaxValue, false, false);
                        }
                    }
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that buffering mixed with non-buffering reader works.")]
        public void BufferingAndNonBufferingReaderTest()
        {
            this.CombinatorialEngineProvider.RunCombinations(
                new bool[] { false, true },
                new int[][] {  // The arrays will be looped over until the end of the input. Start non-buffering, reads the number, switches to buffering, reads the number and over.
                    new int[] { 3 },
                    new int[] { 3, 0 },
                    new int[] { 0, 3, 3, 0 },
                    new int[] { 1 },
                    new int[] { 7 },
                },
                (verifyAttribtues, readCountLoop) =>
                {
                    using (XmlReader comparedReader = XmlReader.Create(new StringReader(TestXml)))
                    {
                        BufferingXmlReader bufferingXmlReader = new BufferingXmlReader(
                            comparedReader,
                            /*parentXmlReader*/ null,
                            /*documentBaseUri*/ null,
                            /*disableXmlBase*/ false,
                            ODataConstants.DefaultMaxRecursionDepth,
                            this.Assert);

                        var countLoop = readCountLoop.EndLessLoop();
                        bool buffering = false;
                        int nodesConsumed = 0;
                        while (true)
                        {
                            countLoop.MoveNext();
                            int readCount = countLoop.Current;

                            using (XmlReader baselineReader = XmlReader.Create(new StringReader(TestXml)))
                            {
                                this.expectedXmlBaseUriStack.Clear();
                                for (int i = 0; i < nodesConsumed; i++)
                                {
                                    this.ReadFromBaselineReader(baselineReader);
                                }

                                // Note we have to move to element always, because StartBuffering/StopBuffering is supported only on element boundary.
                                bool moreAvailable = this.VerifyReadersAreEqual(baselineReader, bufferingXmlReader, readCount, true, verifyAttribtues);
                                if (buffering)
                                {
                                    bufferingXmlReader.StopBuffering();
                                    buffering = false;
                                }
                                else
                                {
                                    nodesConsumed += readCount;

                                    if (!moreAvailable)
                                    {
                                        break;
                                    }

                                    bufferingXmlReader.StartBuffering();
                                    buffering = true;
                                }
                            }
                        }
                    }
                });
        }

        [TestMethod, TestCategory("Reader.Atom"), Variation(Description = "Verifies that XML base is correctly reported in corner cases.")]
        public void XmlBaseTest()
        {
            string xml = "<root><child xml:base='relative'/></root>";
            using (XmlReader baseReader = XmlReader.Create(new StringReader(xml)))
            {
                BufferingXmlReader bufferingXmlReader = new BufferingXmlReader(
                    baseReader,
                    /*parentXmlReader*/ null,
                    /*documentBaseUri*/ null,
                    /*disableXmlBase*/ false,
                    ODataConstants.DefaultMaxRecursionDepth,
                    this.Assert);

                // Move to the root element
                bufferingXmlReader.Read();
                this.Assert.AreEqual(XmlNodeType.Element, bufferingXmlReader.NodeType, "Unexpected node type.");
                this.Assert.IsNull(bufferingXmlReader.XmlBaseUri, "XmlBaseUri should be null if none is defined.");

                // Move to the child - should fail since there's a relative xml:base without base URI on it.
                this.Assert.ExpectedException(() => bufferingXmlReader.Read(), ODataExpectedExceptions.ODataException("ODataAtomDeserializer_RelativeUriUsedWithoutBaseUriSpecified", "relative"), this.ExceptionVerifier);
            }
        }

        private bool ReadFromBaselineReader(XmlReader baselineReader)
        {
            baselineReader.MoveToElement();
            if (baselineReader.NodeType == XmlNodeType.EndElement || baselineReader.IsEmptyElement)
            {
                this.expectedXmlBaseUriStack.Pop();
            }

            bool result = baselineReader.Read();
            if (result)
            {
                if (baselineReader.NodeType == XmlNodeType.Element)
                {
                    string expectedXmlBaseValue = baselineReader.GetAttribute("expectedXmlBase");
                    if (expectedXmlBaseValue != null)
                    {
                        this.expectedXmlBaseUriStack.Push(new Uri(expectedXmlBaseValue));
                    }
                    else
                    {
                        if (this.expectedXmlBaseUriStack.Count == 0)
                        {
                            this.expectedXmlBaseUriStack.Push(null);
                        }
                        else
                        {
                            this.expectedXmlBaseUriStack.Push(this.expectedXmlBaseUriStack.Peek());
                        }
                    }
                }
            }

            return result;
        }

        private bool VerifyReadersAreEqual(XmlReader expectedReader, XmlReader actualReader, int numberOfNodesToRead, bool moveToElementBeforeRead, bool verifyAttributes)
        {
            int countRead = 0;
            int countEnd = -1;

            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            while (countRead < numberOfNodesToRead)
            {
                if (this.ReadFromBaselineReader(expectedReader))
                {
                    this.Assert.IsTrue(actualReader.Read(), "Read differs");

                    this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
                    if (verifyAttributes)
                    {
                        this.VerifyAttributesAreEqual(expectedReader, actualReader, moveToElementBeforeRead);
                    }
                }
                else
                {
                    this.Assert.IsFalse(actualReader.Read(), "Read differs");
                    if (countEnd == -1)
                    {
                        countEnd = countRead;
                    }
                }

                countRead++;
                if (countEnd >= 0 && countRead > countEnd)
                {
                    return false;
                }
            }

            return true;
        }

        private void VerifyAttributesAreEqual(XmlReader expectedReader, XmlReader actualReader, bool moveToElementBeforeRead)
        {
            List<Tuple<string, string, string>> attributeNames = new List<Tuple<string, string, string>>();

            // Do not compare attributes on XmlDeclaration, the buffering reader doesn't implement this (intentionally)
            if (expectedReader.NodeType == XmlNodeType.XmlDeclaration)
            {
                return;
            }

            // Use MoveToFirst and then MoveToNext
            int attributeCount = 0;
            if (this.MoveToFirstAttributeAndCompare(expectedReader, actualReader))
            {
                attributeNames.Add(new Tuple<string, string, string>(actualReader.LocalName, actualReader.NamespaceURI, actualReader.Name));
                attributeCount++;
                while (this.MoveToNextAttributeAndCompare(expectedReader, actualReader))
                {
                    attributeNames.Add(new Tuple<string, string, string>(actualReader.LocalName, actualReader.NamespaceURI, actualReader.Name));
                    attributeCount++;
                }
            }

            this.MoveToElementAndCompare(expectedReader, actualReader);

            // Use just MoveNext
            int count = 0;
            while (this.MoveToNextAttributeAndCompare(expectedReader, actualReader))
            {
                this.VerifyAttributeValue(expectedReader, actualReader, moveToElementBeforeRead);
                count++;
            }

            this.Assert.AreEqual(attributeCount, count, "MoveToNextAttribute reported different attribute count.");

            // Move to specific attribute
            for (int i = 0; i < attributeCount; i++)
            {
                this.MoveToElementAndCompare(expectedReader, actualReader);
                for (int j = 0; j <= i; j++)
                {
                    this.MoveToNextAttributeAndCompare(expectedReader, actualReader);
                    this.VerifyAttributeValue(expectedReader, actualReader, moveToElementBeforeRead);
                }

                this.MoveToFirstAttributeAndCompare(expectedReader, actualReader);
                this.VerifyAttributeValue(expectedReader, actualReader, moveToElementBeforeRead);
            }

            this.MoveToElementAndCompare(expectedReader, actualReader);

            // Use MoveToAttribute(name, namespaceURI)
            foreach (var attributeName in attributeNames)
            {
                this.MoveToAttributeAndCompare(expectedReader, actualReader, attributeName.Item1, attributeName.Item2);
                this.VerifyAttributeValue(expectedReader, actualReader, moveToElementBeforeRead);
            }

            this.MoveToElementAndCompare(expectedReader, actualReader);

            // Use GetAttribute(name, namespaceURI)
            foreach (var attributeName in attributeNames)
            {
                string expectedValue = expectedReader.GetAttribute(attributeName.Item1, attributeName.Item2);
                string actualValue = actualReader.GetAttribute(attributeName.Item1, attributeName.Item2);
                this.Assert.AreEqual(expectedValue, actualValue, "GetAttribute(name, ns) differs");
                this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            }

            this.MoveToElementAndCompare(expectedReader, actualReader);

            // Use MoveToAttribute(name)
            foreach (var attributeName in attributeNames)
            {
                this.MoveToAttributeAndCompare(expectedReader, actualReader, attributeName.Item3);
                this.VerifyAttributeValue(expectedReader, actualReader, moveToElementBeforeRead);
            }

            this.MoveToElementAndCompare(expectedReader, actualReader);

            // Use GetAttribute(name)
            foreach (var attributeName in attributeNames)
            {
                string expectedValue = expectedReader.GetAttribute(attributeName.Item3);
                string actualValue = actualReader.GetAttribute(attributeName.Item3);
                this.Assert.AreEqual(expectedValue, actualValue, "GetAttribute(name) differs");
                this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            }

            this.MoveToElementAndCompare(expectedReader, actualReader);
            
            // Use GetAttribute(i)
            for (int i = 0; i < attributeNames.Count; i++)
            {
                string expectedValue = expectedReader.GetAttribute(i);
                string actualValue = actualReader.GetAttribute(i);
                this.Assert.AreEqual(expectedValue, actualValue, "GetAttribute(i) differs");
                this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            }

            if (moveToElementBeforeRead)
            {
                this.MoveToElementAndCompare(expectedReader, actualReader);
            }
        }

        private bool MoveToFirstAttributeAndCompare(XmlReader expectedReader, XmlReader actualReader)
        {
            bool result = expectedReader.MoveToFirstAttribute();
            this.Assert.AreEqual(result, actualReader.MoveToFirstAttribute(), "MoveToFirstAttribute differs");
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            return result;
        }

        private bool MoveToNextAttributeAndCompare(XmlReader expectedReader, XmlReader actualReader)
        {
            bool result = expectedReader.MoveToNextAttribute();
            this.Assert.AreEqual(result, actualReader.MoveToNextAttribute(), "MoveToNextAttribute differs");
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            return result;
        }

        private bool MoveToAttributeAndCompare(XmlReader expectedReader, XmlReader actualReader, string name, string ns)
        {
            bool result = expectedReader.MoveToAttribute(name, ns);
            this.Assert.AreEqual(result, actualReader.MoveToAttribute(name, ns), "MoveToAttribute(name, ns) differs");
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            return result;
        }

        private bool MoveToAttributeAndCompare(XmlReader expectedReader, XmlReader actualReader, string name)
        {
            bool result = expectedReader.MoveToAttribute(name);
            this.Assert.AreEqual(result, actualReader.MoveToAttribute(name), "MoveToAttribute(name) differs");
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            return result;
        }

        private bool MoveToElementAndCompare(XmlReader expectedReader, XmlReader actualReader)
        {
            bool result = expectedReader.MoveToElement();
            this.Assert.AreEqual(result, actualReader.MoveToElement(), "MoveToElement differs");
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            return result;
        }

        private void VerifyReaderNodesAreEqual(XmlReader expectedReader, XmlReader actualReader)
        {
            this.Assert.AreEqual(expectedReader.NodeType, actualReader.NodeType, "NodeType differs");
            this.Assert.AreEqual(expectedReader.IsEmptyElement, actualReader.IsEmptyElement, "IsEmptyElement differs");
            this.Assert.AreEqual(expectedReader.LocalName, actualReader.LocalName, "LocalName differs");
            this.Assert.AreEqual(expectedReader.NamespaceURI, actualReader.NamespaceURI, "NamespaceURI differs");
            this.Assert.AreEqual(expectedReader.Value, actualReader.Value, "Value differs");
            this.Assert.AreEqual(expectedReader.Depth, actualReader.Depth, "Depth differs");
            this.Assert.AreEqual(expectedReader.EOF, actualReader.EOF, "EOF differs");
            this.Assert.AreEqual(expectedReader.ReadState, actualReader.ReadState, "ReadState differs");
            this.Assert.AreEqual(expectedReader.HasValue, actualReader.HasValue, "HasValue differs");
            if (expectedReader.NodeType == XmlNodeType.Element)
            {
                // We only report attributes on elements, the fake attributes on the XmlDeclaration are not reported this way
                // and that's fine, for our purposes, nothing relies on these.
                this.Assert.AreEqual(expectedReader.AttributeCount, actualReader.AttributeCount, "AttributeCount differs");
            }

            this.Assert.IsNull(actualReader.BaseURI, "BaseURI should always be null");
            this.Assert.AreEqual(expectedReader.HasValue, actualReader.HasValue, "HasValue differs");

            if (this.expectedXmlBaseUriStack.Count > 0)
            {
                this.Assert.AreEqual(this.expectedXmlBaseUriStack.Peek(), ((BufferingXmlReader)actualReader).XmlBaseUri, "The XmlBaseUri differs.");
            }
            else
            {
                this.Assert.IsNull(((BufferingXmlReader)actualReader).XmlBaseUri, "The XmlBaseUri should be null.");
            }
        }

        private void VerifyAttributeValue(XmlReader expectedReader, XmlReader actualReader, bool useReadAttributeValue)
        {
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            if (expectedReader.NodeType != XmlNodeType.Attribute || !useReadAttributeValue)
            {
                return;
            }

            while (expectedReader.ReadAttributeValue())
            {
                this.Assert.AreEqual(true, actualReader.ReadAttributeValue(), "ReadAttributeValue differs.");
                this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
            }

            this.Assert.AreEqual(false, actualReader.ReadAttributeValue(), "ReadAttributeValue differs.");
            this.VerifyReaderNodesAreEqual(expectedReader, actualReader);
        }
    }
}
