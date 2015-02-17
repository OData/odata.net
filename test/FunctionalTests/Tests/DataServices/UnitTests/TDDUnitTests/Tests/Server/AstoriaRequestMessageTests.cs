//---------------------------------------------------------------------
// <copyright file="AstoriaRequestMessageTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using System.IO;
    using System.Net;
    using AstoriaUnitTests.TDD.Tests.Server.Simulators;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using Microsoft.OData.Service;
    using ErrorStrings = Microsoft.OData.Service.Strings;

    /// <summary>
    /// Unit Tests for the AstoriaRequestMessage class. Mostly ensures that it picks up values from the IDataServiceHost class
    /// and ensures a few methods Astoria calls do as they claim.
    /// </summary>
    [TestClass]
    public class AstoriaRequestMessageTests
    {
        private static readonly Version V4 = new Version(4, 0);
        private readonly Uri absoluteServiceUri = new Uri("http://services.odata.org/Northwind/Northwind.svc/");
        private readonly Uri absoluteRequestUri = new Uri("http://services.odata.org/Northwind/Northwind.svc/Categories(1)");
        private readonly Uri absoluteTestUri = new Uri("http://temp.org");
        private readonly Uri relativeTestUri = new Uri("/temp", UriKind.Relative);
        private AstoriaRequestMessage requestMessageWithDefaultUris;
        private AstoriaRequestMessage requestMessageWithQueryStrings;

        [TestInitialize]
        public void Init()
        {
            this.requestMessageWithDefaultUris = new AstoriaRequestMessage(new DataServiceHostSimulator { AbsoluteServiceUri = this.absoluteServiceUri, AbsoluteRequestUri = this.absoluteRequestUri });
            this.requestMessageWithQueryStrings = new AstoriaRequestMessage(
                new DataServiceHostSimulator
                {
                    AbsoluteServiceUri = new Uri(this.absoluteServiceUri.OriginalString + "?originalServiceQueryString"),
                    AbsoluteRequestUri = new Uri(this.absoluteRequestUri + "?originalRequestQueryString"),
                });
        }

        [TestMethod]
        public void AbsoluteRequestUriIsFromHost()
        {
            this.requestMessageWithDefaultUris.AbsoluteRequestUri.Should().BeSameAs(this.absoluteRequestUri);
        }

        [TestMethod]
        public void AbsoluteServiceUriIsFromHost()
        {
            this.requestMessageWithDefaultUris.AbsoluteServiceUri.Should().BeSameAs(this.absoluteServiceUri);
        }

        [TestMethod]
        public void ContentTypeIsFromHost()
        {
            const string contentType = "application/atom+xml";
            var host = new DataServiceHostSimulator { RequestContentType = contentType };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.ContentType.Should().Be(contentType);
        }

        [TestMethod]
        public void GetQueryStringItemShouldGetComponentFromHostGetQueryStringItemMethod()
        {
            const string queryKey = "queryKey";
            const string queryValue = "queryValue";
            var host = new DataServiceHostSimulator { AbsoluteRequestUri = new Uri("http://www.service.com/there/is/not/even/a/query-string") };
            host.SetQueryStringItem(queryKey, queryValue);
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.GetQueryStringItem(queryKey).Should().Be(queryValue);
        }

        [TestMethod]
        public void GetDollarFormatQueryItemShouldGetValueFromHostGetQueryStringItemMethod()
        {
            const string queryKey = "$format";
            const string queryValue = "custom";
            var host = new DataServiceHostSimulator { };
            host.SetQueryStringItem(queryKey, queryValue);
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.GetQueryStringItem(queryKey).Should().Be(queryValue);
        }

        [TestMethod]
        public void HttpVerbIsFromHost()
        {
            foreach (var verb in HttpVerbUtils.KnownVerbs)
            {
                var host = new DataServiceHostSimulator { RequestHttpMethod = verb.ToString() };
                var requestMessage = new AstoriaRequestMessage(host);

                requestMessage.HttpVerb.Should().Be(verb);
                requestMessage.RequestHttpMethod.Should().Be(verb.ToString());
            }
        }

        [TestMethod]
        public void AfterInitializeVerionFieldsAreSetToDefaults()
        {
            var host = new DataServiceHostSimulator { };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.InitializeRequestVersionHeaders(V4);

            requestMessage.RequestVersion.Should().Be(V4);
            requestMessage.RequestMaxVersion.Should().Be(V4);
        }

        [TestMethod]
        public void AfterInitializeVerionFieldsAreSetWithHostValues()
        {
            var host = new DataServiceHostSimulator { RequestVersion = "4.0", RequestMaxVersion = "4.0" };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.InitializeRequestVersionHeaders(V4);

            requestMessage.RequestVersion.Should().Be(V4);
            requestMessage.RequestMaxVersion.Should().Be(V4);
        }

        [TestMethod]
        public void ProcessExceptionShouldCallHost()
        {
            var callbackInvoked = false;
            var host = new DataServiceHostSimulator { };
            host.ProcessExceptionCallBack = args => callbackInvoked = true;
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.ProcessException(new HandleExceptionArgs(new Exception(), true, null, false));
            callbackInvoked.Should().BeTrue();
        }

        [TestMethod]
        public void RequestAcceptCharSetIsFromHost()
        {
            const string value = "some_other_value";
            var host = new DataServiceHostSimulator { RequestAcceptCharSet = value };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.GetHeader("Accept-Charset").Should().Be(value);
        }

        [TestMethod]
        public void RequestContentTypeIsFromHost()
        {
            const string value = "a-content-type";
            var host = new DataServiceHostSimulator { RequestContentType = value };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.GetHeader("Content-Type").Should().Be(value);
        }

        [TestMethod]
        public void RequestHeadersIsFromHost()
        {
            var value = new WebHeaderCollection { { "RequestVersion", "2.0" }, { "Accept", "json" }, { "Custom", "customValue" } };
            var host = new DataServiceHost2Simulator { RequestHeaders = value };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.RequestHeaders.Should().BeEquivalentTo(value);
            requestMessage.RequestHeaders["RequestVersion"] = "2.0";
            requestMessage.RequestHeaders["Accept"] = "json";
            requestMessage.RequestHeaders["Custom"] = "customValue";
        }

        [TestMethod]
        public void RequestIfMatch()
        {
            const string value = "guid";
            var host = new DataServiceHostSimulator { RequestIfMatch = value };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.GetHeader("If-Match").Should().Be(value);
        }

        [TestMethod]
        public void RequestIfNoneMatch()
        {
            const string value = "someguid";
            var host = new DataServiceHostSimulator { RequestIfNoneMatch = value };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.GetHeader("If-None-Match").Should().Be(value);
        }

        [TestMethod]
        public void GetAcceptableFormatTypesAcceptHeaderIsFromHost()
        {
            const string value = "some_value";
            var host = new DataServiceHostSimulator { RequestAccept = value };
            var requestMessage = new AstoriaRequestMessage(host);

            requestMessage.InitializeRequestVersionHeaders(V4);
            requestMessage.GetAcceptableContentTypes().Should().Be(value);
        }

        [TestMethod]
        public void GetAcceptableContentTypesShouldCallDelegate()
        {
            var contentTypeSelector = new AcceptableContentTypeSelectorSimulator();
            var host = new DataServiceHostSimulator();
            var requestMessage = new AstoriaRequestMessage(host, contentTypeSelector);

            requestMessage.GetAcceptableContentTypes().Should().Be(AcceptableContentTypeSelectorSimulator.GetFormatReturnValue);
        }

        [TestMethod]
        public void AbsoluteRequestUriCanBeChanged()
        {
            this.requestMessageWithDefaultUris.AbsoluteRequestUri.Should().BeSameAs(this.absoluteRequestUri);
            this.requestMessageWithDefaultUris.AbsoluteRequestUri = this.absoluteTestUri;
            this.requestMessageWithDefaultUris.AbsoluteRequestUri.Should().BeSameAs(this.absoluteTestUri);
        }

        [TestMethod]
        public void AbsoluteRequestUriCannotBeSetToNull()
        {
            Action setToNull = () => this.requestMessageWithDefaultUris.AbsoluteRequestUri = null;
            setToNull.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.RequestUriProcessor_AbsoluteRequestUriCannotBeNull);
        }

        [TestMethod]
        public void AbsoluteRequestUriCannotBeSetToRelative()
        {
            Action setToRelative = () => this.requestMessageWithDefaultUris.AbsoluteRequestUri = this.relativeTestUri;
            setToRelative.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.RequestUriProcessor_AbsoluteRequestUriMustBeAbsolute);
        }

        [TestMethod]
        public void AbsoluteRequestUriCannotBeSetAfterMarkedReadOnly()
        {
            this.requestMessageWithDefaultUris.MakeRequestAndServiceUrisReadOnly();
            Action changeValue = () => this.requestMessageWithDefaultUris.AbsoluteRequestUri = this.absoluteTestUri;
            changeValue.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.AstoriaRequestMessage_CannotModifyRequestOrServiceUriAfterReadOnly);
        }

        [TestMethod]
        public void AbsoluteRequestUriCanBeReadFromHostAfterMarkedReadOnly()
        {
            this.requestMessageWithDefaultUris.MakeRequestAndServiceUrisReadOnly();
            this.requestMessageWithDefaultUris.AbsoluteRequestUri.Should().BeSameAs(this.absoluteRequestUri);
        }

        [TestMethod]
        public void AbsoluteServiceUriCanBeChanged()
        {
            this.requestMessageWithDefaultUris.AbsoluteServiceUri.Should().BeSameAs(this.absoluteServiceUri);
            this.requestMessageWithDefaultUris.AbsoluteServiceUri = this.absoluteTestUri;
            this.requestMessageWithDefaultUris.AbsoluteServiceUri.Should().BeSameAs(this.absoluteTestUri);
        }

        [TestMethod]
        public void AbsoluteServiceUriCannotBeSetToNull()
        {
            Action setToNull = () => this.requestMessageWithDefaultUris.AbsoluteServiceUri = null;
            setToNull.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.RequestUriProcessor_AbsoluteServiceUriCannotBeNull);
        }

        [TestMethod]
        public void AbsoluteServiceUriCannotBeSetToRelative()
        {
            Action setToRelative = () => this.requestMessageWithDefaultUris.AbsoluteServiceUri = this.relativeTestUri;
            setToRelative.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.RequestUriProcessor_AbsoluteServiceUriMustBeAbsolute);
        }

        [TestMethod]
        public void AbsoluteServiceUriCannotBeSetAfterMarkedReadOnly()
        {
            this.requestMessageWithDefaultUris.MakeRequestAndServiceUrisReadOnly();
            Action changeValue = () => this.requestMessageWithDefaultUris.AbsoluteServiceUri = this.absoluteTestUri;
            changeValue.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.AstoriaRequestMessage_CannotModifyRequestOrServiceUriAfterReadOnly);
        }

        [TestMethod]
        public void AbsoluteServiceUriCanBeReadFromHostAfterMarkedReadOnly()
        {
            this.requestMessageWithDefaultUris.MakeRequestAndServiceUrisReadOnly();
            this.requestMessageWithDefaultUris.AbsoluteServiceUri.Should().BeSameAs(this.absoluteServiceUri);
        }

        [TestMethod]
        public void AbsoluteServiceUriFromHostShouldHaveTrailingSlashAppended()
        {
            var requestMessage = new AstoriaRequestMessage(new DataServiceHostSimulator { AbsoluteServiceUri = new Uri("http://temp.org") });
            requestMessage.AbsoluteServiceUri.Should().Be("http://temp.org/");
        }

        [TestMethod]
        public void AbsoluteServiceUriFromUserShouldHaveTrailingSlashAppended()
        {
            this.requestMessageWithDefaultUris.AbsoluteServiceUri = new Uri("http://temp.org");
            this.requestMessageWithDefaultUris.AbsoluteServiceUri.Should().Be("http://temp.org/");
        }

        [TestMethod]
        public void AbsoluteRequestUriShouldNotAllowQueryStringToBeAdded()
        {
            Action setWithQueryString = () => this.requestMessageWithDefaultUris.AbsoluteRequestUri = new Uri("http://somewhere.org/?newQueryString");
            setWithQueryString.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.AstoriaRequestMessage_CannotChangeQueryString);
        }

        [TestMethod]
        public void AbsoluteRequestUriShouldNotAllowQueryStringToBeChanged()
        {
            Action setWithDifferentQueryString = () => this.requestMessageWithQueryStrings.AbsoluteRequestUri = new Uri("http://somewhere.org/?newQueryString");
            setWithDifferentQueryString.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.AstoriaRequestMessage_CannotChangeQueryString);
        }

        [TestMethod]
        public void AbsoluteRequestUriShouldNotAllowQueryStringToBeRemoved()
        {
            Action setWithNoQueryString = () => this.requestMessageWithQueryStrings.AbsoluteRequestUri = new Uri("http://somewhere.org/");
            setWithNoQueryString.ShouldThrow<InvalidOperationException>().WithMessage(ErrorStrings.AstoriaRequestMessage_CannotChangeQueryString);
        }

        [TestMethod]
        public void AbsoluteRequestUriShouldAllowUriWithQueryStringToBeModifiedIfQueryStringIsUnchanged()
        {
            Action setWithSameQueryString = () => this.requestMessageWithQueryStrings.AbsoluteRequestUri = new Uri("http://somewhere.org/?originalRequestQueryString");
            setWithSameQueryString.ShouldNotThrow();
        }

        [TestMethod]
        public void AbsoluteServiceUriShouldAllowQueryStringToBeAdded()
        {
            Action setWithQueryString = () => this.requestMessageWithDefaultUris.AbsoluteServiceUri = new Uri("http://somewhere.org/?newQueryString");
            setWithQueryString.ShouldNotThrow();
        }

        [TestMethod]
        public void AbsoluteServiceUriShouldtAllowQueryStringToBeChanged()
        {
            Action setWithDifferentQueryString = () => this.requestMessageWithQueryStrings.AbsoluteServiceUri = new Uri("http://somewhere.org/?newQueryString");
            setWithDifferentQueryString.ShouldNotThrow();
        }

        [TestMethod]
        public void AbsoluteServiceUriShouldAllowQueryStringToBeRemoved()
        {
            Action setWithNoQueryString = () => this.requestMessageWithQueryStrings.AbsoluteServiceUri = new Uri("http://somewhere.org/");
            setWithNoQueryString.ShouldNotThrow();
        }

        [TestMethod]
        public void AbsoluteServiceUriShouldAllowUriWithQueryStringToBeModifiedIfQueryStringIsUnchanged()
        {
            Action setWithQueryString = () => this.requestMessageWithQueryStrings.AbsoluteServiceUri = new Uri("http://somewhere.org/?originalServiceQueryString");
            setWithQueryString.ShouldNotThrow();
        }

        private class AcceptableContentTypeSelectorSimulator : IAcceptableContentTypeSelector
        {
            public const string GetFormatReturnValue = "GetFormatSimulatorReturnValue";

            public string GetFormat(string dollarFormatValue, string acceptHeaderValue, Version maxDataServiceVersion)
            {
                return GetFormatReturnValue;
            }
        }

        private class NotImplementedHost : IDataServiceHost2
        {
            public Uri AbsoluteRequestUri
            {
                get { throw new NotImplementedException(); }
            }

            public Uri AbsoluteServiceUri
            {
                get { throw new NotImplementedException(); }
            }

            public string RequestAccept
            {
                get { return null; }
            }

            public string RequestAcceptCharSet
            {
                get { return null; }
            }

            public string RequestContentType
            {
                get { return null; }
            }

            public string RequestHttpMethod
            {
                get { throw new NotImplementedException(); }
            }

            public string RequestIfMatch
            {
                get { return null; }
            }

            public string RequestIfNoneMatch
            {
                get { return null; }
            }

            public string RequestMaxVersion
            {
                get { return null; }
            }

            public Stream RequestStream
            {
                get { throw new NotImplementedException(); }
            }

            public string RequestVersion
            {
                get { return null; }
            }

            public string ResponseCacheControl
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string ResponseContentType
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string ResponseETag
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string ResponseLocation
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public int ResponseStatusCode
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public Stream ResponseStream
            {
                get { throw new NotImplementedException(); }
            }

            public string ResponseVersion
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string GetQueryStringItem(string item)
            {
                throw new NotImplementedException();
            }

            public void ProcessException(HandleExceptionArgs args)
            {
                throw new NotImplementedException();
            }

            public WebHeaderCollection RequestHeaders
            {
                get { throw new NotImplementedException(); }
            }

            public WebHeaderCollection ResponseHeaders
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
