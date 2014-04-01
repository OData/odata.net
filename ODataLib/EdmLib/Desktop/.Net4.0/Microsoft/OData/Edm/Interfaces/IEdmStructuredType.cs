//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Common base interface for definitions of EDM structured types.
    /// </summary>
    public interface IEdmStructuredType : IEdmType
    {
        /// <summary>
        /// Gets a value indicating whether this type is abstract.
        /// </summary>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether this type is open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the base type of this type.
        /// </summary>
        IEdmStructuredType BaseType { get; }

        /// <summary>
        /// Gets the properties declared immediately within this type.
        /// </summary>
        IEnumerable<IEdmProperty> DeclaredProperties { get; }

        /// <summary>
        /// Searches for a structural or navigation property with the given name in this type and all base types and returns null if no such property exists.
        /// </summary>
        /// <param name="name">The name of the property being found.</param>
        /// <returns>The requested property, or null if no such property exists.</returns>
        IEdmProperty FindProperty(string name);
    }
}
