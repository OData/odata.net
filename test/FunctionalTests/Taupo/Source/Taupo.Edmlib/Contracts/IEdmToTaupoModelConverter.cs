//---------------------------------------------------------------------
// <copyright file="IEdmToTaupoModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib.Contracts
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Converts a model from Edm term into Taupo term
    /// </summary>
    [ImplementationSelector("EdmToTaupoModelConverter", DefaultImplementation = "Default")]
    public interface IEdmToTaupoModelConverter
    {
        /// <summary>
        /// Converts a model from Edm term into Taupo term
        /// </summary>
        /// <param name="edmModel">The input model in Edm term</param>
        /// <returns>The output model in Taupo term</returns>
        EntityModelSchema ConvertToTaupoModel(IEdmModel edmModel);
    }
}
