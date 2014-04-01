//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

#if SPATIAL
namespace Microsoft.Data.Spatial
#else
namespace Microsoft.OData.Core.Json
#endif
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Enumeration of all JSON node type.
    /// </summary>
#if ODATALIB_PUBLICJSONREADER
    public
#else
    internal
#endif
    enum JsonNodeType
    {
        /// <summary>
        /// No node - invalid value.
        /// </summary>
        None,

        /// <summary>
        /// Start of JSON object record, the { character.
        /// </summary>
        StartObject,

        /// <summary>
        /// End of JSON object record, the } character.
        /// </summary>
        EndObject,

        /// <summary>
        /// Start of JSON array, the [ character.
        /// </summary>
        StartArray,

        /// <summary>
        /// End of JSON array, the ] character.
        /// </summary>
        EndArray,

        /// <summary>
        /// Property, the name of the property (the value will be reported as a separate node or nodes)
        /// </summary>
        Property,

        /// <summary>
        /// Primitive value, that is either null, true, false, number or string.
        /// </summary>
        PrimitiveValue,

        /// <summary>
        /// End of input reached.
        /// </summary>
        EndOfInput
    }
}
