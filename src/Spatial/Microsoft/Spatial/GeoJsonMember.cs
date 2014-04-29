//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    /// <summary>
    /// Defines the members that may be found in a GeoJSON object.
    /// </summary>
    internal enum GeoJsonMember
    {
        /// <summary>
        /// "type" member in a GeoJSON object.
        /// </summary>
        Type,

        /// <summary>
        /// "coordinates" member in GeoJSON object.
        /// </summary>
        Coordinates,

        /// <summary>
        /// "geometries" member in GeoJSON object.
        /// </summary>
        Geometries,

        /// <summary>
        /// "crs" member in GeoJSON object.
        /// </summary>
        Crs,

        /// <summary>
        /// 'properties' member in GeoJSON object
        /// </summary>
        Properties,

        /// <summary>
        /// 'name' member in GeoJSON object
        /// </summary>
        Name,
    }
}
