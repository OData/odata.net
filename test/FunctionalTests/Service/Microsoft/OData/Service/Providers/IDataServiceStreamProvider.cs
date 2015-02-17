//---------------------------------------------------------------------
// <copyright file="IDataServiceStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.IO;

    /// <summary>
    /// The IDataServiceStreamProvider interface defines the contract between the data services framework server component
    /// and a data source's stream implementation (ie. a stream provider).
    /// </summary>
    public interface IDataServiceStreamProvider
    {
        /// <summary>Gets the size of the stream buffer.</summary>
        /// <returns>Integer that represents the size of buffer.</returns>
        /// <remarks>If the size is 0, the default of 64k will be used.</remarks>
        int StreamBufferSize
        {
            get;
        }

        /// <summary>Returns a stream that contains the media resource data for the specified entity, which is a media link entry.</summary>
        /// <returns>The data <see cref="T:System.IO.Stream" /> that contains the binary property data of the <paramref name="entity" />.</returns>
        /// <param name="entity">The entity that is a media link entry with a related media resource.</param>
        /// <param name="etag">The eTag value sent as part of the HTTP request that is sent to the data service.</param>
        /// <param name="checkETagForEquality">A nullable <see cref="T:System.Boolean" /> value that determines what kind of conditional request was issued to the data service, which is true when the eTag was sent in an If-Match header, false when the eTag was sent in an If-None-Match header, and null when the request was not conditional and no eTag was included in the request.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to retrieve the default stream associated
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
        /// Stream Ownership
        /// The data service framework will dispose the stream once all bytes have been successfully read.
        /// 
        /// If an error occurs while reading the stream, then the data services framework will generate an
        /// in-stream error which is sent back to the client.  See the error contract specification for a 
        /// description of the format of in-stream errors
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "etag is not Hungarian notation")]
        Stream GetReadStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext);

        /// <summary>Returns the stream that the data service uses to write the binary data for the media resource received from the client that belongs to the specified entity.</summary>
        /// <returns>A valid <see cref="T:System.Stream" /> the data service uses to write the contents of a binary data received from the client.</returns>
        /// <param name="entity">The entity that is a media link entry with a related media resource.</param>
        /// <param name="etag">The eTag value that is sent as part of the HTTP request that is sent to the data service.</param>
        /// <param name="checkETagForEquality">A nullable <see cref="T:System.Boolean" /> value that determines what kind of conditional request was issued to the data service, which is true when the eTag was sent in an If-Match header, false when the eTag was sent in an If-None-Match header, and null when the request was not conditional and no eTag was included in the request.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance that is used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework whenever an insert or update operation is 
        /// being processed for the stream associated with the entity instance specified via the entity parameter.
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
        /// An implementer of this method MUST NOT set the following HTTP response headers on the <paramref name="operationContext"/> parameter
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
        /// error response to the client as per the "error contract" semantics followed by V1 data services
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "etag is not Hungarian notation")]
        Stream GetWriteStream(object entity, string etag, bool? checkETagForEquality, DataServiceOperationContext operationContext);

        /// <summary>Deletes the associated media resource when a media link entry is deleted. </summary>
        /// <param name="entity">The media link entry that is deleted.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance that processes the request.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entity" /> or <paramref name="operationContext" /> are null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="entity" /> is not an entity that has a binary property to stream.</exception>
        /// <exception cref="T:Microsoft.OData.Service.DataServiceException">When the stream associated with the <paramref name="entity" /> cannot be deleted.</exception>
        /// <remarks>
        /// This method is invoked by the data services framework whenever an delete operation is being processed for the streams associated with
        /// the entity instance specified via the entity parameter.
        /// 
        /// Notes to Interface Implementers
        /// If this method is being invoked as part of a request to delete the MLE and its associated stream (ie. MR):
        ///   This method will be invoked AFTER IUpdatable.DeleteResource(entity) is called.  An implementer of this method must be able to
        ///   delete a stream even if the associated entity (passed as a parameter to this method) has already been removed from the
        ///   underlying data source.
        /// 
        /// The <paramref name="operationContext"/> argument is passed as a means for this method to read the HTTP request headers provided with the delete request.
        /// 
        /// An implementer of this method MUST NOT set the following HTTP response headers on the <paramref name="operationContext"/> parameter as
        /// they are set by the data service runtime:
        ///   Content-Type
        ///   ETag
        ///   
        /// An implementer of this method may set HTTP response headers (other than those forbidden above) on the <paramref name="operationContext"/> parameter.
        /// 
        /// An implementer of this method should only set the properties on the <paramref name="operationContext"/> parameter which it requires to be set for a successful response.
        /// Altering other properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        void DeleteStream(object entity, DataServiceOperationContext operationContext);

        /// <summary>Returns the content-type of the media resource that belongs to the specified entity.</summary>
        /// <returns>A valid MIME Content-Type value for the binary data.</returns>
        /// <param name="entity">The entity that is a media link entry with a related media resource.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to obtain the IANA content type (aka media type) of the stream associated
        /// with the specified entity.  This metadata is needed when constructing the payload for the Media Link Entry associated with the
        /// stream (aka Media Resource) or setting the Content-Type HTTP response header.
        /// 
        /// The string should be returned in a format which is directly usable as the value of an HTTP Content-Type response header.
        /// For example, if the stream represented a PNG image the return value would be "image/png"
        /// 
        /// This method MUST always return a valid content type string.  If null or string.empty is returned the data service framework will
        /// consider that an error case and return a 500 (Internal Server Error) to the client.
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        string GetStreamContentType(object entity, DataServiceOperationContext operationContext);

        /// <summary>Returns the URI that is used to request the media resource that belongs to the specified entity.</summary>
        /// <returns>A <see cref="T:System.Uri" /> value that is used to request the binary data stream.</returns>
        /// <param name="entity">The entity that is a media link entry with a related media resource.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to obtain the URI clients should use when making retrieve (ie. GET)
        /// requests to the stream(ie. Media Resource).   This metadata is needed when constructing the payload for the Media Link Entry
        /// associated with the stream (aka Media Resource).
        /// 
        /// This method was added such that a Media Link Entry�s representation could state that a stream (Media Resource) is to
        /// be edited using one URI and read using another.   This is supported such that a data service could leverage a Content
        /// Distribution Network for its stream content.
        /// 
        /// The URI returned maps to the value of the src attribute on the atom:content element of a payload representing the Media
        /// Link Entry associated with the stream described by this DataServiceStreamDescriptor instance.  If the JSON format is
        /// used (as noted in section 3.2.3) this URI represents the value of the src_media name/value pair.
        /// 
        /// The returned URI MUST be an absolute URI and represents the location where a consumer (reader) of the stream should send
        /// requests to in order to obtain the contents of the stream.  
        /// 
        /// If URI returned is null, then the data service runtime will automatically generate the URI representing the location
        /// where the stream can be read from.  The URI generated by the runtime will equal the canonical URI for the associated Media Link
        /// Entry followed by a �/$value� path segment. 
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        Uri GetReadStreamUri(object entity, DataServiceOperationContext operationContext);

        /// <summary>Returns the eTag of the media resource that belongs to the specified media link entry.</summary>
        /// <returns>eTag of the media resource associated with the <paramref name="entity" />.</returns>
        /// <param name="entity">The entity that is a media link entry with a related media resource.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework to obtain the ETag of the stream associated with the entity specified.
        /// This metadata is needed when constructing the payload for the Media Link Entry associated with the stream (aka Media Resource)
        /// as well as to be used as the value of the ETag HTTP response header.
        /// 
        /// This method enables a stream (Media Resource) to have an ETag which is different from that of its associated Media Link Entry.
        /// The returned string MUST be formatted such that it is directly usable as the value of an HTTP ETag response header.
        /// If null is returned the data service framework will assume that no ETag is associated with the stream
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        string GetStreamETag(object entity, DataServiceOperationContext operationContext);

        /// <summary>Returns a namespace-qualified type name that represents the type that the data service runtime must create for the media link entry that is associated with the data stream for the media resource that is being inserted.</summary>
        /// <returns>A namespace-qualified type name.</returns>
        /// <param name="entitySetName">Fully-qualified entity set name.</param>
        /// <param name="operationContext">The <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> instance that is used by the data service to process the request.</param>
        /// <remarks>
        /// This method is invoked by the data services framework when a request is received to insert into an Entity Set with an associated
        /// Entity Type hierarchy that has > 1 Entity Type and >= 1 Entity Type which is tagged as an MLE (ie. includes a stream).
        /// 
        /// An implementer of this method should inspect the request headers provided by the <paramref name="operationContext"/> parameter and return the namespace
        /// qualified type name which represents the type the Astoria framework should instantiate to create the MLE associated with the
        /// BLOB/MR being inserted.  The string representing the MLE type name returned from this method will subsequently be passed to
        /// IUpdatable.CreateResource to create the MLE (of the specified type).
        /// 
        /// NOTE: Altering properties on the <paramref name="operationContext"/> parameter may corrupt the response from the data service.
        /// </remarks>
        string ResolveType(string entitySetName, DataServiceOperationContext operationContext);
    }
}
