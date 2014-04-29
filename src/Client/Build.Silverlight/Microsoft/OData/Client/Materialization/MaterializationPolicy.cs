//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Client.Materialization
{
    using System;
    using Microsoft.OData.Client;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class responsible for materializing from OData to Objects
    /// </summary>
    internal abstract class MaterializationPolicy
    {
        /// <summary>
        /// Creates the specified edm type.
        /// </summary>
        /// <param name="edmTypeReference">Type of the edm.</param>
        /// <param name="type">The type.</param>
        /// <remarks>In the future this class will have Materialize and Update will be adding this in upcoming changes</remarks>
        /// <returns>A created object</returns>
        public virtual object CreateNewInstance(IEdmTypeReference edmTypeReference, Type type)
        {
            return Util.ActivatorCreateInstance(type);
        }
    }
}
