//---------------------------------------------------------------------
// <copyright file="ContainmentAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Collections;               //IEnumerator
using System.Data.Test.Astoria.ReflectionProvider;
using System.Xml;

namespace System.Data.Test.Astoria
{
    public class ContainmentAttribute : ResourceAttribute
    {
        public ResourceProperty ParentNavigationProperty
        {
            get;
            private set;
        }
        public ResourceContainer ParentContainer
        {
            get;
            private set;
        }
        public ResourceContainer ChildContainer
        {
            get;
            private set;
        }
        public Dictionary<string, string> KeyMapping
        {
            get;
            private set;
        }
        public bool Canonical
        {
            get;
            private set;
        }
        public Boolean TopLevelAccess
        {
            get;
            private set;
        }

        private ResourceProperty GetReferencedProperty(ResourceProperty prop)
        {
            switch (prop.ForeignKeys.Count())
            {
                case 0:
                    return prop;

                case 1:
                    ForeignKey fk = prop.ForeignKeys.First();

                    if (fk.PrimaryKey.Properties.Count() == 1)
                        return fk.PrimaryKey.Properties.Cast<ResourceProperty>().First();
                    break;
            }
            throw new Exception(String.Format("Property '{0}' does not map to a specific foreign property", prop));
        }

        private void InferProperties()
        {
            ParentNavigationProperty =
                (from p in ParentContainer.BaseType.Properties.OfType<ResourceProperty>()
                 where p.IsNavigation && p.OtherAssociationEnd.ResourceType == ChildContainer.BaseType
                 select p).FirstOrDefault();

            AstoriaTestLog.Compare(ParentNavigationProperty != null,
                "Failed to identify parent navigation property");

            // figure out keymapping
            // each property in the key of the parent and child COULD be a local or foreign key
            // if it is a foreign key, then it could point to 

            foreach (ResourceProperty c_prop in ChildContainer.BaseType.Key.Properties.Cast<ResourceProperty>())
            {
                ResourceProperty c_foreign = GetReferencedProperty(c_prop);
                foreach (ResourceProperty p_prop in ParentContainer.BaseType.Key.Properties.Cast<ResourceProperty>())
                {
                    ResourceProperty p_foreign = GetReferencedProperty(p_prop);
                    if (c_foreign == p_foreign)
                    {
                        KeyMapping.Add(p_prop.Name, c_prop.Name);
                        break;
                    }
                }
            }

            //AccessPathAttribute(string parentEntitySetName, 
            //                    string parentNavigationPropertyName, 
            //                    string childEntitySetName, 
            //                    string parentKeys, 
            //                    string childKeys)
            this.orderedParams.Add(String.Format("\"{0}\"", ParentContainer.Name));
            this.orderedParams.Add(String.Format("\"{0}\"", ParentNavigationProperty.Name));
            this.orderedParams.Add(String.Format("\"{0}\"", ChildContainer.Name));
            this.orderedParams.Add(String.Format("\"{0}\"", String.Join(",", KeyMapping.Keys.ToArray())));
            this.orderedParams.Add(String.Format("\"{0}\"", String.Join(",", KeyMapping.Values.ToArray())));
        }

        private ContainmentAttribute(bool canonical, bool topLevelAccess, ResourceContainer parent, ResourceContainer child)
            : base(canonical ? "CanonicalAccessPath" : "AccessPath")
        {
            parent.Workspace.Settings.HasContainment = true;

            this.ParentContainer = parent;
            this.ChildContainer = child;
            this.Canonical = canonical;
            this.TopLevelAccess = topLevelAccess;
            this.KeyMapping = new Dictionary<string, string>();
            this.InferProperties();

            if (canonical)
                this.namedParams.Add("TopLevelAccess", topLevelAccess.ToString().ToLowerInvariant());

            ParentNavigationProperty.Facets.Add(NodeFacet.CanonicalAccessPath(Canonical));
            ChildContainer.BaseType.Facets.Add(NodeFacet.TopLevelAccess(topLevelAccess));
        }

        public ContainmentAttribute(ResourceContainer parent, ResourceContainer child)
            : this(false, true, parent, child)
        { }

        public ContainmentAttribute(ResourceContainer parent, ResourceContainer child, bool topLevelAccess)
            : this(true, topLevelAccess, parent, child)
        { }

        private static readonly string csdlNamespace = "http://docs.oasis-open.org/odata/ns/metadata";
        public override void Apply(XmlDocument csdl)
        {
            if (Canonical)
            {
                TestUtil.AssertSelectSingleElement(csdl,
                    String.Format("//csdl:EntitySet[@Name='{0}']", ChildContainer.Name))
                    .SetAttribute("TopLevelAccess", csdlNamespace, TopLevelAccess.ToString());
            }

            TestUtil.AssertSelectSingleElement(csdl,
                String.Format("//csdl:EntityType[@Name='{0}']/csdl:NavigationProperty[@Name='{1}']",
                ParentContainer.BaseType.Name, ParentNavigationProperty.Name))
                .SetAttribute("AccessPath", csdlNamespace, Canonical ? "Canonical" : "NonCanonical");
        }
    }
}