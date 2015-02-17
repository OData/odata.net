//---------------------------------------------------------------------
// <copyright file="IEntityToDatabaseModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Converts storage EntityModelSchema into DatabaseSchema
    /// </summary>
    [ImplementationSelector("EntityToDatabaseModelConverter", DefaultImplementation = "Default")]
    public interface IEntityToDatabaseModelConverter
    {
        /// <summary>
        /// Converts the specified storage model schema into a database schema.
        /// </summary>
        /// <param name="storageModel">The storage model.</param>
        /// <returns>Instance of <see cref="DatabaseSchema"/> with <see cref="Table"/> initialized from <see cref="EntitySet"/> objects in the storage model.</returns>
        DatabaseSchema Convert(EntityModelSchema storageModel);
    }
}
