//---------------------------------------------------------------------
// <copyright file="LinqToSqlWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections.Generic;
    using System.Data.EntityClient; //EntityConnectionStringBUilder
    using System.Data.Objects;
    using System.Data.Test.Astoria.TestExecutionLayer;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Test.ModuleCore;
    using System.Data.Linq;
    using System.Linq.Expressions;

    [WorkspaceDefaultSettings(
        ServiceBaseClass = "System.Data.Test.Astoria.TestDataWebService",
        UpdatableImplementation = UpdatableImplementation.None)
    ]
    public abstract class LinqToSqlWorkspace : Workspace, IDisposable
    {
        //Constructor
        public LinqToSqlWorkspace(String name, string contextNamespace, string contextTypeName)
            : base(DataLayerProviderKind.LinqToSql, name, contextNamespace, contextTypeName)
        {
            BeforeServiceCreation.Add(() => WorkspaceLibrary.CreateDefaultDatabase(this));
        }

        /// <summary>
        /// Populates the source folder with the files that should be 
        /// available on the service host.
        /// </summary>
        public override void PopulateHostSourceFolder()
        {
            base.PopulateHostSourceFolder();

            Assembly resourceAssembly = this.GetType().Assembly;

            string codeFilePath = Path.Combine(WebServiceAppCodeDir, this.ObjectLayerOutputFileName);
            IOUtil.FindAndWriteResourceToFile(resourceAssembly, this.ObjectLayerResourceName, codeFilePath);
        }

        protected internal override string ObjectLayerResourceName
        {
            get { return this.Name + ".LinqToSql.ObjectLayer" + this.LanguageExtension; }
        }
        protected internal override string ObjectLayerOutputFileName
        {
            get { return this.Name + ".LinqToSql.ObjectLayer" + this.LanguageExtension; }
        }

        public override System.Configuration.ConnectionStringSettings GetConnectionStringSettingsForProvider(AstoriaWebDataService service, string databaseConnectionString)
        {
 	        return new System.Configuration.ConnectionStringSettings(this.ContextTypeName, databaseConnectionString, "System.Data.SqlClient");
        }
        
        /// <summary>Populates the client types on the ResourceType</summary>
        protected internal override void PopulateClientTypes()
        {
            Assembly currentAssembly = this.GetType().Assembly;

            //If running under a Friendly feeds test, use the dynamically created assembly and not the current assembly
            if (GenerateClientTypes)
            {
                if (this.ClientTypesAssembly != null)
                {
                    currentAssembly = this.ClientTypesAssembly;
                }
            }
            _resourceTypeToClientTypeList = CreateResourceTypeToClientClrTypeMap(currentAssembly);
            IEnumerable<ResourceType> r = this.ServiceContainer.ResourceTypes;
            foreach (ResourceType rt in r)
            {
                rt.ClientClrType = _resourceTypeToClientTypeList[rt];
            }

            foreach (ResourceType rt in r)
            {
                foreach (ResourceProperty rp in rt.Properties)
                {
                    if ((rp.IsNavigation || rp.IsComplexType) && rp.Type.ClientClrType == null)
                    {
                        Type clientType = currentAssembly.GetType(this.ContextNamespace + "ClientLTS." + rp.Type.ToString());
                        if (clientType != null)
                        {
                            rp.Type.ClientClrType = clientType;
                        }

                        if (rp.Type is CollectionType)
                        {
                            CollectionType collectionType = (CollectionType)rp.Type;
                            NodeType subType = collectionType.SubType;

                            if (subType.ClientClrType == null)
                            {
                                clientType = currentAssembly.GetType(this.ContextNamespace + "ClientLTS." + subType.Name);
                                if (clientType != null)
                                    subType.ClientClrType = clientType;
                            }
                        }
                    }
                    else
                    {
                        Type clientType = currentAssembly.GetType(this.ContextNamespace + "ClientLTS." + rp.Type.ToString());
                        if (clientType != null)
                        {
                            rp.Type.ClientClrType = clientType;
                        }
                    }
                }
            }
        }
        public override void ApplyFriendlyFeeds()
        {
            if (GenerateServerMappings)
            {
                string partialClassPath = Path.Combine(Path.Combine(this.DataService.DestinationFolder, "App_Code"), this.ObjectLayerOutputFileName.Replace("LinqToSql", "LinqToSqlPartial"));
                if (File.Exists(partialClassPath))
                {
                    IOUtil.EnsureFileDeleted(partialClassPath);
                }

                StreamWriter textStream = File.CreateText(partialClassPath);
                ReflectionProvider.CSharpCodeLanguageHelper codeHelper
                    = new ReflectionProvider.CSharpCodeLanguageHelper(textStream);

                WritePartialClassAttributes(this.ContextNamespace, codeHelper);
                textStream.Close();
            }
            if (GenerateClientTypes || GenerateClientTypesManually)
            {
                GenerateAndLoadClientTypes();
            }
            PopulateClientTypes();

        }

        protected virtual AstoriaHashTable<ResourceType, Type> CreateResourceTypeToClientClrTypeMap(Assembly currentAssembly)
        {
            AstoriaHashTable<ResourceType, Type> typeMap = new AstoriaHashTable<ResourceType, Type>();
            foreach (ResourceType t in this.ServiceContainer.ResourceTypes)
            {
                if (!typeMap.ContainsKey(t))
                {
                    Type clientType = currentAssembly.GetType(this.ContextNamespace + "ClientLTS." + t.Name);
                    if (clientType == null)
                        throw new TestFailedException("Unable to find client Clr Type:" + this.ContextNamespace + "." + t.Name + " in Assembly:" + currentAssembly);
                    typeMap.Add(t, clientType);
                }
            }
            return typeMap;
        }

        internal override HashSet<Assembly> GetReferencedAssemblies()
        {
            HashSet<Assembly> collection = base.GetReferencedAssemblies();
            collection.Add(typeof(Microsoft.OData.Client.ChangeOperationResponse).Assembly);
            collection.Add(typeof(System.Xml.Linq.XElement).Assembly);
            collection.Add(typeof(System.Data.Linq.DataContext).Assembly);
            collection.Add(typeof(System.Configuration.ConfigurationManager).Assembly);
            return collection;
        }

        public override IQueryable ResourceContainerToQueryable(ResourceContainer container)
        {
            //Get Lts Type
            Type type = this._resourceTypeToWorkspaceTypeList[container.BaseType];

            DataContext context = new DataContext(this.Database.DatabaseConnectionString);

            MethodInfo method = context.GetType().GetMethod("GetTable", new Type[] { });
            MethodInfo genMethod = method.MakeGenericMethod(new Type[] { type });

            object o = genMethod.Invoke(context, new object[] { });

            if (container.HasInterceptorExpression)
            {
                o = ApplyQueryInterceptorExpression(container, o as IQueryable);
            }
            return (IQueryable)o;
        }

        public override void PrepareRequest(AstoriaRequest request)
        {
            request.ExtraVerification += ResponseVerification.DefaultVerify;
        }
    }
}
