//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
