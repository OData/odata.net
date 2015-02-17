//---------------------------------------------------------------------
// <copyright file="SpatialUtilities.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.Contracts
{
    using System;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Utility methods specific to spatial components
    /// </summary>
    public static class SpatialUtilities
    {
        /// <summary>
        /// Tries to infer the spatial type kind for the given type.
        /// </summary>
        /// <param name="clrType">The type to check</param>
        /// <param name="spatialTypeKind">The spatial type kind for the type</param>
        /// <returns>true if its a spatial type, otherwise false</returns>
        public static bool TryInferSpatialTypeKind(Type clrType, out SpatialTypeKind? spatialTypeKind)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");

            if (typeof(Geography).IsAssignableFrom(clrType))
            {
                spatialTypeKind = SpatialTypeKind.Geography;
                return true;
            }
            else if (typeof(Geometry).IsAssignableFrom(clrType))
            {
                spatialTypeKind = SpatialTypeKind.Geometry;
                return true;
            }

            spatialTypeKind = null;
            return false;
        }

        /// <summary>
        /// Infers the spatial type kind for the given type. Will throw if the type is not spatial.
        /// </summary>
        /// <param name="clrType">The type to check</param>
        /// <returns>The spatial type kind for the type</returns>
        public static SpatialTypeKind InferSpatialTypeKind(Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");

            SpatialTypeKind? kind;
            ExceptionUtilities.Assert(TryInferSpatialTypeKind(clrType, out kind), "Type must be either geographic or geometric. Type was: '{0}'", clrType);
            return kind.Value;
        }

        /// <summary>
        /// Infers the spatial type kind for the given clr type name. Will throw if the type is not one of the spatial types.
        /// </summary>
        /// <param name="clrTypeName">The clr type name to check</param>
        /// <returns>The spatial type kind for the type name</returns>
        public static SpatialTypeKind InferSpatialTypeKind(string clrTypeName)
        {
            var type = typeof(ISpatial).GetAssembly().GetTypes().SingleOrDefault(t => t.IsPublic() && t.Name == clrTypeName);
            ExceptionUtilities.CheckObjectNotNull(type, "Could not find spatial type with name '{0}'", clrTypeName);
            return InferSpatialTypeKind(type);
        }
    }
}
