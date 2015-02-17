//---------------------------------------------------------------------
// <copyright file="OpenTypeResourceAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace System.Data.Test.Astoria
{
    public class OpenTypeResourceAttribute : ResourceAttribute
    {
        public OpenTypeResourceAttribute(ResourceType type, string propertyName, Func<ResourceProperty, bool> declaredPropertyFilter)
            : base("OpenType")
        {
            this.orderedParams = new List<string>() { "\"" + propertyName + "\"" };
            this.PropertyName = propertyName;

            type.Facets.Add(NodeFacet.IsOpenType());
            foreach (ResourceProperty property in type.Properties.OfType<ResourceProperty>())
            {
                if (!declaredPropertyFilter(property))
                    property.Facets.IsDeclaredProperty = false;
            }
        }

        public OpenTypeResourceAttribute(ResourceType type, string propertyName, IEnumerable<string> declaredPropertyNames)
            : this(type, propertyName, (rp => declaredPropertyNames.Contains(rp.Name)))
        { }

        public OpenTypeResourceAttribute(ResourceType type, string propertyName, params string[] declaredPropertyNames)
            : this(type, propertyName, declaredPropertyNames.AsEnumerable())
        { }

        public OpenTypeResourceAttribute(ResourceType type, Func<ResourceProperty, bool> declaredPropertyFilter)
            : this(type, null, declaredPropertyFilter)
        { }

        public OpenTypeResourceAttribute(ResourceType type)
            : this(type, null, (rp => true))
        { }
        
        public override void Apply(XmlDocument csdl)
        {
            AstoriaTestLog.FailAndThrow("Open types not supported for EDM");
        }

        public string PropertyName
        {
            get;
            private set;
        }
    }
}
