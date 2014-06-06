//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    /// <summary>
    /// This interface declares the methods required to support getting values
    /// from projected results for $select queries
    /// </summary>
    internal interface IProjectedResult
    {
        /// <summary>The full name of the <see cref="Microsoft.OData.Service.Providers.ResourceType"/> which represents the type
        /// of this result.</summary>
        string ResourceTypeName 
        { 
            get; 
        }

        /// <summary>Gets the value for named property for the result.</summary>
        /// <param name="propertyName">Name of property for which to get the value.</param>
        /// <returns>The value for the named property of the result.</returns>
        object GetProjectedPropertyValue(string propertyName);
    }
}
