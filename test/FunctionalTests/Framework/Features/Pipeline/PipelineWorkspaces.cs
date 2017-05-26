//---------------------------------------------------------------------
// <copyright file="PipelineWorkspaces.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Test.KoKoMo;

namespace System.Data.Test.Astoria
{
    using Microsoft.OData.Client;

    public class ProcessingPipelineWorkspaces : FeatureWorkspaces
    {
        public const string StreamETag = "streamETag";
        public static readonly object[] ContinuationToken = 
            { null, false, "pagingRules", 1.0f, 2.0d, 3, 4L, 5M, ((sbyte)6), ((byte)7), Guid.Empty, new DateTime(2009, 9, 29), new byte[] { 1, 2, 3}};
        public const string ContinuationTokenAsCode = "new object[] { null, false, \"pagingRules\", 1.0f, 2.0d, 3, 4L, 5M, ((sbyte)6), ((byte)7), Guid.Empty, new DateTime(2009, 9, 29), new byte[] { 1, 2, 3}}";

        public override bool IgnoreWorkspacePriority
        {
            get
            {
                return true;
            }
        }

        protected override bool WorkspacePredicate(WorkspaceAttribute attribute)
        {
            // only run on standard workspaces
            return attribute.Standard && attribute.Name.Equals("Aruba", StringComparison.InvariantCultureIgnoreCase);
        }

        protected override IEnumerable<Workspace> Construct(Type t)
        {
            List<Workspace> constructed = new List<Workspace>();
            Workspace w;

            w = (Workspace)Activator.CreateInstance(t);
            constructed.Add(w);

            if (AstoriaTestProperties.ResolveProperty("Pipeline-LimitWorkspaces") == null)
            {
                // add a workspace with the custom providers (paging/expand/update)
                if (typeof(EdmWorkspace).IsAssignableFrom(t))
                {
                    if (Versioning.Server.SupportsV2Features)
                    {
                        w = (Workspace)Activator.CreateInstance(t);
                        AddExpandProvider(w);
                        constructed.Add(w);
                    }
                }
                else
                {
                    w = (Workspace)Activator.CreateInstance(t);
                    if (Versioning.Server.SupportsV2Features)
                    {
                        AddUpdateProvider(w);
                        AddPagingProvider(w);
                    }

                    AddExpandProvider(w);
                    constructed.Add(w);
                }

                // set all workspaces so far to have max version
                constructed.ForEach(ws => ws.Settings.MaxProtocolVersion = ODataProtocolVersion.V4);
            }

            return constructed;
        }

        private void AddUpdateProvider(Workspace w)
        {
            w.Settings.UpdatableImplementation = UpdatableImplementation.DataServiceUpdateProvider;
            w.WebServiceName = w.WebServiceName + "_UP";
        }

        private void AddPagingProvider(Workspace w)
        {
            w.Settings.HasPagingProvider = true;
            w.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServicePagingProvider)]
                    = "new PagingProviderWrapper(new SimplePagingProvider())";

            w.GlobalAdditionalCode += string.Join(Environment.NewLine, new string[]
                {
                    "internal class SimplePagingProvider : IDataServicePagingProvider",
                    "{",
                    "   object[] IDataServicePagingProvider.GetContinuationToken(IEnumerator enumerator)",
                    "   {",
                    "       return " + ContinuationTokenAsCode + ";",
                    "   }",
                    "   void IDataServicePagingProvider.SetContinuationToken(IQueryable query, ResourceType resourceType, object[] continuationToken)",
                    "   {",
                    "       // do nothing",
                    "   }",
                    "}"
                });
            w.WebServiceName = w.WebServiceName + "_PP";
        }

        private void AddExpandProvider(Workspace w)
        {
            w.Settings.HasExpandProvider = true;
            w.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.IExpandProvider)]
                = "new ExpandProviderWrapper(new SimpleExpandProvider())";

            w.GlobalAdditionalCode += string.Join(Environment.NewLine, new string[]
                {
                    "internal class SimpleExpandProvider : IExpandProvider",
                    "{",
                    "   IEnumerable IExpandProvider.ApplyExpansions(IQueryable queryable, ICollection<ExpandSegmentCollection> expandPaths)",
                    "   {",
                    "       return queryable;",
                    "   }",
                    "}"
                });
            w.WebServiceName = w.WebServiceName + "_EP";
        }

        protected override void WorkspaceCallback(Workspace workspace)
        {
            Microsoft.OData.Client.ODataProtocolVersion maxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;
            if (workspace.Settings.MaxProtocolVersion.HasValue)
            {
                maxProtocolVersion = workspace.Settings.MaxProtocolVersion.Value;
            }
            
            workspace.GenerateCallOrderInterceptors = true;
            
            // use custom concurrency provider for workspaces that can
            if(workspace.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq)
            {
                if (workspace.Settings.UpdatableImplementation == UpdatableImplementation.DataServiceUpdateProvider)
                {
                    workspace.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider)]
                        = "new UpdateProviderWrapper(this.CurrentDataSource)";
                }
                else
                {
                    workspace.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.IUpdatable)]
                        = "new UpdatableWrapper(this.CurrentDataSource)";
                }
            }
            else if (workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr)
            {
                workspace.Settings.UpdatableImplementation = UpdatableImplementation.DataServiceUpdateProvider;
                workspace.Settings.UseLazyPropertyLoading = false;

                // do this NOW instead of later, so that we can refer to these settings
                (workspace as NonClrWorkspace).DefineClrProperties();
                if (AstoriaTestProperties.UseOpenTypes)
                {
                    OpenTypesUtil.SetupDefaultOpenTypeAttributes(workspace);
                }
            }

            workspace.ServiceContainer.ResourceContainers.Remove("Invoices");

            #region set up blobs
            if (Versioning.Server.SupportsLiveFeatures && workspace.DataLayerProviderKind != DataLayerProviderKind.LinqToSql)
            {    
                var edmWorkspace = workspace as EdmWorkspace;
                if (edmWorkspace != null)
                {
                    edmWorkspace.CsdlCallbacks.Add(delegate(XmlDocument doc)
                    {
                        doc.InnerXml = doc.InnerXml.Replace(TestUtil.TestNamespaceManager.LookupNamespace("csdl"), TestUtil.TestNamespaceManager.LookupNamespace("csdl2"));
                    });
                    edmWorkspace.SsdlCallbacks.Add(delegate(XmlDocument doc)
                    {
                        doc.InnerXml = doc.InnerXml.Replace("http://schemas.microsoft.com/ado/2006/04/edm/ssdl", "http://schemas.microsoft.com/ado/2009/02/edm/ssdl");
                    });
                    edmWorkspace.MslCallbacks.Add(delegate(XmlDocument doc)
                    {
                        doc.InnerXml = doc.InnerXml.Replace("urn:schemas-microsoft-com:windows:storage:mapping:CS", "http://schemas.microsoft.com/ado/2008/09/mapping/cs");
                    });  
                }

                foreach(ResourceContainer container in workspace.ServiceContainer.ResourceContainers.Where(rc => !(rc is ServiceOperation)))
                {
                    if (workspace.DataLayerProviderKind == DataLayerProviderKind.Edm)
                    {
                        // After named-stream redesign, adding streams to EF is non-trivial
                        continue;
                    }

                    if (container.ResourceTypes.Any(rt => rt.Facets.NamedStreams.Any()))
                    {
                        continue;
                    }

                    // because we don't represent streams as properties, having streams appear on anything other than the absolute base type
                    // makes call order prediction very hard
                    if (container.BaseType.BaseTypes.Any())
                    {
                        continue;
                    }

                    var type = container.BaseType;

                    switch(AstoriaTestProperties.Random.Next(3))
                    {
                        case 0:
                            type.Facets.Add(NodeFacet.Attribute(new NamedStreamResourceAttribute(type, "PhotoStream")));
                            break;

                        case 1:
                            type.Facets.Add(NodeFacet.Attribute(new NamedStreamResourceAttribute(type, "PhotoStream")));
                            type.Facets.Add(NodeFacet.Attribute(new NamedStreamResourceAttribute(type, "ThumbnailStream")));
                            type.Facets.Add(NodeFacet.Attribute(new NamedStreamResourceAttribute(type, "HighResolutionStream")));
                            break;
                    }
                }
            }

            if (workspace.DataLayerProviderKind != DataLayerProviderKind.LinqToSql)
            {
                string streamProviderComparerTypeName = typeof(ReferenceEqualityComparer).FullName;
                if (workspace.DataLayerProviderKind == DataLayerProviderKind.Edm)
                {
                    streamProviderComparerTypeName = workspace.Name + "EqualityComparer";

                    List<string> equalityComparerCode = new List<string>()
                    {
                        "public class " + streamProviderComparerTypeName + " : System.Data.Test.Astoria.KeyBasedEqualityComparerBase",
                        "{",
                        "    protected override string[] GetKeyPropertyNames(Type entityType)",
                        "    {",
                        "        entityType = ObjectContext.GetObjectType(entityType);",
                        "        var fullTypeName = entityType.FullName;",
                    };

                    foreach (var type in workspace.ServiceContainer.ResourceTypes)
                    {
                        equalityComparerCode.Add(string.Join(Environment.NewLine, new string[]
                        {
                            "        if(fullTypeName == \"" + type.FullName + "\")",
                            "        {",
                            "             return new string[] { " + string.Join(", ", type.Properties.Where(p => p.PrimaryKey != null).Select(p => '"' + p.Name + '"').ToArray()) + " };",
                            "        }",
                        }));
                    }

                    equalityComparerCode.Add("        throw new Exception(\"Unrecognized type\");");
                    equalityComparerCode.Add("    }");
                    equalityComparerCode.Add("}");

                    workspace.GlobalAdditionalCode += string.Join(Environment.NewLine, equalityComparerCode);
                }

                #region for inmemory/nonclr, make all int key properties server generated
                if (workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr || workspace.DataLayerProviderKind == DataLayerProviderKind.InMemoryLinq)
                {
                    foreach (ResourceType type in workspace.ServiceContainer.ResourceTypes)
                        foreach (NodeProperty property in type.Key.Properties)
                            if (property.Type == Clr.Types.Int32)
                                property.Facets.ServerGenerated = true;
                }
                #endregion

                #region find possible blob types, and set up stream provider
                List<ResourceType> possibleBlobTypes = new List<ResourceType>();
                foreach (ResourceType type in workspace.ServiceContainer.ResourceContainers.Where(rc => !(rc is ServiceOperation)).Select(rc => rc.BaseType))
                {
                    // need to find one that has only generated keys, all other propeties nullable, and no nav props
                    if (type.Key.Properties.Any(p => !p.Facets.ServerGenerated))
                        continue;
                    if (type.Properties.OfType<ResourceProperty>().Any(p => p.PrimaryKey == null && !p.IsNavigation && !p.Facets.Nullable))
                        continue;
                    if (type.Properties.OfType<ResourceProperty>().Any(p => p.IsNavigation))
                        continue;
                    possibleBlobTypes.Add(type);
                }

                if (possibleBlobTypes.Any())
                {
                    List<ResourceType> blobTypes = new List<ResourceType>();
                    // want one with etags, and one without

                    ResourceType blobType = possibleBlobTypes.Where(rt => rt.Properties.Any(p => p.Facets.ConcurrencyModeFixed)).Choose();
                    if (blobType != null)
                    {
                        blobType.Facets.Add(NodeFacet.Attribute(new BlobsAttribute(blobType)));
                        blobTypes.Add(blobType);
                    }

                    blobType = possibleBlobTypes.Where(rt => !rt.Properties.Any(p => p.Facets.ConcurrencyModeFixed)).Choose();
                    if (blobType != null)
                    {
                        blobType.Facets.Add(NodeFacet.Attribute(new BlobsAttribute(blobType)));
                        blobTypes.Add(blobType);
                    }

                    workspace.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServiceStreamProvider)]
                        = "new StreamProviderWrapper(new System.Data.Test.Astoria.InMemoryStreamProvider<" + streamProviderComparerTypeName + ">())";
                    workspace.Settings.StreamProviderImplementation = StreamProviderImplementation.DataServiceStreamProvider;

                    if (Versioning.Server.SupportsLiveFeatures && workspace.DataLayerProviderKind != DataLayerProviderKind.Edm)
                    {
                        workspace.ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServiceStreamProvider2)]
                            = "new StreamProvider2Wrapper(new System.Data.Test.Astoria.InMemoryStreamProvider<" + streamProviderComparerTypeName + ">())";
                        workspace.Settings.StreamProviderImplementation = StreamProviderImplementation.DataServiceStreamProvider2;

                        var blobTypesWithoutNamedStreams = blobTypes.Where(t => !t.Facets.NamedStreams.Any()).ToList();
                        if (blobTypesWithoutNamedStreams.Count != 0)
                        {
                            var blobTypeWithNamedStreams = blobTypesWithoutNamedStreams.Choose();
                            blobTypeWithNamedStreams.Facets.Add(NodeFacet.Attribute(new NamedStreamResourceAttribute(blobTypeWithNamedStreams, "Thumbnail")));
                        }
                    }

                    List<string> typeResolverLines = new List<string>();
                    foreach (ResourceType rt in blobTypes)
                    {
                        // safe because we know the types are disjoint (either do or do not have etags) and that they were the basetypes of their containers
                        IEnumerable<ResourceContainer> containers = workspace.ServiceContainer.ResourceContainers.Where(rc => rc.BaseType == rt);
                        foreach (ResourceContainer rc in containers)
                        {
                            typeResolverLines.Add("           if(entitySetName == \"" + rc.Name + "\")");
                            typeResolverLines.Add("               return \"" + rt.Namespace + "." + rt.Name + "\";");
                        }
                    }
                    typeResolverLines.Add("           return null;");
                    string typeResolverCode = string.Join(Environment.NewLine, typeResolverLines.ToArray());

                    workspace.RequiredFrameworkSources.Add("InMemoryStreamProvider.cs");
                    workspace.RequiredFrameworkSources.Add("KeyBasedEqualityComparerBase.cs");
                    workspace.RequiredFrameworkSources.Add("ReferenceEqualityComparer.cs");

                    var streamProviderFile = new ConstructedFile("InMemoryStreamProvider.cs");
                    workspace.ServiceModifications.Files.Add(streamProviderFile);
                    streamProviderFile.AddMethod("ResolveType", new NewMethodInfo() 
                    {
                         MethodSignature = "string ResolveType(string entitySetName, DataServiceOperationContext operationContext)",
                         BodyText = typeResolverCode,
                    });
                }
                #endregion
            }
            #endregion

            #region set up service operations
            List<ServiceOperation> serviceOperations = new List<ServiceOperation>();
            foreach(ResourceContainer container in workspace.ServiceContainer.ResourceContainers)
            {
                // no MEST yet for service ops!!
                if (!workspace.ServiceContainer.ResourceContainers.Any(c => c != container && c.BaseType == container.BaseType))
                {
                    foreach (RequestVerb verb in new RequestVerb[] { RequestVerb.Get, RequestVerb.Post })
                    {
                        ServiceOperation serviceOp = Resource.ServiceOperation(verb.ToString() + "_" + container.Name, container, container.BaseType, container.ResourceTypes.ToArray());
                        serviceOp.Verb = verb;

                        string fullTypeName = container.BaseType.Namespace + "." + container.BaseType.Name;
                        
			            if (workspace.DataLayerProviderKind == DataLayerProviderKind.NonClr && !container.BaseType.Facets.IsClrType)
	                        fullTypeName = typeof(NonClr.RowEntityType).FullName;

                        serviceOp.ServiceOpCode = string.Join(Environment.NewLine, new string[]
                        {
                            (verb == RequestVerb.Get ? "[WebGet]" : "[WebInvoke(Method = \"POST\")]"),
                            "public IQueryable<" + fullTypeName + "> " + serviceOp.Name + "()",
                            "{",
                            "     return GetEntitySet<" + fullTypeName + ">(\"" + container.Name + "\");",
                            "}"
                        });
                        serviceOp.ServiceOperationResultKind = Microsoft.OData.Service.Providers.ServiceOperationResultKind.QueryWithMultipleResults;
                        serviceOp.ExpectedTypeName = fullTypeName;
                        serviceOperations.Add(serviceOp);
                    }
                }
            }
            workspace.ServiceContainer.AddNodes(serviceOperations);
            workspace.AddServiceOperationCode();
            #endregion
        }
    }
}
