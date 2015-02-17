//---------------------------------------------------------------------
// <copyright file="IEntityModelSchemaComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Compares two EntityModelSchemas
    /// </summary>
    [ImplementationSelector("EntityModelSchemaComparer", DefaultImplementation = "Default")]
    public interface IEntityModelSchemaComparer
    {
        /// <summary>
        /// Compares the two models to each other and throws an exception when there is an erro
        /// </summary>
        /// <param name="expectedTestEntityModel">Expected EntityModelSchema to compare from</param>
        /// <param name="actualEntityModelSchema">Actual EntityModelSchema to compare Expected to</param>
        /// <returns>List of errors encountered</returns>
        ICollection<string> Compare(EntityModelSchema expectedTestEntityModel, EntityModelSchema actualEntityModelSchema);
    }
}
