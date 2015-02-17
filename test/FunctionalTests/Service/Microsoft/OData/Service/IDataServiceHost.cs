//---------------------------------------------------------------------
// <copyright file="IDataServiceHost.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.IO;
    using System.Net;

    #endregion Namespaces

    /// <summary>
    /// Provides access to the environment for a DataService,
    /// including information about the current request.
    /// </summary>
    public interface IDataServiceHost
    {
        #region Properties

        /// <summary>Gets an absolute URI that is the URI as sent by the client.</summary>
        /// <returns>A string that is the absolute URI of the request.</returns>
        Uri AbsoluteRequestUri
        {
            get;
        }

        /// <summary>Gets an absolute URI that is the root URI of the data service.</summary>
        /// <returns>A string that is the absolute root URI of the data service.</returns>
        Uri AbsoluteServiceUri
        {
            get;
        }

        /// <summary>The transport protocol specified by the request accept header.</summary>
        /// <returns>String that indicates the transport protocol required by the request.</returns>
        string RequestAccept
        {
            get;
        }

        /// <summary>Gets a string representing the value of the Accept-Charset HTTP header.</summary>
        /// <returns>String representing the value of the Accept-Charset HTTP header.</returns>
        string RequestAcceptCharSet
        {
            get;
        }

        /// <summary>Gets the transport protocol specified by the content type header.</summary>
        /// <returns>String value that indicates content type.</returns>
        string RequestContentType
        {
            get;
        }

        /// <summary>Gets the request methods such as GET, PUT, POST, PATCH or DELETE.</summary>
        /// <returns>String value that indicates request method.</returns>
        string RequestHttpMethod
        {
            get;
        }

        /// <summary>Gets the value for the If-Match header on the current request.</summary>
        /// <returns>String value for the If-Match header on the current request.</returns>
        string RequestIfMatch
        {
            get;
        }

        /// <summary>Gets the value for the If-None-Match header on the current request.</summary>
        /// <returns>String value for the If-None-Match header on the current request.</returns>
        string RequestIfNoneMatch
        {
            get;
        }

        /// <summary>Gets the value that identifies the highest version that the request client is able to process.</summary>
        /// <returns>A string that contains the highest version that the request client is able to process, possibly null.</returns>
        string RequestMaxVersion
        {
            get;
        }

        /// <summary>Gets the stream that contains the HTTP request body.</summary>
        /// <returns><see cref="T:System.IO.Stream" /> object that contains the request body.</returns>
        Stream RequestStream
        {
            get;
        }

        /// <summary>Gets the value that identifies the version of the request that the client submitted, possibly null.</summary>
        /// <returns>A string that identifies the version of the request that the client submitted, possibly null.</returns>
        string RequestVersion
        {
            get;
        }

        /// <summary>Gets a string value that represents cache control information.</summary>
        /// <returns>A string value that represents cache control information.</returns>
        string ResponseCacheControl
        {
            get;
            set;
        }

        /// <summary>Gets the transport protocol of the response.</summary>
        /// <returns>String value containing the content type.</returns>
        string ResponseContentType
        {
            get;
            set;
        }

        /// <summary>Gets an eTag value that represents the state of data in response.</summary>
        /// <returns>A string value that represents the eTag state value.</returns>
        string ResponseETag
        {
            get;
            set;
        }

        /// <summary>Gets or sets the service location.</summary>
        /// <returns>String that contains the service location.</returns>
        string ResponseLocation
        {
            get;
            set;
        }

        /// <summary>Gets or sets the response code that indicates results of query.</summary>
        /// <returns>Integer value that contains the response code.</returns>
        int ResponseStatusCode
        {
            get;
            set;
        }

        /// <summary>Gets the response stream to which the HTTP response body will be written.</summary>
        /// <returns><see cref="T:System.IO.Stream" /> object to which the response body will be written.</returns>
        Stream ResponseStream
        {
            get;
        }

        /// <summary>Gets the version used by the host in the response.</summary>
        /// <returns>A string value that contains the host version.</returns>
        string ResponseVersion
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>Gets a data item identified by the identity key contained by the parameter of the method.</summary>
        /// <returns>The data item requested by the query serialized as a string.</returns>
        /// <param name="item">String value containing identity key of item requested.</param>
        string GetQueryStringItem(string item);

        /// <summary>Handles a data service exception using information in  the <paramref name="args" /> parameter.</summary>
        /// <param name="args"><see cref="T:Microsoft.OData.Service.HandleExceptionArgs" />  that contains information on the exception object.</param>
        void ProcessException(HandleExceptionArgs args);

        #endregion Methods
    }

    /// <summary>
    /// Extends IDataServiceHost to include extra request and response headers.
    /// </summary>
    public interface IDataServiceHost2 : IDataServiceHost
    {
        /// <summary>Request header for an HTTP request.</summary>
        /// <returns>String value of header.</returns>
        WebHeaderCollection RequestHeaders
        {
            get;
        }

        /// <summary>Response header for an HTTP response. </summary>
        /// <returns>String value of header.</returns>
        WebHeaderCollection ResponseHeaders
        {
            get;
        }
    }
}
