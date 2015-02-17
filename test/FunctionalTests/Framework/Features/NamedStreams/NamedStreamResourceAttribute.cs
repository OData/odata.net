//---------------------------------------------------------------------
// <copyright file="NamedStreamResourceAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Xml;
    using System.Collections.Generic;
    using System.Linq;

    public class NamedStreamResourceAttribute : ResourceAttribute
    {
        private ResourceType type;
        private string name;

        public NamedStreamResourceAttribute(ResourceType type, string name)
            : base("NamedStream")
        {
            this.type = type;
            this.name = name;
            this.orderedParams = new List<string>() { '"' + name + '"' };
            type.Facets.NamedStreams.Add(name);

            if (type.HasDerivedTypes)
            {
                foreach (ResourceType derived in type.DerivedTypes)
                {
                    derived.Facets.NamedStreams.Add(name);
                }
            }
        }

        //---------------------------------------------------------------------
        // Modifies CSDL.
        //---------------------------------------------------------------------
        public override void Apply(XmlDocument csdl)
        {
            throw new NotSupportedException("Named streams aren't supported on EF in the old framework");
        }
    }
}
