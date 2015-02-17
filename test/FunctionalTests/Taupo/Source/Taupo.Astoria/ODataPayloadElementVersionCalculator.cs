//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementVersionCalculator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Component that calculates the version of the DataServiceVersion based on the payload
    /// </summary>
    [ImplementationName(typeof(IODataPayloadElementVersionCalculator), "Default")]
    public class ODataPayloadElementVersionCalculator : ODataPayloadElementVisitorBase, IODataPayloadElementVersionCalculator, IODataPayloadElementVisitor
    {
        private DataServiceProtocolVersion version;
        private DataServiceProtocolVersion currentMaxProtocolVersion;
        private DataServiceProtocolVersion currentMaxDataServiceVersion;
        private string responseContentType;

        /// <summary>
        /// Calculates the Protocol Version based on the payloadElement provided
        /// </summary>
        /// <param name="payloadElement">Payload Element</param>
        /// <param name="contentType">Content Type</param>
        /// <param name="maxProtocolVersion">The max protocol version</param>
        /// <param name="maxDataServiceVersion">The max data service version of the request</param>
        /// <returns>Data Service protocol version</returns>
        public DataServiceProtocolVersion CalculateProtocolVersion(ODataPayloadElement payloadElement, string contentType, DataServiceProtocolVersion maxProtocolVersion, DataServiceProtocolVersion maxDataServiceVersion)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
            ExceptionUtilities.Assert(maxProtocolVersion != DataServiceProtocolVersion.Unspecified, "Max protocol version cannot be unspecified");

            this.responseContentType = contentType;
            this.version = DataServiceProtocolVersion.V4;
            this.currentMaxProtocolVersion = maxProtocolVersion;
            this.currentMaxDataServiceVersion = maxDataServiceVersion;

            payloadElement.Accept(this);
            return this.version;
        }

        /// <summary>
        /// Visits a BatchRequestPayload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(BatchRequestPayload payloadElement)
        {
            throw new TaupoNotSupportedException("Batch payloads are not supported");
        }

        /// <summary>
        /// Visits a BatchResponsePayload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(BatchResponsePayload payloadElement)
        {
            throw new TaupoNotSupportedException("Batch payloads are not supported");
        }

        /// <summary>
        /// Visits a ComplexInstanceCollection, if this is not the result of a service operation call, it will be a V3 payload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(ComplexMultiValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            this.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits a ComplexMultiValueProperty, if this is not the result of a service operation call, it will be a V3 payload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(ComplexMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            this.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits a DeferredLink, if IncludeRelationshipLinks = true then this will be a V3 payload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(DeferredLink payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            this.IncreaseVersionIfIncludeRelationshipLinksIsTrue(payloadElement);

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element throws not supported
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EmptyCollectionProperty payloadElement)
        {
            throw new TaupoNotSupportedException("Should have been fixed in normalizer");
        }

        /// <summary>
        /// Visits the payload element and annotates it with metadata
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EmptyPayload payloadElement)
        {
            throw new TaupoNotSupportedException("Should have been fixed in normalizer");
        }

        /// <summary>
        /// Visits the payload element and throws not supported
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EmptyUntypedCollection payloadElement)
        {
            throw new TaupoNotSupportedException("Should have been fixed in normalizer");
        }

        /// <summary>
        /// Visits an EntityInstance and determines the version of it
        /// </summary>
        /// <param name="payloadElement">EntityInstance payload</param>
        public override void Visit(EntityInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            EntitySetAnnotation entitySetAnnotation = payloadElement.Annotations.OfType<EntitySetAnnotation>().SingleOrDefault();

            // skip when payloadElement.IsNull since null EntityInstance does not have FullType name and we don't need to increase version for null EntityInstance
            if (entitySetAnnotation != null && !payloadElement.IsNull)
            {
                var entitySet = entitySetAnnotation.EntitySet;
                var entityType = entitySet.Container.Model.EntityTypes.Single(t => t.IsKindOf(entitySet.EntityType) && t.FullName == payloadElement.FullTypeName);

                this.IncreaseVersionIfRequired(entityType.CalculateProtocolVersion(this.responseContentType, VersionCalculationType.Response, this.currentMaxProtocolVersion, this.currentMaxDataServiceVersion));

                if (payloadElement.Annotations.OfType<XmlTreeAnnotation>().Any())
                {
                    var epmVersion = VersionHelper.CalculateEntityPropertyMappingProtocolVersion(entityType, VersionCalculationType.Response, this.responseContentType, this.currentMaxProtocolVersion, this.currentMaxDataServiceVersion);
                    this.IncreaseVersionIfRequired(epmVersion);
                }
            }

            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(EntitySetInstance payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.IncreaseVersionIfNonNull(payloadElement.NextLink, DataServiceProtocolVersion.V4);
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits a LinkCollection, if IncludeRelationshipLinks = true then this will be a V3 payload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(LinkCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.IncreaseVersionIfIncludeRelationshipLinksIsTrue(payloadElement);
            this.IncreaseVersionIfNonNull(payloadElement.NextLink, DataServiceProtocolVersion.V4);
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits the payload element and determines the version of it
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NamedStreamInstance payloadElement)
        {
            this.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4); 
        }

        /// <summary>
        /// Visits the payload element and throws not supported
        /// </summary>
        /// <param name="payloadElement">The payload element to visit</param>
        public override void Visit(NullPropertyInstance payloadElement)
        {
            throw new TaupoNotSupportedException("Should have been fixed in normalizer");
        }

        /// <summary>
        /// Visits a PrimitiveCollection, if it visits this it will be a V3 payload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(PrimitiveMultiValue payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            base.Visit(payloadElement);
        }

        /// <summary>
        /// Visits a PrimitiveMultiValueProperty, if it visits this it will be a V3 payload
        /// </summary>
        /// <param name="payloadElement">payload Element</param>
        public override void Visit(PrimitiveMultiValueProperty payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
            base.Visit(payloadElement);
        }
        
        /// <summary>
        /// Helper method for visiting collections
        /// </summary>
        /// <param name="payloadElement">The collection to visit</param>
        protected override void VisitCollection(ODataPayloadElementCollection payloadElement)
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            this.IncreaseVersionIfTrue(payloadElement.Annotations.OfType<JsonCollectionResultWrapperAnnotation>().Any(a => a.Value), DataServiceProtocolVersion.V4);
            base.VisitCollection(payloadElement);
        }

        private void IncreaseVersionIfRequired(DataServiceProtocolVersion newVersion)
        {
            this.version = VersionHelper.IncreaseVersionIfRequired(this.version, newVersion);
        }

        private void IncreaseVersionIfNonNull(object valueToCheck, DataServiceProtocolVersion newVersion)
        {
            this.IncreaseVersionIfTrue(valueToCheck != null, newVersion);
        }

        private void IncreaseVersionIfTrue(bool test, DataServiceProtocolVersion newVersion)
        {
            if (test)
            {
                this.IncreaseVersionIfRequired(newVersion);
            }
        }

        private void IncreaseVersionIfIncludeRelationshipLinksIsTrue(ODataPayloadElement payloadElement)
        {
            var navPropAnnotation = payloadElement.Annotations.OfType<NavigationPropertyAnnotation>().SingleOrDefault();
            if (navPropAnnotation != null)
            {
                var dataServiceBehaviorAnnotation = navPropAnnotation.Property.Association.Model.GetDefaultEntityContainer().GetDataServiceBehavior();

                if (dataServiceBehaviorAnnotation != null && dataServiceBehaviorAnnotation.IncludeAssociationLinksInResponse.HasValue && dataServiceBehaviorAnnotation.IncludeAssociationLinksInResponse.Value)
                {
                    this.IncreaseVersionIfRequired(DataServiceProtocolVersion.V4);
                }
            }
        }
    }
}
