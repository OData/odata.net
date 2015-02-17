//---------------------------------------------------------------------
// <copyright file="ServiceOperationDescriptorVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.ResponseVerification
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ResponseVerification;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// A class for response action/function verification.
    /// </summary>
    public class ServiceOperationDescriptorVerifier : ResponseVerifierBase, ISelectiveResponseVerifier
    {
        private EntityModelSchema model;

        /// <summary>
        /// Initializes a new instance of the ServiceOperationDescriptorVerifier class.
        /// </summary>
        /// <param name="model">The conceptual model of the service</param>
        public ServiceOperationDescriptorVerifier(EntityModelSchema model)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            this.model = model;
        }

        /// <summary>
        /// Returns true if this verifier applies to responses for this kind of request
        /// </summary>
        /// <param name="request">The request that was issued</param>
        /// <returns>Whether or not this verifier applies to responses for this kind of request</returns>
        public bool Applies(ODataRequest request)
        {
            // Do not need to verify action/function links in metadata response.
            return !request.Uri.IsMetadata();
        }

        /// <summary>
        /// Return true if this verifier applies to the response
        /// </summary>
        /// <param name="response">The response that might need to be verified</param>
        /// <returns>Whether or not this verifier applies to the response</returns>
        public bool Applies(ODataResponse response)
        {
            return response.StatusCode != HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Verifies that the actions/functions in the response are correct.
        /// </summary>
        /// <param name="request">The request that was sent</param>
        /// <param name="response">The response to verify</param>
        public override void Verify(ODataRequest request, ODataResponse response)
        {
            base.Verify(request, response);
            this.VerifyPayloadElementOperations(response.RootElement, request, response, true);
        }

        /// <summary>
        /// Checks for a name collision between an action's name and a property's name
        /// </summary>
        /// <param name="function">The function to check for name property collision</param>
        /// <param name="bindingEntityDataType">The bindingEntityDataType is the entity to check against</param>
        /// <param name="actualInstanceEntityType">the entity type of the instance of the entity</param>
        /// <returns>a string representing the entity container's name</returns>
        public string BuildExpectedContainerName(Function function, EntityDataType bindingEntityDataType, EntityType actualInstanceEntityType)
        {
            string containerName = string.Empty;

            // check for open entity type on the function definition or on the actual entity itself
            if (bindingEntityDataType.Definition.IsOpen || actualInstanceEntityType.IsOpen)
            {
                containerName = string.Concat(function.Model.GetDefaultEntityContainer().Name, ".");
            }

            var foundNameCollision = bindingEntityDataType.Definition.AllProperties.Where(a => a.Name.Equals(function.Name)).FirstOrDefault();
            if (foundNameCollision != null && !bindingEntityDataType.Definition.IsOpen)
            {
                containerName = string.Concat(function.Model.GetDefaultEntityContainer().Name, ".");
            }

            return containerName;
        }

        /// <summary>
        /// Checks if the expected action descriptor in the case where the entity type is open and the property is open
        /// </summary>
        /// <param name="function">The function to check for name property collision</param>
        /// <param name="bindingEntityDataType">The bindingEntityDataType is the entity to check against</param>
        /// <param name="expectActionDescriptor">current value of the expected behavior</param>
        /// <returns>true is the action descriptor is expected in response payload</returns>
        public bool VerifyIfOpenType(Function function, EntityDataType bindingEntityDataType, bool expectActionDescriptor)
        {
            if ((expectActionDescriptor == false) && bindingEntityDataType.Definition.IsOpen)
            {
                // in open types, an action with a name collision will be returned in metadata
                var possibleNameCollision = bindingEntityDataType.Definition.AllProperties.Where(a => a.Name.Equals(function.Name)).FirstOrDefault();
                if (possibleNameCollision != null)
                {
                    // if property is not declared, then action descriptor will be present
                    expectActionDescriptor = !possibleNameCollision.IsMetadataDeclaredProperty();
                }
            }

            return expectActionDescriptor;
        }

        private static string ExtractSimpleActionName(string fullName)
        {
            string actionName = string.Empty;
            foreach (var actionChar in fullName)
            {
                if (actionChar == '.')
                {
                    actionName = string.Empty;
                }
                else
                {
                    actionName = actionName + actionChar;
                }
            }

            return actionName;
        }

        private static bool IsJsonLightResponse(string acceptHeaderValue)
        {
            return acceptHeaderValue.Contains(MimeTypes.ApplicationJson) && !acceptHeaderValue.Contains("verbose");
        }

        /// <summary>
        /// Verifies that any action/function in the payload element are correct.
        /// </summary>
        /// <param name="payloadElement">The payload element to verify</param>
        /// <param name="request">The request that was sent</param>
        /// <param name="response">The response to verify</param>
        /// <param name="isTopLevelElement">Whether the payloadElement is the top level of response</param>
        private void VerifyPayloadElementOperations(ODataPayloadElement payloadElement, ODataRequest request, ODataResponse response, bool isTopLevelElement)
        {
            // find all the entities and verify action/functions for the entities.
            List<EntityInstance> entities = new List<EntityInstance>();
            if (payloadElement.ElementType == ODataPayloadElementType.EntityInstance)
            {
                entities.Add(payloadElement as EntityInstance);
            }
            else if (payloadElement.ElementType == ODataPayloadElementType.EntitySetInstance)
            {
                foreach (EntityInstance ei in payloadElement as EntitySetInstance)
                {
                    entities.Add(ei as EntityInstance);
                }
            }

            this.VerifyEntityOperations(request, response, entities, isTopLevelElement);
        }

        /// <summary>
        /// Verifies that any action/function in the entities are correct.
        /// </summary>
        /// <param name="request">The request that was sent</param>
        /// <param name="response">The response to verify</param>
        /// <param name="entities">List of entities to verify actions/functions</param>
        /// <param name="isTopLevelElement">Whether the payloadElement is the top level of response</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Temporarily suppressing this message. Have opened bug against the test owner to fix this.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Temporarily suppressing this message.")]
        private void VerifyEntityOperations(ODataRequest request, ODataResponse response, List<EntityInstance> entities, bool isTopLevelElement)
        {
            foreach (var entity in entities.Where(et => !et.IsNull))
            {
                var instanceEntityType = this.model.EntityTypes.Single(et => et.FullName == entity.FullTypeName);

                // look at each action/function bound to the same type as entity, ensure it is advertised correctly
                int numberOfDescriptorsVerified = 0;
                var functionsWithBindings = this.model.Functions.Where(f => f.Annotations.OfType<ServiceOperationAnnotation>().Any(s => s.BindingKind.IsBound())).ToArray();
                foreach (Function function in functionsWithBindings)
                {
                    ServiceOperationAnnotation serviceOperationAnnotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
                    if (serviceOperationAnnotation != null)
                    {
                        DataType bindingParameterDataType = function.Parameters[0].DataType;
                        EntityDataType bindingEntityDataType = bindingParameterDataType as EntityDataType;
                        if (bindingEntityDataType == null)
                        {
                            ExceptionUtilities.Assert(bindingParameterDataType is CollectionDataType, "Unsupported binding parameter data type.");
                            ExceptionUtilities.Assert(entity.ServiceOperationDescriptors.SingleOrDefault(d => d.Title == function.Name) == null, "Unexpected feed-bound action advertised in entry.");
                            continue;
                        }

                        // Check the base type as well as derived types
                        bool beginServiceOpMatchMatching = bindingEntityDataType.Definition.Model.EntityTypes.Where(t => t.IsKindOf(bindingEntityDataType.Definition)).Where(a => a.FullName.Equals(entity.FullTypeName)).Any();

                        if (beginServiceOpMatchMatching)
                        {
                            // find ServiceOperationDescriptor that matches the function
                            ServiceOperationDescriptor descriptor = entity.ServiceOperationDescriptors.SingleOrDefault(d => ExtractSimpleActionName(d.Title) == function.Name);
                            bool expectActionDescriptor = true;
                            if (request.Uri.SelectSegments.Count > 0)
                            {
                                expectActionDescriptor = this.ExpectActionWithProjection(request.Uri, function, isTopLevelElement);
                            }

                            if (descriptor == null)
                            {
                                ExceptionUtilities.Assert(!expectActionDescriptor, "Missing service operation descriptor for " + function.Name);
                            }
                            else
                            {
                                expectActionDescriptor = this.VerifyIfOpenType(function, bindingEntityDataType, expectActionDescriptor);

                                ExceptionUtilities.Assert(expectActionDescriptor, "Unexpected service operation descriptor for " + function.Name);

                                // JSONLight will always add the type segment if its a derived type now
                                string derivedTypeFullName = string.Empty;
                                var acceptHeaderValue = response.Headers[HttpHeaders.ContentType];
                                if (bindingEntityDataType.Definition.BaseType != null)
                                {
                                    derivedTypeFullName = string.Concat(bindingEntityDataType.Definition.FullName, "/");
                                }
                              
                                if (bindingEntityDataType.Definition.FullName != instanceEntityType.FullName)
                                {
                                    derivedTypeFullName = string.Concat(instanceEntityType.FullName, "/");
                                }

                                // check if there is a possible name collision between a property and an action, if so add the Entity Container name to the expected result
                                string containerName = this.BuildExpectedContainerName(function, bindingEntityDataType, instanceEntityType);
                                string expectedMetadataContainerName = string.Concat(function.Model.GetDefaultEntityContainer().Name, ".");

                                // When running in JSONLight descriptor returns full name, not partial
                                if (IsJsonLightResponse(acceptHeaderValue))
                                {
                                    containerName = string.Concat(function.Model.GetDefaultEntityContainer().Name, ".");

                                    expectedMetadataContainerName = containerName;
                                }
                                
                                string expectedTarget = string.Concat(entity.Id.TrimEnd('/'), "/", derivedTypeFullName, containerName, function.Name);
                                string expectedMetadata = string.Concat(((ServiceRootSegment)request.Uri.RootSegment).Uri.AbsoluteUri.TrimEnd('/'), "/", Endpoints.Metadata, "#", expectedMetadataContainerName, function.Name);

                                this.Assert(descriptor.Target == expectedTarget, "Expected target " + expectedTarget + " Actual " + descriptor.Target, request, response);
                                this.Assert(descriptor.Metadata == expectedMetadata, "Expected Metadata " + expectedMetadata + " Actual " + descriptor.Metadata, request, response);

                                // verify IsAction flag
                                this.Assert(serviceOperationAnnotation.IsAction == descriptor.IsAction, "Expected action " + serviceOperationAnnotation.IsAction + " Actual " + descriptor.IsAction, request, response);
                                numberOfDescriptorsVerified++;
                            }
                        }
                    }
                }

                this.Assert(numberOfDescriptorsVerified == entity.ServiceOperationDescriptors.Count, "Wrong number of action/function.", request, response);
                this.VerifyExpandedEntityOperations(entity, request, response);
            }
        }

        /// <summary>
        /// Verifies actions/functions in the expanded entity.
        /// </summary>
        /// <param name="entity">The entity to expand</param>
        /// <param name="request">The request that was sent</param>
        /// <param name="response">The response to verify</param>
        private void VerifyExpandedEntityOperations(EntityInstance entity, ODataRequest request, ODataResponse response)
        {
            foreach (var navigation in entity.Properties.OfType<NavigationPropertyInstance>())
            {
                // verify expanded entities
                if (navigation.Value.ElementType == ODataPayloadElementType.ExpandedLink)
                {
                    ODataPayloadElement expandedElement = ((ExpandedLink)navigation.Value).ExpandedElement;
                    if (expandedElement != null)
                    {
                        this.VerifyPayloadElementOperations(expandedElement, request, response, false);
                    }
                }
            }
        }

        /// <summary>
        /// Find out whether to expect action descriptor with projection in request uri
        /// </summary>
        /// <param name="requestUri">The request uri</param>
        /// <param name="action">The action</param>
        /// <param name="isTopLevelElement">Whether the entity being verified is top level payload element</param>
        /// <returns>Whether to expect action descriptor</returns>
        private bool ExpectActionWithProjection(ODataUri requestUri, Function action, bool isTopLevelElement)
        {
            ODataUriSegmentPathCollection selectSegments = requestUri.SelectSegments;
            ODataUriSegmentPathCollection expandSegments = requestUri.ExpandSegments;

            // handle single level $select path, expect action descriptor if $select=ActionName or $select=Container.*
            foreach (var selectSegmentPath in selectSegments.Where(ss => ss.Count == 1))
            {
                ODataUriSegment selectSegment = selectSegmentPath.Single();

                if (isTopLevelElement && this.FuctionMatchWithSelectFunctionSegment(action, selectSegment))
                {
                    return true;
                }

                if (this.IsSelectAllFunctionSegment(selectSegment))
                {
                    return true;
                }
            }

            // handle multiple level $select path, expect action descriptor for $expand=Rating if: $select=Rating or $select=Rating/ActionName or $Select=Rating/Container.*
            foreach (var expandSegmentPath in expandSegments)
            {
                List<ODataUriSegment> expandSegmentList = expandSegmentPath.ToList();
                foreach (var selectSegmentPath in selectSegments.Where(ss => ss.Count == expandSegmentPath.Count || ss.Count == expandSegmentPath.Count + 1))
                {
                    List<ODataUriSegment> selectSegmentList = selectSegmentPath.ToList();
                    if (this.FunctionMatchWithExpandSegmentList(selectSegmentList, expandSegmentList, action))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Find out whether to expect action descriptor given expand and select segments.
        /// </summary>
        /// <param name="selectSegmentList">The select segments</param>
        /// <param name="expandSegmentList">The expand segments</param>
        /// <param name="action">The action</param>
        /// <returns>Whether to expect action descriptor</returns>
        private bool FunctionMatchWithExpandSegmentList(List<ODataUriSegment> selectSegmentList, List<ODataUriSegment> expandSegmentList, Function action)
        {
            // check if $select path and $expand path matchs
            int index = 0;
            NavigationSegment expandNavSegment = null;
            foreach (var expandSegment in expandSegmentList)
            {
                expandNavSegment = expandSegment as NavigationSegment;
                ExceptionUtilities.CheckArgumentNotNull(expandNavSegment, "Unexpected expand segment.");
                NavigationSegment selectNavSegment = selectSegmentList[index] as NavigationSegment;
                if (selectNavSegment == null || !expandNavSegment.NavigationProperty.Equals(selectNavSegment.NavigationProperty))
                {
                    return false;
                }

                index++;
            }

            // check if the action binding type matches with last segment of $expand path
            EntityDataType bindingEntityDataType = action.Parameters.First().DataType as EntityDataType;
            ExceptionUtilities.CheckArgumentNotNull(bindingEntityDataType, "Unexpected feed-bound action.");
            EntityType bindingEntityType = bindingEntityDataType.Definition;
            if (bindingEntityType != expandNavSegment.NavigationProperty.ToAssociationEnd.EntityType)
            {
                return false;
            }

            // expect action descriptor (return true) for $expand=Rating if: $select=Rating, $select=Rating/ActionName, $Select=Rating/Container.*
            if (selectSegmentList.Count == expandSegmentList.Count + 1)
            {
                ODataUriSegment lastSelectSegment = selectSegmentList.Last();
                return this.FuctionMatchWithSelectFunctionSegment(action, lastSelectSegment) || this.IsSelectAllFunctionSegment(lastSelectSegment);
            }

            return true;
        }

        /// <summary>
        /// Find out whether action matches with the ODataUri select function segment
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="selectSegment">The segment</param>
        /// <returns>Whether action matches with the function segment</returns>
        private bool FuctionMatchWithSelectFunctionSegment(Function action, ODataUriSegment selectSegment)
        {
            FunctionSegment selectFunctionSegment = selectSegment as FunctionSegment;
            if (selectFunctionSegment != null && selectFunctionSegment.Function == action)
            {
                return true;
            }

            // Sometimes the $select segment is an UnrecognizedSegment
            UnrecognizedSegment otherTypeOfSelectSegment = selectSegment as UnrecognizedSegment;
            if (otherTypeOfSelectSegment != null && selectFunctionSegment == null)
            {
                string actionName = action.Name;

                // Determine the if selction segment was <Action Name> or <EntityContainer>.<Action Name>
                if (otherTypeOfSelectSegment.Value.Contains('.'))
                {
                    actionName = string.Concat(action.Model.GetDefaultEntityContainer().Name, ".", actionName);    
                }

                if (otherTypeOfSelectSegment.Value.Equals(actionName, System.StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find out whehter the given segment is select-all-function-segment (ContainerName.*)
        /// </summary>
        /// <param name="selectSegment">The segment</param>
        /// <returns>Whether segment is select-all-function-segment</returns>
        private bool IsSelectAllFunctionSegment(ODataUriSegment selectSegment)
        {
            UnrecognizedSegment selectAllFunctionSegment = selectSegment as UnrecognizedSegment;
            if (selectAllFunctionSegment != null && selectAllFunctionSegment.Value == this.model.GetDefaultEntityContainer().Name + ".*")
            {
                return true;
            }

            return false;
        }
    }
}
