namespace Microsoft.Epm.Peachy.Tests //// TODO get the right namespace
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json.Nodes;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class EpmEndToEndTests //// TODO probably this should go in a microsoft.epm.testcore to be shared across the peachy implementation, the webapi implementation, etc
    {
        private readonly Uri rootUri;

        public EpmEndToEndTests(Uri rootUri)
        {
            if (rootUri == null)
            {
                throw new ArgumentNullException(nameof(rootUri));
            }

            this.rootUri = rootUri;
        }

        public async Task GetAuthorizationSystem()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1 HTTP/1.1


HTTP/1.1 200 OK
****

{
  "id": "1",
  "authorizationSystemName": "chrispre auth system",
  "authorizationSystemType": "aws"
}
"""
);
        }

        public async Task _1()
        {
            await SampleAsync(
"""
GET /extern HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"There is no singleton or entity set defined with name 'extern'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _2()
        {
            await SampleAsync(
"""
GET /external HTTP/1.1


HTTP/1.1 200 OK
****

{
}
"""
);
        }

        public async Task _3()
        {
            await SampleAsync(
"""
GET /external/auth HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"The path '/external' refers to an entity of type 'microsoft.graph.externalConnectors.external'. There is no property with name 'auth' defined on 'microsoft.graph.externalConnectors.external'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _4()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": [
    {
      "id": "1",
      "authorizationSystemName": "chrispre auth system",
      "authorizationSystemType": "aws"
    },
    {
      "id": "2",
      "authorizationSystemName": "mikep auth system",
      "authorizationSystemType": "azure"
    },
    {
      "id": "3",
      "authorizationSystemName": "gdebruin auth system",
      "authorizationSystemType": "gcp"
    }
  ]
}
"""
);
        }

        public async Task _5()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/doesntexist HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"Not entity with key 'doesntexist' found in the collection at '/external/authorizationSystems'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _6()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2 HTTP/1.1


HTTP/1.1 200 OK
****

{
  "id": "2",
  "authorizationSystemName": "mikep auth system",
  "authorizationSystemType": "azure"
}
"""
);
        }

        public async Task _7()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/foo HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"The path '/external/authorizationSystems/2' refers to an entity of type 'microsoft.graph.authorizationSystem'. There is no property with name 'foo' defined on 'microsoft.graph.authorizationSystem'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _8()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/id HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": "2"
}
"""
);
        }

        public async Task _9()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/authorizationSystemName HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": "mikep auth system"
}
"""
);
        }

        public async Task _10()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/authorizationSystemType HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": "azure"
}
"""
);
        }

        public async Task _11()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/associatedIdentities HTTP/1.1


HTTP/1.1 200 OK
****

{
}
"""
);
        }

        public async Task _12()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/associatedIdentities/foo HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"The path '/external/authorizationSystems/2/associatedIdentities' refers to an instance of the complex type with name 'microsoft.graph.associatedIdentities'. There is no property with name 'foo' defined on 'microsoft.graph.associatedIdentities'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _13()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/2/associatedIdentities/all HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": [
  ]
}
"""
);
        }

        public async Task _14()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": [
    {
      "id":"first",
      "displayName":"adam"
    },
    {
      "id":"second",
      "displayName":"jessie"
    },
    {
      "id":"third",
      "displayName":"a role"
    }
  ]
}
"""
);
        }

        public async Task _15()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/doesntexist HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"No entity with key 'doesntexist' found in the collection at '/external/authorizationSystems/1/associatedIdentities/all'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _16()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/third HTTP/1.1


HTTP/1.1 200 OK
****

{
  "id":"third",
  "displayName":"a role"
}
"""
);
        }

        public async Task _17()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first HTTP/1.1


HTTP/1.1 200 OK
****

{
  "id":"first",
  "displayName":"adam"
}
"""
);
        }

        public async Task _18()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/foo HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"The path '/external/authorizationSystems/1/associatedIdentities/all/first' refers to an instance of the entity type with name 'microsoft.graph.authorizationSystemIdentity'. There is no property with name 'foo' defined on 'microsoft.graph.authorizationSystemIdentity'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _19()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/id HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": "first"
}
"""
);
        }

        public async Task _20()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/displayName HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": "adam"
}
"""
);
        }

        public async Task _21()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/assumableRoles HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"The path '/external/authorizationSystems/1/associatedIdentities/all/first' refers to an instance of the entity type with name 'microsoft.graph.authorizationSystemIdentity'. There is no property with name 'assumableRoles' defined on 'microsoft.graph.authorizationSystemIdentity'.",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _22()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser HTTP/1.1


HTTP/1.1 200 OK
****

{
  "id":"first",
  "displayName":"adam"
}
"""
);
        }

        public async Task _23()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser/id HTTP/1.1


HTTP/1.1 501 NotImplemented
****

{
  "code":"NotImplemented",
  "message":"TODO this functionality has nopt been implemented in peachy",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _24()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser/displayName HTTP/1.1


HTTP/1.1 501 NotImplemented
****

{
  "code":"NotImplemented",
  "message":"TODO this functionality has nopt been implemented in peachy",
  "target":null,
  "details":null
}
"""
);
        }

        public async Task _25()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser/assumableRoles HTTP/1.1


HTTP/1.1 200 OK
****

{
  "value": [
    {
      "id": "third",
      "displayName": "a role"
    }
  ]
}
"""
);
        }

        public async Task _26()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser/assumableRoles/doesntexist HTTP/1.1


HTTP/1.1 404 NotFound
****

{
  "code":"NotFound",
  "message":"No entity with key 'doesntexist' found in the collection at '/external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser/assumableRoles'.",
  "target":null,
  "details":null}
"""
);
        }

        public async Task _27()
        {
            await SampleAsync(
"""
GET /external/authorizationSystems/1/associatedIdentities/all/first/graph.awsUser/assumableRoles/third HTTP/1.1


HTTP/1.1 200 OK
****

{
  "id": "third",
  "displayName": "a role"
}
"""
);
        }

        private async Task SampleAsync(string sampleText)
        {
            var sample = await ParseSampleAsync(sampleText).ConfigureAwait(false);
            using (var httpClient = new HttpClient())
            {
                foreach (var header in sample.Request.Headers)
                {
                    var headerParts = header.Split(':', 2);
                    if (headerParts.Length == 0)
                    {
                        throw new InvalidOperationException("TODO invalid header");
                    }

                    httpClient.DefaultRequestHeaders.Add(headerParts[0], headerParts.Length > 1 ? headerParts[1] : string.Empty);
                }

                HttpResponseMessage? httpResponseMessage = null;
                try
                {
                    if (string.Equals(sample.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                    {
                        httpResponseMessage = await httpClient.GetAsync(new Uri(this.rootUri, sample.Request.Url)).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new Exception("TODO unknown http method");
                    }

                    Assert.AreEqual(sample.Response.StatusCode, (int)httpResponseMessage.StatusCode);
                    Assert.AreEqual(sample.Response.StatusText, httpResponseMessage.ReasonPhrase?.Replace(" ", string.Empty));

                    var actualResponseHeaders = httpResponseMessage.Headers.SelectMany(_ => _.Value.Select(value => $"{_.Key}: {value}"));
                    using (var expectedHeadersEnumerator = sample.Response.Headers.GetEnumerator())
                    using (var actualHeadersEnumerator = actualResponseHeaders.GetEnumerator())
                    {
                        bool expectedMore = expectedHeadersEnumerator.MoveNext();
                        bool actuallyMore = actualHeadersEnumerator.MoveNext();
                        Assert.AreEqual(expectedMore, actuallyMore);
                        while (expectedMore && actuallyMore)
                        {
                            var expectedHeader = expectedHeadersEnumerator.Current;
                            var actualHeader = actualHeadersEnumerator.Current;
                            if (!string.Equals(expectedHeader, actualHeader, StringComparison.OrdinalIgnoreCase))
                            {
                                if (!string.Equals(expectedHeader, "****", StringComparison.OrdinalIgnoreCase))
                                {
                                    Assert.Fail("expected header wasn't found");
                                }
                            }
                            else
                            {
                                expectedMore = expectedHeadersEnumerator.MoveNext();
                            }

                            actuallyMore = actualHeadersEnumerator.MoveNext();
                        }

                        if (expectedMore ^ actuallyMore && !string.Equals(expectedHeadersEnumerator.Current, "****", StringComparison.OrdinalIgnoreCase))
                        {
                            Assert.Fail("not all expected headers were found");
                        }
                    }


                    var expectedBody = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(sample.Response.Body));
                    var actualBody = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false)));
                    Assert.AreEqual(expectedBody, actualBody);
                }
                finally
                {
                    httpResponseMessage?.Dispose();
                }
            }
        }

        private static async Task<HttpResponse> ParseSampleResponseAsync(StringReader stringReader)
        {
            var responseBuilder = new HttpResponse.Builder();

            {
                var statusLine = await stringReader.ReadLineAsync().ConfigureAwait(false);
                if (statusLine == null)
                {
                    throw new InvalidOperationException("TODO sample doesn't have any response");
                }

                var httpText = "HTTP/1.1";
                var postHttpIndex = statusLine.IndexOf(httpText) + httpText.Length;
                var statusCodeEndIndex = statusLine.LastIndexOf(' ');
                if (statusCodeEndIndex == -1)
                {
                    throw new InvalidOperationException("TODO sample either doesn't have a response status code or a response status text");
                }

                var statusCodeText = statusLine.Substring(postHttpIndex + 1, statusCodeEndIndex - postHttpIndex).Trim();
                responseBuilder.StatusCode = int.Parse(statusCodeText);

                responseBuilder.StatusText = statusLine.Substring(statusCodeEndIndex).Trim();
            }

            {
                string? headerLine;
                while (!string.IsNullOrEmpty(headerLine = await stringReader.ReadLineAsync().ConfigureAwait(false)))
                {
                    //// TODO why is there a warning for this?
                    responseBuilder.Headers.Add(headerLine);
                }
            }

            {
                responseBuilder.Body = await stringReader.ReadToEndAsync().ConfigureAwait(false);
            }

            return responseBuilder.Build();
        }

        private sealed class HttpResponse
        {
            private HttpResponse(int statusCode, string statusText, IEnumerable<string> headers, string body)
            {
                this.StatusCode = statusCode;
                this.StatusText = statusText;
                this.Headers = headers;
                this.Body = body;
            }

            public int StatusCode { get; }

            public string StatusText { get; }

            public IEnumerable<string> Headers { get; }

            public string Body { get; }

            public sealed class Builder
            {
                public int? StatusCode { get; set; }

                public string? StatusText { get; set; }

                public List<string>? Headers { get; set; } = new List<string>();

                public string? Body { get; set; }

                public HttpResponse Build()
                {
                    if (this.StatusCode == null)
                    {
                        throw new ArgumentNullException(nameof(this.StatusCode));
                    }

                    if (this.StatusText == null)
                    {
                        throw new ArgumentNullException(nameof(this.StatusText));
                    }

                    if (this.Headers == null)
                    {
                        throw new ArgumentNullException(nameof(this.Headers));
                    }

                    if (this.Body == null)
                    {
                        throw new ArgumentNullException(nameof(this.Body));
                    }

                    return new HttpResponse(this.StatusCode.Value, this.StatusText, this.Headers.ToList(), this.Body);
                }
            }
        }

        private static async Task<HttpRequest> ParseSampleRequestAsync(StringReader stringReader)
        {
            var requestBuilder = new HttpRequest.Builder();

            {
                var startLine = await stringReader.ReadLineAsync().ConfigureAwait(false);
                if (startLine == null)
                {
                    throw new InvalidOperationException("TODO sample doesn't have any characters");
                }

                var firstSpaceIndex = startLine.IndexOf(' ');
                if (firstSpaceIndex == -1)
                {
                    throw new InvalidOperationException("TODO sample either doesn't have an http method or it doesn't have a url");
                }

                requestBuilder.HttpMethod = startLine.Substring(0, firstSpaceIndex);
                var httpIndex = startLine.LastIndexOf(" HTTP/1.1");
                requestBuilder.Url = startLine.Substring(firstSpaceIndex, (httpIndex == -1 ? startLine.Length : httpIndex) - firstSpaceIndex).Trim();
            }

            {
                string? headerLine;
                while (!string.IsNullOrEmpty(headerLine = await stringReader.ReadLineAsync().ConfigureAwait(false)))
                {
                    //// TODO why is there a warning for this?
                    requestBuilder.Headers.Add(headerLine);
                }
            }

            {
                var stringBuilder = new StringBuilder();
                string? line;
                while (!string.IsNullOrEmpty(line = await stringReader.ReadLineAsync().ConfigureAwait(false)))
                {
                    stringBuilder.AppendLine(line);
                }

                if (line == null)
                {
                    throw new InvalidOperationException("TODO no response body defined");
                }

                requestBuilder.Body = stringBuilder.ToString();
            }

            return requestBuilder.Build();
        }

        private sealed class HttpRequest
        {
            private HttpRequest(string httpMethod, string url, IEnumerable<string> headers, string body)
            {
                this.HttpMethod = httpMethod;
                this.Url = url;
                this.Headers = headers;
                this.Body = body;
            }

            public string HttpMethod { get; }

            public string Url { get; }

            public IEnumerable<string> Headers { get; }

            public string Body { get; }

            public sealed class Builder
            {
                public string? HttpMethod { get; set; }

                public string? Url { get; set; }

                public List<string>? Headers { get; set; } = new List<string>();

                public string? Body { get; set; }

                public HttpRequest Build()
                {
                    if (this.HttpMethod == null)
                    {
                        throw new ArgumentNullException(nameof(this.HttpMethod));
                    }

                    if (this.Url == null)
                    {
                        throw new ArgumentNullException(nameof(this.Url));
                    }

                    if (this.Headers == null)
                    {
                        throw new ArgumentNullException(nameof(this.Headers));
                    }

                    if (this.Body == null)
                    {
                        throw new ArgumentNullException(nameof(this.Body));
                    }

                    return new HttpRequest(this.HttpMethod, this.Url, this.Headers.ToList(), this.Body);
                }
            }
        }

        private static async Task<(HttpRequest Request, HttpResponse Response)> ParseSampleAsync(string sample)
        {
            using (var stringReader = new StringReader(sample))
            {
                var request = await ParseSampleRequestAsync(stringReader);
                var response = await ParseSampleResponseAsync(stringReader);

                return (request, response);
            }
        }

        public async Task GetInvalidSegment()
        {
            using (var httpClient = new HttpClient())
            {
                //// TODO make this look more like the api.md
                using (var httpResponse = await httpClient.GetAsync(new Uri(this.rootUri, "foo")))
                {
                    Assert.AreEqual(501, (int)httpResponse.StatusCode);
                    Assert.AreEqual("TODO this portion of the OData standard has not been implemented", await httpResponse.Content.ReadAsStringAsync());
                }
            }
        }
    }
}
