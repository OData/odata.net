//---------------------------------------------------------------------
// <copyright file="MetadataDocumentReorderingTransform.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.PayloadTransformation
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    #endregion Namespaces

    /// <summary>
    /// This transform reorders the elements within a metadata document payload.
    /// </summary>
    public class MetadataDocumentReorderingTransform : IPayloadTransform<XElement>
    {
        // Maps the metadata document element type enum to the corresponding XML element local name
        private static readonly Dictionary<MetadataDocumentElementType, string> metadataElementTypeNameMap = new Dictionary<MetadataDocumentElementType, string>
            {
                { MetadataDocumentElementType.AssociationType, "Association"  },
                { MetadataDocumentElementType.ComplexType, "ComplexType"  },
                { MetadataDocumentElementType.EntityType, "EntityType"  },
                { MetadataDocumentElementType.EntityContainer, "EntityContainer" },
            };

        // Maps the metadata document element type enum to the corresponding XML element local name
        private static readonly Dictionary<EntityContainerElementType, string> entityContainerElementTypeNameMap = new Dictionary<EntityContainerElementType, string>
            {
                { EntityContainerElementType.AssociationSet, "AssociationSet" },
                { EntityContainerElementType.EntitySet, "EntitySet" },
                { EntityContainerElementType.FunctionImport, "FunctionImport" },
            };

        private IEnumerable<MetadataDocumentElementType> metadataDocumentElementTypeOrder;
        private IEnumerable<EntityContainerElementType> entityContainerElementTypeOrder;

        /// <summary>
        /// Initializes a new instance of the MetadataDocumentReorderingTransform class.
        /// </summary>
        /// <param name="metadataDocumentElementOrder">The order to arrange metadata document elements.</param>
        /// <param name="entityContainerElementOrder">The order to arrange entity container elements.</param>
        public MetadataDocumentReorderingTransform(IEnumerable<MetadataDocumentElementType> metadataDocumentElementOrder, IEnumerable<EntityContainerElementType> entityContainerElementOrder)
        {
            ExceptionUtilities.CheckArgumentNotNull(metadataDocumentElementOrder, "metadataDocumentElementOrder");
            ExceptionUtilities.CheckArgumentNotNull(entityContainerElementOrder, "entityContainerElementOrder");
            ExceptionUtilities.Assert(!metadataDocumentElementOrder.GroupBy(e => e).Any(g => g.Count() > 1), "Metadata Document Element order list contains duplicate element types");
            ExceptionUtilities.Assert(!entityContainerElementOrder.GroupBy(e => e).Any(g => g.Count() > 1), "Entity Container Element order list contains duplicate element types");

            this.metadataDocumentElementTypeOrder = metadataDocumentElementOrder;
            this.entityContainerElementTypeOrder = entityContainerElementOrder;
        }

        /// <summary>
        /// Attempts to transform the metadata document by reordering the child elements.
        /// </summary>
        /// <param name="originalPayload">The payload to transform.</param>
        /// <param name="transformedPayload">The transformed payload.</param>
        /// <returns>Whether or not the transformation was successful.</returns>
        public bool TryTransform(XElement originalPayload, out XElement transformedPayload)
        {
            ExceptionUtilities.CheckArgumentNotNull(originalPayload, "originalPayload");

            XElement copyOfPayload = new XElement(originalPayload);

            XElement schemaElement = copyOfPayload.Descendants().Where(d => d.Name.LocalName.Equals("Schema")).SingleOrDefault();
            if (schemaElement != null)
            {
                ReorderChildElements(schemaElement, this.metadataDocumentElementTypeOrder.Select(e => metadataElementTypeNameMap[e]));
                foreach(var entityContainerElement in schemaElement.Elements().Where(e => e.Name.LocalName.Equals(metadataElementTypeNameMap[MetadataDocumentElementType.EntityContainer])))
                {
                    ReorderChildElements(entityContainerElement, this.entityContainerElementTypeOrder.Select(e => entityContainerElementTypeNameMap[e]));
                }

                transformedPayload = copyOfPayload;
                return true;
            }

            transformedPayload = null;
            return false;
        }

        private static void ReorderChildElements(XElement parentElement, IEnumerable<string> elementNameOrder)
        {
            // Organize elements according to element type
            var elementBackup = elementNameOrder.SelectMany(name => parentElement.Elements().Where(e => e.Name.LocalName.Equals(name))).ToArray();

            // Cache the elements which do not belong to any of the configured types
            var remainingElements = parentElement.Elements().Where(e => !elementNameOrder.Contains(e.Name.LocalName)).ToArray();

            // Cache the non-element nodes
            var nonElementNodes = parentElement.Nodes().Where(n => !(n is XElement)).ToArray();

            parentElement.RemoveNodes();

            // Add the elements back, in the specified type order
            parentElement.Add(elementBackup);
            parentElement.Add(remainingElements);
            parentElement.Add(nonElementNodes);
        }

        /// <summary>
        /// Enum representing the metadata document element types that will be reordered.
        /// </summary>
        public enum MetadataDocumentElementType
        {
            EntityContainer,
            EntityType,
            ComplexType,
            AssociationType,
        }

        /// <summary>
        /// Enum representing the entity container element types that will be reordered.
        /// </summary>
        public enum EntityContainerElementType
        {
            EntitySet,
            AssociationSet,
            FunctionImport,
        }
    }
}
