//---------------------------------------------------------------------
// <copyright file="PicturesTagsWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace System.Data.Test.Astoria
{
    public class PictureTagsWorkspaces : FeatureWorkspaces
    {
        // Filters unsupported workspaces out.
        protected override bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            // Do not create services older than V2.
            if (!Versioning.Server.SupportsV2Features)
                return false;

            return DataLayerProviderKind.Edm == attribute.DataLayerProviderKind && attribute.Name == "PicturesTags";
        }

        // Turns a resource type into an MLE.
        private static void MakeMLE(Workspace w, string typeName)
        {
            // Add blob attribute.
            ResourceType type = w.ServiceContainer.ResourceTypes.Where(p => p.Name == typeName).First();
            type.Facets.Add(NodeFacet.Attribute(new BlobsAttribute(type)));
            AstoriaTestLog.WriteLineIgnore("MLE type: " + type.Name);
        }

        // Creates MLEs and generates server-side code.
        protected override void WorkspaceCallback(Workspace w)
        {
            // Turn some entities into MLEs.
            MakeMLE(w, "InternalPicture");
            MakeMLE(w, "ExternalPicture");

            // Read additional code from resources.
            string resPath = "Microsoft.Data.Test.Features.Blobs.Client.Resources.PicturesTagsEdm.res.cs";

            using (StreamReader reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(resPath), true))
            {
                w.GlobalAdditionalCode = reader.ReadToEnd();
            }

            w.GlobalAdditionalCode = w.GlobalAdditionalCode.Replace("ContextNamespace", w.ContextNamespace).Replace("ContextTypeName", w.ContextTypeName).Replace("DataServiceClass", w.ServiceClassName);

            // Dump additional code for easier lab run investigations.
            if (AstoriaTestProperties.IsLabRun)
            {
                AstoriaTestLog.WriteLineIgnore("Global additional code for workspace " + w.Name + ":");
                AstoriaTestLog.WriteLineIgnore(w.GlobalAdditionalCode);
            }
        }
    }
}
