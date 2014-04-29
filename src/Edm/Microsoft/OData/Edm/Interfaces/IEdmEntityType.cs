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
    /// Represents a definition of an EDM entity type.
    /// </summary>
    public interface IEdmEntityType : IEdmStructuredType, IEdmSchemaType, IEdmTerm
    {
        /// <summary>
        /// Gets the structural properties of the entity type that make up the entity key.
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DeclaredKey { get; }

        /// <summary>
        /// Gets the value indicating whether or not this type is a media entity.
        /// </summary>
        bool HasStream { get; }
    }
}
