//---------------------------------------------------------------------
// <copyright file="DefaultNamespaceCompensatingXmlWriterTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Xml;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Xunit;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class DefaultNamespaceCompensatingXmlWriterTests : IDisposable
    {
        private readonly StringBuilder builder;
        private readonly DefaultNamespaceCompensatingXmlWriter testSubject;

        public DefaultNamespaceCompensatingXmlWriterTests()
        {
            this.builder = new StringBuilder();
            this.testSubject = new DefaultNamespaceCompensatingXmlWriter(XmlWriter.Create(this.builder, new XmlWriterSettings { OmitXmlDeclaration = true }));
        }

        public void Dispose()
        {
            this.testSubject.Flush();
            this.builder.Clear();
        }

        [Fact]
        public void CompensatingWriterShouldRemovePrefixFromRootElement()
        {
            this.testSubject.WriteElementString("x", "foo", "http://fake.org", "value");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\">value</foo>");
        }

        [Fact]
        public void CompensatingWriterShouldLeavePrefixOnAttribute()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteAttributeString("x", "bar", "http://fake.org", "value");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo x:bar=\"value\"");
        }

        [Fact]
        public void CompensatingWriterShouldRemovePrefixFromChildElement()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteStartElement("x", "bar", "http://fake.org");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\"><bar");
        }

        [Fact]
        public void CompensatingWriterShouldNotRemovePrefixFromChildElementWithDifferentPrefixEvenIfSameNamespace()
        {
            this.testSubject.WriteStartElement("x", "foo", "http://fake.org");
            this.testSubject.WriteStartElement("y", "bar", "http://fake.org");
            this.testSubject.Flush();
            this.builder.ToString().Should().Be("<foo xmlns=\"http://fake.org\"><y:bar");
        }

        [Fact]
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