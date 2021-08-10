//---------------------------------------------------------------------
// <copyright file="TestODataJsonLightWriterResourceState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;

namespace Microsoft.OData.Tests.JsonLight
{
    internal class TestODataJsonLightWriterResourceState : IODataJsonLightWriterResourceState
    {
        public TestODataJsonLightWriterResourceState(
            ODataResourceBase resource,
            IEdmStructuredType structuredType,
            ODataResourceSerializationInfo serializationInfo,
            IEdmNavigationSource navigationSource,
            bool isUndeclared)
        {
            Resource = resource;
            ResourceType = structuredType;
            ResourceTypeFromMetadata = structuredType;
            SerializationInfo = serializationInfo;
            NavigationSource = navigationSource;
            IsUndeclared = isUndeclared;
        }

        public ODataResourceBase Resource { get; }

        public IEdmStructuredType ResourceType { get; }

        public IEdmStructuredType ResourceTypeFromMetadata { get; }

        public ODataResourceSerializationInfo SerializationInfo { get; }

        public IEdmNavigationSource NavigationSource { get; }

        public bool IsUndeclared { get; }

        public bool EditLinkWritten { get; set; }
        public bool ReadLinkWritten { get; set; }
        public bool MediaEditLinkWritten { get; set; }
        public bool MediaReadLinkWritten { get; set; }
        public bool MediaContentTypeWritten { get; set; }
        public bool MediaETagWritten { get; set; }

        public ODataResourceTypeContext GetOrCreateTypeContext(bool writingResponse)
        {
            return new ODataResourceTypeContext.ODataResourceTypeContextWithModel(
                NavigationSource,
                ResourceType as IEdmEntityType,
                ResourceType);
        }
    }
}
