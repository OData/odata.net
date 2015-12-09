//---------------------------------------------------------------------
// <copyright file="CountFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm.Library;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Core.Strings;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Functional tests for query count.
    /// </summary>
    public class CountFunctionalTests
    {
        [Fact]
        public void CountTrueWorks()
        {
            ParseCount("true").Should().BeTrue();
        }

        [Fact]
        public void CountFalseWorks()
        {
            ParseCount("false").Should().BeFalse();
        }

        [Fact]
        public void LeadingAndTrailingWhitespaceIsTrimmed()
        {
            ParseCount("   true  ").Should().BeTrue();
        }

        [Fact]
        public void NullInputCase()
        {
            ParseCount(null).Should().NotHaveValue();
        }

        [Fact]
        public void EmptyInputCase()
        {
            Action createWithInvalidInput = () => ParseCount("");
            createWithInvalidInput.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriParser_InvalidCount(""));
        }

        [Fact]
        public void InvalidCaseThrows()
        {
            Action createWithInvalidInput = () => ParseCount("True");
            createWithInvalidInput.ShouldThrow<ODataException>().WithMessage(ODataErrorStrings.ODataUriParser_InvalidCount("True"));
        }

        [Fact]
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
