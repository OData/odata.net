//---------------------------------------------------------------------
// <copyright file="RemoveAtomMetadataFromPayloadElementVisitor.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Reader.Tests
{
    #region Namespaces
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    #endregion Namespaces

    /// <summary>
    /// Visitor which removes ATOM metadata from <see cref="ODataPayloadElement"/> objects.
    /// </summary>
    public class RemoveAtomMetadataFromPayloadElementVisitor : ODataPayloadElementVisitorBase
    {
        /// <summary>
        /// Removes ATOM metadata from the given payload element and its descendants.
        /// </summary>
        /// <param name="payloadElement">The root of the payload element tree to visit.</param>
        /// <returns>The <paramref name="payloadElement"/> after it has been visited.</returns>
        public static ODataPayloadElement Visit(ODataPayloadElement payloadElement)
        {
            new RemoveAtomMetadataFromPayloadElementVisitor().Recurse(payloadElement);
            return payloadElement;
        }

        /// <summary>
        /// Wrapper which removes XmlTreeAnnotations before visiting the given payload element.
        /// </summary>
        /// <param name="element">The payload element to visit</param>
        protected override void Recurse(ODataPayloadElement element)
        {
            element.RemoveAnnotations(typeof(XmlTreeAnnotation));
            base.Recurse(element);
        }

        /// <summary>
        /// Removes ATOM metadata annotations from an <see cref="EntityInstance"/>.
        /// </summary>
        /// <param name="payloadElement">The entity instance to visit.</param>
        public override void Visit(EntityInstance payloadElement)
        {
            payloadElement.RemoveAnnotations(typeof(NamedStreamAtomLinkMetadataAnnotation));
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Removes ATOM link metadata annotations from a <see cref="NamedStreamInstance"/>.
        /// </summary>
        /// <param name="payloadElement">The named stream instance to visit.</param>
        public override void Visit(NamedStreamInstance payloadElement)
        {
            payloadElement.RemoveAnnotations(typeof(NamedStreamAtomLinkMetadataAnnotation));
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Removes ATOM link metadata annotations from a <see cref="NavigationPropertyInstance"/>.
        /// </summary>
        /// <param name="payloadElement">The navigation property instance to visit.</param>
        public override void Visit(NavigationPropertyInstance payloadElement)
        {
            if (payloadElement.AssociationLink != null)
            {
                payloadElement.AssociationLink.RemoveAnnotations(typeof(XmlTreeAnnotation));
            }
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Removes the title property and ATOM metadata annotations from a <see cref="WorkspaceInstance"/>.
        /// </summary>
        /// <param name="payloadElement">The workspace instance to visit.</param>
        public override void Visit(WorkspaceInstance payloadElement)
        {
            payloadElement.Title = null;
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Removes the title property and ATOM metadata annotations from a <see cref="ResourceCollectionInstance"/>.
        /// </summary>
        /// <param name="payloadElement">The resource collection instance to visit</param>
        public override void Visit(ResourceCollectionInstance payloadElement)
        {
            payloadElement.Title = null;
            base.Visit(payloadElement);
        }
    }
}