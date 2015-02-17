//---------------------------------------------------------------------
// <copyright file="QueryRepositoryBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;

    /// <summary>
    /// Builds query repository.
    /// </summary>
    public abstract class QueryRepositoryBuilderBase : IQueryRepositoryBuilder
    {
        /// <summary>
        /// Initializes a new instance of the QueryRepositoryBuilderBase class.
        /// </summary>
        protected QueryRepositoryBuilderBase()
        {
            this.Logger = Logger.Null;
        }

        /// <summary>
        /// Gets or sets random number generator to be used by tests.
        /// </summary>
        [InjectDependency]
        public IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        [InjectDependency]
        public Logger Logger { get; set; }

        /// <summary>
        /// Gets or sets the DataSetBuilder.
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IQueryDataSetBuilder DataSetBuilder { get; set; }

        /// <summary>
        /// Gets the root data types.
        /// </summary>
        protected IDictionary<string, QueryStructuralType> RootDataTypes { get; private set; }

        /// <summary>
        /// Gets or sets the collection of scalar types which will be set on the constructed repository
        /// </summary>
        protected IEnumerable<QueryScalarType> PrimitiveTypes { get; set; }

        /// <summary>
        /// Gets or sets the collection of constants which will be set on the constructed repository
        /// </summary>
        protected IEnumerable<QueryConstantExpression> Constants { get; set; }

        /// <summary>
        /// Gets or sets the collection of the root queries which will be set on the constructed repository
        /// </summary>
        protected IEnumerable<QueryExpression> RootQueries { get; set; }

        /// <summary>
        /// Factory method that creates a new instance of the QueryRepository.
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        /// <param name="entityContainerData">Entity Container Data</param>
        /// <returns>An instance of the QueryRepository class.</returns>
        public virtual QueryRepository CreateQueryRepository(EntityModelSchema entityModelSchema, QueryTypeLibrary queryTypeLibrary, EntityContainerData entityContainerData)
        {
            this.RootDataTypes = new Dictionary<string, QueryStructuralType>();

            this.BuildPrimitiveTypes(queryTypeLibrary);
            this.BuildRootQueriesAndTypes(entityModelSchema, queryTypeLibrary);

            var dataSet = this.DataSetBuilder.Build(this.RootDataTypes, entityContainerData);

            this.BuildConstants(queryTypeLibrary, dataSet);

            QueryRepository repository = new QueryRepository(queryTypeLibrary, this.RootQueries, this.Constants, this.PrimitiveTypes, this.RootDataTypes, dataSet);

            return repository;
        }

        /// <summary>
        /// Build the collection of primitive types which will be set on the constructed repository
        /// </summary>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        protected abstract void BuildPrimitiveTypes(QueryTypeLibrary queryTypeLibrary);

        /// <summary>
        /// Build the collection of constants which will be set on the constructed repository
        /// </summary>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        /// <param name="dataSet">The data from which the constants would be selected.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "TODO: needs review - warning was introduced when moving build systems.")]
        protected virtual void BuildConstants(QueryTypeLibrary queryTypeLibrary, IQueryDataSet dataSet)
        {
            ExceptionUtilities.CheckObjectNotNull(this.RootQueries, "Build root queries first before building constants");

            List<QueryConstantExpression> queryConstants = new List<QueryConstantExpression>();
            var rootQueries = this.RootQueries.OfType<QueryRootExpression>();

            int constantsOfSameTypeLimit = 5;
            foreach (var rootQuery in rootQueries)
            {
                var entityElements = dataSet[rootQuery.Name].Elements;

                if (entityElements.Count() == 0)
                {
                    continue;
                }

                var dataRow = this.Random.ChooseFrom(entityElements.Cast<QueryStructuralValue>());
                var scalarDataValues = dataRow.Type.Properties.Where(p => p.PropertyType is QueryScalarType).Select(st => dataRow.GetScalarValue(st.Name)).ToList();
                var complexDataValues = dataRow.Type.Properties.Where(p => p.PropertyType is QueryComplexType).Select(ct => dataRow.GetStructuralValue(ct.Name)).ToList();

                while (complexDataValues.Count() > 0)
                {
                    var complexDataValue = complexDataValues.First();

                    foreach (var propertyName in complexDataValue.MemberNames)
                    {
                        var complexPropertyValue = complexDataValue.GetValue(propertyName);
                        var nestedComplexValue = complexPropertyValue as QueryStructuralValue;
                        var nestedScalarValue = complexPropertyValue as QueryScalarValue;

                        if (nestedComplexValue != null)
                        {
                            complexDataValues.Add(nestedComplexValue);
                        }
                        else if (nestedScalarValue != null)
                        {
                            scalarDataValues.Add(nestedScalarValue);
                        }
                    }

                    complexDataValues.Remove(complexDataValue);
                }

                this.AddDefaultConstants(scalarDataValues, queryTypeLibrary);

                foreach (var scalarTypeValue in scalarDataValues)
                {
                    if (queryConstants.Count(qce => qce.ExpressionType.IsSameQueryScalarType(scalarTypeValue.Type as QueryScalarType)) <= constantsOfSameTypeLimit)
                    {
                        queryConstants.Add(CommonQueryBuilder.Constant(scalarTypeValue));
                    }
                }
            }

            var nonMappedEnumTypes = queryTypeLibrary.GetNonMappedEnumTypes();

            foreach (var nonMappedEnum in nonMappedEnumTypes)
            {
                var enumMember = this.Random.ChooseFrom(nonMappedEnum.EnumType.Members);
                var enumObjectToAdd = Enum.Parse(nonMappedEnum.ClrType, enumMember.Name, false);

                queryConstants.Add(CommonQueryBuilder.Constant(enumObjectToAdd));
            }

            this.Constants = queryConstants.AsEnumerable();
        }

        /// <summary>
        /// Returns a value indicating whether we should add a RootQuery for this entity set
        /// </summary>
        /// <param name="entitySet">The entity set to add a root query for</param>
        /// <returns>A value indicating whether we should add a RootQuery for this entity set</returns>
        protected virtual bool ShouldCreateRootQuery(EntitySet entitySet)
        {
            return true;
        }

        /// <summary>
        /// Builds root queries
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        protected virtual void BuildRootQueriesAndTypes(EntityModelSchema entityModelSchema, QueryTypeLibrary queryTypeLibrary)
        {
            var rootQueriesList = new List<QueryExpression>();

            foreach (var container in entityModelSchema.EntityContainers)
            {
                foreach (var entitySet in container.EntitySets)
                {
                    var entityType = queryTypeLibrary.GetQueryEntityTypeForEntitySet(entitySet);
                    this.RootDataTypes.Add(entitySet.Name, entityType);
                    if (this.ShouldCreateRootQuery(entitySet))
                    {
                        QueryRootExpression rootExpression = new QueryRootExpression(entitySet.Name, entityType.CreateCollectionType());
                        rootQueriesList.Add(rootExpression);
                    }
                }
            }

            this.RootQueries = rootQueriesList;
        }

        private void AddDefaultConstants(List<QueryScalarValue> scalarValues, QueryTypeLibrary queryTypeLibrary)
        {
            var intType = (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Int32);
            var stringType = (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.String());
            var boolType = (QueryScalarType)queryTypeLibrary.GetDefaultQueryType(EdmDataTypes.Boolean);

            scalarValues.Add(intType.CreateValue(1));
            scalarValues.Add(stringType.CreateValue("Foo"));
            scalarValues.Add(boolType.CreateValue(true));
            scalarValues.Add(boolType.CreateValue(false));
        }
    }
}
