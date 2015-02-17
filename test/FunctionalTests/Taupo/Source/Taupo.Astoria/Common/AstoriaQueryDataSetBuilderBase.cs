//---------------------------------------------------------------------
// <copyright file="AstoriaQueryDataSetBuilderBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Common
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    using Microsoft.Test.Taupo.Query;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Builds <see cref="QueryDataSet"/> based on <see cref="Workspace"/>.
    /// </summary>
    public abstract class AstoriaQueryDataSetBuilderBase : QueryDataSetBuilderBase
    {
        /// <summary>
        /// Initializes a new instance of the AstoriaQueryDataSetBuilderBase class.
        /// </summary>
        protected AstoriaQueryDataSetBuilderBase()
        {
            this.BuildQueryDataSetFunc = base.Build;
        }

        /// <summary>
        /// Gets or sets the method to use for building the query data set. Default is to use QueryDataSetBuilderBase.Build(), but this is used for testability.
        /// </summary>
        internal Func<IDictionary<string, QueryStructuralType>, EntityContainerData, IQueryDataSet> BuildQueryDataSetFunc { get; set; }

        /// <summary>
        /// Builds the <see cref="QueryDataSet"/> from the specified container data and query queryRepository.
        /// </summary>
        /// <param name="rootDataTypeMap">The collection of rootDataTypes used to build the data set.</param>
        /// <param name="entityContainerData">Entity Container Data that contains the information to be used in the QueryDataSet</param>
        /// <returns>
        /// Instance of <see cref="QueryDataSet"/> with data populated from the containerData
        /// </returns>
        public override IQueryDataSet Build(IDictionary<string, QueryStructuralType> rootDataTypeMap, EntityContainerData entityContainerData)
        {
            return this.BuildQueryDataSetFunc(rootDataTypeMap, entityContainerData);
        }

        /// <summary>
        /// Initializes the given query type by creating a queryStreamValue to hold the expected values
        /// </summary>
        /// <param name="queryType">A QueryEntityType</param>
        /// <returns>a QueryStructurvalValue</returns>
        protected override QueryStructuralValue InitializeEntityValue(QueryEntityType queryType)
        {
            var entity = base.InitializeEntityValue(queryType);
            foreach (var namedStream in queryType.Properties.Streams())
            {
                AstoriaQueryStreamValue qsv = new AstoriaQueryStreamValue((AstoriaQueryStreamType)namedStream.PropertyType, (byte[])null, null, queryType.EvaluationStrategy);
                entity.SetStreamValue(namedStream.Name, qsv);
            }

            return entity;
        }

        /// <summary>
        /// Initializes query collection values, setting default IsSorted value to true. Therefore we enable ordering verfication by default.
        /// </summary>
        /// <param name="entitySetData">entity set data</param>
        /// <returns>initial query collection value</returns>
        protected override QueryCollectionValue BuildStubEntities(EntitySetData entitySetData)
        {
            var collection = base.BuildStubEntities(entitySetData);
            return QueryCollectionValue.Create(collection.Type.ElementType, collection.Elements, true);
        }
    }
}