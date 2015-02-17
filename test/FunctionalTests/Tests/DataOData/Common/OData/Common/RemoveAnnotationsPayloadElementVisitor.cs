//---------------------------------------------------------------------
// <copyright file="RemoveAnnotationsPayloadElementVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// This visitor walks the payload element and removes all annotations of the specified type.
    /// </summary>
    /// <typeparam name="T">the type of payload element annotation to remove</typeparam>
    public class RemoveAnnotationsPayloadElementVisitor<T> : ODataPayloadElementVisitorBase where T : ODataPayloadElementAnnotation
    {
        /// <summary>
        /// Removes annotations on the payload element and initiates the Visitor pattern.
        /// </summary>
        /// <param name="payloadElement">The payload element to remove annotations from.</param>
        /// <returns>The payload element with annotations removed.</returns>
        public static ODataPayloadElement VisitPayload(ODataPayloadElement payloadElement)
        {
            new RemoveAnnotationsPayloadElementVisitor<T>().Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Removes annotations on the payload element and initiates the Visitor pattern.
        /// </summary>
        /// <param name="payloadElement">The payload element to remove annotations from.</param>
        public void RemoveAnnotations(ODataPayloadElement payloadElement)
        {
            payloadElement.RemoveAnnotations(typeof(T));
            payloadElement.Accept(this);
        }

        /// <summary>
        /// Removes annotations from the payload element and recurses to child elements.
        /// </summary>
        /// <param name="element">The payload element to remove annotations from.</param>
        protected override void Recurse(ODataPayloadElement element)
        {
            element.RemoveAnnotations(typeof(T));
            base.Recurse(element);
        }
    }
}
