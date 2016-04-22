//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementReplacingVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base implementation of a replacing visitor for OData payloads
    /// </summary>
    public abstract class ODataPayloadElementReplacingVisitor : IODataPayloadElementVisitor<ODataPayloadElement>
    {
        /// <summary>
        /// Setting this to true Always replaces regardless of whether anything changed or not
        /// </summary>
        private bool alwaysReplace = false;

        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementReplacingVisitor class
        /// </summary>
        protected ODataPayloadElementReplacingVisitor() :
            this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementReplacingVisitor class with an option to always replace regardless of whether anything changed or not
        /// </summary>
        /// <param name="alwaysReplace">Setting this to true Always replaces regardless of whether anything changed or not</param>
        protected ODataPayloadElementReplacingVisitor(bool alwaysReplace)
        {
            this.VisitCallback = e => e.Accept(this);
            this.alwaysReplace = alwaysReplace;
        }

        /// <summary>
        /// Gets or sets the callback to use when visiting elements. Unit test hook only.
        /// </summary>
        internal Func<ODataPayloadElement, ODataPayloadElement> VisitCallback { get; set; }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(BatchRequestPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            return this.VisitBatch<BatchRequestPayload, BatchRequestChangeset, IHttpRequest>(
                payloadElement,
                this.VisitRequestOperation);
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(BatchResponsePayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            return this.VisitBatch<BatchResponsePayload, BatchResponseChangeset, HttpResponseData>(
                payloadElement,
                this.VisitResponseOperation);
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var replacedProperties = this.VisitCollection(payloadElement.Properties);

            if (!this.ShouldReplace(replacedProperties, payloadElement.Properties))
            {
                return payloadElement;
            }

            var replacedInstance = payloadElement.ReplaceWith(new ComplexInstance(payloadElement.FullTypeName, payloadElement.IsNull));
            foreach (var property in replacedProperties)
            {
                replacedInstance.Add(property);
            }

            return replacedInstance;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ComplexInstanceCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = this.VisitCollection(payloadElement);
            if (!this.ShouldReplace(replaced, payloadElement))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new ComplexInstanceCollection(replaced.ToArray()));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ComplexMultiValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = this.VisitCollection(payloadElement);
            if (!this.ShouldReplace(replaced, payloadElement))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new ComplexMultiValue(payloadElement.FullTypeName, payloadElement.IsNull, replaced.ToArray()));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ComplexMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replacedCollection = this.Recurse(payloadElement.Value) as ComplexMultiValue;
            ExceptionUtilities.CheckObjectNotNull(replacedCollection, "Replaced complex collection was null or wrong type");

            if (!this.ShouldReplace(payloadElement.Value, replacedCollection))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new ComplexMultiValueProperty(payloadElement.Name, replacedCollection));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ComplexProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replacedValue = this.Recurse(payloadElement.Value) as ComplexInstance;
            ExceptionUtilities.CheckObjectNotNull(replacedValue, "Replaced value was null or wrong type");
            if (!this.ShouldReplace(payloadElement.Value, replacedValue))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new ComplexProperty(payloadElement.Name, replacedValue));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(DeferredLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith<DeferredLink>(new DeferredLink() { UriString = payloadElement.UriString });
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(EmptyCollectionProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replacedValue = this.Recurse(payloadElement.Value) as EmptyUntypedCollection;
            ExceptionUtilities.CheckObjectNotNull(replacedValue, "Replaced value was null or wrong type");
            if (!this.ShouldReplace(payloadElement.Value, replacedValue))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new EmptyCollectionProperty(payloadElement.Name, replacedValue));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(EmptyPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith(new EmptyPayload());
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(EmptyUntypedCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith(new EmptyUntypedCollection());
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var replacedProperties = this.VisitCollection(payloadElement.Properties);
            var replacedServiceOperationDescriptors = this.VisitCollection(payloadElement.ServiceOperationDescriptors);

            bool shouldNotReplaceProperties = !this.ShouldReplace(replacedProperties, payloadElement.Properties);
            bool shouldNotReplaceServiceOperationDescriptors = !this.ShouldReplace(replacedServiceOperationDescriptors, payloadElement.ServiceOperationDescriptors);

            if (shouldNotReplaceProperties && shouldNotReplaceServiceOperationDescriptors)
            {
                return payloadElement;
            }

            var replacedInstance = payloadElement.ReplaceWith(
                new EntityInstance(payloadElement.FullTypeName, payloadElement.IsNull, payloadElement.Id, payloadElement.ETag));
            replacedInstance.StreamContentType = payloadElement.StreamContentType;
            replacedInstance.StreamEditLink = payloadElement.StreamEditLink;
            replacedInstance.StreamETag = payloadElement.StreamETag;
            replacedInstance.StreamSourceLink = payloadElement.StreamSourceLink;
            foreach (var property in replacedProperties)
            {
                replacedInstance.Add(property);
            }

            foreach (var replacedServiceOperationDescriptor in replacedServiceOperationDescriptors)
            {
                replacedInstance.Add(replacedServiceOperationDescriptor);
            }

            replacedInstance.EditLink = payloadElement.EditLink;
            replacedInstance.IsComplex = payloadElement.IsComplex;
            return replacedInstance;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = this.VisitCollection(payloadElement);
            if (!this.ShouldReplace(replaced, payloadElement))
            {
                return payloadElement;
            }

            var replacedCollection = payloadElement.ReplaceWith(new EntitySetInstance(replaced.ToArray()));
            replacedCollection.InlineCount = payloadElement.InlineCount;
            replacedCollection.NextLink = payloadElement.NextLink;
            return replacedCollection;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ExpandedLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ODataPayloadElement replacedExpandedElement = null;
            if (payloadElement.ExpandedElement != null)
            {
                replacedExpandedElement = this.Recurse(payloadElement.ExpandedElement);
            }

            if (!this.ShouldReplace(payloadElement.ExpandedElement, replacedExpandedElement))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new ExpandedLink(replacedExpandedElement) { UriString = payloadElement.UriString });
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(LinkCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var links = this.VisitCollection(payloadElement);
            if (!this.ShouldReplace(links, payloadElement))
            {
                return payloadElement;
            }

            var replacement = payloadElement.ReplaceWith(new LinkCollection(links.ToArray()));
            replacement.InlineCount = payloadElement.InlineCount;
            replacement.NextLink = payloadElement.NextLink;
            return replacement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(NamedStreamInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith<NamedStreamInstance>(new NamedStreamInstance()
                {
                    Name = payloadElement.Name,
                    EditLink = payloadElement.EditLink,
                    EditLinkContentType = payloadElement.EditLinkContentType,
                    ETag = payloadElement.ETag,
                    SourceLink = payloadElement.SourceLink,
                    SourceLinkContentType = payloadElement.SourceLinkContentType
                });
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(NavigationPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            ODataPayloadElement replacedValue = null;
            if (payloadElement.Value != null)
            {
                replacedValue = this.Recurse(payloadElement.Value);
            }

            DeferredLink replacedAssociationLink = null;
            if (payloadElement.AssociationLink != null)
            {
                replacedAssociationLink = (DeferredLink)this.Recurse(payloadElement.AssociationLink);
            }

            if (!this.ShouldReplace(payloadElement.Value, replacedValue) && !this.ShouldReplace(payloadElement.AssociationLink, replacedAssociationLink))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new NavigationPropertyInstance(payloadElement.Name, replacedValue, replacedAssociationLink));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(NullPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith(new NullPropertyInstance(payloadElement.Name, payloadElement.FullTypeName));
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ODataErrorPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var innerError = payloadElement.InnerError;
            if (innerError != null)
            {
                // Note: we specifically allow it to be replaced with null
                innerError = this.Recurse(innerError) as ODataInternalExceptionPayload;
            }

            if (this.ShouldReplace(payloadElement.InnerError, innerError))
            {
                return payloadElement.ReplaceWith(
                    new ODataErrorPayload()
                    {
                        Message = payloadElement.Message,
                        Code = payloadElement.Code,
                        InnerError = innerError,
                    });
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ODataInternalExceptionPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var innerError = payloadElement.InternalException;
            if (innerError != null)
            {
                // Note: we specifically allow it to be replaced with null
                innerError = this.Recurse(innerError) as ODataInternalExceptionPayload;
            }

            if (this.ShouldReplace(payloadElement.InternalException, innerError))
            {
                return payloadElement.ReplaceWith(
                    new ODataInternalExceptionPayload()
                    {
                        Message = payloadElement.Message,
                        StackTrace = payloadElement.StackTrace,
                        TypeName = payloadElement.TypeName,
                        InternalException = innerError,
                    });
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(PrimitiveCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = this.VisitCollection(payloadElement);
            if (!this.ShouldReplace(replaced, payloadElement))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new PrimitiveCollection(replaced.ToArray()));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(PrimitiveMultiValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replaced = this.VisitCollection(payloadElement);
            if (!this.ShouldReplace(replaced, payloadElement))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new PrimitiveMultiValue(payloadElement.FullTypeName, payloadElement.IsNull, replaced.ToArray()));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(PrimitiveMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replacedCollection = this.Recurse(payloadElement.Value) as PrimitiveMultiValue;
            ExceptionUtilities.CheckObjectNotNull(replacedCollection, "Replaced complex collection was null or wrong type");

            if (!this.ShouldReplace(payloadElement.Value, replacedCollection))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new PrimitiveMultiValueProperty(payloadElement.Name, replacedCollection));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(PrimitiveProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            var replacedValue = this.Recurse(payloadElement.Value) as PrimitiveValue;
            ExceptionUtilities.CheckObjectNotNull(replacedValue, "Replaced value was null or wrong type");
            if (!this.ShouldReplace(payloadElement.Value, replacedValue))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new PrimitiveProperty(payloadElement.Name, replacedValue));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="payloadElement">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith<PrimitiveValue>(new PrimitiveValue(payloadElement.FullTypeName, payloadElement.ClrValue));
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits a payload element whose root is a ResourceCollectionInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ResourceCollectionInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (this.alwaysReplace)
            {
                return payloadElement.ReplaceWith(new ResourceCollectionInstance { Title = payloadElement.Title, Href = payloadElement.Href, Name = payloadElement.Name });
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits a payload element whose root is a ServiceDocumentInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ServiceDocumentInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            IList<WorkspaceInstance> replacedWorkspaces = null;
            if (payloadElement.Workspaces != null)
            {
                replacedWorkspaces = new List<WorkspaceInstance>();

                foreach (WorkspaceInstance workspace in payloadElement.Workspaces)
                {
                    ExceptionUtilities.CheckObjectNotNull(workspace, "Workspace is null");
                    WorkspaceInstance replacedWorkspace = this.Visit(workspace) as WorkspaceInstance;
                    ExceptionUtilities.CheckObjectNotNull(replacedWorkspace, "Replaced workspace was null or wrong type");
                    replacedWorkspaces.Add(replacedWorkspace);
                }
            }

            if (!this.ShouldReplace(payloadElement.Workspaces, replacedWorkspaces))
            {
                return payloadElement;
            }

            return payloadElement.ReplaceWith(new ServiceDocumentInstance(replacedWorkspaces.ToArray()));
        }

        /// <summary>
        /// Visits the children of the given payload element and replaces it with a copy if any child changes
        /// </summary>
        /// <param name="serviceOperationDescriptor">The payload element to potentially replace</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(ServiceOperationDescriptor serviceOperationDescriptor)
        {
            ExceptionUtilities.CheckArgumentNotNull(serviceOperationDescriptor, "serviceOperationDescriptor");

            if (this.alwaysReplace)
            {
                return serviceOperationDescriptor.ReplaceWith<ServiceOperationDescriptor>(
                    new ServiceOperationDescriptor()
                    {
                        IsAction = serviceOperationDescriptor.IsAction,
                        Metadata = serviceOperationDescriptor.Metadata,
                        Target = serviceOperationDescriptor.Target,
                        Title = serviceOperationDescriptor.Title
                    });
            }

            return serviceOperationDescriptor;
        }

        /// <summary>
        /// Visits a payload element whose root is a WorkspaceInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The original element or a copy to replace it with</returns>
        public virtual ODataPayloadElement Visit(WorkspaceInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            IList<ResourceCollectionInstance> replacedCollections = null;
            if (payloadElement.ResourceCollections != null)
            {
                replacedCollections = new List<ResourceCollectionInstance>();

                foreach (ResourceCollectionInstance collection in payloadElement.ResourceCollections)
                {
                    ExceptionUtilities.CheckObjectNotNull(collection, "Resource collection is null");
                    ResourceCollectionInstance replacedCollection = this.Visit(collection) as ResourceCollectionInstance;
                    ExceptionUtilities.CheckObjectNotNull(replacedCollection, "Replaced resource collection  was null or wrong type");
                    replacedCollections.Add(replacedCollection);
                }
            }

            if (!this.ShouldReplace(payloadElement.ResourceCollections, replacedCollections))
            {
                return payloadElement;
            }

            WorkspaceInstance replacedWorkspace = new WorkspaceInstance(replacedCollections.ToArray());
            replacedWorkspace.Title = payloadElement.Title;

            return payloadElement.ReplaceWith(replacedWorkspace);
        }

        /// <summary>
        /// Wrapper for recursively visiting the given element. Used with the callback property to make unit tests easier.
        /// </summary>
        /// <param name="element">The element to visit</param>
        /// <returns>The result of visiting the element</returns>
        protected virtual ODataPayloadElement Recurse(ODataPayloadElement element)
        {
            ExceptionUtilities.CheckArgumentNotNull(element, "element");
            ExceptionUtilities.CheckObjectNotNull(this.VisitCallback, "Visit callback unexpectedly null");
            return this.VisitCallback(element);
        }

        /// <summary>
        /// Determines if the original payloadElement should be replaced with a new one.
        /// </summary>
        /// <param name="original">original payloadElement</param>
        /// <param name="replacement">replacement payloadElement</param>
        /// <returns>true or false indicating whether original should be replaced</returns>
        protected bool ShouldReplace(object original, object replacement)
        {
            return this.alwaysReplace || !object.ReferenceEquals(original, replacement);
        }

        /// <summary>
        /// Visits a batch payload
        /// </summary>
        /// <typeparam name="TBatch">The type of batch payload</typeparam>
        /// <typeparam name="TChangeset">The type of batch changeset</typeparam>
        /// <typeparam name="TOperation">The type of batch operation</typeparam>
        /// <param name="batchPayload">The batch payload to visit</param>
        /// <param name="visitOperation">The function for visiting the operation</param>
        /// <returns>Returns visited batch</returns>
        protected virtual TBatch VisitBatch<TBatch, TChangeset, TOperation>(TBatch batchPayload, Func<TOperation, TOperation> visitOperation)
            where TBatch : BatchPayload<TOperation, TChangeset>, new()
            where TChangeset : BatchChangeset<TOperation>, new()
            where TOperation : class, IMimePart
        {
            var replaced = new TBatch();
            bool batchChanged = false;
            foreach (var part in batchPayload.Parts)
            {
                var changeset = part as TChangeset;
                if (changeset != null)
                {
                    var replacedChangeset = this.VisitChangeset<TChangeset, TOperation>(changeset, visitOperation);
                    ExceptionUtilities.CheckObjectNotNull(replacedChangeset, "VisitChangeset should have returned a BatchChangeset<>");
                    if (changeset != replacedChangeset)
                    {
                        batchChanged = true;
                    }

                    replaced.Add(replacedChangeset);
                }
                else
                {
                    var op = (MimePartData<TOperation>)part;
                    var operation = visitOperation(op.Body);
                    ExceptionUtilities.CheckObjectNotNull(operation, "All parts must be changesets or operations");
                    if (op.Body != operation)
                    {
                        batchChanged = true;
                    }

                    replaced.Add(this.AsBatchFragment(operation));
                }
            }

            foreach (var annotation in batchPayload.Annotations)
            {
                replaced.Add(annotation.Clone());
            }

            if (this.alwaysReplace || batchChanged)
            {
                return replaced;
            }

            return batchPayload;
        }

        /// <summary>
        /// Visits the changeset
        /// </summary>
        /// <typeparam name="TChangeset">The type of batch changeset</typeparam>
        /// <typeparam name="TOperation">The type of operation in the batch changeset</typeparam>
        /// <param name="changeset">The changeset to visit</param>
        /// <param name="visitOperation">The function for visiting the operation</param>
        /// <returns>The visited changeset</returns>
        protected virtual TChangeset VisitChangeset<TChangeset, TOperation>(TChangeset changeset, Func<TOperation, TOperation> visitOperation)
            where TOperation : class, IMimePart
            where TChangeset : BatchChangeset<TOperation>, new()
        {
            var operations = new List<TOperation>();
            bool operationsChanged = false;
            foreach (var operation in changeset.Operations)
            {
                var result = visitOperation(operation);
                if (operation != result)
                {
                    operationsChanged = true;
                }

                operations.Add(visitOperation(operation));
            }

            if (this.alwaysReplace || operationsChanged)
            {
                TChangeset replaced = new TChangeset();
                foreach (var header in changeset.Headers)
                {
                    replaced.Headers.Add(header);
                }

                foreach (var operation in operations)
                {
                    replaced.Add(this.AsBatchFragment(operation));
                }

                return replaced;
            }

            return changeset;
        }

        /// <summary>
        /// Replaces operation if payload is updated
        /// </summary>
        /// <param name="operation">Operation to visit</param>
        /// <returns>Original or replaced operation</returns>
        protected virtual HttpResponseData VisitResponseOperation(HttpResponseData operation)
        {
            ExceptionUtilities.CheckArgumentNotNull(operation, "operation");
            ODataPayloadElement newRootElement = null;
            var responseOperation = operation as ODataResponse;
            ExceptionUtilities.CheckObjectNotNull(responseOperation, "ODataRequest was expected");

            var newResponse = new ODataResponse(operation);
            if (responseOperation.RootElement != null)
            {
                newRootElement = this.Recurse(responseOperation.RootElement);
            }
            
            if (this.ShouldReplace(newRootElement, responseOperation.RootElement))
            {
                newResponse.RootElement = newRootElement;    
                return newResponse;
            }

            return operation;
        }

        /// <summary>
        /// Replaces operation if payload is updated
        /// </summary>
        /// <param name="operation">Operation to visit</param>
        /// <returns>Original or replaced operation</returns>
        protected virtual IHttpRequest VisitRequestOperation(IHttpRequest operation)
        {
            ExceptionUtilities.CheckArgumentNotNull(operation, "operation");
            ODataPayloadElement newRootElement = null;
            var requestOperation = operation as ODataRequest;
            ExceptionUtilities.CheckObjectNotNull(requestOperation, "ODataRequest was expected");
            if (requestOperation.Body != null && requestOperation.Body.RootElement != null)
            {
                newRootElement = this.Recurse(requestOperation.Body.RootElement);
            }

            if (this.ShouldReplace(newRootElement, (requestOperation.Body == null) ? null : requestOperation.Body.RootElement))
            {
                var newRequest = requestOperation.RequestManager.BuildRequest(requestOperation.Uri, operation.Verb, operation.Headers);
                if (requestOperation.Body != null)
                {
                    string contentType = null;
                    if (operation.Headers.TryGetValue(HttpHeaders.ContentType, out contentType))
                    {
                        newRequest.Body = requestOperation.RequestManager.BuildBody(contentType, requestOperation.Uri, newRootElement);
                    }
                    else if (requestOperation.Body != null)
                    {
                        newRequest.Body = new ODataPayloadBody(new byte[0], newRootElement);
                    }
                }
                
                return newRequest;
            }

            return operation;
        }

        /// <summary>
        /// Determines if the original payloadElement should be replaced with a new one.
        /// </summary>
        /// <param name="payloadElement">replacement payloadElement</param>
        /// <typeparam name="TElement">type of payload to visit</typeparam>
        /// <returns>true or false indicating whether original should be replaced</returns>
        protected IEnumerable<TElement> VisitCollection<TElement>(IEnumerable<TElement> payloadElement)
            where TElement : ODataPayloadElement
        {
            bool unchanged = true;
            List<TElement> elements = new List<TElement>();
            foreach (var element in payloadElement)
            {
                var replacedElement = this.Recurse(element);
                unchanged = unchanged && object.ReferenceEquals(element, replacedElement);

                if (replacedElement != null)
                {
                    var replacedElementAsGeneric = replacedElement as TElement;
                    ExceptionUtilities.CheckObjectNotNull(replacedElementAsGeneric, "Replaced element was of unexpected type");
                    elements.Add(replacedElementAsGeneric);
                }
            }

            if (unchanged)
            {
                return payloadElement;
            }

            return elements;
        }

        /// <summary>
        /// Wraps TOperation in MimePartData wrapper
        /// </summary>
        /// <typeparam name="TOperation">Type of operation</typeparam>
        /// <param name="operation">operation to wrap</param>
        /// <returns>Wrapped operation</returns>
        private MimePartData<TOperation> AsBatchFragment<TOperation>(TOperation operation)
            where TOperation : IMimePart
        {
            ExceptionUtilities.CheckArgumentNotNull(operation, "operation");

            return new MimePartData<TOperation>()
            {
                Headers =
                {
                    { HttpHeaders.ContentType, MimeTypes.ApplicationHttp },
                    { HttpHeaders.ContentTransferEncoding, "binary" },
                    { HttpHeaders.ContentId, "1" },
                },
                Body = operation
            };
        }
    }
}
