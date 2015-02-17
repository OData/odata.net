//---------------------------------------------------------------------
// <copyright file="IQueryDataSetBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Builds <see cref="IQueryDataSet"/> from <see cref="EntityContainerData"/>.
    /// </summary>
    public interface IQueryDataSetBuilder
    {
        /// <summary>
        /// Builds the <see cref="QueryDataSet"/> for the specified query queryRepository.
        /// </summary>
        /// <param name="rootDataTypeMap">The collection of rootDataTypes used to build the data set.</param>
        /// <param name="entityContainerData">EntityContainerData that contains the initial set of data to build the QueryDataSet over</param>
        /// <returns>
        /// Instance of <see cref="QueryDataSet"/> with populated data.
        /// </returns>
        IQueryDataSet Build(IDictionary<string, QueryStructuralType> rootDataTypeMap, EntityContainerData entityContainerData);
    }
}