//---------------------------------------------------------------------
// <copyright file="ResouceNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;       //IEnumerable<T>


namespace System.Data.Test.Astoria
{

    ////////////////////////////////////////////////////////
    // EdmContainer
    //
    ////////////////////////////////////////////////////////   
    public class ServiceContainer : NodeSet<ResourceContainer>
    {
        private class TypeComparer : IEqualityComparer<ComplexType>
        {
            #region IEqualityComparer<ComplexType> Members
            private string GetFullName(ComplexType t)
            {
                if (t.Namespace == null)
                    return t.Name;
                return t.Namespace + "." + t.Name;
            }

            public bool Equals(ComplexType x, ComplexType y)
            {
                return GetFullName(x).Equals(GetFullName(y));
            }

            public int GetHashCode(ComplexType obj)
            {
                return GetFullName(obj).GetHashCode();
            }
            #endregion
        }

        HashSet<ComplexType> types = new HashSet<ComplexType>(new TypeComparer());

        //Constructor
        public ServiceContainer(String name, Workspace workspace, params ResourceContainer[] resourceContainers)
            : base(name, resourceContainers)
        {
            Workspace = workspace;
            _desc = "Container";
        }

        public virtual IEnumerable<ServiceOperation> ServiceOperations
        {
            get
            {
                return _nodes.OfType<ServiceOperation>();
            }
        }

        public virtual Nodes<ResourceContainer> ResourceContainers
        {
            get { return _nodes; }
        }

        public virtual IEnumerable<ResourceType> ResourceTypes
        {
            get { return types.OfType<ResourceType>(); }
        }

        public virtual IEnumerable<ComplexType> ComplexTypes
        {
            get { return types.Where(t => !(t is ResourceType)); }
        }

        public virtual IEnumerable<ComplexType> AllTypes
        {
            get { return types.AsEnumerable(); }
        }

        public override void AddNode(ResourceContainer node)
        {
            base.AddNode(node);
            foreach (ResourceType type in node.ResourceTypes)
                this.AddNode(type);
        }

        public override void RemoveNode(ResourceContainer node)
        {
            base.RemoveNode(node);
            types.Clear();
            foreach (ResourceContainer container in ResourceContainers)
            {
                foreach (ResourceType type in container.ResourceTypes)
                    AddNode(type);
            }
        }

        public void AddNode(ComplexType type)
        {
            // add base types first
            if (type.BaseType != null)
                AddNode(type.BaseType);

            // if the type is already known, don't recurse into it
            if (!types.Add(type))
                return;

            foreach (ResourceProperty property in type.Properties.OfType<ResourceProperty>())
            {
                if (property.IsComplexType)
                    this.AddNode((ComplexType)property.Type);
            }
        }

        public virtual void AddNodes(IEnumerable<ServiceOperation> serviceOps)
        {
            this.AddNodes(serviceOps.Cast<ResourceContainer>());
        }

        public Workspace Workspace
        {
            get;
            private set;
        }
    }

    public enum ServiceOpBackingType
    {
        EntitySet,
        PrimitiveType,
        ComplexType,
        Void
    }
    public class ServiceOperation : ResourceContainer
    {
        public ServiceOperation(String name, ResourceContainer container, ResourceType baseType, params Node[] nodes)
            : base(name, baseType, nodes)
        {
            this.Container = container;
            this.BackingType = ServiceOpBackingType.EntitySet;
        }

        public int ResultLimit { get; set; }
        public ServiceOpBackingType BackingType { get; set; }
        public List<KeyValuePair<string, Type>> InputParameters { get; set; }
#if !ClientSKUFramework
        public Microsoft.OData.Service.Providers.ServiceOperationResultKind? ServiceOperationResultKind { get; set; }
#endif

        private RequestVerb _verb = RequestVerb.Get;
        public RequestVerb Verb
        {
            get
            {
                return _verb;
            }
            set
            {
                _verb = value;
            }
        }
        public object PrimitiveTypeData { get; set; }
        public object ComplexTypeData { get; set; }
        public Type ClrTypeOfExpression { get; set; }
        public Type ClrComplexTypeOfExpression { get; set; }
        private Func<Type, System.Linq.Expressions.Expression> getResourceTypeExpression;
        private Func<Type, Type, System.Linq.Expressions.Expression> getComplexTypeExpression;
        public void SetQueryExpressionLambda(Func<Type, System.Linq.Expressions.Expression> typeResolvedExpression)
        {
            getResourceTypeExpression = typeResolvedExpression;
        }
        public void SetComplexTypeQueryExpressionLambda(Func<Type, Type, System.Linq.Expressions.Expression> typeResolvedExpression)
        {
            getComplexTypeExpression = typeResolvedExpression;
        }
        public string ServiceOpCode
        {
            get;
            set;
        }
        public ResourceContainer Container { get; set; }
        public bool HasQueryExpression
        {
            get
            {
                return getResourceTypeExpression != null || getComplexTypeExpression != null;
            }
        }
        public string ExpectedTypeName { get; set; }
        public string ExpectedComplexPropertyName { get; set; }
        private System.Linq.Expressions.Expression _queryExpression;
        public System.Linq.Expressions.Expression QueryExpression
        {
            get
            {
                {
                    if (getResourceTypeExpression != null)
                        return getResourceTypeExpression(ClrTypeOfExpression);
                    if (getComplexTypeExpression != null)
                    {
                        return getComplexTypeExpression(ClrTypeOfExpression, ClrComplexTypeOfExpression);
                    }
                    else return null;
                }
            }

        }
    }
    ////////////////////////////////////////////////////////
    // EdmEntitySet
    //
    ////////////////////////////////////////////////////////   
    public class ResourceContainer : NodeSet<ResourceType>
    {
        //Data
        private fxList<ResourceAssociationSet> _sets = null;
        private fxList<ResourceAssociationSet> _initial = null;
        private ResourceType _baseType;
        private IEnumerable<ResourceType> _derivedTypes;
        //Constructor
        public ResourceContainer(String name, ResourceType baseType, params Node[] nodes)
            : base(name, nodes)
        {
            _baseType = baseType;
            if (nodes != null)
            {
                _derivedTypes = nodes.OfType<ResourceType>().Where(rt => rt.BaseTypes.Contains(baseType));
                _initial = new fxList<ResourceAssociationSet>(nodes.OfType<ResourceAssociationSet>().ToArray());
            }
            _desc = "ResourceContainer";
        }
        public bool HasInterceptorExpression
        {
            get
            {
                return _queryExpressionFunc != null;
            }
        }
        public string QueryInterceptorExpectedTypeName { get; set; }
        private Func<Type, System.Linq.Expressions.Expression> _queryExpressionFunc;
        public void SetInterceptorExpression(Func<Type, System.Linq.Expressions.Expression> queryExpressionFunc)
        {
            _queryExpressionFunc = queryExpressionFunc;
        }
        public Type CLRTypeOfExpression { get; set; }
        public System.Linq.Expressions.Expression InterceptorExpression
        {
            get
            {
                if (HasInterceptorExpression)
                {
                    return _queryExpressionFunc(CLRTypeOfExpression);
                }
                else
                {
                    return null;
                }
            }
        }
        public IEnumerable<ResourceAssociationSet> ResourceAssociationSets
        {
            get
            {
                if (_sets == null)
                {
                    _sets = new fxList<ResourceAssociationSet>();
                    foreach (ResourceType t in this.ResourceTypes)
                    {
                        foreach (ResourceProperty navProperty in t.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation))
                        {
                            ResourceAssociation resourceAssociation = navProperty.ResourceAssociation;
                            //If association is already in there it has been processed
                            if (_sets.Where(associationSets => associationSets.ResourceAssociation.Equals(resourceAssociation)).Count() > 1)
                                continue;
                            IEnumerable<ResourceAssociationSet> preCreatedSets = _initial.Where(resSet => resSet.ResourceAssociation.Equals(resourceAssociation));
                            if (preCreatedSets.Count() > 0)
                            {
                                _sets.Add(preCreatedSets);
                            }
                            else
                            {
                                ResourceAssociationEnd otherEnd = resourceAssociation.GetOtherEnd(navProperty);
                                IEnumerable<ResourceContainer> containers = this.Workspace.ServiceContainer.ResourceContainers.Where(rc => rc.ResourceTypes.Contains(otherEnd.ResourceType));
                                if (containers.Count() > 1)
                                    throw new Microsoft.Test.ModuleCore.TestFailedException("Unable to determine through which AssociationSet ResourceType:" + otherEnd.ResourceType.Name + " is related to, there is more than one");
                                //Make it automatically
                                _sets.Add(Resource.AssociationSet(resourceAssociation.Name + "Set", resourceAssociation, this, containers.First()));
                            }
                        }
                    }
                }
                return _sets;
            }
        }
        //Accessors
        public virtual ServiceContainer ServiceContainer
        {
            get { return (ServiceContainer)this.Parent; }
        }

        public IEnumerable<ResourceType> ResourceTypes
        {
            get
            {
                fxList<ResourceType> resourceTypes = new fxList<ResourceType>();
                if (this.BaseType != null) resourceTypes.Add(this.BaseType);
                if (this._derivedTypes != null) resourceTypes.Add(_derivedTypes);
                return resourceTypes;
            }
        }

        private ResourceContainer FindDefaultRelatedContainer(ResourceType resourceType, ResourceProperty property)
        {
            IEnumerable<ResourceContainer> containers;

            if ((resourceType == null) && (property == null))
                throw new ArgumentNullException();
            else if (resourceType == null && property != null)
            {
                if (property.Type is CollectionType)
                    containers = Workspace.ServiceContainer.ResourceContainers.Where(rc => rc.ResourceTypes.Contains((property.Type as CollectionType).SubType as ResourceType));
                else
                    containers = Workspace.ServiceContainer.ResourceContainers.Where(rc => rc.ResourceTypes.Contains(property.Type as ResourceType));
            }
            else if (resourceType != null && property == null)
            {
                containers = Workspace.ServiceContainer.ResourceContainers.Where(rc => rc.ResourceTypes.Contains(resourceType));
            }
            else
            {
                return null;
            }

            if (this.Facets.MestTag != null)
                return containers.Single(rc => rc.Facets.MestTag == this.Facets.MestTag);
            else if (property != null && property.Facets.MestTag != null)
                return containers.Single(rc => rc.Facets.MestTag == property.Facets.MestTag);
            else
                return containers.First(); //TODO: this could still be wrong if the MEST tag isnt set
        }

        public ResourceContainer FindDefaultRelatedContainer(ResourceType resourceType)
        {
            return FindDefaultRelatedContainer(resourceType, null);
        }

        public ResourceContainer FindDefaultRelatedContainer(ResourceProperty resourceProperty)
        {
            return FindDefaultRelatedContainer(null, resourceProperty);
        }

        public virtual ResourceType BaseType
        {
            get { return _baseType; }
        }

        public Workspace Workspace
        {
            get
            {
                return this.ServiceContainer.Workspace;
            }
        }
    }

    ////////////////////////////////////////////////////////
    // EdmProperty
    //
    ////////////////////////////////////////////////////////   
    public class ResourceProperty : NodeProperty
    {
        //Data
        private ResourceAssociationEnd _fromEnd;
        private ResourceAssociationEnd _toEnd;

        //Constructor
        public ResourceProperty(String name, NodeType type, params Node[] facets)
            : base(name, type, facets)
        {
        }

        public virtual bool IsNavigation
        {
            get { return this.Type is ResourceType || this.Type is CollectionType; }
        }

        public ResourceProperty OtherSideNavigationProperty
        {
            get
            {
                return this.OtherAssociationEnd.ResourceType.Properties.OfType<ResourceProperty>()
                    .SingleOrDefault(p => p != this && p.IsNavigation && p.ResourceAssociation.Name == this.ResourceAssociation.Name);
            }
        }

        public ResourceAssociation ResourceAssociation
        {
            get
            {
                if (_fromEnd != null)
                {
                    return this.ResourceType.Associations.OfType<ResourceAssociation>().Where(ra => ra.Ends.Contains(_fromEnd)).First();
                }
                else
                {
                    IEnumerable<ResourceAssociation> resourceAssociations = this.ResourceType.Associations.OfType<ResourceAssociation>();
                    //Find the association for that has as its end this property
                    ResourceAssociation associationWithEnd = null;
                    foreach (ResourceAssociation resourceAssociation in resourceAssociations)
                    {
                        int numberEndsWithPropertyName = resourceAssociation.Ends.Where(re => re.Name.Equals(this.Name)).Count();
                        if (numberEndsWithPropertyName > 0)
                        {
                            associationWithEnd = resourceAssociation;
                            break;
                        }
                    }
                    return associationWithEnd;
                }
            }
        }
        public ResourceAssociationEnd AssociationEnd
        {
            get
            {
                if (_fromEnd != null)
                    return _fromEnd;
                else
                {
                    return this.ResourceAssociation.Ends.Where(end => end.Name.Equals(this.Name)).Single();
                }
            }
        }
        public ResourceAssociationEnd OtherAssociationEnd
        {
            get
            {
                if (_toEnd != null)
                    return _toEnd;
                else
                {
                    IEnumerable<ResourceAssociationEnd> otherEnds = this.ResourceAssociation.Ends.Where(end => !end.Equals(this.AssociationEnd));
                    return otherEnds.First();
                }
            }
        }
        public virtual bool IsComplexType
        {
            get { return (this.Type is ComplexType && !(this.Type is ResourceType)); }
        }
        public virtual ResourceType ResourceType
        {
            get { return (ResourceType)this.Parent; }
        }
        internal void SetFromAndToAssociationEnds(ResourceAssociationEnd fromEnd, ResourceAssociationEnd toEnd)
        {
            _fromEnd = fromEnd;
            _toEnd = toEnd;
        }
        public ResourceInstanceSimpleProperty CreateResourceSimpleInstanceProperty(NodeValue nodeValue)
        {
            if (this.IsNavigation == true)
                throw new InvalidOperationException("Needs to be a simple property");
            ResourceInstanceSimpleProperty resourceInstanceSimpleProperty = new ResourceInstanceSimpleProperty(this.Name, nodeValue);
            return resourceInstanceSimpleProperty;
        }
        public ResourceInstanceSimpleProperty CreateRandomResourceSimpleInstanceProperty()
        {
            return this.CreateResourceSimpleInstanceProperty((this.Type as PrimitiveType).CreateRandomValueForFacets(this.Facets));
        }

        public ResourceInstanceProperty CreateRandomResourceInstanceProperty()
        {
            ResourceInstanceProperty instanceProperty;
            if (this.IsComplexType)
            {
                ComplexType ct = (ComplexType)this.Type;
                if (ct.Facets.AbstractType)
                    ct = ct.DerivedTypes.Where(t => !t.Facets.AbstractType).Choose();

                ComplexResourceInstance cri = ResourceInstanceUtil.CreateComplexResourceInstance(ct);
                return instanceProperty = new ResourceInstanceComplexProperty(ct.Name, this.Name, cri);
            }
            else
                return this.CreateRandomResourceSimpleInstanceProperty();
        }
    }

    ////////////////////////////////////////////////////////
    // EdmCollection
    //
    ////////////////////////////////////////////////////////   
    public class ResourceCollection : CollectionType
    {
        //Data

        //Constructor
        public ResourceCollection(NodeType subtype, params NodeFacet[] facets)
            : base(subtype, facets)
        {
        }

        //Overrides
        public override NodeValue CreateValue()
        {
            //TODO:
            throw new NotImplementedException();
        }

        public override int Compare(Object x, Object y)
        {
            //TODO:
            throw new NotImplementedException();
        }
    }


    ////////////////////////////////////////////////////////
    // EdmAssociation
    //
    ////////////////////////////////////////////////////////   
    public class ResourceAssociation : NodeRelation
    {
        //Data

        //Constructor
        public ResourceAssociation(String name, params ResourceAssociationEnd[] ends)
            : base(name, ends)
        {
        }

        //Accessors
        public virtual IEnumerable<ResourceAssociationEnd> Ends
        {
            get { return _nodes.OfType<ResourceAssociationEnd>(); }
        }

        internal virtual ResourceAssociationEnd Source
        {
            get { return this.Ends.FirstOrDefault(); }
        }

        internal virtual ResourceAssociationEnd Target
        {
            get { return this.Ends.LastOrDefault(); }
        }
        public ResourceAssociationEnd GetEnd(ResourceProperty property)
        {
            return property.AssociationEnd;
        }
        public ResourceAssociationEnd GetOtherEnd(ResourceProperty property)
        {
            return property.OtherAssociationEnd;
        }
        public bool IsSelfAssociation
        {
            get
            {
                ResourceAssociationEnd[] ends = this.Ends.ToArray();
                if (ends.Length == 2)
                {
                    ResourceAssociationEnd end1 = ends[0];
                    ResourceAssociationEnd end2 = ends[1];
                    if (end1.ResourceType == end2.ResourceType)
                        return true;
                    else if (end1.ResourceType.BaseTypes.Contains(end2.ResourceType))
                        return true;
                    else if (end1.ResourceType.BaseTypes.Contains(end1.ResourceType))
                        return true;
                }
                return false;
            }
        }
        //Override
    }
    public class ResourceAssociations : fxList<ResourceAssociation>
    {

        public ResourceAssociations NonSelfAssociations
        {
            get
            {
                ResourceAssociations nonSelfAssociations = new ResourceAssociations();
                foreach (ResourceAssociation resourceAssociation in this)
                {
                    if (!resourceAssociation.IsSelfAssociation)
                        nonSelfAssociations.Add(resourceAssociation);
                }
                return nonSelfAssociations;
            }
        }
        public ResourceAssociations SelfAssociations
        {
            get
            {
                ResourceAssociations selfAssociations = new ResourceAssociations();
                foreach (ResourceAssociation resourceAssociation in this)
                {
                    if (resourceAssociation.IsSelfAssociation)
                        selfAssociations.Add(resourceAssociation);
                }
                return selfAssociations;
            }
        }
    }
    public class ResourceAssociationSet : Node
    {
        private ResourceContainer[] _containers;
        private ResourceAssociation _assocation;
        public ResourceAssociationSet(string name, ResourceAssociation association, params ResourceContainer[] containers)
            : base(name)
        {
            _containers = containers;
            _assocation = association;
        }
        public ResourceAssociation ResourceAssociation
        {
            get { return _assocation; }
        }
        public IEnumerable<ResourceContainer> Containers
        {
            get { return _containers.AsEnumerable(); }
        }
    }
    ////////////////////////////////////////////////////////
    // Multiplicity
    //
    ////////////////////////////////////////////////////////   
    public enum Multiplicity
    {
        Zero,       //0
        One,        //1
        Many,       //M
    }

    ////////////////////////////////////////////////////////
    // EdmAssociationEnd
    //
    ////////////////////////////////////////////////////////   
    public class ResourceAssociationEnd : Node
    {
        //Data
        protected Multiplicity? _multiplicity;
        protected ResourceType _resourceType;
        internal ResourceProperty _associatedProperty = null;
        //Constructor
        public ResourceAssociationEnd(String name, ResourceType resourceType, Multiplicity? multiplicity)
            : base(name)
        {
            _resourceType = resourceType;
            _multiplicity = multiplicity;
        }
        public bool IsRefConstraint { get; set; }
        //Accessors
        public virtual ResourceType ResourceType
        {
            get { return _resourceType; }
            set { _resourceType = value; }
        }

        public virtual Multiplicity? Multiplicity
        {
            get { return _multiplicity; }
            set { _multiplicity = value; }
        }

        //Override
        internal void SetAssociatedProperty(ResourceProperty resourceProperty)
        {
            _associatedProperty = resourceProperty;
        }
    }

}

