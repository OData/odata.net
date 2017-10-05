//---------------------------------------------------------------------
// <copyright file="HttpWebRequestUnitTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
#if WIN8 || WINDOWSPHONE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
    using Microsoft.OData.Client;
    using System.Net;
    using FluentAssertions;

    /// <summary>
    /// Unit tests for the HttpWebRequest class. Primarily used for testing differences accoss all the platforms
    /// </summary>
    [TestClass]
    public class HttpWebRequestMessageUnitTests
    {
#if PORTABLELIB || SILVERLIGHT
        [TestMethod]
        public void IsRunningOnSilverlightShouldReturnTrueWhenRunningOnSilverlightOnly()
        {
#if SILVERLIGHT
            HttpWebRequestMessage.IsRunningOnSilverlight.Should().BeTrue();
#else
            HttpWebRequestMessage.IsRunningOnSilverlight.Should().BeFalse();
#endif
        }
#endif

        [TestMethod]
        public void SetUserAgentShouldSucceed()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.svc");
            HttpWebRequestMessage.SetUserAgentHeader(request, "MyUserAgent");
#if WIN8
            request.Headers["UserAgent"].Should().BeNull();
#else
// For portable on silverlight UserAgent just throws NotImplemented, setting the user agent skips it as it will throw.
#if !(SILVERLIGHT && PORTABLELIB) && !(NETCOREAPP1_0 || NETCOREAPP2_0)
            request.UserAgent.Should().Be("MyUserAgent");
#endif
#endif
        }

        [TestMethod]
        public void SetAcceptCharsetShouldNotBeSetOnSilverlightAndSetOnOtherPlatforms()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.svc");
            HttpWebRequestMessage.SetAcceptCharset(request, "utf8");

            // For portable on silverlight UserAgent just throws NotImplemented, setting the user agent skips it as it will throw.
#if !(SILVERLIGHT && PORTABLELIB)
            request.Headers["Accept-Charset"].Should().Be("utf8");
#endif
        }

        [TestMethod]
        public void SetContentLengthShouldSucceed()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://www.svc");
            HttpWebRequestMessage.SetHttpWebRequestContentLength(request, 1);

#if WIN8
            request.Headers["Content-Length"].Should().Be("0");
#elif !(NETCOREAPP1_0 || NETCOREAPP2_0)
            request.ContentLength.Should().Be(1);
#endif
        }
    }
}
