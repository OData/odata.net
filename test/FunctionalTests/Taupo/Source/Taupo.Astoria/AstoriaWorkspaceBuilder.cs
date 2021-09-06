//---------------------------------------------------------------------
// <copyright file="AstoriaWorkspaceBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using Contracts.Http;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataServiceBuilderService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.Utilities;
    
    /// <summary>
    /// Builds Astoria workspaces.
    /// </summary>
    [ImplementationName(typeof(IAstoriaWorkspaceBuilder), "Default")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Added MaxVersion for data service builder fixup")]
    public class AstoriaWorkspaceBuilder : AstoriaWorkspaceBuilderBase
    {
        public const int DefaultDataServiceInstallWaitTimeSeconds = 60;
        
        /// <summary>
        /// Initializes a new instance of the AstoriaWorkspaceBuilder class.
        /// </summary>
        public AstoriaWorkspaceBuilder()
        {
            this.WaitUntilServiceInstalledTimeoutSeconds = 60;
            this.ShouldWaitUntilServiceInstalled = true;
            this.SkipEdmModelDownload = false;
        }

        /// <summary>
        /// Gets or sets the data service builder.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IDataServiceBuilderService DataServiceBuilder { get; set; }

        /// <summary>
        /// Gets or sets the CSDL generator.
        /// </summary>
        /// <value>The CSDL generator.</value>
        [InjectDependency(IsRequired = true)]
        public ICsdlContentGenerator CsdlGenerator { get; set; }

        /// <summary>
        /// Gets or sets the CSDL parser.
        /// </summary>
        /// <value>The CSDL parser.</value>
        [InjectDependency(IsRequired = true)]
        public ICsdlParser CsdlParser { get; set; }

        /// <summary>
        /// Gets or sets the deployment settings to be applied when installing generated service.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public DeploymentSettings DeploymentSettings { get; set; }

        /// <summary>
        /// Gets or sets the random number generator to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets or sets the name of the random number generator to provide to the service builder
        /// </summary>
        [InjectTestParameter("RNG")]
        public string RandomImplementationName { get; set; }

        /// <summary>
        /// Gets or sets the initial seed for the service builder. If null, a random number will be used.
        /// </summary>
        [InjectTestParameter("ServiceBuilderSeed")]
        public int? ServiceBuilderInitialSeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the data service to be installed or not, default is true
        /// </summary>
        [InjectTestParameter("ShouldWaitUntilServiceInstalled")]
        public bool ShouldWaitUntilServiceInstalled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to skip downloading the EdmModel
        /// </summary>
        [InjectTestParameter("SkipEdmModelDownload")]
        public bool SkipEdmModelDownload { get; set; }

        /// <summary>
        /// Gets or sets the amount of time in seconds to wait for the data service to be installed
        /// </summary>
        [InjectTestParameter("WaitUntilServiceInstalledTimeoutSeconds", DefaultValueDescription = "30 seconds wait time for service to be installed")]
        public int WaitUntilServiceInstalledTimeoutSeconds { get; set; }

        /// <summary>
        /// Builds the data service
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void BuildDataService(IAsyncContinuation continuation)
        {
            var csdlGen = this.CsdlGenerator as CsdlContentGenerator;
            if (csdlGen != null)
            {
                csdlGen.GenerateTaupoAnnotations = true;
                csdlGen.IgnoreUnsupportedAnnotations = true;
            }

            var csdlContent = this.CsdlGenerator.Generate(this.EdmVersion, this.CurrentWorkspace.ConceptualModel).ToArray();
            var csdlContentStrings = csdlContent.Select(c => c.Contents.ToString()).ToArray();

            string errorMessage;
            var settings = this.BuildServiceBuilderParameters();

            string deployerName = this.DeploymentSettings.ServiceDeployerKind;
            if (this.ShouldUseRelayService())
            {
                deployerName = DeploymentSettings.RelayServiceDeployerPrefix + deployerName;
            }

            this.DataServiceBuilder.BeginCreateCustomDataService(
                csdlContentStrings,
                this.DataProviderSettings.DataProviderKind,
                deployerName,
                settings,
                result => AsyncHelpers.CatchErrors(
                    continuation,
                    () =>
                    {
                        this.CurrentWorkspace.WorkspaceInfo = this.DataServiceBuilder.EndCreateCustomDataService(out errorMessage, result);
                        
                        ExceptionUtilities.CheckObjectNotNull(this.CurrentWorkspace.WorkspaceInfo, "Cannot get workspace info after creation. Error:" + errorMessage);

                        string serviceUri = this.CurrentWorkspace.WorkspaceInfo.ServiceUri;   
                        if (serviceUri == null)
                        {
                            continuation.Fail(new TaupoInfrastructureException(errorMessage));
                            return;
                        }

                        this.Logger.WriteLine(LogLevel.Verbose, "Service URI: {0}", serviceUri);
                        var builder = this.DataServiceBuilder;

                        this.CurrentWorkspace.OnDispose += (sender, e) =>
                            {
                                if (!this.SkipDataServiceDispose)
                                {
                                    // begin removal of the data service - note that nobody is waiting
                                    // for this operation to complete
                                    this.Logger.WriteLine(LogLevel.Verbose, "Uninstalling service {0} asynchronously", serviceUri);
                                    builder.BeginUninstallDataService(serviceUri, null, null);
                                }
                            };

                        this.CurrentWorkspace.ServiceUri = new Uri(serviceUri);
                        continuation.Continue();
                    }),
                    null);
        }

        /// <summary>
        /// Builds the Edm Model
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void BuildEdmModel(IAsyncContinuation continuation)
        {
            if (this.SkipEdmModelDownload)
            {
                continuation.Continue();
                return;
            }

            var serviceMetadataUri = new Uri(this.CurrentWorkspace.ServiceUri.AbsoluteUri + "/$metadata", UriKind.Absolute);
            this.Logger.WriteLine(LogLevel.Trace, "Retrieve metadata of service: {0}", serviceMetadataUri.AbsoluteUri);
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(serviceMetadataUri);
            httpWebRequest.Accept = MimeTypes.ApplicationXml;
             httpWebRequest.BeginGetResponse(
                (asyncResult) =>
                    {
                        AsyncHelpers.CatchErrors(
                            continuation,
                            () =>
                                {
                                    var webResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(asyncResult);
                                    ExceptionUtilities.Assert(webResponse.StatusCode == HttpStatusCode.OK, "Cannot query $metadata getting non OK status code '{0}'", webResponse.StatusCode);
                                    var stream = webResponse.GetResponseStream();
                                    this.CurrentWorkspace.EdmModel = GetEdmModelFromEdmxStream(stream);
                                    continuation.Continue();
                                });
                    },
                null);
        }

        /// <summary>
        /// Update entity model based on csdl sent back from server.
        /// </summary>
        /// <param name="continuation">The async continuation</param>
        protected override void UpdateEntityModel(IAsyncContinuation continuation)
        {
            // get CSDL contents
            List<string> csdlContent = new List<string>();
            foreach (var key in this.CurrentWorkspace.WorkspaceInfo.AdditionalProviderInfo.Keys.Where(k => k.StartsWith(AstoriaWorkspaceInfoConstants.CsdlKeyPrefix, StringComparison.OrdinalIgnoreCase)))
            {
                string content;
                ExceptionUtilities.Assert(this.CurrentWorkspace.WorkspaceInfo.AdditionalProviderInfo.TryGetValue(key, out content), "Cannot get csdl content with key {0}.", key);
                csdlContent.Add(content);
            }

            // preserve Annotations which were not serialised
            var functionAnnotationPreserver = new FunctionAnnotationPreserver();
            functionAnnotationPreserver.PreserveFunctionAnnotations(this.CurrentWorkspace.ConceptualModel.Functions);

            // parse CSDL into EntityModelSchema
            XElement[] xelements = csdlContent.Select(XElement.Parse).ToArray();
            this.CurrentWorkspace.ConceptualModel = this.CsdlParser.Parse(xelements);
            CustomAnnotationConverter customAnnotationConverter = new CustomAnnotationConverter();
            customAnnotationConverter.AnnotationAssemblies.Add(typeof(TypeBackedAnnotation).GetAssembly());
            customAnnotationConverter.AnnotationNamespaces.Add(typeof(TypeBackedAnnotation).Namespace);
            customAnnotationConverter.ConvertAnnotations(this.CurrentWorkspace.ConceptualModel);

            functionAnnotationPreserver.RestoreFunctionAnnotations(this.CurrentWorkspace.ConceptualModel.Functions);
            base.UpdateEntityModel(continuation);
        }

        /// <summary>
        /// Builds the system data services resource lookup.
        /// </summary>
        /// <returns>The resource lookup</returns>
        protected override IResourceLookup BuildSystemDataServicesResourceLookup()
        {
            string dataServicesResources;
            ExceptionUtilities.Assert(
                this.CurrentWorkspace.WorkspaceInfo.AdditionalProviderInfo.TryGetValue(AstoriaWorkspaceInfoConstants.SystemDataServicesResourcesKey, out dataServicesResources),
                "Could not get resource-string information from the service builder");

            var dictionary = SplitConcatenatedResourceStrings(dataServicesResources);

            return new DictionaryResourceLookup(dictionary);
        }

        private static IEdmModel GetEdmModelFromEdmxStream(Stream s)
        {
            // Update the EdmModel on the workspace
            IEdmModel model = null;
            IEnumerable<EdmError> errors = null;

            using (var xmlTextReader = XmlTextReader.Create(s, new XmlReaderSettings()))
            {
                ExceptionUtilities.Assert(CsdlReader.TryParse(xmlTextReader, out model, out errors), "Cannot read csdl model");
                ExceptionUtilities.Assert(!errors.Any(), "Errors on parsing csdl");
            }

            return model;
        }

        private static Dictionary<string, string> SplitConcatenatedResourceStrings(string resources)
        {
            var dictionary = new Dictionary<string, string>();

            var lines = resources.Split(new string[] { AstoriaWorkspaceInfoConstants.ResourceLineSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var separator = AstoriaWorkspaceInfoConstants.ResourceKeyValueSeparator;
                var index = line.IndexOf(separator, StringComparison.Ordinal);
                var key = line.Substring(0, index);
                var value = line.Substring(index + separator.Length);
                dictionary[key] = value;
            }

            return dictionary;
        }

        private bool ShouldUseRelayService()
        {
            var entitySets = this.CurrentWorkspace.ConceptualModel.EntityContainers.SelectMany(ec => ec.EntitySets);
            return entitySets.Any(entitySet => entitySet.Annotations.OfType<EntitySetEndPointAnnotation>().Any()) ||
                entitySets.Any(entitySet => entitySet.Annotations.OfType<NestedResourceInfoEndPointAnnotation>().Any()) ||
                entitySets.Any(entitySet => entitySet.Annotations.OfType<RelationshipLinkEndPointAnnotation>().Any()) ||
                entitySets.Any(entitySet => entitySet.Annotations.OfType<EditLinkReplacementAnnotation>().Any()) ||
                entitySets.Any(entitySet => entitySet.Annotations.OfType<EditMediaLinkReplacementAnnotation>().Any()) ||
                entitySets.Any(entitySet => entitySet.Annotations.OfType<RemoveEditLinkAnnotation>().Any());
        }

        private ServiceBuilderParameter[] BuildServiceBuilderParameters()
        {
            int? serviceCreationSeed = this.ServiceBuilderInitialSeed;
            if (!serviceCreationSeed.HasValue)
            {
                serviceCreationSeed = this.Random.Next(int.MaxValue);
            }

            this.Logger.WriteLine(LogLevel.Verbose, "Using '{0}' as the random number generator seed for the service builder", serviceCreationSeed.Value);

            var settings = new List<ServiceBuilderParameter>();
            settings.AddRange(this.DataProviderSettings.BuildSettings());
            settings.AddRange(this.DeploymentSettings.BuildSettings());
            settings.Add(new ServiceBuilderParameter() { Name = "Seed", Value = serviceCreationSeed.Value.ToString(CultureInfo.InvariantCulture) });

            var randomImplementationSelector = typeof(IRandomNumberGenerator).GetCustomAttributes(typeof(ImplementationSelectorAttribute), false).Cast<ImplementationSelectorAttribute>().Single();
            if (this.RandomImplementationName == null)
            {
                this.RandomImplementationName = randomImplementationSelector.DefaultImplementation;
            }

            settings.Add(new ServiceBuilderParameter() { Name = randomImplementationSelector.TestArgumentName, Value = this.RandomImplementationName });

            settings.Add(new ServiceBuilderParameter() { Name = "ShouldWaitUntilServiceInstalled", Value = this.ShouldWaitUntilServiceInstalled.ToString() });

            settings.Add(new ServiceBuilderParameter() { Name = "WaitUntilServiceInstalledTimeoutSeconds", Value = this.WaitUntilServiceInstalledTimeoutSeconds.ToString(CultureInfo.InvariantCulture) });

            return settings.ToArray();
        }

        /// <summary>
        /// Maintains non-serialisable annotations on Functions that may be lost during the entity model update from CSDL
        /// </summary>
        private class FunctionAnnotationPreserver
        {
            private Dictionary<string, IEnumerable<Annotation>> savedFunctionAnnotations = new Dictionary<string, IEnumerable<Annotation>>();

            internal void PreserveFunctionAnnotations(IEnumerable<Function> functions)
            {
                this.savedFunctionAnnotations.Clear();

                foreach (var f in functions)
                {
                    var nonSerialisedAnnotations = f.Annotations.Where(a => a.IsNotSerializable());
                    if (nonSerialisedAnnotations.Any())
                    {
                        this.savedFunctionAnnotations.Add(f.Name, nonSerialisedAnnotations.ToList());
                    }
                }
            }

            internal void RestoreFunctionAnnotations(IEnumerable<Function> functions)
            {
                if (this.savedFunctionAnnotations.Any())
                {
                    foreach (var annotations in this.savedFunctionAnnotations)
                    {
                        var serviceMethod = functions.Single(f => f.Name.Equals(annotations.Key));
                        serviceMethod.Annotations.AddRange(annotations.Value.Where(a => !serviceMethod.Annotations.Contains(a)));
                    }
                }
            }
        }
    }
}
