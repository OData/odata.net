//---------------------------------------------------------------------
// <copyright file="IODataPayloadElementVisitor`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    /// <summary>
    /// Visits an OData payload element tree following the double dispatch visitor pattern. 
    /// </summary>
    /// <typeparam name="TResult">The type which the visitor will return.</typeparam>
    public interface IODataPayloadElementVisitor<TResult>
    {
        /// <summary>
        /// Visits a payload element whose root is a BatchRequestPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(BatchRequestPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a BatchResponsePayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(BatchResponsePayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ComplexInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexInstanceCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ComplexInstanceCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexMultiValue.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ComplexMultiValue payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexMultiValueProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ComplexMultiValueProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ComplexProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ComplexProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a DeferredLink.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(DeferredLink payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EmptyCollectionProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(EmptyCollectionProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EmptyPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(EmptyPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EmptyUntypedCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(EmptyUntypedCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EntityInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(EntityInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an EntitySetInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(EntitySetInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an ExpandedLink.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ExpandedLink payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a LinkCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(LinkCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a NamedStreamProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(NamedStreamInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a NavigationProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(NavigationPropertyInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a NullPropertyInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(NullPropertyInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an ODataErrorPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ODataErrorPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an ODataInternalExceptionPayload.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ODataInternalExceptionPayload payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveCollection.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(PrimitiveCollection payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveMultiValue.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(PrimitiveMultiValue payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveMultiValueProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(PrimitiveMultiValueProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveProperty.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(PrimitiveProperty payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a PrimitiveValue.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(PrimitiveValue payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ResourceCollectionInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ResourceCollectionInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is a ServiceDocumentInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(ServiceDocumentInstance payloadElement);

        /// <summary>
        /// Visits a payload element whose root is an service Operation Descriptor.
        /// </summary>
        /// <param name="serviceOperationDescriptor">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element</returns>
        TResult Visit(ServiceOperationDescriptor serviceOperationDescriptor);

        /// <summary>
        /// Visits a payload element whose root is a WorkspaceInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of payload element being visited.</param>
        /// <returns>The result of visiting this element.</returns>
        TResult Visit(WorkspaceInstance payloadElement);
    }
}
