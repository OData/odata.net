//---------------------------------------------------------------------
// <copyright file="RemoveCollectionNameFromCollectionPayloadElementVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Visitor which removes the collection name from the collection payload element.
    /// </summary>
    public class RemoveCollectionNameFromCollectionPayloadElementVisitor : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visit the collection payload element and removes the collection name if it is present.
        /// </summary>
        /// <param name="payloadElement">The root of the payload element tree to visit.</param>
        /// <remarks>This method will modify the input payload element, so if there is a need to 
        /// preserve the actual input payload element it should be cloned.
        /// </remarks>
        public static ODataPayloadElement Visit(ODataPayloadElement payloadElement)
        {
            if (payloadElement != null)
            {
                new RemoveCollectionNameFromCollectionPayloadElementVisitor().Recurse(payloadElement);
            }

            return payloadElement;
        }

        /// <summary>
        /// Visits the payload element and removes the collection name if it is present.
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveCollection payloadElement)
        {
            CollectionNameAnnotation collectionNameAnnotation = payloadElement.GetAnnotation<CollectionNameAnnotation>();
            if (payloadElement != null && collectionNameAnnotation != null)
            {
                payloadElement.Annotations.Remove(collectionNameAnnotation);
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element and removes the collection name if it is present.
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstanceCollection payloadElement)
        {
            CollectionNameAnnotation collectionNameAnnotation = payloadElement.GetAnnotation<CollectionNameAnnotation>();
            if (payloadElement != null && collectionNameAnnotation != null)
            {
                payloadElement.Annotations.Remove(collectionNameAnnotation);
            }

            base.Visit(payloadElement);
        }
    }
}
