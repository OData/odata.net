//---------------------------------------------------------------------
// <copyright file="ReorderPayloadElementPropertiesTransform.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.PayloadTransformation
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.OData.Common;
    #endregion Namespaces

    /// <summary>
    /// Paylaod transform class for re-ordering payload element properties.
    /// </summary>
    public class ReorderPayloadElementPropertiesTransform : ODataPayloadElementReplacingVisitor, IPayloadTransform<ODataPayloadElement>
    {
        /// <summary>
        /// Gets or sets injected test parameter ODataPayloadElementDeepCopyingVisitor.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ODataPayloadElementDeepCopyingVisitor ODataPayloadElementDeepCopyingVisitor { get; set; }

        /// <summary>
        /// Transforms the original payload by re-ordering properties.
        /// </summary>
        /// <param name="originalPayload">Original payload.</param>
        /// <param name="transformedPayload">Transformed payload.</param>
        /// <returns>True if the transformation is successful else returns false.</returns>
        public bool TryTransform(ODataPayloadElement originalPayload, out ODataPayloadElement transformedPayload)
        {
            ExceptionUtilities.CheckObjectNotNull(originalPayload, "Payload cannot be null.");
            transformedPayload = originalPayload.Accept(this);

            return true;
        }

        /// <summary>
        /// Visits a payload element whose root is an EntitySetInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override ODataPayloadElement Visit(EntitySetInstance payloadElement)
        {
            EntitySetInstance entitySetInstance = (EntitySetInstance)base.Visit(payloadElement);

            foreach (EntityInstance instance in entitySetInstance)
            {
                instance.Accept(this);
            }

            return entitySetInstance;
        }

        /// <summary>
        /// Visits a payload element whose root is an EntityInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override ODataPayloadElement Visit(EntityInstance payloadElement)
        {
            EntityInstance entityInstance = (EntityInstance)base.Visit(payloadElement);
            ReorderProperties(entityInstance);

            return entityInstance;
        }

        /// <summary>
        /// Visits a payload element whose root is an ComplexInstance.
        /// </summary>
        /// <param name="payloadElement">The root node of the payload element being visited.</param>
        public override ODataPayloadElement Visit(ComplexInstance payloadElement)
        {
            ComplexInstance complexInstance = (ComplexInstance)base.Visit(payloadElement);
            ReorderProperties(complexInstance);

            return complexInstance;
        }

        /// <summary>
        /// Reorders payloadElement properties.
        /// </summary>
        /// <param name="complexInstance">The payloadElement for which properties need to be reordered.</param>
        /// <returns>PayloadElement with reordered properties.</returns>
        private ODataPayloadElement ReorderProperties(ComplexInstance complexInstance)
        {
            ComplexInstance instance = (ComplexInstance)ODataPayloadElementDeepCopyingVisitor.DeepCopy(complexInstance);

            if (instance != null)
            {
                ((List<PropertyInstance>)instance.Properties).Reverse();
            }

            return complexInstance;
        }
    }
}
