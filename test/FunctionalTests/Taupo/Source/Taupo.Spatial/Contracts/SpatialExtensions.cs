//---------------------------------------------------------------------
// <copyright file="SpatialExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Extension methods specific to spatial components
    /// </summary>
    public static class SpatialExtensions
    {
        /// <summary>
        /// Tries to convert the given spatial instance to the formatted representation, and throws if it cannot
        /// </summary>
        /// <typeparam name="TFormatted">The type used to represent the formatted spatial value</typeparam>
        /// <param name="formatter">The formatter to use</param>
        /// <param name="spatialInstance">The spatial instance to format</param>
        /// <returns>The formatted spatial instance</returns>
        public static TFormatted Convert<TFormatted>(this ISpatialPrimitiveFormatter<TFormatted> formatter, object spatialInstance)
        {
            ExceptionUtilities.CheckArgumentNotNull(formatter, "formatter");
            ExceptionUtilities.CheckArgumentNotNull(spatialInstance, "spatialInstance");

            TFormatted formatted;
            bool converted = formatter.TryConvert(spatialInstance, out formatted);
            ExceptionUtilities.Assert(converted, "Could not convert object: '{0}'.", spatialInstance);
            return formatted;
        }

        /// <summary>
        /// Tries to parse the given formatted representation into a spatial instance, and throws if it cannot
        /// </summary>
        /// <typeparam name="TFormatted">The type used to represent the formatted spatial value</typeparam>
        /// <param name="formatter">The formatter to use</param>
        /// <param name="expectedTypeKind">The expected spatial type kind</param>
        /// <param name="formatted">The formatted representation</param>
        /// <returns>The spatial instance</returns>
        public static object Parse<TFormatted>(this ISpatialPrimitiveFormatter<TFormatted> formatter, SpatialTypeKind? expectedTypeKind, TFormatted formatted)
        {
            ExceptionUtilities.CheckArgumentNotNull(formatter, "formatter");

            object parsedSpatialValue;
            ExceptionUtilities.Assert(
                    formatter.TryParse(formatted, expectedTypeKind, out parsedSpatialValue),
                    "Could not parse formatted spatial data '{0}' with expected type kind '{1}",
                    formatted,
                    expectedTypeKind);

            return parsedSpatialValue;
        }
    }
}
