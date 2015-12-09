//---------------------------------------------------------------------
// <copyright file="ExceptionUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using FluentAssertions;
using Microsoft.OData.Core.UriParser;
using Xunit;

namespace Microsoft.OData.Core.Tests.UriParser
{
    public class ExceptionUtilTests
    {
        [Fact]
        public void NotFoundHelperShouldThrowSpecialExceptionType()
        {
            ExceptionUtil.CreateResourceNotFoundError("foo").Should().BeAssignableTo<ODataUnrecognizedPathException>();
        }

        [Fact]
        public void SyntaxErrorHelperShouldThrowNormalODataException()
        {
            ExceptionUtil.CreateSyntaxError().GetType().Should().Be(typeof(ODataException));
        }

        [Fact]
        public void NotFoundHelperShouldSetMessageCorrectly()
        {
            ExceptionUtil.CreateResourceNotFoundError("foo").Message.Should().Be(Strings.RequestUriProcessor_ResourceNotFound("foo"));
        }

        [Fact]
        public void SyntaxErrorHelperShouldSetMessageCorrectly()
        {
            ExceptionUtil.CreateSyntaxError().Message.Should().Be(Strings.RequestUriProcessor_SyntaxError);

            string  a =  Strings.General_InternalError(Microsoft.OData.Core.UriParser.InternalErrorCodes.UriQueryExpressionParser_ParseComparison);
        }
    }
}