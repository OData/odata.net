//---------------------------------------------------------------------
// <copyright file="AstoriaQueryTypeLibraryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.EntityModel;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Query type library builder for Astoria
    /// </summary>
    public class AstoriaQueryTypeLibraryBuilder : QueryTypeLibraryBuilderBase
    {  
        private ILinqToAstoriaQueryEvaluationStrategy strategy;

        /// <summary>
        /// store data type resolver use for creating QueryMappedScalarType if using EF provider.
        /// </summary>
        private IPrimitiveDataTypeResolver storeTypeResolver;

        /// <summary>
        /// edm model type resolver use for creating QueryMappedScalarType if using EF provider.
        /// </summary>
        private IPrimitiveDataTypeResolver modelTypeResolver;

        /// <summary>
        /// Initializes a new instance of the AstoriaQueryTypeLibraryBuilder class if running EF provider.
        /// </summary>
        /// <param name="strategy">The query evaluation strategy</param>
        /// <param name="storeTypeResolver">The store data type resolver</param>
        /// <param name="modelTypeResolver">The model data type resolver</param>
        public AstoriaQueryTypeLibraryBuilder(ILinqToAstoriaQueryEvaluationStrategy strategy, IPrimitiveDataTypeResolver storeTypeResolver, IPrimitiveDataTypeResolver modelTypeResolver)
            : base(strategy)
        {
            ExceptionUtilities.CheckArgumentNotNull(strategy, "strategy");
            ExceptionUtilities.CheckArgumentNotNull(storeTypeResolver, "storeTypeResolver");
            ExceptionUtilities.CheckArgumentNotNull(modelTypeResolver, "modelTypeResolver");

            this.strategy = strategy;
            this.storeTypeResolver = storeTypeResolver;
            this.modelTypeResolver = modelTypeResolver;
        }

        /// <summary>
        /// Build Library Without Clr Type Mappings based on the EntityModelSchema given
        /// </summary>
        /// <param name="model">Entity Model Schema to create Query Model of</param>
        /// <returns>Query Type Library of Entity Model Schema</returns>
        public override QueryTypeLibrary BuildLibraryWithoutClrTypeMapping(EntityModelSchema model)
        {
            QueryTypeLibrary queryTypeLibrary = base.BuildLibraryWithoutClrTypeMapping(model);
            
            // add new properties for each of the named streams annotations to the query entity type to be used for verification
            foreach (var container in model.EntityContainers)
            {
                foreach (var entitySet in container.EntitySets)
                {
                    foreach (EntityType entityType in model.EntityTypes.Where(t => t.IsKindOf(entitySet.EntityType)))
                    {
                        QueryEntityType queryEntityType = queryTypeLibrary.GetQueryEntityType(entitySet, entityType);

                        // add the named streams properties
                        foreach (var streamProperty in queryEntityType.EntityType.Properties.Where(p => p.IsStream()))
                        {
                            queryEntityType.Add(new QueryProperty<AstoriaQueryStreamType>(streamProperty.Name, new AstoriaQueryStreamType(this.strategy)));
                            foreach (QueryEntityType qet in queryEntityType.DerivedTypes)
                            {
                                qet.Add(new QueryProperty<AstoriaQueryStreamType>(streamProperty.Name, new AstoriaQueryStreamType(this.strategy)));
                            }
                        }

                        // add the default stream property
                        if (entityType.HasStream())
                        {
                            var queryStreamType = new AstoriaQueryStreamType(this.strategy);
                            queryEntityType.Add(new QueryProperty<AstoriaQueryStreamType>(AstoriaQueryStreamType.DefaultStreamPropertyName, queryStreamType));
                            foreach (QueryEntityType qet in queryEntityType.DerivedTypes)
                            {
                                qet.Add(new QueryProperty<AstoriaQueryStreamType>(AstoriaQueryStreamType.DefaultStreamPropertyName, new AstoriaQueryStreamType(this.strategy)));
                            }
                        }
                    }
                }
            }

            return queryTypeLibrary;
        }

        /// <summary>
        /// Creates a QueryProperty for a property that is a non entity Collection 
        /// </summary>
        /// <param name="library">Library Query Type</param>
        /// <param name="result">resulting Query Structural Type</param>
        /// <param name="collectionProperty">Member to calculate</param>
        /// <param name="pathToProperty">Path to the Property</param>
        /// <returns>A Query Property of the collectionType</returns>
        protected override QueryProperty CreateNonentityCollectionMember(QueryTypeLibrary library, QueryStructuralType result, MemberProperty collectionProperty, PathToProperty pathToProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(collectionProperty, "collectionProperty");

            var collectionType = collectionProperty.PropertyType as CollectionDataType;
            ExceptionUtilities.Assert(collectionType != null, "This type of property is not supported.");
            var collectionPrimitiveElementType = collectionType.ElementDataType as PrimitiveDataType;
            var collectionComplexElementType = collectionType.ElementDataType as ComplexDataType;
            QueryCollectionType queryCollectionType = null;
            if (collectionPrimitiveElementType != null)
            {
                QueryScalarType queryPropertyType = this.GetQueryTypeForMappedProperty(pathToProperty, library, collectionPrimitiveElementType);
                queryCollectionType = queryPropertyType.CreateCollectionType();
            }
            else
            {
                ExceptionUtilities.Assert(collectionComplexElementType != null, "This type of property is not supported.");
                QueryComplexType queryComplexType = this.CreateStubComplexType(collectionComplexElementType.Definition);
                CreateMembers(library, queryComplexType, collectionComplexElementType.Definition.Properties, pathToProperty);
                queryCollectionType = queryComplexType.CreateCollectionType();
            }

            return QueryProperty.Create(collectionProperty.Name, queryCollectionType);
        }

        /// <summary>
        /// Create initial query type library. If running on EF provider, use QueryMappedScalarType instead of QueryClrPrimitiveType.
        /// </summary>
        /// <returns>initial query type library</returns>
        protected override QueryTypeLibrary CreateInitialQueryTypeLibrary()
        {
            var storeTypeResolverToUse = this.storeTypeResolver;
            if (this.storeTypeResolver is NullPrimitiveDataTypeResolver)
            {
                storeTypeResolverToUse = null;
            }

            return new QueryTypeLibrary(this.strategy, storeTypeResolverToUse, this.modelTypeResolver); 
        }
    }
}
