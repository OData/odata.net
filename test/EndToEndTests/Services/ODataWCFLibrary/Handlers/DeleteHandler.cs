//---------------------------------------------------------------------
// <copyright file="DeleteHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;
    using Microsoft.Test.OData.Services.ODataWCFService.Vocabularies;

    /// <summary>
    /// Class for handling delete requests.
    /// </summary>
    public class DeleteHandler : RequestHandler
    {
        public DeleteHandler(RequestHandler other, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
            : base(other, HttpMethod.DELETE, requestUri, headers)
        {
        }

        protected override RequestHandler DispatchHandler()
        {
            if (!this.QueryContext.Target.IsReference)
            {
                if (this.QueryContext.Target.Type is IEdmCollectionType)
                {
                    if (Utility.IsMediaEntity(EdmClrTypeUtils.GetInstanceType(this.QueryContext.Target.ElementType.FullTypeName())))
                    {
                        return new MediaStreamHandler(this, this.HttpMethod);
                    }
                }
                else
                {
                    if (Utility.IsMediaEntity(EdmClrTypeUtils.GetInstanceType(this.QueryContext.Target.Type.FullTypeName())))
                    {
                        return new MediaStreamHandler(this, this.HttpMethod);
                    }
                }
            }

            return null;
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            if (!this.IsAllowDelete())
            {
                throw new ODataServiceException(HttpStatusCode.BadRequest, "The delete request is not allowed.", null);
            }

            if (this.QueryContext.QueryPath.LastSegment is NavigationPropertyLinkSegment)
            {
                this.ProcessDeleteLink(responseMessage);
                return;
            }

            try
            {
                using (DeletionContext.Create())
                {
                    var targetObject = this.QueryContext.ResolveQuery(this.DataSource);

                    if (targetObject == null)
                    {
                        throw Utility.BuildException(HttpStatusCode.NotFound);
                    }

                    var targetETag = Utility.GetETagValue(targetObject);

                    if (targetETag != null)
                    {
                        string requestETag;
                        if (Utility.TryGetIfMatch(this.RequestHeaders, out requestETag))
                        {
                            if (requestETag == ServiceConstants.ETagValueAsterisk || requestETag == targetETag)
                            {
                                ProcessDelete(targetObject, responseMessage);
                            }
                            else
                            {
                                ProcessPreconditionFailed(responseMessage);
                            }
                        }
                        else if (Utility.TryGetIfNoneMatch(this.RequestHeaders, out requestETag))
                        {
                            if (requestETag == ServiceConstants.ETagValueAsterisk || requestETag == targetETag)
                            {
                                ProcessPreconditionFailed(responseMessage);
                            }
                            else
                            {
                                ProcessDelete(targetObject, responseMessage);
                            }
                        }
                        else
                        {
                            ProcessPreconditionRequired(responseMessage);
                        }
                    }
                    else
                    {
                        ProcessDelete(targetObject, responseMessage);
                    }

                    this.DataSource.UpdateProvider.SaveChanges();
                }
            }
            catch
            {
                this.DataSource.UpdateProvider.ClearChanges();
                throw;
            }
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

        private void ProcessDeleteLink(IODataResponseMessage responseMessage)
        {
            var segment = (NavigationPropertyLinkSegment)this.QueryContext.QueryPath.LastSegment;
            var propertyName = segment.NavigationProperty.Name;
            var parent = default(object);
            var target = default(object);

            if (this.QueryContext.QueryEntityIdSegment == null)
            {
                // single-valued navigation property
                parent = this.QueryContext.ResolveQuery(this.DataSource, this.QueryContext.QueryPath.Count - 2);
            }
            else
            {
                // collection-valued navigation property
                var queryUri = this.QueryContext.QueryUri;
                var parentUri = queryUri.AbsoluteUri.Substring(0, queryUri.AbsoluteUri.Length - queryUri.Query.Length);
                var parentContext = new QueryContext(this.ServiceRootUri, new Uri(parentUri, UriKind.Absolute), this.DataSource.Model, this.RequestContainer);
                parent = parentContext.ResolveQuery(this.DataSource, parentContext.QueryPath.Count - 2);
                target = this.QueryContext.ResolveQuery(this.DataSource);
            }

            this.DataSource.UpdateProvider.DeleteLink(parent, propertyName, target);
            this.DataSource.UpdateProvider.SaveChanges();

            ResponseWriter.WriteEmptyResponse(responseMessage);
        }

        private void ProcessDelete(object target, IODataResponseMessage responseMessage)
        {
            this.DataSource.UpdateProvider.Delete(target);

            // Protocol 11.4.5 Delete an Entity
            // On successful completion of the delete, the response MUST be 204 No Content and contain an empty body.
            ResponseWriter.WriteEmptyResponse(responseMessage);
        }

        private static void ProcessPreconditionRequired(IODataResponseMessage responseMessage)
        {
            ResponseWriter.WriteEmptyResponse(responseMessage, (HttpStatusCode)428);
        }

        private static void ProcessPreconditionFailed(IODataResponseMessage responseMessage)
        {
            ResponseWriter.WriteEmptyResponse(responseMessage, HttpStatusCode.PreconditionFailed);
        }
    }
}
