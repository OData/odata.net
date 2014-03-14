//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Spatial
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
