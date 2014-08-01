//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents a definition of an EDM type definition with additional facets.
    /// </summary>
    public interface IEdmTypeDefinitionWithFacets : IEdmTypeDefinition
    {
        /// <summary>
        /// Gets the maximum length of the underlying type.
        /// </summary>
        int? MaxLength { get; }

        /// <summary>
        /// Gets a value indicating whether the underlying string type supports unicode encoding.
        /// </summary>
        bool? IsUnicode { get; }

        /// <summary>
        /// Gets the precision of the underlying type.
        /// </summary>
        int? Precision { get; }

        /// <summary>
        /// Gets the scale of the underlying decimal type.
        /// </summary>
        int? Scale { get; }

        /// <summary>
        /// Gets the Spatial Reference Identifier of the underlying spatial type.
        /// </summary>
        int? Srid { get; }
    }
}
