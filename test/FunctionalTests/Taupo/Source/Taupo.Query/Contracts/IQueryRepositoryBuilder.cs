//---------------------------------------------------------------------
// <copyright file="IQueryRepositoryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;

    /// <summary>
    /// Factory for constructing instances of the <see cref="QueryRepository"/> class.
    /// </summary>
    public interface IQueryRepositoryBuilder
    {
        /// <summary>
        /// Factory method that creates a new instance of the QueryRepository.
        /// </summary>
        /// <param name="entityModelSchema">Entity Model Schema</param>
        /// <param name="queryTypeLibrary">Query Type Library</param>
        /// <param name="entityContainerData">Entity Container Data</param>
        /// <returns>An instance of the QueryRepository class.</returns>
        QueryRepository CreateQueryRepository(EntityModelSchema entityModelSchema, QueryTypeLibrary queryTypeLibrary, EntityContainerData entityContainerData);
    }
}
