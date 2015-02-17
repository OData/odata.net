//---------------------------------------------------------------------
// <copyright file="ConcurrencyAttribute.cs" company="Microsoft">
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
    public class ConcurrencyAttribute : ResourceAttribute
    {
        public List<ResourceProperty> etagProperties;

        public ConcurrencyAttribute(ResourceType type, IEnumerable<string> propertyNames)
            : base("ETag")
        {
            etagProperties = type.Properties.OfType<ResourceProperty>().Where(p => propertyNames.Contains(p.Name)).ToList();
            etagProperties.ForEach(p => p.Facets.Add(NodeFacet.ConcurrencyModeFixed()));
            this.orderedParams = etagProperties.Select(p => "\"" + p.Name + "\"").ToList();
        }

        public ConcurrencyAttribute(ResourceType type, params string[] propertyNames)
            : this(type, propertyNames.AsEnumerable())
        { }

        public override void Apply(XmlDocument csdl)
        {
            foreach (ResourceProperty p in etagProperties)
            {
                TestUtil.AssertSelectSingleElement(csdl,
                    String.Format("//csdl:EntityType[@Name='{0}']/csdl:Property[@Name='{1}']",
                    p.ResourceType.Name, p.Name))
                    .SetAttribute("ConcurrencyMode", "Fixed");
            }
        }
    }
}
