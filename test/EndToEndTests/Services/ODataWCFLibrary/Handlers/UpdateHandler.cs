//---------------------------------------------------------------------
// <copyright file="UpdateHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
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

            using (var messageReader = new ODataMessageReader(requestMessage, this.GetReaderSettings()))
            {
                var referenceLink = messageReader.ReadEntityReferenceLink();
                var queryContext = new QueryContext(this.ServiceRootUri, referenceLink.Url, this.DataSource.Model, this.RequestContainer);
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
                QueryContext parentContext = new QueryContext(this.ServiceRootUri, parentUri, this.DataSource.Model, this.RequestContainer);

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
            if ((this.QueryContext.Target.NavigationSource != null
                && (this.QueryContext.Target.TypeKind == EdmTypeKind.Entity || this.QueryContext.Target.TypeKind == EdmTypeKind.Complex))
                || (this.QueryContext.Target.Property != null && this.QueryContext.Target.TypeKind == EdmTypeKind.Complex))
            {
                using (var messageReader = new ODataMessageReader(requestMessage, this.GetReaderSettings()))
                {
                    var entryReader = messageReader.CreateODataResourceReader(this.QueryContext.Target.NavigationSource, (IEdmStructuredType)this.QueryContext.Target.Type);
                    // Need to handle complex property or collection of complex property
                    var odataItemStack = new Stack<ODataItem>();
                    var entityStack = new Stack<IEdmNavigationSource>();
                    var parentInstances = new Stack<object>();
                    var currentTargetEntitySet = this.QueryContext.Target.NavigationSource;

                    int levelOfUndeclaredProperty = 0;
                    while (entryReader.Read())
                    {
                        switch (entryReader.State)
                        {
                            case ODataReaderState.ResourceStart:
                                {
                                    if (levelOfUndeclaredProperty > 0)
                                    {
                                        break;
                                    }

                                    var entry = (ODataResource)entryReader.Item;
                                    if (entry == null)
                                    {
                                        break;
                                    }

                                    odataItemStack.Push(entryReader.Item);
                                    entityStack.Push(currentTargetEntitySet);
                                    if (parentInstances.Count == 0)
                                    {
                                        parentInstances.Push(targetObject);
                                    }
                                    else
                                    {
                                        var parent2 = parentInstances.Peek();
                                        if (parent2 == null)
                                        {
                                            // Here is for collection, we need to create a brand new instance.
                                            var valueType = EdmClrTypeUtils.GetInstanceType(entry.TypeName);
                                            parentInstances.Push(Utility.QuickCreateInstance(valueType));
                                        }
                                        else
                                        {

                                            parentInstances.Push(parent2);
                                        }
                                    }

                                    break;
                                }

                            case ODataReaderState.ResourceEnd:
                                {
                                    if (levelOfUndeclaredProperty > 0)
                                    {
                                        break;
                                    }

                                    var entry = (ODataResource)entryReader.Item;
                                    if (entry == null)
                                    {
                                        break;
                                    }

                                    object newInstance = parentInstances.Pop();
                                    currentTargetEntitySet = entityStack.Pop();

                                    foreach (var property in entry.Properties)
                                    {
                                        if (Utility.IsETagProperty(targetObject, property.Name)) continue;
                                        // the property might be an open property, so test null first
                                        var propertyInfo = newInstance.GetType().GetProperty(property.Name);
                                        if (propertyInfo != null)
                                        {
                                            if (!isUpsert && Utility.IsReadOnly(propertyInfo)) continue;
                                        }

                                        this.DataSource.UpdateProvider.Update(newInstance, property.Name, property.Value);
                                    }

                                    var boundNavPropAnnotation = odataItemStack.Pop().GetAnnotation<BoundNavigationPropertyAnnotation>();
                                    if (boundNavPropAnnotation != null)
                                    {
                                        foreach (var boundProperty in boundNavPropAnnotation.BoundProperties)
                                        {
                                            var isCollection = boundProperty.Item1.IsCollection == true;
                                            var propertyValue = isCollection ? boundProperty.Item2 : ((IEnumerable<object>)boundProperty.Item2).Single();
                                            this.DataSource.UpdateProvider.Update(newInstance, boundProperty.Item1.Name, propertyValue);
                                        }
                                    }

                                    var parentItem = odataItemStack.Count > 0 ? odataItemStack.Peek() : null;
                                    if (parentItem != null)
                                    {
                                        // This new entry belongs to a navigation property and/or feed -
                                        // propagate it up the tree for further processing.
                                        AddChildInstanceAnnotation(parentItem, newInstance);
                                    }
                                }

                                break;
                            case ODataReaderState.ResourceSetStart:
                                if (levelOfUndeclaredProperty > 0)
                                {
                                    break;
                                }

                                odataItemStack.Push(entryReader.Item);

                                //"null" here indicate that we need to create a new instance for collection to replace the old one.
                                parentInstances.Push(null);
                                break;

                            case ODataReaderState.ResourceSetEnd:
                                {
                                    if (levelOfUndeclaredProperty > 0)
                                    {
                                        break;
                                    }

                                    parentInstances.Pop();
                                    var childAnnotation = odataItemStack.Pop().GetAnnotation<ChildInstanceAnnotation>();

                                    var parentNavLink = odataItemStack.Count > 0 ? odataItemStack.Peek() as ODataNestedResourceInfo : null;
                                    if (parentNavLink != null)
                                    {
                                        // This feed belongs to a navigation property or complex property or a complex collection property.
                                        // propagate it up the tree for further processing.
                                        AddChildInstanceAnnotations(parentNavLink, childAnnotation == null ? new object[0] : (childAnnotation.ChildInstances ?? new object[0]));
                                    }
                                }

                                break;

                            case ODataReaderState.NestedResourceInfoStart:
                                {
                                    object parent = parentInstances.Peek();
                                    var nestedResourceInfo = (ODataNestedResourceInfo)entryReader.Item;
                                    var property = parent.GetType().GetProperty(nestedResourceInfo.Name);
                                    if (property == null || levelOfUndeclaredProperty > 0)
                                    {
                                        levelOfUndeclaredProperty++;
                                    }

                                    // skip undeclared property
                                    if (levelOfUndeclaredProperty > 0)
                                    {
                                        break;
                                    }

                                    odataItemStack.Push(entryReader.Item);
                                    var propertyInstance = property.GetValue(parent);
                                    parentInstances.Push(propertyInstance);

                                    IEdmNavigationProperty navigationProperty = currentTargetEntitySet == null ? null : currentTargetEntitySet.EntityType().FindProperty(nestedResourceInfo.Name) as IEdmNavigationProperty;

                                    // Current model implementation doesn't expose associations otherwise this would be much cleaner.
                                    if (navigationProperty != null)
                                    {
                                        currentTargetEntitySet = this.DataSource.Model.EntityContainer.EntitySets().Single(s => s.EntityType() == navigationProperty.Type.Definition);
                                    }
                                    else
                                    {
                                        currentTargetEntitySet = null;
                                    }

                                    entityStack.Push(currentTargetEntitySet);
                                    break;
                                }
                            case ODataReaderState.NestedResourceInfoEnd:
                                {
                                    // check: skip or not
                                    int theLevelOfUndeclaredProperty = levelOfUndeclaredProperty;
                                    if (levelOfUndeclaredProperty > 0)
                                    {
                                        levelOfUndeclaredProperty--;
                                    }

                                    // skip undeclared property
                                    if (theLevelOfUndeclaredProperty > 0)
                                    {
                                        break;
                                    }

                                    var navigationLink = (ODataNestedResourceInfo)entryReader.Item;
                                    parentInstances.Pop();
                                    var childAnnotation = odataItemStack.Pop().GetAnnotation<ChildInstanceAnnotation>();
                                    if (childAnnotation != null)
                                    {
                                        // Propagate the bound resources to the parent resource.
                                        AddBoundNavigationPropertyAnnotation(odataItemStack.Peek(), navigationLink, childAnnotation == null ? new object[0] : childAnnotation.ChildInstances);
                                    }

                                    entityStack.Pop();
                                    currentTargetEntitySet = entityStack.Peek();
                                    break;
                                }
                        }
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
