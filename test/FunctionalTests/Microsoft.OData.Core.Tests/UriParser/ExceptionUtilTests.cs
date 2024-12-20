﻿//---------------------------------------------------------------------
// <copyright file="ExceptionUtilTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    public class ExceptionUtilTests
    {
        [Fact]
        public void NotFoundHelperShouldThrowSpecialExceptionType()
        {
            var exception = ExceptionUtil.CreateResourceNotFoundError("foo");
            Assert.IsType<ODataUnrecognizedPathException>(exception);
        }

        [Fact]
        public void SyntaxErrorHelperShouldThrowNormalODataException()
        {
            Assert.IsType<ODataException>(ExceptionUtil.CreateSyntaxError());
        }

        [Fact]
        public void NotFoundHelperShouldSetMessageCorrectly()
        {
            var exception = ExceptionUtil.CreateResourceNotFoundError("foo");
            Assert.Equal(Error.Format(SRResources.RequestUriProcessor_ResourceNotFound, "foo"), exception.Message);
        }

        [Fact]
        public void SyntaxErrorHelperShouldSetMessageCorrectly()
        {
            var exception = ExceptionUtil.CreateSyntaxError();
            Assert.Equal(SRResources.RequestUriProcessor_SyntaxError, exception.Message);

            string a = Error.Format(SRResources.General_InternalError, OData.UriParser.InternalErrorCodes.UriQueryExpressionParser_ParseComparison);
        }
    }
}