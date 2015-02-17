//---------------------------------------------------------------------
// <copyright file="QueryWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#if !ClientSKUFramework
using Microsoft.OData.Service.Providers;
#endif

using System.Reflection;

namespace System.Data.Test.Astoria
{
    //---------------------------------------------------------------------
    public class QueryWorkspaces : FeatureWorkspaces
    {
        protected override bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            // Skip workspace creation for service versions older than V2.
            if (!Versioning.Server.SupportsV2Features)
                return false;

            return attribute.Name == "Aruba";
        }

        private Workspace _w = null;

        protected override void WorkspaceCallback(Workspace w)
        {
            // Turn some entities into MLEs.
            MakeMLE(w, "Project");

            _w = w;
            w.BeforeServiceCreation.Insert(w.BeforeServiceCreation.IndexOf(w.PopulateHostSourceFolder) + 1, this.CreateDataStreamProviderCode);
        }

        private void MakeMLE(Workspace w, string typeName)
        {
            ResourceType type = w.ServiceContainer.ResourceTypes.Where(p => p.Name == typeName).First();
            type.Facets.Add(NodeFacet.Attribute(new BlobsAttribute(type)));
        }

        public void CreateDataStreamProviderCode()
        {
            Assembly resourceAssembly = this.GetType().Assembly;

            string serviceSourceCodePath = Path.Combine(_w.WebServiceWorkspaceDir, "App_Code\\IDataStreamProviderImplementation.cs");
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, "SimpleDataStreamProvider.cs", serviceSourceCodePath);

            string sourceText = File.ReadAllText(serviceSourceCodePath);
            sourceText = sourceText.Replace("[[Usings]]", _w.BuildDataServiceClassUsings());
            sourceText = sourceText.Replace("[[ContextTypeName]]", _w.ContextTypeName);
            sourceText = sourceText.Replace("[[ContextNamespace]]", _w.ContextNamespace);
            File.WriteAllText(serviceSourceCodePath, sourceText);
        }
    }

    
}
