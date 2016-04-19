//---------------------------------------------------------------------
// <copyright file="UpdateHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;

    public class UpdateHandler : RequestHandler
    {
        public UpdateHandler(RequestHandler other, HttpMethod httpMethod, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
            : base(other, httpMethod, requestUri, headers)
        {
            if (httpMethod != HttpMethod.PATCH && httpMethod != HttpMethod.PUT)
            {
                throw new ArgumentException("The HttpMethod in UpdateHandler must be PATCH or PUT.");
            }
        }

        protected override RequestHandler DispatchHandler()
        {
            if (this.QueryContext.QueryPath.LastSegment is ValueSegment && this.QueryContext.Target.TypeKind == EdmTypeKind.Primitive)
            {
                var primitive = this.QueryContext.Target.Type as IEdmPrimitiveType;
                if (primitive != null && primitive.PrimitiveKind == EdmPrimitiveTypeKind.Stream)
                {
                    return new MediaStreamHandler(this, this.HttpMethod);
                }
            }

            return null;
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            try
            {
                var odataPath = this.QueryContext.QueryPath;

                if (this.QueryContext.Target.IsReference && this.QueryContext.Target.TypeKind != EdmTypeKind.Collection)
                {
                    ProcessUpdateEntityReference(requestMessage, responseMessage, odataPath);
                }
                else
                {
                    ProcessUpdateOrUpsertEntity(requestMessage, responseMessage, odataPath);
                }
            }
            catch
            {
                this.DataSource.UpdateProvider.ClearChanges();
                throw;
            }
        }

        private void ProcessUpdateEntityReference(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage, ODataPath odataPath)
        {
            // This is for change the reference in single-valued navigation property
            // PUT ~/Person(0)/Parent/$ref
            // {
            //     "@odata.context": "http://host/service/$metadata#$ref",
            //     "@odata.id": "Orders(10643)"
            // }

            if (this.HttpMethod == HttpMethod.PATCH)
            {
                throw Utility.BuildException(HttpStatusCode.MethodNotAllowed, "PATCH on a reference link is not supported.", null);
            }

            // Get the parent first
            var level = this.QueryContext.QueryPath.Count - 2;
            var parent = this.QueryContext.ResolveQuery(this.DataSource, level);

            var navigationPropertyName = ((NavigationPropertyLinkSegment)odataPath.LastSegment).NavigationProperty.Name;

            using (var messageReader = new ODataMessageReader(requestMessage, this.GetReaderSettings(), this.DataSource.Model))
            {
                var referenceLink = messageReader.ReadEntityReferenceLink();
                var queryContext = new QueryContext(this.ServiceRootUri, referenceLink.Url, this.DataSource.Model);
                var target = queryContext.ResolveQuery(this.DataSource);

                this.DataSource.UpdateProvider.UpdateLink(parent, navigationPropertyName, target);
                this.DataSource.UpdateProvider.SaveChanges();
            }

            ResponseWriter.WriteEmptyResponse(responseMessage);
        }

        private void ProcessUpdateOrUpsertEntity(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage, ODataPath odataPath)
        {
            var targetObject = this.QueryContext.ResolveQuery(this.DataSource);
            string targetETag = null;

            if (targetObject != null)
            {
                targetETag = Utility.GetETagValue(targetObject);
            }

            var requestETagKind = RequestETagKind.None;
            string requestETag;
            if (Utility.TryGetIfMatch(this.RequestHeaders, out requestETag))
            {
                requestETagKind = RequestETagKind.IfMatch;
            }
            else if (Utility.TryGetIfNoneMatch(this.RequestHeaders, out requestETag))
            {
                requestETagKind = RequestETagKind.IfNoneMatch;
            }

            switch (requestETagKind)
            {
                case RequestETagKind.None:
                    {
                        if (targetETag == null)
                        {
                            if (targetObject == null)
                            {
                                ProcessUpsertEntity(requestMessage, responseMessage, odataPath);
                            }
                            else
                            {
                                ProcessUpdateRequestBody(requestMessage, responseMessage, targetObject, false);
                            }
                        }
                        else
                        {
                            ResponseWriter.WriteEmptyResponse(responseMessage, (HttpStatusCode)428);
                        }

                        break;
                    }
                case RequestETagKind.IfMatch:
                    {
                        if (requestETag == ServiceConstants.ETagValueAsterisk || requestETag == targetETag)
                        {
                            if (targetObject == null)
                            {
                                throw Utility.BuildException(HttpStatusCode.NotFound);
                            }

                            ProcessUpdateRequestBody(requestMessage, responseMessage, targetObject, false);
                        }
                        else
                        {
                            ResponseWriter.WriteEmptyResponse(responseMessage, HttpStatusCode.PreconditionFailed);
                        }

                        break;
                    }
                case RequestETagKind.IfNoneMatch:
                    {
                        if (requestETag == ServiceConstants.ETagValueAsterisk)
                        {
                            ProcessUpsertEntity(requestMessage, responseMessage, odataPath);
                        }
                        else if (requestETag != targetETag)
                        {
                            if (targetObject == null)
                            {
                                throw Utility.BuildException(HttpStatusCode.NotFound);
                            }

                            ProcessUpdateRequestBody(requestMessage, responseMessage, targetObject, false);
                        }
                        else
                        {
                            ResponseWriter.WriteEmptyResponse(responseMessage, HttpStatusCode.PreconditionFailed);
                        }

                        break;
                    }
            }
        }

        private void ProcessUpsertEntity(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage, ODataPath odataPath)
        {
            if (this.QueryContext.Target.TypeKind == EdmTypeKind.Entity && !this.QueryContext.Target.IsReference)
            {
                Uri parentUri = this.QueryContext.Target.BuildContainerUri(this.ServiceRootUri);
                QueryContext parentContext = new QueryContext(this.ServiceRootUri, parentUri, this.DataSource.Model);

                if (parentContext.Target.IsEntitySet)
                {
                    // Update a entity under a entity set => Upsert

                    // TODO: Do we need to preserver the key value?
                    new CreateHandler(this, parentContext.QueryUri).Process(requestMessage, responseMessage);
                }
                else
                {
                    // Update Singleton or single value entity from null value.
                    var parent = parentContext.ResolveQuery(this.DataSource);

                    // TODO: It might not correct here, since the last segment could be type segment.
                    NavigationPropertySegment navSegment = (NavigationPropertySegment)odataPath.LastSegment;
                    var targetObject = Utility.CreateResource(this.QueryContext.Target.Type);
                    parent.GetType().GetProperty(navSegment.NavigationProperty.Name).SetValue(parent, targetObject, null);

                    ProcessUpdateRequestBody(requestMessage, responseMessage, targetObject, true);
                }
            }
            else
            {
                throw Utility.BuildException(HttpStatusCode.NotFound);
            }
        }

        private void ProcessUpdateRequestBody(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage, object targetObject, bool isUpsert)
        {
            if (this.QueryContext.Target.NavigationSource != null && this.QueryContext.Target.TypeKind == EdmTypeKind.Entity)
            {
                using (var messageReader = new ODataMessageReader(requestMessage, this.GetReaderSettings(), this.DataSource.Model))
                {
                    var entryReader = messageReader.CreateODataResourceReader(this.QueryContext.Target.NavigationSource, (IEdmEntityType)this.QueryContext.Target.Type);

                    while (entryReader.Read())
                    {
                        switch (entryReader.State)
                        {
                            case ODataReaderState.ResourceEnd:
                                var entry = (ODataResource)entryReader.Item;
                                foreach (var property in entry.Properties)
                                {
                                    if (Utility.IsETagProperty(targetObject, property.Name)) continue;
                                    // the property might be an open property, so test null first
                                    var propertyInfo = targetObject.GetType().GetProperty(property.Name);
                                    if (propertyInfo != null)
                                    {
                                        if (!isUpsert && Utility.IsReadOnly(propertyInfo)) continue;
                                    }

                                    this.DataSource.UpdateProvider.Update(targetObject, property.Name, property.Value);
                                }

                                break;
                        }
                    }
                }
            }
            else if (this.QueryContext.Target.Property != null && this.QueryContext.Target.TypeKind == EdmTypeKind.Complex)
            {
                using (var messageReader = new ODataMessageReader(requestMessage, this.GetReaderSettings(), this.DataSource.Model))
                {
                    var property = messageReader.ReadProperty(this.QueryContext.Target.Property);
                    ODataComplexValue complexValue = property.Value as ODataComplexValue;

                    foreach (var p in complexValue.Properties)
                    {
                        if (Utility.IsETagProperty(targetObject, property.Name)) continue;
                        this.DataSource.UpdateProvider.Update(targetObject, p.Name, p.Value);
                    }
                }
            }
            else
            {
                throw Utility.BuildException(
                    HttpStatusCode.NotImplemented,
                    string.Format("PATCH/PUT for '{0}' type is not supported.", this.QueryContext.Target.TypeKind),
                    null);
            }

            var currentETag = Utility.GetETagValue(targetObject);
            // if the current entity has ETag field
            if (currentETag != null)
            {
                if (!isUpsert)
                {
                    this.DataSource.UpdateProvider.UpdateETagValue(targetObject);
                }

                this.DataSource.UpdateProvider.SaveChanges();

                currentETag = Utility.GetETagValue(targetObject);
                responseMessage.SetHeader(ServiceConstants.HttpHeaders.ETag, currentETag);
            }
            else
            {
                this.DataSource.UpdateProvider.SaveChanges();
            }

            ResponseWriter.WriteEmptyResponse(responseMessage);
        }

        private enum RequestETagKind { None, IfMatch, IfNoneMatch }
    }
}
