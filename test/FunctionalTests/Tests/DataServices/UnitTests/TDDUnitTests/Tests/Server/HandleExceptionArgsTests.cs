//---------------------------------------------------------------------
// <copyright file="HandleExceptionArgsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using FluentAssertions;
    using Microsoft.OData.Service;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HandleExceptionArgsTests
    {
        [TestMethod]
        public void CreateODataErrorFromExceptionArgsShouldCreateODataErrorWithCustomAnnotationsAndInstanceAnnotations()
        {
            DataServiceException dse = new DataServiceException(500, "500", "Test message", "en-US", null);
            HandleExceptionArgs args = new HandleExceptionArgs(dse, responseWritten:false, contentType:"application/json",verboseResponse:true);
            ODataError error = args.CreateODataError();
            error.InstanceAnnotations.As<object>().Should().BeSameAs(args.InstanceAnnotations);
        }
    }
}
