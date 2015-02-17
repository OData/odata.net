//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementVisitorBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Basic implementation of the payload element that includes calls to visit child nodes
    /// </summary>
    public abstract class ODataPayloadElementVisitorBase : IODataPayloadElementVisitor
    {
        /// <summary>
        /// Initializes a new instance of the ODataPayloadElementVisitorBase class
        /// </summary>
        protected ODataPayloadElementVisitorBase()
        {
            this.VisitCallback = e => e.Accept(this);
        }

        /// <summary>
        /// Gets or sets the callback to use when visiting elements. Unit test hook only.
        /// </summary>
        internal Action<ODataPayloadElement> VisitCallback { get; set; }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(BatchRequestPayload payloadElement)
        {
            this.VisitBatch<IHttpRequest, BatchRequestChangeset, BatchRequestPayload>(payloadElement, this.VisitRequestOperation);
        }
        
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(BatchResponsePayload payloadElement)
        {
            this.VisitBatch<HttpResponseData, BatchResponseChangeset, BatchResponsePayload>(payloadElement, this.VisitResponseOperation);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitEnumerable(payloadElement.Properties);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ComplexInstanceCollection payloadElement)
        {
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ComplexMultiValue payloadElement)
        {
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ComplexMultiValueProperty payloadElement)
        {
            this.VisitProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ComplexProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(DeferredLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(EmptyCollectionProperty payloadElement)
        {
            this.VisitEmptyOrNullProperty(payloadElement);
        }
        
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(EmptyPayload payloadElement)
        {
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(EmptyUntypedCollection payloadElement)
        {
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            this.VisitEnumerable(payloadElement.Properties);
            this.VisitEnumerable(payloadElement.ServiceOperationDescriptors);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ExpandedLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (payloadElement.ExpandedElement != null)
            {
                this.Recurse(payloadElement.ExpandedElement);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(LinkCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(NamedStreamInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(NavigationPropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (payloadElement.Value != null)
            {
                this.Recurse(payloadElement.Value);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(NullPropertyInstance payloadElement)
        {
            this.VisitEmptyOrNullProperty(payloadElement);
        }

        /// <summary>
        /// Visits a payload element whose root is an ODataErrorPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        public virtual void Visit(ODataErrorPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (payloadElement.InnerError != null)
            {
                this.Recurse(payloadElement.InnerError);
            }
        }

        /// <summary>
        /// Visits a payload element whose root is an ODataInternalExceptionPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        public virtual void Visit(ODataInternalExceptionPayload payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            if (payloadElement.InternalException != null)
            {
                this.Recurse(payloadElement.InternalException);
            }
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(PrimitiveCollection payloadElement)
        {
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(PrimitiveMultiValue payloadElement)
        {
            this.VisitCollection(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitProperty(payloadElement, payloadElement.Value);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(PrimitiveProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitProperty(payloadElement, payloadElement.Value);
        }
        
        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(PrimitiveValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ResourceCollectionInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ServiceDocumentInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitEnumerable<WorkspaceInstance>(payloadElement.Workspaces);
        }

        /// <summary>
        /// Visits the Service Operation Descriptor payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(ServiceOperationDescriptor payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public virtual void Visit(WorkspaceInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.VisitEnumerable<ResourceCollectionInstance>(payloadElement.ResourceCollections);
        }

        /// <summary>
        /// Wrapper for recursively visiting the given element. Used with the callback property to make unit tests easier.
        /// </summary>
        /// <param name="element">The element to visit</param>
        protected virtual void Recurse(ODataPayloadElement element)
        {
            ExceptionUtilities.CheckArgumentNotNull(element, "element");
            ExceptionUtilities.CheckObjectNotNull(this.VisitCallback, "Visit callback unexpectedly null");
            this.VisitCallback(element);
        }

        /// <summary>
        /// Helper method for visiting properties
        /// </summary>
        /// <param name="payloadElement">The property to visit</param>
        /// <param name="value">The value of the property</param>
        protected virtual void VisitProperty(PropertyInstance payloadElement, ODataPayloadElement value)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            if (value != null)
            {
                this.Recurse(value);
            }
        }

        /// <summary>
        /// Visits the batch payload.
        /// </summary>
        /// <typeparam name="TOperation">The operation type.</typeparam>
        /// <typeparam name="TChangeset">The changeset type.</typeparam>
        /// <typeparam name="TBatch">The batch payload type.</typeparam>
        /// <param name="batchPayload">The batch payload we are visiting.</param>
        /// <param name="visitOperation">The action to use to visit the operation.</param>
        protected virtual void VisitBatch<TOperation, TChangeset, TBatch>(TBatch batchPayload, Action<TOperation> visitOperation)
            where TBatch : BatchPayload<TOperation, TChangeset>
            where TChangeset : BatchChangeset<TOperation>
            where TOperation : class, IMimePart
        {
            foreach (var part in batchPayload.Parts)
            {
                var changeSet = part as TChangeset;
                if (changeSet != null)
                {
                    this.VisitChangeset(changeSet, visitOperation);
                }
                else
                {
                    var operation = (MimePartData<TOperation>)part;
                    visitOperation(operation.Body);
                }
            }
        }

        /// <summary>
        /// Visits the batch changeset.
        /// </summary>
        /// <typeparam name="TOperation">Type of operation in the changeset.</typeparam>
        /// <typeparam name="TChangeset">Type of changeset.</typeparam>
        /// <param name="changeset">Changeset to visit.</param>
        /// <param name="visitOperation">Action to use when visiting operations.</param>
        protected virtual void VisitChangeset<TOperation, TChangeset>(TChangeset changeset, Action<TOperation> visitOperation)
            where TOperation : class, IMimePart
            where TChangeset : BatchChangeset<TOperation>
        {   
            foreach (var operation in changeset.Operations)
            {
                visitOperation(operation);
            }
        }

        /// <summary>
        /// Replaces operation if payload is updated
        /// </summary>
        /// <param name="operation">Operation to visit</param>
        protected virtual void VisitResponseOperation(HttpResponseData operation)
        {
            var responseOperation = operation as ODataResponse;

            if (responseOperation.RootElement != null)
            {
                this.Recurse(responseOperation.RootElement);
            }
        }

        /// <summary>
        /// Visits a request payload
        /// </summary>
        /// <param name="operation">Operation to visit</param>
        protected virtual void VisitRequestOperation(IHttpRequest operation)
        {
            ExceptionUtilities.CheckArgumentNotNull(operation, "operation");
            var requestOperation = operation as ODataRequest;
            if (requestOperation.Body != null && requestOperation.Body.RootElement != null)
            {
                this.Recurse(requestOperation.Body.RootElement);
            }
        }

        /// <summary>
        /// Helper method for visiting collections
        /// </summary>
        /// <param name="payloadElement">The collection to visit</param>
        protected virtual void VisitCollection(ODataPayloadElementCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            var enumerable = payloadElement as IEnumerable;
            if (enumerable != null)
            {
                this.VisitEnumerable(enumerable.Cast<ODataPayloadElement>());
            }
        }

        /// <summary>
        /// Helper method for visiting enumerables of payload elements
        /// </summary>
        /// <typeparam name="T">The observed type of the <see cref="ODataPayloadElement"/> used in the enumerable.</typeparam>
        /// <param name="enumerable">The collection to visit</param>
        protected virtual void VisitEnumerable<T>(IEnumerable<T> enumerable) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(enumerable, "enumerable");

            foreach (var element in enumerable)
            {
                this.Recurse(element);
            }
        }

        /// <summary>
        /// Helper method for visiting null or empty properties
        /// </summary>
        /// <param name="payloadElement">The property to visit</param>
        protected virtual void VisitEmptyOrNullProperty(PropertyInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
        }
    }
}
