//---------------------------------------------------------------------
// <copyright file="Resource.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;


namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // ResExpHelper
    //
    ////////////////////////////////////////////////////////   
    public class Resource
    {
        public static ServiceContainer ServiceContainer(Workspace workspace, String name, params ResourceContainer[] resourceContainers)
        {
            return new ServiceContainer(name, workspace, resourceContainers);
        }
        public static ServiceOperation ServiceOperation(String name, ResourceContainer container, ResourceType baseType, List<KeyValuePair<string, Type>> InputParams, params ResourceType[] resourceTypes)
        {
            ServiceOperation serviceOp = new ServiceOperation(name, container, baseType, resourceTypes);
            serviceOp.InputParameters = InputParams;
            return serviceOp;
        }
        public static ServiceOperation ServiceOperation(String name, ResourceContainer container, ResourceType baseType, params ResourceType[] resourceTypes)
        {
            ServiceOperation serviceOp = new ServiceOperation(name, container, baseType, resourceTypes);
            return serviceOp;
        }
        public static ResourceContainer ResourceContainer(String name, ResourceType baseType, params ResourceType[] resourceTypes)
        {
            return new ResourceContainer(name, baseType, resourceTypes);
        }
        public static ResourceAssociationSet AssociationSet(String name, ResourceAssociation association, params ResourceContainer[] containers)
        {
            return new ResourceAssociationSet(name, association, containers);
        }
        public static ResourceType ResourceType(String name, string typeNamespace, params Node[] properties)
        {
            return new ResourceType(name, typeNamespace, properties);
        }
        public static ResourceType ResourceType(String name, string typeNamespace, ResourceType baseType, params Node[] properties)
        {
            return new ResourceType(name, typeNamespace, baseType, properties);
        }
        public static ResourceType ResourceType(String name, Type clrType, params Node[] properties)
        {
            return new ResourceType(name, clrType, properties);
        }
        public static ComplexType ComplexType(String name, string typeNamespace, params Node[] properties)
        {
            return new ComplexType(name, typeNamespace, properties);
        }
        public static PrimitiveOrComplexCollectionType CollectionType(NodeType subtype, params NodeFacet[] facets)
        {
            return new PrimitiveOrComplexCollectionType(subtype, facets);
        }

        public static ResourceProperty Property(String name, NodeType type, params Node[] facets)
        {
            return new ResourceProperty(name, type, facets);
        }

        public static ResourceAssociation Association(String name, params ResourceAssociationEnd[] ends)
        {
            return new ResourceAssociation(name, ends);
        }

        public static ResourceAssociationEnd End()
        {
            return new ResourceAssociationEnd(null, null, null);
        }

        public static ResourceAssociationEnd End(String name, ResourceType entity, Multiplicity multiplicity)
        {
            return new ResourceAssociationEnd(name, entity, multiplicity);
        }

        public static ExpNode Condition(ExpNode expression)
        {
            return expression;
        }

        public static BaseType BaseType(ResourceType basetype)
        {
            return new BaseType(basetype);
        }

        public static NodeType Collection(NodeType subtype)
        {
            return new ResourceCollection(subtype);
        }

        public static PrimaryKey Key()
        {
            return Resource.Key(null);   //Delegate
        }

        public static PrimaryKey Key(String name)
        {
            return new PrimaryKey(name);
        }
        public static ForeignKey ForeignKey(ResourceProperty primaryKeyProperty)
        {
            PrimaryKey pk = new PrimaryKey(null, primaryKeyProperty);
            return new ForeignKey(null, pk);
        }
        public static NodeValue CreateValue(ResourceProperty resourceProperty)
        {
            if (!(resourceProperty.Type is PrimitiveType))
                throw new ArgumentException("ResourceProperty must be a PrimitiveType", "resourceProperty");
            PrimitiveType pt = resourceProperty.Type as PrimitiveType;
            return pt.CreateRandomValueForFacets(resourceProperty.Facets);
        }
    }
}
