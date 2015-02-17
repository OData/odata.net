//---------------------------------------------------------------------
// <copyright file="IPrimitiveDataTypeToDataGenerationHintsResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Resolves data generation hints for primitive data types.
    /// </summary>
    [ImplementationSelector("PrimitiveDataTypeToDataGenerationHintsResolver", DefaultImplementation = "Default")]
    public interface IPrimitiveDataTypeToDataGenerationHintsResolver
    {
        /// <summary>
        /// Resolves hints for data generation for the given primitive type.
        /// </summary>
        /// <param name="dataType">Primitive data type to resolve data generator for.</param>
        /// <returns>Data generation hints.</returns>
        IEnumerable<DataGenerationHint> ResolveDataGenerationHints(PrimitiveDataType dataType);
    }
}
