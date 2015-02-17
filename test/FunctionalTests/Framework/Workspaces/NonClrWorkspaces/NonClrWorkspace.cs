//---------------------------------------------------------------------
// <copyright file="NonClrWorkspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Data.Test.Astoria.LateBound;
using System.Data.Test.Astoria.NonClr;
using System.Data.Test.Astoria.ReflectionProvider;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;
using Microsoft.Test.ModuleCore;
using System.Text;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria
{
    [WorkspaceDefaultSettings(ServiceBaseClass = "System.Data.Test.Astoria.NonClr.NonClrTestWebService",
        UpdatableImplementation = UpdatableImplementation.DataServiceUpdateProvider,
        UseLazyPropertyLoading = true
        )]
    public abstract class NonClrWorkspace : Workspace, IDisposable
    {
        private NonClrEntitySetDictionary _resources = null;

        //Constructor
        public NonClrWorkspace(String name, string contextNamespace, string contextTypeName)
            : base(DataLayerProviderKind.NonClr, name, contextNamespace, contextTypeName)
        {
            _resources = new NonClrEntitySetDictionary();

            this.DataGenerator = new GraphBasedDataGenerator(this, new CodeGeneratingDataInserter(this));
            this.DataGenerator.DataInserter.OnAddingEntity += this.DataInserter_HandleAddingEntity;
            this.DataGenerator.DataInserter.OnAddingAssociation += this.DataInserter_HandleAddingAssociation;

            BeforeServiceCreation.Insert(0, DefineClrProperties);

            if (AstoriaTestProperties.IsRemoteClient)
                BeforeServiceCreation.Add(() => WorkspaceLibrary.AddSilverlightHostFiles(this));

            RequiredFrameworkSources.Add("NonClrContext.cs");
            RequiredFrameworkSources.Add("NonClrDataService.cs");
            RequiredFrameworkSources.Add("NonClrEntitySetDictionary.cs");
            RequiredFrameworkSources.Add("NonClrExpressionTreeToXmlSerializer.cs");
            RequiredFrameworkSources.Add("NonClrQueryable.cs");
            RequiredFrameworkSources.Add("DSPMethodsImplementation.cs");
            RequiredFrameworkSources.Add("LateBoundBaseExpressionVisitor.cs");
            RequiredFrameworkSources.Add("LateBoundExpressionVisitor.cs");
            RequiredFrameworkSources.Add("LateBoundToClrConverter.cs");
            RequiredFrameworkSources.Add("OpenTypeMethodsImplementation.cs");

            ConstructedFile nonClrContextFile = new ConstructedFile("NonClrContext.cs");

            ServiceModifications.Files.Add(nonClrContextFile);
            ServiceModifications.Interfaces.IUpdatable.ImplementingFile = nonClrContextFile;
            ServiceModifications.Interfaces.IDataServiceUpdateProvider.ImplementingFile = nonClrContextFile;
            ServiceModifications.Interfaces.IDataServiceMetadataProvider.ImplementingFile = nonClrContextFile;
            ServiceModifications.Interfaces.IDataServiceQueryProvider.ImplementingFile = nonClrContextFile;

            ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServiceQueryProvider)]
                = "new QueryProviderWrapper(ContextInstance)";
            ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServiceMetadataProvider)]
                = "new MetadataProviderWrapper(ContextInstance)";
            ServiceModifications.Interfaces.IServiceProvider.Services[typeof(Microsoft.OData.Service.Providers.IDataServiceUpdateProvider)]
                = "new UpdateProviderWrapper(ContextInstance)";
        }

        protected internal override void CreateLocalAssembly()
        {
            // do nothing (this is to balance out moving the logic from the other 3 workspaces into the base workspace class)
        }

        /// <summary>
        /// Populates the source folder with the files that should be 
        /// available on the service host.
        /// </summary>
        public override void PopulateHostSourceFolder()
        {
            base.PopulateHostSourceFolder();

            string filePath = Path.Combine(WebServiceAppCodeDir, this.ObjectLayerOutputFileName);

            NonClrContextTemplateFile contextFile = new NonClrContextTemplateFile(this, filePath);
            contextFile.Build();
        }

        #region overrides to allow customization of NonClr names

        protected internal override string ObjectLayerResourceName
        {
            get { return this.Name + ".NonClr.ObjectLayer" + this.LanguageExtension; }
        }
        protected internal override string ObjectLayerOutputFileName
        {
            get { return this.Name + ".NonClr.ObjectLayer" + this.LanguageExtension; }
        }

        protected internal override string ServiceResourceName
        {
            get { return this.Name + ".NonClr.Service" + this.LanguageExtension; }
        }

        protected internal override string ServiceOutputFileName
        {
            get { return this.Name + ".NonClr.Service" + this.LanguageExtension; }
        }
        #endregion

        /// <summary>Populates the client types on the ResourceType</summary>
        protected internal override void PopulateClientTypes()
        {
            string clientNamespace = this.Name + "Client";
            Assembly currentAssembly = this.GetType().Assembly;


            if (GenerateClientTypes)
            {
                if (this.ClientTypesAssembly != null)
                {
                    clientNamespace = clientNamespace == "ArubaClient" ? "Aruba.NonClr" : "northwind.NonClr";
                    currentAssembly = this.ClientTypesAssembly;
                }
            }


            _resourceTypeToClientTypeList = CreateResourceTypeToUnderlyingClrTypeMap(currentAssembly, clientNamespace);

            // Make sure ClrType is set for all types in the assembly and servicecontainer
            IEnumerable<ResourceType> r = this.ServiceContainer.ResourceTypes;
            foreach (ResourceType rt in r)
            {
                rt.ClientClrType = _resourceTypeToClientTypeList[rt];

                if (rt.BaseType != null && !this.ServiceContainer.ResourceTypes.Where(rt2 => rt2.Name.Equals(rt.BaseType.Name)).Any())
                {
                    rt.BaseType.ClientClrType = _resourceTypeToClientTypeList[rt];
                }
            }

            SetResourceTypesToClrTypes();
        }

        public void SetResourceTypesToClrTypes()
        {

            // Make sure ClrType is set for all types in the assembly and servicecontainer
            IEnumerable<ResourceType> r = this.ServiceContainer.ResourceTypes;
            foreach (ResourceType rt in r)
            {
                rt.ClrType = typeof(RowEntityType);
                if (rt.BaseType != null)
                {
                    rt.BaseType.ClrType = typeof(RowEntityType);
                }
            }

            foreach (ResourceType rt in r)
            {
                foreach (ResourceProperty rp in rt.Properties)
                {
                    if ((rp.IsNavigation) && rp.Type.ClrType == null)
                        rp.Type.ClrType = typeof(RowEntityType);
                    else if (rp.IsComplexType)
                        rp.Type.ClrType = typeof(RowComplexType);

                    if (rp.Type is CollectionType)
                    {
                        rp.Type.ClrType = typeof(List<RowEntityType>);
                    }
                }
            }
        }

        public override void ApplyFriendlyFeeds()
        {
            if (GenerateServerMappings)
            {
                PopulateHostSourceFolder();
                string destinationPath = Path.Combine(Path.Combine(this.DataService.DestinationFolder_Local, "App_Code"), this.ObjectLayerOutputFileName);
                string sourceFilePath = Path.Combine(WebServiceAppCodeDir, this.ObjectLayerOutputFileName);
                TrustedMethods.CopyFile(sourceFilePath, destinationPath);
                this.DataService.ConfigSettings.ClearAll();
                ClearMetadataCache();
            }
            if (GenerateClientTypes || GenerateClientTypesManually)
            {
                GenerateAndLoadClientTypes();

            }
            PopulateClientTypes();
            this.RestoreData();
        }

        protected override IDictionary<string, object> GetPropertyValues(object entity)
        {
            RowEntityType cast = entity as RowEntityType;
            if (cast == null)
                AstoriaTestLog.FailAndThrow("Entity was not a RowEntityType instance");
            return cast.Properties;
        }

        public override IQueryable ResourceContainerToQueryable(ResourceContainer container)
        {
            return _resources[container.Name].AsQueryable();
        }

        //TODO: needed?
        public override QueryBuilder QueryBuilder
        {
            get { throw new NotImplementedException(); }
        }

        public override void PrepareRequest(AstoriaRequest request)
        {
            request.ExtraVerification += this.NonClrDefaultVerify;

            request.OnReceiveEvent += HandleResponseEvent;
        }

        public void NonClrDefaultVerify(AstoriaResponse response)
        {
            if (!this.SkipContentVerification)
            {
                // special case for ETags
                if (response.ActualStatusCode == System.Net.HttpStatusCode.NotModified)
                    return;

                AstoriaRequest request = response.Request;

                RequestVerb verb = request.EffectiveVerb;

                switch (verb)
                {
                    case RequestVerb.Get:
                        ResponseVerification.VerifyGet(response);
                        break;

                    case RequestVerb.Post:
                    case RequestVerb.Patch:
                    case RequestVerb.Put:
                    case RequestVerb.Delete:
                        // other verbs now handled by default elsewhere
                        break;

                    default:
                        ResponseVerification.VerifyUnknownVerb(response);
                        break;
                }
            }
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

            ExpNode query = ContainmentUtil.BuildCanonicalQuery(keyExpression).Nav(property.Property()).Select();

            AstoriaRequest request = this.CreateRequest(query);

            AstoriaResponse response = request.GetResponse();
            // normal verification doesn't work here safely due to ETags and No-Content responses
            if (response.ActualStatusCode != HttpStatusCode.OK && response.ActualStatusCode != HttpStatusCode.NoContent)
            {
                ResponseVerification.LogFailure(response, new TestFailedException("Unexpected status code"));
            }

            ResourceContainer otherResourceContainer = resourceContainer.FindDefaultRelatedContainer(property);
            KeyExpressions keyExpressions = GetKeyExpressionsFromPayload(otherResourceContainer, response);
            if (pageSizeChanged)
            {
                this.DataService.ConfigSettings.SetEntitySetPageSize(resourceContainer.Name, originalPageSize);
            }

            return keyExpressions;
        }

        private KeyExpressions GetKeyExpressionsFromPayload(ResourceContainer container, AstoriaResponse response)
        {
            CommonPayload payload = response.CommonPayload;

            KeyExpressions keys = new KeyExpressions();

            //IEnumerable<ResourceProperty> keyProperties = container.BaseType.Properties.OfType<ResourceProperty>().Where(rp => rp.PrimaryKey != null);

            if (payload.Resources != null)
            {
                foreach (PayloadObject entity in (List<PayloadObject>)payload.Resources)
                {
                    keys.Add(ConcurrencyUtil.ConstructKey(container, entity));
                }
            }

            return keys;
        }

        public override KeyExpressions GetAllExistingKeysOfType(ResourceContainer resourceContainer, ResourceType resourceType)
        {
            if (!this.DataGenerator.Done)
            {
                return this.DataGenerator.GetAllGeneratedKeys(resourceContainer, resourceType);
            }

            bool rightsChanged = false;
            EntitySetRights oldRights = this.DataService.ConfigSettings.GetEntitySetAccessRule(resourceContainer.Name);

            if ((EntitySetRights.ReadMultiple & oldRights) == 0)
            {
                rightsChanged = true;
                this.DataService.ConfigSettings.SetEntitySetAccessRule(resourceContainer.Name, EntitySetRights.All);
            }

            bool resourceTypeUnspecified = resourceType == null;
            if (resourceTypeUnspecified)
            {
                resourceType = resourceContainer.BaseType;
            }

            ExpNode query = Query.From(Exp.Variable(resourceContainer)).Where(Exp.IsOf(null, resourceType));
            if (Versioning.Server.SupportsV2Features && !this.Settings.HasExpandProvider)
            {
                query = ((QueryNode)query).New(resourceType.Properties.Where(p => p.PrimaryKey != null || p.Facets.ConcurrencyModeFixed).Select(p => p.Property()).ToArray());
            }
            else
            {
                query = ((QueryNode)query).Select();
            }
            
            AstoriaRequest request = this.CreateRequest(query);

            AstoriaResponse response = request.GetResponse();

            // When the response is 404 this means no keys were found so return empty
            if(response.ActualStatusCode == HttpStatusCode.NotFound)
            {
                return new KeyExpressions();
            }

            // normal verification doesn't work here safely due to ETags and No-Content responses
            if (response.ActualStatusCode != HttpStatusCode.OK && response.ActualStatusCode != HttpStatusCode.NoContent)
                ResponseVerification.LogFailure(response, new TestFailedException("Unexpected status code"));

            if (rightsChanged)
                this.DataService.ConfigSettings.SetEntitySetAccessRule(resourceContainer.Name, oldRights);

            KeyExpressions allKeys = GetKeyExpressionsFromPayload(resourceContainer, response);

            if (resourceTypeUnspecified)
            {
                return allKeys;
            }

            return new KeyExpressions(allKeys.Where(key => resourceType.Equals(key.ResourceType)));
        }

        public override KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer)
        {
            return GetAllExistingKeysOfType(resourceContainer, null).Choose();
        }

        public override KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer, ResourceType resourceType)
        {
            return this.GetAllExistingKeysOfType(resourceContainer, resourceType).Choose();
        }

        public override KeyedResourceInstance GetSingleResourceByKey(KeyExpression keyExpression)
        {
            ExpNode query = ContainmentUtil.BuildCanonicalQuery(keyExpression);
            AstoriaRequest request = this.CreateRequest(query);

            if (request.URI.Length > 260) // Can't make this request.
                return null;

            AstoriaResponse response = request.GetResponse();

            if (response.ActualStatusCode == HttpStatusCode.NotFound)
                return null;
            else if (response.ActualStatusCode == HttpStatusCode.OK)
            {
                CommonPayload payload = response.CommonPayload;
                if (payload.Resources is List<PayloadObject>)
                {
                    List<PayloadObject> payloadObjects = (List<PayloadObject>)payload.Resources;
                    if (payloadObjects.Count == 1)
                        return ResourceInstanceUtil.CreateKeyedResourceInstanceFromPayloadObject(keyExpression.ResourceContainer, keyExpression.ResourceType, payloadObjects.First());
                    else if (payloadObjects.Count == 0 && keyExpression.ResourceContainer is ServiceOperation)
                    {
                        // we change serviceoperations into $filter requests, so they come back as empty feeds instead of 404s
                        return null;
                    }
                }
            }

            ResponseVerification.LogFailure(response, new TestFailedException("Could not get resource for the given key"));
            return null;
        }
        
        string _workaroundDateTimeQuery = null;
        void ctx_SendingRequest(object sender, Microsoft.OData.Client.SendingRequestEventArgs e)
        {
            if (_workaroundDateTimeQuery != null)
            {
                Net.HttpWebRequest request = (Net.HttpWebRequest)Net.HttpWebRequest.Create(_workaroundDateTimeQuery);
                request.Method = "Get";
                e.Request = request;
                _workaroundDateTimeQuery = null;
            }
            e.Request.UseDefaultCredentials = true;
        }

        //TODO: Clean up constant strings
        public override Dictionary<string, string> BaselineFiles()
        {
            return null;
        }

        
        protected override ResourceType DetermineResourceTypeFromProviderObject(object o)
        {
            RowEntityType ret = (RowEntityType)o;

            List<ResourceType> types = this.ServiceContainer.ResourceTypes.Where(rt => rt.Name.Equals(ret.TypeName)).ToList();
            if (types.Count == 0)
                throw new TestFailedException("Unable to determine ResourceType from object where type is:" + o.GetType().Name);

            return types.First();
        }

        protected override ResourceContainer DetermineResourceContainerFromProviderObject(object o)
        {
            RowEntityType ret = (RowEntityType)o;

            List<ResourceContainer> containers = this.ServiceContainer.ResourceContainers.Where(rc => rc.Name.Equals(ret.EntitySet)).ToList();
            if (containers.Count == 0)
                throw new TestFailedException("Could not find EntitySet:" + ret.EntitySet);

            return containers.First();
        }

        public override void RestoreData()
        {
            RequestUtil.GetAndVerifyStatusCode(this, this.ServiceUri + "/RestoreData", HttpStatusCode.NoContent);
        }

        public void ResetMetadata()
        {
            string url = this.ServiceUri + "/ResetMetadata";
            this.ExecuteServiceOp(url);
            this.ServiceContainer = (Activator.CreateInstance(this.GetType()) as NonClrWorkspace).ServiceContainer;
            this.RestoreData();
        }

        public void ResourceTypeComplete(string name)
        {
            string url = this.ServiceUri + String.Format("/ResourceTypeComplete?name='{0}'", name);
            this.ExecuteServiceOp(url);
        }

        public ResourceType AddResourceType(string name, ResourceTypeKind resourceTypeKind, bool isOpenType)
        {
            return this.AddResourceType(name, "RowEntityType", resourceTypeKind, isOpenType, null, null, false);
        }

        public ResourceType AddResourceType(string name, string typeName, ResourceTypeKind resourceTypeKind, bool isOpenType,
                                    string baseType, string namespaceName, bool isAbstract)
        {
            //TODO: Add to existing container (possible?)

            // add to service container
            ResourceType baseResourceType = null;
            if (baseType != null)
            {
                foreach (ResourceContainer rc in this.ServiceContainer.ResourceContainers)
                {
                    foreach (ResourceType rt in rc)
                    {
                        if (rt.Name == baseType)
                        {
                            baseResourceType = rt;
                            break;
                        }
                    }
                }
            }

            ResourceType newResourceType = null;

            if (baseResourceType == null)
                newResourceType = Resource.ResourceType(name, namespaceName);
            else
                newResourceType = Resource.ResourceType(name, namespaceName, baseResourceType);

            // call service op to add to metadata
            string url = this.ServiceUri + String.Format("/AddResourceType?name='{0}'&typeName='{1}'&resourceTypeKind='{2}'&isOpenType={3}" +
                                                         "{4}{5}&isAbstract=false",
                                                         name, typeName, resourceTypeKind.ToString(), isOpenType.ToString().ToLowerInvariant(),
                                                         baseType == null ? "" : "&baseType='" + baseType + "'",
                                                         namespaceName == null ? "&namespaceName='" + this.ContextNamespace + "'" : "&namespaceName='" + namespaceName + "'");
            this.ExecuteServiceOp(url);

            return newResourceType;
        }

        public ResourceContainer AddResourceSet(string name, ResourceType baseResourceType, params ResourceType[] resourceTypes)
        {
            // add to servicecontainer

            ResourceContainer newContainer = Resource.ResourceContainer(name, baseResourceType, resourceTypes);
            this.ServiceContainer.ResourceContainers.Add(newContainer);

            foreach (ResourceType type in resourceTypes)
            {
                this.ServiceContainer.AddNode(type);
            }

            // call service op to add to metadata
            string url = this.ServiceUri + String.Format("/AddResourceSet?name='{0}'&typeName='{1}'", name, baseResourceType.Name);
            this.ExecuteServiceOp(url);

            return newContainer;
        }

        public ResourceProperty AddResourceProperty(string propertyName, ResourceType resourceType, ResourcePropertyKind resourcePropertyKind,
                                        NodeType propertyType, string containerName, bool IsClrProperty)
        {
            ResourceProperty newProperty = null;
            string propertyTypeName = null;

            if (resourcePropertyKind == ResourcePropertyKind.ComplexType)
            {
                newProperty = Resource.Property(propertyName, propertyType);
                propertyTypeName = propertyType.Name;
            }
            else if (resourcePropertyKind == ResourcePropertyKind.Primitive)
            {
                newProperty = Resource.Property(propertyName, propertyType);
                propertyTypeName = propertyType.ClrType.FullName;
            }
            else if (resourcePropertyKind == ResourcePropertyKind.Key)
            {
                newProperty = Resource.Property(propertyName, propertyType, Resource.Key());
                resourceType.Key = new PrimaryKey(propertyName, newProperty);
                propertyTypeName = propertyType.ClrType.FullName;
            }
            else if (resourcePropertyKind == ResourcePropertyKind.ResourceReference)
            {
                // TODO: does this work?
                ResourceAssociationEnd zeroRole = Resource.End(propertyType.Name, (ResourceType)propertyType, Multiplicity.Zero);
                ResourceAssociationEnd manyRole = Resource.End(resourceType.Name, resourceType, Multiplicity.Many);
                ResourceAssociation fk = Resource.Association("FK_" + propertyType.Name + "_" + resourceType.Name, zeroRole, manyRole);

                newProperty = Resource.Property(propertyName, propertyType, fk, manyRole, zeroRole);
                propertyTypeName = propertyType.Name;
            }
            else if (resourcePropertyKind == ResourcePropertyKind.ResourceSetReference)
            {
                // TODO: does this work?
                ResourceAssociationEnd zeroRole = Resource.End(resourceType.Name, resourceType, Multiplicity.Zero);
                ResourceAssociationEnd manyRole = Resource.End(propertyType.Name, (ResourceType)propertyType, Multiplicity.Many);
                ResourceAssociation fk = Resource.Association("FK_" + resourceType.Name + "_" + propertyType.Name, zeroRole, manyRole);

                newProperty = Resource.Property(propertyName, Resource.Collection(propertyType), fk, zeroRole, manyRole);
                propertyTypeName = propertyType.Name;
            }

            resourceType.Properties.Add(newProperty);

            // call service op to add to metadata
            string url = this.ServiceUri + String.Format("/AddResourceProperty?propertyName='{0}'&addToResourceType='{1}'&resourcePropertyKind='{2}'" +
                                             "&propertyType='{3}'{4}&isClrProperty={5}",
                                             propertyName, resourceType.Name, resourcePropertyKind.ToString(),
                                             propertyTypeName, containerName == null ? "" : "&containerName='" + containerName + "'",
                                             IsClrProperty.ToString().ToLowerInvariant());

            this.ExecuteServiceOp(url);

            return newProperty;
        }

        public void AddServiceOperation(string name, ServiceOperationResultKind kind, string typeName)
        {
            // call service op to add to metadata
            string url = this.ServiceUri + String.Format("/AddServiceOperation?name='{0}'&serviceOperationResultKind='{1}'&typeName='{2}'", name, kind.ToString(), typeName);
            this.ExecuteServiceOp(url);
        }

        public void RemoveServiceOperation(string name)
        {
            // call service op to remove from metadata
            string url = this.ServiceUri + String.Format("/RemoveServiceOperation?name='{0}'", name);
            this.ExecuteServiceOp(url);
        }

        public string ExecuteServiceOp(string url)
        {
            AstoriaRequest request = this.CreateRequest();

            request.URI = url;
            request.Accept = "*/*";

            AstoriaResponse response = request.GetResponse();
            if (response.ActualStatusCode != HttpStatusCode.OK && response.ActualStatusCode != HttpStatusCode.NoContent)
                ResponseVerification.LogFailure(response, new Exception("Unexpected status code"));

            return response.Payload;
        }

        public bool SkipContentVerification
        {
            get;
            set;
        }

        public virtual void HandleResponseEvent(object sender, ResponseEventArgs e)
        {
            AstoriaResponse response = e.Response;
            AstoriaRequest request = response.Request;

            KeyExpression keyExp = null;
            RowEntityType entityRow = null;
            PredicateExpression exp = null;

            bool linkUpdate = false;
            string linkProperty = "";

            // don't track error responses
            if ((int)response.ActualStatusCode >= 400)
                return;

            // special case for ETags
            if (response.ActualStatusCode == System.Net.HttpStatusCode.NotModified)
                return;

            // don't track changes that would fail
            if (request.ErrorExpected)
                return;
            if (request.DefaultExpectedStatusCode() != request.ExpectedStatusCode)
                return;

            // Can't handle updates withouth update tree or queries
            if (request.UpdateTree == null && request.Payload != null)
                this.SkipContentVerification = true;

            if (request.Query == null)
                this.SkipContentVerification = true;

            if (this.SkipContentVerification)
                return;

            RequestVerb verb = request.Verb;
            if (request.TunnelledVerb != null)
                verb = (RequestVerb)request.TunnelledVerb;

            switch (verb)
            {
                case RequestVerb.Delete:
                    DeleteParams deleteParams = DeleteVisit(null, request.Query, new DeleteParams());

                    if (deleteParams.LinkUpdate)
                    {
                        entityRow = FindRowInstance(deleteParams.Source);
                        List<RowEntityType> resourceList = _resources[entityRow.EntitySet];

                        if (deleteParams.Property.Type is CollectionType)
                        {
                            // Delete /FailureSet(1)/Configs/$ref?$id=Configs(1)
                            RowEntityType removeRow = FindRowInstance(deleteParams.Target);
                            ((IList)entityRow.Properties[deleteParams.Property.Name]).Remove(removeRow);
                        }
                        else
                        {
                            // Delete /Vehicles(1)/OldDrivers/$ref
                            entityRow.Properties[deleteParams.Property.Name] = null;
                        }
                    }
                    else
                    {
                        if (deleteParams.Property == null)
                        {
                            // Delete /Vehicles(1)
                            entityRow = FindRowInstance(deleteParams.Target);

                            List<RowEntityType> resourceList = _resources[entityRow.EntitySet];
                            resourceList.Remove(entityRow);
                        }
                        else if (deleteParams.Property.IsNavigation)
                        {
                            if (deleteParams.Property.Type is CollectionType)
                            {
                                // Delete /FailureSet(1)/Configs(1)
                                entityRow = FindRowInstance(deleteParams.Source);
                                RowEntityType propertyEntityRow = FindRowInstance(deleteParams.Target);

                                // remove from Source entity's property list
                                ((IList)entityRow.Properties[deleteParams.Property.Name]).Remove(propertyEntityRow);

                                // remove entity from its own list
                                List<RowEntityType> resourceList = _resources[entityRow.EntitySet];
                                resourceList.Remove(propertyEntityRow);
                            }
                            else
                            {
                                // Delete /Vehicles(1)/OldDrivers
                                entityRow = FindRowInstance(deleteParams.Target);

                                RowEntityType propertyEntityRow = (RowEntityType)entityRow.Properties[deleteParams.Property.Name];

                                List<RowEntityType> resourceList = _resources[entityRow.EntitySet];
                                resourceList.Remove(propertyEntityRow);

                                entityRow.Properties[deleteParams.Property.Name] = null;
                            }
                        }
                        else if (deleteParams.IsValue)
                        {
                            entityRow.Properties[deleteParams.Property.Name] = null;
                        }
                    }

                    break;

                case RequestVerb.Post:
                    if (request.Query.Input.Type is ResourceType)
                    {
                        AddNewEntity((KeyedResourceInstance)request.UpdateTree, (ResourceType)request.Query.Input.Type);
                    }
                    else if (request.Query.Input.Type is ResourceCollection)
                    {
                        NavigationExpression ne = (NavigationExpression)request.Query.Input;
                        linkProperty = ne.Property.Name;

                        keyExp = (KeyExpression)((PredicateExpression)ne.Input).Predicate;
                        entityRow = FindRowInstance(keyExp);

                        AddEntityToCollection((KeyedResourceInstance)request.UpdateTree, entityRow, linkProperty);
                    }

                    break;

                case RequestVerb.Patch:
                case RequestVerb.Put:

                    bool replace = false;

                    if (request.Verb == RequestVerb.Put)
                        replace = true;

                    if (request.Query is PredicateExpression)
                        exp = (PredicateExpression)request.Query;
                    else if (request.Query is ProjectExpression)
                    {
                        ProjectExpression projectExp = (ProjectExpression)request.Query;

                        if (projectExp.Input is NavigationExpression)
                        {
                            NavigationExpression ne = (NavigationExpression)projectExp.Input;
                            linkProperty = ne.Property.Name;
                            exp = (PredicateExpression)ne.Input;
                            linkUpdate = true;
                        }
                        else if (projectExp.Input is PredicateExpression)
                        {
                            exp = (PredicateExpression)projectExp.Input;
                        }
                        else
                        {
                            throw new TestException(TestResult.Warning, "Unhandled query expression - " + request.Query.GetType().Name);
                        }
                    }
                    else
                        throw new TestException(TestResult.Warning, "Unhandled query expression - " + request.Query.GetType().Name);

                    keyExp = (KeyExpression)exp.Predicate;
                    entityRow = FindRowInstance(keyExp);

                    if (request.UpdateTree is ResourceInstanceSimpleProperty)
                    {
                        ResourceInstanceSimpleProperty singleProperty = (ResourceInstanceSimpleProperty)request.UpdateTree;
                        ModifyProperty(singleProperty, entityRow);
                    }
                    else if (request.UpdateTree is ResourceInstanceComplexProperty)
                    {
                        ResourceInstanceComplexProperty complexProperty = (ResourceInstanceComplexProperty)request.UpdateTree;
                        ModifyProperty(complexProperty, entityRow);
                    }
                    else if (request.UpdateTree is KeyedResourceInstance)
                    {
                        KeyedResourceInstance updateTree = (KeyedResourceInstance)request.UpdateTree;

                        if (linkUpdate)
                            UpdateLink(updateTree, entityRow, linkProperty);
                        else
                            ModifyProperties(updateTree, entityRow, replace);
                    }
                    else
                        throw new TestException(TestResult.Failed, "Unhandled UpdateTree - " + request.UpdateTree.GetType().Name);


                    break;

                default:
                    break;
            }
        }

        private DeleteParams DeleteVisit(ExpNode caller, ExpNode node, DeleteParams deleteParams)
        {
            if (node is PredicateExpression)
            {
                PredicateExpression e = (PredicateExpression)node;

                if (caller is NavigationExpression)
                    deleteParams.Source = (KeyExpression)e.Predicate;
                else
                    deleteParams.Target = (KeyExpression)e.Predicate;

                deleteParams = DeleteVisit(e, e.Input, deleteParams);
            }
            else if (node is ProjectExpression)
            {
                ProjectExpression e = (ProjectExpression)node;
                if (e.Projections.Count == 1 && e.Projections[0] is PropertyExpression)
                {
                    deleteParams = DeleteVisit(e, e.Projections[0], deleteParams);
                }
                deleteParams = DeleteVisit(e, e.Input, deleteParams);
            }
            else if (node is NavigationExpression)
            {
                NavigationExpression e = (NavigationExpression)node;
                deleteParams.LinkUpdate = e.IsLink;

                deleteParams = DeleteVisit(e, e.Input, deleteParams);
                deleteParams = DeleteVisit(e, e.PropertyExp, deleteParams);
            }
            else if (node is PropertyExpression)
            {
                PropertyExpression e = (PropertyExpression)node;
                deleteParams.Property = (ResourceProperty)e.Property;
                deleteParams.IsValue = e.ValueOnly;
            }

            return deleteParams;
        }

        private RowEntityType AddNewEntity(KeyedResourceInstance updateTree, ResourceType entityType)
        {
            List<RowEntityType> resourceList = _resources[updateTree.ResourceSetName];
            RowEntityType newResource = new RowEntityType(updateTree.ResourceSetName, updateTree.TypeName);

            foreach (ResourceInstanceProperty keyProperty in updateTree.KeyProperties)
            {
                ResourceInstanceSimpleProperty tempProperty = (ResourceInstanceSimpleProperty)keyProperty;
                newResource.Properties.Add(tempProperty.Name, tempProperty.PropertyValue);
            }

            AddProperties(updateTree, newResource);

            if (entityType != null)
                AddNavigationProperties(entityType, newResource);

            resourceList.Add(newResource);

            return newResource;
        }

        private RowEntityType FindRowInstance(string setName, List<string> propertyNames, List<object> propertyValues)
        {
            if (propertyNames.Count != propertyValues.Count)
                throw new TestFailedException("Property names/values mismatch");

            if (!_resources.ContainsKey(setName))
                return null;

            List<RowEntityType> resourceList = _resources[setName];

            foreach (RowEntityType row in resourceList)
            {
                bool matchFound = true;
                for (int index = 0; index < propertyNames.Count; index++)
                {
                    string propertyName = propertyNames[index];
                    object keyValue = propertyValues[index];
                    object value;
                    if (!row.Properties.TryGetValue(propertyName, out value) || (value == null && keyValue != null) || !value.Equals(keyValue))
                    {
                        matchFound = false;
                        break;
                    }
                }
                if (matchFound)
                    return row;
            }

            return null;
        }

        private RowEntityType FindRowInstance(KeyExpression keyExp)
        {
            RowEntityType type = FindRowInstance(keyExp.ResourceContainer.Name,
                keyExp.Properties.Select(p => p.Name).ToList(),
                keyExp.Values.Select(v => v.ClrValue).ToList());

            if (type != null)
                return type;

            throw new TestException(TestResult.Failed, "Could not find row instance for keyExpression: "
               + keyExp.ResourceContainer.Name + "(" + UriQueryBuilder.CreateKeyString(keyExp, false) + ")");
        }

        private RowEntityType FindRowInstance(KeyedResourceInstance updateTree)
        {
            return FindRowInstance(updateTree.ResourceSetName,
                updateTree.KeyProperties.OfType<ResourceInstanceSimpleProperty>().Select(p => p.Name).ToList(),
                updateTree.KeyProperties.OfType<ResourceInstanceSimpleProperty>().Select(p => p.PropertyValue).ToList());
        }

        private void ModifyProperty(ResourceInstanceSimpleProperty property, RowComplexType newResource)
        {
            newResource.Properties[property.Name] = property.PropertyValue;
        }

        private void ModifyProperty(ResourceInstanceComplexProperty property, RowComplexType newResource)
        {
            ModifyProperties(property.ComplexResourceInstance, (RowComplexType)newResource.Properties[property.Name], false);
        }

        private void ModifyProperties(ComplexResourceInstance updateTree, RowComplexType newResource, bool replace)
        {
            foreach (ResourceInstanceProperty property in updateTree.Properties)
            {
                if (property is ResourceInstanceSimpleProperty)
                {
                    ResourceInstanceSimpleProperty tempProperty = (ResourceInstanceSimpleProperty)property;
                    newResource.Properties[tempProperty.Name] = tempProperty.PropertyValue;
                }
                else if (property is ResourceInstanceComplexProperty)
                {
                    ResourceInstanceComplexProperty tempProperty = (ResourceInstanceComplexProperty)property;
                    RowComplexType newComplexType = new RowComplexType(tempProperty.TypeName);
                    ModifyProperties(tempProperty.ComplexResourceInstance, newComplexType, replace);
                    newResource.Properties[tempProperty.Name] = newComplexType;
                }
            }

            if (replace)
            {
                string[] keys = newResource.Properties.Keys.ToArray();
                foreach (string key in keys)
                {
                    if (updateTree.Properties.Where(p => p.Name == key).Count() == 0)
                    {
                        if (updateTree is KeyedResourceInstance)
                        {
                            KeyedResourceInstance keyedResource = (KeyedResourceInstance)updateTree;
                            if (keyedResource.KeyProperties.Where(p => p.Name == key).Count() == 0)
                                newResource.Properties[key] = null;
                        }
                        else
                            newResource.Properties[key] = null;
                    }
                }
            }
        }

        private void AddProperties(ComplexResourceInstance updateTree, RowComplexType newResource)
        {
            foreach (ResourceInstanceProperty property in updateTree.Properties)
            {
                if (property is ResourceInstanceSimpleProperty)
                {
                    ResourceInstanceSimpleProperty tempProperty = (ResourceInstanceSimpleProperty)property;
                    newResource.Properties.Add(tempProperty.Name, tempProperty.PropertyValue);
                }
                else if (property is ResourceInstanceComplexProperty)
                {
                    ResourceInstanceComplexProperty tempProperty = (ResourceInstanceComplexProperty)property;
                    RowComplexType newComplexType = new RowComplexType(tempProperty.TypeName);
                    AddProperties(tempProperty.ComplexResourceInstance, newComplexType);
                    newResource.Properties.Add(tempProperty.Name, newComplexType);
                }
                else if (property is ResourceInstanceNavRefProperty)
                {
                    ResourceInstanceNavRefProperty tempProperty = (ResourceInstanceNavRefProperty)property;
                    RowEntityType nestedResource = FindRowInstance((KeyedResourceInstance)tempProperty.TreeNode);

                    if (nestedResource == null)
                        nestedResource = AddNewEntity((KeyedResourceInstance)tempProperty.TreeNode, null);

                    newResource.Properties.Add(tempProperty.Name, nestedResource);
                }
                else if (property is ResourceInstanceNavColProperty)
                {
                    ResourceInstanceNavColProperty tempProperty = (ResourceInstanceNavColProperty)property;

                    List<RowEntityType> entityList = new List<RowEntityType>();

                    foreach (ResourceBodyTree entity in tempProperty.Collection.NodeList)
                    {
                        RowEntityType nestedResource = FindRowInstance((KeyedResourceInstance)entity);

                        if (nestedResource == null)
                            nestedResource = AddNewEntity((KeyedResourceInstance)entity, null);

                        entityList.Add(nestedResource);
                    }

                    newResource.Properties.Add(tempProperty.Name, entityList);
                }
                else
                    throw new TestException(TestResult.Failed, "NonClr - Unhandled property type.");
            }
        }

        private void AddNavigationProperties(ResourceType resourceType, RowComplexType newResource)
        {
            foreach (ResourceProperty property in resourceType.Properties)
            {
                if (property.IsNavigation && !newResource.Properties.ContainsKey(property.Name))
                {
                    newResource.Properties.Add(property.Name, null);

                    if (property.Type is CollectionType)
                        newResource.Properties[property.Name] = new List<RowEntityType>();
                }
            }
        }

        private bool AddEntityToCollection(KeyedResourceInstance updateTree, RowEntityType entityType, string linkProperty)
        {
            RowEntityType linkedEntity = FindRowInstance(updateTree);
            if (linkedEntity == null)
                throw new TestFailedException("Could not find row instance for adding entity to collection");

            //need to make sure its not already in the list
            List<RowEntityType> list = entityType.Properties[linkProperty] as List<RowEntityType>;
            if (list == null)
                entityType.Properties[linkProperty] = list = new List<RowEntityType>();

            foreach (RowEntityType e in list)
            {
                // if its actually the same reference, then the keys must match
                if (e == linkedEntity)
                    return false;

                // check the keys if its not the same reference
                bool match = true;
                foreach (ResourceInstanceSimpleProperty property in updateTree.KeyProperties)
                {
                    object value;
                    if (e.Properties.TryGetValue(property.Name, out value))
                    {
                        if (value != property.PropertyValue)
                        {
                            match = false;
                            break;
                        }
                    }
                }

                if (match)
                    return false;
            }

            list.Add(linkedEntity);
            return true;
        }

        private bool UpdateLink(KeyedResourceInstance updateTree, RowEntityType entityType, string linkProperty)
        {
            RowEntityType linkedEntity = FindRowInstance(updateTree);
            if (linkedEntity == null)
                throw new TestFailedException("Could not find row instance for updating link");
            entityType.Properties[linkProperty] = linkedEntity;
            return true;
        }

        private void DataInserter_HandleAddingEntity(KeyExpression key, KeyedResourceInstance entity)
        {
            AddNewEntity(entity, key.ResourceType);
        }

        private void AddAssociation_Internal(KeyedResourceInstance parent, ResourceProperty property, KeyedResourceInstance child, bool addReverseLink)
        {
            bool changed;
            if (property.OtherAssociationEnd.Multiplicity == Multiplicity.Many)
                changed = AddEntityToCollection(child, FindRowInstance(parent), property.Name);
            else
                changed = UpdateLink(child, FindRowInstance(parent), property.Name);

            if (changed && addReverseLink && property.OtherSideNavigationProperty != null)
                AddAssociation_Internal(child, property.OtherSideNavigationProperty, parent, false);
        }

        private void DataInserter_HandleAddingAssociation(KeyedResourceInstance parent, ResourceProperty property, KeyedResourceInstance child)
        {
            AddAssociation_Internal(parent, property, child, true);
        }

        private bool _clrPropertiesDefined = false;
        public virtual void DefineClrProperties()
        {
            if (_clrPropertiesDefined)
                return;
            _clrPropertiesDefined = true;

            const int skipChance = 25;

            int skipAll = AstoriaTestProperties.Random.Next(skipChance);
            if (skipAll == skipChance - 1)
                return;

            // Use our own random number generator.
            Random rnd = new Random(AstoriaTestProperties.Seed);

            foreach (ResourceType resourceType in this.ServiceContainer.ResourceTypes)
            {
                if (resourceType.BaseTypes.Any(ct => ct.Facets.IsClrType))
                    resourceType.Facets.IsClrType = true;

                bool allClr = false;
                int seed = 0;

                int val = rnd.Next(4);
                if (val == 0)
                    continue;           // none
                else if (val == 1)
                    allClr = true;      // all
                else if (val == 2)
                    seed = 2;           // many
                else
                    seed = 10;          // few

                resourceType.Facets.IsClrType = true;
                foreach (ResourceProperty resourceProperty in resourceType.Properties)
                {
                    if (resourceType.BaseType != null && resourceType.BaseType.Properties.Any(p => p.Name == resourceProperty.Name))
                        continue;

                    if (resourceProperty.IsComplexType)
                        continue;

                    if (allClr || rnd.Next(seed) == (seed - 1))
                    {
                        resourceProperty.Facets.IsClrProperty = true;
                        foreach (ComplexType derived in resourceType.DerivedTypes.Cast<ComplexType>().Union(resourceType.BaseTypes))
                        {
                            // if the derived types have a property with the same name
                            // then make sure that one is a clr property as well
                            // (note that this only happens with nav-props right now, due to
                            // how they are set up in the Aruba___Workspace-style classes)
                            NodeProperty p = derived.Properties[resourceProperty.Name];
                            if (p != null && p != resourceProperty)
                                p.Facets.IsClrProperty = true;
                        }
                    }
                }

                foreach (ResourceType derived in resourceType.DerivedTypes)
                    derived.Facets.IsClrType = true;
            }
        }

        private OpenTypeMethodsImplementations _openTypeMethodsImplementation = OpenTypeMethodsImplementations.Default;
        public virtual OpenTypeMethodsImplementations OpenTypeMethodsImplementation
        {
            get
            {
                return _openTypeMethodsImplementation;
            }
            set
            {
                if (value != _openTypeMethodsImplementation)
                {
                    _openTypeMethodsImplementation = value;
                    SetOpenTypeMethodsImplementation();
                }
            }
        }

        private bool _openTypeMethodsLazyBooleanEvaluation = false;
        public virtual bool OpenTypeMethodsLazyBooleanEvaluation
        {
            get
            {
                return _openTypeMethodsLazyBooleanEvaluation;
            }
            set
            {
                if (value != _openTypeMethodsLazyBooleanEvaluation)
                {
                    _openTypeMethodsLazyBooleanEvaluation = value;
                    SetOpenTypeMethodsImplementation();
                }
            }
        }

        public void SetOpenTypeMethodsImplementation(OpenTypeMethodsImplementations implementation, bool lazyEvaluation)
        {
            _openTypeMethodsImplementation = implementation;
            _openTypeMethodsLazyBooleanEvaluation = lazyEvaluation;
            SetOpenTypeMethodsImplementation();
        }

        private void SetOpenTypeMethodsImplementation()
        {
            string url = this.ServiceUri + "/SetOpenTypeMethodsImplementation?name='" + OpenTypeMethodsImplementation.ToString()
                + "'&lazyEvaluation=" + OpenTypeMethodsLazyBooleanEvaluation.ToString().ToLowerInvariant();
            RequestUtil.GetAndVerifyStatusCode(this, url, HttpStatusCode.NoContent);
        }
    }

    public class DeleteParams
    {
        public KeyExpression Source
        {
            get;
            set;
        }

        public KeyExpression Target
        {
            get;
            set;
        }

        public bool LinkUpdate
        {
            get;
            set;
        }

        public ResourceProperty Property
        {
            get;
            set;
        }

        public bool IsValue
        {
            get;
            set;
        }
    }
}
