//---------------------------------------------------------------------
// <copyright file="CreateHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Services.ODataWCFService.Vocabularies;

    /// <summary>
    /// This class is responsible for handling requests to insert new entities.
    /// </summary>
    public class CreateHandler : RequestHandler
    {
        public CreateHandler(RequestHandler other, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
            : base(other, HttpMethod.POST, requestUri, headers)
        {
        }

        protected override RequestHandler DispatchHandler()
        {
            if (this.QueryContext.QueryPath.LastSegment is OperationSegment || this.QueryContext.QueryPath.LastSegment is OperationImportSegment)
            {
                return new OperationHandler(this, HttpMethod.POST);
            }
            if (this.QueryContext.QueryPath.LastSegment is BatchSegment)
            {
                return new BatchHandler(this);
            }
            if (!this.QueryContext.Target.IsReference && Utility.IsMediaEntity(EdmClrTypeUtils.GetInstanceType(this.QueryContext.Target.ElementType.FullTypeName())))
            {
                return new MediaStreamHandler(this, this.HttpMethod);
            }

            return null;
        }

        public override void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            if (this.TryDispatch(requestMessage, responseMessage))
            {
                return;
            }

            if (this.QueryContext.Target.TypeKind != EdmTypeKind.Collection)
            {
                throw Utility.BuildException(HttpStatusCode.BadRequest, "The new resource can only be created under collection resource.", null);
            }

            if (this.QueryContext.Target.IsReference)
            {
                this.ProcessCreateLink(requestMessage, responseMessage);
                return;
            }

            try
            {
                var targetEntitySet = (IEdmEntitySetBase)this.QueryContext.Target.NavigationSource;

                // TODO: [lianw] Try to remove "targetEntitySet" later.
                var queryResults = this.QueryContext.ResolveQuery(this.DataSource);

                if (!IsAllowInsert(targetEntitySet as IEdmEntitySet))
                {
                    throw new ODataServiceException(HttpStatusCode.BadRequest, "The insert request is not allowed.", null);
                }

                var bodyObject = ProcessPostBody(requestMessage, targetEntitySet, queryResults);

                using (var messageWriter = this.CreateMessageWriter(responseMessage))
                {
                    this.DataSource.UpdateProvider.SaveChanges();

                    // 11.4.2 Create an Entity
                    // Upon successful completion the service MUST respond with either 201 Created, or 204 No Content if the request included a return Prefer header with a value of return=minimal.
                    responseMessage.SetStatusCode(HttpStatusCode.Created);
                    responseMessage.SetHeader(ServiceConstants.HttpHeaders.Location, Utility.BuildLocationUri(this.QueryContext, bodyObject).OriginalString);
                    var currentETag = Utility.GetETagValue(bodyObject);
                    // if the current entity has ETag field
                    if (currentETag != null)
                    {
                        responseMessage.SetHeader(ServiceConstants.HttpHeaders.ETag, currentETag);
                    }

                    ResponseWriter.WriteEntry(messageWriter.CreateODataResourceWriter(targetEntitySet), bodyObject, targetEntitySet, ODataVersion.V4, null);
                }
            }
            catch
            {
                this.DataSource.UpdateProvider.ClearChanges();
                throw;
            }
        }

        private void ProcessCreateLink(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            var level = this.QueryContext.QueryPath.Count - 2;
            var parent = this.QueryContext.ResolveQuery(this.DataSource, level);

            var odataPath = this.QueryContext.QueryPath;
            var collectionPropertyName = ((NavigationPropertyLinkSegment)odataPath.LastSegment).NavigationProperty.Name;

            using (var messageReader = new ODataMessageReader(requestMessage, this.GetReaderSettings(), this.DataSource.Model))
            {
                var referenceLink = messageReader.ReadEntityReferenceLink();
                var queryContext = new QueryContext(this.ServiceRootUri, referenceLink.Url, this.DataSource.Model);
                var target = queryContext.ResolveQuery(this.DataSource);

                this.DataSource.UpdateProvider.CreateLink(parent, collectionPropertyName, target);
                this.DataSource.UpdateProvider.SaveChanges();
            }

            // Protocol 11.4.6.1 Add a Reference to a Collection-Valued Navigation Property
            // On successful completion, the response MUST be 204 No Content and contain an empty body.
            ResponseWriter.WriteEmptyResponse(responseMessage);
        }

        private object ProcessPostBody(IODataRequestMessage message, IEdmEntitySetBase entitySet, object queryResults)
        {
            object lastNewInstance = null;

            using (var messageReader = new ODataMessageReader(message, this.GetReaderSettings(), this.DataSource.Model))
            {
                var odataItemStack = new Stack<ODataItem>();
                var entryReader = messageReader.CreateODataResourceReader(entitySet, entitySet.EntityType());
                var currentTargetEntitySet = entitySet;

                while (entryReader.Read())
                {
                    switch (entryReader.State)
                    {
                        case ODataReaderState.ResourceStart:
                            odataItemStack.Push(entryReader.Item);
                            break;

                        case ODataReaderState.ResourceEnd:
                            {
                                var entry = (ODataResource)entryReader.Item;

                                // TODO: the code here will be changed to handle following scenarios
                                //       1: non-contained navigation, e.g. People(1)/Friends
                                //       2. general entity set, e.g. People
                                //       3. contained navigation, e.g. People(1)/Trips
                                //       4. upsert, e.g. People(1)/Friends(2)
                                var newInstance = this.DataSource.UpdateProvider.Create(entry.TypeName, queryResults);

                                foreach (var property in entry.Properties)
                                {
                                    if (Utility.IsETagProperty(newInstance, property.Name)) continue;
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

                                lastNewInstance = newInstance;
                            }

                            break;

                        case ODataReaderState.ResourceSetStart:
                            odataItemStack.Push(entryReader.Item);
                            break;

                        case ODataReaderState.ResourceSetEnd:
                            {
                                var childAnnotation = odataItemStack.Pop().GetAnnotation<ChildInstanceAnnotation>();

                                var parentNavLink = odataItemStack.Count > 0 ? odataItemStack.Peek() as ODataNestedResourceInfo : null;
                                if (parentNavLink != null)
                                {
                                    // This feed belongs to a navigation property -
                                    // propagate it up the tree for further processing.
                                    AddChildInstanceAnnotation(parentNavLink, childAnnotation.ChildInstances ?? new object[0]);
                                }
                            }

                            break;

                        case ODataReaderState.NestedResourceInfoStart:
                            {
                                odataItemStack.Push(entryReader.Item);
                                var navigationLink = (ODataNestedResourceInfo)entryReader.Item;
                                var navigationProperty = (IEdmNavigationProperty)currentTargetEntitySet.EntityType().FindProperty(navigationLink.Name);

                                // Current model implementation doesn't expose associations otherwise this would be much cleaner.
                                currentTargetEntitySet = this.DataSource.Model.EntityContainer.EntitySets().Single(s => s.EntityType() == navigationProperty.Type.Definition);
                            }

                            break;

                        case ODataReaderState.NestedResourceInfoEnd:
                            {
                                var navigationLink = (ODataNestedResourceInfo)entryReader.Item;
                                var childAnnotation = odataItemStack.Pop().GetAnnotation<ChildInstanceAnnotation>();
                                if (childAnnotation != null)
                                {
                                    // Propagate the bound entries to the parent entry.
                                    AddBoundNavigationPropertyAnnotation(odataItemStack.Peek(), navigationLink, childAnnotation.ChildInstances);
                                }
                            }

                            break;
                    }
                }
            }

            return lastNewInstance;
        }

        private static void AddBoundNavigationPropertyAnnotation(ODataItem item, ODataNestedResourceInfo navigationLink, object boundValue)
        {
            var annotation = item.GetAnnotation<BoundNavigationPropertyAnnotation>();
            if (annotation == null)
            {
                annotation = new BoundNavigationPropertyAnnotation { BoundProperties = new List<Tuple<ODataNestedResourceInfo, object>>() };
                item.SetAnnotation(annotation);
            }

            annotation.BoundProperties.Add(new Tuple<ODataNestedResourceInfo, object>(navigationLink, boundValue));
        }

        private static void AddChildInstanceAnnotation(ODataItem item, object childEntry)
        {
            var annotation = item.GetAnnotation<ChildInstanceAnnotation>();
            if (annotation == null)
            {
                annotation = new ChildInstanceAnnotation { ChildInstances = new List<object>() };
                item.SetAnnotation(annotation);
            }

            annotation.ChildInstances.Add(childEntry);
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

        /// <summary>
        /// Annotation for marking a new entry with bound associated entries.
        /// </summary>
        private class BoundNavigationPropertyAnnotation
        {
            public IList<Tuple<ODataNestedResourceInfo, object>> BoundProperties { get; set; }
        }

        /// <summary>
        /// Annotation for marking a navigation property or feed with new entry instances belonging to it.
        /// </summary>
        private class ChildInstanceAnnotation
        {
            public IList<object> ChildInstances { get; set; }
        }
    }
}
