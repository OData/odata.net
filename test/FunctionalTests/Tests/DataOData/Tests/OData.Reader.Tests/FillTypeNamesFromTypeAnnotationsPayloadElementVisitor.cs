//---------------------------------------------------------------------
// <copyright file="FillTypeNamesFromTypeAnnotationsPayloadElementVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Visitor which fills in missing type names from the type annotations.
    /// </summary>
    public class FillTypeNamesFromTypeAnnotationsPayloadElementVisitor : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Visit the payload element and fill in type names from the type annotations.
        /// </summary>
        /// <param name="payloadElement">The root of the payload element tree to visit.</param>
        /// <returns>The visited payload element.</returns>
        public static ODataPayloadElement Visit(ODataPayloadElement payloadElement)
        {
            new FillTypeNamesFromTypeAnnotationsPayloadElementVisitor().Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            if (!payloadElement.IsNull)
            {
                EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
                if (payloadElement.FullTypeName == null && typeAnnotation != null)
                {
                    payloadElement.FullTypeName = ((IEdmComplexTypeReference)typeAnnotation.EdmModelType).FullName();
                }
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveValue payloadElement)
        {
            if (!payloadElement.IsNull)
            {
                EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
                if (payloadElement.FullTypeName == null && typeAnnotation != null)
                {
                    payloadElement.FullTypeName = ((IEdmPrimitiveTypeReference)typeAnnotation.EdmModelType).FullName();
                }
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(ComplexMultiValue payloadElement)
        {
            EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
            if (payloadElement.FullTypeName == null && typeAnnotation != null)
            {
                payloadElement.FullTypeName = EntityModelUtils.GetCollectionTypeName(((IEdmCollectionTypeReference)typeAnnotation.EdmModelType).ElementType().FullName());
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntityInstance payloadElement)
        {
            EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
            if (payloadElement.FullTypeName == null && typeAnnotation != null)
            {
                payloadElement.FullTypeName = ((IEdmEntityTypeReference)typeAnnotation.EdmModelType).FullName();
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(PrimitiveMultiValue payloadElement)
        {
            EntityModelTypeAnnotation typeAnnotation = payloadElement.GetAnnotation<EntityModelTypeAnnotation>();
            if (payloadElement.FullTypeName == null && typeAnnotation != null)
            {
                payloadElement.FullTypeName = EntityModelUtils.GetCollectionTypeName(((IEdmCollectionTypeReference)typeAnnotation.EdmModelType).ElementType().FullName());
            }

            base.Visit(payloadElement);
        }
    }
}