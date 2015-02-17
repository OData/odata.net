//---------------------------------------------------------------------
// <copyright file="NullPrimitiveDataTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Null data type resolver is just a holder that does not resolve any types.
    /// </summary>
    public class NullPrimitiveDataTypeResolver : IPrimitiveDataTypeResolver
    {
        /// <summary>
        /// Resolves the primitive type specification into store-specific type.
        /// </summary>
        /// <param name="dataTypeSpecification">The data type specification.</param>
        /// <returns>Fully resolved data type.</returns>
        public PrimitiveDataType ResolvePrimitiveType(PrimitiveDataType dataTypeSpecification)
        {
            throw new TaupoNotSupportedException("Do not support resolving primitive types when using null resolver.");
        }
    }
}
