//---------------------------------------------------------------------
// <copyright file="StandardRequests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    public class StandardRequests
    {
        public enum UriTypeFacets
        {
            None = 0,
            ServiceRoot = 1,
            Metadata = 2,
            EntitySet = 4,
            SingleEntity = 8,
            ComplexType = 16,
            PrimitiveProperty = 32,
            RawValue = 64,
            ServiceOperation = 128,
            TopLevel = 256,
            CollectionProperty = 512,
            ReferenceProperty = 1024,
            Count = 2048,
            Links = 4096
        }

        public enum UriType
        {
            ServiceRoot = UriTypeFacets.ServiceRoot,
            Metadata = UriTypeFacets.Metadata,

            EntitySetTopLevel = UriTypeFacets.EntitySet | UriTypeFacets.TopLevel,
            EntitySetNavProp = UriTypeFacets.EntitySet | UriTypeFacets.CollectionProperty,
            EntitySetServiceOp = UriTypeFacets.EntitySet | UriTypeFacets.ServiceOperation,
            EntitySetTopLevelCount = UriTypeFacets.EntitySet | UriTypeFacets.TopLevel | UriTypeFacets.Count,
            EntitySetNavPropCount = UriTypeFacets.EntitySet | UriTypeFacets.CollectionProperty | UriTypeFacets.Count,

            SingleEntityKeyedTopLevel = UriTypeFacets.SingleEntity | UriTypeFacets.TopLevel,              // Customers(1)
            SingleEntityKeyedNavProp = UriTypeFacets.SingleEntity | UriTypeFacets.CollectionProperty,     // Customers(1)/Orders(2)
            SingleEntityReferenceProp = UriTypeFacets.SingleEntity | UriTypeFacets.ReferenceProperty,     // Orders(1)/Customer

            ComplexProperty = UriTypeFacets.ComplexType,
            PrimitiveProperty = UriTypeFacets.PrimitiveProperty,
            PrimitivePropertyValue = UriTypeFacets.PrimitiveProperty | UriTypeFacets.RawValue,
            Links = UriTypeFacets.Links
        }

        internal static bool UriTypeHasFacet(UriType type, UriTypeFacets facet)
        {
            return ((uint)type & (uint)facet) != 0;
        }

        Workspace _workspace;

        public StandardRequests(Workspace workspace)
        {
            _workspace = workspace;
        }

        // ServiceRoot
        public AstoriaRequest ServiceRoot()
        {
            AstoriaRequest request = _workspace.CreateRequest();
            request.Accept = "*/*";

            return request;
        }

        // Metadata
        public AstoriaRequest Metadata()
        {
            AstoriaRequest request = _workspace.CreateRequest();
            request.URI += "/$metadata";
            request.Accept = "*/*";

            return request;
        }

        // EntitySetTopLevel
        public AstoriaRequest EntitySetTopLevel()
        {
            return EntitySetTopLevel(_workspace.ServiceContainer.ResourceContainers.Choose());
        }

        public AstoriaRequest EntitySetTopLevel(out ResourceContainer container)
        {
            container = _workspace.ServiceContainer.ResourceContainers.Choose();
            return EntitySetTopLevel(container);
        }

        public AstoriaRequest EntitySetTopLevel(ResourceContainer container)
        {
            ExpNode q = Query.From(
                Exp.Variable(container))
                .Select();

            return _workspace.CreateRequest(q);
        }

        // EntitySetTopLevelWithExpand
        public AstoriaRequest EntitySetTopLevelWithSingleExpand()
        {
            ResourceContainer container;
            ResourceProperty property;

            return EntitySetTopLevelWithSingleExpand(out container, out property);
        }

        public AstoriaRequest EntitySetTopLevelWithSingleExpand(out ResourceContainer container, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers
                .Where(c => c.ResourceTypes.Any(rt => rt.Properties.OfType<ResourceProperty>().Any(p => p.IsNavigation && p.Type is CollectionType))).Choose();

            var properties = container.BaseType.Properties.OfType<ResourceProperty>().
                    Where(p => p.IsNavigation);

            if (properties.Count() > 0)
            {
                property = properties.Choose();
                return EntitySetTopLevelWithSingleExpand(container, property);
            }
            else
            {
                container = null;
                property = null;
                return null;
            }
        }

        public AstoriaRequest EntitySetTopLevelWithSingleExpand(ResourceContainer container, ResourceProperty property)
        {
            PropertyExpression[] expandValues = new PropertyExpression[] { new PropertyExpression(property) };
            
            ExpNode q = Query.From(
                Exp.Variable(container))
                .Expand(expandValues)
                .Select();

            return _workspace.CreateRequest(q);
        }

        // EntitySetNavProp
        public AstoriaRequest EntitySetNavProp()
        {
            ResourceContainer container;
            KeyExpression keyExp;
            ResourceProperty property;

            return EntitySetNavProp(out container, out keyExp, out property);
        }

        public AstoriaRequest EntitySetNavProp(out ResourceContainer container, out KeyExpression keyExp, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers
                .Where(c => c.ResourceTypes.Any(rt => rt.Properties.OfType<ResourceProperty>().Any(p => p.IsNavigation && p.Type is CollectionType))).Choose();

            keyExp = _workspace.GetRandomExistingKey(container);
            property = container.BaseType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation && p.Type is CollectionType).Choose();

            return EntitySetNavProp(container, keyExp, property);
        }

        public AstoriaRequest EntitySetNavProp(ResourceContainer container, KeyExpression keyExp, ResourceProperty property)
        {
            ExpNode q = Query.From(
                Exp.Variable(container))
                .Where(keyExp)
                .Nav(property.Property());

            return _workspace.CreateRequest(q);
        }

        // EntitySetServiceOp
        public AstoriaRequest EntitySetServiceOp()
        {
            ResourceContainer container;

            return EntitySetServiceOp(out container);
        }

        public AstoriaRequest EntitySetServiceOp(out ResourceContainer container)
        {
            container = _workspace.ServiceContainer.ResourceContainers.Choose();

            AstoriaRequest request = _workspace.CreateRequest();
            request.URI += "Get" + container.Name;

            return request;
        }

        public AstoriaRequest EntitySetServiceOp(string serviceOpName)
        {
            AstoriaRequest request = _workspace.CreateRequest();
            request.URI += serviceOpName;

            return request;
        }

        // EntitySetNavPropCount 

        // EntitySetTopLevelCount 
        public AstoriaRequest EntitySetTopLevelCount()
        {
            ResourceContainer container;
            return EntitySetTopLevelCount(out container);
        }

        public AstoriaRequest EntitySetTopLevelCount(out ResourceContainer container)
        {
            container = _workspace.ServiceContainer.ResourceContainers.Choose();
            return EntitySetTopLevelCount(container);
        }

        public AstoriaRequest EntitySetTopLevelCount(ResourceContainer container)
        {
            ExpNode q = Query.From(
                    Exp.Variable(container))
                    .Count(true)
                    .Select();

            return _workspace.CreateRequest(q);
        }

        // SingleEntityKeyedTopLevel
        public AstoriaRequest SingleEntityKeyedTopLevel()
        {
            ResourceContainer container;
            KeyExpression keyExp;

            return SingleEntityKeyedTopLevel(out container, out keyExp);
        }

        public AstoriaRequest SingleEntityKeyedTopLevel(out ResourceContainer container, out KeyExpression keyExp)
        {
            container = _workspace.ServiceContainer.ResourceContainers.Choose();
            keyExp = _workspace.GetRandomExistingKey(container);

            return SingleEntityKeyedTopLevel(container, keyExp);
        }

        public AstoriaRequest SingleEntityKeyedTopLevel(ResourceContainer container, KeyExpression keyExp)
        {
            ExpNode q = Query.From(
                 Exp.Variable(container))
                 .Where(keyExp)
                 .Select();

            return _workspace.CreateRequest(q);
        }

        // SingleEntityKeyedNavProp

        // SingleEntityReferenceProp
        public AstoriaRequest SingleEntityKeyedNavProp()
        {
            ResourceContainer container;
            KeyExpression keyExp;
            ResourceProperty property;

            return SingleEntityKeyedNavProp(out container, out keyExp, out property);
        }

        public AstoriaRequest SingleEntityKeyedNavProp(out ResourceContainer container, out KeyExpression keyExp, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers
                .Where(c => c.ResourceTypes.Any(rt => rt.Properties.OfType<ResourceProperty>().Any(p => p.IsNavigation && !(p.Type is CollectionType)))).Choose();

            keyExp = _workspace.GetRandomExistingKey(container);
            property = container.BaseType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation && !(p.Type is CollectionType)).Choose();

            return SingleEntityKeyedNavProp(container, keyExp, property);
        }

        public AstoriaRequest SingleEntityKeyedNavProp(ResourceContainer container, KeyExpression keyExp, ResourceProperty property)
        {
            ExpNode q = Query.From(
                 Exp.Variable(container))
                 .Where(keyExp)
                 .Select(new PropertyExpression(property, false));

            return _workspace.CreateRequest(q);
        }

        // ComplexProperty
        public AstoriaRequest ComplexProperty()
        {
            ResourceContainer container;
            KeyExpression keyExp;
            ResourceProperty property;

            return ComplexProperty(out container, out keyExp, out property);
        }

        public AstoriaRequest ComplexProperty(out ResourceContainer container, out KeyExpression keyExp, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers
                .Where(c => c.ResourceTypes.Any(rt => rt.Properties.OfType<ResourceProperty>().Any(p => p.IsComplexType))).Choose();

            keyExp = _workspace.GetRandomExistingKey(container);
            property = container.BaseType.Properties.OfType<ResourceProperty>().Where(p => p.IsComplexType).Choose();

            return ComplexProperty(container, keyExp, property);
        }

        public AstoriaRequest ComplexProperty(ResourceContainer container, KeyExpression keyExp, ResourceProperty property)
        {
            ExpNode q = Query.From(
                 Exp.Variable(container))
                 .Where(keyExp)
                 .Select(new PropertyExpression(property, false));

            return _workspace.CreateRequest(q);
        }

        // PrimitiveProperty
        public AstoriaRequest PrimitiveProperty()
        {
            ResourceContainer container;
            KeyExpression keyExp;
            ResourceProperty property;

            return PrimitiveProperty(out container, out keyExp, out property);
        }

        public AstoriaRequest PrimitiveProperty(out ResourceContainer container, out KeyExpression keyExp, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers.Choose();
            keyExp = _workspace.GetRandomExistingKey(container);
            property = container.BaseType.Properties.OfType<ResourceProperty>().Where(p => !p.IsNavigation && !p.IsComplexType).Choose();

            return PrimitiveProperty(container, keyExp, property);
        }

        public AstoriaRequest PrimitiveProperty(ResourceContainer container, KeyExpression keyExp, ResourceProperty property)
        {
            ExpNode q = Query.From(
                 Exp.Variable(container))
                 .Where(keyExp)
                 .Select(new PropertyExpression(property, false));

            return _workspace.CreateRequest(q);
        }

        // PrimitivePropertyValue
        public AstoriaRequest PrimitivePropertyValue()
        {
            ResourceContainer container;
            KeyExpression keyExp;
            ResourceProperty property;

            return PrimitivePropertyValue(out container, out keyExp, out property);
        }

        public AstoriaRequest PrimitivePropertyValue(out ResourceContainer container, out KeyExpression keyExp, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers.Choose();
            keyExp = _workspace.GetRandomExistingKey(container);
            property = container.BaseType.Properties.OfType<ResourceProperty>().Where(p => !p.IsNavigation && !p.IsComplexType).Choose();

            return PrimitivePropertyValue(container, keyExp, property);
        }

        public AstoriaRequest PrimitivePropertyValue(ResourceContainer container, KeyExpression keyExp, ResourceProperty property)
        {
            ExpNode q = Query.From(
                 Exp.Variable(container))
                 .Where(keyExp)
                 .Select(new PropertyExpression(property, true));

            return _workspace.CreateRequest(q);
        }

        // Links
        public AstoriaRequest Links()
        {
            ResourceContainer container;
            KeyExpression keyExp;
            ResourceProperty property;

            return Links(out container, out keyExp, out property);
        }

        public AstoriaRequest Links(out ResourceContainer container, out KeyExpression keyExp, out ResourceProperty property)
        {
            container = _workspace.ServiceContainer.ResourceContainers
                .Where(c => c.ResourceTypes.Any(rt => rt.Properties.OfType<ResourceProperty>().Any(p => p.IsNavigation && p.Type is CollectionType))).Choose();

            keyExp = _workspace.GetRandomExistingKey(container);
            property = container.BaseType.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation && p.Type is CollectionType).Choose();

            return Links(container, keyExp, property);
        }

        public AstoriaRequest Links(ResourceContainer container, KeyExpression keyExp, ResourceProperty property)
        {
            ExpNode q = Query.From(
                Exp.Variable(container))
                .Where(keyExp)
                .Nav(property.Property(), true)
                .Select();

            return _workspace.CreateRequest(q);
        }

        public AstoriaRequest MakePostRequest(AstoriaRequest request, KeyedResourceInstance resourceInstance)
        {
            request.Verb = RequestVerb.Post;
            request.ContentType = "application/atom+xml";
            request.UpdateTree = resourceInstance;

            return request;
        }

        public AstoriaRequest MakeDeleteRequest(AstoriaRequest request)
        {
            request.Verb = RequestVerb.Delete;
            return request;
        }

        public AstoriaRequest MakeReplaceRequest(AstoriaRequest request, KeyedResourceInstance resourceInstance)
        {
            request.Verb = RequestVerb.Put;
            request.ContentType = "application/atom+xml";
            request.UpdateTree = resourceInstance;

            return request;
        }

        public AstoriaRequest MakeMergeRequest(AstoriaRequest request, KeyedResourceInstance resourceInstance)
        {
            request.Verb = RequestVerb.Patch;
            request.ContentType = "application/atom+xml";
            request.UpdateTree = resourceInstance;

            return request;
        }
    }
}
