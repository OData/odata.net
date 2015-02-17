//---------------------------------------------------------------------
// <copyright file="IDataServiceStreamProvider2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.IO;

    /// <summary>
    /// The IDataServiceStreamProvider2 interface defines the contract between the data services framework server component
    /// and a data source's named stream implementation (ie. a stream provider).
    /// </summary>
    public interface IDataServiceStreamProvider2 : IDataServiceStreamProvider
    {
        /// <summary>Returns a stream that contains the binary data for the named stream.</summary>
        /// <returns>A valid stream the data service use to query / read a named stream which is associated with the <paramref name="entity" />. Null may be returned from this method if the requested named stream has not been created since the creation of <paramref name="entity" />. The data service will respond with 204 if this method returns null.</returns>
        /// <param name="entity">The entity to which the named stream belongs.</param>
        /// <param name="streamProperty">A <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> object that represents the named stream.</param>
        /// <param name="etag">The eTag value sent as part of the HTTP request that is sent to the data service.</param>
        /// <param name="checkETagForEquality">A nullable <see cref="T:System.Boolean" /> value that determines what kind of conditional request was issued to the data service, which is true when the eTag was sent in an If-Match header, false when the eTag was sent in an If-None-Match header, and null when the request was not conditional and no eTag was included in the request. </param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to retrieve the named stream associated
        /// with the entity instance specified by the <paramref name="entity"/> parameter.
        /// 
        /// Notes to Interface Implementers
        /// The <paramref name="operationContext"/> argument is passed as it is likely that an implementer of this interface method
        /// will need information from the HTTP request headers in order to construct a stream.  Likely header
        /// values required are: 
        ///   'Accept'
        ///   'Accept-Charset'
        ///   'Accept-Encoding'
        ///   
        /// An implementer of this method MUST perform concurrency checks as needed in their implementation of
        /// this method.  If an If-Match or If-None-Match request header was included in the request, then the
        /// etag parameter will be non null, which indicates this method MUST perform the appropriate concurrency
        /// check.  If the concurrency check passes, this method should return the requested stream.  If the
        /// concurrency checks fails, the method should throw  a DataServiceException with the appropriate HTTP
        /// response code as defined in HTTP RFC 2616 section 14.24 and section 14.26.
        ///   If the etag was sent as the value of an If-Match request header, the value of the �checkETagForEquality�
        ///   header will be set to true
        ///   If the etag was sent as the value of an If-None-Match request header, the value of the
        ///   �checkETagForEquality� header will be set to false
        ///
        /// It is the responsibility of the implementer of this method to honor the values of the appropriate request
        /// headers when generating the returned response stream.
        /// 
        /// An implementer of this method MUST NOT set the following HTTP response headers on the <paramref name="operationContext"/> parameter
        /// as they are set by the data service runtime:
        ///   Content-Type
        ///   ETag   
        /// An implementer of this method may set HTTP response headers (other than those forbidden above) on
        /// the <paramref name="operationContext"/> parameter.
        ///
        /// An implementer of this method should only set the properties on the <paramref name="operationContext"/> parameter which it
        /// requires to be set for a successful response.  Altering other properties on the <paramref name="operationContext"/> parameter may
        /// corrupt the response from the data service.
        /// 
        /// Null may be returned from this method if the requested named stream has not been created since the creation of <paramref name="entity"/>.
        /// The data service will respond with 204 if this method returns null. If the stream returned from this method contains 0 byte, this method
        /// may set the response status code on the <paramref name="operationContext"/> parameter to 204.  The GetStreamContentType method should
        /// return null or string.Empty if the current request will respond with 204.
        /// 
        /// Stream Ownership
        /// The data service framework will dispose the stream once all bytes have been successfully read.
        /// 
        /// If an error occurs while reading the stream, then the data services framework will generate an
        /// in-stream error which is sent back to the client.  See the error contract specification for a 
        /// description of the format of in-stream errors.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "etag is not Hungarian notation")]
        Stream GetReadStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext);

        /// <summary>Returns the stream that the data service uses to write the binary data received from the client as the specified named stream.</summary>
        /// <returns>A valid <see cref="T:System.Stream" /> the data service uses to write the contents of a binary data received from the client.</returns>
        /// <param name="entity">The entity to which the named stream belongs.</param>
        /// <param name="streamProperty">A <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> object that represents the named stream.</param>
        /// <param name="etag">The eTag value sent as part of the HTTP request that is sent to the data service.</param>
        /// <param name="checkETagForEquality">A nullable <see cref="T:System.Boolean" /> value that determines what kind of conditional request was issued to the data service, which is true when the eTag was sent in an If-Match header, false when the eTag was sent in an If-None-Match header, and null when the request was not conditional and no eTag was included in the request. </param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework whenever an insert or update operation is 
        /// being processed for the named stream associated with the entity instance specified via the entity parameter.
        /// 
        /// Notes to Interface Implementers
        /// The <paramref name="operationContext"/> argument is passed as it is likely that an implementer of this interface method will 
        /// need information from the HTTP request headers in order to construct a write stream.  Likely header 
        /// values required are: 
        ///   'Content-Type'
        ///   'Content-Disposition'
        ///   'Slug' (as specified in the AtomPub RFC 5023)
        ///   
        /// An implementer of this method MUST perform concurrency checks as needed in their implementation of this method.
        /// If an If-Match or If-None-Match request header was included in the request, then the etag parameter will be non null,
        /// which indicates this method MUST perform the appropriate concurrency check.  If the concurrency check passes, this
        /// method should return the requested stream.  If the concurrency checks fails, the method should throw  a DataServiceException
        /// with the appropriate HTTP response code as defined in HTTP RFC 2616 section 14.24 and section 14.26. 
        ///   If the etag was sent as the value of an If-Match request header, the value of the �checkETagForEquality� header will be set to true
        ///   If the etag was sent as the value of an If-None-Match request header, the value of the �checkETagForEquality� header will be set to false
        ///   
        /// An implementer of this method MUST NOT set the following HTTP response headers the <paramref name="operationContext"/> parameter
        /// as they are set by the data service runtime:
        ///   Content-Type
        ///   ETag
        ///   
        /// An implementer of this method may set HTTP response headers (other than those forbidden above) on the <paramref name="operationContext"/> parameter.
        /// 
        /// An implementer of this method should only set the properties on the <paramref name="operationContext"/> parameter which it requires to be set for a successful
        /// response.  Altering other properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// 
        /// Stream Ownership
        /// The data service framework will dispose the stream once all bytes have been successfully written to
        /// the stream.
        /// 
        /// If an error occurs while writing to the stream, then the data services framework will generate an 
        /// error response to the client as per the "error contract" semantics followed by V1 data services.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "etag is not Hungarian notation")]
        Stream GetWriteStream(object entity, ResourceProperty streamProperty, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext);

        /// <summary>Returns the content-type of the specified named stream.</summary>
        /// <returns>A valid MIME Content-Type value for the binary data.</returns>
        /// <param name="entity">The entity to which the named stream belongs.</param>
        /// <param name="streamProperty">A <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> object that represents the named stream.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to obtain the IANA content type (aka media type) of the named stream associated
        /// with the specified entity.  This metadata is needed when constructing the payload for the entity associated with the
        /// named stream or setting the Content-Type HTTP response header.
        /// 
        /// The string should be returned in a format which is directly usable as the value of an HTTP Content-Type response header.
        /// For example, if the stream represented a PNG image the return value would be "image/png"
        /// 
        /// If the requested named stream has not yet been uploaded or the GetReadStream method will set the response status code to 204, this method should 
        /// return null or string.Empty.  Otherwise this method MUST always return a valid content type string for the requested named stream.
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        string GetStreamContentType(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext);

        /// <summary>Returns the URI that is used to request a specific named stream.</summary>
        /// <returns>A <see cref="T:System.Uri" /> value that is used to request the named binary data stream.</returns>
        /// <param name="entity">The entity with the named stream being requested.</param>
        /// <param name="streamProperty">A <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> object that represents the named stream.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to obtain the URI clients should use when making retrieve (ie. GET)
        /// requests to the named stream.  This metadata is needed when constructing the payload for the entity
        /// associated with the named stream.
        /// 
        /// This method was added such that an entity�s representation could state that a named stream is to
        /// be edited using one URI and read using another.   This is supported such that a data service could leverage a Content
        /// Distribution Network for its stream content.
        /// 
        /// The URI returned maps to the value of the self link for the named media resource.  If the JSON format is
        /// used (as noted in section 3.2.3) this URI represents the value of the src_media name/value pair.
        /// 
        /// The returned URI MUST be an absolute URI and represents the location where a consumer (reader) of the stream should send
        /// requests to in order to obtain the contents of the stream.  
        /// 
        /// If URI returned is null, then the data service runtime omit the self link for the named media resource.
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        Uri GetReadStreamUri(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext);

        /// <summary>Returns the eTag of the specified named stream.</summary>
        /// <returns>eTag value of the specified named stream.</returns>
        /// <param name="entity">The entity to which the named stream belongs.</param>
        /// <param name="streamProperty">A <see cref="T:Microsoft.OData.Service.Providers.ResourceProperty" /> object that represents the named stream.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to obtain the ETag of the name stream associated with the entity specified.
        /// This metadata is needed when constructing the payload for the entity associated with the named stream
        /// as well as to be used as the value of the ETag HTTP response header.
        /// 
        /// This method enables a named stream to have an ETag which is different from that of its associated entity.
        /// The returned string MUST be formatted such that it is directly usable as the value of an HTTP ETag response header.
        /// If null is returned the data service framework will assume that no ETag is associated with the stream
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        string GetStreamETag(object entity, ResourceProperty streamProperty, DataServiceOperationContext operationContext);
    }
}
