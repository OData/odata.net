//---------------------------------------------------------------------
// <copyright file="KeyAsSegmentTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.KeyAsSegmentTests
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.KeyAsSegmentServiceReference;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class KeyAsSegmentTest : EndToEndTestBase
    {
        public KeyAsSegmentTest()
            : base(ServiceDescriptors.KeyAsSegmentService)
        {
        }

        protected static void VerifyUriDoesNotContainParentheses(Uri uri)
        {
            Assert.IsFalse(uri.OriginalString.Contains("("), "Uri contains left parentheses");
            Assert.IsFalse(uri.OriginalString.Contains(")"), "Uri contains right parentheses");
        }

        protected DataServiceContextWrapper<DefaultContainer> CreateWrappedContext()
        {
            var wrappedContext = base.CreateWrappedContext<DefaultContainer>();
            wrappedContext.UrlKeyDelimiter = DataServiceUrlKeyDelimiter.Slash;
            return wrappedContext;
        }
    }
}
