//---------------------------------------------------------------------
// <copyright file="SpatialClrTypeResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Spatial.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Spatial;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Spatial;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Class to resolve spatial clr type.
    /// </summary>
    public class SpatialClrTypeResolver : ISpatialClrTypeResolver
    {
        private static readonly Dictionary<string, Type> spatialClrTypes = new Dictionary<string, Type>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Needs to do reflection to initialize mapping")]
        static SpatialClrTypeResolver()
        {
            foreach (var spatialType in typeof(ISpatial).GetAssembly().GetTypes().Where(t => typeof(ISpatial).IsAssignableFrom(t)))
            {
                string typeName = spatialType.Name;

                typeName = typeName.ToUpperInvariant();

                spatialClrTypes[typeName] = spatialType;
            }
        }

        /// <summary>
        /// Finds and returns the clr type for given spatial type
        /// </summary>
        /// <returns>the clr type for given spatial type</returns>
        /// <param name="spatialType">the spatial type to get clr type for</param>
        public Type GetClrType(SpatialDataType spatialType)
        {
            ExceptionUtilities.CheckArgumentNotNull(spatialType, "spatialType");

            string dataTypeName = spatialType.GetFacetValue<EdmTypeNameFacet, string>(string.Empty).ToUpperInvariant();
            Type clrType;
            ExceptionUtilities.Assert(
                spatialClrTypes.TryGetValue(dataTypeName, out clrType),
                "the data type {0} is not supported as spatial data type.",
                dataTypeName);

            return clrType;
        }

        /// <summary>
        /// Returns true if the specified clr type is spatial, false otherwise.
        /// </summary>
        /// <param name="clrType">The clr type to check.</param>
        /// <returns>Value indicating whether the specified clr type is spatial.</returns>
        public bool IsSpatial(Type clrType)
        {
            ExceptionUtilities.CheckArgumentNotNull(clrType, "clrType");
            return typeof(ISpatial).IsAssignableFrom(clrType);
        }
    }
}
