//---------------------------------------------------------------------
// <copyright file="ExceptionUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests
{
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExceptionUtilTests
    {
        [TestMethod]
        public void NotFoundHelperShouldThrowSpecialExceptionType()
        {
            ExceptionUtil.CreateResourceNotFoundError("foo").Should().BeAssignableTo<ODataUnrecognizedPathException>();
        }

        [TestMethod]
        public void SyntaxErrorHelperShouldThrowNormalODataException()
        {
            ExceptionUtil.CreateSyntaxError().GetType().Should().Be(typeof(ODataException));
        }

        [TestMethod]
        public void NotFoundHelperShouldSetMessageCorrectly()
        {
            ExceptionUtil.CreateResourceNotFoundError("foo").Message.Should().Be(Strings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [TestMethod]
        public void SyntaxErrorHelperShouldSetMessageCorrectly()
        {
            ExceptionUtil.CreateSyntaxError().Message.Should().Be(Strings.RequestUriProcessor_SyntaxError);

            string  a =  Strings.General_InternalError(Microsoft.OData.Core.UriParser.InternalErrorCodes.UriQueryExpressionParser_ParseComparison);
        }
    }
}