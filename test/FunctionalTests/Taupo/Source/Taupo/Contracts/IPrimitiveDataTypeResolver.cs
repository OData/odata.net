//---------------------------------------------------------------------
// <copyright file="IPrimitiveDataTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Primitive data type resolver. Converts type specifications into fully resolved Edm/store type information.
    /// </summary>
    [ImplementationSelector("EdmDataTypeResolver", DefaultImplementation = "Default")]
    public interface IPrimitiveDataTypeResolver
    {
        /// <summary>
        /// Resolves the primitive type specification into Edm or store-specific type.
        /// </summary>
        /// <param name="dataTypeSpecification">The data type specification.</param>
        /// <returns>Fully resolved data type.</returns>
        /// <remarks>
        /// The method handles 3 kinds of type specifications:
        /// <list>
        /// <item>Using only facets, for example DataTypes.Integer.WithSize(32). In this case the method returns a usable type that matches the specified facet combination.</item>
        /// <item>Using unqualified store type name and facets - for example DataTypes.Integer.WithUnqualifiedStoreTypeName("bigint")</item>
        /// <item>Using qualified store type name - for example - SqlServerDataTypes.Int - that indicates that type has already been resolved and this function should be a no-op.</item>
        /// </list>
        /// </remarks>
        PrimitiveDataType ResolvePrimitiveType(PrimitiveDataType dataTypeSpecification);
    }
}
