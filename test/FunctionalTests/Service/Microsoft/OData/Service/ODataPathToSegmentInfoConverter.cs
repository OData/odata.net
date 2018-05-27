//---------------------------------------------------------------------
// <copyright file="ODataPathToSegmentInfoConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.UriParser;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// Semantic parser for the path of the request URI.
    /// </summary>
    internal sealed class ODataPathToSegmentInfoConverter
    {
        /// <summary>
        /// The provider wrapper to use for looking up types/sets, etc.
        /// </summary>
        private readonly DataServiceProviderWrapper providerWrapper;

        /// <summary>
        /// A callback to get cross-referenced segments (ie '$0') when inside a batch request changeset.
        /// </summary>
        private readonly Func<string, SegmentInfo> crossReferenceCallback;

        /// <summary>
        /// The max protocol version of the service.
        /// </summary>
        private readonly Version maxProtocolVersion;

        /// <summary>
        /// Initializes a new instance of <see cref="ODataPathToSegmentInfoConverter"/>.
        /// </summary>
        /// <param name="maxProtocolVersion">The max protocol version of the service.</param>
        /// <param name="providerWrapper">The provider wrapper to use for looking up types/sets, etc.</param>
        /// <param name="crossReferenceCallback">A callback to get cross-referenced segments (ie '$0') when inside a batch request changeset.</param>
        private ODataPathToSegmentInfoConverter(
            Version maxProtocolVersion,
            DataServiceProviderWrapper providerWrapper,
            Func<string, SegmentInfo> crossReferenceCallback)
        {
            Debug.Assert(providerWrapper != null, "providerWrapper != null");
            Debug.Assert(crossReferenceCallback != null, "crossReferenceCallback != null");
            Debug.Assert(maxProtocolVersion != null, "maxProtocolVersion != null");

            this.providerWrapper = providerWrapper;
            this.crossReferenceCallback = crossReferenceCallback;
            this.maxProtocolVersion = maxProtocolVersion;
        }

        /// <summary>
        /// Creates a new <see cref="ODataPathToSegmentInfoConverter"/> for the given data service.
        /// </summary>
        /// <param name="service">The data service.</param>
        /// <returns>A new path parsers.</returns>
        internal static ODataPathToSegmentInfoConverter Create(IDataService service)
        {
            Debug.Assert(service != null, "service != null");
            return new ODataPathToSegmentInfoConverter(
                service.Configuration.DataServiceBehavior.MaxProtocolVersion.ToVersion(),
                service.Provider,
                service.GetSegmentForContentId);
        }

        /// <summary>Creates an <see cref="SegmentInfo"/> list for the given <paramref name="path"/>.</summary>
        /// <param name="path">Segments to process.</param>
        /// <returns>Segment information describing the given <paramref name="path"/>.</returns>
        internal IList<SegmentInfo> ConvertPath(ODataPath path)
        {
            Debug.Assert(path != null, "path != null");

            SegmentInfo previous = null;
            List<SegmentInfo> segmentInfos = new List<SegmentInfo>();
            bool crossReferencingUri = false;
            foreach (ODataPathSegment segment in path)
            {
                crossReferencingUri |= segment is BatchReferenceSegment;

                SegmentInfo segmentInfo;
                if (previous == null)
                {
                    segmentInfo = this.CreateFirstSegment(segment);
                }
                else
                {
                    ThrowIfMustBeLeafSegment(previous);

                    var keySegment = segment as KeySegment;
                    if (keySegment != null)
                    {
                        // All service operations other than those returning IQueryable MUST NOT have a key expression.
                        if (!IsSegmentComposable(previous))
                        {
                            throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_SegmentDoesNotSupportKeyPredicates(previous.Identifier));
                        }

                        CheckSegmentIsComposable(previous);

                        if (crossReferencingUri)
                        {
                            throw DataServiceException.CreateBadRequestError(Strings.BadRequest_ResourceCanBeCrossReferencedOnlyForBindOperation);
                        }

                        previous.SingleResult = true;
                        previous.Key = keySegment;
                        continue;
                    }

                    if (segment is NavigationPropertyLinkSegment)
                    {
                        segmentInfo = CreateEntityReferenceLinkSegment(previous);
        #if DEBUG
                        segmentInfo.AssertValid();
        #endif
                        segmentInfos.Add(segmentInfo);

                        previous = segmentInfo;
                    }

                    segmentInfo = this.CreateNextSegment(previous, segment);
                }

                Debug.Assert(segmentInfo != null, "segment != null");
#if DEBUG
                segmentInfo.AssertValid();
#endif
                segmentInfos.Add(segmentInfo);

                // we need to copy the segment over (even if it was an escape marker) as decisions will be made about it the next time through the loop.
                previous = segmentInfo;
            }

            return segmentInfos;
        }

        /// <summary>
        /// Creates a segment for a service operation
        /// </summary>
        /// <param name="serviceOperation">The service operation for the segment.</param>
        /// <returns>A fully populated PathSegment representing the service operation</returns>
        private static SegmentInfo CreateSegmentForServiceOperation(OperationWrapper serviceOperation)
        {
            Debug.Assert(serviceOperation != null, "serviceOperation != null");
            WebUtil.DebugEnumIsDefined(serviceOperation.ResultKind);

            SegmentInfo segment = new SegmentInfo
            {
                Identifier = serviceOperation.Name,
                Operation = serviceOperation, 
                TargetSource = RequestTargetSource.ServiceOperation,
                TargetResourceSet = serviceOperation.ResourceSet
            };

            if (serviceOperation.ResultKind != ServiceOperationResultKind.Void)
            {
                segment.TargetResourceType = serviceOperation.ResultType;
                segment.TargetKind = TargetKindFromType(segment.TargetResourceType);

                // this service operation returns results
                // we should check service operation rights
                // SingleResult operations are service operations defined with [SingleResult] attribute, returns a DirectValue
                // or a service operation returning multiple results, but contains key predicates in the query portion.
                // For these operations, you MUST have ReadSingle defined
                // For multiple-result-operations(IQueryable/IEnumerable), you MUST have ReadMultiple defined.
                segment.SingleResult = (serviceOperation.ResultKind == ServiceOperationResultKind.QueryWithSingleResult || serviceOperation.ResultKind == ServiceOperationResultKind.DirectValue);
            }
            else
            {
                segment.TargetResourceType = null;
                segment.TargetKind = RequestTargetKind.VoidOperation;
            }

            return segment;
        }

        /// <summary>
        /// Throws if the given segment must be a leaf, as a later segment is being created.
        /// </summary>
        /// <param name="previous">The previous segment which may need to be a leaf.</param>
        private static void ThrowIfMustBeLeafSegment(SegmentInfo previous)
        {
            if (previous.IsServiceActionSegment)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_MustBeLeafSegment(previous.Identifier));
            }

            if (previous.TargetKind == RequestTargetKind.Batch || previous.TargetKind == RequestTargetKind.Metadata || previous.TargetKind == RequestTargetKind.PrimitiveValue || previous.TargetKind == RequestTargetKind.VoidOperation || previous.TargetKind == RequestTargetKind.OpenPropertyValue || previous.TargetKind == RequestTargetKind.MediaResource || previous.TargetKind == RequestTargetKind.Collection)
            {
                // Nothing can come after a $metadata, $value or $batch segment.
                // Nothing can come after a service operation with void return type.
                // Nothing can come after a collection property.
                throw DataServiceException.ResourceNotFoundError(Strings.RequestUriProcessor_MustBeLeafSegment(previous.Identifier));
            }
        }

        /// <summary>
        /// Checks that the previous segment can be composed upon.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        private static void CheckSegmentIsComposable(SegmentInfo previous)
        {
            if (!IsSegmentComposable(previous))
            {
                // Enumerable and DirectValue results cannot be composed at all, and we don't allow we can't access properties in a single queryable complex either
                throw DataServiceException.ResourceNotFoundError(
                    Strings.RequestUriProcessor_IEnumerableServiceOperationsCannotBeFurtherComposed(previous.Identifier));
            }
        }

        /// <summary>
        /// Determines if the previous segment can be composed upon.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <returns>
        ///   <c>true</c> if the segment can be composed upon; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsSegmentComposable(SegmentInfo previous)
        {
            OperationWrapper op = previous.Operation;
            return op == null || 
                   (op.ResultKind != ServiceOperationResultKind.Enumeration 
                    && op.ResultKind != ServiceOperationResultKind.DirectValue 
                    && (op.ResultKind != ServiceOperationResultKind.QueryWithSingleResult
                        || op.ResultType.ResourceTypeKind == ResourceTypeKind.EntityType));
        }

        /// <summary>
        /// Handle $count segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <returns>The count segment info</returns>
        private static SegmentInfo CreateCountSegment(SegmentInfo previous)
        {
            CheckSegmentIsComposable(previous);

            if ((previous.TargetKind != RequestTargetKind.Resource || previous.SingleResult) && previous.TargetKind != RequestTargetKind.Collection)
            {
                throw DataServiceException.CreateResourceNotFound(Strings.RequestUriProcessor_CountNotSupported(previous.Identifier));
            }

            var segment = new SegmentInfo();
            segment.Identifier = XmlConstants.UriCountSegment;

            // DEVNOTE: prior to refactoring, $count was handled alongside properties
            // TODO: introduce a new target-source value or otherwise refactor this so that
            // $count is not treated like a property. Using the previous segment's kind
            // becomes problematic for service operations because the current segment
            // will not know the specific operation.
            segment.TargetSource = RequestTargetSource.Property;

            segment.SingleResult = true;
            segment.TargetKind = RequestTargetKind.PrimitiveValue;
            segment.TargetResourceType = previous.TargetResourceType;
            segment.TargetResourceSet = previous.TargetResourceSet;

            return segment;
        }

        /// <summary>
        /// Handle $ref segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <returns>The links segment info</returns>
        private static SegmentInfo CreateEntityReferenceLinkSegment(SegmentInfo previous)
        {
            Debug.Assert(previous.TargetKind == RequestTargetKind.Resource, "Can we ever get here?");
            WebUtil.CheckSyntaxValid(previous.TargetKind == RequestTargetKind.Resource);
            CheckSingleResult(previous.SingleResult, previous.Identifier);

            return new SegmentInfo(previous) { Identifier = XmlConstants.UriLinkSegment, TargetKind = RequestTargetKind.Link, };
        }

        /// <summary>
        /// Create a $value segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <returns>new segement info for $value.</returns>
        private static SegmentInfo CreateValueSegment(SegmentInfo previous)
        {
            SegmentInfo segment;
            if (previous.TargetKind == RequestTargetKind.Primitive)
            {
                segment = new SegmentInfo(previous);
            }
            else
            {
                CheckSegmentIsComposable(previous);

                segment = new SegmentInfo
                {
                    TargetSource = RequestTargetSource.Property,
                    TargetResourceType = previous.TargetResourceType
                };
            }

            segment.Identifier = XmlConstants.UriValueSegment;
            CheckSingleResult(previous.SingleResult, previous.Identifier);

            segment.SingleResult = true;

            if (previous.TargetKind == RequestTargetKind.Primitive)
            {
                segment.TargetKind = RequestTargetKind.PrimitiveValue;
            }
            else if (previous.TargetKind == RequestTargetKind.OpenProperty)
            {
                segment.TargetKind = RequestTargetKind.OpenPropertyValue;
            }
            else
            {
                // If the previous segment is an entity, we expect it to be an MLE. We cannot validate our assumption
                // until later when we get the actual instance of the entity because the type hierarchy can contain
                // a mix of MLE and non-MLE types.
                segment.TargetKind = RequestTargetKind.MediaResource;
            }

            return segment;
        }

        /// <summary>
        /// Creates a new segment for an open property.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="identifier">name of the segment.</param>
        /// <returns>new open property segment.</returns>
        private static SegmentInfo CreateOpenPropertySegment(SegmentInfo previous, string identifier)
        {
            SegmentInfo segment = new SegmentInfo { Identifier = identifier };

            // Handle an open type property. If the current leaf isn't an 
            // object (which implies it's already an open type), then
            // it should be marked as an open type.
            if (previous.TargetResourceType != null)
            {
                WebUtil.CheckResourceExists(previous.TargetResourceType.IsOpenType, segment.Identifier);
            }

            // Open navigation properties are not supported on OpenTypes.  We should throw on the following cases:
            // 1. the segment before $ref is always a navigation property pointing to a resource
            // 2. if the segment hasQuery, it is pointing to a resource
            // 3. if this is a POST operation, the target has to be either a set of links or an entity set
            if (previous.TargetKind == RequestTargetKind.Link)
            {
                throw DataServiceException.CreateBadRequestError(Strings.OpenNavigationPropertiesNotSupportedOnOpenTypes(segment.Identifier));
            }

            segment.TargetSource = RequestTargetSource.Property;
            segment.TargetResourceType = null;
            segment.TargetKind = RequestTargetKind.OpenProperty;
            segment.SingleResult = true;
            return segment;
        }

        /// <summary>
        /// Creates a named stream segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="streamProperty">stream property to create the segment for.</param>
        /// <returns>new named stream segment.</returns>
        private static SegmentInfo CreateNamedStreamSegment(SegmentInfo previous, ResourceProperty streamProperty)
        {
            Debug.Assert(streamProperty.IsOfKind(ResourcePropertyKind.Stream), "streamProperty.IsOfKind(ResourcePropertyKind.Stream)");

            // Handle Named Stream.
            SegmentInfo segment = new SegmentInfo() { Identifier = streamProperty.Name };
            segment.TargetKind = RequestTargetKind.MediaResource;
            segment.SingleResult = true;
            segment.TargetResourceType = previous.TargetResourceType;
            segment.TargetSource = RequestTargetSource.Property;
            Debug.Assert(segment.Identifier != XmlConstants.UriValueSegment, "'$value' cannot be the name of a named stream.");

            return segment;
        }

        /// <summary>Determines a matching target kind from the specified type.</summary>
        /// <param name="type">ResourceType of element to get kind for.</param>
        /// <returns>An appropriate <see cref="RequestTargetKind"/> for the specified <paramref name="type"/>.</returns>
        private static RequestTargetKind TargetKindFromType(ResourceType type)
        {
            Debug.Assert(type != null, "type != null");

            switch (type.ResourceTypeKind)
            {
                case ResourceTypeKind.ComplexType:
                    return RequestTargetKind.ComplexObject;
                case ResourceTypeKind.EntityType:
                case ResourceTypeKind.EntityCollection:
                    return RequestTargetKind.Resource;
                case ResourceTypeKind.Collection:
                    return RequestTargetKind.Collection;
                default:
                    Debug.Assert(type.ResourceTypeKind == ResourceTypeKind.Primitive, "typeKind == ResourceTypeKind.Primitive");
                    return RequestTargetKind.Primitive;
            }
        }

        /// <summary>
        /// Checks for single result, otherwise throws.
        /// </summary>
        /// <param name="isSingleResult">indicates whether the current result is single result or not.</param>
        /// <param name="identifier">current segment identifier.</param>
        private static void CheckSingleResult(bool isSingleResult, string identifier)
        {
            if (!isSingleResult)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_CannotQueryCollections(identifier));
            }
        }

        /// <summary>
        /// Tries to get the resource property for the given segment, if it is one of the segment types that refers to a property, a navigation, or a navigation before $ref.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <param name="projectedProperty">The property, if the segment represented a property or navigation.</param>
        /// <returns>Whether the segment represented a property or navigation.</returns>
        private static bool TryGetPropertyFromSegment(ODataPathSegment segment, out ResourceProperty projectedProperty)
        {
            Debug.Assert(segment != null, "segment != null");

            IEdmProperty edmProperty = null;
            var propertySegment = segment as PropertySegment;
            if (propertySegment != null)
            {
                edmProperty = propertySegment.Property;
            }
            else
            {
                var navigationSegment = segment as NavigationPropertySegment;
                if (navigationSegment != null)
                {
                    edmProperty = navigationSegment.NavigationProperty;
                }
                else
                {
                    var propertyLinkSegment = segment as NavigationPropertyLinkSegment;
                    if (propertyLinkSegment != null)
                    {
                        edmProperty = propertyLinkSegment.NavigationProperty;
                    }
                }
            }

            if (edmProperty != null)
            {
                projectedProperty = ((IResourcePropertyBasedEdmProperty)edmProperty).ResourceProperty;
                Debug.Assert(projectedProperty != null, "projectedProperty != null");
                return true;
            }

            projectedProperty = null;
            return false;
        }

        /// <summary>Creates the first <see cref="SegmentInfo"/> for a request.</summary>
        /// <param name="segment">The text of the segment.</param>
        /// <returns>A description of the information on the segment.</returns>
        private SegmentInfo CreateFirstSegment(ODataPathSegment segment)
        {
            Debug.Assert(segment != null, "identifier != null");

            // Look for well-known system entry points.
            if (segment is MetadataSegment)
            {
                return new SegmentInfo { Identifier = XmlConstants.UriMetadataSegment, TargetKind = RequestTargetKind.Metadata };
            }

            if (segment is BatchSegment)
            {
                return new SegmentInfo { Identifier = XmlConstants.UriBatchSegment, TargetKind = RequestTargetKind.Batch };
            }

            if (segment is CountSegment)
            {
                // $count on root: throw
                throw DataServiceException.CreateResourceNotFound(Strings.RequestUriProcessor_CountOnRoot);
            }

            // Look for a service operation.
            OperationImportSegment serviceOperation = segment as OperationImportSegment;
            if (serviceOperation != null)
            {
                Debug.Assert(serviceOperation.OperationImports.Count() == 1, "Operation import segment should only ever have exactly one operation. Was a change made to how MetadataProviderEdmModel finds actions/service operations");
                var operationImport = serviceOperation.OperationImports.Single();
                var operation = ((MetadataProviderEdmOperationImport)operationImport).ServiceOperation;
                Debug.Assert(operation != null, "operation != null");

                if (operation.Kind == OperationKind.ServiceOperation)
                {
                    return CreateSegmentForServiceOperation(operation);
                }

                Debug.Assert(operation.Kind == OperationKind.Action, "serviceAction.Kind == OperationKind.Action");
                return this.CreateSegmentForServiceAction(null /*previousSegment*/, operation);
            }

            var batchReferenceSegment = segment as BatchReferenceSegment;
            if (batchReferenceSegment != null)
            {
                SegmentInfo referencedSegmentInfo = this.crossReferenceCallback(batchReferenceSegment.ContentId);
                Debug.Assert(referencedSegmentInfo != null, "Could not find SegmentInfo for content-id: " + batchReferenceSegment.ContentId);
                referencedSegmentInfo.Identifier = batchReferenceSegment.ContentId;
                return referencedSegmentInfo;
            }

            // Look for an entity set.
            EntitySetSegment entitySetSegment = segment as EntitySetSegment;
            if (entitySetSegment != null)
            {
                var container = ((IResourceSetBasedEdmEntitySet)entitySetSegment.EntitySet).ResourceSet;
                Debug.Assert(container != null, "container != null");
                SegmentInfo segmentInfo = new SegmentInfo
                {
                    Identifier = container.Name,
                    TargetResourceSet = container,
                    TargetResourceType = container.ResourceType,
                    TargetSource = RequestTargetSource.EntitySet,
                    TargetKind = RequestTargetKind.Resource,
                    SingleResult = false
                };

                return segmentInfo;
            }

            WebUtil.CheckResourceExists(false, segment.ToString());
            return null;
        }

        /// <summary>
        /// Creates a segment for the given service action.
        /// </summary>
        /// <param name="previousSegment">The previous segment before the operation to be invoked.</param>
        /// <param name="serviceAction">The service action to create the segment for.</param>
        /// <returns>A fully populated PathSegment representing the service action</returns>
        private SegmentInfo CreateSegmentForServiceAction(SegmentInfo previousSegment, OperationWrapper serviceAction)
        {
            Debug.Assert(serviceAction != null && serviceAction.Kind == OperationKind.Action, "serviceAction != null && serviceAction.Kind == OperationKind.Action");

            SegmentInfo segment = new SegmentInfo() { Identifier = serviceAction.Name, Operation = serviceAction };

            WebUtil.DebugEnumIsDefined(serviceAction.ResultKind);
            Debug.Assert(segment.IsServiceActionSegment, "IsServiceActionSegment(segment)");

            if (previousSegment != null && previousSegment.TargetKind == RequestTargetKind.Link)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(serviceAction.Name, XmlConstants.UriLinkSegment));
            }

            segment.TargetSource = RequestTargetSource.ServiceOperation;

            if (serviceAction.ResultKind != ServiceOperationResultKind.Void)
            {
                segment.TargetResourceSet = serviceAction.GetResultSet(this.providerWrapper, previousSegment == null ? null : previousSegment.TargetResourceSet);
                segment.TargetResourceType = serviceAction.ReturnType;
                segment.TargetKind = TargetKindFromType(segment.TargetResourceType);
                if (segment.TargetKind == RequestTargetKind.Resource && segment.TargetResourceSet == null)
                {
                    // Service actions are either visible (ServiceActionRights.Invoke) or not (ServiceActionRight.None). The fact that
                    // DataServiceActionProviderWrapper.TryResolveServiceAction() returns a non-null value means the action is visible.
                    // If the result of the action is of entity type, we need to make sure the target set is visible or else the self
                    // and edit links of the entities in the response payload would not be usable.
                    Debug.Assert(serviceAction.IsVisible, "serviceAction.IsVisible");
                    throw DataServiceException.CreateForbidden();
                }

                segment.SingleResult = serviceAction.ResultKind == ServiceOperationResultKind.DirectValue;
                Debug.Assert(serviceAction.ResultKind != ServiceOperationResultKind.QueryWithSingleResult, "QueryWithSingleResult is not applicable for Actions.");
            }
            else
            {
                segment.TargetResourceSet = null;
                segment.TargetResourceType = null;
                segment.TargetKind = RequestTargetKind.VoidOperation;
            }

            return segment;
        }

        /// <summary>
        /// Creates the next segment.
        /// </summary>
        /// <param name="previous">The previous segment.</param>
        /// <param name="segment">The the next segment.</param>
        /// <returns>The newly created next segment.</returns>
        private SegmentInfo CreateNextSegment(SegmentInfo previous, ODataPathSegment segment)
        {
            if (segment is ValueSegment)
            {
                return CreateValueSegment(previous);
            }

            SegmentInfo segmentInfo;
            if (segment is CountSegment)
            {
                segmentInfo = CreateCountSegment(previous);

                // DEVNOTE: Prior to refactor, $count was handled alongside properties. See more detailed comment in HandleCountSegment
                Debug.Assert(segmentInfo.TargetSource == RequestTargetSource.Property, "segment.TargetSource == RequestTargetSource.Property");
                return segmentInfo;
            }

            // if its not one of the recognized special segments, then it must be a property, type-segment, or key value.
            Debug.Assert(
                previous.TargetKind == RequestTargetKind.ComplexObject
                || previous.TargetKind == RequestTargetKind.Resource
                || previous.TargetKind == RequestTargetKind.OpenProperty
                || previous.TargetKind == RequestTargetKind.Link,
                "previous.TargetKind(" + previous.TargetKind + ") can have properties");

            CheckSegmentIsComposable(previous);

            // if the segment corresponds to a declared property, handle it
            // otherwise, fall back to type-segments, actions, and dynamic/open properties
            ResourceProperty projectedProperty;
            if (TryGetPropertyFromSegment(segment, out projectedProperty))
            {
                CheckSingleResult(previous.SingleResult, previous.Identifier);

                if (projectedProperty.IsOfKind(ResourcePropertyKind.Stream))
                {
                    segmentInfo = CreateNamedStreamSegment(previous, projectedProperty);
                    Debug.Assert(segmentInfo.TargetSource == RequestTargetSource.Property, "segment.TargetSource == RequestTargetSource.Property");
                    return segmentInfo;
                }

                segmentInfo = this.CreatePropertySegment(previous, projectedProperty);
                Debug.Assert(segmentInfo.TargetSource == RequestTargetSource.Property, "segment.TargetSource == RequestTargetSource.Property");
                return segmentInfo;
            }

            // If the property resolution failed, and the previous segment was targeting an entity, then we should
            // try and resolve the identifier as type name.
            SegmentInfo typeNameSegment;
            if (this.TryCreateTypeNameSegment(previous, segment, out typeNameSegment))
            {
                Debug.Assert(typeNameSegment.TargetSource == previous.TargetSource, "segment.TargetSource == previous.TargetSource");
                return typeNameSegment;
            }

            OperationWrapper serviceAction = null;
            var actionSegment = segment as OperationSegment;
            if (actionSegment != null)
            {
                Debug.Assert(actionSegment.Operations.Count() == 1, "Operation segment should only ever have exactly one operation. Was a change made to how MetadataProviderEdmModel finds actions/service operations");
                serviceAction = ((MetadataProviderEdmOperation)actionSegment.Operations.Single()).ServiceOperation;
                Debug.Assert(serviceAction != null, "serviceAction != null");
                Debug.Assert(serviceAction.Kind == OperationKind.Action, "serviceAction.Kind == OperationKind.Action");
            }

            if (serviceAction != null)
            {
                Debug.Assert(serviceAction.Kind == OperationKind.Action, "serviceAction.Kind == OperationKind.Action");

                // Service Actions can bind to a set with any ResourceSetRights except ResourceSetRights.None.
                segmentInfo = this.CreateSegmentForServiceAction(previous, serviceAction);
                Debug.Assert(segmentInfo.TargetSource == RequestTargetSource.ServiceOperation, "segment.TargetSource == RequestTargetSource.ServiceOperation");
                return segmentInfo;
            }

            CheckSingleResult(previous.SingleResult, previous.Identifier);
            segmentInfo = CreateOpenPropertySegment(previous, ((DynamicPathSegment)segment).Identifier);
            Debug.Assert(segmentInfo.TargetSource == RequestTargetSource.Property, "segment.TargetSource == RequestTargetSource.Property");
            return segmentInfo;
        }

        /// <summary>
        /// Tries to create a type name segment if the given identifier refers to a known type.
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="segment">The segment being interpreted.</param>
        /// <param name="typeNameSegment">The type name segment, if one was created.</param>
        /// <returns>Whether or not a type segment was created for the identifier.</returns>
        private bool TryCreateTypeNameSegment(SegmentInfo previous, ODataPathSegment segment, out SegmentInfo typeNameSegment)
        {
            var typeSegment = segment as TypeSegment;
            if (typeSegment == null || previous.TargetResourceSet == null)
            {
                typeNameSegment = null;
                return false;
            }

            ResourceType targetResourceType = MetadataProviderUtils.GetResourceType(typeSegment);
            
            // if the new type segment prevents any results from possibly being returned, then short-circuit and throw a 404.
            ResourceType previousResourceType = previous.TargetResourceType;
            Debug.Assert(previousResourceType != null, "previous.TargetResourceType != null");
            if (!targetResourceType.IsAssignableFrom(previousResourceType) && !previousResourceType.IsAssignableFrom(targetResourceType))
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_InvalidTypeIdentifier_UnrelatedType(targetResourceType.FullName, previousResourceType.FullName));
            }

            // Since we allow derived navigation properties or named streams in V1/V2, the server will generate edit links and navigation links with type segment in it.
            // Hence we need to be able to process type segment in the request even when the server MPV is set to V1/V2. But we do not want to expose new functionality
            // like filtering collections based on type, etc on V1/V2 servers. Hence only checking for MPV to be v3 or greater if the previous segment is a collection
            if (!previous.SingleResult)
            {
                VersionUtil.CheckMaxProtocolVersion(VersionUtil.Version4Dot0, this.maxProtocolVersion);
            }

            typeNameSegment = new SegmentInfo
            {
                Identifier = targetResourceType.FullName,
                Operation = previous.Operation,
                TargetKind = previous.TargetKind,
                TargetSource = previous.TargetSource,
                TargetResourceType = targetResourceType,
                SingleResult = previous.SingleResult,
                TargetResourceSet = previous.TargetResourceSet,
                ProjectedProperty = previous.ProjectedProperty,
                Key = previous.Key,
                RequestExpression = previous.RequestExpression,
                RequestEnumerable = previous.RequestEnumerable,
                IsTypeIdentifierSegment = true
            };

            return true;
        }

        /// <summary>
        /// Creates a property segment
        /// </summary>
        /// <param name="previous">previous segment info.</param>
        /// <param name="property">property to create the segment for.</param>
        /// <returns>new segment for the given property.</returns>
        private SegmentInfo CreatePropertySegment(SegmentInfo previous, ResourceProperty property)
        {
            // Handle a strongly-typed property.
            SegmentInfo segment = new SegmentInfo() { Identifier = property.Name, ProjectedProperty = property };
            segment.TargetResourceType = property.ResourceType;
            ResourcePropertyKind propertyKind = property.Kind;
            segment.SingleResult = (propertyKind != ResourcePropertyKind.ResourceSetReference);
            segment.TargetSource = RequestTargetSource.Property;

            if (previous.TargetKind == RequestTargetKind.Link && property.TypeKind != ResourceTypeKind.EntityType)
            {
                throw DataServiceException.CreateBadRequestError(Strings.RequestUriProcessor_LinkSegmentMustBeFollowedByEntitySegment(segment.Identifier, XmlConstants.UriLinkSegment));
            }

            switch (propertyKind)
            {
                case ResourcePropertyKind.ComplexType:
                    segment.TargetKind = RequestTargetKind.ComplexObject;
                    break;
                case ResourcePropertyKind.Collection:
                    segment.TargetKind = RequestTargetKind.Collection;
                    break;
                case ResourcePropertyKind.ResourceReference:
                case ResourcePropertyKind.ResourceSetReference:
                    segment.TargetKind = RequestTargetKind.Resource;
                    segment.TargetResourceSet = this.providerWrapper.GetResourceSet(previous.TargetResourceSet, previous.TargetResourceType, property);
                    if (segment.TargetResourceSet == null)
                    {
                        throw DataServiceException.CreateResourceNotFound(property.Name);
                    }

                    break;
                default:
                    Debug.Assert(property.IsOfKind(ResourcePropertyKind.Primitive), "must be primitive type property");
                    segment.TargetKind = RequestTargetKind.Primitive;
                    break;
            }

            return segment;
        }
    }
}