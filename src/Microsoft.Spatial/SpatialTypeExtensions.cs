//---------------------------------------------------------------------
// <copyright file="SpatialTypeExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    /// <summary>Provides a place to add extension methods that work with ISpatial.</summary>
    public static class SpatialTypeExtensions
    {
        /// <summary> Allows the delegation of the call to the proper type (geography or Geometry).</summary>
        /// <param name="shape">The instance that will have SendTo called.</param>
        /// <param name="destination">The pipeline that the instance will be sent to.</param>
        public static void SendTo(this ISpatial shape, SpatialPipeline destination)
        {
            if (shape == null)
            {
                return;
            }

            if (shape.GetType().IsSubclassOf(typeof(Geometry)))
            {
                ((Geometry)shape).SendTo(destination);
            }
            else
            {
                ((Geography)shape).SendTo(destination);
            }
        }
    }
}
