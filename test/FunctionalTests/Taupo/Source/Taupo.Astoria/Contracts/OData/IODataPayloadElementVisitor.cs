//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Visits an OData payload element tree following the double dispatch visitor pattern. 
    /// </summary>
    public interface IODataPayloadElementVisitor
    {
        /// <summary>
        /// Visits a payload element whose root is a BatchRequestPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(BatchRequestPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a BatchResponsePayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(BatchResponsePayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ComplexInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexInstanceCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ComplexInstanceCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexMultiValue.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ComplexMultiValue payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexMultiValueProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ComplexMultiValueProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ComplexProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a DeferredLink.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(DeferredLink payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EmptyCollectionProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(EmptyCollectionProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EmptyPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(EmptyPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EmptyUntypedCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(EmptyUntypedCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EntityInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(EntityInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EntitySetInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(EntitySetInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an ExpandedLink.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ExpandedLink payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a LinkCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(LinkCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a NamedStreamProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(NamedStreamInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a NavigationProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(NavigationPropertyInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a NullPropertyInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(NullPropertyInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an ODataErrorPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ODataErrorPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an ODataInternalExceptionPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ODataInternalExceptionPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(PrimitiveCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveMultiValue.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(PrimitiveMultiValue payloadElement);
        
        /// <summary>
        /// Visits a payload element whose root is a PrimitiveMultiValueProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(PrimitiveMultiValueProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(PrimitiveProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveValue.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(PrimitiveValue payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ResourceCollectionInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ResourceCollectionInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ServiceDocumentInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ServiceDocumentInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an service Operation descriptor.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(ServiceOperationDescriptor payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a WorkspaceInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        void Visit(WorkspaceInstance payloadElement);
    }
}