//---------------------------------------------------------------------
// <copyright file="DefaultNamespaceCompensatingXmlWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;

namespace Microsoft.Test.OData.TDD.Tests.Writer.Atom
{
    using System;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.Atom;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultNamespaceCompensatingXmlWriterTests
    {
        private readonly StringBuilder builder;
        private readonly DefaultNamespaceCompensatingXmlWriter testSubject;

        public DefaultNamespaceCompensatingXmlWriterTests()
        {
            this.builder = new StringBuilder();
            this.testSubject = new DefaultNamespaceCompensatingXmlWriter(XmlWriter.Create(this.builder, new XmlWriterSettings { OmitXmlDeclaration = true }));
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.testSubject.Flush();
            this.builder.Clear();
        }

        [TestMethod]
        public void CompensatingWriterShouldRemovePrefixFromRootElement()
        {
            this.testSubject.WriteElementString("x", "foo", "http://fake.org", "value");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\">value</foo>");
        }

        [TestMethod]
        public void CompensatingWriterShouldLeavePrefixOnAttribute()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteAttributeString("x", "bar", "http://fake.org", "value");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo x:bar=\"value\"");
        }

        [TestMethod]
        public void CompensatingWriterShouldRemovePrefixFromChildElement()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteStartElement("x", "bar", "http://fake.org");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\"><bar");
        }

        [TestMethod]
        public void CompensatingWriterShouldNotRemovePrefixFromChildElementWithDifferentPrefixEvenIfSameNamespace()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteStartElement("y", "bar", "http://fake.org");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\"><y:bar");
        }

        [TestMethod]
        public void CompensatingWriterShouldRemovePrefixFromDeepChildElementEvenIfMiddleElementHadDifferentPrefix()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteStartElement("y", "bar", "http://fake.org");
            this.testSubject.WriteStartElement("x", "baz", "http://fake.org");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\"><y:bar xmlns:y=\"http://fake.org\"><baz");
        }
    }
}