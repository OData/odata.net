//---------------------------------------------------------------------
// <copyright file="MediaStreamHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.ServiceModel.Web;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using Microsoft.Test.OData.Services.ODataWCFService.Vocabularies;

    public class MediaStreamHandler : RequestHandler
    {
        public MediaStreamHandler(RequestHandler other, HttpMethod method)
            : base(other, method)
        {
        }

        public override Stream Process(Stream requestStream)
        {
            switch (this.HttpMethod)
            {
                case HttpMethod.GET:
                    return this.ProcessQuery();
                case HttpMethod.POST:
                    return base.Process(requestStream);
                case HttpMethod.PUT:
                    return base.Process(requestStream);
                case HttpMethod.DELETE:
                    return this.ProcessDelete();
            }

            throw Utility.BuildException(HttpStatusCode.MethodNotAllowed, string.Format("The HTTP method '{0}' is not supported", this.HttpMethod), null);
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            switch (this.HttpMethod)
            {
                case HttpMethod.POST:
                    this.ProcessCreate(requestMessage.GetStream(), responseMessage);
                    break;
                case HttpMethod.PUT:
                    this.ProcessUpdate(requestMessage.GetStream(), responseMessage);
                    break;
            }
        }

        private Stream ProcessQuery()
        {
            // handle entity
            var entity = this.HandleEntity();

            // handle ETag
            var currentETag = this.DataSource.StreamProvider.GetETag(entity);
            if (currentETag != null)
            {
                var ifMatch = WebOperationContext.Current.IncomingRequest.IfMatch;
                if (ifMatch != null && ifMatch.Any())
                {
                    var requestETag = ifMatch.ElementAt(0);
                    if (requestETag == ServiceConstants.ETagValueAsterisk || requestETag == currentETag)
                    {
                        return this.HandleNotModified();
                    }
                }
            }

            return this.HandleResponse(entity, HttpStatusCode.OK, true, true);
        }

        private void ProcessCreate(Stream requestStream, IODataResponseMessage responseMessage)
        {
            try
            {
                // handle insert annotation
                if (!this.IsAllowInsert(this.QueryContext.Target.NavigationSource as IEdmEntitySet))
                {
                    throw new ODataServiceException(HttpStatusCode.BadRequest, "The insert request is not allowed.", null);
                }

                // handle content type
                var contentType = this.HandleContentType((IEdmEntityType)this.QueryContext.Target.ElementType);

                // handle entity
                var entities = this.QueryContext.ResolveQuery(this.DataSource);
                var entity = this.DataSource.UpdateProvider.Create(this.QueryContext.Target.ElementType.FullTypeName(), entities);
                this.DataSource.StreamProvider.CreateStream(entity, requestStream, contentType);

                this.DataSource.UpdateProvider.SaveChanges();

                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    responseMessage.SetHeader(ServiceConstants.HttpHeaders.Location, Utility.BuildLocationUri(this.QueryContext, entity).OriginalString);

                    if (this.PreferenceContext.Return == ServiceConstants.PreferenceValue_Return_Minimal)
                    {
                        responseMessage.SetStatusCode(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        responseMessage.SetStatusCode(HttpStatusCode.Created);
                        var edmEntitySet = (IEdmEntitySetBase)this.QueryContext.Target.NavigationSource;
                        ResponseWriter.WriteEntry(messageWriter.CreateODataResourceWriter(edmEntitySet), entity, edmEntitySet, ODataVersion.V4, null);
                    }
                }
            }
            catch
            {
                this.DataSource.UpdateProvider.ClearChanges();
                throw;
            }
        }

        private void ProcessUpdate(Stream requestStream, IODataResponseMessage responseMessage)
        {
            // handle entity
            var entity = this.HandleEntity();

            // handle content type
            var parentUri = this.QueryContext.Target.BuildContainerUri(this.ServiceRootUri);
            var parentContext = new QueryContext(this.ServiceRootUri, parentUri, this.DataSource.Model, this.RequestContainer);
            var contentType = this.HandleContentType((IEdmEntityType)parentContext.Target.ElementType);

            // handle ETag
            this.HandleETag(entity, true);

            // handle update
            this.DataSource.StreamProvider.UpdateStream(entity, requestStream, contentType);

            using (var messageWriter = this.CreateMessageWriter(responseMessage))
            {
                responseMessage.SetStatusCode(HttpStatusCode.OK);
                var edmEntitySet = (IEdmEntitySetBase)parentContext.Target.NavigationSource;
                ResponseWriter.WriteEntry(messageWriter.CreateODataResourceWriter(edmEntitySet), entity, edmEntitySet, ODataVersion.V4, null);
            }
        }

        private Stream ProcessDelete()
        {
            if (!this.IsAllowDelete())
            {
                throw new ODataServiceException(HttpStatusCode.BadRequest, "The delete request is not allowed.", null);
            }

            try
            {
                using (DeletionContext.Create())
                {
                    // handle entity
                    var entity = this.HandleEntity();

                    // handle ETag
                    this.HandleETag(entity, false);

                    // delete stream
                    this.DataSource.StreamProvider.DeleteStream(entity);

                    // delete entity
                    this.DataSource.UpdateProvider.Delete(entity);

                    this.DataSource.UpdateProvider.SaveChanges();

                    return this.HandleResponse(entity, HttpStatusCode.NoContent, false, false);
                }
            }
            catch (Exception)
            {
                this.DataSource.UpdateProvider.ClearChanges();
                throw;
            }
        }

        private ClrObject HandleEntity()
        {
            var entity = this.QueryContext.ResolveQuery(this.DataSource) as ClrObject;
            if (entity == null)
            {
                throw Utility.BuildException(HttpStatusCode.NotFound);
            }

            return entity;
        }

        private void HandleETag(object entity, bool handleStream)
        {
            var entityETag = handleStream ? this.DataSource.StreamProvider.GetETag(entity) : Utility.GetETagValue(entity);
            if (entityETag != null)
            {
                var ifMatch = WebOperationContext.Current.IncomingRequest.IfMatch;
                if (ifMatch != null && ifMatch.Any())
                {
                    var requestETag = ifMatch.ElementAt(0);
                    if (requestETag != ServiceConstants.ETagValueAsterisk && requestETag != entityETag)
                    {
                        throw Utility.BuildException(HttpStatusCode.PreconditionFailed);
                    }
                }
                else
                {
                    throw Utility.BuildException((HttpStatusCode)428);
                }
            }
        }

        private string HandleContentType(IEdmEntityType entityType)
        {
            var contentType = WebOperationContext.Current.IncomingRequest.ContentType;
            if (string.IsNullOrEmpty(contentType))
            {
                throw Utility.BuildException(HttpStatusCode.BadRequest);
            }

            // handle acceptable media types
            var acceptableTypes = this.QueryContext.Model.GetAcceptableMediaTypes(entityType);
            if (acceptableTypes != null && acceptableTypes.All(acceptableType => acceptableType != contentType))
            {
                throw Utility.BuildException(HttpStatusCode.BadRequest);
            }

            return contentType;
        }

        private Stream HandleResponse(object entity, HttpStatusCode code, bool writeContent, bool writeETag)
        {
            var result = default(Stream);

            var response = WebOperationContext.Current.OutgoingResponse;

            // write response body
            var isEmpty = false;
            if (writeContent)
            {
                using (var stream = this.DataSource.StreamProvider.GetStream(entity))
                {
                    isEmpty = stream.Length == 0;
                    if (!isEmpty)
                    {
                        response.ContentType = this.DataSource.StreamProvider.GetContentType(entity);
                        response.ContentLength = stream.Length;
                        result = stream;
                    }
                }
            }

            if (isEmpty)
            {
                response.SuppressEntityBody = true;
                response.ContentLength = 0L;
                result = Stream.Null;
            }

            // set ETag
            if (writeETag)
            {
                var etag = this.DataSource.StreamProvider.GetETag(entity);
                if (etag != null)
                {
                    response.ETag = etag;
                }
            }

            // set status code
            response.StatusCode = code;

            return result;
        }

        private Stream HandleNotModified()
        {
            var response = WebOperationContext.Current.OutgoingResponse;
            response.StatusCode = HttpStatusCode.NotModified;
            response.SuppressEntityBody = true;
            response.ContentLength = 0L;
            return Stream.Null;
        }

        private bool IsAllowInsert(IEdmEntitySet edmEntitySet)
        {
            if (edmEntitySet == null)
            {
                return true;
            }

            bool? result;
            IEnumerable<string> unused;
            this.DataSource.Model.GetInsertRestrictions(edmEntitySet, out result, out unused);
            return result.HasValue ? result.Value : true;
        }

        private bool IsAllowDelete()
        {
            IEdmEntitySet edmEntitySet;

            if (this.QueryContext.Target.Type is IEdmCollectionType)
            {
                edmEntitySet = this.QueryContext.Target.NavigationSource as IEdmEntitySet;
            }
            else
            {
                var parentUri = this.QueryContext.Target.BuildContainerUri(this.ServiceRootUri);
                var parentContext = new QueryContext(this.ServiceRootUri, parentUri, this.DataSource.Model, this.RequestContainer);
                edmEntitySet = parentContext.Target.NavigationSource as IEdmEntitySet;
            }

            if (edmEntitySet != null)
            {
                bool? result;
                IEnumerable<string> unused;
                this.DataSource.Model.GetDeleteRestrictions(edmEntitySet, out result, out unused);
                return result.HasValue ? result.Value : true;
            }

            return true;
        }
    }
}
