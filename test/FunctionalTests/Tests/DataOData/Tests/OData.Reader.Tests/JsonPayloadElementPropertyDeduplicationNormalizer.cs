//---------------------------------------------------------------------
// <copyright file="JsonPayloadElementPropertyDeduplicationNormalizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// Normalizes payload element tree for JSON payloads by removing duplicate properties as per WCF DS Server behavior.
    /// </summary>
    public class JsonPayloadElementPropertyDeduplicationNormalizer : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Normalizes payload element tree for JSON format by deduplicating properties.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        /// <returns>The normalized payload element.</returns>
        public static ODataPayloadElement Normalize(ODataPayloadElement payloadElement)
        {
            new JsonPayloadElementPropertyDeduplicationNormalizer().Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Normalizes entity.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            // First recurse to children so that we get those normalized and then normalize the parent entity.
            base.Visit(payloadElement);

            this.DeduplicateProperties(payloadElement);
        }

        /// <summary>
        /// Normalizes complex value.
        /// </summary>
        /// <param name="payloadElement">The payload element to normalize.</param>
        public override void Visit(ComplexInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            // First recurse to children so that we get those normalized and then normalize the parent entity.
            base.Visit(payloadElement);

            this.DeduplicateProperties(payloadElement);
        }

        /// <summary>
        /// Removes duplicate properties from the specified complex or entity value.
        /// </summary>
        /// <param name="payloadElement"></param>
        private void DeduplicateProperties(ComplexInstance payloadElement)
        {
            Dictionary<string, PropertyInstance> properties = new Dictionary<string, PropertyInstance>();
            foreach (PropertyInstance propertyInstance in payloadElement.Properties.ToList())
            {
                PropertyInstance firstPropertyInstance;
                if (properties.TryGetValue(propertyInstance.Name, out firstPropertyInstance))
                {
                    payloadElement.Remove(propertyInstance);
                    payloadElement.Replace(firstPropertyInstance, propertyInstance);
                    properties[propertyInstance.Name] = propertyInstance;
                }
                else
                {
                    properties.Add(propertyInstance.Name, propertyInstance);
                }
            }
        }
    }
}
