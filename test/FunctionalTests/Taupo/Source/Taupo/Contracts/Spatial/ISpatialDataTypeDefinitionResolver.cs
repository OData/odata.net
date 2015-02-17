//---------------------------------------------------------------------
// <copyright file="ISpatialDataTypeDefinitionResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Component for fully resolving a spatial data type's full definition (ie: all methods/properties) based on a more rudimentary definition which is more generic
    /// </summary>
    [ImplementationSelector("SpatialDataTypeDefinitionResolver", DefaultImplementation = "Default")]
    public interface ISpatialDataTypeDefinitionResolver
    {
        /// <summary>
        /// Resolves the full definition of the spatial type, potentially given only the EDM type name
        /// </summary>
        /// <param name="dataType">The basic definition which has at least the EDM type name</param>
        /// <returns>The fully defined spatial type with properties and methods</returns>
        SpatialDataType ResolveFullDefinition(SpatialDataType dataType);
    }
}
