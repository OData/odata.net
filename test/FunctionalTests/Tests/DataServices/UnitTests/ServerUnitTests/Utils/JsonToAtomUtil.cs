//---------------------------------------------------------------------
// <copyright file="JsonToAtomUtil.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using Microsoft.OData.Service.Providers;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This class has all the utility methods that are used by jsontoatom.xslt to convert json payload to atom payload.
    /// </summary>
    public class JsonToAtomUtil
    {
        private readonly IDataServiceMetadataProvider metadataProvider;

        public JsonToAtomUtil(IDataServiceMetadataProvider metadataProvider)
        {
            this.metadataProvider = metadataProvider;
        }

        public string GetLinkTypeAttributeValue(string typeName, string propertyName)
        {
            ResourceProperty property = this.ResolveProperty(typeName, propertyName);
            Assert.IsTrue(property != null && property.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType, "property != null && property.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType");
            if (property.Kind == ResourcePropertyKind.ResourceReference)
            {
                return "application/atom+xml;type=entry";
            }
            else
            {
                return "application/atom+xml;type=feed";
            }
        }

        public bool IsNavigationProperty(string typeName, string propertyName)
        {
            ResourceProperty property = this.ResolveProperty(typeName, propertyName);
            return property != null && property.ResourceType.ResourceTypeKind == ResourceTypeKind.EntityType;
        }

        public bool IsCollectionNavigationProperty(string typeName, string propertyName)
        {
            ResourceProperty property = this.ResolveProperty(typeName, propertyName);
            return property != null && property.Kind == ResourcePropertyKind.ResourceSetReference;
        }

        public bool IsReferenceNavigationProperty(string typeName, string propertyName)
        {
            ResourceProperty property = this.ResolveProperty(typeName, propertyName);
            return property != null && property.Kind == ResourcePropertyKind.ResourceReference;
        }

        public string GetIdFromEditLink(string id, string editLink, string typeName)
        {
            if (id != null)
            {
                return id;
            }

            if (editLink.EndsWith(typeName))
            {
                return editLink.Substring(0, editLink.Length - typeName.Length - 1);
            }

            return editLink;
        }

        private ResourceProperty ResolveProperty(string typeName, string propertyName)
        {
            ResourceType resourceType;
            ResourceProperty property = null;
            if (this.metadataProvider.TryResolveResourceType(typeName, out resourceType))
            {
                property = resourceType.Properties.FirstOrDefault(p => p.Name == propertyName);
            }

            return property;
            
        }
    }
}
