//---------------------------------------------------------------------
// <copyright file="Request.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.Reliability
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Http Request
    /// </summary>
    public class Request
    {
        public const string Separator = ":";

        private static readonly Dictionary<string, string> LocalDNSCache = new Dictionary<string, string>();

        private readonly RequestCachePolicy cachePolicy =
            new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

        private string name;

        private WebHeaderCollection headers = new WebHeaderCollection();

        private X509CertificateCollection clientCertificates = new X509CertificateCollection();

        private CookieCollection cookies = new CookieCollection();

        private HttpMethod method = HttpMethod.Get;

        // Default HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\TcpTimedWaitDelay = 240 seconds
        // We will wait 240 second for dead connection release, in case of intensive connection scenario like fuzz, data driven functional
        private int timeout = 240000;

        private bool allowAutoRedirect = true;

        private object payloadObject;

        /// <summary>
        /// Initializes a new instance of the Request class
        /// </summary>
        /// <param name="requestUrl">the request url</param>
        public Request(string requestUrl)
        {
            this.RequestUrl = requestUrl;
        }

        /// <summary>
        /// Gets or sets name
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.name))
                {
                    return this.RequestUrl;
                }

                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Gets or sets user agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets request Url
        /// </summary>
        public string RequestUrl { get; internal set; }

        /// <summary>
        /// Gets or sets ContentType
        /// </summary>
        public HttpContentType? ContentType { get; set; }

        /// <summary>
        /// Gets or sets headers
        /// </summary>
        public WebHeaderCollection Headers
        {
            get { return this.headers; }
            set { this.headers = value; }
        }

        /// <summary>
        /// Gets or sets client certificates
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get { return this.clientCertificates; }
            set { this.clientCertificates = value; }
        }

        /// <summary>
        /// Gets cache policy
        /// </summary>
        public RequestCachePolicy CachePolicy
        {
            get { return this.cachePolicy; }
        }

        /// <summary>
        /// Gets or sets cookies
        /// </summary>
        public CookieCollection Cookies
        {
            get { return this.cookies; }
            set { this.cookies = value; }
        }

        /// <summary>
        /// Gets or sets http method
        /// </summary>
        public HttpMethod Method
        {
            get { return this.method; }
            set { this.method = value; }
        }

        /// <summary>
        /// Gets or sets payload
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets timeout
        /// </summary>
        public int Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }

        /// <summary>
        /// Gets or sets accept encoding
        /// </summary>
        public string AcceptEncoding
        {
            get { return this.Headers[HttpRequestHeader.AcceptEncoding]; }
            set { this.Headers[HttpRequestHeader.AcceptEncoding] = value; }
        }

        /// <summary>
        /// Gets or sets accept
        /// </summary>
        public HttpContentType? Accept { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allow auto redirect
        /// </summary>
        public bool AllowAutoRedirect
        {
            get { return this.allowAutoRedirect; }
            set { this.allowAutoRedirect = value; }
        }

        /// <summary>
        /// Gets or sets payload object
        /// </summary>
        public object PayloadObject
        {
            get
            {
                return this.payloadObject;
            }

            set
            {
                this.payloadObject = value;
                var serializer = new DataContractSerializer(value.GetType());
                using (var backing = new StringWriter())
                {
                    using (var writer = new XmlTextWriter(backing))
                    {
                        serializer.WriteObject(writer, value);
                        this.Payload = backing.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a new http  request
        /// </summary>
        private HttpWebRequest HttpRequest
        {
            get
            {
                string uri = this.RequestUrl;
                bool bypassDNS = ResolveDNS(ref uri);
                var httpRequest = (HttpWebRequest)WebRequest.Create(uri);
                if (bypassDNS)
                {
                    httpRequest.Proxy = null;
                }

                if (this.Headers != null)
                {
                    if (this.Headers[HttpRequestHeader.Accept] != null)
                    {
                        httpRequest.Accept = this.Headers[HttpRequestHeader.Accept];
                        this.Headers.Remove(HttpRequestHeader.Accept);
                    }

                    httpRequest.Headers.Add(this.Headers);
                }

                if (this.ClientCertificates != null)
                {
                    foreach (var cer in this.ClientCertificates)
                    {
                        httpRequest.ClientCertificates.Add(cer);
                    }
                }

                httpRequest.Method = this.Method.ToString().ToUpper();
                if (this.Timeout > 0)
                {
                    httpRequest.Timeout = this.Timeout;
                }

                httpRequest.CachePolicy = this.CachePolicy;
                httpRequest.AllowAutoRedirect = this.AllowAutoRedirect;
                if (this.ContentType != null)
                {
                    httpRequest.ContentType = this.ContentType.Value.ToHttpHeader();
                }

                if (this.Accept != null)
                {
                    httpRequest.Accept = this.Accept.Value.ToHttpHeader();
                }

                if (!string.IsNullOrEmpty(this.UserAgent))
                {
                    httpRequest.UserAgent = this.UserAgent;
                }

                if (this.Cookies != null && this.Cookies.Count != 0)
                {
                    httpRequest.Headers.Add(HttpRequestHeader.Cookie, this.Cookies.GetHttpHeader());
                }

                if (!string.IsNullOrEmpty(this.Payload))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(this.Payload);
                    httpRequest.ContentLength = buffer.Length;
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                else
                {
                    httpRequest.ContentLength = 0;
                }

                return httpRequest;
            }
        }

        /// <summary>
        /// 1. Setup http connection limitation to 150. (default is 2)
        /// 2. Use local DNS to avoid DNS lookup
        /// 3. Ignore the certificate error
        /// </summary>
        /// <param name="uri">The uri</param>
        public static void PerfSetup(string uri)
        {
            if (!string.IsNullOrWhiteSpace(uri) && Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            {
                // ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 1000;

                // http://support.microsoft.com/default.aspx?scid=kb;EN-US;915599
                // Release idle connection after 20 seconds, to aviod Exception : The underlying connection was closed: A connection that was expected to be kept alive was closed by the server.
                ServicePointManager.MaxServicePointIdleTime = 20000;

                CacheDNSResult(uri);

                // Force to accept all certs we may get from https server
                if (ServicePointManager.ServerCertificateValidationCallback == null)
                {
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (sender, certificate, chain, sslPolicyErrors) => true;
                }
            }
        }

        /// <summary>
        /// Get response
        /// </summary>
        /// <param name="readResponse">If read the whole response</param>
        /// <returns>The response object</returns>
        public Response GetReponse(bool readResponse)
        {
            Response ret = null;
            HttpWebRequest request = this.HttpRequest;
            try
            {
                ret = new Response((HttpWebResponse)request.GetResponse());
            }
            catch (WebException e)
            {
                ret = new Response(e);
            }
            finally
            {
                if (readResponse && ret != null)
                {
                    ret.Read();
                }
            }

            return ret;
        }

        /// <summary>
        /// Get response and read the whole response
        /// </summary>
        /// <returns>The response</returns>
        public Response GetReponse()
        {
            return this.GetReponse(true);
        }

        /// <summary>
        /// To String overload
        /// </summary>
        /// <returns>return string</returns>
        public override string ToString()
        {
            return this.Name;
        }

        private static void CacheDNSResult(string uri)
        {
            var host = new Uri(uri).Host.ToLower();
            IPHostEntry hostips = Dns.GetHostEntry(host);
            IPAddress ip =
                hostips.AddressList.FirstOrDefault(hostip => hostip.AddressFamily == AddressFamily.InterNetwork);
            if (ip != null)
            {
                LocalDNSCache[host] = ip.ToString();
            }
        }

        private static bool ResolveDNS(ref string uri)
        {
            var host = new Uri(uri).Host.ToLower();
            IPAddress ip;
            if (IPAddress.TryParse(host, out ip))
            {
                return true;
            }

            if (LocalDNSCache.ContainsKey(host))
            {
                uri = Regex.Replace(uri, Regex.Escape(host), LocalDNSCache[host], RegexOptions.IgnoreCase);
                return true;
            }

            return false;
        }
    }
}
