//---------------------------------------------------------------------
// <copyright file="AstoriaTestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataServiceBuilderService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.WebServices.CompilerService.DotNet;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Spatial.Contracts;
    using Microsoft.Test.Taupo.Spatial.EntityModel;

    /// <summary>
    /// Base class for all test using Astoria.
    /// </summary>
    public abstract class AstoriaTestModule : TestModule
    {
        /// <summary>
        /// Gets or sets the uri for the compiler service
        /// </summary>
        [InjectTestParameter("CompilerServiceUri", HelpText = "URI Address of the compiler service")]
        public Uri CompilerServiceUri { get; set; }

        /// <summary>
        /// Gets or sets the uri for the data service builder service
        /// </summary>
        [InjectTestParameter("DataServiceBuilderServiceUri", HelpText = "URI Address of the data service builder service")]
        public Uri DataServiceBuilderServiceUri { get; set; }

        /// <summary>
        /// Gets or sets the uri for the compiler service
        /// </summary>
        [InjectTestParameter("TestServicesBaseUri", HelpText = "Base URI of test services")]
        public Uri TestServicesBaseUri { get; set; }

        /// <summary>
        /// Gets the workspace builder for the test module.
        /// </summary>
        /// <returns>Instance of <see cref="IWorkspaceBuilder" /> or null if no workspace is needed.</returns>
        protected override sealed IWorkspaceBuilder GetWorkspaceBuilder()
        {
            // The module should never be responsible for creating the workspace.
            return null;
        }

        /// <summary>
        /// Adds required assemblies to the <see cref="IImplementationSelector"/>
        /// </summary>
        /// <param name="implementationSelector">Selector to be configured.</param>
        protected override void ConfigureImplementationSelector(IImplementationSelector implementationSelector)
        {
            base.ConfigureImplementationSelector(implementationSelector);
            implementationSelector.AddAssembly(typeof(AstoriaWorkspaceBuilder).GetAssembly());
            implementationSelector.AddAssembly(typeof(MajorAstoriaReleaseVersion).GetAssembly());
            implementationSelector.AddAssembly(typeof(QueryExpression).GetAssembly());
            implementationSelector.AddAssembly(typeof(SpatialTypeKind).GetAssembly());
            implementationSelector.AddAssembly(this.GetType().GetAssembly());
        }

        /// <summary>
        /// Sets up dependecies for this <see cref="TestModule"/>
        /// </summary>
        /// <param name="container">Dependency injection container.</param>
        protected override void ConfigureDependencies(DependencyInjectionContainer container)
        {
            base.ConfigureDependencies(container);
            container.RegisterServiceReference<IDataServiceBuilderService>(() => this.DataServiceBuilderServiceUri, () => this.TestServicesBaseUri, "TestServices/DataServiceBuilder.svc");

            // in Silverlight, we want to do the same default uri for the compiler service. 
            // However, on the desktop we want to do local compiliation unless a specific uri has been specified
            container.RegisterServiceReference<ICompilerService>(() => this.CompilerServiceUri);
            // need to do this here so that the workspace builder can see it, which happens before the test case is initialized
            container.Register<ISpatialClrTypeResolver, SpatialClrTypeResolver>();
            container.Register<ISpatialDataTypeDefinitionResolver, SpatialDataTypeDefinitionResolver>();
        }
    }
}
