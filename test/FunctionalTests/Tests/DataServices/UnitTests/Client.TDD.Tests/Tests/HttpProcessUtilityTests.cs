//---------------------------------------------------------------------
// <copyright file="HttpProcessUtilityTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Xunit;

    public class HttpProcessUtilityTests
    {
        [Fact]
        public void ValidHttpProcessUtilityReadMediaTypeTest()
        {
            MediaTypeTest[] tests = new MediaTypeTest[]
            {
                new MediaTypeTest() { InputText = "text/plain", MediaType = "text/plain" },
                new MediaTypeTest() { InputText = "text/plain ", MediaType = "text/plain", OutputText = "text/plain" },
                new MediaTypeTest() { InputText = "text/plain;", MediaType = "text/plain", OutputText = "text/plain" },
                new MediaTypeTest() 
                { 
                    InputText = "text/plain;a=b", MediaType = "text/plain", OutputText = "text/plain;a=b",
                    Parameters = new ContentTypeUtil.MediaParameter[] { new ContentTypeUtil.MediaParameter("a", "b", false) }
                },
                new MediaTypeTest() 
                { 
                    InputText = "text/plain;a=b; cc=dd ", MediaType = "text/plain", OutputText = "text/plain;a=b;cc=dd",
                    Parameters = new ContentTypeUtil.MediaParameter[] { 
                        new ContentTypeUtil.MediaParameter("a", "b", false),
                        new ContentTypeUtil.MediaParameter("cc", "dd", false) }
                },
                new MediaTypeTest() 
                { 
                    InputText = "text/plain;a=", MediaType = "text/plain",
                    Parameters = new ContentTypeUtil.MediaParameter[] { new ContentTypeUtil.MediaParameter("a", "", false) }
                },
                new MediaTypeTest() 
                { 
                    InputText = "text/plain;a=\"b\"", MediaType = "text/plain",
                    Parameters = new ContentTypeUtil.MediaParameter[] { new ContentTypeUtil.MediaParameter("a", "b", true) }
                },
                new MediaTypeTest() // its weird that we did not fail when no name was specified
                { 
                    InputText = "text/plain;=b", MediaType = "text/plain",
                    Parameters = new ContentTypeUtil.MediaParameter[] { new ContentTypeUtil.MediaParameter("", "b", false) }
                },
                new MediaTypeTest()
                { 
                    InputText = "text/plain;a=", MediaType = "text/plain",
                    Parameters = new ContentTypeUtil.MediaParameter[] { new ContentTypeUtil.MediaParameter("a", "", false) }
                },
            };
            foreach (var test in tests)
            {
                string mimeType = null;
                ContentTypeUtil.MediaParameter[] parameters = null;
                parameters = ContentTypeUtil.ReadContentType(test.InputText, out mimeType);

                //Media types match
                Assert.Equal(mimeType, test.MediaType);

                //Parameter nullability matches.
                Assert.Equal(parameters != null, test.Parameters != null);
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        //Parameters name do not match
                        Assert.Equal(parameters[i].Name, test.Parameters[i].Name);

                        //Parameters value do not match
                        Assert.Equal(parameters[i].Value, test.Parameters[i].Value);

                        //Parameters original value do not match
                        Assert.Equal(parameters[i].GetOriginalValue(), test.Parameters[i].GetOriginalValue());
                        Assert.Equal(test.OutputText ?? test.InputText, ContentTypeUtil.WriteContentType(mimeType, parameters));
                    }
                }
            }
        }

        [Fact]
        public void HttpProcessUtilityReadMediaTypeTest()
        {
            MediaTypeTest[] tests = new MediaTypeTest[]
            {
                new MediaTypeTest() { InputText = "", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text /plain", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/ plain", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain a=b", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain;a", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain;a =b", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain;a= b", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain;;", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain;\"a\"=b;", ExceptionExpected = true },
                new MediaTypeTest() { InputText = "text/plain;a=b=c; cc=dd ", ExceptionExpected = true },
            };

            foreach (var test in tests)
            {
                string mimeType = null;
                Action testAction = () => ContentTypeUtil.ReadContentType(test.InputText, out mimeType);
                testAction.ShouldThrow<Exception>();
            }
        }

        private class MediaTypeTest
        {
            public string InputText { get; set; }
            public string OutputText { get; set; }
            public bool ExceptionExpected { get; set; }
            public string MediaType { get; set; }
            public ContentTypeUtil.MediaParameter[] Parameters { get; set; }
        }
    }
}
