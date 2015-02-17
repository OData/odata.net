//---------------------------------------------------------------------
// <copyright file="ISpatialPrimitiveToODataJsonValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Astoria.Contracts.Json;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract to convert a spatial primitive to and from an OData-specific json value
    /// </summary>
    [ImplementationSelector("SpatialPrimitiveToODataJsonValueConverter", DefaultImplementation = "Default")]
    public interface ISpatialPrimitiveToODataJsonValueConverter
    {
        /// <summary>
        /// Tries to convert the value if it is spatial. Adds OData-specific fields to GeoJSON microformat.
        /// </summary>
        /// <param name="value">The value that might be spatial.</param>
        /// <param name="jsonObject">The converted json object.</param>
        /// <returns>Whether the value was spatial and could be converted</returns>
        bool TryConvertIfSpatial(PrimitiveValue value, out JsonObject jsonObject);

        /// <summary>
        /// Tries to convert the value if it is spatial. Preserves any OData-specific fields.
        /// </summary>
        /// <param name="jsonObject">The json object that might be spatial.</param>
        /// <param name="value">The converted spatial value.</param>
        /// <returns>Whether the object was spatial and could be converted</returns>
        bool TryConvertIfSpatial(JsonObject jsonObject, out PrimitiveValue value);
    }
}