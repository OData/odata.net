//---------------------------------------------------------------------
// <copyright file="BlobsAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Xml;

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    public class BlobsAttribute : ResourceAttribute
    {
        private ResourceType type;

        //---------------------------------------------------------------------
        // Initializes blob attribute for resource type.
        //---------------------------------------------------------------------
        public BlobsAttribute(ResourceType type) : base("HasStream")
        {
            this.type = type;
            SetStreamFacet(type);
        }

        //---------------------------------------------------------------------
        // Recursively sets stream facets.
        //---------------------------------------------------------------------
        private void SetStreamFacet(ResourceType type)
        {
            type.Facets.HasStream = true;
            foreach (ResourceType derived in type.DerivedTypes)
                SetStreamFacet(derived);
        }

        //---------------------------------------------------------------------
        // Modifies CSDL.
        //---------------------------------------------------------------------
        public override void Apply(XmlDocument csdl)
        {
            // Set HasStream=$true.
            XmlElement entityType = TestUtil.AssertSelectSingleElement(csdl, string.Format("//csdl:EntityType[@Name='{0}']", type.Name));
            //entityType.SetAttribute("HasStream", AstoriaUnitTests.Data.XmlData.DataWebEdmNamespace, "true");
            entityType.SetAttribute("HasStream", AstoriaUnitTests.Data.XmlData.DataWebMetadataNamespace, "true");
        }
    }
}
