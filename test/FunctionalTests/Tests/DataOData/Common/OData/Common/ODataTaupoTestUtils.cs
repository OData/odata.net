//---------------------------------------------------------------------
// <copyright file="ODataTaupoTestUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    using Microsoft.Test.Taupo.Astoria;
    using Microsoft.Test.Taupo.Astoria.Client;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.OData.PayloadTransformation;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Spatial.EntityModel;
    using Microsoft.Test.Taupo.Utilities;

    public static class ODataTaupoTestUtils
    {
        /// <summary>
        /// The configuration of these dependencies will be called from other testcase hierarchies that do not inherit from this class.
        /// This method can be used in such hierarchies.
        /// </summary>
        /// <param name="container">container on which to register impelemntation dependencies</param>
        public static void ConfigureDependenciesHelper(DependencyInjectionContainer container)
        {
            container.Register<IClrTypeReferenceResolver, StronglyTypedClrTypeReferenceResolver>();
            container.Register<IObjectLayerCodeGenerator, StronglyTypedObjectLayerCodeGenerator>();
            container.Register<IQueryEvaluationStrategy, ClrQueryEvaluationStrategy>();
            container.Register<IModelGenerator, ODataV2ModelGenerator>();
            container.Register<IWorkspaceBuilder, ODataWorkspaceBuilder>(); // Is this also needed , since we register Custom Resolver below
            container.Register<IQueryRepositoryBuilder, AstoriaQueryRepositoryBuilderBase>();
            container.Register<IQueryDataSetBuilder, ODataQueryDataSetBuilder>();

            container.Register<IResourceLookup, AssemblyResourceLookup>();
            container.Register<IStringResourceVerifier, StringResourceVerifier>();
            container.Register<IQueryScalarValueToClrValueComparer, QueryScalarValueToClrValueComparer>();
            container.Register<IQueryExpressionEvaluator, LinqToAstoriaEvaluator>();
            container.Register<IClientCodeLayerGenerator, PocoClientCodeLayerGenerator>();

            container.Register<ILinqToAstoriaQueryEvaluationStrategy, LinqToAstoriaClrQueryEvaluationStrategy>();
            container.Register<IPrimitiveDataTypeResolver, NullPrimitiveDataTypeResolver>();
            container.Register<ILinqResultComparerContextAdapter, ODataObjectResultComparerContextAdapter>();

            container.Register<ISpatialClrTypeResolver, SpatialClrTypeResolver>();
            container.Register<ISpatialDataTypeDefinitionResolver, SpatialDataTypeDefinitionResolver>();
            container.Register<IPayloadTransformFactory, ODataLibPayloadTransformFactory>();

            container.RegisterCustomResolver(
                typeof(ODataTestWorkspace),
                t =>
                {
                    var builder = container.Resolve<ODataWorkspaceBuilder>();
                    var modelGenerator = container.Resolve<IModelGenerator>();
                    var workspace = builder.BuildWorkspace(modelGenerator.GenerateModel());
                    
                    return workspace;
                });

            container.RegisterCustomResolver(
                typeof(IQueryTypeLibraryBuilder),
                queryTypeLibBuilderResolver =>
                {
                    var queryEvaluationStrategy = container.Resolve<ILinqToAstoriaQueryEvaluationStrategy>();
                    var primitiveTypeResolver = container.Resolve<IPrimitiveDataTypeResolver>();
                    return new AstoriaQueryTypeLibraryBuilder(queryEvaluationStrategy, primitiveTypeResolver, new EdmDataTypeResolver());
                });

            // IQueryDataSet is built by the repository builder
            container.RegisterCustomResolver(
                typeof(IQueryDataSet),
                    t =>
                    {
                        var repository = container.Resolve<QueryRepository>();
                        return repository.DataSet;
                    });

            container.RegisterCustomResolver(
                typeof(QueryTypeLibrary),
                queryTypeLib =>
                {
                    var workspace = container.Resolve<ODataTestWorkspace>();
                    var queryTypeLibraryBuilder = container.Resolve<IQueryTypeLibraryBuilder>();
                    var queryTypeLibrary = queryTypeLibraryBuilder.BuildLibraryWithoutClrTypeMapping(workspace.ConceptualModel);
                    if (workspace.ObjectLayerAssembly != null)
                    {
                        queryTypeLibrary.UpdateClrTypeMapping(new[] { workspace.ObjectLayerAssembly });
                    }

                    return queryTypeLibrary;
                });

            container.RegisterCustomResolver(
                typeof(EntityContainerData),
                t =>
                {
                    // TODO: All Query tests fail when there is actual data, for now starting with empty data
                    var workspace = container.Resolve<ODataTestWorkspace>();
                    EntityContainerData data = null;
                    data = new EntityContainerData(workspace.ConceptualModel.GetDefaultEntityContainer());
                    
                    //var dataPopulationDriver = container.Resolve<EntityContainerDataPopulationDriver>();
                    //dataPopulationDriver.ThresholdForNumberOfEntities = -1;
                    //dataPopulationDriver.EntityContainer = workspace.ConceptualModel.GetDefaultEntityContainer();
                    //dataPopulationDriver.TryPopulateNextData(out data);
                    
                    return data;
                });

            // QueryRepository is constructed by calling IQueryRepositoryBuilder.CreateQueryRepository
            container.RegisterCustomResolver(
                typeof(QueryRepository),
                t =>
                {
                    var repositoryBuilder = container.Resolve<IQueryRepositoryBuilder>();
                    var workspace = container.Resolve<ODataTestWorkspace>();
                    var queryTypeLibrary = container.Resolve<QueryTypeLibrary>();
                    var entityContaineData = container.Resolve<EntityContainerData>();

                    var queryRepository = repositoryBuilder.CreateQueryRepository(workspace.ConceptualModel, queryTypeLibrary, entityContaineData);
                    
                    return queryRepository;
                });

            container.RegisterCustomResolver(
                typeof(IEntityModelConceptualDataServices),
                t =>
                {
                    var workspace = container.Resolve<ODataTestWorkspace>();

                    // Add data generation hints before creating structural data services
                    ODataTaupoTestUtils.ResolveDataGenerationHints(workspace.ConceptualModel, container);

                    var builder = container.Resolve<IEntityModelConceptualDataServicesFactory>();
                    var services = builder.CreateConceptualDataServices(workspace.ConceptualModel);
                    ExceptionUtilities.CheckObjectNotNull(services, "Structural data services builder returned null unexpectedly");

                    return services;
                });

            container.RegisterCustomResolver(typeof(IAsyncDataSynchronizer), t => null);
        }

        private static void ResolveDataGenerationHints(EntityModelSchema model, DependencyInjectionContainer container)
        {
            var entityModelHintsResolver = container.Resolve<IEntityModelDataGenerationHintsResolver>();
            var primitiveResolver = container.Resolve<IPrimitiveDataTypeToDataGenerationHintsResolver>();

            entityModelHintsResolver.ResolveDataGenerationHints(model, primitiveResolver);
        }
    }
}
