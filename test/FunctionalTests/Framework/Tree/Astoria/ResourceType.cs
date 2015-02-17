//---------------------------------------------------------------------
// <copyright file="ResourceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Collections;               //IEnumerator


namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // EdmEntity
    //
    ////////////////////////////////////////////////////////   
    public class ResourceType : ComplexType
    {
        //Data
        private bool _isInsertable = true;
        private Workspace _workspace;
        //Constructor
        public ResourceType(String name, Type clrtype, params Node[] properties)
            : base(name, clrtype, properties)
        {
            if (clrtype == null)
                throw new ArgumentNullException("clrType", "Use other constructor and specify the namespace");
            _desc = "ResourceType";
            this.InferAssociations();
        }

        public ResourceType(String name, string typeNamespace, params Node[] properties)
            : base(name, typeNamespace, properties)
        {
            _desc = "ResourceType";
            this.InferAssociations();
        }
        public ResourceType(String name, string typeNamespace, ResourceType baseType, params Node[] properties)
            : base(name, typeNamespace, baseType, properties)
        {
            _desc = "ResourceType";
            this.Key = (base.BaseType as ResourceType).Key;
            this.InferAssociations();
        }

        public bool IsInsertable
        {
            get { return _isInsertable; }
            set { _isInsertable = value; }
        }

        public new IEnumerable<ResourceType> DerivedTypes
        {
            get
            {
                return base.DerivedTypes.OfType<ResourceType>();
            }
        }

        private List<ResourceAssociation> _resourceAssociation;
        public List<ResourceAssociation> Constraints
        {
            get { return _resourceAssociation; }
            set
            {
                _resourceAssociation = value;
            }
        }

        public override IEnumerable<ComplexType> BaseTypes
        {
            get
            {
                List<ComplexType> baseTypes = new List<ComplexType>();
                ComplexType currentType = this;
                while (currentType.BaseType != null)
                {
                    baseTypes.Add(currentType.BaseType);
                    currentType = currentType.BaseType;
                }
                return baseTypes;
            }
        }
        //Accessors
        /*
        public virtual ResourceContainer Container
        {
            get 
            {
                if (_resourceContainer == null)
                {
                    if (this.BaseType != null)
                        _resourceContainer=(this.BaseType as ResourceType).Container;
                }
                return _resourceContainer;   
            }
            set { _resourceContainer = value; }
        }
        */
        public virtual PrimaryKey Key
        {
            //Simple helper, otherwise use the relations collection direclty
            get { return this.Relations.PrimaryKey; }
            set { this.Relations.PrimaryKey = value; }
        }

        public virtual IEnumerable<ResourceAssociation> Associations
        {
            get
            {
                return this.Relations.OfType<ResourceAssociation>();
            }
        }
        public ResourceAssociations AssociationsRequired
        {
            get
            {
                ResourceAssociations requiredAssociations = new ResourceAssociations();
                requiredAssociations.Add(this.AssociationsOneToMany);
                requiredAssociations.Add(this.AssociationsOneToOne);
                requiredAssociations.Add(this.AssociationsZeroOneToOne);
                requiredAssociations.Add(this.AssociationsManyToOne);
                return requiredAssociations;
            }
        }
        public ResourceAssociations FindAssociationsOfMultiplicity(Multiplicity resourceTypeEndMul, Multiplicity otherEndMul)
        {
            ResourceAssociations associationsFoundWithMul = new ResourceAssociations();
            foreach (ResourceProperty prop in this.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation == true && p.Name != null))
            {
                if (prop.AssociationEnd.Multiplicity == resourceTypeEndMul && prop.OtherAssociationEnd.Multiplicity == otherEndMul)
                {
                    associationsFoundWithMul.Add(prop.ResourceAssociation);
                }
            }
            return associationsFoundWithMul;
        }
        public ResourceAssociations FindRequiredAssociations()
        {
            ResourceAssociations requiredAssociations = new ResourceAssociations();
            foreach (ResourceProperty prop in this.Properties.OfType<ResourceProperty>().Where(p => p.IsNavigation == true && p.Name != null))
            {
                if (prop.AssociationEnd.Multiplicity == Multiplicity.One)
                {
                    requiredAssociations.Add(prop.ResourceAssociation);
                }
            }
            return requiredAssociations;
        }
        public virtual ResourceAssociations AssociationsZeroOneToZeroOne
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.Zero, Multiplicity.Zero); }
        }
        public virtual ResourceAssociations AssociationsZeroOneToOne
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.Zero, Multiplicity.One); }
        }
        public virtual ResourceAssociations AssociationsZeroOneToMany
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.Zero, Multiplicity.Many); }
        }
        public virtual ResourceAssociations AssociationsOneToZeroOne
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.One, Multiplicity.Zero); }
        }
        public virtual ResourceAssociations AssociationsOneToOne
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.One, Multiplicity.One); }
        }
        public virtual ResourceAssociations AssociationsOneToMany
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.One, Multiplicity.Many); }
        }
        public virtual ResourceAssociations AssociationsManyToOne
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.Many, Multiplicity.One); }
        }
        public virtual ResourceAssociations AssociationsManyToZeroToOne
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.Many, Multiplicity.Zero); }
        }
        public virtual ResourceAssociations AssociationsManyToMany
        {
            get { return this.FindAssociationsOfMultiplicity(Multiplicity.Many, Multiplicity.Many); }
        }
        public bool IsChildRefEntity
        {
            get
            {
                int fkCount = 0;
                //If it has two Pks and both have FKS to other ones then it is
                foreach (ResourceProperty key in this.Properties.OfType<ResourceProperty>().Where(p => p.PrimaryKey != null))
                {
                    if (key.ForeignKeys.Count() > 0)
                    {
                        fkCount++;
                    }
                }
                if (fkCount > 0)
                    return true;
                return false;
            }
        }
        public bool IsAssociationEntity
        {
            get
            {
                int fkCount = 0;
                int keyCount = 0;
                //If it has two Pks and both have FKS to other ones then it is
                foreach (ResourceProperty key in this.Properties.OfType<ResourceProperty>().Where(p => p.PrimaryKey != null))
                {
                    keyCount++;
                    if (key.ForeignKeys.Count() > 0)
                    {
                        fkCount++;
                    }
                }
                if (fkCount > 1)
                {
                    if (keyCount == fkCount)
                        return true;
                }
                return false;
            }
        }
        public KeyedResourceInstance CreateRandomResource(ResourceContainer container)
        {
            return CreateRandomResource(container, true);
        }

        public KeyedResourceInstance CreateRandomResource(ResourceContainer container, bool populateNavProps)
        {
            return CreateRandomResource(container, null, populateNavProps);
        }

        public KeyedResourceInstance CreateRandomResource(ResourceContainer container, KeyExpressions existingKeys)
        {
            return CreateRandomResource(container, existingKeys, true);
        }

        internal KeyedResourceInstance CreateRandomResource(ResourceContainer container, KeyExpressions existingKeys, bool populateNavProps)
        {
            KeyExpressions relatedForeignKeys = new KeyExpressions();
            ResourceInstanceKey key = ResourceInstanceUtil.CreateUniqueKey(container, this, relatedForeignKeys, existingKeys);
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();

            // populate the non-key, non-navigation properties
            //
            foreach (ResourceProperty p in this.Properties.OfType<ResourceProperty>().Where(rp => rp.IsNavigation == false && rp.PrimaryKey == null))
            {
                if (p.Facets.IsIdentity)
                    continue;

                ResourceInstanceProperty property = p.CreateRandomResourceInstanceProperty();
                properties.Add(property);
            }

            if (populateNavProps)
            {
                // populate the navigation properties, but don't go by key, as some foreign keys MAY NOT HAVE ASSOCIATED NAVIGATION PROPERTIES
                //
                foreach (ResourceProperty p in this.Properties.OfType<ResourceProperty>().Where(rp => rp.IsNavigation))
                {
                    // find a key for this navigation property
                    KeyExpression navPropKey = null;
                    foreach (KeyExpression keyExp in relatedForeignKeys)
                    {
                        //if (p.Type.Equals(keyExp.ResourceType)
                        //    || p.Type is ResourceType && (p.Type as ResourceType).BaseTypes.Contains(keyExp.ResourceType))
                        if (p.OtherAssociationEnd.ResourceType.Equals(keyExp.ResourceType))
                        {
                            navPropKey = keyExp;
                            break;
                        }
                    }

                    ResourceContainer associatedContainer = container.FindDefaultRelatedContainer(p);
                    if (navPropKey == null)
                    {

                        if (p.OtherAssociationEnd.ResourceType.Key.Properties.OfType<ResourceProperty>()
                            .Where(rp => rp.ForeignKeys.Any())
                            .Any(rp => rp.ForeignKeys.First().PrimaryKey.Properties.OfType<ResourceProperty>().First().ResourceType.Equals(this)))
                        {
                            // this association has a fk back to the current type, so it cannot be based on any existing entity
                            AstoriaTestLog.WriteLineIgnore("Skipping nav prop '{0}.{1}' due to foreign key constraint on entity being created", this.Name, p.Name);
                            continue;
                        }
                        else
                            navPropKey = associatedContainer.Workspace.GetRandomExistingKey(associatedContainer, p.OtherAssociationEnd.ResourceType);
                    }

                    if (navPropKey != null)
                    {
                        if (p.OtherAssociationEnd.Multiplicity == Multiplicity.Many)
                            properties.Add(ResourceInstanceUtil.CreateCollectionInstanceProperty(new KeyExpressions(navPropKey), navPropKey.ResourceContainer, p));
                        else
                            properties.Add(ResourceInstanceUtil.CreateRefInstanceProperty(navPropKey, navPropKey.ResourceContainer, p));
                    }
                }
            }
            return ResourceInstanceUtil.CreateKeyedResourceInstance(key.CreateKeyExpression(container, this), container, properties.ToArray());
        }

        public KeyedResourceInstance CreateRandomResource(ResourceContainer container, KeyExpression specifiedKey)
        {
            List<ResourceInstanceProperty> properties = new List<ResourceInstanceProperty>();

            foreach (ResourceProperty p in this.Properties.OfType<ResourceProperty>().Where(rp => rp.IsNavigation == false && rp.PrimaryKey == null))
            {
                ResourceInstanceProperty property = p.CreateRandomResourceInstanceProperty();
                properties.Add(property);
            }

            return ResourceInstanceUtil.CreateKeyedResourceInstance(specifiedKey, container, properties.ToArray());
        }

        public virtual void InferAssociations()
        {
            //Pull off any assoications bound to the properties
            foreach (ResourceProperty p in this.Properties)
            {
                //Skip associations on properties on basetypes, we don't need duplicate associations
                /*if(this.BaseType!=null)
                {
                    int basePropertyCount = this.BaseType.Properties.OfType<ResourceProperty>().Where(baseProp => baseProp.Name.Equals(p.Name)).Count();
                    if (basePropertyCount > 0)
                        continue;
                }*/
                ResourceAssociation[] associations = p._other.OfType<ResourceAssociation>().ToArray();
                ResourceAssociationEnd[] associationEnds = p._other.OfType<ResourceAssociationEnd>().ToArray();
                ResourceAssociationEnd fromEnd = null;
                ResourceAssociationEnd toEnd = null;

                if (associationEnds.Count() == 2)
                {
                    fromEnd = associationEnds[0];
                    toEnd = associationEnds[1];
                }

                NodeType subtype = p.Type is CollectionType ? ((CollectionType)p.Type).SubType : p.Type;

                if (associations.Length == 0 && subtype is ResourceType)
                {
                    //Inferred Association (ie: Navigation property), with no mappings
                    //To specify mappings, explicitly use an association
                    this.Relations.Add(
                        Resource.Association(this.Name + p.Name, //AssociationName
                            Resource.End(this.Name, this, this.Facets.Nullable ? Multiplicity.Zero : Multiplicity.One),
                            Resource.End(p.Name, (ResourceType)subtype, p.Type is CollectionType ? Multiplicity.Many : (p.Facets.Nullable ? Multiplicity.Zero : Multiplicity.One))
                            )
                        );
                }
                else
                {
                    //Updated specfied with Associations, with any missing data
                    foreach (ResourceAssociation a in associations)
                    {
                        //Name (ie: Default)
                        if (a.Name == null)
                            a.Name = this.Name + p.Name;

                        //End1
                        ResourceAssociationEnd e1 = a.Source;
                        e1.ResourceType = e1.ResourceType ?? this;
                        e1.Name = e1.Name ?? this.Name;
                        e1.Multiplicity = e1.Multiplicity ?? (this.Facets.Nullable ? Multiplicity.Zero : Multiplicity.One);


                        //End2
                        ResourceAssociationEnd e2 = a.Target;
                        e2.ResourceType = e2.ResourceType ?? (ResourceType)subtype;
                        e2.Name = e2.Name ?? p.Name;
                        e2.Multiplicity = e2.Multiplicity ?? (p.Type is CollectionType ? Multiplicity.Many : (p.Facets.Nullable ? Multiplicity.Zero : Multiplicity.One));

                        this.Relations.Add(a);
                    }
                }
                if (fromEnd != null && toEnd != null)
                    p.SetFromAndToAssociationEnds(fromEnd, toEnd);
            }
        }
    }
}
