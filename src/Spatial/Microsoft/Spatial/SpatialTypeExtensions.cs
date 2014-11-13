//   OData .NET Libraries ver. 6.8.1
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
