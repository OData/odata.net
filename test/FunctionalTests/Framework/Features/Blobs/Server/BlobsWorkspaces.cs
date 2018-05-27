//---------------------------------------------------------------------
// <copyright file="BlobsWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if !ClientSKUFramework
using Microsoft.OData.Service.Providers;
#endif

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace System.Data.Test.Astoria
{
    //=====================================================================
    // BASE WORKSPACE
    //=====================================================================

    //---------------------------------------------------------------------
    // Creates workspace with blobs.
    //---------------------------------------------------------------------
    public partial class BlobsWorkspaces : FeatureWorkspaces
    {
        //---------------------------------------------------------------------
        // Filters unsupported workspaces out.
        //---------------------------------------------------------------------
        protected override bool WorkspacePredicate(WorkspaceAttribute w)
        {
            return w.Name == "Aruba" && w.DataLayerProviderKind != DataLayerProviderKind.LinqToSql && Versioning.Server.SupportsV2Features;
        }

        //---------------------------------------------------------------------
        // Customizes the workspace by enabling features.
        //---------------------------------------------------------------------
        protected override void WorkspaceCallback(Workspace w)
        {
            // Enable features.
            EnableServerGeneratedKeys(w);
            AddBlobsAttributes(w, "Project", "College", "GradStudent", "Vehicle");
            ImplementServiceProvider(w);
            ImplementStreamProvider(w);

            // Fixup and inject additional code.
            w.GlobalAdditionalCode = FixupAdditionalCode(w.GlobalAdditionalCode ?? "")
                .Replace("ContextNamespace", w.ContextNamespace)
                .Replace("ContextTypeName", w.ContextTypeName)
                .Replace("DataServiceClass", w.ServiceClassName);

            // Dump additional code for easier lab run investigations.
            if (AstoriaTestProperties.IsLabRun)
                AstoriaTestLog.WriteLineIgnore(w.GlobalAdditionalCode);
            w.GenerateClientTypes = true;
            w.PopulateClientTypes();
        }

        //---------------------------------------------------------------------
        // Adds blobs attributes to resource types.
        //---------------------------------------------------------------------
        protected virtual void AddBlobsAttributes(Workspace w, params string[] typeNames)
        {
            foreach (string typeName in typeNames)
            {
                ResourceType type = w.ServiceContainer.ResourceTypes.Where(t => t.Name == typeName).First();
                type.Facets.Add(NodeFacet.Attribute(new BlobsAttribute(type)));
            }
        }

        //---------------------------------------------------------------------
        // Adds IDataServiceProvider implementation.
        //---------------------------------------------------------------------
        protected virtual void ImplementServiceProvider(Workspace w)
        {
#if !ClientSKUFramework

            w.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(IDataServiceStreamProvider)] =
                "this";
#endif
        }

        //---------------------------------------------------------------------
        // Adds IDataServiceStreamProvider implementation.
        //---------------------------------------------------------------------
        protected virtual void ImplementStreamProvider(Workspace w)
        {
            InjectCode(w, "BlobsStream", "BlobsLogging", "BlobsStreamProvider", "BlobsInterceptors", "BlobsServiceOps");
        }

        //---------------------------------------------------------------------
        // Works around deficiencies in our IUpdatable implementation for InMemoryLinq and NonClr.
        //---------------------------------------------------------------------
        protected virtual void EnableServerGeneratedKeys(Workspace w)
        {
            // Edm does not suffer from this issue.
            if (w.DataLayerProviderKind == DataLayerProviderKind.Edm)
                return;

            // Make all Int32 key properties auto-incrementing.
            foreach (ResourceType type in w.ServiceContainer.ResourceTypes)
                foreach (var property in type.Properties.Where(p => p.PrimaryKey != null && p.Type == Clr.Types.Int32))
                    property.Facets.ServerGenerated = true;
        }

        //---------------------------------------------------------------------
        // Adds code from embedded resources to data service source file.
        //---------------------------------------------------------------------
        protected void InjectCode(Workspace w, params string[] resourceNames)
        {
            foreach (string resourceName in resourceNames)
            {
                // Find embedded resource.
                bool found = false;
                foreach (string fullName in GetType().Assembly.GetManifestResourceNames())
                {
                    if (fullName.Contains(resourceName))
                    {
                        // Append contents to service code.
                        using (StreamReader reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(fullName), true))
                        {
                            w.GlobalAdditionalCode += reader.ReadToEnd();
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                    AstoriaTestLog.FailAndThrow("Test error: embedded resource not found!");
            }
        }

        //---------------------------------------------------------------------
        // Replaces existing method code with that from embedded resources.
        //---------------------------------------------------------------------
        protected string ReplaceCode(string existingCode, string methodRegex, string resourceName)
        {
            // Find embedded resource.
            foreach (string fullName in GetType().Assembly.GetManifestResourceNames())
            {
                if (fullName.Contains(resourceName))
                {
                    // Replace type resolver.
                    using (StreamReader reader = new StreamReader(GetType().Assembly.GetManifestResourceStream(fullName), true))
                    {
                        // Read new code from resources.
                        string newCode = reader.ReadToEnd();

                        // Find method signature in existing code and replace 
                        Regex method = new Regex(methodRegex, RegexOptions.Singleline);
                        if (method.Match(existingCode).Success)
                            return method.Replace(existingCode, newCode);

                        AstoriaTestLog.FailAndThrow("Test error: unable to match " + methodRegex + " in embedded code: " + existingCode);
                        return null;
                    }
                }
            }
            AstoriaTestLog.FailAndThrow("Test error: unable to find embedded resource: " + existingCode);
            return null;
        }

        //---------------------------------------------------------------------
        // Modifies code that is ready to be emitted.
        //---------------------------------------------------------------------
        protected virtual string FixupAdditionalCode(string code)
        {
            if (GetType().Name.Contains("Concurrency"))
            {
                // Replace default ETag handler with real one.
                return ReplaceCode(code,
                    @"string IDataServiceStreamProvider.GetStreamETag\(object entity, DataServiceOperationContext operationContext\).*?null.*?}",
                    "BlobsETags");
            }
            return code;
        }
    }

    //=====================================================================
    // DERIVED WORKSPACES
    //=====================================================================

    //---------------------------------------------------------------------
    // Creates workspace with ResolveType resolving to non-MLE types.
    //---------------------------------------------------------------------
    public class BlobsWrongResolveType1Workspaces : BlobsWorkspaces
    {
        protected override string FixupAdditionalCode(string code)
        {
            return ReplaceCode(base.FixupAdditionalCode(code),
                @"string IDataServiceStreamProvider.ResolveType\(string entitySetName, DataServiceOperationContext operationContext\).*?return.*?}",
                "BlobsWrongResolveType1");
        }
    }

    //---------------------------------------------------------------------
    // Creates workspace with ResolveType returning null.
    //---------------------------------------------------------------------
    public class BlobsWrongResolveType2Workspaces : BlobsWorkspaces
    {
        protected override string FixupAdditionalCode(string code)
        {
            return ReplaceCode(base.FixupAdditionalCode(code),
                @"string IDataServiceStreamProvider.ResolveType\(string entitySetName, DataServiceOperationContext operationContext\).*?return.*?}",
                "BlobsWrongResolveType2");
        }
    }

    //---------------------------------------------------------------------
    // Creates workspace with GetStreamContentType returning null or empty string.
    //---------------------------------------------------------------------
    public class BlobsWrongContentTypeWorkspaces : BlobsWorkspaces
    {
        protected override string FixupAdditionalCode(string code)
        {
            return ReplaceCode(base.FixupAdditionalCode(code),
                @"string IDataServiceStreamProvider.GetStreamContentType\(object entity, DataServiceOperationContext operationContext\).*?}",
                "BlobsWrongContentType");
        }
    }

    //---------------------------------------------------------------------
    // Creates workspace with GetReadStreamUri returning UriKind.Relative.
    //---------------------------------------------------------------------
    public class BlobsWrongStreamUriWorkspaces : BlobsWorkspaces
    {
        protected override string FixupAdditionalCode(string code)
        {
            return ReplaceCode(base.FixupAdditionalCode(code),
                @"Uri IDataServiceStreamProvider.GetReadStreamUri\(object entity, DataServiceOperationContext operationContext\).*?}",
                "BlobsWrongStreamUri");
        }
    }

    //---------------------------------------------------------------------
    // Creates workspace with StreamBufferSize returning 0.
    //---------------------------------------------------------------------
    public class BlobsWrongBufferSizeWorkspaces : BlobsWorkspaces
    {
        protected override string FixupAdditionalCode(string code)
        {
            // Replace default type resolver.
            return ReplaceCode(base.FixupAdditionalCode(code),
                @"int IDataServiceStreamProvider.StreamBufferSize.*?}",
                "BlobsWrongBufferSize");
        }
    }

    //---------------------------------------------------------------------
    // Creates workspace with no IDataServiceStreamProvider implemenentation.
    //---------------------------------------------------------------------
    public class BlobsNoStreamProviderWorkspaces : BlobsWorkspaces
    {
        // Emits no actual stream provider code.
        protected override void ImplementStreamProvider(Workspace w) { }
    }

    //---------------------------------------------------------------------
    // Creates workspace with FF mappings and/or concurrency attributes.
    //---------------------------------------------------------------------
    public class BlobsFriendlyFeedsWorkspaces : BlobsWorkspaces {}
    public class BlobsConcurrencyWorkspaces : BlobsWorkspaces {}
    public class BlobsFriendlyFeedsConcurrencyWorkspaces : BlobsWorkspaces {}
}
