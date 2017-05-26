//---------------------------------------------------------------------
// <copyright file="SpatialValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>
    /// Base class for Spatial Type Validator implementations
    /// </summary>
    public static class SpatialValidator
    {
        /// <summary>Creates the currently registered SpatialValidator implementation.</summary>
        /// <returns>The created SpatialValidator.</returns>
        public static SpatialPipeline Create()
        {
            return SpatialImplementation.CurrentImplementation.CreateValidator();
        }
    }
}
