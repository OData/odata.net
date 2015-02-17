//---------------------------------------------------------------------
// <copyright file="IReferentialConstraintsResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel.Data
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Resolves referential constraints in the entity container data.
    /// </summary>
    [ImplementationSelector("ReferentialConstraintsResolver", DefaultImplementation = "Default")]
    public interface IReferentialConstraintsResolver
    {
        /// <summary>
        /// Resolves referential constraints in the entity container data.
        /// </summary>
        /// <param name="data">Entity container data where to resolve referential constraints.</param>
        void ResolveReferentialConstraints(EntityContainerData data);
    }
}
