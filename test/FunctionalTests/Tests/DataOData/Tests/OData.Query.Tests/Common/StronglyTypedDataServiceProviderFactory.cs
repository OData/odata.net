//---------------------------------------------------------------------
// <copyright file="StronglyTypedDataServiceProviderFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Query.Tests.Common
{
    #region Namespaces
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.OData;
    using Microsoft.Test.Taupo.OData.Common;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    using Microsoft.Test.Taupo.OData.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    #endregion Namespaces

    /// <summary>
    /// DataServiceMetadataProviderGenerator to generate metadata from a model which is backed by clr types.
    /// </summary>
    [ImplementationName(typeof(IDataServiceProviderFactory), "Default")]
    public class StronglyTypedDataServiceProviderFactory : DataServiceProviderFactory
    {
        /// <summary>
        /// Gets or sets the test case's query evaluator.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryExpressionEvaluator Evaluator { get; set; }

        /// <summary>
        /// Gets or sets the test case's query repository.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryRepository Repository { get; set; }

        /// <summary>
        /// Gets or sets the test case's workspace
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public ODataTestWorkspace Workspace { get; set; }

        /// <summary>
        /// Generates a query resolver to resolve entity sets to IQueryable
        /// </summary>
        /// <param name="model">The conceptual schema for the workspace</param>
        /// <returns>a Query resolver to resolver entity sets to IQueryable</returns>
        public override IODataQueryProvider CreateQueryProvider(EntityModelSchema model)
        {
            var queryProvider = new ClrBasedQueryProvider(this.Workspace.ObjectLayerAssembly, model, this.Evaluator, this.Repository);
            queryProvider.IsNullPropagationRequired = NullPropagationRequired;
            return queryProvider;
        }

        /// <summary>
        /// Gets the CLR instance type for the specified complex type.
        /// </summary>
        /// <param name="complex">The complex type to get the instance type for.</param>
        /// <param name="canReflectOnInstanceType">true if reflection over the instance type is allowed; otherwise false.</param>
        /// <returns>The CLR instance type to use.</returns>
        protected override Type GetComplexInstanceType(ComplexType complex, out bool canReflectOnInstanceType)
        {
            Type complexType = null;
            ClrTypeAnnotation clrTypeAnnotation = complex.Annotations.OfType<ClrTypeAnnotation>().FirstOrDefault();

            if (clrTypeAnnotation == null)
            {
                complexType = (from exportedType in Workspace.ObjectLayerAssembly.GetExportedTypes()
                               where exportedType.Name == complex.Name
                               select exportedType).SingleOrDefault();
            }
            else
            {
                complexType = clrTypeAnnotation.ClrType;
            }

            ExceptionUtilities.CheckObjectNotNull(complexType, "Could not find complex type '{0}' in object layer", complex.Name);

            canReflectOnInstanceType = true;
            return complexType;
        }

        /// <summary>
        /// Gets the CLR instance type for the specified entity type.
        /// </summary>
        /// <param name="entity">The entity type to get the instance type for.</param>
        /// <param name="canReflectOnInstanceType">true if reflection over the instance type is allowed; otherwise false.</param>
        /// <returns>The CLR instance type to use.</returns>
        protected override Type GetEntityInstanceType(EntityType entity, out bool canReflectOnInstanceType)
        {
            Type instanceType = null;
            ClrTypeAnnotation clrTypeAnnotation = entity.Annotations.OfType<ClrTypeAnnotation>().FirstOrDefault();

            if (clrTypeAnnotation == null)
            {
                instanceType = (from exportedType in Workspace.ObjectLayerAssembly.GetExportedTypes()
                                where exportedType.Name == entity.Name
                                select exportedType).SingleOrDefault();
            }
            else
            {
                instanceType = clrTypeAnnotation.ClrType;
            }

            ExceptionUtilities.CheckObjectNotNull(instanceType, "Could not find instance type '{0}' in object layer", entity.Name);

            canReflectOnInstanceType = true;
            return instanceType;
        }

        /// <summary>
        /// Internal implementation of IODataQueryProvider which provides a Linq to Objects baed query root
        /// </summary>
        private class ClrBasedQueryProvider : IODataQueryProvider
        {
            private Assembly clrTypesAssembly;
            private EntityModelSchema modelSchema;
            private IQueryExpressionEvaluator evaluator;
            private QueryRepository queryRepository;

            /// <summary>
            /// Initializes a new instance of the ClrBasedQueryProvider type
            /// </summary>
            /// <param name="clrTypesAssembly">The assembly which contains the clr types for resource types</param>
            /// <param name="modelSchema">The schema for the model under test</param>
            /// <param name="evaluator">The evaluator to evaluate queries created after resolution</param>
            /// <param name="repository">The query repository to find root queries in</param>
            public ClrBasedQueryProvider(Assembly clrTypesAssembly, EntityModelSchema modelSchema, IQueryExpressionEvaluator evaluator, QueryRepository repository)
            {
                this.clrTypesAssembly = clrTypesAssembly;
                this.modelSchema = modelSchema;
                this.evaluator = evaluator;
                this.queryRepository = repository;
            }

            /// <summary>
            /// Gets or sets a value indicating whether Null propagation is required for the Linq translator
            /// </summary>
            public bool IsNullPropagationRequired { get; set; }

            /// <summary>
            /// Returns the IQueryable that represents the entity set.
            /// </summary>
            /// <param name="entitySet">The entity set.</param>
            /// <returns>
            /// An IQueryable that represents the set; null if there is 
            /// no set for the specified name.
            /// </returns>
            public IQueryable GetQueryRootForEntitySet(IEdmEntitySet entitySet)
            {
                var rootSetQuery = this.queryRepository.RootQueries.OfType<QueryRootExpression>().FirstOrDefault(rootQuery => rootQuery.Name == entitySet.Name);
                ExceptionUtilities.CheckObjectNotNull(rootSetQuery, "Could not find root query for set '{0}'", entitySet.Name);

                var rootResults = this.evaluator.Evaluate(rootSetQuery);
                var clrRootQueryable = rootResults.Accept(new QueryValueToClrValueConverter()) as IList;
                return clrRootQueryable.AsQueryable();
            }

            /// <summary>
            /// Invoke the given service operation and returns the results.
            /// </summary>
            /// <param name="serviceOperation">service operation to invoke.</param>
            /// <param name="parameters">value of parameters to pass to the service operation.</param>
            /// <returns>returns the result of the service operation. If the service operation returns void, then this should return null.</returns>
            public object InvokeServiceOperation(IEdmOperationImport serviceOperation, object[] parameters)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}