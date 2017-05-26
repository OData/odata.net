//---------------------------------------------------------------------
// <copyright file="ObservedBatchPayloadFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Fixups
{
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;
    using ODataUri = Microsoft.Test.Taupo.Astoria.Contracts.OData.ODataUri;

    /// <summary>
    /// Performs fixups on the payloads in the batch
    /// </summary>
    public class ObservedBatchPayloadFixup : ODataPayloadElementVisitorBase
    {
        private IProtocolFormatNormalizerSelector selector;

        private IODataPayloadElementMetadataResolver resolver;

        public ObservedBatchPayloadFixup(IProtocolFormatNormalizerSelector normalizerSelector, IODataPayloadElementMetadataResolver resolver)
        {
            this.selector = normalizerSelector;
            this.resolver = resolver;
        }

        protected override void VisitRequestOperation(IHttpRequest operation)
        {
            ODataPayloadElement rootElement = null;
            string contentType = null;
            var requestOperation = operation as ODataRequest;

            ExceptionUtilities.CheckObjectNotNull(requestOperation, "Operation must be request");
            if (requestOperation.Body != null && requestOperation.Body.RootElement != null)
            {
                rootElement = requestOperation.Body.RootElement;
                contentType = requestOperation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
            }
            
            if (rootElement != null)
            {
                this.resolver.ResolveMetadata(rootElement, this.BuildODataUri(rootElement));
                this.selector.GetNormalizer(contentType);

            }
        }

        protected override void VisitResponseOperation(HttpResponseData operation)
        {
            var responseOperation = operation as ODataResponse;
            ExceptionUtilities.CheckObjectNotNull(responseOperation, "Operation must be a response");
            ODataPayloadElement rootElement = null;
            string contentType = null;
            if (responseOperation.RootElement != null)
            {
                rootElement = responseOperation.RootElement;
                contentType = responseOperation.GetHeaderValueIfExists(Microsoft.OData.ODataConstants.ContentTypeHeader);
            }
            if (rootElement != null)
            {
                this.resolver.ResolveMetadata(rootElement, this.BuildODataUri(rootElement));
                this.selector.GetNormalizer(contentType);

            }
        }

        private ODataUri BuildODataUri(ODataPayloadElement payload)
        {
            var entityInstance = payload as EntityInstance;
            if (entityInstance != null)
            {
                EntityModelTypeAnnotation type = entityInstance.GetAnnotation<EntityModelTypeAnnotation>();
                ExceptionUtilities.CheckObjectNotNull(type, "Type annotation was expected");

                var entityType = ((EntityDataType)type.EntityModelType).Definition;

                // First is OK because MEST doesnt matter
                var entitySet = entityType.Model.EntityContainers.SelectMany(c => c.EntitySets).First(s => s.EntityType == entityType);
                return new ODataUri(ODataUriBuilder.EntitySet(entitySet));
            }
            var complexProperty = payload as ComplexProperty;
            if (complexProperty != null)
            {
                EntityModelTypeAnnotation type = entityInstance.GetAnnotation<EntityModelTypeAnnotation>();
                ExceptionUtilities.CheckObjectNotNull(type, "Type annotation was expected");

                var entityType = ((EntityDataType)type.EntityModelType).Definition;

                // First is OK because MEST doesnt matter
                var entitySet = entityType.Model.EntityContainers.SelectMany(c => c.EntitySets).First(s => s.EntityType == entityType);
                return new ODataUri(ODataUriBuilder.EntitySet(entitySet));
            }

            // TODO: Add support for other types
            return null;
        }
    }
}
