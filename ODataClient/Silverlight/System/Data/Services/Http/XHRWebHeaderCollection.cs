//---------------------------------------------------------------------
// <copyright file="XHRWebHeaderCollection.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      WebHeaderCollection type.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace System.Data.Services.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Contains protocol headers associated with a request or response.</summary>
    internal sealed class XHRWebHeaderCollection : WebHeaderCollection
    {
        /// <summary>Initial capacity of headers.</summary>
        private const int ApproxHighAvgNumHeaders = 16;

        /// <summary>Inner collection for headers.</summary>
        private NameValueFromDictionary innerCollection;

        /// <summary>Type of header collection.</summary>
        private System.Data.Services.Http.WebHeaderCollectionType collectionType;

        /// <summary>
        /// Initializes a new instance of the WebHeaderCollection class.
        /// </summary>
        public XHRWebHeaderCollection() : this(System.Data.Services.Http.WebHeaderCollectionType.Unknown)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WebHeaderCollection class
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of header collection created.</param>
        internal XHRWebHeaderCollection(System.Data.Services.Http.WebHeaderCollectionType type)
        {
            this.collectionType = type;
        }

        #region Properties.
        /// <summary>Gets the number of headers in the collection.</summary>
        public override int Count
        {
            get
            {
                return this.InnerCollection.Count;
            }
        }

        /// <summary>Collection of header names.</summary>
        public override ICollection<string> AllKeys
        {
            get
            {
                return this.InnerCollection.Keys;
            }
        }

        /// <summary>Whether HTTP request headers are allowed.</summary>
        private bool AllowHttpRequestHeader
        {
            get
            {
                if (this.collectionType == System.Data.Services.Http.WebHeaderCollectionType.Unknown)
                {
                    this.collectionType = System.Data.Services.Http.WebHeaderCollectionType.WebRequest;
                }

                return
                    ((this.collectionType == System.Data.Services.Http.WebHeaderCollectionType.WebRequest) ||
                     (this.collectionType == System.Data.Services.Http.WebHeaderCollectionType.HttpWebRequest));
            }
        }

        /// <summary>Internal collection with headers and values.</summary>
        private NameValueFromDictionary InnerCollection
        {
            get
            {
                if (this.innerCollection == null)
                {
                    this.innerCollection = new NameValueFromDictionary(ApproxHighAvgNumHeaders, System.Data.Services.Http.CaseInsensitiveAscii.StaticInstance);
                }

                return this.innerCollection;
            }
        }

        /// <summary>Gets or sets a named header.</summary>
        /// <param name="name">Header name.</param>
        /// <returns>The header value.</returns>
        public override string this[string name]
        {
            get
            {
                return this.InnerCollection.Get(name);
            }

            set
            {
                name = System.Data.Services.Http.ValidationHelper.CheckBadChars(name, false);
                value = System.Data.Services.Http.ValidationHelper.CheckBadChars(value, true);
                this.ThrowOnRestrictedHeader(name);
                this.InnerCollection.Set(name, value);
            }
        }

        /// <summary>Gets or sets a known request header.</summary>
        /// <param name="header">Header to get or set.</param>
        /// <returns>The header value.</returns>
        /// <remarks>Request headers are always allowed, the checks should be removed.</remarks>
        public override string this[System.Data.Services.Http.HttpRequestHeader header]
        {
            get
            {
                if (!this.AllowHttpRequestHeader)
                {
                    // throw new InvalidOperationException("SR.GetString(SR.net_headers_req)");
                    throw new InvalidOperationException(
                        System.Data.Services.Client.Strings.HttpWeb_Internal("WebHeaderCollection.this[HttpRequestHeader].get"));
                }

                return this[HttpHeaderToName.GetRequestHeaderName(header)];
            }

            set
            {
                if (!this.AllowHttpRequestHeader)
                {
                    // throw new InvalidOperationException("SR.GetString(SR.net_headers_req)");
                    throw new InvalidOperationException(
                        System.Data.Services.Client.Strings.HttpWeb_Internal("WebHeaderCollection.this[HttpRequestHeader].set"));
                }

                this[HttpHeaderToName.GetRequestHeaderName(header)] = value;
            }
        }
        #endregion Properties.

        /// <summary>Inserts a new header into the collection.</summary>
        /// <param name="name">Header name.</param>
        /// <param name="value">Header value.</param>
        /// <remarks>This method does no validation on its arguments.</remarks>
        internal void Add(string name, string value)
        {
            Debug.Assert(name != null, "name != null");
            Debug.Assert(value != null, "value != null");
            this.InnerCollection.Add(name, value);
        }

        /// <summary>Sets a header, checking that the value is valid for an HTTP header.</summary>
        /// <param name="headerName">Header name.</param>
        /// <param name="value">Header value, possibly null or empty.</param>
        internal void SetSpecialHeader(string headerName, string value)
        {
            Debug.Assert(headerName != null, "headerName != null");
            value = System.Data.Services.Http.ValidationHelper.CheckBadChars(value, true);
            this.InnerCollection.Remove(headerName);
            if (value.Length != 0)
            {
                this.InnerCollection.Add(headerName, value);
            }
        }

        /// <summary>Parses headers incrementally.</summary>
        /// <param name="byteBuffer">Buffer containing the data to be parsed.</param>
        /// <param name="size">Size of the buffer.</param>
        /// <param name="unparsed">Offset of data yet to be parsed.</param>
        /// <param name="totalResponseHeadersLength">Total length.</param>
        /// <param name="maximumResponseHeadersLength">Maximum length.</param>
        /// <param name="parseError">Error code.</param>
        /// <returns>Parsing status.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Method is a parser and cannot be easily split up.")]
        internal DataParseStatus ParseHeaders(
            byte[] byteBuffer,
            int size,
            ref int unparsed,
            ref int totalResponseHeadersLength,
            int maximumResponseHeadersLength,
            ref WebParseError parseError)
        {
            // This code is optimized for the case in which all the headers fit in the buffer.
            // we support multiple re-entrance, but we won't save intermediate
            // state, we will just roll back all the parsing done for the current header if we can't
            // parse a whole one (including multiline) or decide something else ("invalid data" or "done parsing").
            //
            // we're going to cycle through the loop until we
            //
            // 1) find an HTTP violation (in this case we return DataParseStatus.Invalid)
            // 2) we need more data (in this case we return DataParseStatus.NeedMoreData)
            // 3) we found the end of the headers and the beginning of the entity body (in this case we return DataParseStatus.Done)

            // quick check in the boundaries (as we use unsafe pointer)
            if (byteBuffer.Length < size)
            {
                return DataParseStatus.NeedMoreData;
            }

            char ch;
            int headerNameStartOffset = -1;
            int headerNameEndOffset = -1;
            int headerValueStartOffset = -1;
            int headerValueEndOffset = -1;
            int numberOfLf = -1;
            int index = unparsed;
            bool spaceAfterLf;
            string headerMultiLineValue;
            string headerName;
            string headerValue;

            // we need this because this method is entered multiple times.
            int localTotalResponseHeadersLength = totalResponseHeadersLength;

            WebParseErrorCode parseErrorCode = WebParseErrorCode.Generic;
            DataParseStatus parseStatus = DataParseStatus.Invalid;

            // according to RFC216 a header can have the following syntax:
            //
            // message-header = field-name ":" [ field-value ]
            // field-name     = token
            // field-value    = *( field-content | LWS )
            // field-content  = <the OCTETs making up the field-value and consisting of either *TEXT or combinations of token, separators, and quoted-string>
            // TEXT           = <any OCTET except CTLs, but including LWS>
            // CTL            = <any US-ASCII control character (octets 0 - 31) and DEL (127)>
            // SP             = <US-ASCII SP, space (32)>
            // HT             = <US-ASCII HT, horizontal-tab (9)>
            // CR             = <US-ASCII CR, carriage return (13)>
            // LF             = <US-ASCII LF, linefeed (10)>
            // LWS            = [CR LF] 1*( SP | HT )
            // CHAR           = <any US-ASCII character (octets 0 - 127)>
            // token          = 1*<any CHAR except CTLs or separators>
            // separators     = "(" | ")" | "<" | ">" | "@" | "," | ";" | ":" | "\" | <"> | "/" | "[" | "]" | "?" | "=" | "{" | "}" | SP | HT
            // quoted-string  = ( <"> *(qdtext | quoted-pair ) <"> )
            // qdtext         = <any TEXT except <">>
            // quoted-pair    = "\" CHAR

            // At each iteration of the following loop we expect to parse a single HTTP header entirely.
            for (;;)
            {
                // trim leading whitespaces (LWS) just for extra robustness, in fact if there are leading white spaces then:
                // 1) it could be that after the status line we might have spaces. handle this.
                // 2) this should have been detected to be a multiline header so there'll be no spaces and we'll spend some time here.
                headerName = string.Empty;
                headerValue = string.Empty;
                spaceAfterLf = false;
                headerMultiLineValue = null;

                if (this.Count == 0)
                {
                    // so, restrict this extra trimming only on the first header line
                    while (index < size)
                    {
                        ch = (char)byteBuffer[index];
                        if (ch == ' ' || ch == '\t')
                        {
                            ++index;
                            if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                            {
                                parseStatus = DataParseStatus.DataTooBig;
                                goto quit;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (index == size)
                    {
                        // we reached the end of the buffer. ask for more data.
                        parseStatus = DataParseStatus.NeedMoreData;
                        goto quit;
                    }
                }

                // what we have here is the beginning of a new header
                headerNameStartOffset = index;

                while (index < size)
                {
                    ch = (char)byteBuffer[index];
                    if (ch != ':' && ch != '\n')
                    {
                        if (ch > ' ')
                        {
                            // if there's an illegal character we should return DataParseStatus.Invalid
                            // instead we choose to be flexible, try to trim it, but include it in the string
                            headerNameEndOffset = index;
                        }

                        ++index;
                        if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                        {
                            parseStatus = DataParseStatus.DataTooBig;
                            goto quit;
                        }
                    }
                    else
                    {
                        if (ch == ':')
                        {
                            ++index;
                            if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                            {
                                parseStatus = DataParseStatus.DataTooBig;
                                goto quit;
                            }
                        }
                        
                        break;
                    }
                }
                
                if (index == size)
                {
                    // we reached the end of the buffer. ask for more data.
                    parseStatus = DataParseStatus.NeedMoreData;
                    goto quit;
                }

            startOfValue:
                // skip all [' ','\t','\r','\n'] characters until HeaderValue starts
                // if we didn't find any headers yet, we set numberOfLf to 1
                // so that we take the '\n' from the status line into account
                numberOfLf = (this.Count == 0 && headerNameEndOffset < 0) ? 1 : 0;
                while (index < size && numberOfLf < 2)
                {
                    ch = (char)byteBuffer[index];
                    if (ch <= ' ')
                    {
                        if (ch == '\n')
                        {
                            numberOfLf++;
                            
                            // In this case, need to check for a space.
                            if (numberOfLf == 1)
                            {
                                if (index + 1 == size)
                                {
                                    // we reached the end of the buffer. ask for more data.
                                    // need to be able to peek after the \n and see if there's some space.
                                    parseStatus = DataParseStatus.NeedMoreData;
                                    goto quit;
                                }

                                spaceAfterLf = (char)byteBuffer[index + 1] == ' ' || (char)byteBuffer[index + 1] == '\t';
                            }
                        }

                        ++index;
                        if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                        {
                            parseStatus = DataParseStatus.DataTooBig;
                            goto quit;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (numberOfLf == 2 || (numberOfLf == 1 && !spaceAfterLf))
                {
                    // if we've counted two '\n' we got at the end of the headers even if we're past the end of the buffer
                    // if we've counted one '\n' and the first character after that was a ' ' or a '\t'
                    // no matter if we found a ':' or not, treat this as an empty header name.
                    goto addHeader;
                }
                
                if (index == size)
                {
                    // we reached the end of the buffer. ask for more data.
                    parseStatus = DataParseStatus.NeedMoreData;
                    goto quit;
                }

                headerValueStartOffset = index;

                while (index < size)
                {
                    ch = (char)byteBuffer[index];
                    if (ch != '\n')
                    {
                        if (ch > ' ')
                        {
                            headerValueEndOffset = index;
                        }

                        ++index;
                        if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                        {
                            parseStatus = DataParseStatus.DataTooBig;
                            goto quit;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (index == size)
                {
                    // we reached the end of the buffer. ask for more data.
                    parseStatus = DataParseStatus.NeedMoreData;
                    goto quit;
                }

                // at this point we found either a '\n' or the end of the headers
                // hence we are at the end of the Header Line. 4 options:
                // 1) need more data
                // 2) if we find two '\n' => end of headers
                // 3) if we find one '\n' and a ' ' or a '\t' => multiline header
                // 4) if we find one '\n' and a valid char => next header
                numberOfLf = 0;
                while (index < size && numberOfLf < 2)
                {
                    ch = (char)byteBuffer[index];
                    if (ch == '\r' || ch == '\n')
                    {
                        if (ch == '\n')
                        {
                            numberOfLf++;
                        }
                        
                        ++index;
                        if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                        {
                            parseStatus = DataParseStatus.DataTooBig;
                            goto quit;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                
                if (index == size && numberOfLf < 2)
                {
                    // we reached the end of the buffer but not of the headers. ask for more data.
                    parseStatus = DataParseStatus.NeedMoreData;
                    goto quit;
                }

            addHeader:
                if (headerValueStartOffset >= 0 && headerValueStartOffset > headerNameEndOffset && headerValueEndOffset >= headerValueStartOffset)
                {
                    // Encoding fastest way to build the UNICODE string off the byte[]
                    ////headerValue = HeaderEncoding.GetString(byteBuffer + headerValueStartOffset, headerValueEndOffset - headerValueStartOffset + 1);
                    headerValue = System.Text.Encoding.UTF8.GetString(byteBuffer, headerValueStartOffset, headerValueEndOffset - headerValueStartOffset + 1);
                }

                // if we got here from the beginning of the for loop, headerMultiLineValue will be null
                // otherwise it will contain the headerValue constructed for the multiline header
                // add this line as well to it, separated by a single space
                headerMultiLineValue = (headerMultiLineValue == null ? headerValue : headerMultiLineValue + " " + headerValue);

                if (index < size && numberOfLf == 1)
                {
                    ch = (char)byteBuffer[index];
                    if (ch == ' ' || ch == '\t')
                    {
                        // since we found only one Lf and the next header line begins with a Lws,
                        // this is the beginning of a multiline header.
                        // parse the next line into headerValue later it will be added to headerMultiLineValue
                        ++index;
                        if (maximumResponseHeadersLength >= 0 && ++localTotalResponseHeadersLength >= maximumResponseHeadersLength)
                        {
                            parseStatus = DataParseStatus.DataTooBig;
                            goto quit;
                        }

                        goto startOfValue;
                    }
                }

                if (headerNameStartOffset >= 0 && headerNameEndOffset >= headerNameStartOffset)
                {
                    // Encoding is the fastest way to build the UNICODE string off the byte[]
                    ////headerName = HeaderEncoding.GetString(byteBuffer + headerNameStartOffset, headerNameEndOffset - headerNameStartOffset + 1);
                    headerName = System.Text.Encoding.UTF8.GetString(byteBuffer, headerNameStartOffset, headerNameEndOffset - headerNameStartOffset + 1);
                }

                // now it's finally safe to add the header if we have a name for it
                if (headerName.Length > 0)
                {
                    // the base class will check for pre-existing headerValue and append
                    // it using commas as indicated in the RFC
                    this.Add(headerName, headerMultiLineValue);
                }

                // and update unparsed
                totalResponseHeadersLength = localTotalResponseHeadersLength;
                unparsed = index;

                if (numberOfLf == 2)
                {
                    parseStatus = DataParseStatus.Done;
                    goto quit;
                }
            }

            quit:
            if (parseStatus == DataParseStatus.Invalid)
            {
                parseError.Section = WebParseErrorSection.ResponseHeader;
                parseError.Code = parseErrorCode;
            }

            return parseStatus;
        }
        
        /// <summary>
        /// Throws an exception if the specified <paramref name="headerName"/> is restricted
        /// for this collection type.
        /// </summary>
        /// <param name="headerName">Name of header to check.</param>
        private void ThrowOnRestrictedHeader(string headerName)
        {
            if ((this.collectionType == System.Data.Services.Http.WebHeaderCollectionType.HttpWebRequest) && HeaderInfoTable.GetHeaderInfo(headerName).IsRequestRestricted)
            {
                // throw new ArgumentException(!object.Equals(headerName, "Host") ? "SR.GetString(SR.net_headerrestrict)" : "SR.GetString(SR.net_headerrestrict_resp, HttpKnownHeaderNames.Host), headerName");
                throw new InvalidOperationException(
                    System.Data.Services.Client.Strings.HttpWeb_Internal("WebHeaderCollection.ThrowOnRestrictedHeader"));
            }
        }        
    }
}
