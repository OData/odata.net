//---------------------------------------------------------------------
// <copyright file="InMemoryWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System.Collections;
    using System.Collections.Generic;
    //EntityConnectionStringBUilder
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using Microsoft.OData.Client;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria.ReflectionProvider;
    using System.IO;            //Path
    using System.Linq;
    using System.Net;
    using System.Reflection; //Assembly
    using Microsoft.Test.ModuleCore; //TestFailedException;

    [WorkspaceDefaultSettings(
        ServiceBaseClass = "System.Data.Test.Astoria.InMemoryTestWebService",
        UpdatableImplementation = UpdatableImplementation.IUpdatable)
    ]
    public abstract class InMemoryWorkspace : Workspace, IDisposable
    {
        //Constructor
        public InMemoryWorkspace(String name, string contextNamespace, string contextTypeName)
            : base(DataLayerProviderKind.InMemoryLinq, name, contextNamespace, contextTypeName)
        {
            this.DataGenerator = new GraphBasedDataGenerator(this, new CodeGeneratingDataInserter(this));

            if (AstoriaTestProperties.IsRemoteClient)
                BeforeServiceCreation.Add(() => WorkspaceLibrary.AddSilverlightHostFiles(this));

            this.RequiredFrameworkSources.Add("InMemoryContext.cs");
            this.RequiredFrameworkSources.Add("InMemoryDataService.cs");
            this.RequiredFrameworkSources.Add("InMemoryEntitySetDictionary.cs");

            ConstructedFile inMemoryContextFile = new ConstructedFile("InMemoryContext.cs");
            ServiceModifications.Files.Add(inMemoryContextFile);
            ServiceModifications.Interfaces.IUpdatable.ImplementingFile = inMemoryContextFile;
            ServiceModifications.Interfaces.IDataServiceUpdateProvider.ImplementingFile = inMemoryContextFile;
        }

        /// <summary>
        /// Populates the source folder with the files that should be 
        /// available on the service host.
        /// </summary>
        public override void PopulateHostSourceFolder()
        {
            base.PopulateHostSourceFolder();

            string inmemoryCodeFilePath = Path.Combine(WebServiceAppCodeDir, this.ObjectLayerOutputFileName);
            using (StronglyTypedCodeLayerBuilder codeLayer = new StronglyTypedCodeLayerBuilder(this, WorkspaceLanguage.CSharp, inmemoryCodeFilePath))
            {
                codeLayer.Build();
            }
        }

        protected internal override string ObjectLayerResourceName
        {
            get { return this.Name + ".InMemory.ObjectLayer" + this.LanguageExtension; }
        }
        protected internal override string ObjectLayerOutputFileName
        {
            get { return this.Name + ".InMemory.ObjectLayer" + this.LanguageExtension; }
        }

        protected internal override string ServiceResourceName
        {
            get { return this.Name + ".InMemory.Service" + this.LanguageExtension; }
        }

        protected internal override string ServiceOutputFileName
        {
            get { return this.Name + ".InMemory.Service" + this.LanguageExtension; }
        }

        public override void ApplyFriendlyFeeds()
        {
            if (GenerateServerMappings)
            {
                string partialClassPath = Path.Combine(Path.Combine(this.DataService.DestinationFolder, "App_Code"), this.ObjectLayerOutputFileName.Replace("InMemory", "InMemoryPartial"));
                IOUtil.EnsureFileDeleted(partialClassPath);

                StreamWriter textStream = IOUtil.CreateTextStream(partialClassPath);
                ReflectionProvider.CSharpCodeLanguageHelper codeHelper
                    = new ReflectionProvider.CSharpCodeLanguageHelper(textStream);

                WritePartialClassAttributes(this.ContextNamespace, codeHelper);
                textStream.Close();

                this.ObjectLayerFileNames.Add(partialClassPath);
            }
            if (GenerateClientTypes)
            {
                GenerateAndLoadClientTypes();
                PopulateClientTypes();
            }
            this.RestoreData();
        }

        /// <summary>Populates the client types on the ResourceType</summary>
        protected internal override void PopulateClientTypes()
        {
            string clientNamespace = this.ContextNamespace.ToLower().Contains("northwind") ? "northwindClient" : this.Name + "Client";
            Assembly currentAssembly = null;
            //If running under a Friendly feeds test, use the dynamically created assembly and not the current assembly
            if (GenerateClientTypes)
            {
                if (this.ClientTypesAssembly != null)
                {
                    clientNamespace = this.ContextNamespace.ToLower().Contains("northwind") ? "northwindClient" : clientNamespace;
                    currentAssembly = this.ClientTypesAssembly;
                }
            }
            else
            {
                currentAssembly = this.GetType().Assembly;
            }
            if (currentAssembly != null)
            {
                _resourceTypeToClientTypeList = CreateResourceTypeToUnderlyingClrTypeMap(currentAssembly, GenerateClientTypes ? ContextNamespace : clientNamespace);
                SetResourceTypesToClrTypes(currentAssembly, this.ContextNamespace, _resourceTypeToClientTypeList, true);
            }
        }

        private PropertyInfo FindContainerProperty(ObjectContext context, string containerName)
        {
            return context.GetType().GetProperties().Where(propi => propi.Name == containerName).First();
        }


        public override IQueryable ResourceContainerToQueryable(ResourceContainer container)
        {
            Uri uri = new Uri(container.Workspace.DataService.ServiceUri.ToString());
            DataServiceContext ctx;
            ODataProtocolVersion? maxProtocolVersion = null;

            // do this agressively, as we weren't successful at predicting when we actually needed it
            maxProtocolVersion = ODataProtocolVersion.V4;

            if (maxProtocolVersion.HasValue)
            {
                ctx = new DataServiceContext(uri, maxProtocolVersion.Value);
            }
            else
            {
                ctx = new DataServiceContext(uri);
            }

            ctx.Configurations.RequestPipeline.OnMessageCreating = this.ctx_OnMessageCreating;
            //ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;

            Type type = this._resourceTypeToWorkspaceTypeList[container.BaseType];

            MethodInfo method = ctx.GetType().GetMethod("CreateQuery", new Type[] { typeof(String) });
            MethodInfo genMethod = method.MakeGenericMethod(new Type[] { type });

            IQueryable o = (IQueryable)genMethod.Invoke(ctx, new object[] { container.Name });
            return (IQueryable)o;
        }

        public IEnumerable ClientExecuteWrapper<T>(string query, ResourceContainer container)
        {
            // we need to wrap the invoked call to execute because
            // it can throw a web exception if we run out of sockets
            DataServiceContext ctx;
            Microsoft.OData.Client.ODataProtocolVersion? maxProtocolVersion = null;

            // do this agressively, as we weren't successful at predicting when we actually needed it
            maxProtocolVersion = Microsoft.OData.Client.ODataProtocolVersion.V4;

            if (maxProtocolVersion.HasValue)
            {
                ctx = new DataServiceContext(new Uri(this.ServiceUri), maxProtocolVersion.Value);
            }
            else
            {
                ctx = new DataServiceContext(new Uri(this.ServiceUri));
            }

            //ctx.UndeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;
            ctx.Configurations.RequestPipeline.OnMessageCreating = this.ctx_OnMessageCreating;

            return SocketExceptionHandler.Execute<IEnumerable>(() => (IEnumerable)ctx.Execute<T>(new Uri(query)));
        }

        public override KeyExpressions GetExistingAssociatedKeys(ResourceContainer resourceContainer, ResourceProperty property, KeyExpression keyExpression)
        {
            bool pageSizeChanged = false;
            int originalPageSize = this.DataService.ConfigSettings.GetEntitySetPageSize(resourceContainer.Name);
            if (originalPageSize < 1000)
            {
                pageSizeChanged = true;
                this.DataService.ConfigSettings.SetEntitySetPageSize(resourceContainer.Name, 100000);
            }

            UriQueryBuilder uriQueryBuilder = new UriQueryBuilder(this, this.ServiceUri);
            ExpNode query = Query.From(
                                 Exp.Variable(resourceContainer))
                                .Where(keyExpression)
                //.OfType(property.ResourceType)
                                .Nav(new PropertyExpression(property))
                                .Select();

            string uri = uriQueryBuilder.Build(query);

            ResourceType associatedResourceType = property.Type as ResourceType;
            if (property.Type is CollectionType)
            {
                associatedResourceType = (property.Type as CollectionType).SubType as ResourceType;
            }
            Type type = this._resourceTypeToWorkspaceTypeList[associatedResourceType];

            MethodInfo method = this.GetType().GetMethod("ClientExecuteWrapper", new Type[] { typeof(string), typeof(ResourceContainer) });
            MethodInfo genMethod = method.MakeGenericMethod(new Type[] { type });
            IEnumerable o = (IEnumerable)genMethod.Invoke(this, new object[] { uri, resourceContainer.FindDefaultRelatedContainer(property) });

            KeyExpressions keys = new KeyExpressions();
            //IEnumerator enumerator = o.GetEnumerator();
            foreach (object current in o)
            {
                if (current != null)
                {
                    //Type t = enumerator.Current.GetType();
                    Type t = current.GetType();

                    IEnumerable<ResourceType> typesWithName = this.ServiceContainer.ResourceTypes.Where(rt => (t.Name.Equals(rt.Name)));
                    IEnumerable<ResourceType> typesWithNamespace = typesWithName.Where(rt2 => rt2.Namespace == t.Namespace).ToList();
                    ResourceType instanceType = typesWithNamespace.First();
                    ResourceContainer relatedContainer = resourceContainer.FindDefaultRelatedContainer(property);
                    keys.Add(GetKeyExpression(relatedContainer, instanceType, current));
                }
            }
            if (pageSizeChanged)
            {
                this.DataService.ConfigSettings.SetEntitySetPageSize(resourceContainer.Name, originalPageSize);
            }
            return keys;
        }
        private KeyExpressions GetAllExistingKeysBase(ResourceContainer resourceContainer)
        {
            return base.GetAllExistingKeysOfType(resourceContainer, null);
        }
        public override KeyExpressions GetAllExistingKeys(ResourceContainer resourceContainer)
        {
            return GetAllExistingKeysOfType(resourceContainer, null);
        }

        public override KeyExpressions GetAllExistingKeysOfType(ResourceContainer resourceContainer, ResourceType resourceType)
        {
            if (!DataGenerator.Done)
                return DataGenerator.GetAllGeneratedKeys(resourceContainer, resourceType);

            bool rightsChanged = false;
            EntitySetRights oldRights = this.DataService.ConfigSettings.GetEntitySetAccessRule(resourceContainer.Name);

            if ((EntitySetRights.ReadMultiple & oldRights) == 0)
            {
                rightsChanged = true;
                this.DataService.ConfigSettings.SetEntitySetAccessRule(resourceContainer.Name, EntitySetRights.All);
            }

            KeyExpressions keys = SocketExceptionHandler.Execute<KeyExpressions>(
                () => GetAllExistingKeysBase(resourceContainer));

            if (rightsChanged)
                this.DataService.ConfigSettings.SetEntitySetAccessRule(resourceContainer.Name, oldRights);

            if (resourceType == null)
                return keys;

            return new KeyExpressions(keys.Where(k => k.ResourceType.Equals(resourceType)));
        }

        public override KeyExpressions GetAllExistingKeys(ExpNode query, ResourceContainer resourceContainer)
        {
            if (DataGenerator.Done)
                return base.GetAllExistingKeys(query, resourceContainer);
            else
                return DataGenerator.GetAllGeneratedKeys(resourceContainer, null);
        }


        public override KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer)
        {
            return this.GetRandomExistingKey(resourceContainer, (ResourceType)null);
        }

        public override KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer, ResourceType resourceType)
        {
            if (DataGenerator.Done)
                return this.GetAllExistingKeysOfType(resourceContainer, resourceType).Choose();
            else
                return DataGenerator.GetRandomGeneratedKey(resourceContainer, resourceType);
        }


        private KeyedResourceInstance GetSingleResourceByKeyBase(KeyExpression keyExpression)
        {
            return base.GetSingleResourceByKey(keyExpression);
        }

        public override KeyedResourceInstance GetSingleResourceByKey(KeyExpression keyExpression)
        {
            if (keyExpression.Properties.OfType<ResourceProperty>().Where(rp => rp.Type.ClrType.Equals(typeof(DateTime))).Count() > 0)
            {
                UriQueryBuilder uriQueryBuilder = new UriQueryBuilder(this, this.ServiceUri);
                _workaroundDateTimeQuery = uriQueryBuilder.Build(Query.From(
                                 Exp.Variable(keyExpression.ResourceContainer))
                                .Where(keyExpression)
                                .Select());
            }
            KeyedResourceInstance o = null;
            try
            {
                o = SocketExceptionHandler.Execute<KeyedResourceInstance>(
                    () => GetSingleResourceByKeyBase(keyExpression));
            }
            catch (Microsoft.OData.Client.DataServiceQueryException exc)
            {
                if (exc.Response.StatusCode != 404)
                {
                    throw exc;
                }

                // if DSV is not present, its not an Astoria-level error
                if (!exc.Response.Headers.ContainsKey("OData-Version"))
                {
                    throw exc;
                }
            }

            return o;

        }
        string _workaroundDateTimeQuery = null;
        DataServiceClientRequestMessage ctx_OnMessageCreating(DataServiceClientRequestMessageArgs args)
        {
            if (_workaroundDateTimeQuery != null)
            {
                args = new DataServiceClientRequestMessageArgs("GET", new Uri(_workaroundDateTimeQuery), false, false, new Dictionary<string, string>());
                _workaroundDateTimeQuery = null;
            }
            HttpClientRequestMessage clientRequestMessage = new HttpClientRequestMessage(args);
            clientRequestMessage.Credentials = CredentialCache.DefaultCredentials;
            return clientRequestMessage;
        }

        protected override ResourceContainer DetermineResourceContainerFromProviderObject(object o)
        {
            IEntityWithKey entityWithKey = o as IEntityWithKey;
            List<ResourceContainer> containers = this.ServiceContainer.ResourceContainers.Where(rc => rc.Name.Equals(entityWithKey.EntityKey.EntitySetName)).ToList();
            if (containers.Count == 0)
                throw new TestFailedException("Could not find EntitySet:" + entityWithKey.EntityKey.EntitySetName);
            return containers.First();
        }

        public override void RestoreData()
        {
            if (this.Settings.SkipDataPopulation)
                return;
            RequestUtil.GetAndVerifyStatusCode(this, this.ServiceUri + "/RestoreData", HttpStatusCode.NoContent);
        }
    }
}
