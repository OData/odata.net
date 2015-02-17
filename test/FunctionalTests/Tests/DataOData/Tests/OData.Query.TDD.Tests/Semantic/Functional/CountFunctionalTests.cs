//---------------------------------------------------------------------
// <copyright file="CountFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.Semantic.Functional
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    /// <summary>
    /// Functional tests for query count.
    /// </summary>
    [TestClass]
    public class CountFunctionalTests
    {
        [TestMethod]
        public void CountTrueWorks()
        {
            ParseCount("true").Should().BeTrue();
        }

        [TestMethod]
        public void CountFalseWorks()
        {
            ParseCount("false").Should().BeFalse();
        }

        [TestMethod]
        public void LeadingAndTrailingWhitespaceIsTrimmed()
        {
            ParseCount("   true  ").Should().BeTrue();
        }

        [TestMethod]
        public void NullInputCase()
        {
            ParseCount(null).Should().NotHaveValue();
        }

        [TestMethod]
        public void EmptyInputCase()
        {
            Action createWithInvalidInput = () => ParseCount("");
            createWithInvalidInput.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriParser_InvalidCount(""));
        }

        [TestMethod]
        public void InvalidCaseThrows()
        {
            Action createWithInvalidInput = () => ParseCount("True");
            createWithInvalidInput.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriParser_InvalidCount("True"));
        }

        [TestMethod]
        public void InvalidThrows()
        {
            Action createWithInvalidInput = () => ParseCount("fasle");
            createWithInvalidInput.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriParser_InvalidCount("fasle"));
        }

        private static bool? ParseCount(string p)
        {
            return new ODataQueryOptionParser(EdmCoreModel.Instance, null, null, new Dictionary<string, string>() { { "$count", p } }).ParseCount();
        }
    }
}
