//---------------------------------------------------------------------
// <copyright file="TestODataResourceMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.OData.Evaluation;
using Microsoft.OData.JsonLight;

namespace Microsoft.OData.Tests.JsonLight
{
    /// <summary>
    /// Metadata builder class specifically designed to help support test scenarios for ODataJsonLightResourceSerializer
    /// </summary>
    internal class TestODataResourceMetadataBuilder : ODataResourceMetadataBuilder
    {
        private readonly Uri resourceId;
        private readonly Uri editLink;
        private readonly string resourceUriString;
        private readonly Func<IEnumerable<ODataJsonLightReaderNestedResourceInfo>> unprocessedNavigationLinksFactory;
        private readonly Func<IEnumerable<ODataProperty>> unprocessedStreamPropertiesFactory;
        private IEnumerator<ODataJsonLightReaderNestedResourceInfo> unprocessedNavigationLinks;
        private IEnumerator<ODataProperty> unprocessedStreamProperties;

        public TestODataResourceMetadataBuilder(
            Uri resourceId,
            Func<IEnumerable<ODataJsonLightReaderNestedResourceInfo>> unprocessedNavigationLinksFactory = null,
            Func<IEnumerable<ODataProperty>> unprocessedStreamPropertiesFactory = null)
        {
            Debug.Assert(resourceId != null, "resourceId != null");

            this.resourceId = resourceId;
            this.unprocessedNavigationLinksFactory = unprocessedNavigationLinksFactory;
            this.unprocessedStreamPropertiesFactory = unprocessedStreamPropertiesFactory;
            this.unprocessedNavigationLinks = null;
            this.unprocessedStreamProperties = null;

            this.resourceUriString = this.resourceId.ToString().TrimEnd('/');
            this.editLink = new Uri($"{this.resourceUriString}/Edit");
        }

        internal override Uri GetEditLink()
        {
            return this.editLink;
        }

        internal override string GetETag()
        {
            return "resource-etag";
        }

        internal override Uri GetId()
        {
            return this.resourceId;
        }

        internal override Uri GetReadLink()
        {
            return this.resourceId;
        }

        internal override bool TryGetIdForSerialization(out Uri id)
        {
            id = resourceId;

            return true;
        }

        internal override Uri GetNavigationLinkUri(string navigationPropertyName, Uri navigationLinkUrl, bool hasNestedResourceInfoUrl)
        {
            return hasNestedResourceInfoUrl ? navigationLinkUrl : new Uri($"{this.resourceUriString}/{navigationPropertyName}");
        }

        internal override Uri GetAssociationLinkUri(string navigationPropertyName, Uri associationLinkUrl, bool hasAssociationLinkUrl)
        {
            return hasAssociationLinkUrl ? associationLinkUrl : new Uri($"{this.resourceUriString}/{navigationPropertyName}/$ref");
        }

        internal override ODataJsonLightReaderNestedResourceInfo GetNextUnprocessedNavigationLink()
        {
            if (this.unprocessedNavigationLinks == null && this.unprocessedNavigationLinksFactory != null)
            {
                this.unprocessedNavigationLinks = this.unprocessedNavigationLinksFactory().GetEnumerator();
            }

            if (this.unprocessedNavigationLinks != null && this.unprocessedNavigationLinks.MoveNext())
            {
                return unprocessedNavigationLinks.Current;
            }

            return null;
        }

        internal override ODataProperty GetNextUnprocessedStreamProperty()
        {
            if (this.unprocessedStreamProperties == null && this.unprocessedStreamPropertiesFactory != null)
            {
                this.unprocessedStreamProperties = this.unprocessedStreamPropertiesFactory().GetEnumerator();
            }

            if (this.unprocessedStreamProperties != null && this.unprocessedStreamProperties.MoveNext())
            {
                return unprocessedStreamProperties.Current;
            }

            return null;
        }
    }
}
