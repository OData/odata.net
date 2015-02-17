//---------------------------------------------------------------------
// <copyright file="ITaupoToEdmModelConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib.Contracts
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Converts a model from Taupo term into Edm term
    /// </summary>
    [ImplementationSelector("TaupoToEdmModelConverter", DefaultImplementation = "ProductParser")]
    public interface ITaupoToEdmModelConverter
    {
        /// <summary>
        /// Converts a model from Taupo term into Edm term
        /// </summary>
        /// <param name="taupoModel">The input model in Taupo term</param>
        /// <returns>The output model in Edm term</returns>
        IEdmModel ConvertToEdmModel(EntityModelSchema taupoModel);
    }
}
