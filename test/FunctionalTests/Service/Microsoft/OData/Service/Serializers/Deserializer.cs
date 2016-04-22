//---------------------------------------------------------------------
// <copyright file="Deserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Provides a abstract base deserializer class
    /// </summary>
    internal abstract class Deserializer : IDisposable
    {
        /// <summary>Maximum recursion limit on deserializer.</summary>
        protected const int RecursionLimit = 100;

        /// <summary>Data service for which the deserializer will act.</summary>
        private readonly IDataService service;

        /// <summary>Tracker for actions taken during deserialization.</summary>
        private readonly UpdateTracker tracker;

        /// <summary> Indicates whether the payload is for update or not </summary>
        private readonly bool update;

        /// <summary>Depth of recursion.</summary>
        private int recursionDepth;

        /// <summary>number of resources (entity or complex type) referred in this request.</summary>
        private int objectCount;

        /// <summary>Request description for the top level target entity.</summary>
        private RequestDescription description;

        /// <summary>
        /// Initializes a new instance of <see cref="Deserializer"/>.
        /// </summary>
        /// <param name="update">true if we're reading an update operation; false if not.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <param name="requestDescription">The request description to use.</param>
        internal Deserializer(bool update, IDataService dataService, UpdateTracker tracker, RequestDescription requestDescription)
        {
            Debug.Assert(dataService != null, "dataService != null");

            this.service = dataService;
            this.tracker = tracker;
            this.update = update;
            this.description = requestDescription;
        }

        /// <summary>Initializes a new <see cref="Deserializer"/> based on a different one.</summary>
        /// <param name="parent">Parent deserializer for the new instance.</param>
        internal Deserializer(Deserializer parent)
        {
            Debug.Assert(parent != null, "parent != null");
            this.recursionDepth = parent.recursionDepth;
            this.service = parent.service;
            this.tracker = parent.tracker;
            //this.update = parent.update;
            this.update = false;
            this.description = parent.description;
        }

        /// <summary>Tracker for actions taken during deserialization.</summary>
        internal UpdateTracker Tracker
        {
            [DebuggerStepThrough]
            get { return this.tracker; }
        }

        /// <summary>Data service for which the deserializer will act.</summary>
        protected IDataService Service
        {
            [DebuggerStepThrough]
            get { return this.service; }
        }

        /// <summary>Return the IUpdatable object to use to make changes to entity states</summary>
        protected UpdatableWrapper Updatable
        {
            get
            {
                return this.Service.Updatable;
            }
        }

        /// <summary>
        /// Returns true if the request method is a PUT, or PATCH method
        /// </summary>
        protected bool Update
        {
            [DebuggerStepThrough]
            get { return this.update; }
        }

        /// <summary>Returns the current count of number of objects referred by this request.</summary>
        protected int MaxObjectCount
        {
            get { return this.objectCount; }
        }

        /// <summary>
        /// Gets a value indicating whether the request is json light
        /// </summary>
        protected abstract bool IsJsonLightRequest { get; }

        /// <summary>Request description for the top level target entity.</summary>
        protected RequestDescription RequestDescription
        {
            get { return this.description; }
        }

        /// <summary>
        /// Whether a response will be sent for the POST/PUT/PATCH request.
        /// </summary>
        /// <returns>true if response will be sent, false otherwise.</returns>
        private bool ResponseWillBeSent
        {
            get
            {
                return (!this.Update && !this.description.Preference.ShouldNotIncludeResponseBody) ||
                        (this.Update && this.description.Preference.ShouldIncludeResponseBody);
            }
        }

        /// <summary>Releases resources held onto by this object.</summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a new <see cref="Deserializer"/> for the specified stream.
        /// </summary>
        /// <param name="description">description about the request uri.</param>
        /// <param name="dataService">Data service for which the deserializer will act.</param>
        /// <param name="update">indicates whether this is a update operation or not</param>
        /// <param name="tracker">Tracker to use for modifications.</param>
        /// <returns>A new instance of <see cref="Deserializer"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        internal static Deserializer CreateDeserializer(RequestDescription description, IDataService dataService, bool update, UpdateTracker tracker)
        {
            string mimeType;
            System.Text.Encoding encoding;
            AstoriaRequestMessage host = dataService.OperationContext.RequestMessage;
            ContentTypeUtil.ReadContentType(host.ContentType, out mimeType, out encoding);
            Debug.Assert(tracker != null, "Change tracker must always be created.");

            Debug.Assert(
                (!update /*POST*/ && dataService.OperationContext.RequestMessage.HttpVerb == HttpVerbs.POST) ||
                (update /*PUT,PATCH*/ && (dataService.OperationContext.RequestMessage.HttpVerb == HttpVerbs.PUT ||
                    dataService.OperationContext.RequestMessage.HttpVerb == HttpVerbs.PATCH)),
                "For PUT and PATCH, update must be true; for POST, update must be false");

            if (description.IsServiceActionRequest)
            {
                return new ParameterDeserializer(update, dataService, tracker, description);
            }

            if (description.TargetKind == RequestTargetKind.OpenPropertyValue ||
                description.TargetKind == RequestTargetKind.PrimitiveValue)
            {
                return new RawValueDeserializer(update, dataService, tracker, description);
            }

            if (description.TargetKind == RequestTargetKind.MediaResource)
            {
                return new MediaResourceDeserializer(update, dataService, tracker, description);
            }

            if (description.TargetKind == RequestTargetKind.Primitive ||
                description.TargetKind == RequestTargetKind.ComplexObject ||
                description.TargetKind == RequestTargetKind.Collection ||
                description.TargetKind == RequestTargetKind.OpenProperty)
            {
                return new PropertyDeserializer(update, dataService, tracker, description);
            }

            if (description.LinkUri)
            {
                return new EntityReferenceLinkDeserializer(update, dataService, tracker, description);
            }

            if (description.TargetKind == RequestTargetKind.Resource)
            {
                return new EntityDeserializer(update, dataService, tracker, description);
            }

            throw new DataServiceException(415, Strings.RequestUriProcessor_MethodNotAllowed);
        }

        /// <summary>
        /// Returns the last segment info whose target request kind is resource
        /// </summary>
        /// <param name="description">description about the target request</param>
        /// <param name="service">data service type to which the request was made</param>
        /// <param name="allowCrossReferencing">whether cross-referencing is allowed for the resource in question.</param>
        /// <param name="entityResource">entity resource which is getting modified.</param>
        /// <param name="entityContainer">entity container of the entity which is getting modified.</param>
        /// <param name="checkETag">whether to check the etag for the entity resource that is getting modified.</param>
        /// <returns>Returns the object that needs to get modified</returns>
        internal static object GetResourceToModify(
            RequestDescription description,
            IDataService service,
            bool allowCrossReferencing,
            out object entityResource,
            out ResourceSetWrapper entityContainer,
            bool checkETag)
        {
            // Set the index of the modifying resource
            int modifyingResourceIndex;
            if (
                description.TargetKind == RequestTargetKind.OpenPropertyValue ||
                description.TargetKind == RequestTargetKind.PrimitiveValue)
            {
                modifyingResourceIndex = description.SegmentInfos.Count - 3;
            }
            else
            {
                modifyingResourceIndex = description.SegmentInfos.Count - 2;
            }

            int entityResourceIndex;
            entityResource = Deserializer.GetEntityResourceToModify(
                description,
                service,
                allowCrossReferencing,
                out entityContainer,
                out entityResourceIndex);

            // now walk from the entity resource to the resource to modify.
            // for open types, as you walk, if the intermediate resource is an entity,
            // update the entityResource accordingly.
            object resourceToModify = entityResource;
            for (int i = entityResourceIndex + 1; i <= modifyingResourceIndex; i++)
            {
                // If the segment is a type identifier segment, skip the segment. There is no need to issue one more query
                // to the provider.
                if (description.SegmentInfos[i].IsTypeIdentifierSegment)
                {
                    continue;
                }

#if DEBUG
                SegmentInfo segmentInfo = description.SegmentInfos[i];
                Debug.Assert(segmentInfo.TargetKind != RequestTargetKind.Resource, "segmentInfo.TargetKind != RequestTargetKind.Resource");
#endif
                resourceToModify = service.Updatable.GetValue(resourceToModify, description.SegmentInfos[i].Identifier);
            }

            // If checkETag is true, then we need to check the etag for the resource
            // Note that MediaResource has a separate etag, we don't need to check the MLE etag if the target kind is MediaResource
            if (checkETag && !WebUtil.IsCrossReferencedSegment(description.SegmentInfos[modifyingResourceIndex], service) && description.TargetKind != RequestTargetKind.MediaResource)
            {
                service.Updatable.SetETagValues(entityResource, entityContainer);
            }

            return resourceToModify;
        }

        /// <summary>
        /// Returns the entity that need to get modified
        /// </summary>
        /// <param name="description">description about the target request</param>
        /// <param name="service">data service type to which the request was made</param>
        /// <param name="allowCrossReferencing">whether cross-referencing is allowed for the resource in question.</param>
        /// <param name="entityContainer">entity container of the entity which is getting modified.</param>
        /// <returns>Returns the entity that needs to get modified</returns>
        internal static object GetEntityResourceToModify(
            RequestDescription description,
            IDataService service,
            bool allowCrossReferencing,
            out ResourceSetWrapper entityContainer)
        {
            int entityResourceIndex;
            return Deserializer.GetEntityResourceToModify(
                description,
                service,
                allowCrossReferencing,
                out entityContainer,
                out entityResourceIndex);
        }

        /// <summary>
        /// Modify the value of the given resource to the given value
        /// </summary>
        /// <param name="description">description about the request</param>
        /// <param name="resourceToBeModified">resource that needs to be modified</param>
        /// <param name="requestValue">the new value for the target resource</param>
        /// <param name="service">Service this request is against</param>
        internal static void ModifyResource(RequestDescription description, object resourceToBeModified, object requestValue, IDataService service)
        {
            if (description.TargetKind == RequestTargetKind.OpenProperty ||
                description.TargetKind == RequestTargetKind.OpenPropertyValue)
            {
                Debug.Assert(!description.LastSegmentInfo.HasKeyValues, "CreateSegments must have caught the problem already.");
                SetOpenPropertyValue(resourceToBeModified, description.ContainerName, requestValue, service);
            }
            else if (description.TargetKind == RequestTargetKind.MediaResource)
            {
                SetStreamPropertyValue(resourceToBeModified, (Stream)requestValue, service, description);
            }
            else
            {
                Debug.Assert(
                    description.TargetKind == RequestTargetKind.Primitive ||
                    description.TargetKind == RequestTargetKind.ComplexObject ||
                    description.TargetKind == RequestTargetKind.PrimitiveValue ||
                    description.TargetKind == RequestTargetKind.Collection,
                    "unexpected target kind encountered");

                // update the primitive value
                ResourceProperty propertyToUpdate = description.LastSegmentInfo.ProjectedProperty;
                SetPropertyValue(propertyToUpdate, resourceToBeModified, requestValue, service);
            }
        }

        /// <summary>
        /// Get the resource referred by the given segment
        /// </summary>
        /// <param name="segmentInfo">information about the segment.</param>
        /// <param name="fullTypeName">full name of the resource referred by the segment.</param>
        /// <param name="service">data service type to which the request was made</param>
        /// <param name="checkForNull">whether to check if the resource is null or not.</param>
        /// <returns>returns the resource returned by the provider.</returns>
        internal static object GetResource(SegmentInfo segmentInfo, string fullTypeName, IDataService service, bool checkForNull)
        {
            if (segmentInfo.TargetResourceSet != null)
            {
                Debug.Assert(
                    segmentInfo.TargetKind != RequestTargetKind.OpenProperty &&
                    segmentInfo.TargetKind != RequestTargetKind.OpenPropertyValue,
                    "container can be null only for open types");

                DataServiceConfiguration.CheckResourceRights(segmentInfo.TargetResourceSet, EntitySetRights.ReadSingle);
            }

            Debug.Assert(segmentInfo.RequestExpression != null, "segmentInfo.RequestExpression != null");
            segmentInfo.RequestEnumerable = (IEnumerable)service.ExecutionProvider.Execute(segmentInfo.RequestExpression);
            object resource = service.Updatable.GetResource((IQueryable)segmentInfo.RequestEnumerable, fullTypeName);
            if (resource == null &&
                (segmentInfo.HasKeyValues || checkForNull))
            {
                throw DataServiceException.CreateResourceNotFound(segmentInfo.Identifier);
            }

            return resource;
        }

        /// <summary>
        /// Creates a Media Link Entry.
        /// </summary>
        /// <param name="fullTypeName">Full type name for the MLE to be created.</param>
        /// <param name="requestStream">Request stream from the host.</param>
        /// <param name="service">Service this request is against.</param>
        /// <param name="description">Description of the target request.</param>
        /// <param name="tracker">Update tracker instance to fire change interceptor calls</param>
        /// <returns>Newly created Media Link Entry.</returns>
        internal static object CreateMediaLinkEntry(string fullTypeName, Stream requestStream, IDataService service, RequestDescription description, UpdateTracker tracker)
        {
            Debug.Assert(!string.IsNullOrEmpty(fullTypeName), "!string.IsNullOrEmpty(fullTypeName)");
            Debug.Assert(requestStream != null, "requestStream != null");
            Debug.Assert(service != null, "service != null");
            Debug.Assert(description != null, "description != null");
            Debug.Assert(tracker != null, "tracker != null");

            object entity = service.Updatable.CreateResource(description.LastSegmentInfo.TargetResourceSet.Name, fullTypeName);
            tracker.TrackAction(entity, description.LastSegmentInfo.TargetResourceSet, UpdateOperations.Add);
            SetStreamPropertyValue(entity, requestStream, service, description);
            return entity;
        }

        /// <summary>
        /// Copy the contents of the request stream into the default stream of the specified entity.
        /// </summary>
        /// <param name="resourceToBeModified">Entity with the associated stream which we will write to.</param>
        /// <param name="requestStream">Request stream from the host</param>
        /// <param name="service">Service this is request is against</param>
        /// <param name="description">Description of the target request.</param>
        internal static void SetStreamPropertyValue(object resourceToBeModified, Stream requestStream, IDataService service, RequestDescription description)
        {
            Debug.Assert(resourceToBeModified != null, "resourceToBeModified != null");
            Debug.Assert(requestStream.CanRead, "requestStream.CanRead");
            Debug.Assert(service != null, "service != null");

            resourceToBeModified = service.Updatable.ResolveResource(resourceToBeModified);
            ResourceType resourceType = service.Provider.GetResourceType(resourceToBeModified);

            // We are here because one of the following conditions is true:
            // 1. This is a POST MR, in which case we get here because resourceType.IsMediaLinkEntry is true.
            // 2. This is a PUT default/named stream, so target kind is MediaResource but resourceType can be either an MLE or non-MLE.
            //
            // If target kind is MediaResource and this is not a named stream, the current endpoint is /entity/$value. We need to verify that
            // resourceType is an MLE. At URI processing time, we don't have the actual entity instance to test whether it is an MLE or not, so
            // we postpone the test untill now.
            if (description.TargetKind == RequestTargetKind.MediaResource && !description.IsNamedStream && !resourceType.IsMediaLinkEntry)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidUriForMediaResource(service.OperationContext.AbsoluteRequestUri));
            }

            if (service.OperationContext.RequestMessage.HttpVerb == HttpVerbs.PATCH)
            {
                throw DataServiceException.CreateMethodNotAllowed(
                    Strings.BadRequest_InvalidUriForPatchOperation(service.OperationContext.AbsoluteRequestUri),
                    DataServiceConfiguration.GetAllowedMethods(service.Configuration, description));
            }

            Debug.Assert(
                (service.OperationContext.RequestMessage.HttpVerb == HttpVerbs.POST && resourceType.IsMediaLinkEntry) ||
                (service.OperationContext.RequestMessage.HttpVerb == HttpVerbs.PUT && description.TargetKind == RequestTargetKind.MediaResource),
                "We are here because this is either a POST to an Atom MR or a PUT to a default/named stream.");

            ResourceProperty streamProperty = null; // null for default stream.
            if (description.TargetKind == RequestTargetKind.MediaResource)
            {
                streamProperty = description.StreamProperty;
            }

            using (Stream writeStream = service.StreamProvider.GetWriteStream(resourceToBeModified, streamProperty, service.OperationContext))
            {
                WebUtil.CopyStream(requestStream, writeStream, service.StreamProvider.StreamBufferSize);
            }
        }

        /// <summary>
        /// Gets the resource from the segment enumerable.
        /// </summary>
        /// <param name="segmentInfo">segment from which resource needs to be returned.</param>
        /// <returns>returns the resource contained in the request enumerable.</returns>
        internal static object GetCrossReferencedResource(SegmentInfo segmentInfo)
        {
            Debug.Assert(segmentInfo.RequestEnumerable != null, "The segment should always have the result");
            object[] results = (object[])segmentInfo.RequestEnumerable;
            Debug.Assert(results != null && results.Length == 1, "results != null && results.Length == 1");
            Debug.Assert(results[0] != null, "results[0] != null");
            return results[0];
        }

        /// <summary>
        /// Handle bind operation
        /// </summary>
        /// <param name="description">information about the request uri.</param>
        /// <param name="linkResource">the child resource which needs to be linked.</param>
        /// <param name="service">data service instance</param>
        /// <param name="tracker">update tracker instance to fire change interceptor calls</param>
        /// <returns>returns the parent object to which an new object was linked to.</returns>
        internal static object HandleBindOperation(RequestDescription description, object linkResource, IDataService service, UpdateTracker tracker)
        {
            Debug.Assert(description != null, "description != null");
            Debug.Assert(linkResource != null, "linkResource != null");
            Debug.Assert(service != null, "service != null");
            Debug.Assert(tracker != null, "tracker != null");

            ResourceSetWrapper container;
            object entityGettingModified = Deserializer.GetEntityResourceToModify(description, service, true /*allowCrossReference*/, out container);

            tracker.TrackAction(entityGettingModified, container, UpdateOperations.Change);
            Debug.Assert(description.Property != null, "description.Property != null");
            if (description.IsSingleResult)
            {
                service.Updatable.SetReference(entityGettingModified, description.Property.Name, linkResource);
            }
            else
            {
                service.Updatable.AddReferenceToCollection(entityGettingModified, description.Property.Name, linkResource);
            }

            return entityGettingModified;
        }

        /// <summary>Creates a new value for a collection property.</summary>
        /// <returns>The new collection as an IList.</returns>
        internal static IList CreateNewCollection()
        {
            return new List<object>();
        }

        /// <summary>Returns a read-only version of the specified collection.</summary>
        /// <param name="collection">The collection to convert to read-only.</param>
        /// <param name="resourceType">The resourceType of this collection property.</param>
        /// <returns>A read-only collection.</returns>
        internal static object GetReadOnlyCollection(IList collection, CollectionResourceType resourceType)
        {
            Debug.Assert(collection.GetType() == typeof(List<object>), "The collection was not created by CreateNewCollectionProperty.");
            return new CollectionPropertyValueEnumerable(collection, resourceType);
        }

        /// <summary>
        /// returns true if the null attribute is specified and the value is true
        /// </summary>
        /// <param name="reader">xml reader from which attribute needs to be read</param>
        /// <returns>true if the null attribute is specified and the attribute value is true</returns>
        internal static bool XmlHasNullAttributeWithTrueValue(XmlReader reader)
        {
            Debug.Assert(reader != null, "reader != null");
            Debug.Assert(reader.NodeType == XmlNodeType.Element, "reader.NodeType == XmlNodeType.Element");

            string elementValue = reader.GetAttribute(XmlConstants.AtomNullAttributeName, XmlConstants.DataWebMetadataNamespace);

            // If the null attribute is specified and the value is true, then set the property value to null,
            // otherwise set the value to empty string
            if ((null != elementValue) && XmlConvert.ToBoolean(elementValue))
            {
                string elementName = reader.LocalName;
                if (!CommonUtil.ReadEmptyElement(reader))
                {
                    throw DataServiceException.CreateBadRequestError(
                        Strings.BadRequest_CannotSpecifyValueOrChildElementsForNullElement(elementName));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the index of the entity that need to get modified
        /// </summary>
        /// <param name="description">description about the target request</param>
        /// <returns>Returns the index of the entity that needs to get modified</returns>
        internal static int GetIndexOfEntityResourceToModify(RequestDescription description)
        {
            Debug.Assert(description.TargetSource == RequestTargetSource.Property, "description.TargetSource == RequestTargetSource.Property");

            int entityResourceIndex = -1;
            if (description.LinkUri)
            {
                Debug.Assert(
                    description.SegmentInfos[description.SegmentInfos.Count - 2].TargetKind == RequestTargetKind.Link,
                    "There should be only one segment allowed after $ref");

                Debug.Assert(
                    description.SegmentInfos[description.SegmentInfos.Count - 3].TargetKind == RequestTargetKind.Resource && description.SegmentInfos[description.SegmentInfos.Count - 3].SingleResult,
                    "The segment previous to the $ref segment must refer to a single resource");

                entityResourceIndex = description.SegmentInfos.Count - 3;
            }
            else
            {
                for (int j = description.SegmentInfos.Count - 1; j >= 0; j--)
                {
                    if (description.SegmentInfos[j].TargetKind == RequestTargetKind.Resource && description.SegmentInfos[j].SingleResult)
                    {
                        entityResourceIndex = j;
                        break;
                    }
                }
            }

            Debug.Assert(entityResourceIndex != -1, "This method should never be called for request that doesn't have a parent resource");
            return entityResourceIndex;
        }

        /// <summary>
        /// Reads the action parameters from the payload.
        /// </summary>
        /// <param name="actionSegment">Segment info for the action whose parameters are being read.</param>
        /// <param name="dataService">Data service instance.</param>
        /// <returns>A dictionary of parameter name and parameter value pairs.</returns>
        internal static Dictionary<string, object> ReadPayloadParameters(SegmentInfo actionSegment, IDataService dataService)
        {
            Debug.Assert(actionSegment != null, "actionSegment != null");
            RequestDescription requestDescription = new RequestDescription(new SegmentInfo[] { actionSegment }, RequestUriProcessor.GetResultUri(dataService.OperationContext));
            Debug.Assert(requestDescription.IsServiceActionRequest, "IsServiceActionRequest(requestDescription)");

            // Verify that the DSV is 4.0 or greater for parameters payload.
            requestDescription.VerifyRequestVersion(VersionUtil.Version4Dot0, dataService);

            using (Deserializer parameterDeserializer = Deserializer.CreateDeserializer(requestDescription, dataService, false /*update*/, UpdateTracker.CreateUpdateTracker(dataService)))
            {
                return (Dictionary<string, object>)parameterDeserializer.Deserialize(actionSegment);
            }
        }

        /// <summary>
        /// Deserializes the given stream into clr object as specified in the payload
        /// </summary>
        /// <param name="entityTypeInPayload">The payload entity type for the instance we are returning.</param>
        /// <returns>the object instance that it created and populated from the reader</returns>
        internal object ReadEntity(out ResourceType entityTypeInPayload)
        {
            RequestDescription requestDescription = this.RequestDescription;
            Debug.Assert(requestDescription != null, "requestDescription != null");

            if (requestDescription.TargetKind == RequestTargetKind.Resource)
            {
                Debug.Assert(requestDescription.LastSegmentInfo != null, "requestDescription.LastSegmentInfo != null");
                Debug.Assert(requestDescription.LastSegmentInfo.TargetResourceSet != null, "requestDescription.LastSegmentInfo.TargetContainer != null");
                Debug.Assert(requestDescription.TargetResourceType != null, "requestDescription.TargetResourceType != null");
            }

            // If the description points to a resource,
            // we need to materialize the object and return back.
            SegmentInfo segmentInfo = requestDescription.LastSegmentInfo;
            if (!this.Update)
            {
                Debug.Assert(!segmentInfo.SingleResult, "POST operation is allowed only on collections");
                SegmentInfo adjustedSegment = new SegmentInfo();
                adjustedSegment.TargetKind = segmentInfo.TargetKind;
                adjustedSegment.TargetSource = segmentInfo.TargetSource;
                adjustedSegment.SingleResult = true;
                adjustedSegment.ProjectedProperty = segmentInfo.ProjectedProperty;
                adjustedSegment.TargetResourceType = segmentInfo.TargetResourceType;
                adjustedSegment.TargetResourceSet = segmentInfo.TargetResourceSet;
                adjustedSegment.Identifier = segmentInfo.Identifier;
                segmentInfo = adjustedSegment;
            }

            object instance = this.Deserialize(segmentInfo);
            EntityDeserializer.ODataEntryAnnotation entryAnnotation = instance as EntityDeserializer.ODataEntryAnnotation;
            if (entryAnnotation != null)
            {
                entityTypeInPayload = entryAnnotation.EntityResourceType;
                return entryAnnotation.EntityResource;
            }

            entityTypeInPayload = null;
            return instance;
        }

        /// <summary>
        /// Handles post request.
        /// </summary>
        /// <param name="targetResourceType">Returns the true target resource type of the request.
        /// i.e. if a more derived type is on the payload, the type from the payload is returned here.</param>
        /// <returns>returns the resource that is getting inserted or binded - as specified in the payload.</returns>
        internal object HandlePostRequest(out ResourceType targetResourceType)
        {
            Debug.Assert(!this.Update, "This method must be called for POST operations only");
            object resourceInPayload;
            RequestDescription requestDescription = this.RequestDescription;
            targetResourceType = requestDescription.TargetResourceType;

            if (requestDescription.LinkUri)
            {
                // Entity reference link deserializer will return the Uri from the payload as the read resource.
                Uri uri = (Uri)this.Deserialize(null);
                resourceInPayload = this.GetTargetResourceToBind(uri, true /*checkNull*/);
                Debug.Assert(resourceInPayload != null, "link resource cannot be null");
                Deserializer.HandleBindOperation(requestDescription, resourceInPayload, this.Service, this.Tracker);
            }
            else
            {
                if (requestDescription.LastSegmentInfo.TargetResourceSet != null)
                {
                    DataServiceConfiguration.CheckResourceRights(requestDescription.LastSegmentInfo.TargetResourceSet, EntitySetRights.WriteAppend);
                }

                resourceInPayload = this.ReadEntity(out targetResourceType);
                Debug.Assert(targetResourceType != null, "targetResourceType != null");

                if (requestDescription.TargetSource == RequestTargetSource.Property)
                {
                    Debug.Assert(requestDescription.Property.Kind == ResourcePropertyKind.ResourceSetReference, "Expecting POST resource set property");
                    Deserializer.HandleBindOperation(requestDescription, resourceInPayload, this.Service, this.Tracker);
                }
                else
                {
                    Debug.Assert(requestDescription.TargetSource == RequestTargetSource.EntitySet, "Expecting POST on entity set");
                    this.Tracker.TrackAction(resourceInPayload, requestDescription.LastSegmentInfo.TargetResourceSet, UpdateOperations.Add);
                }
            }

            return resourceInPayload;
        }

        /// <summary>
        /// Update the resource specified in the given request description.
        /// </summary>
        /// <returns>Returns the entity being modified.</returns>
        internal object HandlePutRequest()
        {
            object requestValue = null;
            object entityGettingModified = null;
            ResourceSetWrapper container = null;
            string mimeType;
            Encoding encoding;
            AstoriaRequestMessage host = this.Service.OperationContext.RequestMessage;
            ContentTypeUtil.ReadContentType(host.ContentType, out mimeType, out encoding);
            RequestDescription requestDescription = this.RequestDescription;

            if (requestDescription.TargetKind == RequestTargetKind.MediaResource ||
                requestDescription.TargetKind == RequestTargetKind.OpenPropertyValue ||
                requestDescription.TargetKind == RequestTargetKind.PrimitiveValue)
            {
                requestValue = this.Deserialize(requestDescription.LastSegmentInfo);
            }
            else
            {
                if (requestDescription.LinkUri)
                {
                    // Entity reference link deserializer will return the Uri from the payload as the read resource.
                    Uri uri = (Uri)this.Deserialize(null);

                    // No need to check for null - if the uri in the payload is /Customer(1)/BestFriend,
                    // and the value is null, it means that the user wants to set the current link to null
                    // i.e. in other words, unbind the relationship.
                    object linkResource = this.GetTargetResourceToBind(uri, true /*checkNull*/);
                    entityGettingModified = Deserializer.HandleBindOperation(requestDescription, linkResource, this.Service, this.Tracker);
                    container = requestDescription.LastSegmentInfo.TargetResourceSet;
                }
                else
                {
                    ResourceType entityTypeInPayload;
                    requestValue = this.ReadEntity(out entityTypeInPayload);
                    if (requestValue == null &&
                        requestDescription.LastSegmentInfo.HasKeyValues &&
                        requestDescription.TargetSource == RequestTargetSource.EntitySet)
                    {
                        throw DataServiceException.CreateBadRequestError(Strings.BadRequest_CannotSetTopLevelResourceToNull(requestDescription.ResultUri.AbsoluteUri));
                    }
                }
            }

            // Update the property value, if the request target is property
            if (!requestDescription.LinkUri && IsQueryRequired(requestDescription, requestValue))
            {
                // Get the parent entity and its container and the resource to modify
                object resourceToModify = GetResourceToModify(
                    requestDescription, this.Service, false /*allowCrossReferencing*/, out entityGettingModified, out container, true /*checkETag*/);

                this.Tracker.TrackAction(entityGettingModified, container, UpdateOperations.Change);

                Deserializer.ModifyResource(requestDescription, resourceToModify, requestValue, this.Service);
            }

            return entityGettingModified ?? requestValue;
        }

        /// <summary>
        /// Update the object count value to the given value.
        /// </summary>
        /// <param name="value">value to be set for object count.</param>
        internal void UpdateObjectCount(int value)
        {
            Debug.Assert(0 <= value, "MaxObjectCount cannot be initialized to a negative number");
            Debug.Assert(value <= this.Service.Configuration.MaxObjectCountOnInsert, "On initialize, the value should be less than max object count");
            this.objectCount = value;
        }

        /// <summary>
        /// Set the value of the given resource property to the new value
        /// </summary>
        /// <param name="resourceProperty">property whose value needs to be updated</param>
        /// <param name="declaringResource">instance of the declaring type of the property for which the property value needs to be updated</param>
        /// <param name="propertyValue">new value for the property</param>
        /// <param name="service">Service this is request is against</param>
        protected static void SetPropertyValue(ResourceProperty resourceProperty, object declaringResource, object propertyValue, IDataService service)
        {
            Debug.Assert(
                resourceProperty.TypeKind == ResourceTypeKind.ComplexType ||
                resourceProperty.TypeKind == ResourceTypeKind.Primitive ||
                resourceProperty.TypeKind == ResourceTypeKind.Collection,
                "Only primitive and complex type values must be set via this method");

            service.Updatable.SetValue(declaringResource, resourceProperty.Name, propertyValue);
        }

        /// <summary>
        /// Set the value of the open property
        /// </summary>
        /// <param name="declaringResource">instance of the declaring type of the property for which the property value needs to be updated</param>
        /// <param name="propertyName">name of the open property to update</param>
        /// <param name="propertyValue">new value for the property</param>
        /// <param name="service">Service this request is against</param>
        protected static void SetOpenPropertyValue(object declaringResource, string propertyName, object propertyValue, IDataService service)
        {
            service.Updatable.SetValue(declaringResource, propertyName, propertyValue);
        }

        /// <summary>
        /// Reads the content from the stream reader and returns it as string
        /// </summary>
        /// <param name="streamReader">stream reader from which the content needs to be read</param>
        /// <returns>string containing the content as read from the stream reader</returns>
        protected static string ReadStringFromStream(StreamReader streamReader)
        {
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Make sure binding operations cannot be performed in PUT operations
        /// </summary>
        /// <param name="requestVerb">http method name for the request.</param>
        protected static void CheckForBindingInPutOperations(HttpVerbs requestVerb)
        {
            // Cannot bind in PUT operations.
            if (requestVerb == HttpVerbs.PUT)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_CannotUpdateRelatedEntitiesInPut);
            }
        }

        /// <summary>Creates a new SegmentInfo for the specified <paramref name="property"/>.</summary>
        /// <param name="property">Property to create segment info for (possibly null).</param>
        /// <param name="propertyName">Name for the property.</param>
        /// <param name="propertySet">Target resource set for the property.</param>
        /// <param name="singleResult">Whether a single result is expected.</param>
        /// <returns>
        /// A new <see cref="SegmentInfo"/> instance that describes the specfied <paramref name="property"/>
        /// as a target, or an open proprty if <paramref name="property"/> is null.
        /// </returns>
        protected static SegmentInfo CreateSegment(ResourceProperty property, string propertyName, ResourceSetWrapper propertySet, bool singleResult)
        {
            SegmentInfo result = new SegmentInfo();
            result.TargetSource = RequestTargetSource.Property;
            result.SingleResult = singleResult;
            result.Identifier = propertyName;
            if (property == null)
            {
                result.TargetKind = RequestTargetKind.OpenProperty;
            }
            else
            {
                result.TargetKind = RequestTargetKind.Resource;
                result.Identifier = propertyName;
                result.ProjectedProperty = property;
                result.TargetResourceType = property.ResourceType;
                result.TargetResourceSet = propertySet;
            }

            return result;
        }

        /// <summary>
        /// Reads the given payload and return the top level object.
        /// </summary>
        /// <param name="segmentInfo">Info about the object being created.</param>
        /// <returns>Instance of the object created.</returns>
        protected abstract object Deserialize(SegmentInfo segmentInfo);

        /// <summary>
        /// Provides an opportunity to clean-up resources.
        /// </summary>
        /// <param name="disposing">
        /// Whether the call is being made from an explicit call to 
        /// IDisposable.Dispose() rather than through the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Marks the fact that a recursive method was entered, and checks that the depth is allowed.</summary>
        protected void RecurseEnter()
        {
            WebUtil.RecurseEnter(RecursionLimit, ref this.recursionDepth);
        }

        /// <summary>Marks the fact that a recursive method is leaving.</summary>
        protected void RecurseLeave()
        {
            WebUtil.RecurseLeave(ref this.recursionDepth);
        }

        /// <summary>
        /// Returns the target/child resource to bind to a resource, which might be getting inserted or modified.
        /// Since this is a target resource, null is a valid value here (for e.g. /Customers(1)/BestFriend value
        /// can be null)
        /// </summary>
        /// <param name="uri">uri referencing to the resource to be returned.</param>
        /// <param name="checkNull">whether the resource can be null or not.</param>
        /// <returns>returns the resource as referenced by the uri. Throws 404 if the checkNull is true and the resource returned is null.</returns>
        protected object GetTargetResourceToBind(string uri, bool checkNull)
        {
            // (Dev10) sometime we check for null and sometimes we don't. We decided not to fix this and thus we need to keep the parameter.
            Uri referencedUri = RequestUriProcessor.GetAbsoluteUriFromReference(uri, this.Service.OperationContext);
            return this.GetTargetResourceToBind(referencedUri, checkNull);
        }

        /// <summary>
        /// Returns the target/child resource to bind to a resource, which might be getting inserted or modified.
        /// Since this is a target resource, null is a valid value here (for e.g. /Customers(1)/BestFriend value
        /// can be null)
        /// </summary>
        /// <param name="referencedUri">uri referencing to the resource to be returned.</param>
        /// <param name="checkNull">whether the resource can be null or not.</param>
        /// <returns>returns the resource as referenced by the uri. Throws 404 if the checkNull is true and the resource returned is null.</returns>
        protected object GetTargetResourceToBind(Uri referencedUri, bool checkNull)
        {
            Debug.Assert(referencedUri != null, "referencedUri != null");
            Debug.Assert(referencedUri.IsAbsoluteUri, "referencedUri.IsAbsoluteUri");

            // (Dev10) sometime we check for null and sometimes we don't. We decided not to fix this and thus we need to keep the parameter.
            RequestDescription requestDescription = RequestUriProcessor.ProcessRequestUri(referencedUri, this.Service, true /*internalQuery*/);

            // Get the resource
            object resourceCookie = this.Service.GetResource(requestDescription, requestDescription.SegmentInfos.Count - 1, null);
            if (checkNull)
            {
                WebUtil.CheckResourceExists(resourceCookie != null, requestDescription.LastSegmentInfo.Identifier);
            }

            return resourceCookie;
        }

        /// <summary>
        /// Gets a resource referenced by the given segment info.
        /// </summary>
        /// <param name="resourceType">resource type whose instance needs to be created</param>
        /// <param name="segmentInfo">segment info containing the description of the uri</param>
        /// <param name="verifyETag">verify etag value of the current resource with one specified in the request header</param>
        /// <param name="checkForNull">validate that the resource cannot be null.</param>
        /// <param name="replaceResource">reset the resource as referred by the segment.</param>
        /// <returns>a new instance of the given resource type with key values populated</returns>
        protected object GetObjectFromSegmentInfo(
            ResourceType resourceType,
            SegmentInfo segmentInfo,
            bool verifyETag,
            bool checkForNull,
            bool replaceResource)
        {
            Debug.Assert(resourceType == null && !verifyETag || resourceType != null, "For etag verification, resource type must be specified");

            if (segmentInfo.RequestExpression == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
            }

            object resourceCookie;
            if (WebUtil.IsCrossReferencedSegment(segmentInfo, this.service))
            {
                resourceCookie = Deserializer.GetCrossReferencedResource(segmentInfo);
            }
            else
            {
                resourceCookie = Deserializer.GetResource(
                    segmentInfo,
                    resourceType != null ? resourceType.FullName : null,
                    this.Service,
                    checkForNull);

                // We only need to check etag if the resource is not cross-referenced. If the resource is cross-referenced,
                // there is no good way to checking etag for that resource, since it might have 
                if (verifyETag)
                {
                    this.service.Updatable.SetETagValues(resourceCookie, segmentInfo.TargetResourceSet);
                }
            }

            if (replaceResource)
            {
                Debug.Assert(checkForNull, "For resetting resource, the value cannot be null");
                resourceCookie = this.Updatable.ResetResource(resourceCookie);
                WebUtil.CheckResourceExists(resourceCookie != null, segmentInfo.Identifier);
            }

            return resourceCookie;
        }

        /// <summary>
        /// Check and increment the object count
        /// </summary>
        protected void CheckAndIncrementObjectCount()
        {
            Debug.Assert(this.Update && this.objectCount == 0 || !this.Update, "For updates, the object count is never tracked");
            Debug.Assert(this.objectCount <= this.Service.Configuration.MaxObjectCountOnInsert, "The object count should never exceed the limit");

            if (!this.Update)
            {
                this.objectCount++;

                if (this.objectCount > this.Service.Configuration.MaxObjectCountOnInsert)
                {
                    throw new DataServiceException(413, Strings.BadRequest_ExceedsMaxObjectCountOnInsert(this.Service.Configuration.MaxObjectCountOnInsert));
                }
            }
        }

        /// <summary>
        /// Bump the minimum DSV requirement based on the specifics of the given resource type.
        /// </summary>
        /// <param name="resourceType">Resource type to inspect</param>
        /// <param name="topLevel">True if resourceType is the type for the top level element in the payload.</param>
        protected void UpdateAndCheckRequestResponseDSV(ResourceType resourceType, bool topLevel)
        {
            Debug.Assert(this.Service != null, "this.Service != null");
            Debug.Assert(this.Service.Provider != null, "this.Service.Provider != null");
            Debug.Assert(resourceType != null, "Must have valid resource type");
            Debug.Assert(resourceType.ResourceTypeKind == ResourceTypeKind.EntityType || resourceType.ResourceTypeKind == ResourceTypeKind.ComplexType, "resourceType is structured type");
            Debug.Assert(this.RequestDescription != null, "this.RequestDescription != null");
            Debug.Assert(this.RequestDescription.LastSegmentInfo != null, "this.RequestDescription.LastSegmentInfo != null");
            Debug.Assert(this.RequestDescription.LastSegmentInfo.TargetResourceSet != null, "this.RequestDescription.LastSegmentInfo.TargetContainer != null");

            // Raise the response version if a response will be sent.
            if (topLevel && this.ResponseWillBeSent)
            {
                // We only raise the response version here but not minimum request version. 
                // Because named stream links are not allowed on the request payload
                // and we allow collection properties in the request payload regardless of the request DSV since 
                // we can easily recognize them (either having m:type="Collection()", or being declared in our metadata as collection)
                // and thus there's no chance to misinterpret the payload.
                //
                // we are getting the MinimumPayloadVersion from the type because we know the specific type so we
                // can get a more specific version this way
                ResourceSetWrapper resourceSet = this.RequestDescription.LastSegmentInfo.TargetResourceSet;
                Version minimumPayloadVersion = resourceType.GetMinimumResponseVersion(this.Service, resourceSet);
                minimumPayloadVersion = resourceType.GetMinimumResponseVersion(this.Service, resourceSet);

                this.RequestDescription.VerifyAndRaiseResponseVersion(minimumPayloadVersion, this.Service);
            }
        }

        /// <summary>
        /// Returns collection type for the specified type name from the payload.
        /// </summary>
        /// <param name="typeName">Collection type name read from payload.</param>
        /// <param name="propertyName">The name of the property being read.</param>
        /// <returns>Collection type.</returns>
        protected CollectionResourceType GetCollectionTypeFromName(string typeName, string propertyName)
        {
            Debug.Assert(propertyName != null, "propertyName != null");

            if (string.IsNullOrEmpty(typeName))
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidTypeName(typeName));
            }

            string itemTypeName = CommonUtil.GetCollectionItemTypeName(typeName, false);
            if (itemTypeName == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_CollectionTypeExpected(typeName, propertyName));
            }

            ResourceType itemType = WebUtil.TryResolveResourceType(this.Service.Provider, itemTypeName);
            if (itemType == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_InvalidTypeName(typeName));
            }

            // This will throw if the item type is not supported for collections (for example if it's an entity type)
            return ResourceType.GetCollectionResourceType(itemType);
        }

        /// <summary>
        /// Returns true if we need to query the provider before updating.
        /// </summary>
        /// <param name="requestDescription">request description</param>
        /// <param name="requestValue">value corresponding to the payload for this request</param>
        /// <returns>returns true if we need to issue an query to satishfy the request</returns>
        private static bool IsQueryRequired(RequestDescription requestDescription, object requestValue)
        {
            Debug.Assert(requestDescription.IsSingleResult, "requestDescription.IsSingleResult");

            if (requestDescription.TargetKind == RequestTargetKind.PrimitiveValue ||
                requestDescription.TargetKind == RequestTargetKind.Primitive ||
                requestDescription.TargetKind == RequestTargetKind.OpenPropertyValue ||
                requestDescription.TargetKind == RequestTargetKind.MediaResource ||
                requestDescription.TargetKind == RequestTargetKind.ComplexObject ||
                requestDescription.TargetKind == RequestTargetKind.Collection)
            {
                return true;
            }

            if (requestDescription.TargetKind == RequestTargetKind.OpenProperty)
            {
                Debug.Assert(!requestDescription.LastSegmentInfo.HasKeyValues, "CreateSegments must have caught this issue.");
                ResourceType requestResourceType = requestDescription.LastSegmentInfo.TargetResourceType;

                // If this open property is collection, use resourceType in requestValue.
                var requestCollectionValue = requestValue as CollectionPropertyValueEnumerable;
                if (requestResourceType == null && requestCollectionValue != null)
                {
                    requestResourceType = requestCollectionValue.ResourceType;
                    return true;
                }

                Debug.Assert(requestValue == null || requestResourceType != null, "requestResourceType != null");

                // if the value is null, then just set it, since we don't know the type
                if (requestValue == null || requestResourceType.ResourceTypeKind == ResourceTypeKind.Primitive)
                {
                    return true;
                }

                // otherwise just set the complex type properties
                if (requestResourceType.ResourceTypeKind == ResourceTypeKind.ComplexType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the entity that need to get modified
        /// </summary>
        /// <param name="description">description about the target request</param>
        /// <param name="service">data service type to which the request was made</param>
        /// <param name="allowCrossReferencing">whether cross-referencing is allowed for the resource in question.</param>
        /// <param name="entityContainer">entity container of the entity which is getting modified.</param>
        /// <param name="entityResourceIndex">index of the segment which refers to the entity getting modified.</param>
        /// <returns>Returns the entity that needs to get modified</returns>
        private static object GetEntityResourceToModify(
           RequestDescription description,
           IDataService service,
           bool allowCrossReferencing,
           out ResourceSetWrapper entityContainer,
           out int entityResourceIndex)
        {
            Debug.Assert(description.SegmentInfos.Count >= 2, "description.SegmentInfos.Count >= 2");

            if (!allowCrossReferencing && description.RequestExpression == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
            }

            // Get the index of the entity resource that is getting modified
            entityResourceIndex = Deserializer.GetIndexOfEntityResourceToModify(description);
            Debug.Assert(entityResourceIndex != -1, "This method should never be called for request that doesn't have a parent resource");
            entityContainer = description.SegmentInfos[entityResourceIndex].TargetResourceSet;
            Debug.Assert(entityContainer != null, "Entity container cannot be null for segments whose targetkind is resource");

            AstoriaRequestMessage host = service.OperationContext.RequestMessage;

            // Since this is the entity which is going to get modified, then we need to check for rights
            if (host.HttpVerb == HttpVerbs.PUT)
            {
                DataServiceConfiguration.CheckResourceRights(entityContainer, EntitySetRights.WriteReplace);
            }
            else if (host.HttpVerb == HttpVerbs.PATCH)
            {
                DataServiceConfiguration.CheckResourceRights(entityContainer, EntitySetRights.WriteMerge);
            }
            else
            {
                Debug.Assert(
                    host.HttpVerb == HttpVerbs.POST ||
                    host.HttpVerb == HttpVerbs.DELETE,
                    "expecting POST and DELETE methods");
                DataServiceConfiguration.CheckResourceRights(entityContainer, EntitySetRights.WriteMerge | EntitySetRights.WriteReplace);
            }

            object entityResource = service.GetResource(description, entityResourceIndex, null);
            if (entityResource == null)
            {
                throw DataServiceException.CreateBadRequestError(Strings.BadRequest_DereferencingNullPropertyValue(description.SegmentInfos[entityResourceIndex].Identifier));
            }

            return entityResource;
        }
    }
}
