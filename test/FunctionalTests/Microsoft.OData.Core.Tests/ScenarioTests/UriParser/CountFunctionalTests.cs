//---------------------------------------------------------------------
// <copyright file="CountFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.UriParser;
using Microsoft.OData.Edm;
using Xunit;
using ODataErrorStrings = Microsoft.OData.Strings;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    /// <summary>
    /// Functional tests for query count.
    /// </summary>
    public class CountFunctionalTests
    {
        [Fact]
        public void CountTrueWorks()
        {
            bool? value = ParseCount("true");
            Assert.True(value.Value);
        }

        [Fact]
        public void CountFalseWorks()
        {
            bool? value = ParseCount("false");
            Assert.False(value.Value);
        }

        [Fact]
        public void LeadingAndTrailingWhitespaceIsTrimmed()
        {
            bool? value = ParseCount("   true  ");
            Assert.True(value.Value);
        }

        [Fact]
        public void NullInputCase()
        {
            bool? value = ParseCount(null);
            Assert.Null(value);
        }

        [Fact]
        public void EmptyInputCase()
        {
            Action createWithInvalidInput = () => ParseCount("");
            createWithInvalidInput.Throws<ODataException>(ODataErrorStrings.ODataUriParser_InvalidCount(""));
        }

        [Fact]
        public void InvalidCaseThrows()
        {
            Action createWithInvalidInput = () => ParseCount("True");
            createWithInvalidInput.Throws<ODataException>(ODataErrorStrings.ODataUriParser_InvalidCount("True"));
        }

        [Fact]
        public void InvalidThrows()
        {
            Action createWithInvalidInput = () => ParseCount("fasle");
            createWithInvalidInput.Throws<ODataException>(ODataErrorStrings.ODataUriParser_InvalidCount("fasle"));
        }

        private static bool? ParseCount(string p)
        {
            return new ODataQueryOptionParser(EdmCoreModel.Instance, null, null, new Dictionary<string, string>() { { "$count", p } }).ParseCount();
        }
    }
}
