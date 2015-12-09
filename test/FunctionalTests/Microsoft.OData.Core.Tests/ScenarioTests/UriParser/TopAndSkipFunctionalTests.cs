//---------------------------------------------------------------------
// <copyright file="TopAndSkipFunctionalTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.UriParser
{
    public class TopAndSkipFunctionalTests
    {
        private static readonly string[] SharedInvalidNumericInput =
        {
            "'9'"   , "-9"      , "0.3"                    ,
            "3.14"  , "-2.77"   , "9223372036854775808"    ,
            "-52"   , "-1"      , "-9223372036854775808"   ,
            "null"  , "inf"     , "I'm long value"         ,

            // The followings are ported from taupo cases
            "c"     , "9L"      , "-12.4m"                 ,
            "(1)"   , "2 + 3"   , "int.MaxValue"           ,
        };

        #region $top option
        [Fact]
        public void PositiveTopValueWorks()
        {
            ParseTop("5").Should().Be(5);
        }

        [Fact]
        public void ZeroTopValueWorks()
        {
            ParseTop(" 0  ").Should().Be(0);
        }

        [Fact]
        public void InvaidTopValueThrows()
        {
            foreach (var input in SharedInvalidNumericInput)
            {
                Action action = () => ParseTop(input);
                action.ShouldThrow<ODataException>("'{0}' should be invalid input", input).WithMessage(Strings.SyntacticTree_InvalidTopQueryOptionValue(input));
            }

        }
        #endregion $top option

        #region $skip option
        [Fact]
        public void PositiveSkipValueWorks()
        {
            ParseSkip("5").Should().Be(5);
        }

        [Fact]
        public void ZeroSkipValueWorks()
        {
            ParseSkip(" 0  ").Should().Be(0);
        }

        [Fact]
        public void InvalidSkipValueThrows()
        {
            foreach (var input in SharedInvalidNumericInput)
            {
                Action action = () => ParseSkip(input);
                action.ShouldThrow<ODataException>("'{0}' should be invalid input", input).WithMessage(Strings.SyntacticTree_InvalidSkipQueryOptionValue(input));
            }
        }
        #endregion $skip option

        private static long? ParseTop(string p)
        {
            return new ODataQueryOptionParser(EdmCoreModel.Instance, null, null, new Dictionary<string, string>() { { "$top", p } }).ParseTop();
        }

        private static long? ParseSkip(string p)
        {
            return new ODataQueryOptionParser(EdmCoreModel.Instance, null, null, new Dictionary<string, string>() { { "$skip", p } }).ParseSkip();
        }
    }
}
