//---------------------------------------------------------------------
// <copyright file="Workspace.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
#if !SilverlightTestFramework
    using System.Data.Test.Astoria.TestExecutionLayer;
#endif
    using System.Xml;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.ModuleCore; //TestFailedException;
    using System.Reflection;
    using System.Text;
    using System.Linq.Expressions;
    using System.Data.Test.Astoria.ReflectionProvider;

    using System.Security.Permissions;
    using System.Security;
    using System.Net;

    ////////////////////////////////////////////////////////
    // Workspace
    //
    ////////////////////////////////////////////////////////   
    [WorkspaceDefaultSettings]
    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    [SecuritySafeCritical]
    public abstract class Workspace : fxBase, IDisposable
    {
        #region service/workspace creation 'events'
        // we do lists so that order is fixed, but can also be easily modified
        public List<Action> BeforeServiceCreation
        {
            get;
            private set;
        }

        public List<Action> AfterServiceCreation
        {
            get;
            private set;
        }
        #endregion

        public abstract void ApplyFriendlyFeeds();

#if !SilverlightTestFramework
        public WorkspaceSettings Settings;
#endif

        //Data
        protected NodeTypes _types;
        protected QueryBuilder _querybuilder;
        protected string _csdlFilePath;

        //private WorkspaceLanguage _language;
        private string _contextNamespace;
        private string _contextTypeName;

        private DataLayerProviderKind _dataLayerProviderKind;

        private string _serviceClassName;

        AstoriaDatabase _astoriaDatabase;
        AstoriaWebDataService _astoriaWebDataService;

        protected ServiceContainer _serviceContainer;
        protected internal Type _underlyingContextType;
        protected string _assemblyPath;

        protected internal AstoriaHashTable<ResourceType, Type> _resourceTypeToWorkspaceTypeList;
        protected internal AstoriaHashTable<ResourceType, Type> _resourceTypeToClientTypeList;

        private bool _disposed = false;

        public IDataGenerator DataGenerator
        {
            get;
            protected set;
        }

        /// <summary>
        /// Set this to true if the client types have to be generated at runtime
        /// </summary>
        public bool GenerateClientTypes { get; set; }

        private bool _generateServerMappings = true;
        /// <summary>
        /// Set this to true if the Server EPM Mappints are to be generated at runtime
        /// </summary>
        public bool GenerateServerMappings
        {
            get
            {
                return _generateServerMappings;
            }

            set
            {
                _generateServerMappings = value;
            }
        }

        protected internal virtual string WebServiceObjectLayerFileName
        {
            get { return this.Name + this.LanguageExtension; }
        }

        private string _webServiceName = null;
        public virtual string WebServiceName
        {
            get
            {
                if (_webServiceName == null)
                    return this.Name;
                return _webServiceName;
            }
            set
            {
                _webServiceName = value;
            }
        }

        protected internal virtual string WebServiceFileName
        {
            get { return this.Name + ".svc"; }
        }
        protected internal virtual string DatabasePrefixName
        {
            get { return this.Name; }
        }

        protected internal virtual string WebServiceWorkspaceDir
        {
            get { return this.HostSourceFolder; }
        }

        protected internal virtual string WebServiceAppCodeDir
        {
            get { return Path.Combine(this.WebServiceWorkspaceDir, "App_Code"); }
        }

        public virtual string WebServiceBinDir
        {
            get { return Path.Combine(this.WebServiceWorkspaceDir, "Bin"); }
        }

        protected internal virtual string ServiceResourceName
        {
            get { return this.WebServiceObjectLayerFileName; }
        }
        protected internal virtual string ServiceOutputFileName
        {
            get { return this.WebServiceObjectLayerFileName; }
        }

        public ServiceModifications ServiceModifications
        {
            get;
            private set;
        }

        protected void WritePartialClassAttributes(string ns, CSharpCodeLanguageHelper codeHelper)
        {
            codeHelper.WriteStartNamespace(ns);

            foreach (ResourceType resourceType in this.ServiceContainer.ResourceTypes)
            {
                foreach (ResourceAttribute resourceAttrib in resourceType.Facets.Attributes)
                {
                    resourceAttrib.Apply(codeHelper);
                }

                codeHelper.WriteLine();
                codeHelper.WriteBeginClass(resourceType.Name, null, null, false);
                codeHelper.WriteEndClass(resourceType.Name);
                codeHelper.WriteLine();
            }

            codeHelper.WriteEndNamespace(this.ContextNamespace);
        }

        protected internal virtual string ObjectLayerResourceName
        {
            get { return this.Name + ".ObjectLayer" + this.LanguageExtension; }
        }

        protected internal virtual string ObjectLayerOutputFileName
        {
            get { return this.Name + ".ObjectLayer" + this.LanguageExtension; }
        }

        private HashSet<string> _objectLayerFileNames = null;
        protected internal virtual HashSet<string> ObjectLayerFileNames
        {
            get
            {
                if (_objectLayerFileNames == null)
                {
                    _objectLayerFileNames = new HashSet<string>();
                    _objectLayerFileNames.Add(Path.Combine(Path.Combine(this.HostSourceFolder, "App_Code"), this.ObjectLayerOutputFileName));
                }
                return _objectLayerFileNames;
            }
        }

        public static event EventHandler<NewWorkspaceEventArgs> CreateNewWorkspaceEvent;

        protected virtual void OnCreateNewWorkspaceEvent(NewWorkspaceEventArgs e)
        {
            EventHandler<NewWorkspaceEventArgs> handler = CreateNewWorkspaceEvent;

            if (handler != null)
                handler(this, e);
        }

        //Constructor
        public Workspace(DataLayerProviderKind kind, String name)
            : base(name)
        {
            AstoriaTestLog.WriteLineIgnore("Constructing workspace of type '" + this.GetType() + "' with name '" + this.Name + "'.");

            OnCreateNewWorkspaceEvent(new NewWorkspaceEventArgs(this));

            DataLayerProviderKind = kind;
            _serviceClassName = this.Name + "Service";
            _resourceTypeToWorkspaceTypeList = new AstoriaHashTable<ResourceType, Type>();
            _resourceTypeToClientTypeList = new AstoriaHashTable<ResourceType, Type>();
            Settings = new WorkspaceSettings(this);

            BeforeServiceCreation = new List<Action>();
            AfterServiceCreation = new List<Action>();

            BeforeServiceCreation.Add(this.InferAssociations);

            if (AstoriaTestProperties.UseOpenTypes)
                BeforeServiceCreation.Add(() => OpenTypesUtil.SetupDefaultOpenTypeAttributes(this));

            BeforeServiceCreation.Add(this.GenerateCallOrderInterceptorCode);

            BeforeServiceCreation.Add(this.GenerateCustomInterceptors);

            BeforeServiceCreation.Add(this.PopulateHostSourceFolder);

            // these can happen before or after, but lets do them before in case other events need them to be done already
            BeforeServiceCreation.Add(this.CreateLocalAssembly);
            BeforeServiceCreation.Add(this.PopulateClientTypes);

            this.DataGenerator = null;

            this.RequiredFrameworkSources = new List<string>() { "TestDataWebService.cs", "APICallLog.cs", "ProviderWrappers.cs" };
            this.ServiceModifications = new ServiceModifications(this);
        }

        public Workspace(DataLayerProviderKind kind, String name, string contextNamespace, string contextTypeName)
            : this(kind, name)
        {
            _contextNamespace = contextNamespace;
            _contextTypeName = contextTypeName;
        }

        ~Workspace()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (this.Database != null)
                {
                    this.Database.Dispose();
                    this.Database = null;
                }

                if (disposing)
                {
                    if (this.DataService != null)
                    {
                        this.DataService.Dispose();
                        this.DataService = null;
                    }
                }
                _disposed = true;
            }
        }

        public virtual void AddServiceOperationCode()
        {
            StringBuilder sbServiceOpCode = new StringBuilder();
            foreach (ServiceOperation serviceOp in this.ServiceContainer.ServiceOperations.Where(sop => sop.BackingType != ServiceOpBackingType.PrimitiveType))
            {
                sbServiceOpCode.AppendFormat("\r\n {0} \r\n", serviceOp.ServiceOpCode);
            }
            this.ServiceAdditionalCode += sbServiceOpCode.ToString();
        }

        public virtual void RemoveServiceOps()
        {
            List<ServiceOperation> currentServiceOps = this.ServiceContainer.ServiceOperations.ToList<ServiceOperation>();
            currentServiceOps.ForEach(
                sOp =>
                {
                    this.ServiceContainer.RemoveNode((ResourceContainer)sOp);
                }
                );
        }

        public virtual void Create()
        {
            // TODO: move this check/fixup to somewhere more specific to whatever is using it
            if (GenerateClientTypes)
            {
                ////RemoveSetupEvent(this.CreateLocalAssembly);
                //RemoveSetupEvent(this.PopulateClientTypes);
                //AddCreateEvent(this.GenerateAndLoadClientTypes);
                //AddCreateEvent(this.PopulateClientTypes);

                bool foundPopulateClientTypes = false;
                for (int i = 0; i < AfterServiceCreation.Count; i++)
                {
                    // find PopulateClientTypes in the list, and add the generation step beforehand
                    if (AfterServiceCreation[i] == this.PopulateClientTypes)
                    {
                        AfterServiceCreation.Insert(i, this.GenerateAndLoadClientTypes);
                        foundPopulateClientTypes = true;
                        break;
                    }
                }
                if (!foundPopulateClientTypes)
                {
                    AfterServiceCreation.Insert(AfterServiceCreation.Count, this.GenerateAndLoadClientTypes);
                    AfterServiceCreation.Insert(AfterServiceCreation.Count, this.PopulateClientTypes);
                }
            }

            BeforeServiceCreation.ForEach(a => a());

            if (this.DataGenerator != null && this.DataGenerator.DataInserter.BeforeServiceCreation)
                this.DataGenerator.Run();

            DataService = AstoriaWebDataService.CreateAstoriaDataWebService(this);

            if (this.DataGenerator != null && !this.DataGenerator.DataInserter.BeforeServiceCreation)
                this.DataGenerator.Run();

            AfterServiceCreation.ForEach(a => a());
        }

        public virtual void InferAssociations()
        {
            foreach (ResourceType t in this.ServiceContainer.ResourceTypes)
                t.InferAssociations();
        }

        private String[] GetCommandLineSwitches(String uri, String csdl, String outFile, String language, String nmspace)
        {
            List<String> paramsList = new List<String> { };
            if (uri != null)
                paramsList.Add("/uri:" + uri);
            if (csdl != null)
                paramsList.Add("/in:" + csdl);
            if (outFile != null)
                paramsList.Add("/out:" + outFile);
            if (language != null)
                paramsList.Add("/language:" + language);
            if (nmspace != null)
                paramsList.Add("/namespace:" + nmspace);

            paramsList.Add("/Version:2.0");

            return paramsList.ToArray();
        }

        private string[] ReferencedDlls
        {
            get
            {
                return new string[] 
                { 
                    "System.dll", 
                    "System.Numerics.dll", 
                    "System.Core.dll", 
                    DataFxAssemblyRef.File.DataServicesClient,
                    DataFxAssemblyRef.File.ODataLib,
                };
            }
        }

        public bool GenerateClientTypesManually { get; set; }

        /// <summary>
        /// Generate the client types for a given service 
        /// </summary>
        protected void GenerateAndLoadClientTypes()
        {
            if (GenerateClientTypes)
            {
                string commonProgramFiles = Environment.GetEnvironmentVariable("CommonProgramFiles");
                string t4TransformToolPath = commonProgramFiles + "\\Microsoft Shared\\TextTemplating\\11.0\\TextTransform.exe";
                string newFileNameSeed = Guid.NewGuid().ToString();
                string outFilePath = Path.Combine(Environment.CurrentDirectory, newFileNameSeed + ".cs");
                string partialClassFilePath = Path.Combine(Environment.CurrentDirectory, newFileNameSeed + "Partial.cs");
                string dllOutputPath = Path.Combine(Environment.CurrentDirectory, newFileNameSeed + ".dll");
                string t4TemplatePath = Path.Combine(Environment.CurrentDirectory, "ODataT4CodeGenerator.tt");

                ClientTypesNamespace = this.Name;

                try
                {
                     string arguments = "-out \"" + outFilePath
                        + "\" -a !!MetadataDocumentUri!" + this.ServiceUri
                        + " -a !!UseDataServiceCollection!False"
                        + " -a !!TargetLanguage!CSharp"
                        + " -p \"" + Environment.CurrentDirectory + "\" \"" + t4TemplatePath + "\"";

                    var returnValue = Execute(t4TransformToolPath, arguments, null);
                    AstoriaTestLog.Compare(string.IsNullOrEmpty(returnValue), "Error \n" + returnValue);
                }
                catch (Exception excp)
                {
                    string activationError = excp.Message;
                    System.Net.WebRequest webRequest = System.Net.WebRequest.Create(this.ServiceUri + "/$metadata");
                    webRequest.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                    try
                    {
                        using (System.Net.WebResponse response = webRequest.GetResponse()) { }
                    }
                    catch (System.Net.WebException inner)
                    {
                        using (Stream respStream = inner.Response.GetResponseStream())
                        {
                            StreamReader strReader = new StreamReader(respStream);
                            activationError += Environment.NewLine + strReader.ReadToEnd();
                            AstoriaTestLog.WriteLineIgnore(activationError);
                            throw new TestException(TestResult.Unknown, activationError);
                        }
                    }

                    throw (new TestSkippedException("Failed to generate client types for server :" + this.ServiceUri + "  because of error \r\n" + activationError));
                }

                AstoriaTestLog.Compare(File.Exists(outFilePath), "Cannot find code generated file " + outFilePath);
                List<string> filesToCompile = new List<string>();

                if (GenerateClientTypesManually)
                {
                    StreamWriter textStream = File.CreateText(partialClassFilePath);
                    ReflectionProvider.CSharpCodeLanguageHelper codeHelper
                        = new ReflectionProvider.CSharpCodeLanguageHelper(textStream);
                    if (this.ContextNamespace == "Aruba")
                        WritePartialClassAttributes("ArubaClient", codeHelper);
                    else
                        WritePartialClassAttributes(this.ContextNamespace, codeHelper);
                    textStream.Close();

                    filesToCompile.Add(partialClassFilePath);
                }
                filesToCompile.Add(outFilePath);

                if (this.ContextNamespace == "Aruba")
                {
                    string originalCode = File.ReadAllText(outFilePath);
                    if (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.Edm))
                    {
                        originalCode = originalCode.Replace("namespace Aruba", "namespace ArubaClient");
                    }
                    if (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.NonClr))
                    {
                        originalCode = originalCode.Replace("namespace Aruba.NonClr", "namespace ArubaClient");
                        originalCode = originalCode.Replace("namespace northwind.NonClr", "namespace northwind");
                    }
                    if (AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.LinqToSql)
                        && (!AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.Edm)))
                    {
                        originalCode = originalCode.Replace(String.Format("namespace {0}", this.ContextNamespace), String.Format("namespace {0}ClientLTS", this.ContextNamespace));

                    }
                    FileStream fsOriginalCode = File.Open(outFilePath, FileMode.OpenOrCreate);
                    byte[] Code = System.Text.Encoding.UTF8.GetBytes(originalCode);
                    fsOriginalCode.Write(Code, 0, Code.Length);
                    fsOriginalCode.Close();
                }
                else if (
                        AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.LinqToSql)
                        &&
                        !AstoriaTestProperties.DataLayerProviderKinds.Contains(DataLayerProviderKind.Edm)
                        )
                {
                    string originalCode = File.ReadAllText(outFilePath);
                    originalCode = originalCode.Replace(String.Format("namespace {0}", this.ContextNamespace), String.Format("namespace {0}ClientLTS", this.ContextNamespace));
                    FileStream fsOriginalCode = File.Open(outFilePath, FileMode.Open);
                    byte[] Code = System.Text.Encoding.UTF8.GetBytes(originalCode);
                    fsOriginalCode.Write(Code, 0, Code.Length);
                    fsOriginalCode.Close();
                }

                //Compile the client types into an assembly
                Util.CodeCompilerHelper.CompileCodeFiles(
                    filesToCompile.ToArray(),
                    dllOutputPath,
                    new string[] { this.DataLayerProviderKind.ToString() },
                    this.Language,
                    ReferencedDlls);

                //Load the newly created assembly into this app domain
                ClientTypesAssembly = Assembly.Load(Path.GetFileNameWithoutExtension(dllOutputPath));


            }
        }

        private static string Execute(string filename, string arguments, int? expectedExitCode)
        {
            Process process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return output + error;
        }

        public Assembly ClientTypesAssembly
        {
            get;
            set;
        }

        public string ClientTypesNamespace { get; set; }


        public Type GetClientType(string typeName)
        {
            Type clientType = ClientTypesAssembly.GetType(ClientTypesNamespace + "." + typeName);
            return clientType;
        }
        /// <summary>
        /// Populates the <see cref="HostSourceFolder"/> with the files that should be 
        /// available on the service host.
        /// </summary>
        public virtual void PopulateHostSourceFolder()
        {
            ClearHostSourceFolder();
            File.WriteAllText(Path.Combine(WebServiceAppCodeDir, ServiceOutputFileName), BuildDataServiceClassUsings() + BuildDataServiceClassCode());
            CreateWebDataServiceSvcFile(Path.Combine(WebServiceWorkspaceDir, WebServiceFileName));
        }

        /// <summary>
        /// Populates the DestinationFolder with the files that should be 
        /// available on the service host.
        /// </summary>
        public virtual void RePopulateHostSourceFolder()
        {
            File.WriteAllText(Path.Combine(Path.Combine(this.DataService.DestinationFolder, "App_Code"), ServiceOutputFileName), BuildDataServiceClassUsings() + BuildDataServiceClassCode());
            CreateWebDataServiceSvcFile(Path.Combine(this.DataService.DestinationFolder, WebServiceFileName));
        }


        protected virtual void ClearHostSourceFolder()
        {
            IOUtil.EnsureDirectoryExists(WebServiceWorkspaceDir);
            IOUtil.EnsureDirectoryExists(WebServiceAppCodeDir);
            IOUtil.EnsureDirectoryExists(WebServiceBinDir);

            IOUtil.EmptyDirectoryRecusively(this.HostSourceFolder);
        }

        protected internal virtual void PopulateClientTypes()
        {
        }

        public virtual ResourceType LanguageDataResource()
        {
            return null;
        }

        protected internal virtual void CreateLocalAssembly()
        {
            string path = Path.Combine(Environment.CurrentDirectory, this.Name + "_" + this.DataLayerProviderKind);
            string pathTemplate = path + "_{0}.dll";
            path += ".dll";

            int count = 0;
            while (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    path = string.Format(pathTemplate, count++);
                }
            }
            _assemblyPath = path;

            this.CompileCodeFiles(ObjectLayerFileNames.ToArray(), new string[] { }, _assemblyPath);

            String assemblyName = Path.GetFileNameWithoutExtension(_assemblyPath);
            Assembly assembly = Assembly.Load(assemblyName);
            _underlyingContextType = assembly.GetType(this.ContextNamespace + "." + this.ContextTypeName, false);
            //Assembly ltsAssembly = TypeUtil.GetAssembly(assembly);
            _resourceTypeToWorkspaceTypeList = CreateResourceTypeToUnderlyingClrTypeMap(assembly, this.ContextNamespace);

            SetResourceTypesToClrTypes(assembly, this.ContextNamespace, _resourceTypeToWorkspaceTypeList, false);
        }

        public static string QueryInterceptorTemplate = "[QueryInterceptor(\"{0}\")]\r\n public Expression<Func<{1}.{2},bool>> OnQuery{0}OfT() {{ return {3} ; }}\r\n";
        public static string IQueryableOfTServiceOpTemplate1 = "[WebGet]\r\n public IQueryable<{2}.{0}> Return{3}OfT() {{ return GetEntitySet<{2}.{0}>(\"{1}\"); }}\r\n";
        public static string IQueryableOfTWithOneIncludeServiceOpTemplate = "[WebGet]\r\n public IQueryable<{2}.{0}> Return{3}OfTWithExpandOneLevel() {{ return ((ObjectQuery<{2}.{0}>) GetEntitySet<{2}.{0}>(\"{1}\")){4}; }}\r\n";
        public static string IQueryableOfTWithNIncludeServiceOpTemplate = "[WebGet]\r\n public IQueryable<{2}.{0}> Return{3}OfTWithExpand{5}Level() {{ return ((ObjectQuery<{2}.{0}>) GetEntitySet<{2}.{0}>(\"{1}\")){4}; }}\r\n";

        public static string KeyedWebInvokeServiceOpTemplate = "[WebInvoke(Method=\"{5}\")]\r\n public IQueryable<{2}.{0}> GetKeyed{3}OfT() {{ return GetEntitySet<{2}.{0}>(\"{1}\"){4}; }}\r\n";
        public static string KeyedWebGETServiceOpTemplate = "[WebGet]\r\n public IQueryable<{2}.{0}> GetKeyed{3}OfT() {{ return GetEntitySet<{2}.{0}>(\"{1}\"){4}; }}\r\n";
        public static string WebGETComplexTypeServiceOpTemplate = "[WebGet]\r\n public IQueryable<{2}.{6}> GetComplexProperty{3}OfT() {{ return GetEntitySet<{2}.{0}>(\"{1}\"){4}; }}\r\n";
        public virtual void SetupServiceOperations()
        {
            List<ServiceOperation> serviceOperations = new List<ServiceOperation>();
            foreach (ResourceContainer container in this.ServiceContainer.ResourceContainers)
            {
                foreach (ResourceType resourceType in container.ResourceTypes)
                {
                    ServiceOperation serviceOp = Resource.ServiceOperation(String.Format("Return{0}OfT()", resourceType.Name), container, resourceType);
#if !ClientSKUFramework
                    serviceOp.ServiceOperationResultKind = Microsoft.OData.Service.Providers.ServiceOperationResultKind.QueryWithMultipleResults;
#endif
                    serviceOp.ServiceOpCode = String.Format(IQueryableOfTServiceOpTemplate1, resourceType.Name, container.Name, this.ContextNamespace, resourceType.Name);
                    if (!serviceOperations.Any(sop => sop.Name == serviceOp.Name)
                        && (!this.ServiceContainer.Any(sop => sop.Name == serviceOp.Name))
                        )
                    {
                        serviceOperations.Add(serviceOp);
                    }
                }
            }
            this.ServiceContainer.AddNodes(serviceOperations);
        }

        //TODO: Need a way to get query source for the oracle.  For now introducing
        //ResourceContainerToQueryable to perform this work.
        public abstract IQueryable ResourceContainerToQueryable(ResourceContainer container);

        public virtual IQueryable ApplyQueryInterceptorExpression(ResourceContainer container, IQueryable containerQueryable)
        {
            container.CLRTypeOfExpression = containerQueryable.ElementType;

            Type interceptorExpressionType = ((LambdaExpression)container.InterceptorExpression).Parameters.Single().Type;
            Type thisQueryableType = containerQueryable.ElementType;
            Expression queryExpression = containerQueryable.Expression;

            Type[] typeArgs = new Type[] { containerQueryable.ElementType };

            if (container.QueryInterceptorExpectedTypeName != thisQueryableType.Name)
            {
                container.CLRTypeOfExpression = this.ServiceContainer.ResourceTypes.First(rt => rt.Name == container.QueryInterceptorExpectedTypeName).ClrType;

                typeArgs = new Type[] { interceptorExpressionType };
                queryExpression = Expression.Call(typeof(Queryable), "OfType", typeArgs, containerQueryable.Expression);
            }
            var intermediateExpression = Expression.Call(typeof(Queryable), "Where", typeArgs, queryExpression, container.InterceptorExpression);
            containerQueryable = containerQueryable.Provider.CreateQuery(intermediateExpression);
            return containerQueryable;
        }
        public virtual IQueryable ServiceOperationToQueryable(ServiceOperation serviceOp)
        {
            IQueryable containerQueryable = null;
            IQueryable serviceOperationResults = null;
            Expression queryExpression = null;
            switch (serviceOp.BackingType)
            {
                case ServiceOpBackingType.EntitySet:
                    containerQueryable = this.ResourceContainerToQueryable(serviceOp.Container);
                    queryExpression = containerQueryable.Expression;

                    if (serviceOp.HasQueryExpression)
                    {
                        Type[] typeArgs = new Type[] { containerQueryable.ElementType };

                        serviceOp.ClrTypeOfExpression = containerQueryable.ElementType;

                        if (containerQueryable.ElementType.Name != serviceOp.ExpectedTypeName
                            ||
                            serviceOp.Container.ResourceTypes.Count() > 1)
                        {
                            serviceOp.ClrTypeOfExpression = this.ServiceContainer.ResourceTypes.First(rt => rt.Name == serviceOp.ExpectedTypeName).ClrType;
                            typeArgs = new Type[] { serviceOp.ClrTypeOfExpression };
                            AstoriaTestLog.WriteLine("types dont match");
                            queryExpression = Expression.Call(typeof(Queryable), "OfType", typeArgs, queryExpression);
                            typeArgs = new Type[] { serviceOp.ClrTypeOfExpression };
                        }
                        var result = Expression.Call(typeof(Queryable), "Where", typeArgs, queryExpression, serviceOp.QueryExpression);
                        serviceOperationResults = containerQueryable.Provider.CreateQuery(result);
                    }
                    else
                    {
                        serviceOperationResults = containerQueryable;
                    }
                    break;

                case ServiceOpBackingType.PrimitiveType:
                    serviceOperationResults = (IQueryable)serviceOp.PrimitiveTypeData;
                    break;
                case ServiceOpBackingType.ComplexType:
                    containerQueryable = this.ResourceContainerToQueryable(serviceOp.Container);
                    queryExpression = containerQueryable.Expression;
                    if (serviceOp.HasQueryExpression)
                    {
                        serviceOp.ClrTypeOfExpression = this.ServiceContainer.ResourceTypes.First(rt => rt.Name == serviceOp.ExpectedTypeName).ClrType;
                        serviceOp.ClrComplexTypeOfExpression = serviceOp.ClrTypeOfExpression.GetProperties().FirstOrDefault(prop => prop.Name == serviceOp.ExpectedComplexPropertyName).PropertyType;
                        Expression serviceOpQueryExpression = serviceOp.QueryExpression;
                        serviceOpQueryExpression = Expression.Call(typeof(Queryable), "Select", new Type[] { serviceOp.ClrTypeOfExpression, serviceOp.ClrComplexTypeOfExpression },
                            queryExpression, serviceOpQueryExpression);
                        serviceOperationResults = containerQueryable.Provider.CreateQuery(serviceOpQueryExpression);
                    }
                    else
                    {
                        serviceOperationResults = (IQueryable)serviceOp.ComplexTypeData;
                    }
                    break;
            }
            return serviceOperationResults;
        }

        internal virtual HashSet<Assembly> GetReferencedAssemblies()
        {
            HashSet<Assembly> collection = new HashSet<Assembly>();
            collection.Add(typeof(System.Xml.Linq.LoadOptions).Assembly);
            collection.Add(typeof(System.ComponentModel.Component).Assembly);
            collection.Add(typeof(System.Data.Common.DbConnection).Assembly);
            collection.Add(typeof(System.Data.Objects.ObjectContext).Assembly);
            collection.Add(typeof(System.Xml.Serialization.XmlIgnoreAttribute).Assembly);
            collection.Add(typeof(System.Runtime.Serialization.CollectionDataContractAttribute).Assembly);
            collection.Add(typeof(System.Numerics.BigInteger).Assembly);
            collection.Add(typeof(System.Linq.Enumerable).Assembly);
            collection.Add(typeof(System.Data.Test.Astoria.AstoriaTestLog).Assembly);
            collection.Add(typeof(System.Data.Test.Astoria.FullTrust.TrustedMethods).Assembly);
            collection.Add(typeof(System.Reflection.MethodInfo).Assembly);
            collection.Add(typeof(Microsoft.OData.Client.KeyAttribute).Assembly);
            collection.Add(typeof(Microsoft.OData.ODataUtils).Assembly);

#if !ClientSKUFramework
            collection.Add(typeof(System.Data.Linq.MemberChangeConflict).Assembly);
            collection.Add(typeof(System.Web.HttpContext).Assembly);
            collection.Add(typeof(Microsoft.OData.Service.SingleResultAttribute).Assembly);
#endif

            return collection;
        }

        public virtual List<string> ServiceHostFiles()
        {
            List<string> files = new List<string>();

            string appCodeFolder = Path.Combine(this.HostSourceFolder, "App_Code");
            files.Add(Path.Combine(appCodeFolder, this.ObjectLayerOutputFileName));

            if (File.Exists(Path.Combine(appCodeFolder, "IDataStreamProviderImplementation.cs")))
                files.Add(Path.Combine(appCodeFolder, "IDataStreamProviderImplementation.cs"));

            return files;
        }

        public virtual List<string> RequiredFrameworkSources
        {
            get;
            private set;
        }

        /// <summary>Restores the underlying data store to a well-known state.</summary>
        public virtual void RestoreData()
        {
            if (this.Database != null)
            {
                this.Database.Restore();

                if (AstoriaTestProperties.Client == ClientEnum.SILVERLIGHT)
                {
                    if (this is EdmWorkspace)
                    {
                        WorkspaceLibrary.AddSilverlightHostFiles(this);
                        ((EdmWorkspace)this).CreateSpawnBrowserScripts();
                    }
                }

                // the server's connection to the DB may still be open, so we make one request that we expect to fail
                ResourceContainer container = this.ServiceContainer.ResourceContainers
                    .Where(rc => !(rc is ServiceOperation))
                    .Choose();
                QueryNode query = Query.From(Exp.Variable(container)).Top(1);
                AstoriaResponse response = this.CreateRequest(query).GetResponse();
                if (response.ActualStatusCode != System.Net.HttpStatusCode.InternalServerError && response.ActualStatusCode != System.Net.HttpStatusCode.OK)
                    ResponseVerification.LogFailure(response, new Exception("Response should have either been OK (for an un-touched database) or an error"), true);
            }
        }

        /// <summary>
        /// The path to the folder with files that should be made available 
        /// on the service host.
        /// </summary>
        public string HostSourceFolder
        {
            get
            {
                string result = Environment.CurrentDirectory;
                result = Path.Combine(result, @"Workspaces\Databases");
                result = Path.Combine(result, this.WebServiceName);
                result = Path.Combine(result, "DataService");
                return result;
            }
        }


        //TODO: needed?
        public virtual QueryBuilder QueryBuilder
        {
            get { throw new NotImplementedException(); }
        }

        public virtual DataLayerProviderKind DataLayerProviderKind
        {
            get;
            private set;
        }
        public virtual WorkspaceLanguage Language
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set;
        }

        /// <summary>Extension for this workspace's <see cref="Language"/>.</summary>
        public string LanguageExtension
        {
            get
            {
                switch (this.Language)
                {
                    case WorkspaceLanguage.CSharp:
                        return ".cs";
                    default:
                        Debug.Assert(this.Language == WorkspaceLanguage.VB);
                        return ".vb";
                }
            }
        }

        public virtual string ContextNamespace
        {
            get { return _contextNamespace; }
            set { _contextNamespace = value; }
        }

        public virtual string ContextTypeName
        {
            get { return _contextTypeName; }
            set { _contextTypeName = value; }
        }

        public virtual string ServiceClassName
        {
            get { return _serviceClassName; }
        }

        public virtual ServiceContainer ServiceContainer
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #region Astoria Request creation
#if !SilverlightTestFramework
        public AstoriaRequest CreateRequest()
        {
            return this.CreateRequest(null, null, RequestVerb.Get);
        }

        public AstoriaRequest CreateRequest(ExpNode query)
        {
            return this.CreateRequest(query, null, RequestVerb.Get);
        }



        public AstoriaRequest CreateRequest(ExpNode query, ResourceBodyTree updateTree, RequestVerb operation)
        {
            AstoriaRequest request = new AstoriaRequest(this);
            request.Verb = operation;
            request.Query = query as QueryNode;
            request.UpdateTree = updateTree;

            PrepareRequest(request);

            return request;
        }

        public virtual void PrepareRequest(AstoriaRequest request)
        {
            // do nothing (to be overridden in derived workspace classes)
        }
#endif
        #endregion

#if !SilverlightTestFramework
        public AstoriaDatabase Database
        {
            [DebuggerStepThrough]
            get { return _astoriaDatabase; }
            [DebuggerStepThrough]
            set { _astoriaDatabase = value; }
        }

        public AstoriaWebDataService DataService
        {
            [DebuggerStepThrough]
            get { return _astoriaWebDataService; }
            [DebuggerStepThrough]
            set { _astoriaWebDataService = value; }
        }

#endif
        public string ServiceEndPoint { get { return this.DataService.ServiceRootUri; } }
        public string ServiceUri
        {
            //[DebuggerStepThrough]
            get
            {
                return this.DataService.ServiceUri;
            }
        }
        public Uri ServiceRoot { get { return new Uri(ServiceUri, UriKind.Absolute); } }


        public virtual IQueryable GetUnderlyingProviderQueryResults(ExpNode q)
        {
            LinqQueryBuilder linqQueryBuilder = new LinqQueryBuilder(this);
            linqQueryBuilder.Build(q);
            return linqQueryBuilder.QueryResult;
        }

        public virtual KeyExpressions GetAllExistingKeys(ExpNode query, ResourceContainer resourceContainer)
        {
            KeyExpressions keys = new KeyExpressions();
            IQueryable objects = this.GetUnderlyingProviderQueryResults(query);
            IEnumerator enumerator = null;
            try
            {
                enumerator = objects.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Type t = enumerator.Current.GetType();
                    if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithProxy && AstoriaTestProperties.DataLayerProviderKinds.Contains(Astoria.DataLayerProviderKind.Edm))
                        t = t.BaseType;
                    List<ResourceType> types = resourceContainer.ResourceTypes.Where(rt => rt.Name.Equals(t.Name) && rt.Namespace.Equals(t.Namespace)).ToList();

                    ResourceType actualResourceType = types.Single();
                    keys.Add(GetKeyExpression(resourceContainer, actualResourceType, enumerator.Current));
                }
            }
            catch (NullReferenceException exc)
            {
                //TODO: Due to a client bug
            }
            finally
            {
                IDisposable disposable = (enumerator as IDisposable);
                if (null != disposable)
                {
                    disposable.Dispose();
                }
            }
            return keys;
        }

        public virtual KeyExpressions GetAllExistingKeys(ResourceContainer resourceContainer)
        {
            return this.GetAllExistingKeysOfType(resourceContainer, null);
        }
        /// <summary>
        /// Returns all keyExpressions for a particular type
        /// NOTE: This is expensive on Adventure works or databases with number of returning keys
        /// </summary>
        /// <param name="resourceType">ResourceType to finds its keys</param>
        /// <returns>list of keys of resourcetype</returns>
        public virtual KeyExpressions GetAllExistingKeysOfType(ResourceContainer resourceContainer, ResourceType resourceType)
        {
            if (this.Settings.HasContainment)
                return ContainmentUtil.GetAllExistingKeys(this, resourceContainer, resourceType);

            QueryNode query = Query.From(
                                  Exp.Variable(resourceContainer))
                                 .Select();
            if (resourceType != null)
                query = query.OfType(resourceType);

            return GetAllExistingKeys(query, resourceContainer);
        }
        public KeyExpression GenerateKeyExpression(ResourceContainer container, ResourceType resourceType, object entity)
        {
            return Exp.Key(container, resourceType, GetPropertyValues(entity));
        }
        public virtual KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer)
        {
            return this.GetRandomExistingKey(resourceContainer, null, null);
        }
        public virtual KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer, KeyExpression partialKey)
        {
            return this.GetRandomExistingKey(resourceContainer, null, partialKey);
        }
        public virtual KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer, ResourceType resourceType)
        {
            return this.GetRandomExistingKey(resourceContainer, resourceType, null);
        }
        protected virtual KeyExpression GetRandomExistingKey(ResourceContainer resourceContainer, ResourceType resourceType, KeyExpression partialKey)
        {
            LinqQueryBuilder linqBuilder = new LinqQueryBuilder(this);
            QueryNode query = Query.From(Exp.Variable(resourceContainer));

            //TODO: Figure out how to make OfType work with astoria dataquerycontext
            if (resourceType != null && resourceContainer.Workspace.DataLayerProviderKind != DataLayerProviderKind.InMemoryLinq && resourceType.BaseType != null)
            {
                query = query.OfType(resourceType);
            }
            else if (partialKey != null)
            {
                query = query.Where(partialKey);
            }
            query = query.Select().Top(50);

            linqBuilder.Build(query);

            int count = 0;
            KeyExpressions keys = new KeyExpressions();

            IQueryable objects = linqBuilder.QueryResult;
            IEnumerator enumerator = null;
            try
            {
                enumerator = objects.GetEnumerator();
                while (enumerator.MoveNext() && count < 50)
                {
                    if (enumerator.Current != null)
                    {
                        Type t = enumerator.Current.GetType();
                        if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithProxy)
                            t = t.BaseType;
                        List<ResourceType> types = resourceContainer.ResourceTypes.Where(rt => rt.Name.Equals(t.Name) && rt.Namespace.Equals(t.Namespace)).ToList();
                        if (types.Count > 0)
                        {
                            ResourceType actualResourceType = types.Single();

                            if (resourceType != null && resourceType != actualResourceType)
                                continue;

                            KeyExpression keyExp = GetKeyExpression(resourceContainer, actualResourceType, enumerator.Current);
                            //If key is something that has approx value
                            //Then its really a nonfunctional key, excluding it as 
                            //we only want valid keys here
                            if (!keyExp.IsApproxKeyValue())
                            {
                                keys.Add(keyExp);
                                count++;
                            }
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = (enumerator as IDisposable);
                if (null != disposable)
                {
                    disposable.Dispose();
                }
            }
            return keys.Choose();
        }

        internal virtual KeyExpression CreateKeyExpressionFromProviderObject(object o)
        {
            ResourceType resourceType = DetermineResourceTypeFromProviderObject(o);
            ResourceContainer container = DetermineResourceContainerFromProviderObject(o);

            return GetKeyExpression(container, resourceType, o);
        }

        protected virtual ResourceType DetermineResourceTypeFromProviderObject(object o)
        {
            Type t = o.GetType();
            if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithProxy)
                t = t.BaseType;
            List<ResourceType> types = this.ServiceContainer.ResourceTypes.Where(rt => rt.Name.Equals(t.Name) && rt.Namespace.Equals(t.Namespace)).ToList();
            if (types.Count == 0)
                throw new TestFailedException("Unable to determine ResourceType from object where type is:" + o.GetType().Name);
            return types.First();
        }

        protected virtual ResourceContainer DetermineResourceContainerFromProviderObject(object o)
        {
            //This function will only work in workspaces that do not support MEST 
            //EDM requires an override
            ResourceType resourceType = DetermineResourceTypeFromProviderObject(o);
            foreach (ResourceContainer rc in this.ServiceContainer.ResourceContainers)
            {
                if (rc.ResourceTypes.Contains(resourceType))
                    return rc;
            }
            throw new TestFailedException("Unable to find ResourceContainer for type:" + resourceType.Name);
        }
        protected virtual KeyExpression GetKeyExpression(ResourceContainer container, ResourceType resourceType, object o)
        {
            return Exp.Key(container, resourceType, GetPropertyValues(o));
        }

        // this works for all but NonCLR, due to there being no backing type
        protected virtual IDictionary<string, object> GetPropertyValues(object entity)
        {
            return entity.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(info => info.Name, info => info.GetValue(entity, null));
        }

        public virtual KeyedResourceInstance GetSingleResourceByKey(KeyExpression keyExpression)
        {
            LinqQueryBuilder linqBuilder = new LinqQueryBuilder(this);

            ExpNode query = null;
            if (this.Settings.HasContainment)
                query = ContainmentUtil.BuildCanonicalQuery(keyExpression).Select();
            else
            {
                query = Query.From(
                                 Exp.Variable(keyExpression.ResourceContainer))
                                .Where(keyExpression)
                                .Select();
            }

            IQueryable queryable = this.GetUnderlyingProviderQueryResults(query);
            IEnumerator enumerator = queryable.GetEnumerator();
            enumerator.MoveNext();
            object dataObject = enumerator.Current;

            if (dataObject == null)
                return null;
            return ResourceInstanceUtil.CreateKeyedResourceInstanceByExactClone(keyExpression.ResourceContainer, keyExpression.ResourceType, dataObject);
        }
        public virtual KeyExpressions GetExistingAssociatedKeys(ResourceContainer resourceContainer, ResourceProperty property, KeyExpression keyExpression)
        {
            LinqQueryBuilder linqBuilder = new LinqQueryBuilder(this);
            ExpNode query = Query.From(
                                 Exp.Variable(resourceContainer))
                                .Where(keyExpression)
                                .OfType(property.ResourceType)
                                .Nav(new PropertyExpression(property))
                                .Select();

            linqBuilder.Build(query);

            ResourceType associatedType = property.Type as ResourceType;
            if (property.Type is ResourceCollection)
            {
                associatedType = (property.Type as ResourceCollection).SubType as ResourceType;
            }
            IQueryable queryable = linqBuilder.QueryResult;
            KeyExpressions keys = new KeyExpressions();
            IEnumerator enumerator = queryable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    Type t = enumerator.Current.GetType();
                    if (AstoriaTestProperties.EdmObjectLayer == ServiceEdmObjectLayer.PocoWithProxy)
                        t = t.BaseType;

                    IEnumerable<ResourceType> typesWithName = resourceContainer.Workspace.ServiceContainer.ResourceTypes.Where(rt => (t.Name.Equals(rt.Name)));
                    IEnumerable<ResourceType> typesWithNamespace = typesWithName.Where(rt2 => rt2.Namespace == t.Namespace).ToList();
                    ResourceType instanceType = typesWithNamespace.First();
                    ResourceContainer relatedContainer = resourceContainer.FindDefaultRelatedContainer(property);
                    keys.Add(GetKeyExpression(relatedContainer, instanceType, enumerator.Current));
                }
            }
            return keys;
        }
        protected virtual AstoriaHashTable<ResourceType, Type> CreateResourceTypeToUnderlyingClrTypeMap(Assembly myAssembly, string clrTypeNamespace)
        {
            AstoriaHashTable<ResourceType, Type> typeMap = new AstoriaHashTable<ResourceType, Type>();
            foreach (ResourceType t in this.ServiceContainer.ResourceTypes)
            {
                if (!typeMap.ContainsKey(t))
                {
                    Type entityType = myAssembly.GetType(clrTypeNamespace + "." + t.Name);
                    if (entityType == null)
                        AstoriaTestLog.WriteLineIgnore("Unable to find Clr Type:" + clrTypeNamespace + "." + t.Name + " in Assembly:" + myAssembly);
                    //throw new TestFailedException("Unable to find Clr Type:" + clrTypeNamespace + "." + t.Name + " in Assembly:" + myAssembly);
                    typeMap.Add(t, entityType);
                }
            }
            return typeMap;
        }

        public void SetResourceTypesToClrTypes(Assembly assembly, string clrTypeNamespace, AstoriaHashTable<ResourceType, Type> typeDictionary, bool fillClientType)
        {
            // Make sure ClrType is set for all types in the assembly and servicecontainer
            foreach (ResourceType rt in this.ServiceContainer.ResourceTypes)
            {
                if (!typeDictionary.ContainsKey(rt))
                    AstoriaTestLog.FailAndThrow(String.Format("ResourceType '{0}' does not have a corresponding CLR type in the type dictionary", rt.Name));

                if (typeDictionary[rt] == null)
                    continue;

                if (fillClientType)
                    rt.ClientClrType = typeDictionary[rt];
                else
                    rt.ClrType = typeDictionary[rt];

                // check to see if the base type will not be returned by an underlying provider query
                if (rt.BaseType != null)
                {
                    ResourceType baseType = rt.BaseType as ResourceType;
                    // if the basetype is not a resource type, or no entity set returns it, then we should use the current type as its Clr type.
                    // note that this would be incorrect in a MEST scenario with inheritance
                    if (baseType == null || !this.ServiceContainer.ResourceContainers.Any(rc => rc.ResourceTypes.Contains(baseType)))
                    {
                        if (fillClientType)
                            rt.BaseType.ClientClrType = typeDictionary[rt];
                        else
                            rt.BaseType.ClrType = typeDictionary[rt];
                    }
                }
                foreach (ResourceProperty navProperty in rt.Properties.Cast<ResourceProperty>().Where(rp => rp.IsNavigation))
                {
                    if (navProperty.Type.ClrType != null)
                    {
                        Type navPropertyType = null;
                        if (navProperty.Type is CollectionType)
                        {
                            navPropertyType = typeof(System.Collections.ObjectModel.Collection<>).MakeGenericType(
                                        typeDictionary[(ResourceType)((CollectionType)navProperty.Type).SubType]);
                        }
                        else
                        {
                            navPropertyType = typeDictionary[(ResourceType)navProperty.Type];
                        }
                        navProperty.Type.ClientClrType = navPropertyType;
                    }
                }
                foreach (ResourceProperty navProperty in rt.Properties.Cast<ResourceProperty>().Where(rp => rp.IsNavigation))
                {
                    if (navProperty.Type.ClrType != null)
                    {
                        Type navPropertyType = null;
                        if (navProperty.Type is CollectionType)
                        {
                            navPropertyType = typeof(System.Collections.ObjectModel.Collection<>).MakeGenericType(
                                        typeDictionary[(ResourceType)((CollectionType)navProperty.Type).SubType]);
                        }
                        else
                        {
                            navPropertyType = typeDictionary[(ResourceType)navProperty.Type];
                        }
                        navProperty.Type.ClientClrType = navPropertyType;
                    }
                }
                foreach (ResourceProperty complexProperty in rt.Properties.Cast<ResourceProperty>().Where(rp => rp.IsComplexType))
                {
                    if (complexProperty.Type.ClrType != null)
                    {
                        complexProperty.Type.ClientClrType = Type.GetType(complexProperty.Type.ClrType.FullName.Replace(this.ContextNamespace, this.ContextNamespace + "Client"));
                    }
                }
            }

            foreach (ResourceType rt in this.ServiceContainer.ResourceTypes)
            {
                foreach (ResourceProperty rp in rt.Properties)
                {
                    if ((rp.IsNavigation || rp.IsComplexType) && (rp.Type.ClrType == null || fillClientType))
                    {
                        Type edmType = assembly.GetType(clrTypeNamespace + "." + rp.Type.ToString());
                        if (edmType != null)
                        {
                            if (fillClientType)
                                rp.Type.ClientClrType = edmType;
                            else
                                rp.Type.ClrType = edmType;
                        }


                        if (rp.Type is CollectionType)
                        {
                            CollectionType collectionType = (CollectionType)rp.Type;
                            NodeType subType = collectionType.SubType;

                            if (subType.ClrType == null)
                            {
                                edmType = assembly.GetType(clrTypeNamespace + "." + subType.Name);
                                if (fillClientType)
                                    subType.ClientClrType = edmType;
                                else
                                    subType.ClrType = edmType;
                            }
                        }
                    }
                }
            }
        }

        public string ServiceAdditionalCode { get; set; }
        public string GlobalAdditionalCode { get; set; }

        public Action<Workspace> CustomInterceptorAction { get; set; }

        public bool GenerateCallOrderInterceptors { get; set; }

        public virtual void GenerateCustomInterceptors()
        {
            if (CustomInterceptorAction != null)
                CustomInterceptorAction(this);
        }

        public virtual void GenerateCallOrderInterceptorCode()
        {
            if (!this.GenerateCallOrderInterceptors)
            {
                return;
            }

            List<string> additionalCodeLines = new List<string>();

            // add interceptors to track call ordering
            foreach (ResourceContainer container in this.ServiceContainer.ResourceContainers)
            {
                if (container is ServiceOperation)
                    continue;

                string typeName;

#if !ClientSKUFramework
                if (this is NonClrWorkspace)
                {
                    typeName = typeof(NonClr.RowEntityType).FullName;
                    if (container.BaseType.Facets.IsClrType)
                        typeName = this.ContextNamespace + "." + container.BaseType.Name;
                }
                else
                {
                    typeName = this.ContextNamespace + "." + container.BaseType.Name;
                }
#else
                typeName = this.ContextNamespace + "." + container.BaseType.Name;
#endif

                additionalCodeLines.AddRange(new string[]
                    {
                        "[ChangeInterceptor(\"" + container.Name + "\")]",
                        "public void ChangeInterceptor_" + container.Name + "(object entity, UpdateOperations action)",
                        "{",
                        "   APICallLog.Current.DataService.ChangeInterceptor(this, \"ChangeInterceptor_" + container.Name + "\", entity, action);",
                        "   APICallLog.Current.Pop();",
                        "}",
                        null,
                    });

                additionalCodeLines.AddRange(new string[]
                    {
                        "[QueryInterceptor(\"" + container.Name + "\")]",
                        "public Expression<Func<" + typeName + ", bool>> QueryInterceptor_" + container.Name + "()",
                        "{",
                        "    APICallLog.Current.DataService.QueryInterceptor(this, \"QueryInterceptor_" + container.Name + "\");",
                        "    try",
                        "    {",
                        "       return entity => true;",
                        "    }",
                        "    finally",
                        "    {",
                        "        APICallLog.Current.Pop();",
                        "    }",
                        "}",
                        null,
                    });
            }

            this.ServiceAdditionalCode += string.Join(Environment.NewLine, additionalCodeLines.ToArray());
        }

        //TODO: steveob There has to be a way to expose whether the service has extension
        //rse or svc.  It should probably be a test property.  For now it is hardcoded here
        //in one place.
        public virtual string ServiceType
        {
            get
            {
                if (AstoriaTestProperties.Host == Host.WebServiceHost)
                    return "WebServiceHost";
                else
                    return "svc";
            }
        }

        // TODO: need a better way to find baseline files
        public virtual System.Collections.Generic.Dictionary<string, string> BaselineFiles()
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, string> localizedResourceStrings = new Dictionary<string, string>();
        public string GetLocalizedResourceString(string identifierId, params Object[] args)
        {
            string localizedResourceString;
            if (!localizedResourceStrings.TryGetValue(identifierId, out localizedResourceString))
            {
                string responseText;
                RequestUtil.GetAndVerifyStatusCode(this,
                    this.ServiceUri + String.Format("/GetResourceString?identifierString='{0}'", identifierId),
                    System.Net.HttpStatusCode.OK,
                    out responseText);

                if (responseText.Contains("Error:"))
                    throw new TestFailedException(responseText);

                // extract text from xml that is returned.
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseText);

                localizedResourceString = xmlDoc.FirstChild.NextSibling.InnerText;

                localizedResourceStrings[identifierId] = localizedResourceString;
            }

            return ResourceUtil.FormatResourceString(localizedResourceString, args);
        }

        protected virtual IEnumerable<string> GetUsingsForDataServiceClass()
        {
            List<string> usings = new List<string>()
            {
                "System",
                "System.Collections",
                "System.Collections.Generic",
                "System.Data",
                "System.Data.Objects",
                "Microsoft.OData.Service",
                "Microsoft.OData.Service.Providers",
                "System.IO",
                "System.Linq",
                "System.Linq.Expressions",
                "System.Threading",
                "System.ServiceModel",
                "System.ServiceModel.Channels",
                "System.ServiceModel.Description",
                "System.ServiceModel.Dispatcher",
                "System.ServiceModel.Web",
                "System.Text",
                "System.Web",
                "System.Xml",
                "System.Xml.Linq",
                "System.Reflection",
                "System.Data.Test.Astoria.CallOrder",
                "System.Data.Test.Astoria.Providers",
                this.ContextNamespace
            };

            if (AstoriaTestProperties.Host == Host.IDSH || AstoriaTestProperties.Host == Host.IDSH2)
            {
                usings.Add("System.Net");
                usings.Add("System.Collections.Specialized");
            }

            return usings;
        }

        protected internal virtual string BuildDataServiceClassUsings()
        {
            StringBuilder builder = new StringBuilder();
            foreach (string u in GetUsingsForDataServiceClass())
                builder.AppendLine("using " + u + ";");
            builder.AppendLine();
            return builder.ToString();
        }

        protected internal virtual string BuildDataServiceClassCode()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("[System.ServiceModel.ServiceBehavior(IncludeExceptionDetailInFaults = true)]");

            string line = "public partial class " + this.ServiceClassName + " : " + Settings.ServiceBaseClass + "<" + this.ContextTypeName + ">";
            bool serviceProvider = ServiceModifications.Interfaces.IServiceProvider.Services.Any();
            if (serviceProvider)
                line += ", IServiceProvider";
            builder.AppendLine(line);

            builder.AppendLine("{");
            builder.AppendLine("    protected override void OnConstruction()");
            builder.AppendLine("    {");

            if (this.Settings.MaxProtocolVersion.HasValue)
            {
                builder.AppendLine("        if (!ConfigurationSettings.PropertySetList.Contains(\"MaxProtocolVersion\"))");
                builder.AppendLine("        {");
                builder.AppendLine("            ConfigurationSettings.MaxProtocolVersion = " + (int)this.Settings.MaxProtocolVersion.Value + ";");
                builder.AppendLine("            ConfigurationSettings.PropertySetList.Add(\"MaxProtocolVersion\");");
                builder.AppendLine("        }");
            }

            // we do it this way because if we put it in TestDataWebService.cs, then we can't deal with these conditions very well
            if (AstoriaTestProperties.Host != Host.Cassini && AstoriaTestProperties.ServiceTrustLevel == TrustLevel.Full)
            {
                // we only want to change the debug listeners if we're on the server... so we emit this extra check
                builder.AppendLine("        if(System.Diagnostics.Process.GetCurrentProcess().Id != " + System.Diagnostics.Process.GetCurrentProcess().Id + ")");
                builder.AppendLine("        {");
                builder.AppendLine("            System.Diagnostics.Debug.Listeners.Clear();");
                builder.AppendLine("            System.Diagnostics.Debug.Listeners.Add(new System.Data.Test.Astoria.ExceptionThrowingTraceListener());");
                builder.AppendLine("        }");
            }

            builder.AppendLine("    }");

            if (serviceProvider)
                builder.AppendLine(ServiceModifications.Interfaces.IServiceProvider.GetMethodDeclaration());

            builder.AppendLine(this.ServiceAdditionalCode);

            builder.AppendLine("}");

            builder.AppendLine(this.GlobalAdditionalCode);

            return builder.ToString();
        }

        public virtual void CreateWebDataServiceSvcFile(string filePath)
        {
            string languageStr = null;
            switch (this.Language)
            {
                case WorkspaceLanguage.CSharp:
                    languageStr = "C#";
                    break;
                case WorkspaceLanguage.VB:
                    languageStr = "VB";
                    break;
            }
            StreamWriter writer = File.CreateText(filePath);
            string line = "<%@ ServiceHost Language=\"" + languageStr + "\" Factory=\"Microsoft.OData.Service.DataServiceHostFactory\" Service=\"" + this.ServiceClassName + "\" %>";
            writer.WriteLine(line);
            writer.Close();
        }

        public virtual Configuration.ConnectionStringSettings GetConnectionStringSettingsForProvider(AstoriaWebDataService service, string databaseConnectionString)
        {
            throw new NotImplementedException();
        }

        public void CompileCodeFiles(string[] codeFilePaths, string[] referencedAssemblies, string dllFilePath)
        {
            Util.CodeCompilerHelper.CompileCodeFiles(codeFilePaths, dllFilePath, new string[] { this.DataLayerProviderKind.ToString() }, this.Language, GetReferencedAssemblies().Select(a => a.ManifestModule.FullyQualifiedName).ToArray());
        }

#if !ClientSKUFramework

        public virtual void VerifyMetadata(string actualMetadata)
        {
            throw new NotImplementedException();
        }

#endif
        public void ClearMetadataCache()
        {
            RequestUtil.GetAndVerifyStatusCode(this, this.ServiceUri + "/ClearMetadataCache", HttpStatusCode.NoContent);
        }

        public bool IsDebugProductBuild()
        {
            string rawValue;
            RequestUtil.GetAndVerifyStatusCode(this, this.ServiceUri + "/IsDebugProductBuild/$value", HttpStatusCode.OK, out rawValue);
            bool value;
            return bool.TryParse(rawValue, out value) && value;
        }
    }

    public enum WorkspaceLanguage { VB, CSharp };

    public class NewWorkspaceEventArgs : EventArgs
    {
        private Workspace _workspace;

        public NewWorkspaceEventArgs(Workspace workspace)
        {
            _workspace = workspace;
        }

        public Workspace Workspace
        {
            get { return _workspace; }
            set { _workspace = value; }
        }
    }

}
