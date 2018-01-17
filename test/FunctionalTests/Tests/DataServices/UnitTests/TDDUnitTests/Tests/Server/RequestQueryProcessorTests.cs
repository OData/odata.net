//---------------------------------------------------------------------
// <copyright file="RequestQueryProcessorTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Serializers;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AstoriaUnitTests.Tests.Server
{
    [TestClass]
    public class DataStringEscapeBuilderTests
    {
        [TestMethod]
        public void AlwaysEscapePlus()
        {
            // '+' should always be escaped, regardless of inside or outside of string
            Assert.AreEqual("'%2B' %2B", DataStringEscapeBuilder.EscapeDataString("'+' +"));
        }

        [TestMethod]
        public void ShouldNotEscapeNonString()
        {
            Assert.AreEqual("'abc' eq%20'abc'", DataStringEscapeBuilder.EscapeDataString("'abc' eq%20'abc'"));
        }

        [TestMethod]
        public void ShouldEscapeString()
        {
            Assert.AreEqual("'a%2520bc' eq%20'a%20bc'", DataStringEscapeBuilder.EscapeDataString("'a%20bc' eq%20'a bc'"));
            Assert.AreEqual("\"a%2520bc\" eq%20\"a%20bc\"", DataStringEscapeBuilder.EscapeDataString("\"a%20bc\" eq%20\"a bc\""));
        }
    }
}
