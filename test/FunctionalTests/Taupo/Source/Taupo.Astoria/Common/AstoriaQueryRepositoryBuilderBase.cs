//---------------------------------------------------------------------
// <copyright file="AstoriaQueryRepositoryBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// QueryRepository base with some implementations.
    /// </summary>
    public class AstoriaQueryRepositoryBuilderBase : QueryRepositoryBuilderBase
    {
        private QueryScalarType stringType;
        private QueryScalarType intType;

        /// <summary>
        /// Initializes a new instance of the AstoriaQueryRepositoryBuilderBase class
        /// </summary>
        public AstoriaQueryRepositoryBuilderBase()
        {
            this.CreateRepositoryFunc = base.CreateQueryRepository;
            this.IncludeOfTypeInRootQueries = false;
        }

        /// <summary>
        /// Gets or sets the query evaluation strategy
        /// </summary>
        /// <value>The query evaluation strategy.</value>
        [InjectDependency(IsRequired = true)]
        public ILinqToAstoriaQueryEvaluationStrategy QueryEvaluationStrategy { get; set; }

        /// <summary>
        /// Gets or sets the store data type resolver. Null if not running EF provider.
        /// </summary>
        /// <value>The store data type resolver.</value>
        [InjectDependency(IsRequired = true)]
        public IPrimitiveDataTypeResolver StoreDataTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include OfType in root queries
        /// </summary>
        /// <value>The store data type resolver.</value>
        [InjectTestParameter("IncludeOfTypeInRootQueries", DefaultValueDescription = "false", HelpText = "Include Root(Query).OfType in RootQueries")]
        public bool IncludeOfTypeInRootQueries { get; set; }

        /// <summary>
        /// Gets or sets the method to use for building the query repository. Default is to use QueryRepositoryBuilderBase.CreateQueryRepository(), but this is used for testability.
        /// </summary>
        internal Func<EntityModelSchema, QueryTypeLibrary, EntityContainerData, QueryRepository> CreateRepositoryFunc { get; set; }

        /// <summary>
        /// Factory method that creates a new instance of the QueryRepository.
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        /// <param name="entityContainerData">Entity Container Data</param>
        /// <returns>An instance of the QueryRepository class.</returns>
        public override QueryRepository CreateQueryRepository(EntityModelSchema entityModelSchema, QueryTypeLibrary queryTypeLibrary, EntityContainerData entityContainerData)
        {
            return this.CreateRepositoryFunc(entityModelSchema, queryTypeLibrary, entityContainerData);
        }

        /// <summary>
        /// Build the collection of primitive types which will be set on the constructed repository
        /// </summary>
        /// <param name="queryTypeLibrary">Query Type Library to build Types from</param>
        protected override void BuildPrimitiveTypes(QueryTypeLibrary queryTypeLibrary)
        {
            this.intType = (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Int32);
            this.stringType = (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.String());

            this.PrimitiveTypes = new List<QueryScalarType>()
            {
                // this is just a sample
                this.intType,
                this.stringType,
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Boolean),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.DateTime()),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Decimal()),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Int64),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Int16),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Byte),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Single),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Double),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Binary()),
                (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Guid),
            };
        }

        /// <summary>
        /// Returns a value indicating whether we should add a RootQuery for this entity set
        /// </summary>
        /// <param name="entitySet">The entity set to add a root query for</param>
        /// <returns>A value indicating whether we should add a RootQuery for this entity set</returns>
        protected override bool ShouldCreateRootQuery(EntitySet entitySet)
        {
            return entitySet.SupportsQuery();
        }

        /// <summary>
        /// Override the default method, adding generation of root queries that call designated service operations
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        protected override void BuildRootQueriesAndTypes(EntityModelSchema entityModelSchema, QueryTypeLibrary queryTypeLibrary)
        {
            base.BuildRootQueriesAndTypes(entityModelSchema, queryTypeLibrary);

            // For each service operation marked as root, add a root query that calls the operation
            var rootServiceOperationQueries = new List<QueryExpression>();
            foreach (var serviceOperation in entityModelSchema.Functions.Where(f => f.Annotations.OfType<FunctionBodyAnnotation>().Any(a => a.IsRoot)))
            {
                QueryExpression bodyExpression = serviceOperation.Annotations.OfType<FunctionBodyAnnotation>().Single().FunctionBody;
                ExceptionUtilities.CheckObjectNotNull(bodyExpression, "Root level function has null body expression");

                QueryType rootQueryType = queryTypeLibrary.GetDefaultQueryType(serviceOperation.ReturnType);
                var rootQuery = new QueryCustomFunctionCallExpression(rootQueryType, serviceOperation, bodyExpression, true, false);
                rootServiceOperationQueries.Add(rootQuery);

                QueryStructuralType structuralType = rootQueryType as QueryStructuralType;
                if (structuralType == null)
                {
                    QueryCollectionType collectionType = rootQueryType as QueryCollectionType;
                    if (collectionType != null)
                    {
                        structuralType = collectionType.ElementType as QueryStructuralType;
                    }
                }

                ExceptionUtilities.CheckObjectNotNull(structuralType, "Root level service op query must return structural type");
                this.RootDataTypes.Add(serviceOperation.Name, structuralType);
            }

            this.RootQueries = this.RootQueries.Concat(rootServiceOperationQueries);
        }
    }
}