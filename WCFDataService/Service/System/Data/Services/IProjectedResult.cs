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

namespace System.Data.Services
{
    /// <summary>
    /// This interface declares the methods required to support getting values
    /// from projected results for $select queries
    /// </summary>
    internal interface IProjectedResult
    {
        /// <summary>The full name of the <see cref="System.Data.Services.Providers.ResourceType"/> which represents the type
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
