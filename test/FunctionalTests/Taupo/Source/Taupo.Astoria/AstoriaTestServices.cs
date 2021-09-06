//---------------------------------------------------------------------
// <copyright file="AstoriaTestServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Client;
    using Microsoft.Test.Taupo.Astoria.Common;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.Contracts.ServiceReferences;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataOracleService.DotNet;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.EntityModel.Edm;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.Linq;
    using Microsoft.Test.Taupo.Spatial.EntityModel;
    
    /// <summary>
    /// Base class for all astoria tests.
    /// </summary>
    public static class AstoriaTestServices
    {
        /// <summary>
        /// Configures the dependencies for the test case.
        /// </summary>
        /// <param name="container">The container (private to the test case).</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "It is test case base which needs to handle dependency injections of contracts")]
        public static void ConfigureDependencies(DependencyInjectionContainer container)
        {
            ILinqToAstoriaQueryEvaluationStrategy strategy;
            if (!container.TryResolve<ILinqToAstoriaQueryEvaluationStrategy>(out strategy))
            {
                container.Register<ILinqToAstoriaQueryEvaluationStrategy, LinqToAstoriaClrQueryEvaluationStrategy>();
                container.Register<IPrimitiveDataTypeResolver, NullPrimitiveDataTypeResolver>();
            }

            container.Register<IQueryDataSetBuilder, AstoriaQueryDataSetBuilder>();
            container.Register<IQueryRepositoryBuilder, AstoriaQueryRepositoryBuilderBase>();
            container.Register<IQueryExpressionEvaluator, LinqToAstoriaEvaluator>();
            container.Register<ILinqToAstoriaQueryResolver, LinqToAstoriaQueryResolver>();
            container.Register<ILinqToAstoriaQuerySpanGenerator, LinqToAstoriaQuerySpanGenerator>();
            container.Register<IClientQueryResultComparer, ClientQueryResultComparer>();
            container.Register<IOracleBasedDataSynchronizer, QueryDataSetSynchronizer>();
            container.Register<ILinqResultComparerContextAdapter, LinqResultComparerDataServiceContextAdapter>();
            container.Register<IQueryValueDeepCopyingVisitor, AstoriaQueryValueDeepCopyingVisitor>();
            container.Register<ISpatialClrTypeResolver, SpatialClrTypeResolver>();
            container.Register<ISpatialDataTypeDefinitionResolver, SpatialDataTypeDefinitionResolver>();

            container.RegisterCustomResolver(
                typeof(QueryRepository),
                resolver =>
                {
                    var workspace = container.Resolve<AstoriaWorkspace>();
                    if (workspace.CurrentQueryRepository == null)
                    {
                        var queryTypeLibrary = container.Resolve<QueryTypeLibrary>();
                        var queryRepositoryBuilder = container.Resolve<IQueryRepositoryBuilder>();
                        var queryRepository = queryRepositoryBuilder.CreateQueryRepository(workspace.ConceptualModel, queryTypeLibrary, workspace.DownloadedEntityContainerData);
                        workspace.CurrentQueryRepository = queryRepository;
                    }

                    return workspace.CurrentQueryRepository;
                });

            container.RegisterCustomResolver(
               typeof(IQueryDataSet),
               resolver =>
               {
                   var queryRepository = container.Resolve<QueryRepository>();
                   return queryRepository.DataSet;
               });

            container.RegisterServiceReference<IDataOracleService>(
               () =>
               {
                   var workspace = container.Resolve<AstoriaWorkspace>();
                   return workspace.OracleServiceUri;
               });

            container.RegisterCustomResolver(
                typeof(IEntitySetResolver),
                esResolver =>
                {
                    var workspace = container.Resolve<AstoriaWorkspace>();
                    return workspace.EntitySetResolver;
                });

            container.RegisterCustomResolver(
                typeof(IQueryTypeLibraryBuilder),
                queryTypeLibBuilderResolver =>
                {
                    var queryEvaluationStrategy = container.Resolve<ILinqToAstoriaQueryEvaluationStrategy>();
                    var primitiveTypeResolver = container.Resolve<IPrimitiveDataTypeResolver>();
                    var edmDataTypeResolver = container.Resolve<EdmDataTypeResolver>();
                    return new AstoriaQueryTypeLibraryBuilder(queryEvaluationStrategy, primitiveTypeResolver, edmDataTypeResolver);
                });

            container.RegisterCustomResolver(
                typeof(QueryTypeLibrary),
                queryTypeLib =>
                {
                    var workspace = container.Resolve<AstoriaWorkspace>();
                    if (workspace.CurrentQueryRepository == null)
                    {
                        var queryTypeLibraryBuilder = container.Resolve<IQueryTypeLibraryBuilder>();
                        var queryTypeLibrary = queryTypeLibraryBuilder.BuildLibraryWithoutClrTypeMapping(workspace.ConceptualModel);
                        queryTypeLibrary.UpdateClrTypeMapping(workspace.Assemblies.Select(ac => ac.Contents).ToArray());
                        return queryTypeLibrary;
                    }
                    else
                    {
                        return workspace.CurrentQueryRepository.TypeLibrary;
                    }
                });

            container.RegisterCustomResolver(
                typeof(IEntityModelConceptualDataServices),
                t =>
                {
                    var workspace = container.Resolve<AstoriaWorkspace>();

                    // Add data generation hints before creating structural data services
                    ResolveDataGenerationHints(workspace.ConceptualModel, container);

                    var builder = container.Resolve<IEntityModelConceptualDataServicesFactory>();
                    var services = builder.CreateConceptualDataServices(workspace.ConceptualModel);
                    ExceptionUtilities.CheckObjectNotNull(services, "Structural data services builder returned null unexpectedly");

                    // use a data population driver to force all the keys forward
                    InitializeEntityModelStructuralDataServices(services, workspace.ConceptualModel.EntityContainers, container);

                    return services;
                });

            container.RegisterCustomResolver(
                typeof(EntityModelSchema),
                t =>
                {
                    var workspace = container.Resolve<AstoriaWorkspace>();
                    return workspace.ConceptualModel;
                });

            container.RegisterCustomResolver(
              typeof(IClientExpectedErrorComparer),
              t =>
              {
                  var workspace = container.Resolve<AstoriaWorkspace>();
                  var payloadFormatStrategy = container.Resolve<IProtocolFormatStrategySelector>();
                  return new ClientExpectedErrorComparer(workspace.SystemDataServicesStringVerifier, workspace.SystemDataServicesClientStringVerifier) { ProtocolFormatStrategySelector = payloadFormatStrategy };
              });
        }

        private static void ResolveDataGenerationHints(EntityModelSchema model, DependencyInjectionContainer container)
        {
            var entityModelHintsResolver = container.Resolve<IEntityModelDataGenerationHintsResolver>();
            var primitiveResolver = container.Resolve<IPrimitiveDataTypeToDataGenerationHintsResolver>();

            entityModelHintsResolver.ResolveDataGenerationHints(model, primitiveResolver);
        }

        private static void InitializeEntityModelStructuralDataServices(IEntityModelConceptualDataServices services, IEnumerable<EntityContainer> entityContainers, DependencyInjectionContainer container)
        {
            var driver = container.Resolve<IEntityContainerDataPopulationDriver>();
            driver.StructuralDataServices = services;
            driver.Random = container.Resolve<IRandomNumberGenerator>();
            driver.ThresholdForNumberOfEntities = -1;

            foreach (var entityContainer in entityContainers)
            {
                driver.EntityContainer = entityContainer;
                EntityContainerData data;
                driver.TryPopulateNextData(out data);
                driver.TryPopulateNextData(out data);
            }
        }
    }
}
