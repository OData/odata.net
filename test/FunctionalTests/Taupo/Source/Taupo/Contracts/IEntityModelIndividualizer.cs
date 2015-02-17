//---------------------------------------------------------------------
// <copyright file="IEntityModelIndividualizer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Individualizes storage schema by appending random suffix to every EntitySet and AssociationSet
    /// in the schema.
    /// </summary>
    [ImplementationSelector("SchemaIndividualizer", DefaultImplementation = "Default", IsTransient = true)]
    public interface IEntityModelIndividualizer
    {
        /// <summary>
        /// Individualizes the specified storage schema.
        /// </summary>
        /// <param name="storageSchema">The storage schema.</param>
        void Individualize(EntityModelSchema storageSchema);
    }
}
