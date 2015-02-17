//---------------------------------------------------------------------
// <copyright file="IEntityModelPrimitiveTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Resolves all primitive types in EntityModel by applying the specific data type resolver.
    /// </summary>
    [ImplementationSelector("EntityModelPrimitiveTypeResolver", DefaultImplementation = "Default")]
    public interface IEntityModelPrimitiveTypeResolver
    {
        /// <summary>
        /// Resolves the primitive types in a model.
        /// </summary>
        /// <param name="model">The model to resolve types for.</param>
        /// <param name="typeResolver">The type resolver.</param>
        void ResolveProviderTypes(EntityModelSchema model, IPrimitiveDataTypeResolver typeResolver);
    }
}
