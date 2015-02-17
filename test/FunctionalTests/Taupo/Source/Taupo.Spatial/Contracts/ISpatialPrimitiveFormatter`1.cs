//---------------------------------------------------------------------
// <copyright file="ISpatialPrimitiveFormatter`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    /// <summary>
    /// Base interface for converting a spatial primitive value to/from a format
    /// </summary>
    /// <typeparam name="TFormatted">The type used to represent the formatted spatial value</typeparam>
    public interface ISpatialPrimitiveFormatter<TFormatted>
    {
        /// <summary>
        /// Tries to convert the given spatial instance to the formatted representation
        /// </summary>
        /// <param name="spatialInstance">The spatial instance to format</param>
        /// <param name="formatted">The formatted spatial value</param>
        /// <returns>True if the value was spatial and could be formatted, otherwise false</returns>
        bool TryConvert(object spatialInstance, out TFormatted formatted);

        /// <summary>
        /// Tries to parse the given formatted spatial representation.
        /// </summary>
        /// <param name="formatted">The formatted representation</param>
        /// <param name="expectedSpatialType">The expected spatial type or null if it is unknown</param>
        /// <param name="spatialInstance">The parsed spatial instance</param>
        /// <returns>True if the value could be parsed, false otherwise</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "using real type forces callers to have extra assembly reference")]
        bool TryParse(TFormatted formatted, SpatialTypeKind? expectedSpatialType, out object spatialInstance);
    }
}
