//---------------------------------------------------------------------
// <copyright file="AstoriaWorkspaceBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Xml.Linq;
    using Microsoft.OData.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Builds Astoria workspaces.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Added MaxVersion for data service builder fixup")]
    public abstract class AstoriaWorkspaceBuilderBase : IAstoriaWorkspaceBuilder
    {
        private static string[] referenceAssemblies = 
        {
            "System.dll",
            "System.Core.dll",
            "System.Net.dll",
            DataFxAssemblyRef.File.DataServicesClient,
            DataFxAssemblyRef.File.ODataLib,
            DataFxAssemblyRef.File.EntityDataModel,
            "System.Xml.dll",
        };

        private EntityModelSchema initialModel;
        private string clientLayerCode;

        /// <summary>
        /// Initializes a new instance of the AstoriaWorkspaceBuilderBase class.
        /// </summary>
        protected AstoriaWorkspaceBuilderBase()
        {
            this.Logger = Logger.Null;
            this.EdmVersion = EdmVersion.V40;
            this.SkipClientCodeGeneration = false;
            this.SkipDataServiceDispose = false;
            this.SkipDataDownload = false;
            this.MaxProtocolVersion = DataServiceProtocolVersion.V4;
            this.ModelFeatureVersion = DataServiceProtocolVersion.V4;
        }

        /// <summary>
        /// Occurs when the model has been generated and can be customized to add Entity Types and ComplexTypes
        /// </summary>
        /// <remarks>
        /// Handlers of this event can manipulate contents of <see cref="Workspace.ConceptualModel"/>
        /// and should set Handled property to true.
        /// </remarks>
        public event EventHandler<WorkspaceEventArgs<Workspace>> CustomizeModelBeforeDefaultFixups;

        /// <summary>
        /// Occurs when the model has been generated and can be customized after the default fixups have been run
        /// </summary>
        /// <remarks>
        /// Handlers of this event can manipulate contents of <see cref="Workspace.ConceptualModel"/>
        /// and should set Handled property to true.
        /// </remarks>
        public event EventHandler<WorkspaceEventArgs<Workspace>> CustomizeModelAfterDefaultFixups;

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the language used to build the code.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IProgrammingLanguageStrategy Language { get; set; }

        /// <summary>
        /// Gets or sets the model generator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IModelGenerator ModelGenerator { get; set; }

        /// <summary>
        /// Gets or sets the primitive type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IEntityModelPrimitiveTypeResolver PrimitiveTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the edm data type resolver
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public EdmDataTypeResolver EdmDataTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the data provider settings to be applied when generating service.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public DataProviderSettings DataProviderSettings { get; set; }

        /// <summary>
        /// Gets or sets the deserializer which converts wire representation of entity container data into <see cref="EntityContainerData" />
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataOracleResultConverter DataOracleConverter { get; set; }

        /// <summary>
        /// Gets or sets the generator to use for the client code layer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IClientCodeLayerGenerator ClientCodeLayerGenerator { get; set; }

        /// <summary>
        /// Gets or sets the service reference factory
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IServiceReferenceFactory ServiceReferenceFactory { get; set; }

        /// <summary>
        /// Gets or sets the URI for the Service document of the target service
        /// </summary>
        /// <value>The uri to the Service Document.</value>
        [InjectTestParameter("ServiceDocumentUri", HelpText = "The Uri for the Service document of the OData Service to target")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is externally injected")]
        public string ServiceDocumentUri { get; set; }

        /// <summary>
        /// Gets or sets the URI for the root of the OData Service being targeted by the tests
        /// </summary>
        /// <value>The OData service's base URI.</value>
        [InjectTestParameter("ServiceBaseUri", HelpText = "The Uri for the Service document of the OData Service to target")]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "This is externally injected")]
        public string ServiceBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the AuthenticationProvider 
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAuthenticationProvider AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets the ServiceDocumentParser to parse OData Service Documents
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IServiceDocumentParser ServiceDocumentParser { get; set; }

        /// <summary>
        /// Gets or sets the MajorReleaseVersion
        /// </summary>
        [InjectTestParameter("MaxProtocolVersion", DefaultValueDescription = "V3")]
        public DataServiceProtocolVersion MaxProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the Max version that the service is suppose to be from a feature stand point
        /// </summary>
        [InjectTestParameter("ModelFeatureVersion", DefaultValueDescription = "V3")]
        public DataServiceProtocolVersion ModelFeatureVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to dispose of the Data Service
        /// </summary>
        [InjectTestParameter("SkipDataServiceDispose", DefaultValueDescription = "false")]
        public bool SkipDataServiceDispose { get; set; }

        /// <summary>
        /// Gets or sets the EDM version to use when sending CSDL to the server.
        /// </summary>
        public EdmVersion EdmVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to generate client code
        /// </summary>
        [InjectTestParameter("SkipClientCodeGeneration", DefaultValueDescription = "false")]
        public bool SkipClientCodeGeneration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to generate client code
        /// </summary>
        [InjectTestParameter("SkipDataDownload", DefaultValueDescription = "false")]
        public bool SkipDataDownload { get; set; }

        /// <summary>
        /// Gets or sets the implementation of globalization fixup to use.
        /// </summary>
        /// <value>The globalization fixup.</value>
        [InjectDependency]
        public IAstoriaGlobalizationFixup AstoriaGlobalizationFixup { get; set; }

        /// <summary>
        /// Gets the current workspace being built
        /// </summary>
        protected AstoriaWorkspace CurrentWorkspace { get; private set; }

        /// <summary>
        /// Builds the workspace asynchronously.
        /// </summary>
        /// <param name="workspace">The workspace to populate.</param>
        /// <param name="continuation">The continuation to invoke at the end of the build operation.</param>
        public void BuildWorkspaceAsync(AstoriaWorkspace workspace, IAsyncContinuation continuation)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.CurrentWorkspace = workspace;

            this.Logger.WriteLine(LogLevel.Verbose, "Initializing model...");
            this.InitializeModel(workspace);

            this.Logger.WriteLine(LogLevel.Verbose, "Building workspace asynchronously.");

            AsyncHelpers.RunActionSequence(
                continuation,
                this.BuildDataService,
                this.UpdateEntityModel,
                this.InitializeResourceStringVerifiers,
                this.GenerateClientLayerCode,
                this.CompileClientLayerCode,
                this.BuildEntitySetResolver,
                this.RegisterServiceUri,
                this.BuildEdmModel,
                this.DownloadData,
                this.FindDataServiceContext);
        }

        /// <summary>
        /// Builds the workspace.
        /// </summary>
        /// <returns>
        /// Returns instance of a class which derives from <see cref="Workspace"/>. The instance is not fully
        /// initialized until the current method (Init() or variation) completes asynchronously.
        /// </returns>
        public AstoriaWorkspace BuildWorkspace()
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            var workspace = new AstoriaWorkspace(this);
            AsyncExecutionContext.EnqueueAsynchronousAction(cb => this.BuildWorkspaceAsync(workspace, cb));

            return workspace;
        }

        /// <summary>
        /// Builds the workspace from the specified model.
        /// </summary>
        /// <param name="modelSchema">The model schema.</param>
        /// <returns>
        /// Returns fully initialized instance of a class which derives from <see cref="Workspace"/>.
        /// </returns>
        public Workspace BuildWorkspace(EntityModelSchema modelSchema)
        {
            this.initialModel = modelSchema;
            return this.BuildWorkspace();
        }

        /// <summary>
        /// Gets the reference assemblies needed to compile a client for the given model and language
        /// </summary>
        /// <param name="model">The model being generated</param>
        /// <param name="language">The current language</param>
        /// <returns>The list of reference assemblies</returns>
        internal static IEnumerable<string> GetClientReferenceAssemblies(EntityModelSchema model, IProgrammingLanguageStrategy language)
        {
            bool isVB = language.LanguageName == "VB";
            IEnumerable<string> languageSpecificReferences = isVB ? referenceAssemblies.Where(l => l != "mscorlib.dll") : referenceAssemblies;

            if (model.HasSpatialFeatures())
            {
                languageSpecificReferences = languageSpecificReferences.Concat(DataFxAssemblyRef.File.SpatialCore);
            }

            return languageSpecificReferences;
        }

        /// <summary>
        /// Builds the system data services client resource lookup.
        /// </summary>
        /// <returns>The resource lookup</returns>
        protected static IResourceLookup BuildSystemDataServicesClientResourceLookup()
        {
            Assembly systemDataServiceClientAssembly;
            systemDataServiceClientAssembly = Assembly.Load(new AssemblyName(DataFxAssemblyRef.DataServicesClient));

            return new AssemblyResourceLookup(systemDataServiceClientAssembly);
        }

        /// <summary>
        /// Builds the OData library resource lookup.
        /// </summary>
        /// <returns>The resource lookup</returns>
        protected static IResourceLookup BuildMicrosoftDataODataResourceLookup()
        {
            Assembly microsoftDataODataAssembly;
            microsoftDataODataAssembly = Assembly.Load(new AssemblyName(DataFxAssemblyRef.OData));
            return new AssemblyResourceLookup(microsoftDataODataAssembly);
        }

        /// <summary>
        /// Raises the workspace event.
        /// </summary>
        /// <param name="eventHandler">The event handler.</param>
        /// <param name="workspace">The workspace.</param>
        /// <returns>Instance of the event args passed to the handler. The caller can examine 'Handled' property
        /// to determine whether the event has been handled.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "This metod is used to raise different kinds of events.")]
        protected WorkspaceEventArgs<Workspace> RaiseWorkspaceEvent(EventHandler<WorkspaceEventArgs<Workspace>> eventHandler, Workspace workspace)
        {
            var e = new WorkspaceEventArgs<Workspace>(workspace);
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }

            return e;
        }

        /// <summary>
        /// Builds the data service
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected abstract void BuildDataService(IAsyncContinuation continuation);

        /// <summary>
        /// Builds the Edm Model
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected abstract void BuildEdmModel(IAsyncContinuation continuation);

        /// <summary>
        /// Update entity model schema based on csdl sent back from server.
        /// </summary>
        /// <param name="continuation">The continuation to invoke at the end of the build operation.</param>
        protected virtual void UpdateEntityModel(IAsyncContinuation continuation)
        {
            // resolve the primitive types in case the model coming back from the service builder doesn't have them resolved.
            this.PrimitiveTypeResolver.ResolveProviderTypes(this.CurrentWorkspace.ConceptualModel, this.EdmDataTypeResolver);

            // Do nothing by default.
            continuation.Continue();
        }

        /// <summary>
        /// Initializes the resource string verifiers.
        /// </summary>
        /// <param name="continuation">The async continuation.</param>
        protected void InitializeResourceStringVerifiers(IAsyncContinuation continuation)
        {
            this.CurrentWorkspace.SystemDataServicesResourceLookup = this.BuildSystemDataServicesResourceLookup();
            this.CurrentWorkspace.SystemDataServicesStringVerifier = new StringResourceVerifier(this.CurrentWorkspace.SystemDataServicesResourceLookup);

            var systemDataServicesClientResourceLookup = BuildSystemDataServicesClientResourceLookup();
            this.CurrentWorkspace.SystemDataServicesClientStringVerifier = new StringResourceVerifier(systemDataServicesClientResourceLookup);

            var microsoftDataODataLookup = BuildMicrosoftDataODataResourceLookup();
            this.CurrentWorkspace.MicrosoftDataODataStringVerifier = new StringResourceVerifier(microsoftDataODataLookup);

            continuation.Continue();
        }

        /// <summary>
        /// Builds the system data services resource lookup.
        /// </summary>
        /// <returns>The resource lookup</returns>
        protected abstract IResourceLookup BuildSystemDataServicesResourceLookup();

        private void InitializeModel(AstoriaWorkspace workspace)
        {
            workspace.ConceptualModel = this.initialModel ?? this.ModelGenerator.GenerateModel();

            this.Logger.WriteLine(LogLevel.Verbose, "Customizing model before fixups...");
            this.RaiseWorkspaceEvent(this.CustomizeModelBeforeDefaultFixups, workspace);

            var configurationFixup = new SetDefaultDataServiceConfigurationBehaviors();
            configurationFixup.MaxProtocolVersion = this.MaxProtocolVersion;
            configurationFixup.Fixup(workspace.ConceptualModel);

            // remove higher version features based on max protocol version.
            new RemoveHigherVersionFeaturesFixup(this.ModelFeatureVersion).Fixup(workspace.ConceptualModel);

            // we keep these fixups here. They remove things that are always invalid, regardless of the provider type.
            new ReplaceBinaryKeysFixup().Fixup(workspace.ConceptualModel);
            new RemoveConcurrencyTokensFromComplexTypesFixup().Fixup(workspace.ConceptualModel);

            // Note this must be done after the binary fixup, as we replace binary keys with integer ones, but before the provider-specific fixup
            // It must also be done before getting the provider-specific fixups as they rely on CLR typing in some cases
            this.PrimitiveTypeResolver.ResolveProviderTypes(workspace.ConceptualModel, this.EdmDataTypeResolver);

            var providerFixup = this.DataProviderSettings.GetProviderSpecificModelFixup();
            if (providerFixup != null)
            {
                providerFixup.Fixup(workspace.ConceptualModel);
            }

            // resolve method body
            IServiceMethodResolver serviceMethodResolver = this.DataProviderSettings.GetProviderSpecificServiceModelResolver();
            foreach (Function f in workspace.ConceptualModel.Functions)
            {
                serviceMethodResolver.ResolveServiceMethodBody(f);
            }

            // Customizes actions to specify entitySets they are in
            new SetActionDefaultEntitySetFixup().Fixup(workspace.ConceptualModel);

            if (this.AstoriaGlobalizationFixup != null)
            {
                this.AstoriaGlobalizationFixup.Fixup(workspace.ConceptualModel);
            }

            // generate method code with potentially localized names
            foreach (Function f in workspace.ConceptualModel.Functions)
            {
                serviceMethodResolver.GenerateServiceMethodCode(f);
            }

            this.Logger.WriteLine(LogLevel.Verbose, "Customizing model after fixups...");
            this.RaiseWorkspaceEvent(this.CustomizeModelAfterDefaultFixups, workspace);
        }

        private void DownloadData(IAsyncContinuation continuation)
        {
            this.Logger.WriteLine(LogLevel.Verbose, "Downloading entity container data from {0}", this.CurrentWorkspace.OracleServiceUri);
            var dataOracleClient = this.ServiceReferenceFactory.CreateInstance<IDataOracleService>(this.CurrentWorkspace.OracleServiceUri);

            if (this.SkipDataDownload)
            {
                // Must create an empty one other wise failures will occur afterwards
                // TODO: handle multiple entity containers
                this.CurrentWorkspace.DownloadedEntityContainerData = new EntityContainerData(this.CurrentWorkspace.ConceptualModel.EntityContainers.Single());
                continuation.Continue();
                return;
            }

            dataOracleClient.BeginGetContainerData(
                result => AsyncHelpers.CatchErrors(
                    continuation,
                    () =>
                    {
                        string errorMessage;
                        var container = dataOracleClient.EndGetContainerData(out errorMessage, result);
                        if (container == null)
                        {
                            continuation.Fail(new TaupoInfrastructureException(errorMessage));
                            return;
                        }

                        this.Logger.WriteLine(LogLevel.Verbose, "Got container with {0} entities.", container.Entities.Count);
                        this.CurrentWorkspace.DownloadedEntityContainerData = this.DataOracleConverter.Convert(this.CurrentWorkspace.ConceptualModel, container);

                        continuation.Continue();
                    }),
                null);
        }

        private void GenerateClientLayerCode(IAsyncContinuation continuation)
        {
            if (this.SkipClientCodeGeneration)
            {
                continuation.Continue();
                return;
            }

            this.Logger.WriteLine(LogLevel.Verbose, "Generating client source code...");
            AsyncHelpers.CatchErrors(
                continuation,
                () =>
                {
                    this.ClientCodeLayerGenerator.GenerateClientCode(
                        continuation,
                        this.CurrentWorkspace.ServiceUri,
                        this.CurrentWorkspace.ConceptualModel,
                        this.Language,
                        code =>
                        {
                            this.clientLayerCode = code;
                            this.Logger.WriteLine(LogLevel.Trace, "Generated Code: {0}", this.clientLayerCode);
                        });
                });
        }

        private void CompileClientLayerCode(IAsyncContinuation continuation)
        {
            if (this.SkipClientCodeGeneration)
            {
                continuation.Continue();
                return;
            }

            this.Logger.WriteLine(LogLevel.Verbose, "Compiling Client Source Code....");

            var languageSpecificReferences = GetClientReferenceAssemblies(this.CurrentWorkspace.ConceptualModel, this.Language);

            string assemblyBaseName = "DataServiceClient" + Guid.NewGuid().ToString("N");
            this.Language.CompileAssemblyAsync(
                assemblyBaseName,
                new[] { this.clientLayerCode },
                languageSpecificReferences.ToArray(),
                (asm, error) =>
                {
                    if (error != null)
                    {
                        continuation.Fail(error);
                        return;
                    }

                    this.CurrentWorkspace.Assemblies.Add(new FileContents<Assembly>(assemblyBaseName + ".dll", asm));
                    continuation.Continue();
                });
        }

        private void RegisterServiceUri(IAsyncContinuation continuation)
        {
            // We set the Oracle Service uri to point to the deployed service and not wherever the serviceBaseUri points to.
            this.CurrentWorkspace.OracleServiceUri = new Uri(this.CurrentWorkspace.ServiceUri.OriginalString.Replace("RelayService", null).Replace(".svc", "DataOracle.svc"));

            // If a ServiceBaseUri was passed in through the test parameters, we set the ServiceUri of the workspace to be that.
            if (!string.IsNullOrEmpty(this.ServiceBaseUri))
            {
                this.CurrentWorkspace.ServiceUri = new Uri(this.ServiceBaseUri);
            }

            continuation.Continue();
        }

        private void BuildEntitySetResolver(IAsyncContinuation continuation)
        {
            if (string.IsNullOrEmpty(this.ServiceDocumentUri))
            {
                IEntitySetResolver defaultEntitySetResolver = new DefaultEntitySetResolver();
                this.CurrentWorkspace.EntitySetResolver = defaultEntitySetResolver;
                continuation.Continue();
            }
            else
            {
                WebRequest serviceDocRequest = WebRequest.Create(this.ServiceDocumentUri);

                if (this.AuthenticationProvider.UseDefaultCredentials)
                {
                    serviceDocRequest.UseDefaultCredentials = true;
                }
                else if (this.AuthenticationProvider.GetAuthenticationCredentials() != null)
                {
                    serviceDocRequest.Credentials = this.AuthenticationProvider.GetAuthenticationCredentials();
                }

                IDictionary<string, string> authenticationHeaders = this.AuthenticationProvider.GetAuthenticationHeaders();
                if (authenticationHeaders != null)
                {
                    foreach (var header in authenticationHeaders)
                    {
                        serviceDocRequest.Headers[header.Key] = header.Value;
                    }
                }

                // we will set the timeout to 5 seconds to avoid waiting forever for a Service doc request to complete.
                serviceDocRequest.Timeout = 5000;
                serviceDocRequest.BeginGetResponse(
                    (asyncResult) =>
                    {
                        try
                        {
                            XDocument serviceDocument = XDocument.Load(serviceDocRequest.EndGetResponse(asyncResult).GetResponseStream());
                            IEntitySetResolver entitySetResolver = this.ServiceDocumentParser.ParseServiceDocument(serviceDocument.Root);
                            this.CurrentWorkspace.EntitySetResolver = entitySetResolver;
                            continuation.Continue();
                        }
                        catch (WebException errorWhileDownload)
                        {
                            continuation.Fail(errorWhileDownload);
                        }
                    },
                    null);
            }
        }

        private void FindDataServiceContext(IAsyncContinuation continuation)
        {
            if (this.SkipClientCodeGeneration)
            {
                continuation.Continue();
                return;
            }

            foreach (var asm in this.CurrentWorkspace.Assemblies.Select(c => c.Contents))
            {
                var contextType = asm.GetExportedTypes().Where(t => typeof(DataServiceContext).IsAssignableFrom(t)).SingleOrDefault();
                if (contextType != null)
                {
                    this.CurrentWorkspace.ContextType = contextType;
                    this.Logger.WriteLine(LogLevel.Verbose, "Data Service Context: {0}", this.CurrentWorkspace.ContextType.AssemblyQualifiedName);
                }
            }

            ExceptionUtilities.Assert(this.CurrentWorkspace.ContextType != null, "DataServiceContext was not found in the compiled assembly.");
            continuation.Continue();
        }
    }
}
